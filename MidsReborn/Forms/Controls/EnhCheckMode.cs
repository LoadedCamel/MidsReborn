using System;
using System.Diagnostics;
using System.Drawing;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Windows.Forms;
using System.ComponentModel;
using Mids_Reborn.Core;

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
            Invalidate(true);
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            BringToFront();
            Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            //imageButtonEx1.UseAlt = !MidsContext.Character.IsHero();
            MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
        }

        private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment e)
        {
            imageButtonEx1.UseAlt = e switch
            {
                Enums.Alignment.Hero => false,
                Enums.Alignment.Villain => true,
                _ => imageButtonEx1.UseAlt
            };
        }

        private void RecalcSalvage()
        {
            BuildSalvageSummary.UpdateAllSalvage(this);
        }

        public void UpdateEnhObtained()
        {
            BuildSalvageSummary.UpdateEnhObtained(this);
        }

        private void imageButtonEx1_Click(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
