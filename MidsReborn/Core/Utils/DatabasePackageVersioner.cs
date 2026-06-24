using System;

namespace Mids_Reborn.Core.Utils
{
    public static class DatabasePackageVersioner
    {
        private const int DailySequenceMultiplier = 100;
        private const int MaxDailySequence = 99;

        public static Version GetNextVersion(Version? currentVersion, DateTime timestamp)
        {
            var dayBuildBase = timestamp.Day * DailySequenceMultiplier;
            var firstVersionForDate = new Version(timestamp.Year, timestamp.Month, dayBuildBase + 1);

            if (currentVersion == null || currentVersion < firstVersionForDate)
            {
                return firstVersionForDate;
            }

            if (currentVersion.Major == timestamp.Year &&
                currentVersion.Minor == timestamp.Month &&
                currentVersion.Build >= dayBuildBase &&
                currentVersion.Build < dayBuildBase + MaxDailySequence)
            {
                return new Version(timestamp.Year, timestamp.Month, currentVersion.Build + 1);
            }

            if (currentVersion.Build == int.MaxValue)
            {
                throw new InvalidOperationException("Database package version build component is already at its maximum value.");
            }

            return new Version(currentVersion.Major, currentVersion.Minor, currentVersion.Build + 1);
        }

        public static Version Stamp(IDatabase database, DateTime? timestamp = null)
        {
            ArgumentNullException.ThrowIfNull(database);

            var effectiveTimestamp = timestamp ?? DateTime.Now;
            database.Date = effectiveTimestamp;
            database.Version = GetNextVersion(database.Version, effectiveTimestamp);

            return database.Version;
        }
    }
}
