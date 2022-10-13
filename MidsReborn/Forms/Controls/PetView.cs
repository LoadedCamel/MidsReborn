using Mids_Reborn.Controls;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Linq;
using System.Text.RegularExpressions;
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
        public delegate void SlotUpdateEventHandler();
        public delegate void UnlockClickEventHandler();
        public event SlotUpdateEventHandler? SlotUpdate;
        public event UnlockClickEventHandler? UnlockClick;

        private IPower? _basePower;
        private IPower? _enhancedPower;
        private int _entryIndex;
        private int _lastScaleVal;
        private int _scaleVal;
        private bool _UseAlt;
        public bool Lock;
        private PetInfo? _petInfo;

        public bool UseAlt
        {
            get => _UseAlt;
            set
            {
                var mainHeroColor = Color.FromArgb(12, 56, 100);
                var mainVillainColor = Color.FromArgb(100, 12, 20);
                var dimmedHeroColor = Color.FromArgb(7, 33, 59);
                var dimmedVillainColor = Color.FromArgb(59, 7, 12);

                _UseAlt = value;
                BackColor = _UseAlt ? mainVillainColor : mainHeroColor;
                panelSeparator1.BackColor = _UseAlt ? dimmedVillainColor : dimmedHeroColor;
                panelSeparator2.BackColor = _UseAlt ? dimmedVillainColor : dimmedHeroColor;
                panelSeparator3.BackColor = _UseAlt ? dimmedVillainColor : dimmedHeroColor;
                panelSeparator4.BackColor = _UseAlt ? dimmedVillainColor : dimmedHeroColor;
                panelSeparator5.BackColor = _UseAlt ? dimmedVillainColor : dimmedHeroColor;
                panelSeparator6.BackColor = _UseAlt ? dimmedVillainColor : dimmedHeroColor;
            }
        }

        public PetView()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            _entryIndex = -1;
            _petInfo = new PetInfo();
            Load += OnLoad;
            InitializeComponent();
        }

        private void OnLoad(object? sender, EventArgs e)
        {
            Lock = false;
            lockIcon.Visible = false;
        }

        private static string CapString(string iString, int capLength)
        {
            return iString.Length >= capLength ? iString[..capLength] : iString;
        }

        private void Lock_Click(object sender, EventArgs e)
        {
            switch (Lock)
            {
                case false:
                    lockIcon.Visible = true;
                    unlockIcon.Visible = false;
                    break;
                case true:
                    lockIcon.Visible = false;
                    unlockIcon.Visible = true;
                    break;
            }

            Lock = !Lock;
            Debug.WriteLine($"Lock is: {Lock}");
            UnlockClick?.Invoke();
        }

        private void DisplayInfo(bool noLevel = false, int iEnhLvl = -1)
        {
            if (_basePower == null)
            {
                return;
            }

            var enhancedPower = _enhancedPower != null && _enhancedPower.PowerIndex == -1 ? _basePower : _enhancedPower;

            info_Title.Text = !noLevel & (_basePower.Level > 0)
                ? $"[{_basePower.Level}] {_basePower.DisplayName}"
                : _basePower.DisplayName;

            if (iEnhLvl > -1)
            {
                var infoTitle = info_Title;
                infoTitle.Text = $@"{infoTitle.Text} (Slot Level {iEnhLvl + 1})";
            }

            //var longInfo = Regex.Replace(_basePower.DescLong.Trim().Replace("<br>", RTF.Crlf()), @"\s{2,}", " ");
            //info_TxtSmall.Rtf = RTF.StartRTF() + RTF.ToRTF(_basePower.DescShort.Trim()) + RTF.EndRTF();
            //info_TxtLarge.Rtf = RTF.StartRTF() + RTF.ToRTF(longInfo) + RTF.EndRTF();
            info_TxtSmall.Text = _basePower.DescShort;
            info_TxtLarge.Text = _basePower.DescLong;
            var suffix1 = _basePower.PowerType != Enums.ePowerType.Toggle ? "" : "/s";
            
            info_DataList.Clear();
            var tip1 = string.Empty;
            if (_basePower.PowerType == Enums.ePowerType.Click)
            {
                if (enhancedPower != null && (enhancedPower.ToggleCost > 0) & (enhancedPower.RechargeTime + (double)enhancedPower.CastTime + enhancedPower.InterruptTime > 0))
                {
                    tip1 = $"Effective end drain per second: {Utilities.FixDP(enhancedPower.ToggleCost / (enhancedPower.RechargeTime + enhancedPower.CastTime + enhancedPower.InterruptTime))}/s";
                }

                if (enhancedPower != null && MidsContext.Config != null && (enhancedPower.ToggleCost > 0) &
                    (MidsContext.Config.DamageMath.ReturnValue == ConfigData.EDamageReturn.Numeric))
                {
                    var damageValue = enhancedPower.FXGetDamageValue(_enhancedPower == null);
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

            /*foreach (var effect in _basePower.Effects)
            {
                effect.UpdateAttrib();
                SetDamageTip();
            }*/
            if (enhancedPower != null)
            {
                info_DataList.AddItem(FastItem(ShortStr("End Cost", "End"), _basePower.ToggleCost,
                    enhancedPower.ToggleCost, suffix1, tip1));
                var flag1 = _basePower.HasAbsorbedEffects && _basePower.PowerIndex > -1 &&
                            DatabaseAPI.Database.Power[_basePower.PowerIndex].EntitiesAutoHit == Enums.eEntity.None;
                var flag2 = _basePower.Effects.Any(t => t.RequiresToHitCheck);

                if ((_basePower.EntitiesAutoHit == Enums.eEntity.None) | flag2 | flag1 | ((_basePower.Range > 20) &
                        _basePower.I9FXPresentP(Enums.eEffectType.Mez, Enums.eMez.Taunt)))
                {
                    var accuracy1 = _basePower.Accuracy;
                    var accuracy2 = enhancedPower.Accuracy;
                    var num2 = MidsContext.Config.ScalingToHit * _basePower.Accuracy;
                    var str = string.Empty;
                    var suffix2 = "%";
                    if ((_basePower.EntitiesAutoHit != Enums.eEntity.None) & flag2)
                    {
                        str = "\r\n* This power is autohit, but has an effect that requires a ToHit roll.";
                        suffix2 += "*";
                    }

                    if ((Math.Abs(accuracy1 - accuracy2) > float.Epsilon) &
                        (Math.Abs(num2 - accuracy2) > float.Epsilon))
                    {
                        var tip2 =
                            $"Accuracy multiplier without other buffs (Real Numbers style): {_basePower.Accuracy + (enhancedPower.Accuracy - (double)MidsContext.Config.ScalingToHit):##0.00000}x{str}";
                        info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                            MidsContext.Config.ScalingToHit * _basePower.Accuracy * 100, enhancedPower.Accuracy * 100,
                            suffix2, tip2));
                    }
                    else
                    {
                        var tip2 =
                            $"Accuracy multiplier without other buffs (Real Numbers style): {_basePower.AccuracyMult:##0.00}x{str}";
                        info_DataList.AddItem(FastItem(ShortStr("Accuracy", "Acc"),
                            MidsContext.Config.ScalingToHit * _basePower.Accuracy * 100,
                            MidsContext.Config.ScalingToHit * _basePower.Accuracy * 100, suffix2, tip2));
                    }
                }
                else
                {
                    info_DataList.AddItem(new PairedListEx.Item(string.Empty, string.Empty, false, false, false,
                        string.Empty));
                }

                info_DataList.AddItem(FastItem(ShortStr("Recharge", "Rchg"), _basePower.RechargeTime,
                    enhancedPower.RechargeTime, "s"));
                var s1 = 0f;
                var s2 = 0f;
                var durationEffectId = _basePower.GetDurationEffectID();
                if (durationEffectId > -1)
                {
                    if ((_basePower.Effects[durationEffectId].EffectType == Enums.eEffectType.EntCreate) &
                        (_basePower.Effects[durationEffectId].Duration >= 9999))
                    {
                        s1 = 0f;
                        s2 = 0f;
                    }
                    else
                    {
                        s1 = _basePower.Effects[durationEffectId].Duration;
                        s2 = enhancedPower.Effects[durationEffectId].Duration;
                    }
                }

                info_DataList.AddItem(FastItem(ShortStr("Duration", "Durtn"), s1, s2, "s"));

                info_DataList.AddItem(FastItem(ShortStr("Range", "Range"), _basePower.Range, enhancedPower.Range,
                    "ft"));
                info_DataList.AddItem(_basePower.Arc > 0
                    ? FastItem("Arc", _basePower.Arc, enhancedPower.Arc, "°")
                    : FastItem("Radius", _basePower.Radius, enhancedPower.Radius, "ft"));
                info_DataList.AddItem(FastItem(ShortStr("Cast Time", "Cast"), enhancedPower.CastTime,
                    _basePower.CastTime, "s",
                    $"CastTime: {_basePower.CastTime}s\r\nArcana CastTime: {(Math.Ceiling(enhancedPower.CastTime / 0.132f) + 1) * 0.132:####0.###}s",
                    false, true, false, false, 3));
                info_DataList.AddItem(_basePower.PowerType == Enums.ePowerType.Toggle
                    ? FastItem(ShortStr("Activate", "Act"), _basePower.ActivatePeriod, enhancedPower.ActivatePeriod,
                        "s", "The effects of this toggle power are applied at this interval.")
                    : FastItem(ShortStr("Interrupt", "Intrpt"), enhancedPower.InterruptTime, _basePower.InterruptTime,
                        "s", "After activating this power, it can be interrupted for this amount of time."));
                if (durationEffectId > -1 &&
                    _basePower.Effects[durationEffectId].EffectType == Enums.eEffectType.Mez &
                    _basePower.Effects[durationEffectId].MezType != Enums.eMez.Taunt &
                    !((_basePower.Effects[durationEffectId].MezType == Enums.eMez.Knockback |
                       _basePower.Effects[durationEffectId].MezType == Enums.eMez.Knockup) &
                      _basePower.Effects[durationEffectId].Mag < 0))
                {
                    info_DataList.AddItem(new PairedListEx.Item("Effect:",
                        Enum.GetName(typeof(Enums.eMez), _basePower.Effects[durationEffectId].MezType), false,
                        _basePower.Effects[durationEffectId].Probability < 1,
                        _basePower.Effects[durationEffectId].CanInclude(),
                        durationEffectId));

                    info_DataList.AddItem(new PairedListEx.Item("Mag:",
                        $"{enhancedPower.Effects[durationEffectId].BuffedMag:####0.##}",
                        Math.Abs(_basePower.Effects[durationEffectId].BuffedMag -
                                 enhancedPower.Effects[durationEffectId].BuffedMag) > float.Epsilon,
                        _basePower.Effects[durationEffectId].Probability < 1));
                }

                var rankedEffects = _basePower.GetRankedEffects();
                var defiancePower = DatabaseAPI.GetPowerByFullName("Inherent.Inherent.Defiance");
                for (var id = 0; id < rankedEffects.Length; id++)
                {
                    if (rankedEffects[id] <= -1)
                    {
                        continue;
                    }

                    if (_basePower.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.Mez)
                    {
                        continue;
                    }

                    var rankedEffect = GetRankedEffect(rankedEffects, id);
                    // if (_basePower.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.PowerRedirect)
                    //     continue;

                    if (!((_basePower.Effects[rankedEffects[id]].Probability > 0) &
                          ((MidsContext.Config.Suppression & _basePower.Effects[rankedEffects[id]].Suppression) ==
                           Enums.eSuppress.None) & _basePower.Effects[rankedEffects[id]].CanInclude()))
                    {
                        continue;
                    }

                    if (_basePower.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.RevokePower &&
                        _basePower.Effects[rankedEffects[id]].nSummon <= -1 &&
                        string.IsNullOrWhiteSpace(_basePower.Effects[rankedEffects[id]].Summon))
                    {
                        continue;
                    }

                    if (_basePower.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.GrantPower &&
                        _basePower.Effects[rankedEffects[id]].nSummon <= -1)
                    {
                        continue;
                    }

                    if (_basePower.Effects[rankedEffects[id]].EffectType != Enums.eEffectType.Enhancement)
                    {
                        if (_basePower.Effects[rankedEffects[id]].EffectType != Enums.eEffectType.Mez)
                        {
                            switch (_basePower.Effects[rankedEffects[id]].EffectType)
                            {
                                case Enums.eEffectType.Recovery:
                                case Enums.eEffectType.Endurance:
                                    rankedEffect.Name = $"{_basePower.Effects[rankedEffects[id]].EffectType}";
                                    var fxTarget = enhancedPower.Effects[rankedEffects[id]].ToWho switch
                                    {
                                        Enums.eToWho.Self => "(Self)",
                                        Enums.eToWho.Target => "(Tgt)",
                                        _ => ""
                                    };
                                    rankedEffect.Value = enhancedPower.Effects[rankedEffects[id]].DisplayPercentage
                                        ? $"{enhancedPower.Effects[rankedEffects[id]].BuffedMag * 100:###0.##}% {fxTarget}"
                                        : $"{enhancedPower.Effects[rankedEffects[id]].BuffedMag:###0.##} {fxTarget}";
                                    break;

                                case Enums.eEffectType.EntCreate when !_basePower.AbsorbSummonEffects |
                                                                      !_basePower.AbsorbSummonAttributes:
                                {
                                    rankedEffect.Name = "Summon";
                                    if (_basePower.Effects[rankedEffects[id]].nSummon > -1)
                                    {
                                        rankedEffect.Value = DatabaseAPI.Database
                                            .Entities[_basePower.Effects[rankedEffects[id]].nSummon].DisplayName;
                                    }
                                    else
                                    {
                                        rankedEffect.Value = _basePower.Effects[rankedEffects[id]].Summon;
                                        rankedEffect.Value = Regex.Replace(rankedEffect.Value,
                                            @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);
                                    }

                                    break;
                                }
                                case Enums.eEffectType.GrantPower:
                                {
                                    rankedEffect.Name = "Grant";
                                    if (_basePower.Effects[rankedEffects[id]].nSummon > -1)
                                    {
                                        rankedEffect.Value = DatabaseAPI.Database
                                            .Power[_basePower.Effects[rankedEffects[id]].nSummon].DisplayName;
                                    }

                                    break;
                                }

                                case Enums.eEffectType.CombatModShift:
                                {
                                    rankedEffect.Name = "LvlShift";
                                    rankedEffect.Value =
                                        $"{(_basePower.Effects[rankedEffects[id]].Mag > 0 ? "+" : "")}{_basePower.Effects[rankedEffects[id]].Mag:##0.##}";

                                    break;
                                }

                                case Enums.eEffectType.RevokePower:
                                {
                                    rankedEffect.Name = "Revoke";
                                    if (_basePower.Effects[rankedEffects[id]].nSummon > -1)
                                    {
                                        rankedEffect.Value = DatabaseAPI.Database
                                            .Entities[_basePower.Effects[rankedEffects[id]].nSummon].DisplayName;
                                    }
                                    else
                                    {
                                        rankedEffect.Value = _basePower.Effects[rankedEffects[id]].Summon;
                                        rankedEffect.Value = Regex.Replace(rankedEffect.Value,
                                            @"^(MastermindPets|Pets|Villain_Pets)_", string.Empty);
                                    }

                                    break;
                                }

                                case Enums.eEffectType.DamageBuff:
                                    var isDefiance =
                                        _basePower.Effects[rankedEffects[id]].SpecialCase ==
                                        Enums.eSpecialCase.Defiance &&
                                        _basePower.Effects[rankedEffects[id]]
                                            .ValidateConditional("Active", "Defiance") |
                                        MidsContext.Character.CurrentBuild.PowerActive(defiancePower);
                                    rankedEffect.Name = isDefiance
                                        ? "Defiance"
                                        : ShortStr(
                                            Enums.GetEffectName(_basePower.Effects[rankedEffects[id]].EffectType),
                                            Enums.GetEffectNameShort(_basePower.Effects[rankedEffects[id]].EffectType));
                                    rankedEffect.ToolTip = isDefiance
                                        ? _basePower.Effects[rankedEffects[id]].BuildEffectString(false,
                                            "DamageBuff (Defiance)", false, false, false, true)
                                        : (_enhancedPower ?? _basePower).BuildTooltipStringAllVectorsEffects(
                                            _basePower.Effects[rankedEffects[id]].EffectType);
                                    break;

                                case Enums.eEffectType.Resistance:
                                case Enums.eEffectType.Defense:
                                case Enums.eEffectType.Elusivity:
                                    rankedEffect.Name = ShortStr(
                                        Enums.GetEffectName(_basePower.Effects[rankedEffects[id]].EffectType),
                                        Enums.GetEffectNameShort(_basePower.Effects[rankedEffects[id]].EffectType));

                                    rankedEffect.ToolTip =
                                        (_enhancedPower ?? _basePower).BuildTooltipStringAllVectorsEffects(_basePower
                                            .Effects[rankedEffects[id]].EffectType);

                                    break;

                                //case Enums.eEffectType.ToHit:
                                default:
                                    rankedEffect.Value =
                                        $"{_enhancedPower.Effects[rankedEffects[id]].BuffedMag * (_enhancedPower.Effects[rankedEffects[id]].DisplayPercentage ? 100 : 1):####0.##}{(_enhancedPower.Effects[rankedEffects[id]].DisplayPercentage ? "%" : "")}";
                                    rankedEffect.Value += _enhancedPower.Effects[rankedEffects[id]].ToWho switch
                                    {
                                        Enums.eToWho.Self => " (Self)",
                                        Enums.eToWho.Target => " (Tgt)",
                                        _ => ""
                                    };

                                    rankedEffect.UseAlternateColor = Math.Abs(_enhancedPower.Effects[rankedEffects[id]].BuffedMag - _basePower.Effects[rankedEffects[id]].BuffedMag) > float.Epsilon;
                                    rankedEffect.Name =
                                        ShortStr(Enums.GetEffectName(_basePower.Effects[rankedEffects[id]].EffectType),
                                            Enums.GetEffectNameShort(_basePower.Effects[rankedEffects[id]].EffectType));
                                    rankedEffect.ToolTip = _enhancedPower.Effects[rankedEffects[id]]
                                        .BuildEffectString(false, "", false, false, false, true);

                                    break;

                                /*default:
                                rankedEffect.Name = ShortStr(Enums.GetEffectName(_basePower.Effects[rankedEffects[id]].EffectType),
                                    Enums.GetEffectNameShort(_basePower.Effects[rankedEffects[id]].EffectType));
                                rankedEffect.ToolTip = _enhancedPower.Effects[rankedEffects[id]].BuildEffectString(false, "", false, false, false, true);
                                
                                break;*/
                            }
                        }
                        else
                        {
                            rankedEffect.Name = ShortStr(
                                Enums.GetMezName((Enums.eMezShort)_basePower.Effects[rankedEffects[id]].MezType),
                                Enums.GetMezNameShort((Enums.eMezShort)_basePower.Effects[rankedEffects[id]].MezType));
                        }
                    }

                    // Ignore fully absorbed entities
                    if (_basePower.Effects[rankedEffects[id]].EffectType == Enums.eEffectType.EntCreate)
                    {
                        if (_basePower.AbsorbSummonEffects & _basePower.AbsorbSummonAttributes)
                        {
                            continue;
                        }
                    }

                    info_DataList.AddItem(rankedEffect);
                    if (_basePower.Effects[rankedEffects[id]].isEnhancementEffect)
                    {
                        info_DataList.SetUnique();
                    }
                }

                info_DataList.Refresh();
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
                {
                    str1 += " (% only)";
                }

                var baseDamage =
                    Math.Abs(_basePower.FXGetDamageValue(_basePower.PowerIndex > -1 & _enhancedPower.PowerIndex > -1));
                var enhancedDamage = _enhancedPower.PowerIndex == -1
                    ? baseDamage
                    : Math.Abs(enhancedPower.FXGetDamageValue());

                if (_basePower.NIDSubPower.Length > 0 & baseDamage == 0 && enhancedDamage == 0)
                {
                    lblDmg.Text = string.Empty;
                    info_Damage.nBaseVal = 0f;
                    info_Damage.nEnhVal = 0f;
                    info_Damage.nMaxEnhVal = 0f;
                    info_Damage.nHighEnh = 0f;
                    info_Damage.Text = string.Empty;
                }
                else
                {
                    lblDmg.Text = $@"{str1}:";
                    //info_Damage.nBaseVal = damageValue1;
                    //info_Damage.nMaxEnhVal = baseDamage * (1f + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f));
                    //info_Damage.nEnhVal = damageValue2;

                    // Mids has no awareness of target data.
                    // When using damage %, estimate damage value based on character HP.
                    var hasPercentDamage = _enhancedPower.Effects
                        .Any(e => e.EffectType == Enums.eEffectType.Damage &&
                                  e.DisplayPercentage | e.Aspect == Enums.eAspect.Str);
                    var dmgMultiplier = hasPercentDamage ? MidsContext.Character.Totals.HPMax : 1;

                    info_Damage.nBaseVal = Math.Max(0, baseDamage * dmgMultiplier); // Negative damage ? (see Toxins)
                    info_Damage.nEnhVal = Math.Max(0, enhancedDamage * dmgMultiplier);
                    info_Damage.nMaxEnhVal =
                        Math.Max(baseDamage * dmgMultiplier * (1 + Enhancement.ApplyED(Enums.eSchedule.A, 2.277f)),
                            enhancedDamage * dmgMultiplier);
                    info_Damage.nHighEnh = Math.Max(414, enhancedDamage * dmgMultiplier); // Maximum graph value
                    info_Damage.Text = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon
                        ? $"{enhancedPower.FXGetDamageString(_enhancedPower.PowerIndex == -1)} ({(hasPercentDamage ? $"{Utilities.FixDP(baseDamage * 100)}%" : Utilities.FixDP(baseDamage))})"
                        : _basePower.FXGetDamageString(_basePower.PowerIndex > -1 & _enhancedPower.PowerIndex > -1);
                }
            }

            SetpowerScaler();
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
                // Ensure _enhancedPower has at least as many effects as _basePower
                if (_basePower.Effects.Length > _enhancedPower.Effects.Length)
                {
                    var swappedFx = SwapExtraEffects(_basePower.Effects, _enhancedPower.Effects);
                    _basePower.Effects = (IEffect[])swappedFx[0].Clone();
                    _enhancedPower.Effects = (IEffect[])swappedFx[1].Clone();
                }
            }

            //lblLock.Visible = Lock & (TabPage != 2);
            DisplayInfo(noLevel, iEnhLevel);
        }

        private string GetToWhoShort(IEffect fx)
        {
            return fx.ToWho switch
            {
                Enums.eToWho.Target => " (Tgt)",
                Enums.eToWho.Self => " (Self)",
                _ => ""
            };
        }

        private static bool IsMezEffect(string iStr)
        {
            var names = Enum.GetNames(Enums.eMez.None.GetType());
            var num = names.Length - 1;
            for (var index = 0; index <= num; ++index)
                if (string.Equals(iStr, names[index], StringComparison.OrdinalIgnoreCase))
                    return true;

            return false;
        }

        // Move the extra effects from the longest array (_basePower) to the shortest (_enhancedPower),
        // Resulting in _enhancedPower always being the longest one.
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

        /// <summary>
        /// Add Mez effects to the effects tab. 
        /// </summary>
        /// <param name="sourcePower">Source power to fetch effects from. Can be _basePower, _enhancedPower, or a virtual set bonuses power <see cref="Build.BuildVirtualSetBonusesPower"></see></param>
        /// <param name="iList">Paired list to add items into</param>
        /// <param name="specialEffects">Set to true if sourcePower is a virtual set bonuses power, otherwise false</param>
        /// <param name="effectsCount">Number of effects processed</param>
        /// <param name="iAlternate">Force value to show as enhanced (true) or equal to base (false). Set to null for auto</param>
        /// <param name="startIndex">Effect index to start processing from. Set to 0 unless processing _enhancedPower effects to avoid duplicates</param>
        private void ProcessMezEffects(IPower sourcePower, ref PairedListEx iList, bool specialEffects, ref int effectsCount, bool? iAlternate = null, int startIndex = 0)
        {
            var names = Enum.GetNames(typeof(Enums.eMezShort));
            var enhancedPower = specialEffects ? sourcePower : _enhancedPower;
            var n = 0;

            for (var tagId = startIndex; tagId < sourcePower.Effects.Length; tagId++)
            {
                if (!(sourcePower.Effects[tagId].EffectType == Enums.eEffectType.Mez &
                      sourcePower.Effects[tagId].Probability > 0 &
                      sourcePower.Effects[tagId].CanInclude()) || !sourcePower.Effects[tagId].PvXInclude())
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ActiveConditionals is {Count: > 0})
                {
                    if (!sourcePower.Effects[tagId].ValidateConditional())
                    {
                        continue;
                    }
                }

                var str = !(sourcePower.Effects[tagId].Duration < 2 | sourcePower.PowerType == Enums.ePowerType.Auto_)
                    ? $" - {sourcePower.Effects[tagId].Duration:#0.#}s"
                    : "";
                if (sourcePower.Effects[tagId].BuffedMag > 0)
                {
                    var iAlternate2 = iAlternate ?? Math.Abs(sourcePower.Effects[tagId].Duration - enhancedPower.Effects[tagId].Duration) > float.Epsilon |
                        !Enums.MezDurationEnhanceable(sourcePower.Effects[tagId].MezType) &
                        Math.Abs(enhancedPower.Effects[tagId].BuffedMag - sourcePower.Effects[tagId].BuffedMag) > float.Epsilon;
                    var iValue = (sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0"
                        : $"Mag {Utilities.FixDP(enhancedPower.Effects[tagId].BuffedMag):####0.##}{str}";
                    if ((sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                    {
                        iValue = "0";
                    }

                    if (enhancedPower != null)
                    {
                        var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                        var activeConditionals = sourcePower.Effects[tagId].ActiveConditionals;
                        var iItem = new PairedListEx.Item($"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, iAlternate2,
                            sourcePower.Effects[tagId].Probability < 1 | sourcePower.Effects[tagId].ValidateConditional("Active", "Combo"),
                            activeConditionals is { Count: > 0 }, tip);
                        iList.AddItem(iItem);
                    }

                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }
                else if (sourcePower.Effects[tagId].MezType == Enums.eMez.ToggleDrop & sourcePower.Effects[tagId].Probability > 0)
                {
                    var iValue = (sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None
                        ? "0%"
                        : $"{sourcePower.Effects[tagId].Probability * 100}%";

                    if (enhancedPower != null)
                    {
                        var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                        var activeConditionals = sourcePower.Effects[tagId].ActiveConditionals;
                        var iItem = new PairedListEx.Item($"{CapString(names[(int)sourcePower.Effects[tagId].MezType], 7)}:", iValue, false,
                            sourcePower.Effects[tagId].Probability < 1 | sourcePower.Effects[tagId].ValidateConditional("Active", "Combo"),
                            activeConditionals is { Count: > 0 }, tip);
                        iList.AddItem(iItem);
                    }

                    if (sourcePower.Effects[tagId].isEnhancementEffect)
                    {
                        iList.SetUnique();
                    }
                }
                else
                {
                    var iAlternate2 = iAlternate ?? Math.Abs(sourcePower.Effects[tagId].Duration - enhancedPower.Effects[tagId].Duration) > float.Epsilon |
                        !Enums.MezDurationEnhanceable(sourcePower.Effects[tagId].MezType) &
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

                n++;
            }

            effectsCount += n;
        }

        /// <summary>
        /// Add MezResist effects to the effects tab
        /// </summary>
        ///
        /// <param name="sourcePower">Source power to fetch effects from. Can be _basePower, _enhancedPower, or a virtual set bonuses power <see cref="Build.BuildVirtualSetBonusesPower"></see></param>
        /// <param name="iList">Paired list to add items into</param>
        /// <param name="specialEffects">Set to true if sourcePower is a virtual set bonuses power, otherwise false</param>
        /// <param name="effectsCount">Number of effects processed</param>
        /// <param name="startIndex">Effect index to start processing from. Set to 0 unless processing _enhancedPower effects to avoid duplicates</param>
        private void ProcessMezResistEffects(IPower sourcePower, ref PairedListEx iList, bool specialEffects, ref int effectsCount, int startIndex = 0)
        {
            var names = Enum.GetNames(typeof(Enums.eMezShort));
            var enhancedPower = specialEffects ? sourcePower : _enhancedPower;
            for (var tagId = startIndex; tagId < sourcePower.Effects.Length; tagId++)
            {
                if (!(sourcePower.Effects[tagId].PvMode != Enums.ePvX.PvP & !MidsContext.Config.Inc.DisablePvE |
                      sourcePower.Effects[tagId].PvMode != Enums.ePvX.PvE & MidsContext.Config.Inc.DisablePvE) ||
                    !(sourcePower.Effects[tagId].EffectType == Enums.eEffectType.MezResist &
                      sourcePower.Effects[tagId].Probability > 0))
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ETModifies == Enums.eEffectType.Null)
                {
                    continue;
                }

                if (sourcePower.Effects[tagId].ActiveConditionals is {Count: > 0})
                {
                    if (!sourcePower.Effects[tagId].ValidateConditional())
                    {
                        continue;
                    }
                }

                var str = enhancedPower.Effects[tagId].Duration >= 15
                    ? $" - {Utilities.FixDP(enhancedPower.Effects[tagId].Duration)}s"
                    : "";
                var iValue = $"{sourcePower.Effects[tagId].MagPercent:####0.##}%{str}";
                if ((sourcePower.Effects[tagId].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
                {
                    iValue = "0%";
                }

                var tip = GenerateTipFromEffect(enhancedPower, enhancedPower.Effects[tagId]);
                var iItem = new PairedListEx.Item(
                    $"{CapString($"-{names[(int) sourcePower.Effects[tagId].MezType]}", 7)}:", iValue, false, false,
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
            if (sFxBaseMezResist.Present | sFxBaseMez.Present)
            {
                iLabel.Text =
                    _basePower.AffectsTarget(Enums.eEffectType.MezResist) |
                    _basePower.AffectsTarget(Enums.eEffectType.Mez)
                        ? @"Status Effects (Target)"
                        : @"Status Effects (Self)";
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
                if (!((_basePower.Effects[index].EffectType == Enums.eEffectType.EntCreate) & (_basePower.Effects[index].Probability > 0)))
                {
                    continue;
                }

                if (_basePower.AbsorbSummonEffects & _basePower.AbsorbSummonAttributes)
                {
                    continue;
                }

                if (_basePower.Effects[index].ActiveConditionals is {Count: > 0})
                {
                    if (!_basePower.Effects[index].ValidateConditional())
                    {
                        continue;
                    }
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
                    if (MidsContext.Config != null && (_basePower.Effects[index].Suppression & MidsContext.Config.Suppression) != Enums.eSuppress.None)
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

                ++num1;
            }

            if (num1 > 0 && flag)
            {
                iLabel.Text = @"Summoned Entities";
            }

            return num1;
        }

        private static string GenerateTipFromEffect(IPower basePower, IEffect baseFx)
        {
            return (string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                MidsContext.Config != null && e.Value.EffectType == baseFx.EffectType &
                                e.Value.DamageType == baseFx.DamageType &
                                e.Value.MezType == baseFx.MezType &
                                e.Value.ETModifies == baseFx.ETModifies &
                                e.Value.ToWho == Enums.eToWho.Self &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e =>
                                (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                 basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                    + "\n\n"
                    + string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                MidsContext.Config != null && e.Value.EffectType == baseFx.EffectType &
                                e.Value.DamageType == baseFx.DamageType &
                                e.Value.MezType == baseFx.MezType &
                                e.Value.ETModifies == baseFx.ETModifies &
                                e.Value.ToWho == Enums.eToWho.Target &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e =>
                                (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                 basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))).Trim();
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
                               MidsContext.Config != null && effectTypes.Contains(e.Value.EffectType) &
                               effectDmgTypes.Contains(e.Value.DamageType) &
                               effectEtModifies.Contains(e.Value.ETModifies) &
                               effectMezTypes.Contains(e.Value.MezType) &
                               e.Value.ToWho == Enums.eToWho.Self &
                               (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP)) &
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                   + "\n\n"
                   + string.Join("\n",
                       basePower.Effects
                           .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                           .Where(e =>
                               MidsContext.Config != null && effectTypes.Contains(e.Value.EffectType) &
                               effectDmgTypes.Contains(e.Value.DamageType) &
                               effectEtModifies.Contains(e.Value.ETModifies) &
                               effectMezTypes.Contains(e.Value.MezType) &
                               e.Value.ToWho == Enums.eToWho.Target &
                               (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                   ? Enums.ePvX.PvE
                                   : Enums.ePvX.PvP)) &
                               (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                           .Select(e =>
                               (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ","))).Trim();
        }

        // Def/Res/Elusivity
        private static string GenerateTipFromEffect(IPower basePower, Enums.eEffectType effectType, Enums.eDamage dmgType)
        {
            return (string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == effectType &
                                e.Value.DamageType == dmgType &
                                e.Value.ToWho == Enums.eToWho.Self &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e => (e.Value.BuildEffectString(false, "", false, false, false, true) +
                                          basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ",")))
                    + "\n\n"
                    + string.Join("\n",
                        basePower.Effects
                            .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                            .Where(e =>
                                e.Value.EffectType == effectType &
                                e.Value.DamageType == dmgType &
                                e.Value.ToWho == Enums.eToWho.Target &
                                (e.Value.PvMode != (MidsContext.Config.Inc.DisablePvE
                                    ? Enums.ePvX.PvE
                                    : Enums.ePvX.PvP)) &
                                (e.Value.Suppression & MidsContext.Config.Suppression) == Enums.eSuppress.None)
                            .Select(e => (e.Value.BuildEffectString(false, "", false, false, false, true) +
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
            if ((Math.Abs(s1.Sum) < float.Epsilon) & !alwaysShow)
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
                        iValue += iValue2.Replace("%", "");
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
            if ((Math.Abs(s1.Sum) < float.Epsilon) & !alwaysShow)
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
                if (Math.Abs(s1.Sum - (double)s2.Sum) > float.Epsilon)
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
            if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
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
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
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
            if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
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
            if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
            {
                itemPair = new PairedListEx.Item(string.Empty, string.Empty, false);
            }
            else
            {
                var iAlternate = false;
                if (Math.Abs(s1 - (double)s2) > float.Epsilon)
                {
                    if (!skiBasePower)
                    {
                        iValue += $" ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                    }

                    iAlternate = true;
                }

                itemPair = new PairedListEx.Item(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
            }

            return itemPair;
        }

        private static string ConvertNewlinesToRtf(string str)
        {
            return str
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", RTF.Crlf());
        }

        private static string GetEnhancementStringLongRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementStringLong();
            if (!string.IsNullOrEmpty(str))
                str = RTF.Color(RTF.ElementID.Enhancement) + RTF.Italic(ConvertNewlinesToRtf(str)) + RTF.Color(RTF.ElementID.Text);

            return str;
        }

        private static string GetEnhancementStringRtf(I9Slot iEnh)
        {
            var str = iEnh.GetEnhancementString();
            if (!string.IsNullOrEmpty(str))
                str = RTF.Color(RTF.ElementID.Enhancement) + ConvertNewlinesToRtf(str) + RTF.Color(RTF.ElementID.Text);

            return str;
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
                    flag = (_basePower.Effects[index[id]].EffectType == _basePower.Effects[index[id - 1]].EffectType) &
                           (_basePower.Effects[index[id]].ToWho == Enums.eToWho.Self) &
                           (_basePower.Effects[index[id - 1]].ToWho == Enums.eToWho.Self) &
                           (_basePower.Effects[index[id]].ToWho == Enums.eToWho.Target);
                }

                if (_basePower.Effects[index[id]].DelayedTime > 5)
                {
                    flag = true;
                }

                var names = Enum.GetNames(typeof(Enums.eEffectTypeShort));
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
                        shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (_basePower.Effects[index[id]].BuffedMag <= 1)
                        {
                            temp = $"{_basePower.Effects[index[id]].BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(_basePower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf, onlyTarget));
                            shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
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
                        suffix = absorbPercent ? "%" : "";
                        break;
                    case Enums.eEffectType.Endurance:
                        if (_basePower.Effects[index[id]].BuffedMag < -0.01 && _basePower.Effects[index[id]].BuffedMag > -1)
                        {
                            temp = $"{_basePower.Effects[index[id]].BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
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
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
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
                    _ => ""
                };

                if (flag)
                {
                    return FastItem("", 0f, 0f, string.Empty);
                }
            }

            for (var sIndex = 0; sIndex < shortFxEnh.Index.Length; sIndex++)
            {
                var sFxIdx = shortFxEnh.Index[sIndex];
                if (sFxIdx >= _basePower.Effects.Length & sFxIdx >= _enhancedPower.Effects.Length)
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
                        //Fixes the Absorb display to correctly show the percentage
                        shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture).Replace("%", ""));
                        break;
                    case Enums.eEffectType.ToHit:
                        //Fixes the ToHit display to correctly show the percentage
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

            // shortFxEnh.Index.Length == 0 will occur if all effects of the same kind
            // have non validated conditionals.
            // E.g. -Recovery on Kick if Cross Punch has not been picked.
            var tip = shortFxEnh.Index.Length <= 0
                ? ""
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

        private void powerScaler_BarClick(float value)
        {
            var num = (int)Math.Round(value);
            if (num < _basePower.VariableMin)
                num = _basePower.VariableMin;
            if (num > _basePower.VariableMax)
                num = _basePower.VariableMax;
            MidsContext.Character.CurrentBuild.Powers[_entryIndex].VariableValue = num;
            MidsContext.Character.CurrentBuild.Powers[_entryIndex].Power.Stacks = num;
            /*foreach (var effect in MidsContext.Character.CurrentBuild.Powers[_entryIndex].Power.Effects)
            {
                effect.UpdateAttrib();
                DisplayInfo();
            }*/
            if (num == _lastScaleVal)
                return;
            SetpowerScaler();
            _lastScaleVal = num;
            MainModule.MidsController.Toon.GenerateBuffedPowerArray();
            var slotUpdate = SlotUpdate;
            slotUpdate?.Invoke();
        }

        private void SetDamageTip()
        {
            var iTip = string.Empty;
            var num1 = -1;
            var num2 = -1;
            var num3 = 0;

            if (_enhancedPower == null || _enhancedPower.Effects.Length <= 0)
            {
                info_Damage.SetTip("");

                return;
            }

            var num4 = _enhancedPower.Effects.Length - 1;
            for (var index = 0; index <= num4; ++index)
            {
                var effect = _enhancedPower.Effects[index];
                if (effect.EffectType != Enums.eEffectType.Damage)
                    continue;
                if (effect.CanInclude() & _enhancedPower.Effects[index].PvXInclude())
                {
                    if (iTip != string.Empty)
                        iTip += "\r\n";
                    var str = _enhancedPower.Effects[index].BuildEffectString(false, "", false, false, false, false, false, true);
                    if (_enhancedPower.Effects[index].isEnhancementEffect & (_enhancedPower.PowerType == Enums.ePowerType.Toggle))
                    {
                        ++num1;
                        str += " (Special only every 10s)";
                    }
                    else if (_enhancedPower.PowerType == Enums.ePowerType.Toggle)
                    {
                        ++num2;
                    }

                    iTip += str;
                }
                else
                {
                    ++num3;
                }
            }

            if (num3 > 0)
            {
                if (iTip != string.Empty)
                    iTip += "\r\n";
                iTip += "\r\nThis power deals different damage in PvP and PvE modes.";
            }

            if (!((_basePower.PowerType == Enums.ePowerType.Toggle) & (num1 == -1) & (num2 == -1)) && (_basePower.PowerType == Enums.ePowerType.Toggle) & (num2 > -1) && !string.IsNullOrEmpty(iTip))
            {
                iTip = "Applied every " + Convert.ToString(_basePower.ActivatePeriod, CultureInfo.InvariantCulture) + "s:\r\n" + iTip;
            }

            info_Damage.SetTip(iTip);
        }

        private void SetpowerScaler()
        {
            if (_basePower == null)
            {
                powerScaler.Visible = false;
            }
            else if (_basePower.VariableEnabled & (_entryIndex > -1))
            {
                var str = _basePower.VariableName;
                if (string.IsNullOrEmpty(str))
                    str = "Targets";
                powerScaler.Visible = true;
                powerScaler.BeginUpdate();
                powerScaler.ForcedMax = _basePower.VariableMax;
                powerScaler.Clear();
                powerScaler.AddItem(
                    str + ":|" + Convert.ToString(MidsContext.Character.CurrentBuild.Powers[_entryIndex].VariableValue),
                    MidsContext.Character.CurrentBuild.Powers[_entryIndex].VariableValue, 0.0f,
                    "Use this slider to vary the power's effect.\r\nMin: " + Convert.ToString(_basePower.VariableMin) +
                    "\r\nMax: " +
                    Convert.ToString(_basePower.VariableMax));
                powerScaler.EndUpdate();
            }
            else
            {
                powerScaler.Visible = false;
            }
        }

        public void SetData(IPower? basePower, IPower enhancedPower, bool noLevel = false, bool locked = false, int i_entryIndex = -1)
        {
            if (basePower == null)
            {
                return;
            }

            Lock = locked;
            switch (locked)
            {
                case false:
                    lockIcon.Visible = false;
                    unlockIcon.Visible = true;
                    break;
                case true:
                    lockIcon.Visible = true;
                    unlockIcon.Visible = false;
                    break;
            }

            var basePowerData = new Power(basePower);
            var enhancedPowerData = new Power(enhancedPower);

            if (enhancedPowerData.PowerIndex == -1 & basePowerData.PowerIndex == -1)
            {
                _basePower = null;
            }
            else if (enhancedPowerData.PowerIndex == -1 & basePowerData.PowerIndex > -1)
            {
                _basePower = basePowerData;
            }
            else
            {
                _basePower = new Power(DatabaseAPI.Database.Power[enhancedPowerData.PowerIndex]);
            }

            _enhancedPower = enhancedPowerData.PowerIndex == -1
                ? new Power(basePower) {PowerIndex = -1}
                : enhancedPowerData;

            // Data sent to the Dataview may differ from DB.
            // Not needed if ActivatePeriod absorb from summons is disabled in Power.AbsorbPetEffects()
            /*var dbPower = DatabaseAPI.GetPowerByFullName(_basePower.FullName);
            if (dbPower != null)
            {
                _basePower.ActivatePeriod = dbPower.ActivatePeriod;
                _enhancedPower.ActivatePeriod = dbPower.ActivatePeriod;
            }*/

            _basePower?.ProcessExecutes();
            _basePower?.AbsorbPetEffects();
            _enhancedPower?.ProcessExecutes();
            //_enhancedPower?.AbsorbPetEffects(); // Done already during calculations (see clsToonX.GenerateBuffedPowerArray())

            _entryIndex = i_entryIndex;
            SetDamageTip();
            DisplayData(noLevel);
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
            bool flag;
            if (!baseSfx.Present)
            {
                flag = false;
            }
            else
            {
                var shortFxArray1 = Power.SplitFX(ref baseSfx, ref _basePower);
                var shortFxArray2 = Power.SplitFX(ref enhSfx, ref _enhancedPower);
                if (shortFxArray2.Length < shortFxArray1.Length)
                {
                    var swappedFx = SwapExtraEffects(shortFxArray1, shortFxArray2);
                    shortFxArray1 = (Enums.ShortFX[])swappedFx[0].Clone();
                    shortFxArray2 = (Enums.ShortFX[])swappedFx[1].Clone();
                }
                
                for (var index = 0; index < shortFxArray1.Length; index++)
                {
                    if (!shortFxArray1[index].Present)
                        continue;
                    var suffix = string.Empty;
                    var num2 = shortFxArray1[index].Value[0];
                    var num3 = index < shortFxArray2.Length
                        ? shortFxArray2[index].Value[0]
                        : shortFxArray2[index - 1].Value[0];
                    if (_enhancedPower.Effects[shortFxArray1[index].Index[0]].DisplayPercentage)
                    {
                        suffix = "%";
                        var effect = _enhancedPower.Effects[shortFxArray1[index].Index[0]];
                        if ((effect.EffectType == Enums.eEffectType.Heal |
                             effect.EffectType == Enums.eEffectType.Endurance |
                             effect.EffectType == Enums.eEffectType.Damage) &
                            _enhancedPower.Effects[shortFxArray1[index].Index[0]].Aspect == Enums.eAspect.Cur)
                        {
                            num2 *= 100f;
                            num3 *= 100f;
                        }
                    }
                    else
                    {
                        switch (_enhancedPower.Effects[shortFxArray1[index].Index[0]].EffectType)
                        {
                            case Enums.eEffectType.Heal:
                                suffix = " HP";
                                break;
                            case Enums.eEffectType.HitPoints:
                                suffix = " HP";
                                break;
                        }
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

                flag = true;
            }

            return flag;
        }

        private void PairedList_Hover(object sender, Enums.ShortFX tag, string tooltip)
        {
            var empty1 = string.Empty;
            var str1 = string.Empty;
            if (tag.Present)
            {
                var empty2 = string.Empty;
                if (_enhancedPower != null)
                {
                    IPower power = new Power(_enhancedPower);
                    if (tag.Index != null)
                    {
                        foreach (var t in tag.Index)
                        {
                            if (t == -1 || power.Effects[t].EffectType == Enums.eEffectType.None)
                            {
                                continue;
                            }

                            var empty3 = string.Empty;
                            var returnMask = Array.Empty<int>();
                            power.GetEffectStringGrouped(t, ref empty3, ref returnMask, false, false);
                            if (returnMask.Length <= 0)
                            {
                                continue;
                            }

                            if (empty2 != string.Empty)
                            {
                                empty2 += "\r\n";
                            }

                            empty2 += empty3;
                            foreach (var m in returnMask)
                            {
                                power.Effects[m].EffectType = Enums.eEffectType.None;
                            }
                        }

                        foreach (var t in tag.Index)
                        {
                            if (power.Effects[t].EffectType == Enums.eEffectType.None)
                            {
                                continue;
                            }

                            if (empty2 != string.Empty)
                            {
                                empty2 += "\r\n";
                            }

                            empty2 += power.Effects[t].BuildEffectString();
                        }
                    }
                }

                str1 = empty1 + empty2;
            }
            else if (string.IsNullOrWhiteSpace(tooltip))
            {
                str1 = string.Empty;
            }
            else
            {
                str1 = tooltip;
            }

            if (!string.IsNullOrWhiteSpace(str1))
            {
                infoTip.SetToolTip((Control) sender, str1);
            }
            else
            {
                infoTip.SetToolTip((Control) sender, string.Empty);
            }
        }

        private void PairedList_ItemOut(object sender)
        {
            infoTip.SetToolTip((Control)sender, string.Empty);
        }
    }
}
