using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mids_Reborn.UI.Controls.Test;

public static class PowerCanonicalStats
{
    private const double Eps = 1e-6;

    // How an enhancement fraction f transforms the base value:
    private enum ApplyMode
    {
        None,           // value = base
        MultiplyUp,     // value = base * (1 + f)        e.g., Range, Accuracy
        MultiplyDown,   // value = base * (1 - f)        e.g., Interrupt, EndCost
        DivideByUp      // value = base / (1 + f)        e.g., Recharge time
    }

    private sealed class StatDef
    {
        public string Label { get; }
        public string Unit { get; }
        public bool HigherIsBetter { get; }
        public ApplyMode Mode { get; }
        public Enums.eEnhance? EdKind { get; }        // null => no ED section
        public Func<IPower, bool> Predicate { get; }  // should this stat show?
        public Func<IPower, double> Select { get; }   // read "as received" from IPower
        public bool DisplayIsPercent { get; }         // if true, grid shows value*100
        public bool HideGainPercent { get; }          // grid hides Δ%
        public string? GainUnitOverride { get; }      // e.g., "pp" for Accuracy

        public StatDef(
            string label, string unit, bool higherIsBetter, ApplyMode mode,
            Enums.eEnhance? edKind,
            Func<IPower, bool> predicate,
            Func<IPower, double> select,
            bool displayIsPercent = false,
            bool hideGainPercent = false,
            string? gainUnitOverride = null)
        {
            Label = label;
            Unit = unit;
            HigherIsBetter = higherIsBetter;
            Mode = mode;
            EdKind = edKind;
            Predicate = predicate;
            Select = select;
            DisplayIsPercent = displayIsPercent;
            HideGainPercent = hideGainPercent;
            GainUnitOverride = gainUnitOverride;
        }
    }

    // The canonical catalog in display order.
    private static IEnumerable<StatDef> Catalog(IPower p)
    {
        bool isToggle = p.PowerType == Enums.ePowerType.Toggle;
        bool isAuto = p.PowerType == Enums.ePowerType.Auto_;

        // Arc (no ED)
        yield return new StatDef(
            label: "Arc", unit: "°", higherIsBetter: true,
            mode: ApplyMode.None, edKind: null,
            predicate: x => x.Arc > Eps,
            select: x => x.Arc);

        // Radius (ED schedule: Range)
        yield return new StatDef(
            label: "Radius", unit: "ft", higherIsBetter: true,
            mode: ApplyMode.MultiplyUp, edKind: null,
            predicate: x => x.Radius > Eps && !IsCone(x),  // <— add !IsCone(x)
            select: x => x.Radius);

        // Range (ED: Range)
        yield return new StatDef(
            label: "Range", unit: "ft", higherIsBetter: true,
            mode: ApplyMode.MultiplyUp, edKind: Enums.eEnhance.Range,
            predicate: x => x.Range > Eps,
            select: x => x.Range);

        // Cast time (no ED)
        yield return new StatDef(
            label: "Cast Time", unit: "s", higherIsBetter: false,
            mode: ApplyMode.None, edKind: null,
            predicate: x => !isAuto && x.PowerType != Enums.ePowerType.Toggle && x.CastTime > Eps,
            select: x => x.CastTime);

        // Interrupt time (ED: Interrupt)
        yield return new StatDef(
            label: "Interrupt", unit: "s", higherIsBetter: false,
            mode: ApplyMode.MultiplyDown, edKind: Enums.eEnhance.Interrupt,
            predicate: x => !isAuto && x.PowerType != Enums.ePowerType.Toggle && x.InterruptTime > Eps,
            select: x => x.InterruptTime);

        yield return new StatDef(
            label: "Root Time", unit: "s", higherIsBetter: false,
            mode: ApplyMode.None, edKind: null,
            predicate: x => !isAuto && x.RootTime > Eps,
            select: x => x.RootTime);

        // Toggle tick period (no ED)
        yield return new StatDef(
            label: "Activate", unit: "s", higherIsBetter: false,
            mode: ApplyMode.None, edKind: null,
            predicate: x => isToggle && x.ActivatePeriod > Eps,
            select: x => x.ActivatePeriod);

        // Recharge (ED: RechargeTime) – lower is better
        yield return new StatDef(
            label: "Recharge", unit: "s", higherIsBetter: false,
            mode: ApplyMode.DivideByUp, edKind: Enums.eEnhance.RechargeTime,
            predicate: x => x.RechargeTime > Eps,
            select: x => x.RechargeTime);

        // End Cost (ED: EnduranceDiscount) – lower is better
        yield return new StatDef(
            label: "End Cost", unit: isToggle ? "/s" : "End", higherIsBetter: false,
            mode: ApplyMode.MultiplyDown, edKind: Enums.eEnhance.EnduranceDiscount,
            predicate: x => isToggle ? x.ToggleCost > Eps : x.EndCost > Eps,
            select: x => isToggle ? x.ToggleCost : x.EndCost);

        // Accuracy (ED: Accuracy). Show as percent in the grid.
        // EXACTLY like the old DisplayInfo: base shows (ScalingToHit * pBase.Accuracy),
        // enhanced shows pEnh.Accuracy as provided by the model.
        yield return new StatDef(
            label: "Accuracy", unit: "%", higherIsBetter: true,
            mode: ApplyMode.MultiplyUp, edKind: Enums.eEnhance.Accuracy,
            predicate: x => !isAuto || x.Accuracy > Eps,
            select: x => x.Accuracy,
            displayIsPercent: true,
            hideGainPercent: true,
            gainUnitOverride: "%");
    }

