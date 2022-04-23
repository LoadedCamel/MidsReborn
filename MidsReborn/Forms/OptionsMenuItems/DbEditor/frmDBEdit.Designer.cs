using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.OptionsMenuItems.DbEditor
{
    public partial class frmDBEdit
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
            this.udIssue = new System.Windows.Forms.NumericUpDown();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.lblDate = new System.Windows.Forms.Label();
            this.btnEditEnh = new System.Windows.Forms.Button();
            this.btnEditIOSetPvE = new System.Windows.Forms.Button();
            this.lblCountSalvage = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.lblCountRecipe = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.lblCountFX = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.lblCountPwr = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.lblCountPS = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.lblCountIOSet = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.lblCountEnh = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.lblCountAT = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSalvage = new System.Windows.Forms.Button();
            this.btnRecipe = new System.Windows.Forms.Button();
            this.btnEditEntity = new System.Windows.Forms.Button();
            this.btnPSBrowse = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.exportIndexes = new System.Windows.Forms.Button();
            this.btnExportJSON = new System.Windows.Forms.Button();
            this.btnJsonImporter = new System.Windows.Forms.Button();
            this.btnAttribModEdit = new System.Windows.Forms.Button();
            this.btnGCMIO = new System.Windows.Forms.Button();
            this.udPageVol = new System.Windows.Forms.NumericUpDown();
            this.txtPageVol = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtDBVer = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnDbCreate = new System.Windows.Forms.Button();
            this.btnServerDataEdit = new System.Windows.Forms.Button();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGeneratePatch = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udIssue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPageVol)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // udIssue
            // 
            this.udIssue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udIssue.Location = new System.Drawing.Point(5, 33);
            this.udIssue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udIssue.Name = "udIssue";
            this.udIssue.Size = new System.Drawing.Size(194, 20);
            this.udIssue.TabIndex = 0;
            this.udIssue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udIssue.Value = new decimal(new int[] {
            12,
            0,
            0,
            0});
            this.udIssue.ValueChanged += new System.EventHandler(this.udIssue_ValueChanged);
            this.udIssue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.udIssue_KeyPress);
            // 
            // Label1
            // 
            this.Label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(5, 2);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(194, 26);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Issue Supported";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label2
            // 
            this.Label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(611, 2);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(197, 26);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Database Date:";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblDate.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.Location = new System.Drawing.Point(611, 30);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(197, 26);
            this.lblDate.TabIndex = 4;
            this.lblDate.Text = "DD/MM/YYYY";
            this.lblDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnEditEnh
            // 
            this.btnEditEnh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(255)))), ((int)(((byte)(102)))));
            this.btnEditEnh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEditEnh.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditEnh.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditEnh.ForeColor = System.Drawing.Color.Black;
            this.btnEditEnh.Location = new System.Drawing.Point(3, 71);
            this.btnEditEnh.Name = "btnEditEnh";
            this.btnEditEnh.Size = new System.Drawing.Size(395, 62);
            this.btnEditEnh.TabIndex = 5;
            this.btnEditEnh.Text = "Enhancement Editor";
            this.btnEditEnh.UseVisualStyleBackColor = false;
            this.btnEditEnh.Click += new System.EventHandler(this.btnEditEnh_Click);
            // 
            // btnEditIOSetPvE
            // 
            this.btnEditIOSetPvE.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
            this.btnEditIOSetPvE.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEditIOSetPvE.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditIOSetPvE.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditIOSetPvE.ForeColor = System.Drawing.Color.Black;
            this.btnEditIOSetPvE.Location = new System.Drawing.Point(404, 71);
            this.btnEditIOSetPvE.Name = "btnEditIOSetPvE";
            this.btnEditIOSetPvE.Size = new System.Drawing.Size(396, 62);
            this.btnEditIOSetPvE.TabIndex = 6;
            this.btnEditIOSetPvE.Text = "Invention Set Editor";
            this.btnEditIOSetPvE.UseVisualStyleBackColor = false;
            this.btnEditIOSetPvE.Click += new System.EventHandler(this.btnEditIOSet_Click);
            // 
            // lblCountSalvage
            // 
            this.lblCountSalvage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountSalvage.Location = new System.Drawing.Point(712, 51);
            this.lblCountSalvage.Name = "lblCountSalvage";
            this.lblCountSalvage.Size = new System.Drawing.Size(96, 47);
            this.lblCountSalvage.TabIndex = 20;
            this.lblCountSalvage.Text = "Count";
            this.lblCountSalvage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label6
            // 
            this.Label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label6.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(712, 2);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(96, 47);
            this.Label6.TabIndex = 19;
            this.Label6.Text = "Salvage";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountRecipe
            // 
            this.lblCountRecipe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountRecipe.Location = new System.Drawing.Point(611, 51);
            this.lblCountRecipe.Name = "lblCountRecipe";
            this.lblCountRecipe.Size = new System.Drawing.Size(93, 47);
            this.lblCountRecipe.TabIndex = 18;
            this.lblCountRecipe.Text = "Count";
            this.lblCountRecipe.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label4
            // 
            this.Label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(611, 2);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(93, 47);
            this.Label4.TabIndex = 17;
            this.Label4.Text = "Recipes";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountFX
            // 
            this.lblCountFX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountFX.Location = new System.Drawing.Point(510, 51);
            this.lblCountFX.Name = "lblCountFX";
            this.lblCountFX.Size = new System.Drawing.Size(93, 47);
            this.lblCountFX.TabIndex = 16;
            this.lblCountFX.Text = "Count";
            this.lblCountFX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label15
            // 
            this.Label15.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label15.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label15.Location = new System.Drawing.Point(510, 2);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(93, 47);
            this.Label15.TabIndex = 15;
            this.Label15.Text = "Power Effects";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountPwr
            // 
            this.lblCountPwr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountPwr.Location = new System.Drawing.Point(409, 51);
            this.lblCountPwr.Name = "lblCountPwr";
            this.lblCountPwr.Size = new System.Drawing.Size(93, 47);
            this.lblCountPwr.TabIndex = 14;
            this.lblCountPwr.Text = "Count";
            this.lblCountPwr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label13
            // 
            this.Label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label13.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label13.Location = new System.Drawing.Point(409, 2);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(93, 47);
            this.Label13.TabIndex = 13;
            this.Label13.Text = "Powers";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountPS
            // 
            this.lblCountPS.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountPS.Location = new System.Drawing.Point(308, 51);
            this.lblCountPS.Name = "lblCountPS";
            this.lblCountPS.Size = new System.Drawing.Size(93, 47);
            this.lblCountPS.TabIndex = 12;
            this.lblCountPS.Text = "Count";
            this.lblCountPS.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label11
            // 
            this.Label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(308, 2);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(93, 47);
            this.Label11.TabIndex = 11;
            this.Label11.Text = "Powersets";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountIOSet
            // 
            this.lblCountIOSet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountIOSet.Location = new System.Drawing.Point(207, 51);
            this.lblCountIOSet.Name = "lblCountIOSet";
            this.lblCountIOSet.Size = new System.Drawing.Size(93, 47);
            this.lblCountIOSet.TabIndex = 10;
            this.lblCountIOSet.Text = "Count";
            this.lblCountIOSet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label9
            // 
            this.Label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(207, 2);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(93, 47);
            this.Label9.TabIndex = 9;
            this.Label9.Text = "Invention Sets";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountEnh
            // 
            this.lblCountEnh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountEnh.Location = new System.Drawing.Point(106, 51);
            this.lblCountEnh.Name = "lblCountEnh";
            this.lblCountEnh.Size = new System.Drawing.Size(93, 47);
            this.lblCountEnh.TabIndex = 8;
            this.lblCountEnh.Text = "Count";
            this.lblCountEnh.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label7
            // 
            this.Label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label7.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(106, 2);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(93, 47);
            this.Label7.TabIndex = 7;
            this.Label7.Text = "Enhancements";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCountAT
            // 
            this.lblCountAT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCountAT.Location = new System.Drawing.Point(5, 51);
            this.lblCountAT.Name = "lblCountAT";
            this.lblCountAT.Size = new System.Drawing.Size(93, 47);
            this.lblCountAT.TabIndex = 6;
            this.lblCountAT.Text = "Count";
            this.lblCountAT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.Color.Transparent;
            this.Label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(5, 2);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(93, 47);
            this.Label5.TabIndex = 5;
            this.Label5.Text = "Classes";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Firebrick;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnClose.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.Black;
            this.btnClose.Location = new System.Drawing.Point(583, 587);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(234, 31);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSalvage
            // 
            this.btnSalvage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(168)))), ((int)(((byte)(102)))));
            this.btnSalvage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSalvage.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSalvage.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalvage.ForeColor = System.Drawing.Color.Black;
            this.btnSalvage.Location = new System.Drawing.Point(404, 139);
            this.btnSalvage.Name = "btnSalvage";
            this.btnSalvage.Size = new System.Drawing.Size(396, 62);
            this.btnSalvage.TabIndex = 14;
            this.btnSalvage.Text = "Salvage Editor";
            this.btnSalvage.UseVisualStyleBackColor = false;
            this.btnSalvage.Click += new System.EventHandler(this.btnSalvage_Click);
            // 
            // btnRecipe
            // 
            this.btnRecipe.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(171)))), ((int)(((byte)(102)))), ((int)(((byte)(255)))));
            this.btnRecipe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRecipe.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnRecipe.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecipe.ForeColor = System.Drawing.Color.Black;
            this.btnRecipe.Location = new System.Drawing.Point(3, 139);
            this.btnRecipe.Name = "btnRecipe";
            this.btnRecipe.Size = new System.Drawing.Size(395, 62);
            this.btnRecipe.TabIndex = 15;
            this.btnRecipe.Text = "Recipe Editor";
            this.btnRecipe.UseVisualStyleBackColor = false;
            this.btnRecipe.Click += new System.EventHandler(this.btnRecipe_Click);
            // 
            // btnEditEntity
            // 
            this.btnEditEntity.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(168)))), ((int)(((byte)(186)))), ((int)(((byte)(194)))));
            this.btnEditEntity.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnEditEntity.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditEntity.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditEntity.ForeColor = System.Drawing.Color.Black;
            this.btnEditEntity.Location = new System.Drawing.Point(404, 3);
            this.btnEditEntity.Name = "btnEditEntity";
            this.btnEditEntity.Size = new System.Drawing.Size(396, 62);
            this.btnEditEntity.TabIndex = 17;
            this.btnEditEntity.Text = "Entity Editor";
            this.btnEditEntity.UseVisualStyleBackColor = false;
            this.btnEditEntity.Click += new System.EventHandler(this.btnEditEntity_Click);
            // 
            // btnPSBrowse
            // 
            this.btnPSBrowse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.btnPSBrowse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPSBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPSBrowse.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPSBrowse.ForeColor = System.Drawing.Color.Black;
            this.btnPSBrowse.Location = new System.Drawing.Point(3, 3);
            this.btnPSBrowse.Name = "btnPSBrowse";
            this.btnPSBrowse.Size = new System.Drawing.Size(395, 62);
            this.btnPSBrowse.TabIndex = 18;
            this.btnPSBrowse.Text = "Main Editor";
            this.btnPSBrowse.UseVisualStyleBackColor = false;
            this.btnPSBrowse.Click += new System.EventHandler(this.btnPSBrowse_Click);
            // 
            // Label3
            // 
            this.Label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(409, 2);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(194, 26);
            this.Label3.TabIndex = 22;
            this.Label3.Text = "Database Version";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exportIndexes
            // 
            this.exportIndexes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.exportIndexes.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exportIndexes.ForeColor = System.Drawing.SystemColors.ControlText;
            this.exportIndexes.Location = new System.Drawing.Point(236, 228);
            this.exportIndexes.Name = "exportIndexes";
            this.exportIndexes.Size = new System.Drawing.Size(164, 24);
            this.exportIndexes.TabIndex = 24;
            this.exportIndexes.Text = "Export Power Indexes";
            this.exportIndexes.UseVisualStyleBackColor = true;
            this.exportIndexes.Visible = false;
            // 
            // btnExportJSON
            // 
            this.btnExportJSON.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnExportJSON.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnExportJSON.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExportJSON.ForeColor = System.Drawing.Color.Black;
            this.btnExportJSON.Location = new System.Drawing.Point(404, 343);
            this.btnExportJSON.Name = "btnExportJSON";
            this.btnExportJSON.Size = new System.Drawing.Size(396, 51);
            this.btnExportJSON.TabIndex = 26;
            this.btnExportJSON.Text = "JSON Exporter";
            this.btnExportJSON.UseVisualStyleBackColor = false;
            this.btnExportJSON.Click += new System.EventHandler(this.btnExportJSON_Click);
            // 
            // btnJsonImporter
            // 
            this.btnJsonImporter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnJsonImporter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnJsonImporter.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnJsonImporter.ForeColor = System.Drawing.Color.Black;
            this.btnJsonImporter.Location = new System.Drawing.Point(404, 275);
            this.btnJsonImporter.Name = "btnJsonImporter";
            this.btnJsonImporter.Size = new System.Drawing.Size(396, 62);
            this.btnJsonImporter.TabIndex = 27;
            this.btnJsonImporter.Text = "JSON Importer";
            this.btnJsonImporter.UseVisualStyleBackColor = false;
            this.btnJsonImporter.Click += new System.EventHandler(this.btnJsonImporter_Click);
            // 
            // btnAttribModEdit
            // 
            this.btnAttribModEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(102)))));
            this.btnAttribModEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAttribModEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAttribModEdit.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAttribModEdit.ForeColor = System.Drawing.Color.Black;
            this.btnAttribModEdit.Location = new System.Drawing.Point(404, 207);
            this.btnAttribModEdit.Name = "btnAttribModEdit";
            this.btnAttribModEdit.Size = new System.Drawing.Size(396, 62);
            this.btnAttribModEdit.TabIndex = 28;
            this.btnAttribModEdit.Text = "AttribMod Editor";
            this.btnAttribModEdit.UseVisualStyleBackColor = false;
            this.btnAttribModEdit.Click += new System.EventHandler(this.btnAttribModEdit_Click);
            // 
            // btnGCMIO
            // 
            this.btnGCMIO.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(102)))), ((int)(((byte)(143)))));
            this.btnGCMIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGCMIO.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGCMIO.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGCMIO.ForeColor = System.Drawing.Color.Black;
            this.btnGCMIO.Location = new System.Drawing.Point(3, 207);
            this.btnGCMIO.Name = "btnGCMIO";
            this.btnGCMIO.Size = new System.Drawing.Size(395, 62);
            this.btnGCMIO.TabIndex = 30;
            this.btnGCMIO.Text = "GCM Editor";
            this.btnGCMIO.UseVisualStyleBackColor = false;
            this.btnGCMIO.Click += new System.EventHandler(this.btnGCMIO_Click);
            // 
            // udPageVol
            // 
            this.udPageVol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udPageVol.Location = new System.Drawing.Point(207, 33);
            this.udPageVol.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udPageVol.Name = "udPageVol";
            this.udPageVol.Size = new System.Drawing.Size(194, 20);
            this.udPageVol.TabIndex = 33;
            this.udPageVol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udPageVol.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.udPageVol.ValueChanged += new System.EventHandler(this.udPageVol_ValueChanged);
            this.udPageVol.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.udPageVol_KeyPress);
            // 
            // txtPageVol
            // 
            this.txtPageVol.Cursor = System.Windows.Forms.Cursors.Hand;
            this.txtPageVol.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPageVol.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPageVol.Location = new System.Drawing.Point(207, 2);
            this.txtPageVol.Name = "txtPageVol";
            this.txtPageVol.Size = new System.Drawing.Size(194, 26);
            this.txtPageVol.TabIndex = 35;
            this.txtPageVol.Text = "Page";
            this.txtPageVol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.txtPageVol.Click += new System.EventHandler(this.txtPageVol_Click);
            this.txtPageVol.MouseLeave += new System.EventHandler(this.txtPageVol_MouseLeave);
            this.txtPageVol.MouseHover += new System.EventHandler(this.txtPageVol_MouseHover);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.txtDBVer, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.udIssue, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.udPageVol, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtPageVol, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblDate, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.Label2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.Label3, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 14);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.02041F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.97959F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(813, 58);
            this.tableLayoutPanel1.TabIndex = 37;
            // 
            // txtDBVer
            // 
            this.txtDBVer.AutoSize = true;
            this.txtDBVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDBVer.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDBVer.Location = new System.Drawing.Point(409, 30);
            this.txtDBVer.Name = "txtDBVer";
            this.txtDBVer.Size = new System.Drawing.Size(194, 26);
            this.txtDBVer.TabIndex = 38;
            this.txtDBVer.Text = "2021.1205";
            this.txtDBVer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.tableLayoutPanel2.ColumnCount = 8;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.lblCountSalvage, 7, 1);
            this.tableLayoutPanel2.Controls.Add(this.Label5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.Label6, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblCountAT, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblCountRecipe, 6, 1);
            this.tableLayoutPanel2.Controls.Add(this.Label7, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.Label4, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblCountEnh, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblCountFX, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.Label9, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.Label15, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblCountIOSet, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblCountPwr, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.Label11, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.Label13, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.lblCountPS, 3, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 481);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(813, 100);
            this.tableLayoutPanel2.TabIndex = 38;
            // 
            // btnDbCreate
            // 
            this.btnDbCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(126)))), ((int)(((byte)(51)))));
            this.btnDbCreate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDbCreate.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDbCreate.ForeColor = System.Drawing.Color.Black;
            this.btnDbCreate.Location = new System.Drawing.Point(17, 586);
            this.btnDbCreate.Name = "btnDbCreate";
            this.btnDbCreate.Size = new System.Drawing.Size(234, 32);
            this.btnDbCreate.TabIndex = 39;
            this.btnDbCreate.Text = "Create New Database";
            this.btnDbCreate.UseVisualStyleBackColor = false;
            this.btnDbCreate.Click += new System.EventHandler(this.btnDbCreate_Click);
            // 
            // btnServerDataEdit
            // 
            this.btnServerDataEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(92)))), ((int)(((byte)(255)))));
            this.btnServerDataEdit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnServerDataEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnServerDataEdit.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnServerDataEdit.ForeColor = System.Drawing.Color.Black;
            this.btnServerDataEdit.Location = new System.Drawing.Point(3, 275);
            this.btnServerDataEdit.Name = "btnServerDataEdit";
            this.btnServerDataEdit.Size = new System.Drawing.Size(395, 62);
            this.btnServerDataEdit.TabIndex = 41;
            this.btnServerDataEdit.Text = "Server Data Editor";
            this.btnServerDataEdit.UseVisualStyleBackColor = false;
            this.btnServerDataEdit.Click += new System.EventHandler(this.btnServerDataEdit_Click);
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.btnGeneratePatch, 0, 5);
            this.tableLayoutPanel3.Controls.Add(this.btnServerDataEdit, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.btnPSBrowse, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnEditEntity, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnEditEnh, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnExportJSON, 1, 5);
            this.tableLayoutPanel3.Controls.Add(this.btnAttribModEdit, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnEditIOSetPvE, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.btnGCMIO, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.btnRecipe, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnSalvage, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.btnJsonImporter, 1, 4);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(17, 78);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.14384F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.14384F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.14384F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.14384F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.14384F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28082F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(803, 397);
            this.tableLayoutPanel3.TabIndex = 42;
            // 
            // btnGeneratePatch
            // 
            this.btnGeneratePatch.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.btnGeneratePatch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnGeneratePatch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGeneratePatch.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGeneratePatch.ForeColor = System.Drawing.Color.Black;
            this.btnGeneratePatch.Location = new System.Drawing.Point(3, 343);
            this.btnGeneratePatch.Name = "btnGeneratePatch";
            this.btnGeneratePatch.Size = new System.Drawing.Size(395, 51);
            this.btnGeneratePatch.TabIndex = 43;
            this.btnGeneratePatch.Text = "Generate Patch File";
            this.btnGeneratePatch.UseVisualStyleBackColor = false;
            this.btnGeneratePatch.Click += new System.EventHandler(this.btnGeneratePatch_Click);
            // 
            // frmDBEdit
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(837, 630);
            this.Controls.Add(this.btnDbCreate);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDBEdit";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Database Menu";
            ((System.ComponentModel.ISupportInitialize)(this.udIssue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udPageVol)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        System.Windows.Forms.NumericUpDown udIssue;
        #endregion

        private Button btnExportJSON;
        private Button btnJsonImporter;
        private Button btnGCMIO;
        private Button btnAttribModEdit;
        private NumericUpDown udPageVol;
        private Label txtPageVol;
        private TableLayoutPanel tableLayoutPanel1;
        private Label txtDBVer;
        private TableLayoutPanel tableLayoutPanel2;
        private Button btnDbCreate;
        private Button btnServerDataEdit;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnGeneratePatch;
    }
}