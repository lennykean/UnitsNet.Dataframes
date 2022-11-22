using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using Humanizer;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public class QuantityTypeMetadata
    {
        public QuantityTypeMetadata(QuantityInfo quantityInfo, CultureInfo? culture = null)
        {
            Name = quantityInfo.Name;
            DisplayName = quantityInfo.Name.Humanize(LetterCasing.LowerCase);
            BaseUnit = SimpleCache<Enum, UnitMetadata>.Instance.GetOrAdd(quantityInfo.BaseUnitInfo.Value, () => new(quantityInfo.BaseUnitInfo, quantityInfo, culture));
        }

        public string Name { get; }
        public string DisplayName { get; }
        public UnitMetadata? BaseUnit { get; }
    }
}