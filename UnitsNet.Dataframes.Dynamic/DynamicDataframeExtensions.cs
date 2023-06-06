using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

public static class DynamicDataframeExtensions
{
    public static DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> AsDynamicDataframes<TDataframe, TMetadataAttribute, TMetadata>(this IEnumerable<TDataframe> dataframes)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
        where TDataframe : class
    {
        return new DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata>(dataframes);
    }

    public static DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata> AsDynamicDataframe<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
        where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
        where TDataframe : class
    {
        return new DynamicDataframesBuilder<TDataframe, TMetadataAttribute, TMetadata>(new[] { dataframe });
    }
}
