using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Forms.Controls;
using mrbBase;
using static mrbBase.Base.Data_Classes.Effect;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmExpressionBuilder : Form
    {

        public string Duration { get; set; }
        public string Magnitude { get; set; }
        public string Probability { get; set; }

        private readonly Expression _myExpressions;
        private TextBox SelectedExpression { get; set; }
        private int InsertAtPos { get; set; }

        public frmExpressionBuilder(Expression expressions)
        {
            InitializeComponent();
            _myExpressions = expressions.Clone();
            Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            tbDurationExp.Text = _myExpressions.Duration;
            tbMagExpr.Text = _myExpressions.Magnitude;
            tbProbExpr.Text = _myExpressions.Probability;
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
