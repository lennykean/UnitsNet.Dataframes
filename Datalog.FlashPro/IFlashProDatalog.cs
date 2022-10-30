using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.FlashPro
{
    public interface IFlashProDatalog : IOBDIIDatalog<FlashProDatalogFrame, FlashProFaultCode, FlashProDatalogFrameComment, FlashProReadinessTests, FlashProReadinessCode>
    {
    }
}
