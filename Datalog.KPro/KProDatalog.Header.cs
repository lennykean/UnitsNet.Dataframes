using System.Runtime.InteropServices;

namespace HondataDotNet.Datalog.KPro
{
    partial class KProDatalog
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Header
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public byte[] TypeIdentifier;
            public int FrameCount;
            public int FrameSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Unknown;
            public int Duration;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 120)]
            public byte[] Unknown2;
            public ushort SerialNumber;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] Unknown3;
            public short CommentLength;
            public short Version;
        }

        private const string TYPE_IDENTIFIER = "KFLASH";

        private static readonly int StructSize = Marshal.SizeOf<Header>();
    }
}
