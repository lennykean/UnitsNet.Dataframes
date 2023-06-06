using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace UnitsNet.Dataframes.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class QuantityAttribute : Attribute, DataFrameMetadata.IMetadataFactory
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

    QuantityMetadata DataFrameMetadata.IMetadataFactory.CreateMetadata(
        PropertyInfo property,
        IEnumerable<UnitMetadataBasic> allowedConversions,
        CultureInfo? culture,
        UnitMetadata? overrideUnit)
    {
        return QuantityMetadata.FromQuantityAttribute(property, this, allowedConversions, culture, overrideUnit);
    }
}