using System.Collections.Generic;

namespace HondataDotNet.Datalog.Core
{
    public interface IReadWriteCollection<T> : IReadOnlyCollection<T>, ICollection<T>
    {
    }
}
