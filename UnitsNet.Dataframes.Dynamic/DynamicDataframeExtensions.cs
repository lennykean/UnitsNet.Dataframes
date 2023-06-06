using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

public static class DynamicDataframeExtensions
{
    public static DynamicDataframesBuilder<TDataframe, TMetadata> AsDynamicDataframes<TDataframe, TMetadataAttribute, TMetadata>(this IEnumerable<TDataframe> dataframes)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
        where TDataframe : class
    {
        return new DynamicDataframesBuilder<TDataframe, TMetadata>(dataframes);
    }

    public static DynamicDataframesBuilder<TDataframe, TMetadata> AsDynamicDataframe<TDataframe, TMetadataAttribute, TMetadata>(this TDataframe dataframe)
        where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
        where TMetadata : QuantityMetadata
        where TDataframe : class
    {
        return new DynamicDataframesBuilder<TDataframe, TMetadata>(new[] { dataframe });
    }
}
