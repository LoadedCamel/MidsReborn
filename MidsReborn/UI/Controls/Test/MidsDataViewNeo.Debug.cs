using System.Globalization;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Data_Classes;

namespace Mids_Reborn.UI.Controls;

public partial class MidsDataViewNeo
{
    private static string BuildDebugDamageRowIdentity(IEffect effect)
    {
        return string.Join("|",
            effect.EffectType,
            effect.DamageType,
            effect.ToWho,
            effect.PvMode,
            effect.ModifierTable ?? string.Empty,
            effect.Scale.ToString("0.####", CultureInfo.InvariantCulture),
            effect.BuffedMag.ToString("0.####", CultureInfo.InvariantCulture),
            Power.GetDamageEffectEffectiveTicks(effect).ToString("0.####", CultureInfo.InvariantCulture),
            effect.OmniSource ?? string.Empty);
    }

    private static string BuildDebugGroupedIdentity(GroupedFx groupedFx, IPower power)
    {
        var firstIndex = groupedFx.GetRankedEffectIndex(power.GetRankedEffects(true), 0);
        return string.Join("|",
            groupedFx.EffectType,
            groupedFx.DamageType,
            groupedFx.ToWho,
            groupedFx.PvMode,
            groupedFx.NumEffects,
            firstIndex,
            groupedFx.GetTooltip(power, simple: true));
    }

    public DamageDisplayDebugSnapshot CreateDebugSnapshot()
    {
        var enhancedPower = (pEnh == null || pEnh.PowerIndex == -1) ? pBase : pEnh;
        var baseDamage = pBase == null
            ? 0f
            : Math.Abs(pBase.FXGetDamageValue(pBase.PowerIndex > -1 & pEnh?.PowerIndex > -1));
        var enhancedDamage = enhancedPower == null
            ? 0f
            : Math.Abs(enhancedPower.FXGetDamageValue());
        var baseDamageAbsorbTrue = pBase == null
            ? 0f
            : Math.Abs(pBase.FXGetDamageValue(absorb: true));
        var enhancedDamageAbsorbTrue = enhancedPower == null
            ? 0f
            : Math.Abs(enhancedPower.FXGetDamageValue(absorb: true));
        var displayText = infoDamageDisplay.Presentation.HasContent
            ? $"{infoDamageDisplay.Presentation.PrimaryText} | {infoDamageDisplay.Presentation.ModeBadgeText} | {infoDamageDisplay.Presentation.SubtitleText}"
            : string.Empty;

        return new DamageDisplayDebugSnapshot
        {
            ControlType = GetType().FullName ?? nameof(MidsDataViewNeo),
            BasePowerFullName = pBase?.FullName ?? string.Empty,
            EnhancedPowerFullName = enhancedPower?.FullName ?? string.Empty,
            BaseEffectCount = pBase?.Effects.Length ?? 0,
            EnhancedEffectCount = enhancedPower?.Effects.Length ?? 0,
            BaseDamage = baseDamage,
            EnhancedDamage = enhancedDamage,
            BaseDamageAbsorbTrue = baseDamageAbsorbTrue,
            EnhancedDamageAbsorbTrue = enhancedDamageAbsorbTrue,
            BaseDamageString = pBase?.FXGetDamageString() ?? string.Empty,
            EnhancedDamageString = enhancedPower?.FXGetDamageString() ?? string.Empty,
            BaseDamageStringAbsorbTrue = pBase?.FXGetDamageString(absorb: true) ?? string.Empty,
            EnhancedDamageStringAbsorbTrue = enhancedPower?.FXGetDamageString(absorb: true) ?? string.Empty,
            DisplayText = displayText,
            ToolTipText = infoDamageDisplay.ToolTipText ?? string.Empty,
            Branch = Math.Abs(enhancedDamage - baseDamage) > float.Epsilon ? "enhanced != base" : "enhanced == base",
            DisplayContainsDuplicateIdenticalTerms = DamageDisplayDebugSnapshot.DetectDuplicateIdenticalDamageTerms(infoDamageDisplay.ToolTipText),
            BaseHasAbsorbedRows = pBase?.Effects.Any(effect => effect.Absorbed_Effect) == true,
            EnhancedHasAbsorbedRows = enhancedPower?.Effects.Any(effect => effect.Absorbed_Effect) == true,
            DamageRowIdentities = enhancedPower == null
                ? Array.Empty<string>()
                : Power.GetIncludedDamageEffects(enhancedPower).Select(BuildDebugDamageRowIdentity).ToArray(),
            GroupedEffectIdentities = enhancedPower == null
                ? Array.Empty<string>()
                : GroupedRankedEffects.Select(gre => BuildDebugGroupedIdentity(gre, enhancedPower)).ToArray()
        };
    }
}
