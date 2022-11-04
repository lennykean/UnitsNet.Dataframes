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
        }
    }
}
