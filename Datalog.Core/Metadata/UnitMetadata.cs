using System;
using System.Globalization;
using System.Linq;

using Humanizer;

using UnitsNet;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public class UnitMetadata
    {
        public UnitMetadata(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null)
        {
            Name = unitInfo.Name;
            Value = Convert.ToInt32(unitInfo.Value);
            DisplayName = unitInfo.PluralName.Humanize(LetterCasing.LowerCase);
            Abbriviation = UnitAbbreviationsCache.Default.GetUnitAbbreviations(quantityInfo.UnitType, Convert.ToInt32(unitInfo.Value), culture).FirstOrDefault() ?? "";
        }

        public string Name { get; }
        public int Value { get; }
        public string DisplayName { get; }
        public string Abbriviation { get; }
    }
}