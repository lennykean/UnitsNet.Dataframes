using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using Humanizer;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(string name, UnitMetadata? unit, IList<UnitMetadataBasic> conversions)
        {
            Name = name;
            Unit = unit;
            Conversions = new(conversions);
        }

        public string Name { get; }
        public UnitMetadata? Unit { get; }
        public ReadOnlyCollection<UnitMetadataBasic> Conversions { get; }

        public static QuantityMetadata FromQuantityAttribute(QuantityAttribute metadataAttribute, string name, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
        {
            if (metadataAttribute is null)
                throw new ArgumentNullException(nameof(metadataAttribute));

            var unitInfo = metadataAttribute.UnitInfo;
            var quantityInfo = metadataAttribute.QuantityInfo;
            var unit = unitInfo is null || quantityInfo is null ? null : UnitMetadata.FromUnitInfo(unitInfo, quantityInfo, culture);

            return new(name, unit, GetConversions(metadataAttribute, allowedConversions, culture).ToList());
        }

        protected static IEnumerable<UnitMetadataBasic> GetConversions(QuantityAttribute metadataAttribute, IEnumerable<AllowUnitConversionAttribute> allowedConversions,  CultureInfo? culture = null)
        {
            if (metadataAttribute.UnitInfo is null || metadataAttribute.QuantityInfo is null)
                yield break;

            if (allowedConversions.Any())
            {
                foreach (var conversion in allowedConversions)
                {
                    yield return UnitMetadataBasic.FromUnitInfo(conversion.UnitInfo, GetConversionQuantityInfo(conversion, metadataAttribute), culture);
                }
            }
            else
            {
                foreach (var unit in metadataAttribute.QuantityInfo.UnitInfos)
                {
                    yield return UnitMetadataBasic.FromUnitInfo(unit, metadataAttribute.QuantityInfo, culture);
                }
            }
        }

        private static QuantityInfo GetConversionQuantityInfo(AllowUnitConversionAttribute conversion, QuantityAttribute metadataAttribute)
        {
            var quantityInfo = conversion.QuantityInfo;
            if (quantityInfo is null && metadataAttribute.QuantityInfo?.UnitInfos.Any(u => u.Value.Equals(metadataAttribute.Unit)) == true)
                quantityInfo = metadataAttribute.QuantityInfo;
            if (quantityInfo is null)
                throw new ArgumentException($"{metadataAttribute.Unit?.GetType()} is not a known unit type.");

            return quantityInfo;
        }
    }
}