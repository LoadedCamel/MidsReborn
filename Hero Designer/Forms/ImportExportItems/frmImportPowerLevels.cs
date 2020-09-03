using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer.Forms.ImportExportItems
{
    public partial class frmImportPowerLevels : Form
    {
        private frmBusy bFrm;

        private string FullFileName;

        public frmImportPowerLevels()
        {
            Load += frmImportPowerLevels_Load;
            FullFileName = "";
            InitializeComponent();
            Name = nameof(frmImportPowerLevels);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImportPowerLevels));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
        }

        private void btnClose_Click(object sender, EventArgs e)

        {
            Close();
        }

        private void btnFile_Click(object sender, EventArgs e)

        {
            dlgBrowse.FileName = FullFileName;
            if (dlgBrowse.ShowDialog(this) == DialogResult.OK)
                FullFileName = dlgBrowse.FileName;
            BusyHide();
            DisplayInfo();
        }

        private void btnImport_Click(object sender, EventArgs e)

        {
            ParseClasses(FullFileName);
            BusyHide();
            DisplayInfo();
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
                           Strings.Format(DatabaseAPI.Database.PowerLevelVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new decimal(DatabaseAPI.Database.PowerLevelVersion.Revision);
        }

        private void frmImportPowerLevels_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.PowerLevelVersion.SourceFile;
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
                MessageBox.Show(ex.Message, "Power CSV Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            var num5 = 0;
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

                    var array = CSV.ToArray(iLine);
                    var index1 = DatabaseAPI.NidFromUidPower(array[1]);
                    if (index1 > -1)
                    {
                        DatabaseAPI.Database.Power[index1].Level = (int) Math.Round(Conversion.Val(array[2]) + 1.0);
                        var index2 = DatabaseAPI.NidFromUidPowerset(DatabaseAPI.Database.Power[index1].FullSetName);
                        if (index2 > -1 && DatabaseAPI.Database.Powersets[index2].SetType == Enums.ePowerSetType.Pool &&
                            DatabaseAPI.Database.Power[index1].Level == 1)
                            DatabaseAPI.Database.Power[index1].Level = 4;
                        ++num1;
                    }
                    else
                    {
                        ++num4;
                    }

                    ++num3;
                } while (iLine != null);
            }
            catch (Exception ex)
            {
                iStream.Close();
                MessageBox.Show(ex.Message, "Power Class CSV Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            iStream.Close();
            DatabaseAPI.Database.PowerLevelVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.PowerLevelVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.PowerLevelVersion.Revision = Convert.ToInt32(udRevision.Value);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            DisplayInfo();
            MessageBox.Show($"Parse Completed!\r\nTotal Records: {num3}\r\nGood: {num1}\r\nRejected: {num4}",
                "File Parsed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
    }
}