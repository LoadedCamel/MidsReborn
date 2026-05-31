using System;

namespace Mids_Reborn.Core.Compatibility
{
    internal static class LegacyCompatibilityRegistry
    {
        public static bool TryGetCurrent(out LegacyHomecomingMap? map, out string error)
        {
            return TryGetForDatabase(DatabaseAPI.DatabaseName, out map, out error);
        }

        public static bool TryGetForDatabase(string? databaseName, out LegacyHomecomingMap? map, out string error)
        {
            map = null;
            error = string.Empty;

            if (!string.Equals(databaseName, "Homecoming", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            try
            {
                map = LegacyHomecomingMap.Instance;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
