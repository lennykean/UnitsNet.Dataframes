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
        }
    }
}
