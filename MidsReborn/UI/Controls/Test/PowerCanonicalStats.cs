using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;

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
        public Enums.eEnhance? EdKind { get; }      // null => no ED applies
        public Func<IPower, bool> Predicate { get; }           // Should we show this stat for this power?
        public Func<IPower, double> BaseValue { get; }         // Base numeric value (in display unit)
        public bool DisplayIsPercent { get; }                   // true => BaseValue/Enhanced should be multiplied by 100 for display
        public bool HideGainPercent { get; }                    // grid: suppress Δ%
        public string? GainUnitOverride { get; }               // e.g., "pp" for Accuracy

        public StatDef(string label, string unit, bool higherIsBetter, ApplyMode mode,
                       Enums.eEnhance? edKind,
                       Func<IPower, bool> predicate,
                       Func<IPower, double> baseValue,
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
            BaseValue = baseValue;
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
            predicate: x => (x.Arc > Eps),
            baseValue: x => x.Arc);

        // Radius (scales by Range’s ED schedule)
        yield return new StatDef(
            label: "Radius", unit: "ft", higherIsBetter: true,
            mode: ApplyMode.MultiplyUp, edKind: Enums.eEnhance.Range,
            predicate: x => (x.Radius > Eps),
            baseValue: x => x.Radius);

        // Range (ED: Range)
        yield return new StatDef(
            label: "Range", unit: "ft", higherIsBetter: true,
            mode: ApplyMode.MultiplyUp, edKind: Enums.eEnhance.Range,
            predicate: x => (x.Range > Eps),
            baseValue: x => x.Range);

        // Cast time (no ED)
        yield return new StatDef(
            label: "Cast Time", unit: "s", higherIsBetter: false,
            mode: ApplyMode.None, edKind: null,
            predicate: x => !isAuto && x.PowerType != Enums.ePowerType.Toggle && x.CastTime > Eps,
            baseValue: x => x.CastTime);

        // Interrupt time (ED: Interrupt)
        yield return new StatDef(
            label: "Interrupt", unit: "s", higherIsBetter: false,
            mode: ApplyMode.MultiplyDown, edKind: Enums.eEnhance.Interrupt,
            predicate: x => !isAuto && x.PowerType != Enums.ePowerType.Toggle && x.InterruptTime > Eps,
            baseValue: x => x.InterruptTime);

        // Toggle tick period (no ED)
        yield return new StatDef(
            label: "Activate", unit: "s", higherIsBetter: false,
            mode: ApplyMode.None, edKind: null,
            predicate: x => isToggle && x.ActivatePeriod > Eps,
            baseValue: x => x.ActivatePeriod);

        // Recharge (ED: RechargeTime) – lower is better → DivideByUp
        yield return new StatDef(
            label: "Recharge", unit: "s", higherIsBetter: false,
            mode: ApplyMode.DivideByUp, edKind: Enums.eEnhance.RechargeTime,
            predicate: x => x.RechargeTime > Eps,
            baseValue: x => x.RechargeTime);

        // End Cost (ED: EnduranceDiscount) – lower is better
        yield return new StatDef(
            label: "End Cost", unit: isToggle ? "/s" : "", higherIsBetter: false,
            mode: ApplyMode.MultiplyDown, edKind: Enums.eEnhance.EnduranceDiscount,
            predicate: x => isToggle ? x.ToggleCost > Eps : x.EndCost > Eps,
            baseValue: x => isToggle ? x.ToggleCost : x.EndCost);

        // Accuracy (ED: Accuracy). Display as percent; Δ in percentage points (hide Gain%).
        yield return new StatDef(
            label: "Accuracy", unit: "%", higherIsBetter: true,
            mode: ApplyMode.MultiplyUp, edKind: Enums.eEnhance.Accuracy,
            predicate: x => !isAuto || (MidsContext.Config.ScalingToHit * x.Accuracy) > Eps,
            baseValue: x => MidsContext.Config.ScalingToHit * x.Accuracy,  // scalar (e.g., 0.866)
            displayIsPercent: true,
            hideGainPercent: true,
            gainUnitOverride: "%");
    }

    // Public entrypoint — descriptor-driven
    public static IEnumerable<PowerStatsGrid.Row> BuildRows(IPower? pBase, IPower? pEnh)
    {
        if (pBase == null) yield break;
        if (pEnh == null || pEnh.PowerIndex == -1) pEnh = pBase;

        var pe = TryGetPowerEntry(pBase);

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

            // Base value (scalar; % stats are FRACTIONS here)
            double baseVal = def.BaseValue(pBase);

            double f = 0.0;
            bool edActive = false;
            int edBand = -1;
            string? tip = null;

            if (def.EdKind.HasValue)
            {
                var ed = GetEd(def.EdKind.Value); // FRACTIONS: PreED/PostED
                f = ed.Active ? ed.PostED : ed.PreED;
                edActive = ed.Active;
                edBand = ed.Active ? ed.BandIndex : -1;

                // Human-friendly tooltip (works whether ED is active or not)
                tip = BuildFriendlyTooltip(
                    baseVal: baseVal,
                    unit: def.Unit,
                    displayAsPercent: def.DisplayIsPercent,
                    mode: def.Mode,
                    higherIsBetter: def.HigherIsBetter,
                    ed: ed);
            }
            else
            {
                // Optional simple tips for non-ED stats (keep concise)
                if (def.Label == "Activate")
                {
                    tip = "Interval at which the toggle applies its effects.";
                }
                else if (def.Label == "Cast Time")
                {
                    tip = $"Base animation/cast time for this activation.\r\n\r\nDetails:\r\nCast Time: {pEnh.CastTimeBase:0.###} sec\r\nArcana Cast Time: {pEnh.ArcanaCastTime:0.###} sec";
                }
            }

            // Apply the chosen formula to get Enhanced (post-ED when applicable)
            double enhScalar = Apply(def.Mode, baseVal, f);

            // For percent-display stats (Accuracy): multiply both by 100 for the grid.
            double displayBase = def.DisplayIsPercent ? baseVal * 100.0 : baseVal;
            double displayEnh = def.DisplayIsPercent ? enhScalar * 100.0 : enhScalar;

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
        public bool Active;               // true only if postED < preED
        public Enums.eSchedule Sched;
        public double PreED;              // FRACTION (e.g., 0.125 = +12.5%)
        public double PostED;             // FRACTION after ED
        public double ReducedAbs;         // PreED - PostED
        public double ReducedPct;         // (ReducedAbs / PreED)*100
        public int BandIndex;          // -1 none, 0/1/2
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

    // Compact, user-friendly tooltip:
    // - Shows "With enhancements" ONLY when something is slotted.
    // - Shows "After ED (shown)", "ED reduction", and "Band" ONLY when ED is active.
    // - Hides all ED jargon when unslotted.
    private static string BuildFriendlyTooltip(
        double baseVal,               // FRACTION for % stats; seconds/feet/etc otherwise
        string unit,
        bool displayAsPercent,
        ApplyMode mode,
        bool higherIsBetter,
        in EdImpact ed)
    {
        // Prepare “no-ED” and “with-ED” values
        double noEd = Apply(mode, baseVal, ed.PreED);
        double withEd = Apply(mode, baseVal, ed.PostED);

        // Gains in the user’s intuition
        double gainNoEd = Benefit(higherIsBetter, baseVal, noEd);
        double gainWithEd = Benefit(higherIsBetter, baseVal, withEd);

        // Display conversions
        double k = displayAsPercent ? 100.0 : 1.0;
        string u = string.IsNullOrEmpty(unit) ? "" : $" {unit}";
        string gainUnit = displayAsPercent ? " %" : u; // percentage-points for % stats

        static string F(double v) =>
            Math.Abs(v) >= 100 ? v.ToString("0") :
            Math.Abs(v) >= 10 ? v.ToString("0.0") :
            Math.Abs(v) >= 1 ? v.ToString("0.00") :
                                 v.ToString("0.000");

        var lines = new List<string>(8)
            {
                $"Base:               {F(baseVal * k)}{u}"
            };

        bool anySlotting = ed.PreED > Eps;

        if (anySlotting)
        {
            lines.Add($"With enhancements:  {F(noEd * k)}{u}");
            lines.Add($"Gain vs base:       {F(gainNoEd * k)}{gainUnit}");
        }
        else
        {
            lines.Add("Enhancements:       none");
        }

        if (ed.Active)
        {
            // Only show ED-specific parts when ED actually trimmed
            lines.Add($"After ED (shown):   {F(withEd * k)}{u}");
            lines.Add($"After-ED gain:      {F(gainWithEd * k)}{gainUnit}");
            double edLost = Math.Abs(gainNoEd - gainWithEd);
            lines.Add($"ED reduction:       {F(edLost * k)}{gainUnit}  ({F(ed.ReducedPct)}%)");

            var bandText = BandText(ed.BandIndex);
            if (!string.IsNullOrEmpty(bandText))
                lines.Add($"Band:               {bandText}");
        }
        else
        {
            // If something is slotted but ED not active, be explicit
            if (anySlotting)
                lines.Add("ED:                 none at this slotting");
            else
                lines.Add("ED:                 not applicable");
        }

        return string.Join("\r\n", lines);
    }

    private static PowerEntry? TryGetPowerEntry(IPower p)
    {
        var build = MidsContext.Character?.CurrentBuild;
        if (build == null) return null;
        return build.Powers.FirstOrDefault(x => x?.Power != null && x.Power.PowerIndex == p.PowerIndex);
    }
}