    // Public entrypoint — descriptor-driven
    internal static IEnumerable<PowerStatsGrid.Row> BuildRows(
        IPower? pBase,
        IPower? pEnh,
        CalculationContributionSnapshot? contributions = null,
        int historyIndex = -1)
    {
        if (pBase == null) yield break;

        var pe = TryGetPowerEntry(pBase, pEnh, historyIndex);
        bool isInBuild = pe != null;

        // If pEnh is missing or unresolved, fall back to pBase.
        if (pEnh == null || pEnh.PowerIndex == -1) pEnh = pBase;

        // Cache ED per kind so Range & Radius reuse the same computed impact.
        var edCache = new Dictionary<Enums.eEnhance, EdImpact>();
        EdImpact GetEd(Enums.eEnhance kind)
        {
            if (!edCache.TryGetValue(kind, out var impact))
            {
                impact = ComputeEdImpact(pe, kind);
                edCache[kind] = impact;
            }
            return impact;
        }

        foreach (var def in Catalog(pBase))
        {
            if (!def.Predicate(pBase)) continue;

            // --- Base column: always from pBase as delivered by clsToonX ---
            double baseVal = def.Select(pBase);

            // --- Enhanced column: only show pEnh values AFTER the power is actually in the build ---
            double enhancedVal = isInBuild ? def.Select(pEnh) : baseVal;

            // Accuracy must be aligned with how your pipeline scales:
            //   Base = ScalingToHit * pBase.Accuracy
            //   Enhanced = pEnh.Accuracy (already scaled) — but only once the power is in the build.
            if (def.Label == "Accuracy")
            {
                double baseAcc = MidsContext.Config.ScalingToHit * pBase.Accuracy;
                double enhAcc = isInBuild ? pEnh.Accuracy : baseAcc;

                baseVal = baseAcc;
                enhancedVal = enhAcc;
            }

            // ED context (fractions) for this stat if applicable
            bool edActive = false;
            int edBand = -1;
            EdImpact ed = new EdImpact { Active = false, BandIndex = -1 };

            if (def.EdKind.HasValue)
            {
                ed = GetEd(def.EdKind.Value);
                edActive = ed.Active;
                edBand = edActive ? ed.BandIndex : -1;
            }

            // Unified tooltip (ED + per-source contributions)
            string tip = BuildTooltip(
                label: def.Label,
                baseVal: baseVal,
                shownVal: enhancedVal,
                unit: def.Unit,
                displayAsPercent: def.DisplayIsPercent,
                mode: def.Mode,
                higherIsBetter: def.HigherIsBetter,
                ed: ed,
                contributions: contributions);

            // Friendly extras for non-ED stats
            if (!def.EdKind.HasValue)
            {
                if (def.Label == "Activate")
                {
                    tip = tip + (string.IsNullOrEmpty(tip) ? "" : "\r\n") +
                          "Interval at which the toggle applies its effects.";
                }
                else if (def.Label == "Cast Time")
                {
                    tip = tip + (string.IsNullOrEmpty(tip) ? "" : "\r\n\r\n") +
                          $"Details:\r\nCast Time: {pEnh.CastTimeBase:0.###} sec\r\nArcana Cast Time: {pEnh.ArcanaCastTime:0.###} sec";
                }
                else if (def.Label == "Root Time")
                {
                    tip = tip + (string.IsNullOrEmpty(tip) ? "" : "\r\n\r\n") +
                          "Time the caster remains rooted while activating this power.";

                    var policyTooltip = EnhancementPolicyAxes.GetStatPolicyTooltip(pBase, "Root Time");
                    if (!string.IsNullOrWhiteSpace(policyTooltip))
                    {
                        tip = tip + "\r\n\r\n" + policyTooltip;
                    }
                }
            }

            bool isConeHere = IsCone(pBase);

            // Replace tooltip for Circle-radius row (TAoE/PBAoE). Radius uses the Range ED schedule.
            if (def.Label == "Radius" && !isConeHere)
            {
                tip = CircleTooltip(
                    baseRadiusFt: baseVal,
                    shownRadiusFt: enhancedVal);
            }

            // Replace tooltip for Cone-range row
            if (def.Label == "Range" && isConeHere)
            {
                // 'ed' already corresponds to Range here (def.EdKind == Range for Range row)
                tip = ConeTooltipWithEd(
                    arcDeg: pBase.Arc,
                    baseLenFt: baseVal,
                    shownLenFt: enhancedVal,
                    ed: ed,
                    mode: def.Mode); // MultiplyUp
            }

            if (def.Label == "Recharge")
            {
                tip = AppendSharedRechargeTooltip(pBase, tip);
            }

            // Grid display: convert to % only for stats flagged as DisplayIsPercent (e.g., Accuracy)
            double displayBase = def.DisplayIsPercent ? baseVal * 100.0 : baseVal;
            double displayEnh = def.DisplayIsPercent ? enhancedVal * 100.0 : enhancedVal;

            yield return new PowerStatsGrid.Row(
                label: def.Label,
                baseValue: displayBase,
                enhancedValue: displayEnh,
                unit: def.Unit,
                higherIsBetter: def.HigherIsBetter,
                tooltip: tip,
                affectedByEd: edActive,
                hideGainPercent: def.HideGainPercent,
                gainUnitOverride: def.GainUnitOverride,
                neutralWhenZero: true,
                band: edBand);
        }
    }

