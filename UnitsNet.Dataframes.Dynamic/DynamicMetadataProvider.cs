using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicMetadataProvider<TMetadataAttribute, TMetadata> : IMetadataProvider<TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    private readonly Type _dataframeType;
    private readonly DataframeMetadata<TMetadataAttribute, TMetadata> _baseDataframeMetadata;
    private readonly ConcurrentDictionary<PropertyInfo, TMetadata> _dynamicMetadata;

    public DynamicMetadataProvider(
        Type dataframeType,
        IEnumerable<TMetadata> baseDataframeMetadata,
        IEnumerable<KeyValuePair<PropertyInfo, TMetadata>>? dynamicMetadata = null)
    {
        _baseDataframeMetadata = new(baseDataframeMetadata);
        _dynamicMetadata = new(dynamicMetadata);
        _dataframeType = dataframeType;
    }


    public DynamicMetadataProvider<TMetadataAttribute, TMetadata> HoistMetadata<TDataframe>()
        where TDataframe : class
    {
        throw new NotImplementedException("Do mapping");
        return new(typeof(TDataframe), GetAllMetadata(), _dynamicMetadata);
    }

    public IEnumerable<TMetadata> GetAllMetadata()
    {
        return
            from p in _dataframeType.GetProperties(inherit: true).Distinct()
            let m = (hasMetadata: TryGetMetadata(p, out var metadata), metadata)
            where m.hasMetadata
            select m.metadata;
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)]out TMetadata? metadata, CultureInfo? culture = null)
    {
        if (property is null)
            throw new ArgumentNullException(nameof(property));

        if (_dynamicMetadata.TryGetValue(property, out metadata))
            return true;

        var baseMetadata = _baseDataframeMetadata.FirstOrDefault(m => m.Property == property);
        if (baseMetadata?.Unit is null)
            return false;
        
        metadata = _dynamicMetadata.GetOrAdd(property, key =>
        {
            if (_dynamicMetadata.TryGetValue(key, out var dynamicMetadata))
                return dynamicMetadata;

            return baseMetadata;
        });
        return true;
    }

    public void AddConversion(PropertyInfo property, Enum unit)
    {   
        var (_, toMetadata) = property.GetConversionMetadata<TMetadataAttribute, TMetadata>(unit);

        var baseMetadata = _baseDataframeMetadata.FirstOrDefault(m => m.Property == property);
        if (baseMetadata is null || baseMetadata.Unit is null)
            throw new InvalidOperationException($"No metadata found for property {property}.");

        var convertBack = UnitMetadataBasic.FromUnitInfo(baseMetadata.Unit.UnitInfo, baseMetadata.Unit.QuantityType.QuantityInfo)!;
        var allowedConversions = baseMetadata.Conversions.Append(convertBack);

        var metadata = baseMetadata.Clone(property, allowedConversions, toMetadata);

        _dynamicMetadata.AddOrUpdate(property, _ => metadata, (_, _) => metadata);
    }
}