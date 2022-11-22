using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(QuantityAttribute metadataAttribute, string name, CultureInfo? culture = null, params Enum[] allowedConversions)
        {
            UnitInfo = metadataAttribute.UnitInfo;
            QuantityInfo = metadataAttribute.QuantityInfo;
            Name = name;
            Unit = UnitInfo == null || QuantityInfo == null ? null : SimpleCache<Enum, UnitMetadataFull>.Instance.GetOrAdd(UnitInfo.Value, () => new UnitMetadataFull(UnitInfo, QuantityInfo, culture));
            Conversions = new Dictionary<int, UnitMetadata>(
                from u in metadataAttribute.QuantityInfo?.UnitInfos ?? Enumerable.Empty<UnitInfo>()
                where allowedConversions.Length == 0 || allowedConversions.Contains(u.Value) || metadataAttribute.UnitInfo!.Value == u.Value
                select new KeyValuePair<int, UnitMetadata>(
                    key: Convert.ToInt32(u.Value), 
                    value: SimpleCache<Enum, UnitMetadata>.Instance.GetOrAdd(u.Value, () => new(u, metadataAttribute.QuantityInfo!, culture))));
        }

        [JsonIgnore]
        public UnitInfo? UnitInfo { get; }
        [JsonIgnore]
        public QuantityInfo? QuantityInfo { get; }
        public string Name { get; }
        public UnitMetadataFull? Unit { get; }
        public IReadOnlyDictionary<int, UnitMetadata> Conversions { get; }
    }
}