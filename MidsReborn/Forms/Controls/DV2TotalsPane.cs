using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using mrbControls;
using Syncfusion.Drawing;

namespace Mids_Reborn.Forms.Controls
{
    public partial class DV2TotalsPane : UserControl
    {
        private List<Item> Items;
        private Color _BackgroundColorEnd;
        private int VisibleItemsCount => Math.Min(Items.Count, MaxItems);
        private const int BarHeight = 10;
        private const float LabelsFontSize = 7;

        [Description("Maximum visible items"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int MaxItems;

        [Description("Maximum global bar value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GlobalMaxValue;
        
        [Description("Bar stat group"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Group;

        [Description("Bars background gradient end color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackgroundColorEnd
        {
            get => _BackgroundColorEnd;
            set
            {
                _BackgroundColorEnd = value;
                gradientPanel1.BackgroundColor = new BrushInfo(GradientStyle.Horizontal,Color.Black, _BackgroundColorEnd);
            }
        }

        [Description("Bars color (main value)"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BarColorMain;
        
        [Description("Bars color (uncapped value)"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BarColorUncapped;
        
        [Description("Enable uncapped values (dual bars)"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableUncappedValues;

        public DV2TotalsPane()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            InitializeComponent();
        }

        public void ClearItems()
        {
            Items = new List<Item>();
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        private void DrawBars()
        {
            gradientPanel1.Controls.Clear();
            for (var i = 0; i < VisibleItemsCount; i++)
            {
                var bar = new ctlLayeredBarPb
                {
                    MinimumBarValue = 0,
                    MaximumBarValue = GlobalMaxValue,
                    Size = new Size(gradientPanel1.Width - 2, BarHeight),
                    Location = new Point(1, (1 + i) * BarHeight),
                    Group = Group
                };

                if (EnableUncappedValues)
                {
                    bar.AssignNames(new List<(string name, Color color)>
                    {
                        (name: "value", color: BarColorMain),
                        (name: "uncapped", color: BarColorUncapped)
                    });

                    bar.AssignValues(new List<(string name, float value)>
                    {
                        (name: "value", value: Items[i].Value),
                        (name: "uncapped", value: Items[i].UncappedValue)
                    });
                }
                else
                {
                    bar.AssignNames(new List<(string name, Color color)>
                    {
                        (name: "value", color: BarColorMain)
                    });

                    bar.AssignValues(new List<(string name, float value)>
                    {
                        (name: "value", value: Items[i].Value)
                    });
                }

                gradientPanel1.Controls.Add(bar);
            }
        }

        private void DrawLabels()
        {
            var grid = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = Math.Min(Items.Count, MaxItems),
                BackColor = Color.Black,
                Dock = DockStyle.Fill
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            for (var i = 0; i < VisibleItemsCount; i++)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Percent, 12));
                var labelTag = new Label
                {
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Text = $"{Items[i].Name}:",
                    Font = new Font(FontFamily.GenericSansSerif, LabelsFontSize, FontStyle.Regular, GraphicsUnit.Pixel)
                };

                var labelValue = new Label
                {
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleRight,
                    Dock = DockStyle.Fill,
                    AutoSize = false,
                    Text = $"{Items[i].Value:####.##}{(Items[i].DisplayPercentage ? "%" : "")}",
                    Font = new Font(FontFamily.GenericSansSerif, LabelsFontSize, FontStyle.Regular, GraphicsUnit.Pixel)
                };

                grid.Controls.Add(labelTag, 0, i);
                grid.Controls.Add(labelValue, 1, i);
            }

            panel1.Controls.Clear();
            panel1.Controls.Add(grid);
        }

        public void Draw()
        {
            DrawLabels();
            DrawBars();
        }

        #region Table label sub-class

        public class Item
        {
            private float _Value;
            private float _UncappedValue;

            public string Name { get; set; }
            public float Value => DisplayPercentage ? _Value * 100 : _Value;
            public float UncappedValue => DisplayPercentage ? _UncappedValue * 100 : _UncappedValue;
            public bool DisplayPercentage { get; set; }

            public Item(string name, float value, float uncappedValue, bool displayPercentage)
            {
                Name = name;
                _Value = value;
                _UncappedValue = uncappedValue;
                DisplayPercentage = displayPercentage;
            }
        }

        #endregion

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Draw();
        }
    }
}