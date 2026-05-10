using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;

namespace Mids_Reborn.Core.Omni;

public enum PlannerBuildRecipientKind
{
    Player,
    OwnedRealPet,
    PseudoPet
}

public sealed class PlannerBuildRecipientContext
{
    public PlannerBuildRecipientKind Kind { get; init; }
    public SummonedEntity? Entity { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public int ActorSourceHistoryIndex { get; init; } = -1;
    public string ClassName => Entity?.ClassName ?? MidsContext.Character?.Archetype?.ClassName ?? MidsContext.Archetype?.ClassName ?? string.Empty;

    public static PlannerBuildRecipientContext CreatePlayerRecipient()
    {
        return new PlannerBuildRecipientContext
        {
            Kind = PlannerBuildRecipientKind.Player
        };
    }

    public static PlannerBuildRecipientContext CreateOwnedPetRecipient(SummonedEntity entity, int actorSourceHistoryIndex = -1)
    {
        return new PlannerBuildRecipientContext
        {
            Kind = entity.IsPseudoPet ? PlannerBuildRecipientKind.PseudoPet : PlannerBuildRecipientKind.OwnedRealPet,
            Entity = entity,
            Tags = entity.ActorTags
                .Where(tag => !string.IsNullOrWhiteSpace(tag))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray(),
            ActorSourceHistoryIndex = actorSourceHistoryIndex
        };
    }
}

public static partial class OmniPowerRouting
{
    public static PlannerBuildRecipientContext CreatePlayerRecipient() =>
        PlannerBuildRecipientContext.CreatePlayerRecipient();

    public static PlannerBuildRecipientContext CreateOwnedPetRecipient(SummonedEntity entity, int actorSourceHistoryIndex = -1) =>
        PlannerBuildRecipientContext.CreateOwnedPetRecipient(entity, actorSourceHistoryIndex);

    public static IPower CreateDisplayPower(IPower sourcePower)
    {
        var clone = new Power(sourcePower);
        clone.Effects = CloneEffects(GetDisplayEffects(sourcePower), rewriteTargetToSelf: false);
        return clone;
    }

    public static IEnumerable<IEffect> GetDisplayEffects(IPower sourcePower)
    {
        return sourcePower is Power concretePower && concretePower.ActivationEffectsRuntime.Length > 0
            ? concretePower.ActivationEffectsRuntime
            : sourcePower.Effects;
    }

    public static IPower? CreatePlannerPower(
        IPower sourcePower,
        PlannerBuildRecipientContext recipient,
        bool sourceIsRecipientOwnedPower = false)
    {
        var clone = new Power(sourcePower);
        clone.OmniDisplayClassName = recipient.ClassName;

        if (sourceIsRecipientOwnedPower)
        {
            clone.Effects = CloneEffects(sourcePower.Effects, rewriteTargetToSelf: false);
            return clone;
        }

        var routedEffects = GetPlannerEffects(sourcePower, recipient).ToArray();
        if (routedEffects.Length == 0)
        {
            return null;
        }

        clone.Effects = routedEffects;
        return clone;
    }

    public static IEnumerable<IEffect> GetPlannerEffects(IPower sourcePower, PlannerBuildRecipientContext recipient)
    {
        var routedEffects = new List<IEffect>();
        var includePrimary = RecipientMatchesPrimaryLane(sourcePower, recipient);

        if (recipient.Kind == PlannerBuildRecipientKind.Player &&
            sourcePower is Power concretePower &&
            concretePower.ActivationEffectsRuntime.Length > 0)
        {
            routedEffects.AddRange(CloneEffects(concretePower.ActivationEffectsRuntime, rewriteTargetToSelf: true));
        }

        if (includePrimary)
        {
            routedEffects.AddRange(CloneEffects(sourcePower.Effects, rewriteTargetToSelf: recipient.Kind != PlannerBuildRecipientKind.Player));
        }

        return routedEffects;
    }

