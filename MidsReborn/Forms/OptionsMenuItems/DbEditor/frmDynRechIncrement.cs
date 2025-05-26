using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDynRechIncrement : Form
    {
        public float IncrementValue { get; private set; }

        public frmDynRechIncrement()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            var target = (TextBox)sender;

            // Filter out keyboard input: only allow KeyChars used in positive numbers.
            // This won't sanitize copy-pasted text, and it will need an extra validation step.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // Only allow one decimal point
            if (e.KeyChar == '.' && target.Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void frmDynRechIncrement_Load(object sender, EventArgs e)
        {
            textBox1.Select();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            var target = (TextBox)sender;

            var validValue = float.TryParse(target.Text.Trim(), NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out var val);
            validValue = validValue && val > 0;

            btnOk.Enabled = validValue;
            if (!validValue)
            {
                return;
            }

            IncrementValue = val;
        }
    }
}
