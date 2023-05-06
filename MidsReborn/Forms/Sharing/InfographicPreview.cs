using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Sharing
{
    public partial class InfographicPreview : Form
    {
        public InfographicPreview()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            var imageBytes = Convert.FromBase64String(InfoGraphic.Generate());
            using var stream = new MemoryStream(imageBytes);
            var image = Image.FromStream(stream);
            pictureBox1.Image = image;
        }
    }
}
