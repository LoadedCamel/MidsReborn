using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.SupportsTransparentBackColor | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            var initializing = new frmInitializing();
            initializing.ShowDialog();
        }
    }
}
