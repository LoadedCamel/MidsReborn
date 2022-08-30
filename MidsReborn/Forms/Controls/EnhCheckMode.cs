using System;
using System.Diagnostics;
using System.Drawing;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class EnhCheckMode : UserControl
    {
        private readonly frmMain _myParent;


        private delegate void UpdateLabelHandler(Control control, string? text = null, Color? color = null);
        private event UpdateLabelHandler UpdateLabel;


        public string Catalyst
        {
            get => lblCatalysts.Text;
            set => UpdateLabel.Invoke(lblCatalysts, value);
        }

        public Color CatalystColor
        {
            get => lblCatalysts.ForeColor;
            set => UpdateLabel.Invoke(lblCatalysts, null, value);
        }

        public string Boosters
        {
            get => lblBoosters.Text;
            set => UpdateLabel.Invoke(lblBoosters, value);
        }

        public Color BoostersColor
        {
            get => lblBoosters.ForeColor;
            set => UpdateLabel.Invoke(lblBoosters, null, value);
        }

        public string EnhObtained
        {
            get => lblEnhObtained.Text;
            set => UpdateLabel.Invoke(lblEnhObtained, value);
        }

        public Color EnhObtainedColor
        {
            get => lblEnhObtained.ForeColor;
            set => UpdateLabel.Invoke(lblEnhObtained, null, value);
        }

        public EnhCheckMode(frmMain myParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            _myParent = myParent;
            Load += OnLoad;
            VisibleChanged += OnVisibleChanged;
            UpdateLabel += OnUpdateLabel;
            InitializeComponent();
        }

        private static void OnUpdateLabel(Control control, string? text, Color? color)
        {
            switch (control)
            {
                case Label label:
                    if (text != null) label.Text = text;
                    if (color != null) label.ForeColor = (Color)color;
                    label.Invalidate();
                    break;
            }
        }

        private void OnVisibleChanged(object? sender, EventArgs e)
        {
            if (Visible)
            {
                RecalcSalvage();
                _myParent.Activate();
                Refresh();
                MidsContext.EnhCheckMode = true;
                _myParent.UpdateEnhCheckModeToolStrip();
                return;
            }
            MidsContext.EnhCheckMode = false;
            _myParent.UpdateEnhCheckModeToolStrip();
            //_myParent.DoRedraw();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            // BringToFront();
            // Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            // UpdateColorTheme();
            // RecalcSalvage();
            // MidsContext.EnhCheckMode = true;
            // _myParent.UpdateEnhCheckModeToolStrip();
            // _myParent.DoRedraw();
        }

        private void RecalcSalvage()
        {
            BuildSalvageSummary.UpdateAllSalvage(this);
        }

        public void UpdateEnhObtained()
        {
            BuildSalvageSummary.UpdateEnhObtained(this);
        }

        private void UpdateColorTheme()
        {
            ibClose.IA = _myParent.Drawing.pImageAttributes;
            ibClose.ImageOff = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[2].Bitmap
                : _myParent.Drawing.bxPower[4].Bitmap;
            ibClose.ImageOn = MidsContext.Character.IsHero()
                ? _myParent.Drawing.bxPower[3].Bitmap
                : _myParent.Drawing.bxPower[5].Bitmap;
        }

        private void ibClose_ButtonClicked()
        {
            Hide();
        }
    }
}
