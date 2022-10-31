
using System;
using System.IO;

namespace HondataDotNet.Datalog.FlashPro.Compression
{
    public class Bz2ToOpdlConvertionStream : Stream
    {
        private readonly Stream _bz2Stream;

        public Bz2ToOpdlConvertionStream(Stream bz2Stream)
        {
            _bz2Stream = bz2Stream ?? throw new ArgumentNullException(nameof(bz2Stream));
        }

        internal uint StoredDataSize { get; private set; }

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => _bz2Stream.CanWrite;

        public override long Length => _bz2Stream.Length;

        public override long Position
        {
            get => _bz2Stream.Position;
            set => throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

        public override void SetLength(long value) => throw new NotImplementedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
        }

        public override void Flush()
        {
            _bz2Stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();
    }
}