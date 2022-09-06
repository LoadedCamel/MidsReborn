using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    [DefaultEvent("Click")]
    public partial class ImageButtonEx : UserControl
    {
        #region Hidden Designer Properties

        [Browsable(false)] public override Color BackColor { get; set; }
        [Browsable(false)] public override Image? BackgroundImage { get; set; }
        [Browsable(false)] public override ImageLayout BackgroundImageLayout { get; set; }
        [Browsable(false)] public new BorderStyle BorderStyle { get; set; }
        [Browsable(false)] public new ImeMode ImeMode { get; set; }
        [Browsable(false)] public override AutoValidate AutoValidate { get; set; }

        #endregion

        #region Events

        private new event EventHandler<Font>? FontChanged;
        private new event EventHandler<Color>? ForeColorChanged;
        private static event EventHandler<Image?>? ImageChanged;
        private new event EventHandler<string?>? TextChanged;
        private event EventHandler<bool> ThreeStateChanged;
        private event EventHandler<ButtonTypes>? ButtonTypeChanged;
        private event EventHandler<bool>? UseAltChanged;

        private static event EventHandler<int>? OutlineWidthChanged;
        private static event EventHandler<Color>? OutlineColorChanged;

        private delegate void StateChangedEventHandler(States state);

        private event StateChangedEventHandler? StateChanged;

        public new event EventHandler? Click;

        #endregion

        #region Enums

        public enum ButtonTypes
        {
            Normal,
            Toggle
        }

        public enum States
        {
            ToggledOff,
            ToggledOn,
            Indeterminate
        }

        #endregion

        #region Private Properties

        private Font _font = DefaultFont;
        private static Color _foreColor = Color.WhiteSmoke;

        private Color _currentTextColor = _foreColor;
        private States _state = States.ToggledOff;
        private Image? _currentImage;
        private static Image? _baseImage;
        private static Image? _hoverImage;
        private static Image? _altImage;
        private static Image? _altHoverImage;
        private string? _currentText;
        private string? _text;
        private ButtonTypes _buttonType = ButtonTypes.Normal;
        private static bool _isThreeState;
        private int _outlineWidth;
        private Color _outlineColor;
        private bool _useAlt;

        private static Color ColorWhenClicked
        {
            get
            {
                var origColor = _foreColor.ToArgb();
                return Color.FromArgb(origColor ^ 0xFFFFFF);
            }
        }

        #endregion

        #region Designer Properties (Appearance)

        [Description("The font used to display text in the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font
        {
            get => _font;
            set
            {
                _font = base.Font = value;
                FontChanged?.Invoke(this, value);
            }
        }

        [Description("The color used for the display text of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color ForeColor
        {
            get => _foreColor;
            set
            {
                _foreColor = base.ForeColor = value;
                ForeColorChanged?.Invoke(this, value);
            }
        }

        [Description("The images to be used by the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public BaseImages? Images { get; set; }

        [Description("The alternate images to be used by the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public AltImages? ImagesAlt { get; set; }

        [Description("The text to display on the control when not in toggle mode.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string? Text
        {
            get => _text ??= base.Text;
            set
            {
                _text = value;
                TextChanged?.Invoke(this, value);
            }
        }

        [Description("Defines the outline to be used for the displayed text on the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public Outline TextOutline { get; set; }

        [Description("Indicates the state of the control when in toggle mode.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public States ToggleState
        {
            get => _state;
            set
            {
                _state = value;
                StateChanged?.Invoke(value);
            }
        }

        [Description("The text to be used by the control when in toggle mode.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public StateText ToggleText { get; set; }

        [Description("Indicates whether the control should use alternate images.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseAlt
        {
            get => _useAlt;
            set
            {
                _useAlt = value;
                UseAltChanged?.Invoke(this, value);
            }
        }

        #endregion

        #region Designer Properties (Behavior)

        [Description("Determines the behavior of the control.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(ButtonTypes.Normal)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ButtonTypes ButtonType
        {
            get => _buttonType;
            set
            {
                _buttonType = value;
                ButtonTypeChanged?.Invoke(this, value);
            }
        }

        [Description("Indicates whether the ImageButton allows three states instead of only two when in toggle mode.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ThreeState
        {
            get => _isThreeState;
            set
            {
                _isThreeState = value;
                ThreeStateChanged?.Invoke(this, value);
            }
        }

        #endregion

        #region Classes

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class BaseImages
        {
            [Description("The base image to be used by the control.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Background
            {
                get => _baseImage;
                set
                {
                    _baseImage = value;
                    ImageChanged?.Invoke(this, value);
                }
            }

            [Description("The image to be used when hovering over the control.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Hover
            {
                get => _hoverImage;
                set => _hoverImage = value;
            }

            public override string ToString()
            {
                return @"Base ImagesCollection";
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class AltImages
        {
            [Description("The alternate base image used when the 'UseAlt' flag is true.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Background
            {
                get => _altImage;
                set => _altImage = value;
            }

            [Description("The alternate hover image used when the 'UseAlt' flag is true.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Hover
            {
                get => _altHoverImage;
                set => _altHoverImage = value;
            }

            public override string ToString()
            {
                return @"Alternate ImagesCollection";
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class StateText
        {
            [Description("Text to set for the '1st' state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string ToggledOff { get; set; } = "ToggledOff State";

            [Description("Text to set for the '2nd' state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string ToggledOn { get; set; } = "ToggledOn State";

            [Description("Text to set for the '3rd' state.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public string Indeterminate { get; set; } = "Indeterminate State";

            public override string ToString()
            {
                var output = _isThreeState switch
                {
                    false => $"{ToggledOff}, {ToggledOn}",
                    true => $"{ToggledOff}, {ToggledOn}, {Indeterminate}"
                };

                return output;
            }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class Outline
        {
            private int _width = 2;
            private Color _color = Color.Black;

            [Description("The color to be used for the outline.")]
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

            [Description("The width of the outline to be used.")]
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
                return $"{Color}, {Width}";
            }
        }

        #endregion

        public ImageButtonEx()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint |
                ControlStyles.ResizeRedraw, true);
            ButtonTypeChanged += OnButtonTypeChanged;
            FontChanged += OnFontChanged;
            ForeColorChanged += OnForeColorChanged;
            ImageChanged += OnImageChanged;
            MouseDown += OnMouseDown;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseUp += OnMouseUp;
            OutlineColorChanged += OnOutlineColorChanged;
            OutlineWidthChanged += OnOutlineWidthChanged;
            StateChanged += OnStateChanged;
            TextChanged += OnTextChanged;
            ThreeStateChanged += OnThreeStateChanged;
            UseAltChanged += OnUseAltChanged;
            InitializeComponent();
            Images = new BaseImages();
            ImagesAlt = new AltImages();
            TextOutline = new Outline();
            ToggleText = new StateText();
        }

        private void OnUseAltChanged(object? sender, bool e)
        {
            if (Images == null) return;
            var usedImage = e switch
            {
                true => _altImage,
                false => _baseImage
            };
            ImageChanged?.Invoke(this, usedImage);
            Refresh();
        }

        private void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (ButtonType == ButtonTypes.Normal)
            {
                ForeColorChanged?.Invoke(this, _foreColor);
            }

            Click?.Invoke(this, e);
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            switch (ButtonType)
            {
                case ButtonTypes.Normal:
                    ForeColorChanged?.Invoke(this, ColorWhenClicked);
                    break;
                case ButtonTypes.Toggle:
                    if (!_isThreeState)
                    {
                        StateChanged?.Invoke(_state == States.ToggledOff ? States.ToggledOn : States.ToggledOff);
                    }
                    else
                    {
                        switch (ToggleState)
                        {
                            case States.ToggledOff:
                                StateChanged?.Invoke(States.ToggledOn);
                                break;
                            case States.ToggledOn:
                                StateChanged?.Invoke(States.Indeterminate);
                                break;
                            case States.Indeterminate:
                                StateChanged?.Invoke(States.ToggledOff);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            Image? usedImage;
            switch (UseAlt)
            {
                case true:
                    if (_altImage == null || _currentImage == _altImage || _currentImage != _altHoverImage) return;
                    usedImage = _altImage;
                    break;
                case false:
                    if (_baseImage == null || _currentImage == _baseImage || _currentImage != _hoverImage) return;
                    usedImage = _baseImage;
                    break;
            }

            ImageChanged?.Invoke(this, usedImage);
            Refresh();
        }

        private void OnMouseEnter(object? sender, EventArgs e)
        {
            Image? usedImage;
            switch (UseAlt)
            {
                case true:
                    if (_altHoverImage == null) return;
                    usedImage = _altHoverImage;
                    break;
                case false:
                    if (_hoverImage == null) return;
                    usedImage = _hoverImage;
                    break;
            }

            ImageChanged?.Invoke(this, usedImage);
            Refresh();
        }

        private void OnButtonTypeChanged(object? sender, ButtonTypes e)
        {
            _buttonType = e;
            switch (e)
            {
                case ButtonTypes.Normal:
                    TextChanged?.Invoke(this, _text);
                    break;
                case ButtonTypes.Toggle:
                    StateChanged?.Invoke(ToggleState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(e), e, null);
            }

            Refresh();
        }

        private void OnForeColorChanged(object? sender, Color e)
        {
            _currentTextColor = e;
            Refresh();
        }

        private void OnFontChanged(object? sender, Font e)
        {
            _font = e;
            Refresh();
        }

        private static void OnThreeStateChanged(object? sender, bool e)
        {
            _isThreeState = e;
        }

        private void OnOutlineColorChanged(object? sender, Color e)
        {
            _outlineColor = e;
            Refresh();
        }

        private void OnOutlineWidthChanged(object? sender, int e)
        {
            _outlineWidth = e;
            Refresh();
        }

        private void OnImageChanged(object? sender, Image? e)
        {
            if (e != null) _currentImage = e;
            Refresh();
        }

        private void OnTextChanged(object? sender, string? e)
        {
            if (e != null) _currentText = e;
            Refresh();
        }

        private void OnStateChanged(States state)
        {
            if (ButtonType != ButtonTypes.Toggle) return;
            switch (state)
            {
                case States.ToggledOff:
                    TextChanged?.Invoke(this, ToggleText.ToggledOff);
                    break;
                case States.ToggledOn:
                    TextChanged?.Invoke(this, ToggleText.ToggledOn);
                    break;
                case States.Indeterminate:
                    if (_isThreeState) TextChanged?.Invoke(this, ToggleText.Indeterminate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            _state = state;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Set graphics quality
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // Clear graphics with transparency
            e.Graphics.Clear(Color.Transparent);

            // Assign variable to be used in drawing operations (some may be wrapped in using statements)
            var rect = ClientRectangle with { X = 0, Y = 0 };
            var rectPen = new Pen(Color.Cyan, 1f);
            var outlinePen = new Pen(_outlineColor, _outlineWidth) { LineJoin = LineJoin.Round };
            var brush = new SolidBrush(_currentTextColor);
            var sFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.None
            };
            var gfxPath = new GraphicsPath();

            // Perform drawing operations dependent on if currentImage is null for Designer support
            if (_currentImage == null)
            {
                e.Graphics.DrawRectangle(rectPen, rect);
                gfxPath.AddString($"{_currentText}", Font.FontFamily, (int)Font.Style, Font.Size, ClientRectangle,
                    sFormat);
                outlinePen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(outlinePen, gfxPath);
                e.Graphics.FillPath(brush, gfxPath);
            }
            else
            {
                e.Graphics.DrawImage(_currentImage, ClientRectangle);
                gfxPath.AddString($"{_currentText}", Font.FontFamily, (int)Font.Style, Font.Size, ClientRectangle,
                    sFormat);
                outlinePen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(outlinePen, gfxPath);
                e.Graphics.FillPath(brush, gfxPath);
            }
        }
    }
}