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
        public double RPM => _frame.RPM;
        int IDatalogFrame.RPM => (int)RPM;
        public double VSS => _frame.VSS;
        public double INJ => _frame.INJ;
        public double IGN => _frame.IGN;
        public double IAT => _frame.IAT;
        public double ECT => _frame.IAT;
        public bool VTP => throw new NotImplementedException();
        public double Lambda => _frame.Lambda;
        public double STRIM => _frame.STRIM;
        public double LTRIM => _frame.LTRIM;
        public double KLevel => _frame.KLevel;
        public double PA => throw new NotImplementedException();
        public double BAT => _frame.BAT;
        public int Gear => (int)_frame.Gear;
        public byte FuelStatus => (byte)_frame.FuelStatus;

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
