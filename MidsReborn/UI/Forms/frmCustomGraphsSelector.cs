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
                AvailableItems.ReplaceWith(AvailableStats.Select(s => new AvailableItem(s)), raiseReset: false);
            }

            public void SetSelectedStats(IEnumerable<CustomGraphStat.eCustomGraphStat>? stats)
            {
                SelectedStats = stats?.ToArray() ?? [];

                SelectedItems.ReplaceWith(
                    Enumerable.Range(0, SelectedStats.Count)
                        .Select(i => new SelectedItem(i, SelectedStats[i], _selectedLabelFactory)),
                    raiseReset: false
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
                public void ReplaceWith(IEnumerable<T> items, bool raiseReset = true)
                {
                    if (items is null) throw new ArgumentNullException(nameof(items));

                    RaiseListChangedEvents = false;
                    try
                    {
                        Items.Clear();
                        foreach (var item in items)
                        {
                            Items.Add(item);
                        }
                    }
                    finally
                    {
                        RaiseListChangedEvents = true;
                        if (raiseReset)
                        {
                            Reset();
                        }
                    }
                }

                public void RefreshAll() => Reset();

                private void Reset() => OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        private CustomGraphStat.eCustomGraphStat[] _availableStats = [];
        private CustomGraphStat.eCustomGraphStat[] _selectedStats = [];
        private ConfigData.CustomGraphSettings[] _selectedSettings = [];

        private int? _selectedStatSelectedItem = null;
        private StatSettings _statSettings = null!;

        private const int MaxItems = 8;

        private readonly BindingSource _availableSource = new();
        private readonly BindingSource _selectedSource = new();
        private bool _isRefreshingLists;

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
            _selectedStats = MidsContext.Config?.CustomGraphs == null
                ? []
                : MidsContext.Config.CustomGraphs.Clone() as CustomGraphStat.eCustomGraphStat[] ?? [];

            _selectedSettings = MidsContext.Config?.CustomGraphSetting == null
                ? []
                : MidsContext.Config.CustomGraphSetting.Clone() as ConfigData.CustomGraphSettings[] ?? [];

            // Keep them aligned
            if (_selectedSettings.Length != _selectedStats.Length)
            {
                Array.Resize(ref _selectedSettings, _selectedStats.Length);
                for (var i = 0; i < _selectedSettings.Length; i++)
                {
                    _selectedSettings[i] ??= new ConfigData.CustomGraphSettings();
                }
            }

            _statSettings = new StatSettings(GetSelectedStatListItem);

            _availableSource.DataSource = _statSettings.AvailableItems;
            lbAvailableStats.DataSource = _availableSource;
            lbAvailableStats.DisplayMember = nameof(StatSettings.AvailableItem.Display);
            lbAvailableStats.ValueMember = nameof(StatSettings.AvailableItem.Stat);

            _selectedSource.DataSource = _statSettings.SelectedItems;
            lbActiveStats.DataSource = _selectedSource;
            lbActiveStats.DisplayMember = nameof(StatSettings.SelectedItem.Display);
            lbActiveStats.ValueMember = nameof(StatSettings.SelectedItem.Index);

            CalcAvailableStats();
            RefreshLists();

            UpdateMoveButtons();
            UpdateSelectedStatsLabel();
        }

        private void CalcAvailableStats()
        {
            var usedUniqueValues = _selectedStats.Where(UniqueStat).ToHashSet();

            Debug.WriteLine($"Used unique values ({usedUniqueValues.Count}): {string.Join(", ", usedUniqueValues)}");

            var allStats = Enum.GetValues<CustomGraphStat.eCustomGraphStat>();
            _availableStats = allStats
                .Where(s => !usedUniqueValues.Contains(s))
                .ToArray();

            Debug.WriteLine($"Available stats: {_availableStats.Length} / {allStats.Length}");
        }

        private void RefreshLists()
        {
            _isRefreshingLists = true;

            // capture requested selection before reset
            var restoreIndex = _selectedStatSelectedItem;

            lbAvailableStats.BeginUpdate();
            lbActiveStats.BeginUpdate();

            try
            {
                // Ensure the CurrencyManager isn't trying to keep a stale selection/position.
                lbAvailableStats.SelectedIndex = -1;
                lbActiveStats.SelectedIndex = -1;

                _availableSource.Position = -1;
                _selectedSource.Position = -1;

                _availableSource.SuspendBinding();
                _selectedSource.SuspendBinding();

                // Replace list contents while binding is suspended.
                _statSettings.SetAvailableStats(_availableStats);
                _statSettings.SetSelectedStats(_selectedStats);

                // Resume first so ResetBindings runs against a stable list.
                _availableSource.ResumeBinding();
                _selectedSource.ResumeBinding();

                _availableSource.ResetBindings(false);
                _selectedSource.ResetBindings(false);

                // Restore selection while events are suppressed by _isRefreshingLists.
                if (restoreIndex is >= 0 && restoreIndex.Value < lbActiveStats.Items.Count)
                {
                    lbActiveStats.SelectedIndex = restoreIndex.Value;
                }
                else
                {
                    _selectedStatSelectedItem = null;
                }
            }
            finally
            {
                if (_availableSource.IsBindingSuspended) _availableSource.ResumeBinding();
                if (_selectedSource.IsBindingSuspended) _selectedSource.ResumeBinding();

                lbAvailableStats.EndUpdate();
                lbActiveStats.EndUpdate();

                _isRefreshingLists = false;
            }
        }


        private void btnOk_Click(object sender, EventArgs e)
        {
            MidsContext.Config.CustomGraphs = (_selectedStats ?? []).ToArray();
            MidsContext.Config.CustomGraphSetting = (_selectedSettings ?? []).ToArray();

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

            if (_selectedStats.Length >= MaxItems)
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

            _selectedStats = _selectedStats.Append(stat).ToArray();
            _selectedSettings = _selectedSettings.Append(statSettings).ToArray();

            _selectedStatSelectedItem = _selectedStats.Length - 1;

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
            if (idx < 0 || idx >= _selectedStats.Length)
            {
                return;
            }

            _selectedStats = _selectedStats.Where((_, i) => i != idx).ToArray();
            _selectedSettings = _selectedSettings.Where((_, i) => i != idx).ToArray();

            _selectedStatSelectedItem = _selectedStats.Length == 0
                ? null
                : Math.Min(idx, _selectedStats.Length - 1);

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
            if (idx <= 0 || idx >= _selectedStats.Length)
            {
                return;
            }

            Swap(ref _selectedStats, idx, idx - 1);
            Swap(ref _selectedSettings, idx, idx - 1);

            _selectedStatSelectedItem = idx - 1;

            RefreshLists();
            UpdateMoveButtons();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var idx = lbActiveStats.SelectedIndex;
            if (idx < 0 || idx >= _selectedStats.Length - 1)
            {
                return;
            }

            Swap(ref _selectedStats, idx, idx + 1);
            Swap(ref _selectedSettings, idx, idx + 1);

            _selectedStatSelectedItem = idx + 1;

            RefreshLists();
            UpdateMoveButtons();
        }

        private void lbActiveStats_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isRefreshingLists) return;

            _selectedStatSelectedItem = lbActiveStats.SelectedIndex >= 0 ? lbActiveStats.SelectedIndex : null;
            UpdateMoveButtons();
        }

        private void UpdateMoveButtons()
        {
            var idx = lbActiveStats.SelectedIndex;
            var hasSelection = idx >= 0 && idx < _selectedStats.Length;

            btnUp.Enabled = hasSelection && idx > 0;
            btnDown.Enabled = hasSelection && idx < _selectedStats.Length - 1;
            btnRemove.Enabled = hasSelection;
            btnAdd.Enabled = lbAvailableStats.SelectedIndex >= 0 && _selectedStats.Length < MaxItems;
        }

        private void UpdateSelectedStatsLabel()
        {
            label2.Text = $"Selected items ({_selectedStats.Length}/{MaxItems}):";
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
            if (i < 0 || i >= _selectedStats.Length || i >= _selectedSettings.Length)
            {
                return string.Empty;
            }

            var stat = _selectedStats[i];
            var opt = _selectedSettings[i];
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
