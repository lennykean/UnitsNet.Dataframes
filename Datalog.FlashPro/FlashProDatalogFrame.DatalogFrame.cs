using System.Runtime.InteropServices;

namespace HondataDotNet.Datalog.FlashPro
{
    partial class FlashProDatalogFrame
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DatalogFrame
        {
            public int FrameNumber;
            public int Offset;
            public byte Unknown;
            public double RPM;
            public double VSS;
            public double Gear;
            public double MAP;
            public double TPedal;
            public double TPlate;
            public double AFMv;
            public double AFM;
            public double INJ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown2;
            public double IGN;
            public double IAT;
            public double ECT;
            public double CAM;
            public double CAMCMD;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown3;
            public double Lambda;
            public double LambdaCMD;
            public double STRIM;
            public double LTRIM;
            public double FuelStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown4;
            public double KLevel;
            public double KRetard;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Unknown5;
            public double BAT;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 472)]
            public byte[] Unknown6;
            public double IAT2;
            public double BP;
            public double EXCAM;
            public double EXCAMCMD;
            public double DIFP;
            public double WG;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown7;
            public double BPCMD;
            public double DIFPCMD;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Unknown8;
            public double AIRC;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Unknown9;
            public double CatT;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Unknown10;
            public double Yaw;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 241)]
            public byte[] Unknown11;
        }
    }
}
