using System;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIDatalog<TDatalogFrame, TFaultCode, TReadinessTest, TReadinessCode, TDatalogComment> : IDatalog<TDatalogFrame, TFaultCode, TDatalogComment>
        where TDatalogFrame : IOBDIIDatalogFrame<TFaultCode, TReadinessTest, TReadinessCode>
        where TFaultCode : IOBDIIFaultCode
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
        where TDatalogComment : IDatalogComment
    {
    }
}
