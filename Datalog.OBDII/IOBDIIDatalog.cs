using System;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIDatalog<TFrame, TFaultCode, TReadinessTest, TReadinessCode, TComment> : IDatalog<TFrame, TFaultCode, TComment>
        where TFrame : IOBDIIFrame<TFaultCode, TReadinessTest, TReadinessCode>
        where TFaultCode : IOBDIIFaultCode
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
        where TComment : IComment
    {
    }
}
