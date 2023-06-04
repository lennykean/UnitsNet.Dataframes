using System;

namespace UnitsNet.Dataframes.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class AllowUnitConversionAttribute : Attribute
{
    private readonly Lazy<Type?> _lazyQuantityType;
    private readonly Lazy<UnitInfo> _lazyUnitInfo;
    private readonly Lazy<QuantityInfo> _lazyQuantityInfo;

    public AllowUnitConversionAttribute(object unit, Type? quantityType = null)
    {
        if (unit is not Enum unitValue)
            throw new ArgumentException($"{nameof(unit)} must be an enum value");

        Unit = unitValue;

        _lazyUnitInfo = new(() =>
        {
            Unit.TryGetUnitInfo(QuantityType, out var unitInfo);
            return unitInfo!;
        });
        _lazyQuantityInfo = new(() =>
        {
            Unit.TryGetQuantityInfo(QuantityType, out var quantityInfo);
            return quantityInfo!;
        });
        _lazyQuantityType = new(() =>
        {
            if (quantityType is not null)
                return quantityType;

            Unit.TryGetQuantityInfo(null, out var quantityInfo);

            return quantityInfo?.ValueType;
        });
    }

    public Enum Unit { get; }

    public Type? QuantityType => _lazyQuantityType.Value;
    public QuantityInfo? QuantityInfo => _lazyQuantityInfo.Value;
    public UnitInfo? UnitInfo => _lazyUnitInfo.Value;
}