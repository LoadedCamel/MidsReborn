using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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
        protected struct FxIdentifierShort
        {
            public Enums.eEffectType EffectType;
            public Enums.eEffectType ETModifies;
            public Enums.eDamage DamageType;
            public Enums.eToWho ToWho;
            public bool DisplayPercentage;
        }

        private readonly frmMain myParent;
        private frmTimelineColorRefTable? fTimelineColorRefTable;
        private Stopwatch Stopwatch;
        private Size NormalGraphSize;
        private float GraphZoom;

        private List<Enums.eEffectType> AllowedEffectsCursor = new()
        {
            Enums.eEffectType.Absorb,
            Enums.eEffectType.Regeneration,
            Enums.eEffectType.Recovery,
            Enums.eEffectType.HitPoints,
            Enums.eEffectType.Resistance,
            Enums.eEffectType.Defense,
            Enums.eEffectType.ToHit,
            Enums.eEffectType.Accuracy,
            Enums.eEffectType.SpeedRunning,
            Enums.eEffectType.SpeedFlying,
            Enums.eEffectType.SpeedJumping,
            Enums.eEffectType.DamageBuff,
            Enums.eEffectType.ResEffect,
            Enums.eEffectType.EnduranceDiscount,
        };

        private List<Enums.eEffectType> AllowedEnhancementEffectsCursor = new()
        {
            Enums.eEffectType.RechargeTime,
            Enums.eEffectType.Heal
        };

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
            if (MidsContext.Config.RotationHelperLocation is not null)
            {
                Location = (Point)MidsContext.Config.RotationHelperLocation;
            }

            SetDataSources();
            UpdateColorTheme();

            NormalGraphSize = new Size(1118, 209);

            cbViewProfile.SelectedIndex = 0;

            label4.Visible = false;
            progressBarEx1.Visible = false;

            lblRotation.Text = "";
            lblBoosts.Text = "";
            lblDamage.Text = "";
            lblDps.Text = "";

            TopMost = true;
            imageButtonEx2.ToggleState = TopMost ? ImageButtonEx.States.ToggledOn : ImageButtonEx.States.ToggledOff;
            chkCastTimeReal.Checked = ctlCombatTimeline1.UseArcanaTime;

            GraphZoom = 100;
        }

        private void frmRotationHelper_Move(object sender, EventArgs e)
        {
            MidsContext.Config.RotationHelperLocation = Location;
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
            if (ctlCombatTimeline1.Powers.Count <= 0)
            {
                return;
            }

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
                ibxZoomIn.Visible = !ctlCombatTimeline1.MaxZoomIn();
                ibxZoomOut.Visible = !ctlCombatTimeline1.MaxZoomOut();

                var totalDamage = ctlCombatTimeline1.GetTotalDamage();
                lblDamage.Text = $"Damage: {totalDamage:#####0.##}\r\nTotal time: {ctlCombatTimeline1.MaxTime:####0.##} s.";
                lblDps.Text = ctlCombatTimeline1.MaxTime <= float.Epsilon
                    ? "N/A"
                    : $"Estimated DPS: {totalDamage / ctlCombatTimeline1.MaxTime:####0.##} dmg/s.";

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
                3 => ctlCombatTimeline.ViewProfileType.Debuff,
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
                Clipboard.SetText($"{lblRotation.Text}\r\n\r\n{lblDamage.Text}\r\n{lblDps.Text}");
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
            ctlCombatTimeline1.SetView(viewInterval);
        }

        private void ibxZoomOut_Click(object sender, EventArgs e)
        {
            if (ctlCombatTimeline1.MaxZoomOut())
            {
                return;
            }

            ctlCombatTimeline1.ZoomOut();
            ibxZoomIn.Visible = !ctlCombatTimeline1.MaxZoomIn();
            ibxZoomOut.Visible = !ctlCombatTimeline1.MaxZoomOut();
        }

        private void ibxZoomIn_Click(object sender, EventArgs e)
        {
            if (ctlCombatTimeline1.MaxZoomIn())
            {
                return;
            }

            ctlCombatTimeline1.ZoomIn();
            ibxZoomIn.Visible = !ctlCombatTimeline1.MaxZoomIn();
            ibxZoomOut.Visible = !ctlCombatTimeline1.MaxZoomOut();
        }

        private void ctlCombatTimeline1_SetZoom(object sender, Interval? viewInterval)
        {
            timelineCursorZoom1.SetView(viewInterval);
        }

        private void timelineCursorZoom1_HoveredPosChanged(float? time, int? pixelValue)
        {
            if (time == null | pixelValue == null)
            {
                return;
            }

            var currentEffects = new Dictionary<FxIdentifierShort, float>();
            foreach (var ev in ctlCombatTimeline1.Timeline)
            {
                if (ev.PowerSlot.EnhancedPower is null & ev.PowerSlot.BasePower is null)
                {
                    continue;
                }

                var power = ev.PowerSlot.EnhancedPower ?? ev.PowerSlot.BasePower;
                // Too far in the future
                if (ev.Time > time)
                {
                    continue;
                }

                foreach (var fx in power.Effects)
                {
                    if (fx.EffectType == Enums.eEffectType.Enhancement)
                    {
                        if (!AllowedEnhancementEffectsCursor.Contains(fx.ETModifies))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (!AllowedEffectsCursor.Contains(fx.EffectType))
                        {
                            continue;
                        }
                    }

                    // Too far in the past
                    if (ev.Time + fx.Duration < time)
                    {
                        continue;
                    }

                    // Extract value/percentage from BuildEffectString() (simple mode)
                    var fxString = fx.BuildEffectString(true).Trim();
                    var m = Regex.Match(fxString, @"^([0-9\.,\-\+\%]+)");
                    var fxValueString = m.Groups[1].Value;
                    var fxValueF = float.TryParse(fxValueString
                        .Replace("+", "")
                        .Replace(",", ".")
                        .Replace("%", ""), out var fxValue);

                    fxValue = fxValueF ? fxValue : 0;
                    if (Math.Abs(fxValue) < float.Epsilon)
                    {
                        continue;
                    }

                    var fxKey = new FxIdentifierShort
                    {
                        EffectType = fx.EffectType,
                        ETModifies = fx.ETModifies,
                        DamageType = fx.DamageType,
                        ToWho = fx.ToWho,
                        DisplayPercentage = fxValueString.Contains('%')
                    };

                    if (!currentEffects.TryAdd(fxKey, fxValue))
                    {
                        currentEffects[fxKey] += fxValue;
                    }
                }
            }

            var selfEffects = string.Join("\r\n", currentEffects
                .Where(e => e.Key.ToWho == Enums.eToWho.Self)
                .Select(e => $"{e.Value:####0.##}{(e.Key.DisplayPercentage ? "%" : "")} {(e.Key.EffectType == Enums.eEffectType.Enhancement ? $"{e.Key.EffectType}({e.Key.ETModifies})" : $"{e.Key.EffectType}")}{(e.Key.DamageType != Enums.eDamage.None ? $" ({e.Key.DamageType})" : "")} to {e.Key.ToWho}"));

            var targetEffects = string.Join("\r\n", currentEffects
                .Where(e => e.Key.ToWho == Enums.eToWho.Target)
                .Select(e => $"{e.Value:####0.##}{(e.Key.DisplayPercentage ? "%" : "")} {(e.Key.EffectType == Enums.eEffectType.Enhancement ? $"{e.Key.EffectType}({e.Key.ETModifies})" : $"{e.Key.EffectType}")}{(e.Key.DamageType != Enums.eDamage.None ? $" ({e.Key.DamageType})" : "")} to {e.Key.ToWho}"));

            var tTip = $"T = {time:####0.##}s{(!string.IsNullOrWhiteSpace(selfEffects) | !string.IsNullOrWhiteSpace(targetEffects)? "\r\n" : "")}{selfEffects}{(!string.IsNullOrWhiteSpace(selfEffects) & !string.IsNullOrWhiteSpace(targetEffects) ? "\r\n============================\r\n" : "")}{targetEffects}";

            toolTip1.SetToolTip(timelineCursorZoom1 == null ? this : timelineCursorZoom1, tTip);
        }
    }
}
