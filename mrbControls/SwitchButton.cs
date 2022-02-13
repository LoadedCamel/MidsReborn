using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using mrbControls.Properties;

namespace mrbControls
{
    public partial class SwitchButton : UserControl
    {
        #region Declarations

        private readonly OutlinedLabelSub _outlinedLabel;

        #endregion

        #region Events

        public new event EventHandler Click
        {
            add
            {
                base.Click += value;
                foreach (Control control in Controls)
                {
                    control.Click += value;
                }
            }
            remove
            {
                base.Click -= value;
                foreach (Control control in Controls)
                {
                    control.Click -= value;
                }
            }
        }

        private new event EventHandler MouseEnter
        {
            add
            {
                base.MouseEnter += value;
                foreach (Control control in Controls)
                {
                    control.MouseEnter += value;
                }
            }
            remove
            {
                base.MouseEnter -= value;
                foreach (Control control in Controls)
                {
                    control.MouseEnter -= value;
                }
            }
        }

        private new event EventHandler MouseLeave
        {
            add
            {
                base.MouseLeave += value;
                foreach (Control control in Controls)
                {
                    control.MouseLeave += value;
                }
            }
            remove
            {
                base.MouseLeave -= value;
                foreach (Control control in Controls)
                {
                    control.MouseLeave -= value;
                }
            }
        }

        #endregion

        #region Custom Events

        private event EventHandler<string> DisplayTextChanged;
        private new event EventHandler<Font> FontChanged;
        private new event EventHandler<Color> ForeColorChanged;
        private event EventHandler<Image> ImageChanged;
        private static event EventHandler<bool> OutlineEnablementChanged;
        private static event EventHandler<int> OutlineWidthChanged;
        private static event EventHandler<Color> OutlineColorChanged;
        private event EventHandler<SwitchState> SwitchStateChanged;

        #endregion

        #region Properties

        private SwitchState _switchedState;

        public SwitchState SwitchedState
        {
            get => _switchedState;
            set
            {
                _switchedState = value;
                SwitchStateChanged?.Invoke(this, value);
            }
        }
        private string _displayText;

        public string DisplayText
        {
            get => _displayText;
            set
            {
                _displayText = value;
                DisplayTextChanged?.Invoke(this, value);
            }
        }
        private Image _image;
        private Color _foreColor;
        private Font _font;

        #endregion

        #region Designer Properties (HIDDEN)

        [Browsable(false)] public override Color BackColor { get; set; } = Color.Transparent;
        [Browsable(false)] public override Image BackgroundImage { get; set; }
        [Browsable(false)] public override ImageLayout BackgroundImageLayout { get; set; } = ImageLayout.Stretch;
        [Browsable(false)] public override string Text { get; set; }

        #endregion

        #region Designer Properties (VISIBLE)

