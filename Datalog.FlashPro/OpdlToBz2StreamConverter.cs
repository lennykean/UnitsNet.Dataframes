
using System;
using System.IO;
using System.Numerics;

namespace HondataDotNet.Datalog.FlashPro
{
    public class OpdlToBz2StreamConverter : Stream
    {
        const int READ_AHEAD = 6;

        private static readonly byte[] _magicHeader = new byte[] { 0x42, 0x5a, 0x68, 0x38 };
        private static readonly byte[] _magicFooter = new byte[] { 0x17, 0x72, 0x45, 0x38, 0x50, 0x90 };

        private readonly MemoryStream _bZipHeaderStream;
        private readonly MemoryStream _bZipFooterStream;
        private readonly MemoryStream _readAheadBuffer;
        private readonly Stream _opdlStream;

        public OpdlToBz2StreamConverter(Stream opdlStream)
        {
            _opdlStream = opdlStream;
            _bZipHeaderStream = new MemoryStream(_magicHeader);
            _bZipFooterStream = new MemoryStream(); 
            _readAheadBuffer = new MemoryStream();
        }

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
            {
                var opdlHeader = new byte[13];
                 _opdlStream.Read(opdlHeader);
            }
            
            var read = _bZipHeaderStream.Read(buffer, offset, count);

            while (read < count)
            {
                var bufferRead = _readAheadBuffer.Read(buffer, offset + read, count - read);
                read += bufferRead;

                var bufferBytes = bufferRead;
                if (_readAheadBuffer.Length == 0)
                    bufferBytes = READ_AHEAD;

                if (bufferBytes == 0)
                    break;

                var opdlBuffer = new byte[bufferBytes];
                var opdlRead = _opdlStream.Read(opdlBuffer);
                if (opdlRead == 0)
                {
                    BuildFooter();
                    _readAheadBuffer.Seek(0, SeekOrigin.End);
                    continue;
                }

                var readAheadBufferBuffer = new byte[_readAheadBuffer.Length - _readAheadBuffer.Position];
                _readAheadBuffer.Read(readAheadBufferBuffer);
                _readAheadBuffer.Seek(0, SeekOrigin.Begin);
                _readAheadBuffer.Write(readAheadBufferBuffer);
                _readAheadBuffer.Write(opdlBuffer, 0, opdlRead);
                _readAheadBuffer.Seek(0, SeekOrigin.Begin);                
            }
            if (read < count)
            {
                read += _bZipFooterStream.Read(buffer, offset + read, count - read);
            }
            return read;
        }

        private void BuildFooter()
        {
            if (_bZipFooterStream.Length > 0)
                return;

            var originalFooter = new BigInteger(_readAheadBuffer.ToArray(), isUnsigned: true, isBigEndian: true);

            var offset = FindFooterOffset(originalFooter);
            var byteAlignedOriginalfooter = originalFooter << offset;
            var byteAlignedOriginalFooterBytes = byteAlignedOriginalfooter.ToByteArray(isUnsigned: true, isBigEndian: true);

            using (var byteAlignedBz2FooterStream = new MemoryStream())
            {
                var crcBytes = byteAlignedOriginalFooterBytes.AsSpan().Slice(byteAlignedOriginalFooterBytes.Length - (offset > 0 ? 5 : 4), (offset > 0 ? 5 : 4));

                byteAlignedBz2FooterStream.Write(_magicFooter);
                byteAlignedBz2FooterStream.Write(crcBytes);

                var byteAlignedBz2Footer = new BigInteger(byteAlignedBz2FooterStream.ToArray(), isUnsigned: true, isBigEndian: true);
                var bz2Footer = byteAlignedBz2Footer >> offset;

                _bZipFooterStream.Write(bz2Footer.ToByteArray(isUnsigned: true, isBigEndian: true));
                _bZipFooterStream.Seek(0, SeekOrigin.Begin);
            }
        }

        private static int FindFooterOffset(BigInteger originalFooter)
        {
            var startBit = (originalFooter.GetByteCount(isUnsigned: true) - 1) * 8;

            for (var offset = 0; offset < 8; offset++)
            {
                if (((originalFooter >> (startBit - offset)) & 0xFF) == _magicFooter[0])
                {
                    return offset;
                }
            }
            return 8;
        }
    }
}