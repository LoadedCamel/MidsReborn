using System;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Utils;

namespace Mids_Reborn.Forms.Controls
{
    public partial class PatchGenQuery : FormExt
    {
        public PatchGenQuery(string question, bool appGenVis = false, string dbText = "Yes", string cancelText = "No")
        {
            Load += OnLoad;
            InitializeComponent();
            lblQuestion.Text = question;
            btnAppGen.Visible = appGenVis;
            btnAppGen.Enabled = appGenVis;
            btnDbGen.Text = dbText;
            btnCancel.Text = cancelText;
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            CenterToParent();
        }

        private void btnAppGen_Click(object sender, EventArgs e)
        {
            GenResult = GenResult.Application;
            Close();
        }

        private void btnDbGen_Click(object sender, EventArgs e)
        {
            GenResult = GenResult.Database;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            GenResult = GenResult.Cancel;
            Close();
        }
    }
}
