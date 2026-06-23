using System.ComponentModel;

using System.Drawing.Drawing2D;
using Mids_Reborn.UI.Theming;

namespace Mids_Reborn.UI.Forms.Controls
{
    public partial class AdvListView : ListView
    {
        public event EventHandler? DataSourceChanged;

        private object? _dataSource;
        private bool _useAppTheme;

        [Category("Appearance")]
        [DefaultValue(false)]
        public bool UseAppTheme
        {
            get => _useAppTheme;
            set
            {
                if (_useAppTheme == value)
                {
                    return;
                }

                _useAppTheme = value;
                OwnerDraw = value;
                ApplyThemeColors();
                Invalidate();
            }
        }

        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        public object? DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                ColumnMappings = new List<ColumnMapping>();
                BindListView();
                DataSourceChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal List<ColumnMapping>? ColumnMappings { get; set; }
        
        public AdvListView()
        {
            InitializeComponent();
        }

        private DataViewTheme CurrentTheme => DesignMode
            ? ThemeManager.DesignTime.DataView
            : ThemeManager.CurrentTheme?.DataView ?? ThemeManager.DesignTime.DataView;

        public void AddColumnMapping(int columnIndex, Func<object, object?> dataRetriever, Func<object?, object?>? transformer1 = null, Func<object?, object?>? dataRetriever2 = null, Func<object?, object>? transformer2 = null, Func<object?, object?>? tagFunction = null)
        {
            ColumnMappings ??= new List<ColumnMapping>();

            // Check if the mapping for the specified column index already exists and update it if needed
            var existingMapping = ColumnMappings.FirstOrDefault(mapping => mapping.ColumnIndex == columnIndex);
            if (existingMapping != null)
            {
                existingMapping.DataRetriever = dataRetriever;
                existingMapping.DataRetriever2 = dataRetriever2;
                existingMapping.Transformer1 = transformer1;
                existingMapping.Transformer2 = transformer2;
                existingMapping.TagFunction = tagFunction;
            }
            else
            {
                ColumnMappings.Add(new ColumnMapping
                {
                    ColumnIndex = columnIndex,
                    DataRetriever = dataRetriever,
                    DataRetriever2 = dataRetriever2,
                    Transformer1 = transformer1,
                    Transformer2 = transformer2,
                    TagFunction = tagFunction
                });
            }

            // Rebind the ListView to reflect the changes
            BindListView();
        }

        private void BindListView()
        {
            // Clear existing items in the ListView
            Items.Clear();

            // Check if there's no data source or no columns specified
            if (_dataSource == null || ColumnMappings == null || ColumnMappings.Count == 0)
            {
                return;
            }

            // Ensure that the data source is of type IEnumerable<object>
            if (_dataSource is not IEnumerable<object> dataObjects)
            {
                throw new ArgumentException("DataSource must implement IEnumerable<object>");
            }

            // Loop through each data object in the data source
            var enumerable = dataObjects.ToList();
            foreach (var dataObject in enumerable)
            {
                // Create a ListViewItem to represent this data object
                var item = new ListViewItem();
                
                // Loop through each column mapping
                foreach (var mapping in ColumnMappings)
                {
                    var columnIndex = mapping.ColumnIndex;

                    // Use the custom data retriever delegate to get the data from the object
                    var dataValue = mapping.DataRetriever(dataObject);
                    var transformedValue1 = mapping.Transformer1?.Invoke(dataValue) ?? dataValue;

                    string columnText;

                    if (mapping.DataRetriever2 != null)
                    {
                        var dataValue2 = mapping.DataRetriever2(dataObject);
                        var transformedValue2 = mapping.Transformer2?.Invoke(dataValue2) ?? dataValue2;
                        columnText = $"{transformedValue1} - {transformedValue2}";
                    }
                    else if (mapping.Transformer2 != null)
                    {
                        // Use the second transformer if provided and second data retriever is null
                        var transformedValue2 = mapping.Transformer2(dataValue);
                        columnText = $"{transformedValue1} - {transformedValue2}";
                    }
                    else
                    {
                        columnText = transformedValue1?.ToString() ?? string.Empty;
                    }

                    // Add the column text to the ListViewItem
                    if (item.SubItems.Count <= columnIndex)
                    {
                        item.SubItems.Add(columnText);
                    }
                    else
                    {
                        item.SubItems[columnIndex].Text = columnText;
                    }

                    // Set the Tag value if applicable
                    if (mapping.TagFunction != null)
                    {
                        item.Tag = mapping.TagFunction(dataObject);
                    }
                }

                // Check to make sure the image list is not null and is not empty prior to assigning an image to an item
                if (SmallImageList is not null && SmallImageList.Images.Count > 0)
                {
                    var imageIndex = enumerable.IndexOf(dataObject);
                    if (imageIndex > -1) item.ImageIndex = imageIndex;
                }

                // Add the ListViewItem to the ListView
                Items.Add(item);
            }
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            if (!UseAppTheme)
            {
                e.DrawDefault = true;
                base.OnDrawColumnHeader(e);
                return;
            }

            var theme = CurrentTheme;
            using (var brush = new LinearGradientBrush(
                       e.Bounds,
                       ResolveColor(theme.GridHeaderTop, theme.HeaderTop),
                       ResolveColor(theme.GridHeaderBottom, theme.HeaderBottom),
                       LinearGradientMode.Vertical))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            using (var pen = new Pen(ResolveColor(theme.GridHeaderBorder, theme.Border)))
            {
                e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
                e.Graphics.DrawLine(pen, e.Bounds.Right - 1, e.Bounds.Top, e.Bounds.Right - 1, e.Bounds.Bottom);
            }

            var textBounds = Rectangle.Inflate(e.Bounds, -6, 0);
            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                Font,
                textBounds,
                ResolveColor(theme.Text, ForeColor),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        }

        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (!UseAppTheme || View != View.Details)
            {
                e.DrawDefault = true;
                base.OnDrawItem(e);
            }
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            if (!UseAppTheme)
            {
                e.DrawDefault = true;
                base.OnDrawSubItem(e);
                return;
            }

