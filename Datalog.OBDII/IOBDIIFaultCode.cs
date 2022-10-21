using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIFaultCode : IFaultCode
    {
        DTC DTC { get; }
    }
}
