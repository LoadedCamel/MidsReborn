using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Base.Data_Classes;
using Microsoft.VisualBasic;

namespace Hero_Designer.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmEditArchetype : Form
    {
        public readonly Archetype MyAT;
        private readonly bool ONDuplicate;
        private readonly string OriginalName;
        private Button btnCancel;

        private Button btnOK;

        private ComboBox cbClassType;
        private ComboBox cbPriGroup;
        private ComboBox cbSecGroup;

        private CheckBox chkPlayable;
        private CheckedListBox clbOrigin;
        private GroupBox GroupBox1;
        private GroupBox GroupBox2;
        private GroupBox GroupBox3;
        private GroupBox GroupBox4;
        private GroupBox GroupBox5;
        private Label Label1;
        private Label Label10;
        private Label Label11;
        private Label Label12;
        private Label Label13;
        private Label Label14;
        private Label Label15;
        private Label Label16;
        private Label Label17;
        private Label Label18;
        private Label Label19;
        private Label Label2;
        private Label Label20;
        private Label Label21;
        private Label Label22;
        private Label Label23;
        private Label Label24;
        private Label Label3;
        private Label Label4;
        private Label Label5;
        private Label Label6;
        private Label Label7;
        private Label Label8;
        private Label Label9;

        private bool Loading;
        private TextBox txtBaseRec;
        private TextBox txtBaseRegen;

        private TextBox txtClassName;
        private TextBox txtDamCap;

        private TextBox txtDescLong;

        private TextBox txtDescShort;
        private TextBox txtHP;
        private TextBox txtHPCap;

        private TextBox txtName;
        private TextBox txtPerceptionCap;
        private TextBox txtRecCap;
        private TextBox txtRechargeCap;
        private TextBox txtRegCap;
        private TextBox txtResCap;
        private NumericUpDown udColumn;
        private NumericUpDown udThreat;

        public frmEditArchetype(ref Archetype iAT)
        {
            Load += frmEditArchetype_Load;
            Loading = true;
            OriginalName = "";
            ONDuplicate = false;
            InitializeComponent();
            var componentResourceManager = new ComponentResourceManager(typeof(frmEditArchetype));
            Icon = Resources.reborn;
            Name = nameof(frmEditArchetype);
            MyAT = new Archetype(iAT);
            OriginalName = MyAT.ClassName;
            var num = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (index != MyAT.Idx && string.Equals(DatabaseAPI.Database.Classes[index].ClassName, OriginalName,
                    StringComparison.OrdinalIgnoreCase))
                    ONDuplicate = true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckClassName())
                return;
            if (clbOrigin.CheckedItems.Count < 1)
            {
                MessageBox.Show("An archetype class must have at least one valid origin!", "Oops", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else if ((cbPriGroup.Text == "") | (cbSecGroup.Text == ""))
            {
                MessageBox.Show("You must set both a Primary and Secondary Powerset Group!", "Oops",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var num3 = (float) Conversion.Val(txtHP.Text);
                if (num3 < 1.0)
                {
                    num3 = 1f;
                    MessageBox.Show("Hit Point value of less than 1 is invalid. Hit Points set to 1.", "Hit Points",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                MyAT.Hitpoints = (int) Math.Round(num3);
                var num5 = (float) Conversion.Val(txtHPCap.Text);
                if (num5 < 1.0)
                    num5 = 1f;
                if (num5 < (double) MyAT.Hitpoints)
                    num5 = MyAT.Hitpoints;
                MyAT.HPCap = num5;
                var num6 = (float) Conversion.Val(txtResCap.Text);
                if (num6 < 1.0)
                    num6 = 1f;
                MyAT.ResCap = num6 / 100f;
                var num7 = (float) Conversion.Val(txtDamCap.Text);
                if (num7 < 1.0)
                    num7 = 1f;
                MyAT.DamageCap = num7 / 100f;
                var num8 = (float) Conversion.Val(txtRechargeCap.Text);
                if (num8 < 1.0)
                    num8 = 1f;
                MyAT.RechargeCap = num8 / 100f;
                var num9 = (float) Conversion.Val(txtRecCap.Text);
                if (num9 < 1.0)
                    num9 = 1f;
                MyAT.RecoveryCap = num9 / 100f;
                var num10 = (float) Conversion.Val(txtRegCap.Text);
                if (num10 < 1.0)
                    num10 = 1f;
                MyAT.RegenCap = num10 / 100f;
                var num11 = (float) Conversion.Val(txtBaseRec.Text);
                if (num11 < 0.0)
                    num11 = 0.0f;
                if (num11 > 100.0)
                    num11 = 1.67f;
                MyAT.BaseRecovery = num11;
                var num12 = (float) Conversion.Val(txtBaseRegen.Text);
                if (num12 < 0.0)
                    num12 = 0.0f;
                if (num12 > 100.0)
                    num12 = 100f;
                MyAT.BaseRegen = num12;
                var num13 = (float) Conversion.Val(txtPerceptionCap.Text);
                if (num13 < 0.0)
                    num13 = 0.0f;
                if (num13 > 10000.0)
                    num13 = 1153f;
                MyAT.PerceptionCap = num13;
                MyAT.PrimaryGroup = cbPriGroup.Text;
                MyAT.SecondaryGroup = cbSecGroup.Text;
                MyAT.Origin = new string[clbOrigin.CheckedItems.Count - 1 + 1];
                var num14 = clbOrigin.CheckedItems.Count - 1;
                for (var index = 0; index <= num14; ++index)
                    MyAT.Origin[index] = Convert.ToString(clbOrigin.CheckedItems[index]);
                MyAT.Column = decimal.Compare(udColumn.Value, new decimal(0)) >= 0
                    ? Convert.ToInt32(udColumn.Value)
                    : 0;
                MyAT.BaseThreat = decimal.Compare(udThreat.Value, new decimal(0)) >= 0
                    ? Convert.ToSingle(udThreat.Value)
                    : 0.0f;
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        private void cbClassType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.ClassType = (Enums.eClassType) cbClassType.SelectedIndex;
        }

        private bool CheckClassName()
        {
            if (ONDuplicate)
                return true;
            var num1 = DatabaseAPI.Database.Classes.Length - 1;
            for (var index = 0; index <= num1; ++index)
            {
                if (index == MyAT.Idx || !string.Equals(DatabaseAPI.Database.Classes[index].ClassName,
                    txtClassName.Text,
                    StringComparison.OrdinalIgnoreCase))
                    continue;
                MessageBox.Show($"{txtClassName.Text} is already in use, please select a unique class name.",
                    "Name in Use", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }

        private void chkPlayable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.Playable = chkPlayable.Checked;
        }

        private void DisplayData()
        {
            Text = "Edit Class (" + MyAT.ClassName + " - " + MyAT.DisplayName + ")";
            txtName.Text = MyAT.DisplayName;
            txtClassName.Text = MyAT.ClassName;
            cbClassType.BeginUpdate();
            cbClassType.Items.Clear();
            cbClassType.Items.AddRange(Enum.GetNames(MyAT.ClassType.GetType()));
            if ((MyAT.ClassType > ~Enums.eClassType.None) &
                (MyAT.ClassType < (Enums.eClassType) cbClassType.Items.Count))
                cbClassType.SelectedIndex = (int) MyAT.ClassType;
            else
                cbClassType.SelectedIndex = 0;
            cbClassType.EndUpdate();
            udColumn.Value =
                !((decimal.Compare(new decimal(MyAT.Column + 2), udColumn.Maximum) <= 0) &
                  (decimal.Compare(new decimal(MyAT.Column), udColumn.Minimum) >= 0))
                    ? udColumn.Minimum
                    : new decimal(MyAT.Column);
            udThreat.Value =
                !((MyAT.BaseThreat > (double) Convert.ToSingle(udThreat.Maximum)) |
                  (MyAT.BaseThreat < (double) Convert.ToSingle(udThreat.Minimum)))
                    ? new decimal(MyAT.BaseThreat)
                    : new decimal(0);
            chkPlayable.Checked = MyAT.Playable;
            txtHP.Text = Convert.ToString(MyAT.Hitpoints, CultureInfo.InvariantCulture);
            txtHPCap.Text = Convert.ToString(MyAT.HPCap, CultureInfo.InvariantCulture);
            txtResCap.Text = Convert.ToString(MyAT.ResCap * 100f, CultureInfo.InvariantCulture);
            txtDamCap.Text = Convert.ToString(MyAT.DamageCap * 100f, CultureInfo.InvariantCulture);
            txtRechargeCap.Text = Convert.ToString(MyAT.RechargeCap * 100f, CultureInfo.InvariantCulture);
            txtRecCap.Text = Convert.ToString(MyAT.RecoveryCap * 100f, CultureInfo.InvariantCulture);
            txtRegCap.Text = Convert.ToString(MyAT.RegenCap * 100f, CultureInfo.InvariantCulture);
            txtBaseRec.Text = Strings.Format(MyAT.BaseRecovery,
                "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00##");
            txtBaseRegen.Text = Strings.Format(MyAT.BaseRegen,
                "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00##");
            txtPerceptionCap.Text = Convert.ToString(MyAT.PerceptionCap, CultureInfo.InvariantCulture);
            cbPriGroup.BeginUpdate();
            cbSecGroup.BeginUpdate();
            cbPriGroup.Items.Clear();
            cbSecGroup.Items.Clear();
            foreach (var key in DatabaseAPI.Database.PowersetGroups.Keys)
            {
                cbPriGroup.Items.Add(key);
                cbSecGroup.Items.Add(key);
            }

            cbPriGroup.SelectedValue = MyAT.PrimaryGroup;
            cbSecGroup.SelectedValue = MyAT.SecondaryGroup;
            cbPriGroup.EndUpdate();
            cbSecGroup.EndUpdate();
            udColumn.Value = new decimal(MyAT.Column);
            clbOrigin.BeginUpdate();
            clbOrigin.Items.Clear();
            foreach (var origin in DatabaseAPI.Database.Origins)
            {
                var isChecked = false;
                var num = MyAT.Origin.Length - 1;
                for (var index = 0; index <= num; ++index)
                    if (origin.Name.ToLower() == MyAT.Origin[index].ToLower())
                        isChecked = true;
                clbOrigin.Items.Add(origin.Name, isChecked);
            }

            clbOrigin.EndUpdate();
            txtDescShort.Text = MyAT.DescShort;
            txtDescLong.Text = MyAT.DescLong;
        }

        private void frmEditArchetype_Load(object sender, EventArgs e)
        {
            DisplayData();
            Loading = false;
        }

        private void txtClassName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.ClassName = txtClassName.Text;
        }

        private void txtDescLong_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.DescLong = txtDescLong.Text;
        }

        private void txtDescShort_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.DescShort = txtDescShort.Text;
        }

        private void txtName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.DisplayName = txtName.Text;
        }
    }
}