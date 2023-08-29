using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.UIv2
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.TopPanel = new Mids_Reborn.Controls.ctlPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.tbCharName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.MainPanel = new Mids_Reborn.Controls.ctlPanel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.powerFlowSplitter = new System.Windows.Forms.TableLayoutPanel();
            this.primaryFlow = new Mids_Reborn.Forms.Controls.PowerFlowPanel();
            this.inherentPoolFlow = new Mids_Reborn.Forms.Controls.PowerFlowPanel();
            this.secondaryFlow = new Mids_Reborn.Forms.Controls.PowerFlowPanel();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ancillaryPoolList = new Mids_Reborn.Forms.Controls.PowerList();
            this.poolList4 = new Mids_Reborn.Forms.Controls.PowerList();
            this.poolList3 = new Mids_Reborn.Forms.Controls.PowerList();
            this.poolList2 = new Mids_Reborn.Forms.Controls.PowerList();
            this.poolList1 = new Mids_Reborn.Forms.Controls.PowerList();
            this.secondaryPowerList = new Mids_Reborn.Forms.Controls.PowerList();
            this.primaryPowerList = new Mids_Reborn.Forms.Controls.PowerList();
            this.originCombo = new AtOriginCombo();
            this.atCombo = new AtOriginCombo();
            this.popup1 = new Mids_Reborn.Forms.Controls.Popup();
            this.MainPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.powerFlowSplitter.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.BackColor = System.Drawing.Color.Transparent;
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(1530, 50);
            this.TopPanel.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(50, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "Name:";
            // 
            // tbCharName
            // 
            this.tbCharName.Location = new System.Drawing.Point(109, 14);
            this.tbCharName.Name = "tbCharName";
            this.tbCharName.Size = new System.Drawing.Size(209, 22);
            this.tbCharName.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(21, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Archetype:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(50, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Origin:";
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.Transparent;
            this.MainPanel.Controls.Add(this.rightPanel);
            this.MainPanel.Controls.Add(this.buttonPanel);
            this.MainPanel.Controls.Add(this.leftPanel);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 50);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(1530, 903);
            this.MainPanel.TabIndex = 3;
            // 
            // rightPanel
            // 
            this.rightPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.rightPanel.Controls.Add(this.powerFlowSplitter);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(606, 80);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(924, 823);
            this.rightPanel.TabIndex = 50;
            // 
            // powerFlowSplitter
            // 
            this.powerFlowSplitter.ColumnCount = 3;
            this.powerFlowSplitter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.powerFlowSplitter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.powerFlowSplitter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.powerFlowSplitter.Controls.Add(this.inherentPoolFlow, 0, 0);
            this.powerFlowSplitter.Controls.Add(this.secondaryFlow, 0, 0);
            this.powerFlowSplitter.Controls.Add(this.primaryFlow, 0, 0);
            this.powerFlowSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.powerFlowSplitter.Location = new System.Drawing.Point(0, 0);
            this.powerFlowSplitter.Name = "powerFlowSplitter";
            this.powerFlowSplitter.RowCount = 1;
            this.powerFlowSplitter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.powerFlowSplitter.Size = new System.Drawing.Size(924, 823);
            this.powerFlowSplitter.TabIndex = 46;
            // 
            // inherentPoolFlow
            // 
            this.inherentPoolFlow.BorderColor = System.Drawing.Color.DodgerBlue;
            this.inherentPoolFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.inherentPoolFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.inherentPoolFlow.Location = new System.Drawing.Point(619, 3);
            this.inherentPoolFlow.Name = "inherentPoolFlow";
            this.inherentPoolFlow.Size = new System.Drawing.Size(302, 817);
            this.inherentPoolFlow.TabIndex = 0;
            // 
            // secondaryFlow
            // 
            this.secondaryFlow.BorderColor = System.Drawing.Color.DodgerBlue;
            this.secondaryFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.secondaryFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.secondaryFlow.Location = new System.Drawing.Point(311, 3);
            this.secondaryFlow.Name = "secondaryFlow";
            this.secondaryFlow.Size = new System.Drawing.Size(302, 817);
            this.secondaryFlow.TabIndex = 2;
            // 
            // primaryFlow
            // 
            this.primaryFlow.BorderColor = System.Drawing.Color.DodgerBlue;
            this.primaryFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.primaryFlow.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.primaryFlow.Location = new System.Drawing.Point(3, 3);
            this.primaryFlow.Name = "primaryFlow";
            this.primaryFlow.Size = new System.Drawing.Size(302, 817);
            this.primaryFlow.TabIndex = 1;
            // 
            // buttonPanel
            // 
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPanel.Location = new System.Drawing.Point(606, 0);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(924, 80);
            this.buttonPanel.TabIndex = 3;
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.panel1);
            this.leftPanel.Controls.Add(this.ancillaryPoolList);
            this.leftPanel.Controls.Add(this.poolList4);
            this.leftPanel.Controls.Add(this.poolList3);
            this.leftPanel.Controls.Add(this.poolList2);
            this.leftPanel.Controls.Add(this.poolList1);
            this.leftPanel.Controls.Add(this.secondaryPowerList);
            this.leftPanel.Controls.Add(this.primaryPowerList);
            this.leftPanel.Controls.Add(this.originCombo);
            this.leftPanel.Controls.Add(this.atCombo);
            this.leftPanel.Controls.Add(this.label4);
            this.leftPanel.Controls.Add(this.label3);
            this.leftPanel.Controls.Add(this.tbCharName);
            this.leftPanel.Controls.Add(this.label2);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(606, 903);
            this.leftPanel.TabIndex = 49;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panel1.Location = new System.Drawing.Point(12, 516);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(373, 375);
            this.panel1.TabIndex = 46;
            // 
            // ancillaryPoolList
            // 
            this.ancillaryPoolList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ancillaryPoolList.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.ancillaryPoolList.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Ancillary;
            this.ancillaryPoolList.Location = new System.Drawing.Point(404, 724);
            this.ancillaryPoolList.Name = "ancillaryPoolList";
            this.ancillaryPoolList.Size = new System.Drawing.Size(190, 150);
            this.ancillaryPoolList.TabIndex = 45;
            this.ancillaryPoolList.Text = "Ancillary/Epic Pool";
            this.ancillaryPoolList.TextOutline.Color = System.Drawing.Color.Black;
            this.ancillaryPoolList.TextOutline.Enabled = false;
            this.ancillaryPoolList.TextOutline.Width = 2;
            this.ancillaryPoolList.ItemHovered += PowerListOnItemHovered;
            this.ancillaryPoolList.MouseLeave += PowerListOnMouseLeave;
            // 
            // poolList4
            // 
            this.poolList4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.poolList4.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.poolList4.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Pool;
            this.poolList4.Location = new System.Drawing.Point(404, 568);
            this.poolList4.Name = "poolList4";
            this.poolList4.Size = new System.Drawing.Size(190, 150);
            this.poolList4.TabIndex = 44;
            this.poolList4.Text = "Pool 4";
            this.poolList4.TextOutline.Color = System.Drawing.Color.Black;
            this.poolList4.TextOutline.Enabled = false;
            this.poolList4.TextOutline.Width = 2;
            this.poolList4.ItemHovered += PowerListOnItemHovered;
            this.poolList4.MouseLeave += PowerListOnMouseLeave;
            // 
            // poolList3
            // 
            this.poolList3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.poolList3.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.poolList3.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Pool;
            this.poolList3.Location = new System.Drawing.Point(404, 412);
            this.poolList3.Name = "poolList3";
            this.poolList3.Size = new System.Drawing.Size(190, 150);
            this.poolList3.TabIndex = 43;
            this.poolList3.Text = "Pool 3";
            this.poolList3.TextOutline.Color = System.Drawing.Color.Black;
            this.poolList3.TextOutline.Enabled = false;
            this.poolList3.TextOutline.Width = 2;
            this.poolList3.ItemHovered += PowerListOnItemHovered;
            this.poolList3.MouseLeave += PowerListOnMouseLeave;
            // 
            // poolList2
            // 
            this.poolList2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.poolList2.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.poolList2.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Pool;
            this.poolList2.Location = new System.Drawing.Point(404, 256);
            this.poolList2.Name = "poolList2";
            this.poolList2.Size = new System.Drawing.Size(190, 150);
            this.poolList2.TabIndex = 42;
            this.poolList2.Text = "Pool 2";
            this.poolList2.TextOutline.Color = System.Drawing.Color.Black;
            this.poolList2.TextOutline.Enabled = false;
            this.poolList2.TextOutline.Width = 2;
            this.poolList2.ItemHovered += PowerListOnItemHovered;
            this.poolList2.MouseLeave += PowerListOnMouseLeave;
            // 
            // poolList1
            // 
            this.poolList1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.poolList1.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.poolList1.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Pool;
            this.poolList1.Location = new System.Drawing.Point(404, 100);
            this.poolList1.Name = "poolList1";
            this.poolList1.Size = new System.Drawing.Size(190, 150);
            this.poolList1.TabIndex = 41;
            this.poolList1.Text = "Pool 1";
            this.poolList1.TextOutline.Color = System.Drawing.Color.Black;
            this.poolList1.TextOutline.Enabled = false;
            this.poolList1.TextOutline.Width = 2;
            this.poolList1.ItemHovered += PowerListOnItemHovered;
            this.poolList1.MouseLeave += PowerListOnMouseLeave;
            // 
            // secondaryPowerList
            // 
            this.secondaryPowerList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.secondaryPowerList.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.secondaryPowerList.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Secondary;
            this.secondaryPowerList.Location = new System.Drawing.Point(208, 100);
            this.secondaryPowerList.Name = "secondaryPowerList";
            this.secondaryPowerList.Size = new System.Drawing.Size(190, 410);
            this.secondaryPowerList.TabIndex = 40;
            this.secondaryPowerList.Text = "Secondary";
            this.secondaryPowerList.TextOutline.Color = System.Drawing.Color.Black;
            this.secondaryPowerList.TextOutline.Enabled = false;
            this.secondaryPowerList.TextOutline.Width = 2;
            this.secondaryPowerList.ItemHovered += PowerListOnItemHovered;
            this.secondaryPowerList.MouseLeave += PowerListOnMouseLeave;
            // 
            // primaryPowerList
            // 
            this.primaryPowerList.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.primaryPowerList.DropDown.HighlightColor = System.Drawing.Color.DodgerBlue;
            this.primaryPowerList.DropDown.Type = Mids_Reborn.Forms.Controls.PowerList.DropDownType.Primary;
            this.primaryPowerList.Location = new System.Drawing.Point(12, 100);
            this.primaryPowerList.Name = "primaryPowerList";
            this.primaryPowerList.Size = new System.Drawing.Size(190, 410);
            this.primaryPowerList.TabIndex = 39;
            this.primaryPowerList.Text = "Primary";
            this.primaryPowerList.TextOutline.Color = System.Drawing.Color.Black;
            this.primaryPowerList.TextOutline.Enabled = false;
            this.primaryPowerList.TextOutline.Width = 2;
            this.primaryPowerList.ItemClicked += PrimaryPowerListOnItemClicked;
            this.primaryPowerList.ItemHovered += PowerListOnItemHovered;
            this.primaryPowerList.MouseLeave += PowerListOnMouseLeave;
            // 
            // originCombo
            // 
            this.originCombo.ComboType = AtOriginCombo.ComboBoxType.Origin;
            this.originCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.originCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.originCombo.FormattingEnabled = true;
            this.originCombo.HighlightColor = System.Drawing.Color.Empty;
            this.originCombo.Location = new System.Drawing.Point(109, 71);
            this.originCombo.Name = "originCombo";
            this.originCombo.Size = new System.Drawing.Size(209, 23);
            this.originCombo.TabIndex = 38;
            // 
            // atCombo
            // 
            this.atCombo.ComboType = AtOriginCombo.ComboBoxType.Archetype;
            this.atCombo.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.atCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.atCombo.FormattingEnabled = true;
            this.atCombo.HighlightColor = System.Drawing.Color.Empty;
            this.atCombo.Location = new System.Drawing.Point(109, 42);
            this.atCombo.Name = "atCombo";
            this.atCombo.Size = new System.Drawing.Size(209, 23);
            this.atCombo.TabIndex = 37;
            this.atCombo.SelectedIndexChanged += new System.EventHandler(this.AtComboOnSelectedIndexChanged);
            // 
            // popup1
            // 
            this.popup1.AutoSize = true;
            this.popup1.BackColor = System.Drawing.Color.Black;
            this.popup1.BorderColor = System.Drawing.Color.DodgerBlue;
            this.popup1.Location = new System.Drawing.Point(576, 50);
            this.popup1.Name = "popup1";
            this.popup1.Size = new System.Drawing.Size(212, 91);
            this.popup1.TabIndex = 47;
            this.popup1.Visible = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 6F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1530, 953);
            this.Controls.Add(this.popup1);
            this.Controls.Add(this.MainPanel);
            this.Controls.Add(this.TopPanel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mids` Reborn : Hero Designer";
            this.MainPanel.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.powerFlowSplitter.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.leftPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ctlPanel TopPanel;
        private AtOriginCombo cbAT;
        private AtOriginCombo cbOrigin;
        private AtOriginCombo cbSecondary;
        private AtOriginCombo cbPrimary;
        private AtOriginCombo cbPool1;
        private AtOriginCombo cbPool0;
        private AtOriginCombo cbPool3;
        private AtOriginCombo cbPool2;
        private AtOriginCombo cbAncillary;
        private Label label2;
        private TextBox tbCharName;
        private Label label3;
        private Label label4;
        private ctlPanel MainPanel;
        private AtOriginCombo originCombo;
        private AtOriginCombo atCombo;
        private PowerList ancillaryPoolList;
        private PowerList poolList4;
        private PowerList poolList3;
        private PowerList poolList2;
        private PowerList poolList1;
        private PowerList secondaryPowerList;
        private PowerList primaryPowerList;
        private Panel rightPanel;
        private PowerFlowPanel inherentPoolFlow;
        private PowerFlowPanel secondaryFlow;
        private PowerFlowPanel primaryFlow;
        private Panel leftPanel;
        private Panel buttonPanel;
        private TableLayoutPanel powerFlowSplitter;
        private Panel panel1;
        private Popup popup1;
    }
}