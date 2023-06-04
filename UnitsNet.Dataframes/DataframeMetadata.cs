using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public sealed class DataFrameMetadata :
    DataframeMetadataBase<QuantityMetadata>,
    IDataframeMetadata<QuantityAttribute, QuantityMetadata, DataFrameMetadata.Mapper>
{
    public class Mapper : IDataframeMetadata<QuantityAttribute, QuantityMetadata, Mapper>.IMetadataAttributeMapper
    {
        public QuantityMetadata Map(QuantityAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
        {
            return QuantityMetadata.FromQuantityAttribute(metadataAttribute, property, allowedConversions, culture);
        }
    }

    public DataFrameMetadata(Type forType, CultureInfo? culture = null) : base(IDataframeMetadata<QuantityAttribute, QuantityMetadata, Mapper>.BuildMetadata(forType, culture))
    {
    }

    public static DataFrameMetadata For<TDataframe>(CultureInfo? culture = null)
    {
        return new DataFrameMetadata(typeof(TDataframe), culture);
    }

    public static bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)]out QuantityMetadata? metadata, CultureInfo? culture = null)
    {
        return IDataframeMetadata<QuantityAttribute, QuantityMetadata, Mapper>.TryGetMetadata(property, out metadata, culture);
    }

    public static IEnumerable<QuantityMetadata> BuildMetadata(Type forType, CultureInfo? culture)
    {
        return IDataframeMetadata<QuantityAttribute, QuantityMetadata, Mapper>.BuildMetadata(forType, culture);
    }
}