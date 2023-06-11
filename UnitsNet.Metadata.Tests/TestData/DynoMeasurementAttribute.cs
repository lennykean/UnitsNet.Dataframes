using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using UnitsNet.Metadata;
using UnitsNet.Metadata.Annotations;

namespace UnitsNet.Metadata.Tests.TestData
{
    public class DynoMeasurementAttribute : QuantityAttribute, IMetadataAttribute<DynoMeasurementAttribute, DynoMetadata>
    {
        public DynoMeasurementAttribute(object unit, Type? quantityType = null, string? displayName = null) : base(unit, quantityType)
        {
            DisplayName = displayName;
        }

        public string? DisplayName { get; }

        public DynoMetadata ToMetadata(PropertyInfo property, IEnumerable<UnitMetadataBasic> conversions, UnitMetadata? overrideUnit = null, CultureInfo? culture = null)
        {
            var unit = overrideUnit;
            if (unit is null && UnitInfo is not null && QuantityInfo is not null)
                unit = UnitMetadata.FromUnitInfo(UnitInfo, QuantityInfo, culture);

            return new(property, unit, conversions.ToList(), DisplayName);
        }

        public override void Validate()
        {
            base.Validate();
            if (DisplayName is null)
                throw new InvalidOperationException($"The {nameof(DisplayName)} property must be set.");
        }
    }
}
