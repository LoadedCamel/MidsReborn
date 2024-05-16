using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using Mids_Reborn.Controls.Extensions;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls
{
    public sealed partial class EnhSetInfo : UserControl
    {
        private string? _title;
        private string? _rarity;
        private string? _setTypeName;
        private string? _levelRange;
        private string? _enhCount;
        private string[]? _enhancements;
        private string[]? _setBonuses;
        private string? _selectedBonus;

        private Bitmap? _textBitmap;
        private readonly VScrollBar _vScrollBar;
        private int _scrollOffset;

        public override Color BackColor { get; set; } = Color.Black;
        private Color TitleColor { get; set; } = Color.WhiteSmoke;
        private Color BonusColor { get; set; } = Color.SpringGreen;

        private readonly Dictionary<string, Color> _rarityColors = new()
        {
            { string.Empty, Color.WhiteSmoke },
            { "Common", Color.White },
            { "Uncommon", Color.Yellow },
            { "Rare", Color.Orange },
            { "UltraRare", Color.FromArgb(192, 96, 192) }
        };

        public EnhSetInfo()
        {
            DoubleBuffered = true;
            InitializeComponent();

            _vScrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
            };
            _vScrollBar.Scroll += VScrollBar_Scroll;
            Controls.Add(_vScrollBar);
            MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            // Check if the scrollbar is visible
            if (!_vScrollBar.Visible) return;
            // Determine the amount to scroll
            var numberOfTextLinesToMove = e.Delta * SystemInformation.MouseWheelScrollLines / 120;

            // Compute the new value of the scrollbar
            var newScrollValue = _vScrollBar.Value - (numberOfTextLinesToMove * 20); // Assuming 20 pixels per line

            // Make sure the new value is within the bounds of the scrollbar min and max values
            newScrollValue = Math.Max(_vScrollBar.Minimum, Math.Min(_vScrollBar.Maximum - _vScrollBar.LargeChange + 1, newScrollValue));

            // Update the scrollbar value
            _vScrollBar.Value = newScrollValue;

            // Trigger the scroll event manually
            VScrollBar_Scroll(_vScrollBar, new ScrollEventArgs(ScrollEventType.ThumbTrack, newScrollValue));
        }

        public void SetInfo(SetData data)
        {
            _textBitmap?.Dispose();
            _title = data.Set;
            _rarity = data.SetRarity;
            _setTypeName = data.SetType;
            _levelRange = data.LevelRange;
            _enhCount = data.EnhCount;
            _enhancements = data.Enhancements.ToArray();
            _setBonuses = data.Bonuses.ToArray();
            _selectedBonus = data.Selected;
            _textBitmap = null;
            Invalidate();
        }

        public void Clear()
        {
            _textBitmap?.Dispose();
            _title = null;
            _rarity = null;
            _setTypeName = null;
            _levelRange = null;
            _enhCount = null;
            _enhancements = null;
            _setBonuses = null;
            _selectedBonus = null;
            _textBitmap = null;
            Invalidate();
        }

        private void VScrollBar_Scroll(object? sender, ScrollEventArgs e)
        {
            _scrollOffset = e.NewValue;
            Invalidate();
        }

        private int CalculateTotalHeight(IDeviceContext g, Font font)
        {
            var yOffset = 6;
            var bitmapWidth = Width - _vScrollBar.Width; // Account for scrollbar width
            if (!string.IsNullOrWhiteSpace(_title))
            {
                var size = TextRenderer.MeasureText(g, _title, font);
                yOffset += size.Height + 10;

                size = TextRenderer.MeasureText(g, $"Set Type: {_setTypeName}", font);
                yOffset += size.Height + 10;

                size = TextRenderer.MeasureText(g, $"Level Range: {_levelRange}", font);
                yOffset += size.Height + 10;

                size = TextRenderer.MeasureText(g, $"Enhancements in Set: {_enhCount}", font);
                yOffset += size.Height + 10;

                if (_enhancements != null)
                {
                    foreach (var enhancement in _enhancements)
                    {
                        size = TextRenderer.MeasureText(g, enhancement, font);
                        yOffset += size.Height + 5;
                    }
                }

                size = TextRenderer.MeasureText(g, "Set Bonuses:", font);
                yOffset += size.Height + 10;

                if (_setBonuses == null) return yOffset;
                foreach (var bonus in _setBonuses)
                {
                    size = TextRenderer.MeasureText(g, bonus, font);
                    if (size.Width > bitmapWidth - 12)
                    {
                        yOffset += size.Height * 2 + 5;
                    }
                    else
                    {
                        yOffset += size.Height + 5;
                    }
                }
            }
            else
            {
                yOffset = Height;
            }

            return yOffset;
        }

        private Bitmap GenerateTextBitmap()
        {
            using var font = new Font("Segoe UI", 12f, FontStyle.Bold);
            using var titleFont = new Font(font.FontFamily, 14.25f, FontStyle.Bold);
            int totalHeight;

            // Set graphics quality and calculate total height
            using (var g = CreateGraphics())
            {
                g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingMode = CompositingMode.SourceOver;
                totalHeight = CalculateTotalHeight(g, font);
            }

            var bitmapWidth = Width - _vScrollBar.Width;

            // Create a Bitmap and Graphics object
            var bitmap = new Bitmap(bitmapWidth, totalHeight, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(BackColor);
                var yOffset = 6;

                if (!string.IsNullOrWhiteSpace(_title))
                {
                    // Draw Title
                    TextRenderer.DrawText(g, _title, titleFont, new Point(6, yOffset), _rarityColors[_rarity ?? string.Empty]);
                    var size = TextRenderer.MeasureText(g, _title, font);
                    yOffset += size.Height + 5;

                    // Draw Set Type
                    TextRenderer.DrawText(g, $"Set Type: {_setTypeName}", font, new Point(6, yOffset), TitleColor);
                    yOffset += size.Height + 1;

                    // Draw Level Range
                    TextRenderer.DrawText(g, $"Level Range: {_levelRange}", font, new Point(6, yOffset), TitleColor);
                    yOffset += size.Height + 1;

                    // Draw Enhancements Subtitle
                    TextRendererExt.DrawTextWithSubColor(g, $"Enhancements in Set: {_enhCount}", _enhCount, font, new Point(6, yOffset), TitleColor, Color.Gold);
                    yOffset += size.Height + 5;

                    // Draw Enhancements
                    if (_enhancements != null)
                    {
                        foreach (var enhancement in _enhancements)
                        {
                            TextRenderer.DrawText(g, $"\u00a4 {enhancement}", font, new Point(20, yOffset), Color.Cyan);
                            size = TextRenderer.MeasureText(g, $"\u00a4 {enhancement}", font);
                            yOffset += size.Height + 2;
                        }
                    }

                    yOffset += 10;

                    // Draw Set Bonuses Subtitle
                    TextRenderer.DrawText(g, "Set Bonuses:", font, new Point(6, yOffset), TitleColor);
                    yOffset += size.Height + 5;

                    // Draw Set Bonuses
                    if (_setBonuses != null)
                    {
                        foreach (var bonus in _setBonuses)
                        {
                            var bonusTextColor = !string.IsNullOrWhiteSpace(_selectedBonus) && bonus.Contains(_selectedBonus) ? Color.Gold : BonusColor;
                            size = TextRenderer.MeasureText(g, bonus, font);

                            if (size.Width > bitmapWidth - 12)
                            {
                                var textRect = new Rectangle(new Point(6, yOffset), new Size(bitmapWidth - 12, size.Height * 2));
                                TextRenderer.DrawText(g, bonus, font, textRect, bonusTextColor, TextFormatFlags.Left | TextFormatFlags.WordBreak);
                                yOffset += size.Height * 2;
                            }
                            else
                            {
                                var textRect = new Rectangle(new Point(6, yOffset), size with { Width = bitmapWidth - 12 });
                                TextRenderer.DrawText(g, bonus, font, textRect, bonusTextColor, TextFormatFlags.Left);
                                yOffset += size.Height;
                            }
                        }
                    }
                    else
                    {
                        return bitmap;
                    }
                }
                else
                {
                    // Draw "Select Enhancement Set"
                    var size = TextRenderer.MeasureText("Select Enhancement Set", font);
                    var x = (Width - size.Width) / 2;
                    var y = (Height - size.Height) / 2;
                    TextRenderer.DrawText(g, "Select Enhancement Set", font, new Point(x, y), Color.Gray);
                }
            }

            return bitmap;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Draw border
            const int borderWidth = 2; // Set the border width
            using var borderPen = new Pen(Color.Azure, borderWidth); // Create a pen to draw the border
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1); // Draw the border

            if (_textBitmap == null)
            {
                _textBitmap = GenerateTextBitmap();

                // Determine the scroll bar height and if it will be shown
                if (_textBitmap.Height > Height)
                {
                    _vScrollBar.Visible = true;
                    _vScrollBar.Minimum = 0;
                    _vScrollBar.Maximum = _textBitmap.Height;
                    _vScrollBar.LargeChange = Height;
                    _vScrollBar.SmallChange = 20;
                }
                else
                {
                    _vScrollBar.Visible = false;
                }
            }

            var sourceRect = new Rectangle(0, _scrollOffset, _textBitmap.Width, Height - borderWidth * 2);
            var destRect = new Rectangle(borderWidth, borderWidth, Width - (_vScrollBar.Visible ? _vScrollBar.Width : 0) - borderWidth * 2, Height - borderWidth * 2);
            g.DrawImage(_textBitmap, destRect, sourceRect, GraphicsUnit.Pixel);
        }

        public class SetData
        {
            public string Set { get; init; } = string.Empty;
            public string SetRarity { get; init; } = string.Empty;
            public string SetType { get; init; } = string.Empty;
            public string LevelRange { get; init; } = string.Empty;
            public string EnhCount { get; set; } = string.Empty;
            public List<string> Enhancements { get; set; } = new();
            public List<string> Bonuses { get; set; } = new();
            public string Selected { get; set; } = string.Empty;
        }
    }
}
