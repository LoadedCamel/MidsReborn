using System.ComponentModel;
using Mids_Reborn.Controls;

namespace Mids_Reborn.Forms.ImportExportItems
{
    public partial class frmForum
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
            this.Label4 = new System.Windows.Forms.Label();
            this.csSlots = new System.Windows.Forms.Label();
            this.csLevel = new System.Windows.Forms.Label();
            this.csHeading = new System.Windows.Forms.Label();
            this.csTitle = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.csList = new System.Windows.Forms.ListBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.lstCodes = new System.Windows.Forms.ListBox();
            this.lblCodeInf = new System.Windows.Forms.Label();
            this.chkDataChunk = new System.Windows.Forms.CheckBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkNoSetName = new System.Windows.Forms.CheckBox();
            this.chkNoIOLevel = new System.Windows.Forms.CheckBox();
            this.chkNoEnh = new System.Windows.Forms.CheckBox();
            this.chkBonusList = new System.Windows.Forms.CheckBox();
            this.chkBreakdown = new System.Windows.Forms.CheckBox();
            this.chkChunkOnly = new System.Windows.Forms.CheckBox();
            this.chkAlwaysDataChunk = new System.Windows.Forms.CheckBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.pbTitle = new System.Windows.Forms.PictureBox();
            this.IBCancel = new ImageButton();
            this.IBExport = new ImageButton();
            this.lblRecess = new System.Windows.Forms.Label();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.pbTitle).BeginInit();
            this.SuspendLayout();
            this.GroupBox1.BackColor = System.Drawing.Color.Black;
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.csSlots);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.csLevel);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.csHeading);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.csTitle);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label21);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label22);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label20);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.Label19);
            this.GroupBox1.Controls.Add((System.Windows.Forms.Control)this.csList);
            this.GroupBox1.ForeColor = System.Drawing.Color.White;

            this.GroupBox1.Location = new System.Drawing.Point(19, 34);
            this.GroupBox1.Name = "GroupBox1";

            this.GroupBox1.Size = new System.Drawing.Size(328, 222);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Color Scheme:";

            this.Label4.Location = new System.Drawing.Point(165, 105);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(157, 89);
            this.Label4.TabIndex = 19;
            this.Label4.Text = "Color schemes marked '(US)' are intended for display on a dark bacground, and are suitable for the USA CoX forums.";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.csSlots.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csSlots.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csSlots.Location = new System.Drawing.Point(260, 80);
            this.csSlots.Name = "csSlots";

            this.csSlots.Size = new System.Drawing.Size(52, 16);
            this.csSlots.TabIndex = 18;
            this.csLevel.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csLevel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csLevel.Location = new System.Drawing.Point(260, 60);
            this.csLevel.Name = "csLevel";

            this.csLevel.Size = new System.Drawing.Size(52, 16);
            this.csLevel.TabIndex = 17;
            this.csHeading.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csHeading.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csHeading.Location = new System.Drawing.Point(260, 40);
            this.csHeading.Name = "csHeading";

            this.csHeading.Size = new System.Drawing.Size(52, 16);
            this.csHeading.TabIndex = 16;
            this.csTitle.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csTitle.Location = new System.Drawing.Point(260, 20);
            this.csTitle.Name = "csTitle";

            this.csTitle.Size = new System.Drawing.Size(52, 16);
            this.csTitle.TabIndex = 15;

            this.Label21.Location = new System.Drawing.Point(168, 80);
            this.Label21.Name = "Label21";

            this.Label21.Size = new System.Drawing.Size(88, 16);
            this.Label21.TabIndex = 14;
            this.Label21.Text = "Slots:";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label22.Location = new System.Drawing.Point(168, 60);
            this.Label22.Name = "Label22";

            this.Label22.Size = new System.Drawing.Size(88, 16);
            this.Label22.TabIndex = 13;
            this.Label22.Text = "Levels:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label20.Location = new System.Drawing.Point(168, 40);
            this.Label20.Name = "Label20";

            this.Label20.Size = new System.Drawing.Size(88, 16);
            this.Label20.TabIndex = 12;
            this.Label20.Text = "Subheadings:";
            this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label19.Location = new System.Drawing.Point(168, 20);
            this.Label19.Name = "Label19";

            this.Label19.Size = new System.Drawing.Size(88, 16);
            this.Label19.TabIndex = 11;
            this.Label19.Text = "Title Text:";
            this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.csList.ItemHeight = 14;

            this.csList.Location = new System.Drawing.Point(8, 20);
            this.csList.Name = "csList";

            this.csList.Size = new System.Drawing.Size(151, 186);
            this.csList.TabIndex = 10;
            this.csList.SelectedIndexChanged += new System.EventHandler(csList_SelectedIndexChanged);
            this.GroupBox2.BackColor = System.Drawing.Color.Black;
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lstCodes);
            this.GroupBox2.Controls.Add((System.Windows.Forms.Control)this.lblCodeInf);
            this.GroupBox2.ForeColor = System.Drawing.Color.White;

            this.GroupBox2.Location = new System.Drawing.Point(353, 34);
            this.GroupBox2.Name = "GroupBox2";

            this.GroupBox2.Size = new System.Drawing.Size(268, 222);
            this.GroupBox2.TabIndex = 1;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Formatting Code Type:";

            this.Label1.Location = new System.Drawing.Point(8, 16);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(256, 56);
            this.Label1.TabIndex = 5;
            this.Label1.Text = "Different forums use different tags to change font style. Pick the forum type you need form this list. You can add more forum code sets in the program options window.";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lstCodes.ItemHeight = 14;

            this.lstCodes.Location = new System.Drawing.Point(8, 76);
            this.lstCodes.Name = "lstCodes";

            this.lstCodes.Size = new System.Drawing.Size(252, 102);
            this.lstCodes.TabIndex = 0;
            this.lstCodes.SelectedIndexChanged += new System.EventHandler(lstCodes_SelectedIndexChanged);
            this.lblCodeInf.BackColor = System.Drawing.Color.Black;
            this.lblCodeInf.ForeColor = System.Drawing.Color.White;

            this.lblCodeInf.Location = new System.Drawing.Point(6, 187);
            this.lblCodeInf.Name = "lblCodeInf";

            this.lblCodeInf.Size = new System.Drawing.Size(256, 32);
            this.lblCodeInf.TabIndex = 4;
            this.chkDataChunk.Checked = true;
            this.chkDataChunk.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDataChunk.ForeColor = System.Drawing.Color.White;

            this.chkDataChunk.Location = new System.Drawing.Point(11, 76);
            this.chkDataChunk.Name = "chkDataChunk";

            this.chkDataChunk.Size = new System.Drawing.Size(371, 20);
            this.chkDataChunk.TabIndex = 3;
            this.chkDataChunk.Text = "Export Data Chunk if creating a DataLink isn't possible";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkDataChunk, "Enable this to include a data chunk which can be copied by other forum users and imported into the Hero Designer");
            this.chkNoSetName.ForeColor = System.Drawing.Color.White;

            this.chkNoSetName.Location = new System.Drawing.Point(12, 52);
            this.chkNoSetName.Name = "chkNoSetName";

            this.chkNoSetName.Size = new System.Drawing.Size(180, 20);
            this.chkNoSetName.TabIndex = 7;
            this.chkNoSetName.Text = "Don't Export IO Set Names";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkNoSetName, "Invention enhancements will not show which set they're from.");
            this.chkNoIOLevel.ForeColor = System.Drawing.Color.White;

            this.chkNoIOLevel.Location = new System.Drawing.Point(12, 76);
            this.chkNoIOLevel.Name = "chkNoIOLevel";

            this.chkNoIOLevel.Size = new System.Drawing.Size(180, 20);
            this.chkNoIOLevel.TabIndex = 8;
            this.chkNoIOLevel.Text = "Don't Export Invention Levels";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkNoIOLevel, "Hides the level of Invention origin enhancements.");
            this.chkNoEnh.ForeColor = System.Drawing.Color.White;

            this.chkNoEnh.Location = new System.Drawing.Point(12, 100);
            this.chkNoEnh.Name = "chkNoEnh";

            this.chkNoEnh.Size = new System.Drawing.Size(180, 20);
            this.chkNoEnh.TabIndex = 9;
            this.chkNoEnh.Text = "Don't Export Enhancements";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkNoEnh, "Exported builds won't show which enhancements are slotted into powers.");
            this.chkBonusList.ForeColor = System.Drawing.Color.White;

            this.chkBonusList.Location = new System.Drawing.Point(12, 19);
            this.chkBonusList.Name = "chkBonusList";

            this.chkBonusList.Size = new System.Drawing.Size(143, 20);
            this.chkBonusList.TabIndex = 8;
            this.chkBonusList.Text = "Export Bonus Totals";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkBonusList, "The total effects of your set bonuses will be added to the end of the export");
            this.chkBreakdown.ForeColor = System.Drawing.Color.White;

            this.chkBreakdown.Location = new System.Drawing.Point(12, 43);
            this.chkBreakdown.Name = "chkBreakdown";

            this.chkBreakdown.Size = new System.Drawing.Size(143, 20);
            this.chkBreakdown.TabIndex = 9;
            this.chkBreakdown.Text = "Export Set Breakdown";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkBreakdown, "A detailed breakdown of your set bonuses, including which power\r\nthey're coming from, will be appended to the export.");
            this.chkChunkOnly.ForeColor = System.Drawing.Color.White;

            this.chkChunkOnly.Location = new System.Drawing.Point(32, 100);
            this.chkChunkOnly.Name = "chkChunkOnly";

            this.chkChunkOnly.Size = new System.Drawing.Size(168, 20);
            this.chkChunkOnly.TabIndex = 9;
            this.chkChunkOnly.Text = "Only export Data Chunk";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkChunkOnly, "Exports the Data Chunk by itself.\r\nDoesn't export any human-readable build information.");
            this.chkAlwaysDataChunk.ForeColor = System.Drawing.Color.White;

            this.chkAlwaysDataChunk.Location = new System.Drawing.Point(8, 126);
            this.chkAlwaysDataChunk.Name = "chkAlwaysDataChunk";

            this.chkAlwaysDataChunk.Size = new System.Drawing.Size(371, 20);
            this.chkAlwaysDataChunk.TabIndex = 10;
            this.chkAlwaysDataChunk.Text = "Export Data Chunk as well as a DataLink";
            this.ToolTip1.SetToolTip((System.Windows.Forms.Control)this.chkAlwaysDataChunk, "Enable this to include a data chunk which can be copied by other forum users and imported into the Hero Designer");
            this.GroupBox3.BackColor = System.Drawing.Color.Black;
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.Label3);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.chkNoIOLevel);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.chkNoSetName);
            this.GroupBox3.Controls.Add((System.Windows.Forms.Control)this.chkNoEnh);
            this.GroupBox3.ForeColor = System.Drawing.Color.White;

            this.GroupBox3.Location = new System.Drawing.Point(413, 262);
            this.GroupBox3.Name = "GroupBox3";

            this.GroupBox3.Size = new System.Drawing.Size(208, 124);
            this.GroupBox3.TabIndex = 10;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Enhancements:";

            this.Label3.Location = new System.Drawing.Point(12, 20);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(192, 28);
            this.Label3.TabIndex = 10;
            this.Label3.Text = "These affect the build profile only, the data chunk is unaffected.";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.GroupBox4.BackColor = System.Drawing.Color.Black;
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkAlwaysDataChunk);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkChunkOnly);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.GroupBox4.Controls.Add((System.Windows.Forms.Control)this.chkDataChunk);
            this.GroupBox4.ForeColor = System.Drawing.Color.White;

            this.GroupBox4.Location = new System.Drawing.Point(19, 262);
            this.GroupBox4.Name = "GroupBox4";

            this.GroupBox4.Size = new System.Drawing.Size(388, 152);
            this.GroupBox4.TabIndex = 11;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Data Chunk:";

            this.Label2.Location = new System.Drawing.Point(8, 16);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(374, 56);
            this.Label2.TabIndex = 7;
            this.Label2.Text = "If a build is too complex to be exported in a DataLink, export can fall back to exporting a Data Chunk at the end of a build. Users can import the data chunk to view the build.";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.GroupBox5.BackColor = System.Drawing.Color.Black;
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.chkBreakdown);
            this.GroupBox5.Controls.Add((System.Windows.Forms.Control)this.chkBonusList);
            this.GroupBox5.ForeColor = System.Drawing.Color.White;

            this.GroupBox5.Location = new System.Drawing.Point(413, 392);
            this.GroupBox5.Name = "GroupBox5";

            this.GroupBox5.Size = new System.Drawing.Size(208, 72);
            this.GroupBox5.TabIndex = 12;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Set Bonuses:";
            this.pbTitle.BackColor = System.Drawing.Color.Transparent;

            this.pbTitle.Location = new System.Drawing.Point(180, 6);
            this.pbTitle.Name = "pbTitle";
            this.pbTitle.Size = new System.Drawing.Size(263, 24);
            this.pbTitle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbTitle.TabIndex = 15;
            this.pbTitle.TabStop = false;
            this.pbTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(pbTitle_MouseMove);
            this.pbTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(pbTitle_MouseDown);
            // 
            // ibCancel
            // 
            this.IBCancel.Checked = false;
            this.IBCancel.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.IBCancel.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.IBCancel.Location = new System.Drawing.Point(52, 431);
            this.IBCancel.Name = "IBCancel";
            this.IBCancel.Size = new System.Drawing.Size(105, 22);
            this.IBCancel.TabIndex = 14;
            this.IBCancel.TextOff = "Cancel";
            this.IBCancel.TextOn = "Alt Text";
            this.IBCancel.Toggle = false;
            this.IBCancel.ButtonClicked += new ImageButton.ButtonClickedEventHandler(ibCancel_ButtonClicked);
            // 
            // ibExport
            // 
            this.IBExport.Checked = false;
            this.IBExport.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.IBExport.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.IBExport.Location = new System.Drawing.Point(246, 431);
            this.IBExport.Name = "IBExport";
            this.IBExport.Size = new System.Drawing.Size(105, 22);
            this.IBExport.TabIndex = 13;
            this.IBExport.TextOff = "Export Now";
            this.IBExport.TextOn = "Alt Text";
            this.IBExport.Toggle = false;
            this.IBExport.ButtonClicked += new ImageButton.ButtonClickedEventHandler(ibExport_ButtonClicked);
            this.lblRecess.BackColor = System.Drawing.Color.Black;
            this.lblRecess.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

            this.lblRecess.Location = new System.Drawing.Point(12, 31);
            this.lblRecess.Name = "lblRecess";

            this.lblRecess.Size = new System.Drawing.Size(616, 439);
            this.lblRecess.TabIndex = 16;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(0, 0, 32);
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;

            this.ClientSize = new System.Drawing.Size(640, 480);
            this.Controls.Add((System.Windows.Forms.Control)this.pbTitle);
            this.Controls.Add((System.Windows.Forms.Control)this.IBCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.IBExport);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox5);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox4);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox3);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox2);
            this.Controls.Add((System.Windows.Forms.Control)this.GroupBox1);
            this.Controls.Add((System.Windows.Forms.Control)this.lblRecess);
            this.Font = new System.Drawing.Font("Arial", 11f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, (byte)0);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Forum Export";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.pbTitle).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        System.Windows.Forms.CheckBox chkAlwaysDataChunk;
        System.Windows.Forms.CheckBox chkBonusList;
        System.Windows.Forms.CheckBox chkBreakdown;
        System.Windows.Forms.CheckBox chkChunkOnly;
        System.Windows.Forms.CheckBox chkDataChunk;
        System.Windows.Forms.CheckBox chkNoEnh;
        System.Windows.Forms.CheckBox chkNoIOLevel;
        System.Windows.Forms.CheckBox chkNoSetName;
        System.Windows.Forms.Label csHeading;
        System.Windows.Forms.Label csLevel;
        System.Windows.Forms.ListBox csList;
        System.Windows.Forms.Label csSlots;
        System.Windows.Forms.Label csTitle;
        System.Windows.Forms.GroupBox GroupBox1;
        System.Windows.Forms.GroupBox GroupBox2;
        System.Windows.Forms.GroupBox GroupBox3;
        System.Windows.Forms.GroupBox GroupBox4;
        System.Windows.Forms.GroupBox GroupBox5;
        System.Windows.Forms.Label Label1;
        System.Windows.Forms.Label Label19;
        System.Windows.Forms.Label Label2;
        System.Windows.Forms.Label Label20;
        System.Windows.Forms.Label Label21;
        System.Windows.Forms.Label Label22;
        System.Windows.Forms.Label Label3;
        System.Windows.Forms.Label Label4;
        System.Windows.Forms.Label lblCodeInf;
        System.Windows.Forms.Label lblRecess;
        System.Windows.Forms.ListBox lstCodes;
        System.Windows.Forms.PictureBox pbTitle;
        System.Windows.Forms.ToolTip ToolTip1;

        #endregion
    }
}