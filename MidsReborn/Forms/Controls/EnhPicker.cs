using Mids_Reborn.Core.Base.Master_Classes;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public sealed partial class EnhPicker : UserControl
    {
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

        #region Private Properties

        private Color _borderColor = Color.FromArgb(12, 56, 100);

        #endregion


        public override Font Font { get; set; } = new(@"Microsoft Sans Serif", 10.25f, FontStyle.Bold);

        [Description("The border color of the component and it's cells.")]
        [Category("Appearance")]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
            }
        }

        internal class Cells
        {
            public Rectangle Title { get; set; }
            public Rectangle EnhType { get; set; }
            public Rectangle EnhSubType { get; set; }
            public Rectangle Enhancements { get; set; }
            public Rectangle Description { get; set; }
            public Rectangle Level { get; set; }
        }

        private Cells Cell { get; set; }

        public EnhPicker()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ContainerControl | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            MinimumSize = new Size(300, 300);
            Cell = new Cells();
            InitializeComponent();

        }

        private void DrawLayout(Graphics gfx)
        {
            var client = ClientRectangle;
            using var pen = new Pen(Color.FromArgb(12, 56, 100), 2);
            pen.Color = MidsContext.Character?.IsHero() switch
            {
                false => Color.FromArgb(100, 0, 0),
                _ => pen.Color
            };

            var textMeasurement = gfx.MeasureString("Enhancement Picker", Font);

            Cell.Title = new Rectangle(client.X + 2, client.Y - 2, client.Width - 4, (int)textMeasurement.Height + 15);
            Cell.EnhType = new Rectangle(client.X + 2, Cell.Title.Bottom, client.Width - 4, 46);
            Cell.EnhSubType = new Rectangle(client.Right - 48, Cell.EnhType.Bottom, 46, client.Height - 38);
            Cell.Enhancements = new Rectangle(client.X + 2, Cell.EnhType.Bottom, Cell.EnhSubType.Left - 3, client.Height - 38);
            Cell.Description = new Rectangle(client.X + 2, client.Bottom - 38, client.Width - Cell.EnhSubType.Width - 4, client.Height - 2);
            Cell.Level = new Rectangle(client.Right - 48, client.Bottom - 38, 46, client.Height - 2);


            gfx.DrawRectangle(pen, Cell.Title);
            gfx.DrawRectangle(pen, Cell.EnhType);
            gfx.DrawRectangle(pen, Cell.EnhSubType);
            gfx.DrawRectangle(pen, Cell.Enhancements);
            gfx.DrawRectangle(pen, Cell.Description);
            gfx.DrawRectangle(pen, Cell.Level);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            DrawLayout(e.Graphics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }
    }
}
