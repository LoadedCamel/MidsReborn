using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

public class Modifiers
{
    private const string StoreName = "Mids' Hero Designer Attribute Modifier Tables";
    public ModifierTable[] Modifier = new ModifierTable[0];
    public int Revision;
    public DateTime RevisionDate = new DateTime(0L);
    public string SourceIndex = string.Empty;
    public string SourceTables = string.Empty;

    public bool ImportModifierTablefromCSV(string baseFn, string tableFn, int iRevision)
    {
        StreamReader iStream1;
        try
        {
            iStream1 = new StreamReader(baseFn);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            return false;
        }

        Modifier = new ModifierTable[0];
        string iLine1;
        do
        {
            iLine1 = FileIO.ReadLineUnlimited(iStream1, char.MinValue);
            if (iLine1 == null || iLine1.StartsWith("#"))
                continue;
            var array = CSV.ToArray(iLine1);
            Array.Resize(ref Modifier, Modifier.Length + 1);
            Modifier[Modifier.Length - 1] = new ModifierTable
            {
                BaseIndex = Convert.ToInt32(array[0]),
                ID = array[1]
            };
        } while (iLine1 != null);

        iStream1.Close();
        StreamReader iStream2;
        try
        {
            iStream2 = new StreamReader(tableFn);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            return false;
        }

        string iLine2;
        do
        {
            iLine2 = FileIO.ReadLineUnlimited(iStream2, char.MinValue);
            if (iLine2 == null || iLine2.StartsWith("#"))
                continue;
            var array = CSV.ToArray(iLine2);
            if (array.Length <= 0)
                continue;
            var num = int.Parse(array[0]) - 1;
            for (var index1 = 0; index1 <= Modifier.Length - 1; ++index1)
            {
                if (!((num >= Modifier[index1].BaseIndex) & (num <= Modifier[index1].BaseIndex + 55)))
                    continue;
                var index2 = num - Modifier[index1].BaseIndex;
                Modifier[index1].Table[index2] = new float[array.Length - 1];
                for (var index3 = 0; index3 <= array.Length - 2; ++index3)
                    Modifier[index1].Table[index2][index3] = float.Parse(array[index3 + 1]);
                break;
            }
        } while (iLine2 != null);

        bool flag;
        if (Modifier.Length > 0)
        {
            SourceIndex = baseFn;
            SourceTables = tableFn;
            RevisionDate = DateTime.Now;
            Revision = iRevision;
            flag = true;
        }
        else
        {
            flag = false;
        }

        return flag;
    }

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