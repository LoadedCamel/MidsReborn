using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UIv2.Controls
{
    public partial class MultiOrientButton : Button
    {

        #region Events

        private new event EventHandler<Font>? FontChanged;
        private new event EventHandler<Color>? ForeColorChanged;
        private event EventHandler<bool>? UseVerticalTextChanged;
        private new event EventHandler<string?>? TextChanged;

        #endregion

        #region PrivateProps

        private Font _font = new("Segoe UI", DefaultFont.Size, FontStyle.Bold, GraphicsUnit.Point);
        private Color _foreColor = Color.WhiteSmoke;
        private string? _text;
        private bool _useVerticalText;

        #endregion

        #region DesignerProps

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

        [Description("The text to display on the control.")]
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

        [Description("Determines if the text should be displayed vertically on the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool UseVerticalText
        {
            get => _useVerticalText;
            set
            {
                _useVerticalText = value;
                UseVerticalTextChanged?.Invoke(this, value);
            }
        }

        #endregion


        public MultiOrientButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            FontChanged += MultiOrientButton_FontChanged;
            ForeColorChanged += MultiOrientButton_ForeColorChanged;
            TextChanged += MultiOrientButton_TextChanged;
            UseVerticalTextChanged += MultiOrientButton_UseVerticalTextChanged;
            InitializeComponent();
        }

        private void MultiOrientButton_UseVerticalTextChanged(object? sender, bool e)
        {
            _useVerticalText = e;
            Invalidate();
        }

        private void MultiOrientButton_TextChanged(object? sender, string? e)
        {
            if (!string.IsNullOrWhiteSpace(e)) _text = e;
            Invalidate();
        }

        private void MultiOrientButton_ForeColorChanged(object? sender, Color e)
        {
            _foreColor = e;
            Invalidate();
        }

        private void MultiOrientButton_FontChanged(object? sender, Font e)
        {
            _font = e;
            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            //base.OnPaint(pevent);

            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            var rect = ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            // Darkened Gradient Background
            var startColor = Color.FromArgb(20, 75, 160); // Darker shade of DodgerBlue
            var endColor = Color.FromArgb(10, 55, 130); // Even darker shade
            using (var brush = new LinearGradientBrush(rect, startColor, endColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(brush, rect);
            }

            // Draw border
            using (var pen = new Pen(Color.FromArgb(255, 255, 255), 1))
            {
                g.DrawRectangle(pen, rect);
            }

            // Focus rectangle handling remains the same as before or can be adjusted similarly.

            // Drawing text
            if (UseVerticalText)
            {
                DrawVerticalText(pevent);
            }
            else
            {
                DrawHorizontalText(pevent);
            }
        }

        private void DrawVerticalText(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            if (Text == null) return;
            var text = Text.Trim();
            var verticalPosition = (ClientRectangle.Height - CalculateTotalTextHeight(text, g, Font)) / 2; // Centering vertically
            var horizontalCenter = ClientRectangle.Width / 2; // Centering horizontally

            foreach (var c in text)
            {
                var charStr = c.ToString();
                var charSize = TextRenderer.MeasureText(g, charStr, Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding);
                var charPosition = new Point(horizontalCenter - (charSize.Width / 2), verticalPosition);

                // Drawing each character vertically
                TextRenderer.DrawText(g, charStr, Font, charPosition, ForeColor, TextFormatFlags.NoPadding);

                verticalPosition += charSize.Height;
            }
        }

        private int CalculateTotalTextHeight(string text, Graphics g, Font font)
        {
            return text.Select(c => TextRenderer.MeasureText(g, c.ToString(), font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.NoPadding))
                .Sum(size => size.Height);
        }

        private void DrawHorizontalText(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            var text = Text;
            var textSize = TextRenderer.MeasureText(g, text, Font, new Size(ClientRectangle.Width, ClientRectangle.Height), TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            var textPosition = new Point((ClientRectangle.Width - textSize.Width) / 2, (ClientRectangle.Height - textSize.Height) / 2);

            // Drawing the text horizontally and centered
            TextRenderer.DrawText(g, text, Font, textPosition, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        }
    }
}
