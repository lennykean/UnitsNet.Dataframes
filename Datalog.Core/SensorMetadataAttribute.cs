using System;
using System.Linq;

using UnitsNet;

namespace HondataDotNet.Datalog.Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class SensorMetadataAttribute : Attribute
    {
        private readonly Lazy<QuantityInfo?> _lazyQuantityInfo;
        private readonly Lazy<UnitInfo?> _lazyUnitInfo;

        public SensorMetadataAttribute(string displayName)
        {
            DisplayName = displayName;
            _lazyQuantityInfo = new(() => Quantity.Infos.SingleOrDefault(i => i.Name == QuantityType));
            _lazyUnitInfo = new(() => QuantityInfo == null || Unit == null ? null : Quantity.GetUnitInfo(UnitParser.Default.Parse(Unit, QuantityInfo.UnitType)));
        }

        public string? DisplayName { get; }
        public string? QuantityType { get; init; }
        public string? Unit { get; init; }
        public string? Description { get; init; }

        public QuantityInfo? QuantityInfo => _lazyQuantityInfo.Value;
        public UnitInfo? UnitInfo => _lazyUnitInfo.Value;
    }
}
