using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Forms
{
    public partial class frmTemplateManage : Form
    {
        private ManageMode Mode;
        private frmMain _ParentForm;

        public enum ManageMode
        {
            Save,
            Load
        }

        public frmTemplateManage(ManageMode mode, frmMain parentForm)
        {
            InitializeComponent();
            Mode = mode;
            _ParentForm = parentForm;
        }

        private void frmTemplateManage_Load(object sender, EventArgs e)
        {
            // Hide tabs
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;

            tabControl1.TabIndex = Mode switch
            {
                ManageMode.Save => 0,
                _ => 1
            };

            btnModeSwitch.Text = Mode == ManageMode.Save
                ? "Template selection >"
                : "< Template creation";

            FillComboBoxes();
        }

        private void FillComboBoxes()
        {
            var names = GetTemplateNames();
            if (Mode == ManageMode.Load)
            {
                cbTemplateName.BeginUpdate();
                cbTemplateName.Items.Clear();
                cbTemplateName.Items.Add($"(none){(MidsContext.Config.ActiveTemplate == null ? " [active]" : "")}");
                foreach (var name in names)
                {
                    cbTemplateName.Items.Add($"{name}{(MidsContext.Config.ActiveTemplate == name ? " [active]" : "")}");
                    
                }

                cbTemplateName.EndUpdate();
                if (MidsContext.Config.Templates != null)
                {
                    if (MidsContext.Config.ActiveTemplate == null)
                    {
                        cbTemplateName.SelectedIndex = 0;
                    }
                    else
                    {
                        var idx = MidsContext.Config.Templates.TryFindIndex(e => e.Name == MidsContext.Config.ActiveTemplate);
                        cbTemplateName.SelectedIndex = idx < 0 ? 0 : idx + 1;
                    }
                }

                return;
            }

            cbTemplateSelect.BeginUpdate();
            cbTemplateSelect.Items.Clear();
            foreach (var name in names)
            {
                cbTemplateSelect.Items.Add(name);
            }

            cbTemplateSelect.EndUpdate();
            if (names.Length > 0)
            {
                cbTemplateSelect.SelectedIndex = 0;
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
            label3.Enabled = radioButton1.Checked;
            tbNewTemplateName.Enabled = radioButton1.Checked;
            label4.Enabled = !radioButton1.Checked;
            cbTemplateSelect.Enabled = !radioButton1.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
            label3.Enabled = radioButton1.Checked;
            tbNewTemplateName.Enabled = radioButton1.Checked;
            label4.Enabled = !radioButton1.Checked;
            cbTemplateSelect.Enabled = !radioButton1.Checked;
        }

        private string[] GetTemplateNames()
        {
            return MidsContext.Config.Templates == null
                ? []
                : MidsContext.Config.Templates
                    .Where(f => f.Name != null)
                    .Select(f => f.Name)
                    .Cast<string>()
                    .ToArray();
        }

        private void btnModeSwitch_Click(object sender, EventArgs e)
        {
            Mode = Mode == ManageMode.Save ? ManageMode.Load : ManageMode.Save;
            tabControl1.SelectTab(Mode == ManageMode.Save ? 0 : 1);
            btnModeSwitch.Text = Mode == ManageMode.Save ? "Template selection >" : "< Template creation";

            FillComboBoxes();
        }

        private void btnCreateSelected_Click(object sender, EventArgs e)
        {
            var overwriteMode = radioButton2.Checked;
            var useAsNewDefault = chkSetNewAsActive.Checked;

            if (!overwriteMode)
            {
                var targetName = tbNewTemplateName.Text.Trim();
                if (string.IsNullOrWhiteSpace(targetName))
                {
                    return;
                }

                if (Regex.IsMatch(targetName, @"^[^a-z]none[^a-z]$", RegexOptions.IgnoreCase))
                {
                    return;
                }

                if (MidsContext.Config?.Templates?.Any(f =>
                        f.Name?.Equals(targetName, StringComparison.InvariantCultureIgnoreCase) == true) == true)
                {
                    return;
                }

                var newTemplate = BuildTemplate.SnapshotBuild(_ParentForm);
                newTemplate.Name = targetName;

                MidsContext.Config.Templates ??= [];
                MidsContext.Config.Templates.Add(newTemplate);

                if (useAsNewDefault)
                {
                    MidsContext.Config.ActiveTemplate = targetName;
                }
            }
            else
            {
                var names = GetTemplateNames();
                var nameIdx = cbTemplateSelect.SelectedIndex;
                if (nameIdx < 0 || nameIdx >= names.Length)
                {
                    return;
                }

                if (MidsContext.Config.Templates == null)
                {
                    return;
                }

                var name = MidsContext.Config.Templates[nameIdx].Name;
                MidsContext.Config.Templates[nameIdx] = BuildTemplate.SnapshotBuild(_ParentForm);
                MidsContext.Config.Templates[nameIdx].Name = name;
            }

            FillComboBoxes();
        }

        private void btnUseTemplate_Click(object sender, EventArgs e)
        {
            var names = GetTemplateNames();
            var nameIdx = cbTemplateName.SelectedIndex;
            Debug.WriteLine($"btnUseTemplate_Click(): NameIdx={nameIdx}");
            if (nameIdx < 0 || nameIdx > names.Length)
            {
                return;
            }

            if (MidsContext.Config.Templates == null)
            {
                return;
            }

            MidsContext.Config.ActiveTemplate = nameIdx == 0
                ? null
                : names[nameIdx - 1];

            Debug.WriteLine($"btnUseTemplate_Click(): ActiveTemplate={MidsContext.Config.ActiveTemplate}");

            FillComboBoxes();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var names = GetTemplateNames();
            var nameIdx = cbTemplateSelect.SelectedIndex;
            if (nameIdx < 0 || nameIdx >= names.Length)
            {
                return;
            }

            if (MidsContext.Config.Templates == null)
            {
                return;
            }

            var targetName = MidsContext.Config.Templates[nameIdx].Name;
            if (targetName == MidsContext.Config.ActiveTemplate)
            {
                MidsContext.Config.ActiveTemplate = null;
            }

            MidsContext.Config.Templates = MidsContext.Config.Templates
                .Where(f => f.Name != targetName)
                .ToList();

            FillComboBoxes();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
