using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;
using FontStyle = System.Drawing.FontStyle;

namespace Mids_Reborn.Forms.Controls
{
    public sealed partial class ProgressBarEx : ProgressBar, INotifyPropertyChanged
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
        public ProgressBorder Border { get; set; } = new();

        [Description("Defines the colors for the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ProgressColors Colors { get; set; } = new();

        [Description("The font used to display the value on the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font { get; set; } = new(@"Microsoft Sans Serif", 9.75f, FontStyle.Bold);

        // Behavior group
        [Description("Defines the position of the controls value text.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(Positions.Center)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Positions Position { get; set; } = Positions.Center;

        [Description("Determines if the value is displayed on the control.")]
        [Category("Behavior")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DefaultValue(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowValue { get; set; } = true;

        #endregion

        public ProgressBarEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            switch (Border.Which)
            {
                case ProgressBorder.BorderToDraw.All:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Colors.BorderColor, Border.Thickness, Border.Style,
                        Colors.BorderColor, Border.Thickness, Border.Style,
                        Colors.BorderColor, Border.Thickness, Border.Style,
                        Colors.BorderColor, Border.Thickness, Border.Style);
                    break;
                case ProgressBorder.BorderToDraw.Left:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Colors.BorderColor, Border.Thickness, Border.Style,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case ProgressBorder.BorderToDraw.Top:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Colors.BorderColor, Border.Thickness, Border.Style,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case ProgressBorder.BorderToDraw.Right:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Colors.BorderColor, Border.Thickness, Border.Style,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case ProgressBorder.BorderToDraw.Bottom:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Colors.BorderColor, Border.Thickness, Border.Style);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var progressRect = new Rectangle(ClientRectangle.X + Border.Thickness, ClientRectangle.Y + Border.Thickness, ClientRectangle.Width - Border.Thickness*2, ClientRectangle.Height - Border.Thickness*2);
            ProgressBarRenderer.DrawHorizontalBar(e.Graphics, progressRect);
            e.Graphics.FillRectangle(new SolidBrush(Colors.BackColor), progressRect);
            if (Value <= 0) return;
            var bar = progressRect with { Width = (int)Math.Round((float)Value / Maximum * progressRect.Width) };
            var barGradient = new LinearGradientBrush(bar, Colors.BarStartColor, Colors.BarEndColor, LinearGradientMode.ForwardDiagonal);
            ProgressBarRenderer.DrawHorizontalBar(e.Graphics, bar);
            e.Graphics.FillRectangle(barGradient, bar);
            var formatFlags = Position switch
            {
                Positions.Left => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                Positions.Right => TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
                Positions.Center => TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (!ShowValue) return;
            TextRenderer.DrawText(e.Graphics, ValueText, Font, progressRect, Colors.TextColor, formatFlags);
        }

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

        #region TypeClasses

        [TypeConverter(typeof(MrbTypeConverter<ProgressColors>))]
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
                return $"{BackColor}, {BarStartColor}, {BarEndColor}, {TextColor}";
            }
        }

        [TypeConverter(typeof(MrbTypeConverter<ProgressBorder>))]
        public sealed class ProgressBorder : INotifyPropertyChanged
        {
            public enum BorderToDraw
            {
                All,
                Left,
                Top,
                Right,
                Bottom
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
                return $"{Which}, {Thickness}, {Style}";
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        #endregion
    }
}
