using System;
using System.Globalization;
using System.Linq.Expressions;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

public static class ObjectExtensions
{
    public static IMetadataDictionary<TMetadata> GetObjectMetadata<TObject, TMetadataAttribute, TMetadata>(this TObject obj, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().GetObjectMetadata(obj, culture);
    }

    public static IMetadataDictionary<QuantityMetadata> GetObjectMetadata<TObject>(this TObject obj, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetObjectMetadata<TObject, QuantityAttribute, QuantityMetadata>(culture);
    }

    public static IQuantity GetQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, string propertyName, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().GetQuantity(obj, propertyName, culture);
    }

    public static IQuantity GetQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().GetQuantity(obj, propertySelectorExpression, culture);
    }

    public static IQuantity GetQuantity<TObject>(this TObject obj, string propertyName)
        where TObject : class
    {
        return obj.GetQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertyName);
    }

    public static IQuantity GetQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression)
        where TObject : class
    {
        return obj.GetQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertySelectorExpression);
    }

    public static IQuantity ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, string propertyName, Enum to)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().ConvertQuantity(obj, propertyName, to);
    }

    public static IQuantity ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().ConvertQuantity(obj, propertySelectorExpression, to);
    }

    public static IQuantity ConvertQuantity<TObject>(this TObject obj, string propertyName, Enum to)
        where TObject : class
    {
        return obj.ConvertQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertyName, to);
    }

    public static IQuantity ConvertQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
        where TObject : class
    {
        return obj.ConvertQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertySelectorExpression, to);
    }
    
    private static IMetadataProvider<TMetadataAttribute, TMetadata> GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>(this TObject obj)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        if (obj is IMetadataProvider<TMetadataAttribute, TMetadata> metadataProvider)
            return metadataProvider;

        return GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance;
    }
}