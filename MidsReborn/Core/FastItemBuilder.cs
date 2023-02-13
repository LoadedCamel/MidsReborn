using Mids_Reborn.Controls;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Globalization;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Mids_Reborn.Core
{
    public static class FastItemBuilder
    {
        public static class Str
        {

            public static string ShortStr(float fontSize, string full, string brief)
            {
                // fontSize == info_DataList.Font.Size
                return fontSize <= 100f / full.Length ? full : brief;
            }

            public static string CapString(string str, int capLength)
            {
                return str.Length >= capLength ? str[..capLength] : str;
            }
        }

        public static PairedList.ItemPair GetRankedEffect(int[] index, int id, IPower pBase, IPower pEnh)
        {
            var title = string.Empty;
            var shortFxBase = new Enums.ShortFX();
            var shortFxEnh = new Enums.ShortFX();
            var tag2 = new Enums.ShortFX();
            var suffix = string.Empty;
            var enhancedPower = pEnh ?? pBase;
            var fx = pEnh != null && index[id] < pEnh.Effects.Length
                ? pEnh.Effects[index[id]]
                : index[id] < pBase.Effects.Length
                    ? pBase.Effects[index[id]]
                    : null;

            var fx2 = id <= 0
                ? null
                : pEnh != null && index[id - 1] < pEnh.Effects.Length
                    ? pEnh.Effects[index[id - 1]]
                    : index[id - 1] < pBase.Effects.Length
                        ? pBase.Effects[index[id - 1]]
                        : null;

            if (fx == null)
            {
                return FI.FastItem("", 0f, 0f, string.Empty);
            }

            if (index[id] > -1)
            {
                var flag = false;
                var onlySelf = fx.ToWho == Enums.eToWho.Self;
                var onlyTarget = fx.ToWho == Enums.eToWho.Target;
                if (id > 0)
                {
                    flag = (fx.EffectType == fx2.EffectType) &
                           (fx.ToWho == Enums.eToWho.Self) &
                           (fx2.ToWho == Enums.eToWho.Self) &
                           (fx.ToWho == Enums.eToWho.Target);
                }

                if (fx.DelayedTime > 5)
                {
                    flag = true;
                }

                var names = Enum.GetNames(typeof(Enums.eEffectTypeShort));
                if (fx.EffectType == Enums.eEffectType.Enhancement)
                {
                    title = fx.ETModifies switch
                    {
                        Enums.eEffectType.EnduranceDiscount => "+EndRdx",
                        Enums.eEffectType.RechargeTime => "+Rechg",
                        Enums.eEffectType.Mez => fx.MezType == Enums.eMez.None
                            ? "+Effects"
                            : $"Enh({Enum.GetName(Enums.eMezShort.None.GetType(), fx.MezType)})",
                        Enums.eEffectType.Defense => "Enh(Def)",
                        Enums.eEffectType.Resistance => "Enh(Res)",
                        _ => Str.CapString(Enum.GetName(fx.ETModifies.GetType(), fx.ETModifies), 7)
                    };

                    shortFxBase.Assign(pBase.GetEffectMagSum(fx.EffectType,
                        fx.ETModifies, fx.DamageType,
                        fx.MezType, false, onlySelf, onlyTarget));

                    shortFxEnh.Assign(enhancedPower.GetEffectMagSum(enhancedPower.Effects[index[id]].EffectType,
                        enhancedPower.Effects[index[id]].ETModifies, enhancedPower.Effects[index[id]].DamageType,
                        enhancedPower.Effects[index[id]].MezType, false, onlySelf, onlyTarget));
                }
                else
                {
                    title = fx.EffectType != Enums.eEffectType.Mez
                        ? names[(int)fx.EffectType]
                        : Enums.GetMezName((Enums.eMezShort)fx.MezType);
                }

                var temp = string.Empty;
                switch (fx.EffectType)
                {
                    case Enums.eEffectType.HitPoints:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf,
                            onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.HitPoints, false, onlySelf,
                            onlyTarget));
                        tag2.Assign(shortFxBase);
                        shortFxBase.Sum = (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Heal:
                        if (fx.BuffedMag <= 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf,
                                onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Heal, false, onlySelf,
                                onlyTarget));
                            shortFxBase.Sum =
                                (float)(shortFxBase.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            shortFxEnh.Sum = (float)(shortFxEnh.Sum / (double)MidsContext.Archetype.Hitpoints * 100);
                            tag2.Assign(shortFxBase);
                        }

                        suffix = "%";
                        break;
                    case Enums.eEffectType.Absorb:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf,
                            onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Absorb, false, onlySelf,
                            onlyTarget));
                        var absorbPercent = pBase.Effects
                            .Where(e => e.EffectType == Enums.eEffectType.Absorb)
                            .Any(e => e.DisplayPercentage);
                        tag2.Assign(shortFxBase);
                        suffix = absorbPercent ? "%" : "";
                        break;
                    case Enums.eEffectType.Endurance:
                        if (fx.BuffedMag < -0.01 && fx.BuffedMag > -1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Endurance, false, onlySelf,
                                onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Endurance, false,
                                onlySelf, onlyTarget));
                            tag2.Assign(shortFxBase);
                        }

                        suffix = "%";
                        break;
                    case Enums.eEffectType.Regeneration:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf,
                            onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Regeneration, false, onlySelf,
                            onlyTarget));
                        shortFxEnh.Sum *= 100;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Null:
                        if (fx.BuffedMag < 1)
                        {
                            temp = $"{fx.BuffedMag:P2}";
                            shortFxBase.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            shortFxEnh.Add(index[id], Convert.ToSingle(temp.Replace("%", "")));
                            tag2.Assign(shortFxBase);
                        }
                        else
                        {
                            shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf,
                                onlyTarget));
                            shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Null, false, onlySelf,
                                onlyTarget));
                            tag2.Assign(shortFxBase);
                        }

                        suffix = "%";
                        break;
                    case Enums.eEffectType.ToHit:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.ToHit, false, onlySelf,
                            onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Fly:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf, onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Fly, false, onlySelf,
                            onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Recovery:
                        shortFxBase.Assign(pBase.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf,
                            onlyTarget));
                        shortFxEnh.Assign(enhancedPower.GetEffectMagSum(Enums.eEffectType.Recovery, false, onlySelf,
                            onlyTarget));
                        shortFxBase.Sum *= 100f;
                        shortFxEnh.Sum *= 100f;
                        tag2.Assign(shortFxBase);
                        suffix = "%";
                        break;
                    case Enums.eEffectType.Mez when fx.MezType is Enums.eMez.Taunt or Enums.eMez.Placate:
                        shortFxBase.Add(index[id], fx.Duration);
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
                        if (fx.EffectType != Enums.eEffectType.Enhancement)
                        {
                            shortFxBase.Add(index[id], fx.BuffedMag);
                            shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        }

                        shortFxBase.Multiply();
                        shortFxEnh.Multiply();

                        tag2.Assign(enhancedPower.GetEffectMagSum(fx.EffectType, false, onlySelf, onlyTarget));
                        break;
                    case Enums.eEffectType.SilentKill:
                        shortFxBase.Add(index[id], fx.Absorbed_Duration);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].Absorbed_Duration);
                        tag2.Assign(shortFxBase);
                        break;
                    default:
                        shortFxBase.Add(index[id], fx.BuffedMag);
                        shortFxEnh.Add(index[id], enhancedPower.Effects[index[id]].BuffedMag);
                        tag2.Assign(shortFxBase);
                        break;
                }

                if (fx.DisplayPercentage)
                {
                    suffix = "%";
                }

                suffix += fx.ToWho switch
                {
                    Enums.eToWho.Target => " (Tgt)",
                    Enums.eToWho.Self => " (Self)",
                    _ => ""
                };

                if (flag)
                {
                    return FI.FastItem("", 0f, 0f, string.Empty);
                }
            }

            for (var i = 0; i < shortFxEnh.Index.Length; i++)
            {
                var sFxIdx = shortFxEnh.Index[i];
                if (sFxIdx >= pBase.Effects.Length & sFxIdx >= pEnh.Effects.Length)
                {
                    continue;
                }

                var effect = sFxIdx < pBase.Effects.Length
                    ? pBase.Effects[sFxIdx]
                    : pEnh.Effects[sFxIdx];

                if (sFxIdx <= -1 || !effect.DisplayPercentage)
                {
                    continue;
                }

                if (shortFxEnh.Value[i] > 1)
                {
                    continue;
                }

                switch (effect.EffectType)
                {
                    case Enums.eEffectType.Absorb:
                        //Fixes the Absorb display to correctly show the percentage
                        shortFxEnh.Sum = float.Parse(shortFxEnh.Sum.ToString("P", CultureInfo.InvariantCulture)
                            .Replace("%", ""));
                        break;
                    case Enums.eEffectType.ToHit:
                        //Fixes the ToHit display to correctly show the percentage
                        if (effect.Stacking == Enums.eStacking.Yes)
                        {
                            var overage = fx.Ticks * 0.05f;
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

            // shortFxEnh.index.Length == 0 will occur if all effects of the same kind
            // have non validated conditionals.
            // E.g. -Recovery on Kick if Cross Punch has not been picked.
            var tip = shortFxEnh.Index.Length <= 0
                ? ""
                : pEnh.BuildTooltipStringAllVectorsEffects(pEnh.Effects[shortFxEnh.Index[0]].EffectType,
                    pEnh.Effects[shortFxEnh.Index[0]].ETModifies, pEnh.Effects[shortFxEnh.Index[0]].DamageType,
                    pEnh.Effects[shortFxEnh.Index[0]].MezType);

            if (fx.ActiveConditionals.Count > 0)
            {
                return FI.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1,
                    fx.ActiveConditionals.Count > 0, tip);
            }

            if (fx.SpecialCase != Enums.eSpecialCase.None)
            {
                return FI.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1,
                    fx.SpecialCase != Enums.eSpecialCase.None, tip);
            }

            return FI.FastItem(title, shortFxBase, shortFxEnh, suffix, true, false, fx.Probability < 1, false, tip);
        }

        // FastItem/ItemPair constructors
        public static class FI
        {

            public static PairedList.ItemPair FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix,
                Enums.ShortFX tag, IPower basePower)
            {
                return FastItem(title, s1, s2, suffix, false, false, false, false, tag, basePower);
            }

            public static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, string tip)
            {
                return FastItem(title, s1, s2, suffix, false, false, false, false, tip.Trim());
            }

            public static PairedList.ItemPair FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix,
                bool skipBase, bool alwaysShow, bool isChance, bool isSpecial, string tip)
            {
                var iValue = Utilities.FixDP(s2.Sum) + suffix;
                PairedList.ItemPair iItem;
                if ((Math.Abs(s1.Sum) < float.Epsilon) & !alwaysShow)
                {
                    iItem = new PairedList.ItemPair(string.Empty, string.Empty, false);
                }
                else if (Math.Abs(s1.Sum) < float.Epsilon)
                {
                    iItem = new PairedList.ItemPair($"{title}:", string.Empty, false);
                }
                else
                {
                    var iAlternate = false;
                    if (Math.Abs(s1.Sum - s2.Sum) > float.Epsilon)
                    {
                        if (!skipBase)
                        {
                            var iValue2 = $"({Utilities.FixDP(s2.Sum)}{suffix})";
                            iValue += iValue2.Replace("%", "");
                        }

                        iAlternate = true;
                    }

                    iItem = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
                }

                return iItem;
            }

            public static PairedList.ItemPair FastItem(string title, Enums.ShortFX s1, Enums.ShortFX s2, string suffix,
                bool skipBase, bool alwaysShow, bool isChance, bool isSpecial, Enums.ShortFX tag, IPower basePower)
            {
                var iValue = Utilities.FixDP(s2.Sum) + suffix;
                PairedList.ItemPair itemPair;
                if ((Math.Abs(s1.Sum) < float.Epsilon) & !alwaysShow)
                {
                    itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
                }
                else if (Math.Abs(s1.Sum) < float.Epsilon)
                {
                    itemPair = new PairedList.ItemPair($"{title}:", string.Empty, false);
                }
                else
                {
                    var iAlternate = false;
                    if (Math.Abs(s1.Sum - (double) s2.Sum) > float.Epsilon)
                    {
                        if (!skipBase)
                        {
                            iValue += $" ({Utilities.FixDP(s1.Sum)})";
                        }

                        iAlternate = true;
                    }

                    var tip = Tooltip.GenerateTipFromEffect(basePower, tag).Trim();
                    itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
                }

                return itemPair;
            }

            public static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, bool skipBase,
                bool alwaysShow, bool isChance, bool isSpecial, string tip)
            {
                var iValue = Utilities.FixDP(s2) + suffix;
                PairedList.ItemPair itemPair;
                if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
                {
                    itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
                }
                else if (Math.Abs(s1) < float.Epsilon)
                {
                    itemPair = new PairedList.ItemPair(title, string.Empty, false);
                }
                else
                {
                    var iAlternate = false;
                    if (Math.Abs(s1 - (double) s2) > float.Epsilon)
                    {
                        if (!skipBase)
                        {
                            iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                        }

                        iAlternate = true;
                    }

                    itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
                }

                return itemPair;
            }

            public static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix,
                bool skipBase = false, bool alwaysShow = false, bool isChance = false, bool isSpecial = false,
                int tagId = -1, int maxDecimal = -1)
            {
                var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + suffix : Utilities.FixDP(s2, maxDecimal) + suffix;
                PairedList.ItemPair itemPair;
                if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
                {
                    itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
                }
                else
                {
                    var iAlternate = false;
                    if (Math.Abs(s1 - (double) s2) > float.Epsilon)
                    {
                        if (!skipBase)
                        {
                            iValue = $"{iValue} ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                        }

                        iAlternate = true;
                    }

                    itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tagId);
                }

                return itemPair;
            }

            public static PairedList.ItemPair FastItem(string title, float s1, float s2, string suffix, string tip,
                bool skipBase = false, bool alwaysShow = false, bool isChance = false, bool isSpecial = false,
                int maxDecimal = -1)
            {
                var iValue = maxDecimal < 0 ? Utilities.FixDP(s2) + suffix : Utilities.FixDP(s2, maxDecimal) + suffix;
                PairedList.ItemPair itemPair;
                if ((Math.Abs(s1) < float.Epsilon) & !alwaysShow)
                {
                    itemPair = new PairedList.ItemPair(string.Empty, string.Empty, false);
                }
                else
                {
                    var iAlternate = false;
                    if (Math.Abs(s1 - (double) s2) > float.Epsilon)
                    {
                        if (!skipBase)
                        {
                            iValue += $" ({Utilities.FixDP(s1)}{(iValue.EndsWith("%") ? "%" : "")})";
                        }

                        iAlternate = true;
                    }

                    itemPair = new PairedList.ItemPair(title, iValue, iAlternate, isChance, isSpecial, tip.Trim());
                }

                return itemPair;
            }
        }

        public static class Tooltip
        {
            public static string GenerateTipFromEffect(IPower basePower, IEffect baseFx)
            {
                return (string.Join("\n",
                            basePower.Effects
                                .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                                .Where(e =>
                                    e.Value.EffectType == baseFx.EffectType &
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
                                    e.Value.EffectType == baseFx.EffectType &
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

            public static string GenerateTipFromEffect(IPower basePower, Enums.ShortFX tag)
            {
                var effects = tag.Index.Select(e => basePower.Effects[e]).ToList();
                var effectTypes = effects.Select(e => e.EffectType).ToList();
                var effectDmgTypes = effects.Select(e => e.DamageType).ToList();
                var effectETModifies = effects.Select(e => e.ETModifies).ToList();
                var effectMezTypes = effects.Select(e => e.MezType).ToList();

                return string.Join("\n",
                           basePower.Effects
                               .Select((e, i) => new KeyValuePair<int, IEffect>(i, e))
                               .Where(e =>
                                   effectTypes.Contains(e.Value.EffectType) &
                                   effectDmgTypes.Contains(e.Value.DamageType) &
                                   effectETModifies.Contains(e.Value.ETModifies) &
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
                                   effectTypes.Contains(e.Value.EffectType) &
                                   effectDmgTypes.Contains(e.Value.DamageType) &
                                   effectETModifies.Contains(e.Value.ETModifies) &
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
            public static string GenerateTipFromEffect(IPower basePower, Enums.eEffectType effectType,
                Enums.eDamage dmgType)
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
                                              basePower.GetDifferentAttributesSubPower(e.Key)).Replace(".,", ","))))
                    .Trim();
            }
        }
    }
}
