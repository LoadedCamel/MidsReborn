namespace MRB_Boostrap.Models
{
    // Clone of MidsReborn/UI/Forms/UpdateSystem/Models/Manifest.cs
    public class Manifest
    {
        public string ManifestVersion { get; set; } = "3.0";
        public List<UpdateEntry> Updates { get; set; } = [];
        public string LastUpdated { get; set; } = string.Empty;
    }
}
