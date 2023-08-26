using System;
using System.Linq;
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
            lbCommandVars.DataSource = new BindingSource(Expressions.CommandsList.Select(e => e.Keyword).ToList(), null);
            lbCommandVars.EndUpdate();
            tbDurationExp.Text = _myEffect.Expressions.Duration;
            tbMagExpr.Text = _myEffect.Expressions.Magnitude;
            tbProbExpr.Text = _myEffect.Expressions.Probability;
        }

        private void btnPowerInsert_Click(object sender, EventArgs e)
        {
            if (lbCommandVars.SelectedIndex < 0)
            {
                return;
            }
            
            var commandKeyword = lbCommandVars.Items[lbCommandVars.SelectedIndex].ToString();
            var command = Expressions.CommandsList.First(e => e.Keyword == commandKeyword);
            var token = "";
            switch (command.CommandTokenType)
            {
                case Expressions.ExprCommandToken.PowerName:
                    var powerSelector = new PowerSelector();
                    var psResult = powerSelector.ShowDialog(this);
                    if (psResult != DialogResult.OK)
                    {
                        return;
                    }

                    token = powerSelector.SelectedPower.FullName;

                    break;

                case Expressions.ExprCommandToken.ArchetypeName:
                    var archetypeSelector = new ArchetypeSelector();
                    var asResult = archetypeSelector.ShowDialog(this);
                    if (asResult != DialogResult.OK)
                    {
                        return;
                    }

                    //token = archetypeSelector.SelectedArchetype.DisplayName;

                    break;

                case Expressions.ExprCommandToken.PowerGroup or Expressions.ExprCommandToken.PowerGroupPrefix:
                    var powerGroupSelector = new PowerGroupSelector(command.CommandTokenType == Expressions.ExprCommandToken.PowerGroupPrefix);
                    var pgResult = powerGroupSelector.ShowDialog(this);
                    //if (pgResult !=)
                    
                    break;
            }
            
            if (SelectedExpression != null)
            {
                //SelectedExpression.Text = SelectedExpression.Text.Insert(InsertAtPos, powerSelector.SelectedPower.FullName);
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
