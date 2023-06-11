using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicDataframes;

public static class DynamicDataframeExtensions
{
    public static DynamicDataframesBuilder<TObject, TMetadataAttribute, TMetadata> AsDynamicDataframes<TObject, TMetadataAttribute, TMetadata>(this IEnumerable<TObject> dataframes)
        where TMetadataAttribute: QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
        where TObject : class
    {
        var baseMetadataProvider =
            dataframes as IMetadataProvider<TMetadataAttribute, TMetadata> ??
            GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        return new(dataframes, baseMetadataProvider);
    }

    public static DynamicDataframesBuilder<TObject, QuantityAttribute, QuantityMetadata> AsDynamicDataframes<TObject>(this IEnumerable<TObject> dataframes)
        where TObject : class
    {
        return dataframes.AsDynamicDataframes<TObject, QuantityAttribute, QuantityMetadata>();
    }
}
