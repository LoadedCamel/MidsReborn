using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
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
        [Browsable(false)] public override bool AutoSize { get; set; }
        [Browsable(false)] public override bool AutoScroll { get; set; }
        [Browsable(false)] public override Point AutoScrollOffset { get; set; }
        [Browsable(false)] public override Size MaximumSize { get; set; }
        [Browsable(false)] public override Size MinimumSize { get; set; }

        #endregion

        #region Events

        private new event EventHandler<Font>? FontChanged;
        private new event EventHandler<Color>? ForeColorChanged;
        private event EventHandler<Image?>? ImageChanged;
        private new event EventHandler<string?>? TextChanged;
        private event EventHandler<bool> ThreeStateChanged;
        private event EventHandler<ButtonTypes>? ButtonTypeChanged;
        private event EventHandler<bool>? UseAltChanged;
        private event EventHandler<MouseButtons>? ToggleMouseButtonChanged;

        private delegate void StateChangedEventHandler(object? sender, States state);

        private event StateChangedEventHandler? StateChanged;

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

        public enum MouseClicks
        {
            LeftButton,
            RightButton
        }

        #endregion

        #region Private Properties

        private Font _font = new("MS Sans Serif", DefaultFont.Size, FontStyle.Regular, GraphicsUnit.Point);
        private Color _foreColor = Color.White;

        private Color _currentTextColor;
        private States _state = States.ToggledOff;
        private Image? _currentImage;
        private string? _text;
        private ButtonTypes _buttonType = ButtonTypes.Normal;
        private bool _isThreeState;
        private bool _useAlt;
        private bool _setByToggle;
        private MouseButtons _toggleMouseButton = MouseButtons.Left;
        private MouseClicks _toggleActivation = MouseClicks.LeftButton;

        private Color ColorWhenClicked
        {
            get
            {
                var origColor = ForeColor.ToArgb();
                return _useAlt ? Color.FromArgb(origColor ^ 0x161CCA) : Color.FromArgb(origColor ^ 0xCA1616);
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The current text of the component. Sets from Text and ToggleText but can be overridden by assigning to it directly.
        /// </summary>
        public string? CurrentText { get; set; }

        /// <summary>
        /// Turns locking mechanism on/off for the component, preventing it from switching states.
        /// </summary>
        public bool Lock { get; set; }


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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BaseImages? Images { get; set; }

        [Description("The alternate images to be used by the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public Outline TextOutline { get; set; }

        [Description("Indicates the state of the control when in toggle mode.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public States ToggleState
        {
            get => _state;
            set
            {
                _state = value;
                StateChanged?.Invoke(this, value);
            }
        }

        [Description("The text to be used by the control when in toggle mode.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
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

        [Description("Indicates which mouse button activates the Toggle mechanism.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(MouseClicks.LeftButton)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public MouseClicks ToggleActivation
        {
            get => _toggleActivation;
            set
            {
                _toggleActivation = value;
                var mouseButton = MouseButtons.None;
                mouseButton = _toggleActivation switch
                {
                    MouseClicks.LeftButton => MouseButtons.Left,
                    MouseClicks.RightButton => MouseButtons.Right,
                    _ => mouseButton
                };
                ToggleMouseButtonChanged?.Invoke(this, mouseButton);
            }
        }


        #endregion

        #region TypeConverter<T>

        public class ImageButtonExTypeConverter<T> : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            {
                return TypeDescriptor.GetProperties(typeof(T));
            }
        }

        #endregion

        #region Classes

        [TypeConverter(typeof(ImageButtonExTypeConverter<BaseImages>))]
        public sealed class BaseImages : INotifyPropertyChanged
        {
            [Description("The base image to be used by the control.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Background { get; set; }

            [Description("The image to be used when hovering over the control.\r\nNote: This image will also be used in non-ThreeState Toggle mode.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Hover { get; set; }

            public override string ToString()
            {
                return "Base Images";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        [TypeConverter(typeof(ImageButtonExTypeConverter<AltImages>))]
        public sealed class AltImages : INotifyPropertyChanged
        {
            [Description("The alternate base image used when the 'UseAlt' flag is true.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Background { get; set; }

            [Description("The alternate hover image used when the 'UseAlt' flag is true.\r\nNote: This image will also be used in non-ThreeState Toggle mode.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Image? Hover { get; set; }

            public override string ToString()
            {
                return @"Alternate Images";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        [TypeConverter(typeof(ImageButtonExTypeConverter<StateText>))]
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
                return @"Toggle State Texts";
            }
        }

        [TypeConverter(typeof(ImageButtonExTypeConverter<Outline>))]
        public sealed class Outline : INotifyPropertyChanged
        {
            [Description("The color to be used for the outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Color Color { get; set; } = Color.Black;

            [Description("The width of the outline to be used.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public int Width { get; set; } = 2;

            public override string ToString()
            {
                return $"{Color}, {Width}";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

            private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value)) return false;
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        #endregion

        public ImageButtonEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            ButtonTypeChanged += OnButtonTypeChanged;
            FontChanged += OnFontChanged;
            ForeColorChanged += OnForeColorChanged;
            ImageChanged += OnImageChanged;
            MouseDown += OnMouseDown;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseUp += OnMouseUp;
            StateChanged += OnStateChanged;
            TextChanged += OnTextChanged;
            ThreeStateChanged += OnThreeStateChanged;
            UseAltChanged += OnUseAltChanged;
            ToggleMouseButtonChanged += OnToggleMouseButtonChanged;
            if (_currentTextColor == Color.Empty) _currentTextColor = _foreColor;
            InitializeComponent();
            Images = new BaseImages();
            Images.PropertyChanged += ImagesOnPropertyChanged;
            ImagesAlt = new AltImages();
            TextOutline = new Outline();
            TextOutline.PropertyChanged += TextOutlineOnPropertyChanged;
            ToggleText = new StateText();
        }

        private void OnToggleMouseButtonChanged(object? sender, MouseButtons e)
        {
            _toggleMouseButton = e;
        }

        #region Event Methods

        private void TextOutlineOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Refresh();
        }

        private void ImagesOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Background") ImageChanged?.Invoke(this, Images?.Background);
        }

        private void OnUseAltChanged(object? sender, bool e)
        {
            if (Images == null) return;
            var usedImage = e switch
            {
                true => ImagesAlt?.Background,
                false => Images?.Background
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
        }

        private void OnMouseDown(object? sender, MouseEventArgs e)
        {
            switch (ButtonType)
            {
                case ButtonTypes.Normal:
                    ForeColorChanged?.Invoke(this, ColorWhenClicked);
                    break;
                case ButtonTypes.Toggle:
                    if (e.Button != _toggleMouseButton) return;
                    if (Lock) return;
                    if (!_isThreeState)
                    {
                        StateChanged?.Invoke(this,
                            _state == States.ToggledOff ? States.ToggledOn : States.ToggledOff);
                    }
                    else
                    {
                        switch (_state)
                        {
                            case States.ToggledOff:
                                StateChanged?.Invoke(this, States.ToggledOn);
                                break;
                            case States.ToggledOn:
                                StateChanged?.Invoke(this, States.Indeterminate);
                                break;
                            case States.Indeterminate:
                                StateChanged?.Invoke(this, States.ToggledOff);
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
            if (_setByToggle) return;
            switch (UseAlt)
            {
                case true:
                    if (ImagesAlt?.Background == null || _currentImage == ImagesAlt?.Background ||
                        _currentImage != ImagesAlt?.Hover) return;
                    usedImage = ImagesAlt?.Background;
                    break;
                case false:
                    if (Images?.Background == null || _currentImage == Images?.Background ||
                        _currentImage != Images?.Hover) return;
                    usedImage = Images?.Background;
                    break;
            }

            ImageChanged?.Invoke(this, usedImage);
            Refresh();
        }

        private void OnMouseEnter(object? sender, EventArgs e)
        {
            if (_setByToggle) return;
            var control = sender as ImageButtonEx;
            if (control?.Name != Name) return;
            Image? usedImage;
            switch (UseAlt)
            {
                case true:
                    if (ImagesAlt?.Hover == null) return;
                    usedImage = ImagesAlt?.Hover;
                    break;
                case false:
                    if (Images?.Hover == null) return;
                    usedImage = Images?.Hover;
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
                    StateChanged?.Invoke(this, ToggleState);
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

        private void OnThreeStateChanged(object? sender, bool e)
        {
            _isThreeState = e;
        }

        private void OnImageChanged(object? sender, Image? e)
        {
            if (e != null) _currentImage = e;
            Refresh();
        }

        private void OnTextChanged(object? sender, string? e)
        {
            if (e != null) CurrentText = e;
            Refresh();
        }

        private void OnStateChanged(object? sender, States state)
        {
            if (ButtonType != ButtonTypes.Toggle || Lock) return;
            switch (state)
            {
                case States.ToggledOff:
                    TextChanged?.Invoke(this, ToggleText.ToggledOff);
                    if (!ThreeState)
                    {
                        switch (UseAlt)
                        {
                            case true:
                                ImageChanged?.Invoke(this, ImagesAlt?.Background);
                                break;
                            case false:
                                ImageChanged?.Invoke(this, Images?.Background);
                                break;
                        }

                        _setByToggle = false;
                    }
                    break;
                case States.ToggledOn:
                    TextChanged?.Invoke(this, ToggleText.ToggledOn);
                    if (!ThreeState)
                    {
                        switch (UseAlt)
                        {
                            case true:
                                ImageChanged?.Invoke(this, ImagesAlt?.Hover);
                                break;
                            case false:
                                ImageChanged?.Invoke(this, Images?.Hover);
                                break;
                        }

                        _setByToggle = true;
                    }
                    break;
                case States.Indeterminate:
                    if (_isThreeState) TextChanged?.Invoke(this, ToggleText.Indeterminate);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }

            _state = state;
        }

        #endregion

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
            var outlinePen = new Pen(TextOutline.Color, TextOutline.Width) { LineJoin = LineJoin.Round };
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
                gfxPath.AddString($"{CurrentText}", Font.FontFamily, (int)Font.Style, Font.Size, ClientRectangle,
                    sFormat);
                outlinePen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(outlinePen, gfxPath);
                e.Graphics.FillPath(brush, gfxPath);
            }
            else
            {
                e.Graphics.DrawImage(_currentImage, ClientRectangle);
                gfxPath.AddString($"{CurrentText}", Font.FontFamily, (int)Font.Style, Font.Size, ClientRectangle,
                    sFormat);
                outlinePen.LineJoin = LineJoin.Round;
                e.Graphics.DrawPath(outlinePen, gfxPath);
                e.Graphics.FillPath(brush, gfxPath);
            }
        }
    }
}