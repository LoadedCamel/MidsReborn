using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using mrbBase.Base.Data_Classes;
using FastDeepCloner;

namespace mrbBase
{
    public class PowersReplTable
    {
        private List<KeyValuePair<int, int>> _table;
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
            _table = new List<KeyValuePair<int, int>>();

            foreach (var l in lines)
            {
                var line = l.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                r = new Regex(@"\,\s*");
                var chunks = r.Split(line);
                if (chunks.Length != 2) continue;

                var res1 = int.TryParse(chunks[0], out var id1);
                var res2 = int.TryParse(chunks[1], out var id2);

                if (!res1 | !res2) continue;

                _table.Add(new KeyValuePair<int, int>(id1, id2));
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
            return _table.Any(item => item.Key == id);
        }

        public int FetchAlternate(int oldId)
        {
            foreach (var item in _table)
            {
                if (item.Key == oldId) return item.Value;
            }

            return -1;
        }

        private void CheckConsistency()
        {
            var itemsToRemove = new List<KeyValuePair<int, int>>();
            var counters = new Dictionary<int, int>();
            foreach (var item in _table)
            {
                if (!counters.ContainsKey(item.Key))
                {
                    counters.Add(item.Key, 1);
                }
                else
                {
                    counters[item.Key]++;
                    itemsToRemove.Add(item);
                    MessageBox.Show(
                        $"Warning: duplicate input power ID {item.Key} found.\r\nPlease ensure input IDs are unique.\r\nThe replacement pair <{item.Key}, {item.Value}> will be disabled.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                var power = DatabaseAPI.Database.Power
                    .DefaultIfEmpty(new Power { StaticIndex = -1 })
                    .FirstOrDefault(e => e.StaticIndex == item.Value);

                var powerName = power.StaticIndex == -1 ? "" : power.FullName;
                if (powerName == "")
                {
                    itemsToRemove.Add(item);
                    MessageBox.Show(
                        $"Warning: power ID {item.Key} can be converted to ID {item.Value} but the matching power doesn't exist.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (itemsToRemove.Count <= 0) return;

            // Remove invalid items and reindex
            var tableTempCopy = new List<KeyValuePair<int, int>>();
            var j = 0;
            foreach (var e in _table)
            {
                if (e.Key == itemsToRemove[j].Key & e.Value == itemsToRemove[j].Value)
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
            if (!EnableDebug) return;
            
            Debug.WriteLine($"Dump() - PowersReplTable Count: {_table.Count}");
            foreach (var item in _table)
            {
                var power = DatabaseAPI.Database.Power
                    .DefaultIfEmpty(new Power {StaticIndex = -1})
                    .FirstOrDefault(e => e.StaticIndex == item.Value);
                
                var powerName = power.StaticIndex == -1 ? "" : power.FullName;

                Debug.WriteLine($"{item.Key} --> {item.Value} [{(powerName == "" ? "<null>" : powerName)}]");
            }
        }
    }
}