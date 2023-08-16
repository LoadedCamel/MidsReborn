namespace Mids_Reborn.Core.Utils
{
    public class FormatCode
    {
        public string Name { get; set; }
        public ExportFormatType Type { get; set; }

        public FormatCode(string name, ExportFormatType type)
        {
            Name = name;
            Type = type;
        }
    }
}
