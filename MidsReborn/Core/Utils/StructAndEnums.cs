using System;

namespace Mids_Reborn.Core.Utils
{
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
        None,
        Application,
        Database
    }

    public enum ManifestStatus
    {
        Unavailable,
        Failure,
        Success
    }

    public struct UpdateObject
    {
        public PatchType Type { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string Version { get; set; }
        public string File { get; set; }
        public string ExtractTo { get; set; }

        public UpdateObject(PatchType type, string name, string uri, string ver, string file, string extract)
        {
            Type = type;
            Name = name;
            Uri = uri;
            Version = ver;
            File = file;
            ExtractTo = extract;
        }
    }
}
