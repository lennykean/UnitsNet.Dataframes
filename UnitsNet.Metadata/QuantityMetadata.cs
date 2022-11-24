using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(string name, UnitMetadata? unit, Dictionary<int, UnitMetadataBasic> conversions)
        {
            Name = name;
            Unit = unit;
            Conversions = new(conversions);
        }

        public string Name { get; }
        public UnitMetadata? Unit { get; }
        public ReadOnlyDictionary<int, UnitMetadataBasic> Conversions { get; }

        public static QuantityMetadata FromQuantityAttribute(QuantityAttribute metadataAttribute, string name, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
        {
            if (metadataAttribute is null)
                throw new ArgumentNullException(nameof(metadataAttribute));

            var unitInfo = metadataAttribute.UnitInfo;
            var quantityInfo = metadataAttribute.QuantityInfo;
            var unit = unitInfo is null || quantityInfo is null ? null : UnitMetadata.FromUnitInfo(unitInfo, quantityInfo, culture);

            return new(name, unit, new(GetConversions(metadataAttribute, allowedConversions, culture)));
        }

        protected static IEnumerable<KeyValuePair<int, UnitMetadataBasic>> GetConversions(QuantityAttribute metadataAttribute, IEnumerable<AllowUnitConversionAttribute> allowedConversions,  CultureInfo? culture = null)
        {
            if (metadataAttribute.UnitInfo is null || metadataAttribute.QuantityInfo is null)
                yield break;

            if (allowedConversions.Any())
            {
                foreach (var conversion in allowedConversions)
                {
                    var quantityInfo = conversion.QuantityInfo;
                    if (quantityInfo is null && metadataAttribute.QuantityInfo.UnitInfos.Any(u => u.Value.Equals(metadataAttribute.Unit)))
                        quantityInfo = metadataAttribute.QuantityInfo;

                    if (quantityInfo is null)
                        throw new ArgumentException($"{metadataAttribute.Unit?.GetType()} is not a known unit type.");

                    yield return new(Convert.ToInt32(conversion.Unit), UnitMetadataBasic.FromUnitInfo(conversion.UnitInfo, quantityInfo, culture));
                }
            }
            else
            {
                foreach (var unit in metadataAttribute.QuantityInfo.UnitInfos)
                {
                    yield return new(Convert.ToInt32(unit.Value), UnitMetadataBasic.FromUnitInfo(unit, metadataAttribute.QuantityInfo, culture));
                }
            }
        }
    }
}