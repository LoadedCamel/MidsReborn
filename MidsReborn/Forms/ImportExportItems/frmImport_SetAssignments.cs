using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.My;
using mrbBase;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmImport_SetAssignments : Form
    {
        private frmBusy bFrm;
        private Button btnClose;

        private Button btnFile;

        private Button btnImport;
        private OpenFileDialog dlgBrowse;

        private string FullFileName;
        private Label Label8;
        private Label lblDate;
        private Label lblFile;
        private NumericUpDown udRevision;

        public frmImport_SetAssignments()
        {
            Load += frmImport_SetAssignments_Load;
            FullFileName = "";
            InitializeComponent();
            Name = nameof(frmImport_SetAssignments);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_SetAssignments));
            Icon = Resources.reborn;
        }

        private void AddSetType(int nIDPower, Enums.eSetType nSetType)
        {
            if (!((nIDPower > -1) & (nIDPower < DatabaseAPI.Database.Power.Length)))
                return;
            var num = DatabaseAPI.Database.Power[nIDPower].SetTypes.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (DatabaseAPI.Database.Power[nIDPower].SetTypes[index] == nSetType)
                    return;
            var power = DatabaseAPI.Database.Power;
            var eSetTypeArray = (Enums.eSetType[]) Utils.CopyArray(power[nIDPower].SetTypes,
                new Enums.eSetType[DatabaseAPI.Database.Power[nIDPower].SetTypes.Length + 1]);
            power[nIDPower].SetTypes = eSetTypeArray;
            DatabaseAPI.Database.Power[nIDPower].SetTypes[DatabaseAPI.Database.Power[nIDPower].SetTypes.Length - 1] =
                nSetType;
            Array.Sort(DatabaseAPI.Database.Power[nIDPower].SetTypes);
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
            lblDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.IOAssignmentVersion.RevisionDate,
                "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new decimal(DatabaseAPI.Database.IOAssignmentVersion.Revision);
        }


        private void frmImport_SetAssignments_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.IOAssignmentVersion.SourceFile;
            DisplayInfo();
        }

        [DebuggerStepThrough]
        private bool ParseClasses(string iFileName)

        {
            var num1 = 0;
            var eSetType = Enums.eSetType.Untyped;
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO CSV Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            var num5 = 0;
            var flagArray = new bool[Enum.GetValues(eSetType.GetType()).Length - 1 + 1];
            var index1 = -1;
            var num6 = DatabaseAPI.Database.Power.Length - 1;
            for (var index2 = 0; index2 <= num6; ++index2)
                DatabaseAPI.Database.Power[index2].SetTypes = new Enums.eSetType[0];
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
                    if (array.Length > 1)
                    {
                        var num2 = DatabaseAPI.NidFromUidioSet(array[0]);
                        if ((num2 != index1) & (index1 > -1))
                            flagArray[(int) DatabaseAPI.Database.EnhancementSets[index1].SetType] = true;
                        index1 = num2;
                        if (index1 > -1 && !flagArray[(int) DatabaseAPI.Database.EnhancementSets[index1].SetType])
                        {
                            var nIDPower = DatabaseAPI.NidFromUidPower(array[1]);
                            if (nIDPower > -1)
                                AddSetType(nIDPower, DatabaseAPI.Database.EnhancementSets[index1].SetType);
                        }

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
                MessageBox.Show(ex.Message, "IO CSV Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            iStream.Close();
            DatabaseAPI.Database.IOAssignmentVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.IOAssignmentVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.IOAssignmentVersion.Revision = Convert.ToInt32(udRevision.Value);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            DisplayInfo();
            MessageBox.Show($"Parse Completed!\r\nTotal Records: {num3}\r\nGood: {num1}\r\nRejected: {num4}",
                "File Parsed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return true;
        }
    }
}