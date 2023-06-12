using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicProxy;

public static class DynamicProxyExtensions
{
    public static DynamicProxyBuilder<TObject, TMetadataAttribute, TMetadata> AsDynamicQuantities<TObject, TMetadataAttribute, TMetadata>(this IEnumerable<TObject> objects)
        where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
        where TMetadata : QuantityMetadata, IMetadata<TMetadata>
        where TObject : class
    {
        var baseMetadataProvider =
            objects as IMetadataProvider<TMetadataAttribute, TMetadata> ??
            GlobalMetadataProvider<TMetadataAttribute, TMetadata>.Instance;

        return new(objects, baseMetadataProvider);
    }

    public static DynamicProxyBuilder<TObject, QuantityAttribute, QuantityMetadata> AsDynamicQuantities<TObject>(this IEnumerable<TObject> objects)
        where TObject : class
    {
        return objects.AsDynamicQuantities<TObject, QuantityAttribute, QuantityMetadata>();
    }
}
