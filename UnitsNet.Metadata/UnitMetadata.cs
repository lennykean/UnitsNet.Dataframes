using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Humanizer;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class UnitMetadataBase
    {
        private protected UnitMetadataBase(UnitInfo unitInfo, string name, int value, string displayName, string abbriviation)
        {
            UnitInfo = unitInfo;
            Name = name;
            Value = value;
            DisplayName = displayName;
            Abbriviation = abbriviation;
        }

        [JsonIgnore, IgnoreDataMember]
        public UnitInfo UnitInfo { get; }

        public string Name { get; }
        public int Value { get; }
        public string DisplayName { get; }
        public string Abbriviation { get; }

        private protected static string GetDisplayName(UnitInfo unitInfo)
        {
            return unitInfo.PluralName.Humanize(LetterCasing.LowerCase);
        }

        private protected static string GetAbbriviation(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture)
        {
            return UnitAbbreviationsCache.Default.GetUnitAbbreviations(quantityInfo.UnitType, Convert.ToInt32(unitInfo.Value), culture).FirstOrDefault() ?? "";
        }
    }

    public sealed class UnitMetadataBasic : UnitMetadataBase
    {
        public UnitMetadataBasic(UnitInfo unitInfo, string name, int value, string displayName, string abbriviation, QuantityTypeMetadataBasic quantityType) 
            : base(unitInfo, name, value, displayName, abbriviation)
        {
            QuantityType = quantityType;
        }

        public QuantityTypeMetadataBasic QuantityType { get; }

        public static UnitMetadataBasic FromUnitInfo(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null)
        {
            return SimpleCache<Enum, UnitMetadataBasic>.Instance.GetOrAdd(unitInfo.Value, _ =>
            {
                var name = unitInfo.Name;
                var value = Convert.ToInt32(unitInfo.Value);
                var displayName = GetDisplayName(unitInfo);
                var abbriviation = GetAbbriviation(unitInfo, quantityInfo, culture);
                var quantityType = QuantityTypeMetadataBasic.FromQuantityInfo(quantityInfo);

                return new(unitInfo, name, value, displayName, abbriviation, quantityType);
            });
        }
    }

    public sealed class UnitMetadata : UnitMetadataBase
    {
        public UnitMetadata(UnitInfo unitInfo, string name, int value, string displayName, string abbriviation, QuantityTypeMetadata quantityType) 
            : base(unitInfo, name, value, displayName, abbriviation)
        {
            QuantityType = quantityType;
        }

        public QuantityTypeMetadata QuantityType { get; }

        public static UnitMetadata FromUnitInfo(UnitInfo unitInfo, QuantityInfo quantityInfo, CultureInfo? culture = null)
        {
            return SimpleCache<Enum, UnitMetadata>.Instance.GetOrAdd(quantityInfo.BaseUnitInfo.Value, _ =>
            {
                var value = Convert.ToInt32(unitInfo.Value);
                var displayName = GetDisplayName(unitInfo);
                var abbriviation = GetAbbriviation(unitInfo, quantityInfo, culture);
                var quantityType = QuantityTypeMetadata.FromQuantityInfo(quantityInfo, culture);

                return new(unitInfo, unitInfo.Name, value, displayName, abbriviation, quantityType);
            });
        }
    }
}