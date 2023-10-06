using System;
using Mids_Reborn.Controls.Extensions;
using Mids_Reborn.Core.Utils;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Text;

namespace Mids_Reborn.Forms.Controls
{
    public partial class TimelineCursorZoom : UserControl
    {
        #region Custom events

        public delegate void ViewIntervalChangedEventHandler(Interval? viewInterval);
        public event ViewIntervalChangedEventHandler? ViewIntervalChanged;

        public delegate void HoveredPosChangedEventHandler(float? time, int? pixelValue);
        public event HoveredPosChangedEventHandler HoveredPosChanged;

        #endregion

        private enum DragTargetType
        {
            Interval,
            LowerBound,
            UpperBound
        }

        public Color GridColor { get; set; }
        public Color MarkersColor { get; set; }
        public Color StartMarkerColor { get; set; }
        public Color EndMarkerColor { get; set; }
        public Color HoveredPosColor { get; set; }
        public Color TextShadowColor { get; set; }
        public Interval? ViewInterval { get; private set; }
        public Interval? TimelineInterval { get; private set; }
        public float TextSize { get; set; }

        private List<float>? PowerTimes;
        private int? HoveredPos;
        private bool HideHovered;
        private int? DragPos;
        private DragTargetType? DragTarget;

        private float Hscale => TimelineInterval == null || TimelineInterval.End < float.Epsilon
            ? 1
            : Width / TimelineInterval.End;

        public TimelineCursorZoom()
        {
            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            // Default settings
            BackColor = Color.FromArgb(41, 49, 52);
            GridColor = Color.FromArgb(108, 120, 140);
            MarkersColor = Color.FromArgb(147, 199, 99); // Color.FromArgb(85, 228, 57); // ???
            StartMarkerColor = Color.FromArgb(32, 32, 255);
            EndMarkerColor = Color.FromArgb(255, 32, 32);
            HoveredPosColor = Color.BurlyWood;
            ForeColor = Color.Gainsboro;
            TextShadowColor = Color.Black;
            TextSize = 9;

            HideHovered = false;

            InitializeComponent();
        }

        public void SetData(List<float> powerTimes, Interval timelineInterval, bool redraw = true)
        {
            PowerTimes = powerTimes;
            TimelineInterval = timelineInterval;
            ViewInterval = timelineInterval;

            if (!redraw)
            {
                return;
            }

            Invalidate();
        }

        #region Draw routine

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;

            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            g.Clear(BackColor);

            if (TimelineInterval == null)
            {
                return;
            }

            if (PowerTimes == null)
            {
                return;
            }

            if (PowerTimes.Count <= 0)
            {
                return;
            }

            if (TimelineInterval.End < float.Epsilon)
            {
                return;
            }

            // Draw grid
            const int gridInterval = 5;
            const int bracketsThickness = 3;
            using var textFont = new Font(new FontFamily("Segoe UI"), TextSize, GraphicsUnit.Pixel);

            using var gridBrush = new Pen(new SolidBrush(GridColor), 1);
            gridBrush.DashStyle = DashStyle.Dot;

            using var startMarkerBrush = new Pen(new SolidBrush(StartMarkerColor), bracketsThickness);
            using var endMarkerBrush = new Pen(new SolidBrush(EndMarkerColor), bracketsThickness);
            for (var i = 0; i < TimelineInterval.End; i += gridInterval)
            {
                g.DrawLine(gridBrush, new PointF(i * Hscale, 0), new PointF(i * Hscale, Height));
            }

            // Draw markers
            using var markerBrush = new Pen(new SolidBrush(MarkersColor), 1);
            foreach (var pt in PowerTimes)
            {
                g.DrawLine(markerBrush, new PointF(pt * Hscale, 0), new PointF(pt * Hscale, Height));
            }

            using var maskBrush = new SolidBrush(Color.FromArgb(192, 0, 0, 0));
            // Draw view slice brackets
            if (ViewInterval != null && ViewInterval.Length < TimelineInterval.Length)
            {
                // Add shadow for areas outside of bounds
                g.FillRectangle(maskBrush, new RectangleF(0, 0, ViewInterval.Start * Hscale, Height));
                g.FillRectangle(maskBrush, new RectangleF(ViewInterval.End * Hscale, 0, Width - ViewInterval.End * Hscale, Height));

                // Replace with DrawPath
                g.DrawLine(startMarkerBrush, new PointF(ViewInterval.Start * Hscale, 4),
                    new PointF(ViewInterval.Start * Hscale + 4, 4));
                g.DrawLine(startMarkerBrush, new PointF(ViewInterval.Start * Hscale, 4),
                    new PointF(ViewInterval.Start * Hscale, Height - 4));
                g.DrawLine(startMarkerBrush, new PointF(ViewInterval.Start * Hscale, Height - 4),
                    new PointF(ViewInterval.Start * Hscale + 4, Height - 4));

                g.DrawLine(endMarkerBrush, new PointF(ViewInterval.End * Hscale, 4),
                    new PointF(ViewInterval.End * Hscale - 4, 4));
                g.DrawLine(endMarkerBrush, new PointF(ViewInterval.End * Hscale, 4),
                    new PointF(ViewInterval.End * Hscale, Height - 4));
                g.DrawLine(endMarkerBrush, new PointF(ViewInterval.End * Hscale, Height - 4),
                    new PointF(ViewInterval.End * Hscale - 4, Height - 4));
            }

