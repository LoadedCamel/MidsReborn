using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace midsControls
{
    public sealed partial class ctlColorList : ListBox
    {
        public readonly Color[] Colors;
        public ctlColorList()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer|ControlStyles.ResizeRedraw|ControlStyles.UserPaint|ControlStyles.SupportsTransparentBackColor, true);
            DrawMode = DrawMode.OwnerDrawVariable;
            InitializeComponent();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            TextRenderer.DrawText(e.Graphics, e.Index.ToString(), e.Font, new Point(e.Bounds.X, e.Bounds.Y), Colors[e.Index]);
        }
    }
}
