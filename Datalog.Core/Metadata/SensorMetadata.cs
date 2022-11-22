using System;
using System.Globalization;

using HondataDotNet.Datalog.Core.Annotations;

using UnitsNet.Metadata;

namespace HondataDotNet.Datalog.Core.Metadata
{

    public class SensorMetadata : QuantityMetadata
    {
        public SensorMetadata(SensorAttribute metadataAttribute, string name, CultureInfo? culture = null, params Enum[] allowedConversions) 
            : base(metadataAttribute, name, culture, allowedConversions)
        {
            DisplayName = metadataAttribute.DisplayName;
            Description = metadataAttribute.Description;
        }

        public string DisplayName { get; }
        public string? Description { get; }
    }
}