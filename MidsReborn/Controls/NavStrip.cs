using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using Mids_Reborn.Controls.Extensions;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Controls
{
    public partial class NavStrip : UserControl, INotifyPropertyChanged
    {
        #region HiddenProps

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image? BackgroundImage { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout { get; set; }

        #endregion
        private FormPages? _dataSource;
        private readonly HashSet<Page> _pageHash = new();
        private readonly Color _separatorColor = Color.Black;

        [Category("Appearance")]
        public Color ActiveTabColor { get; set; } = Color.Goldenrod;

        [Category("Appearance")] 
        public override Color BackColor { get; set; } = Color.Black;

        [Category("Appearance")]
        public Color DisabledTabColor { get; set; } = Color.DarkGray;

        [Category("Appearance")]
        public Color DimmedColor { get; set; } = Color.FromArgb(21, 61, 93);

        [Category("Appearance")] 
        public override Color ForeColor { get; set; } = Color.WhiteSmoke;

        [Category("Appearance")]
        public Color InactiveTabColor { get; set; } = Color.FromArgb(30, 85, 130);

        [Category("Appearance")]
        public Color InactiveTabHoverColor { get; set; } = Color.FromArgb(43, 122, 187);

        [Category("Appearance")]
        public Color OutlineColor { get; set; } = Color.Black;

        [Category("Appearance")]
        [Description("Specifies whether the text has an outline.")]
        [DefaultValue(true)]
        public bool Outline { get; set; } = true;

        [Category("Appearance")]
        [Description("Specified whether or not to use the dimmed color as the BackColor of the control.")]
        [DefaultValue(false)]
        public bool UseDimmed { get; set; }

        [Category("Layout")] 
        public new int Padding { get; set; } = 15;


        [Browsable(false)] 
        private BindingList<NavStripItem> Items { get; } = new();

        [Category("Data")]
        [Description("Data source for the NavStrip control.")]
        public FormPages? DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource == value) return;
                if (_dataSource != null)
                {
                    _dataSource.Pages.ListChanged -= DataSource_Changed;
                    _dataSource.SelectedIndexChanged -= DataSource_SelectedIndexChanged;
                }

                _dataSource = value;

                if (_dataSource != null)
                {
                    _dataSource.Pages.ListChanged += DataSource_Changed;
                    _dataSource.SelectedIndexChanged += DataSource_SelectedIndexChanged;
                }

                UpdateItems();
            }
        }

        public NavStrip()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor | ControlStyles.UserPaint, true);
            InitializeComponent();
            Load += OnLoad;
            PropertyChanged += NavStrip_PropertyChanged;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            GetSelectedIndex();
        }

        private void GetSelectedIndex()
        {
            if (DataSource == null) return;
            for (var index = 0; index < Items.Count; index++)
            {
                Items[index].State = index == DataSource.SelectedIndex ? NavItemState.Active : NavItemState.Inactive;
            }
            Invalidate();
        }

        private void DataSource_Changed(object? sender, ListChangedEventArgs e)
        {
            UpdateItems();
        }

        private void DataSource_SelectedIndexChanged(object? sender, int pageindex)
        {
            for (var index = 0; index < Items.Count; index++)
            {
                Items[index].State = index == pageindex ? NavItemState.Active : NavItemState.Inactive;
            }
            Invalidate();
        }

        private void UpdateItems()
        {
            Items.Clear();
            _pageHash.Clear();
            
            if (_dataSource == null) return;
            
            foreach (var page in _dataSource.Pages)
            {
                if (_pageHash.Contains(page)) continue;
                Items.Add(new NavStripItem(page));
                _pageHash.Add(page); 
            }
            
            Invalidate();
        }

        private void NavStrip_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is not "DataSource" and "_dataSource")
            {
                Invalidate();
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            for (var index = 0; index < Items.Count; index++)
            {
                var item = Items[index];
                if (item.State == NavItemState.Disabled) continue;
                if (e.X >= item.Bounds.Left && e.X <= item.Bounds.Right && e.Y >= item.Bounds.Top && e.Y <= item.Bounds.Bottom)
                {
                    if (DataSource != null)
                    {
                        DataSource.SelectedIndex = index;
                    }
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            foreach (var item in Items)
            {
                item.IsHighlighted = false;
            }
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (Items.Count == 0) return;
            foreach (var item in Items)
            {
                if (e.X >= item.Bounds.Left && e.X <= item.Bounds.Right && e.Y >= item.Bounds.Top && e.Y <= item.Bounds.Bottom)
                {
                    item.IsHighlighted = true;
                }
                else
                {
                    item.IsHighlighted = false;
                }
            }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Set graphics options
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            // Brushes and Pens
            using var backBrush = new SolidBrush(BackColor);
            using var foreBrush = new SolidBrush(ForeColor);
            using var penBrush = new SolidBrush(_separatorColor);
            using var activeBrush = new SolidBrush(ActiveTabColor);
            using var dimmedBrush = new SolidBrush(DimmedColor);
            using var inactiveBrush = new SolidBrush(InactiveTabColor);
            using var inactiveHoverBrush = new SolidBrush(InactiveTabHoverColor);
            using var disabledBrush = new SolidBrush(DisabledTabColor);
            using var pen = new Pen(penBrush, 2);

            // Fill the background
            e.Graphics.FillRectangle(UseDimmed ? dimmedBrush : backBrush, ClientRectangle);

            var xLoc = 0;
            foreach (var item in Items)
            {
                var textSize = TextRenderer.MeasureText(e.Graphics, item.Text, Font);
                var itemRect = new Rectangle(xLoc, 0, textSize.Width + Padding, Height - 1);
                item.Bounds = itemRect;
                e.Graphics.DrawRectangle(pen, itemRect);
                switch (item.State)
                {
                    case NavItemState.Active:
                        e.Graphics.FillRectangle(activeBrush, itemRect);
                        break;
                    case NavItemState.Disabled:
                        e.Graphics.FillRectangle(disabledBrush, itemRect);
                        break;
                    case NavItemState.Inactive when !item.IsHighlighted:
                        e.Graphics.FillRectangle(inactiveBrush, itemRect);
                        break;
                    case NavItemState.Inactive when item.IsHighlighted:
                        e.Graphics.FillRectangle(inactiveHoverBrush, itemRect);
                        break;
                }

                if (Outline)
                {
                    TextRendererExt.DrawOutlineText(e.Graphics, item.Text, Font, itemRect, OutlineColor, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
                else
                {
                    TextRenderer.DrawText(e.Graphics, item.Text, Font, itemRect, ForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }

                xLoc += itemRect.Width;
            }

        }

        private event PropertyChangedEventHandler? PropertyChanged;

        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add => PropertyChanged += value;
            remove => PropertyChanged -= value;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
