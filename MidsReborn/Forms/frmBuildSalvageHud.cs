using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;

namespace Mids_Reborn.Forms
{
    public partial class frmBuildSalvageHud : Form
    {
        private readonly frmMain _myParent;
        private bool _executeOnCloseUpdates = true;

        public frmBuildSalvageHud(frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;
            Load += frmBuildSalvageHud_Load;
            Closed += frmBuildSalvageHud_Closed;
            _myParent = iParent;
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
            ibClose.IA = _myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Bitmap
                : _myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[3].Bitmap
                : _myParent.Drawing.bxPower[5].Bitmap;
        }

        private void frmBuildSalvageHud_Load(object? sender, EventArgs e)
        {
            TopMost = true;
            MaximizeBox = false;
            BringToFront();
            Location = new Point(_myParent.Location.X + 2, _myParent.Location.Y + 56);
            UpdateColorTheme();
            RecalcSalvage();
            MidsContext.EnhCheckMode = true;
            _myParent.UpdateEnhCheckModeToolStrip();
            _myParent.DoRedraw();
            RecalcSalvage();
            _myParent.Activate();
        }

        private void frmBuildSalvageHud_Closed(object? sender, EventArgs e)
        {
            if (_executeOnCloseUpdates)
            {
                MidsContext.EnhCheckMode = false;
                _myParent.UpdateEnhCheckModeToolStrip();
                _myParent.DoRedraw();
            }

            _myParent.FloatBuildSalvageHud(false);
        }

        public void SetOnCloseUpdatesExecution(bool s)
        {
            _executeOnCloseUpdates = s;
        }

        private void ibClose_ButtonClicked()
        {
            Close();
        }
    }
}