using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hero_Designer.Forms.Controls
{
    public class InputBox : Form
    {
        private Label inputLabel;
        private TextBox inputTextBox;
        private Button buttonOK;
        private Button buttonCancel;
        private Panel panel1;
        private PictureBox inputIcon;
        private static ImageList inputImages;
        private ErrorProvider errorProviderText;
        private IContainer components;
        private ComponentResourceManager resources;

        public enum InputBoxIcon
        {
            None,
            Info,
            Question,
            Warning,
            Error
        }

        private InputBox()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
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
            components = new Container();
            resources = new ComponentResourceManager(typeof(InputBox));
            inputLabel = new Label();
            inputTextBox = new TextBox();
            buttonOK = new Button();
            buttonCancel = new Button();
            panel1 = new Panel();
            inputIcon = new PictureBox();
            inputImages = new ImageList(components);
            errorProviderText = new ErrorProvider(components);
            panel1.SuspendLayout();
            ((ISupportInitialize)inputIcon).BeginInit();
            ((ISupportInitialize)errorProviderText).BeginInit();
            SuspendLayout();
            // 
            // inputLabel
            // 
            inputLabel.AutoSize = true;
            inputLabel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, (byte)0);
            inputLabel.Location = new Point(73, 9);
            inputLabel.Name = "inputLabel";
            inputLabel.Size = new Size(71, 13);
            inputLabel.TabIndex = 0;
            inputLabel.Text = "Description";
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new Point(76, 37);
            inputTextBox.Multiline = true;
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(399, 20);
            inputTextBox.TabIndex = 1;
            inputTextBox.TextChanged += new EventHandler(inputTextBox_TextChanged);
            inputTextBox.Validating += new CancelEventHandler(inputTextBox_Validating);
            // 
            // buttonOK
            // 
            buttonOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOK.DialogResult = DialogResult.OK;
            buttonOK.FlatAppearance.BorderSize = 0;
            buttonOK.Location = new Point(319, 4);
            buttonOK.Name = "buttonOK";
            buttonOK.Size = new Size(75, 23);
            buttonOK.TabIndex = 2;
            buttonOK.Text = "OK";
            buttonOK.UseVisualStyleBackColor = true;
            buttonOK.Click += new EventHandler(buttonOK_Click);
            // 
            // buttonCancel
            // 
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.FlatAppearance.BorderSize = 0;
            buttonCancel.Location = new Point(400, 4);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(75, 23);
            buttonCancel.TabIndex = 3;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            buttonCancel.Click += new EventHandler(buttonCancel_Click);
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.Control;
            panel1.Controls.Add(buttonOK);
            panel1.Controls.Add(buttonCancel);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 65);
            panel1.Name = "panel1";
            panel1.Size = new Size(487, 30);
            panel1.TabIndex = 4;
            // 
            // inputIcon
            // 
            inputIcon.BackgroundImageLayout = ImageLayout.Stretch;
            inputIcon.Location = new Point(12, 9);
            inputIcon.Name = "inputIcon";
            inputIcon.Size = new Size(48, 48);
            inputIcon.TabIndex = 5;
            inputIcon.TabStop = false;
            // 
            // inputImages
            // 
            inputImages.ImageStream = (ImageListStreamer)resources.GetObject("inputImages.ImageStream");
            inputImages.TransparentColor = Color.Transparent;
            inputImages.Images.SetKeyName(0, "None.png");
            inputImages.Images.SetKeyName(1, "Info.png");
            inputImages.Images.SetKeyName(2, "Question.png");
            inputImages.Images.SetKeyName(3, "Warning.png");
            inputImages.Images.SetKeyName(4, "Error.png");
            // 
            // errorProviderText
            // 
            errorProviderText.ContainerControl = this;
            errorProviderText.DataMember = "";
            // 
            // InputBox
            // 
            AcceptButton = buttonOK;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            CancelButton = buttonCancel;
            ClientSize = new Size(487, 95);
            Controls.Add(inputIcon);
            Controls.Add(panel1);
            Controls.Add(inputTextBox);
            Controls.Add(inputLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Title";
            TopMost = true;
            panel1.ResumeLayout(false);
            ((ISupportInitialize)inputIcon).EndInit();
            ((ISupportInitialize)errorProviderText).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private static InputBoxResult Show(string prompt, string title, string defaultResponse, InputBoxIcon icon, InputBoxValidatingHandler validator, int xpos, int ypos)
        {
            using var form = new InputBox
            {
                inputLabel = { Text = prompt },
                Text = title,
                inputTextBox = { Text = defaultResponse },
                inputIcon = { Image = inputImages.Images[(int)icon] }
            };
            if (xpos >= 0 && ypos >= 0)
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Left = xpos;
                form.Top = ypos;
            }
            else if (xpos == -1 && ypos == -1)
            {
                form.StartPosition = FormStartPosition.CenterParent;
            }
            form.Validator = validator;

            var result = form.ShowDialog();

            var returnVal = new InputBoxResult();
            if (result != DialogResult.OK) return returnVal;
            returnVal.Text = form.inputTextBox.Text;
            returnVal.OK = true;
            return returnVal;
        }

        public static InputBoxResult Show(string prompt, string title, string defaultText, InputBoxIcon icon, InputBoxValidatingHandler validator)
        {
            return Show(prompt, title, defaultText, icon, validator, -1, -1);
        }

        private void inputTextBox_TextChanged(object sender, EventArgs e)
        {
            errorProviderText.SetError(inputTextBox, "");
        }

        private void inputTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (Validator == null) return;
            var args = new InputBoxValidatingArgs { Text = inputTextBox.Text };
            Validator(this, args);
            if (!args.Cancel) return;
            e.Cancel = true;
            errorProviderText.SetError(inputTextBox, args.Message);
        }

        private InputBoxValidatingHandler Validator { get; set; }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Validator = null;
            Close();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class InputBoxResult
    {
        public bool OK;
        public string Text;
    }

    public class InputBoxValidatingArgs : EventArgs
    {
        public string Text;
        public string Message;
        public bool Cancel;
    }

    public delegate void InputBoxValidatingHandler(object sender, InputBoxValidatingArgs e);
}
