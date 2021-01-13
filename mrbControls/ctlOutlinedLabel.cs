using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace mrbControls
{
    public partial class ctlOutlinedLabel : Label
    {
        public ctlOutlinedLabel()
        {
            InitializeComponent();
            OutlineForeColor = Color.Black;
            OutlineWidth = 2;
        }
        public Color OutlineForeColor { get; set; }
        public float OutlineWidth { get; set; }
        public sealed override ContentAlignment TextAlign { get; set; }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
            using GraphicsPath gp = new GraphicsPath();
            using Pen outline = new Pen(OutlineForeColor, OutlineWidth) { LineJoin = LineJoin.Round };
            using StringFormat sf = new StringFormat();
            using Brush foreBrush = new SolidBrush(ForeColor);
            gp.AddString(Text, Font.FontFamily, (int)Font.Style, Font.Size, ClientRectangle, sf);
            e.Graphics.ScaleTransform(1.3f, 1.35f);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(outline, gp);
            e.Graphics.FillPath(foreBrush, gp);
        }
    }
}
