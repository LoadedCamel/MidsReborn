namespace MRB_Boostrap.Models
{
    public class UpdateEntry
    {
        public string Type { get; set; } = "";
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
        public string File { get; set; } = "";
        public string? SourceUri { get; set; } = "";
    }
}
