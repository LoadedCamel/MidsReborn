using System;
using System.Drawing;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.Controls
{
    public partial class EnhCheckMode : UserControl
    {
        private delegate void UpdateLabelHandler(Control control, string? text = null, Color? color = null);
        private event UpdateLabelHandler UpdateLabel;

        private readonly frmMain _myParent;
        private bool _UseNegativeCount;

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

        public bool UseNegativeCount
        {
            get => _UseNegativeCount;
            set
            {
                if (DesignMode)
                {
                    lblEnhObtained.Text = value
                        ? "50 to go"
                        : "Obtained: 100/100";
                }

                _UseNegativeCount = value;
            }
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
            MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
            _UseNegativeCount = false;
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
            BuildSalvageSummary.UpdateAllSalvage(this, _UseNegativeCount);
        }

        public void UpdateEnhObtained()
        {
            BuildSalvageSummary.UpdateEnhObtained(this, _UseNegativeCount);
        }

        private void imageButtonEx1_Click(object sender, EventArgs e)
        {
            MidsContext.EnhCheckMode = false;
            _myParent.ChildRequestedRedraw();
            _myParent.UpdateEnhCheckModeToolStrip();
            Hide();
        }

        private void lblEnhObtained_MouseClick(object sender, MouseEventArgs e)
        {
            if (MidsContext.Character == null)
            {
                return;
            }

            if (MidsContext.Character.CurrentBuild == null)
            {
                return;
            }

            switch (e.Button)
            {
                case MouseButtons.Left:
                    foreach (var pe in MidsContext.Character.CurrentBuild.Powers)
                    {
                        if (pe.Power == null)
                        {
                            continue;
                        }

                        foreach (var s in pe.Slots)
                        {
                            s.Enhancement.Obtained = !s.Enhancement.Obtained;
                            s.FlippedEnhancement.Obtained = s.Enhancement.Obtained;
                        }
                    }

                    _myParent.ChildRequestedRedraw();
                    break;

                case MouseButtons.Right:
                    _UseNegativeCount = !_UseNegativeCount;
                    break;

                default:
                    return;
            }

            UpdateEnhObtained();
        }
    }
}
