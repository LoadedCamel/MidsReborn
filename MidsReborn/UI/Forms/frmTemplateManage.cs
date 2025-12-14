using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;

namespace Mids_Reborn.Forms
{
    public partial class frmTemplateManage : Form
    {
        public frmTemplateManage()
        {
            InitializeComponent();
        }

        private void frmTemplateManage_Load(object sender, EventArgs e)
        {
            var isUsingTemplate = MidsContext.Config?.ActiveTemplate != null && !string.IsNullOrWhiteSpace(MidsContext.Config.ActiveTemplate);
            radioButton1.Checked = !isUsingTemplate;
            radioButton2.Checked = isUsingTemplate;
            txtBuildName.Enabled = radioButton2.Checked;
            btnBrowse.Enabled = radioButton2.Checked;
            txtBuildName.Text = !isUsingTemplate ? "" : Path.GetFileName(MidsContext.Config.ActiveTemplate);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
            txtBuildName.Enabled = radioButton2.Checked;
            btnBrowse.Enabled = radioButton2.Checked;

            if (radioButton1.Checked)
            {
                MidsContext.Config.ActiveTemplate = null;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
            txtBuildName.Enabled = radioButton2.Checked;
            btnBrowse.Enabled = radioButton2.Checked;

            if (!string.IsNullOrWhiteSpace(txtBuildName.Text))
            {
                MidsContext.Config.ActiveTemplate = txtBuildName.Text;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using var f = new OpenFileDialog();
            f.Title = @"Select template build";
            f.Filter = @"Mids builds (*.mbd)|*.mbd|Mids legacy builds (*.mxd)|*.mxd";
            f.DefaultExt = "mbd";
            f.FilterIndex = 0;
            f.CheckFileExists = true; // ??
            f.CheckPathExists = true; // ??
            f.Multiselect = false;

            if (!string.IsNullOrWhiteSpace(MidsContext.Config?.BuildsPath))
            {
                f.InitialDirectory = MidsContext.Config.BuildsPath;
            }

            var ret = f.ShowDialog(this);
            if (ret != DialogResult.OK)
            {
                return;
            }

            if (f.FileName.EndsWith(".mxd", StringComparison.InvariantCultureIgnoreCase))
            {
                Stream? mStream = null;
                var loadedPowers = MainModule.MidsController.Toon == null
                    ? []
                    : MainModule.MidsController.Toon.LoadFromFileLight(f.FileName);

                var validPowers = loadedPowers != null && loadedPowers
                    .Where(x => x is { Power: not null })
                    .Select(x => x?.Power)
                    .All(x => x?.GetPowerSet()?.SetType is not (Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary or Enums.ePowerSetType.Ancillary));

                if (!validPowers)
                {
                    MessageBox.Show(@"A template build cannot contain powers from either primary, secondary, or epic powersets.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                MidsContext.Config.ActiveTemplate = f.FileName;
                txtBuildName.Text = Path.GetFileName(f.FileName);
            }
            else if (f.FileName.EndsWith(".mbd", StringComparison.InvariantCultureIgnoreCase))
            {
                var loadedResult = BuildManager.Instance.LoadFromFileLight(f.FileName);
                if (!string.IsNullOrEmpty(loadedResult.ErrorMsg))
                {
                    MessageBox.Show(loadedResult.ErrorMsg, @"Error loading build", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                    return;
                }

                var validPowers = loadedResult.BuildData?.PowerEntries
                    .Where(x => x != null && !string.IsNullOrWhiteSpace(x.PowerName))
                    .Select(x => DatabaseAPI.GetPowerByFullName(x?.PowerName))
                    .All(x => x?.GetPowerSet()?.SetType is not (Enums.ePowerSetType.Primary or Enums.ePowerSetType.Secondary or Enums.ePowerSetType.Ancillary));

                if (validPowers != true)
                {
                    MessageBox.Show(@"A template build cannot contain powers from either primary, secondary, or epic powersets.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                MidsContext.Config.ActiveTemplate = f.FileName;
                txtBuildName.Text = Path.GetFileName(f.FileName);
            }
            else
            {
                MessageBox.Show(@"Can only use .mxd or .mbd builds as a template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
