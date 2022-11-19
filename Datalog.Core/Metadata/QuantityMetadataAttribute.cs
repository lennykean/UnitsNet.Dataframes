using System;
using System.Linq;
using System.Reflection;

using HondataDotNet.Datalog.Core.Units;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QuantityMetadataAttribute : Attribute
    {
        private readonly Lazy<UnitInfo>? _lazyUnitInfo;
        private readonly Lazy<QuantityInfo>? _lazyQuantityInfo;

        public QuantityMetadataAttribute(object? unit = null)
        {
            if (unit is null)
                return;
            if (unit is not Enum unitValue)
                throw new ArgumentException($"{nameof(unit)} must be an enum value");

            Unit = unitValue;

            _lazyUnitInfo = new(() =>
            {
                if (!Unit.TryGetUnitInfo(out var unitInfo, out _))
                    throw new ArgumentException($"{Unit.GetType()}.{Unit} is not a known unit value.");

                return unitInfo!;
            });
            _lazyQuantityInfo = new(() =>
            {
                if (!Unit.TryGetUnitInfo(out _, out var quantityInfo))
                    throw new ArgumentException($"{Unit.GetType()} is not a known unit type.");

                return quantityInfo!;
            });
        }

        public Enum? Unit { get; }
        public QuantityInfo? QuantityInfo => _lazyQuantityInfo?.Value;
        public UnitInfo? UnitInfo => _lazyUnitInfo?.Value;
    }
}