using System.Drawing;
using System.IO;
using Newtonsoft.Json;

namespace Mids_Reborn.Core;

internal sealed class PetActorIconManifestEntry
{
    public string UID { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string DisplayKey { get; set; } = string.Empty;
}

public static class PetActorIconCatalog
{
    private static readonly object SyncRoot = new();
    private static readonly Dictionary<string, string> DisplayKeyByUid = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, string> DisplayKeyByName = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, Image> ImagesByKey = new(StringComparer.OrdinalIgnoreCase);
    private static bool _manifestLoaded;

    public static Image? GetIcon(RealPetActorRosterItem? actor)
    {
        return actor == null
            ? null
            : GetIcon(actor.EntityUid, actor.EntityDisplayName);
    }

    public static Image? GetIcon(string? entityUid, string? displayName)
    {
        EnsureManifestLoaded();

        var displayKey = ResolveDisplayKey(entityUid, displayName);
        if (string.IsNullOrWhiteSpace(displayKey))
        {
            return null;
        }

        lock (SyncRoot)
        {
            if (ImagesByKey.TryGetValue(displayKey, out var cached))
            {
                return cached;
            }

            var path = Path.Combine(GetAssetsDirectory(), $"{displayKey}.png");
            if (!File.Exists(path))
            {
                return null;
            }

            using var fileImage = Image.FromFile(path);
            var cloned = new Bitmap(fileImage);
            ImagesByKey[displayKey] = cloned;
            return cloned;
        }
    }

    private static void EnsureManifestLoaded()
    {
        lock (SyncRoot)
        {
            if (_manifestLoaded)
            {
                return;
            }

            _manifestLoaded = true;

            var manifestPath = Path.Combine(GetAssetsDirectory(), "real-pets.json");
            if (!File.Exists(manifestPath))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(manifestPath);
                var entries = JsonConvert.DeserializeObject<List<PetActorIconManifestEntry>>(json) ?? [];
                foreach (var entry in entries)
                {
                    if (!string.IsNullOrWhiteSpace(entry.UID) && !string.IsNullOrWhiteSpace(entry.DisplayKey))
                    {
                        DisplayKeyByUid[entry.UID] = entry.DisplayKey;
                    }

                    if (!string.IsNullOrWhiteSpace(entry.DisplayName) && !string.IsNullOrWhiteSpace(entry.DisplayKey))
                    {
                        DisplayKeyByName[NormalizeToken(entry.DisplayName)] = entry.DisplayKey;
                    }
                }
            }
            catch
            {
                // Ignore manifest failures and fall back to normalized filenames.
            }
        }
    }

    private static string? ResolveDisplayKey(string? entityUid, string? displayName)
    {
        if (!string.IsNullOrWhiteSpace(entityUid) && DisplayKeyByUid.TryGetValue(entityUid, out var keyByUid))
        {
            return keyByUid;
        }

        if (!string.IsNullOrWhiteSpace(displayName))
        {
            var normalizedName = NormalizeToken(displayName);
            if (DisplayKeyByName.TryGetValue(normalizedName, out var keyByName))
            {
                return keyByName;
            }

            return normalizedName;
        }

        if (!string.IsNullOrWhiteSpace(entityUid))
        {
            return NormalizeToken(entityUid);
        }

        return null;
    }

    private static string GetAssetsDirectory()
    {
        return Path.Combine(AppContext.BaseDirectory, "Assets", "PetActors");
    }

    private static string NormalizeToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalizedChars = value
            .Trim()
            .ToLowerInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_')
            .ToArray();

        var normalized = new string(normalizedChars);
        while (normalized.Contains("__", StringComparison.Ordinal))
        {
            normalized = normalized.Replace("__", "_", StringComparison.Ordinal);
        }

        return normalized.Trim('_');
    }
}
