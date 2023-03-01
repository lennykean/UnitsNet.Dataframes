using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet.Metadata;
using UnitsNet.Metadata.Annotations;

namespace HondataDotNet.Datalog.Core.Metadata
{

    public class SensorMetadata : QuantityMetadata
    {
        public SensorMetadata(PropertyInfo property, UnitMetadata? unit, IEnumerable<UnitMetadataBasic> conversions, string displayName, string? description) : base(property, unit, conversions.ToList())
        {
            DisplayName = displayName;
            Description = description;
        }

        public string DisplayName { get; }
        public string? Description { get; }

        internal static SensorMetadata FromSensorAttribute(SensorAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture)
        {
            if (metadataAttribute is null)
                throw new ArgumentNullException(nameof(metadataAttribute));

            var unitInfo = metadataAttribute.UnitInfo;
            var quantityInfo = metadataAttribute.QuantityInfo;
            var conversions = GetConversions(metadataAttribute, allowedConversions, culture);
            var unit = unitInfo is null || quantityInfo is null ? null : UnitMetadata.FromUnitInfo(unitInfo, quantityInfo, culture);

            return new(property, unit, conversions, metadataAttribute.DisplayName, metadataAttribute.Description);
        }
    }
}