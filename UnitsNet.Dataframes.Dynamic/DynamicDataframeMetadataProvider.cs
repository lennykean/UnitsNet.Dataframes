using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata> : IDataframeMetadataProvider<TDataframe, TMetadataAttribute, TMetadata>
    where TDataframe : class
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadataAttribute
    where TMetadata : QuantityMetadata, DataframeMetadata<TMetadataAttribute, TMetadata>.IDataframeMetadata
{
    private readonly IDataframeMetadataProvider<TMetadataAttribute, TMetadata> _baseDataframeMetadataProvider;
    private readonly ConcurrentDictionary<PropertyInfo, TMetadata> _dynamicMetadata;

    public DynamicDataframeMetadataProvider(
        IDataframeMetadataProvider<TMetadataAttribute, TMetadata> baseDataframeMetadataProvider,
        IEnumerable<TMetadata>? dynamicMetadata = null)
    {
        _baseDataframeMetadataProvider = baseDataframeMetadataProvider;
        _dynamicMetadata = new(dynamicMetadata?.ToDictionary(k => k.Property, v => v) ?? new());
    }

    public DynamicDataframeMetadataProvider<THoistedDataframe, TMetadataAttribute, TMetadata> HoistMetadata<THoistedDataframe>(CultureInfo? culture = null)
        where THoistedDataframe : class
    {
        var dynamicMetadatas =
            from metadata in _dynamicMetadata.Values
            let property = metadata.Property.TryGetMappedProperty(typeof(THoistedDataframe), out var mappedProperty) ? mappedProperty : metadata.Property
            select metadata.Clone(overrideProperty: property, overrideCulture: culture);

        return new(this, dynamicMetadatas);
    }

    public IEnumerable<TMetadata> GetAllMetadata(CultureInfo? culture = null)
    {
        return
            from p in typeof(TDataframe).GetProperties(inherit: true).Distinct()
            let m = (hasMetadata: TryGetMetadata(p, out var metadata, culture), metadata)
            where m.hasMetadata
            select m.metadata;
    }

    public IEnumerable<TMetadata> GetAllBaseMetadata()
    {
        return _baseDataframeMetadataProvider.GetAllMetadata();
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata, CultureInfo? culture = null)
    {
        if (property is null)
            throw new ArgumentNullException(nameof(property));

        if (_dynamicMetadata.TryGetValue(property, out metadata) && metadata.Unit is not null)
            return true;

        if (_baseDataframeMetadataProvider.TryGetMetadata(property, out metadata) && metadata?.Unit is not null)
            return true;

        return false;
    }

    public bool TryGetBaseMetadata(PropertyInfo property, [NotNullWhen(true)] out TMetadata? metadata)
    {
        return _baseDataframeMetadataProvider.TryGetMetadata(property, out metadata);
    }

    public void AddConversion(PropertyInfo property, Enum unit, CultureInfo? culture = null)
    {
        var (_, toMetadata) = property.GetConversionMetadatas(to: unit, this, culture);

        if (!_baseDataframeMetadataProvider.TryGetMetadata(property, out var baseMetadata) || baseMetadata is null || baseMetadata.Unit is null)
            throw new InvalidOperationException($"No metadata found for property {property}.");

        var convertBack = UnitMetadataBasic.FromUnitInfo(baseMetadata.Unit.UnitInfo, baseMetadata.Unit.QuantityType.QuantityInfo, culture)!;
        var conversions = baseMetadata.Conversions.Append(convertBack);

        var metadata = baseMetadata.Clone(overrideProperty: property, overrideConversions: conversions, overrideUnit: toMetadata, overrideCulture: culture);

        _dynamicMetadata.AddOrUpdate(property, _ => metadata, (_, _) => metadata);
    }

    public void ValidateAllMetadata()
    {
        foreach (var metadata in GetAllMetadata())
        {
            metadata.Validate();

            if (_baseDataframeMetadataProvider.TryGetMetadata(metadata.Property, out var baseMetadata) &&
                metadata.Unit is not null && metadata.Unit.UnitInfo != baseMetadata.Unit?.UnitInfo &&
                metadata.Property.GetMethod is not null && !metadata.Property.GetMethod.IsAbstract && !metadata.Property.GetMethod.IsVirtual &&
                metadata.Property.SetMethod is not null && !metadata.Property.SetMethod.IsAbstract && !metadata.Property.SetMethod.IsVirtual)
                throw new InvalidOperationException($"{typeof(TDataframe).Name}.{metadata.Property.Name} is non-virtual and cannot converted to {metadata.Unit.Name}");
        }
    }
}