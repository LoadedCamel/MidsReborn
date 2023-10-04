using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Utils;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Forms
{
    public partial class frmRotationHelper : Form, IMessageFilter
    {
        private readonly frmMain myParent;
        private frmTimelineColorRefTable? fTimelineColorRefTable;
        private Stopwatch Stopwatch;
        private Size NormalGraphSize;
        private float GraphZoom;

        [DllImport("user32.dll")]
        private static extern IntPtr WindowFromPoint(Point pt);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        public frmRotationHelper(frmMain parent)
        {
            myParent = parent;
            InitializeComponent();
            Application.AddMessageFilter(this);
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
            ctlCombatTimeline1.UseArcanaTime = true;
            ctlCombatTimeline1.Powers = new List<ctlCombatTimeline.BuildPowerSlot>();
            ctlCombatTimeline1.UserBoosts = new List<IPower>();
        }

        // https://stackoverflow.com/a/10006877
        public bool PreFilterMessage(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x20a;
            if (m.Msg != WM_MOUSEWHEEL)
            {
                return false;
            }

            // Find the control at screen position m.LParam
            var pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
            var hWnd = WindowFromPoint(pos);
            if (hWnd == IntPtr.Zero || hWnd == m.HWnd || FromHandle(hWnd) == null)
            {
                return false;
            }

            SendMessage(hWnd, m.Msg, m.WParam, m.LParam);

            return true;
        }

        private void frmRotationHelper_Load(object sender, EventArgs e)
        {
            SetDataSources();
            UpdateColorTheme();

            NormalGraphSize = new Size(1118, 209);

            cbViewProfile.SelectedIndex = 0;

            label4.Visible = false;
            progressBarEx1.Visible = false;

            lblRotation.Text = "";
            lblBoosts.Text = "";

            TopMost = true;
            imageButtonEx2.ToggleState = TopMost ? ImageButtonEx.States.ToggledOn : ImageButtonEx.States.ToggledOff;
            chkCastTimeReal.Checked = ctlCombatTimeline1.UseArcanaTime;

            GraphZoom = 100;
        }

        public void UpdateData()
        {
            SetDataSources();

            // Data is off-sync, reset selected powers and boosts
            ctlCombatTimeline1.Powers = new List<ctlCombatTimeline.BuildPowerSlot>();
            ctlCombatTimeline1.UserBoosts = new List<IPower>();

            UpdatePowersText();
            UpdateBoostsText();
        }

        private void SetDataSources()
        {
            // Available powers: all powers in build that are not auto, global boost, boost or toggles
            listBox1.DataSource = MidsContext.Character.CurrentBuild.Powers
                .Where(e => e is { Power: not null })
                .Where(e => e.Power.PowerType is not (Enums.ePowerType.Auto_ or Enums.ePowerType.GlobalBoost or Enums.ePowerType.Boost or Enums.ePowerType.Toggle))
                .Select(e => new KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>(
                    $"{e.Power.DisplayName} [{e.Power.GetPowerSet()?.DisplayName}]",
                    new ctlCombatTimeline.BuildPowerSlot(DatabaseAPI.GetPowerByFullName(e.Power.FullName), e.IDXPower)))
                .ToList();
            listBox1.DisplayMember = "Key";

            // Available boosts: same as before, but click-buffs only
            // May be problematic with snipes
            listBox2.DataSource = MidsContext.Character.CurrentBuild.Powers
                .Where(e => e is { Power: not null })
                .Where(e => e.Power.PowerType is not (Enums.ePowerType.Auto_ or Enums.ePowerType.GlobalBoost or Enums.ePowerType.Boost or Enums.ePowerType.Toggle) & e.Power.ClickBuff)
                .Select(e => new KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>(
                    $"{e.Power.DisplayName} [{e.Power.GetPowerSet()?.DisplayName}]",
                    new ctlCombatTimeline.BuildPowerSlot(DatabaseAPI.GetPowerByFullName(e.Power.FullName), e.IDXPower)))
                .ToList();
            listBox2.DisplayMember = "Key";
        }

        // Import from frmDPSCalc
        /*public void SetLocation()
        {
            var rectangle = MainModule.MidsController.SzFrmRecipe with {Width = 800};
            if (rectangle.Width < 1)
            {
                rectangle.Width = Width;
            }

            if (rectangle.Height < 1)
            {
                rectangle.Height = Height;
            }

            if (rectangle.Width < MinimumSize.Width)
            {
                rectangle.Width = MinimumSize.Width;
            }

            if (rectangle.Height < MinimumSize.Height)
            {
                rectangle.Height = MinimumSize.Height;
            }

            if (rectangle.X < 1)
            {
                rectangle.X = (int)Math.Round((Screen.PrimaryScreen.Bounds.Width - Width) / 2f);
            }

            if (rectangle.Y < 32)
            {
                rectangle.Y = (int)Math.Round((Screen.PrimaryScreen.Bounds.Height - Height) / 2f);
            }

            Top = rectangle.Y;
            Left = rectangle.X;
            Height = rectangle.Height;
            Width = rectangle.Width;
        }*/

        /*private void StoreLocation()
        {
            if (!MainModule.MidsController.IsAppInitialized)
            {
                return;
            }

            MainModule.MidsController.SzFrmRecipe.X = Left;
            MainModule.MidsController.SzFrmRecipe.Y = Top;
            MainModule.MidsController.SzFrmRecipe.Width = Width;
            MainModule.MidsController.SzFrmRecipe.Height = Height;
        }*/

        private float CalcArcanaCastTime(float castTime)
        {
            return (float)(Math.Ceiling(castTime / 0.132f) + 1) * 0.132f;
        }

        private void UpdateBoostsText()
        {
            lblBoosts.Text = string.Join(", ", ctlCombatTimeline1.UserBoosts.Select(e => e.DisplayName).OrderBy(e => e));
        }

        private void UpdatePowersText(bool useTimeData = false)
        {
            if (!useTimeData)
            {
                lblRotation.Text = string.Join(" > ", ctlCombatTimeline1.Powers.Select(e => e.BasePower?.DisplayName));

                return;
            }

            var txt = "";
            var prevTime = 0f;
            foreach (var p in ctlCombatTimeline1.Timeline)
            {
                if (!string.IsNullOrWhiteSpace(txt))
                {
                    txt += " > ";

                    if (p.Time - prevTime > 0.132f)
                    {
                        txt += $"[Wait: {p.Time - prevTime:#####0.##} s.] > ";
                    }
                }

                txt += $"{p.PowerSlot.EnhancedPower?.DisplayName}{(p.Time == 0 ? "" : $"({p.Time:#####0.##} s.)")}";
                prevTime = p.Time + (p.PowerSlot.EnhancedPower == null
                    ? 0
                    : ctlCombatTimeline1.UseArcanaTime ? p.PowerSlot.EnhancedPower.ArcanaCastTime : p.PowerSlot.EnhancedPower.CastTimeBase);
            }

            lblRotation.Text = txt;
        }

        private void btnAddPower_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                return;
            }

            var item = (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>)listBox1.SelectedValue;
            ctlCombatTimeline1.Powers.Add(item.Value);
            UpdatePowersText();
        }

        private void btnAddBoost_Click(object sender, EventArgs e)
        {
            var item = (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>)listBox2.SelectedValue;
            if (ctlCombatTimeline1.UserBoosts.Contains(item.Value.BasePower))
            {
                ctlCombatTimeline1.UserBoosts = ctlCombatTimeline1.UserBoosts.Where(e => !e.Equals(item.Value.BasePower)).ToList();
            }
            else
            {
                ctlCombatTimeline1.UserBoosts.Add(item.Value.BasePower);
            }

            UpdateBoostsText();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            var item = (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>)listBox1.SelectedValue;
            ctlCombatTimeline1.Powers.Add(item.Value);

            UpdatePowersText();
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            var item = (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>)listBox2.SelectedValue;
            if (ctlCombatTimeline1.UserBoosts.Contains(item.Value.BasePower))
            {
                ctlCombatTimeline1.UserBoosts = ctlCombatTimeline1.UserBoosts.Where(e => !e.Equals(item.Value.BasePower)).ToList();
            }
            else
            {
                ctlCombatTimeline1.UserBoosts.Add(item.Value.BasePower);
            }

            UpdateBoostsText();
        }

        private void btnAddAllBoosts_Click(object sender, EventArgs e)
        {
            var items = listBox2.Items.Cast<KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot>>().ToList();
            foreach (var item in items)
            {
                if (ctlCombatTimeline1.UserBoosts.Contains(item.Value.BasePower))
                {
                    continue;
                }

                ctlCombatTimeline1.UserBoosts.Add(item.Value.BasePower);
            }

            UpdateBoostsText();
        }

        private void btnClearPowers_Click(object sender, EventArgs e)
        {
            ctlCombatTimeline1.Powers = new List<ctlCombatTimeline.BuildPowerSlot>();
            lblRotation.Text = "";
        }

        private void btnClearBoosts_Click(object sender, EventArgs e)
        {
            ctlCombatTimeline1.UserBoosts = new List<IPower>();
            lblBoosts.Text = "";
        }

        private void btnCalcTimeline_Click(object sender, EventArgs e)
        {
            label4.Visible = true;
            progressBarEx1.Value = 0;
            progressBarEx1.Visible = true;
            Stopwatch = Stopwatch.StartNew();
            ctlCombatTimeline1.PlacePowers(false);
        }

        private void ctlCombatTimeline1_CalcEnhancedProgress(object sender, float value)
        {
            if (value >= 100 - float.Epsilon)
            {
                label4.Visible = false;
                progressBarEx1.Visible = false;
                Stopwatch.Stop();
                Debug.WriteLine($"Place powers on timeline: {Stopwatch.ElapsedMilliseconds} ms");
                UpdatePowersText(true);
                ctlCombatTimeline1.Invalidate();
                timelineCursorZoom1.SetData(ctlCombatTimeline1.Timeline.Select(e => e.Time).Distinct().ToList(), new Interval(ctlCombatTimeline1.MaxTime));

                return;
            }

            progressBarEx1.Value = (int)Math.Round(value);
        }

        private void cbViewProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            ctlCombatTimeline1.Profile = cbViewProfile.SelectedIndex switch
            {
                1 => ctlCombatTimeline.ViewProfileType.Healing,
                2 => ctlCombatTimeline.ViewProfileType.Survival,
                _ => ctlCombatTimeline.ViewProfileType.Damage
            };
        }

        private void imageButtonEx1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void imageButtonEx2_Click(object sender, EventArgs e)
        {
            TopMost = imageButtonEx2.ToggleState switch
            {
                ImageButtonEx.States.ToggledOn => true,
                _ => false
            };
        }

        private void btnCopyRotation_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(lblRotation.Text))
            {
                Clipboard.SetText(lblRotation.Text);
            }
        }

        private void chkCastTimeReal_CheckedChanged(object sender, EventArgs e)
        {
            ctlCombatTimeline1.UseArcanaTime = chkCastTimeReal.Checked;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var powers = new List<string> { "Hasten", "Ionize", "Aim", "Abyssal Gaze", "Dark Blast", "Gloom", "Life Drain", "Hasten", "Ionize", "Aim" };
            var boosts = new List<string> { "Aim", "Hasten", "Ionize" };

            ctlCombatTimeline1.Powers = new List<ctlCombatTimeline.BuildPowerSlot>();
            ctlCombatTimeline1.UserBoosts = new List<IPower>();

            foreach (var p in powers)
            {
                foreach (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot> item in listBox1.Items)
                {
                    if (!item.Key.StartsWith(p))
                    {
                        continue;
                    }

                    ctlCombatTimeline1.Powers.Add(item.Value);
                    break;
                }
            }

            foreach (var b in boosts)
            {
                foreach (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot> item in listBox2.Items)
                {
                    if (!item.Key.StartsWith(b))
                    {
                        continue;
                    }

                    ctlCombatTimeline1.UserBoosts.Add(item.Value.BasePower);
                    break;
                }
            }

            UpdatePowersText();
            UpdateBoostsText();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var powers = new List<string> { "Abyssal Gaze" };
            var boosts = new List<string> { "Aim", "Hasten", "Ionize" };

            ctlCombatTimeline1.Powers = new List<ctlCombatTimeline.BuildPowerSlot>();
            ctlCombatTimeline1.UserBoosts = new List<IPower>();

            foreach (var p in powers)
            {
                foreach (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot> item in listBox1.Items)
                {
                    if (!item.Key.StartsWith(p))
                    {
                        continue;
                    }

                    ctlCombatTimeline1.Powers.Add(item.Value);
                    break;
                }
            }

            foreach (var b in boosts)
            {
                foreach (KeyValuePair<string, ctlCombatTimeline.BuildPowerSlot> item in listBox2.Items)
                {
                    if (!item.Key.StartsWith(b))
                    {
                        continue;
                    }

                    ctlCombatTimeline1.UserBoosts.Add(item.Value.BasePower);
                    break;
                }
            }

            UpdatePowersText();
            UpdateBoostsText();
        }

        private void btnClearLastPower_Click(object sender, EventArgs e)
        {
            if (ctlCombatTimeline1.Powers.Count <= 0)
            {
                return;
            }

            ctlCombatTimeline1.Powers = ctlCombatTimeline1.Powers
                .Take(ctlCombatTimeline1.Powers.Count - 1)
                .ToList();

            UpdatePowersText();
        }

        private void btnColorsRef_Click(object sender, EventArgs e)
        {
            if (fTimelineColorRefTable != null)
            {
                return;
            }

            fTimelineColorRefTable = new frmTimelineColorRefTable(this);
            fTimelineColorRefTable.Show(this);
            fTimelineColorRefTable.Activate();
            fTimelineColorRefTable.Closing += fTimelineColorRefTable_Closing;
        }

        private void fTimelineColorRefTable_Closing(object? sender, EventArgs e)
        {
            if (fTimelineColorRefTable == null)
            {
                return;
            }

            fTimelineColorRefTable.Hide();
            fTimelineColorRefTable.Closing -= fTimelineColorRefTable_Closing;
            fTimelineColorRefTable.Dispose();

            fTimelineColorRefTable = null;
        }

        public void UpdateColorTheme(Enums.Alignment alignment)
        {
            SuspendLayout();

            var isVillain = alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            borderPanel2.Border.Color = isVillain ? Color.DarkRed : Color.FromArgb(16, 76, 135);
            borderPanel2.Invalidate();
            imageButtonEx1.UseAlt = isVillain;
            imageButtonEx2.UseAlt = isVillain;
            btnColorsRef.UseAlt = isVillain;

            ResumeLayout(true);
        }

        private void UpdateColorTheme()
        {
            SuspendLayout();

            var isVillain = MidsContext.Character?.Alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            borderPanel2.Border.Color = isVillain ? Color.DarkRed : Color.FromArgb(16, 76, 135);
            borderPanel2.Invalidate();
            imageButtonEx1.UseAlt = isVillain;
            imageButtonEx2.UseAlt = isVillain;
            btnColorsRef.UseAlt = isVillain;

            ResumeLayout(true);
        }

        private void timelineCursorZoom1_ViewIntervalChanged(Interval? viewInterval)
        {
            Debug.WriteLine($"Total interval: {(timelineCursorZoom1.TimelineInterval == null ? "null" : timelineCursorZoom1.TimelineInterval)}, view interval: {(viewInterval == null ? "null" : viewInterval)}");
        }
    }
}
