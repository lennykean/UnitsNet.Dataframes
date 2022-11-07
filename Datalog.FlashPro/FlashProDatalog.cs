using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;
using HondataDotNet.Datalog.FlashPro.Compression;

using ICSharpCode.SharpZipLib.BZip2;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed partial class FlashProDatalog : IFlashProDatalog
    {
        private const string TYPE_IDENTIFIER = "FPDL";
        private const string COMPRESSED_TYPE_IDENTIFIER = "OPDL";

        private Header _header;
        private byte[] _footer;
        private FlashProDatalogFrameCollection _frames;


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private FlashProDatalog()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        public IReadWriteCollection<FlashProDatalogFrame> Frames => _frames;
        IReadOnlyCollection<IDatalogFrame> IDatalog.Frames => Frames;
        public IReadWriteCollection<FlashProDatalogFrameComment> Comments => throw new NotImplementedException();
        IReadOnlyCollection<IDatalogFrameComment> IDatalog.Comments => throw new NotImplementedException();

        public TimeSpan Duration => throw new NotImplementedException();

        public Version Version => new(
            _header.Version >> 12,
            _header.Version >> 8 & 0x0F,
            _header.Version >> 4 & 0x0F,
            _header.Version & 0x0F);

        public DateTime Recorded => new(_header.RecordedYear, _header.RecordedMonth, _header.RecordedDay, _header.RecordedHour, _header.RecordedMinute, _header.RecordedSecond);

        public static FlashProDatalog FromStream(Stream stream, bool preValidate = true, bool isCompressed = false)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (preValidate)
                ValidateIdentifier(stream, out isCompressed);

            return isCompressed 
                ? ReadFromCompressedStream(stream, preValidate: false)
                : ReadfromUncompressedStream(stream, preValidate: false);
        }

        public static FlashProDatalog FromFile(string filename)
        {
            try
            {
                using var fileStream = File.OpenRead(filename);
                var datalog = FromStream(fileStream);

                return datalog;
            }
            catch (InvalidDatalogFormatException idfEx)
            {
                throw new InvalidDatalogFileException($"File {filename} is not a valid FlashPro datalog file.", idfEx);
            }
        }

        public static bool IsValidIdentifier(byte[] buffer, out string identifier, out bool isCompressed)
        {
            identifier = Encoding.ASCII.GetString(buffer, 0, TYPE_IDENTIFIER.Length);
            isCompressed = identifier.Equals(COMPRESSED_TYPE_IDENTIFIER);
            return isCompressed || identifier.Equals(TYPE_IDENTIFIER);
        }

        private static FlashProDatalog ReadfromUncompressedStream(Stream stream, bool preValidate)
        {
            if (preValidate)
                ValidateIdentifier(stream, out _);

            var datalog = new FlashProDatalog
            {
                _header = stream.ReadStruct<Header>(offset: 6)
            };
            datalog._frames = FlashProDatalogFrameCollection.ReadFromStream(stream, datalog, datalog._header.FrameCount, datalog._header.FrameSize);

            using var footer = new MemoryStream();
            stream.CopyTo(footer);
            datalog._footer = footer.ToArray();

            return datalog;
        }

        private static FlashProDatalog ReadFromCompressedStream(Stream stream, bool preValidate)
        {
            using var opdlStream = new OpdlToBz2ConvertionStream(stream, preValidate);
            using var fpdlStream = new BZip2InputStream(opdlStream);

            return ReadfromUncompressedStream(fpdlStream, preValidate: true);
        }

        private static void ValidateIdentifier(Stream stream, out bool isCompressed)
        {
            var buffer = new byte[6];
            stream.Read(buffer);

            if (!IsValidIdentifier(buffer, out var identifier, out isCompressed))
                throw new InvalidDatalogFormatException($"Identifier \"{identifier}\" does not indicate a valid FlashPro datalog");
        }

        public void Save(Stream stream, bool compress)
        {
            if (!compress)
            {
                Save(stream);
                return;
            }

            using (var opdlStream = new Bz2ToOpdlConvertionStream(stream))
            {
                int bytesSaved;
                using (var bz2Stream = new BZip2OutputStream(opdlStream, 5))
                {
                    bytesSaved = Save(bz2Stream);
                    bz2Stream.Flush();
                }
                opdlStream.WriteOpdlHeader((uint)bytesSaved);
            }
        }

        public int Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _header.TypeIdentifier = new byte[6];
            Encoding.ASCII.GetBytes(TYPE_IDENTIFIER, _header.TypeIdentifier);
            _header.FrameCount = (short)_frames.Count;

            var bytesSaved = stream.WriteStruct(_header);
            bytesSaved += _frames.Save(stream, _header.FrameSize);
            stream.Write(_footer);
            bytesSaved += _footer.Length;

            return bytesSaved;
        }

        void IDatalog.Save(Stream stream)
        {
            Save(stream);
        }
    }
}
