using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

public static class DynamicDataframeExtensions
{
    public static DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> AsDynamicDataframes<TDataframe, TMetadataAttribute, TMetadata>(this IEnumerable<TDataframe> dataframes)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
        where TDataframe : class
    {
        return new(dataframes);
    }

    public static DynamicDataframesBuilder<TDataframe, QuantityAttribute, QuantityMetadata> AsDynamicDataframes<TDataframe>(this IEnumerable<TDataframe> dataframes)
        where TDataframe : class
    {
        return dataframes.AsDynamicDataframes<TDataframe, QuantityAttribute, QuantityMetadata>();
    }
}
