using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core
{
    public class PowersReplTable
    {
        private struct AlternateEntry
        {
            public int SourcePowerId;
            public int TargetPowerId;
            public string Archetype;
        }

        private List<AlternateEntry> _table;
        public int Entries => _table.Count;
        private static PowersReplTable _current { get; set; }
        private const bool EnableDebug = false;

        internal static PowersReplTable Current
        {
            get
            {
                var tableData = _current;
                return tableData;
            }
        }

        private PowersReplTable(string iFilename)
        {
            if (!File.Exists(iFilename)) throw new FileNotFoundException();

            var cnt = "";
            cnt = File.ReadAllText(iFilename).Trim();

            cnt = cnt.Replace("\r", "\n");
            cnt = cnt.Replace("\r\n", "\n");

            // Remove blank lines
            var r = new Regex(@"\n{2,}");
            cnt = r.Replace(cnt, "\n");

            // Remove comments
            r = new Regex(@"\s*\#.*");
            cnt = r.Replace(cnt, "");

            // Split by lines
            var lines = cnt.Split('\n');
            _table = new List<AlternateEntry>();

            var rItems = new Regex(@"\,\s*");
            var rBlocks = new Regex(@"^\[([a-zA-Z\s]+)\]");
            var block = "";

            foreach (var l in lines)
            {
                var line = l.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                if (rBlocks.IsMatch(line))
                {
                    var m = rBlocks.Match(line);
                    block = m.Groups[1].Value.ToLowerInvariant();
                    if (block == "global")
                    {
                        block = "";
                    }

                    continue;
                }

                if (!rItems.IsMatch(line)) continue;

                var chunks = rItems.Split(line);
                if (chunks.Length != 2) continue;

                var res1 = int.TryParse(chunks[0], out var id1);
                var res2 = int.TryParse(chunks[1], out var id2);

                if (!res1 | !res2) continue;

                _table.Add(new AlternateEntry { SourcePowerId = id1, TargetPowerId = id2, Archetype = block });
            }
        }

        public static void Initialize()
        {
            if (EnableDebug) Debug.WriteLine($"Loading PowersReplTable from {Files.FNamePowersRepl}");
            _current = new PowersReplTable(Files.FNamePowersRepl);
            var pass1Count = _current._table.Count;
            if (EnableDebug) Debug.WriteLine($"PowersReplTable Count (pass 1): {pass1Count}");
            _current.CheckConsistency();
            if (!EnableDebug) return;
            Debug.WriteLine($"PowersReplTable Count (pass 2): {_current._table.Count}");
            Debug.WriteLine($"{(pass1Count == _current._table.Count ? "OK" : $"{pass1Count - _current._table.Count} invalid item(s) removed")}");
            Debug.WriteLine("");
            _current.Dump();
        }

        public bool KeyExists(int id)
        {
            return _table.Any(item => item.SourcePowerId == id);
        }

        public int FetchAlternate(int oldId, string archetype = "")
        {
            archetype = archetype.ToLowerInvariant();

            foreach (var item in _table)
            {
                if (item.SourcePowerId == oldId & archetype == "") return item.TargetPowerId;
                if (item.SourcePowerId == oldId & archetype != "" & archetype == item.Archetype) return item.TargetPowerId;
            }

            return -1;
        }

        private static string FirstCharToUpper(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        private void CheckConsistency()
        {
            var itemsToRemove = new List<AlternateEntry>();
            var counters = new Dictionary<int, int>();
            foreach (var item in _table)
            {
                if (!counters.ContainsKey(item.SourcePowerId))
                {
                    counters.Add(item.SourcePowerId, 1);
                }
                else
                {
                    counters[item.SourcePowerId]++;
                    itemsToRemove.Add(item);
                    MessageBox.Show(
                        $"Warning: duplicate input power ID {item.SourcePowerId} found.\r\nPlease ensure input IDs are unique.\r\nThe replacement pair <{item.SourcePowerId}, {item.TargetPowerId}> {(item.Archetype == "" ? "" : $"({FirstCharToUpper(item.Archetype)}) ")}will be disabled.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                var power = DatabaseAPI.Database.Power
                    .DefaultIfEmpty(new Power { StaticIndex = -1 })
                    .FirstOrDefault(e => e.StaticIndex == item.TargetPowerId);

                var powerName = power.StaticIndex == -1 ? "" : power.FullName;
                if (powerName != "") continue;

                itemsToRemove.Add(item);
                MessageBox.Show(
                    $"Warning: power ID {item.SourcePowerId} can be converted to ID {item.TargetPowerId} but the matching power doesn't exist.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (itemsToRemove.Count <= 0) return;

            // Remove invalid items and reindex
            var tableTempCopy = new List<AlternateEntry>();
            var j = 0;
            foreach (var e in _table)
            {
                if (e.SourcePowerId == itemsToRemove[j].SourcePowerId & e.TargetPowerId == itemsToRemove[j].TargetPowerId)
                {
                    j++;
                    continue;
                }

                tableTempCopy.Add(e);
            }

            _table = tableTempCopy.Clone();
        }

        public void Dump()
        {
            if (!Debugger.IsAttached && !Process.GetCurrentProcess().ProcessName.ToLowerInvariant().Contains("devenv")) return;

            Debug.WriteLine($"Dump() - PowersReplTable Count: {_table.Count}");
            foreach (var item in _table)
            {
                var power = DatabaseAPI.Database.Power
                    .DefaultIfEmpty(new Power { StaticIndex = -1 })
                    .FirstOrDefault(e => e.StaticIndex == item.TargetPowerId);

                var powerName = power.StaticIndex == -1 ? "" : power.FullName;

                Debug.WriteLine($"{item.SourcePowerId} --> {item.TargetPowerId} {(item.Archetype == "" ? "(Global)" : $"({FirstCharToUpper(item.Archetype)})")} [{(powerName == "" ? "<null>" : powerName)}]");
            }
        }
    }
}