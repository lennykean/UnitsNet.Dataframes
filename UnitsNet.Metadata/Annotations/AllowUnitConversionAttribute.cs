using System;

namespace UnitsNet.Metadata.Annotations;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AllowUnitConversionAttribute : Attribute
{
    private readonly Lazy<Enum?> _lazyUnit;
    private readonly Lazy<Type?> _lazyQuantityType;
    private readonly Lazy<UnitInfo?> _lazyUnitInfo;
    private readonly Lazy<QuantityInfo?> _lazyQuantityInfo;

    public AllowUnitConversionAttribute(object unit, Type? quantityType = null)
    {
        _lazyUnit = new(() =>
        {
            return unit as Enum;
        });
        _lazyUnitInfo = new(() =>
        {
            if (Unit?.TryGetUnitInfo(QuantityType, out var unitInfo) is not true)
                return default;

            return unitInfo!;
        });
        _lazyQuantityInfo = new(() =>
        {
            if (Unit?.TryGetQuantityInfo(QuantityType, out var quantityInfo) is not true)
                return default;

            return quantityInfo!;
        });
        _lazyQuantityType = new(() =>
        {
            if (quantityType is not null)
                return quantityType;

            if (Unit?.TryGetQuantityInfo(null, out var quantityInfo) is not true)
                return default;

            return quantityInfo?.ValueType;
        });
    }

    public Enum? Unit => _lazyUnit.Value;

    public Type? QuantityType => _lazyQuantityType.Value;
    public QuantityInfo? QuantityInfo => _lazyQuantityInfo.Value;
    public UnitInfo? UnitInfo => _lazyUnitInfo.Value;

    protected internal virtual void Validate()
    {
        if (Unit is null)
            throw new ArgumentException($"{nameof(Unit)} must be an enum value");
        if (!Unit.TryGetUnitInfo(QuantityType, out _))
            throw new ArgumentException($"{Unit?.GetType()}.{Unit} is not a known unit value.");
        if (!Unit.TryGetQuantityInfo(QuantityType, out _))
            throw new ArgumentException($"{Unit?.GetType()} is not a known unit type.");
    }
}