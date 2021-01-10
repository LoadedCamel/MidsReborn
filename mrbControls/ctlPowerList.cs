using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using mrbBase;
using mrbBase.Base.Data_Classes;
using static mrbControls.ctlPowerList;
using static mrbControls.PowerListDrawItemEventArgs;
using ContentAlignment = System.Drawing.ContentAlignment;

namespace mrbControls
{
    public sealed partial class ctlPowerList : ListBox
    {
        public delegate void DrawPowerListItem(PowerListDrawItemEventArgs e);
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
        public ctlPowerList()
        {
            SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawListItem += OnDrawListItem;
            InitializeComponent();
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
                            OnDrawListItem(new PowerListDrawItemEventArgs(g, Font, rec, i, ItemState.Selected, Colors[1], BackColor));
                        }
                        else
                        {
                            OnDrawListItem(new PowerListDrawItemEventArgs(g, Font, rec, i, ItemState.Enabled, Colors[0], BackColor));
                        }

                        reg.Complement(rec);
                    }
                }
            }
        }

        private void OnDrawListItem(PowerListDrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var rec = e.Bounds;
            var g = e.Graphics;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

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
                Colors[(int) state] = color;
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

    public class PowerListItem
    {
        public IPower Power { get; set; }
        public int Index { get; set; }
        public int ItemHeight { get; set; }
        public ItemState ItemState { get; }
        public string Text => Power.DisplayName;
        public ItemAlign TextAlign { get; set; }

        public enum ItemAlign
        {
            Left,
            Right,
            Center
        }

        public PowerListItem()
        {
            Power = new Power();
            Index = -1;
            ItemHeight = 1;
            ItemState = ItemState.Enabled;
            TextAlign = ItemAlign.Left;
        }

        public PowerListItem(IPower power, int index, ItemState state, ItemAlign alignment)
        {
            Power = power;
            Index = index;
            ItemState = state;
            TextAlign = alignment;
        }
    }
    public partial class PowerListDrawItemEventArgs : EventArgs
    {
        public Graphics Graphics { get; }
        public Font Font { get; }
        public Rectangle Bounds { get; }
        public int Index { get; }
        public ctlPowerList.ItemState State { get; }
        public Color ForeColor { get; }
        public Color BackColor { get; }

        public PowerListDrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, ItemState state, Color foreColor, Color backColor)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            State = state;
            ForeColor = foreColor;
            BackColor = backColor;
        }
        public PowerListDrawItemEventArgs(Graphics graphics, Font font, Rectangle rect, int index, ItemState state)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            State = state;
        }
    }
}
