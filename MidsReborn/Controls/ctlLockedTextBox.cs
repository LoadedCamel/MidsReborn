using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Controls
{
    public partial class ctlLockedTextBox : UserControl
    {
        public ctlLockedTextBox()
        {
            InitializeComponent();
            ctlOutlinedLabel1.TextChanged += OnTextChanged;
            //ctlOutlinedLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //ctlOutlinedLabel1.Location = new Point(ctlOutlinedLabel1.Location.X + 10, ctlOutlinedLabel1.Location.Y + 4);
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (Image != null)
            {
                Bitmap bitmap = new Bitmap(Image, new Size(16,16));
                pictureBox1.Image = bitmap;
                pictureBox2.Image = bitmap;
                pictureBox1.Refresh();
                pictureBox2.Refresh();
            }
        }

        public override Color BackColor { get; set; }
        public override Color ForeColor { get; set; }

        public override string Text
        {
            get => ctlOutlinedLabel1.Text;
            set => ctlOutlinedLabel1.Text = value;
        }

        public Image Image { get; set; }
    }
}
