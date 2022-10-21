using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core
{
    public class FrameComparer : IComparer<IFrame>
    {
        public int Compare(IFrame? x, IFrame? y)
        {
            return (x?.FrameOffset ?? default).CompareTo(y?.FrameOffset);
        }
    }
}
