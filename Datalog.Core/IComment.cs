using System;

namespace HondataDotNet.Datalog.Core
{
    public interface IComment
    {
        TimeSpan offset { get; }
        string Comment { get; }
    }
}
