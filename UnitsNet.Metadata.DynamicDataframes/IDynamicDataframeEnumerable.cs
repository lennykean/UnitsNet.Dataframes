using System.Globalization;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.DynamicDataframes;

public interface IDynamicDataframeEnumerable<TObject, TMetadataAttribute, TMetadata> : IEnumerable<TObject>, IMetadataProvider<TMetadataAttribute, TMetadata>
    where TObject : class
    where TMetadataAttribute : QuantityAttribute, IMetadataAttribute<TMetadataAttribute, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
    IMetadataDictionary<TMetadata> GetDataframeMetadata(CultureInfo? culture = null);
}