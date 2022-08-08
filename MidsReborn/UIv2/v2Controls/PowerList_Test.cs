using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.UIv2.v2Controls
{
    public partial class PowerList_Test : UserControl
    {
        #region SubControl Declarations

        private readonly OutlinedLabel _outlinedLabel = new OutlinedLabel();
        private readonly TypeDropDown _typeDropDown = new TypeDropDown();
        private readonly Panel _separatorPanel = new Panel();
        private readonly ListPanel _listPanel = new ListPanel();

        #endregion

        #region Events

        private new event EventHandler<string> TextChanged;
        private event EventHandler<TypeDropDown.DropDownType> TypeChanged;
        public static event EventHandler<int> OutlineWidthChanged;
        public static event EventHandler<Color> OutlineColorChanged;

        #endregion

        #region Designer Properties (HIDDEN)

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Image BackgroundImage { get; set; }

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

        private TypeDropDown.DropDownType _dropDownType;
        private string _text;

        #endregion

        #region Designer Properties (VISIBLE)

        [Description("The background color of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color BackColor { get; set; } = Color.Transparent;

        
        [Description("The background color of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TypeDropDown.DropDownType DropDownType
        {
            get => _dropDownType;
            set
            {
                _dropDownType = value;
                TypeChanged?.Invoke(this, value);
            }
        }

        [Description("The font to be used on the label of the control.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public new Font Font { get; set; } = new Font("Arial", 9.25f);

        [Description("The color of the text used for the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override Color ForeColor { get; set; } = Color.White;

        [Description("Determines the appearance of the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [SettingsBindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [NotifyParentProperty(true)]
        public PowerListOutline Outline { get; set; } = new PowerListOutline{Color = Color.Black, Width = 2, Enabled = false};

        [Description("The text to be displayed as the control label.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
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

        #endregion

        #region Designer Properties TypeConverters and Classes

        public class PowerListOutlineTypeConverter : TypeConverter
        {
            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return TypeDescriptor.GetProperties(typeof(PowerListOutline));
            }
        }

        [TypeConverter(typeof(PowerListOutlineTypeConverter))]
        public class PowerListOutline
        {
            private int _width;

            [Description("Enables the controls label text to be outlined.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public bool Enabled { get; set; } = true;

            [Description("Sets the color of outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public Color Color { get; set; } = Color.Black;

            [Description("Sets the width of the outline.")]
            [Browsable(true)]
            [EditorBrowsable(EditorBrowsableState.Always)]
            [Bindable(true)]
            [NotifyParentProperty(true)]
            public int Width
            {
                get => _width;
                set
                {
                    _width = value;
                    OutlineWidthChanged?.Invoke(this, value);
                }
            }

            public override string ToString()
            {
                return $"{Enabled}, {Color}, {Width}";
            }
        }

        #endregion

        #region Constructor

        public PowerList_Test()
        {
            SuspendLayout();
            _outlinedLabel.Location = new Point(0, 0);
            _outlinedLabel.Size = new Size(83, 16);
            _outlinedLabel.Dock = DockStyle.Top;
            _outlinedLabel.BackColor = Color.Transparent;
            _outlinedLabel.ForeColor = Color.White;
            _outlinedLabel.Font = new Font("Arial", 9.25f);
            _outlinedLabel.Text = _text;
            _outlinedLabel.OutlineColor = Color.Black;
            _outlinedLabel.OutlineEnabled = false;
            _outlinedLabel.OutlineWidth = 2;

            _typeDropDown.Location = new Point(0, 16);
            _typeDropDown.Size = new Size(211, 21);
            _typeDropDown.Dock = DockStyle.Top;
            _typeDropDown.DrawMode = DrawMode.OwnerDrawFixed;
            _typeDropDown.FormattingEnabled = true;
            _typeDropDown.HighlightColor = Color.Empty;
            _typeDropDown.DropType = TypeDropDown.DropDownType.None;

            _separatorPanel.Location = new Point(0, 37);
            _separatorPanel.Size = new Size(211, 8);
            _separatorPanel.Dock = DockStyle.Top;
            _separatorPanel.Enabled = false;
            _separatorPanel.BorderStyle = BorderStyle.None;
            _separatorPanel.BackColor = Color.Transparent;

            _listPanel.Location = new Point(0, 45);
            _listPanel.Size = new Size(211, 169);
            _listPanel.Dock = DockStyle.Top;
            _listPanel.BorderStyle = BorderStyle.None;
            _listPanel.DrawMode = DrawMode.OwnerDrawFixed;
            _listPanel.SelectionBackColor = Color.DarkOrange;
            _listPanel.SelectionColor = Color.Empty;
            _listPanel.FormattingEnabled = true;

            Controls.Add(_listPanel);
            Controls.Add(_separatorPanel);
            Controls.Add(_typeDropDown);
            Controls.Add(_outlinedLabel);
            ResumeLayout(false);
            PerformLayout();

            OutlineWidthChanged += OnOutlineWidthChanged;
            TextChanged += OnTextChanged;
            TypeChanged += OnTypeChanged;
            InitializeComponent();
        }

        #endregion

        #region EventHandler Methods

        private void OnOutlineWidthChanged(object sender, int e)
        {
            _outlinedLabel.OutlineWidth = e;
            _outlinedLabel.Invalidate();
        }

        private void OnTypeChanged(object sender, TypeDropDown.DropDownType e)
        {
            _typeDropDown.DropType = e;
            _typeDropDown.Invalidate();
        }

        private void OnTextChanged(object sender, string e)
        {
            _outlinedLabel.Text = e;
            _outlinedLabel.Invalidate();
        }

        #endregion
    }

    #region SubControls

    internal class OutlinedLabel : Label
    {
        public override Color ForeColor { get; set; } = Color.Azure;
        public override Color BackColor { get; set; } = Color.Transparent;
        public override ContentAlignment TextAlign { get; set; } = ContentAlignment.MiddleLeft;
        public Color OutlineColor { get; set; } = Color.Blue;
        public float OutlineWidth { get; set; } = 2;
        public bool OutlineEnabled { get; set; } = true;

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

    public class TypeDropDown : ComboBox
    {
        public enum DropDownType
        {
            None,
            Archetype,
            Origin,
            Primary,
            Secondary,
            Pool,
            Ancillary
        }

        public DropDownType DropType { get; set; }
        public Color HighlightColor { get; set; }

        public TypeDropDown()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            DropDownStyle = ComboBoxStyle.DropDownList;
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0 && e.Index < Items.Count)
            {
                e.Graphics.FillRectangle((e.State & DrawItemState.Selected) == DrawItemState.Selected ? new SolidBrush(HighlightColor) : new SolidBrush(BackColor), e.Bounds);
                Image comboImage = null;
                string imagesFolder;
                List<string> images;
                switch (DropType)
                {
                    case DropDownType.Archetype:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Archetypes";
                        //var archetype = (Archetype) Items[e.Index];
                        //comboImage = archetype.Image(Directory.GetFiles(imagesFolder).ToList());
                        break;
                    case DropDownType.Origin:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Origins";
                        //images = Directory.GetFiles(imagesFolder).ToList();
                        //comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(Items[e.Index].ToString())) ?? throw new InvalidOperationException());
                        break;
                    case DropDownType.Primary:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var primary = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(primary.ImageName)) ?? throw new InvalidOperationException());
                        break;
                    case DropDownType.Secondary:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var secondary = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(secondary.ImageName)) ?? throw new InvalidOperationException());
                        break;
                    case DropDownType.Pool:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var pool = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(pool.ImageName)) ?? throw new InvalidOperationException());
                        break;
                    case DropDownType.Ancillary:
                        imagesFolder = $"{Application.StartupPath}\\Images\\Powersets";
                        images = Directory.GetFiles(imagesFolder).ToList();
                        var ancillary = (IPowerset)Items[e.Index];
                        comboImage = Image.FromFile(images.FirstOrDefault(i => i.Contains(ancillary.ImageName)) ?? throw new InvalidOperationException());
                        break;
                }

                if (comboImage != null)
                {
                    e.Graphics.DrawImage(comboImage, e.Bounds.Left, e.Bounds.Top);
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left + comboImage.Width + 5, e.Bounds.Top);
                }
                else
                {
                    e.Graphics.DrawString(GetItemText(Items[e.Index]), e.Font, new SolidBrush(e.ForeColor), e.Bounds.Left, e.Bounds.Top);
                }
            }

            base.OnDrawItem(e);
        }
    }

    public sealed class ListPanel : ListBox
    {
        public delegate void DrawPowerListItem(ListPanelDrawItemEventArgs e);
        public event DrawPowerListItem DrawListItem;

        public Color SelectionColor { get; set; }
        public Color SelectionBackColor { get; set; } = Color.DarkOrange;
        public override Color ForeColor { get; set; }
        private List<Color> Colors { get; set; } = new List<Color> { Color.Gold, Color.DodgerBlue, Color.LightGray, Color.DarkBlue, Color.Red };

        [Flags]
        public enum ItemState
        {
            Enabled = 0,
            Selected = 1,
            Disabled = 2,
            SelectedDisabled = 3,
            Invalid = 4
        }

        public ListPanel()
        {
            SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawListItem += OnDrawListItem;
        }

        [DllImport("uxtheme", ExactSpelling = true)]
        private static extern int DrawThemeParentBackground(
            IntPtr hWnd,
            IntPtr hdc,
            ref Rectangle pRect
        );

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var rec = ClientRectangle;

            IntPtr hdc = g.GetHdc();
            DrawThemeParentBackground(Handle, hdc, ref rec);
            g.ReleaseHdc(hdc);

            using Region reg = new Region(e.ClipRectangle);
            if (Items.Count > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    rec = GetItemRectangle(i);

                    if (e.ClipRectangle.IntersectsWith(rec))
                    {
                        if (SelectionMode == SelectionMode.One && SelectedIndex == i || SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i) || SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i))
                        {
                            OnDrawListItem(new ListPanelDrawItemEventArgs(g, Font, rec, i, ItemState.Selected, Colors[1], BackColor));
                        }
                        else
                        {
                            OnDrawListItem(new ListPanelDrawItemEventArgs(g, Font, rec, i, ItemState.Enabled, Colors[0], BackColor));
                        }

                        reg.Complement(rec);
                    }
                }
            }
        }

        private void OnDrawListItem(ListPanelDrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var rec = e.Bounds;
            var g = e.Graphics;

            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            /*var textColor = ForeColor;
            if ((e.State & ItemState.Selected) == ItemState.Selected)
            {
                textColor = SelectionColor;
            }*/
            Color textColor = default;
            switch (e.State)
            {
                case ItemState.Enabled:
                    textColor = Colors[0];
                    break;
                case ItemState.Selected:
                    textColor = Colors[1];
                    break;
                case ItemState.Disabled:
                    textColor = Colors[2];
                    break;
                case ItemState.SelectedDisabled:
                    textColor = Colors[3];
                    break;
                case ItemState.Invalid:
                    textColor = Colors[4];
                    break;
            }

            TextRenderer.DrawText(g, GetItemText(Items[e.Index]), Font, rec, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
        }

        public void UpdateTextColors(ItemState state, Color color)
        {
            if ((state < ItemState.Enabled) | (state > ItemState.Invalid))
                return;
            if (Colors != null)
            {
                Colors[(int)state] = color;
            }
        }


        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Invalidate();
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

        private const int WM_KILLFOCUS = 0x8;
        private const int WM_VSCROLL = 0x115;
        private const int WM_HSCROLL = 0x114;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg != WM_KILLFOCUS &&
                (m.Msg == WM_HSCROLL || m.Msg == WM_VSCROLL))
                Invalidate();
            base.WndProc(ref m);
        }
    }

    public class ListPanelItem
    {
        public IPower Power { get; set; }
        public int Index { get; set; }
        public int ItemHeight { get; set; }
        public ListPanel.ItemState ItemState { get; }
        public string Text => Power.DisplayName;
        public ItemAlign TextAlign { get; set; }

        public enum ItemAlign
        {
            Left,
            Right,
            Center
        }

        public ListPanelItem()
        {
            Power = new Power();
            Index = -1;
            ItemHeight = 1;
            ItemState = ListPanel.ItemState.Enabled;
            TextAlign = ItemAlign.Left;
        }

        public ListPanelItem(IPower power, int index, ListPanel.ItemState state, ItemAlign alignment)
        {
            Power = power;
            Index = index;
            ItemState = state;
            TextAlign = alignment;
        }
    }
    public class ListPanelDrawItemEventArgs : EventArgs
    {
        public Graphics Graphics { get; }
        public Font Font { get; }
        public Rectangle Bounds { get; }
        public int Index { get; }
        public ListPanel.ItemState State { get; }
        public Color ForeColor { get; }
        public Color BackColor { get; }

        public ListPanelDrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, ListPanel.ItemState state, Color foreColor, Color backColor)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            State = state;
            ForeColor = foreColor;
            BackColor = backColor;
        }
        public ListPanelDrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, ListPanel.ItemState state)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            State = state;
        }
    }

    #endregion
}
