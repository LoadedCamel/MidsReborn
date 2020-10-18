using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Base.Master_Classes;
using Hero_Designer.Forms.JsonImport;
using Hero_Designer.My;
using Microsoft.VisualBasic;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDBEdit : Form
    {
        private Button btnClose;

        private Button btnCSV;

        private Button btnDate;

        private Button btnEditEnh;

        private Button btnEditEntity;

        private Button btnEditIOSetPvE;

        private Button btnEditIOSetPvP;

        private Button btnFileReport;

        private Button btnPSBrowse;

        private Button btnRecipe;

        private Button btnSalvage;
        private Button exportIndexes;
        private GroupBox GroupBox1;


        private bool Initialized;
        private Label Label1;
        private Label Label11;
        private Label Label13;
        private Label Label15;
        private Label Label2;
        private Label Label3;
        private Label Label4;
        private Label Label5;
        private Label Label6;
        private Label Label7;
        private Label Label9;
        private Label lblCountAT;
        private Label lblCountEnh;
        private Label lblCountFX;
        private Label lblCountIOSet;
        private Label lblCountPS;
        private Label lblCountPwr;
        private Label lblCountRecipe;
        private Label lblCountSalvage;
        private Label lblDate;

        private TextBox txtDBVer;

        public frmDBEdit()
        {
            Load += frmDBEdit_Load;
            Initialized = false;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmDBEdit));
            Icon = Resources.reborn;
            Name = nameof(frmDBEdit);
        }

        private NumericUpDown UdIssue
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnCSV_Click(object sender, EventArgs e)
        {
            new frmCSV().ShowDialog();
        }

        private void btnDate_Click(object sender, EventArgs e)
        {
            DatabaseAPI.Database.Date = DateTime.Now;
            DisplayInfo();
        }

        private void btnEditEnh_Click(object sender, EventArgs e)
        {
            new frmEnhEdit().ShowDialog();
            DisplayInfo();
        }

        private void btnEditEntity_Click(object sender, EventArgs e)
        {
            new frmEntityListing().ShowDialog();
        }

        private void btnEditIOSet_Click(object sender, EventArgs e)
        {
            new frmSetListing().ShowDialog();
            DisplayInfo();
        }

        private void btnEditIOSetPvP_Click(object sender, EventArgs e)
        {
            new frmSetListingPvP().ShowDialog();
            DisplayInfo();
        }

        private void btnFileReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Files.FileData, "File Loading Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPSBrowse_Click(object sender, EventArgs e)
        {
            new frmPowerBrowser().ShowDialog();
            DisplayInfo();
        }

        private void btnRecipe_Click(object sender, EventArgs e)
        {
            new frmRecipeEdit().ShowDialog();
        }

        private void btnSalvage_Click(object sender, EventArgs e)

        {
            new frmSalvageEdit().ShowDialog();
        }

        private void DisplayInfo()
        {
            if (MainModule.MidsController.Toon == null)
                return;
            lblDate.Text = Strings.Format(DatabaseAPI.Database.Date, "dd/MM/yyyy");
            UdIssue.Value = Convert.ToDecimal(DatabaseAPI.Database.Issue);
            lblCountAT.Text = Convert.ToString(DatabaseAPI.Database.Classes.Length, CultureInfo.InvariantCulture);
            lblCountEnh.Text = Strings.Format(DatabaseAPI.Database.Enhancements.Length, "#,###,##0");
            lblCountIOSet.Text = Strings.Format(DatabaseAPI.Database.EnhancementSets.Count, "#,###,##0");
            lblCountPS.Text = Strings.Format(DatabaseAPI.Database.Powersets.Length, "#,###,##0");
            lblCountPwr.Text = Strings.Format(DatabaseAPI.Database.Power.Length, "#,###,##0");
            txtDBVer.Text = Convert.ToString(DatabaseAPI.Database.Version, CultureInfo.InvariantCulture);
            var num1 = 0;
            var num2 = DatabaseAPI.Database.Power.Length - 1;
            for (var index = 0; index <= num2; ++index)
                num1 += DatabaseAPI.Database.Power[index].Effects.Length;
            lblCountFX.Text = Strings.Format(num1, "#,###,##0");
            var num3 = 0;
            var num4 = DatabaseAPI.Database.Recipes.Length - 1;
            for (var index = 0; index <= num4; ++index)
                num3 += DatabaseAPI.Database.Recipes[index].Item.Length;
            lblCountRecipe.Text = Strings.Format(num3, "#,###,##0");
            lblCountSalvage.Text = Strings.Format(DatabaseAPI.Database.Salvage.Length, "#,###,##0");
            Initialized = true;
        }

        private void frmDBEdit_Load(object sender, EventArgs e)
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

        private void txtDBVer_TextChanged(object sender, EventArgs e)
        {
            var num = Convert.ToSingle(txtDBVer.Text);
            if (num < 1.0)
                num = 1f;
            DatabaseAPI.Database.Version = num;
        }

        private void udIssue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized)
                return;
            DatabaseAPI.Database.Issue = Convert.ToInt32(UdIssue.Value);
        }

        private void udIssue_ValueChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized || !Initialized)
                return;
            DatabaseAPI.Database.Issue = Convert.ToInt32(UdIssue.Value);
        }

        private void btnExportJSON_Click(object sender, EventArgs e)
        {
            var serializer = MyApplication.GetSerializer();
            DatabaseAPI.SaveJsonDatabase(serializer);
        }

        private void btnJsonImporter_Click(object sender, EventArgs e)
        {
            new frmJsonImportMain().ShowDialog();
        }

        private void btnExportAttrib_Click(object sender, EventArgs e)
        {
            DatabaseAPI.ExportAttribMods();
        }

        private void btnExportGCM_Click(object sender, EventArgs e)
        {
            DatabaseAPI.ExportGlobalChanceMods();
        }
    }
}