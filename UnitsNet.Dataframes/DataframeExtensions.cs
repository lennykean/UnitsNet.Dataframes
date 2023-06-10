using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes;

public static class DataframeExtensions
{
    public static DataframeMetadata<TMetadataAttribute, TMetadata> GetDataframeMetadata<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, CultureInfo? culture = null)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        var metadataProvider = dataframe as IDataframeMetadataProvider<TMetadataAttribute, TMetadata>
            ?? DefaultDataframeMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        metadataProvider.ValidateAllMetadata(typeof(TDataframe));

        return new DataframeMetadata<TMetadataAttribute, TMetadata>(metadataProvider.GetAllMetadata(typeof(TDataframe), culture));
    }

    public static DataframeMetadata<QuantityAttribute, QuantityMetadata> GetDataframeMetadata<TDataframe>(this TDataframe dataframe, CultureInfo? culture = null)
        where TDataframe : class
    {
        return dataframe.GetDataframeMetadata<TDataframe, QuantityAttribute, QuantityMetadata>(culture);
    }

    public static IQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, CultureInfo? culture = null)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TDataframe).GetProperty(propertyName);

        return dataframe.GetQuantityFromProperty<TDataframe, TMetadataAttribute, TMetadata>(property, culture);
    }

    public static TQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertySelectorExpression);
    }

    public static IQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, string propertyName, CultureInfo? culture = null)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var property = typeof(TDataframe).GetProperty(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TDataframe).Name}");

        return dataframe.GetQuantityFromProperty<TDataframe, TMetadataAttribute, TMetadata>(property, culture);
    }

    public static TQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, string propertyName)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertyName);
    }

    public static IQuantity GetQuantity<TDataframe>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression)
        where TDataframe : class
    {
        return dataframe.GetQuantity<TDataframe, QuantityAttribute, QuantityMetadata>(propertySelectorExpression);
    }

    public static TQuantity GetQuantity<TDataframe, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression)
        where TDataframe : class
        where TQuantity : IQuantity
    {
        return dataframe.GetQuantity<TDataframe, QuantityAttribute, QuantityMetadata, TQuantity>(propertySelectorExpression);
    }

    public static IQuantity GetQuantity<TDataframe>(this TDataframe dataframe, string propertyName)
        where TDataframe : class
    {
        return dataframe.GetQuantity<TDataframe, QuantityAttribute, QuantityMetadata>(propertyName);
    }

    public static TQuantity GetQuantity<TDataframe, TQuantity>(this TDataframe dataframe, string propertyName)
        where TDataframe : class
        where TQuantity : IQuantity
    {
        return dataframe.GetQuantity<TDataframe, QuantityAttribute, QuantityMetadata, TQuantity>(propertyName);
    }

    public static IQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        var propertyName = propertySelectorExpression.ExtractPropertyName();
        var property = typeof(TDataframe).GetProperty(propertyName);

        return dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(property, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertySelectorExpression, to);
    }

    public static IQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, string propertyName, Enum to)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        var property = typeof(TDataframe).GetProperty(propertyName) ??
            throw new InvalidOperationException($"{propertyName} is not a property of {typeof(TDataframe).Name}");

        return dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(property, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, string propertyName, Enum to)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        var property = typeof(TDataframe).GetProperty(propertyName);

        return (TQuantity)dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(property, to);
    }

    public static IQuantity ConvertQuantity<TDataframe>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
        where TDataframe : class
    {
        return dataframe.ConvertQuantity<TDataframe, QuantityAttribute, QuantityMetadata>(propertySelectorExpression, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
        where TDataframe : class
        where TQuantity : IQuantity
    {
        return dataframe.ConvertQuantity<TDataframe, QuantityAttribute, QuantityMetadata, TQuantity>(propertySelectorExpression, to);
    }

    public static IQuantity ConvertQuantity<TDataframe>(this TDataframe dataframe, string propertyName, Enum to)
        where TDataframe : class
    {
        return dataframe.ConvertQuantity<TDataframe, QuantityAttribute, QuantityMetadata>(propertyName, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TQuantity>(this TDataframe dataframe, string propertyName, Enum to)
        where TDataframe : class
    {
        return dataframe.ConvertQuantity<TDataframe, QuantityAttribute, QuantityMetadata, TQuantity>(propertyName, to);
    }

    private static IQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, PropertyInfo property, Enum to)
        where TDataframe : class
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var (fromMetadata, toMetadata) = property.GetConversionMetadatas(to, dataframe as IDataframeMetadataProvider<TMetadataAttribute, TMetadata>);
        var value = dataframe.GetQuantityValueFromProperty(property);

        if (!fromMetadata.TryConvertQuantity(value, toMetadata, out var quantity))
            throw new InvalidOperationException($"Invalid conversion from {fromMetadata.QuantityType.Name} to {toMetadata.QuantityType.Name}.");

        return quantity;
    }
}