            var theme = CurrentTheme;
            var itemIndex = Math.Max(0, e.Item.Index);
            var selected = e.Item.Selected;
            var rowBack = selected
                ? Blend(ResolveColor(theme.TabActiveBottom, theme.Accent), ResolveColor(theme.Accent, Color.DodgerBlue), 0.22f)
                : itemIndex % 2 == 0
                    ? ResolveColor(theme.GridRowEven, theme.Card)
                    : ResolveColor(theme.GridRowOdd, Blend(theme.Card, theme.Background, 0.25f));

            using (var brush = new SolidBrush(rowBack))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            var textLeft = e.Bounds.Left + 6;
            if (e.ColumnIndex == 0 &&
                SmallImageList is { Images.Count: > 0 } &&
                e.Item.ImageIndex >= 0 &&
                e.Item.ImageIndex < SmallImageList.Images.Count)
            {
                var icon = SmallImageList.Images[e.Item.ImageIndex];
                var iconSize = Math.Max(1, Math.Min(Math.Min(SmallImageList.ImageSize.Width, SmallImageList.ImageSize.Height), e.Bounds.Height - 4));
                var iconBounds = new Rectangle(
                    e.Bounds.Left + 4,
                    e.Bounds.Top + Math.Max(0, (e.Bounds.Height - iconSize) / 2),
                    iconSize,
                    iconSize);

                e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                e.Graphics.DrawImage(icon, iconBounds);
                textLeft = iconBounds.Right + 6;
            }

            var textBounds = new Rectangle(
                textLeft,
                e.Bounds.Top,
                Math.Max(0, e.Bounds.Right - textLeft - 6),
                e.Bounds.Height);

            var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
            flags |= e.Header.TextAlign switch
            {
                HorizontalAlignment.Center => TextFormatFlags.HorizontalCenter,
                HorizontalAlignment.Right => TextFormatFlags.Right,
                _ => TextFormatFlags.Left
            };

            TextRenderer.DrawText(
                e.Graphics,
                e.SubItem.Text,
                Font,
                textBounds,
                selected ? ResolveColor(theme.Text, Color.WhiteSmoke) : ResolveColor(theme.Muted, ResolveColor(theme.Text, Color.WhiteSmoke)),
                flags);

            using var rowLinePen = new Pen(ResolveColor(theme.GridRowLine, theme.Border));
            e.Graphics.DrawLine(rowLinePen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
        }

        internal void RefreshTheme()
        {
            ApplyThemeColors();
            Invalidate();
        }

        private void ApplyThemeColors()
        {
            if (!UseAppTheme)
            {
                return;
            }

            var theme = CurrentTheme;
            BackColor = ResolveColor(theme.GridRowEven, theme.Card);
            ForeColor = ResolveColor(theme.Text, Color.WhiteSmoke);
            HideSelection = false;
            BorderStyle = BorderStyle.FixedSingle;
        }

        private static Color ResolveColor(Color value, Color fallback)
        {
            return value.IsEmpty ? fallback : value;
        }

        private static Color Blend(Color first, Color second, float amountSecond)
        {
            amountSecond = Math.Clamp(amountSecond, 0f, 1f);
            var amountFirst = 1f - amountSecond;
            return Color.FromArgb(
                (int)Math.Round(first.A * amountFirst + second.A * amountSecond),
                (int)Math.Round(first.R * amountFirst + second.R * amountSecond),
                (int)Math.Round(first.G * amountFirst + second.G * amountSecond),
                (int)Math.Round(first.B * amountFirst + second.B * amountSecond));
        }

        internal class ColumnMapping
        {
            public int ColumnIndex { get; init; }
            public Func<object, object?> DataRetriever { get; set; }
            public Func<object, object?>? DataRetriever2 { get; set; }
            public Func<object?, object?>? Transformer1 { get; set; }
            public Func<object?, object>? Transformer2 { get; set; }
            public Func<object?, object?>? TagFunction { get; set; }
        }
    }
}
