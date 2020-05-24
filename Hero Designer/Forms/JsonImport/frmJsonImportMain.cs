using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hero_Designer.Forms.JsonImport
{
    public partial class frmJsonImportMain : Form
    {
        public frmJsonImportMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog1.FileName;
                parseAttriModFile(FileName);
            }
        }

        private void parseAttriModFile(string fileName)
        {
            JsonSerializer serializer = new JsonSerializer();
            string json = File.ReadAllText(fileName);
            List<Class> classes = JsonConvert.DeserializeObject<List<Class>>(json);

            foreach (Class c in classes)
            {
                for (int i = 0; i < DatabaseAPI.Database.AttribMods.Modifier[0].Table.Length - 1; i++)
                {
                    for (int j = 0; j < DatabaseAPI.Database.Classes.Length - 1; j++)
                    {
                        for(int k = 0; k < c.ModTables[0].Values.Count -1; k++) { 
                            if (j < c.ModTables[0].Values.Count - 1)
                            {
                                //Console.WriteLine("Old value {0}, new value {1}", DatabaseAPI.Database.AttribMods.Modifier[0].Table[i][j], c.ModTables[0].Values[j]);
                                int tableId = DatabaseAPI.NidFromUidAttribMod(c.ModTables[k].Name);
                                if (tableId > 0)
                                    DatabaseAPI.Database.AttribMods.Modifier[tableId].Table[i][j] = c.ModTables[k].Values[j];
                            }
                        }
                    }
                }
            }
            DatabaseAPI.Database.AttribMods.Store(My.MyApplication.GetSerializer());
            DatabaseAPI.SaveJSONDatabase(My.MyApplication.GetSerializer());
            MessageBox.Show("Import completed");
        }
    }
}
