using System;
using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core
{
    public interface IFrame
    {
        TimeSpan FrameOffset { get; }
        int RPM { get; }
        double VSS { get; }
        double INJ { get; }
        double IGN { get; }
        double IAT { get; }
        double ECT { get; }
        bool VTP { get; }
        double Lambda { get; }
        double STRIM { get; }
        double LTRIM { get; }
        double KLevel { get; }
        double PA { get; }
        double BAT { get; }
        int Gear { get; }
    }

    public interface IFrame<TFaultCode> : IFrame
        where TFaultCode : IFaultCode
    {
        IReadOnlyCollection<TFaultCode> FaultCodes { get; }
    }
}
