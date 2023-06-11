using System.Collections.Generic;

namespace UnitsNet.Metadata;

public interface IMetadataDictionary<TMetadata> : IReadOnlyDictionary<string, TMetadata>
    where TMetadata : QuantityMetadata, IMetadata<TMetadata>
{
}
