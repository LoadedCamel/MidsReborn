using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class MessageBoxEx : Form
    {
        public enum MessageBoxExButtons
        {
            Ok,
            OkayCancel,
            YesNo,
        }

        public enum MessageBoxExIcon
        {
            Error,
            Information,
            Protected,
            Question,
            Warning
        }

        private readonly Dictionary<MessageBoxExIcon, Bitmap> _icons = new()
        {
            { MessageBoxExIcon.Error , Bitmap.FromHicon(SystemIcons.Error.Handle)},
            { MessageBoxExIcon.Information , Bitmap.FromHicon(SystemIcons.Information.Handle)},
            { MessageBoxExIcon.Protected , Bitmap.FromHicon(SystemIcons.Shield.Handle)},
            { MessageBoxExIcon.Question , Bitmap.FromHicon(SystemIcons.Question.Handle)},
            { MessageBoxExIcon.Warning , Bitmap.FromHicon(SystemIcons.Warning.Handle)}
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

        private MessageBoxExButtons _buttons;
        public MessageBoxExButtons Buttons
        {
            get => _buttons;
            set
            {
                _buttons = value;
                InitializeButtons(_buttons);
            }
        }

        private void InitializeButtons(MessageBoxExButtons messageBoxButtons)
        {
            switch (messageBoxButtons)
            {
                case MessageBoxExButtons.Ok:
                    btnOkay.Text = @"OK";
                    btnOkay.DialogResult = DialogResult.OK;
                    btnCancel.Visible = false;
                    btnCancel.Enabled = false;
                    break;
                case MessageBoxExButtons.OkayCancel:
                    btnOkay.Text = @"OK";
                    btnOkay.DialogResult = DialogResult.OK;
                    btnCancel.Visible = true;
                    btnCancel.Enabled = true;
                    btnCancel.Text = @"Cancel";
                    btnCancel.DialogResult = DialogResult.Cancel;
                    break;
                case MessageBoxExButtons.YesNo:
                    btnOkay.Text = @"Yes";
                    btnOkay.DialogResult = DialogResult.Yes;
                    btnCancel.Visible = true;
                    btnCancel.Enabled = true;
                    btnCancel.Text = @"No";
                    btnCancel.DialogResult = DialogResult.No;
                    break;
            }
        }

        private readonly MessageBoxExIcon _icon;
        public MessageBoxExIcon MessageBoxIcon
        {
            get => _icon;
            private init
            {
                _icon = value;
                iconImage.Image = _icons[_icon];
            }
        }

        private bool CenterParent { get; set; }

        public MessageBoxEx(string title, string message, MessageBoxExButtons buttons, MessageBoxExIcon icon = MessageBoxExIcon.Information, bool centerParent = false)
        {
            InitializeComponent();
            Title = title;
            Message = message;
            Buttons = buttons;
            MessageBoxIcon = icon;
            CenterParent = centerParent;
        }

        public MessageBoxEx(string message, MessageBoxExButtons buttons, MessageBoxExIcon icon = MessageBoxExIcon.Information, bool centerParent = false)
        {
            InitializeComponent();
            titleText.Visible = false;
            Message = message;
            Buttons = buttons;
            MessageBoxIcon = icon;
            CenterParent = centerParent;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (CenterParent && Owner is not null)
            {
                CenterToParent();
            }
            else
            {
                CenterToScreen();
            }
        }

        private void BtnOkay_Click(object sender, EventArgs e)
        {
            DialogResult = btnOkay.DialogResult;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = btnCancel.DialogResult;
            Close();
        }

        public static DialogResult ShowDialog(string message, string title, MessageBoxExButtons buttons, MessageBoxExIcon icon = MessageBoxExIcon.Information)
        {
            using var msgBox = new MessageBoxEx(title, message, buttons, icon);
            return msgBox.ShowDialog();
        }

        public static void Show(string message, string title, MessageBoxExButtons buttons, MessageBoxExIcon icon = MessageBoxExIcon.Information)
        {
            var msgBox = new MessageBoxEx(title, message, buttons, icon)
            {
                TopMost = true
            };
            msgBox.Show();
        }

        public static DialogResult ShowDialog(IWin32Window owner, string message, string title, MessageBoxExButtons buttons, MessageBoxExIcon icon = MessageBoxExIcon.Information)
        {
            using var msgBox = new MessageBoxEx(title, message, buttons, icon);
            return msgBox.ShowDialog(owner);
        }

        public static void Show(IWin32Window owner, string message, string title, MessageBoxExButtons buttons, MessageBoxExIcon icon = MessageBoxExIcon.Information)
        {
            var msgBox = new MessageBoxEx(title, message, buttons, icon)
            {
                TopMost = true
            };
            msgBox.Show(owner);
        }
    }
}
