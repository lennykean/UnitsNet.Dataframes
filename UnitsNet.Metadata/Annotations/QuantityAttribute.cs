using System;

namespace UnitsNet.Metadata.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QuantityAttribute : Attribute
    {
        private readonly Lazy<UnitInfo>? _lazyUnitInfo;
        private readonly Lazy<QuantityInfo>? _lazyQuantityInfo;

        public QuantityAttribute(object? unit = null)
        {
            if (unit is null)
                return;
            if (unit is not Enum unitValue)
                throw new ArgumentException($"{nameof(unit)} must be an enum value");

            Unit = unitValue;

            _lazyUnitInfo = new(() =>
            {
                if (!Unit.TryGetUnitInfo(QuantityType, out var unitInfo))
                    throw new ArgumentException($"{Unit.GetType()}.{Unit} is not a known unit value.");

                return unitInfo!;
            });
            _lazyQuantityInfo = new(() =>
            {
                if (!Unit.TryGetQuantityInfo(QuantityType, out var quantityInfo))
                    throw new ArgumentException($"{Unit.GetType()} is not a known unit type.");

                return quantityInfo!;
            });
        }

        public Enum? Unit { get; }
        public Type? QuantityType { get; set; }

        public QuantityInfo? QuantityInfo => _lazyQuantityInfo?.Value;
        public UnitInfo? UnitInfo => _lazyUnitInfo?.Value;
    }
}