using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class frmEnhMiniPick : Form
    {
        private frmEnhMiniPick()
        {
            InitializeComponent();
            Name = nameof(frmEnhMiniPick);
        }

        public static int MezPicker(int startIndex)
        {
            var eMez = Enums.eMez.None;
            using var frmEnhMiniPick = new frmEnhMiniPick();
            var names = Enum.GetNames(eMez.GetType());
            var num1 = names.Length - 1;
            for (var index = 1; index <= num1; ++index)
                frmEnhMiniPick.lbList.Items.Add(names[index]);
            if ((startIndex > -1) & (startIndex < frmEnhMiniPick.lbList.Items.Count))
                frmEnhMiniPick.lbList.SelectedIndex = startIndex - 1;
            else
                frmEnhMiniPick.lbList.SelectedIndex = 0;
            frmEnhMiniPick.ShowDialog();
            return frmEnhMiniPick.lbList.SelectedIndex + 1;
        }

        private void btnOK_Click(object sender, EventArgs e)

        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void lbList_DoubleClick(object sender, EventArgs e)

        {
            btnOK_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }
    }
}