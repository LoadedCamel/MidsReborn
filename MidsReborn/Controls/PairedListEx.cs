using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using Mids_Reborn.Core;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Linq;

namespace Mids_Reborn.Controls
{
    // DO NOT USE - Still in Development
    public partial class PairedListEx : UserControl
    {
        private int _columns = 2;
        private Color _valueAlternateColor = Color.Chartreuse;
        private Color _valueConditionColor = Color.Firebrick;
        private Color _valueSpecialColor = Color.SlateBlue;
        private Color _highlightColor = Color.CornflowerBlue;
        private Color _highlightTextColor = Color.Black;
        private int _rows = 5;
        private Color _valueColor = Color.WhiteSmoke;
        private Color _itemColor = Color.Silver;
        private int _hoverIndex = -1;
        private bool _setItemsBold;
        private List<Item>? _items;
        private bool _autoSizeLineHeight = true;

        public delegate void ItemClickEventHandler(object? sender, Item item, MouseEventArgs e);
        public delegate void ItemHoverEventHandler(object? sender, int index, Enums.ShortFX tagId, string? tooltip = "");
        public delegate void ItemOutEventHandler(object? sender);

        public event ItemClickEventHandler? ItemClick;
        public event ItemHoverEventHandler? ItemHover;
        public event ItemOutEventHandler? ItemOut;

        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                Invalidate();
            }
        }

        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                Invalidate();
            }
        }

        private List<Item>? Items
        {
            get
            {
                if (_items != null) return _items;
                _items = new List<Item>();
                AddItem(new Item("Item 1:", "Value", false));
                AddItem(new Item("Item 2:", "Alternate", true));
                AddItem(new Item("Item 3:", "1000", false));
                AddItem(new Item("Item 4:", "1,000", false));
                AddItem(new Item("1234567890:", "12345678901234567890", false));
                AddItem(new Item("1234567890:", "12345678901234567890", true));
                AddItem(new Item("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", false));
                AddItem(new Item("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", true));
                AddItem(new Item("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", false, true));
                AddItem(new Item("1 2 3 4 5 6 7 8 9 0:", "1 2 3 4 5 6 7 8 9 0", false, false, true));
                return _items;
            }
        } 

        public bool SetItemsBold
        {
            get => _setItemsBold;
            set
            {
                _setItemsBold = value;
                Invalidate();
            }
        }

        public Color ItemColor
        {
            get => _itemColor;
            set
            {
                _itemColor = value;
                Invalidate();
            }
        }

        public Color ValueColor
        {
            get => _valueColor;
            set
            {
                _valueColor = value;
                Invalidate();
            }
        }

        public Color ValueAlternateColor
        {
            get => _valueAlternateColor;
            set
            {
                _valueAlternateColor = value;
                Invalidate();
            }
        }

        // Unique Color
        public Color ValueConditionColor
        {
            get => _valueConditionColor;
            set
            {
                _valueConditionColor = value;
                Invalidate();
            }
        }

        public Color ValueSpecialColor
        {
            get => _valueSpecialColor;
            set
            {
                _valueSpecialColor = value;
                Invalidate();
            }
        }

        public bool UseHighlighting { get; set; }

        public Color HighlightColor
        {
            get => _highlightColor;
            set
            {
                _highlightColor = value;
                Invalidate();
            }
        }

        public Color HighlightTextColor
        {
            get => _highlightTextColor;
            set
            {
                _highlightTextColor = value;
                Invalidate();
            }
        }

        public bool AutoSizeLineHeight
        {
            get => _autoSizeLineHeight;
            set
            {
                _autoSizeLineHeight = value;
                Invalidate();
            }
        }

        public int ItemCount => Items?.Count ?? 0;

        public PairedListEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            FontChanged += OnFontChanged;
            ForeColorChanged += OnForeColorChanged;
            Load += OnLoad;
            MouseClick += OnMouseClick;
            MouseMove += OnMouseMove;
            MouseLeave += OnMouseLeave;
            Resize += OnResize;
            ItemClick += OnItemClick;
            ItemHover += OnItemHover;
            ItemOut += OnItemOut;
            InitializeComponent();
        }

        private static void OnItemClick(object? sender, Item item, MouseEventArgs mouseEventArgs) { }

        private static void OnItemHover(object? sender, int index, Enums.ShortFX tagId, string? tooltip = "") { }

        private static void OnItemOut(object? sender) { }

        private void OnForeColorChanged(object? sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnFontChanged(object? sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnResize(object? sender, EventArgs e)
        {
            Invalidate();
        }

        private void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (Items == null)
            {
                return;
            }

            foreach (var item in Items.Where(item => e.X >= item.Bounds.Left && e.X <= item.Bounds.Right && e.Y >= item.Bounds.Top && e.Y <= item.Bounds.Bottom))
            {
                ItemClick?.Invoke(this, item, e);
            }
        }

        private void OnMouseLeave(object? sender, EventArgs e)
        {
            if (UseHighlighting)
            {
                Items?.ForEach(i => i.IsHighlightable = false);
            }

            Invalidate();
        }

        private void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (Items == null)
            {
                return;
            }

            var hoveredItem = -1;
            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (e.X >= item.Bounds.Left && e.X <= item.Bounds.Right && e.Y >= item.Bounds.Top && e.Y <= item.Bounds.Bottom)
                {
                    if (UseHighlighting)
                    {
                        item.IsHighlightable = true;
                        Invalidate();
                    }

                    if (item.ToolTip == null)
                    {
                        continue;
                    }

                    hoveredItem = i;
                }
                else
                {
                    item.IsHighlightable = false;
                }
            }

            if (_hoverIndex == hoveredItem)
            {
                return;
            }

            _hoverIndex = hoveredItem;
            if (hoveredItem <= -1)
            {
                ItemOut?.Invoke(this);
                return;
            }

            if (Items[hoveredItem].ToolTip != null) ItemHover?.Invoke(this, hoveredItem, Items[hoveredItem].TagId, Items[hoveredItem].ToolTip);
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            _hoverIndex = -1;
            Redraw();
        }

        public void AddItem(Item iItem)
        {
            Items?.Add(iItem);
            Invalidate();
        }

        public void Clear(bool redraw = false)
        {
            Items?.Clear();
            if (redraw) Invalidate();
        }

        public void Redraw()
        {
            Invalidate();
        }

        public void SetUnique()
        {
            if (Items is { Count: > 0 }) Items[^1].UseUniqueColor = true;
        }

        public bool IsSpecialColor()
        {
            return Items != null && Items[^1].UseSpecialColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call base paint 
            base.OnPaint(e);

            // Clear the canvas
            e.Graphics.Clear(Color.Transparent);

            // Set graphics options
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            // Fill client area with BackColor
            var backBrush = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(backBrush, ClientRectangle);

            // Perform drawing
            if (Items == null)
            {
                return;
            }

            if (!Items.Any())
            {
                return;
            }

            const int interlineHeight = 5;
            const int vPadding = 3;
            var y = vPadding;
            var rect = ClientRectangle;
            var columnWidth = rect.Width / Columns;
            var rowHeight = rect.Height / Rows;
            var font = SetItemsBold
                ? new Font(Font.FontFamily, Font.Size, FontStyle.Bold)
                : Font;

            if (Items.Count > 0 & !_autoSizeLineHeight)
            {
                var nameMeasured = TextRenderer.MeasureText(Items[0].Name?.Trim(), font, new Size(new Point(5, 0)), TextFormatFlags.Left | TextFormatFlags.NoPadding);
                Rows = (int)Math.Floor(Math.Max(0, rect.Height - 2 * vPadding + interlineHeight) / (double)(nameMeasured.Height + interlineHeight));
            }

            var itemLocations = new List<Point>();
            for (var cIndex = 0; cIndex < Columns; cIndex++)
            {
                for (var rIndex = 0; rIndex < Rows; rIndex++)
                {
                    var tLocation = new Point(5 + columnWidth * cIndex, rowHeight * rIndex);
                    itemLocations.Add(tLocation);
                }
            }

            for (var index = 0; index < Math.Min(Items.Count, itemLocations.Count); index++)
            {
                var itemColor = ItemColor;
                Color valueColor;
                var itemName = Items[index].Name?.Trim();
                if (string.IsNullOrWhiteSpace(itemName)) continue;
                if (!itemName.EndsWith(":"))
                {
                    itemName += ":";
                }

                var itemValue = Items[index].Value?.Trim();
                var nameMeasured = TextRenderer.MeasureText(itemName, font, new Size(itemLocations[index]), TextFormatFlags.Left | TextFormatFlags.NoPadding);
                var valueMeasured = TextRenderer.MeasureText(itemValue, font, new Size(itemLocations[index]), TextFormatFlags.Left | TextFormatFlags.NoPadding);
                y += index == 0 ? 0 : nameMeasured.Height + interlineHeight;
                var nameLocation = _autoSizeLineHeight
                    ? itemLocations[index]
                    : new Point(itemLocations[index].X, y);

                if (!_autoSizeLineHeight)
                {
                    itemLocations[index] = nameLocation;
                }

                var valueLocation = nameLocation with {X = nameLocation.X + nameMeasured.Width + 2};

                var itemBounds = new Rectangle(itemLocations[index], nameMeasured with { Width = nameMeasured.Width + valueMeasured.Width + 2 });
                Items[index].SetBounds(itemBounds);
                e.Graphics.FillRectangle(backBrush, itemBounds);

                if (UseHighlighting)
                {
                    if (Items[index].IsHighlightable)
                    {
                        var highlightBrush = new SolidBrush(HighlightColor);
                        e.Graphics.FillRectangle(highlightBrush, itemBounds);
                        itemColor = HighlightTextColor;
                        valueColor = HighlightTextColor;

                    }
                    else
                    {
                        e.Graphics.FillRectangle(backBrush, itemBounds);
                        if (Items[index].UseAlternateColor)
                        {
                            valueColor = ValueAlternateColor;
                        }
                        else if (Items[index].UseSpecialColor)
                        {
                            valueColor = ValueSpecialColor;
                        }
                        else if (Items[index].UseUniqueColor)
                        {
                            valueColor = ValueConditionColor;
                        }
                        else
                        {
                            itemColor = ItemColor;
                            valueColor = ValueColor;
                        }
                    }
                }
                else
                {
                    if (Items[index].UseAlternateColor)
                    {
                        valueColor = ValueAlternateColor;
                    }
                    else if (Items[index].UseSpecialColor)
                    {
                        valueColor = ValueSpecialColor;
                    }
                    else if (Items[index].UseUniqueColor)
                    {
                        valueColor = ValueConditionColor;
                    }
                    else
                    {
                        itemColor = ItemColor;
                        valueColor = ValueColor;
                    }
                }

                TextRenderer.DrawText(e.Graphics, itemName, font, nameLocation, itemColor, TextFormatFlags.Left | TextFormatFlags.NoPadding);
                TextRenderer.DrawText(e.Graphics, itemValue, font, valueLocation, valueColor, TextFormatFlags.Left | TextFormatFlags.NoPadding);
            }
        }

        public class Item
        {
            public string? Name { get; set; }
            public string? Value { get; set; }
            public Enums.ShortFX TagId { get; }
            public string? ToolTip { get; set; }
            public bool UseAlternateColor { get; set; }
            public bool UseSpecialColor { get; set; }
            public bool UseUniqueColor { get; set; }
            public Rectangle Bounds { get; private set; }
            public bool IsHighlightable { get; set; }
            public SummonedEntity? EntTag { get; set; }

            public void SetBounds(Rectangle rect)
            {
                Bounds = rect;
            }

            public Item(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public Item(string name, string value, bool useAlternate, bool useSpecial, bool useUnique, string tip)
            {
                Name = name;
                Value = value;
                TagId.Add(-1, 0f);
                ToolTip = tip;
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
            }

            public Item(string name, string value, bool useAlternate, bool useSpecial, bool useUnique, string tip, SummonedEntity entTag)
            {
                Name = name;
                Value = value;
                TagId.Add(-1, 0f);
                ToolTip = tip;
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
                EntTag = entTag;
            }

            public Item(string name, string value, bool useAlternate = false, bool useSpecial = false, bool useUnique = false, int idValue = -1)
            {
                Name = name;
                Value = value;
                TagId.Add(idValue, 0f);
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
            }

            public Item(string name, string value, bool useAlternate, bool useSpecial, bool useUnique, Enums.ShortFX fxId)
            {
                Name = name;
                Value = value;
                TagId.Assign(fxId);
                UseAlternateColor = useAlternate;
                UseSpecialColor = useSpecial;
                UseUniqueColor = useUnique;
            }

            public Item(Item item)
            {
                Name = item.Name;
                Value = item.Value;
                TagId.Assign(item.TagId);
                ToolTip = item.ToolTip;
                UseAlternateColor = item.UseAlternateColor;
                UseSpecialColor = item.UseSpecialColor;
                UseUniqueColor = item.UseUniqueColor;
            }
        }
    }
}
