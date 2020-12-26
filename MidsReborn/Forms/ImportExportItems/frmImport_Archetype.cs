using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.My;
using mrbBase;
using mrbBase.Import;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmImport_Archetype : Form
    {
        private string FullFileName;

        private ArchetypeData[] ImportBuffer;

        public frmImport_Archetype()
        {
            Load += frmImport_Archetype_Load;
            FullFileName = "";
            ImportBuffer = new ArchetypeData[0];
            InitializeComponent();
            Name = nameof(frmImport_Archetype);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_Archetype));
            Icon = Resources.reborn;
        }

        private void btnATFile_Click(object sender, EventArgs e)

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

        private void btnClose_Click(object sender, EventArgs e)

        {
            Close();
        }

        private void btnImport_Click(object sender, EventArgs e)

        {
            ProcessImport();
        }

        private void DisplayInfo()
        {
            lblATFile.Text = FileIO.StripPath(FullFileName);
            lblATDate.Text = "Date: " +
                             Strings.Format(DatabaseAPI.Database.ArchetypeVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udATRevision.Value = new decimal(DatabaseAPI.Database.ArchetypeVersion.Revision);
            lblATCount.Text = "Classes: " +
                              Convert.ToString(DatabaseAPI.Database.Classes.Length, CultureInfo.InvariantCulture);
        }

        private void FillListView()

        {
            var items = new string[6];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            var num = ImportBuffer.Length - 1;
            for (var index = 0; index <= num; ++index)
            {
                if (!ImportBuffer[index].IsValid)
                    continue;
                items[0] = ImportBuffer[index].Data.DisplayName;
                items[1] = ImportBuffer[index].Data.ClassName;
                items[2] = !ImportBuffer[index].Data.Playable ? "No" : "Yes";
                items[3] = !ImportBuffer[index].IsNew ? "No" : "Yes";
                var flag = ImportBuffer[index].CheckDifference(out items[5]);
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

        private void frmImport_Archetype_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.ArchetypeVersion.SourceFile;
            DisplayInfo();
        }

        [DebuggerStepThrough]
        private bool ParseClasses(string iFileName)

        {
            var num1 = 0;
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Archetype Class CSV Not Opened", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            ImportBuffer = new ArchetypeData[0];
            try
            {
                string iString;
                do
                {
                    iString = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                    if (iString == null || iString.StartsWith("#"))
                        continue;
                    ImportBuffer =
                        (ArchetypeData[]) Utils.CopyArray(ImportBuffer, new ArchetypeData[ImportBuffer.Length + 1]);
                    ImportBuffer[ImportBuffer.Length - 1] = new ArchetypeData(iString);
                    ++num3;
                    if (ImportBuffer[ImportBuffer.Length - 1].IsValid)
                        ++num1;
                    else
                        ++num4;
                } while (iString != null);
            }
            catch (Exception ex)
            {
                iStream.Close();
                MessageBox.Show(ex.Message, "Archetype Class CSV Parse Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return false;
            }

            iStream.Close();
            MessageBox.Show($"Parse Completed!\r\nTotal Records: {num3}\r\nGood: {num1}\r\nRejected: {num4}",
                "File Parsed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }

        private bool ProcessImport()

        {
            var num1 = 0;
            var num2 = lstImport.Items.Count - 1;
            for (var index = 0; index <= num2; ++index)
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
            MessageBox.Show($"Import of {num1} classes completed!", "Done", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            DisplayInfo();
            return false;
        }
    }
}