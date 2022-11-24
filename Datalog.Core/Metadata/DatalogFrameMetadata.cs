using System.Collections.Generic;
using System.Globalization;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet.Metadata;
using UnitsNet.Metadata.Annotations;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public sealed class DatalogFrameMetadata<TObject> : ObjectMetadata<TObject, SensorAttribute, SensorMetadata, DatalogFrameMetadata<TObject>.Mapper> 
        where TObject : IDatalogFrame
    {
        public DatalogFrameMetadata(CultureInfo? culture = null) : base(culture)
        {
        }

        public class Mapper : IMetadataAttributeMapper
        {
            public SensorMetadata Map(SensorAttribute metadataAttribute, string name, IEnumerable<AllowUnitConversionAttribute> allowedConversions, CultureInfo? culture = null)
            {
                return SensorMetadata.FromSensorAttribute(metadataAttribute, name, allowedConversions, culture);
            }
        }
    }
}
