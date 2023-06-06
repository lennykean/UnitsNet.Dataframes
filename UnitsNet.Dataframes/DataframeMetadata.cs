using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public class DataframeMetadata<TMetadataAttribute, TMetadata> : IEnumerable<TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
    where TMetadata : QuantityMetadata
{
    public interface IMetadataFactory
    {
        TMetadata CreateMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> allowedConversions, CultureInfo? culture = null, UnitMetadata? overrideUnit = null);
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

public sealed class DataFrameMetadata : DataframeMetadata<QuantityAttribute, QuantityMetadata>
{
    public DataFrameMetadata(IEnumerable<QuantityMetadata> metadatas) : base(metadatas)
    {
    }
}