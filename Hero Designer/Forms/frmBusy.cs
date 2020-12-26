using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmBusy : Form
    {
        private Label Message;

        public frmBusy()
        {
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmBusy));
            Icon = Resources.reborn;
            Name = nameof(frmBusy);
        }

        [DebuggerStepThrough]
        public void SetMessage(string iMsg)
        {
            if (Message.Text == iMsg)
                return;
            Message.Text = iMsg;
            Refresh();
        }
    }
}