using System;

using UnitsNet.Metadata.Annotations;

namespace HondataDotNet.Datalog.Core.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SensorAttribute : QuantityAttribute
    {
        public SensorAttribute(string displayName, object? unit = null, Type? quantityType = null) : base(unit, quantityType)
        {
            DisplayName = displayName;
        }

        public string DisplayName { get; }
        public string? Description { get; set; }
    }
}
