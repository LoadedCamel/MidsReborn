using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Controls.Converters;

namespace Mids_Reborn.Forms.Controls
{
    public partial class AdvGroupBox : GroupBox
    {
        private Color _titleColor = Color.WhiteSmoke;
        private Font _titleFont = DefaultFont;
        private Color _borderColor = Color.Black;

        [Description("The border color of the component.")]
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
                Invalidate();
            }
        }

        [Description("The title color of the component.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color TitleColor
        {
            get => _titleColor;
            set
            {
                _titleColor = value;
                Invalidate();
            }
        }

        [Description("The font to be used with the title of the component.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font TitleFont
        {
            get => _titleFont;
            set
            {
                _titleFont = value;
                Invalidate();
            }
        }

        [Description("The properties that determine if the component uses rounded corners or not.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public AdvGroupBoxCorners RoundedCorners { get; set; }

        public AdvGroupBox()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            RoundedCorners = new AdvGroupBoxCorners();
            RoundedCorners.PropertyChanged += (_, _) => Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Text formatting and drawing
            var textSize = TextRenderer.MeasureText(Text, TitleFont);
            var textRect = new Rectangle(8, 0, textSize.Width, textSize.Height);

            // Prepare the bounds to account for text
            var bounds = new Rectangle(0, textRect.Height / 2, Width - 1, Height - textRect.Height / 2 - 1);
            using var pen = new Pen(BorderColor);
            if (!RoundedCorners.Enabled)
            {
                // Draw left and right sides of the top border, leaving space for the text
                e.Graphics.DrawLine(pen, bounds.X, bounds.Y, textRect.Left - 2, bounds.Y);
                e.Graphics.DrawLine(pen, textRect.Right - 2, bounds.Y, bounds.Right, bounds.Y);
                e.Graphics.DrawLine(pen, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                e.Graphics.DrawLine(pen, bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
                e.Graphics.DrawLine(pen, bounds.Left, bounds.Bottom, bounds.Right, bounds.Bottom);
                e.Graphics.DrawString(Text, TitleFont, new SolidBrush(TitleColor), textRect, new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                });
            }
            else
            {
                using var path = new GraphicsPath();
                path.AddArc(bounds.X, bounds.Y, RoundedCorners.Radius, RoundedCorners.Radius, 180, 90);
                path.AddArc(bounds.X + bounds.Width - RoundedCorners.Radius, bounds.Y, RoundedCorners.Radius, RoundedCorners.Radius, 270, 90);
                path.AddArc(bounds.X + bounds.Width - RoundedCorners.Radius, bounds.Y + bounds.Height - RoundedCorners.Radius, RoundedCorners.Radius, RoundedCorners.Radius, 0, 90);
                path.AddArc(bounds.X, bounds.Y + bounds.Height - RoundedCorners.Radius, RoundedCorners.Radius, RoundedCorners.Radius, 90, 90);
                path.CloseFigure();

                var exclusionRect = textRect with { X = textRect.X + 2, Width = textRect.Width };
                var exclusionRegion = new Region(exclusionRect);
                e.Graphics.ExcludeClip(exclusionRegion);
                e.Graphics.DrawPath(pen, path);


                e.Graphics.SetClip(exclusionRect);
                e.Graphics.DrawString(Text, TitleFont, new SolidBrush(TitleColor), exclusionRect, new StringFormat
                {
                    Alignment = StringAlignment.Near,
                    LineAlignment = StringAlignment.Center
                });
                e.Graphics.ResetClip();
                e.Graphics.SetClip(path, CombineMode.Replace);
            }
        }

        [TypeConverter(typeof(GenericTypeConverter<AdvGroupBoxCorners>))]
        public sealed class AdvGroupBoxCorners : INotifyPropertyChanged
        {
            private int _radius = 10;
            private bool _enabled;

            [Description("Determines if the components uses rounded corners.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public bool Enabled
            {
                get => _enabled;
                set => SetField(ref _enabled, value);
            }

            [Description("Sets the radius for the components corners if enabled.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public int Radius
            {
                get => _radius;
                set => SetField(ref _radius, value);
            }

            public override string ToString()
            {
                var enabledText = Enabled ? "Yes" : "No";
                return $"Enabled: {enabledText}, Corner Radius: {Radius} px";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

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
        }
    }
}
