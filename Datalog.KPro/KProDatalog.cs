using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;

namespace HondataDotNet.Datalog.KPro
{
    public sealed partial class KProDatalog : IKProDatalog
    {
        private const string TYPE_IDENTIFIER = "KFLASH";
        
        private Header _header;
        private byte[] _footer;
        private KProDatalogFrameCollection _frames;
        private KProDatalogFrameCommentCollection _comments;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private KProDatalog()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public IReadWriteCollection<KProDatalogFrame> Frames => _frames;

        IReadOnlyCollection<IDatalogFrame> IDatalog.Frames => Frames;

        public IReadWriteCollection<KProDatalogFrameComment> Comments => _comments;

        IReadOnlyCollection<IDatalogFrameComment> IDatalog.Comments => Comments;

        public TimeSpan Duration { get => TimeSpan.FromMilliseconds(_header.Duration); set => _header.Duration = (int)value.TotalMilliseconds; }

        public Version Version => new(
            _header.Version >> 12,
            _header.Version >> 8 & 0x0F,
            _header.Version >> 4 & 0x0F,
            _header.Version & 0x0F);

        public ushort SerialNumber => _header.SerialNumber;

        public static KProDatalog FromStream(Stream stream, bool preValidate = true)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (preValidate)
                ValidateIdentifier(stream);

            var datalog = new KProDatalog
            {
                _header = stream.ReadStruct<Header>(offset: TYPE_IDENTIFIER.Length)
            };
            datalog._frames = KProDatalogFrameCollection.ReadFromStream(stream, datalog, datalog._header.FrameCount, datalog._header.FrameSize);
            datalog._comments = KProDatalogFrameCommentCollection.ReadFromStream(stream, datalog._header.CommentCount);

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

        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _header.TypeIdentifier = Encoding.ASCII.GetBytes(TYPE_IDENTIFIER);
            _header.FrameCount = _frames.Count;
            _header.CommentCount = (short)_comments.Count;

            stream.WriteStruct(_header);
            _frames.Save(stream, _header.FrameSize);
            _comments.Save(stream);
            stream.Write(_footer);
        }
    }
}
