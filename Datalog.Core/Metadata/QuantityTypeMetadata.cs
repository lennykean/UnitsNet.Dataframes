using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HondataDotNet.Datalog.Core.Utils;
using Humanizer;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class QuantityTypeMetadata
    {
        public QuantityTypeMetadata(QuantityInfo quantityInfo, CultureInfo? culture = null)
        {
            Name = quantityInfo.Name;
            DisplayName = quantityInfo.Name.Humanize(LetterCasing.LowerCase);
            BaseUnit = SimpleCache<Enum, UnitMetadata>.Instance.GetOrAdd(quantityInfo.BaseUnitInfo.Value, () => new(quantityInfo.BaseUnitInfo, quantityInfo, culture));
            Units = quantityInfo.UnitInfos.ToDictionary(
                k => Convert.ToInt32(k.Value), 
                v => SimpleCache<Enum, UnitMetadata>.Instance.GetOrAdd(v.Value, () => new(v, quantityInfo, culture))!);
        }

        public string Name { get; }
        public string DisplayName { get; }
        public UnitMetadata? BaseUnit { get; }
        public IReadOnlyDictionary<int, UnitMetadata> Units { get; }
    }
}