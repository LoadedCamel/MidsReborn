using System;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace mrbBase.Utils.DataCompression
{
    public class CompressionStream : Stream
    {
        private readonly Stream _internalStream;

        public CompressionStream(CompressionData data)
        {
            var encoded = Encoding.ASCII.GetBytes(data.EncodedString);
            var decodedBytes = new byte[0];
            var compressionStream = new MemoryStream(decodedBytes);
            _internalStream = new InflaterInputStream(compressionStream);
        }

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => _internalStream.Length;
        public override long Position
        {
            get => _internalStream.Position;
            set => _internalStream.Position = value;
        }

        public override void Flush()
        {
            _internalStream.Flush();
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _internalStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _internalStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _internalStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _internalStream.Write(buffer, offset, count);
        }
    }
}
