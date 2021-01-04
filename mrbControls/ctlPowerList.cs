using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace mrbControls
{
    public sealed partial class ctlPowerList : ListBox
    {
        public ctlPowerList()
        {
            InitializeComponent();
            SetStyle(ControlStyles.Opaque|ControlStyles.AllPaintingInWmPaint|ControlStyles.ResizeRedraw|ControlStyles.OptimizedDoubleBuffer|ControlStyles.UserPaint|ControlStyles.SupportsTransparentBackColor,true);
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        public enum ItemState
        {
            Enabled,
            Selected,
            Disabled,
            SelectedDisabled,
            Invalid
        }

        public Color SelectionBackColor { get; set; } = Color.DarkOrange;

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
                            OnDrawItem(new DrawItemEventArgs(g, Font, rec, i, DrawItemState.Selected, ForeColor, BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(g, Font, rec, i, DrawItemState.Default, ForeColor, BackColor));
                        }

                        reg.Complement(rec);
                    }
                }
            }
        }

        public Color SelectionColor { get; set; }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            var rec = e.Bounds;
            var g = e.Graphics;

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            var textColor = ForeColor;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                textColor = SelectionColor;
            }

            TextRenderer.DrawText(g, GetItemText(Items[e.Index]), Font, rec, textColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
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
}
