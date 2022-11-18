using System;
using System.Linq;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SensorMetadataAttribute : Attribute
    {
        private readonly Lazy<UnitInfo>? _lazyUnitInfo;
        private readonly Lazy<QuantityInfo>? _lazyQuantityInfo;

        public SensorMetadataAttribute(string displayName, object? unit = null)
        {
            DisplayName = displayName;

            if (unit is null)
                return;
            if (unit is not Enum unitValue)
                throw new ArgumentException($"{nameof(unit)} must be an enum value");

            Unit = unitValue;

            _lazyUnitInfo = new(() => 
            {
                if (!Quantity.TryGetUnitInfo(Unit, out var unitInfo))
                    throw new ArgumentException($"{Unit.GetType()}.{Unit} is not a known unit enum value.");

                return unitInfo;
            });
            _lazyQuantityInfo = new(() =>
            {
                var quantityInfo = Quantity.Infos.SingleOrDefault(q => q.UnitType == Unit.GetType());

                return  quantityInfo ?? throw new ArgumentException($"{Unit.GetType()} is not a known unit enum type.");
            });
        }

        public string DisplayName { get; }
        public Enum? Unit { get; }
        public string? Description { get; set; }
        public QuantityInfo? QuantityInfo => _lazyQuantityInfo?.Value;
        public UnitInfo? UnitInfo => _lazyUnitInfo?.Value;
    }
}
