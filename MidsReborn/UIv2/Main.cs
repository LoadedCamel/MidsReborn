using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Forms;
using Mids_Reborn.Forms.Controls;
using Mids_Reborn.UIv2.v2Controls;

namespace Mids_Reborn.UIv2
{
    public partial class Main : Form
    {
        #region Form Composting & Api Overrides

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

        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hWnd, WindowAttribute attr, int[] attrValue, int attrSize);

        public enum WindowAttribute : int
        {
            BorderColor = 34,
            CaptionColor = 35,
            TextColor = 36,
            BorderThickness = 37
        }

        private static string GetRgb(Color color)
        {
            return $"{color.B:X2}{color.G:X2}{color.R:X2}";
        }

        private void StylizeWindow(IntPtr handle, Color borderColor, Color? captionColor = null, Color? textColor = null)
        {
            var border = new int[] { int.Parse(GetRgb(borderColor), NumberStyles.HexNumber) };
            _ = DwmSetWindowAttribute(handle, WindowAttribute.BorderColor, border, 4);

            if (captionColor != null)
            {
                var caption = new int[] { int.Parse(GetRgb((Color)captionColor), NumberStyles.HexNumber) };
                _ = DwmSetWindowAttribute(handle, WindowAttribute.CaptionColor, caption, 4);
            }

            if (textColor == null) return;
            var text = new int[] { int.Parse(GetRgb((Color)textColor), NumberStyles.HexNumber) };
            _ = DwmSetWindowAttribute(handle, WindowAttribute.TextColor, text, 4);
        }

        #endregion

        public delegate void PowerSelectedHandler(object? sender, IPower power, bool selected);
        private event PowerSelectedHandler? PowerSelected;
        private frmInitializing? _frmInitializing;


        private bool Loading { get; set; }
        private float OriginalWidth { get; set; }
        private float OriginalHeight { get; set; }
        private static bool GetPlayableClasses(Archetype a) => a.Playable;

        private List<string> ButtonImages => I9Gfx.LoadButtons().GetAwaiter().GetResult();

        public Main()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint|ControlStyles.DoubleBuffer|ControlStyles.ResizeRedraw, true);
            ConfigData.Initialize(Serializer.GetSerializer());
            StylizeWindow(Handle, Color.DodgerBlue, Color.DodgerBlue);
            Load += Main_Load;
            InitializeComponent();
            PowerSelected += OnPowerSelected;
        }

        private void Main_Load(object? sender, EventArgs e)
        {
            Loading = true;
            try
            {
                OriginalWidth = ClientRectangle.Width;
                OriginalHeight = ClientRectangle.Height;
                if (MidsContext.Config is { I9.DefaultIOLevel: 27 })
                {
                    MidsContext.Config.I9.DefaultIOLevel = 49;
                }

                using var iFrm = new frmInitializing();
                _frmInitializing = iFrm;
                _frmInitializing.Show();
                if (MidsContext.Config != null && !this.IsInDesignMode() && !MidsContext.Config.IsInitialized)
                {
                    MidsContext.Config.CheckForUpdates = false;
                    MidsContext.Config.IsInitialized = true;
                }
                MainModule.MidsController.LoadData(ref _frmInitializing, MidsContext.Config?.DataPath);
                _frmInitializing?.SetMessage("Setting up UI...");

                Show();
                _frmInitializing?.Hide();
                _frmInitializing?.Close();
                Refresh();
                Loading = false;
                PopulateBaseFields();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}\r\n\n{exception.StackTrace}");
            }
            Loading = false;
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
                case PowerList.ListPanel.ItemState.Enabled:
                    PowerSelected?.Invoke(this, item.Power, true);
                    break;
                case PowerList.ListPanel.ItemState.Selected:
                    PowerSelected?.Invoke(this, item.Power, false);
                    break;
            }
        }

        private void OnPowerSelected(object? sender, IPower power, bool e)
        {
            switch (e)
            {
                case true:
                    var powerCtrl = new PowerControl
                    {
                        CurrentImage = PowerControl.ButtonType.Hero,
                        ForeColor = Color.WhiteSmoke, 
                        Font = new Font("Microsoft Sans Serif", 16.75f, FontStyle.Bold),
                        Name = power.DisplayName,
                        Text = power.DisplayName,
                        Width = primaryFlow.ClientSize.Width - Margin.Left - Margin.Right,
                        Height = 50,
                        Margin = primaryFlow.Controls.Count < 1 ? new Padding(20, 25, 20, 10) : new Padding(20, 10, 20, 10)
                    };
                    powerCtrl.MouseEnter += OnPowerMouseEnter;
                    powerCtrl.MouseLeave += OnPowerMouseLeave;
                    if (primaryFlow.Controls.OfType<PowerControl>().Any(x => x.Name == powerCtrl.Name)) return;
                    primaryFlow.Controls.Add(powerCtrl);
                    primaryFlow.Invalidate();
                    break;
                case false:
                    powerCtrl = primaryFlow.Controls.OfType<PowerControl>().FirstOrDefault(x => x.Name == power.DisplayName);
                    if (powerCtrl != null) primaryFlow.Controls.Remove(powerCtrl);
                    break;
            }
        }

        private void PopulateBaseFields()
        {
            PopulateArchetypes();
            PopulateOrigins();
            PopulatePools();
        }

        private void PopulateArchetypes()
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
            foreach (var control in leftPanel.Controls.OfType<PowerList>().Where(x => x.DropDown.Type == PowerList.DropDownType.Pool))
            {
                selectedPool++;
                control.BindPool(selectedPool);
            }
        }

        private void AtComboOnSelectedIndexChanged(object? sender, EventArgs e)
        {
            if (atCombo.SelectedIndex < 0) return;
            primaryPowerList.BindToArchetype((Archetype)atCombo.SelectedItem);
            secondaryPowerList.BindToArchetype((Archetype)atCombo.SelectedItem);
            ancillaryPoolList.BindAncillary((Archetype)atCombo.SelectedItem);
        }

        private void PowerListOnItemHovered(object sender, PowerList.ListPanelItem? item, MouseEventArgs e)
        {
            var mousePosition = PointToClient(Cursor.Position);
            var powerList = (PowerList)sender;
            if (!powerList.Bounds.Contains(mousePosition))
            {
                popup1.Reset();
                return;
            }
            if (item != null)
            {
                popup1.Title = item.Text;
                popup1.Powerset = DatabaseAPI.Database.Powersets[item.Power.PowerSetID]?.DisplayName;
                popup1.Available = item.Power.Level;
                popup1.Data = item.Power.DescShort;
                var newLoc = new Point(mousePosition.X + 30, mousePosition.Y + 5);
                if (Bounds.Contains(newLoc))
                {
                    popup1.Location = newLoc;
                }
                else
                {
                    newLoc = new Point(mousePosition.X + 30, mousePosition.Y - 50);
                    popup1.Location = newLoc;
                }
                popup1.Invalidate();
                popup1.IsOpen = true;
            }
            else
            {
                popup1.Reset();
            }
        }

        private void PowerListOnMouseLeave(object sender, EventArgs e)
        {
            popup1.Reset();
        }

        private void OnPowerMouseEnter(object? sender, EventArgs e)
        {
            if (sender is PowerControl panel) panel.CurrentImage = PowerControl.ButtonType.HeroHover;
        }

        private void OnPowerMouseLeave(object? sender, EventArgs e)
        {
            if (sender is PowerControl panel) panel.CurrentImage = PowerControl.ButtonType.Hero;
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
