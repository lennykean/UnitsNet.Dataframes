using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public static class MetadataExtensions
    {
        private static readonly Lazy<Type[]> LazyQuantityValueCompatibleTypes = new(() => (
            from m in typeof(QuantityValue).GetMethods(BindingFlags.Public | BindingFlags.Static)
            where m.Name == "op_Implicit"
            select m.GetParameters().First().ParameterType).ToArray());

        public static ObjectMetadata<T> GetObjectMetadata<T>(this T _)
        {
            return new ObjectMetadata<T>();
        }

        public static ObjectMetadata<T> GetObjectMetadata<T>(this IEnumerable<T> _)
        {
            return new ObjectMetadata<T>();
        }

        public static IQuantity GetQuantity<T>(this T obj, Expression<Func<T, QuantityValue>> propertySelectorExpression)
        {
            var expression = propertySelectorExpression.Body;

            // Unwrap any casts in the expression tree
            while (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            // Ensure the expression is a property accessor and get the PropertyInfo
            if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo property || property.GetGetMethod()?.IsPublic != true)
                throw new InvalidOperationException($"{{{propertySelectorExpression}}} is not a valid property accessor.");

            return GetQuantity(obj, property);
        }

        public static IQuantity GetQuantity<T>(this T obj, string propertyName)
        {
            var property = typeof(T).GetProperty(propertyName);

            return GetQuantity(obj, property);
        }

        public static bool TryGetUnitInfo(this Enum unit, Type? quantityType, out UnitInfo? unitInfo)
        {
            // Check cache
            if (SimpleCache<Enum, UnitInfo>.Instance.TryGet(unit, out unitInfo))
            {
                return true;
            }

            // Check for a built-in unit type
            unitInfo = (
                from q in Quantity.Infos
                from u in q.UnitInfos
                where u.Value.Equals(unit)
                select u).SingleOrDefault();
            if (unitInfo is not null)
            {
                SimpleCache<Enum, UnitInfo>.Instance.TryAdd(unit, unitInfo);
                return true;
            }

            // Check if quantityType can be used to get a matching quantityInfo, and try to get a matching unitInfo from it. 
            if (unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                unitInfo = quantityInfo?.UnitInfos.SingleOrDefault(i => i.Value.Equals(unit));
            if (unitInfo is not null)
            {
                SimpleCache<Enum, UnitInfo>.Instance.TryAdd(unit, unitInfo);
                return true;
            }

            return false;
        }

        public static bool TryGetQuantityInfo(this Enum unit, Type? quantityType, out QuantityInfo? quantityInfo)
        {
            // Check cache
            if (SimpleCache<Enum, QuantityInfo>.Instance.TryGet(unit, out quantityInfo))
            {
                return true;
            }

            // Check for a built-in quantity type
            quantityInfo = (
                from q in Quantity.Infos
                where q.UnitInfos.Any(u => u.Value.Equals(unit))
                select q).SingleOrDefault();
            if (quantityInfo is not null)
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            // Check for a static QuantityInfo property on quantityType and try to invoke the getter
            var staticInfoProperty = quantityType?.GetProperties(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(p => p.PropertyType == typeof(QuantityInfo));
            var staticInfoGetter = staticInfoProperty?.GetGetMethod();
            quantityInfo = staticInfoGetter?.Invoke(null, null) as QuantityInfo;
            if (quantityInfo?.UnitType == unit.GetType())
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            // Check for a default public constructor, try to construct an instance of quantityType, and use the QuantityInfo instance property
            var instance = quantityType?.GetConstructor(Type.EmptyTypes)?.Invoke(null) as IQuantity;
            quantityInfo = instance?.QuantityInfo;
            if (quantityInfo?.UnitType == unit.GetType())
            {
                SimpleCache<Enum, QuantityInfo>.Instance.TryAdd(unit, quantityInfo);
                return true;
            }

            return false;
        }

        private static IQuantity GetQuantity<T>(T obj, PropertyInfo property)
        {
            // Get quantity metadata
            var metadata = ObjectMetadata<T>.GetQuantityMetadata(property);
            if (metadata?.UnitInfo is null || metadata.QuantityInfo is null)
                throw new InvalidOperationException($"Unit metadata does not exist for property {property.DeclaringType}.{property.Name}.");

            // Get property getter from cache, or get and add to cache
            var getter = SimpleCache<PropertyInfo, MethodInfo>.Instance.GetOrAdd(property, () =>
            {
                var getter = property.GetGetMethod();
                if (getter is null)
                    throw new InvalidOperationException($"{property.DeclaringType}.{property.Name} does not have a public getter.");
                if (!LazyQuantityValueCompatibleTypes.Value.Contains(getter.ReturnType))
                    throw new InvalidOperationException($"{property.DeclaringType}.{property.Name} type of {getter.ReturnType} is not compatible with {typeof(QuantityValue)}.");

                return getter;
            });

            // Try to create a quantity for a build-in unit type
            var value = Convert.ToDouble(getter.Invoke(obj, null));
            if (Quantity.TryFrom(value, metadata.UnitInfo.Value, out var quantity))
                return quantity!;

            // Get quantity constructor for a custom unit type from cache, or get and add to cache
            var quantityCtor = SimpleCache<Type, ConstructorInfo>.Instance.GetOrAdd(metadata.QuantityInfo.ValueType, () =>
            {
                var ctor = (
                    from c in metadata.QuantityInfo.ValueType.GetConstructors()
                    let parameters = c.GetParameters()
                    where parameters.Count() == 2
                    where parameters.First().ParameterType == typeof(double)
                    where parameters.Last().ParameterType == metadata.QuantityInfo.UnitType
                    select c).SingleOrDefault();
                if (ctor is null)
                    throw new InvalidOperationException($"Unable to create quantity. No constructor found compatible with {metadata.QuantityInfo.ValueType}({typeof(double)}, {metadata.QuantityInfo.UnitType})");

                return ctor;
            });
            return (IQuantity)quantityCtor.Invoke(new object[] { value, metadata.UnitInfo.Value });
        }
    }
}