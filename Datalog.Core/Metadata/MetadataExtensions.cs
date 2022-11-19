using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using HondataDotNet.Datalog.Core.Units;

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

        public static bool TryGetUnitInfo(this Enum unit, out UnitInfo? unitInfo, out QuantityInfo? quantityInfo)
        {
            (unitInfo, quantityInfo) = (
                from q in Quantity.Infos
                from u in q.UnitInfos
                where u.Value.Equals(unit)
                select (u, q))
                .FirstOrDefault();

            if (unitInfo is not null && quantityInfo is not null)
                return true;

            var quantityTypeAttribute = unit.GetType().GetCustomAttribute<QuantityTypeAttribute>();
            if (quantityTypeAttribute is null || !typeof(IQuantity).IsAssignableFrom(quantityTypeAttribute.Type))
                return false;

            var infoProperty = quantityTypeAttribute.Type.GetProperty(quantityTypeAttribute.InfoProperty ?? nameof(IQuantity.QuantityInfo));
            if (infoProperty is null || !typeof(QuantityInfo).IsAssignableFrom(infoProperty.PropertyType))
                return false;

            var infoGetter = infoProperty.GetGetMethod();
            var instance = infoGetter.IsStatic ? null : quantityTypeAttribute.Type.GetConstructor(Type.EmptyTypes)?.Invoke(null) as IQuantity;

            quantityInfo = infoGetter.Invoke(instance, null) as QuantityInfo;
            if (quantityInfo is null)
                return false;

            unitInfo = quantityInfo.UnitInfos.SingleOrDefault(i => i.Value.Equals(unit));
            if (unitInfo is null)
                return false;

            return true;
        }
    }
}