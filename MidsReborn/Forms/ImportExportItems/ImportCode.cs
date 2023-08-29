using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;
using static Mids_Reborn.Core.Utils.WinApi;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class ImportCode : Form
    {
        internal string? Data;

        public ImportCode()
        {
            InitializeComponent();
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
            Load += OnLoad;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            StylizeWindow(Handle, Color.Silver, Color.Black, Color.Silver);
            CenterToParent();
        }

        private void BdImport_Click(object sender, EventArgs e)
        {
            if (!Helpers.ValidShareData(tbChunkBox.Text))
            {
                errorProvider1.Icon = new Icon(SystemIcons.Error, new Size(32, 32));
                errorProvider1.SetIconAlignment(bdImport, ErrorIconAlignment.BottomRight);
                errorProvider1.SetIconPadding(bdImport, 5);
                errorProvider1.SetError(bdImport, "Invalid or incomplete share data detected, double check the data you pasted above.");
                return;
            }

            Data = ShareGenerator.BuildDataFromChunk(tbChunkBox.Text);
            DialogResult = DialogResult.Continue;
            Close();
        }
    }
}
