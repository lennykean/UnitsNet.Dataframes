using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public interface IDataframeMetadataProvider<TDataframe, TMetadataAttribue, TMetadata>
    where TMetadataAttribue : QuantityAttribute, DataframeMetadata<TMetadataAttribue, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribue, TMetadata>.IClonableMetadata
{
    bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null);
    IEnumerable<TMetadata> GetAllMetadata(CultureInfo? culture = null);
}