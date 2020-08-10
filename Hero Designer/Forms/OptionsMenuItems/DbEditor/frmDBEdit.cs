using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Master_Classes;
using Hero_Designer.Forms.JsonImport;
using Hero_Designer.Forms.OptionsMenuItems.DbEditor;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmDBEdit : Form
    {
        Button btnClose;

        Button btnCSV;

        Button btnDate;

        Button btnEditEnh;

        Button btnEditEntity;

        Button btnEditIOSetPvE;

        Button btnEditIOSetPvP;

        Button btnFileReport;

        Button btnPSBrowse;

        Button btnRecipe;

        Button btnSalvage;
        Button exportIndexes;
        GroupBox GroupBox1;
        Label Label1;
        Label Label11;
        Label Label13;
        Label Label15;
        Label Label2;
        Label Label3;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label9;
        Label lblCountAT;
        Label lblCountEnh;
        Label lblCountFX;
        Label lblCountIOSet;
        Label lblCountPS;
        Label lblCountPwr;
        Label lblCountRecipe;
        Label lblCountSalvage;
        Label lblDate;

        TextBox txtDBVer;


        bool Initialized;
        NumericUpDown UdIssue
        {
            get => udIssue;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                KeyPressEventHandler pressEventHandler = udIssue_KeyPress;
                EventHandler eventHandler = udIssue_ValueChanged;
                if (udIssue != null)
                {
                    udIssue.KeyPress -= pressEventHandler;
                    udIssue.ValueChanged -= eventHandler;
                }
                udIssue = value;
                if (udIssue == null)
                    return;
                udIssue.KeyPress += pressEventHandler;
                udIssue.ValueChanged += eventHandler;
            }
        }

        public frmDBEdit()
        {
            Load += frmDBEdit_Load;
            Initialized = false;
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmDBEdit));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmDBEdit);
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        void btnCSV_Click(object sender, EventArgs e)
        {
            new frmCSV().ShowDialog();
        }

        void btnDate_Click(object sender, EventArgs e)
        {
            DatabaseAPI.Database.Date = DateTime.Now;
            DisplayInfo();
        }

        void btnEditEnh_Click(object sender, EventArgs e)
        {
            new frmEnhEdit().ShowDialog();
            DisplayInfo();
        }

        void btnEditEntity_Click(object sender, EventArgs e)
        {
            new frmEntityListing().ShowDialog();
        }

        void btnEditIOSet_Click(object sender, EventArgs e)
        {
            new frmSetListing().ShowDialog();
            DisplayInfo();
        }

        void btnEditIOSetPvP_Click(object sender, EventArgs e)
        {
            new frmSetListingPvP().ShowDialog();
            DisplayInfo();
        }

        void btnFileReport_Click(object sender, EventArgs e)
        {
            Interaction.MsgBox(Files.FileData, MsgBoxStyle.Information, "File Loading Report");
        }

        void btnPSBrowse_Click(object sender, EventArgs e)
        {
            new frmPowerBrowser().ShowDialog();
            DisplayInfo();
        }

        void btnRecipe_Click(object sender, EventArgs e)
        {
            new frmRecipeEdit().ShowDialog();
        }

        void btnSalvage_Click(object sender, EventArgs e)

        {
            new frmSalvageEdit().ShowDialog();
        }

        public void DisplayInfo()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            lblDate.Text = Strings.Format(DatabaseAPI.Database.Date, "dd/MM/yyyy");
            UdIssue.Value = Convert.ToDecimal(DatabaseAPI.Database.Issue);
            lblCountAT.Text = Convert.ToString(DatabaseAPI.Database.Classes.Length);
            lblCountEnh.Text = Strings.Format(DatabaseAPI.Database.Enhancements.Length, "#,###,##0");
            lblCountIOSet.Text = Strings.Format(DatabaseAPI.Database.EnhancementSets.Count, "#,###,##0");
            lblCountPS.Text = Strings.Format(DatabaseAPI.Database.Powersets.Length, "#,###,##0");
            lblCountPwr.Text = Strings.Format(DatabaseAPI.Database.Power.Length, "#,###,##0");
            txtDBVer.Text = Convert.ToString(DatabaseAPI.Database.Version);
            int num1 = 0;
            int num2 = DatabaseAPI.Database.Power.Length - 1;
            for (int index = 0; index <= num2; ++index)
                num1 += DatabaseAPI.Database.Power[index].Effects.Length;
            lblCountFX.Text = Strings.Format(num1, "#,###,##0");
            int num3 = 0;
            int num4 = DatabaseAPI.Database.Recipes.Length - 1;
            for (int index = 0; index <= num4; ++index)
                num3 += DatabaseAPI.Database.Recipes[index].Item.Length;
            lblCountRecipe.Text = Strings.Format(num3, "#,###,##0");
            lblCountSalvage.Text = Strings.Format(DatabaseAPI.Database.Salvage.Length, "#,###,##0");
            Initialized = true;
        }

        void frmDBEdit_Load(object sender, EventArgs e)
        {
            btnDate.Visible = MidsContext.Config.MasterMode;
            btnCSV.Visible = MidsContext.Config.MasterMode;
            txtDBVer.Enabled = MidsContext.Config.MasterMode;
            UdIssue.Enabled = MidsContext.Config.MasterMode;
            btnFileReport.Visible = MidsContext.Config.MasterMode;
            btnExportJSON.Visible = MidsContext.Config.MasterMode;
            btnJsonImporter.Visible = MidsContext.Config.MasterMode;
            DisplayInfo();
        }

        void txtDBVer_TextChanged(object sender, EventArgs e)
        {
            float num = Convert.ToSingle(txtDBVer.Text);
            if (num < 1.0)
                num = 1f;
            DatabaseAPI.Database.Version = num;
        }

        void udIssue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            DatabaseAPI.Database.Issue = Convert.ToInt32(UdIssue.Value);
        }

        void udIssue_ValueChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized || !Initialized)
                return;
            DatabaseAPI.Database.Issue = Convert.ToInt32(UdIssue.Value);
        }

        private void btnExportJSON_Click(object sender, EventArgs e)
        {
            var serializer = My.MyApplication.GetSerializer();
            DatabaseAPI.SaveJsonDatabase(serializer);
        }

        private void btnJsonImporter_Click(object sender, EventArgs e)
        {
            new frmJsonImportMain().ShowDialog();
        }

        private void btnAttribModEdit_Click(object sender, EventArgs e)
        {
            new frmEditAttribMod().ShowDialog();
        }
    }
}