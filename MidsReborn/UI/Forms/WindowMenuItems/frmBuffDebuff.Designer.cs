namespace Mids_Reborn.UI.Forms.WindowMenuItems
{
    partial class frmBuffDebuff
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            PowerEffectsPanel = new Mids_Reborn.UI.Controls.StickyScrollPanel();
            BtnClose = new Mids_Reborn.UI.Controls.ImageButtonEx();
            cbBuffType = new System.Windows.Forms.ComboBox();
            cbGroup = new System.Windows.Forms.ComboBox();
            cbValueDisplayType = new System.Windows.Forms.ComboBox();
            cbValueGroupMode = new System.Windows.Forms.ComboBox();
            cbValueGroupMode2 = new System.Windows.Forms.ComboBox();
            SuspendLayout();
            // 
            // PowerEffectsPanel
            // 
            PowerEffectsPanel.AutoScroll = true;
            PowerEffectsPanel.Location = new System.Drawing.Point(12, 54);
            PowerEffectsPanel.Name = "PowerEffectsPanel";
            PowerEffectsPanel.Size = new System.Drawing.Size(485, 387);
            PowerEffectsPanel.TabIndex = 0;
            // 
            // BtnClose
            // 
            BtnClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            BtnClose.CurrentText = "Close";
            BtnClose.DisplayVertically = false;
            BtnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            BtnClose.Images.Background = MRBResourceLib.Resources.HeroButton;
            BtnClose.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            BtnClose.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            BtnClose.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            BtnClose.ImagesDis.Flat = MRBResourceLib.Resources.DisabledButtonFlat;
            BtnClose.ImagesDis.Gloss = MRBResourceLib.Resources.DisabledButtonGloss;
            BtnClose.Location = new System.Drawing.Point(351, 450);
            BtnClose.Lock = false;
            BtnClose.Name = "BtnClose";
            BtnClose.Size = new System.Drawing.Size(100, 30);
            BtnClose.TabIndex = 1;
            BtnClose.Text = "Close";
            BtnClose.TextOutline.Color = System.Drawing.Color.Black;
            BtnClose.TextOutline.Width = 2;
            BtnClose.ToggleState = UI.Controls.ImageButtonEx.States.ToggledOff;
            BtnClose.ToggleText.Indeterminate = "Indeterminate State";
            BtnClose.ToggleText.ToggledOff = "ToggledOff State";
            BtnClose.ToggleText.ToggledOn = "ToggledOn State";
            BtnClose.UseAlt = false;
            BtnClose.Click += BtnClose_Click;
            // 
            // cbBuffType
            // 
            cbBuffType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbBuffType.FormattingEnabled = true;
            cbBuffType.Items.AddRange(new object[] { "All effects", "Buffs", "Debuffs" });
            cbBuffType.Location = new System.Drawing.Point(21, 15);
            cbBuffType.Name = "cbBuffType";
            cbBuffType.Size = new System.Drawing.Size(121, 23);
            cbBuffType.TabIndex = 2;
            cbBuffType.SelectedIndexChanged += cbBuffType_SelectedIndexChanged;
            // 
            // cbGroup
            // 
            cbGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbGroup.FormattingEnabled = true;
            cbGroup.Items.AddRange(new object[] { "All", "Mitigation", "Sustain", "Dps", "Misc" });
            cbGroup.Location = new System.Drawing.Point(161, 15);
            cbGroup.Name = "cbGroup";
            cbGroup.Size = new System.Drawing.Size(121, 23);
            cbGroup.TabIndex = 3;
            cbGroup.SelectedIndexChanged += cbGroup_SelectedIndexChanged;
            // 
            // cbValueDisplayType
            // 
            cbValueDisplayType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbValueDisplayType.FormattingEnabled = true;
            cbValueDisplayType.Items.AddRange(new object[] { "Raw", "Duration", "Value/End", "Value/Recharge", "Value/Recharge/End" });
            cbValueDisplayType.Location = new System.Drawing.Point(303, 15);
            cbValueDisplayType.Name = "cbValueDisplayType";
            cbValueDisplayType.Size = new System.Drawing.Size(121, 23);
            cbValueDisplayType.TabIndex = 4;
            cbValueDisplayType.SelectedIndexChanged += cbValueDisplayType_SelectedIndexChanged;
            // 
            // cbValueGroupMode
            // 
            cbValueGroupMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbValueGroupMode.FormattingEnabled = true;
            cbValueGroupMode.Items.AddRange(new object[] { "None", "Avg. per minute" });
            cbValueGroupMode.Location = new System.Drawing.Point(446, 15);
            cbValueGroupMode.Name = "cbValueGroupMode";
            cbValueGroupMode.Size = new System.Drawing.Size(121, 23);
            cbValueGroupMode.TabIndex = 5;
            cbValueGroupMode.SelectedIndexChanged += cbValueGroupMode_SelectedIndexChanged;
            // 
            // cbValueGroupMode2
            // 
            cbValueGroupMode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbValueGroupMode2.FormattingEnabled = true;
            cbValueGroupMode2.Items.AddRange(new object[] { "Power", "Stat" });
            cbValueGroupMode2.Location = new System.Drawing.Point(589, 15);
            cbValueGroupMode2.Name = "cbValueGroupMode2";
            cbValueGroupMode2.Size = new System.Drawing.Size(121, 23);
            cbValueGroupMode2.TabIndex = 6;
            cbValueGroupMode2.SelectedIndexChanged += cbValueGroupMode2_SelectedIndexChanged;
            // 
            // frmBuffDebuff
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(800, 490);
            Controls.Add(cbValueGroupMode2);
            Controls.Add(cbValueGroupMode);
            Controls.Add(cbValueDisplayType);
            Controls.Add(cbGroup);
            Controls.Add(cbBuffType);
            Controls.Add(BtnClose);
            Controls.Add(PowerEffectsPanel);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmBuffDebuff";
            Text = "Buff/Debuffs summary";
            Load += frmBuffDebuff_Load;
            ResumeLayout(false);
        }

        #endregion

        private Mids_Reborn.UI.Controls.StickyScrollPanel PowerEffectsPanel;
        private Controls.ImageButtonEx BtnClose;
        private System.Windows.Forms.ComboBox cbBuffType;
        private System.Windows.Forms.ComboBox cbGroup;
        private System.Windows.Forms.ComboBox cbValueDisplayType;
        private System.Windows.Forms.ComboBox cbValueGroupMode;
        private System.Windows.Forms.ComboBox cbValueGroupMode2;
    }
}