using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Hero_Designer.Forms
{
    public partial class FrmInputLevel : Form
    {
        private readonly bool longFormat;

        private readonly bool mode2;

        private readonly frmMain myparent;

        public FrmInputLevel(frmMain iParent, bool iLF, bool iMode2)
        {
            InitializeComponent();
            Name = nameof(FrmInputLevel);
            var componentResourceManager = new ComponentResourceManager(typeof(FrmInputLevel));
            Icon = Resources.reborn;
            myparent = iParent;
            longFormat = iLF;
            mode2 = iMode2;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int num;
            if (Math.Abs(Conversion.Val(udLevel.Text) - Convert.ToDouble(udLevel.Value)) > float.Epsilon)
            {
                num = (int) Math.Round(Conversion.Val(udLevel.Text));
                if (decimal.Compare(new decimal(num), udLevel.Minimum) < 0)
                    num = Convert.ToInt32(udLevel.Minimum);
                if (decimal.Compare(new decimal(num), udLevel.Maximum) > 0)
                    num = Convert.ToInt32(udLevel.Maximum);
            }
            else
            {
                num = Convert.ToInt32(udLevel.Value);
            }

            if (longFormat)
                myparent.smlRespecLong(num - 1, mode2);
            else
                myparent.smlRespecShort(num - 1, mode2);
            Close();
        }

        private void FrmInputLevel_Load(object sender, EventArgs e)
        {
        }

        private void udLevel_Leave(object sender, EventArgs e)
        {
            udLevel.Validate();
        }
    }
}