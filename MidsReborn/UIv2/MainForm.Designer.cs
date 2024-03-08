using System.Windows.Forms;
using Mids_Reborn.UIv2.Controls;

namespace Mids_Reborn.UIv2
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            panelSideMenu = new Forms.Controls.BorderPanel();
            buttonConfig = new Button();
            panelBuild = new Panel();
            buttonSaveAs = new Button();
            buttonSave = new Button();
            buttonNew = new Button();
            buttonLoad = new Button();
            buttonBuild = new Button();
            panelLogo = new Forms.Controls.BorderPanel();
            tableLayoutPanel1 = new TableLayoutPanel();
            minimizeButton = new FontAwesome.Sharp.IconButton();
            maximizeButton = new FontAwesome.Sharp.IconButton();
            closeButton = new FontAwesome.Sharp.IconButton();
            borderPanel1 = new Forms.Controls.BorderPanel();
            dataVersionLabel = new Label();
            formPages1 = new Mids_Reborn.Controls.FormPages();
            page1 = new Mids_Reborn.Controls.Page();
            powerPanel1 = new PowerPanel();
            panelSideMenu.SuspendLayout();
            panelBuild.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            borderPanel1.SuspendLayout();
            formPages1.SuspendLayout();
            page1.SuspendLayout();
            SuspendLayout();
            // 
            // panelSideMenu
            // 
            panelSideMenu.BackColor = System.Drawing.Color.FromArgb(32, 34, 37);
            panelSideMenu.BackgroundImageLayout = ImageLayout.Stretch;
            panelSideMenu.Border.Color = System.Drawing.Color.FromArgb(32, 34, 37);
            panelSideMenu.Border.Style = ButtonBorderStyle.Solid;
            panelSideMenu.Border.Thickness = 2;
            panelSideMenu.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.All;
            panelSideMenu.Controls.Add(buttonConfig);
            panelSideMenu.Controls.Add(panelBuild);
            panelSideMenu.Controls.Add(buttonBuild);
            panelSideMenu.Controls.Add(panelLogo);
            panelSideMenu.Dock = DockStyle.Fill;
            panelSideMenu.Location = new System.Drawing.Point(3, 3);
            panelSideMenu.Name = "panelSideMenu";
            tableLayoutPanel1.SetRowSpan(panelSideMenu, 2);
            panelSideMenu.Size = new System.Drawing.Size(178, 594);
            panelSideMenu.TabIndex = 5;
            // 
            // buttonConfig
            // 
            buttonConfig.BackColor = System.Drawing.Color.Transparent;
            buttonConfig.Dock = DockStyle.Top;
            buttonConfig.FlatAppearance.BorderSize = 0;
            buttonConfig.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(78, 86, 214);
            buttonConfig.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(108, 122, 224);
            buttonConfig.FlatStyle = FlatStyle.Flat;
            buttonConfig.ForeColor = System.Drawing.Color.WhiteSmoke;
            buttonConfig.Location = new System.Drawing.Point(0, 287);
            buttonConfig.Name = "buttonConfig";
            buttonConfig.Padding = new Padding(10, 0, 0, 0);
            buttonConfig.Size = new System.Drawing.Size(178, 45);
            buttonConfig.TabIndex = 3;
            buttonConfig.Text = "Config";
            buttonConfig.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            buttonConfig.UseVisualStyleBackColor = false;
            // 
            // panelBuild
            // 
            panelBuild.BackColor = System.Drawing.Color.FromArgb(47, 49, 54);
            panelBuild.Controls.Add(buttonSaveAs);
            panelBuild.Controls.Add(buttonSave);
            panelBuild.Controls.Add(buttonNew);
            panelBuild.Controls.Add(buttonLoad);
            panelBuild.Dock = DockStyle.Top;
            panelBuild.Location = new System.Drawing.Point(0, 119);
            panelBuild.Name = "panelBuild";
            panelBuild.Size = new System.Drawing.Size(178, 168);
            panelBuild.TabIndex = 2;
            // 
            // buttonSaveAs
            // 
            buttonSaveAs.BackColor = System.Drawing.Color.Transparent;
            buttonSaveAs.Dock = DockStyle.Top;
            buttonSaveAs.FlatAppearance.BorderSize = 0;
            buttonSaveAs.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(78, 86, 214);
            buttonSaveAs.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(108, 122, 224);
            buttonSaveAs.FlatStyle = FlatStyle.Flat;
            buttonSaveAs.ForeColor = System.Drawing.Color.WhiteSmoke;
            buttonSaveAs.Location = new System.Drawing.Point(0, 120);
            buttonSaveAs.Name = "buttonSaveAs";
            buttonSaveAs.Padding = new Padding(35, 0, 0, 0);
            buttonSaveAs.Size = new System.Drawing.Size(178, 40);
            buttonSaveAs.TabIndex = 3;
            buttonSaveAs.Text = "Save As...";
            buttonSaveAs.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            buttonSaveAs.UseVisualStyleBackColor = false;
            // 
            // buttonSave
            // 
            buttonSave.BackColor = System.Drawing.Color.Transparent;
            buttonSave.Dock = DockStyle.Top;
            buttonSave.FlatAppearance.BorderSize = 0;
            buttonSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(78, 86, 214);
            buttonSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(108, 122, 224);
            buttonSave.FlatStyle = FlatStyle.Flat;
            buttonSave.ForeColor = System.Drawing.Color.WhiteSmoke;
            buttonSave.Location = new System.Drawing.Point(0, 80);
            buttonSave.Name = "buttonSave";
            buttonSave.Padding = new Padding(35, 0, 0, 0);
            buttonSave.Size = new System.Drawing.Size(178, 40);
            buttonSave.TabIndex = 2;
            buttonSave.Text = "Save";
            buttonSave.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            buttonSave.UseVisualStyleBackColor = false;
            // 
            // buttonNew
            // 
            buttonNew.BackColor = System.Drawing.Color.Transparent;
            buttonNew.Dock = DockStyle.Top;
            buttonNew.FlatAppearance.BorderSize = 0;
            buttonNew.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(78, 86, 214);
            buttonNew.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(108, 122, 224);
            buttonNew.FlatStyle = FlatStyle.Flat;
            buttonNew.ForeColor = System.Drawing.Color.WhiteSmoke;
            buttonNew.Location = new System.Drawing.Point(0, 40);
            buttonNew.Name = "buttonNew";
            buttonNew.Padding = new Padding(35, 0, 0, 0);
            buttonNew.Size = new System.Drawing.Size(178, 40);
            buttonNew.TabIndex = 1;
            buttonNew.Text = "New";
            buttonNew.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            buttonNew.UseVisualStyleBackColor = false;
            // 
            // buttonLoad
            // 
            buttonLoad.BackColor = System.Drawing.Color.Transparent;
            buttonLoad.Dock = DockStyle.Top;
            buttonLoad.FlatAppearance.BorderSize = 0;
            buttonLoad.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(78, 86, 214);
            buttonLoad.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(108, 122, 224);
            buttonLoad.FlatStyle = FlatStyle.Flat;
            buttonLoad.ForeColor = System.Drawing.Color.WhiteSmoke;
            buttonLoad.Location = new System.Drawing.Point(0, 0);
            buttonLoad.Name = "buttonLoad";
            buttonLoad.Padding = new Padding(35, 0, 0, 0);
            buttonLoad.Size = new System.Drawing.Size(178, 40);
            buttonLoad.TabIndex = 0;
            buttonLoad.Text = "Load";
            buttonLoad.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            buttonLoad.UseVisualStyleBackColor = false;
            // 
            // buttonBuild
            // 
            buttonBuild.BackColor = System.Drawing.Color.Transparent;
            buttonBuild.Dock = DockStyle.Top;
            buttonBuild.FlatAppearance.BorderSize = 0;
            buttonBuild.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(78, 86, 214);
            buttonBuild.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(108, 122, 224);
            buttonBuild.FlatStyle = FlatStyle.Flat;
            buttonBuild.ForeColor = System.Drawing.Color.WhiteSmoke;
            buttonBuild.Location = new System.Drawing.Point(0, 74);
            buttonBuild.Name = "buttonBuild";
            buttonBuild.Padding = new Padding(10, 0, 0, 0);
            buttonBuild.Size = new System.Drawing.Size(178, 45);
            buttonBuild.TabIndex = 1;
            buttonBuild.Text = "Build";
            buttonBuild.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            buttonBuild.UseVisualStyleBackColor = false;
            // 
            // panelLogo
            // 
            panelLogo.BackColor = System.Drawing.Color.Transparent;
            panelLogo.BackgroundImage = (System.Drawing.Image)resources.GetObject("panelLogo.BackgroundImage");
            panelLogo.BackgroundImageLayout = ImageLayout.Stretch;
            panelLogo.Border.Color = System.Drawing.Color.FromArgb(32, 34, 37);
            panelLogo.Border.Style = ButtonBorderStyle.Solid;
            panelLogo.Border.Thickness = 2;
            panelLogo.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.Bottom;
            panelLogo.Dock = DockStyle.Top;
            panelLogo.Location = new System.Drawing.Point(0, 0);
            panelLogo.Name = "panelLogo";
            panelLogo.Size = new System.Drawing.Size(178, 74);
            panelLogo.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            tableLayoutPanel1.BackgroundImageLayout = ImageLayout.Stretch;
            tableLayoutPanel1.ColumnCount = 6;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 21.6765461F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 78.3234558F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 2F));
            tableLayoutPanel1.Controls.Add(panelSideMenu, 0, 0);
            tableLayoutPanel1.Controls.Add(minimizeButton, 2, 0);
            tableLayoutPanel1.Controls.Add(maximizeButton, 3, 0);
            tableLayoutPanel1.Controls.Add(closeButton, 4, 0);
            tableLayoutPanel1.Controls.Add(borderPanel1, 1, 0);
            tableLayoutPanel1.Controls.Add(formPages1, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 5.16666651F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 94.8333359F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new System.Drawing.Size(950, 600);
            tableLayoutPanel1.TabIndex = 6;
            // 
            // minimizeButton
            // 
            minimizeButton.BackColor = System.Drawing.Color.Transparent;
            minimizeButton.Dock = DockStyle.Fill;
            minimizeButton.FlatAppearance.BorderSize = 0;
            minimizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            minimizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            minimizeButton.FlatStyle = FlatStyle.Flat;
            minimizeButton.IconChar = FontAwesome.Sharp.IconChar.WindowMinimize;
            minimizeButton.IconColor = System.Drawing.Color.WhiteSmoke;
            minimizeButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            minimizeButton.IconSize = 24;
            minimizeButton.Location = new System.Drawing.Point(854, 3);
            minimizeButton.Name = "minimizeButton";
            minimizeButton.Size = new System.Drawing.Size(26, 25);
            minimizeButton.TabIndex = 8;
            minimizeButton.UseVisualStyleBackColor = false;
            minimizeButton.Click += MinimizeButton_Click;
            minimizeButton.MouseEnter += AppButtons_MouseEnter;
            minimizeButton.MouseLeave += AppButtons_MouseLeave;
            // 
            // maximizeButton
            // 
            maximizeButton.BackColor = System.Drawing.Color.Transparent;
            maximizeButton.Dock = DockStyle.Fill;
            maximizeButton.FlatAppearance.BorderSize = 0;
            maximizeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            maximizeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            maximizeButton.FlatStyle = FlatStyle.Flat;
            maximizeButton.IconChar = FontAwesome.Sharp.IconChar.WindowMaximize;
            maximizeButton.IconColor = System.Drawing.Color.WhiteSmoke;
            maximizeButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            maximizeButton.IconSize = 24;
            maximizeButton.Location = new System.Drawing.Point(886, 3);
            maximizeButton.Name = "maximizeButton";
            maximizeButton.Size = new System.Drawing.Size(26, 25);
            maximizeButton.TabIndex = 7;
            maximizeButton.UseVisualStyleBackColor = false;
            maximizeButton.Click += MaximizeButton_Click;
            maximizeButton.MouseEnter += AppButtons_MouseEnter;
            maximizeButton.MouseLeave += AppButtons_MouseLeave;
            // 
            // closeButton
            // 
            closeButton.BackColor = System.Drawing.Color.Transparent;
            tableLayoutPanel1.SetColumnSpan(closeButton, 2);
            closeButton.Dock = DockStyle.Fill;
            closeButton.FlatAppearance.BorderSize = 0;
            closeButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            closeButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            closeButton.FlatStyle = FlatStyle.Flat;
            closeButton.ForeColor = System.Drawing.Color.WhiteSmoke;
            closeButton.IconChar = FontAwesome.Sharp.IconChar.Xmark;
            closeButton.IconColor = System.Drawing.Color.WhiteSmoke;
            closeButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
            closeButton.IconSize = 24;
            closeButton.Location = new System.Drawing.Point(918, 3);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(29, 25);
            closeButton.TabIndex = 6;
            closeButton.UseVisualStyleBackColor = false;
            closeButton.Click += CloseButton_Click;
            closeButton.MouseEnter += AppButtons_MouseEnter;
            closeButton.MouseLeave += AppButtons_MouseLeave;
            // 
            // borderPanel1
            // 
            borderPanel1.Border.Color = System.Drawing.Color.FromArgb(32, 34, 37);
            borderPanel1.Border.Style = ButtonBorderStyle.Solid;
            borderPanel1.Border.Thickness = 2;
            borderPanel1.Border.Which = Forms.Controls.BorderPanel.PanelBorder.BorderToDraw.Bottom;
            borderPanel1.Controls.Add(dataVersionLabel);
            borderPanel1.Dock = DockStyle.Fill;
            borderPanel1.Location = new System.Drawing.Point(187, 3);
            borderPanel1.Name = "borderPanel1";
            borderPanel1.Size = new System.Drawing.Size(661, 25);
            borderPanel1.TabIndex = 11;
            borderPanel1.MouseDown += AppMove;
            // 
            // dataVersionLabel
            // 
            dataVersionLabel.BackColor = System.Drawing.Color.Transparent;
            dataVersionLabel.Dock = DockStyle.Fill;
            dataVersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            dataVersionLabel.Location = new System.Drawing.Point(0, 0);
            dataVersionLabel.Name = "dataVersionLabel";
            dataVersionLabel.Size = new System.Drawing.Size(661, 25);
            dataVersionLabel.TabIndex = 9;
            dataVersionLabel.Text = "Database Version";
            dataVersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            dataVersionLabel.MouseDown += AppMove;
            // 
            // formPages1
            // 
            tableLayoutPanel1.SetColumnSpan(formPages1, 4);
            formPages1.Controls.Add(page1);
            formPages1.Dock = DockStyle.Fill;
            formPages1.Location = new System.Drawing.Point(187, 34);
            formPages1.Name = "formPages1";
            formPages1.Pages.Add(page1);
            formPages1.SelectedIndex = 0;
            formPages1.Size = new System.Drawing.Size(757, 563);
            formPages1.TabIndex = 12;
            // 
            // page1
            // 
            page1.AccessibleRole = AccessibleRole.None;
            page1.Anchor = AnchorStyles.None;
            page1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            page1.BackColor = System.Drawing.Color.FromArgb(44, 47, 51);
            page1.Controls.Add(powerPanel1);
            page1.Dock = DockStyle.Fill;
            page1.ForeColor = System.Drawing.Color.WhiteSmoke;
            page1.Location = new System.Drawing.Point(0, 0);
            page1.Name = "page1";
            page1.Size = new System.Drawing.Size(757, 563);
            page1.TabIndex = 0;
            page1.Title = "My First Page";
            // 
            // powerPanel1
            // 
            powerPanel1.Columns = 3;
            powerPanel1.Dock = DockStyle.Fill;
            powerPanel1.Location = new System.Drawing.Point(0, 0);
            powerPanel1.Name = "powerPanel1";
            powerPanel1.Size = new System.Drawing.Size(757, 563);
            powerPanel1.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = System.Drawing.Color.FromArgb(54, 57, 63);
            BackgroundImageLayout = ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(950, 600);
            Controls.Add(tableLayoutPanel1);
            DoubleBuffered = true;
            Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            ForeColor = System.Drawing.Color.WhiteSmoke;
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            MinimumSize = new System.Drawing.Size(950, 600);
            Name = "MainForm";
            Text = "Mids` Reborn : Hero Designer";
            panelSideMenu.ResumeLayout(false);
            panelBuild.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            borderPanel1.ResumeLayout(false);
            formPages1.ResumeLayout(false);
            page1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Forms.Controls.BorderPanel panelSideMenu;
        private Forms.Controls.BorderPanel panelLogo;
        private TableLayoutPanel tableLayoutPanel1;
        private FontAwesome.Sharp.IconButton closeButton;
        private FontAwesome.Sharp.IconButton maximizeButton;
        private FontAwesome.Sharp.IconButton minimizeButton;
        private Label dataVersionLabel;
        private Button buttonBuild;
        private Panel panelBuild;
        private Button buttonSaveAs;
        private Button buttonSave;
        private Button buttonNew;
        private Button buttonLoad;
        private Button buttonConfig;
        private Forms.Controls.BorderPanel borderPanel1;
        private Mids_Reborn.Controls.FormPages formPages1;
        private Mids_Reborn.Controls.Page page1;
        private PowerPanel powerPanel1;
    }
}