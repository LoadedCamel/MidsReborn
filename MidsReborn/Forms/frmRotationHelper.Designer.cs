using System.Windows.Forms;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms
{
    partial class frmRotationHelper
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRotationHelper));
            listBox1 = new ListBox();
            label1 = new Label();
            cbViewProfile = new ComboBox();
            label2 = new Label();
            label3 = new Label();
            listBox2 = new ListBox();
            progressBarEx1 = new MRBUpdater.Controls.ProgressBarEx();
            imageButtonEx1 = new Controls.ImageButtonEx();
            label4 = new Label();
            btnClearPowers = new Button();
            btnClearBoosts = new Button();
            btnCopyRotation = new Button();
            borderPanel1 = new Controls.BorderPanel();
            lblRotation = new Label();
            label5 = new Label();
            label6 = new Label();
            btnAddPower = new Button();
            btnAddBoost = new Button();
            btnCalcTimeline = new Button();
            borderPanel2 = new Controls.BorderPanel();
            ctlCombatTimeline1 = new ctlCombatTimeline();
            borderPanel3 = new Controls.BorderPanel();
            lblBoosts = new Label();
            chkCastTimeReal = new CheckBox();
            imageButtonEx2 = new Controls.ImageButtonEx();
            button1 = new Button();
            button2 = new Button();
            btnClearLastPower = new Button();
            btnColorsRef = new Controls.ImageButtonEx();
            btnAddAllBoosts = new Button();
            borderPanel4 = new Controls.BorderPanel();
            timelineCursorZoom1 = new Controls.TimelineCursorZoom();
            ibxZoomOut = new Controls.ImageButtonEx();
            ibxZoomIn = new Controls.ImageButtonEx();
            borderPanel1.SuspendLayout();
            borderPanel2.SuspendLayout();
            borderPanel3.SuspendLayout();
            borderPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new System.Drawing.Point(12, 52);
            listBox1.Name = "listBox1";
            listBox1.Size = new System.Drawing.Size(400, 199);
            listBox1.TabIndex = 0;
            listBox1.DoubleClick += listBox1_DoubleClick;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label1.Location = new System.Drawing.Point(12, 20);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(104, 15);
            label1.TabIndex = 1;
            label1.Text = "Available powers:";
            // 
            // cbViewProfile
            // 
            cbViewProfile.FormattingEnabled = true;
            cbViewProfile.Items.AddRange(new object[] { "Damage", "Healing", "Survival" });
            cbViewProfile.Location = new System.Drawing.Point(444, 52);
            cbViewProfile.Name = "cbViewProfile";
            cbViewProfile.Size = new System.Drawing.Size(248, 23);
            cbViewProfile.TabIndex = 2;
            cbViewProfile.SelectedIndexChanged += cbViewProfile_SelectedIndexChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label2.Location = new System.Drawing.Point(444, 20);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(47, 15);
            label2.TabIndex = 3;
            label2.Text = "Profile:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label3.Location = new System.Drawing.Point(727, 20);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(47, 15);
            label3.TabIndex = 4;
            label3.Text = "Boosts:";
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.ItemHeight = 15;
            listBox2.Location = new System.Drawing.Point(727, 52);
            listBox2.Name = "listBox2";
            listBox2.Size = new System.Drawing.Size(399, 199);
            listBox2.TabIndex = 5;
            listBox2.DoubleClick += listBox2_DoubleClick;
            // 
            // progressBarEx1
            // 
            progressBarEx1.Border.Style = ButtonBorderStyle.Solid;
            progressBarEx1.Border.Thickness = 1;
            progressBarEx1.Border.Which = MRBUpdater.Controls.ProgressBarEx.ProgressBorder.BorderToDraw.All;
            progressBarEx1.Colors.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            progressBarEx1.Colors.BarEndColor = System.Drawing.Color.FromArgb(64, 78, 237);
            progressBarEx1.Colors.BarStartColor = System.Drawing.Color.FromArgb(30, 144, 255);
            progressBarEx1.Colors.BorderColor = System.Drawing.Color.FromArgb(44, 47, 51);
            progressBarEx1.Colors.TextColor = System.Drawing.Color.WhiteSmoke;
            progressBarEx1.Location = new System.Drawing.Point(12, 580);
            progressBarEx1.Name = "progressBarEx1";
            progressBarEx1.Size = new System.Drawing.Size(1124, 23);
            progressBarEx1.TabIndex = 6;
            progressBarEx1.Visible = false;
            // 
            // imageButtonEx1
            // 
            imageButtonEx1.BackgroundImageLayout = ImageLayout.None;
            imageButtonEx1.CurrentText = "Close";
            imageButtonEx1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            imageButtonEx1.ForeColor = System.Drawing.Color.WhiteSmoke;
            imageButtonEx1.Images.Background = MRBResourceLib.Resources.HeroButton;
            imageButtonEx1.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            imageButtonEx1.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            imageButtonEx1.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            imageButtonEx1.Location = new System.Drawing.Point(988, 896);
            imageButtonEx1.Lock = false;
            imageButtonEx1.Name = "imageButtonEx1";
            imageButtonEx1.Size = new System.Drawing.Size(120, 30);
            imageButtonEx1.TabIndex = 8;
            imageButtonEx1.Text = "Close";
            imageButtonEx1.TextOutline.Color = System.Drawing.Color.Black;
            imageButtonEx1.TextOutline.Width = 2;
            imageButtonEx1.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            imageButtonEx1.ToggleText.Indeterminate = "Indeterminate State";
            imageButtonEx1.ToggleText.ToggledOff = "ToggledOff State";
            imageButtonEx1.ToggleText.ToggledOn = "ToggledOn State";
            imageButtonEx1.UseAlt = false;
            imageButtonEx1.Click += imageButtonEx1_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
            label4.Location = new System.Drawing.Point(12, 555);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(172, 15);
            label4.TabIndex = 9;
            label4.Text = "Calculating enhanced powers...";
            label4.Visible = false;
            // 
            // btnClearPowers
            // 
            btnClearPowers.Location = new System.Drawing.Point(289, 261);
            btnClearPowers.Name = "btnClearPowers";
            btnClearPowers.Size = new System.Drawing.Size(123, 33);
            btnClearPowers.TabIndex = 10;
            btnClearPowers.Text = "Clear all powers";
            btnClearPowers.UseVisualStyleBackColor = true;
            btnClearPowers.Click += btnClearPowers_Click;
            // 
            // btnClearBoosts
            // 
            btnClearBoosts.Location = new System.Drawing.Point(1003, 261);
            btnClearBoosts.Name = "btnClearBoosts";
            btnClearBoosts.Size = new System.Drawing.Size(123, 33);
            btnClearBoosts.TabIndex = 11;
            btnClearBoosts.Text = "Clear boosts";
            btnClearBoosts.UseVisualStyleBackColor = true;
            btnClearBoosts.Click += btnClearBoosts_Click;
            // 
            // btnCopyRotation
            // 
            btnCopyRotation.Location = new System.Drawing.Point(387, 504);
            btnCopyRotation.Name = "btnCopyRotation";
            btnCopyRotation.Size = new System.Drawing.Size(123, 33);
            btnCopyRotation.TabIndex = 13;
            btnCopyRotation.Text = "Copy rotation";
            btnCopyRotation.UseVisualStyleBackColor = true;
            btnCopyRotation.Click += btnCopyRotation_Click;
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = System.Drawing.Color.FromArgb(44, 47, 51);
            borderPanel1.Border.Style = ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 1;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel1.Controls.Add(lblRotation);
            borderPanel1.Location = new System.Drawing.Point(12, 439);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(1124, 59);
            borderPanel1.TabIndex = 14;
            // 
            // lblRotation
            // 
            lblRotation.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblRotation.ForeColor = System.Drawing.Color.Goldenrod;
            lblRotation.Location = new System.Drawing.Point(3, 3);
            lblRotation.Name = "lblRotation";
            lblRotation.Size = new System.Drawing.Size(1118, 53);
            lblRotation.TabIndex = 0;
            lblRotation.Text = "Rotation";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label5.Location = new System.Drawing.Point(12, 417);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(58, 15);
            label5.TabIndex = 16;
            label5.Text = "Rotation:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            label6.Location = new System.Drawing.Point(12, 317);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(47, 15);
            label6.TabIndex = 17;
            label6.Text = "Boosts:";
            // 
            // btnAddPower
            // 
            btnAddPower.Location = new System.Drawing.Point(12, 261);
            btnAddPower.Name = "btnAddPower";
            btnAddPower.Size = new System.Drawing.Size(123, 33);
            btnAddPower.TabIndex = 19;
            btnAddPower.Text = "Add to rotation";
            btnAddPower.UseVisualStyleBackColor = true;
            btnAddPower.Click += btnAddPower_Click;
            // 
            // btnAddBoost
            // 
            btnAddBoost.Location = new System.Drawing.Point(865, 261);
            btnAddBoost.Name = "btnAddBoost";
            btnAddBoost.Size = new System.Drawing.Size(123, 33);
            btnAddBoost.TabIndex = 20;
            btnAddBoost.Text = "Add to boosts";
            btnAddBoost.UseVisualStyleBackColor = true;
            btnAddBoost.Click += btnAddBoost_Click;
            // 
            // btnCalcTimeline
            // 
            btnCalcTimeline.Location = new System.Drawing.Point(603, 504);
            btnCalcTimeline.Name = "btnCalcTimeline";
            btnCalcTimeline.Size = new System.Drawing.Size(123, 33);
            btnCalcTimeline.TabIndex = 21;
            btnCalcTimeline.Text = "Calculate timeline";
            btnCalcTimeline.UseVisualStyleBackColor = true;
            btnCalcTimeline.Click += btnCalcTimeline_Click;
            // 
            // borderPanel2
            // 
            borderPanel2.AutoScroll = true;
            borderPanel2.Border.Color = System.Drawing.Color.FromArgb(16, 76, 135);
            borderPanel2.Border.Style = ButtonBorderStyle.Solid;
            borderPanel2.Border.Thickness = 1;
            borderPanel2.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel2.Controls.Add(ctlCombatTimeline1);
            borderPanel2.Location = new System.Drawing.Point(12, 619);
            borderPanel2.Name = "borderPanel2";
            borderPanel2.Size = new System.Drawing.Size(1124, 215);
            borderPanel2.TabIndex = 22;
            // 
            // ctlCombatTimeline1
            // 
            ctlCombatTimeline1.Location = new System.Drawing.Point(3, 3);
            ctlCombatTimeline1.Margin = new Padding(0);
            ctlCombatTimeline1.Name = "ctlCombatTimeline1";
            ctlCombatTimeline1.Profile = ctlCombatTimeline.ViewProfileType.Damage;
            ctlCombatTimeline1.Size = new System.Drawing.Size(1118, 209);
            ctlCombatTimeline1.TabIndex = 16;
            ctlCombatTimeline1.UseArcanaTime = true;
            ctlCombatTimeline1.UserBoosts = (System.Collections.Generic.List<Core.IPower>)resources.GetObject("ctlCombatTimeline1.UserBoosts");
            ctlCombatTimeline1.CalcEnhancedProgress += ctlCombatTimeline1_CalcEnhancedProgress;
            ctlCombatTimeline1.SetZoom += ctlCombatTimeline1_SetZoom;
            // 
            // borderPanel3
            // 
            borderPanel3.Border.Color = System.Drawing.Color.FromArgb(44, 47, 51);
            borderPanel3.Border.Style = ButtonBorderStyle.Solid;
            borderPanel3.Border.Thickness = 1;
            borderPanel3.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel3.Controls.Add(lblBoosts);
            borderPanel3.Location = new System.Drawing.Point(12, 339);
            borderPanel3.Name = "borderPanel3";
            borderPanel3.Size = new System.Drawing.Size(1124, 59);
            borderPanel3.TabIndex = 23;
            // 
            // lblBoosts
            // 
            lblBoosts.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lblBoosts.ForeColor = System.Drawing.Color.Chartreuse;
            lblBoosts.Location = new System.Drawing.Point(3, 3);
            lblBoosts.Name = "lblBoosts";
            lblBoosts.Size = new System.Drawing.Size(1118, 53);
            lblBoosts.TabIndex = 19;
            lblBoosts.Text = "Boosts";
            // 
            // chkCastTimeReal
            // 
            chkCastTimeReal.AutoSize = true;
            chkCastTimeReal.Location = new System.Drawing.Point(444, 106);
            chkCastTimeReal.Name = "chkCastTimeReal";
            chkCastTimeReal.Size = new System.Drawing.Size(140, 19);
            chkCastTimeReal.TabIndex = 24;
            chkCastTimeReal.Text = "Use Arcana Cast Time";
            chkCastTimeReal.UseVisualStyleBackColor = true;
            chkCastTimeReal.CheckedChanged += chkCastTimeReal_CheckedChanged;
            // 
            // imageButtonEx2
            // 
            imageButtonEx2.BackgroundImageLayout = ImageLayout.None;
            imageButtonEx2.ButtonType = Forms.Controls.ImageButtonEx.ButtonTypes.Toggle;
            imageButtonEx2.CurrentText = "ToggledOff State";
            imageButtonEx2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            imageButtonEx2.ForeColor = System.Drawing.Color.WhiteSmoke;
            imageButtonEx2.Images.Background = MRBResourceLib.Resources.HeroButton;
            imageButtonEx2.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            imageButtonEx2.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            imageButtonEx2.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            imageButtonEx2.Location = new System.Drawing.Point(835, 896);
            imageButtonEx2.Lock = false;
            imageButtonEx2.Name = "imageButtonEx2";
            imageButtonEx2.Size = new System.Drawing.Size(120, 30);
            imageButtonEx2.TabIndex = 25;
            imageButtonEx2.Text = "Top Most";
            imageButtonEx2.TextOutline.Color = System.Drawing.Color.Black;
            imageButtonEx2.TextOutline.Width = 2;
            imageButtonEx2.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            imageButtonEx2.ToggleText.Indeterminate = "Indeterminate State";
            imageButtonEx2.ToggleText.ToggledOff = "To Top";
            imageButtonEx2.ToggleText.ToggledOn = "Top Most";
            imageButtonEx2.UseAlt = false;
            imageButtonEx2.Click += imageButtonEx2_Click;
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(798, 504);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(131, 33);
            button1.TabIndex = 26;
            button1.Text = "Test rotation (full)";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(967, 504);
            button2.Name = "button2";
            button2.Size = new System.Drawing.Size(131, 33);
            button2.TabIndex = 27;
            button2.Text = "Test rotation (simple)";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // btnClearLastPower
            // 
            btnClearLastPower.Location = new System.Drawing.Point(150, 261);
            btnClearLastPower.Name = "btnClearLastPower";
            btnClearLastPower.Size = new System.Drawing.Size(123, 33);
            btnClearLastPower.TabIndex = 28;
            btnClearLastPower.Text = "Clear last power";
            btnClearLastPower.UseVisualStyleBackColor = true;
            btnClearLastPower.Click += btnClearLastPower_Click;
            // 
            // btnColorsRef
            // 
            btnColorsRef.BackgroundImageLayout = ImageLayout.None;
            btnColorsRef.CurrentText = "Colors Reference";
            btnColorsRef.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            btnColorsRef.ForeColor = System.Drawing.Color.WhiteSmoke;
            btnColorsRef.Images.Background = MRBResourceLib.Resources.HeroButton;
            btnColorsRef.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            btnColorsRef.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            btnColorsRef.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            btnColorsRef.Location = new System.Drawing.Point(630, 896);
            btnColorsRef.Lock = false;
            btnColorsRef.Name = "btnColorsRef";
            btnColorsRef.Size = new System.Drawing.Size(142, 30);
            btnColorsRef.TabIndex = 29;
            btnColorsRef.Text = "Colors Reference";
            btnColorsRef.TextOutline.Color = System.Drawing.Color.Black;
            btnColorsRef.TextOutline.Width = 2;
            btnColorsRef.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            btnColorsRef.ToggleText.Indeterminate = "Indeterminate State";
            btnColorsRef.ToggleText.ToggledOff = "ToggledOff State";
            btnColorsRef.ToggleText.ToggledOn = "ToggledOn State";
            btnColorsRef.UseAlt = false;
            btnColorsRef.Click += btnColorsRef_Click;
            // 
            // btnAddAllBoosts
            // 
            btnAddAllBoosts.Location = new System.Drawing.Point(727, 261);
            btnAddAllBoosts.Name = "btnAddAllBoosts";
            btnAddAllBoosts.Size = new System.Drawing.Size(123, 33);
            btnAddAllBoosts.TabIndex = 33;
            btnAddAllBoosts.Text = "Add all";
            btnAddAllBoosts.UseVisualStyleBackColor = true;
            btnAddAllBoosts.Click += btnAddAllBoosts_Click;
            // 
            // borderPanel4
            // 
            borderPanel4.Border.Color = System.Drawing.Color.FromArgb(12, 56, 100);
            borderPanel4.Border.Style = ButtonBorderStyle.Solid;
            borderPanel4.Border.Thickness = 1;
            borderPanel4.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            borderPanel4.Controls.Add(timelineCursorZoom1);
            borderPanel4.Location = new System.Drawing.Point(12, 840);
            borderPanel4.Name = "borderPanel4";
            borderPanel4.Size = new System.Drawing.Size(1124, 30);
            borderPanel4.TabIndex = 34;
            // 
            // timelineCursorZoom1
            // 
            timelineCursorZoom1.BackColor = System.Drawing.Color.FromArgb(41, 49, 52);
            timelineCursorZoom1.EndMarkerColor = System.Drawing.Color.FromArgb(255, 32, 32);
            timelineCursorZoom1.ForeColor = System.Drawing.Color.Gainsboro;
            timelineCursorZoom1.GridColor = System.Drawing.Color.FromArgb(108, 120, 140);
            timelineCursorZoom1.HoveredPosColor = System.Drawing.Color.BurlyWood;
            timelineCursorZoom1.Location = new System.Drawing.Point(3, 3);
            timelineCursorZoom1.MarkersColor = System.Drawing.Color.FromArgb(147, 199, 99);
            timelineCursorZoom1.Name = "timelineCursorZoom1";
            timelineCursorZoom1.Size = new System.Drawing.Size(1118, 24);
            timelineCursorZoom1.StartMarkerColor = System.Drawing.Color.FromArgb(32, 32, 255);
            timelineCursorZoom1.TabIndex = 0;
            timelineCursorZoom1.TabStop = false;
            timelineCursorZoom1.TextShadowColor = System.Drawing.Color.Black;
            timelineCursorZoom1.TextSize = 10F;
            timelineCursorZoom1.ViewIntervalChanged += timelineCursorZoom1_ViewIntervalChanged;
            // 
            // ibxZoomOut
            // 
            ibxZoomOut.BackgroundImageLayout = ImageLayout.None;
            ibxZoomOut.CurrentText = "-";
            ibxZoomOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibxZoomOut.ForeColor = System.Drawing.Color.WhiteSmoke;
            ibxZoomOut.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibxZoomOut.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibxZoomOut.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibxZoomOut.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibxZoomOut.Location = new System.Drawing.Point(30, 896);
            ibxZoomOut.Lock = false;
            ibxZoomOut.Name = "ibxZoomOut";
            ibxZoomOut.Size = new System.Drawing.Size(40, 30);
            ibxZoomOut.TabIndex = 35;
            ibxZoomOut.Text = "-";
            ibxZoomOut.TextOutline.Color = System.Drawing.Color.Black;
            ibxZoomOut.TextOutline.Width = 2;
            ibxZoomOut.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibxZoomOut.ToggleText.Indeterminate = "Indeterminate State";
            ibxZoomOut.ToggleText.ToggledOff = "ToggledOff State";
            ibxZoomOut.ToggleText.ToggledOn = "ToggledOn State";
            ibxZoomOut.UseAlt = false;
            ibxZoomOut.Visible = false;
            ibxZoomOut.Click += ibxZoomOut_Click;
            // 
            // ibxZoomIn
            // 
            ibxZoomIn.BackgroundImageLayout = ImageLayout.None;
            ibxZoomIn.CurrentText = "+";
            ibxZoomIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            ibxZoomIn.ForeColor = System.Drawing.Color.WhiteSmoke;
            ibxZoomIn.Images.Background = MRBResourceLib.Resources.HeroButton;
            ibxZoomIn.Images.Hover = MRBResourceLib.Resources.HeroButtonHover;
            ibxZoomIn.ImagesAlt.Background = MRBResourceLib.Resources.VillainButton;
            ibxZoomIn.ImagesAlt.Hover = MRBResourceLib.Resources.VillainButtonHover;
            ibxZoomIn.Location = new System.Drawing.Point(95, 896);
            ibxZoomIn.Lock = false;
            ibxZoomIn.Name = "ibxZoomIn";
            ibxZoomIn.Size = new System.Drawing.Size(40, 30);
            ibxZoomIn.TabIndex = 36;
            ibxZoomIn.Text = "+";
            ibxZoomIn.TextOutline.Color = System.Drawing.Color.Black;
            ibxZoomIn.TextOutline.Width = 2;
            ibxZoomIn.ToggleState = Forms.Controls.ImageButtonEx.States.ToggledOff;
            ibxZoomIn.ToggleText.Indeterminate = "Indeterminate State";
            ibxZoomIn.ToggleText.ToggledOff = "ToggledOff State";
            ibxZoomIn.ToggleText.ToggledOn = "ToggledOn State";
            ibxZoomIn.UseAlt = false;
            ibxZoomIn.Visible = false;
            ibxZoomIn.Click += ibxZoomIn_Click;
            // 
            // frmRotationHelper
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.Black;
            ClientSize = new System.Drawing.Size(1148, 944);
            Controls.Add(ibxZoomIn);
            Controls.Add(ibxZoomOut);
            Controls.Add(borderPanel4);
            Controls.Add(btnAddAllBoosts);
            Controls.Add(btnColorsRef);
            Controls.Add(btnClearLastPower);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(imageButtonEx2);
            Controls.Add(chkCastTimeReal);
            Controls.Add(borderPanel3);
            Controls.Add(borderPanel2);
            Controls.Add(btnCalcTimeline);
            Controls.Add(btnAddBoost);
            Controls.Add(btnAddPower);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(borderPanel1);
            Controls.Add(btnCopyRotation);
            Controls.Add(btnClearBoosts);
            Controls.Add(btnClearPowers);
            Controls.Add(label4);
            Controls.Add(imageButtonEx1);
            Controls.Add(progressBarEx1);
            Controls.Add(listBox2);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(cbViewProfile);
            Controls.Add(label1);
            Controls.Add(listBox1);
            DoubleBuffered = true;
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "frmRotationHelper";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Rotation Helper (beta)";
            Load += frmRotationHelper_Load;
            borderPanel1.ResumeLayout(false);
            borderPanel2.ResumeLayout(false);
            borderPanel3.ResumeLayout(false);
            borderPanel4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Label label1;
        private ComboBox cbViewProfile;
        private Label label2;
        private Label label3;
        private ListBox listBox2;
        private MRBUpdater.Controls.ProgressBarEx progressBarEx1;
        private Controls.ImageButtonEx imageButtonEx1;
        private Label label4;
        private Button btnClearPowers;
        private Button btnClearBoosts;
        private Button btnCopyRotation;
        private Controls.BorderPanel borderPanel1;
        private Label lblRotation;
        private Label label5;
        private Label label6;
        private Button btnAddPower;
        private Button btnAddBoost;
        private Button btnCalcTimeline;
        private Controls.BorderPanel borderPanel2;
        private ctlCombatTimeline ctlCombatTimeline1;
        private Controls.BorderPanel borderPanel3;
        private Label lblBoosts;
        private CheckBox chkCastTimeReal;
        private Controls.ImageButtonEx imageButtonEx2;
        private Button button1;
        private Button button2;
        private Button btnClearLastPower;
        private Controls.ImageButtonEx btnColorsRef;
        private Button btnAddAllBoosts;
        private Controls.BorderPanel borderPanel4;
        private Controls.TimelineCursorZoom timelineCursorZoom1;
        private Controls.ImageButtonEx ibxZoomOut;
        private Controls.ImageButtonEx ibxZoomIn;
    }
}