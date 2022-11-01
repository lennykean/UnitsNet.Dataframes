
using System;
using System.IO;
using System.Numerics;
using System.Text;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.FlashPro.Compression
{
    public class OpdlToBz2ConvertionStream : Stream
    {
        const string TYPE_IDENTIFIER = "OPDL";
        const int READ_AHEAD = 6;

        private static readonly byte[] _bz2Header = new byte[] { 0x42, 0x5a, 0x68, 0x38 };
        private static readonly byte[] _eosMagic = new byte[] { 0x17, 0x72, 0x45, 0x38, 0x50, 0x90 };

        private readonly Stream _opdlStream;
        private readonly MemoryStream _bZipHeaderStream;
        private readonly MemoryStream _bZipFooterStream;
        
        private MemoryStream _readAheadStream;

        public OpdlToBz2ConvertionStream(Stream opdlStream)
        {
            _opdlStream = opdlStream ?? throw new ArgumentNullException(nameof(opdlStream));
            _bZipHeaderStream = new MemoryStream(_bz2Header);
            _bZipFooterStream = new MemoryStream();
            _readAheadStream = new MemoryStream();
        }

        internal uint StoredDataSize { get; private set; }

        public override bool CanRead => _opdlStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _bZipHeaderStream.Length + _opdlStream.Length + _bZipFooterStream.Length;

        public override long Position
        {
            get => _bZipHeaderStream.Position + _opdlStream.Position + _bZipFooterStream.Position;
            set => throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_opdlStream.Position == 0)
                ReadOpdlHeader();

            var read = _bZipHeaderStream.Read(buffer, offset, count);

            while (read < count)
            {
                var aheadRead = _readAheadStream.Read(buffer, offset + read, count - read);
                read += aheadRead;

                var refill = aheadRead;
                if (_readAheadStream.Length == 0)
                    refill = READ_AHEAD;

                if (refill == 0)
                    break;

                var opdlBuffer = new byte[refill].AsSpan();
                var opdlRead = _opdlStream.Read(opdlBuffer);
                if (opdlRead == 0)
                {
                    var readAheadBytes = _readAheadStream.ToArray().AsSpan();
                    BuildBz2Footer(readAheadBytes[^READ_AHEAD..]);
                    _readAheadStream.Seek(0, SeekOrigin.End);
                    continue;
                }

                var readAheadBuffer = new byte[_readAheadStream.Length - _readAheadStream.Position];
                _readAheadStream.Read(readAheadBuffer);

                _readAheadStream = new MemoryStream();
                _readAheadStream.Write(readAheadBuffer);
                _readAheadStream.Write(opdlBuffer[..opdlRead]);
                _readAheadStream.Seek(0, SeekOrigin.Begin);
            }
            if (read < count)
            {
                read += _bZipFooterStream.Read(buffer, offset + read, count - read);
            }
            return read;
        }

        private static int FindFooterOffset(BigInteger opdlFooter)
        {
            var startBit = (opdlFooter.GetByteCount(isUnsigned: true) - 1) * 8;

            for (var offset = 0; offset < 8; offset++)
            {
                if ((opdlFooter >> startBit - offset & 0xFF) == _eosMagic[0])
                {
                    return offset;
                }
            }
            return 8;
        }

        private void ReadOpdlHeader()
        {
            var header = new byte[13].AsSpan();
            _opdlStream.Read(header);

            var typeIdentifier = Encoding.ASCII.GetString(header[..4]);
            if (typeIdentifier != TYPE_IDENTIFIER)
                throw new InvalidDatalogFormatException($"Identifier \"{typeIdentifier}\" does not indicate a valid compressed FlashPro datalog");

            var storedDataSize = new BigInteger(header[9..12], isUnsigned: true, isBigEndian: true);
            StoredDataSize = (uint)storedDataSize;
        }

        private void BuildBz2Footer(ReadOnlySpan<byte> originalFooterBytes)
        {
            if (_bZipFooterStream.Length > 0)
                return;

            var opdlFooter = new BigInteger(originalFooterBytes, isUnsigned: true, isBigEndian: true);

            var offset = FindFooterOffset(opdlFooter);
            var byteAlignedOpdlfooter = opdlFooter << offset;
            var byteAlignedOpdlFooterBytes = byteAlignedOpdlfooter.ToByteArray(isUnsigned: true, isBigEndian: true).AsSpan();

            using (var byteAlignedBz2FooterStream = new MemoryStream())
            {
                var crcBytes = byteAlignedOpdlFooterBytes[^5..];

                byteAlignedBz2FooterStream.Write(byteAlignedOpdlFooterBytes[..1]);
                byteAlignedBz2FooterStream.Write(_eosMagic);
                byteAlignedBz2FooterStream.Write(crcBytes);

                var byteAlignedBz2FooterBytes = byteAlignedBz2FooterStream.ToArray();
                var byteAlignedBz2Footer = new BigInteger(byteAlignedBz2FooterBytes, isUnsigned: true, isBigEndian: true);
                var bz2Footer = byteAlignedBz2Footer >> offset;
                var bz2FooterBytes = bz2Footer.ToByteArray(isUnsigned: true, isBigEndian: true).AsSpan();

                _bZipFooterStream.Write(bz2FooterBytes[1..]);
                _bZipFooterStream.Seek(0, SeekOrigin.Begin);
            }
        }
    }
}