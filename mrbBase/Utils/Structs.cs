namespace mrbBase.Utils
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
}
