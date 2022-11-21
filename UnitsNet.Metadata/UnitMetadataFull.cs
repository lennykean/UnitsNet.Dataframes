using System;
using System.Globalization;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public class UnitMetadataFull : UnitMetadata
    {
        public UnitMetadataFull(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null) : base(unitInfo, quantityInfo, culture)
        {
            QuantityType = SimpleCache<Type, QuantityTypeMetadata>.Instance.GetOrAdd(quantityInfo.ValueType, () => new QuantityTypeMetadata(quantityInfo, culture));
        }

        public QuantityTypeMetadata? QuantityType { get; }
    }
}