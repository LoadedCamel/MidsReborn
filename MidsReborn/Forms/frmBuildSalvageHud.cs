using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmBuildSalvageHud : Form
    {
        private readonly frmMain myParent;
        private bool executeOnCloseUpdates = true;

        public frmBuildSalvageHud(frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            Icon = Resources.reborn;
            Load += frmBuildSalvageHud_Load;
            Closed += frmBuildSalvageHud_Closed;
            myParent = iParent;
            Opacity = 0.9d;
        }

        public void RecalcSalvage()
        {
            BuildSalvageSummary.UpdateAllSalvage(lblEnhObtained, lblCatalysts, lblBoosters);
        }

        public void UpdateEnhObtained()
        {
            BuildSalvageSummary.UpdateEnhObtained(lblEnhObtained);
        }

        public void UpdateColorTheme()
        {
            ibClose.IA = myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[2].Bitmap
                : myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero()
                ? myParent.Drawing.bxPower[3].Bitmap
                : myParent.Drawing.bxPower[5].Bitmap;
        }

        private void frmBuildSalvageHud_Load(object sender, EventArgs e)
        {
            TopMost = true;
            MaximizeBox = false;
            BringToFront();
            Location = new Point(myParent.Location.X + 2, myParent.Location.Y + 56);
            UpdateColorTheme();
            RecalcSalvage();
            MidsContext.EnhCheckMode = true;
            myParent.UpdateEnhCheckModeToolStrip();
            myParent.DoRedraw();
            RecalcSalvage();
            myParent.Activate();
        }

        private void frmBuildSalvageHud_Closed(object sender, EventArgs e)
        {
            if (executeOnCloseUpdates)
            {
                MidsContext.EnhCheckMode = false;
                myParent.UpdateEnhCheckModeToolStrip();
                myParent.DoRedraw();
            }

            myParent.FloatBuildSalvageHud(false);
        }

        public void SetOnCloseUpdatesExecution(bool s)
        {
            executeOnCloseUpdates = s;
        }

        private void ibClose_ButtonClicked()
        {
            Close();
        }
    }
}