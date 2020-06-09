using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Hero_Designer
{
    public partial class frmZStatus : Form
    {
        Label lblStatus1;
        Label lblStatus2;
        Label lblTitle;
        PictureBox PictureBox1;

        public frmZStatus()
        {
            VisibleChanged += frmZStatus_VisibleChanged;
            InitializeComponent();
            Name = nameof(frmZStatus);
            var componentResourceManager = new ComponentResourceManager(typeof(frmZStatus));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            PictureBox1.Image = (Image)componentResourceManager.GetObject("PictureBox1.Image");
        }

        void frmZStatus_VisibleChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        private void SetText1(string text)
        {
            if (lblStatus1.InvokeRequired)
                Invoke(new SetTextCallback(SetText1), text);
            else
                lblStatus1.Text = text;
        }

        public void SetText2(string text)
        {
            if (lblStatus2.InvokeRequired)
                Invoke(new SetTextCallback(SetText1), text);
            else
                lblStatus2.Text = text;
        }

        private delegate void SetTextCallback(string text);
    }
}