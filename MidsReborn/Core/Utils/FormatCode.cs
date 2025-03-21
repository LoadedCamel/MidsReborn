namespace Mids_Reborn.Core.Utils
{
    public class FormatCode(string name = "Plain Text", ExportFormatType type = ExportFormatType.None)
    {
        public string Name { get; set; } = name;
        public ExportFormatType Type { get; set; } = type;
    }
}
