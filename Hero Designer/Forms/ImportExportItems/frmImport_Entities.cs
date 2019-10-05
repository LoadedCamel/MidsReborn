
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmImport_Entities : Form
    {
        frmBusy bFrm;

        string FullFileName;

        public frmImport_Entities()
        {
            Load += frmImport_Entities_Load;
            FullFileName = "";
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImport_Entities));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmImport_Entities);
        }

        void frmImport_Entities_Load(object sender, EventArgs e)
        {
            FullFileName = DatabaseAPI.Database.PowersetVersion.SourceFile;
            DisplayInfo();
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

        public void DisplayInfo()
        {
            lblFile.Text = FileIO.StripPath(FullFileName);
            lblDate.Text = "Date: " + Strings.Format(DatabaseAPI.Database.IOAssignmentVersion.RevisionDate, "dd/MMM/yy HH:mm:ss");
            udRevision.Value = new Decimal(DatabaseAPI.Database.IOAssignmentVersion.Revision);
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
                Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "IO CSV Not Opened");
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
                    if (iLine == null)
                        continue;
                    if (!iLine.StartsWith("#"))
                    {
                        ++num5;
                        if (num5 >= 9)
                        {
                            BusyMsg(Strings.Format(num3, "###,##0") + " records parsed.");
                            num5 = 0;
                        }
                        string[] array = CSV.ToArray(iLine);
                        string uidEntity = "";
                        if (array.Length > 1)
                        {
                            int index = -2;
                            if (array[0].StartsWith("Pets."))
                            {
                                uidEntity = "Pets_" + array[1];
                                index = DatabaseAPI.NidFromUidEntity(uidEntity);
                            }
                            else if (array[0].StartsWith("Villain_Pets."))
                            {
                                uidEntity = "Pets_" + array[1];
                                index = DatabaseAPI.NidFromUidEntity(uidEntity);
                            }
                            if (index > -2)
                            {
                                SummonedEntity.Parse(index, array[0], array[2], uidEntity);
                                ++num1;
                            }
                            else
                                ++num4;
                        }
                    }
                    ++num3;
                }
                while (iLine != null);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                Exception exception = ex;
                iStream.Close();
                Interaction.MsgBox(exception.Message, MsgBoxStyle.Critical, "Entity CSV Parse Error");
                ProjectData.ClearProjectError();
                return false;
            }
            iStream.Close();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveMainDatabase(serializer);
            DisplayInfo();
            Interaction.MsgBox(("Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) + "\r\nRejected: " + Convert.ToString(num4)), MsgBoxStyle.Information, "File Parsed");
            return true;
        }
    }
}