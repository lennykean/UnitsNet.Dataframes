using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicMetadataProvider<TMetadataAttribute, TMetadata> : IMetadataProvider<TMetadata>
    where TMetadataAttribute : QuantityAttribute, DataframeMetadata<TMetadataAttribute, TMetadata>.IMetadataFactory
    where TMetadata : QuantityMetadata
{
    private readonly Type _dataframeType;
    private readonly DataframeMetadata<TMetadataAttribute, TMetadata> _baseMetadata;
    private readonly ConcurrentDictionary<PropertyInfo, TMetadata> _dynamicMetadata;

    public DynamicMetadataProvider(
        Type dataframeType,
        IEnumerable<TMetadata> baseMetadata,
        IEnumerable<KeyValuePair<PropertyInfo, TMetadata>>? conversions = null)
    {
        _dataframeType = dataframeType;
        _baseMetadata = new(baseMetadata);
        _dynamicMetadata = new(conversions);
    }


    public DynamicMetadataProvider<TMetadataAttribute, TMetadata> HoistMetadata(Type other)
    {
        throw new NotImplementedException();
        // return new(_dataframeType, _baseMetadata);
            //_dataframeType,
            //_baseMetadata
            //_dynamicOverrides.ToDictionary(
            //    k => k.Key.TryGetMappedProperty(other, out var mappedProperty) ? mappedProperty : k.Key,
            //    v => v.Value));
    }

    public IEnumerable<TMetadata> GetAllMetadata<TDataframe>()
    {
        return
            from p in typeof(TDataframe).GetProperties(inherit: true).Distinct()
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

        if (!MetadataBuilder.TryBuildMetadata<TMetadataAttribute, TMetadata>(property, out var staticMetadataAttribute, out var staticMetadata) || staticMetadata.Unit is null)
            return false;
        
        metadata = _dynamicMetadata.GetOrAdd(property, key =>
        {
            if (!_dynamicMetadata.TryGetValue(key, out var conversionAttribute) || conversionAttribute.Unit?.UnitInfo is null || conversionAttribute.Unit.QuantityType.QuantityInfo is null)
                return staticMetadata;

            var convertBack = UnitMetadataBasic.FromUnitInfo(conversionAttribute.Unit.UnitInfo, conversionAttribute.Unit.QuantityType.QuantityInfo)!;
            var allowedConversions = staticMetadata.Conversions.Append(convertBack);

            return staticMetadataAttribute.CreateMetadata(property, allowedConversions, culture);
        });
        return true;
    }

    public void AddConversion(PropertyInfo property, Enum unit)
    {
        var (fromMetadata, toMetadata) = property.GetConversionMetadata<TMetadataAttribute, TMetadata>(unit);
        var quantityAttribute = new QuantityAttribute(unit, fromMetadata.QuantityType.QuantityInfo.ValueType);

        _dynamicMetadata.AddOrUpdate(property, _ => quantityAttribute, (_, _) => quantityAttribute);
    }
}