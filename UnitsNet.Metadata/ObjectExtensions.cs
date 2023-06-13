using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml.Linq;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata;

public static class ObjectExtensions
{
    public static IReadOnlyDictionary<string, TMetadata> GetObjectMetadata<TObject, TMetadataAttribute, TMetadata>(this TObject obj, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().GetObjectMetadata(obj, culture);
    }

    public static IReadOnlyDictionary<string, QuantityMetadata> GetObjectMetadata<TObject>(this TObject obj, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetObjectMetadata<TObject, QuantityAttribute, QuantityMetadata>(culture);
    }

    public static IQuantity GetQuantity<TObject>(this TObject obj, string propertyName, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>().GetQuantity(obj, propertyName, culture);
    }

    public static IQuantity GetQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>().GetQuantity(obj, propertySelectorExpression, culture);
    }

    public static IQuantity ConvertQuantity<TObject>(this TObject obj, string propertyName, Enum to, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>().ConvertQuantity(obj, propertyName, to, culture);
    }

    public static IQuantity ConvertQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>().ConvertQuantity(obj, propertySelectorExpression, to, culture);
    }

    public static IQuantity SetQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, string propertyName, IQuantity quantity, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().SetQuantity(obj, propertyName, quantity, culture);
    }

    public static IQuantity SetQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, IQuantity quantity, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        return obj.GetMetadataProvider<TObject, TMetadataAttribute, TMetadata>().SetQuantity(obj, propertySelectorExpression, quantity, culture);
    }

    public static IQuantity SetQuantity<TObject>(this TObject obj, string propertyName, IQuantity quantity, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>().SetQuantity(obj, propertyName, quantity, culture);
    }

    public static IQuantity SetQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, IQuantity quantity, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetMetadataProvider<TObject, QuantityAttribute, QuantityMetadata>().SetQuantity(obj, propertySelectorExpression, quantity, culture);
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