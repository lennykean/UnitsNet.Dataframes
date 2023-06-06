using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Dataframes.Utils;

namespace UnitsNet.Dataframes.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class QuantityAttribute : Attribute, DataframeMetadata.IMetadataAttribute
{
    private readonly Lazy<Type?>? _lazyQuantityType;
    private readonly Lazy<UnitInfo>? _lazyUnitInfo;
    private readonly Lazy<QuantityInfo>? _lazyQuantityInfo;

    public QuantityAttribute(object? unit = null, Type? quantityType = null)
    {
        if (unit is null)
            return;
        if (unit is not Enum unitValue)
            throw new ArgumentException($"{nameof(unit)} must be an enum value");

        Unit = unitValue;

        _lazyUnitInfo = new(() =>
        {
            if (!Unit.TryGetUnitInfo(quantityType, out var unitInfo))
                throw new ArgumentException($"{Unit?.GetType()}.{Unit} is not a known unit value.");

            return unitInfo!;
        });
        _lazyQuantityInfo = new(() =>
        {
            if (!Unit.TryGetQuantityInfo(quantityType, out var quantityInfo))
                throw new ArgumentException($"{Unit?.GetType()} is not a known unit type.");

            return quantityInfo!;
        });
        _lazyQuantityType = new(() =>
        {
            if (quantityType is not null)
                return quantityType;

            Unit.TryGetQuantityInfo(quantityType, out var quantityInfo);

            return quantityInfo?.ValueType;
        });
    }

    public Enum? Unit { get; }

    public Type? QuantityType => _lazyQuantityType?.Value;
    public QuantityInfo? QuantityInfo => _lazyQuantityInfo?.Value;
    public UnitInfo? UnitInfo => _lazyUnitInfo?.Value;

    public IEnumerable<UnitMetadataBasic> GetAllowedConversionsMetadata(IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
    {
        var conversions = GetConversionsIterator(allowedConversions, culture);

        return conversions.Distinct(new DelegateEqualityComparer<UnitMetadataBasic>((a, b) => a.UnitInfo.Value.Equals(b.UnitInfo.Value)));
    }

    private IEnumerable<UnitMetadataBasic> GetConversionsIterator(IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
    {
        if (UnitInfo is null || QuantityInfo is null)
            yield break;

        // Quantities are always convertable to their own unit.
        yield return UnitMetadataBasic.FromUnitInfo(UnitInfo, QuantityInfo, culture)!;

        if (allowedConversions.Any())
        {
            foreach (var conversion in allowedConversions)
            {
                var (unitInfo, quantityInfo) = GetConversionInfo(conversion);
                var unitMetadata = UnitMetadataBasic.FromUnitInfo(unitInfo, quantityInfo, culture);
                if (unitMetadata is not null)
                    yield return unitMetadata;
            }
        }
        else
        {
            foreach (var unit in QuantityInfo.UnitInfos)
            {
                var unitMetadata = UnitMetadataBasic.FromUnitInfo(unit, QuantityInfo, culture);
                if (unitMetadata is not null)
                    yield return unitMetadata;
            }
        }
    }

    private (UnitInfo unitInfo, QuantityInfo quantityInfo) GetConversionInfo(AllowUnitConversionAttribute allowedConversion)
    {
        var unitInfo = allowedConversion.UnitInfo;
        var quantityInfo = allowedConversion.QuantityInfo;

        if (unitInfo is not null && quantityInfo is not null)
            return (unitInfo, quantityInfo);

        if (quantityInfo is null && QuantityInfo?.UnitInfos.Any(u => u.Value.Equals(Unit)) == true)
            quantityInfo = QuantityInfo;
        if (quantityInfo is null || unitInfo is null && Unit?.TryGetUnitInfo(quantityInfo.ValueType, out unitInfo) is not true)
            throw new InvalidOperationException($"{Unit?.GetType().Name} is not a known unit type.");

        return (unitInfo!, quantityInfo);
    }

    QuantityMetadata DataframeMetadata<QuantityAttribute, QuantityMetadata>.IMetadataAttribute.ToMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> conversions, UnitMetadata? overrideUnit, CultureInfo? culture)
    {
        var unit = overrideUnit ?? UnitMetadata.FromUnitInfo(
            UnitInfo ?? throw new InvalidOperationException(),
            QuantityInfo ?? throw new InvalidOperationException(),
            culture);

        return new(property, unit, conversions.ToList());
    }
}