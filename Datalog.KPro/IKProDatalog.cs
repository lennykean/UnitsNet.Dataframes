using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public interface IKProDatalog : IOBDIIDatalog<KProFrame, KProFaultCode, KProReadinessTests, KProReadinessCode, KProComment>
    {
    }
}
