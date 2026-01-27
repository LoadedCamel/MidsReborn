using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Extensions;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.UI.Forms.WindowMenuItems;

namespace Mids_Reborn.UI.Forms
{
    public partial class frmCustomGraphsSelector : Form
    {
        private CustomGraphStat.eCustomGraphStat[] AvailableStats = [];
        private CustomGraphStat.eCustomGraphStat[]? SelectedStats = [];
        private ConfigData.CustomGraphSettings[]? SelectedSettings = [];

        private int? AvailableStatSelectedItem = null;
        private int? SelectedStatSelectedItem = null;

        public frmCustomGraphsSelector()
        {
            InitializeComponent();
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;
        }

        private bool UniqueStat(CustomGraphStat.eCustomGraphStat stat)
        {
            return stat is not (CustomGraphStat.eCustomGraphStat.Defense or CustomGraphStat.eCustomGraphStat.Resistance
                or CustomGraphStat.eCustomGraphStat.EnhMez or CustomGraphStat.eCustomGraphStat.StatusProtection
                or CustomGraphStat.eCustomGraphStat.StatusResistance
                or CustomGraphStat.eCustomGraphStat.DebuffResistance or CustomGraphStat.eCustomGraphStat.Elusivity);
        }

        private void frmCustomGraphsSelector_Load(object sender, EventArgs e)
        {
            lvAvailableStats.EnableDoubleBuffer();
            lvActiveStats.EnableDoubleBuffer();

            SelectedStats = MidsContext.Config?.CustomGraphs == null
                ? []
                : MidsContext.Config.CustomGraphs.Clone() as CustomGraphStat.eCustomGraphStat[];
            SelectedSettings = MidsContext.Config?.CustomGraphSetting == null
                ? []
                : MidsContext.Config.CustomGraphSetting.Clone() as ConfigData.CustomGraphSettings[];

            CalcAvailableStats();
            RefreshLvs();
        }

        private void CalcAvailableStats()
        {
            var usedUniqueValues = (SelectedStats ?? []).Where(UniqueStat);
            AvailableStats = Enum.GetValues<CustomGraphStat.eCustomGraphStat>()
                .Where(f => !usedUniqueValues.Contains(f))
                .ToArray();
        }

        private void RefreshLvs()
        {
            lvActiveStats.VirtualListSize = 0;
            lvActiveStats.VirtualListSize = (SelectedStats ?? []).Length;

            lvAvailableStats.VirtualListSize = 0;
            lvAvailableStats.VirtualListSize = AvailableStats.Length;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            MidsContext.Config.CustomGraphs = (SelectedStats ?? []).Clone() as CustomGraphStat.eCustomGraphStat[];
            MidsContext.Config.CustomGraphSetting = (SelectedSettings ?? []).Clone() as ConfigData.CustomGraphSettings[];

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lvAvailableStats.SelectedIndices.Count < 1)
            {
                return;
            }

            if (lvAvailableStats.SelectedIndices[0] < 0)
            {
                return;
            }

            if ((SelectedStats ?? []).Length >= 8)
            {
                return;
            }

