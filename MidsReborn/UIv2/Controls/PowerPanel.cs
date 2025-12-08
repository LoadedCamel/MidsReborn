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
            _mainGrid.ColumnStyles.Clear();
            _mainGrid.RowStyles.Clear();
            _mainGrid.Padding = new Padding(0);

            _inherentGrid.ColumnStyles.Clear();
            _inherentGrid.RowStyles.Clear();
            _inherentGrid.Padding = new Padding(0);

            // Manually position the inherent grid below the main grid
            _inherentGrid.Location = new Point(0, _mainGrid.Bottom);
        }
    }
}
