using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class SensorMetadata
    {
        public SensorMetadata(SensorMetadataAttribute metadataAttribute, CultureInfo? culture = null)
        {
            DisplayName = metadataAttribute.DisplayName;
            Description = metadataAttribute.Description;
            UnitInfo = metadataAttribute.UnitInfo;
            QuantityInfo = metadataAttribute.QuantityInfo;
            Unit = UnitInfo == null || QuantityInfo == null ? null : MetadataCache.Instance.GetOrCreate(UnitInfo.Value, () => new UnitMetadataFull(UnitInfo, QuantityInfo, culture));
        }

        public string DisplayName { get; }
        public string? Description { get; }
        [JsonIgnore]
        public UnitInfo? UnitInfo { get; }
        [JsonIgnore]
        public QuantityInfo? QuantityInfo { get; }
        public UnitMetadataFull? Unit { get; }
    }
}