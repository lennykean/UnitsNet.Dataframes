using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes;

public static class DataframeExtensions
{
    public static DataframeMetadata<TMetadataAttribute, TMetadata> GetDataframeMetadata<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, CultureInfo? culture = null)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        if (dataframe is IMetadataProvider<TMetadata> metadataProvider)
            return new DataframeMetadata<TMetadataAttribute, TMetadata>(metadataProvider.GetAllMetadata<TDataframe>());
        
        var metadatas = MetadataBuilder.BuildDataframeMetadata<TMetadataAttribute, TMetadata>(typeof(TDataframe), culture);

        return new DataframeMetadata<TMetadataAttribute, TMetadata>(metadatas);
    }

    public static DataframeMetadata<TMetadataAttribute, TMetadata> GetDataframesMetadata<TDataframe, TMetadataAttribute, TMetadata>(this IEnumerable<TDataframe> dataframes, CultureInfo? culture = null)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        if (dataframes is IMetadataProvider<TMetadata> metadataProvider)
            return new DataframeMetadata<TMetadataAttribute, TMetadata>(metadataProvider.GetAllMetadata<TDataframe>());

        var metadatas = MetadataBuilder.BuildDataframeMetadata<TMetadataAttribute, TMetadata>(typeof(TDataframe), culture);

        return new DataframeMetadata<TMetadataAttribute, TMetadata>(metadatas);
    }

    public static IQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var property = propertySelectorExpression.ExtractProperty();
        
        return dataframe.GetQuantityFromProperty<TDataframe, TMetadataAttribute, TMetadata>(property);
    }

    public static TQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertySelectorExpression);
    }

    public static IQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, string propertyName)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var property = typeof(TDataframe).GetProperty(propertyName);

        return dataframe.GetQuantityFromProperty<TDataframe, TMetadataAttribute, TMetadata>(property);
    }

    public static TQuantity GetQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, string propertyName)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.GetQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertyName);
    }

    public static IQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        var property = propertySelectorExpression.ExtractProperty();

        return dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(property, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertySelectorExpression, to);
    }

    public static IQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, string propertyName, Enum to)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        var property = typeof(TDataframe).GetProperty(propertyName);
        
        return dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(property, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata, TQuantity>(this TDataframe dataframe, string propertyName, Enum to)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
        where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(propertyName, to);
    }

    private static IQuantity ConvertQuantity<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe, PropertyInfo property, Enum to)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var (fromMetadata, toMetadata) = property.GetConversionMetadata<TMetadataAttribute, TMetadata>(to, dataframe as IMetadataProvider<TMetadata>);
        var value = dataframe.GetQuantityValueFromProperty(property);
        
        if (!fromMetadata.TryConvertQuantity(value, toMetadata, out var quantity))
            throw new InvalidOperationException($"Invalid conversion from {fromMetadata.QuantityType.Name} to {toMetadata.QuantityType.Name}.");

        return quantity;
    }
}