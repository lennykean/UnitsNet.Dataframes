using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet.Metadata;
using UnitsNet.Metadata.Annotations;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public sealed class DatalogFrameMetadata : ObjectMetadata<SensorAttribute, SensorMetadata, DatalogFrameMetadata.Mapper> 
    {
        public DatalogFrameMetadata(Type forType, CultureInfo? culture = null) : base(forType, culture)
        {
        }

        public class Mapper : IMetadataAttributeMapper
        {
            public SensorMetadata Map(SensorAttribute metadataAttribute, PropertyInfo property, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
            {
                return SensorMetadata.FromSensorAttribute(metadataAttribute, property, allowedConversions, culture);
            }
        }
    }
}
