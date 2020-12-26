using System.ComponentModel;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class frmCSV
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
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.at_Revision = new System.Windows.Forms.Label();
            this.Label15 = new System.Windows.Forms.Label();
            this.at_Import = new System.Windows.Forms.Button();
            this.at_Count = new System.Windows.Forms.Label();
            this.at_Date = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.set_Revision = new System.Windows.Forms.Label();
            this.Label17 = new System.Windows.Forms.Label();
            this.set_Import = new System.Windows.Forms.Button();
            this.set_Count = new System.Windows.Forms.Label();
            this.set_Date = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.pow_Revision = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.pow_Import = new System.Windows.Forms.Button();
            this.pow_Count = new System.Windows.Forms.Label();
            this.pow_Date = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.fx_Revision = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.fx_Import = new System.Windows.Forms.Button();
            this.fx_Count = new System.Windows.Forms.Label();
            this.fx_Date = new System.Windows.Forms.Label();
            this.Label11 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.GroupBox6 = new System.Windows.Forms.GroupBox();
            this.inventSetImport = new System.Windows.Forms.Button();
            this.btnIOLevels = new System.Windows.Forms.Button();
            this.btnImportRecipes = new System.Windows.Forms.Button();
            this.btnSalvageUpdate = new System.Windows.Forms.Button();
            this.invent_Revision = new System.Windows.Forms.Label();
            this.btnEnhEffects = new System.Windows.Forms.Button();
            this.Label23 = new System.Windows.Forms.Label();
            this.btnBonusLookup = new System.Windows.Forms.Button();
            this.invent_Import = new System.Windows.Forms.Button();
            this.invent_RecipeDate = new System.Windows.Forms.Label();
            this.invent_Date = new System.Windows.Forms.Label();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label14 = new System.Windows.Forms.Label();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.lev_Revision = new System.Windows.Forms.Label();
            this.Label16 = new System.Windows.Forms.Label();
            this.level_import = new System.Windows.Forms.Button();
            this.lev_Count = new System.Windows.Forms.Label();
            this.lev_date = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label24 = new System.Windows.Forms.Label();
            this.GroupBox8 = new System.Windows.Forms.GroupBox();
            this.btnEntities = new System.Windows.Forms.Button();
            this.btnClearSI = new System.Windows.Forms.Button();
            this.btnStaticIndex = new System.Windows.Forms.Button();
            this.btnDefiance = new System.Windows.Forms.Button();
            this.btnStaticExport = new System.Windows.Forms.Button();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.GroupBox6.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.GroupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.at_Revision);
            this.GroupBox2.Controls.Add(this.Label15);
            this.GroupBox2.Controls.Add(this.at_Import);
            this.GroupBox2.Controls.Add(this.at_Count);
            this.GroupBox2.Controls.Add(this.at_Date);
            this.GroupBox2.Controls.Add(this.Label5);
            this.GroupBox2.Controls.Add(this.Label6);
            this.GroupBox2.Location = new System.Drawing.Point(12, 11);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(257, 115);
            this.GroupBox2.TabIndex = 1;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Archetype Classes";
            // 
            // at_Revision
            // 
            this.at_Revision.Location = new System.Drawing.Point(124, 60);
            this.at_Revision.Name = "at_Revision";
            this.at_Revision.Size = new System.Drawing.Size(112, 22);
            this.at_Revision.TabIndex = 8;
            this.at_Revision.Text = "000";
            this.at_Revision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label15
            // 
            this.Label15.Location = new System.Drawing.Point(6, 60);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(112, 22);
            this.Label15.TabIndex = 7;
            this.Label15.Text = "Revision:";
            this.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // at_Import
            // 
            this.at_Import.Location = new System.Drawing.Point(6, 85);
            this.at_Import.Name = "at_Import";
            this.at_Import.Size = new System.Drawing.Size(245, 23);
            this.at_Import.TabIndex = 4;
            this.at_Import.Text = "Import";
            this.at_Import.UseVisualStyleBackColor = true;
            this.at_Import.Click += new System.EventHandler(this.at_Import_Click);
            // 
            // at_Count
            // 
            this.at_Count.Location = new System.Drawing.Point(124, 38);
            this.at_Count.Name = "at_Count";
            this.at_Count.Size = new System.Drawing.Size(112, 22);
            this.at_Count.TabIndex = 3;
            this.at_Count.Text = "000";
            this.at_Count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // at_Date
            // 
            this.at_Date.Location = new System.Drawing.Point(124, 16);
            this.at_Date.Name = "at_Date";
            this.at_Date.Size = new System.Drawing.Size(112, 22);
            this.at_Date.TabIndex = 2;
            this.at_Date.Text = "DD/MMM/YYYY";
            this.at_Date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(6, 38);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(112, 22);
            this.Label5.TabIndex = 1;
            this.Label5.Text = "Record Count:";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(6, 16);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(112, 22);
            this.Label6.TabIndex = 0;
            this.Label6.Text = "Last Updated:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.set_Revision);
            this.GroupBox3.Controls.Add(this.Label17);
            this.GroupBox3.Controls.Add(this.set_Import);
            this.GroupBox3.Controls.Add(this.set_Count);
            this.GroupBox3.Controls.Add(this.set_Date);
            this.GroupBox3.Controls.Add(this.Label7);
            this.GroupBox3.Controls.Add(this.Label8);
            this.GroupBox3.Location = new System.Drawing.Point(12, 132);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(257, 115);
            this.GroupBox3.TabIndex = 2;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Power Sets";
            // 
            // set_Revision
            // 
            this.set_Revision.Location = new System.Drawing.Point(124, 60);
            this.set_Revision.Name = "set_Revision";
            this.set_Revision.Size = new System.Drawing.Size(112, 22);
            this.set_Revision.TabIndex = 8;
            this.set_Revision.Text = "000";
            this.set_Revision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label17
            // 
            this.Label17.Location = new System.Drawing.Point(6, 60);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(112, 22);
            this.Label17.TabIndex = 7;
            this.Label17.Text = "Revision:";
            this.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // set_Import
            // 
            this.set_Import.Location = new System.Drawing.Point(6, 85);
            this.set_Import.Name = "set_Import";
            this.set_Import.Size = new System.Drawing.Size(245, 23);
            this.set_Import.TabIndex = 4;
            this.set_Import.Text = "Import";
            this.set_Import.UseVisualStyleBackColor = true;
            this.set_Import.Click += new System.EventHandler(this.set_Import_Click);
            // 
            // set_Count
            // 
            this.set_Count.Location = new System.Drawing.Point(124, 38);
            this.set_Count.Name = "set_Count";
            this.set_Count.Size = new System.Drawing.Size(112, 22);
            this.set_Count.TabIndex = 3;
            this.set_Count.Text = "000";
            this.set_Count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // set_Date
            // 
            this.set_Date.Location = new System.Drawing.Point(124, 16);
            this.set_Date.Name = "set_Date";
            this.set_Date.Size = new System.Drawing.Size(112, 22);
            this.set_Date.TabIndex = 2;
            this.set_Date.Text = "DD/MMM/YYYY";
            this.set_Date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(6, 38);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(112, 22);
            this.Label7.TabIndex = 1;
            this.Label7.Text = "Record Count:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(6, 16);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(112, 22);
            this.Label8.TabIndex = 0;
            this.Label8.Text = "Last Updated:";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.pow_Revision);
            this.GroupBox4.Controls.Add(this.Label19);
            this.GroupBox4.Controls.Add(this.pow_Import);
            this.GroupBox4.Controls.Add(this.pow_Count);
            this.GroupBox4.Controls.Add(this.pow_Date);
            this.GroupBox4.Controls.Add(this.Label9);
            this.GroupBox4.Controls.Add(this.Label10);
            this.GroupBox4.Location = new System.Drawing.Point(275, 12);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(257, 115);
            this.GroupBox4.TabIndex = 3;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Powers";
            // 
            // pow_Revision
            // 
            this.pow_Revision.Location = new System.Drawing.Point(124, 60);
            this.pow_Revision.Name = "pow_Revision";
            this.pow_Revision.Size = new System.Drawing.Size(112, 22);
            this.pow_Revision.TabIndex = 8;
            this.pow_Revision.Text = "000";
            this.pow_Revision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label19
            // 
            this.Label19.Location = new System.Drawing.Point(6, 60);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(112, 22);
            this.Label19.TabIndex = 7;
            this.Label19.Text = "Revision:";
            this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pow_Import
            // 
            this.pow_Import.Location = new System.Drawing.Point(6, 85);
            this.pow_Import.Name = "pow_Import";
            this.pow_Import.Size = new System.Drawing.Size(245, 23);
            this.pow_Import.TabIndex = 4;
            this.pow_Import.Text = "Import";
            this.pow_Import.UseVisualStyleBackColor = true;
            this.pow_Import.Click += new System.EventHandler(this.pow_Import_Click);
            // 
            // pow_Count
            // 
            this.pow_Count.Location = new System.Drawing.Point(124, 38);
            this.pow_Count.Name = "pow_Count";
            this.pow_Count.Size = new System.Drawing.Size(112, 22);
            this.pow_Count.TabIndex = 3;
            this.pow_Count.Text = "000";
            this.pow_Count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pow_Date
            // 
            this.pow_Date.Location = new System.Drawing.Point(124, 16);
            this.pow_Date.Name = "pow_Date";
            this.pow_Date.Size = new System.Drawing.Size(112, 22);
            this.pow_Date.TabIndex = 2;
            this.pow_Date.Text = "DD/MMM/YYYY";
            this.pow_Date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label9
            // 
            this.Label9.Location = new System.Drawing.Point(6, 38);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(112, 22);
            this.Label9.TabIndex = 1;
            this.Label9.Text = "Record Count:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label10
            // 
            this.Label10.Location = new System.Drawing.Point(6, 16);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(112, 22);
            this.Label10.TabIndex = 0;
            this.Label10.Text = "Last Updated:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox5
            // 
            this.GroupBox5.Controls.Add(this.fx_Revision);
            this.GroupBox5.Controls.Add(this.Label21);
            this.GroupBox5.Controls.Add(this.fx_Import);
            this.GroupBox5.Controls.Add(this.fx_Count);
            this.GroupBox5.Controls.Add(this.fx_Date);
            this.GroupBox5.Controls.Add(this.Label11);
            this.GroupBox5.Controls.Add(this.Label12);
            this.GroupBox5.Location = new System.Drawing.Point(275, 254);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(257, 115);
            this.GroupBox5.TabIndex = 4;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Power Effects";
            // 
            // fx_Revision
            // 
            this.fx_Revision.Location = new System.Drawing.Point(124, 60);
            this.fx_Revision.Name = "fx_Revision";
            this.fx_Revision.Size = new System.Drawing.Size(112, 22);
            this.fx_Revision.TabIndex = 8;
            this.fx_Revision.Text = "000";
            this.fx_Revision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label21
            // 
            this.Label21.Location = new System.Drawing.Point(6, 60);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(112, 22);
            this.Label21.TabIndex = 7;
            this.Label21.Text = "Revision:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fx_Import
            // 
            this.fx_Import.Location = new System.Drawing.Point(6, 85);
            this.fx_Import.Name = "fx_Import";
            this.fx_Import.Size = new System.Drawing.Size(245, 23);
            this.fx_Import.TabIndex = 4;
            this.fx_Import.Text = "Import";
            this.fx_Import.UseVisualStyleBackColor = true;
            this.fx_Import.Click += new System.EventHandler(this.fx_Import_Click);
            // 
            // fx_Count
            // 
            this.fx_Count.Location = new System.Drawing.Point(124, 38);
            this.fx_Count.Name = "fx_Count";
            this.fx_Count.Size = new System.Drawing.Size(112, 22);
            this.fx_Count.TabIndex = 3;
            this.fx_Count.Text = "000";
            this.fx_Count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // fx_Date
            // 
            this.fx_Date.Location = new System.Drawing.Point(124, 16);
            this.fx_Date.Name = "fx_Date";
            this.fx_Date.Size = new System.Drawing.Size(112, 22);
            this.fx_Date.TabIndex = 2;
            this.fx_Date.Text = "DD/MMM/YYYY";
            this.fx_Date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label11
            // 
            this.Label11.Location = new System.Drawing.Point(6, 38);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(112, 22);
            this.Label11.TabIndex = 1;
            this.Label11.Text = "Record Count:";
            this.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label12
            // 
            this.Label12.Location = new System.Drawing.Point(6, 16);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(112, 22);
            this.Label12.TabIndex = 0;
            this.Label12.Text = "Last Updated:";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox6
            // 
            this.GroupBox6.Controls.Add(this.inventSetImport);
            this.GroupBox6.Controls.Add(this.btnIOLevels);
            this.GroupBox6.Controls.Add(this.btnImportRecipes);
            this.GroupBox6.Controls.Add(this.btnSalvageUpdate);
            this.GroupBox6.Controls.Add(this.invent_Revision);
            this.GroupBox6.Controls.Add(this.btnEnhEffects);
            this.GroupBox6.Controls.Add(this.Label23);
            this.GroupBox6.Controls.Add(this.btnBonusLookup);
            this.GroupBox6.Controls.Add(this.invent_Import);
            this.GroupBox6.Controls.Add(this.invent_RecipeDate);
            this.GroupBox6.Controls.Add(this.invent_Date);
            this.GroupBox6.Controls.Add(this.Label13);
            this.GroupBox6.Controls.Add(this.Label14);
            this.GroupBox6.Location = new System.Drawing.Point(538, 78);
            this.GroupBox6.Name = "GroupBox6";
            this.GroupBox6.Size = new System.Drawing.Size(257, 291);
            this.GroupBox6.TabIndex = 5;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Invention Sets";
            // 
            // inventSetImport
            // 
            this.inventSetImport.Location = new System.Drawing.Point(6, 93);
            this.inventSetImport.Name = "inventSetImport";
            this.inventSetImport.Size = new System.Drawing.Size(245, 23);
            this.inventSetImport.TabIndex = 12;
            this.inventSetImport.Text = "Import Sets";
            this.inventSetImport.UseVisualStyleBackColor = true;
            this.inventSetImport.Click += new System.EventHandler(this.inventSetImport_Click);
            // 
            // btnIOLevels
            // 
            this.btnIOLevels.Location = new System.Drawing.Point(6, 206);
            this.btnIOLevels.Name = "btnIOLevels";
            this.btnIOLevels.Size = new System.Drawing.Size(245, 23);
            this.btnIOLevels.TabIndex = 11;
            this.btnIOLevels.Text = "Compute IO Set Level Ranges";
            this.btnIOLevels.UseVisualStyleBackColor = true;
            this.btnIOLevels.Click += new System.EventHandler(this.btnIOLevels_Click);
            // 
            // btnImportRecipes
            // 
            this.btnImportRecipes.Location = new System.Drawing.Point(6, 148);
            this.btnImportRecipes.Name = "btnImportRecipes";
            this.btnImportRecipes.Size = new System.Drawing.Size(245, 23);
            this.btnImportRecipes.TabIndex = 10;
            this.btnImportRecipes.Text = "Import Recipes (Full Import)";
            this.btnImportRecipes.UseVisualStyleBackColor = true;
            this.btnImportRecipes.Click += new System.EventHandler(this.btnImportRecipes_Click);
            // 
            // btnSalvageUpdate
            // 
            this.btnSalvageUpdate.Location = new System.Drawing.Point(9, 263);
            this.btnSalvageUpdate.Name = "btnSalvageUpdate";
            this.btnSalvageUpdate.Size = new System.Drawing.Size(245, 23);
            this.btnSalvageUpdate.TabIndex = 9;
            this.btnSalvageUpdate.Text = "Update Salvage Requirements";
            this.btnSalvageUpdate.UseVisualStyleBackColor = true;
            this.btnSalvageUpdate.Click += new System.EventHandler(this.btnSalvageUpdate_Click);
            // 
            // invent_Revision
            // 
            this.invent_Revision.Location = new System.Drawing.Point(124, 60);
            this.invent_Revision.Name = "invent_Revision";
            this.invent_Revision.Size = new System.Drawing.Size(112, 22);
            this.invent_Revision.TabIndex = 8;
            this.invent_Revision.Text = "000";
            this.invent_Revision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnEnhEffects
            // 
            this.btnEnhEffects.Location = new System.Drawing.Point(9, 234);
            this.btnEnhEffects.Name = "btnEnhEffects";
            this.btnEnhEffects.Size = new System.Drawing.Size(245, 23);
            this.btnEnhEffects.TabIndex = 8;
            this.btnEnhEffects.Text = "Load Enhancement Effects";
            this.btnEnhEffects.UseVisualStyleBackColor = true;
            this.btnEnhEffects.Click += new System.EventHandler(this.btnEnhEffects_Click);
            // 
            // Label23
            // 
            this.Label23.Location = new System.Drawing.Point(6, 60);
            this.Label23.Name = "Label23";
            this.Label23.Size = new System.Drawing.Size(112, 22);
            this.Label23.TabIndex = 7;
            this.Label23.Text = "Revision:";
            this.Label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnBonusLookup
            // 
            this.btnBonusLookup.Location = new System.Drawing.Point(6, 177);
            this.btnBonusLookup.Name = "btnBonusLookup";
            this.btnBonusLookup.Size = new System.Drawing.Size(245, 23);
            this.btnBonusLookup.TabIndex = 7;
            this.btnBonusLookup.Text = "Import Set Bonus Assignments";
            this.btnBonusLookup.UseVisualStyleBackColor = true;
            this.btnBonusLookup.Click += new System.EventHandler(this.btnBonusLookup_Click);
            // 
            // invent_Import
            // 
            this.invent_Import.Location = new System.Drawing.Point(6, 119);
            this.invent_Import.Name = "invent_Import";
            this.invent_Import.Size = new System.Drawing.Size(245, 23);
            this.invent_Import.TabIndex = 4;
            this.invent_Import.Text = "Import Power-Set Type Assignments";
            this.invent_Import.UseVisualStyleBackColor = true;
            this.invent_Import.Click += new System.EventHandler(this.invent_Import_Click);
            // 
            // invent_RecipeDate
            // 
            this.invent_RecipeDate.Location = new System.Drawing.Point(124, 38);
            this.invent_RecipeDate.Name = "invent_RecipeDate";
            this.invent_RecipeDate.Size = new System.Drawing.Size(112, 22);
            this.invent_RecipeDate.TabIndex = 3;
            this.invent_RecipeDate.Text = "000";
            this.invent_RecipeDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // invent_Date
            // 
            this.invent_Date.Location = new System.Drawing.Point(124, 16);
            this.invent_Date.Name = "invent_Date";
            this.invent_Date.Size = new System.Drawing.Size(112, 22);
            this.invent_Date.TabIndex = 2;
            this.invent_Date.Text = "DD/MMM/YYYY";
            this.invent_Date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label13
            // 
            this.Label13.Location = new System.Drawing.Point(6, 38);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(112, 22);
            this.Label13.TabIndex = 1;
            this.Label13.Text = "Recipe Date:";
            this.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label14
            // 
            this.Label14.Location = new System.Drawing.Point(6, 16);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(112, 22);
            this.Label14.TabIndex = 0;
            this.Label14.Text = "Set-Power Date:";
            this.Label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox7
            // 
            this.GroupBox7.Controls.Add(this.lev_Revision);
            this.GroupBox7.Controls.Add(this.Label16);
            this.GroupBox7.Controls.Add(this.level_import);
            this.GroupBox7.Controls.Add(this.lev_Count);
            this.GroupBox7.Controls.Add(this.lev_date);
            this.GroupBox7.Controls.Add(this.Label22);
            this.GroupBox7.Controls.Add(this.Label24);
            this.GroupBox7.Location = new System.Drawing.Point(275, 133);
            this.GroupBox7.Name = "GroupBox7";
            this.GroupBox7.Size = new System.Drawing.Size(257, 115);
            this.GroupBox7.TabIndex = 6;
            this.GroupBox7.TabStop = false;
            this.GroupBox7.Text = "Power Levels";
            // 
            // lev_Revision
            // 
            this.lev_Revision.Location = new System.Drawing.Point(124, 60);
            this.lev_Revision.Name = "lev_Revision";
            this.lev_Revision.Size = new System.Drawing.Size(112, 22);
            this.lev_Revision.TabIndex = 8;
            this.lev_Revision.Text = "000";
            this.lev_Revision.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label16
            // 
            this.Label16.Location = new System.Drawing.Point(6, 60);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(112, 22);
            this.Label16.TabIndex = 7;
            this.Label16.Text = "Revision:";
            this.Label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // level_import
            // 
            this.level_import.Location = new System.Drawing.Point(6, 85);
            this.level_import.Name = "level_import";
            this.level_import.Size = new System.Drawing.Size(245, 23);
            this.level_import.TabIndex = 4;
            this.level_import.Text = "Import";
            this.level_import.UseVisualStyleBackColor = true;
            this.level_import.Click += new System.EventHandler(this.level_import_Click);
            // 
            // lev_Count
            // 
            this.lev_Count.Location = new System.Drawing.Point(124, 38);
            this.lev_Count.Name = "lev_Count";
            this.lev_Count.Size = new System.Drawing.Size(112, 22);
            this.lev_Count.TabIndex = 3;
            this.lev_Count.Text = "000";
            this.lev_Count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lev_date
            // 
            this.lev_date.Location = new System.Drawing.Point(124, 16);
            this.lev_date.Name = "lev_date";
            this.lev_date.Size = new System.Drawing.Size(112, 22);
            this.lev_date.TabIndex = 2;
            this.lev_date.Text = "DD/MMM/YYYY";
            this.lev_date.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Label22
            // 
            this.Label22.Location = new System.Drawing.Point(6, 38);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(112, 22);
            this.Label22.TabIndex = 1;
            this.Label22.Text = "Record Count:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label24
            // 
            this.Label24.Location = new System.Drawing.Point(6, 16);
            this.Label24.Name = "Label24";
            this.Label24.Size = new System.Drawing.Size(112, 22);
            this.Label24.TabIndex = 0;
            this.Label24.Text = "Last Updated:";
            this.Label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // GroupBox8
            // 
            this.GroupBox8.Controls.Add(this.btnEntities);
            this.GroupBox8.Location = new System.Drawing.Point(538, 12);
            this.GroupBox8.Name = "GroupBox8";
            this.GroupBox8.Size = new System.Drawing.Size(257, 60);
            this.GroupBox8.TabIndex = 9;
            this.GroupBox8.TabStop = false;
            this.GroupBox8.Text = "Entities";
            // 
            // btnEntities
            // 
            this.btnEntities.Location = new System.Drawing.Point(6, 19);
            this.btnEntities.Name = "btnEntities";
            this.btnEntities.Size = new System.Drawing.Size(245, 23);
            this.btnEntities.TabIndex = 4;
            this.btnEntities.Text = "Import";
            this.btnEntities.UseVisualStyleBackColor = true;
            this.btnEntities.Click += new System.EventHandler(this.btnEntities_Click);
            // 
            // btnClearSI
            // 
            this.btnClearSI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnClearSI.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearSI.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnClearSI.Location = new System.Drawing.Point(18, 314);
            this.btnClearSI.Name = "btnClearSI";
            this.btnClearSI.Size = new System.Drawing.Size(245, 24);
            this.btnClearSI.TabIndex = 22;
            this.btnClearSI.Text = "Clear StaticIndex Values";
            this.btnClearSI.UseVisualStyleBackColor = true;
            this.btnClearSI.Click += new System.EventHandler(this.btnClearSI_Click);
            // 
            // btnStaticIndex
            // 
            this.btnStaticIndex.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnStaticIndex.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStaticIndex.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnStaticIndex.Location = new System.Drawing.Point(18, 344);
            this.btnStaticIndex.Name = "btnStaticIndex";
            this.btnStaticIndex.Size = new System.Drawing.Size(245, 24);
            this.btnStaticIndex.TabIndex = 21;
            this.btnStaticIndex.Text = "Assign StaticIndex Values";
            this.btnStaticIndex.UseVisualStyleBackColor = true;
            this.btnStaticIndex.Click += new System.EventHandler(this.Button2_Click);
            // 
            // btnDefiance
            // 
            this.btnDefiance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.btnDefiance.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDefiance.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnDefiance.Location = new System.Drawing.Point(18, 374);
            this.btnDefiance.Name = "btnDefiance";
            this.btnDefiance.Size = new System.Drawing.Size(245, 24);
            this.btnDefiance.TabIndex = 23;
            this.btnDefiance.Text = "Scan and Tag Blaster Defiance Effects";
            this.btnDefiance.UseVisualStyleBackColor = true;
            this.btnDefiance.Click += new System.EventHandler(this.btnDefiance_Click);
            // 
            // btnStaticExport
            // 
            this.btnStaticExport.Location = new System.Drawing.Point(281, 375);
            this.btnStaticExport.Name = "btnStaticExport";
            this.btnStaticExport.Size = new System.Drawing.Size(245, 23);
            this.btnStaticExport.TabIndex = 24;
            this.btnStaticExport.Text = "Export StaticIndex values";
            this.btnStaticExport.UseVisualStyleBackColor = true;
            // 
            // frmCSV
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 562);
            this.Controls.Add(this.btnStaticExport);
            this.Controls.Add(this.btnDefiance);
            this.Controls.Add(this.btnClearSI);
            this.Controls.Add(this.btnStaticIndex);
            this.Controls.Add(this.GroupBox8);
            this.Controls.Add(this.GroupBox7);
            this.Controls.Add(this.GroupBox6);
            this.Controls.Add(this.GroupBox5);
            this.Controls.Add(this.GroupBox4);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCSV";
            this.ShowInTaskbar = false;
            this.Text = "Data Import Hub";
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox6.ResumeLayout(false);
            this.GroupBox7.ResumeLayout(false);
            this.GroupBox8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        Label at_Count;
        Label at_Date;
        Button at_Import;
        Label at_Revision;
        Button btnBonusLookup;
        Button btnClearSI;
        Button btnDefiance;
        Button btnEnhEffects;
        Button btnEntities;
        Button btnImportRecipes;
        Button btnIOLevels;
        Button btnSalvageUpdate;
        Button btnStaticExport;
        Button btnStaticIndex;
        Label fx_Count;
        Label fx_Date;
        Button fx_Import;
        Label fx_Revision;
        GroupBox GroupBox2;
        GroupBox GroupBox3;
        GroupBox GroupBox4;
        GroupBox GroupBox5;
        GroupBox GroupBox6;
        GroupBox GroupBox7;
        GroupBox GroupBox8;
        Label invent_Date;
        Button invent_Import;
        Label invent_RecipeDate;
        Label invent_Revision;
        Button inventSetImport;
        Label Label10;
        Label Label11;
        Label Label12;
        Label Label13;
        Label Label14;
        Label Label15;
        Label Label16;
        Label Label17;
        Label Label19;
        Label Label21;
        Label Label22;
        Label Label23;
        Label Label24;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label8;
        Label Label9;
        Label lev_Count;
        Label lev_date;
        Label lev_Revision;
        Button level_import;
        Label pow_Count;
        Label pow_Date;
        Button pow_Import;
        Label pow_Revision;
        Label set_Count;
        Label set_Date;
        Button set_Import;
        Label set_Revision;
    }
}