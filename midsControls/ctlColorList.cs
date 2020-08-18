using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace midsControls
{
    public sealed partial class ctlColorList : ListBox
    {
        public ctlColorList()
        {
            InitializeComponent();
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        public List<Color> Colors { get; set; }

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

            /*IntPtr hdc = g.GetHdc();
            DrawThemeParentBackground(Handle, hdc, ref rec);
            g.ReleaseHdc(hdc);*/

            using var reg = new Region(e.ClipRectangle);
            if (Items.Count > 0)
                for (var i = 0; i < Items.Count; i++)
                {
                    rec = GetItemRectangle(i);

                    if (e.ClipRectangle.IntersectsWith(rec))
                    {
                        if (SelectionMode == SelectionMode.One && SelectedIndex == i ||
                            SelectionMode == SelectionMode.MultiSimple && SelectedIndices.Contains(i) ||
                            SelectionMode == SelectionMode.MultiExtended && SelectedIndices.Contains(i))
                            OnDrawItem(new DrawItemEventArgs(g, Font, rec, i, DrawItemState.Selected, ForeColor,
                                BackColor));
                        else
                            OnDrawItem(new DrawItemEventArgs(g, Font, rec, i, DrawItemState.Default, ForeColor,
                                BackColor));

                        reg.Complement(rec);
                    }
                }
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            if (e.Index == 2 || e.Index == 5)
            {
                using Brush backBrush = new SolidBrush(Colors[e.Index]);
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                TextRenderer.DrawText(e.Graphics, GetItemText(Items[e.Index]), e.Font, e.Bounds, Colors[e.Index - 2],
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, GetItemText(Items[e.Index]), e.Font, e.Bounds, Colors[e.Index],
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
            }
        }
    }
}