    public static bool RecipientMatchesPrimaryLane(IPower sourcePower, PlannerBuildRecipientContext recipient)
    {
        return recipient.Kind switch
        {
            PlannerBuildRecipientKind.Player => PrimaryLaneTargetsPlayer(sourcePower) && TargetRequiresMatches(sourcePower, recipient),
            PlannerBuildRecipientKind.OwnedRealPet => PrimaryLaneTargetsOwnedPet(sourcePower) && TargetRequiresMatches(sourcePower, recipient),
            _ => false
        };
    }

    private static bool PrimaryLaneTargetsPlayer(IPower sourcePower)
    {
        var target = TargetingExtensions.ResolveCohorts(sourcePower.EntitiesAffected, sourcePower.EntitiesAutoHit);
        return target == Enums.eEntity.None ||
               target.HitsCaster() ||
               target.HitsFriends();
    }

    private static bool PrimaryLaneTargetsOwnedPet(IPower sourcePower)
    {
        var target = TargetingExtensions.ResolveCohorts(sourcePower.EntitiesAffected, sourcePower.EntitiesAutoHit);
        return target.HitsPets();
    }

    private static bool TargetRequiresMatches(IPower sourcePower, PlannerBuildRecipientContext recipient)
    {
        if (sourcePower is not Power concretePower)
        {
            return true;
        }

        return EvaluateTargetRoutingPolicy(concretePower.TargetRoutingPolicy, recipient);
    }

    private static bool EvaluateTargetRoutingPolicy(
        PlannerTargetRoutingPolicy policy,
        PlannerBuildRecipientContext recipient)
    {
        if (!EvaluateBuildSourceGates(policy.BuildSourceGates))
        {
            return false;
        }

        if (recipient.Kind == PlannerBuildRecipientKind.Player &&
            !policy.AllowedRecipients.HasFlag(PlannerRecipientFlags.Player))
        {
            return false;
        }

        if (recipient.Kind == PlannerBuildRecipientKind.OwnedRealPet &&
            !policy.AllowedRecipients.HasFlag(PlannerRecipientFlags.OwnedRealPet))
        {
            return false;
        }

        return EvaluateRecipientClauses(policy.RecipientClauses, recipient);
    }

    private static bool EvaluateBuildSourceGates(AdvancedConditionSet gates)
    {
        if (gates.Rows.Count == 0)
        {
            return true;
        }

        var probe = new Effect();
        return AdvancedConditionEvaluator.Evaluate(probe, gates);
    }

    private static bool EvaluateRecipientClauses(
        IReadOnlyList<PlannerRecipientClause> clauses,
        PlannerBuildRecipientContext recipient)
    {
        if (clauses.Count == 0)
        {
            return true;
        }

        var result = EvaluateRecipientClause(clauses[0], recipient);
        for (var index = 1; index < clauses.Count; index++)
        {
            var clauseResult = EvaluateRecipientClause(clauses[index], recipient);
            result = clauses[index].Link == AdvancedConditionLink.Or
                ? result || clauseResult
                : result && clauseResult;
        }

        return result;
    }

    private static bool EvaluateRecipientClause(
        PlannerRecipientClause clause,
        PlannerBuildRecipientContext recipient)
    {
        var result = clause.Kind switch
        {
            PlannerRecipientClauseKind.PlayerRecipient => recipient.Kind == PlannerBuildRecipientKind.Player,
            PlannerRecipientClauseKind.OwnedRealPetRecipient => recipient.Kind == PlannerBuildRecipientKind.OwnedRealPet,
            PlannerRecipientClauseKind.SelfRecipient => recipient.Kind == PlannerBuildRecipientKind.Player,
            PlannerRecipientClauseKind.TargetTag => recipient.Tags.Any(tag => tag.Equals(clause.Value, StringComparison.OrdinalIgnoreCase)),
            PlannerRecipientClauseKind.Never => false,
            _ => true
        };

        return clause.Negated ? !result : result;
    }

    private static IEffect[] CloneEffects(IEnumerable<IEffect> effects, bool rewriteTargetToSelf)
    {
        var clones = effects
            .Select(effect =>
            {
                var clone = (IEffect)effect.Clone();
                if (rewriteTargetToSelf)
                {
                    clone.ToWho = Enums.eToWho.Self;
                }

                return clone;
            })
            .ToArray();
        return clones;
    }
}
