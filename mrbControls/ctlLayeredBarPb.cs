using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
using mrbBase.Base.Display;

namespace mrbControls
{
    #region BarData sub-class
    public class BarData
    {
        public string FriendlyName { get; set; }
        public bool Enabled { get; set; }
        public float Value { get; set; }

        public Color Color { get; set; }
    }
    #endregion

    public partial class ctlLayeredBarPb : UserControl
    {
        private List<BarData> ListValues;
        private float _MinimumValue;
        private float _MaximumValue = 100;
        private string _Tip = "";
        private string _Group = "";
        private Graphics Gfx;
        private ExtendedBitmap BxBuffer;
        // https://stackoverflow.com/a/34299931
        // https://stackoverflow.com/questions/51597919/c-sharp-winform-stop-control-property-setting-to-default-when-it-is-set-to-be-a
        protected override Size DefaultSize => new Size(277, 13);
        public new Color DefaultBackColor => Color.Transparent;
        private List<Color> HighlightColors;
        public delegate void BarEventHandler(object sender);
        public event BarEventHandler BarHover;

        public ctlLayeredBarPb()
        {
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint |
                ControlStyles.SupportsTransparentBackColor,
                true);
            ListValues = new List<BarData>();
            HighlightColors = new List<Color>();
            InitializeComponent();
        }

        private void AppendVariable(string name, Color color, bool enabled = true, float value = 0)
        {
            ListValues = ListValues.Append(new BarData {FriendlyName = name, Color = color, Enabled = enabled, Value = value}).ToList();
        }

        public void AssignNames(List<string> names)
        {
            ListValues.Clear();
            foreach (var e in names)
            {
                AppendVariable(e, Color.Magenta);
            }
            SetHighlightColors();
        }

        public void AssignNames(List<(string name, Color color)> listSettings)
        {
            ListValues.Clear();
            foreach (var e in listSettings)
            {
                AppendVariable(e.name, e.color);
            }
            SetHighlightColors();
        }

        public void AssignValues(List<float> values)
        {
            for (var i=0 ; i<values.Count; i++)
            {
                try
                {
                    ListValues[i].Value = values[i];
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\r\nControl name: {Name}\r\nListValues.Count: {ListValues.Count}, values.Count: {values.Count}");
                }
            }

            Draw();
        }

        public void AssignValues(List<(string name, float value)> listValues)
        {
            for (var i = 0; i < listValues.Count; i++)
            {
                var idx = ListValues.FindIndex(e => e.FriendlyName == listValues[i].name);
                ListValues[idx].Value = listValues[i].value;
            }

            Draw();
        }

        public void AssignZero()
        {
            foreach (var e in ListValues)
            {
                e.Value = 0;
            }

            Draw();
        }

        private void SetHighlightColors()
        {
            HighlightColors.Clear();
            foreach (var e in ListValues)
            {
                var hlsColor = HLSColor.FromRgb(e.Color);
                HighlightColors = HighlightColors.Append(HLSColor.ToRgb(hlsColor.H, Math.Min(1, hlsColor.L + 0.2), hlsColor.S)).ToList();
            }
        }

        public Dictionary<string, float> GetValues()
        {
            return ListValues.ToDictionary(e => e.FriendlyName, e => e.Value);
        }

        #region Properties
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

