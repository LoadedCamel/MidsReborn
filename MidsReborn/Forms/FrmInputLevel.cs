using MRBResourceLib;
using System;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class FrmInputLevel : Form
    {
        private readonly bool LongFormat;
        private readonly frmMain MyParent;

        public FrmInputLevel(frmMain parent, bool lf)
        {
            InitializeComponent();
            Name = nameof(FrmInputLevel);
            Icon = Resources.MRB_Icon_Concept;
            MyParent = parent;
            LongFormat = lf;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var num = (int)Math.Round(udLevel.Value);
            num = Math.Min((int)udLevel.Maximum, Math.Max(num, (int)udLevel.Minimum));

            if (LongFormat)
            {
                MyParent.smlRespecLong(num - 1);
            }
            else
            {
                MyParent.smlRespecShort(num - 1);
            }

            Close();
        }

        private void udLevel_Leave(object sender, EventArgs e)
        {
            udLevel.Validate();
        }
    }
}