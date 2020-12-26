using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Mids_Reborn.My;
using mrbBase;
using mrbBase.Base.Data_Classes;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmImport_EnhancementEffects : Form
    {
        private frmBusy bFrm;
        private string FullFileName;

        public frmImport_EnhancementEffects()
        {
            Load += frmImport_EnhancementEffects_Load;
            FullFileName = "";
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmImport_EnhancementEffects));
            Icon = Resources.reborn;
            Name = nameof(frmImport_EnhancementEffects);
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

        private void frmImport_EnhancementEffects_Load(object sender, EventArgs e)
        {
            FullFileName = DatabaseAPI.Database.PowerEffectVersion.SourceFile;
            DisplayInfo();
        }

        private bool ParseClasses(string iFileName)
        {
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

            BusyMsg("Working...");
            Enabled = false;
            var num1 = 0;
            var num2 = 0;
            var num3 = 0;
            var num4 = 0;
            var num5 = DatabaseAPI.Database.Enhancements.Length - 1;
            for (var index = 0; index <= num5; ++index)
            {
                var effectArray = new IEffect[0];
                if (DatabaseAPI.Database.Enhancements[index].GetPower() is IPower power)
                    power.Effects = effectArray;
            }

            var str1 = "";
            string str2;
            do
            {
                str2 = FileIO.ReadLineUnlimited(iStream, char.MinValue);
                if (str2 == null)
                    continue;
                ++num3;
                if (num3 >= 101)
                {
                    BusyMsg(Strings.Format(num1, "###,##0") + " records parsed.");
                    Application.DoEvents();
                    num3 = 0;
                }

                ++num1;
                var array = CSV.ToArray(str2);
                if (array.Length <= 0 || !(!str2.StartsWith("#") & array[0].StartsWith("Boosts.")))
                    continue;
                var flag = true;
                var uidEnh = array[0];
                var index = DatabaseAPI.NidFromUidEnhExtended(uidEnh);
                if (index < 0)
                    flag = false;
                if (flag)
                {
                    ++num4;
                    var power1 = DatabaseAPI.Database.Enhancements[index].GetPower();
                    power1.FullName = uidEnh;
                    var strArray = power1.FullName.Split('.');
                    power1.GroupName = strArray[0];
                    power1.SetName = strArray[1];
                    power1.PowerName = strArray[2];
                    var effectArray =
                        (IEffect[]) Utils.CopyArray(power1.Effects, new IEffect[power1.Effects.Length + 1]);
                    power1.Effects = effectArray;
                    // this creates a reference cycle - power has an effect, that effect refers to the power
                    power1.Effects[power1.Effects.Length - 1] = new Effect(power1);
                    power1.Effects[power1.Effects.Length - 1].ImportFromCSV(str2);
                }
                else
                {
                    ++num2;
                    str1 = str1 + uidEnh + "\r\n";
                }
            } while (str2 != null);

            iStream.Close();
            Clipboard.SetDataObject(str1);
            MessageBox.Show($"Import Completed!\r\nTotal Records: {num1}\r\nGood: {num4}\r\nRejected: {num2}\r\nRejected list has been placed on the clipboard. Data will be saved when you click OK.", "Import Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Enabled = true;
            BusyHide();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveEnhancementDb(serializer);
            DisplayInfo();
            return true;
        }
    }
}