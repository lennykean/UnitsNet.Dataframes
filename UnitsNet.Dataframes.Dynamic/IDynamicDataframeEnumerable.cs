using System.Globalization;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes.Dynamic;

public interface IDynamicDataframeEnumerable<TDataframe, TMetadataAttribute, TMetadata> :
    IEnumerable<TDataframe>,
    IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    DataframeMetadata<TMetadataAttribute, TMetadata> GetDataframeMetadata(CultureInfo? culture = null);
}