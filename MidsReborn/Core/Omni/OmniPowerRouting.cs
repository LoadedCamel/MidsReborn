using System.Text.RegularExpressions;
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
    private static readonly Regex TargetHasTagRegex = new(@"^target\.HasTag\?\(([^)]+)\)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
        if (sourcePower is not Power concretePower || string.IsNullOrWhiteSpace(concretePower.OmniTargetRequiresRaw))
        {
            return true;
        }

        return EvaluateTargetRequires(concretePower.OmniTargetRequiresRaw, recipient);
    }

    private static bool EvaluateTargetRequires(string expression, PlannerBuildRecipientContext recipient)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return true;
        }

        var tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length == 1)
        {
            return EvaluateTargetToken(tokens[0], recipient);
        }

        var stack = new Stack<bool>();
        foreach (var token in tokens)
        {
            if (token.Equals("AND", StringComparison.OrdinalIgnoreCase))
            {
                if (stack.Count < 2)
                {
                    return false;
                }

                var right = stack.Pop();
                var left = stack.Pop();
                stack.Push(left && right);
                continue;
            }

            if (token.Equals("OR", StringComparison.OrdinalIgnoreCase))
            {
                if (stack.Count < 2)
                {
                    return false;
                }

                var right = stack.Pop();
                var left = stack.Pop();
                stack.Push(left || right);
                continue;
            }

            if (token is "!" or "NOT")
            {
                if (stack.Count < 1)
                {
                    return false;
                }

                stack.Push(!stack.Pop());
                continue;
            }

            stack.Push(EvaluateTargetToken(token, recipient));
        }

        return stack.Count == 1 && stack.Pop();
    }

    private static bool EvaluateTargetToken(string token, PlannerBuildRecipientContext recipient)
    {
        if (token.Equals("true", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (token.Equals("false", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var match = TargetHasTagRegex.Match(token);
        if (!match.Success)
        {
            return false;
        }

        var tag = match.Groups[1].Value.Trim();
        return recipient.Tags.Any(value => value.Equals(tag, StringComparison.OrdinalIgnoreCase));
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
