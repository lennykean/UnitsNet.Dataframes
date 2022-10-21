using System;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IReadinessCode
    {
        public bool Supported { get; }
        public bool? Ready { get; }
    }

    public interface IReadinessCode<TReadinessTest> : IReadinessCode
        where TReadinessTest : struct, Enum
    {
        public TReadinessTest ReadinessTest { get; }
    }
}
