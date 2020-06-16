
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmImportPowerLevels : Form
    {

        frmBusy bFrm;

        string FullFileName;

        public frmImportPowerLevels()
        {
            Load += frmImportPowerLevels_Load;
            FullFileName = "";
            InitializeComponent();
            Name = nameof(frmImportPowerLevels);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImportPowerLevels));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        }

        void btnClose_Click(object sender, EventArgs e)

        {
            Close();
        }

        void btnFile_Click(object sender, EventArgs e)

        {
            dlgBrowse.FileName = FullFileName;
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
                FullFileName = dlgBrowse.FileName;
            BusyHide();
            DisplayInfo();
        }

        void btnImport_Click(object sender, EventArgs e)

        {
            ParseClasses(FullFileName);
            BusyHide();
            DisplayInfo();
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

        private void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(FullFileName);
            lblDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.PowerLevelVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new Decimal(DatabaseAPI.Database.PowerLevelVersion.Revision);
        }

        void frmImportPowerLevels_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.PowerLevelVersion.SourceFile;
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
                int num2 = (int)Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Power CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            try
            {
                string iLine;
                do
                {
                    iLine = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                    if (iLine == null || iLine.StartsWith("#"))
                        continue;
                    ++num5;
                    if (num5 >= 9)
                    {
                        BusyMsg(Strings.Format(num3, "###,##0") + " records parsed.");
                        num5 = 0;
                    }
                    string[] array = CSV.ToArray(iLine);
                    int index1 = DatabaseAPI.NidFromUidPower(array[1]);
                    if (index1 > -1)
                    {
                        DatabaseAPI.Database.Power[index1].Level = (int)Math.Round(Conversion.Val(array[2]) + 1.0);
                        int index2 = DatabaseAPI.NidFromUidPowerset(DatabaseAPI.Database.Power[index1].FullSetName);
                        if (index2 > -1 && DatabaseAPI.Database.Powersets[index2].SetType == Enums.ePowerSetType.Pool && DatabaseAPI.Database.Power[index1].Level == 1)
                            DatabaseAPI.Database.Power[index1].Level = 4;
                        ++num1;
                    }
                    else
                        ++num4;
                    ++num3;
                }
                while (iLine != null);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                iStream.Close();
                int num2 = (int)Interaction.MsgBox(exception.Message, MsgBoxStyle.Critical, "Power Class CSV Parse Error");
                ProjectData.ClearProjectError();
                return false;
            }
            iStream.Close();
            DatabaseAPI.Database.PowerLevelVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.PowerLevelVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.PowerLevelVersion.Revision = Convert.ToInt32(udRevision.Value);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            DisplayInfo();
            int num6 = (int)Interaction.MsgBox("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nRejected: " + Convert.ToString(num4), MsgBoxStyle.Information, "File Parsed");
            return true;
        }
    }
}