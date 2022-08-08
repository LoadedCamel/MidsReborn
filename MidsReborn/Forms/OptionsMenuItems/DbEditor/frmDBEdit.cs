using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.Forms.JsonImport;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDBEdit : Form
    {
        private Button btnClose;

        private Button btnEditEnh;

        private Button btnEditEntity;

        private Button btnEditIOSetPvE;

        private Button btnPSBrowse;

        private Button btnRecipe;

        private Button btnSalvage;
        private Button exportIndexes;


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
        private frmBusy _frmBusy;

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

        private NumericUpDown UdPageVol
        {
            get => udPageVol;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                KeyPressEventHandler pressEventHandler = udPageVol_KeyPress;
                EventHandler eventHandler = udPageVol_ValueChanged;
                if (udPageVol != null)
                {
                    udPageVol.KeyPress -= pressEventHandler;
                    udPageVol.ValueChanged -= eventHandler;
                }

                udPageVol = value;
                if (udPageVol == null)
                    return;
                udPageVol.KeyPress += pressEventHandler;
                udPageVol.ValueChanged += eventHandler;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnEditEnh_Click(object sender, EventArgs e)
        {
            using frmEnhEdit f = new frmEnhEdit();
            f.ShowDialog();
            DisplayInfo();
        }

        private void btnEditEntity_Click(object sender, EventArgs e)
        {
            using frmEntityListing f = new frmEntityListing();
            f.ShowDialog();
        }

        private void btnEditIOSet_Click(object sender, EventArgs e)
        {
            using (frmSetListing f = new frmSetListing())
            {
                f.ShowDialog();
            }
            DisplayInfo();
        }

        private void btnFileReport_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Files.FileData, "File Loading Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPSBrowse_Click(object sender, EventArgs e)
        {
            using (frmPowerBrowser f = new frmPowerBrowser())
            {
                f.ShowDialog();
            }
            DisplayInfo();
        }

        private void btnRecipe_Click(object sender, EventArgs e)
        {
            using frmRecipeEdit f = new frmRecipeEdit();
            f.ShowDialog();
        }

        private void btnSalvage_Click(object sender, EventArgs e)

        {
            using frmSalvageEdit f = new frmSalvageEdit();
            f.ShowDialog();
        }

        private void DisplayInfo()
        {
            if (MainModule.MidsController.Toon == null) return;
            lblDate.Text = DatabaseAPI.Database.Date.ToString("MM/dd/yyyy");
            UdIssue.Value = Convert.ToDecimal(DatabaseAPI.Database.Issue);
            UdPageVol.Value = Convert.ToDecimal(DatabaseAPI.Database.PageVol);
            txtPageVol.Text = DatabaseAPI.Database.PageVolText;
            lblCountAT.Text = Convert.ToString(DatabaseAPI.Database.Classes.Length, CultureInfo.InvariantCulture);
            lblCountEnh.Text = DatabaseAPI.Database.Enhancements.Length.ToString("#,###,##0");
            lblCountIOSet.Text = DatabaseAPI.Database.EnhancementSets.Count.ToString("#,###,##0");
            lblCountPS.Text = DatabaseAPI.Database.Powersets.Length.ToString("#,###,##0");
            lblCountPwr.Text = DatabaseAPI.Database.Power.Length.ToString("#,###,##0");
            txtDBVer.Text = Convert.ToString(DatabaseAPI.Database.Version, CultureInfo.InvariantCulture);
            var num1 = 0;
            var num2 = DatabaseAPI.Database.Power.Length - 1;
            for (var index = 0; index <= num2; ++index)
                num1 += DatabaseAPI.Database.Power[index].Effects.Length;
            lblCountFX.Text = num1.ToString("#,###,##0");
            var num3 = 0;
            var num4 = DatabaseAPI.Database.Recipes.Length - 1;
            for (var index = 0; index <= num4; ++index)
                num3 += DatabaseAPI.Database.Recipes[index].Item.Length;
            lblCountRecipe.Text = num3.ToString("#,###,##0");
            lblCountSalvage.Text = DatabaseAPI.Database.Salvage.Length.ToString("#,###,##0");
            Initialized = true;
        }

        private void frmDBEdit_Load(object sender, EventArgs e)
        {
            Text = $@"{DatabaseAPI.DatabaseName} Database Menu";
            DisplayInfo();
        }

        private void udIssue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized) return;
            DatabaseAPI.Database.Issue = Convert.ToInt32(UdIssue.Value);
        }

        private void udIssue_ValueChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized || !Initialized) return;
            DatabaseAPI.Database.Issue = Convert.ToInt32(UdIssue.Value);
        }

        private void udPageVol_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized) return;
            DatabaseAPI.Database.PageVol = Convert.ToInt32(UdPageVol.Value);
        }

        private void udPageVol_ValueChanged(object sender, EventArgs e)
        {
            if (!MainModule.MidsController.IsAppInitialized || !Initialized) return;
            DatabaseAPI.Database.PageVol = Convert.ToInt32(UdPageVol.Value);
        }

        private void btnExportJSON_Click(object sender, EventArgs e)
        {
            ISerialize serializer = Serializer.GetSerializer();
            using frmProgress prg = new frmProgress {WindowTitle = "DB Export progress", OperationText = "", Value = 0};
            prg.Show(this);
            DatabaseAPI.SaveJsonDatabaseProgress(serializer, prg.Handle, this);
            prg.Close();
            prg.Dispose();
        }

        private void btnJsonImporter_Click(object sender, EventArgs e)
        {
            using frmJsonImportMain f = new frmJsonImportMain();
            f.ShowDialog();
        }

        private void btnAttribModEdit_Click(object sender, EventArgs e)
        {
            using frmEditAttribMod f = new frmEditAttribMod();
            f.ShowDialog();
        }

        private void btnGCMIO_Click(object sender, EventArgs e)
        {
            var iParent = _frmMain;
            FrmGCMEditor gcmEditor = new FrmGCMEditor(ref iParent);
            gcmEditor.ShowDialog();
        }

        private readonly frmMain _frmMain;

        private void txtPageVol_MouseHover(object sender, EventArgs e)
        {
            txtPageVol.ForeColor = Color.Gold;
        }

        private void txtPageVol_Click(object sender, EventArgs e)
        {
            switch (txtPageVol.Text)
            {
                case "Page":
                    txtPageVol.Text = @"Volume";
                    break;
                case "Volume":
                    txtPageVol.Text = @"Page";
                    break;
                case "PageVol":
                    txtPageVol.Text = @"Volume";
                    break;
            }

            DatabaseAPI.Database.PageVolText = txtPageVol.Text;
        }

        private void txtPageVol_MouseLeave(object sender, EventArgs e)
        {
            txtPageVol.ForeColor = Color.White;
        }

        private async void btnDbCreate_Click(object sender, EventArgs e)
        {
            var iResult = InputBox.Show("Enter a name for your database", "Name your Database", false, "Enter the database name here", InputBox.InputBoxIcon.Info, inputBox_Validating);
            if (!iResult.OK) return;
            var dbName = iResult.Text;
            var path = Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder, dbName);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(Path.Combine(path, "Images"));
            Directory.CreateDirectory(Path.Combine(path, "Images", "Archetypes"));
            Directory.CreateDirectory(Path.Combine(path, "Images", "Enhancements"));
            Directory.CreateDirectory(Path.Combine(path, "Images", "Powersets"));
            Directory.CreateDirectory(Path.Combine(path, "Images", "Sets"));
            var files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, Files.RoamingFolder, "Generic"));
            foreach (var file in files)
            {
                File.Copy(file, Path.Combine(path, Path.GetFileName(file)));
            }

            var result = MessageBox.Show(@"Would you like to load your new database now so you can start editing?", @"Load New Database?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes) return;
            MidsContext.Config.DataPath = path;
            MidsContext.Config.SavePath = path;
            MidsContext.Config.SaveConfig(Serializer.GetSerializer());
            using var iFrm = new frmBusy();
            _frmBusy = iFrm;
            _frmBusy.SetTitle(@"Changing Database");
            _frmBusy.Show();
            await MainModule.MidsController.ChangeDatabase(_frmBusy);
        }

        private static void inputBox_Validating(object sender, InputBoxValidatingArgs e)
        {
            if (e.Text.Trim().Length != 0) return;
            e.Cancel = true;
            e.Message = "Required";
        }

        private void btnServerDataEdit_Click(object sender, EventArgs e)
        {
            using var iFrmServerData = new frmServerData();
            iFrmServerData.ShowDialog(this);
        }

        private async void btnGeneratePatch_Click(object sender, EventArgs e)
        {
            PatchCompressor compressor;
            bool patchGenerated;
            var pgQuery = !MidsContext.Config.IsLcAdmin ? new PatchGenQuery("Do You Wish To Generate A Database Patch Now?") : new PatchGenQuery("Select The Patch Type To Generate Below", true, "Database", "None");

            var result = pgQuery.ShowDialog(this);
            
            switch (result)
            {
                case GenResult.Application:
                    compressor = PatchCompressor.AppPatchCompressor;
                    patchGenerated = compressor.CreatePatchFile(AppContext.BaseDirectory, PatchCompressor.EPatchType.Application);
                    
                    break;
                case GenResult.Database:
                    compressor = PatchCompressor.DbPatchCompressor;
                    patchGenerated =compressor.CreatePatchFile(MidsContext.Config.DataPath, PatchCompressor.EPatchType.Database);
                    
                    break;
                case GenResult.Cancel:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (patchGenerated)
            {
                MessageBox.Show(@"Patch generation complete", @"Patch Generation Status", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    @"Patch generation failed due to an unknown error, please relaunch the application and try again.",
                    @"Patch Generation Status", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}