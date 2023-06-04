using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using UnitsNet.Dataframes.Attributes;
using UnitsNet.Dataframes.Reflection;

namespace UnitsNet.Dataframes.Dynamic;

internal class DynamicMetadataProvider : IMetadataProvider<QuantityMetadata>
{
    private readonly Type _concreteType;
    private readonly ConcurrentDictionary<PropertyInfo, QuantityAttribute> _conversions = new();
    private readonly ConcurrentDictionary<PropertyInfo, QuantityMetadata> _dynamicMetadata = new();

    public DynamicMetadataProvider(Type concreteType, Dictionary<PropertyInfo, QuantityAttribute>? conversions = null)
    {
        _concreteType = concreteType;
        ConcreteMetadata = new(_concreteType);
        _conversions = new(conversions ?? new());
    }

    public DataFrameMetadata ConcreteMetadata { get; }

    public DynamicMetadataProvider MapTo(Type other)
    {
        var interfacePropertyMap = _concreteType.GetBidirectionalInterfacePropertyMap();
        var otherProperties = other.GetProperties();

        return new(_concreteType, _conversions.ToDictionary(k => k.Key switch
        {
            _ when k.Key.DeclaringType == other
                => k.Key,
            _ when interfacePropertyMap.TryGetValue(k.Key, out var interfaceProperty) && interfaceProperty.DeclaringType?.IsAssignableFrom(other) is true
                => interfaceProperty,
            _ when k.Key.GetMethod?.IsVirtual is true || k.Key.GetMethod?.IsAbstract is true
                => otherProperties.SingleOrDefault(p => p.DeclaringType == k.Key.DeclaringType?.DeclaringType && p.Name == k.Key.Name) ?? k.Key,
            _ 
                => k.Key,
        }, v => v.Value));
    }

    public IEnumerable<QuantityMetadata> GetAllMetadata<TDataframe>()
    {
        return
            from p in typeof(TDataframe).GetProperties(inherit: true).Distinct()
            let m = (hasMetadata: TryGetMetadata(p, out var metadata), metadata)
            where m.hasMetadata
            select m.metadata;
    }

    public bool TryGetMetadata(PropertyInfo property, [NotNullWhen(true)]out QuantityMetadata? metadata)
    {
        if (property is not { DeclaringType: not null, GetMethod: not null })
            throw new ArgumentNullException(nameof(property));

        if (_dynamicMetadata.TryGetValue(property, out metadata))
            return true;

        var concreteProperty = property switch
        {
            _ when property.DeclaringType == _concreteType
                => property,
            _ when property.DeclaringType.IsInterface
                => _concreteType.GetBidirectionalInterfacePropertyMap()[property],
            _ when property.GetMethod.IsVirtual || property.GetMethod.IsAbstract
                => _concreteType.GetProperties().SingleOrDefault(p => p.Name == property.Name && p.DeclaringType == property.DeclaringType),
            _ => null,
        };
        if (concreteProperty is null || !ConcreteMetadata.TryGetValue(concreteProperty, out var concreteMetadata) || concreteMetadata.Unit is null)
            return false;
        
        metadata = _dynamicMetadata.GetOrAdd(property, key =>
        {
            if (!_conversions.TryGetValue(key, out var conversion))
                return concreteMetadata;

            var allowConvertBack = new AllowUnitConversionAttribute(concreteMetadata.Unit.UnitInfo.Value, concreteMetadata.Unit.QuantityType.QuantityInfo.ValueType);
            var allowedConversions = concreteProperty.GetCustomAttributes<AllowUnitConversionAttribute>().Append(allowConvertBack);
            
            return QuantityMetadata.FromQuantityAttribute(conversion, key, allowedConversions);
        });
        return true;
    }

    public void AddConversion(PropertyInfo property, Enum unit)
    {
        var (fromMetadata, toMetadata) = property.GetConversionMetadata(unit);
        var quantityAttribute = new QuantityAttribute(unit, fromMetadata.QuantityType.QuantityInfo.ValueType);

        _conversions.AddOrUpdate(property, _ => quantityAttribute, (_, _) => quantityAttribute);
    }
}