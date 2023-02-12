using System.Diagnostics;
using System.Windows.Forms;
using Mids_Reborn.Core.Base.Master_Classes;
using MRBResourceLib;

namespace Mids_Reborn.Forms
{
    public partial class frmEditComment : Form
    {
        private string? OriginalBuildComment;

        public frmEditComment()
        {
            InitializeComponent();
        }
        private void frmEditComment_Load(object sender, System.EventArgs e)
        {
            Icon = Resources.MRB_Icon_Concept;
            OriginalBuildComment = MidsContext.Character.CurrentBuild.Comment == null
                ? null
                : MidsContext.Character.CurrentBuild.Comment.Trim();

            textBox1.Text = MidsContext.Character.CurrentBuild.Comment == null
                ? ""
                : MidsContext.Character.CurrentBuild.Comment.Trim();
        }

        private void btnOk_Click(object sender, System.EventArgs e)
        {
            Debug.WriteLine(textBox1.Text.Trim());

            MidsContext.Character.CurrentBuild.Comment = textBox1.Text.Trim();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            var comment = textBox1.Text.Trim();
            var origComment = OriginalBuildComment ?? "";
            if (comment != origComment)
            {
                var c = MessageBox.Show("Comment has been modified.\r\nIgnore anyway?", "Discard changes",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (c == DialogResult.OK)
                {
                        MidsContext.Character.CurrentBuild.Comment = comment;
                        DialogResult = DialogResult.OK;
                        Close();

                        return;
                }
            }

            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
