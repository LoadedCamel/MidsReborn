using System.Runtime.CompilerServices;

namespace Mids_Reborn.Core;

public static class TargetingExtensions
{
    // =========================
    // Concrete cohort (direction + entities)
    // =========================
    public enum Cohort { Unknown = 0, Self, Friend, Foe, Pet, All }

    // =========================
    // Cohort classification (from Enums.eEntity)
    // =========================

    public static bool HitsCaster(this Enums.eEntity e) =>
        (e & Enums.eEntity.Caster) == Enums.eEntity.Caster;

    public static bool HitsFoes(this Enums.eEntity e) =>
        (e & (Enums.eEntity.Foe | Enums.eEntity.DeadFoe | Enums.eEntity.FoeRezzingFoe | Enums.eEntity.DeadOrAliveFoe)) != 0;

    public static bool HitsFriends(this Enums.eEntity e) =>
        (e & (Enums.eEntity.Friend | Enums.eEntity.DeadFriend |
              Enums.eEntity.Player | Enums.eEntity.DeadPlayer |
              Enums.eEntity.Teammate | Enums.eEntity.DeadTeammate | Enums.eEntity.DeadOrAliveTeammate |
              Enums.eEntity.Leaguemate | Enums.eEntity.AnyLeaguemate | Enums.eEntity.DeadLeaguemate |
              Enums.eEntity.DeadPlayerFriend)) != 0;

    public static bool HitsPets(this Enums.eEntity e) =>
        (e & (Enums.eEntity.MyPet | Enums.eEntity.DeadMyPet | Enums.eEntity.DeadMyCreation)) != 0;

    public static bool HitsNPCs(this Enums.eEntity e) =>
        (e & Enums.eEntity.NPC) == Enums.eEntity.NPC;

    public static bool HitsLocation(this Enums.eEntity e) =>
        (e & (Enums.eEntity.Location | Enums.eEntity.Teleport)) != 0;

    /// <summary>
    /// Return a compact “cohort” label strictly from EntitiesAffected/AutoHit.
    /// Does NOT look at ToWho (no direction mapping).
    /// Collapsed: Friend/Player/Teammate/Leaguemate => "Ally".
    /// </summary>
    public static string ToCohortString(this Enums.eEntity affected, bool expanded = false)
    {
        var parts = new List<string>();

        if (expanded)
        {
            if ((affected & Enums.eEntity.Foe) != 0 || (affected & Enums.eEntity.DeadFoe) != 0 ||
                (affected & Enums.eEntity.FoeRezzingFoe) != 0 || (affected & Enums.eEntity.DeadOrAliveFoe) != 0)
                parts.Add("Foe");

            if ((affected & Enums.eEntity.Friend) != 0) parts.Add("Friend");
            if ((affected & Enums.eEntity.Player) != 0) parts.Add("Player");
            if ((affected & Enums.eEntity.Teammate) != 0 || (affected & Enums.eEntity.DeadOrAliveTeammate) != 0 || (affected & Enums.eEntity.DeadTeammate) != 0)
                parts.Add("Teammate");
            if ((affected & Enums.eEntity.Leaguemate) != 0 || (affected & Enums.eEntity.AnyLeaguemate) != 0 || (affected & Enums.eEntity.DeadLeaguemate) != 0)
                parts.Add("Leaguemate");

            if ((affected & Enums.eEntity.MyPet) != 0 || (affected & Enums.eEntity.DeadMyPet) != 0 || (affected & Enums.eEntity.DeadMyCreation) != 0)
                parts.Add("Pets");
            if ((affected & Enums.eEntity.NPC) != 0) parts.Add("NPC");
            if ((affected & Enums.eEntity.Location) != 0 || (affected & Enums.eEntity.Teleport) != 0) parts.Add("Location");

            if (parts.Count == 0 && (affected & Enums.eEntity.Any) != 0) parts.Add("Any");
            return string.Join("/", parts);
        }

        // Collapsed: Ally vs Foe vs Pets/NPC/Location
        if (affected.HitsFoes()) parts.Add("Foe");
        if (affected.HitsFriends()) parts.Add("Ally");
        if (affected.HitsPets()) parts.Add("Pets");
        if (affected.HitsNPCs()) parts.Add("NPC");
        if (affected.HitsLocation()) parts.Add("Location");

        if (parts.Count == 0 && (affected & Enums.eEntity.Any) == Enums.eEntity.Any)
            parts.Add("Any");

        return string.Join("/", parts);
    }

