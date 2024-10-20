using MRBResourceLib;
using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class FrmInputLevel : Form
    {
        private readonly bool LongFormat;

        private readonly bool Mode2;

        private readonly frmMain MyParent;

        public FrmInputLevel(frmMain parent, bool lf, bool mode2)
        {
            InitializeComponent();
            Name = nameof(FrmInputLevel);
            Icon = Resources.MRB_Icon_Concept;
            MyParent = parent;
            LongFormat = lf;
            Mode2 = mode2;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var num = (int)Math.Round(udLevel.Value);
            num = Math.Min((int)udLevel.Maximum, Math.Max(num, (int)udLevel.Minimum));

            if (LongFormat)
            {
                MyParent.smlRespecLong(num - 1, Mode2);
            }
            else
            {
                MyParent.smlRespecShort(num - 1, Mode2);
            }

            Close();
        }

        private void udLevel_Leave(object sender, EventArgs e)
        {
            udLevel.Validate();
        }
    }
}