
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
    public partial class frmImport_SetAssignments : Form
    {
        Button btnClose;

        Button btnFile;

        Button btnImport;
        OpenFileDialog dlgBrowse;
        Label Label8;
        Label lblDate;
        Label lblFile;
        NumericUpDown udRevision;

        frmBusy bFrm;

        string FullFileName;

        public frmImport_SetAssignments()
        {
            Load += frmImport_SetAssignments_Load;
            FullFileName = "";
            InitializeComponent();
            Name = nameof(frmImport_SetAssignments);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImport_SetAssignments));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
        }

        private void AddSetType(int nIDPower, Enums.eSetType nSetType)
        {
            if (!(nIDPower > -1 & nIDPower < DatabaseAPI.Database.Power.Length))
                return;
            int num = DatabaseAPI.Database.Power[nIDPower].SetTypes.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (DatabaseAPI.Database.Power[nIDPower].SetTypes[index] == nSetType)
                    return;
            }
            IPower[] power = DatabaseAPI.Database.Power;
            Enums.eSetType[] eSetTypeArray = (Enums.eSetType[])Utils.CopyArray(power[nIDPower].SetTypes, new Enums.eSetType[DatabaseAPI.Database.Power[nIDPower].SetTypes.Length + 1]);
            power[nIDPower].SetTypes = eSetTypeArray;
            DatabaseAPI.Database.Power[nIDPower].SetTypes[DatabaseAPI.Database.Power[nIDPower].SetTypes.Length - 1] = nSetType;
            Array.Sort(DatabaseAPI.Database.Power[nIDPower].SetTypes);
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
            lblDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.IOAssignmentVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new Decimal(DatabaseAPI.Database.IOAssignmentVersion.Revision);
        }


        void frmImport_SetAssignments_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.IOAssignmentVersion.SourceFile;
            DisplayInfo();
        }

        [DebuggerStepThrough]

        bool ParseClasses(string iFileName)

        {
            int num1 = 0;
            Enums.eSetType eSetType = Enums.eSetType.Untyped;
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num2 = (int)Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "IO CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            bool[] flagArray = new bool[Enum.GetValues(eSetType.GetType()).Length - 1 + 1];
            int index1 = -1;
            int num6 = DatabaseAPI.Database.Power.Length - 1;
            for (int index2 = 0; index2 <= num6; ++index2)
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
                    string[] array = CSV.ToArray(iLine);
                    if (array.Length > 1)
                    {
                        int num2 = DatabaseAPI.NidFromUidioSet(array[0]);
                        if (num2 != index1 & index1 > -1)
                            flagArray[(int)DatabaseAPI.Database.EnhancementSets[index1].SetType] = true;
                        index1 = num2;
                        if (index1 > -1 && !flagArray[(int)DatabaseAPI.Database.EnhancementSets[index1].SetType])
                        {
                            int nIDPower = DatabaseAPI.NidFromUidPower(array[1]);
                            if (nIDPower > -1)
                                AddSetType(nIDPower, DatabaseAPI.Database.EnhancementSets[index1].SetType);
                        }
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
                int num2 = (int)Interaction.MsgBox(exception.Message, MsgBoxStyle.Critical, "IO CSV Parse Error");
                ProjectData.ClearProjectError();
                return false;
            }
            iStream.Close();
            DatabaseAPI.Database.IOAssignmentVersion.SourceFile = dlgBrowse.FileName;
            DatabaseAPI.Database.IOAssignmentVersion.RevisionDate = DateTime.Now;
            DatabaseAPI.Database.IOAssignmentVersion.Revision = Convert.ToInt32(udRevision.Value);
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            DisplayInfo();
            int num7 = (int)Interaction.MsgBox("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nRejected: " + Convert.ToString(num4), MsgBoxStyle.Information, "File Parsed");
            return true;
        }
    }
}