using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System.Globalization;

namespace Mids_Reborn.UI.Controls.Test;

public static class PowerEffects
{
    private const double Eps = 1e-6;

    private sealed class EdImpact
    {
        public bool Active;               // true only if postED < preED
        public Enums.eSchedule Sched;
        public double PreED;              // FRACTION (e.g., 0.125 = +12.5%)
        public double PostED;             // FRACTION after ED
        public double ReducedAbs;         // PreED - PostED
        public double ReducedPct;         // (ReducedAbs / PreED)*100
        public int BandIndex;          // -1 none, 0/1/2
        public double BandThreshold;      // FRACTION
    }

    /// <summary>
    /// Build the groups/rows for the Effects grid (non-canonical stats + descriptors).
    /// </summary>
    /*public static List<PowerEffectsGrid.Group> Build(IPower pBase, IPower pEnh, List<GroupedFx> groupedRankedEffects, List<int> rankedEffects)
    {
        // 1) Build neutral, UI-agnostic effect items (labels/values/tooltips)
        //    (value = enhanced, alt = base) – straight from GroupedFx.
        var items = GroupedFx.GenerateEffectItems(groupedRankedEffects, pBase, pEnh, rankedEffects); // label/value source 

        // 2) Partition into the same buckets the legacy DataView uses.
        //    (We keep filters simple and readable; these mirror DisplayEffects groupings.)
        var groups = new List<PowerEffectsGrid.Group>();

        groups.Add(MakeGroup(
            "Defense / Resistance",
            Filter(items, id => id.EffectType is Enums.eEffectType.Defense or Enums.eEffectType.Resistance),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Heal / Endurance",
            Filter(items, id => id.EffectType is Enums.eEffectType.HitPoints or Enums.eEffectType.Heal
                                  or Enums.eEffectType.Absorb or Enums.eEffectType.Endurance or Enums.eEffectType.EnduranceDiscount),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Status",
            Filter(items, id => id.EffectType is Enums.eEffectType.Mez or Enums.eEffectType.MezResist),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Buff / Debuff",
            Filter(items, id => id.EffectType is Enums.eEffectType.ToHit or Enums.eEffectType.DamageBuff
                                  or Enums.eEffectType.RechargeTime or Enums.eEffectType.Enhancement
                                  or Enums.eEffectType.ResEffect),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Movement",
            Filter(items, id => id.EffectType is Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedJumping
                                  or Enums.eEffectType.SpeedFlying or Enums.eEffectType.JumpHeight
                                  or Enums.eEffectType.PerceptionRadius or Enums.eEffectType.StealthRadius
                                  or Enums.eEffectType.StealthRadiusPlayer),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Summon",
            Filter(items, id => id.EffectType is Enums.eEffectType.EntCreate),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Granted Powers",
            Filter(items, id => id.EffectType is Enums.eEffectType.GrantPower),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Modify Effect",
            Filter(items, id => id.EffectType is Enums.eEffectType.ModifyAttrib),
            pBase, pEnh, rankedEffects));

        groups.Add(MakeGroup(
            "Elusivity",
            Filter(items, id => id.EffectType is Enums.eEffectType.Elusivity),
            pBase, pEnh, rankedEffects));

        // Remove any empty groups to keep the grid clean.
        return groups.Where(g => g.Rows.Count > 0).ToList();
    }

    private static List<KeyValuePair<GroupedFx, EffectListItem>> Filter(List<KeyValuePair<GroupedFx, EffectListItem>> items, Func<GroupedFx.FxId, bool> predicate) => GroupedFx.FilterEffectItemsExt(items, predicate); // helper from GroupedFx*/

