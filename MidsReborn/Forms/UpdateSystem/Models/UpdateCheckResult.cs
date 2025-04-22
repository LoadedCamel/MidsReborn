namespace Mids_Reborn.Forms.UpdateSystem.Models
{
    public class UpdateCheckResult
    {
        public bool IsAppUpdateAvailable { get; set; }
        public string? AppVersion { get; set; }
        public string? AppFile { get; set; }
        public string? AppName { get; set; }

        public bool IsDbUpdateAvailable { get; set; }
        public string? DbVersion { get; set; }
        public string? DbFile { get; set; }
        public string? DbName { get; set; }
    }
}
