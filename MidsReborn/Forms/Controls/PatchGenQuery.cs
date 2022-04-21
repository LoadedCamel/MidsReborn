using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using mrbBase.Base.Extensions;
using mrbBase.Utils;

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

        private void OnLoad(object sender, EventArgs e)
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
