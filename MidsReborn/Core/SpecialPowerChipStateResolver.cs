namespace Mids_Reborn.Core;

public static class SpecialPowerChipStateResolver
{
    public static string ResolveSelectedChipKey(
        string? preferredKey,
        IReadOnlyList<SpecialPowerChipOption>? chipOptions,
        string? fallbackKey = null)
    {
        if (chipOptions == null || chipOptions.Count == 0)
        {
            return string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(preferredKey))
        {
            var preferred = chipOptions.FirstOrDefault(option =>
                string.Equals(option.Key, preferredKey, StringComparison.OrdinalIgnoreCase));
            if (preferred != null)
            {
                return preferred.Key;
            }
        }

        if (!string.IsNullOrWhiteSpace(fallbackKey))
        {
            var fallback = chipOptions.FirstOrDefault(option =>
                string.Equals(option.Key, fallbackKey, StringComparison.OrdinalIgnoreCase));
            if (fallback != null)
            {
                return fallback.Key;
            }
        }

        return chipOptions[0].Key;
    }
}
