using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.UpdateSystem
{
    public partial class PatchQuery : Form
    {
        public PatchQuery(frmMain parent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            Load += OnQuery_Load;
            InitializeComponent();
        }

        private void OnQuery_Load(object sender, EventArgs e)
        {
            CenterToParent();
        }
    }
}
