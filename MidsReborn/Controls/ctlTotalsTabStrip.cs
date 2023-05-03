using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Controls.Extensions;
using Mids_Reborn.Core;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Mids_Reborn.Controls
{
    public partial class ctlTotalsTabStrip : UserControl
    {
        public delegate void TabClickEventHandler(int index);
        public event TabClickEventHandler TabClick;

        private Color _ActiveTabColor;
        private Color _InactiveTabColor;
        private Color _InactiveHoveredTabColor;
        private Color _StripLineColor;
        private bool _OutlineText = true;
        private int _ItemPadding = 18;
        private int _SelectedItem;
        private int _HighlightedItem = -1;
        private List<float> ItemStops = new();

        public Color ColorActiveTab
        {
            get => _ActiveTabColor;
            set
            {
                _ActiveTabColor = value;

                Draw();
            }
        }

        public Color ColorInactiveTab
        {
            get => _InactiveTabColor;
            set
            {
                _InactiveTabColor = value;

                Draw();
            }
        }

        public Color ColorInactiveHoveredTab
        {
            get => _InactiveHoveredTabColor;
            set
            {
                _InactiveHoveredTabColor = value;

                Draw();
            }
        }

        public Color ColorStripLine
        {
            get => _StripLineColor;
            set
            {
                _StripLineColor = value;

                Draw();
            }
        }

        public bool OutlineText
        {
            get => _OutlineText;
            set
            {
                _OutlineText = value;

                Draw();
            }
        }

        public int ItemPadding
        {
            get => _ItemPadding;
            set
            {
                _ItemPadding = value;

                Draw();
            }
        }

        public List<string> Items = new();

        public ctlTotalsTabStrip()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            InitializeComponent();
        }

        public void AddItem(string item)
        {
            Items.Add(item);
        }

        public void AddItemsRange(IList<string> items)
        {
            Items.AddRange(items);
        }

        public void ClearItems()
        {
            Items = new List<string>();
        }

        public void Redraw()
        {
            Draw();
        }

        private void Draw()
        {
            // Parent is minimized,
            // any draw operation will fail
            if (Width == 0 || Height == 0)
            {
                return;
            }

            using var s = SKSurface.Create(new SKImageInfo(Width, Height));
            const float fontSize = 15;

            var textRect = new SKRect();
            using var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor(),
                TextSize = fontSize
            };

            using var stripPaint = new SKPaint
            {
                IsAntialias = true,
                Color = ForeColor.ToSKColor(),
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1,
                StrokeCap = SKStrokeCap.Butt
            };

            using var activeTabPaint = new SKPaint
            {
                Color = _ActiveTabColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var inactiveTabPaint = new SKPaint
            {
                Color = _InactiveTabColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            using var inactiveHoveredTabPaint = new SKPaint
            {
                Color = _InactiveHoveredTabColor.ToSKColor(),
                Style = SKPaintStyle.Fill
            };

            s.Canvas.Clear(_InactiveTabColor.ToSKColor());
            
            // If initial x position is set to zero, a 1px bar of highlighted color appear on the right on load.
            var x = 1f;
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                textPaint.MeasureText(item, ref textRect);
                var cellRect = new SKRect(x, 0, x + textRect.Width + 2 * _ItemPadding, Height - 1);

                if (i == _SelectedItem)
                {
                    s.Canvas.DrawRect(cellRect, activeTabPaint);
                }
                else if (i == _HighlightedItem)
                {
                    s.Canvas.DrawRect(cellRect, inactiveHoveredTabPaint);
                }
                else
                {
                    s.Canvas.DrawRect(cellRect, inactiveTabPaint);
                }

                x += _ItemPadding;
                if (OutlineText)
                {
                    
                    s.Canvas.DrawOutlineText(Items[i], cellRect, ForeColor.ToSKColor(), Enums.eHTextAlign.Center, Enums.eVTextAlign.Middle, 255, fontSize);
                }
                else
                {
                    s.Canvas.DrawTextShort(Items[i], fontSize, x, Height / 2f + 4, textPaint);
                }

                x += textRect.Width + _ItemPadding;

                ItemStops.Add(x);
            }

            s.Canvas.DrawRect(x, 0, Width, Height - 1, inactiveTabPaint);
            s.Canvas.DrawLine(0, Height, Width, Height, stripPaint);

            BackgroundImage = s.Snapshot().ToBitmap();
        }

        private int GetHighlightedItem(int mx, int my)
        {
            for (var i = 0; i < ItemStops.Count; i++)
            {
                if (mx > ItemStops[i])
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        private void ctlTotalsTabStrip_MouseClick(object sender, MouseEventArgs e)
        {
            _HighlightedItem = GetHighlightedItem(e.X, e.Y);
            if (_HighlightedItem <= -1)
            {
                return;
            }

            _SelectedItem = _HighlightedItem;
            Draw();
            TabClick?.Invoke(_HighlightedItem);
        }

        private void ctlTotalsTabStrip_MouseMove(object sender, MouseEventArgs e)
        {
            var highlightedItem = GetHighlightedItem(e.X, e.Y);

            if (highlightedItem == _HighlightedItem)
            {
                return;
            }

            _HighlightedItem = highlightedItem;
            Draw();
        }

        private void ctlTotalsTabStrip_MouseLeave(object sender, EventArgs e)
        {
            _HighlightedItem = -1;

            Draw();
        }

        private void ctlTotalsTabStrip_Load(object sender, EventArgs e)
        {
            Draw();
        }
    }
}