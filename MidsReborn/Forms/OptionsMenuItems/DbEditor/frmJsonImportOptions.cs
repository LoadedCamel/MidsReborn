using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmJsonImportOptions : Form
    {
        public bool OverrideStaticIndex { get; private set; }
        public bool OverrideBasicData { get; private set; }
        public string FileName { get; private set; }

        public frmJsonImportOptions()
        {
            FileName = "";
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            var dlgOpen = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "json",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false
            };

            if (dlgOpen.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            txtFilename.Text = dlgOpen.FileName;
            FileName = dlgOpen.FileName;
        }

        private void frmJsonImportOptions_Load(object sender, System.EventArgs e)
        {
            chkStaticIndex.Checked = false;
            chkBasicData.Checked = false;
            FileName = "";
            OverrideStaticIndex = false;
            OverrideBasicData = false;
            txtFilename.Text = "";
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            FileName = "";
            Close();
        }

        private void btnOkay_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}