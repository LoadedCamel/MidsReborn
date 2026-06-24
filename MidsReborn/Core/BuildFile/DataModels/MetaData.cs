using System;

namespace Mids_Reborn.Core.BuildFile.DataModels
{
    public class MetaData
    {
        public string App { get; set; }
        public Version Version { get; set; }
        public string Database { get; set; }
        public Version DatabaseVersion { get; set; }
        public string DatabaseProvider { get; set; }

        public MetaData()
        {
            App = string.Empty;
            Version = new Version(0, 0);
            Database = string.Empty;
            DatabaseVersion = new Version(0, 0);
            DatabaseProvider = string.Empty;
        }

        public MetaData(string app, Version version, string database, Version databaseVersion, string? databaseProvider = null)
        {
            App = app;
            Version = version;
            Database = database;
            DatabaseVersion = databaseVersion;
            DatabaseProvider = databaseProvider ?? string.Empty;
        }
    }
}
