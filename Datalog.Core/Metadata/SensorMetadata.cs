using System;
using System.Collections.Generic;
using System.Globalization;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet;
using UnitsNet.Metadata;
using UnitsNet.Metadata.Annotations;

namespace HondataDotNet.Datalog.Core.Metadata
{

    public class SensorMetadata : QuantityMetadata
    {
        public SensorMetadata(string name, UnitMetadata? unit, Dictionary<int, UnitMetadataBasic> conversions, string displayName, string? description) : base(name, unit, conversions)
        {
            DisplayName = displayName;
            Description = description;
        }

        public string DisplayName { get; }
        public string? Description { get; }

        internal static SensorMetadata FromSensorAttribute(SensorAttribute metadataAttribute, string name, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture)
        {
            if (metadataAttribute is null)
                throw new ArgumentNullException(nameof(metadataAttribute));

            var unitInfo = metadataAttribute.UnitInfo;
            var quantityInfo = metadataAttribute.QuantityInfo;
            var conversions = GetConversions(metadataAttribute, allowedConversions, culture);
            var unit = unitInfo is null || quantityInfo is null ? null : UnitMetadata.FromUnitInfo(unitInfo, quantityInfo, culture);

            return new(name, unit, new(conversions), metadataAttribute.DisplayName, metadataAttribute.Description);
        }
    }
}