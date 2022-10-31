using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core.Utils
{
    public interface IReadWriteCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
    }
}
