using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

public class Modifiers : ICloneable
{
    private const string StoreName = "Mids' Hero Designer Attribute Modifier Tables";
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

    #region ICollection implementation

    public int Count()
    {
        return Modifier.Length;
    }
    #endregion


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

    public bool Load()
    {
        var path = Files.SelectDataFileLoad("AttribMod.mhd");
        Modifier = new ModifierTable[0];
        FileStream fileStream;
        BinaryReader reader;
        try
        {
            fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            reader = new BinaryReader(fileStream);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message + '\n' + '\n' + "Modifier tables couldn't be loaded.");
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
            var num = 0;
            Modifier = new ModifierTable[reader.ReadInt32() + 1];
            for (var index = 0; index <= Modifier.Length - 1; ++index)
            {
                Modifier[index] = new ModifierTable();
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
            Modifier = new ModifierTable[0];
            reader.Close();
            fileStream.Close();
            return false;
        }
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

    public void Store(ISerialize serializer)
    {
        var path = Files.SelectDataFileSave("AttribMod.mhd");
        StoreRaw(serializer, path, StoreName);
        FileStream fileStream;
        BinaryWriter writer;
        try
        {
            fileStream = new FileStream(path, FileMode.Create);
            writer = new BinaryWriter(fileStream);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
            return;
        }

        try
        {
            writer.Write(StoreName);
            writer.Write(Revision);
            writer.Write(RevisionDate.ToBinary());
            writer.Write(SourceIndex);
            writer.Write(SourceTables);
            writer.Write(Modifier.Length - 1);
            for (var index = 0; index <= Modifier.Length - 1; ++index)
                Modifier[index].StoreTo(writer);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Message: {ex.Message}\r\nTrace: {ex.StackTrace}");
        }
        finally
        {
            writer.Close();
            fileStream.Close();
        }
    }

    public class ModifierTable
    {
        // https://www.codeproject.com/Questions/1278760/How-to-deserialize-json-array-Csharp
        [JsonProperty("Table")]
        public readonly float[][] Table = new float[55][];
        
        [JsonProperty("BaseIndex")]
        public int BaseIndex;
        
        [JsonProperty("ID")]
        public string ID = string.Empty;

        public ModifierTable()
        {
            for (var index = 0; index < Table.Length; ++index)
                Table[index] = new float[0];
        }

        public ModifierTable(int archetypesListLength)
        {
            for (int index = 0; index < Table.Length; ++index)
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
                    writer.Write(Table[index1][index2]);
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
                    Table[index1][index2] = reader.ReadSingle();
            }
        }
    }
}