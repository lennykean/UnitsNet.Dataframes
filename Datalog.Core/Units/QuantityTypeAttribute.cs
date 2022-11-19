using System;

namespace HondataDotNet.Datalog.Core.Units
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class QuantityTypeAttribute : Attribute
    {
        public QuantityTypeAttribute(Type type, string? infoProperty = null)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            InfoProperty = infoProperty ?? throw new ArgumentNullException(nameof(infoProperty));
        }

        public Type Type { get; }
        public string? InfoProperty { get; }
    }
}