    // ---------- shared helpers ----------

    private static double Apply(ApplyMode mode, double @base, double frac)
    {
        return mode switch
        {
            ApplyMode.None => @base,
            ApplyMode.MultiplyUp => @base * (1.0 + frac),
            ApplyMode.MultiplyDown => @base * Math.Max(0.0, 1.0 - frac),
            ApplyMode.DivideByUp => @base / Math.Max(1e-9, (1.0 + frac)),
            _ => @base
        };
    }

    private static double Benefit(bool higherIsBetter, double @base, double value)
        => higherIsBetter ? (value - @base) : (@base - value);

    private static string BandText(int bandIndex) => bandIndex switch
    {
        2 => "Strong (heavy diminishing returns)",
        1 => "Medium (moderate diminishing returns)",
        0 => "Light (mild diminishing returns)",
        _ => "No ED applied"
    };

    private sealed class EdImpact
    {
        public bool Active;               // true only if postED < preED (fractions)
        public Enums.eSchedule Sched;
        public double PreED;              // FRACTION (e.g., 0.125 = +12.5%)
        public double PostED;             // FRACTION after ED
        public double ReducedAbs;         // PreED - PostED
        public double ReducedPct;         // (ReducedAbs / PreED)*100
        public int BandIndex;             // -1 none, 0/1/2
        public double BandThreshold;      // FRACTION
    }

    /// <summary>Compute ED for an enhancement kind using the power’s current slotting.</summary>
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

    private static PowerEntry? TryGetPowerEntry(IPower pBase, IPower? pEnh, int historyIndex)
    {
        var build = MidsContext.Character?.CurrentBuild;
        if (build == null) return null;

        if (historyIndex >= 0 && historyIndex < build.Powers.Count)
        {
            var historyEntry = build.Powers[historyIndex];
            if (historyEntry?.Power != null)
            {
                return historyEntry;
            }
        }

        return build.Powers.FirstOrDefault(x =>
            x?.Power != null &&
            (x.Power.PowerIndex == pBase.PowerIndex ||
             (pEnh != null && x.Power.PowerIndex == pEnh.PowerIndex)));
    }

