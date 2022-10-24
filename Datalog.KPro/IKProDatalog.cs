using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public interface IKProDatalog : IOBDIIDatalog<KProDatalogFrame, KProFaultCode, KProDatalogComment, KProReadinessTests, KProReadinessCode>
    {
    }
}
