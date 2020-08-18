
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

public class Modifiers : ICloneable
{
    public ModifierTable[] Modifier = new ModifierTable[0];
    public DateTime RevisionDate = new DateTime(0L);
    public string SourceIndex = string.Empty;
    public string SourceTables = string.Empty;
    public int Revision;

    public object Clone()
    {
        return MemberwiseClone();
    }

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
            string[] array = CSV.ToArray(iLine1);
            Array.Resize(ref Modifier, Modifier.Length + 1);
            Modifier[Modifier.Length - 1] = new ModifierTable
            {
                BaseIndex = Convert.ToInt32(array[0]),
                ID = array[1]
            };
        }
        while (iLine1 != null);
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
            string[] array = CSV.ToArray(iLine2);
            if (array.Length <= 0)
                continue;
            int num = int.Parse(array[0]) - 1;
            for (int index1 = 0; index1 <= Modifier.Length - 1; ++index1)
            {
                if (!(num >= Modifier[index1].BaseIndex & num <= Modifier[index1].BaseIndex + 55))
                    continue;
                int index2 = num - Modifier[index1].BaseIndex;
                Modifier[index1].Table[index2] = new float[array.Length - 1];
                for (int index3 = 0; index3 <= array.Length - 2; ++index3)
                    Modifier[index1].Table[index2][index3] = float.Parse(array[index3 + 1]);
                break;
            }
        }
        while (iLine2 != null);
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
            flag = false;
        return flag;
    }

    public bool Load()
    {
        string path = Files.SelectDataFileLoad("AttribMod.mhd");
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
            int num = 0;
            Modifier = new ModifierTable[reader.ReadInt32() + 1];
            for (int index = 0; index <= Modifier.Length - 1; ++index)
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
            MessageBox.Show("Modifier table file isn't how it should be (" + ex.Message + ")" + '\n' + "No modifiers loaded.");
            Modifier = new ModifierTable[0];
            reader.Close();
            fileStream.Close();
            return false;
        }
    }

    void StoreRaw(ISerialize serializer, string path, string name)
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

    const string StoreName = "Mids' Hero Designer Attribute Modifier Tables";
    public void Store(ISerialize serializer)
    {
        string path = Files.SelectDataFileSave("AttribMod.mhd");
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
            for (int index = 0; index <= Modifier.Length - 1; ++index)
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
        public string ID = string.Empty;
        public readonly float[][] Table = new float[55][];
        public int BaseIndex;

        public ModifierTable()
        {
            for (int index = 0; index < Table.Length; ++index)
            {
                Table[index] = new float[0];
            }
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
            for (int index1 = 0; index1 <= Table.Length - 1; ++index1)
            {
                writer.Write(Table[index1].Length - 1);
                for (int index2 = 0; index2 <= Table[index1].Length - 1; ++index2)
                    writer.Write(Table[index1][index2]);
            }
        }

        public void Load(BinaryReader reader)
        {
            ID = reader.ReadString();
            BaseIndex = reader.ReadInt32();
            for (int index1 = 0; index1 <= Table.Length - 1; ++index1)
            {
                Table[index1] = new float[reader.ReadInt32() + 1];
                for (int index2 = 0; index2 <= Table[index1].Length - 1; ++index2)
                    Table[index1][index2] = reader.ReadSingle();
            }
        }
    }
}
