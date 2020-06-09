using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Base.Data_Classes;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Hero_Designer
{
    public partial class frmEditArchetype : Form
    {
        Button btnCancel;

        Button btnOK;

        ComboBox cbClassType;
        ComboBox cbPriGroup;
        ComboBox cbSecGroup;

        CheckBox chkPlayable;
        CheckedListBox clbOrigin;
        GroupBox GroupBox1;
        GroupBox GroupBox2;
        GroupBox GroupBox3;
        GroupBox GroupBox4;
        GroupBox GroupBox5;
        Label Label1;
        Label Label10;
        Label Label11;
        Label Label12;
        Label Label13;
        Label Label14;
        Label Label15;
        Label Label16;
        Label Label17;
        Label Label18;
        Label Label19;
        Label Label2;
        Label Label20;
        Label Label21;
        Label Label22;
        Label Label23;
        Label Label24;
        Label Label3;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        TextBox txtBaseRec;
        TextBox txtBaseRegen;

        TextBox txtClassName;
        TextBox txtDamCap;

        TextBox txtDescLong;

        TextBox txtDescShort;
        TextBox txtHP;
        TextBox txtHPCap;

        TextBox txtName;
        TextBox txtPerceptionCap;
        TextBox txtRecCap;
        TextBox txtRechargeCap;
        TextBox txtRegCap;
        TextBox txtResCap;
        NumericUpDown udColumn;
        NumericUpDown udThreat;

        private bool Loading;
        public readonly Archetype MyAT;
        private readonly bool ONDuplicate;
        private readonly string OriginalName;

        public frmEditArchetype(ref Archetype iAT)
        {
            Load += frmEditArchetype_Load;
            Loading = true;
            OriginalName = "";
            ONDuplicate = false;
            InitializeComponent();
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmEditArchetype));
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmEditArchetype);
            MyAT = new Archetype(iAT);
            OriginalName = MyAT.ClassName;
            int num = DatabaseAPI.Database.Classes.Length - 1;
            for (int index = 0; index <= num; ++index)
            {
                if (index != MyAT.Idx && string.Equals(DatabaseAPI.Database.Classes[index].ClassName, OriginalName, StringComparison.OrdinalIgnoreCase))
                    ONDuplicate = true;
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            if (!CheckClassName())
                return;
            if (clbOrigin.CheckedItems.Count < 1)
            {
                int num1 = (int)Interaction.MsgBox("An archetype class must have at least one valid origin!", MsgBoxStyle.Information, "Oops.");
            }
            else if (cbPriGroup.Text == "" | cbSecGroup.Text == "")
            {
                int num2 = (int)Interaction.MsgBox("You must set a Primary and Secondary Powerset Group!", MsgBoxStyle.Information, "Oops.");
            }
            else
            {
                float num3 = (float)Conversion.Val(txtHP.Text);
                if (num3 < 1.0)
                {
                    num3 = 1f;
                    int num4 = (int)Interaction.MsgBox("Hit Point value of < 1 is invalid. Hit Points set to 1", MsgBoxStyle.Information);
                }
                MyAT.Hitpoints = (int)Math.Round(num3);
                float num5 = (float)Conversion.Val(txtHPCap.Text);
                if (num5 < 1.0)
                    num5 = 1f;
                if (num5 < (double)MyAT.Hitpoints)
                    num5 = MyAT.Hitpoints;
                MyAT.HPCap = num5;
                float num6 = (float)Conversion.Val(txtResCap.Text);
                if (num6 < 1.0)
                    num6 = 1f;
                MyAT.ResCap = num6 / 100f;
                float num7 = (float)Conversion.Val(txtDamCap.Text);
                if (num7 < 1.0)
                    num7 = 1f;
                MyAT.DamageCap = num7 / 100f;
                float num8 = (float)Conversion.Val(txtRechargeCap.Text);
                if (num8 < 1.0)
                    num8 = 1f;
                MyAT.RechargeCap = num8 / 100f;
                float num9 = (float)Conversion.Val(txtRecCap.Text);
                if (num9 < 1.0)
                    num9 = 1f;
                MyAT.RecoveryCap = num9 / 100f;
                float num10 = (float)Conversion.Val(txtRegCap.Text);
                if (num10 < 1.0)
                    num10 = 1f;
                MyAT.RegenCap = num10 / 100f;
                float num11 = (float)Conversion.Val(txtBaseRec.Text);
                if (num11 < 0.0)
                    num11 = 0.0f;
                if (num11 > 100.0)
                    num11 = 1.67f;
                MyAT.BaseRecovery = num11;
                float num12 = (float)Conversion.Val(txtBaseRegen.Text);
                if (num12 < 0.0)
                    num12 = 0.0f;
                if (num12 > 100.0)
                    num12 = 100f;
                MyAT.BaseRegen = num12;
                float num13 = (float)Conversion.Val(txtPerceptionCap.Text);
                if (num13 < 0.0)
                    num13 = 0.0f;
                if (num13 > 10000.0)
                    num13 = 1153f;
                MyAT.PerceptionCap = num13;
                MyAT.PrimaryGroup = cbPriGroup.Text;
                MyAT.SecondaryGroup = cbSecGroup.Text;
                MyAT.Origin = new string[clbOrigin.CheckedItems.Count - 1 + 1];
                int num14 = clbOrigin.CheckedItems.Count - 1;
                for (int index = 0; index <= num14; ++index)
                    MyAT.Origin[index] = Convert.ToString(clbOrigin.CheckedItems[index]);
                MyAT.Column = Decimal.Compare(udColumn.Value, new Decimal(0)) >= 0 ? Convert.ToInt32(udColumn.Value) : 0;
                MyAT.BaseThreat = Decimal.Compare(udThreat.Value, new Decimal(0)) >= 0 ? Convert.ToSingle(udThreat.Value) : 0.0f;
                DialogResult = DialogResult.OK;
                Hide();
            }
        }

        void cbClassType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.ClassType = (Enums.eClassType)cbClassType.SelectedIndex;
        }

        bool CheckClassName()
        {
            if (ONDuplicate)
                return true;
            int num1 = DatabaseAPI.Database.Classes.Length - 1;
            for (int index = 0; index <= num1; ++index)
            {
                if (index == MyAT.Idx || !string.Equals(DatabaseAPI.Database.Classes[index].ClassName, txtClassName.Text,
                        StringComparison.OrdinalIgnoreCase))
                    continue;
                int num2 = (int)Interaction.MsgBox((txtClassName.Text + " is already in use, please select a unique class name."), MsgBoxStyle.Information, "Name in Use");
                return false;
            }
            return true;
        }

        void chkPlayable_CheckedChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.Playable = chkPlayable.Checked;
        }

        void DisplayData()
        {
            Text = "Edit Class (" + MyAT.ClassName + " - " + MyAT.DisplayName + ")";
            txtName.Text = MyAT.DisplayName;
            txtClassName.Text = MyAT.ClassName;
            cbClassType.BeginUpdate();
            cbClassType.Items.Clear();
            cbClassType.Items.AddRange(Enum.GetNames(MyAT.ClassType.GetType()));
            if (MyAT.ClassType > ~Enums.eClassType.None & MyAT.ClassType < (Enums.eClassType)cbClassType.Items.Count)
                cbClassType.SelectedIndex = (int)MyAT.ClassType;
            else
                cbClassType.SelectedIndex = 0;
            cbClassType.EndUpdate();
            udColumn.Value = !(Decimal.Compare(new Decimal(MyAT.Column + 2), udColumn.Maximum) <= 0 & Decimal.Compare(new Decimal(MyAT.Column), udColumn.Minimum) >= 0) ? udColumn.Minimum : new Decimal(MyAT.Column);
            udThreat.Value = !(MyAT.BaseThreat > (double)Convert.ToSingle(udThreat.Maximum) | MyAT.BaseThreat < (double)Convert.ToSingle(udThreat.Minimum)) ? new Decimal(MyAT.BaseThreat) : new Decimal(0);
            chkPlayable.Checked = MyAT.Playable;
            txtHP.Text = Convert.ToString(MyAT.Hitpoints, CultureInfo.InvariantCulture);
            txtHPCap.Text = Convert.ToString(MyAT.HPCap, CultureInfo.InvariantCulture);
            txtResCap.Text = Convert.ToString(MyAT.ResCap * 100f, CultureInfo.InvariantCulture);
            txtDamCap.Text = Convert.ToString(MyAT.DamageCap * 100f, CultureInfo.InvariantCulture);
            txtRechargeCap.Text = Convert.ToString(MyAT.RechargeCap * 100f, CultureInfo.InvariantCulture);
            txtRecCap.Text = Convert.ToString(MyAT.RecoveryCap * 100f, CultureInfo.InvariantCulture);
            txtRegCap.Text = Convert.ToString(MyAT.RegenCap * 100f, CultureInfo.InvariantCulture);
            txtBaseRec.Text = Strings.Format(MyAT.BaseRecovery, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00##");
            txtBaseRegen.Text = Strings.Format(MyAT.BaseRegen, "##0" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator + "00##");
            txtPerceptionCap.Text = Convert.ToString(MyAT.PerceptionCap, CultureInfo.InvariantCulture);
            cbPriGroup.BeginUpdate();
            cbSecGroup.BeginUpdate();
            cbPriGroup.Items.Clear();
            cbSecGroup.Items.Clear();
            foreach (string key in DatabaseAPI.Database.PowersetGroups.Keys)
            {
                cbPriGroup.Items.Add(key);
                cbSecGroup.Items.Add(key);
            }
            cbPriGroup.SelectedValue = MyAT.PrimaryGroup;
            cbSecGroup.SelectedValue = MyAT.SecondaryGroup;
            cbPriGroup.EndUpdate();
            cbSecGroup.EndUpdate();
            udColumn.Value = new Decimal(MyAT.Column);
            clbOrigin.BeginUpdate();
            clbOrigin.Items.Clear();
            foreach (Origin origin in DatabaseAPI.Database.Origins)
            {
                bool isChecked = false;
                int num = MyAT.Origin.Length - 1;
                for (int index = 0; index <= num; ++index)
                {
                    if (origin.Name.ToLower() == MyAT.Origin[index].ToLower())
                        isChecked = true;
                }
                clbOrigin.Items.Add(origin.Name, isChecked);
            }
            clbOrigin.EndUpdate();
            txtDescShort.Text = MyAT.DescShort;
            txtDescLong.Text = MyAT.DescLong;
        }

        void frmEditArchetype_Load(object sender, EventArgs e)
        {
            DisplayData();
            Loading = false;
        }

        void txtClassName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.ClassName = txtClassName.Text;
        }

        void txtDescLong_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.DescLong = txtDescLong.Text;
        }

        void txtDescShort_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.DescShort = txtDescShort.Text;
        }

        void txtName_TextChanged(object sender, EventArgs e)
        {
            if (Loading)
                return;
            MyAT.DisplayName = txtName.Text;
        }
    }
}