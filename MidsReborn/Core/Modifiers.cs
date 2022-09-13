using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Mids_Reborn.Core
{
    public class Modifiers : ICloneable
    {
        public List<ModifierTable> Modifier = new List<ModifierTable>();
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

        public bool Load(string? iPath)
        {
            var path = Files.SelectDataFileLoad(Files.JsonFileModifiers, iPath);
            
            if (File.Exists(path))
            {
                try
                {
                    var jsonText = File.ReadAllText(path);
                    DatabaseAPI.Database.AttribMods = JsonConvert.DeserializeObject<Modifiers>(jsonText, Serializer.SerializerSettings);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($@"Modifier table file isn't how it should be....\r\nMessage: {ex.Message}\r\nStackTrace: {ex.StackTrace}\n No modifiers were loaded.");
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(iPath))
            {
                path = Files.SelectDataFileLoad(Files.MxdbFileModifiers, iPath);


                Modifier = new List<ModifierTable>();
                FileStream fileStream;
                BinaryReader reader;
                try
                {
                    fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    reader = new BinaryReader(fileStream);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + '\n' + '\n' + @"Modifier tables couldn't be loaded.");
                    return false;
                }

                try
                {
                    if (reader.ReadString() != "Mids' Hero Designer Attribute Modifier Tables")
                    {
                        MessageBox.Show("Modifier table header wasn't found, file may be corrupt!");
                        reader.Close();
                        fileStream.Close();
                        return false;
                    }

                    Revision = reader.ReadInt32();
                    RevisionDate = DateTime.FromBinary(reader.ReadInt64());
                    SourceIndex = reader.ReadString();
                    SourceTables = reader.ReadString();
                    int num = 0;
                    Modifier = new List<ModifierTable>();
                    for (int index = 0; index <= Modifier.Count - 1; ++index)
                    {
                        Modifier.Add(new ModifierTable());
                        //Modifier[index] = new ModifierTable();
                        Modifier[index].Load(reader);
                        if (num <= 5)
                            continue;
                        num = 0;
                        Application.DoEvents();
                    }

                    return true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Modifier table file isn't how it should be (" + ex.Message + ")" + '\n' +
                                    "No modifiers loaded.");
                    Modifier = new List<ModifierTable>();
                    reader.Close();
                    fileStream.Close();
                    return false;
                }
            }

            return false;
        }

        public void Store(ISerialize serializer, string? iPath = "")
        {
            string path;
            if (string.IsNullOrWhiteSpace(iPath))
            {
                path = Files.SelectDataFileSave("AttribMod.json");
            }
            else
            {
                path = Files.SelectDataFileSave("AttribMod.json", iPath);
            }

            File.WriteAllText(path, JsonConvert.SerializeObject(DatabaseAPI.Database.AttribMods, Serializer.SerializerSettings));
        }

        [JsonObject]
        public class ModifierTable
        {
            [JsonProperty("Table")] 
            public List<List<float>> Table = new List<List<float>>();
            //public float[][] Table = new float[55][];
            [JsonProperty("BaseIndex")]
            public int BaseIndex;
            [JsonProperty("ID")]
            public string ID = string.Empty;

            public ModifierTable()
            {
                for (var index = 0; index < Table.Count; index++)
                {
                    Table.Add(new List<float>());
                    //Table[index] = new float[0];
                }
            }

            public ModifierTable(int archetypesListLength)
            {
                for (int index = 0; index < Table.Count; index++)
                {
                    Table.Add(new List<float>());
                    Table.Add(Enumerable.Repeat(0f, archetypesListLength).ToList());
                    //Table[index] = new float[archetypesListLength];
                    //Table[index] = Enumerable.Repeat(0f, archetypesListLength).ToArray();
                }
            }

            public void StoreTo(BinaryWriter writer)
            {
                writer.Write(ID);
                writer.Write(BaseIndex);
            
                for (var index1 = 0; index1 <= Table.Count - 1; ++index1)
                {
                    writer.Write(Table[index1].Count - 1);
                    for (var index2 = 0; index2 <= Table[index1].Count - 1; ++index2)
                    {
                        writer.Write(Table[index1][index2]);
                    }
                }
            }

            public void Load(BinaryReader reader)
            {
                ID = reader.ReadString();
                BaseIndex = reader.ReadInt32();
                for (var index1 = 0; index1 <= Table.Count - 1; ++index1)
                {
                    Table.Add(new List<float>());
                    //Table[index1] = new float[reader.ReadInt32() + 1];
                    for (var index2 = 0; index2 <= Table[index1].Count - 1; ++index2)
                    {
                        Table[index1].Add(reader.ReadSingle());
                    }
                }
            }
        }
    }
}