    private static PowerEffectsGrid.Group MakeGroup(
        string title,
        List<KeyValuePair<GroupedFx, EffectListItem>> items,
        IPower pBase,
        IPower pEnh,
        List<int> rankedEffects)
    {
        var rows = new List<PowerEffectsGrid.Row>();

        foreach (var kv in items)
        {
            var gre = kv.Key;
            var item = kv.Value;

            switch (gre.EffectType)
            {
                // --- Status (Mez) => split rows: Magnitude + Duration ---
                case Enums.eEffectType.Mez:
                {
                    // Sum magnitudes across the grouped effects
                    var magSum = gre.GetMagSum(pBase, pEnh);
                    double? bMag = magSum.Base;
                    double? eMag = magSum.Enhanced;

                    // Representative duration from the first ranked effect (as before)
                    var idx = gre.GetRankedEffectIndex(rankedEffects, 0);
                    double? bDur = null, eDur = null;
                    if (idx >= 0 && idx < pBase.Effects.Length && idx < pEnh.Effects.Length)
                    {
                        bDur = pBase.Effects[idx].Duration;
                        eDur = pEnh.Effects[idx].Duration;
                    }

                    // Decide if "higher is better" based on WHO is actually affected
                    var cohort = TargetingExtensions.ResolveCohort(gre.ToWho, pEnh.EntitiesAffected, pEnh.EntitiesAutoHit);
                    bool higherIsBetterForMez = cohort is TargetingExtensions.Cohort.Foe or TargetingExtensions.Cohort.Pet;

                    var ed = ComputeEdForFx(pEnh, gre, mezSubId: (int)gre.MezType);

                    rows.Add(new PowerEffectsGrid.MezRow(
                        label: item.Label,
                        baseMagnitude: bMag,
                        enhancedMagnitude: eMag,
                        baseDuration: bDur,
                        enhancedDuration: eDur,
                        higherIsBetterMagnitude: higherIsBetterForMez,
                        higherIsBetterDuration: higherIsBetterForMez,
                        affectedByEdMagnitude: false,
                        affectedByEdDuration: ed.Active,
                        neutralWhenZeroMagnitude: true,
                        neutralWhenZeroDuration: true,
                        hideGainPercentMagnitude: false,
                        hideGainPercentDuration: false,
                        bandMagnitude: -1,
                        bandDuration: ed.BandIndex,
                        tooltip: item.ToolTip));
                    break;
                }

                // --- Descriptor-only buckets (no numeric columns) ---
                case Enums.eEffectType.EntCreate:
                case Enums.eEffectType.GrantPower:
                case Enums.eEffectType.ModifyAttrib:
                {
                    rows.Add(new PowerEffectsGrid.DescriptorRow(
                        label: item.Label,
                        tag: TagForDescriptor(gre.EffectType),
                        description: item.ToolTip ?? item.Value,
                        tooltip: item.ToolTip));
                    break;
                }

                // --- Everything else => NumericRow using value (enh) and alt (base) from builder ---
                default:
                {
                    // Base/enh texts are already formatted by the builder – keep them.
                    var baseTxt = item.AltValue ?? "—";
                    var enhTxt = item.Value;

                    // Raw magnitude sums for correctness/banding
                    var asPercent = AsPercent(gre.EffectType);
                    var sum = gre.GetMagSum(pBase, pEnh);
                    double bRaw = sum.Base;
                    double eRaw = sum.Enhanced;

                    // Scale for display
                    double b = asPercent ? bRaw * 100.0 : bRaw;
                    double e = asPercent ? eRaw * 100.0 : eRaw;

                    // Cohort + semantics
                    var cohort =
                        TargetingExtensions.ResolveCohort(gre.ToWho, pEnh.EntitiesAffected, pEnh.EntitiesAutoHit);
                    var semanticType = (gre.ETModifies != Enums.eEffectType.None) ? gre.ETModifies : gre.EffectType;

                    bool higherIsBetter = IsHigherIsBetter(semanticType);
                    bool lowerIsBetter = IsLowerIsBetter(semanticType);

                    // Debuff detection for the aggregated row (enhanced value orientation is sufficient here)
                    bool isDebuff = IsDebuffForGroup(semanticType, cohort, eRaw);

                    // Does this benefit the player? (Self/Friend buff OR Foe/Pet debuff)
                    bool benefitsPlayer = BenefitsPlayer(cohort, isDebuff);

                    // Compute oriented gain: + always means "better for the player"
                    double gain = ComputePlayerGain(b, e, higherIsBetter, benefitsPlayer);

                    // % gain relative to base magnitude (use absolute to avoid sign illusion when base < 0)
                    string unitSuffix = asPercent ? "%" : "";
                    string gainTxt = FmtSigned(gain, unitSuffix);
                    string pctTxt = (Math.Abs(b) > 1e-9)
                        ? FmtSigned((gain / Math.Abs(b)) * 100.0, "%", showPlus: false)
                        : "—";

                    // ED banding
                    var ed = ComputeEdForFx(pEnh, gre, -1);

                    rows.Add(new PowerEffectsGrid.NumericRow(
                        label: item.Label,
                        baseText: baseTxt,
                        enhancedText: enhTxt,
                        gainText: (NearZero(gain) ? "—" : gainTxt),
                        gainPctText: (NearZero(gain) ? "—" : pctTxt),
                        higherIsBetter: higherIsBetter,
                        affectedByEd: ed.Active,
                        neutralWhenZero: true,
                        hideGainPercent: false,
                        band: ed.BandIndex,
                        tooltip: item.ToolTip));
                    break;
                }
            }
        }

        return new PowerEffectsGrid.Group(title, rows);
    }


