using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mids_Reborn.Core.Base.Master_Classes;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Utils
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

        private struct Item
        {
            public string Invalid { get; init; }
            public string Valid { get; init; }
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
    }
}
