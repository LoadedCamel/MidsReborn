using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace midsControls
{
    public partial class ctlDataGraph : PictureBox
    {
        public DataGraph Graph;

        public class DataGraph
        {
            private class GraphColorOptions
            {
                public Color LineColor = Color.Yellow;
                public Color DotsColor = Color.FromArgb(Convert.ToInt32(0.66 * 256), Color.LightGreen);
                public Color BGColor = Color.Black;
                public Color AxisColor = Color.Silver;
                public Color InnerGridColor = Color.DarkGray;
                public Color AxisValuesColor = Color.Gainsboro;
            }

            private bool SmoothDraw = true; // Use Bezier curves instead of lines
            private bool DetectPlateau = false;
            private float? IgnoreValue = 0;
            private GraphColorOptions ColorOptions = new GraphColorOptions();
            private Padding InnerPadding = new Padding(15, 8, 8, 15);
            private bool DrawDots = false;
            private int DotRadius = 1;
            private Size BitmapDimensions;
            private List<float> DataSeries;

            public DataGraph()
            {
                BitmapDimensions = new Size(100, 50);
                DataSeries = new List<float>();
            }

            // Size passed to the contructor will be the default size
            public void SetSize(Size s)
            {
                BitmapDimensions = new Size(s.Width, s.Height);
            }

            public bool SetOption(string option, bool value)
            {
                Type t = typeof(DataGraph);
                PropertyInfo info = t.GetProperty(option);
                if (info == null || !info.CanWrite) return false;
                if (!(info.PropertyType == typeof(bool))) return false;

                info.SetValue(this, value, null);

                return true;
            }

            public bool SetOption(string option, float? value)
            {
                Type t = typeof(DataGraph);
                PropertyInfo info = t.GetProperty(option);
                if (info == null || !info.CanWrite) return false;
                if (!(info.PropertyType == typeof(float?))) return false;

                info.SetValue(this, value, null);

                return true;
            }

            public bool SetOption(string option, int value)
            {
                Type t = typeof(DataGraph);
                PropertyInfo info = t.GetProperty(option);
                if (info == null || !info.CanWrite) return false;
                if (!(info.PropertyType == typeof(int))) return false;

                info.SetValue(this, value, null);

                return true;
            }

            public bool SetOption(string option, Padding value)
            {
                Type t = typeof(DataGraph);
                PropertyInfo info = t.GetProperty(option);
                if (info == null || !info.CanWrite) return false;
                if (!(info.PropertyType == typeof(Padding))) return false;

                info.SetValue(this, value, null);

                return true;
            }

            public bool SetOption(string option, Color value)
            {
                Type t = typeof(GraphColorOptions);
                PropertyInfo info = t.GetProperty(option);
                if (info == null || !info.CanWrite) return false;
                if (!(info.PropertyType == typeof(Color))) return false;

                info.SetValue(ColorOptions, value, null);

                return true;
            }

            public void SetDataPoints(List<float> dataPoints)
            {
                DataSeries.Clear();
                DataSeries.AddRange(dataPoints);
            }

            public void SetDataPoints(float[] dataPoints)
            {
                DataSeries.Clear();
                DataSeries.AddRange(dataPoints);
            }

            public void Draw(Graphics g)
            {
                int i;
                int w = BitmapDimensions.Width - InnerPadding.Horizontal;
                int h = BitmapDimensions.Height - InnerPadding.Vertical;

                float xMin = 0;
                float xMax = DataSeries.Count;
                float yMin;
                float yMax;
                float xPlateau = float.NegativeInfinity;
                float v;

                if (DataSeries.Count == 0) return;

                v = DataSeries.First(); // Only because it needs to be forcibly initialized.
                yMin = DataSeries.Min();
                yMax = DataSeries.Max();

                for (i = DataSeries.Count - 1; i > 0; i--)
                {
                    if (IgnoreValue == null || DataSeries[i] == IgnoreValue) continue;
                    v = DataSeries[i];
                    break;
                }

                for (i = DataSeries.Count - 1; i > 0; i--)
                {
                    if (DataSeries[i] == v || i >= DataSeries.Count - 2) continue;
                    xPlateau = i + 1;
                    break;
                }

                Pen p;
                Bitmap res = new Bitmap(BitmapDimensions.Width, BitmapDimensions.Height);
                g.Clear(ColorOptions.BGColor);

                /*
                 * Layers:
                 * Inner grid (dotted)
                 * Axis
                 * Lines
                 * Dots
                 * Plateau secondary X axis
                 * Axis Values
                */

                float xScale = w / Math.Max(xMax - xMin, 1);
                float yScale = h / Math.Max(yMax - yMin, 1);

                #region Graph: Inner Grid
                using (p = new Pen(ColorOptions.InnerGridColor) { DashStyle = DashStyle.Dot })
                {
                    // Verticals
                    for (i = (int)xMin; i <= xMax; i += 5)
                    {
                        g.DrawLine(p,
                            new Point((int)Math.Round(i * xScale) + InnerPadding.Left, InnerPadding.Top),
                            new Point((int)Math.Round(i * xScale) + InnerPadding.Left, h + InnerPadding.Top));
                    }

                    // Horizontals
                    for (i = InnerPadding.Top; i <= h + InnerPadding.Top; i += h / 5)
                    {
                        g.DrawLine(p,
                            new Point(InnerPadding.Left, i),
                            new Point(w + InnerPadding.Left, i));
                    }
                }
                #endregion

                #region Graph: Axis
                using (p = new Pen(ColorOptions.AxisColor) { DashStyle = DashStyle.Solid })
                {
                    g.DrawLine(p,
                        new Point(InnerPadding.Left, h + InnerPadding.Top),
                        new Point(w + InnerPadding.Left, h + InnerPadding.Top));
                    g.DrawLine(p,
                        new Point(InnerPadding.Left, InnerPadding.Top),
                        new Point(InnerPadding.Left, h + InnerPadding.Top));

                    // May need to draw the axis for value 0 here, if needed.
                    if (DataSetCrossZero())
                    {
                        //g.DrawLine(p, new Point(), new Point()); // Not implemented
                    }
                }
                #endregion

                #region Graph: Lines
                // Note: negate values if they are all negative, e.g. for damage tables.
                bool reverseValues = CheckAllNegativeValues();
                float val;

                // Not using Array for graph data points.
                // It may not have the same length as the source dataSeries.
                List<Point> dataPoints = new List<Point>();

                using (p = new Pen(ColorOptions.LineColor) { DashStyle = DashStyle.Solid, Width = 1 })
                {
                    for (i = 0; i < Math.Min(50, DataSeries.Count); i++)
                    {
                        if (IgnoreValue != null && DataSeries[i] == IgnoreValue) continue;
                        val = DataSeries[i] * (reverseValues ? -1 : 1);
                        val = h + InnerPadding.Top - val * yScale;
                        dataPoints.Add(new Point((int)(i * xScale + InnerPadding.Left), (int)val));
                    }

                    if (dataPoints.Count >= 2)
                    {
                        if (SmoothDraw)
                        {
                            g.DrawCurve(p, dataPoints.ToArray());
                        }
                        else
                        {
                            g.DrawLines(p, dataPoints.ToArray());
                        }
                    }
                }
                #endregion

                #region Graph: Dots

                if (DrawDots)
                {
                    using (p = new Pen(ColorOptions.DotsColor))
                    {
                        for (i = 0; i < Math.Min(50, DataSeries.Count); i++)
                        {
                            if (IgnoreValue != null && DataSeries[i] == IgnoreValue) continue;
                            val = DataSeries[i] * (reverseValues ? -1 : 1);
                            val = h + InnerPadding.Top - val * yScale;
                            g.DrawEllipse(p,
                                new Rectangle(
                                    new Point((int) (i * xScale + InnerPadding.Left) - DotRadius,
                                        (int) val - DotRadius),
                                    new Size(DotRadius * 2 + 1, DotRadius * 2 + 1)
                                ));
                        }
                    }
                }

                #endregion

                #region Axis values
                using Font segoeFont = new Font("Segoe UI", 7, FontStyle.Regular, GraphicsUnit.Pixel);
                using Brush txtBrush = new SolidBrush(ColorOptions.AxisValuesColor);
                TextFormatFlags sf = TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                TextRenderer.DrawText(g, "0", segoeFont,
                    new Point(Math.Max(0, InnerPadding.Left - 5),
                    Math.Min(h + InnerPadding.Vertical - 7, h + InnerPadding.Top + 5)),
                    ColorOptions.AxisValuesColor,
                    Color.Transparent,
                    sf);
                
                for (i = (int)xMin + 10; i <= xMax; i += 10)
                {
                    TextRenderer.DrawText(g, Convert.ToString(i, null), segoeFont, new Point(
                        (int) Math.Round(i * xScale) + InnerPadding.Left - 5,
                        Math.Min(h + InnerPadding.Vertical, h + InnerPadding.Top + 5)), ColorOptions.AxisValuesColor, Color.Transparent, sf);
                }

                #endregion

                //g.DrawImage(img, new Point(0, 0));
            }

            private bool CheckAllNegativeValues()
            {
                return DataSeries.All(t => !(t > 0));
            }

            private bool DataSetCrossZero()
            {
                bool vPositive = false;
                bool vNegative = false;
                foreach (float t in DataSeries)
                {
                    if (t > 0) vPositive = true;
                    if (t < 0) vNegative = true;
                }

                return (vPositive & vNegative);
            }
        }

        public ctlDataGraph()
        {
            InitializeComponent();
            Graph = new DataGraph();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graph.Draw(e.Graphics);
            base.OnPaint(e);
        }
    }
}
