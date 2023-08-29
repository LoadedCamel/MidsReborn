using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.Controls
{
    public partial class Canvas : UserControl
    {
        private record Pad
        {
            public int X = 5;
            public int Y = 10;
        }

        public List<EntryItem>? Entries { get; set; }
        public override Font Font { get; set; } = new(Fonts.Family(@"Noto Sans"), 12.25f, FontStyle.Bold);

        private List<Image>? _powerButtons;
        private Image? _slotImage;
        private Pad _padding = new();

        #region Column/Row Properties

        private const int TotalPowers = 24;
        private int _columns;
        private int _rows;

        #endregion

        private Size _powerSize;
        private Size _slotSize;

        private Size DrawingArea => ClientRectangle.Size;

        public Canvas()
        {
            InitializeVariables();
            InitializeComponent();
        }

        private async void InitializeVariables()
        {
            _columns = 3;
            _rows = TotalPowers / _columns;

            _powerButtons = new List<Image>();
            var powerButtons = await I9Gfx.LoadButtons();
            foreach (var buttonPath in powerButtons)
            {
                _powerButtons.Add(Image.FromFile(buttonPath));
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Call base paint 
            base.OnPaint(e);

            // Clear the canvas
            e.Graphics.Clear(Color.Transparent);

            // Set graphics quality
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            
            // Fill client area with BackColor
            var backBrush = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(backBrush, ClientRectangle);

            // Check if Entries is null
            if (Entries == null)
            {
                return;
            }

            // Main drawing logic
            var rect = ClientRectangle;
            var columnWidth = rect.Width / _columns;
            var rowHeight = rect.Height / _rows;

            for (var columnIndex = 0; columnIndex < _columns; columnIndex++)
            {
                for (var rowIndex = 0; rowIndex < _rows; rowIndex++)
                {
                    e.Graphics.DrawRectangle(Pens.Black, columnIndex * columnWidth, rowIndex * rowHeight, columnWidth, rowHeight);
                }
            }

        }

        public class EntryItem
        {
            public int Column { get; set; }
            public int Row { get; set; }
            public Rectangle Bounds { get; set; }
            public bool IsHighlightable { get; set; }
            

            public EntryItem(int column, int row, Rectangle bounds)
            {
                Column = column;
                Row = row;
                Bounds = bounds;
            }
        }
    }
}
