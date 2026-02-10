using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.UI.Forms.WindowMenuItems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.UI.Forms
{
    public partial class frmCustomGraphsSelector : Form
    {
        internal sealed class StatSettings(Func<int, string> selectedLabelFactory)
        {
            private readonly Func<int, string> _selectedLabelFactory = selectedLabelFactory ?? throw new ArgumentNullException(nameof(selectedLabelFactory));

            public ResettableBindingList<AvailableItem> AvailableItems { get; } = [];
            public ResettableBindingList<SelectedItem> SelectedItems { get; } = [];

            public IReadOnlyList<CustomGraphStat.eCustomGraphStat> AvailableStats { get; private set; } = [];

            public IReadOnlyList<CustomGraphStat.eCustomGraphStat> SelectedStats { get; private set; } = [];

            public void SetAvailableStats(IEnumerable<CustomGraphStat.eCustomGraphStat>? stats)
            {
                AvailableStats = stats?.ToArray() ?? [];
                AvailableItems.ReplaceWith(AvailableStats.Select(s => new AvailableItem(s)));
            }

            public void SetSelectedStats(IEnumerable<CustomGraphStat.eCustomGraphStat>? stats)
            {
                SelectedStats = stats?.ToArray() ?? [];

                SelectedItems.ReplaceWith(
                    Enumerable.Range(0, SelectedStats.Count)
                              .Select(i => new SelectedItem(i, SelectedStats[i], _selectedLabelFactory))
                );
            }

            /// <summary>
            /// Call when the text produced by GetSelectedStatListItem(i) changes
            /// (e.g., mode/damage-type changes) without changing the list content.
            /// </summary>
            public void RefreshSelectedLabels() => SelectedItems.RefreshAll();

            public sealed class AvailableItem(CustomGraphStat.eCustomGraphStat stat)
            {
                public CustomGraphStat.eCustomGraphStat Stat { get; } = stat;
                public string Display => CustomGraphStat.Names.CustomStatNameLong(Stat);

                public override string ToString() => Display;
            }

            public sealed class SelectedItem(int index, CustomGraphStat.eCustomGraphStat stat, Func<int, string> labelFactory)
            {
                private readonly Func<int, string> _labelFactory = labelFactory ?? throw new ArgumentNullException(nameof(labelFactory));

                public int Index { get; } = index;
                public CustomGraphStat.eCustomGraphStat Stat { get; } = stat;
                public string Display => _labelFactory(Index);

                public override string ToString() => Display;
            }

            public sealed class ResettableBindingList<T> : BindingList<T>
            {
                public void ReplaceWith(IEnumerable<T> items)
                {
                    if (items is null)
                    {
                        throw new ArgumentNullException(nameof(items));
                    }

                    RaiseListChangedEvents = false;
                    try
                    {
                        ClearItems();
                        foreach (var item in items)
                        {
                            Add(item);
                        }
                    }
                    finally
                    {
                        RaiseListChangedEvents = true;
                        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
                    }
                }

                public void RefreshAll() => OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        private CustomGraphStat.eCustomGraphStat[] AvailableStats = [];
        private CustomGraphStat.eCustomGraphStat[] SelectedStats = [];
        private ConfigData.CustomGraphSettings[] SelectedSettings = [];

        private int? _selectedStatSelectedItem = null;
        private StatSettings _statSettings = null!;

        private const int MaxItems = 8;

        public frmCustomGraphsSelector()
        {
            InitializeComponent();
            Icon = MRBResourceLib.Resources.MRB_Icon_Concept;

            lbAvailableStats.SelectionMode = SelectionMode.One;
            lbActiveStats.SelectionMode = SelectionMode.One;
        }

        private bool UniqueStat(CustomGraphStat.eCustomGraphStat stat)
        {
            return stat is not (CustomGraphStat.eCustomGraphStat.Defense
                or CustomGraphStat.eCustomGraphStat.Resistance
                or CustomGraphStat.eCustomGraphStat.EnhMez
                or CustomGraphStat.eCustomGraphStat.StatusProtection
                or CustomGraphStat.eCustomGraphStat.StatusResistance
                or CustomGraphStat.eCustomGraphStat.DebuffResistance
                or CustomGraphStat.eCustomGraphStat.Elusivity);
        }

        private bool NeedsOptionsDialog(CustomGraphStat.eCustomGraphStat stat)
        {
            return stat is CustomGraphStat.eCustomGraphStat.DebuffResistance
                or CustomGraphStat.eCustomGraphStat.Defense
                or CustomGraphStat.eCustomGraphStat.Elusivity
                or CustomGraphStat.eCustomGraphStat.EnhMez
                or CustomGraphStat.eCustomGraphStat.Resistance
                or CustomGraphStat.eCustomGraphStat.StatusProtection
                or CustomGraphStat.eCustomGraphStat.StatusResistance;
        }

        private void frmCustomGraphsSelector_Load(object sender, EventArgs e)
        {
            SelectedStats = MidsContext.Config?.CustomGraphs == null
                ? []
                : MidsContext.Config.CustomGraphs.Clone() as CustomGraphStat.eCustomGraphStat[] ?? [];

            SelectedSettings = MidsContext.Config?.CustomGraphSetting == null
                ? []
                : MidsContext.Config.CustomGraphSetting.Clone() as ConfigData.CustomGraphSettings[] ?? [];

            // Keep them aligned
            if (SelectedSettings.Length != SelectedStats.Length)
            {
                Array.Resize(ref SelectedSettings, SelectedStats.Length);
                for (var i = 0; i < SelectedSettings.Length; i++)
                {
                    SelectedSettings[i] ??= new ConfigData.CustomGraphSettings();
                }
            }

            _statSettings = new StatSettings(GetSelectedStatListItem);

            lbAvailableStats.DataSource = _statSettings.AvailableItems;
            lbAvailableStats.DisplayMember = nameof(StatSettings.AvailableItem.Display);
            lbAvailableStats.ValueMember = nameof(StatSettings.AvailableItem.Stat);

            lbActiveStats.DataSource = _statSettings.SelectedItems;
            lbActiveStats.DisplayMember = nameof(StatSettings.SelectedItem.Display);
            lbActiveStats.ValueMember = nameof(StatSettings.SelectedItem.Index);

            CalcAvailableStats();
            RefreshLists();

            UpdateMoveButtons();
            UpdateSelectedStatsLabel();
        }

        private void CalcAvailableStats()
        {
            var usedUniqueValues = SelectedStats.Where(UniqueStat).ToHashSet();

            Debug.WriteLine($"Used unique values ({usedUniqueValues.Count}): {string.Join(", ", usedUniqueValues)}");

            var allStats = Enum.GetValues<CustomGraphStat.eCustomGraphStat>();
            AvailableStats = allStats
                .Where(s => !usedUniqueValues.Contains(s))
                .ToArray();

            Debug.WriteLine($"Available stats: {AvailableStats.Length} / {allStats.Length}");
        }

        private void RefreshLists()
        {
            _statSettings.SetAvailableStats(AvailableStats);
            _statSettings.SetSelectedStats(SelectedStats);

            // Restore selection if possible
            if (_selectedStatSelectedItem is >= 0 && _selectedStatSelectedItem.Value < lbActiveStats.Items.Count)
            {
                lbActiveStats.SelectedIndex = _selectedStatSelectedItem.Value;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            MidsContext.Config.CustomGraphs = (SelectedStats ?? []).ToArray();
            MidsContext.Config.CustomGraphSetting = (SelectedSettings ?? []).ToArray();

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
            if (lbAvailableStats.SelectedItem is not StatSettings.AvailableItem item)
            {
                return;
            }

            if (SelectedStats.Length >= MaxItems)
            {
                return;
            }

            var stat = item.Stat;
            var statSettings = CreateDefaultSettings(stat);

            if (NeedsOptionsDialog(stat))
            {
                using var statOptions = new frmCustomGraphSettingsSelector(stat, statSettings);
                if (statOptions.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                statSettings = statOptions.Settings;
            }

            SelectedStats = SelectedStats.Append(stat).ToArray();
            SelectedSettings = SelectedSettings.Append(statSettings).ToArray();

            _selectedStatSelectedItem = SelectedStats.Length - 1;

            CalcAvailableStats();
            RefreshLists();
            UpdateMoveButtons();
            UpdateSelectedStatsLabel();
        }

        private void lbAvailableStats_DoubleClick(object sender, EventArgs e)
        {
            btnAdd_Click(sender, e);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var idx = lbActiveStats.SelectedIndex;
            if (idx < 0 || idx >= SelectedStats.Length)
            {
                return;
            }

            SelectedStats = SelectedStats.Where((_, i) => i != idx).ToArray();
            SelectedSettings = SelectedSettings.Where((_, i) => i != idx).ToArray();

            _selectedStatSelectedItem = SelectedStats.Length == 0
                ? null
                : Math.Min(idx, SelectedStats.Length - 1);

            CalcAvailableStats();
            RefreshLists();
            UpdateMoveButtons();
            UpdateSelectedStatsLabel();
        }

        private void lbActiveStats_DoubleClick(object sender, EventArgs e)
        {
            btnRemove_Click(sender, e);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            var idx = lbActiveStats.SelectedIndex;
            if (idx <= 0 || idx >= SelectedStats.Length)
            {
                return;
            }

            Swap(ref SelectedStats, idx, idx - 1);
            Swap(ref SelectedSettings, idx, idx - 1);

            _selectedStatSelectedItem = idx - 1;

            RefreshLists();
            UpdateMoveButtons();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var idx = lbActiveStats.SelectedIndex;
            if (idx < 0 || idx >= SelectedStats.Length - 1)
            {
                return;
            }

            Swap(ref SelectedStats, idx, idx + 1);
            Swap(ref SelectedSettings, idx, idx + 1);

            _selectedStatSelectedItem = idx + 1;

            RefreshLists();
            UpdateMoveButtons();
        }

        private void lbActiveStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            _selectedStatSelectedItem = lbActiveStats.SelectedIndex >= 0 ? lbActiveStats.SelectedIndex : null;
            UpdateMoveButtons();
        }

        private void UpdateMoveButtons()
        {
            var idx = lbActiveStats.SelectedIndex;
            var hasSelection = idx >= 0 && idx < SelectedStats.Length;

            btnUp.Enabled = hasSelection && idx > 0;
            btnDown.Enabled = hasSelection && idx < SelectedStats.Length - 1;
            btnRemove.Enabled = hasSelection;
            btnAdd.Enabled = lbAvailableStats.SelectedIndex >= 0 && SelectedStats.Length < MaxItems;
        }

        private void UpdateSelectedStatsLabel()
        {
            label2.Text = $"Selected items ({SelectedStats.Length}/{MaxItems}):";
        }

        private static void Swap<T>(ref T[] arr, int a, int b)
        {
            (arr[a], arr[b]) = (arr[b], arr[a]);
        }

        private ConfigData.CustomGraphSettings CreateDefaultSettings(CustomGraphStat.eCustomGraphStat stat)
        {
            return stat switch
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
        }

        private string GetSelectedStatListItem(int i)
        {
            if (i < 0 || i >= SelectedStats.Length || i >= SelectedSettings.Length)
            {
                return string.Empty;
            }

            var stat = SelectedStats[i];
            var opt = SelectedSettings[i];
            var statName = CustomGraphStat.Names.CustomStatNameLong(stat);

            if (!NeedsOptionsDialog(stat))
            {
                return statName;
            }

            var modeText = stat switch
            {
                CustomGraphStat.eCustomGraphStat.DebuffResistance =>
                    opt.EffectMode == CustomGraphStat.eCustomGraphMode.Single
                        ? $"{opt.EffectTypeAux} only"
                        : $"{opt.EffectMode}",

                CustomGraphStat.eCustomGraphStat.Defense or CustomGraphStat.eCustomGraphStat.Resistance =>
                    opt.DamageMode == CustomGraphStat.eCustomGraphMode.Single
                        ? $"{opt.DamageType} only"
                        : $"{opt.DamageMode}",

                CustomGraphStat.eCustomGraphStat.StatusProtection =>
                    opt.MezMode == CustomGraphStat.eCustomGraphMode.Single
                        ? $"Protection to {opt.MezType} only"
                        : $"{opt.MezMode}",

                CustomGraphStat.eCustomGraphStat.StatusResistance =>
                    opt.MezMode == CustomGraphStat.eCustomGraphMode.Single
                        ? $"Resistance to {opt.MezType} only"
                        : $"{opt.MezMode}",

                CustomGraphStat.eCustomGraphStat.EnhMez =>
                    opt.MezMode == CustomGraphStat.eCustomGraphMode.Single
                        ? $"{opt.MezType} only"
                        : $"{opt.MezMode}",

                CustomGraphStat.eCustomGraphStat.Elusivity =>
                    opt.DamageMode == CustomGraphStat.eCustomGraphMode.Single
                        ? $"{(opt.DamageType == Enums.eDamage.None ? "Untyped" : opt.DamageType)} only"
                        : $"{opt.DamageMode}",

                _ => ""
            };

            return $"{statName}{(modeText == "" ? "" : $" [{modeText}]")}";
        }
    }
}