    /// <summary>Prefer EntitiesAffected; fall back to EntitiesAutoHit if affected is None.</summary>
    public static Enums.eEntity ResolveCohorts(Enums.eEntity affected, Enums.eEntity autohit) =>
        affected == Enums.eEntity.None ? autohit : affected;

    // =========================
    // Direction (from Enums.eToWho) – formatting-only
    // =========================

    /// <summary>Short direction suffix without any cohort mapping.</summary>
    public static string ToDirectionShort(this Enums.eToWho who) => who switch
    {
        Enums.eToWho.Self => " (Slf)",
        Enums.eToWho.Target => " (Tgt)",
        Enums.eToWho.All => " (Slf+Tgt)",
        _ => string.Empty
    };

    /// <summary>Phrase direction (“to Self”, “to Target”, “to Self & Target”) without cohort.</summary>
    public static string ToDirectionPhrase(this Enums.eToWho who) => who switch
    {
        Enums.eToWho.Self => " to Self",
        Enums.eToWho.Target => " to Target",
        Enums.eToWho.All => " to Self & Target",
        _ => string.Empty
    };

    // =========================
    // Direction + Entities → Concrete cohort
    // =========================

    /// <summary>
    /// Map (ToWho, EntitiesAffected/AutoHit) to a concrete cohort:
    /// Self / Friend / Foe / Pet / All. Mirrors the normalization you do when absorbing pet/grant effects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Cohort ResolveCohort(Enums.eToWho who, Enums.eEntity affected, Enums.eEntity autohit)
    {
        var aff = ResolveCohorts(affected, autohit);
        var hitsCaster = aff.HitsCaster() || aff.HitsFriends();
        var hitsFoe = aff.HitsFoes();
        var hitsPet = aff.HitsPets();

        if (who == Enums.eToWho.Self)
            return hitsCaster ? Cohort.Self : Cohort.Unknown;

        if (who == Enums.eToWho.Target)
        {
            if (hitsPet) return Cohort.Pet;
            if (hitsFoe) return Cohort.Foe;
            if (aff.HitsFriends() || aff.HitsCaster()) return Cohort.Friend;
            return Cohort.Unknown;
        }

        // who == All
        if (hitsCaster && hitsFoe) return Cohort.All;
        if (hitsCaster && !hitsFoe) return Cohort.Self;
        if (!hitsCaster && hitsFoe) return Cohort.Foe;
        return Cohort.Unknown;
    }

    /// <summary>Effect + Owner-aware cohort resolution.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Cohort ResolveCohort(this IEffect fx, IPower owner) =>
        ResolveCohort(fx.ToWho, owner.EntitiesAffected, owner.EntitiesAutoHit);

    // =========================
    // UI composition – keep your existing call sites working
    // =========================

    /// <summary>
    /// Build a UI suffix like: " (Tgt) [Foe]" or " (Slf+Tgt) [Foe/Ally]".
    /// Direction comes from ToWho; cohort comes ONLY from EntitiesAffected/AutoHit.
    /// </summary>
    public static string JoinDirectionAndCohort(this Enums.eToWho who, Enums.eEntity affected, Enums.eEntity autohit, bool showForSelf = false, bool expanded = false)
    {
        var dir = who.ToDirectionShort();
        if (who == Enums.eToWho.Self && !showForSelf) return dir;

        var cohorts = ResolveCohorts(affected, autohit).ToCohortString(expanded);
        return string.IsNullOrWhiteSpace(cohorts) ? dir : $"{dir} [{cohorts}]";
    }

    /// <summary>
    /// Build a phrase like: "to Target (Foe)" or "to Self & Target (Foe/Ally)".
    /// Keeps direction and cohort independent.
    /// </summary>
    public static string JoinPhraseAndCohort(this Enums.eToWho who, Enums.eEntity affected, Enums.eEntity autohit, bool showForSelf = false, bool expanded = false)
    {
        var phrase = who.ToDirectionPhrase();
        if (who == Enums.eToWho.Self && !showForSelf) return phrase;

        var cohorts = ResolveCohorts(affected, autohit).ToCohortString(expanded);
        return string.IsNullOrWhiteSpace(cohorts) ? phrase : $"{phrase} ({cohorts})";
    }