            // Draw hover indicator
            if (HoveredPos != null && !HideHovered)
            {
                using var hoveredIndicatorBrush = new Pen(new SolidBrush(HoveredPosColor), 1);
                g.DrawLine(hoveredIndicatorBrush, new Point(HoveredPos.Value, 0), new Point(HoveredPos.Value, Height));
            }

            // Draw grid values
            for (var i = 0; i < TimelineInterval.End; i += 2 * gridInterval)
            {
                var textValue = i.ToString();
                var textSize = TextRenderer.MeasureText(textValue, textFont);
                var x = i == 0
                    ? (int)Math.Round(i * Hscale + 2)
                    : (int)Math.Round(i * Hscale);

                if ((i == 0 ? x + textSize.Width : x + textSize.Width / 2f + 1) > Width - 2)
                {
                    continue;
                }

                if (i == 0)
                {
                    TextRendererExt.DrawOutlineText(g, textValue, textFont,
                        new Rectangle(new Point((int)Math.Round(i * Hscale + 2), 1), new Size(Width - x, Height - 2)), TextShadowColor, ForeColor,
                        TextFormatFlags.Left | TextFormatFlags.Bottom);
                }
                else
                {
                    TextRendererExt.DrawOutlineText(g, textValue, textFont,
                        new Rectangle(new Point((int)Math.Round((i - 2 * gridInterval) * Hscale), 1), new Size((int)Math.Round(4 * gridInterval * Hscale), Height - 2)), TextShadowColor, ForeColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom);
                }
            }
        }

        #endregion

        #region Event handlers

        private void TimelineCursorZoom_MouseMove(object sender, MouseEventArgs e)
        {
            if (!HideHovered)
            {
                if (HoveredPos == null || HoveredPos != e.X)
                {
                    HoveredPos = e.X;
                    HoveredPosChanged?.Invoke(HoveredPos / Hscale, HoveredPos);
                    Invalidate();
                }

                return;
            }

            HoveredPosChanged?.Invoke(null, null);

            if (DragPos == null)
            {
                return;
            }

            if (TimelineInterval == null)
            {
                return;
            }

            if (DragTarget == null)
            {
                return;
            }

            var valOffset = (e.X - DragPos.Value) / Hscale;
            ViewInterval = ViewInterval ?? TimelineInterval;
            ViewInterval = DragTarget switch
            {
                DragTargetType.LowerBound => ViewInterval?.OffsetStart(valOffset).MinMax(TimelineInterval),
                DragTargetType.UpperBound => ViewInterval?.OffsetEnd(valOffset).MinMax(TimelineInterval),
                DragTargetType.Interval => ViewInterval?.Offset(valOffset).MinMax(TimelineInterval),
                _ => ViewInterval
            };

            HoveredPos = e.X;
            DragPos = e.X;
            ViewIntervalChanged?.Invoke(ViewInterval);

            Invalidate();
        }

        private void TimelineCursorZoom_MouseLeave(object sender, EventArgs e)
        {
            HoveredPos = null;
            HoveredPosChanged?.Invoke(null, null);
            Invalidate();
        }

        private void TimelineCursorZoom_MouseWheel(object? sender, MouseEventArgs e)
        {
            if (TimelineInterval == null)
            {
                return;
            }

            ViewInterval ??= new Interval(TimelineInterval);

            switch (e.Delta)
            {
                case > 0 when ViewInterval.Length <= 5: // ???
                    return;

                case > 0:
                    ViewInterval = ViewInterval.ScaleCenter(0.8f).MinMax(TimelineInterval);
                    break;

                case < 0 when ViewInterval.Length >= TimelineInterval.Length:
                    return;

                case < 0:
                    ViewInterval = ViewInterval.ScaleCenter(1.25f).MinMax(TimelineInterval);
                    break;

                default:
                    return;
            }

            ViewIntervalChanged?.Invoke(ViewInterval);
            Invalidate();
        }

        private void TimelineCursorZoom_MouseDown(object sender, MouseEventArgs e)
        {
            if (TimelineInterval == null)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            ViewInterval ??= new Interval(TimelineInterval);

            const int bracketGap = 4;
            var pixelViewInterval = ViewInterval.Scale(Hscale);
            var pixelBar = new Interval(Width);
            var lowerBracketInterval = new Interval(pixelViewInterval.Start - bracketGap, pixelViewInterval.Start + bracketGap).MinMax(pixelBar);
            var upperBracketInterval = new Interval(pixelViewInterval.End - bracketGap, pixelViewInterval.End + bracketGap).MinMax(pixelBar);

            DragTarget = e.X switch
            {
                _ when lowerBracketInterval.Contains(e.X) => DragTargetType.LowerBound,
                _ when upperBracketInterval.Contains(e.X) => DragTargetType.UpperBound,
                _ when pixelViewInterval.Contains(e.X) => DragTargetType.Interval,
                _ => null
            };

            if (DragTarget == null)
            {
                return;
            }

            HideHovered = true;
            DragPos = e.X;
        }

        private void TimelineCursorZoom_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            DragTarget = null;
            HideHovered = false;
        }

        private void TimelineCursorZoom_HoveredPosChanged(float? time, int? pixelValue)
        {
            toolTip1.SetToolTip(this, time == null ? "" : $"Time: {time.Value:####0.##} s.");
        }

        #endregion
    }
}