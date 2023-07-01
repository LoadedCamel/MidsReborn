using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace Mids_Reborn.Core.Utils
{
    internal class ProgressZipStream : ZipOutputStream
    {
        public int Progress;

        public ProgressZipStream(Stream stream) : base(stream)
        {
            Progress = 0;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Progress += count;
            return base.Read(buffer, offset, count);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Progress += count;
            return base.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Progress += count;
            base.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            Progress += count;
            return base.WriteAsync(buffer, offset, count, cancellationToken);
        }
    }
}
