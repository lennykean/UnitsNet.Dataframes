using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata.Annotations;
using UnitsNet.Metadata.Utils;

namespace UnitsNet.Metadata
{
    public class QuantityMetadata
    {
        public QuantityMetadata(PropertyInfo property, UnitMetadata? unit, IList<UnitMetadataBasic> conversions)
        {
            Property = property;
            Unit = unit;
            Conversions = new(conversions);
        }

        public PropertyInfo Property { get; }
        public UnitMetadata? Unit { get; }
        public ReadOnlyCollection<UnitMetadataBasic> Conversions { get; }

        public static QuantityMetadata FromQuantityAttribute(QuantityAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
        {
            if (metadataAttribute is null)
                throw new ArgumentNullException(nameof(metadataAttribute));

            var unitInfo = metadataAttribute.UnitInfo;
            var quantityInfo = metadataAttribute.QuantityInfo;
            var unit = unitInfo is null || quantityInfo is null ? null : UnitMetadata.FromUnitInfo(unitInfo, quantityInfo, culture);

            return new(property, unit, GetConversions(metadataAttribute, allowedConversions, culture).ToList());
        }

        protected static IEnumerable<UnitMetadataBasic> GetConversions(QuantityAttribute metadataAttribute, IEnumerable<AllowUnitConversionAttribute> allowedConversions,  CultureInfo? culture = null)
        {
            var conversions = GetConversionsIterator(metadataAttribute, allowedConversions, culture);

            return conversions.Distinct(new DelegateEqualityComparer<UnitMetadataBasic>((a, b) => a.UnitInfo.Value.Equals(b.UnitInfo.Value)));
        }

        private static IEnumerable<UnitMetadataBasic> GetConversionsIterator(QuantityAttribute metadataAttribute, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
        {
            if (metadataAttribute.UnitInfo is null || metadataAttribute.QuantityInfo is null)
                yield break;

            // Quantities are always convertable to their own unit. 
            yield return UnitMetadataBasic.FromUnitInfo(metadataAttribute.UnitInfo, metadataAttribute.QuantityInfo, culture);

            if (allowedConversions.Any())
            {
                foreach (var conversion in allowedConversions)
                {
                    var (unitInfo, quantityInfo) = GetConversionInfo(conversion, metadataAttribute);
                    yield return UnitMetadataBasic.FromUnitInfo(unitInfo, quantityInfo, culture);
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

        private static (UnitInfo unitInfo, QuantityInfo quantityInfo) GetConversionInfo(AllowUnitConversionAttribute conversion, QuantityAttribute metadataAttribute)
        {
            var unitInfo = conversion.UnitInfo;
            var quantityInfo = conversion.QuantityInfo;

            if (unitInfo is not null && quantityInfo is not null)
                return (unitInfo, quantityInfo);

            if (quantityInfo is null && metadataAttribute.QuantityInfo?.UnitInfos.Any(u => u.Value.Equals(metadataAttribute.Unit)) == true)
                quantityInfo = metadataAttribute.QuantityInfo;
            if (quantityInfo is null || (unitInfo is null && !conversion.Unit.TryGetUnitInfo(quantityInfo.ValueType, out unitInfo)))
                throw new InvalidOperationException($"{metadataAttribute.Unit?.GetType().Name} is not a known unit type.");

            return (unitInfo!, quantityInfo);
        }
    }
}