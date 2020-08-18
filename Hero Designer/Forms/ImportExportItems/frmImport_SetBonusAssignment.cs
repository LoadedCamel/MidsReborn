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
    public partial class frmImport_SetBonusAssignment : Form
    {
        private frmBusy bFrm;
        private Button btnClose;

        private Button btnFile;

        private Button btnImport;
        private OpenFileDialog dlgBrowse;

        private string FullFileName;
        private Label lblFile;

        public frmImport_SetBonusAssignment()
        {
            Load += frmImport_SetBonusAssignment_Load;
            FullFileName = "";
            InitializeComponent();
            Name = nameof(frmImport_SetBonusAssignment);
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_SetBonusAssignment));
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
        }

        private void frmImport_SetBonusAssignment_Load(object sender, EventArgs e)

        {
            FullFileName = DatabaseAPI.Database.PowerLevelVersion.SourceFile.Replace("powersets2", "boostsets4");
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
                ProjectData.SetProjectError(ex);
                var num2 = (int) Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Bonus CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }

            var num3 = 0;
            var num4 = 0;
            var num5 = 0;
            var num6 = DatabaseAPI.Database.EnhancementSets.Count - 1;
            for (var index1 = 0; index1 <= num6; ++index1)
            {
                DatabaseAPI.Database.EnhancementSets[index1].Bonus = new EnhancementSet.BonusItem[0];
                DatabaseAPI.Database.EnhancementSets[index1].SpecialBonus = new EnhancementSet.BonusItem[6];
                var num2 = DatabaseAPI.Database.EnhancementSets[index1].SpecialBonus.Length - 1;
                for (var index2 = 0; index2 <= num2; ++index2)
                    DatabaseAPI.Database.EnhancementSets[index1].SpecialBonus[index2] =
                        new EnhancementSet.BonusItem
                        {
                            Name = new string[0], Index = new int[0], AltString = "", Special = -1
                        };
            }

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
                    var index1 = DatabaseAPI.NidFromUidioSet(array[0]);
                    if (index1 > -1)
                    {
                        var integer = Convert.ToInt32(array[1]);
                        var strArray1 = array[3].Split(" ".ToCharArray());
                        var ePvX = Enums.ePvX.Any;
                        if (array[2].Contains("isPVPMap?"))
                        {
                            ePvX = Enums.ePvX.PvP;
                            array[2] = array[2].Replace("isPVPMap?", "").Replace("  ", " ");
                        }

                        var strArray2 = array[2].Split(" ".ToCharArray());
                        if (array[2] == "")
                        {
                            DatabaseAPI.Database.EnhancementSets[index1].Bonus =
                                (EnhancementSet.BonusItem[]) Utils.CopyArray(
                                    DatabaseAPI.Database.EnhancementSets[index1].Bonus,
                                    new EnhancementSet.BonusItem[DatabaseAPI.Database.EnhancementSets[index1].Bonus
                                        .Length + 1]);
                            DatabaseAPI.Database.EnhancementSets[index1]
                                    .Bonus[DatabaseAPI.Database.EnhancementSets[index1].Bonus.Length - 1] =
                                new EnhancementSet.BonusItem();
                            var bonus = DatabaseAPI.Database.EnhancementSets[index1].Bonus;
                            var index2 = DatabaseAPI.Database.EnhancementSets[index1].Bonus.Length - 1;
                            bonus[index2].AltString = "";
                            bonus[index2].Name = new string[strArray1.Length - 1 + 1];
                            bonus[index2].Index = new int[strArray1.Length - 1 + 1];
                            var num2 = bonus[index2].Name.Length - 1;
                            for (var index3 = 0; index3 <= num2; ++index3)
                            {
                                bonus[index2].Name[index3] = strArray1[index3];
                                bonus[index2].Index[index3] = DatabaseAPI.NidFromUidPower(strArray1[index3]);
                            }

                            bonus[index2].Special = -1;
                            bonus[index2].PvMode = ePvX;
                            bonus[index2].Slotted = integer;
                        }
                        else
                        {
                            var num2 = -1;
                            var num7 = strArray2.Length - 1;
                            for (var index2 = 0; index2 <= num7; ++index2)
                            {
                                var num8 = DatabaseAPI.NidFromUidEnh(strArray2[index2]);
                                if (num8 <= -1)
                                    continue;
                                var num9 = DatabaseAPI.Database.EnhancementSets[index1].Enhancements.Length - 1;
                                for (var index3 = 0; index3 <= num9; ++index3)
                                {
                                    if (DatabaseAPI.Database.EnhancementSets[index1].Enhancements[index3] != num8)
                                        continue;
                                    num2 = index3;
                                    break;
                                }

                                break;
                            }

                            if (num2 > -1)
                            {
                                var specialBonus = DatabaseAPI.Database.EnhancementSets[index1].SpecialBonus;
                                var index2 = num2;
                                specialBonus[index2].AltString = "";
                                specialBonus[index2].Name = new string[strArray1.Length - 1 + 1];
                                specialBonus[index2].Index = new int[strArray1.Length - 1 + 1];
                                var num8 = specialBonus[index2].Name.Length - 1;
                                for (var index3 = 0; index3 <= num8; ++index3)
                                {
                                    specialBonus[index2].Name[index3] = strArray1[index3];
                                    specialBonus[index2].Index[index3] = DatabaseAPI.NidFromUidPower(strArray1[index3]);
                                }

                                specialBonus[index2].Special = num2;
                                specialBonus[index2].PvMode = ePvX;
                                specialBonus[index2].Slotted = integer;
                            }
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
                ProjectData.SetProjectError(ex);
                var exception = ex;
                iStream.Close();
                var num2 = (int) Interaction.MsgBox(exception.Message, MsgBoxStyle.Critical,
                    "Power Class CSV Parse Error");
                var flag = false;
                ProjectData.ClearProjectError();
                return flag;
            }

            iStream.Close();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveEnhancementDb(serializer);
            DisplayInfo();
            var num10 = (int) Interaction.MsgBox(
                "Parse Completed!\r\nTotal Records: " + Convert.ToString(num3) + "\r\nGood: " + Convert.ToString(num1) +
                "\r\nRejected: " + Convert.ToString(num4), MsgBoxStyle.Information, "File Parsed");
            return true;
        }
    }
}