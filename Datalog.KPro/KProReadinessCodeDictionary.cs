using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProReadinessCodeDictionary : ReadinessCodeDictionary<KProReadinessTests, IReadinessCode<KProReadinessTests>>
    {
        private readonly KProReadinessTests _supportFlags;
        private readonly KProReadinessTests _statusFlags;

        internal KProReadinessCodeDictionary(KProReadinessTests supportFlags, KProReadinessTests statusFlags)
        {
            _statusFlags = statusFlags;
            _supportFlags = supportFlags;
        }

        public override IReadinessCode<KProReadinessTests> this[KProReadinessTests key] => new KProReadinessCode(key, _supportFlags.HasFlag(key), _statusFlags.HasFlag(key));
    }
}