    private static string FmtSigned(double v, string unit, bool showPlus = true)
    {
        var sign = v > 0 ? (showPlus ? "+" : "") : "";
        double abs = Math.Abs(v);
        string num = abs >= 100 ? abs.ToString("0", CultureInfo.InvariantCulture)
                  : abs >= 10 ? abs.ToString("0.0", CultureInfo.InvariantCulture)
                  : abs >= 1 ? abs.ToString("0.00", CultureInfo.InvariantCulture)
                               : abs.ToString("0.000", CultureInfo.InvariantCulture);
        return string.IsNullOrEmpty(unit) ? $"{sign}{num}" : $"{sign}{num}{unit}";
    }

    private static string TagForDescriptor(Enums.eEffectType t) => t switch
    {
        Enums.eEffectType.EntCreate => "Summon",
        Enums.eEffectType.GrantPower => "Grant Power",
        Enums.eEffectType.ModifyAttrib => "Modify Effect",
        _ => "Descriptor"
    };

    private static bool AsPercent(Enums.eEffectType t) =>
        t is Enums.eEffectType.Defense
           or Enums.eEffectType.Resistance
           or Enums.eEffectType.DamageBuff
           or Enums.eEffectType.ToHit
           or Enums.eEffectType.RechargeTime
           or Enums.eEffectType.Elusivity
           or Enums.eEffectType.MezResist
           or Enums.eEffectType.Enhancement;

    /// <summary>
    /// Compute ED activity/band for a grouped effect.
    /// </summary>
    private static EdImpact ComputeEdForFx(IPower pEnh, GroupedFx gre, int mezSubId)
    {
        // Try to map effect type to a matching eEnhance by name (DataView ED code uses the same mapping).
        Enums.eEnhance? kind;

        if (gre.EffectType == Enums.eEffectType.Mez)
        {
            kind = Enums.eEnhance.Mez;
        }
        else
        {
            try
            {
                // Most effect types that can be enhanced share names with eEnhance (e.g., Defense, Resistance, ToHit, RechargeTime, DamageBuff, etc.)
                kind = (Enums.eEnhance)Enum.Parse(typeof(Enums.eEnhance), gre.EffectType.ToString());
            }
            catch
            {
                // Not an ED-governed type
                kind = null;
            }
        }

        if (kind == null) return new EdImpact { Active = false, Sched = Enums.eSchedule.None, BandIndex = -1 };

        var pe = TryGetPowerEntry(pEnh);

        return ComputeEdImpact(pe, kind.Value, (kind == Enums.eEnhance.Mez) ? mezSubId : -1);
    }

    /// <summary>
    /// Mirrors the canonical ED computation (schedule, pre/post ED, band).
    /// </summary>
    private static EdImpact ComputeEdImpact(PowerEntry? pe, Enums.eEnhance kind, int subId = -1)
    {
        var impact = new EdImpact { Active = false, Sched = Enhancement.GetSchedule(kind, subId) };
        if (pe == null || impact.Sched == Enums.eSchedule.None) return impact;

        float pre = 0f;
        for (int i = 0; i < pe.Slots.Length; i++)
        {
            var enh = pe.Slots[i].Enhancement;
            if (enh == null) continue;
            pre += enh.GetEnhancementEffect(kind, subId, 1f);
        }
        if (pre <= 0f) return impact;

        float post = Enhancement.ApplyED(impact.Sched, pre);

        impact.PreED = pre;
        impact.PostED = post;

        var ed = DatabaseAPI.Database.MultED[(int)impact.Sched];
        int band = -1; double bandThresh = 0;
        if (pre > ed[2]) { band = 2; bandThresh = ed[2]; }
        else if (pre > ed[1]) { band = 1; bandThresh = ed[1]; }
        else if (pre > ed[0]) { band = 0; bandThresh = ed[0]; }

        impact.BandIndex = band;
        impact.BandThreshold = bandThresh;

        if (post + Eps < pre)
        {
            impact.Active = true;
            impact.ReducedAbs = pre - post;
            impact.ReducedPct = (pre > 0) ? ((pre - post) / pre) * 100.0 : 0.0;
        }

        return impact;
    }

