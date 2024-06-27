using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using FastDeepCloner;

namespace Mids_Reborn.Forms.Controls
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

        private void FormPages1OnSelectedIndexChanged(object? sender, int pageIndex)
        {
            if (pageIndex >= 0 && pageIndex < formPages1.Pages.Count)
            {
                switch (pageIndex)
                {
                    case 0: // Info Page
                    {
                        break;
                    }
                    case 1: // Effects Page
                    {
                        break;
                    }
                }
            }
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

        private string GetToWhoShort(IEffect fx)
        {
            return fx.ToWho switch
            {
                Enums.eToWho.Target => " (Tgt)",
                Enums.eToWho.Self => " (Self)",
                _ => string.Empty
            };
        }

        private static bool IsMezEffect(string iStr)
        {
            return Enum.GetNames<Enums.eMez>().Any(name => string.Equals(iStr, name, StringComparison.OrdinalIgnoreCase));
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

        private int EffectsStatus(Label iLabel, PairedListEx iList)
        {
            var sFxBaseMezResist = new Enums.ShortFX();
            var sFxEnhMezResist = new Enums.ShortFX();
            var sFxBaseMez = new Enums.ShortFX();
            var sFxEnhMez = new Enums.ShortFX();
            var effectsCount = 0;

            if (_basePower == null) return effectsCount;

            sFxBaseMezResist.Assign(_basePower.GetEffectMag(Enums.eEffectType.MezResist));
            if (_enhancedPower == null) return effectsCount;

            sFxEnhMezResist.Assign(_enhancedPower.GetEffectMag(Enums.eEffectType.MezResist));
            sFxBaseMez.Assign(_basePower.GetEffectMag(Enums.eEffectType.Mez, Enums.eToWho.Unspecified, true));
            sFxEnhMez.Assign(_enhancedPower.GetEffectMag(Enums.eEffectType.Mez, Enums.eToWho.Unspecified, true));

            sFxBaseMezResist.Multiply();
            sFxEnhMezResist.Multiply();

            if (sFxBaseMezResist.Present || sFxBaseMez.Present)
            {
                iLabel.Text = _basePower.AffectsTarget(Enums.eEffectType.MezResist) ||
                              _basePower.AffectsTarget(Enums.eEffectType.Mez)
                    ? "Status Effects (Target)"
                    : "Status Effects (Self)";
            }

            if (sFxBaseMez.Present)
            {
                ProcessMezEffects(_basePower, ref iList, false, ref effectsCount);
            }

            if (sFxEnhMez.Present)
            {
                ProcessMezEffects(_enhancedPower, ref iList, false, ref effectsCount, true,
                    _basePower.Effects.Length);
            }

            if (sFxBaseMezResist.Present)
            {
                ProcessMezResistEffects(_basePower, ref iList, false, ref effectsCount);
            }

            if (sFxEnhMezResist.Present)
            {
                ProcessMezResistEffects(_enhancedPower, ref iList, false, ref effectsCount,
                    _basePower.Effects.Length);
            }

            return effectsCount;
        }

        private int EffectsSummon(Label iLabel, PairedListEx iList)
        {
            var num1 = 0;
            var flag = iList.ItemCount == 0;

            for (var index = 0; index < _basePower?.Effects.Length; index++)
            {
                if (!(_basePower.Effects[index].EffectType == Enums.eEffectType.EntCreate && _basePower.Effects[index].Probability > 0) ||
                    _basePower.AbsorbSummonEffects && _basePower.AbsorbSummonAttributes ||
                    _basePower.Effects[index].ActiveConditionals?.Count > 0 && !_basePower.Effects[index].ValidateConditional())
                {
                    continue;
                }

                if (_enhancedPower != null)
                {
                    var iValue = _enhancedPower.Effects[index].SummonedEntityName;
                    if (iValue.StartsWith("MastermindPets_"))
                    {
                        iValue = iValue.Replace("MastermindPets_", string.Empty);
                    }

                    if (iValue.StartsWith("Pets_"))
                    {
                        iValue = iValue.Replace("Pets_", string.Empty);
                    }

                    if (iValue.StartsWith("Villain_Pets_"))
                    {
                        iValue = iValue.Replace("Villain_Pets_", string.Empty);
                    }

                    var iTip = _enhancedPower.Effects[index].BuildEffectString();
                    if ((_basePower.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                    {
                        iValue = "(suppressed)";
                    }

                    var iItem = new PairedListEx.Item("Summon:", iValue, false, _basePower.Effects[index].Probability < 1.0, false, iTip, DatabaseAPI.Database.Entities[_enhancedPower.Effects[index].nSummon]);
                    iList.AddItem(iItem);
                }

                if (_basePower.Effects[index].isEnhancementEffect)
                {
                    iList.SetUnique();
                }

                num1++;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = "Summoned Entities";
            }

            return num1;
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

        private static string GenerateTipFromEffect(IPower basePower, Enums.eEffectType effectType, Enums.eDamage dmgType)
        {
            return (string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == effectType &&
                                e.Value.DamageType == dmgType &&
                                e.Value.ToWho == Enums.eToWho.Self &&
                                e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP) &&
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e => (e.Value.BuildEffectString(false, string.Empty, false, false, false, true) +
                                          basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                    + "\n\n"
                    + string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == effectType &&
                                e.Value.DamageType == dmgType &&
                                e.Value.ToWho == Enums.eToWho.Target &&
                                e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP) &&
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e => (e.Value.BuildEffectString(false, string.Empty, false, false, false, true) +
                                          basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))).Trim();
        }

        private static PairedListEx.Item FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix, Enums.ShortFX tag, IPower basePower)
        {
            return FastItem(title, s1, s2, suffix, false, false, false, false, tag, basePower);
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

        private static string ConvertNewlinesToRtf(string str)
        {
            return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", RTF.Crlf());
        }

        private static string GetEnhancementStringLongRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementStringLong();
            return !string.IsNullOrEmpty(str) ? RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic(ConvertNewlinesToRtf(str)) + RTF.Color(RTF.ElementID.Text) : str;
        }

        private static string GetEnhancementStringRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementString();
            return !string.IsNullOrEmpty(str) ? RTF.Color(RTF.ElementID.Enhancement) + ConvertNewlinesToRtf(str) + RTF.Color(RTF.ElementID.Text) : str;
        }

        private PairedListEx.Item GetRankedEffect(IReadOnlyList<int> index, int id)
        {
            var title = string.Empty;
            var shortFxBase = new Enums.ShortFX();
            var shortFxEnh = new Enums.ShortFX();
            var tag2 = new Enums.ShortFX();
            var suffix = string.Empty;
            var enhancedPower = _enhancedPower ?? _basePower;

            if (index[id] > -1)
            {
                var flag = false;
                var onlySelf = _basePower.Effects[index[id]].ToWho == Enums.eToWho.Self;
                var onlyTarget = _basePower.Effects[index[id]].ToWho == Enums.eToWho.Target;

                if (id > 0)
                {
                    flag = _basePower.Effects[index[id]].EffectType == _basePower.Effects[index[id - 1]].EffectType &&
                           _basePower.Effects[index[id]].ToWho == Enums.eToWho.Self &&
                           _basePower.Effects[index[id - 1]].ToWho == Enums.eToWho.Self &&
                           _basePower.Effects[index[id]].ToWho == Enums.eToWho.Target;
                }

                if (_basePower.Effects[index[id]].DelayedTime > 5)
                {
                    flag = true;
                }

                var names = Enum.GetNames<Enums.eEffectTypeShort>();
                if (_basePower.Effects[index[id]].EffectType == Enums.eEffectType.Enhancement)
                {
                    title = _basePower.Effects[index[id]].ETModifies switch
                    {
                        Enums.eEffectType.EnduranceDiscount => "+EndRdx",
                        Enums.eEffectType.RechargeTime => "+Rechg",
                        Enums.eEffectType.Mez => _basePower.Effects[index[id]].MezType == Enums.eMez.None
                            ? "+Effects"
                            : $"Enh({Enum.GetName(Enums.eMezShort.None.GetType(), _basePower.Effects[index[id]].MezType)})",
                        Enums.eEffectType.Defense => "Enh(Def)",
                        Enums.eEffectType.Resistance => "Enh(Res)",
                        _ => CapString(Enum.GetName(_basePower.Effects[index[id]].ETModifies.GetType(), _basePower.Effects[index[id]].ETModifies), 7)
                    };

                    shortFxBase.Assign(_basePower.GetEffectMagSum(_basePower.Effects[index[id]].EffectType,
                        _basePower.Effects[index[id]].ETModifies, _basePower.Effects[index[id]].DamageType,
                        _basePower.Effects[index[id]].MezType, false, onlySelf, onlyTarget));

                    shortFxEnh.Assign(enhancedPower.GetEffectMagSum(enhancedPower.Effects[index[id]].EffectType,
                        enhancedPower.Effects[index[id]].ETModifies, enhancedPower.Effects[index[id]].DamageType,
                        enhancedPower.Effects[index[id]].MezType, false, onlySelf, onlyTarget));
                }
                else
                {
                    title = _basePower.Effects[index[id]].EffectType != Enums.eEffectType.Mez
                        ? names[(int)_basePower.Effects[index[id]].EffectType]
                        : Enums.GetMezName((Enums.eMezShort)_basePower.Effects[index[id]].MezType);
                }

                var temp = string.Empty;
                switch (_basePower.Effects[index[id]].EffectType)
                {
                    case Enums.eEffectType.HitPoints:
                        shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf, onlyTarget));
                        tag2.Assign(shortFxBase);
                        shortFxBase.Sum = shortFxBase.Sum / (float)MidsContext.Archetype.Hitpoints * 100;
                        shortFxEnh.Sum = shortFxEnh.Sum / (float)MidsContext.Archetype.Hitpoints * 100;
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (_basePower.Effects[index[id]].BuffedMag <= 1)
                        {
                            temp = $"{_basePower.Effects[index[id]].BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", string.Empty)));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", string.Empty)));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxBase.Sum = shortFxBase.Sum / (float)MidsContext.Archetype.Hitpoints * 100;
                            shortFxEnh.Sum = shortFxEnh.Sum / (float)MidsContext.Archetype.Hitpoints * 100;
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Absorb:
                        shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf, onlyTarget));
                        var absorbPercent = _basePower.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.Absorb)
                            .Any(e => e.DisplayPercentage);
                        tag2.Assign(shortFxBase);
                        suffix = absorbPercent ? "%" : string.Empty;
                        break;
                    case Enums.eEffectType.Endurance:
                        if (_basePower.Effects[index[id]].BuffedMag < -0.01 && _basePower.Effects[index[id]].BuffedMag > -1)
                        {
                            temp = $"{_basePower.Effects[index[id]].BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", string.Empty)));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", string.Empty)));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Regeneration:
                        shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf, onlyTarget));
                        shortFxEnh.Sum *= 100;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Null:
                        if (_basePower.Effects[index[id]].BuffedMag < 1)
                        {
                            temp = $"{_basePower.Effects[index[id]].BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", string.Empty)));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", string.Empty)));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }
                        suffix = "%";
                        break;
                    case Enums.eEffectType.ToHit:
                        shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Fly:
                        shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Recovery:
                        shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf, onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Mez when _basePower.Effects[index[id]].MezType is Enums.eMez.Taunt or Enums.eMez.Placate:
                        shortFxBase.Add(index[id], _basePower.Effects[index[id]].Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Duration);
                        tag2.Assign(shortFxBase);
                        suffix = "s";
                        break;

                    // Set list of effects below that are treated as percentages
                    // Base and enhanced values will be multiplied by 100
                    case Enums.eEffectType.DamageBuff:
                    case Enums.eEffectType.Defense:
                    case Enums.eEffectType.Resistance:
                    case Enums.eEffectType.ResEffect:
                    case Enums.eEffectType.Enhancement:
                    case Enums.eEffectType.MezResist:
                    case Enums.eEffectType.RechargeTime:
                    case Enums.eEffectType.SpeedFlying:
                    case Enums.eEffectType.SpeedRunning:
                    case Enums.eEffectType.SpeedJumping:
                    case Enums.eEffectType.JumpHeight:
                    case Enums.eEffectType.PerceptionRadius:
                    case Enums.eEffectType.Meter:
                    case Enums.eEffectType.Range:
                    case Enums.eEffectType.MaxFlySpeed:
                    case Enums.eEffectType.MaxRunSpeed:
                    case Enums.eEffectType.MaxJumpSpeed:
                    case Enums.eEffectType.Jumppack:
                    case Enums.eEffectType.GlobalChanceMod:
                        if (_basePower.Effects[index[id]].EffectType != Enums.eEffectType.Enhancement)
                        {
                            shortFxBase.Add(index[id], _basePower.Effects[index[id]].BuffedMag);
                            shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        }

                        shortFxBase.Multiply();
                        shortFxEnh.Multiply();

                        tag2.Assign(enhancedPower.GetEffectMagSum(_basePower.Effects[index[id]].EffectType, false, onlySelf, onlyTarget));
                        break;
                    case Enums.eEffectType.SilentKill:
                        shortFxBase.Add(index[id], _basePower.Effects[index[id]].Absorbed_Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Absorbed_Duration);
                        tag2.Assign(shortFxBase);
                        break;
                    default:
                        shortFxBase.Add(index[id], _basePower.Effects[index[id]].BuffedMag);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        tag2.Assign(shortFxBase);
                        break;
                }

                if (_basePower.Effects[index[id]].DisplayPercentage)
                {
                    suffix = "%";
                }

                suffix += _basePower.Effects[index[id]].ToWho switch
                {
                    Enums.eToWho.Target => " (Tgt)",
                    Enums.eToWho.Self => " (Self)",
                    _ => string.Empty
                };

                if (flag)
                {
                    return FastItem(string.Empty, 0f, 0f, string.Empty);
                }
            }

            for (var sIndex = 0; sIndex < shortFxEnh.Index.Length; sIndex++)
            {
                var sFxIdx = shortFxEnh.Index[sIndex];
                if (sFxIdx >= _basePower.Effects.Length && sFxIdx >= _enhancedPower.Effects.Length)
                {
                    continue;
                }

                var effect = sFxIdx < _basePower.Effects.Length
                    ? _basePower.Effects[sFxIdx]
                    : _enhancedPower.Effects[sFxIdx];

                if (sFxIdx <= -1 || !effect.DisplayPercentage)
                {
                    continue;
                }

                if (shortFxEnh.Value[sIndex] > 1)
                {
                    continue;
                }

                switch (effect.EffectType)
                {
                    case Enums.eEffectType.Absorb:
                        shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture).Replace("%", string.Empty), CultureInfo.InvariantCulture);
                        break;
                    case Enums.eEffectType.ToHit:
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            var overage = _basePower.Effects[index[id]].Ticks * 0.05f;
                            shortFxEnh.Sum -= overage;
                            shortFxEnh.Sum /= 2;
                        }

                        break;
                    default:
                        shortFxEnh.ReSum();
                        break;
                }

                break;
            }

            var tip = shortFxEnh.Index.Length <= 0
                ? string.Empty
                : _enhancedPower.BuildTooltipStringAllVectorsEffects(_enhancedPower.Effects[shortFxEnh.Index[0]].EffectType,
                _enhancedPower.Effects[shortFxEnh.Index[0]].ETModifies, _enhancedPower.Effects[shortFxEnh.Index[0]].DamageType,
                _enhancedPower.Effects[shortFxEnh.Index[0]].MezType);

            if (_basePower.Effects[index[id]].ActiveConditionals.Count > 0)
            {
                return FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, _basePower.Effects[index[id]].Probability < 1, _basePower.Effects[index[id]].ActiveConditionals.Count > 0, tip);
            }

            if (_basePower.Effects[index[id]].SpecialCase != Enums.eSpecialCase.None)
            {
                return FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, _basePower.Effects[index[id]].Probability < 1, _basePower.Effects[index[id]].SpecialCase != Enums.eSpecialCase.None, tip);
            }

            return FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, _basePower.Effects[index[id]].Probability < 1, false, tip);
        }

        private int SelectPetPowerEntry()
        {
            if (_basePower == null) return -1;

            var t = MidsContext.Character.CurrentBuild.Powers
                .Select((e, i) => new KeyValuePair<int, PowerEntry?>(i, e))
                .Where(e => e.Value?.Power != null)
                .DefaultIfEmpty(new KeyValuePair<int, PowerEntry?>(-1, new PowerEntry()))
                .FirstOrDefault(e => e.Value?.Power?.FullName == _basePower.FullName)
                .Key;

            Debug.WriteLine($"Base: {_basePower.FullName}\r\nIdx: {t}");
            return t;
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

        private bool SplitFX_AddToList(ref Enums.ShortFX baseSfx, ref Enums.ShortFX enhSfx, ref PairedListEx iList, string specialTitle = "")
        {
            if (!baseSfx.Present) return false;

            var shortFxArray1 = Power.SplitFX(ref baseSfx, ref _basePower);
            var shortFxArray2 = Power.SplitFX(ref enhSfx, ref _enhancedPower);

            if (shortFxArray2.Length < shortFxArray1.Length)
            {
                var swappedFx = SwapExtraEffects(shortFxArray1, shortFxArray2);
                shortFxArray1 = (Enums.ShortFX[])swappedFx[0].Clone();
                shortFxArray2 = (Enums.ShortFX[])swappedFx[1].Clone();
            }

            foreach (var index in Enumerable.Range(0, shortFxArray1.Length).Where(index => shortFxArray1[index].Present))
            {
                var suffix = string.Empty;
                var num2 = shortFxArray1[index].Value[0];
                var num3 = index < shortFxArray2.Length
                    ? shortFxArray2[index].Value[0]
                    : shortFxArray2[index - 1].Value[0];

                if (_enhancedPower.Effects[shortFxArray1[index].Index[0]].DisplayPercentage)
                {
                    suffix = "%";
                    var effect = _enhancedPower.Effects[shortFxArray1[index].Index[0]];
                    if ((effect.EffectType == Enums.eEffectType.Heal ||
                         effect.EffectType == Enums.eEffectType.Endurance ||
                         effect.EffectType == Enums.eEffectType.Damage) &&
                        _enhancedPower.Effects[shortFxArray1[index].Index[0]].Aspect == Enums.eAspect.Cur)
                    {
                        num2 *= 100f;
                        num3 *= 100f;
                    }
                }
                else
                {
                    suffix = _enhancedPower.Effects[shortFxArray1[index].Index[0]].EffectType switch
                    {
                        Enums.eEffectType.Heal => " HP",
                        Enums.eEffectType.HitPoints => " HP",
                        _ => suffix
                    };
                }

                var title = Enums.GetEffectNameShort(_enhancedPower.Effects[shortFxArray1[index].Index[0]].EffectType);
                if (specialTitle != string.Empty)
                    title = specialTitle;

                var s1 = num2;
                var s2 = num3;
                if ((_enhancedPower.Effects[shortFxArray1[index].Index[0]].Suppression & MidsContext.Config.Suppression) !=
                    Enums.eSuppress.None)
                {
                    s1 = 0.0f;
                    s2 = 0.0f;
                }

                iList.AddItem(FastItem(title, s1, s2, suffix, false, false, _enhancedPower.Effects[shortFxArray1[index].Index[0]].Probability < 1.0, _enhancedPower.Effects[shortFxArray1[index].Index[0]].ActiveConditionals.Count > 0, Power.SplitFXGroupTip(ref shortFxArray1[index], ref _enhancedPower, false)));
                if (_enhancedPower.Effects[shortFxArray1[index].Index[0]].isEnhancementEffect)
                    iList.SetUnique();
            }

            return true;
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
