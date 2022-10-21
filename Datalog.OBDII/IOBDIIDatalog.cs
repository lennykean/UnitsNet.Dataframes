using System;
using System.Collections.Generic;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIDatalog<TFrameCollection, TFrame, TFaultCodeCollection, TFaultCode, TReadinessCodeDictionary, TReadinessTest, TReadinessCode> : IDatalog<TFrameCollection, TFrame, TFaultCodeCollection, TFaultCode>
        where TFrameCollection : IReadOnlyCollection<TFrame>
        where TFrame : IOBDIIFrame<TFaultCodeCollection, TFaultCode, TReadinessCodeDictionary, TReadinessTest, TReadinessCode>
        where TFaultCodeCollection : IReadOnlyCollection<TFaultCode>
        where TFaultCode : IOBDIIFaultCode
        where TReadinessCodeDictionary : IReadOnlyDictionary<TReadinessTest, TReadinessCode>
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
    {
    }
}
