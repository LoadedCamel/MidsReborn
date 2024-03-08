using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using FontStyle = System.Drawing.FontStyle;

namespace MRBUpdater.Controls
{
    public partial class ProgressBarEx : ProgressBar, INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region HiddenProps

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color BackColor { get; set; } = Color.Transparent;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Color ForeColor { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int MarqueeAnimationSpeed { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new ProgressBarStyle Style { get; set; }

        #endregion

        #region Enums

        public enum Positions
        {
            Left,
            Right,
            Center
        }

        #endregion

        #region PrivateProps

        private ProgressBorder _border = new();
        private ProgressColors _colors = new();
        private Font _font = new(@"Microsoft Sans Serif", 9.75f, FontStyle.Bold);
        private Positions _position = Positions.Center;
        private bool _showValue = true;

        [Description("The value text of the control")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        private string ValueText
        {
            get
            {
                var value = ShowValue switch
                {
                    true => $"{Value}",
                    false => "",
                };

                return $"{value}%";
            }
        }

        #endregion

        #region PublicProps

        // Appearance group
        [Description("Sets the border for the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProgressBorder Border
        {
            get => _border;
            set => SetField(ref _border, value);
        }

        [Description("Defines the colors for the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProgressColors Colors
        {
            get => _colors;
            set => SetField(ref _colors, value);
        }

        [Description("The font used to display the value on the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font
        {
            get => _font;
            set => SetField(ref _font, value);
        }

        // Behavior group
        [Description("Defines the position of the controls value text.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(Positions.Center)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Positions Position
        {
            get => _position;
            set => SetField(ref _position, value);
        }

        [Description("Determines if the value is displayed on the control.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowValue
        {
            get => _showValue; 
            set => SetField(ref _showValue, value);
        }

        #endregion

        #region Constructor

        public ProgressBarEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            PropertyChanged += OnPropertyChanged;
        }

        #endregion
        
        #region Methods

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Set high-quality rendering modes for smooth output.
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Determine if all borders should be drawn, to optimize border color and thickness setup.
            var drawAllBorders = Border.Which == ProgressBorder.BorderToDraw.All ||
                                 Border.Which.HasFlag(ProgressBorder.BorderToDraw.All);
            var effectiveBorderColor = drawAllBorders ? Colors.BorderColor : Color.Transparent;
            var effectiveBorderThickness = drawAllBorders ? Border.Thickness : 0;
            var effectiveBorderStyle = drawAllBorders ? Border.Style : ButtonBorderStyle.None;

            // Draw border considering individual sides if not drawing all.
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                Border.Which.HasFlag(ProgressBorder.BorderToDraw.Left) ? effectiveBorderColor : Color.Transparent,
                effectiveBorderThickness, effectiveBorderStyle,
                Border.Which.HasFlag(ProgressBorder.BorderToDraw.Top) ? effectiveBorderColor : Color.Transparent,
                effectiveBorderThickness, effectiveBorderStyle,
                Border.Which.HasFlag(ProgressBorder.BorderToDraw.Right) ? effectiveBorderColor : Color.Transparent,
                effectiveBorderThickness, effectiveBorderStyle,
                Border.Which.HasFlag(ProgressBorder.BorderToDraw.Bottom) ? effectiveBorderColor : Color.Transparent,
                effectiveBorderThickness, effectiveBorderStyle);

            // Adjust the progress rectangle to respect border thickness.
            var progressRect = new Rectangle(ClientRectangle.X + effectiveBorderThickness,
                ClientRectangle.Y + effectiveBorderThickness,
                ClientRectangle.Width - 2 * effectiveBorderThickness,
                ClientRectangle.Height - 2 * effectiveBorderThickness);

            // Draw the progress bar within the adjusted rectangle.
            ProgressBarRenderer.DrawHorizontalBar(e.Graphics, progressRect);
            e.Graphics.FillRectangle(new SolidBrush(Colors.BackColor), progressRect);

            // Draw filled part of the progress bar based on the Value.
            if (Value > 0)
            {
                var fillWidth = (int)Math.Round((double)Value / Maximum * progressRect.Width);
                var filledRect = progressRect with { Width = fillWidth };
                using var barGradient = new LinearGradientBrush(filledRect, Colors.BarStartColor, Colors.BarEndColor,
                    LinearGradientMode.ForwardDiagonal);
                e.Graphics.FillRectangle(barGradient, filledRect);
            }

            // Conditionally draw the text.
            if (ShowValue)
            {
                TextRenderer.DrawText(e.Graphics, ValueText, Font, progressRect, Colors.TextColor,
                    GetFormatFlags(Position));
            }
        }

        private TextFormatFlags GetFormatFlags(Positions position)
        {
            return position switch
            {
                Positions.Left => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                Positions.Right => TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                Positions.Center => TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                _ => throw new ArgumentOutOfRangeException(nameof(Position), $@"Invalid position value: {Position}.")
            };
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        #endregion

        #region TypeClasses

        [TypeConverter(typeof(GenericTypeConverter<ProgressColors>))]
        public sealed class ProgressColors : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            [Description("Sets the containers background color.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color BackColor { get; set; } = ColorTranslator.FromHtml("#2c2f33");

            [Description("Sets the color of the slider.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color BarStartColor { get; set; } = ColorTranslator.FromHtml("#1e90ff");

            [Description("Sets the color of the slider.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color BarEndColor { get; set; } = ColorTranslator.FromHtml("#404eed");

            [Description("The border color of the component.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color BorderColor { get; set; } = ColorTranslator.FromHtml("#2c2f33");

            [Description("Sets the color of the progress text.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public Color TextColor { get; set; } = Color.WhiteSmoke;

            public override string ToString()
            {
                return $"back={BackColor.Name}, start={BarStartColor.Name}, end={BarEndColor.Name}, text={TextColor.Name}";
            }
        }

        [TypeConverter(typeof(GenericTypeConverter<ProgressBorder>))]
        public sealed class ProgressBorder : INotifyPropertyChanged
        {
            [Flags]
            public enum BorderToDraw
            {
                None = 0,
                Left = 1 << 0, // 1
                Top = 1 << 1, // 2
                Right = 1 << 2, // 4
                Bottom = 1 << 3, // 8
                All = Left | Top | Right | Bottom // 15
            }

            [Description("Determines which of the components borders to draw.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public BorderToDraw Which { get; set; } = BorderToDraw.All;

            [Description("Determines the thickness of the components border.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public int Thickness { get; set; } = 1;

            [Description("Defines the style of the components border.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
            public ButtonBorderStyle Style { get; set; } = ButtonBorderStyle.Solid;

            public override string ToString()
            {
                return $"{Which}, {Thickness}pt, style={Style}";
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        #endregion

        #region TypeConverter

        public class GenericTypeConverter<T> : TypeConverter
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
    }
}
