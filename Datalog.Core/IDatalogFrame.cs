using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core
{
    public interface IDatalogFrame : ITimeSeriesElement
    {
        int RPM { get; }
        double VSS { get; }
        int Gear { get; }
        double INJ { get; }
        double IGN { get; }
        double IAT { get; }
        double ECT { get; }
        double AF { get; }
        double STRIM { get; }
        double LTRIM { get; }
        double KLevel { get; }
        double PA { get; }
        double BAT { get; }
        bool VTS { get; }

        IReadOnlyCollection<IFaultCode> FaultCodes { get; }
    }

    public interface IDatalogFrame<out TFaultCode> : IDatalogFrame
        where TFaultCode : IFaultCode
    {
        new IReadOnlyCollection<TFaultCode> FaultCodes { get; }
    }
}
