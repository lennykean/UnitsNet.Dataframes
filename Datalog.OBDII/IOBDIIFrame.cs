﻿using System;
using System.Collections.Generic;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.OBDII
{
    public interface IOBDIIFrame<TFaultCode, TReadinessTest, TReadinessCode> : IFrame<TFaultCode>
        where TFaultCode : IOBDIIFaultCode
        where TReadinessTest : struct, Enum
        where TReadinessCode : IReadinessCode<TReadinessTest>
    {
        byte FuelStatus { get; }
        IReadOnlyDictionary<TReadinessTest, TReadinessCode> ReadinessCodes { get; }
    }
}
