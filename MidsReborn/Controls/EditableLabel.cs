using System.Drawing;
using System.Windows.Forms;

// https://stackoverflow.com/questions/2309662/editable-label-controls
namespace Mids_Reborn.Controls
{
    public partial class EditableLabel : Form
    {
        private TextBox textBox;

        public EditableLabel()
        {
            InitializeComponent();

            textBox = new TextBox()
            {
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Name = "textBox",
                Size = new Size(40, 20),
                TabIndex = 0
            };

            textBox.KeyDown += new KeyEventHandler(OnKeyDown);

            AutoSize = true;
            ClientSize = new Size(60, 20);
            Controls.Add(textBox);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(20, 20);
            Name = "EditableLabel";
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
        }

        public override string Text
        {
            get
            {
                if (textBox == null) return string.Empty;

                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
                ResizeEditor();
            }
        }

        private void ResizeEditor()
        {
            var size = TextRenderer.MeasureText(textBox.Text, textBox.Font);
            size.Width += 20;

            Size = size;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    Close();
                    break;
                case Keys.Return:
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
            }
        }
    }
}