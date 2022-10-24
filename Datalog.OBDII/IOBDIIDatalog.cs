using System;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIDatalog<TDatalogFrame, TFaultCode, TDatalogComment, TReadinessTest, TReadinessCode> : IDatalog<TDatalogFrame, TFaultCode, TDatalogComment>
        where TDatalogFrame : IOBDIIDatalogFrame<TFaultCode, TReadinessTest, TReadinessCode>
        where TFaultCode : IOBDIIFaultCode
        where TDatalogComment : IDatalogFrameComment
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
    {
    }
}
