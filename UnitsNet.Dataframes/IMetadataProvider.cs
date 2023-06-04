using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace UnitsNet.Dataframes;

public interface IMetadataProvider<out TMetadata> where TMetadata : QuantityMetadata
{
    IEnumerable<TMetadata> GetAllMetadata<TDataframe>();
    bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out QuantityMetadata? metadata);
}