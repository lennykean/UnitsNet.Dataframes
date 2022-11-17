using System;
using System.Collections.Generic;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIDatalogFrame<TFaultCode, TReadinessTest, TReadinessCode> : IDatalogFrame<TFaultCode>
        where TFaultCode : IOBDIIFaultCode
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
    {
        int FuelStatus { get; }
        IReadOnlyDictionary<TReadinessTest, TReadinessCode> ReadinessCodes { get; }
    }
}
