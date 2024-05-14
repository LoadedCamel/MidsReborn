using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Windows.Forms;
using FontAwesome.Sharp;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.UIv2.Controls;

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
        private event PowerSelectedHandler? PowerSelected;
        internal Loader? Loader;


        private bool Loading { get; set; }
        private float OriginalWidth { get; set; }
        private float OriginalHeight { get; set; }
        private static bool GetPlayableClasses(Archetype? a) => a is { Playable: true };

        private List<string?> _buttonImages = new();

        public MainForm(string[]? args = null)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw, true);
            WinApi.SetWindowBackdropType(Handle, WinApi.BackdropTypes.MainWindow);
            LoadButtonImages();
            Load += OnLoad;
            Closed += OnClosed;
            InitializeComponent();
            //PowerSelected += OnPowerSelected;
        }

        private async void LoadButtonImages()
        {
            _buttonImages = await I9Gfx.LoadButtons();
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

            //cbAT.SelectedItem = MidsContext.Character.Archetype;

            if (MidsContext.Config.Bounds.Location.IsEmpty)
            {
                // Location = new Point((Screen.PrimaryScreen.Bounds.Width - Width) / 2,
                //     (Screen.PrimaryScreen.Bounds.Height - Height) / 2);
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

            PopulateBaseFields();
        }

        private void PowerButton_PaintSlot(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            using Pen pen = new Pen(Color.White, 1);
            //Rectangle rectangle = new Rectangle(point, 20, 30, 30);
            //g.DrawEllipse(pen, rectangle);
        }

        private void PrimaryPowerListOnItemClicked(object sender, PowerList.ListPanelItem item, MouseEventArgs e)
        {
            switch (item.ItemState)
            {
                case PowerList.ItemState.Enabled:
                    PowerSelected?.Invoke(this, item.Power, true);
                    break;
                case PowerList.ItemState.Selected:
                    PowerSelected?.Invoke(this, item.Power, false);
                    break;
            }
        }

        /*private void OnPowerSelected(object? sender, IPower? power, bool e)
        {
            switch (e)
            {
                case true:
                    var powerCtrl = new PowerControl2
                    {
                        CurrentImage = PowerControl2.ButtonType.Hero,
                        ForeColor = Color.WhiteSmoke, 
                        Font = new Font("Microsoft Sans Serif", 16.75f, FontStyle.Bold),
                        Name = power?.DisplayName,
                        Text = power?.DisplayName,
                        Width = primaryFlow.ClientSize.Width - Margin.Left - Margin.Right,
                        Height = 50,
                        Margin = primaryFlow.Controls.Count < 1 ? new Padding(20, 25, 20, 10) : new Padding(20, 10, 20, 10)
                    };
                    powerCtrl.MouseEnter += OnPowerMouseEnter;
                    powerCtrl.MouseLeave += OnPowerMouseLeave;
                    if (primaryFlow.Controls.OfType<PowerControl2>().Any(x => x.Name == powerCtrl.Name)) return;
                    primaryFlow.Controls.Add(powerCtrl);
                    primaryFlow.Invalidate();
                    break;
                case false:
                    powerCtrl = primaryFlow.Controls.OfType<PowerControl2>().FirstOrDefault(x => x.Name == power.DisplayName);
                    if (powerCtrl != null) primaryFlow.Controls.Remove(powerCtrl);
                    break;
            }
        }*/

        private void PopulateBaseFields()
        {
            // PopulateArchetypes();
            // PopulateOrigins();
            // PopulatePools();
        }

        /*private void PopulateArchetypes()
        {
            var playableArchetypes = Array.FindAll(DatabaseAPI.Database.Classes, GetPlayableClasses).ToList();
            atCombo.DisplayMember = "DisplayName";
            atCombo.ValueMember = null;
            atCombo.DataSource = playableArchetypes;
        }

        private void PopulateOrigins()
        {
            var selected = (Archetype)atCombo.SelectedItem;
            originCombo.DataSource = selected.Origin;
        }

        private void PopulatePools()
        {
            var selectedPool = -1;
            foreach (var control in collapsiblePanel1.Controls.OfType<PowerList>().Where(x => x.DropDown.Type == PowerList.DropDownType.Pool))
            {
                selectedPool++;
                control.BindPool(selectedPool);
            }
        }

        private void AtCombo_SelectionChangeCommitted(object? sender, EventArgs e)
        {
            if (atCombo.SelectedIndex < 0) return;
            primaryPowerList.BindToArchetype((Archetype)atCombo.SelectedItem);
            secondaryPowerList.BindToArchetype((Archetype)atCombo.SelectedItem);
            ancillaryPowerList.BindAncillary((Archetype)atCombo.SelectedItem);
        }*/

        private void PowerListOnItemHovered(object sender, PowerList.ListPanelItem? item, MouseEventArgs e)
        {
            if (item is not null)
            {
                var mousePosition = PointToClient(Cursor.Position);
                var popupItem = new Popup.Item
                {
                    Title = item.Text,
                    Powerset = DatabaseAPI.Database.Powersets[item.Power.PowerSetID].DisplayName,
                    Available = item.Power.Level,
                    Data = item.Power.DescShort
                };
                //popup1.Show(popupItem, new Point(mousePosition.X + 30, mousePosition.Y + 5));
            }
            else
            {
                //popup1.Hide();
            }
        }

        private void PowerListOnMouseLeave(object sender, EventArgs e)
        {
            //popup1.Hide();
        }

        private void OnPowerMouseEnter(object? sender, EventArgs e)
        {
            if (sender is PowerControl2 panel) panel.CurrentImage = PowerControl2.ButtonType.HeroHover;
        }

        private void OnPowerMouseLeave(object? sender, EventArgs e)
        {
            if (sender is PowerControl2 panel) panel.CurrentImage = PowerControl2.ButtonType.Hero;
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

        /*private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            switch (checkBox1.CheckState)
            {
                case CheckState.Checked:
                    checkBox1.Text = @"Hero";
                    StylizeWindow(Handle, Color.DodgerBlue, Color.DodgerBlue, Color.Black);
                    break;
                case CheckState.Unchecked:
                    checkBox1.Text = @"Villain";
                    StylizeWindow(Handle, Color.Firebrick, Color.Firebrick, Color.GhostWhite);
                    break;
            }
        }*/
    }
}
