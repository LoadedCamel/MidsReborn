using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmTweakMatching
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = (System.ComponentModel.IContainer)new System.ComponentModel.Container();

            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Button2 = new System.Windows.Forms.Button();
            this.Button1 = new System.Windows.Forms.Button();
            this.btnDel = new System.Windows.Forms.Button();
            this.txtOvr = new System.Windows.Forms.TextBox();
            this.lstTweaks = new System.Windows.Forms.ListBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.cbPower = new System.Windows.Forms.ComboBox();
            this.cbSet1 = new System.Windows.Forms.ComboBox();
            this.cbType1 = new System.Windows.Forms.ComboBox();
            this.cbAT1 = new System.Windows.Forms.ComboBox();
            this.txtAddActual = new System.Windows.Forms.TextBox();
            this.txtAddOvr = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Button2);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Button1);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.btnDel);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.txtOvr);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.lstTweaks);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);

            this.GroupBox1.Location = new System.Drawing.Point(8, 8);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(580, 424);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Override Editor";
            this.Button2.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.Button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.Button2.ForeColor = System.Drawing.Color.Black;

            this.Button2.Location = new System.Drawing.Point(320, 52);
            this.Button2.Name = "Button2";

            this.Button2.Size = new System.Drawing.Size(56, 24);
            this.Button2.TabIndex = 9;
            this.Button2.Text = "Update";
            this.Button2.UseVisualStyleBackColor = false;
            this.Button2.Click += new System.EventHandler(Button2_Click);
            this.Button1.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.Button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.Button1.ForeColor = System.Drawing.Color.Black;

            this.Button1.Location = new System.Drawing.Point(516, 392);
            this.Button1.Name = "Button1";

            this.Button1.Size = new System.Drawing.Size(56, 24);
            this.Button1.TabIndex = 8;
            this.Button1.Text = "Close";
            this.Button1.UseVisualStyleBackColor = false;
            this.Button1.Click += new System.EventHandler(Button1_Click);
            this.btnDel.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.btnDel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.btnDel.ForeColor = System.Drawing.Color.Black;

            this.btnDel.Location = new System.Drawing.Point(236, 52);
            this.btnDel.Name = "btnDel";

            this.btnDel.Size = new System.Drawing.Size(56, 24);
            this.btnDel.TabIndex = 7;
            this.btnDel.Text = "Delete";
            this.btnDel.UseVisualStyleBackColor = false;
            this.btnDel.Click += new System.EventHandler(btnDel_Click);

            this.txtOvr.Location = new System.Drawing.Point(232, 24);
            this.txtOvr.Name = "txtOvr";

            this.txtOvr.Size = new System.Drawing.Size(336, 20);
            this.txtOvr.TabIndex = 6;
            this.txtOvr.Text = "Override Desc";

            this.lstTweaks.Location = new System.Drawing.Point(8, 20);
            this.lstTweaks.Name = "lstTweaks";

            this.lstTweaks.Size = new System.Drawing.Size(216, 394);
            this.lstTweaks.TabIndex = 0;
            this.lstTweaks.SelectedIndexChanged += new System.EventHandler(lstTweaks_SelectedIndexChanged);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbPower);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbSet1);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbType1);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.cbAT1);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtAddActual);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.txtAddOvr);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.btnAdd);

            this.GroupBox2.Location = new System.Drawing.Point(232, 112);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(336, 204);
            this.GroupBox2.TabIndex = 3;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Add:";
            this.cbPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbPower.Location = new System.Drawing.Point(8, 92);
            this.cbPower.Name = "cbPower";

            this.cbPower.Size = new System.Drawing.Size(132, 21);
            this.cbPower.TabIndex = 6;
            this.cbPower.SelectedIndexChanged += new System.EventHandler(cbPower_SelectedIndexChanged);
            this.cbSet1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbSet1.Location = new System.Drawing.Point(8, 68);
            this.cbSet1.Name = "cbSet1";

            this.cbSet1.Size = new System.Drawing.Size(132, 21);
            this.cbSet1.TabIndex = 2;
            this.cbSet1.SelectedIndexChanged += new System.EventHandler(cbSet1_SelectedIndexChanged);
            this.cbType1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbType1.Location = new System.Drawing.Point(8, 44);
            this.cbType1.Name = "cbType1";

            this.cbType1.Size = new System.Drawing.Size(132, 21);
            this.cbType1.TabIndex = 1;
            this.cbType1.SelectedIndexChanged += new System.EventHandler(cbType1_SelectedIndexChanged);
            this.cbAT1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            this.cbAT1.Location = new System.Drawing.Point(8, 20);
            this.cbAT1.Name = "cbAT1";

            this.cbAT1.Size = new System.Drawing.Size(132, 21);
            this.cbAT1.TabIndex = 0;
            this.cbAT1.SelectedIndexChanged += new System.EventHandler(cbAT1_SelectedIndexChanged);
            this.txtAddActual.Enabled = false;

            this.txtAddActual.Location = new System.Drawing.Point(8, 120);
            this.txtAddActual.Name = "txtAddActual";

            this.txtAddActual.Size = new System.Drawing.Size(336, 20);
            this.txtAddActual.TabIndex = 3;
            this.txtAddActual.Text = "Actual Desc";

            this.txtAddOvr.Location = new System.Drawing.Point(8, 148);
            this.txtAddOvr.Name = "txtAddOvr";

            this.txtAddOvr.Size = new System.Drawing.Size(336, 20);
            this.txtAddOvr.TabIndex = 4;
            this.txtAddOvr.Text = "Override Desc";
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.btnAdd.ForeColor = System.Drawing.Color.Black;

            this.btnAdd.Location = new System.Drawing.Point(8, 172);
            this.btnAdd.Name = "btnAdd";

            this.btnAdd.Size = new System.Drawing.Size(56, 24);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(btnAdd_Click);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);

            this.ClientSize = new System.Drawing.Size(602, 446);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tweak Powerset Matching";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.ResumeLayout(false);
        }
        #endregion

        Button btnAdd;
        Button btnDel;
        Button Button1;
        Button Button2;
        ComboBox cbAT1;
        ComboBox cbPower;
        ComboBox cbSet1;
        ComboBox cbType1;
        GroupBox GroupBox1;
        GroupBox GroupBox2;
        ListBox lstTweaks;
        TextBox txtAddActual;
        TextBox txtAddOvr;
        TextBox txtOvr;
    }
}