using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
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
                public Color AxisPlateauColor = Color.DeepSkyBlue;
            }

            public bool SmoothDraw = true; // Use Bezier curves instead of lines
            public bool DetectPlateau = false;
            public float? IgnoreValue = 0;
            private GraphColorOptions ColorOptions = new GraphColorOptions();
            private Padding InnerPadding = new Padding(32, 8, 8, 15);
            public bool DrawDots = false;
            public int DotRadius = 1;
            private Size BitmapDimensions;
            private List<float> DataSeries;
            public bool ForceInvertValues;

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
                float yMinR;
                float yMaxR;
                float xPlateau = float.NegativeInfinity;
                float v;

                if (DataSeries.Count == 0) return;

                yMin = DataSeries.Min();
                yMax = DataSeries.Max();

                yMinR = float.PositiveInfinity;
                yMaxR = float.NegativeInfinity;
                for (i = 0; i < DataSeries.Count; i++)
                {
                    if (DataSeries[i] < yMinR && DataSeries[i] != IgnoreValue) yMinR = DataSeries[i];
                    if (DataSeries[i] > yMaxR && DataSeries[i] != IgnoreValue) yMaxR = DataSeries[i];
                }

                if (DataSeries.Count(e => e == DataSeries[0]) == DataSeries.Count)
                {
                    xPlateau = xMin;
                }
                else
                {
                    v = IgnoreValue != null ? DataSeries.Last(e => e != IgnoreValue) : DataSeries.Last();
                    for (i = DataSeries.Count - 1; i > 0; i--)
                    {
                        if (IgnoreValue == null)
                        {
                            if (DataSeries[i] == v) continue;
                        }
                        else
                        {
                            if (DataSeries[i] == v | DataSeries[i] == IgnoreValue) continue;
                        }

                        xPlateau = xMin + i + 1;
                        break;
                    }
                }

                Pen p;
                Bitmap res = new Bitmap(BitmapDimensions.Width, BitmapDimensions.Height);
                g.Clear(ColorOptions.BGColor);

                using Font segoeFont = new Font("Segoe UI", 9, FontStyle.Regular, GraphicsUnit.Pixel);
                TextFormatFlags sfH = TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter;
                TextFormatFlags sfV = TextFormatFlags.Right | TextFormatFlags.NoPadding | TextFormatFlags.VerticalCenter;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                float xScale = w / Math.Max(xMax - xMin, 0.001f);
                float yScale = h / Math.Max(yMax - yMin, 0.001f);

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

                if (DetectPlateau && xPlateau > xMin && xPlateau < 49)
                {
                    using (p = new Pen(ColorOptions.AxisPlateauColor) {DashStyle = DashStyle.Dash})
                    {
                        g.DrawLine(p,
                            new Point((int) Math.Round((xPlateau + xMin) * xScale) + InnerPadding.Left,
                                InnerPadding.Top),
                            new Point((int) Math.Round((xPlateau + xMin) * xScale) + InnerPadding.Left,
                                h + InnerPadding.Top));
                    }

                    TextRenderer.DrawText(g, Convert.ToString(xPlateau + 1, null), segoeFont,
                        new Point((int) ((xMin + xPlateau) * xScale + InnerPadding.Left + 3), InnerPadding.Top + h / 2),
                        ColorOptions.AxisPlateauColor, Color.FromArgb(128, ColorOptions.BGColor), sfH);
                }

                #endregion

                #region Graph: Lines
                // Note: negate values if they are all negative, e.g. for damage tables.
                bool reverseValues = ForceInvertValues || CheckAllNegativeValues();
                float val;

                // Not using Array for graph data points.
                // It may not have the same length as the source dataSeries.
                List<Point> dataPoints = new List<Point>();

                using (p = new Pen(ColorOptions.LineColor) { DashStyle = DashStyle.Solid, Width = 1 })
                {
                    for (i = 0; i < Math.Min(50, DataSeries.Count); i++)
                    {
                        if (IgnoreValue != null && DataSeries[i] == IgnoreValue && xPlateau > xMin) continue;
                        if (xPlateau == xMin)
                        {
                            val = InnerPadding.Top + h / 2; // If all values are the same, vertically center the line
                        }
                        else
                        {
                            val = DataSeries[i] * (reverseValues ? -1 : 1);
                            val = h + InnerPadding.Top - (val - (reverseValues ? Math.Abs(yMax) : yMin)) * yScale;
                        }
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
                using Brush txtBrush = new SolidBrush(ColorOptions.AxisValuesColor);
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                for (i = (int)xMin; i <= xMax; i += 10)
                {
                    TextRenderer.DrawText(g, Convert.ToString(i + 1, null), segoeFont, new Point(
                        (int) Math.Round((i - xMin) * xScale) + InnerPadding.Left - (i < 10 ? 2 : 4),
                        h + InnerPadding.Top + 7), ColorOptions.AxisValuesColor, Color.Transparent, sfH);
                }

                if (xPlateau > 0)
                {
                    float k = reverseValues ? Math.Abs(yMax) : yMin;
                    for (i = 0; i < 6; i++)
                    {
                        TextRenderer.DrawText(g,
                            Convert.ToString(Math.Round(k, Math.Abs(yMaxR - yMinR) < 1 ? 2 : 1), null), segoeFont,
                            new Rectangle(1, InnerPadding.Top + h - h / 5 * i - 7,
                                InnerPadding.Left - 5, 11),
                            ColorOptions.AxisValuesColor, Color.Transparent,
                            sfV);

                        k += Math.Abs(yMax - yMin) / 5;
                    }
                }

                #endregion
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
