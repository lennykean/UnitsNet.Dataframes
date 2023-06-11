using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Reflection;

namespace UnitsNet.Metadata;
public static class ObjectExtensions
{
    public static IMetadataDictionary<TMetadata> GetDataframeMetadata<TObject, TMetadataAttribute, TMetadata>(this TObject obj, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        var metadataProvider = obj as IMetadataProvider<TMetadataAttribute, TMetadata>
            ?? AnnotationMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        metadataProvider.ValidateAllMetadata(typeof(TObject));

        return new MetadataDictionary<TMetadata>(metadataProvider.GetAllMetadata(typeof(TObject), culture));
    }

    public static IMetadataDictionary<QuantityMetadata> GetDataframeMetadata<TObject>(this TObject obj, CultureInfo? culture = null)
        where TObject : class
    {
        return obj.GetDataframeMetadata<TObject, QuantityAttribute, QuantityMetadata>(culture);
    }

    public static IQuantity GetQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetProperty(propertyName);

        return obj.GetQuantityFromProperty<TObject, TMetadataAttribute, TMetadata>(property, culture);
    }

    public static IQuantity GetQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, string propertyName, CultureInfo? culture = null)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        var property = typeof(TObject).GetProperty(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        return obj.GetQuantityFromProperty<TObject, TMetadataAttribute, TMetadata>(property, culture);
    }

    public static IQuantity GetQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression)
        where TObject : class
    {
        return obj.GetQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertySelectorExpression);
    }


    public static IQuantity GetQuantity<TObject>(this TObject obj, string propertyName)
        where TObject : class
    {
        return obj.GetQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertyName);
    }

    public static IQuantity ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TObject).GetProperty(propertyName);

        return obj.ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(property, to);
    }

    public static IQuantity ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, string propertyName, Enum to)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        var property = typeof(TObject).GetProperty(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TObject).Name}");

        return obj.ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(property, to);
    }

    public static IQuantity ConvertQuantity<TObject>(this TObject obj, Expression<Func<TObject, QuantityValue>> propertySelectorExpression, Enum to)
        where TObject : class
    {
        return obj.ConvertQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertySelectorExpression, to);
    }

    public static IQuantity ConvertQuantity<TObject>(this TObject obj, string propertyName, Enum to)
        where TObject : class
    {
        return obj.ConvertQuantity<TObject, QuantityAttribute, QuantityMetadata>(propertyName, to);
    }

    private static IQuantity ConvertQuantity<TObject, TMetadataAttribute, TMetadata>(this TObject obj, PropertyInfo property, Enum to)
        where TObject : class
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
    {
        if (obj is null)
            throw new ArgumentNullException(nameof(obj));

        var metadataProvider = obj as IMetadataProvider<TMetadataAttribute, TMetadata>
            ?? AnnotationMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        var (fromMetadata, toMetadata) = property.GetConversionMetadatas(to, metadataProvider);
        var value = obj.GetQuantityValueFromProperty(property);

        if (!fromMetadata.TryConvertQuantity(value, toMetadata, out var quantity))
            throw new InvalidOperationException($"Invalid conversion from {fromMetadata.QuantityType.Name} to {toMetadata.QuantityType.Name}.");

        return quantity;
    }
}