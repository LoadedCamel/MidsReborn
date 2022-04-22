using System;
using System.IO;

namespace MRBUpdater.Utils
{
    public static class StreamUtils
    {
        public static void Copy(Stream source, Stream destination, byte[] buffer)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException(@"Buffer is too small", nameof(buffer));
            }

            var copying = true;

            while (copying)
            {
                var bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }
            }
        }

        public static void Copy(Stream source, Stream destination, byte[] buffer, ProgressEventHandler progressHandler, TimeSpan updateInterval, object sender, string name, float fixedTarget = -1f)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
            {
                throw new ArgumentException(@"Buffer is too small", nameof(buffer));
            }

            if (progressHandler == null)
            {
                throw new ArgumentNullException(nameof(progressHandler));
            }

            var copying = true;

            var marker = DateTime.Now;
            float processed = 0;
            float target = 0;

            if (fixedTarget >= 0)
            {
                target = fixedTarget;
            }
            else if (source.CanSeek)
            {
                target = source.Length - source.Position;
            }

            // Always fire 0% progress..
            var args = new ProgressEventArgs(name, processed, target);
            progressHandler(sender, args);

            var progressFired = true;

            while (copying)
            {
                var bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    processed += bytesRead;
                    progressFired = false;
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }

                if (DateTime.Now - marker <= updateInterval) continue;
                progressFired = true;
                marker = DateTime.Now;
                args = new ProgressEventArgs(name, processed, target);
                progressHandler(sender, args);

                copying = args.ContinueRunning;
            }

            if (progressFired) return;
            args = new ProgressEventArgs(name, processed, target);
            progressHandler(sender, args);
        }
	}
}
