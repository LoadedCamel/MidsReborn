using System;

namespace Mids_Reborn.Core.BuildFile.DataModels
{
    public class MetaData
    {
        public string App { get; set; }
        public Version Version { get; set; }
        public string Database { get; set; }
        public Version DatabaseVersion { get; set; }

        public MetaData(string app, Version version, string database, Version databaseVersion)
        {
            App = app;
            Version = version;
            Database = database;
            DatabaseVersion = databaseVersion;
        }
    }
}
