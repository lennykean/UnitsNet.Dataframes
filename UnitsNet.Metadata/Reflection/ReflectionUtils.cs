using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata.Reflection
{
    internal static class ReflectionUtils
    {
        private static readonly Lazy<Type[]> LazyQuantityValueCompatibleTypes = new(() => (
            from m in typeof(QuantityValue).GetMethods(BindingFlags.Public | BindingFlags.Static)
            where m.Name == "op_Implicit"
            select m.GetParameters().First().ParameterType).ToArray());

        public static PropertyInfo ExtractProperty<TObject>(Expression<Func<TObject, QuantityValue>> propertySelectorExpression)
        {
            var expression = propertySelectorExpression.Body;

            // Unwrap any casts in the expression tree
            while (expression is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
                expression = unaryExpression.Operand;

            // Ensure the expression is a property accessor and get the PropertyInfo
            if (expression is not MemberExpression memberExpression || memberExpression.Member is not PropertyInfo property || property.GetGetMethod()?.IsPublic != true)
                throw new InvalidOperationException($"{{{propertySelectorExpression}}} is not a valid property accessor.");

            return property;
        }

        public static double GetValue<TObject>(TObject obj, PropertyInfo property)
        {
            // Get property getter from cache, or get and add to cache
            var getter = SimpleCache<PropertyInfo, MethodInfo>.Instance.GetOrAdd(property, p =>
            {
                var getter = p.GetGetMethod();
                if (getter is null)
                    throw new InvalidOperationException($"{p.DeclaringType}.{p.Name} does not have a public getter.");
                if (!LazyQuantityValueCompatibleTypes.Value.Contains(getter.ReturnType))
                    throw new InvalidOperationException($"{p.DeclaringType}.{p.Name} type of {getter.ReturnType} is not compatible with {typeof(QuantityValue)}.");

                return getter;
            });
            return Convert.ToDouble(getter.Invoke(obj, new object[] { }));
        }

        public static IQuantity GetQuantity<TObject>(TObject obj, PropertyInfo property)
        {
            var metadata = ObjectMetadata.GetQuantityMetadata(property);
            if (metadata?.Unit is null)
                throw new InvalidOperationException($"Unit metadata does not exist for {property.DeclaringType.Name}.{property.Name}.");

            var value = GetValue(obj, property);

            return CreateQuantity(
                metadata.Unit.UnitInfo.Value,
                metadata.Unit.QuantityType.QuantityInfo.ValueType,
                value);
        }

        public static IQuantity ConvertQuantity<TObject>(TObject obj, PropertyInfo property, Enum to)
        {
            var metadata = ObjectMetadata.GetQuantityMetadata(property);
            if (metadata?.Unit is null)
                throw new InvalidOperationException($"Unit metadata does not exist for {property.DeclaringType.Name}.{property.Name}.");

            var conversion = metadata.Conversions.SingleOrDefault(c => c.UnitInfo.Value.Equals(to));
            if (conversion is null)
                throw new InvalidOperationException($"Conversion to {to} is not allowed on {property.DeclaringType.Name}.{property.Name}.");

            var quantity = GetQuantity(obj, property);
            if (!UnitConverter.Default.TryGetConversionFunction(
                metadata.Unit.QuantityType.QuantityInfo.ValueType,
                metadata.Unit.UnitInfo.Value,
                conversion.QuantityType.QuantityInfo.ValueType,
                to,
                out var conversionFunction))
            {
                throw new InvalidOperationException($"Invalid unit conversion from {quantity.Unit} to {to}.");
            }
            return conversionFunction(quantity);
        }

        public static IQuantity CreateQuantity(Enum unit, Type quantityType, double value)
        {
            // Get quantity metadata
            if (!unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                throw new ArgumentException($"{unit.GetType().Name} is not a known unit type.");
            if (!unit.TryGetUnitInfo(quantityType, out var unitInfo))
                throw new ArgumentException($"{unit.GetType().Name}.{unit} is not a known unit value.");

            // Try to create a quantity for a build-in unit type
            if (Quantity.TryFrom(value, unit, out var quantity))
                return quantity!;

            // Get quantity constructor for a custom unit type from cache, or get and add to cache
            var quantityCtor = SimpleCache<Type, (ConstructorInfo ctor, Type valueParamType)>.Instance.GetOrAdd(quantityType, t =>
            {
                var ctor = (
                    from c in t.GetConstructors()
                    let parameters = c.GetParameters()
                    where parameters.Count() == 2
                    where 
                        parameters.Last().ParameterType == typeof(QuantityValue) 
                        || LazyQuantityValueCompatibleTypes.Value.Contains(parameters.First().ParameterType)
                    where parameters.Last().ParameterType == quantityInfo!.UnitType
                    select c).SingleOrDefault();
                if (ctor is null)
                    throw new InvalidOperationException($"Unable to create quantity. No constructor found compatible with {t.Name}({typeof(QuantityValue).Name}, {quantityInfo!.UnitType.Name})");

                return (ctor, ctor.GetParameters().First().ParameterType);
            });
            return (IQuantity)quantityCtor.ctor.Invoke(new object[] { Convert.ChangeType(value, quantityCtor.valueParamType), unit });
        }

        public static bool TryGetStaticPropertyByType<TReturn>(Type type, out TReturn? value)
        {
            var staticProperty = type.GetProperties(BindingFlags.Public | BindingFlags.Static).SingleOrDefault(p => typeof(TReturn).IsAssignableFrom(p.PropertyType));
            var staticGetter = staticProperty?.GetGetMethod();
            if (staticGetter is null)
            {
                value = default;
                return false;
            }
            value = (TReturn)staticGetter.Invoke(null, null);
            return true;
        }

        public static bool TryConstructQuantity(Type type, out IQuantity? instance)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor is null || !typeof(IQuantity).IsAssignableFrom(type))
            {
                instance = default;
                return false;
            }
            instance = (IQuantity)ctor.Invoke(null);
            return true;
        }
    }
}