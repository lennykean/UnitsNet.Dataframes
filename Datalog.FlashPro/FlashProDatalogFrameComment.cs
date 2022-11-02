using System;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed class FlashProDatalogFrameComment : IDatalogFrameComment
    {
        public TimeSpan Offset => throw new NotImplementedException();
        public string Comment => throw new NotImplementedException();
    }
}