    private static PowerEntry? TryGetPowerEntry(IPower p)
    {
        var build = MidsContext.Character?.CurrentBuild;
        if (build == null) return null;
        return build.Powers.FirstOrDefault(x => x?.Power != null && x.Power.PowerIndex == p.PowerIndex);
    }

    // Orientation maps (keep EnduranceDiscount as "higher is better")
    private static bool IsHigherIsBetter(Enums.eEffectType t) =>
        t is Enums.eEffectType.Defense or Enums.eEffectType.Resistance or Enums.eEffectType.Elusivity
        or Enums.eEffectType.ToHit or Enums.eEffectType.DamageBuff or Enums.eEffectType.Recovery
        or Enums.eEffectType.HitPoints or Enums.eEffectType.Heal or Enums.eEffectType.Absorb
        or Enums.eEffectType.PerceptionRadius or Enums.eEffectType.StealthRadius or Enums.eEffectType.StealthRadiusPlayer
        or Enums.eEffectType.SpeedRunning or Enums.eEffectType.SpeedFlying or Enums.eEffectType.SpeedJumping
        or Enums.eEffectType.JumpHeight or Enums.eEffectType.MezResist
        or Enums.eEffectType.EnduranceDiscount;

    private static bool IsLowerIsBetter(Enums.eEffectType t) =>
        t is Enums.eEffectType.RechargeTime or Enums.eEffectType.InterruptTime;

    private static bool NearZero(double v) => Math.Abs(v) < 1e-9;

    // +gain = better for the player, regardless of buff/debuff/cohort.
    private static double ComputePlayerGain(double baseVal, double enhVal, bool higherIsBetter, bool benefitsPlayer)
    {
        double oriented = higherIsBetter ? (enhVal - baseVal) : (baseVal - enhVal);
        return benefitsPlayer ? oriented : -oriented;
    }

    // Group-level debuff inference aligned with CoH semantics.
    private static bool IsDebuffForGroup(Enums.eEffectType semanticType, TargetingExtensions.Cohort cohort, double enhancedMag)
    {
        // Mez application: positive magnitude on non-friendly is a debuff
        if (semanticType == Enums.eEffectType.Mez)
            return cohort is TargetingExtensions.Cohort.Foe or TargetingExtensions.Cohort.Pet or TargetingExtensions.Cohort.All
                   && enhancedMag > 0.0;

        if (IsHigherIsBetter(semanticType))
            return cohort is TargetingExtensions.Cohort.Foe or TargetingExtensions.Cohort.Pet or TargetingExtensions.Cohort.All
                   && enhancedMag < 0.0;

        if (IsLowerIsBetter(semanticType))
            return cohort is TargetingExtensions.Cohort.Foe or TargetingExtensions.Cohort.Pet or TargetingExtensions.Cohort.All
                   && enhancedMag > 0.0;

        // Default
        return cohort is TargetingExtensions.Cohort.Foe or TargetingExtensions.Cohort.Pet or TargetingExtensions.Cohort.All
               && enhancedMag < 0.0;
    }

    // Self/Friend buff OR Foe/Pet debuff => benefits player
    private static bool BenefitsPlayer(TargetingExtensions.Cohort c, bool isDebuff) =>
        c switch
        {
            TargetingExtensions.Cohort.Self or TargetingExtensions.Cohort.Friend => !isDebuff,
            TargetingExtensions.Cohort.Foe or TargetingExtensions.Cohort.Pet => isDebuff,
            TargetingExtensions.Cohort.All => false,
            _ => false
        };

}