        [Description("Bar group"), Category("Data"),
         Browsable(true), EditorBrowsable(EditorBrowsableState.Always), Bindable(true),
         DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public string Group
        {
            get => _Group;
            set => _Group = value;
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

        public void Draw(bool highlighted = false)
        {
            var sortedList = new List<BarData>();
            sortedList = ListValues.Clone();
            if (!highlighted)
            {
                //sortedList.AddRange(ListValues.Where(e => e.Enabled & e.Value > _MinimumValue));
                sortedList = sortedList.Where(e => e.Enabled & e.Value > _MinimumValue).ToList();
            }
            else
            {
                //sortedList.AddRange(ListValues);
                for (var i = 0; i < sortedList.Count; i++)
                {
                    sortedList[i].Color = HighlightColors[i];
                }

                sortedList = sortedList.Where(e => e.Enabled & e.Value > _MinimumValue).ToList();
            }

            Gfx = null;
            Gfx = CreateGraphics();
            BxBuffer = new ExtendedBitmap(Size);
            if (BxBuffer.Graphics == null) return;

            BxBuffer.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            BxBuffer.Graphics.Clear(Color.Transparent);
            if (highlighted)
            {
                BxBuffer.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(90, 128, 128, 128)), 0, 0, BxBuffer.Size.Width, BxBuffer.Size.Height);
            }

            if (sortedList.Count > 0)
            {

                sortedList.Sort((a, b) => -a.Value.CompareTo(b.Value));
                sortedList = sortedList.Select(e => new BarData
                {
                    FriendlyName = e.FriendlyName,
                    Color = e.Color,
                    Enabled = e.Enabled,
                    Value = Value2Pixels(e.Value)
                }).ToList();

                var values = GetValues();
                var vp = Value2Pixels(values["value"]);

                // Draw bars
                for (var i = 0; i < sortedList.Count; i++)
                {
                    if (sortedList[i].FriendlyName == "base" &&
                        Math.Abs(sortedList[i].Value - vp) < float.Epsilon) continue;
                    if (sortedList[i].FriendlyName == "uncapped" &&
                        Math.Abs(sortedList[i].Value - vp) < float.Epsilon) continue;
                    BxBuffer.Graphics.FillRectangle(new SolidBrush(sortedList[i].Color), 0, 0, sortedList[i].Value,
                        BxBuffer.Size.Height);
                }

                // Draw outlines
                var outlinePen = new Pen(new SolidBrush(Color.Black), 1);
                for (var i = 0; i < sortedList.Count; i++)
                {
                    if (sortedList[i].FriendlyName == "base" &&
                        Math.Abs(sortedList[i].Value - vp) < float.Epsilon) continue;
                    if (sortedList[i].FriendlyName == "uncapped" &&
                        Math.Abs(sortedList[i].Value - vp) < float.Epsilon) continue;
                    BxBuffer.Graphics.DrawLine(outlinePen, sortedList[i].Value, 0, sortedList[i].Value,
                        BxBuffer.Size.Height);
                }
            }

            Gfx.DrawImageUnscaled(BxBuffer.Bitmap, 0, 0);
        }
        public void SetTip(string iTip)
        {
            TTip.SetToolTip(this, iTip);
        }

        #region Event handlers
        private void ctlLayeredBarPb_Paint(object sender, PaintEventArgs e)
        {
            if (BxBuffer != null)
            {
                e.Graphics.DrawImage(BxBuffer.Bitmap, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle, GraphicsUnit.Pixel);
            }
        }

        private void ctlLayeredBarPb_Resize(object sender, EventArgs e)
        {
            Draw();
        }

        private void ctlLayeredBarPb_MouseEnter(object sender, EventArgs e)
        {
            Draw(true);
        }

        private void ctlLayeredBarPb_MouseMove(object sender, MouseEventArgs e)
        {
            BarHover?.Invoke(sender);
        }

        private void ctlLayeredBarPb_MouseLeave(object sender, EventArgs e)
        {
            var target = sender as ctlLayeredBarPb;
            TTip.SetToolTip(this, "");
            Draw();
            Refresh();
        }
        #endregion

        #region HLSColor sub-class
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
                var doubleR = c.R / 255d;
                var doubleG = c.G / 255d;
                var doubleB = c.B / 255d;

                // Get the maximum and minimum RGB components.
                var max = doubleR;
                if (max < doubleG) max = doubleG;
                if (max < doubleB) max = doubleB;

                var min = doubleR;
                if (min > doubleG) min = doubleG;
                if (min > doubleB) min = doubleB;

                var diff = max - min;
                l = (max + min) / 2;
                if (Math.Abs(diff) < double.Epsilon) //0.00001
                {
                    s = 0;
                    h = 0; // H is really undefined.
                }
                else
                {
                    s = l <= 0.5 ? diff / (max + min) : diff / (2 - max - min);

                    var rDist = (max - doubleR) / diff;
                    var gDist = (max - doubleG) / diff;
                    var bDist = (max - doubleB) / diff;

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

                var p1 = 2 * L - p2;
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

                var p1 = 2 * l - p2;
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
        #endregion
    }
}