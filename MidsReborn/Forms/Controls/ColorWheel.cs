using System;
using System.Drawing;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Mids_Reborn.Forms.Controls
{
    public partial class ColorWheel : UserControl
    {
        internal static class HSLConverter
        {
            public struct HSLColor
            {
                public float H;
                public float S;
                public float L;

                public override string ToString()
                {
                    return $"({H}, {S}, {L})";
                }
            }

            public static HSLColor ToHSL(Color rgb)
            {
                var skColor = new SKColor(rgb.R, rgb.G, rgb.B);
                skColor.ToHsl(out var h, out var s, out var l);

                return new HSLColor { H = h, S = s, L = l };
            }

            public static Color ToRGB(HSLColor hsl)
            {
                var skColor = SKColor.FromHsl(hsl.H, hsl.S, hsl.L);

                return skColor.ToDrawingColor();
            }
        }

        public delegate void SelectionChangedHandler(object? sender, SelectedValues selected);
        public event SelectionChangedHandler? SelectionChanged;
        public struct RgbValues
        {
            public int R { get; set; }
            public int G { get; set; }
            public int B { get; set; }

            public RgbValues(int red, int green, int blue)
            {
                R = red;
                G = green;
                B = blue;
            }
        }

        public class SelectedValues
        {
            public Color Color { get; set; } = Color.White;
            public string Hex => $@"#{ColorTranslator.ToWin32(Color):X8}";
            public RgbValues Rgb => new(Color.R, Color.G, Color.B);
        }

        private SelectedValues Selected { get; } = new();
        private Color SelectedColorMaxBrightness;

        public ColorWheel()
        {
            SetStyle(ControlStyles.DoubleBuffer, true);
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            colorSelection.BackColor = Selected.Color;
        }

        private void ColorPicker_MouseMove(object sender, MouseEventArgs e)
        {
            if (colorPicker.Image is not Bitmap pixelData) return;

            var radius = pixelData.Height / 2f;
            var pointRadius = Math.Sqrt(Math.Pow(Math.Abs(e.X - radius), 2) + Math.Pow(Math.Abs(e.Y - radius), 2));
            if (pointRadius > radius)
            {
                colorPreview.BackColor = Color.Transparent;

                return;
            }

            var colorData = pixelData.GetPixel(e.X, e.Y);
            var adjustedColor = ControlPaint.Dark(colorData, brightnessSelector.Brightness / 100f);
            colorPreview.BackColor = adjustedColor;
        }

        private void ColorPicker_MouseDown(object sender, MouseEventArgs e)
        {
            if (colorPicker.Image is not Bitmap pixelData) return;

            var radius = pixelData.Height / 2f;
            var pointRadius = Math.Sqrt(Math.Pow(Math.Abs(e.X - radius), 2) + Math.Pow(Math.Abs(e.Y - radius), 2));
            if (pointRadius > radius)
            {
                colorPreview.BackColor = Color.Transparent;

                return;
            }

            var colorData = pixelData.GetPixel(e.X, e.Y);
            var adjustedColor = ControlPaint.Dark(colorData, brightnessSelector.Brightness / 100f);
            colorSelection.BackColor = adjustedColor;
            Selected.Color = adjustedColor;
            SelectedColorMaxBrightness = ControlPaint.Dark(colorData, 0);
            SelectionChanged?.Invoke(this, Selected);
        }

        private void brightnessSelector_PositionChanged(object sender, float percentage)
        {
            var hslColor = HSLConverter.ToHSL(SelectedColorMaxBrightness);
            var adjustedColor = HSLConverter.ToRGB(hslColor with {L = hslColor.L * ((100 - percentage) / 100f)});
            colorSelection.BackColor = adjustedColor;
            Selected.Color = adjustedColor;
            SelectionChanged?.Invoke(this, Selected);
        }
    }
}
