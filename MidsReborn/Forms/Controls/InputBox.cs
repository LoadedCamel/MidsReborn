using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace Mids_Reborn.Forms.Controls
{
    public class InputBox : Form
    {
        private Label InputLabel;
        private TextBox InputTextBox;
        private Button ButtonOk;
        private Button ButtonCancel;
        private Panel Panel1;
        private IconButton IconButton1;
        private PictureBox InputIcon;
        private ImageList InputImages;
        private ErrorProvider ErrorProviderText;
        private IContainer Components;
        private IContainer components;
        private ComponentResourceManager Resources;

        public delegate void InputBoxValidatingHandler(object sender, InputBoxValidatingArgs e);
        private InputBoxValidatingHandler? Validator { get; set; }

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

        private InputBox(string prompt, string title, string defaultResponse, bool isPassword, InputBoxIcon icon)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();

            InputLabel.Text = prompt;
            Text = title;
            InputTextBox.Text = defaultResponse;
            IconButton1.Visible = isPassword;
            InputIcon.Image = InputImages.Images[(int)icon];
        }

        private InputBox(string prompt, string title, string defaultResponse, bool isPassword, int iconIndex)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            InitializeComponent();

            InputLabel.Text = prompt;
            Text = title;
            InputTextBox.Text = defaultResponse;
            IconButton1.Visible = isPassword;
            InputIcon.Image = InputImages.Images[iconIndex < 0 | iconIndex >= InputImages.Images.Count ? 0 : iconIndex];
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Components?.Dispose();
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
            ComponentResourceManager resources = new ComponentResourceManager(typeof(InputBox));
            InputLabel = new Label();
            InputTextBox = new TextBox();
            ButtonOk = new Button();
            ButtonCancel = new Button();
            Panel1 = new Panel();
            IconButton1 = new IconButton();
            InputIcon = new PictureBox();
            InputImages = new ImageList(components);
            ErrorProviderText = new ErrorProvider(components);
            Panel1.SuspendLayout();
            ((ISupportInitialize)InputIcon).BeginInit();
            ((ISupportInitialize)ErrorProviderText).BeginInit();
            SuspendLayout();
            // 
            // InputLabel
            // 
            InputLabel.AutoSize = true;
            // Disabled: large interline with Noto
            //InputLabel.Font = new Font(Fonts.Family("Noto Sans"), 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            InputLabel.Location = new Point(73, 9);
            InputLabel.Name = "InputLabel";
            InputLabel.Size = new Size(67, 15);
            InputLabel.TabIndex = 0;
            InputLabel.Text = "Description";
            // 
            // InputTextBox
            // 
            InputTextBox.Location = new Point(76, 45);
            InputTextBox.Name = "InputTextBox";
            InputTextBox.Size = new Size(399, 23);
            InputTextBox.TabIndex = 1;
            InputTextBox.TextChanged += InputTextBox_TextChanged;
            InputTextBox.Validating += InputTextBox_Validating;
            // 
            // ButtonOk
            // 
            ButtonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonOk.DialogResult = DialogResult.OK;
            ButtonOk.FlatAppearance.BorderSize = 0;
            ButtonOk.Location = new Point(319, 4);
            ButtonOk.Name = "ButtonOk";
            ButtonOk.Size = new Size(75, 23);
            ButtonOk.TabIndex = 2;
            ButtonOk.Text = "OK";
            ButtonOk.UseVisualStyleBackColor = true;
            ButtonOk.Click += ButtonOk_Click;
            // 
            // ButtonCancel
            // 
            ButtonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ButtonCancel.CausesValidation = false;
            ButtonCancel.DialogResult = DialogResult.Cancel;
            ButtonCancel.FlatAppearance.BorderSize = 0;
            ButtonCancel.Location = new Point(400, 4);
            ButtonCancel.Name = "ButtonCancel";
            ButtonCancel.Size = new Size(75, 23);
            ButtonCancel.TabIndex = 3;
            ButtonCancel.Text = "Cancel";
            ButtonCancel.UseVisualStyleBackColor = true;
            ButtonCancel.Click += ButtonCancel_Click;
            // 
            // Panel1
            // 
            Panel1.BackColor = SystemColors.Control;
            Panel1.Controls.Add(ButtonOk);
            Panel1.Controls.Add(ButtonCancel);
            Panel1.Dock = DockStyle.Bottom;
            Panel1.Location = new Point(0, 73);
            Panel1.Name = "Panel1";
            Panel1.Size = new Size(492, 30);
            Panel1.TabIndex = 4;
            // 
            // IconButton1
            // 
            IconButton1.FlatAppearance.BorderSize = 0;
            IconButton1.FlatStyle = FlatStyle.Flat;
            IconButton1.IconChar = IconChar.Eye;
            IconButton1.IconColor = Color.Black;
            IconButton1.IconFont = IconFont.Auto;
            IconButton1.IconSize = 20;
            IconButton1.Location = new Point(460, 45);
            IconButton1.Name = "IconButton1";
            IconButton1.Size = new Size(20, 19);
            IconButton1.TabIndex = 5;
            IconButton1.UseVisualStyleBackColor = true;
            IconButton1.Click += IconButton1_Click;
            // 
            // InputIcon
            // 
            InputIcon.BackgroundImageLayout = ImageLayout.Stretch;
            InputIcon.Location = new Point(12, 17);
            InputIcon.Name = "InputIcon";
            InputIcon.Size = new Size(48, 48);
            InputIcon.TabIndex = 6;
            InputIcon.TabStop = false;
            // 
            // InputImages
            // 
            InputImages.ColorDepth = ColorDepth.Depth32Bit;
            InputImages.ImageStream = (ImageListStreamer)resources.GetObject("InputImages.ImageStream");
            InputImages.TransparentColor = Color.Transparent;
            InputImages.Images.SetKeyName(0, "None.png");
            InputImages.Images.SetKeyName(1, "Info.png");
            InputImages.Images.SetKeyName(2, "Question.png");
            InputImages.Images.SetKeyName(3, "Warning.png");
            InputImages.Images.SetKeyName(4, "Error.png");
            // 
            // ErrorProviderText
            // 
            ErrorProviderText.ContainerControl = this;
            ErrorProviderText.DataMember = "";
            // 
            // InputBox
            // 
            AcceptButton = ButtonOk;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            CancelButton = ButtonCancel;
            ClientSize = new Size(492, 103);
            Controls.Add(IconButton1);
            Controls.Add(InputIcon);
            Controls.Add(Panel1);
            Controls.Add(InputTextBox);
            Controls.Add(InputLabel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InputBox";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Title";
            TopMost = true;
            Panel1.ResumeLayout(false);
            ((ISupportInitialize)InputIcon).EndInit();
            ((ISupportInitialize)ErrorProviderText).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private static InputBoxResult Show(string prompt, string title, bool isPassword, string defaultResponse, InputBoxIcon icon, InputBoxValidatingHandler validator, int xPos, int yPos)
        {
            using var form = new InputBox(prompt, title, defaultResponse, isPassword, icon);

            switch (xPos)
            {
                case >= 0 when yPos >= 0:
                    form.StartPosition = FormStartPosition.Manual;
                    form.Left = xPos;
                    form.Top = yPos;
                    break;
                
                case -1 when yPos == -1:
                    form.StartPosition = FormStartPosition.CenterParent;
                    break;
            }

            form.Validator = validator;
            var result = form.ShowDialog();
            var returnVal = new InputBoxResult();
            if (result != DialogResult.OK)
            {
                return returnVal;
            }

            returnVal.Text = form.InputTextBox.Text;
            returnVal.OK = true;
            
            return returnVal;
        }

        public static InputBoxResult Show(string prompt, string title, bool isPassword, string defaultText, InputBoxIcon icon, InputBoxValidatingHandler validator)
        {
            return Show(prompt, title, isPassword, defaultText, icon, validator, -1, -1);
        }

        private void InputTextBox_TextChanged(object sender, EventArgs e)
        {
            ErrorProviderText.SetError(InputTextBox, "");
        }

        private void InputTextBox_Validating(object sender, CancelEventArgs e)
        {
            if (Validator == null)
            {
                return;
            }

            if (ButtonCancel.Focused)
            {
                return;
            }

            var args = new InputBoxValidatingArgs {Text = InputTextBox.Text};
            Validator(this, args);
            if (!args.Cancel)
            {
                return;
            }

            e.Cancel = true;
            ErrorProviderText.SetError(InputTextBox, args.Message);
        }

        private void IconButton1_Click(object sender, EventArgs e)
        {
            InputTextBox.UseSystemPasswordChar = InputTextBox.UseSystemPasswordChar switch
            {
                false => true,
                true => false
            };
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ButtonOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
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
}