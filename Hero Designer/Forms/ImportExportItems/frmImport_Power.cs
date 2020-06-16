
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Base.Data_Classes;
using Hero_Designer.My;
using Import;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmImport_Power : Form
    {

        frmBusy bFrm;

        string FullFileName;

        PowerData[] ImportBuffer;

        public frmImport_Power()
        {
            Load += frmImport_Power_Load;
            FullFileName = "";
            ImportBuffer = new PowerData[0];
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImport_Power));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmImport_Power);
        }

        void btnCheckAll_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            int num = lstImport.Items.Count - 1;
            for (int index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = true;
            lstImport.EndUpdate();
        }

        void btnCheckModified_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            int num = lstImport.Items.Count - 1;
            for (int index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = lstImport.Items[index].SubItems[2].Text == "No" & lstImport.Items[index].SubItems[3].Text == "Yes";
            lstImport.EndUpdate();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        void btnEraseAll_Click(object sender, EventArgs e)
        {
            if (Interaction.MsgBox("Really wipte the power array. You shouldn't do this if you want to preserve any special power settings.", MsgBoxStyle.YesNo, "Really?") == MsgBoxResult.No)
                return;
            DatabaseAPI.Database.Power = new IPower[0];
            int num1 = ImportBuffer.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                if (ImportBuffer[index].IsValid)
                    ImportBuffer[index].IsNew = true;
            }
            int num2 = (int)Interaction.MsgBox("All powers removed!");
        }

        void btnFile_Click(object sender, EventArgs e)
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

        void btnImport_Click(object sender, EventArgs e)
        {
            ProcessImport();
        }

        void btnUncheckAll_Click(object sender, EventArgs e)
        {
            lstImport.BeginUpdate();
            int num = lstImport.Items.Count - 1;
            for (int index = 0; index <= num; ++index)
                lstImport.Items[index].Checked = false;
            lstImport.EndUpdate();
        }

        void BusyHide()
        {
            if (bFrm == null)
                return;
            bFrm.Close();
            bFrm = null;
        }

        void BusyMsg(string sMessage)
        {
            if (bFrm == null)
            {
                bFrm = new frmBusy();
                bFrm.Show(this);
            }
            bFrm.SetMessage(sMessage);
        }

        int[] CheckForDeletedPowers()
        {
            int[] numArray = new int[0];
            int num1 = 0;
            int num2 = DatabaseAPI.Database.Power.Length - 1;
            for (int index1 = 0; index1 <= num2; ++index1)
            {
                ++num1;
                if (num1 >= 9)
                {
                    BusyMsg("Checking for deleted powers..." + Strings.Format(index1, "###,##0") + " of " + Convert.ToString(DatabaseAPI.Database.Power.Length) + " done.");
                    Application.DoEvents();
                    num1 = 0;
                }
                bool flag = false;
                int num3 = ImportBuffer.Length - 1;
                for (int index2 = 0; index2 <= num3; ++index2)
                {
                    if (ImportBuffer[index2].Index != index1)
                        continue;
                    flag = true;
                    break;
                }

                if (flag)
                    continue;
                numArray = (int[])Utils.CopyArray(numArray, new int[numArray.Length + 1]);
                numArray[numArray.Length - 1] = index1;
            }
            BusyHide();
            string str = "";
            int num4 = numArray.Length - 1;
            for (int index = 0; index <= num4; ++index)
                str = str + DatabaseAPI.Database.Power[numArray[index]].FullName + "\r\n";
            Clipboard.SetDataObject(str);
            return numArray;
        }

        static int DeletePowers(int[] pList)
        {
            int index1 = 0;
            IPower[] powerArray = new IPower[DatabaseAPI.Database.Power.Length - pList.Length - 1 + 1];
            int num1 = DatabaseAPI.Database.Power.Length - 1;
            for (int index2 = 0; index2 <= num1; ++index2)
            {
                bool flag = false;
                int num2 = pList.Length - 1;
                for (int index3 = 0; index3 <= num2; ++index3)
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
                int num2 = (int)Interaction.MsgBox("Power array size mismatch! Count: " + Convert.ToString(index1) + " Array Length: " + Convert.ToString(powerArray.Length) + "\r\nNothing deleted.");
                num3 = 0;
            }
            else
            {
                DatabaseAPI.Database.Power = new IPower[powerArray.Length - 1 + 1];
                int powerCount = DatabaseAPI.Database.Power.Length - 1;
                for (int index2 = 0; index2 <= powerCount; ++index2)
                    DatabaseAPI.Database.Power[index2] = new Power(powerArray[index2]);
                num3 = index1;
            }
            return num3;
        }

        void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(FullFileName);
            lblDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.PowerVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new decimal(DatabaseAPI.Database.PowerVersion.Revision);
            lblCount.Text = "Records: " + Convert.ToString(DatabaseAPI.Database.Power.Length);
        }

        void FillListView()
        {
            string[] items = new string[5];
            lstImport.BeginUpdate();
            lstImport.Items.Clear();
            int num1 = 0;
            int num2 = ImportBuffer.Length - 1;
            for (int index = 0; index <= num2 - 1; ++index)
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
                bool flag = ImportBuffer[index].CheckDifference(out items[4]);
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

        void frmImport_Power_Load(object sender, EventArgs e)
        {
            FullFileName = DatabaseAPI.Database.PowerVersion.SourceFile;
            DisplayInfo();
        }

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
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Power CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }
            int num3 = 0;
            int num4 = 0;
            ImportBuffer = new PowerData[0];
            int num5 = 0;
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
                    ImportBuffer = (PowerData[])Utils.CopyArray(ImportBuffer, new PowerData[ImportBuffer.Length + 1]);
                    ImportBuffer[ImportBuffer.Length - 1] = new PowerData(iString);
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
                Interaction.MsgBox(exception.Message, MsgBoxStyle.Critical, "Power Class CSV Parse Error");
                ProjectData.ClearProjectError();
                return false;
            }
            iStream.Close();
            Interaction.MsgBox("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nRejected: " + Convert.ToString(num4), MsgBoxStyle.Information, "File Parsed");
            return true;
        }

        bool ProcessImport()
        {
            int num1 = 0;
            int num2 = lstImport.Items.Count - 1;
            for (int index = 0; index <= num2 - 1; ++index)
            {
                if (!lstImport.Items[index].Checked)
                    continue;
                ImportBuffer[Convert.ToInt32(lstImport.Items[index].Tag)].Apply();
                ++num1;
            }
            if (Interaction.MsgBox("Check for deleted powers?", MsgBoxStyle.YesNo, "Additional Check") == MsgBoxResult.Yes)
            {
                int[] pList = CheckForDeletedPowers();
                if (pList.Length > 0 && Interaction.MsgBox(Convert.ToString(pList.Length) + "  deleted powers found. Delete them?", MsgBoxStyle.YesNo, "Additional Check") == MsgBoxResult.Yes)
                    DeletePowers(pList);
            }
            DatabaseAPI.Database.PowerVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.PowerVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.PowerVersion.Revision = Convert.ToInt32(udRevision.Value);
            DatabaseAPI.MatchAllIDs();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            Interaction.MsgBox("Import of " + Convert.ToString(num1) + " records completed!", MsgBoxStyle.Information, "Done");
            DisplayInfo();
            return false;
        }
    }
}