    private static string BuildTooltip(
    string label,
    double baseVal,
    double shownVal,
    string unit,
    bool displayAsPercent,
    ApplyMode mode,
    bool higherIsBetter,
    in EdImpact ed,
    CalculationContributionSnapshot? contributions)
    {

        // ---- math helpers ----
        static string F(double v) =>
            Math.Abs(v) >= 100 ? v.ToString("0") :
            Math.Abs(v) >= 10 ? v.ToString("0.0") :
            Math.Abs(v) >= 1 ? v.ToString("0.00") :
                                 v.ToString("0.000");

        static List<(ContributionSource Source, double Total)> SumBySource(
            params IEnumerable<(ContributionSource Source, ContributionChannel Channel, double Total)>[] groups)
            => groups.SelectMany(g => g)
                .GroupBy(x => x.Source)
                .Select(g => (g.Key, g.Sum(v => v.Total)))
                .OrderByDescending(x => x.Item2)
                .ToList();

        // ---- base values & gains (natural units) ----
        double beforeEdVal = Apply(mode, baseVal, ed.PreED);   // enhancements before ED
        double afterEdVal = Apply(mode, baseVal, ed.PostED);  // enhancements after ED
        double finalVal = shownVal;                         // includes powers/globals

        // "gains" are signed and respect whether higher is better
        double preEdGain = Benefit(higherIsBetter, baseVal, beforeEdVal);
        double postEdGain = Benefit(higherIsBetter, baseVal, afterEdVal);
        double otherGain = Benefit(higherIsBetter, afterEdVal, finalVal);
        double totalGain = Benefit(higherIsBetter, baseVal, finalVal);

        // display scaling
        double k = displayAsPercent ? 100.0 : 1.0;
        string uValue = string.IsNullOrEmpty(unit) ? "" : $" {unit}";
        string uGain = displayAsPercent ? " %" : uValue;
        double dBase = baseVal * k;
        double dFinal = finalVal * k;
        double dPreEdGain = preEdGain * k;
        double dPostEdGain = postEdGain * k;
        double dOtherGain = otherGain * k;
        double dTotalGain = totalGain * k;

        // ---- build lines in the requested format ----
        var lines = new List<string>(64) { "Detailed Readout:", $"• Base: {F(dBase)}{uValue}" };

        // From Enhancements
        if (ed.PreED > Eps) // we have slotting
        {
            if (ed.Active)  // ED actually trims
            {
                string bandText = BandText(ed.BandIndex);
                double trimmedGain = Math.Max(0.0, (preEdGain - postEdGain) * k);

                lines.Add("⤷ From Enhancements");
                lines.Add($"    • Pre-ED: {(dPreEdGain >= 0 ? "+" : "−")}{F(Math.Abs(dPreEdGain))}{uGain}");
                lines.Add($"    • {bandText} reduced this by {F(trimmedGain)}{uGain}");
                lines.Add($"    • Total (Post-ED): {(dPostEdGain >= 0 ? "+" : "−")}{F(Math.Abs(dPostEdGain))}{uGain}");
            }
            else
            {
                // ED is not limiting; put the post-ED (== pre-ED) gain inline
                lines.Add($"⤷ From Enhancements: {(dPostEdGain >= 0 ? "+" : "−")}{F(Math.Abs(dPostEdGain))}{uGain}");
            }
        }
        // else: no slotting; omit the Enhancements section entirely

        // From Powers & Globals (only when something actually contributes)
        var detail = new List<string>(32);

        if (label == "Recharge")
        {
            // value = base / (1 + H). Merge Haste & RechargeTime.
            var haste = GetSummedContributions(contributions, ContributionBucket.Effect, (int)Enums.eStatType.Haste);
            var rech = GetSummedContributions(contributions, ContributionBucket.Effect, (int)Enums.eEffectType.RechargeTime);
            var items = SumBySource(haste, rech);

            if (items.Count > 0)
            {
                double H = items.Sum(x => x.Total);
                if (Math.Abs(H) > 1e-12)
                {
                    detail.Add("⤷ From Globals, Incarnates, and Powers");
                    foreach (var (source, frac) in items)
                    {
                        // Per-source contribution in *seconds saved* (positive number).
                        // Δ_i = base * ( 1/(1 + H - f_i) - 1/(1 + H) )
                        double delta = baseVal * (1.0 / (1.0 + H - frac) - 1.0 / (1.0 + H));
                        detail.Add($"    • {source.Name}: {(higherIsBetter ? "+" : "−")}{F(delta)} s");
                    }
                }
            }
        }
        else if (label == "End Cost")
        {
            // value = base * (1 − F). Merge BuffEndRdx & EnduranceDiscount.
            var rdx = GetSummedContributions(contributions, ContributionBucket.Effect, (int)Enums.eStatType.BuffEndRdx);
            var endd = GetSummedContributions(contributions, ContributionBucket.Effect, (int)Enums.eEffectType.EnduranceDiscount);
            var items = SumBySource(rdx, endd);

            if (items.Count > 0)
            {
                detail.Add("⤷ From Globals, Incarnates, and Powers");
                foreach (var (source, frac) in items)
                {
                    double delta = baseVal * frac; // absolute End saved
                    detail.Add($"    • {source.Name}: {(higherIsBetter ? "+" : "−")}{F(delta)}{uValue}");
                }
            }
        }
        else if (label == "Accuracy")
        {
            // Show as *percentage points*. Merge BuffAcc & ToHit.
            var acc = GetSummedContributions(contributions, ContributionBucket.Effect, (int)Enums.eStatType.BuffAcc);
            var tohit = GetSummedContributions(contributions, ContributionBucket.Effect, (int)Enums.eStatType.ToHit);
            var items = SumBySource(acc, tohit);

            if (items.Count > 0)
            {
                detail.Add("⤷ From Globals, Incarnates, and Powers");
                foreach (var (source, frac) in items)
                {
                    double pp = frac * 100.0;
                    detail.Add($"    • {source.Name}: +{F(pp)} pp");
                }
            }
        }

        if (detail.Count > 0)
            lines.AddRange(detail);
        // otherwise: omit the "Powers & Globals" block entirely

        // Total (only include "(±X vs Base)" when there is a real change)
        if (Math.Abs(dTotalGain) > (displayAsPercent ? 0.005 : 0.0005))
        {
            string signTotal = dTotalGain >= 0 ? "+" : "−";
            lines.Add($"• Total: {F(dFinal)}{uValue} ({signTotal}{F(Math.Abs(dTotalGain))}{uGain} vs Base)");
        }
        else
        {
            lines.Add($"• Total: {F(dFinal)}{uValue}");
        }

        return string.Join("\r\n", lines);
    }

