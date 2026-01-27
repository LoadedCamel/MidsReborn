using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FastDeepCloner;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.UI.Controls
{
    public partial class PetView : UserControl
    {
        public delegate void SliderUpdateHandler();
        public static SliderUpdateHandler? SliderUpdated;

        private event EventHandler? ViewColorChanged;

        private IPower? _basePower;
        private IPower? _enhancedPower;
        private int _entryIndex;
        private int _lastScaleVal;
        private bool _useAlt;

        private List<GroupedFx>? _groupedRankedEffects;
        private List<KeyValuePair<GroupedFx, PairedListEx.Item>>? _effectsItemPairs;

        private readonly Color _mainHeroColor = Color.FromArgb(12, 56, 100);
        private readonly Color _dimmedHeroColor = Color.FromArgb(30, 53, 76); // S = 60%, L = 30%
        private readonly Color _mainVillainColor = Color.FromArgb(100, 12, 12);
        private readonly Color _dimmedVillainColor = Color.FromArgb(77, 31, 31);

        private struct ItemPairGroupEx
        {
            public string Label;
            public Func<GroupedFx.FxId, bool> Filter;
            public List<KeyValuePair<GroupedFx, PairedListEx.Item>> ItemPairsEx;
        }

        public bool UseAlt
        {
            get => _useAlt;
            set
            {
                _useAlt = value;
                ViewColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public PetView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            _entryIndex = -1;
            ViewColorChanged += OnViewColorChanged;
            InitializeComponent();
        }

        private void OnViewColorChanged(object? sender, EventArgs e)
        {
            navStrip1.Theme = _useAlt ? NavStrip.ThemeColor.Villain : NavStrip.ThemeColor.Hero;
            var backColor = _useAlt ? _mainVillainColor : _mainHeroColor;
            var separatorColor = _useAlt ? _dimmedVillainColor : _dimmedHeroColor;

            BackColor = backColor;
            PerformActionOnMatchingPanels(this, separatorColor);
            Invalidate();
        }

        private void PerformActionOnMatchingPanels(Control parent, Color separatorColor)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Panel panel && panel.Name.Contains("panelSeparator"))
                {
                    // Perform your desired action on the panel
                    panel.BackColor = separatorColor;
                }

                // Recursively check the children of the current control
                if (control.HasChildren)
                {
                    PerformActionOnMatchingPanels(control, separatorColor);
                }
            }
        }

        private static string CapString(string iString, int capLength)
        {
            return iString.Length >= capLength ? iString[..capLength] : iString;
        }

        private void DisplayInfo(bool noLevel = false, int iEnhLvl = -1)
        {
            if (_basePower == null) return;

            var enhancedPower = _enhancedPower?.PowerIndex == -1 ? _basePower : _enhancedPower;
            var infoTitleText = !noLevel && _basePower.Level > 0 ? $"[{_basePower.Level}] {_basePower.DisplayName}" : _basePower.DisplayName;

            if (iEnhLvl > -1)
                infoTitleText = $"{infoTitleText} (Slot Level {iEnhLvl + 1})";

            info_Title.Text = infoTitleText;
            fx_Title.Text = infoTitleText;
            info_TxtSmall.Text = _basePower.DescShort;
            info_TxtLarge.Text = _basePower.DescLong.Trim().Replace("\0", "");

            info_DataList.Clear();
            var tip1 = GetTip1(_basePower, enhancedPower);
            AddInfoDataItems(_basePower, enhancedPower, tip1);

            SetDamageTip();
        }

        private void DisplayEffects(bool noLevel = false, int iEnhLvl = -1)
        {
            if (_basePower == null)
            {
                return;
            }

            fx_Title.Text = !noLevel & (_basePower.Level > 0)
                ? $"[{_basePower.Level}] {_basePower.DisplayName}"
                : _basePower.DisplayName;

            if (iEnhLvl > -1)
            {
                var fxTitle = fx_Title;
                fxTitle.Text = $"{fxTitle.Text} (Slot Level {iEnhLvl + 1})";
            }

            PairedListEx[] pairedListArray =
            {
                fx_List1, fx_List2, fx_List3
            };

            Label[] labelArray =
            {
                fx_lblHead1, fx_lblHead2, fx_LblHead3
            };

            fx_List1.Clear();
            fx_List2.Clear();
            fx_List3.Clear();
            fx_lblHead1.Text = string.Empty;
            fx_lblHead2.Text = string.Empty;
            fx_LblHead3.Text = string.Empty;

            var itemPairGroups = new List<ItemPairGroupEx>
            {
                new()
                {
                    Label = "Defense/Resistance",
                    Filter = e => e.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Resistance,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Heal/Endurance",
                    Filter = e => e.EffectType is Enums.eEffectType.Heal or Enums.eEffectType.HitPoints
                        or Enums.eEffectType.Regeneration or Enums.eEffectType.Endurance
                        or Enums.eEffectType.EnduranceDiscount or Enums.eEffectType.Recovery
                        or Enums.eEffectType.Absorb,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Status",
                    Filter = e => e.EffectType is Enums.eEffectType.Mez or Enums.eEffectType.MezResist
                        or Enums.eEffectType.Translucency,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Buff/Debuff",
                    Filter = e => e.EffectType is Enums.eEffectType.ToHit or Enums.eEffectType.DamageBuff
                        or Enums.eEffectType.PerceptionRadius or Enums.eEffectType.StealthRadius
                        or Enums.eEffectType.StealthRadiusPlayer or Enums.eEffectType.ResEffect
                        or Enums.eEffectType.ThreatLevel or Enums.eEffectType.DropToggles
                        or Enums.eEffectType.RechargeTime or Enums.eEffectType.Enhancement,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Movement",
                    Filter = e => e.EffectType is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping
                        or Enums.eEffectType.SpeedFlying or Enums.eEffectType.JumpHeight or Enums.eEffectType.Jumppack
                        or Enums.eEffectType.Fly or Enums.eEffectType.MaxRunSpeed or Enums.eEffectType.MaxJumpSpeed
                        or Enums.eEffectType.MaxFlySpeed,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Summon",
                    Filter = e => e.EffectType is Enums.eEffectType.EntCreate,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Granted Powers",
                    Filter = e => e.EffectType == Enums.eEffectType.GrantPower,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Modify Effect",
                    Filter = e => e.EffectType == Enums.eEffectType.ModifyAttrib,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                },

                new()
                {
                    Label = "Elusivity",
                    Filter = e =>
                        (MidsContext.Config.Inc.DisablePvE) &
                        e.EffectType == Enums.eEffectType.Elusivity,
                    ItemPairsEx = new List<KeyValuePair<GroupedFx, PairedListEx.Item>>()
                }
            };

            for (var i = 0; i < itemPairGroups.Count; i++)
            {
                itemPairGroups[i] = new ItemPairGroupEx
                {
                    Label = itemPairGroups[i].Label,
                    Filter = itemPairGroups[i].Filter,
                    ItemPairsEx = GroupedFx.FilterListItemsExt(_effectsItemPairs, itemPairGroups[i].Filter)
                };
            }

            var activeItemPairGroups = itemPairGroups
                .Where(e => e.ItemPairsEx.Count > 0)
                .ToList();

            // Fill the 3 blocks once
            // If more categories left, combine titles then display along with the other first 3 groups
            for (var i = 0; i < activeItemPairGroups.Count; i++)
            {
                labelArray[i % 3].Text = labelArray[i % 3].Text.EndsWith(":")
                    ? labelArray[i % 3].Text.Replace(":", $" | {activeItemPairGroups[i].Label}:")
                    : $"{activeItemPairGroups[i].Label}:";

                foreach (var ip in activeItemPairGroups[i].ItemPairsEx)
                {
                    pairedListArray[i % 3].AddItem(ip.Value);
                    if (ip.Key.EnhancementEffect)
                    {
                        pairedListArray[i % 3].SetUnique();
                    }
                }
            }

            fx_List1.Redraw();
            fx_List2.Redraw();
            fx_List3.Redraw();
        }

        private static string GetTip1(IPower basePower, IPower? enhancedPower)
        {
            var tip1 = string.Empty;

            if (basePower.PowerType == Enums.ePowerType.Click && enhancedPower != null)
            {
                if (enhancedPower.ToggleCost > 0 && enhancedPower.RechargeTime + enhancedPower.CastTime + enhancedPower.InterruptTime > 0)
                {
                    tip1 = $"Effective end drain per second: {Utilities.FixDP(enhancedPower.ToggleCost / (enhancedPower.RechargeTime + enhancedPower.CastTime + enhancedPower.InterruptTime))}/s";
                }

                if (enhancedPower.ToggleCost > 0 && MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.Numeric)
                {
                    var damageValue = enhancedPower.FXGetDamageValue(enhancedPower == null);
                    if (damageValue > 0)
                    {
                        if (!string.IsNullOrEmpty(tip1))
                        {
                            tip1 += "\r\n";
                        }

                        tip1 = $"{tip1}Effective damage per unit of end: {Utilities.FixDP(damageValue / enhancedPower.ToggleCost)}";
                    }
                }
            }

            return tip1;
        }

        private void AddInfoDataItems(IPower basePower, IPower? enhancedPower, string tip1)
        {
            var suffix1 = basePower.PowerType != Enums.ePowerType.Toggle ? "" : "/s";
            if (enhancedPower != null)
            {
                info_DataList.AddItem(FastItem(ShortStr("End Cost", "End"), basePower.ToggleCost, enhancedPower.ToggleCost, suffix1, tip1));
                AddAccuracyItem(basePower, enhancedPower);
                info_DataList.AddItem(FastItem(ShortStr("Recharge", "Rchg"), basePower.RechargeTime, enhancedPower.RechargeTime, "s"));
                AddDurationItem(basePower, enhancedPower);
                info_DataList.AddItem(FastItem(ShortStr("Range", "Range"), basePower.Range, enhancedPower.Range, "ft"));
                AddArcOrRadiusItem(basePower, enhancedPower);
                info_DataList.AddItem(FastItem(ShortStr("Cast Time", "Cast"), enhancedPower.CastTime, basePower.CastTime, "s",
                    $"CastTime: {basePower.CastTime}s\r\nArcana CastTime: {(Math.Ceiling(enhancedPower.CastTime / 0.132f) + 1) * 0.132:####0.###}s",
                    false, true, false, false, 3));
                AddToggleOrInterruptItem(basePower, enhancedPower);
                AddEffectItems(basePower, enhancedPower);

                var rankedEffectsExt = GroupedFx.FilterListItemsExt(_effectsItemPairs, e => e.EffectType is not (Enums.eEffectType.GrantPower or Enums.eEffectType.MaxRunSpeed
                         or Enums.eEffectType.MaxFlySpeed or Enums.eEffectType.MaxJumpSpeed or Enums.eEffectType.Mez) ||
                     e is { EffectType: Enums.eEffectType.Mez, ToWho: Enums.eToWho.Self } or
                     { EffectType: Enums.eEffectType.Mez, MezType: Enums.eMez.Taunt or Enums.eMez.Teleport });

                foreach (var rex in rankedEffectsExt)
                {
                    info_DataList.AddItem(rex.Value);
                    if (rex.Key.EnhancementEffect)
                    {
                        info_DataList.SetUnique();
                    }
                }

                info_DataList.Refresh();
                SetDamageValues(basePower, enhancedPower);
            }
        }

        private void AddAccuracyItem(IPower basePower, IPower? enhancedPower)
        {
            if (basePower.HasAbsorbedEffects && basePower.PowerIndex > -1 &&
                DatabaseAPI.Database.Power[basePower.PowerIndex].EntitiesAutoHit == Enums.eEntity.None ||
                basePower.Effects.Any(t => t.RequiresToHitCheck) ||
                basePower.EntitiesAutoHit == Enums.eEntity.None && basePower.Range > 20 &&
                basePower.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt))
            {
                var accuracy1 = basePower.Accuracy;
                var accuracy2 = enhancedPower.Accuracy;
                var num2 = MidsContext.Config.ScalingToHit * basePower.Accuracy;
                var str = string.Empty;
                var suffix2 = "%";

                if (basePower.EntitiesAutoHit != Enums.eEntity.None && basePower.Effects.Any(t => t.RequiresToHitCheck))
                {
                    str = "\r\n* This power is autohit, but has an effect that requires a ToHit roll.";
                    suffix2 += "*";
                }

                var tip2 = Math.Abs(accuracy1 - accuracy2) > float.Epsilon &&
                           Math.Abs(num2 - accuracy2) > float.Epsilon
                    ? $"Accuracy multiplier without other buffs (Real Numbers style): {basePower.Accuracy + (enhancedPower.Accuracy - (double)MidsContext.Config.ScalingToHit):##0.00000}x{str}"
                    : $"Accuracy multiplier without other buffs (Real Numbers style): {basePower.AccuracyMult:##0.00}x{str}";

                info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                    MidsContext.Config.ScalingToHit * basePower.Accuracy * 100, enhancedPower.Accuracy * 100, suffix2, tip2));
            }
            else
            {
                info_DataList.AddItem(new PairedListEx.Item(string.Empty, string.Empty, false, false, false, string.Empty));
            }
        }

        private void AddDurationItem(IPower basePower, IPower? enhancedPower)
        {
            var s1 = 0f;
            var s2 = 0f;
            var durationTip = string.Empty;
            var durationEffectId = basePower.GetDurationEffectID();

            if (durationEffectId > -1 && basePower.Effects[durationEffectId].EffectType == Enums.eEffectType.Mez &&
                basePower.Effects[durationEffectId].Duration <= 9999)
            {
                s1 = basePower.Effects[durationEffectId].Duration;
                s2 = enhancedPower.Effects[durationEffectId].Duration;

                durationTip = string.Join("\r\n", enhancedPower.Effects
                    .Where(e => e.EffectType == Enums.eEffectType.Mez &&
                                e.ToWho == enhancedPower.Effects[durationEffectId].ToWho &&
                                Math.Abs(e.Duration - s2) <= 0.1 &&
                                e.PvMode == Enums.ePvX.Any ||
                                e.PvMode == Enums.ePvX.PvE && !MidsContext.Config.Inc.DisablePvE ||
                                e.PvMode == Enums.ePvX.PvP && MidsContext.Config.Inc.DisablePvE)
                    .OrderBy(e => e.PvMode)
                    .Select(e => e.BuildEffectString(false, string.Empty, false, false, false, true)));
            }

            info_DataList.AddItem(FastItem(ShortStr("Duration", "Durtn"), s1, s2, "s", durationTip));
        }

        private void AddArcOrRadiusItem(IPower basePower, IPower? enhancedPower)
        {
            info_DataList.AddItem(basePower.Arc > 0
                ? FastItem("Arc", basePower.Arc, enhancedPower.Arc, "°")
                : FastItem("Radius", basePower.Radius, enhancedPower.Radius, "ft"));
        }

        private void AddToggleOrInterruptItem(IPower basePower, IPower? enhancedPower)
        {
            info_DataList.AddItem(basePower.PowerType == Enums.ePowerType.Toggle
                ? FastItem(ShortStr("Activate", "Act"), basePower.ActivatePeriod, enhancedPower.ActivatePeriod,
                    "s", "The effects of this toggle power are applied at this interval.")
                : FastItem(ShortStr("Interrupt", "Intrpt"), enhancedPower.InterruptTime, basePower.InterruptTime,
                    "s", "After activating this power, it can be interrupted for this amount of time."));
        }

        private void AddEffectItems(IPower basePower, IPower? enhancedPower)
        {
            var durationEffectId = basePower.GetDurationEffectID();

            if (durationEffectId > -1 &&
                basePower.Effects[durationEffectId].EffectType == Enums.eEffectType.Mez &&
                basePower.Effects[durationEffectId].MezType != Enums.eMez.Taunt &&
                !(basePower.Effects[durationEffectId].MezType is Enums.eMez.Knockback or Enums.eMez.Knockup &&
                  basePower.Effects[durationEffectId].Mag < 0))
            {
                info_DataList.AddItem(new PairedListEx.Item("Effect:",
                    Enum.GetName(typeof(Enums.eMez), basePower.Effects[durationEffectId].MezType), false,
                    basePower.Effects[durationEffectId].Probability < 1,
                    basePower.Effects[durationEffectId].CanInclude(),
                    durationEffectId));

                info_DataList.AddItem(new PairedListEx.Item("Mag:",
                    $"{enhancedPower.Effects[durationEffectId].BuffedMag:####0.##}",
                    Math.Abs(basePower.Effects[durationEffectId].BuffedMag -
                             enhancedPower.Effects[durationEffectId].BuffedMag) > float.Epsilon,
                    basePower.Effects[durationEffectId].Probability < 1));
            }
        }

        private void SetDamageValues(IPower basePower, IPower? enhancedPower)
        {
            var str1 = "Damage";
            switch (MidsContext.Config.DamageMath.ReturnValue)
            {
                case ConfigData.EDamageReturn.DPS:
                    str1 += " Per Second";
                    break;
                case ConfigData.EDamageReturn.DPA:
                    str1 += " Per Animation Second";
                    break;
            }

            if (MidsContext.Config.DataDamageGraphPercentageOnly)
                str1 += " (% only)";

            var baseDamage = Math.Abs(basePower.FXGetDamageValue(basePower.PowerIndex > -1 && enhancedPower.PowerIndex > -1));
            var enhancedDamage = enhancedPower.PowerIndex == -1 ? baseDamage : Math.Abs(enhancedPower.FXGetDamageValue());

            if (basePower.NIDSubPower.Length > 0 && baseDamage == 0 && enhancedDamage == 0)
            {
                lblDmg.Text = string.Empty;
                info_Damage.nBaseVal = 0;
                info_Damage.nEnhVal = 0;
                info_Damage.nMaxEnhVal = 0;
                info_Damage.nHighEnh = 0;
                info_Damage.Text = string.Empty;
            }
            else
            {
                lblDmg.Text = $"{str1}:";

                var hasPercentDamage = enhancedPower.Effects
                    .Any(e => e.EffectType == Enums.eEffectType.Damage &&
                              e.DisplayPercentage || e.Aspect == Enums.eAspect.Str);
                var dmgMultiplier = hasPercentDamage ? MidsContext.Character.Totals.HPMax : 1;

                info_Damage.nBaseVal = Math.Max(0, baseDamage * dmgMultiplier);
                info_Damage.nEnhVal = Math.Max(0, enhancedDamage * dmgMultiplier);
                info_Damage.nMaxEnhVal = Math.Max(
                    baseDamage * dmgMultiplier * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f)),
                    enhancedDamage * dmgMultiplier);
                info_Damage.nHighEnh = Math.Max(414, enhancedDamage * dmgMultiplier);
                info_Damage.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                    ? $"{enhancedPower.FXGetDamageString(enhancedPower.PowerIndex == -1)} ({(hasPercentDamage ? $"{Utilities.FixDP(baseDamage * 100)}%" : Utilities.FixDP(baseDamage))})"
                    : basePower.FXGetDamageString(basePower.PowerIndex > -1 && enhancedPower.PowerIndex > -1);
            }

            SetPowerScaler();
        }

        private void DisplayData(bool noLevel = false, int iEnhLevel = -1)
        {
            if (!MidsContext.Config.DisableDataDamageGraph)
            {
                info_Damage.GraphType = MidsContext.Config.DataGraphType;
                info_Damage.TextAlign = Enums.eDDAlign.Center;
                info_Damage.Style = Enums.eDDStyle.TextUnderGraph;
            }
            else
            {
                info_Damage.TextAlign = Enums.eDDAlign.Center;
                info_Damage.Style = Enums.eDDStyle.Text;
            }

            if (_basePower != null && _enhancedPower != null)
            {
                if (_basePower.Effects.Length > _enhancedPower.Effects.Length)
                {
                    var swappedFx = SwapExtraEffects(_basePower.Effects, _enhancedPower.Effects);
                    _basePower.Effects = (IEffect[])swappedFx[0].Clone();
                    _enhancedPower.Effects = (IEffect[])swappedFx[1].Clone();
                }
            }

            DisplayInfo(noLevel, iEnhLevel);
            DisplayEffects(noLevel, iEnhLevel);
        }

        private List<IEffect[]> SwapExtraEffects(IEffect[] baseEffects, IEffect[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((IEffect)baseEffects[i].Clone());
            }

            var baseFxList = new List<IEffect>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((IEffect)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return new List<IEffect[]> { baseEffects, enhEffects };
        }

        private List<Enums.ShortFX[]> SwapExtraEffects(Enums.ShortFX[] baseEffects, Enums.ShortFX[] enhEffects)
        {
            var enhFxList = enhEffects.ToList();
            for (var i = enhEffects.Length; i < baseEffects.Length; i++)
            {
                enhFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            var baseFxList = new List<Enums.ShortFX>();
            for (var i = 0; i < enhEffects.Length; i++)
            {
                baseFxList.Add((Enums.ShortFX)baseEffects[i].Clone());
            }

            baseEffects = baseFxList.ToArray();
            enhEffects = enhFxList.ToArray();

            return new List<Enums.ShortFX[]> { baseEffects, enhEffects };
        }

        private void ProcessMezEffects(IPower sourcePower, ref PairedListEx iList, bool specialEffects, ref int effectsCount, bool? iAlternate = null, int startIndex = 0)
        {
            var names = Enum.GetNames<Enums.eMezShort>();
            var enhancedPower = specialEffects ? sourcePower : _enhancedPower;

            for (var tagId = startIndex; tagId < sourcePower.Effects.Length; tagId++)
            {
                if (!(sourcePower.Effects[tagId].EffectType == Enums.eEffectType.Mez &&
                      sourcePower.Effects[tagId].Probability > 0 &&
                      sourcePower.Effects[tagId].CanInclude()) || !sourcePower.Effects[tagId].PvXInclude())
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ActiveConditionals?.Count > 0 &&
                    !sourcePower.Effects[tagId].ValidateConditional())
                {
                    continue;
                }

                var str = !(sourcePower.Effects[tagId].Duration < 2 || sourcePower.PowerType == Enums.ePowerType.Auto_)
                    ? $" - {sourcePower.Effects[tagId].Duration:#0.#}s"
                    : string.Empty;

                if (sourcePower.Effects[tagId].BuffedMag > 0)
                {
                    var iAlternate2 = iAlternate ?? Math.Abs(sourcePower.Effects[tagId].Duration - enhancedPower.Effects[tagId].Duration) > float.Epsilon ||
                        !Enums.MezDurationEnhanceable(sourcePower.Effects[tagId].MezType) &&
                        Math.Abs(enhancedPower.Effects[tagId].BuffedMag - sourcePower.Effects[tagId].BuffedMag) > float.Epsilon;

                    var iValue = (sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0"
                        : $"Mag {Utilities.FixDP(enhancedPower.Effects[tagId].BuffedMag):####0.##}{str}";

                    if (enhancedPower != null)
                    {
                        var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                        var activeConditionals = sourcePower.Effects[tagId].ActiveConditionals;
                        var iItem = new PairedListEx.Item($"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, iAlternate2,
                            sourcePower.Effects[tagId].Probability < 1 || sourcePower.Effects[tagId].ValidateConditional("Active", "Combo"),
                            activeConditionals?.Count > 0, tip);
                        iList.AddItem(iItem);
                    }

                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }
                else if (sourcePower.Effects[tagId].MezType == Enums.eMez.ToggleDrop && sourcePower.Effects[tagId].Probability > 0)
                {
                    var iValue = (sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0%"
                        : $"{sourcePower.Effects[tagId].Probability * 100}%";

                    if (enhancedPower != null)
                    {
                        var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                        var activeConditionals = sourcePower.Effects[tagId].ActiveConditionals;
                        var iItem = new PairedListEx.Item($"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, false,
                            sourcePower.Effects[tagId].Probability < 1 || sourcePower.Effects[tagId].ValidateConditional("Active", "Combo"),
                            activeConditionals?.Count > 0, tip);
                        iList.AddItem(iItem);
                    }

                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }
                else
                {
                    var iAlternate2 = iAlternate ?? Math.Abs(sourcePower.Effects[tagId].Duration - enhancedPower.Effects[tagId].Duration) > float.Epsilon ||
                        !Enums.MezDurationEnhanceable(sourcePower.Effects[tagId].MezType) &&
                        Math.Abs(enhancedPower.Effects[tagId].BuffedMag - sourcePower.Effects[tagId].BuffedMag) > float.Epsilon;

                    var iValue = (enhancedPower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0"
                        : $"Mag {Utilities.FixDP(enhancedPower.Effects[tagId].BuffedMag):####0.##}{str}";

                    var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                    var iItem = new PairedListEx.Item(
                        $"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, iAlternate2,
                        sourcePower.Effects[tagId].Probability < 1,
                        sourcePower.Effects[tagId].ActiveConditionals.Count > 0, tip);
                    iList.AddItem(iItem);

                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }

                effectsCount++;
            }
        }

        private void ProcessMezResistEffects(IPower sourcePower, ref PairedListEx iList, bool specialEffects, ref int effectsCount, int startIndex = 0)
        {
            var names = Enum.GetNames<Enums.eMezShort>();
            var enhancedPower = specialEffects ? sourcePower : _enhancedPower;

            for (var tagId = startIndex; tagId < sourcePower.Effects.Length; tagId++)
            {
                if (!(sourcePower.Effects[tagId].PvMode != Enums.ePvX.PvP && !MidsContext.Config.Inc.DisablePvE ||
                      sourcePower.Effects[tagId].PvMode != Enums.ePvX.PvE && MidsContext.Config.Inc.DisablePvE) ||
                    !(sourcePower.Effects[tagId].EffectType == Enums.eEffectType.MezResist &&
                      sourcePower.Effects[tagId].Probability > 0))
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ETModifies == Enums.eEffectType.Null) continue;

                if (sourcePower.Effects[tagId].ActiveConditionals?.Count > 0 &&
                    !sourcePower.Effects[tagId].ValidateConditional())
                {
                    continue;
                }

                var str = enhancedPower.Effects[tagId].Duration >= 15
                    ? $" - {Utilities.FixDP(enhancedPower.Effects[tagId].Duration)}s"
                    : string.Empty;

                var iValue = $"{sourcePower.Effects[tagId].MagPercent:####0.##}%{str}";
                if ((sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "0%";
                }

                var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                var iItem = new PairedListEx.Item(
                    $"{CapString($"-{names[(int)sourcePower.Effects[tagId].MezType]}", 7)}:", iValue, false, false,
                    false, tip);
                iList.AddItem(iItem);

                if (sourcePower.Effects[tagId].isEnhancementEffect)
                {
                    iList.SetUnique();
                }

                effectsCount++;
            }
        }

        private static string GenerateTipFromEffect(IPower basePower, IEffect baseFx)
        {
            return string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               e.Value.EffectType == baseFx.EffectType &&
                               e.Value.DamageType == baseFx.DamageType &&
                               e.Value.MezType == baseFx.MezType &&
                               e.Value.ETModifies == baseFx.ETModifies &&
                               e.Value.ToWho == Enums.eToWho.Self &&
                               e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP) &&
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, string.Empty, false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                   + "\n\n"
                   + string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               e.Value.EffectType == baseFx.EffectType &&
                               e.Value.DamageType == baseFx.DamageType &&
                               e.Value.MezType == baseFx.MezType &&
                               e.Value.ETModifies == baseFx.ETModifies &&
                               e.Value.ToWho == Enums.eToWho.Target &&
                               e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP) &&
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, string.Empty, false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ","))).Trim();
        }

        private static string GenerateTipFromEffect(IPower basePower, Enums.ShortFX tag)
        {
            var effects = tag.Index.Select(e => basePower.Effects[e]).ToList();
            var effectTypes = effects.Select(e => e.EffectType).ToList();
            var effectDmgTypes = effects.Select(e => e.DamageType).ToList();
            var effectEtModifies = effects.Select(e => e.ETModifies).ToList();
            var effectMezTypes = effects.Select(e => e.MezType).ToList();

            return string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               effectTypes.Contains(e.Value.EffectType) &&
                               effectDmgTypes.Contains(e.Value.DamageType) &&
                               effectEtModifies.Contains(e.Value.ETModifies) &&
                               effectMezTypes.Contains(e.Value.MezType) &&
                               e.Value.ToWho == Enums.eToWho.Self &&
                               e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP) &&
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, string.Empty, false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                   + "\n\n"
                   + string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               effectTypes.Contains(e.Value.EffectType) &&
                               effectDmgTypes.Contains(e.Value.DamageType) &&
                               effectEtModifies.Contains(e.Value.ETModifies) &&
                               effectMezTypes.Contains(e.Value.MezType) &&
                               e.Value.ToWho == Enums.eToWho.Target &&
                               e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP) &&
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, string.Empty, false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ","))).Trim();
        }

        private static PairedListEx.Item FastItem(string title, float s1, float s2, string suffix, string tip)
        {
            return FastItem(title, s1, s2, suffix, false, false, false, false, tip.Trim());
        }

        private static PairedListEx.Item FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix, bool skiBasePower, bool alwaysShow, bool isChance, bool isSpecial, string tip)
        {
            var iValue = Utilities.FixDP(s2.Sum) + suffix;
            PairedListEx.Item iItem;

            if (Math.Abs(s1.Sum) < float.Epsilon && !alwaysShow)
            {
                iItem = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1.Sum) < float.Epsilon)
            {
                iItem = new PairedListEx.Item($"{title}:", string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1.Sum - s2.Sum) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        var iValue2 = $"({Utilities.FixDP(s2.Sum)}{suffix})";
                        iValue += iValue2.Replace("%", string.Empty);
                    }

                    iAlternate = true;
                }

                iItem = new PairedListEx.Item(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return iItem;
        }

        private static PairedListEx.Item FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix, bool skiBasePower, bool alwaysShow, bool isChance, bool isSpecial, Enums.ShortFX tag, IPower basePower)
        {
            var iValue = Utilities.FixDP(s2.Sum) + suffix;
            PairedListEx.Item itemPair;

            if (Math.Abs(s1.Sum) < float.Epsilon && !alwaysShow)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1.Sum) < float.Epsilon)
            {
                itemPair = new PairedListEx.Item($"{title}:", string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1.Sum - s2.Sum) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue += $" ({Utilities.FixDP(s1.Sum)})";
                    }

                    iAlternate = true;
                }

                var tip = GenerateTipFromEffect(basePower, tag).Trim();
                itemPair = new PairedListEx.Item(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        private static PairedListEx.Item FastItem(string title, float s1, float s2, string suffix, bool skiBasePower, bool alwaysShow, bool isChance, bool isSpecial, string tip)
        {
            var iValue = Utilities.FixDP(s2) + suffix;
            PairedListEx.Item itemPair;

            if (Math.Abs(s1) < float.Epsilon && !alwaysShow)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else if (Math.Abs(s1) < float.Epsilon)
            {
                itemPair = new PairedListEx.Item(title, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - s2) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : string.Empty)})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedListEx.Item(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        private static PairedListEx.Item FastItem(string title, float s1, float s2, string suffix, bool skiBasePower = false, bool alwaysShow = false, bool isChance = false, bool isSpecial = false, int tagId = -1, int maxDecimal = -1)
        {
            var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + suffix : Utilities.FixDP(s2, maxDecimal) + suffix;
            PairedListEx.Item itemPair;

            if (Math.Abs(s1) < float.Epsilon && !alwaysShow)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - s2) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : string.Empty)})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedListEx.Item(title, iValue, iAlternate, isChance, isSpecial, tagId);
            }

            return itemPair;
        }

        private static PairedListEx.Item FastItem(string title, float s1, float s2, string suffix, string tip, bool skiBasePower = false, bool alwaysShow = false, bool isChance = false, bool isSpecial = false, int maxDecimal = -1)
        {
            var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + suffix : Utilities.FixDP(s2, maxDecimal) + suffix;
            PairedListEx.Item itemPair;

            if (Math.Abs(s1) < float.Epsilon && !alwaysShow)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - s2) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue += $" ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : string.Empty)})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedListEx.Item(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        private void powerScaler_BarClick(float value)
        {
            if (_basePower == null || _entryIndex < 0)
            {
                return;
            }

            var num = (int)Math.Round(value);
            num = Math.Clamp(num, _basePower.VariableMin, _basePower.VariableMax);

            var currentBuild = MidsContext.Character.CurrentBuild;
            var powerEntry = currentBuild.Powers[_entryIndex];
            powerEntry.VariableValue = num;
            powerEntry.Power.Stacks = num;

            if (num == _lastScaleVal)
            {
                return;
            }

            SetPowerScaler();
            _lastScaleVal = num;
            SliderUpdated?.Invoke();
        }


        private void SetDamageTip()
        {
            var iTip = _enhancedPower?.GetDamageTip() ?? string.Empty;
            info_Damage.SetTip(iTip);
        }

        private void SetPowerScaler()
        {
            if (_basePower is not { VariableEnabled: true } || _entryIndex <= -1)
            {
                powerScaler.Visible = false;
                return;
            }

            var scalerValue = MidsContext.Character.CurrentBuild.Powers[_entryIndex].VariableValue;
            var variableName = string.IsNullOrEmpty(_basePower.VariableName) ? "Targets" : _basePower.VariableName;
            var tooltipText = $"Use this slider to vary the power's effect.\r\nMin: {_basePower.VariableMin}\r\nMax: {_basePower.VariableMax}";

            powerScaler.BeginUpdate();
            powerScaler.Visible = true;
            powerScaler.Clickable = true;
            powerScaler.ForcedMax = _basePower.VariableMax;
            powerScaler.Clear();
            powerScaler.AddItem($"{variableName}:|{scalerValue}", scalerValue, 0, tooltipText);
            powerScaler.EndUpdate();
        }


        public void SetData(IPower? basePower, IPower? enhancedPower, bool noLevel = false, bool locked = false, int historyIdx = -1)
        {
            if (basePower == null)
            {
                return;
            }

            _entryIndex = historyIdx;

            var basePowerData = new Power(basePower);
            var enhancedPowerData = enhancedPower == null || enhancedPower.PowerIndex == -1 ? null : new Power(enhancedPower);

            // If both powers are null or their indices are invalid, reset _basePower and _enhancedPower
            if (enhancedPowerData == null && basePowerData.PowerIndex == -1)
            {
                _basePower = null;
                _enhancedPower = null;
            }
            else
            {
                _basePower = basePowerData.PowerIndex == -1 ? basePowerData : new Power(DatabaseAPI.Database.Power[basePowerData.PowerIndex]);
                _enhancedPower = enhancedPowerData ?? new Power(basePower) { PowerIndex = -1 };
            }

            if (_basePower != null)
            {
                _basePower.ProcessExecutes();
                _basePower.AbsorbPetEffects();
            }

            _enhancedPower?.ProcessExecutes();

            _groupedRankedEffects = GroupedFx.AssembleGroupedEffects(_enhancedPower);
            _effectsItemPairs = GroupedFx.GenerateListItems(_groupedRankedEffects, _basePower, _enhancedPower, _enhancedPower?.GetRankedEffects(true).ToList() ?? throw new InvalidOperationException(), info_DataList.Font.Size);

            SetDamageTip();
            DisplayData(noLevel);
            SetPowerScaler();
        }


        private string ShortStr(string full, string brief)
        {
            return info_DataList.Font.Size <= 100f / full.Length ? full : brief;
        }

        public void SetGraphType(Enums.eDDGraph graphType, Enums.eDDStyle graphStyle)
        {
            info_Damage.GraphType = graphType;
            info_Damage.Style = graphStyle;
        }

        private void PairedList_Hover(object? sender, int index, Enums.ShortFX tag, string? tooltip)
        {
            var str1 = tag.Present
                ? BuildToolTipStringFromTag(tag)
                : string.IsNullOrWhiteSpace(tooltip) ? string.Empty : tooltip;

            infoTip.SetToolTip((Control)sender, !string.IsNullOrWhiteSpace(str1) ? str1 : string.Empty);
            if (!string.IsNullOrWhiteSpace(str1)) infoTip.Show(str1, (Control)sender);
        }

        private string BuildToolTipStringFromTag(Enums.ShortFX tag)
        {
            var str1 = string.Empty;
            var empty1 = string.Empty;
            var str2 = string.Empty;

            if (_enhancedPower != null)
            {
                IPower power = new Power(_enhancedPower);

                foreach (var t in tag.Index)
                {
                    if (t == -1 || power.Effects[t].EffectType == Enums.eEffectType.None) continue;

                    var empty2 = string.Empty;
                    var returnMask = Array.Empty<int>();
                    power.GetEffectStringGrouped(t, ref empty2, ref returnMask, false, false);
                    if (returnMask.Length <= 0) continue;

                    if (empty1 != string.Empty)
                    {
                        empty1 += "\r\n";
                    }

                    empty1 += empty2;
                    foreach (var m in returnMask)
                    {
                        power.Effects[m].EffectType = Enums.eEffectType.None;
                    }
                }

                foreach (var t in tag.Index)
                {
                    if (power.Effects[t].EffectType == Enums.eEffectType.None) continue;

                    if (empty1 != string.Empty)
                    {
                        empty1 += "\r\n";
                    }

                    empty1 += power.Effects[t].BuildEffectString();
                }

                str2 = empty1;
            }

            str1 = empty1 + str2;
            return str1;
        }

        private void PairedList_ItemOut(object sender)
        {
            infoTip.SetToolTip((Control)sender, string.Empty);
        }
    }
}
