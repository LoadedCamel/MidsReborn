using System;
using System.Drawing;
using System.Windows.Forms;
//using MaterialSkin;
//using MaterialSkin.Controls;

namespace Mids_Reborn.UIv2.Controls
{
    public partial class PowerPanel : UserControl
    {
        // Controls
        //private MaterialPanel _scrollPanel;
        private TableLayoutPanel _mainGrid;
        private TableLayoutPanel _inherentGrid;


        // Columns and Rows Calculated
        private const int MainPowers = 24;
        public int Columns { get; set; } = 3;
        private int Rows => MainPowers / Columns;

        public PowerPanel()
        {
            InitializeComponent();
            // _scrollPanel = new MaterialPanel
            // {
            //     BackColor = Color.Transparent,
            //     AccentColor = MaterialSkinManager.Instance.ColorScheme.AccentColor,
            //     Dock = DockStyle.Fill,
            //     AutoScroll = true
            // };

            _mainGrid = new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.Top,
                ColumnCount = Columns,
                RowCount = Rows
            };

            _inherentGrid = new TableLayoutPanel
            {
                BackColor = Color.Transparent,
                Dock = DockStyle.None,
                ColumnCount = Columns,
                GrowStyle = TableLayoutPanelGrowStyle.AddRows
            };
            // _scrollPanel.Controls.Add(_mainGrid);
            // _scrollPanel.Controls.Add(_inherentGrid);
            // Controls.Add(_scrollPanel);
            AdjustGrid();
        }

        private void AdjustGrid()
        {
            AdjustCellSizes();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AdjustGrid();
        }

        private void AdjustCellSizes()
        {
            var spacing = CalculatedSpacing();
    
            // Validate the width to ensure it's positive after subtracting paddings
            var totalPadding = (Columns + 1) * spacing;
            //var availableWidth = _scrollPanel.ClientSize.Width - totalPadding;
    
            // Ensure cellWidth cannot be negative; set a minimum cell width if necessary
            //var cellWidth = Math.Max(availableWidth / Columns, 10); // Assuming 10 as a reasonable minimum width
            //var cellHeight = cellWidth; // Maintain aspect ratio

            _mainGrid.ColumnStyles.Clear();
            _mainGrid.RowStyles.Clear();
            _mainGrid.Padding = new Padding(spacing);
    
            for (var col = 0; col < Columns; col++)
            {
                //_mainGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellWidth));
            }
    
            for (var row = 0; row < Rows; row++)
            {
                //_mainGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, cellHeight));
            }

            _inherentGrid.ColumnStyles.Clear();
            _inherentGrid.RowStyles.Clear();
            _inherentGrid.Padding = new Padding(spacing);
            for (var col = 0; col < Columns; col++)
            {
                //_inherentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, cellWidth));
            }

            // Adjust the heights of the grids based on their row counts and spacing
            //_mainGrid.Height = Rows * (cellHeight + spacing) + spacing;
           // _inherentGrid.Height = _inherentGrid.RowCount * (cellHeight + spacing) + spacing;

            // Manually position the inherent grid below the main grid
            _inherentGrid.Location = new Point(0, _mainGrid.Bottom + spacing);
        }

        private int CalculatedSpacing()
        {
            // Get dimensions of the control
            //var width = _scrollPanel.ClientSize.Width;
            //var height = _scrollPanel.ClientSize.Height;

            // Define minimum and maximum spacing values
            const int minSpacing = 5;
            const int maxSpacing = 20;

            // Calculate a factor based on control size
            //var sizeFactor = Math.Min(width, height) / 100.0;

            // Adjust spacing based on size factor within defined range
            //var spacing = (int)Math.Max(minSpacing, Math.Min(maxSpacing, sizeFactor * 10));

            //return spacing;
            return 0;
        }
    }
}
