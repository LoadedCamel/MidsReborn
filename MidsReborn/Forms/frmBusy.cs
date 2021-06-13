using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmBusy : Form
    {
        public frmBusy()
        {
            Closing += Busy_Closing;
            InitializeComponent();
            Icon = Resources.reborn;
        }

        public void SetMessage(string iMsg)
        {
            if (Message.Text == iMsg)
                return;
            Message.Text = iMsg;
            Refresh();
        }

        public void SetTitle(string iTitle)
        {
            if (Text == iTitle)
                return;
            Text = iTitle;
        }

        public void Completed()
        {
            Close();
        }

        private void Busy_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = false;
            Dispose();
        }
    }
}