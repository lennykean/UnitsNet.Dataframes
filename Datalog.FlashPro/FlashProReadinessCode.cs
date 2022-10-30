using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.FlashPro
{
    public class FlashProReadinessCode : IReadinessCode<FlashProReadinessTests>
    {
        public FlashProReadinessTests ReadinessTest => throw new System.NotImplementedException();
        public bool Supported => throw new System.NotImplementedException();
        public bool? Ready => throw new System.NotImplementedException();
    }
}
