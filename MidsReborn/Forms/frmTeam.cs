using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Forms
{
    public partial class FrmTeam : Form
    {
        private class TabColorScheme
        {
            public readonly Color HeroInactiveTabColor = ctlTotalsTabStrip.DefaultColors.HeroInactiveTabColor;
            public readonly Color HeroInactiveHoveredTabColor = ctlTotalsTabStrip.DefaultColors.HeroInactiveHoveredTabColor;
            public readonly Color HeroBorderColor = ctlTotalsTabStrip.DefaultColors.HeroBorderColor;
            public readonly Color HeroActiveTabColor = ctlTotalsTabStrip.DefaultColors.HeroActiveTabColor;

            public readonly Color VillainInactiveTabColor = ctlTotalsTabStrip.DefaultColors.VillainInactiveTabColor;
            public readonly Color VillainInactiveHoveredTabColor = ctlTotalsTabStrip.DefaultColors.VillainInactiveHoveredTabColor;
            public readonly Color VillainBorderColor = ctlTotalsTabStrip.DefaultColors.VillainBorderColor;
            public readonly Color VillainActiveTabColor = ctlTotalsTabStrip.DefaultColors.VillainActiveTabColor;
        }

        private readonly frmMain _myParent;
        private const int MaxMembers = 7;
        private int TotalMembers { get; set; }
        private int SelectedPage = 0;
        private readonly TabColorScheme _tabColors = new();

        public FrmTeam(frmMain iParent)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
            Load += frmTeam_OnLoad;
            InitializeComponent();
            _myParent = iParent;
        }

        private void frmTeam_OnLoad(object? sender, EventArgs e)
        {
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
            Size = new Size(603, 402);

            // Panels/tabs config
            playerPanel.Visible = false;
            playerPanel.Location = new Point(0, 0);
            playerPanel.Dock = DockStyle.Fill;

            targetPanel.Visible = false;
            targetPanel.Location = new Point(0, 0);
            targetPanel.Dock = DockStyle.Fill;

            teamPanel.Dock = DockStyle.Fill;

            // Strip config
            if (MidsContext.Character != null)
            {
                MidsContext.Character.AlignmentChanged += CharacterOnAlignmentChanged;
            }

            SetStripColors(MidsContext.Character?.Alignment);

            tabStrip.ClearItems();
            var headersText = new[]
            {
                "Team",
                "Player",
                "Target"
            };

            foreach (var h in headersText)
            {
                tabStrip.AddItem(h);
            }

            tabStrip.Invalidate();

            // Team ATs config
            var charVillain = MidsContext.Character?.Alignment is Enums.Alignment.Villain or Enums.Alignment.Rogue or Enums.Alignment.Loyalist;
            btnCancel.UseAlt = charVillain;
            btnSave.UseAlt = charVillain;

            switch (DatabaseAPI.DatabaseName)
            {
                case "Homecoming":
                    label9.Text = @"Sentinel";
                    label9.Visible = true;
                    udSentGuard.Visible = true;
                    udSentGuard.Enabled = true;
                    break;

                case "Rebirth":
                    label9.Text = @"Guardian";
                    label9.Visible = true;
                    udSentGuard.Visible = true;
                    udSentGuard.Enabled = true;
                    break;

                default:
                    label9.Visible = false;
                    udSentGuard.Visible = false;
                    udSentGuard.Enabled = false;
                    break;
            }

            if (MidsContext.Config is { TeamMembers.Count: > 0 })
            {
                foreach (var mVp in MidsContext.Config.TeamMembers)
                {
                    TotalMembers += mVp.Value;
                }

                var udControls = tableLayoutPanel1.Controls.OfType<EnhancedUpDown>();
                foreach (var udControl in udControls)
                {
                    var name = udControl.Name;
                    var archetype = string.Empty;
                    archetype = name switch
                    {
                        "udWidow" or "udSoldier" => name.Replace("ud", "Arachnos "),
                        "SentGuard" => DatabaseAPI.DatabaseName switch
                        {
                            "Homecoming" => name.Replace("udSentGuard", "Sentinel"),
                            "Rebirth" => name.Replace("udSentGuard", "Guardian"),
                            _ => archetype
                        },
                        _ => name.Replace("ud", "")
                    };

                    if (MidsContext.Config.TeamMembers.ContainsKey(archetype))
                    {
                        udControl.Value = MidsContext.Config.TeamMembers[archetype];
                    }

                    /*if (name != "udWidow" && name != "udSoldier")
                    {
                        archetype = name.Replace("ud", "");
                        if (MidsContext.Config.TeamMembers.ContainsKey(archetype))
                        {
                            udControl.Value = MidsContext.Config.TeamMembers[archetype];
                        }
                    }
                    else
                    {
                        archetype = name.Replace("ud", "Arachnos ");
                        if (MidsContext.Config.TeamMembers.ContainsKey(archetype))
                        {
                            udControl.Value = MidsContext.Config.TeamMembers[archetype];
                        }
                    }*/
                }
            }
            else
            {
                TotalMembers = 0;
            }

            tbTotalTeam.Text = Convert.ToString(TotalMembers);

            // Player config
            playerHP.BeginUpdate();
            playerHP.ForcedMax = 100;
            playerHP.Clear();
            playerHP.AddItem(
                $"HP %:|{MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent}",
                MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent, 0,
                "Use this slider to vary player HP percentage.\r\nMin: 0\r\nMax: 100");
            playerHP.EndUpdate();
            playerHP.Invalidate();

            lblPlayerHP.Text = $"{MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent:##0} %";

            playerEnd.BeginUpdate();
            playerEnd.ForcedMax = 100;
            playerEnd.Clear();
            playerEnd.AddItem(
                $"HP %:|{MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent}",
                MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent, 0,
                "Use this slider to vary player Endurance percentage.\r\nMin: 0\r\nMax: 100");
            playerEnd.EndUpdate();
            playerEnd.Invalidate();

            lblPlayerEnd.Text = $"{MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent:##0} %";

            rbPlayerStatusAlive.Checked = MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive;
            rbPlayerStatusDead.Checked = !MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive;

            // Target config
            targetHP.BeginUpdate();
            targetHP.ForcedMax = 100;
            targetHP.Clear();
            targetHP.AddItem(
                $"HP %:|{MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent}",
                MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent, 0,
                "Use this slider to vary target HP percentage.\r\nMin: 0\r\nMax: 100");
            targetHP.EndUpdate();
            targetHP.Invalidate();

            lblTargetHP.Text = $"{MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent:##0} %";

            targetEnd.BeginUpdate();
            targetEnd.ForcedMax = 100;
            targetEnd.Clear();
            targetEnd.AddItem(
                $"End %:|{MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent}",
                MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent, 0,
                "Use this slider to vary target Endurance percentage.\r\nMin: 0\r\nMax: 100");
            targetEnd.EndUpdate();
            targetEnd.Invalidate();

            lblTargetEnd.Text = $"{MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent:##0} %";
        }

        private void SetStripColors(Enums.Alignment? alignment)
        {
            switch (alignment)
            {
                case null:
                case Enums.Alignment.Hero:
                case Enums.Alignment.Vigilante:
                case Enums.Alignment.Resistance:

                    tabStrip.InactiveTabColor = _tabColors.HeroInactiveTabColor;
                    tabStrip.BackColor = _tabColors.HeroInactiveTabColor;
                    tabStrip.ActiveTabColor = _tabColors.HeroActiveTabColor;
                    tabStrip.StripLineColor = _tabColors.HeroBorderColor;
                    tabStrip.InactiveHoveredTabColor = _tabColors.HeroInactiveHoveredTabColor;

                    break;


                default:
                    tabStrip.InactiveTabColor = _tabColors.VillainInactiveTabColor;
                    tabStrip.BackColor = _tabColors.VillainInactiveTabColor;
                    tabStrip.ActiveTabColor = _tabColors.VillainActiveTabColor;
                    tabStrip.StripLineColor = _tabColors.VillainBorderColor;
                    tabStrip.InactiveHoveredTabColor = _tabColors.VillainInactiveHoveredTabColor;

                    break;
            }
        }

        private void BuildUpdate(string settingName, int val)
        {
            for (var i = 0; i < MidsContext.Character?.CurrentBuild?.Powers.Count; i++)
            {
                if (MidsContext.Character.CurrentBuild?.Powers[i] == null ||
                    MidsContext.Character.CurrentBuild?.Powers[i]?.Power == null)
                {
                    continue;
                }

                if (MidsContext.Character.CurrentBuild?.Powers[i]?.Power?.DescLong.Contains($"{{link:{settingName}}}") != true)
                {
                    continue;
                }

                MidsContext.Character.CurrentBuild.Powers[i].VariableValue = val;
                MidsContext.Character.CurrentBuild.Powers[i].Power.Stacks = val;
            }

            _myParent.RefreshInfo();
        }

        public void FeedbackUpdate(string settingName, float val)
        {
            switch (settingName.ToLowerInvariant())
            {
                case "cfg.player.hp":
                    MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = (int)Math.Round(val);

                    playerHP.BeginUpdate();
                    playerHP.ForcedMax = 100;
                    playerHP.Clear();
                    playerHP.AddItem(
                        $"HP %:|{MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent}",
                        MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent, 0,
                        "Use this slider to vary player HP percentage.\r\nMin: 0\r\nMax: 100");
                    playerHP.EndUpdate();

                    lblPlayerHP.Text = $"{MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent:##0} %";

                    break;

                case "cfg.player.end":
                    MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent = (int)Math.Round(val);
                    playerEnd.BeginUpdate();
                    playerEnd.ForcedMax = 100;
                    playerEnd.Clear();
                    playerEnd.AddItem(
                        $"HP %:|{MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent}",
                        MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent, 0,
                        "Use this slider to vary player Endurance percentage.\r\nMin: 0\r\nMax: 100");
                    playerEnd.EndUpdate();

                    lblPlayerEnd.Text = $"{MidsContext.Config.CombatContextSettings.PlayerSettings.EndPercent:##0} %";

                    break;

                case "cfg.target.hp":
                    MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent = (int)Math.Round(val);

                    targetHP.BeginUpdate();
                    targetHP.ForcedMax = 100;
                    targetHP.Clear();
                    targetHP.AddItem(
                        $"HP %:|{MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent}",
                        MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent, 0,
                        "Use this slider to vary target HP percentage.\r\nMin: 0\r\nMax: 100");
                    targetHP.EndUpdate();
                    
                    break;

                case "cfg.target.end":
                    MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent = (int)Math.Round(val);

                    targetEnd.BeginUpdate();
                    targetEnd.ForcedMax = 100;
                    targetEnd.Clear();
                    targetEnd.AddItem(
                        $"End %:|{MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent}",
                        MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent, 0,
                        "Use this slider to vary target Endurance percentage.\r\nMin: 0\r\nMax: 100");
                    targetEnd.EndUpdate();

                    break;
            }
        }

        private void CharacterOnAlignmentChanged(object? sender, Enums.Alignment e)
        {
            SetStripColors(e);
            btnToTop.UseAlt = e is Enums.Alignment.Loyalist or Enums.Alignment.Rogue or Enums.Alignment.Villain;
            btnCancel.UseAlt = e is Enums.Alignment.Loyalist or Enums.Alignment.Rogue or Enums.Alignment.Villain;
            btnSave.UseAlt = e is Enums.Alignment.Loyalist or Enums.Alignment.Rogue or Enums.Alignment.Villain;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MidsContext.Config?.SaveConfig();
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Hide();
        }

        private void OnUpClicked(object sender, EventArgs e)
        {
            var udControl = (NumericUpDown)sender;
            if (TotalMembers < MaxMembers)
            {
                udControl.Value += 1;
                TotalMembers += 1;
            }

            tbTotalTeam.Text = Convert.ToString(TotalMembers);
            UpdateMembers(udControl.Name, Convert.ToInt32(udControl.Value));
        }

        private void OnDownClicked(object sender, EventArgs e)
        {
            var udControl = (NumericUpDown)sender;
            if (TotalMembers > 0 && udControl.Value != 0)
            {
                udControl.Value -= 1;
                TotalMembers -= 1;
            }

            tbTotalTeam.Text = Convert.ToString(TotalMembers);
            UpdateMembers(udControl.Name, Convert.ToInt32(udControl.Value));
        }

        private static void UpdateMembers(string name, int value)
        {
            var dictTm = MidsContext.Config?.TeamMembers;
            var archetype = name != "udWidow" && name != "udSoldier"
                ? name.Replace("ud", "")
                : name.Replace("ud", "Arachnos ");

            if (dictTm != null && dictTm.ContainsKey(archetype))
            {
                if (value != 0)
                {
                    dictTm[archetype] = value;
                }
                else
                {
                    dictTm.Remove(archetype);
                }
            }
            else
            {
                if (value != 0)
                {
                    dictTm?.Add(archetype, value);
                }
            }
        }

        private void tabStrip_TabClick(int index)
        {
            SelectedPage = index;

            teamPanel.Visible = SelectedPage == 0;
            playerPanel.Visible = SelectedPage == 1;
            targetPanel.Visible = SelectedPage == 2;
        }

        private void rbPlayerStatusAlive_CheckedChanged(object sender, EventArgs e)
        {
            rbPlayerStatusDead.Checked = !rbPlayerStatusAlive.Checked;
            MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = rbPlayerStatusAlive.Checked;
        }

        private void rbPlayerStatusDead_CheckedChanged(object sender, EventArgs e)
        {
            rbPlayerStatusAlive.Checked = !rbPlayerStatusDead.Checked;
            MidsContext.Config.CombatContextSettings.PlayerSettings.IsAlive = !rbPlayerStatusDead.Checked;
        }

        private void playerHP_BarClick(float value)
        {
            var val = (int)Math.Round(value);
            MidsContext.Config.CombatContextSettings.PlayerSettings.HpPercent = val;
            BuildUpdate("cfg.player.hp", val);
        }

        private void targetHP_BarClick(float value)
        {
            var val = (int)Math.Round(value);
            MidsContext.Config.CombatContextSettings.TargetSettings.HpPercent = val;
            BuildUpdate("cfg.target.hp", (int)Math.Round(value));
        }

        private void targetEnd_BarClick(float value)
        {
            var val = (int)Math.Round(value);
            MidsContext.Config.CombatContextSettings.TargetSettings.EndPercent = val;
            BuildUpdate("cfg.target.end", (int)Math.Round(value));
        }
    }

    public class EnhancedUpDown : NumericUpDown
    {
        public event EventHandler? UpButtonClicked;
        public event EventHandler? DownButtonClicked;

        public override void UpButton()
        {
            UpButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        public override void DownButton()
        {
            DownButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
