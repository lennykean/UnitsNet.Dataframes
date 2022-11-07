using System;
using System.Collections.Generic;
using System.IO;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;
using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed partial class FlashProDatalogFrame : IOBDIIDatalogFrame<FlashProFaultCode, FlashProReadinessTests, FlashProReadinessCode>
    {
        private DatalogFrame _frame;

        public FlashProDatalog? Datalog { get; internal set; }
        public TimeSpan Offset => TimeSpan.FromMilliseconds(_frame.Offset);
        public int RPM => throw new NotImplementedException();
        public double VSS => throw new NotImplementedException();
        public double INJ => throw new NotImplementedException();
        public double IGN => throw new NotImplementedException();
        public double IAT => throw new NotImplementedException();
        public double ECT => throw new NotImplementedException();
        public bool VTP => throw new NotImplementedException();
        public double Lambda => throw new NotImplementedException();
        public double STRIM => throw new NotImplementedException();
        public double LTRIM => throw new NotImplementedException();
        public double KLevel => throw new NotImplementedException();
        public double PA => throw new NotImplementedException();
        public double BAT => throw new NotImplementedException();
        public int Gear => throw new NotImplementedException();
        public byte FuelStatus => throw new NotImplementedException();

        public IReadOnlyDictionary<FlashProReadinessTests, FlashProReadinessCode> ReadinessCodes => throw new NotImplementedException();
        public IReadOnlyCollection<FlashProFaultCode> FaultCodes => throw new NotImplementedException();
        IReadOnlyCollection<IFaultCode> IDatalogFrame.FaultCodes => throw new NotImplementedException();

        internal static FlashProDatalogFrame ReadFromStream(Stream stream, int frameSize)
        {
            return new()
            {
                _frame = stream.ReadStruct<DatalogFrame>(0, frameSize)
            };
        }

        internal int Save(Stream stream, int frameNumber, int frameSize)
        {
            _frame.FrameNumber = frameNumber;

            return stream.WriteStruct(_frame, offset: 0, length: frameSize);
        }
    }
}