            var statSettings = AvailableStats[lvAvailableStats.SelectedIndices[0]] switch
            {
                CustomGraphStat.eCustomGraphStat.EnhAccuracy => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Accuracy
                },
                CustomGraphStat.eCustomGraphStat.EnhEndurance => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Endurance
                },
                CustomGraphStat.eCustomGraphStat.EnhEnduranceDiscount => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.EnduranceDiscount
                },
                CustomGraphStat.eCustomGraphStat.EnhSpeedFlying => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.SpeedFlying
                },
                CustomGraphStat.eCustomGraphStat.EnhJumpHeight => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.JumpHeight
                },
                CustomGraphStat.eCustomGraphStat.EnhSpeedJumping => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.SpeedJumping
                },
                CustomGraphStat.eCustomGraphStat.EnhMez => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Mez
                },
                CustomGraphStat.eCustomGraphStat.EnhPerceptionRadius => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.PerceptionRadius
                },
                CustomGraphStat.eCustomGraphStat.EnhSpeedRunning => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.SpeedRunning
                },
                CustomGraphStat.eCustomGraphStat.EnhToHit => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.ToHit
                },
                CustomGraphStat.eCustomGraphStat.EnhAbsorb => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Absorb
                },
                CustomGraphStat.eCustomGraphStat.Defense => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.Defense
                },
                CustomGraphStat.eCustomGraphStat.Resistance => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Resistance
                },
                CustomGraphStat.eCustomGraphStat.Regeneration => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Regeneration,
                },
                CustomGraphStat.eCustomGraphStat.MaxHP => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.HitPoints,
                },
                CustomGraphStat.eCustomGraphStat.Absorb => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Absorb
                },
                CustomGraphStat.eCustomGraphStat.EndRec => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single
                },
                CustomGraphStat.eCustomGraphStat.EndUse => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single
                },
                CustomGraphStat.eCustomGraphStat.MaxEnd => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single
                },
                CustomGraphStat.eCustomGraphStat.SpeedRunning => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.SpeedRunning
                },
                CustomGraphStat.eCustomGraphStat.SpeedJumping => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.SpeedJumping
                },
                CustomGraphStat.eCustomGraphStat.JumpHeight => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.JumpHeight
                },
                CustomGraphStat.eCustomGraphStat.SpeedFlying => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.SpeedFlying
                },
                CustomGraphStat.eCustomGraphStat.StealthPvE => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.StealthRadius
                },
                CustomGraphStat.eCustomGraphStat.StealthPvP => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.StealthRadiusPlayer,
                },
                CustomGraphStat.eCustomGraphStat.PerceptionRadius => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.PerceptionRadius
                },
                CustomGraphStat.eCustomGraphStat.Recharge => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.RechargeTime
                },
                CustomGraphStat.eCustomGraphStat.ToHit => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.ToHit
                },
                CustomGraphStat.eCustomGraphStat.Accuracy => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Accuracy
                },
                CustomGraphStat.eCustomGraphStat.Damage => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.DamageBuff
                },
                CustomGraphStat.eCustomGraphStat.Range => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Range
                },
                CustomGraphStat.eCustomGraphStat.EndRdx => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.EnduranceDiscount
                },
                CustomGraphStat.eCustomGraphStat.Heal => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.Enhancement,
                    EffectTypeAux = Enums.eEffectType.Heal
                },
                CustomGraphStat.eCustomGraphStat.Threat => new ConfigData.CustomGraphSettings
                {
                    EffectMode = CustomGraphStat.eCustomGraphMode.Single,
                    EffectType = Enums.eEffectType.ThreatLevel
                },
                CustomGraphStat.eCustomGraphStat.StatusProtection => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.Mez
                },
                CustomGraphStat.eCustomGraphStat.StatusResistance => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.MezResist
                },
                CustomGraphStat.eCustomGraphStat.DebuffResistance => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.ResEffect
                },
                CustomGraphStat.eCustomGraphStat.Elusivity => new ConfigData.CustomGraphSettings
                {
                    EffectType = Enums.eEffectType.Elusivity
                },
                _ => new ConfigData.CustomGraphSettings()
            };

            if (AvailableStats[lvAvailableStats.SelectedIndices[0]] is CustomGraphStat.eCustomGraphStat.DebuffResistance
                or CustomGraphStat.eCustomGraphStat.Defense or CustomGraphStat.eCustomGraphStat.Elusivity
                or CustomGraphStat.eCustomGraphStat.EnhMez or CustomGraphStat.eCustomGraphStat.Resistance
                or CustomGraphStat.eCustomGraphStat.StatusProtection or CustomGraphStat.eCustomGraphStat.StatusResistance)
            {
                using var statOptions = new frmCustomGraphSettingsSelector(AvailableStats[lvAvailableStats.SelectedIndices[0]], statSettings);
                var ret = statOptions.ShowDialog(this);
                if (ret != DialogResult.OK)
                {
                    return;
                }

                statSettings = statOptions.Settings;
            }

            SelectedStats = (SelectedStats ?? [])
                .Append(AvailableStats[lvAvailableStats.SelectedIndices[0]])
                .ToArray();

            SelectedSettings = (SelectedSettings ?? [])
                .Append(statSettings)
                .ToArray();

            CalcAvailableStats();
            RefreshLvs();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvActiveStats.SelectedIndices.Count < 1)
            {
                return;
            }

            if (lvActiveStats.SelectedIndices[0] < 0)
            {
                return;
            }

            SelectedStats = (SelectedStats ?? [])
                .Select((s, i) => new KeyValuePair<int, CustomGraphStat.eCustomGraphStat>(i, s))
                .Where(f => f.Key != lvActiveStats.SelectedIndices[0])
                .Select(f => f.Value)
                .ToArray();

            SelectedSettings = (SelectedSettings ?? [])
                .Select((s, i) => new KeyValuePair<int, ConfigData.CustomGraphSettings>(i, s))
                .Where(f => f.Key != lvActiveStats.SelectedIndices[0])
                .Select(f => f.Value)
                .ToArray();

            CalcAvailableStats();
            RefreshLvs();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (lvActiveStats.SelectedIndices.Count < 1)
            {
                return;
            }

            if (lvActiveStats.SelectedIndices[0] <= 0)
            {
                return;
            }

            if (SelectedStats == null || SelectedStats.Length < 2)
            {
                return;
            }

            var selectedItem = lvActiveStats.SelectedIndices[0];

            (SelectedStats[lvActiveStats.SelectedIndices[0]], SelectedStats[lvActiveStats.SelectedIndices[0] - 1]) = (SelectedStats[lvActiveStats.SelectedIndices[0] - 1], SelectedStats[lvActiveStats.SelectedIndices[0]]);
            SelectedStatSelectedItem = selectedItem - 1;

            RefreshLvs();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (lvActiveStats.SelectedIndices.Count < 1)
            {
                return;
            }

            if (lvActiveStats.SelectedIndices[0] >= MidsContext.Config.CustomGraphs.Length)
            {
                return;
            }

            if (SelectedStats == null || SelectedStats.Length < 2)
            {
                return;
            }

            var selectedItem = lvActiveStats.SelectedIndices[0];

            (SelectedStats[lvActiveStats.SelectedIndices[0]], SelectedStats[lvActiveStats.SelectedIndices[0] + 1]) = (SelectedStats[lvActiveStats.SelectedIndices[0] + 1], SelectedStats[lvActiveStats.SelectedIndices[0]]);
            SelectedStatSelectedItem = selectedItem + 1;

            RefreshLvs();
        }

        private void lvAvailableStats_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = new ListViewItem(CustomGraphStat.Names.CustomStatNameLong(AvailableStats[e.ItemIndex]));
        }

        private void lvActiveStats_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            var stat = SelectedStats[e.ItemIndex];
            var statOptions = SelectedSettings[e.ItemIndex];

            string label;

            if (stat is CustomGraphStat.eCustomGraphStat.DebuffResistance
                or CustomGraphStat.eCustomGraphStat.Defense or CustomGraphStat.eCustomGraphStat.Elusivity
                or CustomGraphStat.eCustomGraphStat.EnhMez or CustomGraphStat.eCustomGraphStat.Resistance
                or CustomGraphStat.eCustomGraphStat.StatusProtection or CustomGraphStat.eCustomGraphStat.StatusResistance)
            {
                var statName = CustomGraphStat.Names.CustomStatNameLong(SelectedStats[e.ItemIndex]);
                var suffix = stat switch
                {
                    CustomGraphStat.eCustomGraphStat.DebuffResistance => $"{(statOptions.EffectMode == CustomGraphStat.eCustomGraphMode.Single ? $"{statOptions.EffectTypeAux}) only" : statOptions.EffectMode)}",
                    CustomGraphStat.eCustomGraphStat.Defense or CustomGraphStat.eCustomGraphStat.Resistance => $"[{(statOptions.DamageMode == CustomGraphStat.eCustomGraphMode.Single ? $"{statOptions.DamageType}) only" : statOptions.DamageMode)}",
                    CustomGraphStat.eCustomGraphStat.StatusProtection => $"{(statOptions.MezMode == CustomGraphStat.eCustomGraphMode.Single ? $"Protection to {statOptions.MezType}) only" : statOptions.MezMode)}",
                    CustomGraphStat.eCustomGraphStat.StatusResistance => $"{(statOptions.MezMode == CustomGraphStat.eCustomGraphMode.Single ? $"Resistance to {statOptions.MezType}) only" : statOptions.MezMode)}",
                    CustomGraphStat.eCustomGraphStat.Elusivity => $"{(statOptions.DamageMode == CustomGraphStat.eCustomGraphMode.Single ? $"{statOptions.EffectType}({(statOptions.DamageType == Enums.eDamage.None ? "Untyped" : statOptions.DamageType)}) only" : statOptions.DamageMode)}",
                    CustomGraphStat.eCustomGraphStat.EnhMez => $"{(statOptions.MezMode == CustomGraphStat.eCustomGraphMode.Single ? $"{statOptions.EffectType}({statOptions.MezType}) only" : statOptions.MezMode)}",
                };

                label = $"{statName} [{suffix}]";
            }
            else
            {
                label = CustomGraphStat.Names.CustomStatNameLong(SelectedStats[e.ItemIndex]);
            }

            e.Item = new ListViewItem(label)
            {
                Selected = SelectedStatSelectedItem != null && e.ItemIndex == SelectedStatSelectedItem
            };
        }

        private void lvActiveStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvActiveStats.SelectedIndices.Count <= 0)
            {
                SelectedStatSelectedItem = null;
                return;
            }

            SelectedStatSelectedItem = lvActiveStats.SelectedIndices[0];

            if (lvActiveStats.SelectedIndices[0] == 0)
            {
                btnUp.Enabled = false;
                btnDown.Enabled = true;
            }
            else if (lvActiveStats.SelectedIndices[0] >= Math.Max(0, (SelectedStats ?? []).Length - 1))
            {
                btnUp.Enabled = true;
                btnDown.Enabled = false;
            }
            else
            {
                btnUp.Enabled = true;
                btnDown.Enabled = true;
            }
        }
    }
}
