using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.UIv2.Controls
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

        private Item _popupItem = new();

        public Popup()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BorderColorChanged += OnBorderColorChanged;
            VisibilityChanged += OnVisibilityChanged;
        }

        public Popup(Item popupItem)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
            BorderColorChanged += OnBorderColorChanged;
            VisibilityChanged += OnVisibilityChanged;
            _popupItem = popupItem;
        }

        public void Show(Item item, Point location)
        {
            _popupItem = item;
            Location = location;
            IsOpen = true;
        }

        public new void Hide()
        {
            Reset();
        }

        private void Reset()
        {
            _popupItem = new Item();
            IsOpen = false;
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
            if (!_isOpen) return;
            var gfx = e.Graphics;
            gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            float currentY = 5; // Start drawing from this Y position
            float maxWidth = 0; // Keep track of the maximum width

            // Draw the image if it's not null
            DrawImageIfNotNull(_popupItem.Image, ref currentY, ref maxWidth);

            var textOffset = maxWidth > 0 ? maxWidth + 5 : 0; // Calculate text offset based on image width

            // Draw each property if it's not empty
            DrawTextIfNotEmpty(_popupItem.Title, new Font(Font, FontStyle.Underline | FontStyle.Bold), _type1Colors["Title"],
                ref currentY, textOffset);
            DrawTextIfNotEmpty(_popupItem.Powerset, Font, _type1Colors["Powerset"], ref currentY,
                0); // xOffset is 0 for subsequent texts
            DrawTextIfNotEmpty(_popupItem.Available > 0 ? $"Available: Level {_popupItem.Available}" : null, Font, _type1Colors["Available"],
                ref currentY, 0);
            DrawTextIfNotEmpty(_popupItem.Data, Font, _type1Colors["Data"], ref currentY, 0);

            // Adjust control width and height based on content
            Width = (int)maxWidth + 30;
            Height = (int)currentY;
            return;

            // Local function to draw image if it's not null
            void DrawImageIfNotNull(Image? image, ref float y, ref float maxWidth)
            {
                if (image == null) return;
                var imageSize = new Size(image.Width, image.Height); // You might want to scale the image
                gfx.DrawImage(image, new Rectangle(5, (int)y, imageSize.Width, imageSize.Height));
                maxWidth = Math.Max(maxWidth, imageSize.Width); // Update maxWidth if needed
            }

            // Local function to draw text if it's not null or empty
            void DrawTextIfNotEmpty(string? text, Font font, Color color, ref float y, float xOffset)
            {
                if (string.IsNullOrEmpty(text)) return;
                var textSize = TextRenderer.MeasureText(gfx, text, font);
                var textRec =
                    new Rectangle((int)(5 + xOffset), (int)y, textSize.Width,
                        textSize.Height); // Adjust X position based on xOffset
                TextRenderer.DrawText(gfx, text, font, textRec, color);
                y += textSize.Height + 5; // Increment Y position for next text
                maxWidth = Math.Max(maxWidth, textSize.Width + xOffset); // Update maxWidth if needed
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

        protected override void WndProc(ref Message m)
        {
            const int wmMousemove = 0x0200;
            const int wmLbuttondown = 0x0201;
            const int wmLbuttonup = 0x0202;

            // Ignore mouse messages
            if (m.Msg is wmMousemove or wmLbuttondown or wmLbuttonup)
            {
                return;
            }

            base.WndProc(ref m);
        }

        public class Item
        {
            public string? Title { get; set; }
            public Image? Image { get; set; }
            public string? Powerset { get; set; }
            public int Available { get; set; } = -1;
            public string? Data { get; set; }

            public Item()
            {
                Title = string.Empty;
                Image = null;
                Powerset = string.Empty;
                Available = -1;
                Data = string.Empty;
            }

            public Item(string? title, Image? image, string? powerset, int available, string? data)
            {
                Title = title;
                Image = image;
                Powerset = powerset;
                Available = available;
                Data = data;
            }

            public Item(string? title, string? powerset, int available, string? data)
            {
                Title = title;
                Powerset = powerset;
                Available = available;
                Data = data;
            }

            public Item(string? title, string? data)
            {
                Title = title;
                Data = data;
            }
        }
    }
}
