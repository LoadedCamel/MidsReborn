using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms.ShareItems
{
    public partial class InfoGraphicPreview : Form
    {
        public Image? Image;

        public InfoGraphicPreview()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            var gfxBytes = InfoGraphic.Generate();
            using var stream = new MemoryStream(gfxBytes);
            var image = Image.FromStream(stream);
            pictureBox1.Image = image;
        }

        private void On_Continue(object sender, EventArgs e)
        {
            Image = pictureBox1.Image;
        }
    }
}
