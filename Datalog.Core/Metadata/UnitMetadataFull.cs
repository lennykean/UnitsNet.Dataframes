using System;
using System.Globalization;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class UnitMetadataFull : UnitMetadata
    {
        public UnitMetadataFull(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null) : base(unitInfo, quantityInfo, culture)
        {
            QuantityType = MetadataCache<Type, QuantityTypeMetadata>.Instance.GetOrCreate(quantityInfo.ValueType, () => new QuantityTypeMetadata(quantityInfo, culture));
        }

        public QuantityTypeMetadata? QuantityType { get; }
    }
}