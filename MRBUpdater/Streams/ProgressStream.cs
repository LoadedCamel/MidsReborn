using System;
using System.IO;

namespace MRBUpdater
{
    public class ProgressStream : Stream
    {
        private readonly Stream _mInput;
        private readonly long _mLength;
        private long _mPosition;

        public ProgressStream()
        {
        }

        public ProgressStream(Stream input)
        {
            _mInput = input;
            _mLength = input.Length;
        }
        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var n = _mInput.Read(buffer, offset, count);
            _mPosition += n;
            //UpdateProgress?.Invoke(this, new ProgressEventArgs(1.0f * _mPosition / _mLength));
            return n;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _mLength;
        public override long Position
        {
            get => _mPosition;
            set => throw new NotImplementedException();
        }
    }
}
