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

        void Save(Stream stream);
    }

    public interface IDatalog<TFrameCollection, TFrame, TFaultCodeCollection, TFaultCode> : IDatalog
        where TFrameCollection : IReadOnlyCollection<TFrame>
        where TFrame : IFrame<TFaultCodeCollection, TFaultCode>
        where TFaultCodeCollection : IReadOnlyCollection<TFaultCode>
        where TFaultCode : IFaultCode
    {
        new TFrameCollection Frames { get; }
    }
}