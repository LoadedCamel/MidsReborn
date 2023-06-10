using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.Core.Base
{
    public class CrypticReplTable
    {
        private Dictionary<string, string> _table;
        public int Entries => _table.Count;
        private static CrypticReplTable _current { get; set; }
        public static CrypticReplTable Current
        {
            get
            {
                var tableData = _current;

                return tableData;
            }
        }

        private CrypticReplTable(string iFilename)
        {
            if (!File.Exists(iFilename)) throw new FileNotFoundException();

            var cnt = File.ReadAllText(iFilename).Trim();

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
            _table = new Dictionary<string, string>();

            r = new Regex(@"\,\s*");
            foreach (var l in lines)
            {
                var line = l.Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;

                var chunks = r.Split(line);
                if (chunks.Length != 2) continue;

                var id1 = chunks[0];
                var id2 = chunks[1];

                if (string.Equals(id1.ToLowerInvariant(), id2.ToLowerInvariant(), StringComparison.InvariantCultureIgnoreCase)) continue;

                _table.Add(id1, id2);
            }
        }

        public static void Initialize()
        {
            _current = new CrypticReplTable(Files.CNamePowersRepl);
            var pass1Count = _current._table.Count;
            _current.CheckConsistency();
        }

        public bool KeyExists(string id)
        {
            return _table.Any(item => item.Key == id);
        }

        public string FetchAlternate(string id)
        {
            return _table.ContainsKey(id)
                ? _table[id]
                : "";
        }

        public string FetchSource(string id)
        {
            foreach (var item in _table)
            {
                if (item.Value != id) continue;

                return item.Key;
            }

            return "";
        }

        private void CheckConsistency()
        {
            var itemsToRemove = new List<KeyValuePair<string, string>>();
            var counters = new Dictionary<string, int>();
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
                    .FirstOrDefault(e => e.FullName == item.Key);

                var powerName = power.StaticIndex == -1 ? "" : power.FullName;
                if (powerName == "")
                {
                    itemsToRemove.Add(item);
                    MessageBox.Show(
                        $"Warning: power ID {item.Key} can be converted to ID {item.Value} but the source power doesn't exist.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            if (itemsToRemove.Count <= 0) return;

            // Remove invalid items and reindex
            var tableTempCopy = new Dictionary<string, string>();
            var j = 0;
            foreach (var e in _table)
            {
                if (e.Key == itemsToRemove[j].Key & e.Value == itemsToRemove[j].Value)
                {
                    j++;
                    continue;
                }

                tableTempCopy.Add(e.Key, e.Value);
            }

            _table = tableTempCopy.Clone();
        }
    }
}
