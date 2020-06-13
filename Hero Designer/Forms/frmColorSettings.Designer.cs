using System.ComponentModel;
using System.Windows.Forms;

namespace Hero_Designer
{
    public partial class frmColorSettings
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

            this.rtPreview = new System.Windows.Forms.RichTextBox();
            this.csInv = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.csAlert = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.csEnh = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.csFade = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.csInvInv = new System.Windows.Forms.Label();
            this.csText = new System.Windows.Forms.Label();
            this.csVillain = new System.Windows.Forms.Label();
            this.csHero = new System.Windows.Forms.Label();
            this.Label21 = new System.Windows.Forms.Label();
            this.Label22 = new System.Windows.Forms.Label();
            this.Label20 = new System.Windows.Forms.Label();
            this.Label19 = new System.Windows.Forms.Label();
            this.cPicker = new System.Windows.Forms.ColorDialog();
            this.Label1 = new System.Windows.Forms.Label();
            this.csSpecial = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.csValue = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.Listlabel1 = new midsControls.ListLabelV3();
            this.SuspendLayout();

            this.rtPreview.Location = new System.Drawing.Point(218, 49);
            this.rtPreview.Name = "rtPreview";

            this.rtPreview.Size = new System.Drawing.Size(197, 195);
            this.rtPreview.TabIndex = 0;
            this.rtPreview.Text = "";
            this.csInv.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csInv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csInv.Location = new System.Drawing.Point(134, 109);
            this.csInv.Name = "csInv";

            this.csInv.Size = new System.Drawing.Size(52, 16);
            this.csInv.TabIndex = 51;
            this.csInv.Click += new System.EventHandler(csInv_Click);

            this.Label3.Location = new System.Drawing.Point(12, 109);
            this.Label3.Name = "Label3";

            this.Label3.Size = new System.Drawing.Size(118, 16);
            this.Label3.TabIndex = 50;
            this.Label3.Text = "Inventions:";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.btnCancel.Location = new System.Drawing.Point(192, 371);
            this.btnCancel.Name = "btnCancel";

            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 49;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(btnCancel_Click);
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.btnOK.Location = new System.Drawing.Point(276, 371);
            this.btnOK.Name = "btnOK";

            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 48;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(btnOK_Click);
            this.csAlert.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csAlert.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csAlert.Location = new System.Drawing.Point(134, 189);
            this.csAlert.Name = "csAlert";

            this.csAlert.Size = new System.Drawing.Size(52, 16);
            this.csAlert.TabIndex = 47;
            this.csAlert.Click += new System.EventHandler(csAlert_Click);

            this.Label9.Location = new System.Drawing.Point(12, 189);
            this.Label9.Name = "Label9";

            this.Label9.Size = new System.Drawing.Size(118, 16);
            this.Label9.TabIndex = 46;
            this.Label9.Text = "Alert:";
            this.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.csEnh.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csEnh.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csEnh.Location = new System.Drawing.Point(134, 169);
            this.csEnh.Name = "csEnh";

            this.csEnh.Size = new System.Drawing.Size(52, 16);
            this.csEnh.TabIndex = 45;
            this.csEnh.Click += new System.EventHandler(csEnh_Click);

            this.Label7.Location = new System.Drawing.Point(12, 169);
            this.Label7.Name = "Label7";

            this.Label7.Size = new System.Drawing.Size(118, 16);
            this.Label7.TabIndex = 44;
            this.Label7.Text = "Enhancements:";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label5.Location = new System.Drawing.Point(17, 16);
            this.Label5.Name = "Label5";

            this.Label5.Size = new System.Drawing.Size(192, 16);
            this.Label5.TabIndex = 43;
            this.Label5.Text = "Click on a color box to modify it.";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.csFade.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csFade.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csFade.Location = new System.Drawing.Point(134, 149);
            this.csFade.Name = "csFade";

            this.csFade.Size = new System.Drawing.Size(52, 16);
            this.csFade.TabIndex = 42;
            this.csFade.Click += new System.EventHandler(csFade_Click);

            this.Label4.Location = new System.Drawing.Point(12, 149);
            this.Label4.Name = "Label4";

            this.Label4.Size = new System.Drawing.Size(118, 16);
            this.Label4.TabIndex = 41;
            this.Label4.Text = "Faded:";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.csInvInv.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csInvInv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csInvInv.Location = new System.Drawing.Point(134, 129);
            this.csInvInv.Name = "csInvInv";

