using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core
{
    public interface IDatalogFrame : ITimeSeriesElement
    {
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

        IReadOnlyCollection<IFaultCode> FaultCodes { get; }
    }

    public interface IDatalogFrame<TFaultCode> : IDatalogFrame
        where TFaultCode : IFaultCode
    {
        new IReadOnlyCollection<TFaultCode> FaultCodes { get; }
    }
}
