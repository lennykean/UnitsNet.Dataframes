using System.Runtime.InteropServices;

namespace HondataDotNet.Datalog.FlashPro
{
    partial class FlashProDatalog
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] TypeIdentifier;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Unknown;
            public short Version;
            public ushort SerialNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 23)]
            public byte[] Unknown2;
            public byte RecordedDay;
            public byte RecordedMonth;
            public short RecordedYear;
            public byte RecordedHour;
            public byte RecordedMinute;
            public byte RecordedSecond;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
            public byte[] Unknown3;
            public short FrameCount;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
            public byte[] Unknown4;
            public int FrameSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 288)]
            public byte[] Unknown5;
        }
    }
}
