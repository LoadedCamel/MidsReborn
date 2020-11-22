using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

public class Modifiers : ICloneable
{
    //private const string StoreName = "Mids' Hero Designer Attribute Modifier Tables";
    public ModifierTable[] Modifier = new ModifierTable[0];
    public int Revision;
    public DateTime RevisionDate = new DateTime(0L);
    public string SourceIndex = string.Empty;
    public string SourceTables = string.Empty;

    #region ICloneable implementation
    public object Clone()
    {
        return MemberwiseClone();
    }
    #endregion

    private void StoreRaw(ISerialize serializer, string path, string name)
    {
        var toSerialize = new
        {
            name,
            Revision,
            RevisionDate,
            SourceIndex,
            SourceTables,
            Modifier
        };
        ConfigData.SaveRawMhd(serializer, toSerialize, path, null);
    }

    public bool Load()
    {
        var path = Files.SelectDataFileLoad("AttribMod.json");
        try
        {
            var jsonText = File.ReadAllText(path);
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                NullValueHandling = NullValueHandling.Ignore,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
            };
            DatabaseAPI.Database.AttribMods = JsonConvert.DeserializeObject<Modifiers>(jsonText, settings);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Modifier table file isn't how it should be....\r\nMessage: {ex.Message}\r\nStackTrace: {ex.StackTrace}\n No modifiers were loaded.");
            return false;
        }
    }

    public void Store(ISerialize serializer)
    {
        var path = Files.SelectDataFileSave("AttribMod.json");
        var serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Formatting = Formatting.Indented
        };
        File.WriteAllText(path, JsonConvert.SerializeObject(DatabaseAPI.Database.AttribMods, serializerSettings));
    }

    [JsonObject]
    public class ModifierTable
    {
        [JsonProperty("Table")]
        public float[][] Table = new float[55][];
        [JsonProperty("BaseIndex")]
        public int BaseIndex;
        [JsonProperty("ID")]
        public string ID = string.Empty;

        public ModifierTable()
        {
            for (var index = 0; index < Table.Length; index++)
            {
                Table[index] = new float[0];
            }
        }

        public ModifierTable(int archetypesListLength)
        {
            for (int index = 0; index < Table.Length; index++)
            {
                Table[index] = new float[archetypesListLength];
                Table[index] = Enumerable.Repeat(0f, archetypesListLength).ToArray();
            }
        }

        public void StoreTo(BinaryWriter writer)
        {
            writer.Write(ID);
            writer.Write(BaseIndex);
            
            for (var index1 = 0; index1 <= Table.Length - 1; ++index1)
            {
                writer.Write(Table[index1].Length - 1);
                for (var index2 = 0; index2 <= Table[index1].Length - 1; ++index2)
                {
                    writer.Write(Table[index1][index2]);
                }
            }
        }

        public void Load(BinaryReader reader)
        {
            ID = reader.ReadString();
            BaseIndex = reader.ReadInt32();
            for (var index1 = 0; index1 <= Table.Length - 1; ++index1)
            {
                Table[index1] = new float[reader.ReadInt32() + 1];
                for (var index2 = 0; index2 <= Table[index1].Length - 1; ++index2)
                {
                    Table[index1][index2] = reader.ReadSingle();
                }
            }
        }
    }
}