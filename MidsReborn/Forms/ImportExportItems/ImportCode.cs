using System;
using System.Windows.Forms;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;


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
            //StylizeWindow(Handle, Color.Silver, Color.Black, Color.Silver);
            CenterToParent();
        }

        private void BdImport_Click(object sender, EventArgs e)
        {
            var chunkData = tbChunkBox.Text.Trim();
            ImportClassificationResult = DataClassifier.ClassifyAndExtractData(chunkData);

            if (ImportClassificationResult.IsValid && ImportClassificationResult.Type != DataClassifier.DataType.UnkBase64)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else if (ImportClassificationResult is { IsValid: true, Type: DataClassifier.DataType.UnkBase64 })
            {
                var result = MessageBoxEx.ShowDialog("This may be a MBD chunk that is missing its header.\r\nDo you wish to import it anyways?", "Unknown Base64 detected!", MessageBoxEx.MessageBoxExButtons.YesNo, MessageBoxEx.MessageBoxExIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    DialogResult = DialogResult.None;
                }
            }
            else
            {
                MessageBoxEx.Show("Please double check the data chunk and try again.", "Invalid chunk data detected!", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                DialogResult = DialogResult.None;
            }
        }
    }
}
