using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Mids_Reborn.Core.Omni;

public sealed partial class OmniImporter
{
    private sealed class OmniTagDefinition
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

    private static OmniEffectDefinition CloneEffectGroup(OmniEffectDefinition source)
    {
        var serialized = JsonConvert.SerializeObject(source);
        return JsonConvert.DeserializeObject<OmniEffectDefinition>(serialized) ?? new OmniEffectDefinition();
    }

    private void ApplyEntityImportMetadata(
        IDatabase database,
        string exportRoot,
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

        foreach (var file in Directory.EnumerateFiles(tagsRoot, "*.json", SearchOption.TopDirectoryOnly))
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
