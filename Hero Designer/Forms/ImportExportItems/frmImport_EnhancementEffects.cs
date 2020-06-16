
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Base.Data_Classes;
using Hero_Designer.My;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmImport_EnhancementEffects : Form
    {
        frmBusy bFrm;
        string FullFileName;

        public frmImport_EnhancementEffects()
        {
            Load += frmImport_EnhancementEffects_Load;
            FullFileName = "";
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmImport_EnhancementEffects));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmImport_EnhancementEffects);
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
        }

        void frmImport_EnhancementEffects_Load(object sender, EventArgs e)
        {
            FullFileName = DatabaseAPI.Database.PowerEffectVersion.SourceFile;
            DisplayInfo();
        }

        bool ParseClasses(string iFileName)
        {
            StreamReader iStream;
            try
            {
                iStream = new StreamReader(iFileName);
            }
            catch (Exception ex)
            {
                ProjectData.SetProjectError(ex);
                int num = (int)Interaction.MsgBox(ex.Message, MsgBoxStyle.Critical, "Power CSV Not Opened");
                ProjectData.ClearProjectError();
                return false;
            }
            BusyMsg("Working...");
            Enabled = false;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            int num4 = 0;
            int num5 = DatabaseAPI.Database.Enhancements.Length - 1;
            for (int index = 0; index <= num5; ++index)
            {
                IEffect[] effectArray = new IEffect[0];
                if (DatabaseAPI.Database.Enhancements[index].GetPower() is IPower power)
                    power.Effects = effectArray;
            }
            string str1 = "";
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
                string[] array = CSV.ToArray(str2);
                if (array.Length <= 0 || !(!str2.StartsWith("#") & array[0].StartsWith("Boosts.")))
                    continue;
                bool flag = true;
                string uidEnh = array[0];
                int index = DatabaseAPI.NidFromUidEnhExtended(uidEnh);
                if (index < 0)
                    flag = false;
                if (flag)
                {
                    ++num4;
                    IPower power1 = DatabaseAPI.Database.Enhancements[index].GetPower();
                    power1.FullName = uidEnh;
                    string[] strArray = power1.FullName.Split('.');
                    power1.GroupName = strArray[0];
                    power1.SetName = strArray[1];
                    power1.PowerName = strArray[2];
                    IEffect[] effectArray = (IEffect[])Utils.CopyArray(power1.Effects, new IEffect[power1.Effects.Length + 1]);
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
            }
            while (str2 != null);
            iStream.Close();
            Clipboard.SetDataObject(str1);
            Interaction.MsgBox("Import Completed!\r\nTotal Records: " + Convert.ToString(num1) + "\r\nGood: " + Convert.ToString(num4) + "\r\nRejected: " + Convert.ToString(num2) + "\r\nRejected List has been placed on the clipboard. Database will be saved when you click OK", MsgBoxStyle.Information, "Import Done");
            Enabled = true;
            BusyHide();
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveEnhancementDb(serializer);
            DisplayInfo();
            return true;
        }
    }
}