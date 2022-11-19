using System;

namespace HondataDotNet.Datalog.Core.Metadata
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SensorMetadataAttribute : QuantityMetadataAttribute
    {
        public SensorMetadataAttribute(string displayName, object? unit = null) : base(unit)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; }
        public string? Description { get; set; }
    }
}