    private static IEnumerable<(ContributionSource Source, ContributionChannel Channel, double Total)> GetSummedContributions(
        CalculationContributionSnapshot? contributions,
        ContributionBucket bucket,
        int index)
    {
        return contributions?.GetSummed(bucket, index) ??
               ContributionTracker.GetSummed(bucket, index);
    }

    private static string F0(double v) => Math.Round(v).ToString("0");     // integers (ft², etc.)
    private static string F1(double v) => v.ToString("0.0");               // one decimal (ft)
    private static string F2(double v) => v.ToString("0.##");              // up to two decimals

    // Builds the player-friendly tooltip for CONES, including ED details inline.
    private static string ConeTooltipWithEd(double arcDeg, double baseLenFt, double shownLenFt, in EdImpact ed, ApplyMode mode)
    {
        static double R1(double v) => Math.Round(v, 1, MidpointRounding.AwayFromZero);
        static double R0(double v) => Math.Round(v, 0, MidpointRounding.AwayFromZero);

        // Geometry
        double arcRad = arcDeg * Math.PI / 180.0;
        double widthBase = 2.0 * baseLenFt * Math.Tan(arcRad / 2.0);
        double widthShown = 2.0 * shownLenFt * Math.Tan(arcRad / 2.0);

        double areaBase = 0.5 * arcRad * baseLenFt * baseLenFt;
        double areaShown = 0.5 * arcRad * shownLenFt * shownLenFt;

        // ED math (mirror your BuildTooltip math)
        double preEdVal = Apply(mode, baseLenFt, ed.PreED);
        double postEdVal = Apply(mode, baseLenFt, ed.PostED);
        double preGain = preEdVal - baseLenFt;   // absolute ft before ED
        double postGain = postEdVal - baseLenFt;   // absolute ft after ED (this is the "+X ft" from slotting)
        double trimmedAbs = Math.Max(0.0, preGain - postGain);
        string bandText = BandText(ed.BandIndex);

        // Overall gains
        double gLen = shownLenFt - baseLenFt;
        double gLenPc = (Math.Abs(baseLenFt) < 1e-9) ? 0.0 : (gLen / baseLenFt) * 100.0;
        double gW = widthShown - widthBase;
        double gA = areaShown - areaBase;

        bool lengthChanged = Quantize(shownLenFt) != Quantize(baseLenFt);

        // Compose 
        var sb = new System.Text.StringBuilder(256);
        sb.AppendLine("Detailed Readout:");
        sb.AppendLine($"• Shape: Cone → {arcDeg:0.#}° Arc to {F1(shownLenFt)} ft");

        if (lengthChanged)
        {
            sb.AppendLine($"  ⤷ From Base: {F1(baseLenFt)} ft");
            sb.AppendLine($"  ⤷ From Enhancements: +{F1(postGain)} ft");
            if (ed.Active)
            {
                sb.AppendLine($"        • Pre-ED: {F1(preGain)} ft");
                if (ed.Active && trimmedAbs > 1e-6)
                    sb.AppendLine($"        • {bandText} reduced this by {F1(trimmedAbs)} ft");
            }
        }

        if (lengthChanged)
        {
            sb.AppendLine($"• Width at Max Reach: ~{F1(widthShown)} ft (base {F1(widthBase)} ft)");
            sb.AppendLine($"• Coverage Area: ~{F0(areaShown)} ft² (base {F0(areaBase)} ft²)");
        }
        else
        {
            sb.AppendLine($"• Width at Max Reach: ~{F1(widthShown)} ft");
            sb.AppendLine($"• Coverage Area: ~{F0(areaShown)} ft²");
        }

        if (!lengthChanged) return sb.ToString();
        sb.AppendLine();
        sb.AppendLine("★ Gain:");
        sb.AppendLine($"  ⤷ Length: +{F1(gLen)} ft (+{gLenPc:0.#}%)");
        sb.AppendLine($"  ⤷ Width:  +{F1(gW)} ft");
        sb.Append($"  ⤷ Area:   +{F0(gA)} ft²");

        return sb.ToString();
    }

