using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using Humanizer;

using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class QuantityTypeMetadataBase
    {
        private protected QuantityTypeMetadataBase(QuantityInfo quantityInfo, string name)
        {
            QuantityInfo = quantityInfo;
            Name = name;
        }

        [JsonIgnore, IgnoreDataMember]
        public QuantityInfo QuantityInfo { get; }

        public string Name { get; }
    }

    public sealed class QuantityTypeMetadataBasic : QuantityTypeMetadataBase
    {
        public QuantityTypeMetadataBasic(QuantityInfo quantityInfo, string name) : base(quantityInfo, name)
        {
        }

        public static QuantityTypeMetadataBasic FromQuantityInfo(QuantityInfo quantityInfo)
        {
            return SimpleCache<Type, QuantityTypeMetadataBasic>.Instance.GetOrAdd(quantityInfo.ValueType, _ => 
            {
                var name = quantityInfo.Name;

                return new(quantityInfo, name);
            });
        }
    }

    public class QuantityTypeMetadata : QuantityTypeMetadataBase
    {
        public QuantityTypeMetadata(QuantityInfo quantityInfo, string name, string displayName, UnitMetadataBasic? baseUnit) : base(quantityInfo, name)
        {
            DisplayName = displayName;
            BaseUnit = baseUnit;
        }

        public string DisplayName { get; }
        public UnitMetadataBasic? BaseUnit { get; }

        public static QuantityTypeMetadata FromQuantityInfo(QuantityInfo quantityInfo, CultureInfo? culture = null)
        {
            return SimpleCache<Type, QuantityTypeMetadata>.Instance.GetOrAdd(quantityInfo.ValueType, _ =>
            { 
                var name = quantityInfo.Name;
                var displayName = quantityInfo.Name.Humanize(LetterCasing.LowerCase);
                var baseUnit = UnitMetadataBasic.FromUnitInfo(quantityInfo.BaseUnitInfo, quantityInfo, culture);

                return new(quantityInfo, name, displayName, baseUnit);
            });
        }
    }
}