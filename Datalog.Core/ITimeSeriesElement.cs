using System;

namespace HondataDotNet.Datalog.Core
{
    public interface ITimeSeriesElement
    {
        TimeSpan Offset { get; }
    }
}