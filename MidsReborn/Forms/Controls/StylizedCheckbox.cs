using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class StylizedCheckbox : UserControl
    {
        public enum Alignment
        {
            Hero,
            Villain
        }

        public Color CheckIconColor
        {
            get => _CheckIconColor;
            set
            {
                _CheckIconColor = value;
                iconButton1.IconColor = _CheckIconColor;
            }
        }

        public Alignment CharAlignment
        {
            get => _Alignment;
            set
            {
                _Alignment = value;
                Invalidate();
            }
        }

        public bool Checked
        {
            get => _Checked;
            set
            {
                _Checked = value;
                iconButton1.Visible = _Checked;
            }
        }

        public Color BgColor
        {
            get => BackColor;
            set
            {
                BackColor = value;
                Invalidate();
            }
        }

        public int CornerRadius
        {
            get => _CornerRadius;
            set
            {
                _CornerRadius = value;
                Invalidate();
            }
        }

        public bool RemoteControlled;

        private Color _CheckIconColor = Color.Goldenrod;
        private Alignment _Alignment = Alignment.Hero;
        private bool _Checked;
        private int _CornerRadius = 4;
        private bool _Drawing;
        private bool _Hovered;

        public delegate void CheckChangedEventHandler(object sender, bool isChecked);
        public event CheckChangedEventHandler? CheckChanged;


        public StylizedCheckbox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (_Drawing)
            {
                return;
            }

            _Drawing = true;

            using (var g = pe.Graphics)
            {
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using var buttonTexture = _Hovered switch
                {
                    true => new TextureBrush(_Alignment == Alignment.Hero
                        ? MRBResourceLib.Resources.HeroButtonHover
                        : MRBResourceLib.Resources.VillainButtonHover),
                    _ => new TextureBrush(_Alignment == Alignment.Hero
                        ? MRBResourceLib.Resources.HeroButton
                        : MRBResourceLib.Resources.VillainButton),
                };

                using var gp = new GraphicsPath();
                gp.AddArc(0, 0, _CornerRadius, _CornerRadius, 180, 90);
                gp.AddArc(Width - _CornerRadius, 0, _CornerRadius, _CornerRadius, 270, 90);
                gp.AddArc(Width - _CornerRadius, Height - _CornerRadius, _CornerRadius, _CornerRadius, 0, 90);
                gp.AddArc(0, Height - _CornerRadius, _CornerRadius, _CornerRadius, 90, 90);
                g.FillPath(buttonTexture, gp);
            }

            _Drawing = false;
        }

        private void StylizedCheckbox_MouseEnter(object sender, System.EventArgs e)
        {
            if (RemoteControlled)
            {
                return;
            }

            if (_Hovered)
            {
                return;
            }

            _Hovered = true;
            Invalidate();
        }

        private void StylizedCheckbox_MouseLeave(object sender, System.EventArgs e)
        {
            if (RemoteControlled)
            {
                return;
            }

            if (!_Hovered)
            {
                return;
            }

            _Hovered = false;
            Invalidate();
        }

        private void StylizedCheckbox_MouseClick(object sender, MouseEventArgs e)
        {
            if (RemoteControlled)
            {
                return;
            }

            _Checked = !_Checked;
            CheckChanged?.Invoke(this, _Checked);
            Invalidate();
        }
    }
}
