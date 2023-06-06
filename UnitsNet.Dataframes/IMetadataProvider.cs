using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

namespace UnitsNet.Dataframes;

public interface IMetadataProvider<TMetadata> where TMetadata : QuantityMetadata
{
    IEnumerable<TMetadata> GetAllMetadata<TDataframe>();
    bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null);
}