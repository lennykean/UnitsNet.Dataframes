using System.Globalization;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicProxy;

public interface IDynamicProxyEnumerable<TObject, TMetadataAttribute, TMetadata> : IEnumerable<TObject>, IMetadataProvider<TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    IReadOnlyDictionary<string, TMetadata> GetObjectMetadata(CultureInfo? culture = null);
}