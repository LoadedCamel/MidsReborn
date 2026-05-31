using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Compatibility
{
    public enum LegacyLineage
    {
        Unknown = 0,
        Homecoming = 1,
        Rebirth = 2
    }

    internal enum LegacyMapKind
    {
        Power = 0,
        Enhancement = 1,
        EnhancementSet = 2
    }

    internal enum LegacyEnhancementEncoding
    {
        None = 0,
        NormalLike = 1,
        InventLike = 2
    }

    internal sealed record LegacyMapEntry(
        LegacyMapKind Kind,
        int? StaticIndex,
        string LegacyName,
        string LegacyFullName,
        string CurrentTarget,
        int SourceLine);

    public sealed class CompatibilityFailureItem
    {
        public string Category { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? PowerIndex { get; set; }
        public int? SlotIndex { get; set; }
        public int? StaticIndex { get; set; }
        public string LegacyName { get; set; } = string.Empty;
        public string LegacyFullName { get; set; } = string.Empty;
        public string CurrentTarget { get; set; } = string.Empty;
    }

    public sealed class CompatibilityFailureReport
    {
        public string Title { get; set; } = "Compatibility Load Failed";
        public string SourceName { get; set; } = string.Empty;
        public LegacyLineage Lineage { get; set; } = LegacyLineage.Unknown;
        public string LegacyTag { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<CompatibilityFailureItem> Items { get; } = [];

        public bool HasItems => Items.Count > 0;

        public string BuildDisplayMessage()
        {
            var lines = new List<string>();
            if (!string.IsNullOrWhiteSpace(Summary))
            {
                lines.Add(Summary.Trim());
            }

            if (!string.IsNullOrWhiteSpace(LegacyTag))
            {
                lines.Add($"Legacy version: {LegacyTag}");
            }

            if (HasItems)
            {
                lines.Add(string.Empty);
                foreach (var item in Items.Take(10))
                {
                    lines.Add("- " + item.Message);
                }

                if (Items.Count > 10)
                {
                    lines.Add($"- ...and {Items.Count - 10} more issue(s).");
                }
            }

            return string.Join(Environment.NewLine, lines.Where(line => line != null));
        }

        public string BuildDiagnosticJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public sealed class CompatibilityLoadSummary
    {
        public string SourceName { get; set; } = string.Empty;
        public LegacyLineage Lineage { get; set; } = LegacyLineage.Unknown;
        public string LegacyTag { get; set; } = string.Empty;
        public int RemappedPowerCount { get; set; }
        public int RemappedEnhancementCount { get; set; }

        public bool HasUserVisibleChanges => false;

        public bool HasChanges => RemappedPowerCount > 0 || RemappedEnhancementCount > 0;

        public string BuildDisplayMessage()
        {
            return string.Empty;
        }
    }
}
