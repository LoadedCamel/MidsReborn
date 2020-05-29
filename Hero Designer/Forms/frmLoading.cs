
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Base;
using Base.IO_Classes;

namespace Hero_Designer
{
    public partial class frmLoading : Form, IMessager
    {
        Label Label1;
        PictureBox PictureBox1;
        Timer tmrOpacity;

        public frmLoading()
        {
            InitializeComponent();
            Text = nameof(frmLoading);
            ComponentResourceManager componentResourceManager = new ComponentResourceManager(typeof(frmLoading));
            PictureBox1.Image = (Image)componentResourceManager.GetObject("PictureBox1.Image");
            Icon = (Icon)componentResourceManager.GetObject("$this.Icon");
            Name = nameof(frmLoading);
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public void SetMessage(string text)
        {
            if (Label1.InvokeRequired)
            {
                void Action() => SetMessage(text);
                Invoke((Action) Action);
            }
            else
            {
                if (Label1.Text == text)
                    return;
                Label1.Text = text;
                Label1.Refresh();
                Refresh();
            }
        }

        static void ShowPrettyError(string title, string text, Action onLabelClick = null)
        {
            using frmLoading frm = new frmLoading {Label1 = {Text = text}, Text = title};
            if (onLabelClick != null)
            {
                frm.Label1.Font = new Font(frm.Label1.Font.Name, frm.Label1.Font.SizeInPoints, FontStyle.Underline);
                frm.Label1.Click += (sender, e) => onLabelClick();
            }
            frm.Label1.Dock = DockStyle.Fill;
            frm.Size = new Size(frm.Size.Width, frm.Size.Height + 100);
            frm.FormBorderStyle = FormBorderStyle.FixedDialog;
            frm.ShowDialog();
        }

        public static void ShowLinkDialog(string title, string text, string url)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentException("url value not found", nameof(url));
            if (!url.StartsWith("http")) throw new ArgumentException("url must be http, for security purposes");
            // hosts file could still cause this to be a problem, but if they have access there, security is gone anyhow
            if (!char.IsLetter(url.After("://")[0])) throw new ArgumentException("url must be named, for security purposes");
            ShowPrettyError(title, text, () => Process.Start(url));
        }

        void tmrOpacity_Tick(object sender, EventArgs e)
        {
            if (Opacity < 1.0)
                Opacity += 0.05;
            else
                tmrOpacity.Enabled = false;
        }
    }
}