using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace midsControls
{
    public partial class ctlLayeredBar : UserControl
    {
        private bool _EnableOverCap;
        private bool _EnableBaseValue;
        private bool _EnableOverlay1;
        private bool _EnableOverlay2;
        private Color _OverCapColor = Color.Magenta;
        private Color _BaseValueColor = Color.Magenta;
        private Color _BarColor = Color.Magenta;
        private Color _Overlay1Color = Color.Magenta;
        private Color _Overlay2Color = Color.Magenta;

        // https://stackoverflow.com/a/34299931
        protected override Size DefaultSize => new Size(277, 13);

        #region Properties
        // https://www.codeproject.com/Tips/403782/Making-an-overridden-Text-property-visible-in-the
        [Description("Enable over cap bar"), Category("Layout"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableOverCap
        {
            get => _EnableOverCap;
            set
            {
                panel1.Visible = value;
                _EnableOverCap = value;
            }
        }

        [Description("Enable base value bar"), Category("Layout"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableBaseValue
        {
            get => _EnableBaseValue;
            set
            {
                panel2.Visible = value;
                _EnableBaseValue = value;
            }
        }

        [Description("Enable overlay #1 bar"), Category("Layout"),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableOverlay1
        {
            get => _EnableOverlay1;
            set
            {
                panel4.Visible = value;
                _EnableOverlay1 = value;
            }
        }

        [Description("Enable overlay #2 bar"), Category("Layout"),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableOverlay2
        {
            get => _EnableOverlay2;
            set
            {
                panel5.Visible = value;
                _EnableOverlay2 = value;
            }
        }

        [Description("Minimum bar value"), Category("Data"),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float MinimumBarValue { get; set; }

        [Description("Maximum bar value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float MaximumBarValue { get; set; }

        [Description("Main bar value"), Category("Data"),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float BarValue { get; set; }

        [Description("Over cap value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float OverCapValue { get; set; }

        [Description("Base value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float BaseValue { get; set; }

        [Description("Overlay #1 value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Overlay1Value { get; set; }

        [Description("Overlay #2 value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float Overlay2Value { get; set; }

        [Description("Over cap bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color OverCapColor
        {
            get => _OverCapColor;
            set
            {
                panel1.BackColor = value;
                _OverCapColor = value;
            }
        }

        [Description("Base value bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BaseValueColor
        {
            get => _BaseValueColor;
            set
            {
                panel2.BackColor = value;
                _BaseValueColor = value;
            }
        }

        [Description("Main bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BarColor
        {
            get => _BarColor;
            set
            {
                panel3.BackColor = value;
                _BarColor = value;
            }
        }

        [Description("Overlay #1 bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color Overlay1Color
        {
            get => _Overlay1Color;
            set
            {
                panel4.BackColor = value;
                _Overlay1Color = value;
            }
        }

        [Description("Overlay #2 bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color Overlay2Color
        {
            get => _Overlay2Color;
            set
            {
                panel5.BackColor = value;
                _Overlay2Color = value;
            }
        }
        #endregion

        private int Value2Pixels(float value)
        {
            return (int)Math.Round(Width / Math.Abs(MaximumBarValue - MinimumBarValue) * (value - MinimumBarValue));
        }

        private int Value2Pixels(float value, float vMax)
        {
            return (int)Math.Round(Width / Math.Abs(vMax - MinimumBarValue) * (value - MinimumBarValue));
        }

        public void SetValues(float value)
        {
            panel3.Width = Value2Pixels(value);
        }

        public void SetValues(float mainValue, float auxValue)
        {
            if (EnableOverCap)
            {
                // auxValue: overCapValue
                if (auxValue > mainValue)
                {
                    float relativeMax = Math.Max(MaximumBarValue, auxValue);
                    panel1.Width = Value2Pixels(auxValue, relativeMax);
                    panel3.Width = Value2Pixels(mainValue, relativeMax);
                }
                else
                {
                    panel1.Width = panel3.Width = Value2Pixels(mainValue);
                }
            }

            // http://csharphelper.com/blog/2014/08/change-control-stacking-order-in-c/
            if (EnableBaseValue)
            {
                // auxValue: baseValue
                if (auxValue < mainValue)
                {
                    panel2.Parent.Controls.SetChildIndex(panel2, 2);
                    panel3.Parent.Controls.SetChildIndex(panel3, 1);
                }
                else
                {
                    panel2.Parent.Controls.SetChildIndex(panel2, 1);
                    panel3.Parent.Controls.SetChildIndex(panel3, 2);
                }

                panel2.Width = Value2Pixels(auxValue);
                panel3.Width = Value2Pixels(mainValue);
            }
        }

        public void SetValues(float mainValue, float baseValue, float uncappedValue)
        {
            if (uncappedValue > mainValue)
            {
                float relativeMax = Math.Max(MaximumBarValue, uncappedValue);
                panel1.Width = Value2Pixels(uncappedValue, relativeMax);
                panel3.Width = Value2Pixels(mainValue, relativeMax);
            }
            else
            {
                panel1.Width = Value2Pixels(mainValue);
                panel3.Width = Value2Pixels(mainValue);
            }

            panel1.Parent.Controls.SetChildIndex(panel1, 1);

            if (baseValue < mainValue)
            {
                panel2.Parent.Controls.SetChildIndex(panel2, 3);
                panel3.Parent.Controls.SetChildIndex(panel3, 2);
            }
            else
            {
                panel2.Parent.Controls.SetChildIndex(panel2, 2);
                panel3.Parent.Controls.SetChildIndex(panel3, 3);
            }

            panel2.Width = Value2Pixels(baseValue);
        }

        public void SetValues(float mainValue, float baseValue, float uncappedValue, float overlay1Value)
        {
            SetValues(mainValue, baseValue, uncappedValue);

            panel4.Parent.Controls.SetChildIndex(panel4, 4);
            panel4.Width = Value2Pixels(overlay1Value);
        }

        public void SetValues(float mainValue, float baseValue, float uncappedValue, float overlay1Value, float overlay2Value)
        {
            SetValues(mainValue, baseValue, uncappedValue, overlay1Value);

            panel4.Parent.Controls.SetChildIndex(panel5, 5);
            panel5.Width = Value2Pixels(overlay2Value);
        }

        public ctlLayeredBar()
        {
            InitializeComponent();
        }
    }
}