            this.csInvInv.Size = new System.Drawing.Size(52, 16);
            this.csInvInv.TabIndex = 40;
            this.csInvInv.Click += new System.EventHandler(csInvInv_Click);
            this.csText.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csText.Location = new System.Drawing.Point(134, 89);
            this.csText.Name = "csText";

            this.csText.Size = new System.Drawing.Size(52, 16);
            this.csText.TabIndex = 39;
            this.csText.Click += new System.EventHandler(csText_Click);

            this.csVillain.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csVillain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.csVillain.Location = new System.Drawing.Point(134, 69);
            this.csVillain.Name = "csVillain";
            this.csVillain.Size = new System.Drawing.Size(52, 16);
            this.csVillain.TabIndex = 38;
            this.csVillain.Click += new System.EventHandler(csVillain_Click);

            this.csHero.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csHero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.csHero.Location = new System.Drawing.Point(134, 49);
            this.csHero.Name = "csHero";
            this.csHero.Size = new System.Drawing.Size(52, 16);
            this.csHero.TabIndex = 37;
            this.csHero.Click += new System.EventHandler(csHero_Click);

            this.Label21.Location = new System.Drawing.Point(12, 129);
            this.Label21.Name = "Label21";

            this.Label21.Size = new System.Drawing.Size(118, 16);
            this.Label21.TabIndex = 36;
            this.Label21.Text = "Inventions (On White)";
            this.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label22.Location = new System.Drawing.Point(12, 89);
            this.Label22.Name = "Label22";

            this.Label22.Size = new System.Drawing.Size(118, 16);
            this.Label22.TabIndex = 35;
            this.Label22.Text = "Text:";
            this.Label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label20.Location = new System.Drawing.Point(12, 69);
            this.Label20.Name = "Label20";

            this.Label20.Size = new System.Drawing.Size(118, 16);
            this.Label20.TabIndex = 34;
            this.Label20.Text = "Villain Background:";
            this.Label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.Label19.Location = new System.Drawing.Point(12, 49);
            this.Label19.Name = "Label19";

            this.Label19.Size = new System.Drawing.Size(118, 16);
            this.Label19.TabIndex = 33;
            this.Label19.Text = "Background:";
            this.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cPicker.FullOpen = true;

            this.Label1.Location = new System.Drawing.Point(215, 30);
            this.Label1.Name = "Label1";

            this.Label1.Size = new System.Drawing.Size(88, 16);
            this.Label1.TabIndex = 52;
            this.Label1.Text = "Preview:";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.csSpecial.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csSpecial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csSpecial.Location = new System.Drawing.Point(134, 229);
            this.csSpecial.Name = "csSpecial";

            this.csSpecial.Size = new System.Drawing.Size(52, 16);
            this.csSpecial.TabIndex = 56;
            this.csSpecial.Click += new System.EventHandler(csSpecial_Click);

            this.Label6.Location = new System.Drawing.Point(12, 228);
            this.Label6.Name = "Label6";

            this.Label6.Size = new System.Drawing.Size(118, 16);
            this.Label6.TabIndex = 55;
            this.Label6.Text = "Special Case Value:";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.csValue.BackColor = System.Drawing.Color.FromArgb(128, 128, (int)byte.MaxValue);
            this.csValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.csValue.Location = new System.Drawing.Point(134, 209);
            this.csValue.Name = "csValue";

            this.csValue.Size = new System.Drawing.Size(52, 16);
            this.csValue.TabIndex = 54;
            this.csValue.Click += new System.EventHandler(csValue_Click);

            this.Label10.Location = new System.Drawing.Point(12, 208);
            this.Label10.Name = "Label10";

            this.Label10.Size = new System.Drawing.Size(118, 16);
            this.Label10.TabIndex = 53;
            this.Label10.Text = "Value Name:";
            this.Label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.btnReset.Location = new System.Drawing.Point(85, 371);
            this.btnReset.Name = "btnReset";

            this.btnReset.Size = new System.Drawing.Size(98, 23);
            this.btnReset.TabIndex = 57;
            this.btnReset.Text = "Set to Defaults";
            this.btnReset.Click += new System.EventHandler(btnReset_Click);

            this.Label2.Location = new System.Drawing.Point(20, 265);
            this.Label2.Name = "Label2";

