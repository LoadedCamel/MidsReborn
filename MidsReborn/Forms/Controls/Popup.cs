using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class Popup : UserControl
    {
        #region Form Composting

        protected override CreateParams CreateParams
        {
            get 
            { 
                var cp = base.CreateParams;
                cp.Style &= ~0x0002;
                cp.Style &= ~0x0080;
                cp.Style &= ~0x8000;
                cp.ExStyle &= ~0x02000000;
                cp.ExStyle &= ~0x00000020;
                return cp;
            }
        }


        #endregion

        private event EventHandler<bool>? VisibilityChanged;

        private readonly Dictionary<string, Color> _type1Colors = new()
        {
            { "Title", Color.FromArgb(150, Color.LightSteelBlue) },
            { "Powerset", Color.WhiteSmoke },
            { "Available", Color.LightSteelBlue },
            { "Data", Color.WhiteSmoke },
        };

        public event EventHandler? BorderColorChanged;

        private Color _borderColor = Color.DodgerBlue;

        [Description("The font to be used on the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font Font { get; set; } = new("Microsoft Sans Serif", 9.25f, FontStyle.Bold);

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BorderStyle BorderStyle { get; set; } = BorderStyle.None;

        [Description("The border color of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                BorderColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Title { get; set; }

        private string? _powerSet;
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Powerset
        {
            get => _powerSet;
            set => _powerSet = $"Powerset: {value}";
        }

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Available { get; set; }

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string? Data { get; set; }


        private bool _isOpen;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                _isOpen = value;
                VisibilityChanged?.Invoke(this, value);
            }
        }

        public void Reset()
        {
            Title = string.Empty;
            Powerset = string.Empty;
            Available = 0;
            Data = string.Empty;
            IsOpen = false;
        }

        public Popup()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BorderColorChanged += OnBorderColorChanged;
            VisibilityChanged += OnVisibilityChanged;
        }

        private void OnVisibilityChanged(object? sender, bool e)
        {
            Visible = e;
            Invalidate();
        }

        private void OnBorderColorChanged(object? sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_isOpen)
            {
                var gfx = e.Graphics;
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                var titleSize = TextRenderer.MeasureText(gfx, Title, Font);
                var psSize = TextRenderer.MeasureText(gfx, Powerset, Font);
                var avSize = TextRenderer.MeasureText(gfx, $"Available: Level {Available}", Font);
                var dataSize = TextRenderer.MeasureText(gfx, Data, Font);
                var titleRec = new Rectangle(5, 5, titleSize.Width, titleSize.Height);
                var psRec = new Rectangle(20, titleRec.Height + 5, psSize.Width, psSize.Height);
                var avRec = new Rectangle(20, titleRec.Height + psRec.Height + 5, avSize.Width, avSize.Height);
                var dataRec = new Rectangle(3, titleRec.Height + psRec.Height + avRec.Height + 5, dataSize.Width, dataSize.Height);


                int[] szWidthArray = { titleSize.Width, psSize.Width, avSize.Width, dataSize.Width, titleRec.Width, psRec.Width, avRec.Width, dataRec.Width };
                int[] szHeightArray = { titleSize.Height, psSize.Height, avSize.Height, dataSize.Height };
                Width = szWidthArray.Max() + 30;
                Height = szHeightArray.Sum() + 10;

                var titleFont = new Font(Font, FontStyle.Underline | FontStyle.Bold);
                TextRenderer.DrawText(gfx, Title, titleFont, titleRec, _type1Colors["Title"]);
                TextRenderer.DrawText(gfx, Powerset, Font, psRec, _type1Colors["Powerset"]);
                TextRenderer.DrawText(gfx, $"Available: Level {Available}", Font, avRec, _type1Colors["Available"]);
                TextRenderer.DrawText(gfx, Data, Font, dataRec, _type1Colors["Data"]);

                //base.OnPaint(e);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            using var brush = new SolidBrush(BorderColor);
            e.Graphics.DrawRectangle(new Pen(brush, 2), ClientRectangle);
        }

        protected override void OnResize(EventArgs e) 
        {
            base.OnResize(e);
            Invalidate();
        }
    }
}
