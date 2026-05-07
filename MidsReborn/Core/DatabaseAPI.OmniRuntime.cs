using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;

namespace Mids_Reborn.Core;

public static partial class DatabaseAPI
{
    public static int GetClassHitPoints(string className, int? zeroBasedLevel = null)
    {
        var resolvedClass = ResolveExplicitClassName(className);
        var fallbackArchetype = GetArchetypeByClassName(resolvedClass);
        var fallback = fallbackArchetype?.Hitpoints ?? 0;
        return TryGetClassAttributeMax(resolvedClass, "hit_points", zeroBasedLevel ?? MidsContext.MathLevelBase, out var value)
            ? (int)Math.Round(value)
            : fallback;
    }

    public static float GetClassBaseRecovery(string className)
    {
        var resolvedClass = ResolveExplicitClassName(className);
        var fallbackArchetype = GetArchetypeByClassName(resolvedClass);
        var fallback = fallbackArchetype?.BaseRecovery ?? 1.67f;
        return TryGetClassAttributeBase(resolvedClass, "recovery", out var value)
            ? value
            : fallback;
    }

    public static float GetClassBaseRegen(string className)
    {
        var resolvedClass = ResolveExplicitClassName(className);
        var fallbackArchetype = GetArchetypeByClassName(resolvedClass);
        var fallback = fallbackArchetype?.BaseRegen ?? 1f;
        return TryGetClassAttributeBase(resolvedClass, "regeneration", out var value)
            ? value
            : fallback;
    }

    public static float GetClassBaseThreat(string className)
    {
        var resolvedClass = ResolveExplicitClassName(className);
        var fallbackArchetype = GetArchetypeByClassName(resolvedClass);
        var fallback = fallbackArchetype?.BaseThreat ?? 1f;
        return TryGetClassAttributeBase(resolvedClass, "threat_level", out var value)
            ? value
            : fallback;
    }

    public static float GetClassRechargeCap(string className, int? zeroBasedLevel = null)
    {
        var resolvedClass = ResolveExplicitClassName(className);
        var fallbackArchetype = GetArchetypeByClassName(resolvedClass);
        var fallback = fallbackArchetype?.RechargeCap ?? 5f;
        return TryGetClassAttributeMax(resolvedClass, "recharge_time", zeroBasedLevel ?? MidsContext.MathLevelBase, out var value)
            ? value
            : fallback;
    }

    public static float GetClassResistanceCap(string className, int? zeroBasedLevel = null)
    {
        var resolvedClass = ResolveExplicitClassName(className);
        var fallbackArchetype = GetArchetypeByClassName(resolvedClass);
        var fallback = fallbackArchetype?.ResCap ?? 0.9f;
        return TryGetClassAttributeMax(resolvedClass, "damage_resistance", zeroBasedLevel ?? MidsContext.MathLevelBase, out var value)
            ? value
            : fallback;
    }

    public static void HydrateOmniRuntimeMetadata()
    {
        PetActorPowerResolver.ResetOverlayCatalog();

        var powerImport = Database?.PowerImportMetadata?.Powers;
        if (Database?.Power != null)
        {
            foreach (var dbPower in Database.Power.OfType<Power>())
            {
                dbPower.OmniDisplayClassName = string.Empty;

                if (powerImport == null || !powerImport.TryGetValue(dbPower.FullName, out var semantics))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(dbPower.OmniTargetRequiresRaw))
                {
                    dbPower.OmniTargetRequiresRaw = semantics.TargetRequires ?? string.Empty;
                }

                if (dbPower.ActivationEffectsRuntime.Length > 0 ||
                    semantics.ActivationEffects == null ||
                    semantics.ActivationEffects.Count == 0)
                {
                    continue;
                }

                var omniPower = CreateHydrationPowerDefinition(dbPower);
                var flattened = OmniMidsMapper.FlattenEffectGroups(omniPower, semantics.ActivationEffects, "activation_effect").Cast<IEffect>().ToArray();
                foreach (var effect in flattened)
                {
                    effect.PowerFullName = dbPower.FullName;
                    effect.ActiveConditionals = effect.AdvancedConditions.ToLegacyActiveConditionals();
                    effect.SetPower(dbPower);
                }

                dbPower.ActivationEffectsRuntime = flattened;
            }
        }

        var entityTags = Database?.EntityImportMetadata?.EntityTagsByUid;
        if (Database?.Entities == null)
        {
            return;
        }

        foreach (var entity in Database.Entities.Where(entity => entity != null))
        {
            if (entity.ActorTags.Count > 0)
            {
                continue;
            }

            entity.ActorTags = entityTags != null && entityTags.TryGetValue(entity.UID, out var tags)
                ? tags
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : [];
        }

        Database.HasPersistedOmniRuntimeMetadata = true;
    }

    private static OmniPowerDefinition CreateHydrationPowerDefinition(IPower power)
    {
        return new OmniPowerDefinition
        {
            FullName = power.FullName,
            TargetType = MapEntityFlagsToTargetType(power.Target, power.EntitiesAffected),
            TargetsAffected = MapEntityFlagsToTargetList(power.EntitiesAffected),
            TargetsAutoHit = MapEntityFlagsToTargetList(power.EntitiesAutoHit)
        };
    }

    private static string MapEntityFlagsToTargetType(Enums.eEntity target, Enums.eEntity affected)
    {
        var flags = target != Enums.eEntity.None ? target : affected;
        if ((flags & Enums.eEntity.Caster) == Enums.eEntity.Caster)
        {
            return "Self";
        }

        if ((flags & Enums.eEntity.MyPet) == Enums.eEntity.MyPet)
        {
            return "Own Pet (Alive)";
        }

        if ((flags & Enums.eEntity.Foe) == Enums.eEntity.Foe)
        {
            return "Foe (Alive)";
        }

        if ((flags & Enums.eEntity.Friend) == Enums.eEntity.Friend ||
            (flags & Enums.eEntity.Teammate) == Enums.eEntity.Teammate ||
            (flags & Enums.eEntity.Player) == Enums.eEntity.Player)
        {
            return "Ally (Alive)";
        }

        return string.Empty;
    }

    private static List<string> MapEntityFlagsToTargetList(Enums.eEntity flags)
    {
        var targets = new List<string>();
        if ((flags & Enums.eEntity.Caster) == Enums.eEntity.Caster)
        {
            targets.Add("Self");
        }

        if ((flags & Enums.eEntity.MyPet) == Enums.eEntity.MyPet)
        {
            targets.Add("Own Pet (Alive)");
        }

        if ((flags & Enums.eEntity.Friend) == Enums.eEntity.Friend ||
            (flags & Enums.eEntity.Teammate) == Enums.eEntity.Teammate ||
            (flags & Enums.eEntity.Player) == Enums.eEntity.Player)
        {
            targets.Add("Ally (Alive)");
        }

        if ((flags & Enums.eEntity.Foe) == Enums.eEntity.Foe)
        {
            targets.Add("Foe (Alive)");
        }

        if ((flags & Enums.eEntity.Any) == Enums.eEntity.Any)
        {
            targets.Add("Any");
        }

        return targets;
    }

    private static string ResolveExplicitClassName(string className)
    {
        if (!string.IsNullOrWhiteSpace(className))
        {
            return className;
        }

        return ResolveClassName();
    }
}
