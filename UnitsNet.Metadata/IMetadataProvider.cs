using System.Collections.Generic;
using System.Reflection;

namespace UnitsNet.Metadata
{
    public interface IMetadataProvider<out TMetadata> where TMetadata : QuantityMetadata
    {
        IEnumerable<TMetadata> GetAllMetadata<TObject>();
        TMetadata? GetMetadata(PropertyInfo property);
    }
}