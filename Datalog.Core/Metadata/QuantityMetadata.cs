using System;
using System.Globalization;
using System.Text.Json.Serialization;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(QuantityMetadataAttribute metadataAttribute, CultureInfo? culture = null)
        {
            UnitInfo = metadataAttribute.UnitInfo;
            QuantityInfo = metadataAttribute.QuantityInfo;
            Unit = UnitInfo == null || QuantityInfo == null ? null : MetadataCache<Enum, UnitMetadataFull>.Instance.GetOrCreate(UnitInfo.Value, () => new UnitMetadataFull(UnitInfo, QuantityInfo, culture));
        }

        [JsonIgnore]
        public UnitInfo? UnitInfo { get; }
        [JsonIgnore]
        public QuantityInfo? QuantityInfo { get; }
        public UnitMetadataFull? Unit { get; }
    }
}