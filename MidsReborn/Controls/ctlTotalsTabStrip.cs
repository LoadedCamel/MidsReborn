using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlTotalsTabStrip : UserControl, INotifyPropertyChanged
    {
        #region Events
        public delegate void TabClickEventHandler(int index);
        public event TabClickEventHandler? TabClick;
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Private properties
        private int _SelectedItem;
        private int _HighlightedItem = -1;
        private List<float> ItemStops = new();
        #endregion

        #region Public properties/enabled in designer
        /// <summary>
        /// Background color for currently active tab
        /// </summary>
        [Category("Appearance")]
        public Color ActiveTabColor { get; set; } = Color.Goldenrod;
        
        /// <summary>
        /// Background color for inactive tabs
        /// </summary>
        [Category("Appearance")]
        public Color InactiveTabColor { get; set; } = Color.FromArgb(30, 85, 130);

        /// <summary>
        /// Background color for inactive, hovered tabs
        /// </summary>
        [Category("Appearance")]
        public Color InactiveHoveredTabColor { get; set; } = Color.FromArgb(43, 122, 187);

        /// <summary>
        /// Dimmed background color for the unused portion of the strip
        /// </summary>
        [Category("Appearance")]
        public Color DimmedBackgroundColor { get; set; } = Color.FromArgb(21, 61, 93);

        /// <summary>
        /// Color of the bottom strip line
        /// </summary>
        [Category("Appearance")]
        public Color StripLineColor { get; set; } = Color.Goldenrod;

        /// <summary>
        /// Allow to outline items text
        /// </summary>
        [Category("Layout")]
        public bool OutlineText { get; set; } = true;

        /// <summary>
        /// Item padding
        /// </summary>
        [Category("Layout")]
        public int ItemPadding { get; set; } = 18;

        /// <summary>
        /// Use a dimmed background color to fill the unused portion of the strip
        /// </summary>
        [Category("Layout")]
        public bool UseDimmedBackground { get; set; } = false;
        
        /// <summary>
        /// Items texts
        /// </summary>
        [Category("Data")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version = 2.0.0.0, Culture = neutral, PublicKeyToken = b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public List<string> Items { get; set; } = new();
        #endregion

        #region Items handling
        /// <summary>
        /// Add a single item to the collection
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(string item)
        {
            Items.Add(item);
        }

        /// <summary>
        /// Add multiple items to the collection
        /// </summary>
        /// <param name="items">List/Array of items</param>
        public void AddItemsRange(IList<string> items)
        {
            Items.AddRange(items);
        }

        /// <summary>
        /// Purges all items from collection
        /// </summary>
        public void ClearItems()
        {
            Items = new List<string>();
        }
        #endregion

        public ctlTotalsTabStrip()
        {
            SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            InitializeComponent();

            PropertyChanged += OnPropertyChanged;
        }

        #region Event handlers
        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Need to filter on property name ?
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Parent is minimized,
            // any draw operation will fail
            if (Width == 0 || Height == 0)
            {
                return;
            }

            const float fontSize = 14;

            e.Graphics.Clear(UseDimmedBackground ? DimmedBackgroundColor : InactiveTabColor);
            using var activeTabBrush = new SolidBrush(ActiveTabColor);
            using var inactiveTabBrush = new SolidBrush(InactiveTabColor);
            using var inactiveHoveredBrush = new SolidBrush(InactiveHoveredTabColor);
            using var dimmedBrush = new SolidBrush(DimmedBackgroundColor);
            using var stripPen = new Pen(StripLineColor);
            using var textFont = new Font(new FontFamily("Segoe UI"), fontSize, FontStyle.Bold, GraphicsUnit.Pixel);

            var x = 1;
            for (var i = 0; i < Items.Count; i++)
            {
                var itemTextSize = TextRenderer.MeasureText(Items[i], textFont);
                var cellRect = new Rectangle(x, 0, itemTextSize.Width + 2 * ItemPadding, Height - 1);

                if (i == _SelectedItem)
                {
                    e.Graphics.FillRectangle(activeTabBrush, cellRect);
                }
                else if (i == _HighlightedItem)
                {
                    e.Graphics.FillRectangle(inactiveHoveredBrush, cellRect);
                }
                else
                {
                    e.Graphics.FillRectangle(inactiveTabBrush, cellRect);
                }

                x += ItemPadding;
                if (OutlineText)
                {
                    DrawOutlineText(e.Graphics, cellRect, Color.Black, Color.WhiteSmoke, textFont, Items[i], TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, Items[i], textFont, cellRect, Color.WhiteSmoke, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                x += itemTextSize.Width + ItemPadding;

                ItemStops.Add(x);
            }

            e.Graphics.FillRectangle(UseDimmedBackground ? dimmedBrush : inactiveTabBrush, new Rectangle(x, 0, Width, Height - 1));
            e.Graphics.DrawLine(stripPen, new Point(0, Height), new Point(Width, Height));
        }

        private void ctlTotalsTabStrip_MouseClick(object sender, MouseEventArgs e)
        {
            _HighlightedItem = GetHighlightedItem(e.X, e.Y);
            if (_HighlightedItem <= -1)
            {
                return;
            }

            _SelectedItem = _HighlightedItem;
            Invalidate();
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
            Invalidate();
        }

        private void ctlTotalsTabStrip_MouseLeave(object sender, EventArgs e)
        {
            _HighlightedItem = -1;

            Invalidate();
        }

        private void ctlTotalsTabStrip_Load(object sender, EventArgs e)
        {
            Invalidate();
        }
        #endregion

        #region Utility methods
        private void DrawOutlineText(Graphics g, Rectangle bounds, Color outlineColor, Color textColor, Font font, string text, TextFormatFlags formatFlags)
        {
            // Possibly slow - needs improvement.

            // W E N S
            bounds.Offset(-1, 0);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);
            bounds.Offset(2, 0);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);
            bounds.Offset(-1, -1);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);
            bounds.Offset(0, 2);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);

            // NW NE SW SE
            bounds.Offset(-1, -2);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);
            bounds.Offset(2, 0);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);
            bounds.Offset(-2, 2);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);
            bounds.Offset(2, 0);
            TextRenderer.DrawText(g, text, font, bounds, outlineColor, formatFlags);

            bounds.Offset(-1, -1);
            TextRenderer.DrawText(g, text, font, bounds, textColor, formatFlags);
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
        #endregion
    }
}