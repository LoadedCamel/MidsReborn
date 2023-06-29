using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Mids_Reborn.Core.Utils
{
    internal class ProgressFileStream : FileStream
    {
        public int Progress;

        public ProgressFileStream(string path, FileMode mode) : base(path, mode)
        {
            Progress = 0;
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
