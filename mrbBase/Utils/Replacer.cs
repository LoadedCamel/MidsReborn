using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using mrbBase.Base.Master_Classes;
using Newtonsoft.Json;

namespace mrbBase.Utils
{
    public class Replacer
    {
        private static Replacer? _replacerInstance;
        private static List<Item>? Items { get; set; } = new();
        private readonly string _replacerFile = Path.Combine(MidsContext.Config.DataPath, "RepData.json");

        private Replacer()
        {
            var data = string.Empty;
            if (File.Exists(_replacerFile))
            {
                data = File.ReadAllText(_replacerFile);
            }

            if (!string.IsNullOrWhiteSpace(data))
            {
                Items = JsonConvert.DeserializeObject<List<Item>>(data);
            }
        }

        public static Replacer GetReplacer()
        {
            if (_replacerInstance != null) return _replacerInstance;
            _replacerInstance = new Replacer();
            return _replacerInstance;
        }

        private struct Item
        {
            public string Invalid { get; init; }
            public string Valid { get; init; }
            public string ClassName { get; set; }
        }

        public enum RepType
        {
            PowerSet,
            Power
        }

        private void Serialize()
        {
            var serializedData = JsonConvert.SerializeObject(Items);
            File.WriteAllText(_replacerFile, serializedData);
        }

        public void AddItem(string invalid, string valid)
        {
            Items?.Add(new Item { Invalid = invalid, Valid = valid });
            Serialize();
        }

        public void RemoveItem(string invalid)
        {
            if (Items == null) return;
            foreach (var item in Items.Where(set => set.Invalid == invalid))
            {
                Items.Remove(item);
                Serialize();
            }
        }

        public string ScanAndReplace(RepType repType, string inc)
        {
            var returnedItem = string.Empty;
            bool exists;
            switch (repType)
            {
                case RepType.PowerSet:
                    if (Items != null && Items.Any())
                    {
                        returnedItem = Items.Where(x => x.Invalid == inc).Select(x => x.Valid).FirstOrDefault();
                    }
                    else
                    {
                        var powerSets = DatabaseAPI.Database.Powersets;
                        exists = powerSets.Any(x => x?.FullName == inc);
                        if (exists)
                        {
                            returnedItem = inc;
                        }
                    }

                    break;
                case RepType.Power:
                    if (Items != null && Items.Any())
                    {
                        returnedItem = Items.Where(x => x.Invalid == inc).Select(x => x.Valid).FirstOrDefault();
                    }
                    else
                    {
                        var powers = DatabaseAPI.Database.Power;
                        exists = powers.Any(x => x?.FullName == inc);
                        if (exists)
                        {
                            returnedItem = inc;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(repType), repType, null);
            }
            return returnedItem ?? string.Empty;
        }
    }
}
