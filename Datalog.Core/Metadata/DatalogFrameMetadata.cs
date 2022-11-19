using System.Globalization;

namespace HondataDotNet.Datalog.Core.Metadata
{
    public sealed class DatalogFrameMetadata<TObject> : ObjectMetadata<TObject, SensorMetadataAttribute, SensorMetadata, DatalogFrameMetadata<TObject>.Mapper> 
        where TObject : IDatalogFrame
    {
        public DatalogFrameMetadata(CultureInfo? culture = null) : base(culture)
        {
        }

        public class Mapper : IMetadataAttributeMapper
        {
            public SensorMetadata? Map(SensorMetadataAttribute metadataAttribute, CultureInfo? culture)
            {
                return new SensorMetadata(metadataAttribute, culture);
            }
        }
    }
}
