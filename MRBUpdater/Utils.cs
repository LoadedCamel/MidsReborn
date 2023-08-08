namespace MRBUpdater
{
    internal static class Utils
    {
        public enum UpdateTypes
        {
            Application,
            Database
        }

        public struct UpdateDetails
        {
            public UpdateTypes Type { get; set; }

            public string Uri { get; set; }

            public string Version { get; set; }

            public string File { get; set; }

            public string ExtractTo { get; set; }
        }

        public struct InstallDetails
        {
            public string File { get; set; }
            public string ExtractPath { get; set; }

            public InstallDetails(string file, string extractPath)
            {
                File = file;
                ExtractPath = extractPath;
            }
        }
    }
}
