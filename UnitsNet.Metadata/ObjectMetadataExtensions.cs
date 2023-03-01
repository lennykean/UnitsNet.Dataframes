using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata
{
    public static class ObjectMetadataExtensions
    {
        public static ObjectMetadata<QuantityMetadata> GetObjectMetadata<T>(this T obj)
        {
            if (obj is IMetadataProvider<QuantityMetadata> metadataProvider)
                return new ObjectMetadata<QuantityMetadata>(metadataProvider.GetAllMetadata<T>());

            return QuantityObjectMetadata.For<T>();
        }

        public static ObjectMetadata<QuantityMetadata> GetObjectMetadata<T>(this IEnumerable<T> obj)
        {
            if (obj is IMetadataProvider<QuantityMetadata> metadataProvider)
                return new ObjectMetadata<QuantityMetadata>(metadataProvider.GetAllMetadata<T>());

            return QuantityObjectMetadata.For<T>();
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