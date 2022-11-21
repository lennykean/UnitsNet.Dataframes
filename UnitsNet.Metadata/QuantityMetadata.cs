using System;
using System.Globalization;
using System.Text.Json.Serialization;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(QuantityAttribute metadataAttribute, CultureInfo? culture = null)
        {
            UnitInfo = metadataAttribute.UnitInfo;
            QuantityInfo = metadataAttribute.QuantityInfo;
            Unit = UnitInfo == null || QuantityInfo == null ? null : SimpleCache<Enum, UnitMetadataFull>.Instance.GetOrAdd(UnitInfo.Value, () => new UnitMetadataFull(UnitInfo, QuantityInfo, culture));
        }

        [JsonIgnore]
        public UnitInfo? UnitInfo { get; }
        [JsonIgnore]
        public QuantityInfo? QuantityInfo { get; }
        public UnitMetadataFull? Unit { get; }
    }
}