using System.Runtime.InteropServices;

namespace HondataDotNet.Datalog.KPro
{
    partial class KProDatalogFrame
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DatalogFrame
        {
            public int FrameNumber;
            public int Offset;
            public int RPM;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] Unknown;
            public double VSS;
            public double MAP;
            public double CLV;
            public double TPS;
            public double CAM;
            public double CAMCMD;
            public double INJ;
            public double IGN;
            public double IAT;
            public double ECT;
            public double BCDuty;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
            public byte[] Unknown2;
            [MarshalAs(UnmanagedType.I1)]
            public bool RVSLK;
            [MarshalAs(UnmanagedType.I1)]
            public bool BKSW;
            [MarshalAs(UnmanagedType.I1)]
            public bool ACSW;
            [MarshalAs(UnmanagedType.I1)]
            public bool ACCL;
            [MarshalAs(UnmanagedType.I1)]
            public bool SCS;
            [MarshalAs(UnmanagedType.I1)]
            public bool EPS;
            [MarshalAs(UnmanagedType.I1)]
            public bool FLR;
            [MarshalAs(UnmanagedType.I1)]
            public bool VTP;
            [MarshalAs(UnmanagedType.I1)]
            public bool VTS;
            [MarshalAs(UnmanagedType.I1)]
            public bool FANC;
            [MarshalAs(UnmanagedType.I1)]
            public bool MIL;
            public byte Unknown3;
            public double O2;
            public double O2C;
            public double S02;
            public double Lambda;
            public double LambdaCMD;
            public double STRIM;
            public double LTRIM;
            public byte FuelStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] Unknown4;
            public double KRetard;
            public double KLevel;
            public double KThres;
            public long KCount;
            public double PA;
            public double PTANK;
            public double BAT;
            public double ELD;
            [MarshalAs(UnmanagedType.I1)]
            public bool N2OArm1;
            [MarshalAs(UnmanagedType.I1)]
            public bool N2OOn1;
            [MarshalAs(UnmanagedType.I1)]
            public bool N2OArm2;
            [MarshalAs(UnmanagedType.I1)]
            public bool N2OOn2;
            [MarshalAs(UnmanagedType.I1)]
            public bool N2OArm3;
            [MarshalAs(UnmanagedType.I1)]
            public bool N2OOn3;
            [MarshalAs(UnmanagedType.I1)]
            public KProReadinessTests ReadinessCodeSupportFlags;
            [MarshalAs(UnmanagedType.I1)]
            public KProReadinessTests ReadinessCodeStatusFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] FaultCodeFlags;
            public int Gear;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Unknown5;
            public double ELDV;
            [MarshalAs(UnmanagedType.I1)]
            public bool Data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] Unknown6;
            public double KLimit;
            public double KControl;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
            public byte[] Unknown7;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] AnalogInputs;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] DigitalInputs;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
            public byte[] Unknown8;
            public double EContent;
            public double FTemp;
            public double VSSInput;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Unknown9;
            [MarshalAs(UnmanagedType.I1)]
            public bool RevLimit;
        }
    }
}
