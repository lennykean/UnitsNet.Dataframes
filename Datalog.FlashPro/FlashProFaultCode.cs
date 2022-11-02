using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed class FlashProFaultCode : IOBDIIFaultCode
    {
        public string Description => throw new System.NotImplementedException();
        public DTC DTC => throw new System.NotImplementedException();
    }
}
