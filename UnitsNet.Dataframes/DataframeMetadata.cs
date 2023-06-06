using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public class DataframeMetadata<TMetadataAttribute, TMetadata> : IEnumerable<TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    public interface IMetadataAttribute
    {
        TMetadata ToMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> conversions, UnitMetadata? overrideUnit = null, CultureInfo? culture = null);
    }

    public interface IClonableMetadata
    {
        TMetadata Clone(PropertyInfo property, IEnumerable<UnitMetadataBasic>? overrideConversions = null, UnitMetadata? overrideUnit = null, CultureInfo? culture = null);
    }

    private readonly IEnumerable<TMetadata> _metadatas;

    internal DataframeMetadata(IEnumerable<TMetadata> metadatas)
    {
        _metadatas = metadatas;
    }

    public IEnumerator<TMetadata> GetEnumerator()
    {
        return _metadatas.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public sealed class DataframeMetadata : DataframeMetadata<QuantityAttribute, QuantityMetadata>
{
    public DataframeMetadata(IEnumerable<QuantityMetadata> metadatas) : base(metadatas)
    {
    }
}