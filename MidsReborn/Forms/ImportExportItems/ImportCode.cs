using System;
using System.Drawing;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;
using static Mids_Reborn.Core.Utils.WinApi;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class ImportCode : Form
    {
        internal DataClassifier.ClassificationResult ImportClassificationResult { get; private set; }

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
            var chunkData = tbChunkBox.Text;
            ImportClassificationResult = DataClassifier.ClassifyAndExtractData(chunkData);

            if (ImportClassificationResult.IsValid)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBoxEx.Show("Invalid chunk data detected! Please double check the data chunk and try again.", "Unable To Process Chunk", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
