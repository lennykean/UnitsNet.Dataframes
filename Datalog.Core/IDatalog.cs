using System;
using System.Collections.Generic;
using System.IO;

namespace HondataDotNet.Datalog.Core
{
    public interface IDatalog
    {
        TimeSpan Duration { get; }
        Version Version { get; }

        IReadOnlyCollection<IDatalogFrame> Frames { get; }
        
        IReadOnlyCollection<IDatalogComment> Comments { get; }

        void Save(Stream stream);
    }

    public interface IDatalog<TDatalogFrame, TFaultCode, TDatalogComment> : IDatalog
        where TDatalogFrame : IDatalogFrame<TFaultCode>
        where TFaultCode : IFaultCode
        where TDatalogComment : IDatalogComment
    {
        new IReadWriteCollection<TDatalogFrame> Frames { get; }
        new IReadWriteCollection<TDatalogComment> Comments { get; }
    }
}
