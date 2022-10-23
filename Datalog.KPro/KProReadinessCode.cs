using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProReadinessCode : IReadinessCode<KProReadinessTests>
    {
        public KProReadinessCode(KProReadinessTests readinessTest, bool supported, bool? ready)
        {
            ReadinessTest = readinessTest;
            Supported = supported;
            Ready = ready;
        }

        public KProReadinessTests ReadinessTest { get; }
        public bool Supported { get; }
        public bool? Ready { get; }

        public override string ToString()
        {
            return $"{nameof(KProReadinessCode)}({nameof(Supported)}: {Supported}, {nameof(Ready)} {Ready?.ToString() ?? "null"})";
        }
    }
}
