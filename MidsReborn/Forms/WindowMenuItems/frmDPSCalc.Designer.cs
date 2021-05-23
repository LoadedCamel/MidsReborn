using System.ComponentModel;
using mrbControls;

namespace Mids_Reborn.Forms.WindowMenuItems
{
    public partial class frmDPSCalc
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
            this.components = new System.ComponentModel.Container();
            this.lvPower = new System.Windows.Forms.ListView();
            this.chPower = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDPA = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDamage = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chRecharge = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chAnimation = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEndurance = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDamageBuff = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chResistanceDebuff = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chBuildID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ilAttackChain = new System.Windows.Forms.ImageList(this.components);
            this.chkSortByLevel = new System.Windows.Forms.CheckBox();
            this.chkDamageBuffs = new System.Windows.Forms.CheckBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblDPS = new System.Windows.Forms.Label();
            this.lblEPS = new System.Windows.Forms.Label();
            this.lblDPSNum = new System.Windows.Forms.Label();
            this.lblEPSNum = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.tbDPSOutput = new System.Windows.Forms.TextBox();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ibAutoMode = new mrbControls.ImageButton();
            this.ibClear = new mrbControls.ImageButton();
            this.ibTopmost = new mrbControls.ImageButton();
            this.ibClose = new mrbControls.ImageButton();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvPower
            // 
            this.lvPower.CheckBoxes = true;
            this.lvPower.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chPower,
            this.chDPA,
            this.chDamage,
            this.chRecharge,
            this.chAnimation,
            this.chEndurance,
            this.chDamageBuff,
            this.chResistanceDebuff,
            this.chBuildID});
            this.lvPower.FullRowSelect = true;
            this.lvPower.HideSelection = false;
            this.lvPower.Location = new System.Drawing.Point(12, 12);
            this.lvPower.MultiSelect = false;
            this.lvPower.Name = "lvPower";
            this.lvPower.Size = new System.Drawing.Size(768, 250);
            this.lvPower.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvPower.TabIndex = 1;
            this.lvPower.UseCompatibleStateImageBehavior = false;
            this.lvPower.View = System.Windows.Forms.View.Details;
            this.lvPower.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvPower_ColumnClick);
            this.lvPower.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvPower_ItemChecked);
            this.lvPower.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lvPower_Clicked);
            this.lvPower.MouseEnter += new System.EventHandler(this.lvPower_MouseEnter);
            // 
            // chPower
            // 
            this.chPower.Text = "Power";
            this.chPower.Width = 150;
            // 
            // chDPA
            // 
            this.chDPA.Text = "DPA";
            this.chDPA.Width = 83;
            // 
            // chDamage
            // 
            this.chDamage.Text = "Damage";
            this.chDamage.Width = 83;
            // 
            // chRecharge
            // 
            this.chRecharge.Text = "Recharge";
            this.chRecharge.Width = 83;
            // 
            // chAnimation
            // 
            this.chAnimation.Text = "Animation";
            this.chAnimation.Width = 83;
            // 
            // chEndurance
            // 
            this.chEndurance.Text = "Endurance";
            this.chEndurance.Width = 83;
            // 
            // chDamageBuff
            // 
            this.chDamageBuff.Tag = "Damage Buff";
            this.chDamageBuff.Text = "Dmg Buff";
            this.chDamageBuff.Width = 83;
            // 
            // chResistanceDebuff
            // 
            this.chResistanceDebuff.Tag = "Resistance Debuff";
            this.chResistanceDebuff.Text = "Res Debuff";
            this.chResistanceDebuff.Width = 83;
            // 
            // chBuildID
            // 
            this.chBuildID.Tag = "chBuildID";
            this.chBuildID.Width = 0;
            // 
            // ilAttackChain
            // 
            this.ilAttackChain.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.ilAttackChain.ImageSize = new System.Drawing.Size(16, 16);
            this.ilAttackChain.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // chkSortByLevel
            // 
            this.chkSortByLevel.Checked = true;
            this.chkSortByLevel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSortByLevel.ForeColor = System.Drawing.Color.White;
            this.chkSortByLevel.Location = new System.Drawing.Point(12, 263);
            this.chkSortByLevel.Name = "chkSortByLevel";
            this.chkSortByLevel.Size = new System.Drawing.Size(176, 16);
            this.chkSortByLevel.TabIndex = 9;
            this.chkSortByLevel.Text = "Sort By Level";
            this.chkSortByLevel.UseVisualStyleBackColor = true;
            this.chkSortByLevel.CheckedChanged += new System.EventHandler(this.chkSortByLevel_CheckedChanged);
            // 
            // chkDamageBuffs
            // 
            this.chkDamageBuffs.Checked = true;
            this.chkDamageBuffs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDamageBuffs.ForeColor = System.Drawing.Color.White;
            this.chkDamageBuffs.Location = new System.Drawing.Point(250, 450);
            this.chkDamageBuffs.Name = "chkDamageBuffs";
            this.chkDamageBuffs.Size = new System.Drawing.Size(150, 18);
            this.chkDamageBuffs.TabIndex = 9;
            this.chkDamageBuffs.Text = "Add Damage Buffs?";
            this.chkDamageBuffs.UseVisualStyleBackColor = true;
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Arial", 17.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblHeader.Location = new System.Drawing.Point(0, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(700, 30);
            this.lblHeader.TabIndex = 10;
            this.lblHeader.Text = "You may select -All Powers- or just the powers you want to consider.";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDPS
            // 
            this.lblDPS.BackColor = System.Drawing.Color.Black;
            this.lblDPS.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblDPS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblDPS.Location = new System.Drawing.Point(650, 10);
            this.lblDPS.MinimumSize = new System.Drawing.Size(200, 30);
            this.lblDPS.Name = "lblDPS";
            this.lblDPS.Size = new System.Drawing.Size(200, 30);
            this.lblDPS.TabIndex = 1;
            this.lblDPS.Text = "Estimated DPS: ";
            // 
            // lblEPS
            // 
            this.lblEPS.BackColor = System.Drawing.Color.Black;
            this.lblEPS.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblEPS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblEPS.Location = new System.Drawing.Point(650, 75);
            this.lblEPS.MinimumSize = new System.Drawing.Size(200, 30);
            this.lblEPS.Name = "lblEPS";
            this.lblEPS.Size = new System.Drawing.Size(200, 30);
            this.lblEPS.TabIndex = 2;
            this.lblEPS.Text = "Estimated EPS: ";
            // 
            // lblDPSNum
            // 
            this.lblDPSNum.BackColor = System.Drawing.Color.Black;
            this.lblDPSNum.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblDPSNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblDPSNum.Location = new System.Drawing.Point(660, 40);
            this.lblDPSNum.MinimumSize = new System.Drawing.Size(200, 30);
            this.lblDPSNum.Name = "lblDPSNum";
            this.lblDPSNum.Size = new System.Drawing.Size(200, 30);
            this.lblDPSNum.TabIndex = 3;
            this.lblDPSNum.Text = "-Null-";
            // 
            // lblEPSNum
            // 
            this.lblEPSNum.BackColor = System.Drawing.Color.Black;
            this.lblEPSNum.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.lblEPSNum.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lblEPSNum.Location = new System.Drawing.Point(660, 105);
            this.lblEPSNum.MinimumSize = new System.Drawing.Size(200, 30);
            this.lblEPSNum.Name = "lblEPSNum";
            this.lblEPSNum.Size = new System.Drawing.Size(200, 30);
            this.lblEPSNum.TabIndex = 4;
            this.lblEPSNum.Text = "-Null-";
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.tbDPSOutput);
            this.Panel1.Controls.Add(this.lblDPS);
            this.Panel1.Controls.Add(this.lblEPS);
            this.Panel1.Controls.Add(this.lblDPSNum);
            this.Panel1.Controls.Add(this.lblEPSNum);
            this.Panel1.Controls.Add(this.lblHeader);
            this.Panel1.Location = new System.Drawing.Point(0, 36);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(790, 177);
            this.Panel1.TabIndex = 11;
            // 
            // tbDPSOutput
            // 
            this.tbDPSOutput.BackColor = System.Drawing.Color.Black;
            this.tbDPSOutput.Font = new System.Drawing.Font("Arial", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.tbDPSOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.tbDPSOutput.Location = new System.Drawing.Point(0, 36);
            this.tbDPSOutput.Name = "tbDPSOutput";
            this.tbDPSOutput.Size = new System.Drawing.Size(600, 24);
            this.tbDPSOutput.TabIndex = 0;
            // 
            // Panel2
            // 
            this.Panel2.BackColor = System.Drawing.Color.Black;
            this.Panel2.Controls.Add(this.Panel1);
            this.Panel2.Location = new System.Drawing.Point(12, 260);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(790, 213);
            this.Panel2.TabIndex = 12;
            // 
            // ibAutoMode
            // 
            this.ibAutoMode.Checked = false;
            this.ibAutoMode.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibAutoMode.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibAutoMode.Location = new System.Drawing.Point(123, 445);
            this.ibAutoMode.Name = "ibAutoMode";
            this.ibAutoMode.Size = new System.Drawing.Size(105, 22);
            this.ibAutoMode.TabIndex = 14;
            this.ibAutoMode.TextOff = "Manual";
            this.ibAutoMode.TextOn = "Alt Text";
            this.ibAutoMode.Toggle = false;
            this.ToolTip1.SetToolTip(this.ibAutoMode, "Click to enable Automagical Mode");
            this.ibAutoMode.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibAutoMode_ButtonClicked);
            // 
            // ibClear
            // 
            this.ibClear.Checked = false;
            this.ibClear.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClear.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClear.Location = new System.Drawing.Point(12, 445);
            this.ibClear.Name = "ibClear";
            this.ibClear.Size = new System.Drawing.Size(105, 22);
            this.ibClear.TabIndex = 13;
            this.ibClear.TextOff = "Clear";
            this.ibClear.TextOn = "Alt Text";
            this.ibClear.Toggle = false;
            this.ibClear.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibClear_ButtonClicked);
            // 
            // ibTopmost
            // 
            this.ibTopmost.Checked = true;
            this.ibTopmost.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibTopmost.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibTopmost.Location = new System.Drawing.Point(406, 445);
            this.ibTopmost.Name = "ibTopmost";
            this.ibTopmost.Size = new System.Drawing.Size(105, 22);
            this.ibTopmost.TabIndex = 7;
            this.ibTopmost.TextOff = "Keep On Top";
            this.ibTopmost.TextOn = "Keep On Top";
            this.ibTopmost.Toggle = true;
            this.ibTopmost.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibTopmost_ButtonClicked);
            // 
            // ibClose
            // 
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClose.Location = new System.Drawing.Point(517, 445);
            this.ibClose.Name = "ibClose";
            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 6;
            this.ibClose.TextOff = "Close";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            this.ibClose.ButtonClicked += new mrbControls.ImageButton.ButtonClickedEventHandler(this.ibClose_ButtonClicked);
            // 
            // frmDPSCalc
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(793, 479);
            this.Controls.Add(this.ibAutoMode);
            this.Controls.Add(this.ibClear);
            this.Controls.Add(this.chkSortByLevel);
            this.Controls.Add(this.chkDamageBuffs);
            this.Controls.Add(this.ibTopmost);
            this.Controls.Add(this.ibClose);
            this.Controls.Add(this.lvPower);
            this.Controls.Add(this.Panel2);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmDPSCalc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Damage Per Second Calculator (Beta)";
            this.TopMost = true;
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.Panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
    }
}