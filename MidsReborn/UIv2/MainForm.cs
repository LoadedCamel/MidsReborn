using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms;

namespace Mids_Reborn.UIv2
{
    public partial class MainForm : Form
    {
        #region Form Composting

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style &= ~0x0002;
                cp.ExStyle &= ~0x02000000;
                cp.ExStyle &= ~0x00000020;
                return cp;
            }
        }

        #endregion

        public delegate void PowerSelectedHandler(object? sender, IPower? power, bool selected);
        internal Loader? Loader;

        public MainForm(string[]? args = null)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            WinApi.SetWindowBackdropType(Handle, WinApi.BackdropTypes.MainWindow);
            Load += OnLoad;
            Closed += OnClosed;
            InitializeComponent();
            //PowerSelected += OnPowerSelected;
        }

        private void OnClosed(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            Loader?.SetMessage(@"Initializing...");
            if (MidsContext.Config.I9.DefaultIOLevel == 27)
            {
                MidsContext.Config.I9.DefaultIOLevel = 49;
            }

            if (MidsContext.Config.Bounds.Location.IsEmpty)
            {
                Size = new Size(1342, 1001);
                CenterToScreen();
            }
            else
            {
                switch (MidsContext.Config.WindowState)
                {
                    case "Maximized":
                        WindowState = FormWindowState.Maximized;
                        DesktopBounds = MidsContext.Config.Bounds;
                        break;
                    case "Normal":
                        DesktopBounds = MidsContext.Config.Bounds;
                        break;
                    case "Minimized":
                        WindowState = FormWindowState.Normal;
                        Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2,
                            (Screen.PrimaryScreen.Bounds.Height - Height) / 2);
                        Size = new Size(1342, 1001);
                        break;
                }
            }
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Normal ? FormWindowState.Maximized : FormWindowState.Normal;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void AppButtons_MouseEnter(object sender, EventArgs e)
        {
            if (sender is not IconButton button) return;
            button.BackColor = Color.Silver;
            switch (button.Name)
            {
                case "minimizeButton":
                    minimizeButton.IconColor = Color.DodgerBlue;
                    break;
                case "maximizeButton":
                    maximizeButton.IconColor = Color.DodgerBlue;
                    break;
                case "closeButton":
                    closeButton.IconColor = Color.FromArgb(255, 30, 30);
                    break;
            }
            button.Invalidate();
        }

        private void AppButtons_MouseLeave(object sender, EventArgs e)
        {
            if (sender is not IconButton button) return;
            button.BackColor = Color.Transparent;
            button.IconColor = Color.WhiteSmoke;
        }

        private void AppMove(object sender, MouseEventArgs e)
        {
            if (e.Button is not MouseButtons.Left) return;
            WinApi.ReleaseCapture();
            _ = WinApi.SendMessage(Handle, 0xA1, 0x2, 0);
        }
    }
}
