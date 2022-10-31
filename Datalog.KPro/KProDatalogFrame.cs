using System;
using System.Collections.Generic;
using System.IO;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;
using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed partial class KProDatalogFrame : IOBDIIDatalogFrame<KProFaultCode, KProReadinessTests, KProReadinessCode>
    {
        private readonly Lazy<KProReadinessCodeDictionary> _lazyReadinessCodes;
        private readonly Lazy<KProFaultCodeCollection> _lazyFaultCodes;
        private readonly Lazy<TimeSpan> _lazyFrameOffset;

        private DatalogFrame _frame;

        public KProDatalogFrame()
        {
            _lazyReadinessCodes = new(() => new(_frame.ReadinessCodeSupportFlags, _frame.ReadinessCodeStatusFlags));
            _lazyFaultCodes = new(() => new(_frame.FaultCodeFlags));
            _lazyFrameOffset = new(() => TimeSpan.FromMilliseconds(_frame.Offset));
        }

        public KProDatalog? Datalog { get; internal set; }
        public TimeSpan Offset => _lazyFrameOffset.Value;
        public int RPM => _frame.RPM;
        public double VSS => _frame.VSS;
        public double MAP => _frame.MAP;
        public double CLV => _frame.CLV;
        public double TPS => _frame.TPS;
        public double CAM => _frame.CAM;
        public double CAMCMD => _frame.CAMCMD;
        public double INJ => _frame.INJ;
        public double IGN => _frame.IGN;
        public double IAT => _frame.IAT;
        public double ECT => _frame.ECT;
        public double BCDuty => _frame.BCDuty;
        public bool RVSLK => _frame.RVSLK;
        public bool BKSW => _frame.BKSW;
        public bool ACSW => _frame.ACSW;
        public bool ACCL => _frame.ACCL;
        public bool SCS => _frame.SCS;
        public bool EPS => _frame.EPS;
        public bool FLR => _frame.FLR;
        public bool VTP => _frame.VTP;
        public bool VTS => _frame.VTS;
        public bool FANC => _frame.FANC;
        public bool MIL => _frame.MIL;
        public double O2 => _frame.O2;
        public double O2C => _frame.O2C;
        public double S02 => _frame.S02;
        public double Lambda => _frame.Lambda;
        public double LambdaCMD => _frame.LambdaCMD;
        public double STRIM => _frame.STRIM;
        public double LTRIM => _frame.LTRIM;
        public byte FuelStatus => _frame.FuelStatus;
        public double KRetard => _frame.KRetard;
        public double KLevel => _frame.KLevel;
        public double KThres => _frame.KThres;
        public long KCount => _frame.KCount;
        public double PA => _frame.PA;
        public double PTANK => _frame.PTANK;
        public double BAT => _frame.BAT;
        public double ELD => _frame.ELD;
        public bool N2OArm1 => _frame.N2OArm1;
        public bool N2OOn1 => _frame.N2OOn1;
        public bool N2OArm2 => _frame.N2OArm2;
        public bool N2OOn2 => _frame.N2OOn2;
        public bool N2OArm3 => _frame.N2OArm3;
        public bool N2OOn3 => _frame.N2OOn3;
        public IReadOnlyDictionary<KProReadinessTests, KProReadinessCode> ReadinessCodes => _lazyReadinessCodes.Value;
        public IReadOnlyCollection<KProFaultCode> FaultCodes => _lazyFaultCodes.Value;
        IReadOnlyCollection<IFaultCode> IDatalogFrame.FaultCodes => FaultCodes;
        public int Gear => _frame.Gear;
        public double ELDV => _frame.ELDV;
        public bool Data => _frame.Data;
        public double KLimit => _frame.KLimit;
        public double KControl => _frame.KControl;
        public IReadOnlyList<double> AnalogInputs => _frame.AnalogInputs;
        public IReadOnlyList<double> DigitalInputs => _frame.DigitalInputs;
        public double EContent => _frame.EContent;
        public double FTemp => _frame.FTemp;
        public double VSSInput => _frame.VSSInput;
        public bool RevLimit => _frame.RevLimit;

        public double AF => Lambda * Datalog?.StoichiometricRatio ?? 0;
        public double AFCMD => LambdaCMD * Datalog?.StoichiometricRatio ?? 0;
        public double DUTY => RPM * INJ / 1200;

        internal static KProDatalogFrame ReadFromStream(Stream stream, int frameSize)
        {
            return new()
            {
                _frame = stream.ReadStruct<DatalogFrame>(0, frameSize)
            };
        }

        internal void Save(Stream stream, int frameNumber, int frameSize)
        {
            _frame.FrameNumber = frameNumber;

            stream.WriteStruct(_frame, offset: 0, length: frameSize);
        }
    }
}
