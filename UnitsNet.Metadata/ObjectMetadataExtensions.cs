using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public static class ObjectMetadataExtensions
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

        public static IQuantity GetQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression)
        {
            var property = ExtractProperty(propertySelectorExpression);

            return GetQuantity(obj, property);
        }

        public static IQuantity GetQuantity<TObject>(this TObject obj, string propertyName)
        {
            var property = typeof(TObject).GetProperty(propertyName);

            return GetQuantity(obj, property);
        }

        public static IQuantity ConvertQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
        {
            var property = ExtractProperty(propertySelectorExpression);

            return GetQuantity(obj, property);
        }

        public static IQuantity ConvertQuantity<TObject>(this TObject obj, string propertyName, Enum to)
        {
            var property = typeof(TObject).GetProperty(propertyName);

            return GetQuantity(obj, property);
        }

        private static PropertyInfo ExtractProperty<TObject>(Expression<Func<TObject, QuantityValue>> propertySelectorExpression)
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

        private static IQuantity GetQuantity<TObject>(TObject obj, PropertyInfo property)
        {
            var metadata = ObjectMetadata.GetQuantityMetadata(property);
            if (metadata?.Unit is null)
                throw new InvalidOperationException($"Unit metadata does not exist for property {property.DeclaringType}.{property.Name}.");
            
            var value = GetValue(obj, property);

            return metadata.Unit.UnitInfo.Value.CreateQuantity(metadata.Unit.QuantityType.QuantityInfo.ValueType, value);
        }

        private static double GetValue<TObject>(TObject obj, PropertyInfo property)
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
    }
}