        [Description("The font used to display text in the control.")]
        [Category("SwitchButton Settings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font Font
        {
            get => _font;
            set
            {
                _font = value;
                FontChanged?.Invoke(this, value);
            }
        }

        [Description("The color used for the display text of the control.")]
        [Category("SwitchButton Settings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = value;
                ForeColorChanged?.Invoke(this, value);
            }
        }

        [Description("The image to be used when hovering over the control.")]
        [Category("SwitchButton Settings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image HoverImage { get; set; } = Resources.pSlot3;
        
        [Description("The image to be used for the control.")]
        [Category("SwitchButton Settings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Image
        {
            get => _image;
            set
            {
                _image = value;
                ImageChanged?.Invoke(this, value);
            }
        }

        [Description("Indicates whether the text should be outlined or not.")]
        [Category("SwitchButton Settings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public SwitchButtonOutline Outline { get; set; }

        [Description("The text to be used by the control when in switch mode.")]
        [Category("SwitchButton Settings")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public SwitchButtonStateText SwitchText { get; set; }

        #endregion

        #region Enums

        public enum SwitchState
        {
            None,
            StateA,
            StateB,
            StateC
        }

        #endregion

        #region TypeConverters

        internal class SwitchButtonOutlineTypeConverter : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(typeof(SwitchButtonOutline));
            }
        }

        [TypeConverter(typeof(SwitchButtonOutlineTypeConverter))]
        [Serializable]
        public class SwitchButtonOutline
        {
            private int _width;
            private bool _enabled;
            private Color _color;

            [Description("Enables the controls label text to be outlined.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    _enabled = value;
                    OutlineEnablementChanged?.Invoke(this, value);
                }
            }

            [Description("Sets the color of outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Color Color
            {
                get => _color;
                set
                {
                    _color = value;
                    OutlineColorChanged?.Invoke(this, value);
                }
            }

            [Description("Sets the width of the outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public int Width
            {
                get => _width;
                set
                {
                    _width = value;
                    OutlineWidthChanged?.Invoke(this, value);
                }
            }

            public override string ToString()
            {
                return $"{Enabled}, {Color}, {Width}";
            }
        }

        public class SwitchButtonStateTextTypeConverter : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(typeof(SwitchButtonOutline));
            }
        }

        [TypeConverter(typeof(SwitchButtonStateTextTypeConverter))]
        [Serializable]
        public class SwitchButtonStateText
        {
            [Description("Text to set for the 1st state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string StateA { get; set; }

            [Description("Text to set for the 2nd state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string StateB { get; set; }

            [Description("Text to set for the 3rd state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string StateC { get; set; }

            public override string ToString()
            {
                return $"{StateA}, {StateB}, {StateC}";
            }
        }

        #endregion

        #region Constructor

        public SwitchButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint  | ControlStyles.Opaque | ControlStyles.ResizeRedraw, true);
            SuspendLayout();
            _outlinedLabel = new OutlinedLabelSub(this);
            _outlinedLabel.AutoSize = false;
            _outlinedLabel.Font = Font;
            _outlinedLabel.Size = new Size(Width, Height / 2);
            _outlinedLabel.Location = new Point(0, Height - _outlinedLabel.Height / 2);
            _outlinedLabel.TextAlign = ContentAlignment.MiddleCenter;
            _outlinedLabel.BackColor = Color.Transparent;
            _outlinedLabel.ForeColor = ForeColor;
            _outlinedLabel.Text = "";
            _outlinedLabel.OutlineColor = Color.Black;
            _outlinedLabel.OutlineEnabled = true;
            _outlinedLabel.OutlineWidth = 2;

            Controls.Add(_outlinedLabel);
            ResumeLayout(false);
            PerformLayout();

            Click += OnClicked;
            DisplayTextChanged += OnDisplayTextChanged;
            FontChanged += OnFontChanged;
            ForeColorChanged += OnForeColorChanged;
            ImageChanged += OnImageChanged;
            OutlineEnablementChanged += OnOutlineEnablementChanged;
            OutlineColorChanged += OnOutlineColorChanged;
            OutlineWidthChanged += OnOutlineWidthChanged;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            SwitchStateChanged += OnSwitchStateChanged;

            InitializeComponent();

            Font = DefaultFont;
            ForeColor = Color.Azure;
            Image = Resources.pSlot2;
            SwitchedState = SwitchState.None;
        }

        #endregion

        #region Methods

        private void GetState()
        {
            
        }

        #endregion

        #region Override Methods

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            pevent.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            pevent.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;
            pevent.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            pevent.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            pevent.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            pevent.Graphics.CompositingMode = CompositingMode.SourceCopy;
            if (BackgroundImage != null)
            {
                pevent.Graphics.DrawImage(BackgroundImage, ClientRectangle);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_TRANSPARENT = 0x20;
                var createParams = base.CreateParams;
                createParams.ExStyle |= WS_EX_TRANSPARENT;
                return createParams;
            }
        }

        #endregion

        #region EventHandler Methods

        private void OnDisplayTextChanged(object sender, string e)
        {
            _outlinedLabel.Text = e;
            _outlinedLabel.Invalidate();
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Hand;
            BackgroundImage = HoverImage;
            Invalidate();
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            BackgroundImage = Image;
            Invalidate();
        }

        private void OnForeColorChanged(object sender, Color e)
        {
            _outlinedLabel.ForeColor = e;
            _outlinedLabel.Invalidate();
        }

        private void OnFontChanged(object sender, Font e)
        {
            _outlinedLabel.Font = e;
            _outlinedLabel.Invalidate();
        }

        private void OnImageChanged(object sender, Image e)
        {
            BackgroundImage = e;
            Invalidate();
        }

        private void OnOutlineEnablementChanged(object sender, bool e)
        {
            _outlinedLabel.OutlineEnabled = e;
            _outlinedLabel.Invalidate();
        }

        private void OnOutlineColorChanged(object sender, Color e)
        {
            _outlinedLabel.OutlineColor = e;
            _outlinedLabel.Invalidate();
        }

        private void OnOutlineWidthChanged(object sender, int e)
        {
            _outlinedLabel.OutlineWidth = e;
            _outlinedLabel.Invalidate();
        }

        
        private void OnClicked(object sender, EventArgs e)
        {
            switch (SwitchedState)
            {
                case SwitchState.None:
                    SwitchedState = SwitchState.StateA;
                    break;
                case SwitchState.StateA:
                    SwitchedState = SwitchState.StateB;
                    break;
                case SwitchState.StateB:
                    SwitchedState = SwitchState.StateC;
                    break;
                case SwitchState.StateC:
                    SwitchedState = SwitchState.StateA;
                    break;
            }
        }

        private void OnSwitchStateChanged(object sender, SwitchState e)
        {
            switch (e)
            {
                case SwitchState.None:
                    DisplayText = "InitialState";
                    break;
                case SwitchState.StateA:
                    DisplayText = SwitchText.StateA;
                    break;
                case SwitchState.StateB:
                    DisplayText = SwitchText.StateB;
                    break;
                case SwitchState.StateC:
                    DisplayText = SwitchText.StateC;
                    break;
            }
        }

        #endregion

        #region SubControls

        internal sealed class OutlinedLabelSub : Label
        {
            public new Size Size { get; set; }
            public override Color ForeColor { get; set; }
            public override Color BackColor { get; set; } = Color.Transparent;
            public override ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleCenter;
            public Color OutlineColor { get; set; }
            public float OutlineWidth { get; set; }
            public bool OutlineEnabled { get; set; }
            public override string Text { get; set; }

            private PointF _point;
            private SizeF _drawSize;

            public OutlinedLabelSub(SwitchButton parent)
            {
                Dock = DockStyle.Fill;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                e.Graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                if (OutlineEnabled)
                {
                    using var gp = new GraphicsPath();
                    using var outline = new Pen(OutlineColor, OutlineWidth) { LineJoin = LineJoin.Round };
                    using Brush foreBrush = new SolidBrush(ForeColor);

                    _drawSize = TextRenderer.MeasureText(Text, Font);

                    if (AutoSize)
                    {
                        _point.X = Padding.Left;
                        _point.Y = Padding.Top;
                    }
                    else
                    {
                        _point.X = TextAlign switch
                        {
                            ContentAlignment.TopLeft or ContentAlignment.MiddleLeft or ContentAlignment.BottomLeft => Padding.Left,
                            ContentAlignment.TopCenter or ContentAlignment.MiddleCenter or ContentAlignment.BottomCenter => (Width - _drawSize.Width) / 2,
                            _ => Width - (Padding.Right + _drawSize.Width)
                        };

                        _point.Y = TextAlign switch
                        {
                            ContentAlignment.TopLeft or ContentAlignment.TopCenter or ContentAlignment.TopRight => Padding.Top,
                            ContentAlignment.MiddleLeft or ContentAlignment.MiddleCenter or ContentAlignment.MiddleRight => (Height - _drawSize.Height) / 2,
                            _ => Height - (Padding.Bottom + _drawSize.Height)
                        };
                    }

                    var fontSize = e.Graphics.DpiY * Font.SizeInPoints / 72;
                    gp.Reset();
                    gp.AddString(Text, Font.FontFamily, (int)Font.Style, fontSize, _point, StringFormat.GenericTypographic);
                    e.Graphics.DrawPath(outline, gp);
                    e.Graphics.FillPath(foreBrush, gp);
                }
                else
                {
                    base.OnPaint(e);
                }
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    const int WS_EX_TRANSPARENT = 0x20;
                    var createParams = base.CreateParams;
                    createParams.ExStyle |= WS_EX_TRANSPARENT;
                    return createParams;
                }
            }
        }

        #endregion

    }
}
