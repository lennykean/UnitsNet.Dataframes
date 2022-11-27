using System.Collections.Generic;
using System.Reflection;

namespace UnitsNet.Metadata
{
    public interface IMetadataProvider<out TMetadata> where TMetadata : QuantityMetadata
    {
        IEnumerable<TMetadata> GetMetadatas();
        TMetadata GetMetadata(PropertyInfo property);
    }
}