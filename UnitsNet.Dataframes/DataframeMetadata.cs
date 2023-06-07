using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;

namespace UnitsNet.Dataframes;

public class DataframeMetadata<TMetadataAttribute, TMetadata> : IReadOnlyDictionary<string, TMetadata>, IReadOnlyDictionary<PropertyInfo, TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    public interface IMetadataAttribute
    {
        TMetadata ToMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> conversions, UnitMetadata? overrideUnit = null, CultureInfo? culture = null);
    }

    public interface IClonableMetadata
    {
        TMetadata Clone(PropertyInfo? overrideProperty = null, IEnumerable<UnitMetadataBasic>? overrideConversions = null, UnitMetadata? overrideUnit = null, CultureInfo? overrideCulture = null);
    }

    private readonly IEnumerable<TMetadata> _metadatas;

    internal DataframeMetadata(IEnumerable<TMetadata> metadatas)
    {
        _metadatas = metadatas;
    }

    public IEnumerable<string> Keys => _metadatas.Select(metadata => metadata.FieldName).Distinct();
    
    public IEnumerable<TMetadata> Values => _metadatas;

    public int Count => _metadatas.Count();

    public TMetadata this[PropertyInfo key] => _metadatas.FirstOrDefault(metadata => metadata.Property == key);

    public TMetadata this[string key] => _metadatas.FirstOrDefault(metadata => metadata.FieldName == key);

    IEnumerable<PropertyInfo> IReadOnlyDictionary<PropertyInfo, TMetadata>.Keys => _metadatas.Select(metadata => metadata.Property).Distinct();

    public bool ContainsKey(string key)
    {
        return _metadatas.Any(metadata => metadata.FieldName == key);
    }

    public bool ContainsKey(PropertyInfo key)
    {
        return _metadatas.Any(metadata => metadata.Property == key);
    }

    public bool TryGetValue(string key, out TMetadata value)
    {
        value = default!;
        if (!ContainsKey(key))
            return false;
        
        value = this[key];
        return true;        
    }

    public bool TryGetValue(PropertyInfo key, out TMetadata value)
    {
        value = default!;
        if (!ContainsKey(key))
            return false;

        value = this[key];
        return true;
    }

    public IEnumerator<KeyValuePair<string, TMetadata>> GetEnumerator()
    {
        return _metadatas.Select(metadata => new KeyValuePair<string, TMetadata>(metadata.FieldName, metadata)).GetEnumerator();
    }

    IEnumerator<KeyValuePair<PropertyInfo, TMetadata>> IEnumerable<KeyValuePair<PropertyInfo, TMetadata>>.GetEnumerator()
    {
        return _metadatas.Select(metadata => new KeyValuePair<PropertyInfo, TMetadata>(metadata.Property, metadata)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}