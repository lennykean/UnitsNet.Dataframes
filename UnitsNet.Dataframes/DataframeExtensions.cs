using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes;

public static class DataframeExtensions
{
    public static DataframeMetadataBase<QuantityMetadata> GetDataframeMetadata<TDataframe>(this TDataframe dataframe)
    {
        if (dataframe is IMetadataProvider<QuantityMetadata> metadataProvider)
            return new DataframeMetadataBase<QuantityMetadata>(metadataProvider.GetAllMetadata<TDataframe>());

        return DataFrameMetadata.For<TDataframe>();
    }

    public static DataframeMetadataBase<QuantityMetadata> GetDataframesMetadata<TDataframe>(this IEnumerable<TDataframe> dataframes)
    {
        if (dataframes is IMetadataProvider<QuantityMetadata> metadataProvider)
            return new DataframeMetadataBase<QuantityMetadata>(metadataProvider.GetAllMetadata<TDataframe>());

        return DataFrameMetadata.For<TDataframe>();
    }

    public static IQuantity GetQuantity<TDataframe>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression)
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var property = propertySelectorExpression.ExtractProperty();
        
        return dataframe.GetQuantityFromProperty(property);
    }

    public static TQuantity GetQuantity<TDataframe, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression) where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.GetQuantity(propertySelectorExpression);
    }

    public static IQuantity GetQuantity<TDataframe>(this TDataframe dataframe, string propertyName)
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var property = typeof(TDataframe).GetProperty(propertyName);

        return dataframe.GetQuantityFromProperty(property);
    }

    public static TQuantity GetQuantity<TDataframe, TQuantity>(this TDataframe dataframe, string propertyName) where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.GetQuantity(propertyName);
    }

    public static IQuantity ConvertQuantity<TDataframe>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to)
    {
        var property = propertySelectorExpression.ExtractProperty();

        return dataframe.ConvertQuantity(property, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TQuantity>(this TDataframe dataframe, Expression<Func<TDataframe, QuantityValue>> propertySelectorExpression, Enum to) where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.ConvertQuantity(propertySelectorExpression, to);
    }

    public static IQuantity ConvertQuantity<TDataframe>(this TDataframe dataframe, string propertyName, Enum to)
    {
        var property = typeof(TDataframe).GetProperty(propertyName);
        
        return dataframe.ConvertQuantity(property, to);
    }

    public static TQuantity ConvertQuantity<TDataframe, TQuantity>(this TDataframe dataframe, string propertyName, Enum to) where TQuantity : IQuantity
    {
        return (TQuantity)dataframe.ConvertQuantity(propertyName, to);
    }

    private static IQuantity ConvertQuantity<TDataframe>(this TDataframe dataframe, PropertyInfo property, Enum to)
    {
        if (dataframe is null)
            throw new ArgumentNullException(nameof(dataframe));

        var (fromMetadata, toMetadata) = property.GetConversionMetadata(to, dataframe as IMetadataProvider<QuantityMetadata>);
        var value = dataframe.GetQuantityValueFromProperty(property);
        
        if (!fromMetadata.TryConvertQuantity(value, toMetadata, out var quantity))
            throw new InvalidOperationException($"Invalid conversion from {fromMetadata.QuantityType.Name} to {toMetadata.QuantityType.Name}.");

        return quantity;
    }
}