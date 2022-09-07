using System;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using MRBResourceLib;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmConditionalAttributeSearch : Form
    {
        public struct ConditionalSearchTerms
        {
            public string PowerName;
            public string AtGroup;
        }

        public ConditionalSearchTerms SearchTerms;
        private readonly string[] IgnoredClasses;
        public frmConditionalAttributeSearch()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            InitializeComponent();
            Icon = Resources.MRB_Icon_Concept;

            IgnoredClasses = Archetype.GetNpcClasses();
        }

        private void frmConditionalAttributeSearch_Load(object sender, EventArgs e)
        {
            CenterToParent(); 
            cbAtGroup.SuspendLayout();
            cbAtGroup.Items.Clear();
            cbAtGroup.Items.Add("Any");
            cbAtGroup.Items.Add("Inherent");
            foreach (var at in DatabaseAPI.Database.Classes)
            {
                var atName = at.DisplayName;
                if (Array.IndexOf(IgnoredClasses, atName) > -1) continue;
                cbAtGroup.Items.Add(at.DisplayName);
            }
            cbAtGroup.ResumeLayout();
            textBoxPowerName.Text = "";
            textBoxPowerName.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SearchTerms = new ConditionalSearchTerms
            {
                PowerName = textBoxPowerName.Text,
                AtGroup = cbAtGroup.SelectedIndex < 0
                    ? ""
                    : cbAtGroup.Items[cbAtGroup.SelectedIndex].ToString()
            };
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            SearchTerms = new ConditionalSearchTerms
            {
                PowerName = "",
                AtGroup = ""
            };
            Close();
        }

        private void textBoxPowerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter) return;

            btnOk_Click(this, new EventArgs());
        }

        private void cbAtGroup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData != Keys.Enter) return;

            btnOk_Click(this, new EventArgs());
        }
    }
}