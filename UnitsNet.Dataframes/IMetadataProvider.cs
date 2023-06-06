using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public interface IMetadataProvider<TMetadataAttribue, TMetadata>
    where TMetadataAttribue : QuantityAttribute, DataframeMetadata<TMetadataAttribue, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribue, TMetadata>.IClonableMetadata
{
    IEnumerable<TMetadata> GetAllMetadata();
    bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null);
}