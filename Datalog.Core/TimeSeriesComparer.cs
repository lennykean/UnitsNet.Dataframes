using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core
{
    public class TimeSeriesComparer : IComparer<ITimeSeriesElement>
    {
        public int Compare(ITimeSeriesElement? x, ITimeSeriesElement? y)
        {
            return (x?.Offset ?? default).CompareTo(y?.Offset);
        }
    }
}
