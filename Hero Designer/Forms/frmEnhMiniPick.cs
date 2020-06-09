
using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace Hero_Designer
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
            Enums.eMez eMez = Enums.eMez.None;
            using frmEnhMiniPick frmEnhMiniPick = new frmEnhMiniPick();
            string[] names = Enum.GetNames(eMez.GetType());
            int num1 = names.Length - 1;
            for (int index = 1; index <= num1; ++index)
                frmEnhMiniPick.lbList.Items.Add(names[index]);
            if (startIndex > -1 & startIndex < frmEnhMiniPick.lbList.Items.Count)
                frmEnhMiniPick.lbList.SelectedIndex = startIndex - 1;
            else
                frmEnhMiniPick.lbList.SelectedIndex = 0;
            frmEnhMiniPick.ShowDialog();
            return frmEnhMiniPick.lbList.SelectedIndex + 1;
        }

        void btnOK_Click(object sender, EventArgs e)

        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        void lbList_DoubleClick(object sender, EventArgs e)

        {
            btnOK_Click(RuntimeHelpers.GetObjectValue(sender), e);
        }
    }
}