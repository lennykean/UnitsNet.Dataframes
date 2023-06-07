using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public interface IDataframeMetadataProvider<TMetadataAttribue, TMetadata>
    where TMetadataAttribue : QuantityAttribute, DataframeMetadata<TMetadataAttribue, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribue, TMetadata>.IDataframeMetadata
{
    bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null);
    IEnumerable<TMetadata> GetAllMetadata(CultureInfo? culture = null);
    void ValidateAllMetadata();
}

public interface IDataframeMetadataProvider<TDataframe, TMetadataAttribue, TMetadata> : IDataframeMetadataProvider<TMetadataAttribue, TMetadata>
    where TDataframe : class
    where TMetadataAttribue : QuantityAttribute, DataframeMetadata<TMetadataAttribue, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribue, TMetadata>.IDataframeMetadata
{
}