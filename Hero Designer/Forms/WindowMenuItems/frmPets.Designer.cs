using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer
{
    public partial class frmPets
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
            this.cbSelPetPower = new System.Windows.Forms.ComboBox();
            this.cbSelPets = new System.Windows.Forms.ComboBox();
            this.lblPrimary = new System.Windows.Forms.Label();
            this.ibClose = new midsControls.ImageButton();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbSelPetPower
            // 
            this.cbSelPetPower.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelPetPower.Location = new System.Drawing.Point(88, 29);
            this.cbSelPetPower.Name = "cbSelPetPower";
            this.cbSelPetPower.Size = new System.Drawing.Size(167, 22);
            this.cbSelPetPower.TabIndex = 33;
            this.cbSelPetPower.SelectedIndexChanged += new System.EventHandler(this.cbSelPetPower_SelectedIndexChanged);
            // 
            // cbSelPets
            // 
            this.cbSelPets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSelPets.Location = new System.Drawing.Point(88, 69);
            this.cbSelPets.Name = "cbSelPets";
            this.cbSelPets.Size = new System.Drawing.Size(167, 22);
            this.cbSelPets.TabIndex = 34;
            this.cbSelPets.SelectedIndexChanged += new System.EventHandler(this.cbSelPet_SelectedIndexChanged);
            // 
            // lblPrimary
            // 
            this.lblPrimary.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrimary.ForeColor = System.Drawing.Color.White;
            this.lblPrimary.Location = new System.Drawing.Point(64, 118);
            this.lblPrimary.Name = "lblPrimary";
            this.lblPrimary.Size = new System.Drawing.Size(136, 17);
            this.lblPrimary.TabIndex = 9;
            this.lblPrimary.Text = "Selected Pets Powers";
            this.lblPrimary.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ibClose
            // 
            this.ibClose.Checked = false;
            this.ibClose.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.ibClose.KnockoutLocationPoint = new System.Drawing.Point(0, 0);
            this.ibClose.Location = new System.Drawing.Point(454, 345);
            this.ibClose.Name = "ibClose";
            this.ibClose.Size = new System.Drawing.Size(105, 22);
            this.ibClose.TabIndex = 7;
            this.ibClose.TextOff = "Done";
            this.ibClose.TextOn = "Alt Text";
            this.ibClose.Toggle = false;
            this.ibClose.ButtonClicked += new midsControls.ImageButton.ButtonClickedEventHandler(this.ibClose_ButtonClicked);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.pictureBox1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(279, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(406, 314);
            this.flowLayoutPanel1.TabIndex = 35;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(403, 311);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 14;
            this.listBox1.Location = new System.Drawing.Point(32, 138);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(202, 186);
            this.listBox1.TabIndex = 36;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(15, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 19);
            this.label1.TabIndex = 37;
            this.label1.Text = "Pet Power:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(57, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 19);
            this.label2.TabIndex = 38;
            this.label2.Text = "Pet:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(702, 12);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(300, 312);
            this.listView1.TabIndex = 39;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // frmPets
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(32)))));
            this.ClientSize = new System.Drawing.Size(1014, 379);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.cbSelPetPower);
            this.Controls.Add(this.cbSelPets);
            this.Controls.Add(this.lblPrimary);
            this.Controls.Add(this.ibClose);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmPets";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "  Pet Info";
            this.TopMost = true;
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private PictureBox pictureBox1;
        private ListBox listBox1;
        private Label label1;
        private Label label2;
        private ListView listView1;
    }
}