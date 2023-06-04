using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UnitsNet.Dataframes;

public class DataframeMetadataBase<TMetadata> : ReadOnlyDictionary<PropertyInfo, TMetadata>
    where TMetadata : QuantityMetadata
{
    public DataframeMetadataBase(IEnumerable<TMetadata> metadatas) : base(metadatas.ToDictionary(k => k.Property, v => v))
    {
    }
}