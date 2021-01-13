using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class UpdateQuery : Form
    {
        public string Type  { get; set; }
        public UpdateQuery(frmMain parent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            Load += OnQuery_Load;
            InitializeComponent();
        }

        private void OnQuery_Load(object sender, EventArgs e)
        {
            queryLabel.Text = $@"There is a new {Type} update available.";
            CenterToParent();
        }
    }
}
