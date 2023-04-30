using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public class BorderPanel : Panel
    {
        public enum BorderToDraw
        {
            All,
            Left,
            Top,
            Right,
            Bottom
        }

        public BorderToDraw BorderChosen { get; set; }
        public Color BorderColor { get; set; }
        public int BorderThickness { get; set; }
        public new ButtonBorderStyle BorderStyle { get; set; }

        public BorderPanel(BorderToDraw whichBorder, int thickness, Color color, ButtonBorderStyle style)
        {
           SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
           BorderChosen = whichBorder;
           BorderColor = color;
           BorderStyle = style;
           BorderThickness = thickness;
        }

        public BorderPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true); 
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            switch (BorderChosen)
            {
                case BorderToDraw.All:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        BorderColor, BorderThickness, BorderStyle,
                        BorderColor, BorderThickness, BorderStyle,
                        BorderColor, BorderThickness, BorderStyle,
                        BorderColor, BorderThickness, BorderStyle);
                    break;
                case BorderToDraw.Left:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        BorderColor, BorderThickness, BorderStyle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case BorderToDraw.Top:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        BorderColor, BorderThickness, BorderStyle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case BorderToDraw.Right:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        BorderColor, BorderThickness, BorderStyle,
                        Color.Empty, 0, ButtonBorderStyle.None);
                    break;
                case BorderToDraw.Bottom:
                    ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        Color.Empty, 0, ButtonBorderStyle.None,
                        BorderColor, BorderThickness, BorderStyle);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
