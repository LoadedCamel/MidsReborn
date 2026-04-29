using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mids_Reborn.Core.Omni;

public sealed class OmniEnhancementDefinition
{
    [JsonProperty("canonical_id")]
    public OmniCanonicalId? CanonicalId { get; set; }

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("full_name")]
    public string FullName { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("display_fullname")]
    public string DisplayFullName { get; set; } = string.Empty;

    [JsonProperty("enhancement_type")]
    public string EnhancementType { get; set; } = string.Empty;

    [JsonProperty("enhancement_family")]
    public string EnhancementFamily { get; set; } = string.Empty;

    [JsonProperty("attuned")]
    public bool Attuned { get; set; }

    [JsonProperty("superior_attuned")]
    public bool SuperiorAttuned { get; set; }

    [JsonProperty("boosts_allowed")]
    [JsonConverter(typeof(OmniBoostAllowedListConverter))]
    public List<OmniBoostAllowedRef> BoostsAllowed { get; set; } = [];

    [JsonProperty("boosts_allowed_raw")]
    public List<int> BoostsAllowedRaw { get; set; } = [];

    [JsonProperty("slot_requires")]
    public string SlotRequires { get; set; } = string.Empty;

    [JsonProperty("enhancement_set")]
    public OmniEnhancementSetRef? EnhancementSet { get; set; }

    [JsonProperty("recipe")]
    public OmniRecipeRef? Recipe { get; set; }

    [JsonProperty("power_full_name")]
    public string PowerFullName { get; set; } = string.Empty;

    [JsonProperty("level")]
    public int? Level { get; set; }

    [JsonProperty("min_level")]
    public int? MinLevel { get; set; }

    [JsonProperty("max_level")]
    public int? MaxLevel { get; set; }

    [JsonProperty("level_variants")]
    public List<int> LevelVariants { get; set; } = [];

    [JsonProperty("minimum_use_level_raw")]
    public int? MinimumUseLevelRaw { get; set; }

    [JsonProperty("minimum_use_level")]
    public int? MinimumUseLevel { get; set; }

    [JsonProperty("maximum_use_level_raw")]
    public int? MaximumUseLevelRaw { get; set; }

    [JsonProperty("maximum_use_level")]
    public int? MaximumUseLevel { get; set; }

    [JsonProperty("min_slot_level_raw")]
    public int? MinSlotLevelRaw { get; set; }

    [JsonProperty("min_slot_level")]
    public int? MinSlotLevel { get; set; }

    [JsonProperty("max_slot_level_raw")]
    public int? MaxSlotLevelRaw { get; set; }

    [JsonProperty("max_slot_level")]
    public int? MaxSlotLevel { get; set; }

    [JsonProperty("max_boost_level")]
    public int? MaxBoostLevel { get; set; }

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonProperty("source_file")]
    public string SourceFile { get; set; } = string.Empty;

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }
}

public sealed class OmniBoostAllowedRef
{
    [JsonProperty("id")]
    public int? Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}

public sealed class OmniEnhancementSetRef
{
    [JsonProperty("canonical_id")]
    public OmniCanonicalId? CanonicalId { get; set; }

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("group_name")]
    public string GroupName { get; set; } = string.Empty;

    [JsonProperty("min_level")]
    public int? MinLevel { get; set; }

    [JsonProperty("max_level")]
    public int? MaxLevel { get; set; }
}

public sealed class OmniRecipeRef
{
    [JsonProperty("canonical_id")]
    public OmniCanonicalId? CanonicalId { get; set; }

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;
}

