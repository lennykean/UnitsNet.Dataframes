using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public static class MetadataExtensions
    {
        public static IQuantity GetQuantity<T>(this T obj, PropertyInfo property, Func<T, double> getter)
        {
            var metadata = ObjectMetadata<T>.GetQuantityMetadata(property);
            if (metadata?.UnitInfo == null || metadata.QuantityInfo == null)
                throw new InvalidOperationException($"Unit metadata does not exist for property {property.DeclaringType}.{property.Name}.");

            if (Quantity.TryFrom(getter(obj), metadata.UnitInfo.Value, out var quantity))
                return quantity!;

            var constructor = (
                from c in metadata.QuantityInfo.ValueType.GetConstructors()
                let parameters = c.GetParameters()
                where parameters.Count() == 2
                where parameters.First().ParameterType == typeof(double)
                where parameters.Last().ParameterType == metadata.QuantityInfo.UnitType
                select c).SingleOrDefault();

            if (constructor == null)
                throw new InvalidOperationException($"Unable to create quantity. No constructor found compatible with {metadata.QuantityInfo.ValueType}({typeof(double)}, {metadata.QuantityInfo.UnitType})");

            return (IQuantity)constructor.Invoke(new object[] { getter(obj), metadata.UnitInfo.Value });
        }

        public static IQuantity GetQuantity<T>(this T obj, Expression<Func<T, double>> propertySelectorExpression)
        {
            var expression = propertySelectorExpression.Body;

            while (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo property)
                throw new InvalidOperationException($"{{{propertySelectorExpression}}} is not a valid property accessor.");

            return obj.GetQuantity(property, propertySelectorExpression.Compile());
        }

        public static IQuantity GetQuantity<T>(this T obj, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);

            return obj.GetQuantity(property, o => Convert.ToDouble(property.GetValue(o)));
        }

        public static bool TryGetUnitInfo(this Enum unit, Type? quantityType, out UnitInfo? unitInfo)
        {
            // Check for a built-in unit type
            unitInfo = (
                from q in Quantity.Infos
                from u in q.UnitInfos
                where u.Value.Equals(unit)
                select u).SingleOrDefault();
            if (unitInfo is not null)
                return true;

            // Check if quantityType can be used to get a matching quantityInfo, and try to get a matching unitInfo from it. 
            if (unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                unitInfo = quantityInfo?.UnitInfos.SingleOrDefault(i => i.Value.Equals(unit));
            if (unitInfo is not null)
                return true;

            return true;
        }

        public static bool TryGetQuantityInfo(this Enum unit, Type? quantityType, out QuantityInfo? quantityInfo)
        {
            // Check for a built-in quantity type
            quantityInfo = (
                from q in Quantity.Infos
                where q.UnitInfos.Any(u => u.Value.Equals(unit))
                select q).SingleOrDefault();
            if (quantityInfo is not null)
                return true;

            // Check for a static QuantityInfo property on quantityType and try to invoke the getter
            var staticInfoProperty = quantityType?.GetProperties(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(p => p.PropertyType == typeof(QuantityInfo));
            var staticInfoGetter = staticInfoProperty?.GetGetMethod();
            quantityInfo = staticInfoGetter?.Invoke(null, null) as QuantityInfo;
            if (quantityInfo?.UnitType == unit.GetType())
                return true;

            // Check for a default public constructor, try to construct an instance of quantityType, and use the QuantityInfo instance property
            var instance = quantityType?.GetConstructor(Type.EmptyTypes)?.Invoke(null) as IQuantity;
            quantityInfo = instance?.QuantityInfo;
            if (quantityInfo?.UnitType == unit.GetType())
                return true;

            return false;
        }
    }
}