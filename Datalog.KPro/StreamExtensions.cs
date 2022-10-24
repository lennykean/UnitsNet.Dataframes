using System.IO;
using System.Runtime.InteropServices;

namespace HondataDotNet.Datalog.KPro
{
    internal static class StreamExtensions
    {
        public static TStruct ReadStruct<TStruct>(this Stream stream, int? offset = null, int? length = null) where TStruct : struct
        {
            var structSize = Marshal.SizeOf<TStruct>();
            var ptr = Marshal.AllocHGlobal(structSize);
            try
            {
                var buffer = new byte[length ?? structSize];
                stream.Read(buffer, offset ?? 0, (length ?? structSize) - (offset ?? 0));
                                
                Marshal.Copy(buffer, 0, ptr, structSize);
                
                return Marshal.PtrToStructure<TStruct>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }
    }
}
