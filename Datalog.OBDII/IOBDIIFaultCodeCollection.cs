using System.Collections.Generic;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIFaultCodeCollection<TOBDIIFaultCode> : IReadOnlyCollection<TOBDIIFaultCode>
        where TOBDIIFaultCode : IOBDIIFaultCode
    {
        bool HasDTC(DTC dtc);

        IEnumerable<TOBDIIFaultCode> GetByDTC(DTC dtc);
    }
}
