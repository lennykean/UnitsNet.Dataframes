using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed partial class KProDatalog : IOBDIIDatalog<KProFrameCollection, KProFrame, KProFaultCodeCollection, KProFaultCode, KProReadinessCodeDictionary, KProReadinessTests, IReadinessCode<KProReadinessTests>>
    {
        private Header _header;
        private byte[] _footer;
        private KProFrameCollection _frames;

#pragma warning disable CS8618
        private KProDatalog()
#pragma warning restore CS8618
        {
        }

        public double StoichiometricRatio { get; set; } = 14.7;

        public KProFrameCollection Frames => _frames;

        IReadOnlyCollection<IFrame> IDatalog.Frames => Frames;

        public TimeSpan Duration => TimeSpan.FromMilliseconds(_header.Duration);

        public Version Version => new(
            _header.Version >> 12,
            _header.Version >> 8 & 0x0F,
            _header.Version >> 4 & 0x0F,
            _header.Version & 0x0F);

        public ushort SerialNumber => _header.SerialNumber;


        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _header.TypeIdentifier = Encoding.ASCII.GetBytes(TYPE_IDENTIFIER);
            _header.FrameCount = Frames.Count;

            var ptr = Marshal.AllocHGlobal(StructSize);
            try
            {
                var buffer = new byte[StructSize];
                Marshal.StructureToPtr(_header, ptr, false);
                Marshal.Copy(ptr, buffer, 0, StructSize);
                stream.Write(buffer);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            Frames.Save(stream, _header.FrameSize);

            stream.Write(_footer);
        }

        public static KProDatalog FromStream(Stream stream, bool preValidate = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (preValidate)
                ValidateIdentifier(stream);

            var datalog = new KProDatalog();

            var ptr = Marshal.AllocHGlobal(StructSize);
            try
            {
                var buffer = new byte[StructSize];
                stream.Read(buffer, TYPE_IDENTIFIER.Length, StructSize - TYPE_IDENTIFIER.Length);

                Marshal.Copy(buffer, 0, ptr, StructSize);
                datalog._header = Marshal.PtrToStructure<Header>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            datalog._frames = KProFrameCollection.ReadFramesFromStream(stream, datalog, datalog._header.FrameCount, datalog._header.FrameSize);

            var footer = new MemoryStream();
            stream.CopyTo(footer);
            datalog._footer = footer.ToArray();

            return datalog;
        }

        public static KProDatalog FromFile(string filename)
        {
            try
            {
                using var fileStream = File.OpenRead(filename);
                var datalog = FromStream(fileStream);

                return datalog;
            }
            catch (InvalidDatalogFormatException idfEx)
            {
                throw new InvalidDatalogFileException($"File {filename} is not a valid KPro datalog file.", idfEx);
            }
        }

        public static bool IsValidIdentifier(byte[] buffer)
        {
            var identifier = Encoding.ASCII.GetString(buffer, 0, TYPE_IDENTIFIER.Length);
            return identifier.Equals(TYPE_IDENTIFIER);
        }

        private static void ValidateIdentifier(Stream stream)
        {
            var buffer = new byte[TYPE_IDENTIFIER.Length];
            stream.Read(buffer, 0, TYPE_IDENTIFIER.Length);

            if (!IsValidIdentifier(buffer))
                throw new InvalidDatalogFormatException($"Identifier \"{buffer}\" does not indicate a valid KPro datalog");
        }
    }
}