            this.Label2.Size = new System.Drawing.Size(189, 91);
            this.Label2.TabIndex = 59;
            this.Label2.Text = "Click an item in the list to the right to modify the colors used in power lists";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.Listlabel1.AutoSize = true;
            this.Listlabel1.Expandable = false;
            this.Listlabel1.Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.Listlabel1.HighVis = true;
            this.Listlabel1.HoverColor = System.Drawing.Color.WhiteSmoke;

            this.Listlabel1.Location = new System.Drawing.Point(218, 265);
            this.Listlabel1.MaxHeight = 600;
            this.Listlabel1.Name = "Listlabel1";
            this.Listlabel1.PaddingX = 2;
            this.Listlabel1.PaddingY = 2;
            this.Listlabel1.Scrollable = true;
            this.Listlabel1.ScrollBarColor = System.Drawing.Color.Red;
            this.Listlabel1.ScrollBarWidth = 11;
            this.Listlabel1.ScrollButtonColor = System.Drawing.Color.FromArgb(192, 0, 0);

            this.Listlabel1.Size = new System.Drawing.Size(197, 91);

            this.Listlabel1.SizeNormal = new System.Drawing.Size(197, 91);
            this.Listlabel1.SuspendRedraw = false;
            this.Listlabel1.TabIndex = 111;
            this.Listlabel1.ItemClick += new midsControls.ListLabelV3.ItemClickEventHandler(Listlabel1_ItemClick);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;

            this.ClientSize = new System.Drawing.Size(600, 600);
            this.Controls.Add((System.Windows.Forms.Control)this.Listlabel1);
            this.Controls.Add((System.Windows.Forms.Control)this.Label2);
            this.Controls.Add((System.Windows.Forms.Control)this.btnReset);
            this.Controls.Add((System.Windows.Forms.Control)this.csSpecial);
            this.Controls.Add((System.Windows.Forms.Control)this.Label6);
            this.Controls.Add((System.Windows.Forms.Control)this.csValue);
            this.Controls.Add((System.Windows.Forms.Control)this.Label10);
            this.Controls.Add((System.Windows.Forms.Control)this.Label1);
            this.Controls.Add((System.Windows.Forms.Control)this.csInv);
            this.Controls.Add((System.Windows.Forms.Control)this.Label3);
            this.Controls.Add((System.Windows.Forms.Control)this.btnCancel);
            this.Controls.Add((System.Windows.Forms.Control)this.btnOK);
            this.Controls.Add((System.Windows.Forms.Control)this.csAlert);
            this.Controls.Add((System.Windows.Forms.Control)this.Label9);
            this.Controls.Add((System.Windows.Forms.Control)this.csEnh);
            this.Controls.Add((System.Windows.Forms.Control)this.Label7);
            this.Controls.Add((System.Windows.Forms.Control)this.Label5);
            this.Controls.Add((System.Windows.Forms.Control)this.csFade);
            this.Controls.Add((System.Windows.Forms.Control)this.Label4);
            this.Controls.Add((System.Windows.Forms.Control)this.csInvInv);
            this.Controls.Add((System.Windows.Forms.Control)this.csText);
            this.Controls.Add((System.Windows.Forms.Control)this.csVillain);
            this.Controls.Add((System.Windows.Forms.Control)this.csHero);
            this.Controls.Add((System.Windows.Forms.Control)this.Label21);
            this.Controls.Add((System.Windows.Forms.Control)this.Label22);
            this.Controls.Add((System.Windows.Forms.Control)this.Label20);
            this.Controls.Add((System.Windows.Forms.Control)this.Label19);
            this.Controls.Add((System.Windows.Forms.Control)this.rtPreview);
            this.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, (byte)0);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Colors";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        Button btnCancel;
        Button btnOK;
        Button btnReset;
        ColorDialog cPicker;
        Label csAlert;
        Label csEnh;
        Label csFade;
        Label csHero;
        Label csInv;
        Label csInvInv;
        Label csSpecial;
        Label csText;
        Label csValue;
        Label csVillain;
        Label Label1;
        Label Label10;
        Label Label19;
        Label Label2;
        Label Label20;
        Label Label21;
        Label Label22;
        Label Label3;
        Label Label4;
        Label Label5;
        Label Label6;
        Label Label7;
        Label Label9;
        RichTextBox rtPreview;
    }
}