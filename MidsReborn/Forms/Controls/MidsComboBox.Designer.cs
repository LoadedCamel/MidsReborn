namespace Mids_Reborn.Forms.Controls
{
    sealed partial class MidsComboBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.label = new System.Windows.Forms.Label();
            this.dropDownButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label
            // 
            this.label.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.label.Location = new System.Drawing.Point(3, 3);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(124, 13);
            this.label.TabIndex = 0;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label.BackColor = System.Drawing.Color.White; // Initial background color to mimic TextBox
            this.label.Click += new System.EventHandler(this.DropDownButton_Click);
            // 
            // dropDownButton
            // 
            this.dropDownButton.Location = new System.Drawing.Point(133, 0);
            this.dropDownButton.Name = "dropDownButton";
            this.dropDownButton.Size = new System.Drawing.Size(24, 21);
            this.dropDownButton.TabIndex = 1;
            this.dropDownButton.UseVisualStyleBackColor = true;
            this.dropDownButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dropDownButton.FlatAppearance.BorderSize = 0;
            this.dropDownButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.dropDownButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.dropDownButton.BackColor = System.Drawing.Color.Transparent;
            this.dropDownButton.Paint += new System.Windows.Forms.PaintEventHandler(this.DropDownButton_Paint);
            this.dropDownButton.Click += new System.EventHandler(this.DropDownButton_Click);
            // 
            // CustomComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dropDownButton);
            this.Controls.Add(this.label);
            this.Name = "CustomComboBox";
            this.Size = new System.Drawing.Size(160, 21);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button dropDownButton;
    }
}
