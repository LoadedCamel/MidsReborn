using System;

namespace Mids_Reborn.Core
{
    public sealed class ExportBuilder : IDisposable
    {
        private string _export;
        private bool _disposed;

        public ExportBuilder(string export)
        {
            _export = export;
        }

        public void Append(string text)
        {
            _export += $" {text}";
        }

        public void AppendLine(string text)
        {
            _export += $"\r\n{text}";
        }

        public override string ToString()
        {
            return _export;
        }

        public void Dispose()
        {
            Dispose(true);
            // ReSharper disable once GCSuppressFinalizeForTypeWithoutDestructor
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) 
            {
                _export = string.Empty;
            }

            _disposed = true;
        }
    }
}
