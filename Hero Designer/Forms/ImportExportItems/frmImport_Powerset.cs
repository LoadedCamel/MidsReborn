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
    public partial class frmImport_Powerset : Form
    {
        private frmBusy bFrm;

        private string FullFileName;

        private PowersetData[] ImportBuffer;

        public frmImport_Powerset()
        {
            Load += frmImport_Powerset_Load;
            FullFileName = "";
            ImportBuffer = new PowersetData[0];
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_Powerset));
            Icon = Resources.reborn;
            Name = nameof(frmImport_Powerset);
        }

        private void btnCheckAll_Click(object sender, EventArgs e)

        {
            lstImport.BeginUpdate();
            var num = lstImport.Items.Count - 1;
            for (var index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = true;
            lstImport.EndUpdate();
        }

        private void btnClose_Click(object sender, EventArgs e)

        {
            Close();
        }

        private void btnFile_Click(object sender, EventArgs e)

        {
            dlgBrowse.FileName = FullFileName;
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
            {
                FullFileName = dlgBrowse.FileName;
                if (ParseClasses(FullFileName))
                    FillListView();
            }

            BusyHide();
            DisplayInfo();
        }

        private void btnImport_Click(object sender, EventArgs e)

        {
            ProcessImport();
        }

        private void btnUncheckAll_Click(object sender, EventArgs e)

        {
            lstImport.BeginUpdate();
            var num = lstImport.Items.Count - 1;
            for (var index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = false;
            lstImport.EndUpdate();
        }

        private void BusyHide()

        {
            if (bFrm == null)
                return;
            bFrm.Close();
            bFrm = null;
        }

        private void BusyMsg(string sMessage)

        {
            if (bFrm == null)
            {
                bFrm = new frmBusy();
                bFrm.Show(this);
            }

            bFrm.SetMessage(sMessage);
        }

        private void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(FullFileName);
            lblDate.Text = "Date: " +
                           Strings.Format(DatabaseAPI.Database.PowersetVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new decimal(DatabaseAPI.Database.PowersetVersion.Revision);
            lblCount.Text = "Records: " +
                            Convert.ToString(DatabaseAPI.Database.Powersets.Length, CultureInfo.InvariantCulture);
        }

        private void FillListView()

        {
            var items = new string[5];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            var num1 = 0;
            var num2 = ImportBuffer.Length - 1;
            for (var index = 0; index <= num2; ++index)
            {
                ++num1;
                if (num1 >= 100)
                {
                    BusyMsg(Strings.Format(index, "###,##0") + " records checked.");
                    num1 = 0;
                }

                if (!ImportBuffer[index].IsValid)
                    continue;
                items[0] = ImportBuffer[index].Data.FullName;
                items[1] = ImportBuffer[index].Data.GroupName;
                items[2] = !ImportBuffer[index].IsNew ? "No" : "Yes";
                var flag = ImportBuffer[index].CheckDifference(out items[4]);
                items[3] = !flag ? "No" : "Yes";
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

        private void frmImport_Powerset_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.PowersetVersion.SourceFile;
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
                MessageBox.Show(ex.Message, "Powerset CSV Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            ImportBuffer = new PowersetData[0];
            var num5 = 0;
            try
            {
                string iString;
                do
                {
                    iString = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                    if (iString == null || iString.StartsWith("#"))
                        continue;
                    ++num5;
                    if (num5 >= 100)
                    {
                        BusyMsg(Strings.Format(num3, "###,##0") + " records parsed.");
                        num5 = 0;
                    }

                    ImportBuffer =
                        (PowersetData[]) Utils.CopyArray(ImportBuffer, new PowersetData[ImportBuffer.Length + 1]);
                    ImportBuffer[ImportBuffer.Length - 1] = new PowersetData(iString);
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
                MessageBox.Show(ex.Message, "Powerset Class CSV Parse Error", MessageBoxButtons.OK,
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

            DatabaseAPI.Database.PowersetVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.PowersetVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.PowersetVersion.Revision = Convert.ToInt32(udRevision.Value);
            DatabaseAPI.MatchAllIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            MessageBox.Show($"Import of {num1} records completed!", "Done", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            DisplayInfo();
            return false;
        }
    }
}