    // Convenience wrappers that match common call-sites you already use in Effect.cs:
    public static string ToShort(this Enums.eToWho who, Enums.eEntity affected, Enums.eEntity autohit, bool showForSelf = false, bool expanded = false)
        => JoinDirectionAndCohort(who, affected, autohit, showForSelf, expanded);

    public static string ToPhrase(this Enums.eToWho who, Enums.eEntity affected, Enums.eEntity autohit, bool showForSelf = false, bool expanded = false)
        => JoinPhraseAndCohort(who, affected, autohit, showForSelf, expanded);

    // =========================
    // Semantic orientation (buff vs debuff)
    // =========================

    // Higher value helps the recipient (classic additive buffs).
    private static readonly HashSet<Enums.eEffectType> HigherIsBetter = new()
    {
        Enums.eEffectType.Defense, Enums.eEffectType.Resistance, Enums.eEffectType.Elusivity,
        Enums.eEffectType.ToHit, Enums.eEffectType.DamageBuff, Enums.eEffectType.Recovery,
        Enums.eEffectType.HitPoints, Enums.eEffectType.Heal, Enums.eEffectType.Absorb,
        Enums.eEffectType.PerceptionRadius, Enums.eEffectType.StealthRadiusPlayer,
        Enums.eEffectType.SpeedRunning, Enums.eEffectType.SpeedFlying, Enums.eEffectType.SpeedJumping, Enums.eEffectType.JumpHeight,
        Enums.eEffectType.MezResist,
        // CoH UI "Endurance Reduction" (discount %) — higher is better.
        Enums.eEffectType.EnduranceDiscount
    };

    // Lower value helps the recipient (time scalars).
    private static readonly HashSet<Enums.eEffectType> LowerIsBetter = new()
        {
            Enums.eEffectType.RechargeTime,
            Enums.eEffectType.InterruptTime
        };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsMovement(Enums.eEffectType t) =>
        t == Enums.eEffectType.SpeedRunning || t == Enums.eEffectType.SpeedFlying ||
        t == Enums.eEffectType.SpeedJumping || t == Enums.eEffectType.JumpHeight;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsTimeOrCost(Enums.eEffectType t) =>
        t == Enums.eEffectType.RechargeTime || t == Enums.eEffectType.InterruptTime;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsHigher(Enums.eEffectType t) => HigherIsBetter.Contains(t) || IsMovement(t);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsLower(Enums.eEffectType t) => LowerIsBetter.Contains(t);

    /// <summary>
    /// True if the effect, as applied to the resolved cohort, should be considered a debuff.
    /// CoH has no "Debuff" type — orientation is (semantics) × (sign) × (who is hit).
    /// </summary>
    public static bool IsDebuff(this IEffect fx, IPower owner)
    {
        // Explicit data hint wins when present.
        if (fx.buffMode == Enums.eBuffMode.Debuff) return true;

        var cohort = fx.ResolveCohort(owner);
        var mag = fx.BuffedMag;
        var t = fx.EffectType;

        // Mez application: positive mag applied to non-friendly is a debuff.
        if (t == Enums.eEffectType.Mez)
            return cohort is Cohort.Foe or Cohort.Pet or Cohort.All && mag > 0;

        // Enhancement carrier: pivot on ETModifies when present.
        var mod = fx.ETModifies != Enums.eEffectType.None ? fx.ETModifies : t;

        if (IsHigher(mod))
        {
            // Higher helps the recipient; decreasing a foe is a debuff.
            return cohort is Cohort.Foe or Cohort.Pet or Cohort.All ? mag < 0 : false;
        }

        if (IsLower(mod))
        {
            // Lower helps the recipient; increasing a foe is a debuff.
            return cohort is Cohort.Foe or Cohort.Pet or Cohort.All ? mag > 0 : false;
        }

        // Default: negative on foe is a debuff.
        return cohort is Cohort.Foe or Cohort.Pet or Cohort.All && mag < 0;
    }

    /// <summary>True if this effect is a buff for the recipient as applied.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBuff(this IEffect fx, IPower owner) => !fx.IsDebuff(owner);
}