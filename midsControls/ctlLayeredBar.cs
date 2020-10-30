using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace midsControls
{
    public partial class ctlLayeredBar : UserControl
    {
        private bool _DesignMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        private bool _CanUpdate = true;
        private bool _EnableOverCap;
        private bool _EnableBaseValue;
        private bool _EnableOverlay1;
        private bool _EnableOverlay2;
        private float _ValueUncapped;
        private float _ValueBase;
        private float _Value = 100;
        private float _ValueOverlay1;
        private float _ValueOverlay2;
        private float _MinimumValue;
        private float _MaximumValue = 100;
        private string _Tip = "";
        //private ulong _AnimDuration = 1000;
        private Color _OverCapColor = Color.Magenta;
        private Color _BaseValueColor = Color.Magenta;
        private Color _BarColor = Color.Magenta;
        private Color _Overlay1Color = Color.Magenta;
        private Color _Overlay2Color = Color.Magenta;

        // https://stackoverflow.com/a/34299931
        // https://stackoverflow.com/questions/51597919/c-sharp-winform-stop-control-property-setting-to-default-when-it-is-set-to-be-a
        protected override Size DefaultSize => new Size(277, 13);
        public new static Color DefaultBackColor => Color.Transparent;

        [field: AccessedThroughProperty("TTip")]
        protected virtual ToolTip TTip
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public void SuspendUpdate()
        {
            _CanUpdate = false;
        }

        public void ResumeUpdate(bool forceUpdate = true)
        {
            if (_CanUpdate) return;
            
            _CanUpdate = true;
            if (!forceUpdate) return;

            if (_EnableOverCap & !_EnableBaseValue)
            {
                if (_ValueUncapped <= _Value)
                {
                    panel1.BackColor = _BarColor;
                    panel3.BackColor = _OverCapColor;
                }
                else
                {
                    panel1.BackColor = _OverCapColor;
                    panel3.BackColor = _BarColor;
                }
            }

            SetValues();
        }

        #region SubBarsDimensions sub-class
        private class SubBarsDimensions
        {
            private int _P1Width;
            private int _P2Width;
            private int _P3Width;
            private int _P4Width;
            private int _P5Width;

            public int P1Width
            {
                get => Math.Max(0, _P1Width - 1);
                set => _P1Width = value;
            }
            
            public int P2Width
            {
                get => Math.Max(0, _P2Width - 1);
                set => _P2Width = value;
            }

            public int P3Width
            {
                get => Math.Max(0, _P3Width - 1);
                set => _P3Width = value;
            }

            public int P4Width
            {
                get => Math.Max(0, _P4Width - 1);
                set => _P4Width = value;
            }

            public int P5Width
            {
                get => Math.Max(0, _P5Width - 1);
                set => _P5Width = value;
            }

            public int P3Pos = 0;
        }
        #endregion

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
        public float MinimumBarValue
        {
            get => _MinimumValue;
            set => _MinimumValue = value;
        }

        [Description("Maximum bar value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float MaximumBarValue
        {
            get => _MaximumValue;
            set => _MaximumValue = value;
        }

        [Description("Main bar value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ValueMainBar
        {
            get => _Value;
            set
            {
                _Value = value;
                if (!_EnableBaseValue && !_EnableOverCap) SetValues();
            }
        }

        [Description("Over cap value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ValueOverCap
        {
            get => _ValueUncapped;
            set => _ValueUncapped = value;
        }

        [Description("Base value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ValueBase
        {
            get => _ValueBase;
            set => _ValueBase = value;
        }

        [Description("Overlay #1 value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ValueOverlay1
        {
            get => _ValueOverlay1;
            set => _ValueOverlay1 = value;
        }

        [Description("Overlay #2 value"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ValueOverlay2
        {
            get => _ValueOverlay2;
            set => _ValueOverlay2 = value;
        }

        [Description("Over cap bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorOverCap
        {
            get => _OverCapColor;
            set
            {
                panel1.BackColor = value;
                _OverCapColor = value;
                Refresh();
            }
        }

        [Description("Base value bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorBaseValue
        {
            get => _BaseValueColor;
            set
            {
                panel2.BackColor = value;
                _BaseValueColor = value;
                Refresh();
            }
        }

        [Description("Main bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorMainBar
        {
            get => _BarColor;
            set
            {
                panel3.BackColor = value;
                _BarColor = value;
                Refresh();
            }
        }

        [Description("Overlay #1 bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorOverlay1
        {
            get => _Overlay1Color;
            set
            {
                panel4.BackColor = value;
                _Overlay1Color = value;
                Refresh();
            }
        }

        [Description("Overlay #2 bar color"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColorOverlay2
        {
            get => _Overlay2Color;
            set
            {
                panel5.BackColor = value;
                _Overlay2Color = value;
                Refresh();
            }
        }

        [Description("Tooltip text"), Category("Appearance"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Tip
        {
            get => _Tip;
            set => _Tip = value;
        }
        #endregion

        private int Value2Pixels(float value)
        {
            return (int)Math.Floor(Width / Math.Abs(_MaximumValue - _MinimumValue) * (value - _MinimumValue));
        }

        #region CalcSubBarsDimensions() overloads
        private SubBarsDimensions CalcSubBarsDimensions(float value)
        {
            SubBarsDimensions dim = new SubBarsDimensions()
            {
                P3Width = Value2Pixels(value)
            };

            return dim;
        }

        private SubBarsDimensions CalcSubBarsDimensions(float mainValue, float auxValue)
        {
            SubBarsDimensions dim = new SubBarsDimensions();
            if (_EnableOverCap)
            {
                // auxValue: overCapValue
                if (auxValue > mainValue)
                {

                    dim.P1Width = Value2Pixels(auxValue);
                    dim.P3Width = Value2Pixels(mainValue);
                    dim.P3Pos = 0;
                }
                else
                {
                    dim.P1Width = Value2Pixels(mainValue);
                    dim.P3Width = Value2Pixels(mainValue - auxValue);
                    dim.P3Pos = Value2Pixels(auxValue);
                    int offset = dim.P3Pos - dim.P1Width;
                    dim.P3Pos -= offset;
                    dim.P3Width += offset;
                }
            }

            if (_EnableBaseValue)
            {
                // auxValue: baseValue
                dim.P2Width = Value2Pixels(auxValue);
                if (auxValue < mainValue)
                {
                    dim.P3Width = Value2Pixels(mainValue - auxValue);
                    dim.P3Pos = Value2Pixels(auxValue);
                    int offset = dim.P3Pos - dim.P2Width + 1;
                    dim.P3Pos -= offset;
                    dim.P3Width += offset;
                }
                else
                {
                    dim.P3Width = Value2Pixels(mainValue);
                    dim.P3Pos = 0;
                }
            }

            return dim;
        }

        private SubBarsDimensions CalcSubBarsDimensions(float mainValue, float baseValue, float uncappedValue)
        {
            SubBarsDimensions dim = new SubBarsDimensions
            {
                P3Pos = 0, P1Width = Value2Pixels(uncappedValue), P2Width = Value2Pixels(baseValue)
            };

            //float relativeMax = Math.Max(_MaximumValue, uncappedValue);
            if (uncappedValue > mainValue)
            {
                dim.P3Width = Value2Pixels(mainValue);
            }
            else
            {
                dim.P3Width = Value2Pixels(mainValue - uncappedValue);
                dim.P3Pos = Value2Pixels(uncappedValue);
                int offset = dim.P3Pos - dim.P1Width + 1;
                dim.P3Pos -= offset;
                dim.P3Width += offset;
            }

            if (baseValue < mainValue)
            {
                dim.P3Width = Value2Pixels(mainValue - baseValue);
                dim.P3Pos = Value2Pixels(baseValue);
                int offset = dim.P3Pos - dim.P2Width + 1;
                dim.P3Pos -= offset;
                dim.P3Width += offset;
            }

            return dim;
        }

        private SubBarsDimensions CalcSubBarsDimensions(float mainValue, float baseValue, float uncappedValue, float overlay1Value)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(mainValue, baseValue, uncappedValue);
            dim.P4Width = Value2Pixels(overlay1Value);

            return dim;
        }

        private SubBarsDimensions CalcSubBarsDimensions(float mainValue, float baseValue, float uncappedValue, float overlay1Value, float overlay2Value)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(mainValue, baseValue, uncappedValue, overlay1Value);
            dim.P5Width = Value2Pixels(overlay2Value);

            return dim;
        }
        #endregion

        #region SetValues() overloads
        public void SetValues()
        {
            if (_EnableOverlay2)
                SetValues(_Value, _ValueBase, _ValueUncapped, _ValueOverlay1, _ValueOverlay2);

            else if (_EnableOverlay1)
                SetValues(_Value, _ValueBase, _ValueUncapped, _ValueOverlay1);

            else if (_EnableOverCap && _EnableBaseValue)
                SetValues(_Value, _ValueBase, _ValueUncapped);

            else if (_EnableOverCap)
                SetValues(_Value, _ValueUncapped);

            else if (_EnableBaseValue)
                SetValues(_Value, _ValueBase);

            else
                SetValues(_Value);
        }


        public void SetValues(float value)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(value);
            panel3.Width = dim.P3Width;
        }

        public void SetValues(float mainValue, float auxValue)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(mainValue, auxValue);
            if (_EnableOverCap)
            {
                panel1.Width = dim.P1Width;
                panel3.Width = dim.P3Width;
                panel3.Location = new Point(dim.P3Pos, panel3.Location.Y);
            }

            if (_EnableBaseValue)
            {
                panel2.Width = dim.P2Width;
                panel3.Width = dim.P3Width;
                panel3.Location = new Point(dim.P3Pos, panel3.Location.Y);
            }
        }

        public void SetValues(float mainValue, float baseValue, float uncappedValue)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(mainValue, baseValue, uncappedValue);
            panel1.Width = dim.P1Width;
            panel2.Width = dim.P2Width;
            panel3.Width = dim.P3Width;
            panel3.Location = new Point(dim.P3Pos, panel3.Location.Y);
        }

        public void SetValues(float mainValue, float baseValue, float uncappedValue, float overlay1Value)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(mainValue, baseValue, uncappedValue, overlay1Value);
            panel1.Width = dim.P1Width;
            panel2.Width = dim.P2Width;
            panel3.Width = dim.P3Width;
            panel3.Location = new Point(dim.P3Pos, panel3.Location.Y);
            panel4.Width = dim.P4Width;
        }

        public void SetValues(float mainValue, float baseValue, float uncappedValue, float overlay1Value, float overlay2Value)
        {
            SubBarsDimensions dim = CalcSubBarsDimensions(mainValue, baseValue, uncappedValue, overlay1Value, overlay2Value);
            panel1.Width = dim.P1Width;
            panel2.Width = dim.P2Width;
            panel3.Width = dim.P3Width;
            panel3.Location = new Point(dim.P3Pos, panel3.Location.Y);
            panel4.Width = dim.P4Width;
            panel5.Width = dim.P5Width;
        }
        #endregion

        public ctlLayeredBar()
        {
            try
            {
                SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.UserPaint,
                    true);
                InitializeComponent();
                TTip = new ToolTip
                {
                    AutoPopDelay = 10000,
                    InitialDelay = 500,
                    ReshowDelay = 100
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exception {ex.GetType()}\r\n{ex.Message}\r\n\r\n{ex.StackTrace}");
            }
        }

        private void ctlLayeredBar_Load(object sender, EventArgs e)
        {
            // Required to avoid artifacts in the designer with bars showing a single color at 100%
            SetValues();
        }

        private void ctlLayeredBar_MouseMove(object sender, MouseEventArgs e)
        {
            TTip.SetToolTip(this, _Tip);
        }

        private void ctlLayeredBar_MouseLeave(object sender, EventArgs e)
        {
            TTip.Hide(this);
        }
    }
}