public sealed class OmniEnhancementSetDefinition
{
    [JsonProperty("canonical_id")]
    public OmniCanonicalId? CanonicalId { get; set; }

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("group_name")]
    public string GroupName { get; set; } = string.Empty;

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonProperty("min_level")]
    public int? MinLevel { get; set; }

    [JsonProperty("max_level")]
    public int? MaxLevel { get; set; }

    [JsonProperty("conversion_groups")]
    public List<string> ConversionGroups { get; set; } = [];

    [JsonProperty("boost_lists")]
    public List<string> BoostLists { get; set; } = [];

    [JsonProperty("enhancements")]
    public List<OmniEnhancementSetMemberRef> Enhancements { get; set; } = [];

    [JsonProperty("attuned_enhancements")]
    public List<OmniEnhancementSetMemberRef> AttunedEnhancements { get; set; } = [];

    [JsonProperty("superior_attuned_enhancements")]
    public List<OmniEnhancementSetMemberRef> SuperiorAttunedEnhancements { get; set; } = [];

    [JsonProperty("base_variant_set")]
    public OmniEnhancementSetRef? BaseVariantSet { get; set; }

    [JsonProperty("superior_variant_set")]
    public OmniEnhancementSetRef? SuperiorVariantSet { get; set; }

    [JsonProperty("bonuses")]
    public List<OmniSetBonusDefinition> Bonuses { get; set; } = [];

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }
}

public sealed class OmniEnhancementSetMemberRef
{
    [JsonProperty("canonical_id")]
    public OmniCanonicalId? CanonicalId { get; set; }

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;
}

public sealed class OmniSetBonusSetDefinition
{
    [JsonProperty("canonical_id")]
    public OmniCanonicalId? CanonicalId { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("bonuses")]
    public List<OmniSetBonusDefinition> Bonuses { get; set; } = [];
}

public sealed class OmniSetBonusDefinition
{
    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("minimum_boosts")]
    public int MinimumBoosts { get; set; }

    [JsonProperty("maximum_boosts")]
    public int MaximumBoosts { get; set; }

    [JsonProperty("requires")]
    [JsonConverter(typeof(OmniStringListOrStringConverter))]
    public List<string> Requires { get; set; } = [];

    [JsonProperty("auto_powers")]
    public List<string> AutoPowers { get; set; } = [];

    [JsonProperty("bonus_power")]
    public string BonusPower { get; set; } = string.Empty;
}

[JsonConverter(typeof(OmniCanonicalIdConverter))]
public sealed class OmniCanonicalId
{
    [JsonProperty("value")]
    public string Value { get; set; } = string.Empty;
}

public sealed class OmniRecipeDefinition
{
    [JsonProperty("record_id")]
    public string RecordId { get; set; } = string.Empty;

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("group")]
    public string Group { get; set; } = string.Empty;

    [JsonProperty("source_file")]
    public string SourceFile { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonProperty("enhancement_reward")]
    public string EnhancementReward { get; set; } = string.Empty;

    [JsonProperty("salvage_required")]
    public List<OmniRecipeSalvageRequirement> SalvageRequired { get; set; } = [];

    [JsonProperty("rarity")]
    public int Rarity { get; set; }

    [JsonProperty("rarity_name")]
    public string RarityName { get; set; } = string.Empty;

    [JsonProperty("level")]
    public int? Level { get; set; }

    [JsonProperty("creation_cost")]
    public List<string> CreationCost { get; set; } = [];

    [JsonProperty("buy_from_vendor")]
    public int? BuyFromVendor { get; set; }

    [JsonProperty("sell_to_vendor")]
    public int? SellToVendor { get; set; }

    [JsonProperty("display_help")]
    public string DisplayHelp { get; set; } = string.Empty;

    [JsonProperty("workshops")]
    public List<string> Workshops { get; set; } = [];

    [JsonProperty("level_variants")]
    public List<OmniRecipeLevelVariant> LevelVariants { get; set; } = [];

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }
}

public sealed class OmniRecipeLevelVariant
{
    [JsonProperty("level")]
    public int? Level { get; set; }

    [JsonProperty("level_min")]
    public int? LevelMin { get; set; }

    [JsonProperty("level_max")]
    public int? LevelMax { get; set; }

    [JsonProperty("creation_cost")]
    public List<string> CreationCost { get; set; } = [];

    [JsonProperty("sell_to_vendor")]
    public int? SellToVendor { get; set; }

    [JsonProperty("buy_from_vendor")]
    public int? BuyFromVendor { get; set; }

    [JsonProperty("salvage_required")]
    public List<OmniRecipeSalvageRequirement> SalvageRequired { get; set; } = [];