    // Builds the player-friendly tooltip for CIRCLES (TAoE/PBAoE).
    private static string CircleTooltip(double baseRadiusFt, double shownRadiusFt)
    {
        double areaBase = Math.PI * baseRadiusFt * baseRadiusFt;
        double areaShown = Math.PI * shownRadiusFt * shownRadiusFt;

        var sb = new System.Text.StringBuilder(256);
        sb.AppendLine("Detailed Readout:");
        sb.AppendLine($"• Shape: Circle → {F1(shownRadiusFt)} ft");
        sb.AppendLine($"• Coverage Area: ~{F0(areaShown)} ft²");

        return sb.ToString();
    }

    private static string AppendSharedRechargeTooltip(IPower power, string tooltip)
    {
        var sharedRechargeTooltip = BuildSharedRechargeTooltip(power);
        if (string.IsNullOrWhiteSpace(sharedRechargeTooltip))
        {
            return tooltip;
        }

        return string.IsNullOrWhiteSpace(tooltip)
            ? sharedRechargeTooltip
            : $"{tooltip}\r\n\r\n{sharedRechargeTooltip}";
    }

    private static string BuildSharedRechargeTooltip(IPower power)
    {
        var groups = power.RechargeGroups
            .Where(group => !string.IsNullOrWhiteSpace(group))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (groups.Length == 0)
        {
            return string.Empty;
        }

        var lines = new List<string>
        {
            $"Shared recharge groups: {string.Join(", ", groups.Select(HumanizeRechargeGroup))}"
        };

        var linkedPowers = (DatabaseAPI.Database?.Power ?? Array.Empty<IPower>())
            .Where(other => other != null &&
                            !string.Equals(other.FullName, power.FullName, StringComparison.OrdinalIgnoreCase) &&
                            other.RechargeGroups.Any(group => groups.Contains(group, StringComparer.OrdinalIgnoreCase)))
            .Select(other => other.DisplayName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .Take(6)
            .ToArray();

        if (linkedPowers.Length > 0)
        {
            lines.Add($"Also linked to: {string.Join(", ", linkedPowers)}");
        }

        return string.Join("\r\n", lines);
    }

    private static string HumanizeRechargeGroup(string group)
    {
        return string.IsNullOrWhiteSpace(group)
            ? string.Empty
            : group.Replace('_', ' ').Trim();
    }

    private static bool IsCone(IPower p) => p.Arc > Eps && p.Range > Eps;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Quantize(double feet, int decimals = 1)
    {
        // 1 decimal place -> factor 10
        var factor = (int)Math.Pow(10, decimals);
        return (int)Math.Round(feet * factor, MidpointRounding.AwayFromZero);
    }
}
