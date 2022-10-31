using System;
using System.Collections.Generic;
using System.IO;
using HondataDotNet.Datalog.Core.Utils;

namespace HondataDotNet.Datalog.Core
{
    public interface IDatalog
    {
        TimeSpan Duration { get; }
        Version Version { get; }

        IReadOnlyCollection<IDatalogFrame> Frames { get; }
        
        IReadOnlyCollection<IDatalogFrameComment> Comments { get; }

        void Save(Stream stream);
    }

    public interface IDatalog<TDatalogFrame, TFaultCode, TDatalogComment> : IDatalog
        where TDatalogFrame : IDatalogFrame<TFaultCode>
        where TFaultCode : IFaultCode
        where TDatalogComment : IDatalogFrameComment
    {
        new IReadWriteCollection<TDatalogFrame> Frames { get; }
        new IReadWriteCollection<TDatalogComment> Comments { get; }
    }
}
