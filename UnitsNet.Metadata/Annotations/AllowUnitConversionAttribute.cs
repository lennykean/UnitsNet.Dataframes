using System;

namespace UnitsNet.Metadata.Annotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AllowUnitConversionAttribute : Attribute
    {
        public AllowUnitConversionAttribute(object? unit = null)
        {
            if (unit is null)
                return;
            if (unit is not Enum unitValue)
                throw new ArgumentException($"{nameof(unit)} must be an enum value");

            Unit = unitValue;
        }

        public Enum? Unit { get; }
    }
}