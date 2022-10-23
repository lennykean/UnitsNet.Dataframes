using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.KPro
{
    public sealed partial class KProDatalog : IKProDatalog
    {
        private Header _header;
        private byte[] _footer;
        private KProDatalogFrameCollection _frames;
        private KProDatalogCommentCollection _comments;

#pragma warning disable CS8618
        private KProDatalog()
#pragma warning restore CS8618
        {
        }

        public double StoichiometricRatio { get; set; } = 14.7;

        public IReadWriteCollection<KProDatalogFrame> Frames => _frames;

        IReadOnlyCollection<IDatalogFrame> IDatalog.Frames => Frames;

        public IReadWriteCollection<KProDatalogComment> Comments => _comments;

        IReadOnlyCollection<IDatalogComment> IDatalog.Comments => Comments;

        public TimeSpan Duration { get => TimeSpan.FromMilliseconds(_header.Duration); set => _header.Duration = (int)value.TotalMilliseconds; }

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
            _header.FrameCount = _frames.Count;
            _header.CommentCount = (short)_comments.Count;

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
            _frames.Save(stream, _header.FrameSize);
            _comments.Save(stream);

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

            datalog._frames = KProDatalogFrameCollection.ReadFromStream(stream, datalog, datalog._header.FrameCount, datalog._header.FrameSize);

            datalog._comments = KProDatalogCommentCollection.ReadFromStream(stream, datalog._header.CommentCount);

            using var footer = new MemoryStream();
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
