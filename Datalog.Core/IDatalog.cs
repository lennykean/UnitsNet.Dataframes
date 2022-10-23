using System;
using System.Collections.Generic;
using System.IO;

namespace HondataDotNet.Datalog.Core
{
    public interface IDatalog
    {
        TimeSpan Duration { get; }
        Version Version { get; }

        IReadOnlyCollection<IFrame> Frames { get; }
        
        IReadOnlyCollection<IComment> Comments { get; }

        void Save(Stream stream);
    }

    public interface IDatalog<TFrame, TFaultCode, TComment> : IDatalog
        where TFrame : IFrame<TFaultCode>
        where TFaultCode : IFaultCode
        where TComment : IComment
    {
        new IReadWriteCollection<TFrame> Frames { get; }
        new IReadWriteCollection<TComment> Comments { get; }
    }
}
