
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Hero_Designer.My;
using Import;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmImport_Archetype : Form
    {

        string FullFileName;

        ArchetypeData[] ImportBuffer;

        public frmImport_Archetype()
        {
            Load += frmImport_Archetype_Load;
            FullFileName = "";
            ImportBuffer = new ArchetypeData[0];
            InitializeComponent();
            Name = nameof(frmImport_Archetype);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImport_Archetype));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        }

        void btnATFile_Click(object sender, EventArgs e)

        {
            dlgBrowse.FileName = FullFileName;
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
            {
                FullFileName = dlgBrowse.FileName;
                if (ParseClasses(FullFileName))
                    FillListView();
            }
            DisplayInfo();
        }

        void btnClose_Click(object sender, EventArgs e)

        {
            Close();
        }

        void btnImport_Click(object sender, EventArgs e)

        {
            ProcessImport();
        }

        public void DisplayInfo()
        {
            lblATFile.Text = FileIO.StripPath(FullFileName);
            lblATDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.ArchetypeVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udATRevision.Value = new Decimal(DatabaseAPI.Database.ArchetypeVersion.Revision);
            lblATCount.Text = "Classes: " + Convert.ToString(DatabaseAPI.Database.Classes.Length);
        }

        void FillListView()

        {
            string[] items = new string[6];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            int num = ImportBuffer.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (!ImportBuffer[index].IsValid)
                    continue;
                items[0] = ImportBuffer[index].Data.DisplayName;
                items[1] = ImportBuffer[index].Data.ClassName;
                items[2] = !ImportBuffer[index].Data.Playable ? "No" : "Yes";
                items[3] = !ImportBuffer[index].IsNew ? "No" : "Yes";
                bool flag = ImportBuffer[index].CheckDifference(out items[5]);
                items[4] = !flag ? "No" : "Yes";
                lstImport.Items.Add(new ListViewItem(items)
                {
                    Checked = flag,
                    Tag = index
                });
            }
            if (lstImport.Items.Count > 0)
                lstImport.Items[0].EnsureVisible();
            lstImport.EndUpdate();
        }

        void frmImport_Archetype_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.ArchetypeVersion.SourceFile;
            DisplayInfo();
        }

        [DebuggerStepThrough]

        bool ParseClasses(string iFileName)

        {
            int num1 = 0;
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num2 = (int)Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Archetype Class CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }
            int num3 = 0;
            int num4 = 0;
            ImportBuffer = new ArchetypeData[0];
            try
            {
                string iString;
                do
                {
                    iString = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                    if (iString == null || iString.StartsWith("#"))
                        continue;
                    ImportBuffer = (ArchetypeData[])Utils.CopyArray(ImportBuffer, new ArchetypeData[ImportBuffer.Length + 1]);
                    ImportBuffer[ImportBuffer.Length - 1] = new ArchetypeData(iString);
                    ++num3;
                    if (ImportBuffer[ImportBuffer.Length - 1].IsValid)
                        ++num1;
                    else
                        ++num4;
                }
                while (iString != null);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                iStream.Close();
                int num2 = (int)Interaction.MsgBox(exception.Message, MsgBoxStyle.Critical, "Archetype Class CSV Parse Error");
                ProjectData.ClearProjectError();
                return false;
            }
            iStream.Close();
            int num5 = (int)Interaction.MsgBox(("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nBad: " + Convert.ToString(num4)), MsgBoxStyle.Information, "File Parsed");
            return true;
        }

        bool ProcessImport()

        {
            int num1 = 0;
            int num2 = lstImport.Items.Count - 1;
            for (int index = 0; index <= num2; ++index)
            {
                if (!lstImport.Items[index].Checked)
                    continue;
                ImportBuffer[Convert.ToInt32(lstImport.Items[index].Tag)].Apply();
                ++num1;
            }
            DatabaseAPI.Database.ArchetypeVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.ArchetypeVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.ArchetypeVersion.Revision = Convert.ToInt32(udATRevision.Value);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            int num3 = (int)Interaction.MsgBox(("Import of " + Convert.ToString(num1) + " classes completed!"), MsgBoxStyle.Information, "Done");
            DisplayInfo();
            return false;
        }
    }
}