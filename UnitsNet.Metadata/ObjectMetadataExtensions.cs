using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata
{
    public static class ObjectMetadataExtensions
    {
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
            var property = ReflectionUtils.ExtractProperty(propertySelectorExpression);

            return ReflectionUtils.GetQuantity(obj, property);
        }

        public static IQuantity GetQuantity<TObject>(this TObject obj, string propertyName)
        {
            var property = typeof(TObject).GetProperty(propertyName);

            return ReflectionUtils.GetQuantity(obj, property);
        }

        public static IQuantity ConvertQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
        {
            var property = ReflectionUtils.ExtractProperty(propertySelectorExpression);

            return ReflectionUtils.ConvertQuantity(obj, property, to);
        }

        public static IQuantity ConvertQuantity<TObject>(this TObject obj, string propertyName, Enum to)
        {
            var property = typeof(TObject).GetProperty(propertyName);

            return ReflectionUtils.ConvertQuantity(obj, property, to);
        }
    }
}