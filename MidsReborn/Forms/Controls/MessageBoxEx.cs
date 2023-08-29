using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class MessageBoxEx : Form
    {
        public enum MessageBoxButtons
        {
            Okay,
            OkayCancel,
            YesNo,
        }

        public enum MessageBoxIcon
        {
            Error,
            Information,
            Protected,
            Question,
            Warning
        }

        private readonly Dictionary<MessageBoxIcon, Bitmap> _icons = new()
        {
            { MessageBoxIcon.Error , Bitmap.FromHicon(SystemIcons.Error.Handle)},
            { MessageBoxIcon.Information , Bitmap.FromHicon(SystemIcons.Information.Handle)},
            { MessageBoxIcon.Protected , Bitmap.FromHicon(SystemIcons.Shield.Handle)},
            { MessageBoxIcon.Question , Bitmap.FromHicon(SystemIcons.Question.Handle)},
            { MessageBoxIcon.Warning , Bitmap.FromHicon(SystemIcons.Warning.Handle)}
        };

        private Font _font = new("Microsoft Sans Serif", 9.25f);
        public override Font Font
        {
            get => _font;
            set
            {
                _font = value;
                messageText.Font = _font;
            }
        }

        private string? _message;
        public string? Message
        {
            get => _message;
            set
            {
                _message = value;
                messageText.Text = _message;
            }
        }

        private string? _title;

        public string? Title
        {
            get => _title;
            set
            {
                _title = value;
                titleText.Text = _title;
            }
        }

        private MessageBoxButtons _buttons;
        public MessageBoxButtons Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                InitializeButtons(_buttons);
            }
        }

        private void InitializeButtons(MessageBoxButtons messageBoxButtons)
        {
            switch (messageBoxButtons)
            {
                case MessageBoxButtons.Okay:
                    btnOkay.Text = @"OK";
                    btnOkay.DialogResult = DialogResult.OK;
                    btnCancel.Visible = false;
                    btnCancel.Enabled = false;
                    break;
                case MessageBoxButtons.OkayCancel:
                    btnOkay.Text = @"OK";
                    btnOkay.DialogResult = DialogResult.OK;
                    btnCancel.Visible = true;
                    btnCancel.Enabled = true;
                    btnCancel.Text = @"Cancel";
                    btnCancel.DialogResult = DialogResult.Cancel;
                    break;
                case MessageBoxButtons.YesNo:
                    btnOkay.Text = @"Yes";
                    btnOkay.DialogResult = DialogResult.Yes;
                    btnCancel.Visible = true;
                    btnCancel.Enabled = true;
                    btnCancel.Text = @"No";
                    btnCancel.DialogResult = DialogResult.No;
                    break;
            }
        }

        private MessageBoxIcon _icon;
        public new MessageBoxIcon Icon
        {
            get => _icon;
            set
            {
                _icon = value;
                iconImage.Image = _icons[_icon];
            }
        }

        private bool _centerParent;
        public bool CenterParent
        {
            get => _centerParent;
            set
            {
                _centerParent = value;
                if (_centerParent) CenterToParent();
            }
        }

        public MessageBoxEx(string title, string message, MessageBoxButtons buttons, MessageBoxIcon icon = MessageBoxIcon.Information, bool centerParent = false)
        {
            InitializeComponent();
            Title = title;
            Message = message;
            Buttons = buttons;
            Icon = icon;
            CenterParent = centerParent;
        }

        public MessageBoxEx(string message, MessageBoxButtons buttons, MessageBoxIcon icon = MessageBoxIcon.Information, bool centerParent = false)
        {
            InitializeComponent();
            titlePanel.Visible = false;
            Message = message;
            Buttons = buttons;
            Icon = icon;
            CenterParent = centerParent;
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
