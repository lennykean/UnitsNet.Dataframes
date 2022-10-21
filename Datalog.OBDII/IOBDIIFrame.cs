using System;
using System.Collections.Generic;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIFrame<TOBDIIFaultCodeCollection, TOBDIIFaultCode, TReadinessCodeDictionary, TReadinessTest, TReadinessCode> : IFrame<TOBDIIFaultCodeCollection, TOBDIIFaultCode>
        where TOBDIIFaultCodeCollection : IReadOnlyCollection<TOBDIIFaultCode>
        where TOBDIIFaultCode : IOBDIIFaultCode
        where TReadinessCodeDictionary : IReadOnlyDictionary<TReadinessTest, TReadinessCode>
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
    {
        byte FuelStatus { get; }
        TReadinessCodeDictionary ReadinessCodes { get; }
    }
}
