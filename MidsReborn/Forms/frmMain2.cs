using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mrbBase;
using mrbBase.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class FrmMain2 : Form
    {
        private frmInitializing _frmInitializing;
        private bool Loading { get; set; }

        public FrmMain2()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            ConfigData.Initialize(Serializer.GetSerializer());
            Load += FrmMain2_Load;
            InitializeComponent();
        }

        private void FrmMain2_Load(object sender, EventArgs e)
        {
            if (MidsContext.Config.I9.DefaultIOLevel == 27)
            {
                MidsContext.Config.I9.DefaultIOLevel = 49;
            }

            using frmInitializing iFrm = new frmInitializing();
            _frmInitializing = iFrm;
            _frmInitializing.Show();
            if (!this.IsInDesignMode() && !MidsContext.Config.IsInitialized)
            {
                MidsContext.Config.CheckForUpdates = false;
                MidsContext.Config.DefaultSaveFolderOverride = null;
                MidsContext.Config.CreateDefaultSaveFolder();
                MidsContext.Config.IsInitialized = true;
            }
            MainModule.MidsController.LoadData(ref _frmInitializing);
            _frmInitializing?.SetMessage("Setting up UI...");

            Show();
            _frmInitializing.Hide();
            _frmInitializing.Close();
            Refresh();
            Loading = false;
        }

        private void tsAdvResetTips_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsPatchNotes_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsBugReportCrytilis_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TsForumLink(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsKoFi_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsPatreon_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TsCrytilisLink(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsHelperShort2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsHelperLong2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AccoladesWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void IncarnateWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TemporaryPowersWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToPlus5_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToTO_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToDO_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToSO_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsToggleCheckModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsIODefault_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsIOMin_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsIOMax_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsAdvFreshInstall_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsAdvDBEdit_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsUpdateCheck_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsConfig_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsExportDiscord_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsGenFreebies_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsExportDataLink_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsExportLong_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsExport_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsImport_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFileQuit_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFilePrint_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFileSaveAs_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFileSave_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFileOpen_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFileNew_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToPlus4_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToPlus3_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToPlus2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToPlus1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToEven_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToMinus1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsFlipAllEnh_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToNone_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToMinus3_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsEnhToMinus2_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsClearAllEnh_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsRemoveAllSlots_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AutoArrangeAllSlotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsView2Col_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsView3Col_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsView4Col_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsView5Col_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsView6Col_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewIOLevels_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewSOLevels_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewRelative_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewRelativeAsSigns_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewSlotLevels_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewActualDamage_New_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewDPS_New_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tlsDPA_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewSets_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewGraphs_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewSetCompare_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewData_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsViewTotals_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsRecipeViewer_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsDPSCalc_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsSetFind_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsHelperShort_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void tsHelperLong_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void sbMode_ButtonClicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ibSets_ButtonClicked()
        {
            throw new NotImplementedException();
        }
    }
}
