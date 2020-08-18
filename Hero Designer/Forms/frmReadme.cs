using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using midsControls;

namespace Hero_Designer.Forms
{
    public partial class frmReadme : Form
    {
        private readonly string myFile;
        private int btnY;
        private bool Loading;
        private Point mouse_offset;
        private PictureBox pbBackground;
        private PictureBox pbBottom;
        private RichTextBox rtfRead;
        private int rtH;
        private int rtW;

        public frmReadme(string iFile)
        {
            Loading = true;
            myFile = "";
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmReadme));
            Icon = (Icon) componentResourceManager.GetObject("$this.Icon");
            pbBottom.Image = (Image) componentResourceManager.GetObject("pbBottom.Image");
            pbBackground.Image = (Image) componentResourceManager.GetObject("pbBackground.Image");
            myFile = iFile;
        }

        internal ImageButton BtnClose { get; private set; }

        private void All_MouseMove(MouseEventArgs e)

        {
            if (e.Button != MouseButtons.Left)
                return;
            var mousePosition = MousePosition;
            mousePosition.Offset(mouse_offset.X, mouse_offset.Y);
            Location = mousePosition;
        }

        private void btnClose_ButtonClicked()
        {
            Close();
        }

        private void btnClose_Load(object sender, EventArgs e)
        {
        }

        private void frmReadme_Load(object sender, EventArgs e)

        {
            rtW = Size.Width - (rtfRead.Width + rtfRead.Left);
            rtH = Size.Height - (rtfRead.Height + rtfRead.Top);
            BtnClose.KnockoutLocationPoint = new Point(BtnClose.Left, BtnClose.Top - pbBottom.Top);
            BtnClose.KnockoutPB = pbBottom;
            BtnClose.Refresh();
            btnY = Size.Height - BtnClose.Top;
            try
            {
                rtfRead.LoadFile(myFile);
            }
            catch (Exception)
            {
                rtfRead.Text = "Unable to find " + myFile + "!";
            }

            Loading = false;
        }

        private void frmReadme_MouseDown(object sender, MouseEventArgs e)

        {
            pbBackground_MouseDown(RuntimeHelpers.GetObjectValue(sender), e);
        }

        private void frmReadme_MouseMove(object sender, MouseEventArgs e)

        {
            All_MouseMove(e);
        }

        private void frmReadme_Resize(object sender, EventArgs e)

        {
            if (Loading)
                return;
            var rtfRead1 = rtfRead;
            var size = Size;
            var num1 = size.Height - (rtH + rtfRead.Top);
            rtfRead1.Height = num1;
            var rtfRead2 = rtfRead;
            size = Size;
            var num2 = size.Width - (rtW + rtfRead.Left);
            rtfRead2.Width = num2;
            var btnClose1 = BtnClose;
            size = Size;
            var num3 = size.Height - btnY;
            btnClose1.Top = num3;
            var btnClose2 = BtnClose;
            size = Size;
            var num4 = (int) Math.Round((size.Width - BtnClose.Width) / 2.0);
            btnClose2.Left = num4;
        }

        [DebuggerStepThrough]
        private void pbBackground_Click(object sender, EventArgs e)

        {
        }

        private void pbBackground_MouseDown(object sender, MouseEventArgs e)

        {
            mouse_offset = new Point(-e.X, -e.Y);
        }

        private void pbBackground_MouseMove(object sender, MouseEventArgs e)

        {
            All_MouseMove(e);
        }

        private void pbBottom_MouseDown(object sender, MouseEventArgs e)

        {
            mouse_offset = new Point(-e.X, -pbBottom.Top + -e.Y);
        }

        private void pbBottom_MouseMove(object sender, MouseEventArgs e)

        {
            All_MouseMove(e);
        }
    }
}