using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.My;
using mrbBase;
using mrbBase.Base.Data_Classes;
using mrbBase.Import;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmImport_Power : Form
    {
        private frmBusy bFrm;

        private string FullFileName;

        private PowerData[] ImportBuffer;

        public frmImport_Power()
        {
            Load += frmImport_Power_Load;
            FullFileName = "";
            ImportBuffer = new PowerData[0];
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_Power));
            Icon = Resources.reborn;
            Name = nameof(frmImport_Power);
        }

        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            var num = lstImport.Items.Count - 1;
            for (var index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = true;
            lstImport.EndUpdate();
        }

        private void btnCheckModified_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            var num = lstImport.Items.Count - 1;
            for (var index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = (lstImport.Items[index].SubItems[2].Text == "No") &
                                                 (lstImport.Items[index].SubItems[3].Text == "Yes");
            lstImport.EndUpdate();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnEraseAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to wipe the power array?\r\nNote: You shouldn't do this if you want to preserve any special power settings.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;
            DatabaseAPI.Database.Power = new IPower[0];
            var num1 = ImportBuffer.Length - 1;
            for (var index = 0; index <= num1; ++index)
                if (ImportBuffer[index].IsValid)
                    ImportBuffer[index].IsNew = true;
            MessageBox.Show("All powers removed!");
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            dlgBrowse.FileName = FullFileName;
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
            {
                FullFileName = dlgBrowse.FileName;
                Enabled = false;
                if (ParseClasses(FullFileName))
                    FillListView();
                Enabled = true;
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

        private int[] CheckForDeletedPowers()
        {
            var numArray = new int[0];
            var num1 = 0;
            var num2 = DatabaseAPI.Database.Power.Length - 1;
            for (var index1 = 0; index1 <= num2; ++index1)
            {
                ++num1;
                if (num1 >= 9)
                {
                    BusyMsg("Checking for deleted powers..." + Strings.Format(index1, "###,##0") + " of " +
                            Convert.ToString(DatabaseAPI.Database.Power.Length) + " done.");
                    Application.DoEvents();
                    num1 = 0;
                }

                var flag = false;
                var num3 = ImportBuffer.Length - 1;
                for (var index2 = 0; index2 <= num3; ++index2)
                {
                    if (ImportBuffer[index2].Index != index1)
                        continue;
                    flag = true;
                    break;
                }

                if (flag)
                    continue;
                numArray = (int[]) Utils.CopyArray(numArray, new int[numArray.Length + 1]);
                numArray[numArray.Length - 1] = index1;
            }

            BusyHide();
            var str = "";
            var num4 = numArray.Length - 1;
            for (var index = 0; index <= num4; ++index)
                str = str + DatabaseAPI.Database.Power[numArray[index]].FullName + "\r\n";
            Clipboard.SetDataObject(str);
            return numArray;
        }

        private static int DeletePowers(int[] pList)
        {
            var index1 = 0;
            var powerArray = new IPower[DatabaseAPI.Database.Power.Length - pList.Length - 1 + 1];
            var num1 = DatabaseAPI.Database.Power.Length - 1;
            for (var index2 = 0; index2 <= num1; ++index2)
            {
                var flag = false;
                var num2 = pList.Length - 1;
                for (var index3 = 0; index3 <= num2; ++index3)
                {
                    if (index2 != pList[index3])
                        continue;
                    flag = true;
                    break;
                }

                if (flag)
                    continue;
                powerArray[index1] = new Power(DatabaseAPI.Database.Power[index2]);
                ++index1;
            }

            int num3;
            if (index1 != powerArray.Length)
            {
                MessageBox.Show($"Power array size mismatch! Count: {index1} Array Length: {powerArray.Length}\r\nNothing deleted.");
                num3 = 0;
            }
            else
            {
                DatabaseAPI.Database.Power = new IPower[powerArray.Length - 1 + 1];
                var powerCount = DatabaseAPI.Database.Power.Length - 1;
                for (var index2 = 0; index2 <= powerCount; ++index2)
                    DatabaseAPI.Database.Power[index2] = new Power(powerArray[index2]);
                num3 = index1;
            }

            return num3;
        }

        private void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(FullFileName);
            lblDate.Text = "Date: " +
                           Strings.Format(DatabaseAPI.Database.PowerVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new decimal(DatabaseAPI.Database.PowerVersion.Revision);
            lblCount.Text = "Records: " + Convert.ToString(DatabaseAPI.Database.Power.Length);
        }

        private void FillListView()
        {
            var items = new string[5];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            var num1 = 0;
            var num2 = ImportBuffer.Length - 1;
            for (var index = 0; index <= num2 - 1; ++index)
            {
                ++num1;
                if (num1 >= 100)
                {
                    BusyMsg(Strings.Format(index, "###,##0") + " records checked.");
                    Application.DoEvents();
                    num1 = 0;
                }

                if (!ImportBuffer[index].IsValid)
                    continue;
                items[0] = ImportBuffer[index].Data.FullName;
                items[1] = ImportBuffer[index].Data.DisplayName;
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

        private void frmImport_Power_Load(object sender, EventArgs e)
        {
            FullFileName = DatabaseAPI.Database.PowerVersion.SourceFile;
            DisplayInfo();
        }

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
                MessageBox.Show(ex.Message, "Power CSV Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            ImportBuffer = new PowerData[0];
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
                    if (num5 >= 9)
                    {
                        BusyMsg(Strings.Format(num3, "###,##0") + " records parsed.");
                        Application.DoEvents();
                        num5 = 0;
                    }

                    ImportBuffer = (PowerData[]) Utils.CopyArray(ImportBuffer, new PowerData[ImportBuffer.Length + 1]);
                    ImportBuffer[ImportBuffer.Length - 1] = new PowerData(iString);
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
                MessageBox.Show(ex.Message, "Power Class CSV Parse Error", MessageBoxButtons.OK,
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
            for (var index = 0; index <= num2 - 1; ++index)
            {
                if (!lstImport.Items[index].Checked)
                    continue;
                ImportBuffer[Convert.ToInt32(lstImport.Items[index].Tag)].Apply();
                ++num1;
            }

            if (MessageBox.Show("Check for delted powers?", "Additional Check", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                var pList = CheckForDeletedPowers();
                if (pList.Length > 0 && MessageBox.Show($"{pList.Length} deleted powers found. Delete them?", "Additional Check", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    DeletePowers(pList);
            }

            DatabaseAPI.Database.PowerVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.PowerVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.PowerVersion.Revision = Convert.ToInt32(udRevision.Value);
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