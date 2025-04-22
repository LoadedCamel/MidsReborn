using System;
using System.Drawing;

namespace Mids_Reborn.Core.Utils
{
    public struct CompressionResult
    {
        public readonly string OutString;
        public readonly int UncompressedSize;
        public readonly int CompressedSize;
        public readonly int EncodedSize;

        public CompressionResult(string outString, int uncompressedSize, int compressedSize, int encodedSize)
        {
            OutString = outString;
            UncompressedSize = uncompressedSize;
            CompressedSize = compressedSize;
            EncodedSize = encodedSize;
        }
    }

    public struct TypeGrade
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
    }

    public struct FileData
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string Path { get; set; }
    }

    public struct DatabaseItems
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public enum PatchType
    {
        Application,
        Database
    }

    public enum ManifestStatus
    {
        Unavailable,
        Failure,
        Success
    }

    [Flags]
    public enum NavItemState
    {
        Active = 0,
        Disabled = 1,
        Inactive = 2,
    }

    [Flags]
    public enum NavLayout
    {
        Horizontal = 0,
        Vertical = 1
    }

    public enum ThemeFilter
    {
        Any,
        Light,
        Dark,
    }

    public enum ExportFormatType
    {
        None,
        BbCode,
        MarkdownHtml,
        Markdown,
        Html
    }
}
