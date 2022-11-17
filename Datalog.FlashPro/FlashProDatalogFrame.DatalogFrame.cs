using HondataDotNet.Datalog.Core;

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
            public double AF;
            public double AFCMD;
            public double STRIM;
            public double LTRIM;
            public double FuelStatus;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown4;
            public double KLevel;
            public double KRetard;
            public double KControl;
            public double PA;
            public double BAT;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] Unknown5;
            public double ACCL;
            public double VTS;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown6;
            public double Eco;
            public double FuelUsed;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Unknown7;
            public double Wide;
            public double WideV;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown8;
            public double KCount1;
            public double KCount2;
            public double KCount3;
            public double KCount4;
            public double KCount5;
            public double KCount6;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Unknown9;
            public double BCDuty;
            public double IgnLimit;
            public double ECT2;
            public double TCV;
            public double TCECUSlip;
            public double TCR;
            public double TCLF;
            public double TCRF;
            public double TCLR;
            public double TCRR;
            public double TCSlip;
            public double TCTurn;
            public double TCOverSlip;
            public double TCOut;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] Unknown10;
            public double SVS;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Unknown11;
            public double PTANK;
            public double Purge;
            public double AFBank2;
            public double AFCMDBank2;
            public double STRIMBank2;
            public double LTRIMBank2;
            public double FuelStatusBank2;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown12;
            public double INJBank2;
            public double IAT2;
            public double BP;
            public double EXCAM;
            public double EXCAMCMD;
            public double DIFP;
            public double WG;
            public double WGCMD;
            public double BPCMD;
            public double DIFPCMD;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Unknown13;
            public double AIRC;
            public double EGR;
            public double OilPress;
            public double CatT;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown15;
            public double KRetard1;
            public double KRetard2;
            public double KRetard3;
            public double KRetard4;
            public double GLat;
            public double GLong;
            public double Yaw;
            public double GZ;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown16;
            public double Ethanol;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] Unknown17;
            public double CVTTemp;
            public double ABSLF;
            public double ABSRF;
            public double ABSLR;
            public double ABSRR;
            public double ClutchPos;
            public double BrakePress;
            public double SteerAng;
            public double SteerTrq;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
            public byte[] Unknown18;
            public double FuelP;
            public double AFMHz;
        }
    }
}
