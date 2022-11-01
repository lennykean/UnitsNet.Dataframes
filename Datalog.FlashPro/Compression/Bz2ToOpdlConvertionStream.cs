using System;
using System.IO;
using System.Numerics;
using System.Text;

using HondataDotNet.Datalog.Core;

namespace HondataDotNet.Datalog.FlashPro.Compression
{
    public class Bz2ToOpdlConvertionStream : Stream
    {
        private const int WRITE_BUFFER_SIZE = 11;

        private static readonly byte[] _opdlHeader1 = { 0x4F, 0x50, 0x44, 0x4C, 0x06, 0x00, 0x43, 0x50, 0 };
        private static readonly byte[] _opdlHeader2 = { 0x05 };

        private readonly Stream _innerStream;
        private readonly MemoryStream _bzip2Header;
        private readonly uint _uncompressedDataSize;
        
        private MemoryStream _writeBuffer;
        private bool _isDisposed;

        public Bz2ToOpdlConvertionStream(Stream innerStream, uint uncompressedDataSize = 0)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _bzip2Header = new MemoryStream();
            _writeBuffer = new MemoryStream();
            _uncompressedDataSize = uncompressedDataSize;
        }

        ~Bz2ToOpdlConvertionStream()
        {
            Dispose(false);
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => _innerStream.Length;

        public override long Position
        {
            get => _innerStream.Position;
            set => throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            _writeBuffer.Write(buffer, offset, count);
            if (_writeBuffer.Position > WRITE_BUFFER_SIZE)
                FlushBytes(_writeBuffer.Position - WRITE_BUFFER_SIZE);
        }

        private void FlushBytes(long bytes)
        {
            var buffer = new byte[bytes];

            _writeBuffer.Seek(0, SeekOrigin.Begin);
            _writeBuffer.Read(buffer);

            InnerWrite(buffer, 0, (int)bytes);

            buffer = new byte[_writeBuffer.Length - _writeBuffer.Position];
            _writeBuffer.Read(buffer);

            _writeBuffer = new MemoryStream();
            _writeBuffer.Write(buffer);
        }

        public override void Flush()
        {
            _innerStream.Flush();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                _isDisposed = true;
                WriteOpdlFooter();
            }
            base.Dispose(disposing);
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

        private void InnerWrite(byte[] buffer, int offset, int count)
        {
            var headerBytes = 0;

            if (_bzip2Header.Length < 4)
            {
                headerBytes = (4 - (int)_bzip2Header.Length);

                if (headerBytes > count)
                    headerBytes = count;

                _bzip2Header.Write(buffer, offset, headerBytes);
                if (_bzip2Header.Length == 4)
                {
                    ReadBz2Header();
                    WriteOpdlHeader(_uncompressedDataSize);
                }
            }
            _innerStream.Write(buffer, offset + headerBytes, count - headerBytes);
        }

        private void ReadBz2Header()
        {
            var header = new byte[4];

            _bzip2Header.Seek(0, SeekOrigin.Begin);
            _bzip2Header.Read(header);

            var signature = Encoding.ASCII.GetString(header, 0, 2);
            if (signature != "BZ")
                throw new InvalidDatalogFormatException($"Signature \"{signature}\" does not indicate bzip2 data");

            var version = Encoding.ASCII.GetString(header, 2, 1);
            if (version != "h")
                throw new InvalidDatalogFormatException($"Invalid bzip2 version \"{signature}\"");

            var blockSize = Encoding.ASCII.GetChars(header, 3, 1);
            if (blockSize[0] < '1' || blockSize[0] > '5')
                throw new InvalidDatalogFormatException($"Unsupported block size \"{blockSize[0]}\"");
        }

        public void WriteOpdlHeader(uint uncompressedDataSize)
        {
            var position = _innerStream.Position;

            if (position != 0)
                _innerStream.Seek(0, SeekOrigin.Begin);

            var uncompressedDataSizeBytes = new byte[3];
            var uncompressedDataSizeBigInt = new BigInteger(uncompressedDataSize);
            uncompressedDataSizeBigInt.TryWriteBytes(uncompressedDataSizeBytes, out _, isUnsigned: true, isBigEndian: true);

            _innerStream.Write(_opdlHeader1);
            _innerStream.Write(uncompressedDataSizeBytes);
            _innerStream.Write(_opdlHeader2);

            if (position != 0)
                _innerStream.Seek(position, SeekOrigin.Begin);
        }

        private static int FindFooterOffset(BigInteger originalFooter)
        {
            var startBit = (originalFooter.GetByteCount(isUnsigned: true) - 1) * 8;

            for (var offset = 0; offset < 8; offset++)
            {
                if ((originalFooter >> startBit - offset & 0xFF) == 0x17)
                {
                    return offset;
                }
            }
            return 8;
        }

        private void WriteOpdlFooter()
        {
            var originalFooter = new BigInteger(_writeBuffer.ToArray(), isUnsigned: true, isBigEndian: true);

            var offset = FindFooterOffset(originalFooter);
            var byteAlignedBz2footer = originalFooter << offset;
            var byteAlignedBz2FooterBytes = byteAlignedBz2footer.ToByteArray(isUnsigned: true, isBigEndian: true);

            using (var byteAlignedOpdlFooterStream = new MemoryStream())
            {
                var crcBytes = byteAlignedBz2FooterBytes.AsSpan()[^5..];

                byteAlignedOpdlFooterStream.Write(byteAlignedBz2FooterBytes.AsSpan()[..1]);
                byteAlignedOpdlFooterStream.WriteByte(0x17);
                byteAlignedOpdlFooterStream.Write(crcBytes);

                var byteAlignedOpdlFooterBytes = byteAlignedOpdlFooterStream.ToArray();
                var byteAlignedOpdlFooter = new BigInteger(byteAlignedOpdlFooterBytes, isUnsigned: true, isBigEndian: true);
                var opdlFooter = byteAlignedOpdlFooter >> offset;
                var opdlFooterBytes = opdlFooter.ToByteArray(isUnsigned: true, isBigEndian: true);

                _innerStream.Write(opdlFooterBytes);
            }
        }
    }
}