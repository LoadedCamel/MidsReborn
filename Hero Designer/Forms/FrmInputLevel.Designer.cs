using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer.Forms
{
    public partial class FrmInputLevel
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
            this.udLevel = new System.Windows.Forms.NumericUpDown();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.udLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // udLevel
            // 
            this.udLevel.Location = new System.Drawing.Point(41, 42);
            this.udLevel.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udLevel.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.udLevel.Name = "udLevel";
            this.udLevel.Size = new System.Drawing.Size(120, 20);
            this.udLevel.TabIndex = 0;
            this.udLevel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.udLevel.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.udLevel.Leave += new System.EventHandler(this.udLevel_Leave);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(3, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(188, 21);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Please input the level to respec to:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOK.Location = new System.Drawing.Point(64, 77);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrmInputLevel
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnOK;
            this.ClientSize = new System.Drawing.Size(203, 114);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.udLevel);
            this.Font = new System.Drawing.Font("Arial", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmInputLevel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Respec Helper";
            this.Load += new System.EventHandler(this.FrmInputLevel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.udLevel)).EndInit();
            this.ResumeLayout(false);

        }


        #endregion

        Button btnOK;
        Label Label1;
        NumericUpDown udLevel;
    }
}