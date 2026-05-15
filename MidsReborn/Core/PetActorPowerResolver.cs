using System.Text.RegularExpressions;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.Omni;
using Newtonsoft.Json;

namespace Mids_Reborn.Core;

public static class PetActorPowerResolver
{
    private sealed class ResolutionContext
    {
        public required SummonedEntity Entity { get; init; }
        public required RealPetActorRosterItem RosterItem { get; init; }
        public required Build Build { get; init; }
        public required Dictionary<string, int> OwnedPowerHistoryByFullName { get; init; }
        public required PetActorPreviewState PreviewState { get; init; }
        public required IReadOnlyList<PetUpgradeOverlay> OwnedUpgrades { get; init; }
    }

    private static readonly Regex TargetVillainRegex = new(@"target\.VillainName>([A-Za-z0-9_]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly object CatalogSync = new();
    private static string _cachedOverlayRoot = string.Empty;
    private static PetUpgradeOverlayCatalog? _cachedOverlayCatalog;

    public static (
        IReadOnlyList<ResolvedPetPower> ResolvedPowers,
        IReadOnlyList<PetUpgradeOverlay> AvailableUpgrades,
        IReadOnlyList<ResolvedPetPower> AvailableSelfClickBuffs,
        PetActorPreviewState EffectivePreviewState)
        Resolve(Build build, SummonedEntity entity, RealPetActorRosterItem rosterItem, PetActorPreviewState? previewState)
    {
        var ownedPowerHistoryByFullName = build.Powers
            .Select((entry, historyIndex) => new { Entry = entry, HistoryIndex = historyIndex })
            .Where(pair => pair.Entry?.Power != null)
            .GroupBy(pair => pair.Entry!.Power!.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().HistoryIndex, StringComparer.OrdinalIgnoreCase);

        var availableUpgrades = GetUpgradeOverlayCatalog()
            .OverlaysByEntityUid.TryGetValue(entity.UID, out var overlays)
            ? overlays.Where(overlay => ownedPowerHistoryByFullName.ContainsKey(overlay.UpgradePowerFullName)).ToArray()
            : [];

        var effectivePreviewState = CreateEffectivePreviewState(previewState, availableUpgrades);
        var context = new ResolutionContext
        {
            Entity = entity,
            RosterItem = rosterItem,
            Build = build,
            OwnedPowerHistoryByFullName = ownedPowerHistoryByFullName,
            PreviewState = effectivePreviewState,
            OwnedUpgrades = availableUpgrades
        };

        var resolvedPowers = new List<ResolvedPetPower>();
        resolvedPowers.AddRange(ResolveBaselinePowers(context));
        resolvedPowers.AddRange(ResolveGrantedOverlayPowers(context));

        var deduped = resolvedPowers
            .GroupBy(power => power.Power.FullName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderBy(power => power.GrantKind)
                .ThenBy(power => power.SourceHistoryIndex)
                .First())
            .OrderBy(power => power.Power.GetPowerSet()?.DisplayName ?? power.Power.SetName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(power => power.Power.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var availableSelfClickBuffs = deduped
            .Where(power => power.VisibleInGrid && power.IsSelfClickBuff)
            .ToArray();

        return (deduped, availableUpgrades, availableSelfClickBuffs, effectivePreviewState);
    }

    public static bool ShouldIncludeInTotals(ResolvedPetPower power, PetActorPreviewState previewState)
    {
        if (power.IncludeInTotalsByDefault)
        {
            return true;
        }

        return power.IsSelfClickBuff && previewState.IsPetClickBuffIncluded(power.Power.FullName);
    }

    public static string DescribePowerSource(ResolvedPetPower power)
    {
        return power.GrantKind switch
        {
            PetGrantKind.Baseline => "Baseline",
            PetGrantKind.GrantBoosted when !string.IsNullOrWhiteSpace(power.GrantedByPowerFullName)
                => $"Boosted by {GetPowerDisplayName(power.GrantedByPowerFullName!)}",
            PetGrantKind.Grant when !string.IsNullOrWhiteSpace(power.GrantedByPowerFullName)
                => $"Granted by {GetPowerDisplayName(power.GrantedByPowerFullName!)}",
            PetGrantKind.GrantBoosted => "Boosted grant",
            PetGrantKind.Grant => "Granted overlay",
            _ => "Baseline"
        };
    }

    private static PetActorPreviewState CreateEffectivePreviewState(PetActorPreviewState? previewState, IReadOnlyList<PetUpgradeOverlay> availableUpgrades)
    {
        var effective = previewState?.Clone() ?? new PetActorPreviewState();
        var ownedUpgradeNames = availableUpgrades
            .Select(overlay => overlay.UpgradePowerFullName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        effective.Upgrades.AppliedUpgradePowerFullNames.RemoveWhere(fullName => !ownedUpgradeNames.Contains(fullName));
        if (!effective.Upgrades.DefaultsInitialized)
        {
            foreach (var overlay in availableUpgrades)
            {
                effective.Upgrades.AppliedUpgradePowerFullNames.Add(overlay.UpgradePowerFullName);
            }

            effective = new PetActorPreviewState
            {
                Upgrades = new PetUpgradePreviewState
                {
                    AppliedUpgradePowerFullNames = new HashSet<string>(effective.Upgrades.AppliedUpgradePowerFullNames, StringComparer.OrdinalIgnoreCase),
                    DefaultsInitialized = true
                },
                ClickBuffs = effective.ClickBuffs.Clone(),
                InRange = effective.InRange
            };
        }

        effective.ClickBuffs.IncludedPetSelfClickFullNames.RemoveWhere(string.IsNullOrWhiteSpace);
        return effective;
    }

    private static IEnumerable<ResolvedPetPower> ResolveBaselinePowers(ResolutionContext context)
    {
        foreach (var powersetFullName in context.Entity.PowersetFullName.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            var powerset = DatabaseAPI.GetPowersetByFullname(powersetFullName);
            if (powerset == null)
            {
                continue;
            }

            foreach (var basePower in powerset.Powers.OfType<IPower>())
            {
                var clonedPower = PrepareResolvedPower(basePower, context.Entity.ClassName);
                yield return new ResolvedPetPower
                {
                    Power = clonedPower,
                    SourceHistoryIndex = context.RosterItem.SourceHistoryIndex,
                    GrantKind = PetGrantKind.Baseline,
                    VisibleInGrid = IsVisiblePetPower(clonedPower),
                    IncludeInTotalsByDefault = ShouldIncludeInTotalsByDefault(clonedPower),
                    IsSelfClickBuff = IsPetSelfClickBuff(clonedPower)
                };
            }
        }
    }

    private static IEnumerable<ResolvedPetPower> ResolveGrantedOverlayPowers(ResolutionContext context)
    {
        foreach (var overlay in context.OwnedUpgrades)
        {
            if (!context.PreviewState.IsUpgradeApplied(overlay.UpgradePowerFullName))
            {
                continue;
            }

            if (!context.OwnedPowerHistoryByFullName.TryGetValue(overlay.UpgradePowerFullName, out var upgradeHistoryIndex))
            {
                continue;
            }

            foreach (var grant in overlay.GrantedPowers)
            {
                var grantedPower = DatabaseAPI.GetPowerByFullName(grant.GrantedPowerFullName);
                if (grantedPower == null)
                {
                    continue;
                }

                var clonedPower = PrepareResolvedPower(grantedPower, context.Entity.ClassName);
                yield return new ResolvedPetPower
                {
                    Power = clonedPower,
                    SourceHistoryIndex = grant.GrantKind == PetGrantKind.GrantBoosted
                        ? upgradeHistoryIndex
                        : context.RosterItem.SourceHistoryIndex,
                    GrantKind = grant.GrantKind,
                    GrantedByPowerFullName = overlay.UpgradePowerFullName,
                    RequiredUpgradePowerFullName = overlay.UpgradePowerFullName,
                    VisibleInGrid = IsVisiblePetPower(clonedPower),
                    IncludeInTotalsByDefault = ShouldIncludeInTotalsByDefault(clonedPower),
                    IsSelfClickBuff = IsPetSelfClickBuff(clonedPower)
                };
            }
        }
    }

    private static Power PrepareResolvedPower(IPower sourcePower, string actorClassName)
    {
        var clone = new Power(sourcePower)
        {
            OmniDisplayClassName = actorClassName
        };
        return clone;
    }

    private static bool IsVisiblePetPower(IPower power)
    {
        if (IsSpawnOnlyPetSetupPower(power))
        {
            return false;
        }

        if (power.FullName.Contains(".PM_", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !IsUpgradeMarkerPower(power);
    }

    private static bool IsSpawnOnlyPetSetupPower(IPower power)
    {
        return power.FullName.Equals("Mastermind_Pets.Materialization.Materialization", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUpgradeMarkerPower(IPower power)
    {
        return power.PowerName.Equals("Train_Beasts", StringComparison.OrdinalIgnoreCase) ||
               power.PowerName.Equals("Tame_Beasts", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldIncludeInTotalsByDefault(IPower power)
    {
        if (IsUpgradeMarkerPower(power) || IsSpawnOnlyPetSetupPower(power))
        {
            return false;
        }

        return power.PowerType is Enums.ePowerType.Auto_ or Enums.ePowerType.Toggle;
    }

    private static bool IsPetSelfClickBuff(IPower power)
    {
        if (power.PowerType != Enums.ePowerType.Click)
        {
            return false;
        }

        if (power.ClickBuff)
        {
            return true;
        }

        return power.Effects.Any(effect =>
            effect.ToWho == Enums.eToWho.Self &&
            effect.Duration > 0.25f &&
            effect.EffectType is not Enums.eEffectType.GrantPower and not Enums.eEffectType.EntCreate);
    }

    private static PetUpgradeOverlayCatalog GetUpgradeOverlayCatalog()
    {
        var sourceRoot = NormalizeSourceRoot(DatabaseAPI.Database?.EnhancementImportMetadata?.SourceRoot);
        lock (CatalogSync)
        {
            if (_cachedOverlayCatalog != null && string.Equals(_cachedOverlayRoot, sourceRoot, StringComparison.OrdinalIgnoreCase))
            {
                return _cachedOverlayCatalog;
            }

            _cachedOverlayCatalog = BuildUpgradeOverlayCatalog(sourceRoot);
            _cachedOverlayRoot = sourceRoot;
            return _cachedOverlayCatalog;
        }
    }

    public static void ResetOverlayCatalog()
    {
        lock (CatalogSync)
        {
            _cachedOverlayCatalog = null;
            _cachedOverlayRoot = string.Empty;
        }
    }

    private static PetUpgradeOverlayCatalog BuildUpgradeOverlayCatalog(string sourceRoot)
    {
        var catalog = new PetUpgradeOverlayCatalog();
        foreach (var power in DatabaseAPI.Database?.Power.OfType<IPower>() ?? [])
        {
            if (!LooksLikePetUpgradePower(power))
            {
                continue;
            }

            var overlays = !string.IsNullOrWhiteSpace(sourceRoot)
                ? ReadUpgradeOverlaysFromExport(power, sourceRoot)
                : ReadUpgradeOverlaysFromImportedPower(power);

            foreach (var overlay in overlays)
            {
                if (!catalog.OverlaysByEntityUid.TryGetValue(overlay.TargetEntityUid, out var bucket))
                {
                    bucket = [];
                    catalog.OverlaysByEntityUid[overlay.TargetEntityUid] = bucket;
                }

                bucket.Add(overlay);
            }
        }

        foreach (var overlays in catalog.OverlaysByEntityUid.Values)
        {
            overlays.Sort((left, right) => string.Compare(left.UpgradePowerDisplayName, right.UpgradePowerDisplayName, StringComparison.OrdinalIgnoreCase));
        }

        return catalog;
    }

    private static bool LooksLikePetUpgradePower(IPower power)
    {
        if (power.PowerType != Enums.ePowerType.Click)
        {
            return false;
        }

        if ((power.Target & Enums.eEntity.MyPet) != Enums.eEntity.MyPet &&
            (power.EntitiesAffected & Enums.eEntity.MyPet) != Enums.eEntity.MyPet)
        {
            return false;
        }

        return power.FullName.Contains("Mastermind_Summon", StringComparison.OrdinalIgnoreCase) ||
               power.Effects.Any(effect => effect.EffectType == Enums.eEffectType.GrantPower);
    }

    private static IEnumerable<PetUpgradeOverlay> ReadUpgradeOverlaysFromExport(IPower power, string sourceRoot)
    {
        var filePath = GetPowerExportFilePath(sourceRoot, power.FullName);
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            return ReadUpgradeOverlaysFromImportedPower(power);
        }

        OmniPowerDefinition? source;
        try
        {
            source = JsonConvert.DeserializeObject<OmniPowerDefinition>(File.ReadAllText(filePath));
        }
        catch
        {
            return ReadUpgradeOverlaysFromImportedPower(power);
        }

        if (source == null)
        {
            return [];
        }

        var overlays = new List<PetUpgradeOverlay>();
        foreach (var effect in source.Effects ?? [])
        {
            var targetEntityUid = TryParseTargetEntityUid(effect.RequiresExpression);
            if (string.IsNullOrWhiteSpace(targetEntityUid))
            {
                continue;
            }

            var grantedPowers = ExtractGrantedPowers(effect).ToArray();
            if (grantedPowers.Length == 0)
            {
                continue;
            }

            overlays.Add(new PetUpgradeOverlay
            {
                TargetEntityUid = targetEntityUid,
                UpgradePowerFullName = power.FullName,
                UpgradePowerDisplayName = power.DisplayName,
                GrantedPowers = grantedPowers
            });
        }

        return overlays;
    }

    private static IEnumerable<PetUpgradeOverlay> ReadUpgradeOverlaysFromImportedPower(IPower power)
    {
        var grantsByTarget = new Dictionary<string, List<PetUpgradeGrantedPower>>(StringComparer.OrdinalIgnoreCase);

        foreach (var effect in power.Effects.Where(effect => effect.EffectType == Enums.eEffectType.GrantPower))
        {
            var targetEntityUid = TryParseTargetEntityUid(effect.AdvancedConditions?.Rows.Select(row => row.RawExpression).FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                                                         ?? effect.ActiveConditionals.FirstOrDefault().Key
                                                         ?? string.Empty);
            if (string.IsNullOrWhiteSpace(targetEntityUid) || string.IsNullOrWhiteSpace(effect.Summon))
            {
                continue;
            }

            if (!grantsByTarget.TryGetValue(targetEntityUid, out var granted))
            {
                granted = [];
                grantsByTarget[targetEntityUid] = granted;
            }

            granted.Add(new PetUpgradeGrantedPower
            {
                GrantedPowerFullName = effect.Summon,
                GrantKind = effect.GrantBoosted
                    ? PetGrantKind.GrantBoosted
                    : PetGrantKind.Grant
            });
        }

        return grantsByTarget.Select(pair => new PetUpgradeOverlay
        {
            TargetEntityUid = pair.Key,
            UpgradePowerFullName = power.FullName,
            UpgradePowerDisplayName = power.DisplayName,
            GrantedPowers = pair.Value
        });
    }

    private static IEnumerable<PetUpgradeGrantedPower> ExtractGrantedPowers(OmniEffectDefinition effect)
    {
        foreach (var template in effect.Templates)
        {
            foreach (var grant in ExtractGrantedPowers(template))
            {
                yield return grant;
            }
        }

        foreach (var childEffect in effect.ChildEffects)
        {
            foreach (var grant in ExtractGrantedPowers(childEffect))
            {
                yield return grant;
            }
        }
    }

    private static IEnumerable<PetUpgradeGrantedPower> ExtractGrantedPowers(OmniEffectTemplate template)
    {
        if (template.Params == null)
        {
            yield break;
        }

        foreach (var attrib in template.Attribs)
        {
            var normalizedAttrib = NormalizeToken(attrib);
            var grantKind = normalizedAttrib switch
            {
                "grantpower" => PetGrantKind.Grant,
                "grantboostedpower" => PetGrantKind.GrantBoosted,
                _ => (PetGrantKind?)null
            };

            if (grantKind == null)
            {
                continue;
            }

            var grantedPowerFullName = template.Params["power_names"]?.Values<string>().FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))
                                       ?? template.Params.Value<string>("power")
                                       ?? template.Params.Value<string>("power_name")
                                       ?? string.Empty;
            if (string.IsNullOrWhiteSpace(grantedPowerFullName))
            {
                continue;
            }

            yield return new PetUpgradeGrantedPower
            {
                GrantedPowerFullName = grantedPowerFullName,
                GrantKind = grantKind.Value
            };
        }
    }

    private static string TryParseTargetEntityUid(string? expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return string.Empty;
        }

        var match = TargetVillainRegex.Match(expression);
        return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
    }

    private static string NormalizeSourceRoot(string? sourceRoot)
    {
        if (string.IsNullOrWhiteSpace(sourceRoot))
        {
            return string.Empty;
        }

        return Path.GetFullPath(sourceRoot);
    }

    private static string GetPowerExportFilePath(string sourceRoot, string powerFullName)
    {
        if (string.IsNullOrWhiteSpace(sourceRoot) || string.IsNullOrWhiteSpace(powerFullName))
        {
            return string.Empty;
        }

        var relativePath = powerFullName
            .Replace('.', Path.DirectorySeparatorChar)
            .ToLowerInvariant() + ".json";
        return Path.Combine(sourceRoot, "powers", relativePath);
    }

    private static string NormalizeToken(string value)
    {
        return value.Replace("_", string.Empty).Replace(" ", string.Empty).Trim().ToLowerInvariant();
    }

    private static string GetPowerDisplayName(string powerFullName)
    {
        return DatabaseAPI.GetPowerByFullName(powerFullName)?.DisplayName ?? powerFullName;
    }
}