    [JsonProperty("power_required")]
    public List<string> PowerRequired { get; set; } = [];

    [JsonProperty("visible_requires")]
    public List<string> VisibleRequires { get; set; } = [];

    [JsonProperty("create_requires")]
    public List<string> CreateRequires { get; set; } = [];

    [JsonProperty("receive_requires")]
    public List<string> ReceiveRequires { get; set; } = [];

    [JsonProperty("never_receive_requires")]
    public List<string> NeverReceiveRequires { get; set; } = [];

    [JsonProperty("auction_requires")]
    public List<string> AuctionRequires { get; set; } = [];
}

public sealed class OmniRecipeSalvageRequirement
{
    [JsonProperty("salvage")]
    public string Salvage { get; set; } = string.Empty;

    [JsonProperty("amount")]
    public int Amount { get; set; }
}

public sealed class OmniSalvageDefinition
{
    [JsonProperty("record_id")]
    public string RecordId { get; set; } = string.Empty;

    [JsonProperty("storage_key")]
    public string StorageKey { get; set; } = string.Empty;

    [JsonProperty("source_file")]
    public string SourceFile { get; set; } = string.Empty;

    [JsonProperty("source_index")]
    public int? SourceIndex { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("display_name")]
    public string DisplayName { get; set; } = string.Empty;

    [JsonProperty("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonProperty("rarity")]
    public int Rarity { get; set; }

    [JsonProperty("rarity_name")]
    public string RarityName { get; set; } = string.Empty;

    [JsonProperty("type")]
    public int? Type { get; set; }

    [JsonProperty("type_name")]
    public string TypeName { get; set; } = string.Empty;

    [JsonProperty("workshops")]
    public List<string> Workshops { get; set; } = [];

    [JsonExtensionData]
    public IDictionary<string, JToken>? ExtensionData { get; set; }
}

internal sealed class OmniCanonicalIdConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(OmniCanonicalId);
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonToken.String)
        {
            return new OmniCanonicalId { Value = reader.Value?.ToString() ?? string.Empty };
        }

        var token = JToken.Load(reader);
        if (token.Type == JTokenType.String)
        {
            return new OmniCanonicalId { Value = token.Value<string>() ?? string.Empty };
        }

        if (token is JObject obj)
        {
            return new OmniCanonicalId
            {
                Value = obj.Value<string>("value") ?? string.Empty
            };
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value is OmniCanonicalId canonicalId)
        {
            writer.WriteValue(canonicalId.Value);
            return;
        }

        writer.WriteNull();
    }
}

internal sealed class OmniBoostAllowedListConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<OmniBoostAllowedRef>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return new List<OmniBoostAllowedRef>();
        }

        var token = JToken.Load(reader);
        if (token is not JArray array)
        {
            return new List<OmniBoostAllowedRef>();
        }

        var results = new List<OmniBoostAllowedRef>();
        foreach (var item in array)
        {
            switch (item.Type)
            {
                case JTokenType.Object:
                {
                    var obj = (JObject)item;
                    results.Add(new OmniBoostAllowedRef
                    {
                        Id = obj.Value<int?>("id"),
                        Name = obj.Value<string>("name") ?? string.Empty
                    });
                    break;
                }
                case JTokenType.String:
                    results.Add(new OmniBoostAllowedRef
                    {
                        Name = item.Value<string>() ?? string.Empty
                    });
                    break;
                case JTokenType.Integer:
                    results.Add(new OmniBoostAllowedRef
                    {
                        Id = item.Value<int>()
                    });
                    break;
            }
        }

        return results;
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}

internal sealed class OmniStringListOrStringConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(List<string>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return new List<string>();
        }

        var token = JToken.Load(reader);
        return token.Type switch
        {
            JTokenType.String => string.IsNullOrWhiteSpace(token.Value<string>())
                ? new List<string>()
                : new List<string> { token.Value<string>() ?? string.Empty },
            JTokenType.Array => token.Values<string>()
                .Where(static value => !string.IsNullOrWhiteSpace(value))
                .ToList(),
            _ => new List<string>()
        };
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
