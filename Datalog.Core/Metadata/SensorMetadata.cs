using System.Globalization;
using System.Reflection;

namespace HondataDotNet.Datalog.Core.Metadata
{

    public class SensorMetadata : QuantityMetadata
    {
        public SensorMetadata(SensorMetadataAttribute metadataAttribute, CultureInfo? culture = null) : base(metadataAttribute, culture)
        {
            DisplayName = metadataAttribute.DisplayName;
            Description = metadataAttribute.Description;
        }

        public string DisplayName { get; }
        public string? Description { get; }
    }
}