using System;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmExpressionBuilder : Form
    {

        public string Duration { get; set; }
        public string Magnitude { get; set; }
        public string Probability { get; set; }

        private readonly IEffect _myEffect;
        private TextBox SelectedExpression { get; set; }
        private int InsertAtPos { get; set; }

        public frmExpressionBuilder(ICloneable fx)
        {
            InitializeComponent();
            _myEffect = fx.Clone() as IEffect;
            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            lbCommandVars.BeginUpdate();
            lbCommandVars.Items.Clear();
            lbCommandVars.DataSource = new BindingSource(Expressions.CommandsList, null);
            lbCommandVars.EndUpdate();
            tbDurationExp.Text = _myEffect.Expressions.Duration;
            tbMagExpr.Text = _myEffect.Expressions.Magnitude;
            tbProbExpr.Text = _myEffect.Expressions.Probability;
        }

        private void btnPowerInsert_Click(object sender, EventArgs e)
        {
            var powerSelector = new PowerSelector();
            var result = powerSelector.ShowDialog(this);
            if (result != DialogResult.OK) return;
            if (SelectedExpression != null)
            {
                SelectedExpression.Text = SelectedExpression.Text.Insert(InsertAtPos, powerSelector.SelectedPower.FullName);
            }
        }

        private void ExprOnGotFocus(object sender, EventArgs e)
        {
            SelectedExpression = sender as TextBox;
            if (SelectedExpression != null) InsertAtPos = SelectedExpression.SelectionStart;
        }

        private void ExprOnTextChanged(object sender, EventArgs e)
        {
            if (sender is not TextBox selectedBox) return;
            if (SelectedExpression != null && selectedBox == SelectedExpression)
            {
                InsertAtPos = selectedBox.SelectionStart;
            }
        }

        private void ExpOnMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is not TextBox selectedBox) return;
            if (SelectedExpression != null && selectedBox == SelectedExpression)
            {
                InsertAtPos = selectedBox.SelectionStart;
            }
        }

        private void ExpOnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not TextBox selectedBox) return;
            if (SelectedExpression != null && selectedBox == SelectedExpression)
            {
                InsertAtPos = selectedBox.SelectionStart;
            }
        }

        private void LbCommandVarsOnDoubleClick(object sender, EventArgs e)
        {
            if (lbCommandVars.SelectedItem == null) return;
            SelectedExpression.Text = SelectedExpression.Text.Insert(InsertAtPos, lbCommandVars.SelectedItem.ToString());
        }

        private void Cancel_Clicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void Okay_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tbDurationExp.Text))
            {
                Duration = tbDurationExp.Text;
            }

            if (!string.IsNullOrEmpty(tbMagExpr.Text))
            {
                Magnitude = tbMagExpr.Text;
            }

            if (!string.IsNullOrEmpty(tbProbExpr.Text))
            {
                Probability = tbProbExpr.Text;
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
