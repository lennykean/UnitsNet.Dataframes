using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> : IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IClonableMetadata
{
    private readonly DataframeMetadata<TMetadataAttribute, TMetadata> _baseDataframeMetadata;
    private readonly ConcurrentDictionary<PropertyInfo, TMetadata> _dynamicMetadata;

    public DynamicDataframeMetadataProvider(
        IEnumerable<TMetadata> baseDataframeMetadata,
        IEnumerable<TMetadata>? dynamicMetadata = null)
    {
        _baseDataframeMetadata = new(baseDataframeMetadata);
        _dynamicMetadata = new(dynamicMetadata.ToDictionary(k => k.Property, v => v));
    }

    public DynamicDataframeMetadataProvider<THoistedDataframe, TMetadataAttribute, TMetadata> HoistMetadata<THoistedDataframe>(CultureInfo? culture = null)
        where THoistedDataframe : class
    {
        var baseMetadatas =
            from metadata in _baseDataframeMetadata.Values
            let property = metadata.Property.TryGetMappedProperty(typeof(THoistedDataframe), out var mappedProperty) ? mappedProperty : metadata.Property
            select metadata.Clone(overrideProperty: property, overrideCulture: culture);
        var dynamicMetadatas =
            from metadata in _dynamicMetadata.Values
            let property = metadata.Property.TryGetMappedProperty(typeof(THoistedDataframe), out var mappedProperty) ? mappedProperty : metadata.Property
            select metadata.Clone(overrideProperty: property, overrideCulture: culture);

        return new(baseMetadatas, dynamicMetadatas);
    }

    public IEnumerable<TMetadata> GetAllMetadata(CultureInfo? culture = null)
    {
        return
            from p in typeof(TDataframe).GetProperties(inherit: true).Distinct()
            let m = (hasMetadata: TryGetMetadata(p, out var metadata, culture), metadata)
            where m.hasMetadata
            select m.metadata;
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        if (property is null)
            throw new ArgumentNullException(nameof(property));

        if (!_dynamicMetadata.TryGetValue(property, out metadata))
        {
            if (!_baseDataframeMetadata.TryGetValue(property, out var baseMetadata) || baseMetadata?.Unit is null)
                return false;

            metadata = _dynamicMetadata.GetOrAdd(property, key =>
            {
                if (_dynamicMetadata.TryGetValue(key, out var dynamicMetadata))
                    return dynamicMetadata;

                return baseMetadata;
            });
        }
        return true;
    }

    public void AddConversion(PropertyInfo property, Enum unit, CultureInfo? culture = null)
    {
        var (_, toMetadata) = property.GetConversionMetadatas(to: unit, this, culture);

        if (!_baseDataframeMetadata.TryGetValue(property, out var baseMetadata) || baseMetadata is null || baseMetadata.Unit is null)
            throw new InvalidOperationException($"No metadata found for property {property}.");

        var convertBack = UnitMetadataBasic.FromUnitInfo(baseMetadata.Unit.UnitInfo, baseMetadata.Unit.QuantityType.QuantityInfo, culture)!;
        var conversions = baseMetadata.Conversions.Append(convertBack);

        var metadata = baseMetadata.Clone(overrideProperty: property, overrideConversions: conversions, overrideUnit: toMetadata, overrideCulture: culture);

        _dynamicMetadata.AddOrUpdate(property, _ => metadata, (_, _) => metadata);
    }
}