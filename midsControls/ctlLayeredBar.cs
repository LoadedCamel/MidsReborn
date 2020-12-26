using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace mrbControls
{
    public partial class ctlLayeredBar : clsCustomBorderUC
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
        private string _Group = "";
        private Color _OverCapColor = Color.FromArgb(128, 0, 128);
        private Color _BaseValueColor = Color.FromArgb(191, 0, 191);
        private Color _BarColor = Color.Magenta;
        private Color _Overlay1Color = Color.FromArgb(255, 64, 255);
        private Color _Overlay2Color = Color.FromArgb(255, 128, 255);
        private Color[] _NormalColors;
        private Color[] _HighlightColors;

        // https://stackoverflow.com/a/34299931
        // https://stackoverflow.com/questions/51597919/c-sharp-winform-stop-control-property-setting-to-default-when-it-is-set-to-be-a
        protected override Size DefaultSize => new Size(277, 13);
        public new static Color DefaultBackColor => Color.Transparent;

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

            SetHighlightColors();
            SetValues();
        }

        private void SetHighlightColors()
        {
            HLSColor p1Color = HLSColor.FromRgb(panel1.BackColor);
            HLSColor p2Color = HLSColor.FromRgb(panel2.BackColor);
            HLSColor p3Color = HLSColor.FromRgb(panel3.BackColor);
            HLSColor p4Color = HLSColor.FromRgb(panel4.BackColor);
            HLSColor p5Color = HLSColor.FromRgb(panel5.BackColor);

            _NormalColors = new[]
            {
                panel1.BackColor,
                panel2.BackColor,
                panel3.BackColor,
                panel4.BackColor,
                panel5.BackColor
            };
            
            _HighlightColors = new[]
            {
                HLSColor.ToRgb(p1Color.H, Math.Min(1, p1Color.L + 0.2), p2Color.S),
                HLSColor.ToRgb(p2Color.H, Math.Min(1, p2Color.L + 0.2), p2Color.S),
                HLSColor.ToRgb(p3Color.H, Math.Min(1, p3Color.L + 0.2), p3Color.S),
                HLSColor.ToRgb(p4Color.H, Math.Min(1, p4Color.L + 0.2), p4Color.S),
                HLSColor.ToRgb(p5Color.H, Math.Min(1, p5Color.L + 0.2), p5Color.S)
            };
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

        [Description("Bar group"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Group
        {
            get => _Group;
            set => _Group = value;
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

        public void SetTip(string iTip)
        {
            TTip.SetToolTip(this, iTip);
        }

        private void ctlLayeredBar_MouseMove(object sender, MouseEventArgs e)
        {
            BarHover?.Invoke(sender);
        }

        private void ctlLayeredBar_MouseLeave(object sender, EventArgs e)
        {
            TTip.SetToolTip(this, "");
            NormalBarColors();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics,
                new Rectangle(ClientRectangle.X - 1, ClientRectangle.Y - 1, ClientRectangle.Width + 2,
                    ClientRectangle.Height + 2), Color.LightSkyBlue, ButtonBorderStyle.Solid);
        }

        public void HighlightBarColors()
        {
            SuspendLayout();
            BackColor = Color.FromArgb(90, 128, 128, 128);
            panel1.BackColor = _HighlightColors[0];
            panel2.BackColor = _HighlightColors[1];
            panel3.BackColor = _HighlightColors[2];
            panel4.BackColor = _HighlightColors[3];
            panel5.BackColor = _HighlightColors[4];
            ResumeLayout();
        }

        public void NormalBarColors()
        {
            SuspendLayout();
            BackColor = Color.Transparent;
            panel1.BackColor = _NormalColors[0];
            panel2.BackColor = _NormalColors[1];
            panel3.BackColor = _NormalColors[2];
            panel4.BackColor = _NormalColors[3];
            panel5.BackColor = _NormalColors[4];
            ResumeLayout();
        }

        public delegate void BarEventHandler(object sender);
        public event BarEventHandler BarHover;

        private class HLSColor
        {
            public readonly double H;
            public readonly double L;
            public readonly double S;

            private HLSColor(double h, double l, double s)
            {
                H = h;
                L = l;
                S = s;
            }

            public static HLSColor FromRgb(Color c)
            {
                double h;
                double s;
                double l;

                // Convert RGB to a 0.0 to 1.0 range.
                double doubleR = c.R / 255d;
                double doubleG = c.G / 255d;
                double doubleB = c.B / 255d;

                // Get the maximum and minimum RGB components.
                double max = doubleR;
                if (max < doubleG) max = doubleG;
                if (max < doubleB) max = doubleB;

                double min = doubleR;
                if (min > doubleG) min = doubleG;
                if (min > doubleB) min = doubleB;

                double diff = max - min;
                l = (max + min) / 2;
                if (Math.Abs(diff) < double.Epsilon) //0.00001
                {
                    s = 0;
                    h = 0; // H is really undefined.
                }
                else
                {
                    s = l <= 0.5 ? diff / (max + min) : diff / (2 - max - min);

                    double rDist = (max - doubleR) / diff;
                    double gDist = (max - doubleG) / diff;
                    double bDist = (max - doubleB) / diff;

                    if (Math.Abs(doubleR - max) < double.Epsilon)
                    {
                        h = bDist - gDist;
                    }
                    else if (Math.Abs(doubleG - max) < double.Epsilon)
                    {
                        h = 2 + rDist - bDist;
                    }
                    else
                    {
                        h = 4 + gDist - rDist;
                    }

                    h *= 60;
                    if (h < 0)
                    {
                        h += 360;
                    }
                }

                return new HLSColor(h, l, s);
            }

            public Color ToRgb()
            {
                int r;
                int g;
                int b;

                double p2;
                if (L <= 0.5) p2 = L * (1 + S);
                else p2 = L + S - L * S;

                double p1 = 2 * L - p2;
                double doubleR, doubleG, doubleB;
                if (S == 0)
                {
                    doubleR = L;
                    doubleG = L;
                    doubleB = L;
                }
                else
                {
                    doubleR = QqhToRgb(p1, p2, H + 120);
                    doubleG = QqhToRgb(p1, p2, H);
                    doubleB = QqhToRgb(p1, p2, H - 120);
                }

                // Convert RGB to the 0 to 255 range.
                r = (int)(doubleR * 255.0);
                g = (int)(doubleG * 255.0);
                b = (int)(doubleB * 255.0);

                return Color.FromArgb(r, g, b);
            }

            public static Color ToRgb(double h, double l, double s)
            {
                int r;
                int g;
                int b;

                double p2;
                if (l <= 0.5)
                {
                    p2 = l * (1 + s);
                }
                else
                {
                    p2 = l + s - l * s;
                }

                double p1 = 2 * l - p2;
                double doubleR, doubleG, doubleB;
                if (s == 0)
                {
                    doubleR = l;
                    doubleG = l;
                    doubleB = l;
                }
                else
                {
                    doubleR = QqhToRgb(p1, p2, h + 120);
                    doubleG = QqhToRgb(p1, p2, h);
                    doubleB = QqhToRgb(p1, p2, h - 120);
                }

                // Convert RGB to the 0 to 255 range.
                r = (int)(doubleR * 255.0);
                g = (int)(doubleG * 255.0);
                b = (int)(doubleB * 255.0);

                return Color.FromArgb(r, g, b);
            }

            private static double QqhToRgb(double q1, double q2, double hue)
            {
                if (hue > 360) hue -= 360;
                else if (hue < 0) hue += 360;

                if (hue < 60) return q1 + (q2 - q1) * hue / 60;
                if (hue < 180) return q2;
                if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
                return q1;
            }
        }
    }

    public class BarPanel : Panel
    {
        private IContainer components;
        public Color BorderColor = clsCustomBorderHandlers.GetDefaultColor();

        [field: AccessedThroughProperty("TTip")]
        protected virtual ToolTip Tip
        {
            get;
            [MethodImpl(MethodImplOptions.Synchronized)]
            set;
        }

        public BarPanel()
        {
            SetStyle(ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            components = new Container();
            Tip = new ToolTip(components);
            Name = "BarPanel";
        }

        /*protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            clsCustomBorderHandlers.OnResizeRedrawWindow(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            clsCustomBorderHandlers.WndProcDrawBorder(ref m, this, BorderColor);
        }*/

        public void SetTip(string iTip)
        {
            Tip.SetToolTip(this, iTip);
        }
    }
}