using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mids_Reborn.Core.Base.Data_Classes;
using Mids_Reborn.Core.Base.Master_Classes;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    internal sealed class OmniTagDefinition
    {
        [JsonProperty("tag")]
        public string Tag { get; set; } = string.Empty;

        [JsonProperty("bears")]
        public List<List<string>> Bears { get; set; } = [];
    }

    private static void ResetRuntimeImportMetadata(IDatabase database)
    {
        database.PowerImportMetadata = new PowerImportMetadata();
        database.EntityImportMetadata = new EntityImportMetadata();
    }

    private static void CapturePowerImportMetadata(IDatabase database, string powerFullName, OmniPowerDefinition source)
    {
        if (database.PowerImportMetadata?.Powers == null)
        {
            database.PowerImportMetadata = new PowerImportMetadata();
        }

        if (string.IsNullOrWhiteSpace(source.TargetRequires) &&
            (source.ActivationEffects == null || source.ActivationEffects.Count == 0))
        {
            database.PowerImportMetadata.Powers.Remove(powerFullName);
            return;
        }

        database.PowerImportMetadata.Powers[powerFullName] = new ImportedPowerSemantics
        {
            TargetRequires = source.TargetRequires ?? string.Empty,
            ActivationEffects = source.ActivationEffects?.Select(CloneEffectGroup).ToList() ?? []
        };
    }

    private static void ApplyPlannerRuntimeMetadata(IPower power, OmniPowerDefinition source)
    {
        if (power is not Power midsPower)
        {
            return;
        }

        midsPower.OmniTargetRequiresRaw = source.TargetRequires ?? string.Empty;
        if (source.ActivationEffects == null || source.ActivationEffects.Count == 0)
        {
            midsPower.ActivationEffectsRuntime = [];
            return;
        }

        var activationEffects = OmniMidsMapper.FlattenEffectGroups(source, source.ActivationEffects, "activation_effect")
            .Cast<IEffect>()
            .ToArray();
        foreach (var effect in activationEffects)
        {
            effect.PowerFullName = midsPower.FullName;
            effect.ActiveConditionals = effect.AdvancedConditions.ToLegacyActiveConditionals();
            effect.SetPower(midsPower);
        }

        midsPower.ActivationEffectsRuntime = activationEffects;
    }

    private static OmniEffectDefinition CloneEffectGroup(OmniEffectDefinition source)
    {
        var serialized = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<OmniEffectDefinition>(serialized) ?? new OmniEffectDefinition();
    }

    private void ApplyEntityImportMetadata(
        IDatabase database,
        string exportRoot,
        OmniExportManifest? manifest,
        IReadOnlyDictionary<string, OmniBuildActor> entityActors,
        OmniApplyResult applyResult)
    {
        database.EntityImportMetadata ??= new EntityImportMetadata();
        database.EntityImportMetadata.EntityTagsByUid.Clear();

        var tagsRoot = Path.Combine(exportRoot, "tags");
        if (!Directory.Exists(tagsRoot))
        {
            return;
        }

        var entities = BuildEntityUidLookup(database.Entities, applyResult);

        foreach (var actor in entityActors.Values)
        {
            if (string.IsNullOrWhiteSpace(actor.EntityName))
            {
                continue;
            }

            entities[NormalizeEntityKey(actor.EntityName)] = actor.EntityName;
        }

        var tagFiles = manifest?.TagFiles?.Count > 0
            ? manifest.TagFiles
            : Directory.EnumerateFiles(tagsRoot, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToArray();

        foreach (var file in tagFiles)
        {
            OmniTagDefinition? tagDefinition;
            try
            {
                tagDefinition = JsonConvert.DeserializeObject<OmniTagDefinition>(File.ReadAllText(file));
            }
            catch
            {
                continue;
            }

            if (tagDefinition == null || string.IsNullOrWhiteSpace(tagDefinition.Tag))
            {
                continue;
            }

            foreach (var bear in tagDefinition.Bears)
            {
                if (bear.Count < 2)
                {
                    continue;
                }

                var normalizedEntityKey = NormalizeEntityKey(bear[1]);
                if (!entities.TryGetValue(normalizedEntityKey, out var entityUid))
                {
                    continue;
                }

                if (!database.EntityImportMetadata.EntityTagsByUid.TryGetValue(entityUid, out var tags))
                {
                    tags = [];
                    database.EntityImportMetadata.EntityTagsByUid[entityUid] = tags;
                }

                if (!tags.Contains(tagDefinition.Tag, StringComparer.OrdinalIgnoreCase))
                {
                    tags.Add(tagDefinition.Tag);
                }
            }
        }

        foreach (var pair in database.EntityImportMetadata.EntityTagsByUid)
        {
            pair.Value.Sort(StringComparer.OrdinalIgnoreCase);
        }

        foreach (var entity in database.Entities.Where(entity => entity != null))
        {
            entity.ActorTags = database.EntityImportMetadata.EntityTagsByUid.TryGetValue(entity.UID, out var tags)
                ? tags
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(tag => tag, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : [];
        }

        applyResult.AddLimited(
            applyResult.PseudoPetAbsorptionAuditDetails,
            $"Hydrated entity tags for {database.EntityImportMetadata.EntityTagsByUid.Count:n0} entities from export tags.");
    }

    private static Dictionary<string, string> BuildEntityUidLookup(
        IEnumerable<SummonedEntity>? entities,
        OmniApplyResult applyResult)
    {
        var lookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (entities == null)
        {
            return lookup;
        }

        foreach (var entity in entities.Where(entity => entity != null && !string.IsNullOrWhiteSpace(entity.UID)))
        {
            var normalizedKey = NormalizeEntityKey(entity.UID);
            if (string.IsNullOrWhiteSpace(normalizedKey))
            {
                continue;
            }

            if (lookup.TryGetValue(normalizedKey, out var existingUid))
            {
                if (!string.Equals(existingUid, entity.UID, StringComparison.OrdinalIgnoreCase))
                {
                    applyResult.AddLimited(
                        applyResult.PseudoPetAbsorptionAuditDetails,
                        $"Entity tag lookup collision on '{normalizedKey}': keeping '{existingUid}', skipping duplicate '{entity.UID}'.");
                }

                continue;
            }

            lookup[normalizedKey] = entity.UID;
        }

        return lookup;
    }
}
