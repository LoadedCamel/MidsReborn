using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms.Controls
{
    public sealed partial class PowerList : UserControl
    {
        #region SubControl Declarations

        private readonly OutlinedLabel _outlinedLabel = new();
        private readonly TypeDropDown _typeDropDown = new();
        private readonly Panel _separatorPanel = new();
        private readonly ListPanel _listPanel = new();

        #endregion

        #region Events

        private new event EventHandler<string>? TextChanged;

        public delegate void ListItemClickedHandler(object sender, ListPanelItem item, MouseEventArgs e);
        public delegate void ListItemHoveredHandler(object sender, ListPanelItem? item, MouseEventArgs e);
        public delegate void DropDownItemHoverHandler(object sender, IPowerset? item);
        public event ListItemClickedHandler? ItemClicked;
        public event ListItemHoveredHandler? ItemHovered;
        public event DropDownItemHoverHandler? DropDownItemHovered;

        #endregion

        #region Designer Properties (HIDDEN)

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Image? BackgroundImage { get; set; } = null;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override ImageLayout BackgroundImageLayout { get; set; }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new BorderStyle BorderStyle { get; set; } = BorderStyle.None;

        #endregion

        #region Designer Properties (PRIVATE)

        private string _text = "";

        private Color _backColor = Color.Transparent;

        private Color _foreColor = Color.GhostWhite;

        #endregion

        #region Designer Properties (VISIBLE)

        [Description("The background color of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                _outlinedLabel.BackColor = _backColor;
            }
        }

        [Description("Sets the dropdown styles for the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PlDropDown DropDown { get; set; } = new();

        [Description("The font to be used on the label of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Font Font { get; set; } = new("Microsoft Sans Serif", 8.25f, FontStyle.Bold);

        [Description("The color of the text used for the control label.")]
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
                _foreColor = value;
                _outlinedLabel.ForeColor = _foreColor;
            }
        }

        [Description("The text to be displayed as the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override string Text
        {
            get => _text;
            set
            {
                _text = value;
                TextChanged?.Invoke(this, _text);
            }
        }

        [Description("Determines the outline appearance of the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public PlOutline TextOutline { get; set; } = new();

        #endregion

        #region Internal Properties

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal static PlDropDown IplDropDown = new();

        #endregion

        #region Public Properties

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ListPanelItem> Items;

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListPanelItem? HoverItem;

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ListPanelItem? SelectedItem;

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPowerset? SelectedPowerset;

        [Browsable(false)] [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LevelUpMode { get; set; }

        public enum DropDownType
        {
            None,
            Primary,
            Secondary,
            Pool,
            Ancillary
        }

        [Browsable(false)] 
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DropDownEnabled
        {
            get => _typeDropDown.Enabled;
            set => _typeDropDown.Enabled = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Point DropDownLocation => _typeDropDown.Location;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size DropDownSize => _typeDropDown.Size;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle DropDownBounds => _typeDropDown.Bounds;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle ListBounds => _listPanel.Bounds;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle LabelBounds => _outlinedLabel.Bounds;

        public void UpdateTextColors(ItemState state, Color color)
        {
            if ((state < ItemState.Enabled) | (state > ItemState.Invalid))
                return;
            _listPanel.Colors[(int)state] = color;
        }

        [Flags]
        public enum ItemState
        {
            Enabled = 0,
            Selected = 1,
            Disabled = 2,
            SelectedDisabled = 3,
            Invalid = 4,
        }

        #endregion

        #region Designer Properties TypeConverters and Classes

        public class PowerListTypeConverter<T> : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext? context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            {
                return TypeDescriptor.GetProperties(typeof(T));
            }
        }

        [TypeConverter(typeof(PowerListTypeConverter<PlDropDown>))]
        public class PlDropDown : INotifyPropertyChanged
        {
            [Description("The color to use for selection highlighting.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Color HighlightColor { get; set; } = Color.DodgerBlue;

            [Description("The type of dropdown to be used.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public DropDownType Type { get; set; } = DropDownType.None;

            public override string ToString()
            {
                return $"{HighlightColor}, {Type}";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

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
        }

        [TypeConverter(typeof(PowerListTypeConverter<PlOutline>))]
        public class PlOutline : INotifyPropertyChanged
        {
            [Description("Enables the controls label text to be outlined.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public bool Enabled { get; set; } = false;

            [Description("The color to be used for the outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Color Color { get; set; } = Color.Black;

            [Description("The width of the outline to be used.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public int Width { get; set; } = 2;

            public override string ToString()
            {
                var isEnabled = Enabled ? "Enabled" : "Disabled";
                return $"{isEnabled}, {Color}, {Width}";
            }

            public event PropertyChangedEventHandler? PropertyChanged;

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
        }

        #endregion

        #region Constructor

        public PowerList()
        {
            Items = new List<ListPanelItem>();
            IplDropDown = DropDown;
            SuspendLayout();
            _outlinedLabel.Location = new Point(0, 0);
            _outlinedLabel.Size = new Size(83, 16);
            _outlinedLabel.Dock = DockStyle.Top;
            _outlinedLabel.Font = Font;
            _outlinedLabel.Margin = new Padding(0, 0, 0, 8);
            _outlinedLabel.Text = _text;
            _outlinedLabel.BackColor = _backColor;
            _outlinedLabel.ForeColor = _foreColor;

            _typeDropDown.Location = new Point(0, 16);
            _typeDropDown.Size = new Size(211, 21);
            _typeDropDown.Dock = DockStyle.Top;
            _typeDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            _typeDropDown.Font = new Font("Microsoft Sans Serif", 9.75f, FontStyle.Bold);
            _typeDropDown.FormattingEnabled = true;
            _typeDropDown.SelectedIndexChanged += TypeDropDownOnSelectedIndexChanged;
            _typeDropDown.DropDownHoveredItem += TypeDropDown_DropDownHoveredItem;

            _separatorPanel.Location = new Point(0, 37);
            _separatorPanel.Size = new Size(211, 8);
            _separatorPanel.Dock = DockStyle.Top;
            _separatorPanel.Enabled = false;
            _separatorPanel.BorderStyle = BorderStyle.None;
            _separatorPanel.BackColor = Color.Transparent;

            _listPanel.Location = new Point(0, 45);
            _listPanel.Size = new Size(211, 410);
            _listPanel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Bold, GraphicsUnit.Point);
            _listPanel.Dock = DockStyle.Top;
            _listPanel.BorderStyle = BorderStyle.None;
            _listPanel.DrawMode = DrawMode.OwnerDrawVariable;
            _listPanel.ItemHeight = 18;
            _listPanel.FormattingEnabled = true;
            _listPanel.ScrollAlwaysVisible = false;
            _listPanel.ItemsUpdated += ListPanelOnItemsUpdated;
            _listPanel.MouseDown += ListPanelOnMouseDown;
            _listPanel.MouseMove += ListPanelOnMouseMove;
            _listPanel.MeasureItem += ListPanelOnMeasureItem;

            Controls.Add(_listPanel);
            Controls.Add(_separatorPanel);
            Controls.Add(_typeDropDown);
            Controls.Add(_outlinedLabel);
            ResumeLayout(false);
            PerformLayout();

            DropDown.PropertyChanged += DropDownOnPropertyChanged;
            TextChanged += OnTextChanged;
            TextOutline.PropertyChanged += TextOutlineOnPropertyChanged;
            //TypeChanged += OnTypeChanged;
            InitializeComponent();
        }

        private void TypeDropDown_DropDownHoveredItem(object? sender, DropDownHoverEventArgs e)
        {
            if (e.Index >= 0) DropDownItemHovered?.Invoke(this, (IPowerset)_typeDropDown.Items[e.Index]);
        }

        private void ListPanelOnMeasureItem(object? sender, MeasureItemEventArgs e)
        {
            var measurement = TextRenderer.MeasureText(e.Graphics, Items[e.Index].Text, Font);
            e.ItemHeight = measurement.Height;
            e.ItemWidth = measurement.Width;
        }

        private void ListPanelOnMouseMove(object? sender, MouseEventArgs e)
        {
            if (_listPanel.HoverIndex != -1)
            {
                HoverItem = (ListPanelItem)_listPanel.Items[_listPanel.HoverIndex];
                ItemHovered?.Invoke(this, HoverItem, e);
            }
            else
            {
                HoverItem = null;
                ItemHovered?.Invoke(this, null, e);
            }
        }

        #endregion

        #region EventHandler Methods

        private void DropDownOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            IplDropDown = DropDown;
            _typeDropDown.Invalidate();
        }

        private void ListPanelOnItemsUpdated(object? sender, EventArgs e)
        {
            Items = _listPanel.Items.Cast<ListPanelItem>().ToList();
        }

        private void TextOutlineOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender == null) return;
            switch (e.PropertyName)
            {
                case "Enabled":
                    _outlinedLabel.OutlineEnabled = ((PlOutline)sender).Enabled;
                    break;
                case "Color":
                    _outlinedLabel.OutlineColor = ((PlOutline)sender).Color;
                    break;
                case "Width":
                    _outlinedLabel.OutlineWidth = ((PlOutline)sender).Width;
                    break;
            }

            _outlinedLabel.Invalidate();
        }

        private void OnTextChanged(object? sender, string e)
        {
            _outlinedLabel.Text = e;
            _outlinedLabel.Invalidate();
        }

        private void ListPanelOnMouseDown(object? sender, MouseEventArgs e)
        {
            var index = _listPanel.IndexFromPoint(e.Location);
            if (index < 0)
            {
                SelectedItem = null;
                return;
            }
            SelectedItem = Items[index];
            ItemClicked?.Invoke(this, Items[index], e);
        }

        private void TypeDropDownOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_typeDropDown.SelectedIndex == -1)
            {
                SelectedPowerset = null;
                return;
            }
            var selectedSet = (Powerset)_typeDropDown.Items[_typeDropDown.SelectedIndex];
            SelectedPowerset = selectedSet;
            var powerList = new List<IPower?>();
            switch (DropDown.Type)
            {
                case DropDownType.Primary:
                    switch (selectedSet.DisplayName)
                    {
                        case "Arachnos Soldier":
                            powerList = new List<IPower?>(selectedSet.Powers);
                            break;
                        case "Bane Spider Soldier":
                            var selected = DatabaseAPI.GetPowersetByName("Arachnos Soldier", Enums.ePowerSetType.Primary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);

                            break;
                        case "Crab Spider Soldier":
                            selected = DatabaseAPI.GetPowersetByName("Arachnos Soldier", Enums.ePowerSetType.Primary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);

                            break;
                        case "Fortunata Training":
                            selected = DatabaseAPI.GetPowersetByName("Widow Training", Enums.ePowerSetType.Primary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);

                            break;
                        case "Night Widow Training":
                            selected = DatabaseAPI.GetPowersetByName("Widow Training", Enums.ePowerSetType.Primary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);

                            break;
                        case "Widow Training":
                            powerList = new List<IPower?>(selectedSet.Powers);
                            break;
                        default:
                            powerList = selectedSet.Powers.ToList();
                            break;
                    }

                    break;
                case DropDownType.Secondary:
                    switch (selectedSet.DisplayName)
                    {
                        case "Bane Spider Training":
                            var selected = DatabaseAPI.GetPowersetByName("Training and Gadgets", Enums.ePowerSetType.Secondary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);
                            break;
                        case "Crab Spider Training":
                            selected = DatabaseAPI.GetPowersetByName("Training and Gadgets", Enums.ePowerSetType.Secondary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);
                            break;
                        case "Fortunata Teamwork":
                            selected = DatabaseAPI.GetPowersetByName("Teamwork", Enums.ePowerSetType.Secondary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);
                            break;
                        case "Widow Teamwork":
                            selected = DatabaseAPI.GetPowersetByName("Teamwork", Enums.ePowerSetType.Secondary);
                            if (selected != null) powerList.AddRange(selected.Powers);
                            powerList.AddRange(selectedSet.Powers);
                            break;
                        default:
                            powerList = new List<IPower?>(selectedSet.Powers);
                            break;
                    }

                    break;
                case DropDownType.Pool or DropDownType.Ancillary:
                    powerList = new List<IPower?>(selectedSet.Powers);
                    break;
            }

            powerList = powerList.Where(p => p is { Level: > 0 }).ToList();
            var powerListItems = powerList.Select((power, index) => new ListPanelItem(index, power, ItemState.Enabled)).ToList();
            
            if (LevelUpMode)
            {
                foreach (var powerItem in powerListItems)
                {
                    if (powerItem.Power?.Level != MidsContext.Character.Level) powerItem.ItemState = ItemState.Disabled;
                }
                _listPanel.Invalidate();
            }

            var boundSource = new BindingSource
            {
                DataSource = powerListItems
            };
            _listPanel.SelectionMode = SelectionMode.MultiSimple;
            _listPanel.DataSource = null;
            _listPanel.DisplayMember = "Text";
            _listPanel.ValueMember = null;
            _listPanel.DataSource = boundSource;
            _listPanel.SelectedIndex = -1;
            Items = _listPanel.Items.Cast<ListPanelItem>().ToList();
        }

        #endregion

        #region Public Methods

        public void BindToArchetype(Archetype archetype)
        {
            if (DropDown.Type == DropDownType.None) return;
            var setList = DatabaseAPI.Database.Powersets.Where(p => p?.ATClass == archetype.ClassName && p.SetType == (Enums.ePowerSetType)Enum.Parse(typeof(Enums.ePowerSetType), DropDown.Type.ToString())).ToList();
            var bindingSource = new BindingSource
            {
                DataSource = setList
            };
            _typeDropDown.DataSource = null;
            _typeDropDown.DisplayMember = "DisplayName";
            _typeDropDown.ValueMember = null;
            _typeDropDown.DataSource = bindingSource;
            _typeDropDown.SelectedIndex = 0;
            _typeDropDown.Invalidate();
        }

        public void BindAncillary(Archetype archetype)
        {
            if (DropDown.Type != DropDownType.Ancillary) return;
            if (archetype.DisplayName != "Peacebringer" && archetype.DisplayName != "Warshade")
            {
                var setList = archetype.Ancillary.Select(t => DatabaseAPI.Database.Powersets.FirstOrDefault(p => p?.SetType == Enums.ePowerSetType.Ancillary && p.nID.Equals(t))).ToList();
                var bindingSource = new BindingSource
                {
                    DataSource = setList
                };
                _typeDropDown.DataSource = null;
                _typeDropDown.Enabled = true;
                _typeDropDown.DisplayMember = "DisplayName";
                _typeDropDown.ValueMember = null;
                _typeDropDown.DataSource = bindingSource;
                _typeDropDown.SelectedIndex = 0;
                _typeDropDown.Invalidate();
            }
            else
            {
                _typeDropDown.Enabled = false;
                _typeDropDown.SelectedItem = null;
                _typeDropDown.DataSource = null;
                _typeDropDown.Items.Clear();
                _listPanel.DataSource = null;
                _listPanel.Items.Clear();
            }
        }

        public void BindPool(int selectedPool)
        {
            if (DropDown.Type != DropDownType.Pool) return;
            var setList = DatabaseAPI.Database.Powersets.Where(p => p?.SetType == Enums.ePowerSetType.Pool).ToList();
            var bindingSource = new BindingSource
            {
                DataSource = setList
            };
            _typeDropDown.DataSource = null;
            _typeDropDown.DisplayMember = "DisplayName";
            _typeDropDown.ValueMember = null;
            _typeDropDown.DataSource = bindingSource;
            _typeDropDown.SelectedIndex = selectedPool;
            _typeDropDown.Invalidate();
        }

        #endregion

        #region SubControl EventArgs

        internal class DropDownHoverEventArgs : EventArgs
        {
            public int Index { get; }
            public string Text { get; set; }
            public DropDownHoverEventArgs(int index, string text)
            {
                Index = index;
                Text = text;
            }
        }

        public class ListPanelDrawItemEventArgs : EventArgs
        {
            public Graphics Graphics { get; }
            public Font Font { get; }
            public Rectangle Bounds { get; }
            public int Index { get; }
            public ListPanelItem Item { get; set; }
            public Color ForeColor { get; }

            public ListPanelDrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, ListPanelItem item, Color foreColor)
            {
                Graphics = graphics;
                Font = font;
                Bounds = rect;
                Index = index;
                Item = item;
                ForeColor = foreColor;
            }

            public ListPanelDrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, ListPanelItem item)
            {
                Graphics = graphics;
                Font = font;
                Bounds = rect;
                Index = index;
                Item = item;
            }
        }

        #endregion

        #region SubControls

        internal class OutlinedLabel : Label
        {
            public override Color ForeColor { get; set; }
            public override Color BackColor { get; set; }
            public override ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;
            public Color OutlineColor { get; set; }
            public float OutlineWidth { get; set; }
            public bool OutlineEnabled { get; set; }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (OutlineEnabled)
                {
                    e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                    using var gp = new GraphicsPath();
                    using var outline = new Pen(OutlineColor, OutlineWidth) { LineJoin = LineJoin.Round };
                    using var sf = new StringFormat();
                    using Brush foreBrush = new SolidBrush(ForeColor);
                    gp.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, ClientRectangle, sf);
                    e.Graphics.ScaleTransform(1.3f, 1.35f);
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(outline, gp);
                    e.Graphics.FillPath(foreBrush, gp);
                }
                else
                {
                    base.OnPaint(e);
                }
            }
        }

        internal sealed class TypeDropDown : ComboBox
        {
            public event EventHandler<DropDownHoverEventArgs>? DropDownHoveredItem;

            private const int CB_GETCURSEL = 0x0147;
            private int _listItem = -1;

            private void OnListItemSelectionChanged(DropDownHoverEventArgs e) => DropDownHoveredItem?.Invoke(this, e);

            public DropDownType DropType => IplDropDown.Type;
            public Color HighlightColor => IplDropDown.HighlightColor;

            public TypeDropDown()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true); DropDownStyle = ComboBoxStyle.DropDownList;
                DrawMode = DrawMode.OwnerDrawFixed;
            }

            protected override void WndProc(ref Message m)
            {
                var selItem = -1;
                base.WndProc(ref m);

                selItem = m.Msg switch
                {
                    CB_GETCURSEL => m.Result.ToInt32(),
                    _ => selItem
                };
                if (_listItem == selItem) return;
                _listItem = selItem;
                OnListItemSelectionChanged(new DropDownHoverEventArgs(_listItem, _listItem < 0 ? string.Empty : GetItemText(Items[_listItem])));
            }

            protected override void OnDrawItem(DrawItemEventArgs e)
            {
                e.DrawBackground();
                e.DrawFocusRectangle();

                if (e.Index >= 0 && e.Index < Items.Count)
                {
                    e.Graphics.FillRectangle((e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(HighlightColor) : new SolidBrush(BackColor), e.Bounds);
                    Image? comboImage = null;
                    List<string> images;
                    string selectedImage;
                    IPowerset selected;
                    switch (DropType)
                    {
                        case DropDownType.Primary:
                            images = I9Gfx.LoadPowerSets().GetAwaiter().GetResult();
                            selected = (IPowerset)Items[e.Index];
                            selectedImage = images.FirstOrDefault(i => i.Contains(selected.ImageName));
                            if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                            break;
                        case DropDownType.Secondary:
                            images = I9Gfx.LoadPowerSets().GetAwaiter().GetResult();
                            selected = (IPowerset)Items[e.Index];
                            selectedImage = images.FirstOrDefault(i => i.Contains(selected.ImageName));
                            if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                            break;
                        case DropDownType.Pool:
                            images = I9Gfx.LoadPowerSets().GetAwaiter().GetResult();
                            selected = (IPowerset)Items[e.Index];
                            selectedImage = images.FirstOrDefault(i => i.Contains(selected.ImageName));
                            if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                            break;
                        case DropDownType.Ancillary:
                            images = I9Gfx.LoadPowerSets().GetAwaiter().GetResult();
                            selected = (IPowerset)Items[e.Index];
                            selectedImage = images.FirstOrDefault(i => i.Contains(selected.ImageName));
                            if (selectedImage != null) comboImage = Image.FromFile(selectedImage);
                            break;
                    }

                    if (e.Font == null) return;
                    if (comboImage != null)
                    {
                        e.Graphics.DrawImage(comboImage, e.Bounds.Left, e.Bounds.Top);
                        e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(e.ForeColor),
                            e.Bounds.Left + comboImage.Width + 5, e.Bounds.Top);
                    }
                    else
                    {
                        e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(e.ForeColor),
                            e.Bounds.Left, e.Bounds.Top);
                    }
                }

                base.OnDrawItem(e);
            }
        }

        public sealed class ListPanel : ListBox
        {
            public delegate void DrawPowerListItem(ListPanelDrawItemEventArgs e);
            public event DrawPowerListItem? DrawListItem;
            public event EventHandler? ItemsUpdated;

            public override Color ForeColor { get; set; }
            internal List<Color> Colors { get; set; }

            public ListPanel()
            {
                SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
                DrawMode = DrawMode.OwnerDrawFixed;
                DoubleBuffered = true;
                Colors = new List<Color>
                {
                    Color.Gold, Color.DodgerBlue, Color.LightGray, Color.DarkBlue, Color.Red,
                    Color.FromArgb(50, Color.GhostWhite)
                };
                DrawListItem += OnDrawListItem;
            }


            [DllImport("uxtheme", ExactSpelling = true)]
            private static extern int DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref Rectangle pRect);

            private int _hoverIndex = -1;

            internal int HoverIndex
            {
                get => _hoverIndex;
                private set
                {
                    if (value == _hoverIndex) return;
                    if (_hoverIndex >= 0 && _hoverIndex < Items.Count)
                    {
                        Invalidate(GetItemRectangle(_hoverIndex));
                    }

                    _hoverIndex = value;
                    if (_hoverIndex is >= 0 and < 65535) Invalidate(GetItemRectangle(_hoverIndex));
                }
            }

            private Size GetItemSize(IDeviceContext g, string itemText)
            {
                var size = TextRenderer.MeasureText(g, itemText, Font, ClientSize, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.WordBreak);
                size.Height = Math.Max(Math.Min(size.Height, 247), Font.Height) + 4;
                return size;
            }

            protected override void OnMeasureItem(MeasureItemEventArgs e)
            {
                if (Items.Count == 0) return;
                var size = GetItemSize(e.Graphics, GetItemText(Items[e.Index]));
                e.ItemWidth = size.Width;
                e.ItemHeight = size.Height;
                //base.OnMeasureItem(e);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                var rec = ClientRectangle;

                var hdc = g.GetHdc();
                _ = DrawThemeParentBackground(Handle, hdc, ref rec);
                g.ReleaseHdc(hdc);

                using var reg = new Region(e.ClipRectangle);
                if (Items.Count <= 0) return;
                for (var i = 0; i < Items.Count; i++)
                {
                    rec = GetItemRectangle(i);

                    if (!e.ClipRectangle.IntersectsWith(rec)) continue;

                    var item = (ListPanelItem)Items[i];
                    var measuredItem = TextRenderer.MeasureText(item.Text, Font, rec.Size, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.WordBreak);
                    rec = new Rectangle(rec.Location, measuredItem);
                    item.Bounds = rec;

                    item.ItemState = SelectedIndices.Contains(i) ? ItemState.Selected : ItemState.Enabled;
                    DrawListItem?.Invoke(new ListPanelDrawItemEventArgs(g, Font, rec, i, item));
                    reg.Complement(rec);
                }
            }

            private void OnDrawListItem(ListPanelDrawItemEventArgs e)
            {
                if (e.Index < 0) return;

                var rec = e.Bounds;
                var g = e.Graphics;

                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                Color textColor = default;
                if (e.Index == HoverIndex)
                {
                    g.DrawRectangle(new Pen(Color.MediumSlateBlue, 1), rec);
                    g.FillRectangle(new SolidBrush(Colors[5]), rec);
                }

                textColor = e.Item.ItemState switch
                {
                    ItemState.Enabled => Colors[0],
                    ItemState.Selected => Colors[1],
                    ItemState.Disabled => Colors[2],
                    ItemState.SelectedDisabled => Colors[3],
                    ItemState.Invalid => Colors[4],
                    _ => textColor
                };
                
                TextRenderer.DrawText(g, GetItemText(Items[e.Index]), Font, e.Bounds, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.WordBreak);
            }

            protected override void OnDataSourceChanged(EventArgs e)
            {
                ItemsUpdated?.Invoke(this, e);
                base.OnDataSourceChanged(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                Invalidate();
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                HoverIndex = -1;
                base.OnMouseLeave(e);
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                var point = PointToClient(Cursor.Position);
                var index = IndexFromPoint(point);
                if (index < 0) return;
                var item = (ListPanelItem)Items[index];
                if (item.Bounds.Contains(point))
                {
                    HoverIndex = index;
                }
                else
                {
                    HoverIndex = -1;
                }
                base.OnMouseMove(e);
            }

            protected override void OnForeColorChanged(EventArgs e)
            {
                base.OnForeColorChanged(e);
                Invalidate();
            }

            protected override void OnMouseWheel(MouseEventArgs e)
            {
                base.OnMouseWheel(e);
                Invalidate();
            }

            private const int WmKillFocus = 0x8;
            private const int WmVScroll = 0x115;
            private const int WmHScroll = 0x114;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg != WmKillFocus &&
                    m.Msg is WmHScroll or WmVScroll)
                    Invalidate();
                base.WndProc(ref m);
            }
        }

        public class ListPanelItem
        {
            public Rectangle Bounds { get; set; }
            public int Index { get; set; }
            public IPower? Power { get; set; }
            public ItemState ItemState { get; set; }
            public string Text => Power.DisplayName;

            public ListPanelItem()
            {
                Power = new Power();
                ItemState = ItemState.Enabled;
            }

            public ListPanelItem(int index, IPower? power, ItemState state)
            {
                Index = index;
                Power = power;
                ItemState = state;
            }
        }

        #endregion
    }
}