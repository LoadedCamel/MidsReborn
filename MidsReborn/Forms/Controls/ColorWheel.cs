using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class ColorWheel : UserControl
    {
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

        public ColorWheel()
        {
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
            var colorData = pixelData.GetPixel(e.X, e.Y);
            colorPreview.BackColor = colorData;
        }

        private void ColorPicker_MouseDown(object sender, MouseEventArgs e)
        {
            if (colorPicker.Image is not Bitmap pixelData) return;
            var colorData = pixelData.GetPixel(e.X, e.Y);
            colorSelection.BackColor = colorData;
            Selected.Color = colorData;
            SelectionChanged?.Invoke(this, Selected);
        }
    }
}
