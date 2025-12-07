using MRBResourceLib;

namespace Mids_Reborn.UI.Forms
{
    public partial class FrmInputLevel : Form
    {
        private readonly bool LongFormat;
        private readonly MainWindow2 MyParent;

        public FrmInputLevel(MainWindow parent, bool lf)
        {
            InitializeComponent();
            Name = nameof(FrmInputLevel);
            Icon = Resources.MRB_Icon_Concept;
            //MyParent = parent;
            LongFormat = lf;
        }

        public FrmInputLevel(MainWindow2 parent, bool lf)
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
                MyParent.SmlRespecLong(num - 1);
            }
            else
            {
                MyParent.SmlRespecShort(num - 1);
            }

            Close();
        }

        private void udLevel_Leave(object sender, EventArgs e)
        {
            udLevel.Validate();
        }
    }
}