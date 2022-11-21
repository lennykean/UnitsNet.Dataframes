using System.Globalization;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet.Metadata;

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
            public SensorMetadata? Map(SensorAttribute metadataAttribute, CultureInfo? culture)
            {
                return new SensorMetadata(metadataAttribute, culture);
            }
        }
    }
}
