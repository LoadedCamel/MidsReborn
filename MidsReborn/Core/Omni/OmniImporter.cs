using Mids_Reborn.Core.Base.Data_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Mids_Reborn.Core;
using Mids_Reborn.Core.PlannerRulesets;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mids_Reborn.Core.Omni;

public sealed class OmniImportResult
{
    public string ExportRoot { get; set; } = string.Empty;
    public OmniImportReport Report { get; init; } = new();
    public OmniImportScope Scope { get; init; } = new();
    public Dictionary<string, OmniClassAttributeTable> ClassAttributes { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, OmniBuildActor> Actors { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> ReferencedEntities { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> GcmTags { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> ScopedPowerFullNames { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> ScopedPowersetFullNames { get; init; } = new(StringComparer.OrdinalIgnoreCase);
    internal List<OmniPowersetDefinition> CachedScopedPowersets { get; set; } = [];
    internal List<OmniPowerDefinition> CachedScopedPowers { get; set; } = [];
    internal Dictionary<string, string> CachedEntityFileLookup { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    internal List<OmniImporter.PetImportManifest> CachedPetManifest { get; set; } = [];
    internal OmniImporter.NormalizedEnhancementImportData? CachedNormalizedEnhancementData { get; set; }

    public void TrimTransientData()
    {
        Actors.Clear();
        ReferencedEntities.Clear();
        GcmTags.Clear();
        ScopedPowerFullNames.Clear();
        ScopedPowersetFullNames.Clear();
        CachedEntityFileLookup.Clear();
        CachedPetManifest.Clear();
    }

    public void TrimForApply()
    {
        TrimTransientData();
        Report.TrimDetails();
    }
}

public sealed partial class OmniImporter
{
    private const string DefaultRetainedEntityClass = "Class_Minion_Pets";
    private OmniImportWorkPlan? _activeWorkPlan;

    private static readonly string[] PetImportRoots =
    [
        "incarnate",
        "kheldian_pets",
        "mastermind_pets",
        "pets",
        "villain_pets"
    ];

    private static readonly HashSet<string> RequiredPseudoPetAbsorptionCaseKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        RequiredPseudoPetAbsorptionCaseKey("Blaster_Support.Tactical_Arrow.Gymnastics", "Pets_OilSlickOil_Blaster"),
        RequiredPseudoPetAbsorptionCaseKey("Controller_Buff.Poison.Poison_Trap", "Pets_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Controller_Buff.Traps.Poison_Trap", "Pets_Traps_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Controller_Buff.Trick_Arrow.Oil_Slick_Arrow", "Pets_OilSlickOil"),
        RequiredPseudoPetAbsorptionCaseKey("Corruptor_Buff.Poison.Poison_Trap", "Pets_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Corruptor_Buff.Traps.Poison_Trap", "Pets_Traps_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Corruptor_Buff.Trick_Arrow.Oil_Slick_Arrow", "Pets_OilSlickOil"),
        RequiredPseudoPetAbsorptionCaseKey("Defender_Buff.Poison.Poison_Trap", "Pets_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Defender_Buff.Traps.Poison_Trap", "Pets_Traps_Poison_Trap_Defender"),
        RequiredPseudoPetAbsorptionCaseKey("Defender_Buff.Trick_Arrow.Oil_Slick_Arrow", "Pets_OilSlickOil"),
        RequiredPseudoPetAbsorptionCaseKey("Mastermind_Buff.Poison.Poison_Trap", "Pets_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Mastermind_Buff.Traps.Poison_Trap", "Pets_Traps_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Mastermind_Buff.Trick_Arrow.Oil_Slick_Arrow", "Pets_OilSlickOil"),
        RequiredPseudoPetAbsorptionCaseKey("Mission_Maker_Secondary.Tactical_Arrow.Oil_Slick_Arrow", "Pets_OilSlickOil"),
        RequiredPseudoPetAbsorptionCaseKey("Mission_Maker_Secondary.Traps.Poison_Trap", "Pets_Traps_Poison_Trap"),
        RequiredPseudoPetAbsorptionCaseKey("Mission_Maker_Secondary.Trick_Arrow.Oil_Slick_Arrow", "Pets_OilSlickOil"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.OilSlickOil.Generate_Target", "Pets_OilSlickTarget"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.OilSlickOil.Res_Target", "Pets_OilSlickBurn"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.OilSlickOil_Blaster.Generate_Target", "Pets_OilSlickTarget_Blaster"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.OilSlickOil_Blaster.Res_Target", "Pets_OilSlickBurn_Blaster"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.OilSlickTarget.Ignited_Oil", "Pets_OilSlickBurn"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.OilSlickTarget_Blaster.Ignited_Oil", "Pets_OilSlickBurn_Blaster"),
        RequiredPseudoPetAbsorptionCaseKey("Pets.Traps_Poison_Trap_Defender.Self_Destruct", "Pets_Traps_Poison_Gas"),
        RequiredPseudoPetAbsorptionCaseKey("V_Arachnos_Proxy.Tactical_Arrow.Gymnastics", "Pets_OilSlickOil_Blaster"),
        RequiredPseudoPetAbsorptionCaseKey("Villain_Pets.Poison_Trap.Self_Destruct", "Pets_Poison_Poison_Gas"),
        RequiredPseudoPetAbsorptionCaseKey("Villain_Pets.Traps_Poison_Trap.Self_Destruct", "Pets_Traps_Poison_Gas"),
        RequiredPseudoPetAbsorptionCaseKey("V_Wyvern.Raptor_VHigh_Piercing.Talon_Oil_Slick_Arrow", "Pets_OilSlickOil")
    };

    private static readonly IReadOnlyDictionary<string, string> PowerNameAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["boosts"] = "Boosts",
            ["epic"] = "Epic",
            ["incarnate_pets"] = "Incarnate_Pets",
            ["incarnate"] = "Incarnate",
            ["inherent"] = "Inherent",
            ["kheldian_pets"] = "Kheldian_Pets",
            ["mastermind_pets"] = "Mastermind_Pets",
            ["pets"] = "Pets",
            ["pool"] = "Pool",
            ["prestige"] = "Prestige",
            ["redirects"] = "Redirects",
            ["set_bonus"] = "Set_Bonus",
            ["temporary_powers"] = "Temporary_Powers",
            ["villain_pets"] = "Villain_Pets",
            ["Blaster_Support.Time_Manipulation"] = "Blaster_Support.Temporal_Manipulation",
            ["Brute_Melee.Brawling"] = "Brute_Melee.Street_Justice",
            ["Controller_Buff.Shock_Therapy"] = "Controller_Buff.Electrical_Affinity",
            ["Corruptor_Buff.Shock_Therapy"] = "Corruptor_Buff.Electrical_Affinity",
            ["Controller_Buff.Electrical_Affinity.Defibrilate"] = "Controller_Buff.Electrical_Affinity.Defibrillate",
            ["Corruptor_Buff.Electrical_Affinity.Defibrilate"] = "Corruptor_Buff.Electrical_Affinity.Defibrillate",
            ["Defender_Buff.Shock_Therapy.Defibrilate"] = "Defender_Buff.Shock_Therapy.Defibrillate",
            ["Epic.Corr_Flame_Mastery.Char"] = "Epic.Corruptor_Fire_Mastery.Char",
            ["Epic.Corr_Flame_Mastery.Consume"] = "Epic.Corruptor_Fire_Mastery.Consume",
            ["Epic.Corr_Flame_Mastery.Fire_Shield"] = "Epic.Corruptor_Fire_Mastery.Fire_Shield",
            ["Epic.Corr_Flame_Mastery.Greater_Fire_Sword"] = "Epic.Corruptor_Fire_Mastery.Greater_Fire_Sword",
            ["Epic.Corr_Flame_Mastery.Rise_of_the_Phoenix"] = "Epic.Corruptor_Fire_Mastery.Rise_of_the_Phoenix",
            ["Epic.Def_Flame_Mastery.Char"] = "Epic.Defender_Fire_Mastery.Char",
            ["Epic.Def_Flame_Mastery.Consume"] = "Epic.Defender_Fire_Mastery.Consume",
            ["Epic.Def_Flame_Mastery.Fire_Shield"] = "Epic.Defender_Fire_Mastery.Fire_Shield",
            ["Epic.Def_Flame_Mastery.Greater_Fire_Sword"] = "Epic.Defender_Fire_Mastery.Greater_Fire_Sword",
            ["Epic.Def_Flame_Mastery.Rise_of_the_Phoenix"] = "Epic.Defender_Fire_Mastery.Rise_of_the_Phoenix",
            ["Epic.Sentinel_Elec_Mastery.Chain_Fences"] = "Epic.Sentinel_Electricity_Mastery.Chain_Fences",
            ["Epic.Sentinel_Elec_Mastery.Havoc_Punch"] = "Epic.Sentinel_Electricity_Mastery.Havok_Punch",
            ["Epic.Sentinel_Elec_Mastery.Lightning_Field"] = "Epic.Sentinel_Electricity_Mastery.Lightning_Field",
            ["Epic.Sentinel_Elec_Mastery.Paralyzing_Jolt"] = "Epic.Sentinel_Electricity_Mastery.Paralyzing_Jolt",
            ["Epic.Sentinel_Elec_Mastery.Rehabilitating_Circuit"] = "Epic.Sentinel_Electricity_Mastery.Rehabilitating_Circuit",
            ["Epic.Sentinel_Lev_Mastery.Chum_Spray"] = "Epic.Sentinel_Leviathan_Mastery.Arctic_Breath",
            ["Epic.Sentinel_Lev_Mastery.Knockout_Blow"] = "Epic.Sentinel_Leviathan_Mastery.Knockout_Blow",
            ["Epic.Sentinel_Lev_Mastery.School_of_Sharks"] = "Epic.Sentinel_Leviathan_Mastery.School_of_Sharks",
            ["Epic.Sentinel_Lev_Mastery.Spirit_Shark_Jaws"] = "Epic.Sentinel_Leviathan_Mastery.Spirit_Shark_Jaws",
            ["Epic.Sentinel_Lev_Mastery.Summon_Coralax"] = "Epic.Sentinel_Leviathan_Mastery.Summon_Coralax",
            ["Epic.Sentinel_Psi_Mastery.Dominate"] = "Epic.Sentinel_Psionic_Mastery.Dominate",
            ["Epic.Sentinel_Psi_Mastery.Link_Minds"] = "Epic.Sentinel_Psionic_Mastery.Link_Minds",
            ["Epic.Sentinel_Psi_Mastery.Mass_Hypnosis"] = "Epic.Sentinel_Psionic_Mastery.Mass_Hypnosis",
            ["Epic.Sentinel_Psi_Mastery.Mind_Probe"] = "Epic.Sentinel_Psionic_Mastery.Mind_Probe",
            ["Epic.Sentinel_Psi_Mastery.Psychic_Shockwave"] = "Epic.Sentinel_Psionic_Mastery.Psychic_Shockwave",
            ["Epic.Corr_Flame_Mastery"] = "Epic.Corruptor_Fire_Mastery",
            ["Epic.Def_Flame_Mastery"] = "Epic.Defender_Fire_Mastery",
            ["Epic.Sentinel_Elec_Mastery"] = "Epic.Sentinel_Electricity_Mastery",
            ["Epic.Sentinel_Lev_Mastery"] = "Epic.Sentinel_Leviathan_Mastery",
            ["Epic.Sentinel_Psi_Mastery"] = "Epic.Sentinel_Psionic_Mastery",
            ["Epic.Dark_Mastery_Blaster"] = "Epic.Blaster_Dark_Mastery",
            ["Epic.Dark_Mastery_Controller"] = "Epic.Controller_Dark_Mastery",
            ["Epic.Dark_Mastery_Dominator"] = "Epic.Dominator_Dark_Mastery",
            ["Epic.Dark_Mastery_Mastermind"] = "Epic.Mastermind_Dark_Mastery",
            ["Epic.Dark_Mastery_TankBrute"] = "Epic.Tank_Dark_Mastery",
            ["Epic.Ice_Mastery_DefCorr"] = "Epic.Defender_Ice_Mastery",
            ["Epic.Ice_Mastery_ScrapStalk"] = "Epic.Scrapper_Ice_Mastery",
            ["Epic.Psionic_Mastery_ScrapStalk"] = "Epic.Melee_Psionic_Mastery",
            ["Epic.Psionic_Mastery_TankBrute"] = "Epic.Tank_Psionic_Mastery",
            ["Epic.Scrapper_Mace_Mastery"] = "Epic.Stalker_Mace_Mastery",
            ["Mastermind_Buff.Shock_Therapy"] = "Mastermind_Buff.Electrical_Affinity",
            ["Mastermind_Buff.Electrical_Affinity.Defibrilate"] = "Mastermind_Buff.Electrical_Affinity.Defibrillate",
            ["Scrapper_Melee.Brawling"] = "Scrapper_Melee.Street_Justice",
            ["Stalker_Melee.Brawling"] = "Stalker_Melee.Street_Justice",
            ["Tanker_Melee.Brawling"] = "Tanker_Melee.Street_Justice"
        };

    private static readonly HashSet<string> StaffMasteryFullNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Brute_Melee.Staff_Fighting.Staff_Mastery",
        "Scrapper_Melee.Staff_Fighting.Staff_Mastery",
        "Tanker_Melee.Staff_Fighting.Staff_Mastery"
    };

    private static readonly Regex TemporaryPowerReferenceRegex =
        new(@"\bTemporary_Powers\.[A-Za-z0-9_]+\.[A-Za-z0-9_]+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly HashSet<string> IgnoredPowerJsonFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "abusive_buff",
        "ai_groups",
        "attrib_cache",
        "cancelable",
        "chain_delay",
        "chain_forks",
        "chain_into_optional",
        "chain_into_power_name",
        "custom_fx",
        "deletable",
        "destroy_on_last_use",
        "display_attacker_attack_floater",
        "display_attacker_attack_floater_defense",
        "display_confirm",
        "display_fullname",
        "display_target_hit_floater",
        "display_victim_hit_floater",
        "enforced_level_bought",
        "face_target",
        "fx",
        "fx_only_on_main_target",
        "highlight_color",
        "highlight_eval",
        "highlight_icon",
        "highlight_ring",
        "idea_cost",
        "ignore_level_gained",
        "ignore_stance",
        "ignore_toggle_max_distance",
        "instance_locked",
        "interrupt_like_sleep",
        "is_environment_hit",
        "local_available_level",
        "message_caster",
        "message_attacker_attack",
        "message_attacker_attack_floater",
        "message_attacker_hit",
        "message_attacker_hit_floater",
        "message_attacker_miss",
        "message_defense_avoidance",
        "message_floater",
        "message_float_reward",
        "message_target_confirm",
        "message_target_hit",
        "message_victim_hit",
        "message_victim_hit_floater",
        "message_victim_miss",
        "position_center",
        "position_distance",
        "position_distance_below",
        "position_distance_left",
        "position_distance_right",
        "position_distance_up",
        "position_height",
        "position_yaw",
        "refreshes_on_active_player_change",
        "reward_requires",
        "remember_stance",
        "server_tray_priority",
        "show_in_inventory",
        "self_confirm",
        "short_name",
        "shuffle_target_list",
        "stacking_lifetime",
        "target_untargetable",
        "time_to_confirm",
        "toggle_detoggle_time",
        "toggle_droppable",
        "toggle_ignores",
        "toggle_on_time",
        "tradeable",
        "tray_number",
        "tray_placement",
        "tray_slot",
        "travel_suppression_time",
        "vars_attribmods_can_reference",
        "vec_box_max",
        "vec_box_min",
        "vec_box_offset",
        "vec_box_size",
        "visibility",
        "works_on_untouchable",
        "works_through_vision_phase"
    };

    private sealed record ImportIntegritySnapshot(
        IReadOnlyDictionary<string, PowerIntegrityInfo> Powers,
        IReadOnlyDictionary<string, PowersetIntegrityInfo> Powersets);

    private static readonly IReadOnlyDictionary<string, string> AcceptedScopedCanonicalPowerReplacements =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Temporary_Powers.Temporary_Powers.Second_Wind"] = "Temporary_Powers.Temporary_Powers.Revive",
            ["Temporary_Powers.Temporary_Powers.Memento_Mori_Execute"] = "Temporary_Powers.Temporary_Powers.Memento_Mori",
            ["Temporary_Powers.Temporary_Powers.Soul_Transfer_Execute"] = "Temporary_Powers.Temporary_Powers.Soul_Transfer",
            ["Temporary_Powers.Temporary_Powers.Street_Cred_Monitor"] = "Temporary_Powers.Temporary_Powers.Street_Cred"
        };

    private static readonly string[] RetainedTemporaryIntegrityPowersetNames =
    [
        "Temporary_Powers.Accolades",
        "Temporary_Powers.SilentPowers"
    ];

    private sealed record PowerIntegrityInfo(
        string FullName,
        string FullSetName,
        string GroupName,
        string SetName,
        string PowerName,
        string DisplayName,
        int PowerSetID,
        int PowerSetIndex,
        bool HiddenPower,
        bool NeverAutoUpdate,
        bool IsOrphan,
        bool MissingFullSetName,
        bool UnresolvedFullSetName);

    private sealed record PowersetIntegrityInfo(
        string CanonicalFullName,
        string FullName,
        string GroupName,
        string SetName,
        string DisplayName,
        Enums.ePowerSetType SetType,
        string ATClass);

    private sealed record PowersetIconInfo(
        string CanonicalFullName,
        string GroupName,
        string SetName,
        string DisplayName,
        string ImageName);

    private readonly JsonSerializerSettings _settings = new()
    {
        MissingMemberHandling = MissingMemberHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore
    };

    public OmniImportSession AnalyzeExport(string exportRoot)
    {
        return AnalyzeExport(exportRoot, null, null);
    }

    public OmniImportSession AnalyzeExport(
        string exportRoot,
        OmniImportSession? existingSession,
        IProgress<OmniImportProgress>? progress)
    {
        if (string.IsNullOrWhiteSpace(exportRoot))
        {
            throw new ArgumentException("Omni export root is required.", nameof(exportRoot));
        }

        var normalizedRoot = NormalizeRoot(exportRoot);
        if (!Directory.Exists(normalizedRoot))
        {
            throw new DirectoryNotFoundException(normalizedRoot);
        }

        using var progressReporter = new ThrottledProgress(progress);
        ActivateWorkPlan(OmniImportWorkPlan.CreateAnalysisPlan());
        try
        {
            ReportStageProgress(progressReporter, OmniImportStageId.ValidateExport, normalizedRoot);
            var manifest = BuildExportManifest(normalizedRoot, progressReporter);
            if (existingSession != null &&
                existingSession.AnalysisResult != null &&
                existingSession.Manifest.Matches(manifest))
            {
                ReportStageProgress(
                    progressReporter,
                    OmniImportStageId.AnalysisComplete,
                    $"{existingSession.AnalysisResult.Report.PowersInScope:n0} scoped powers",
                    markComplete: true);
                return existingSession;
            }

            var result = DryRunInternal(normalizedRoot, manifest, progressReporter);
            return new OmniImportSession
            {
                ExportRoot = normalizedRoot,
                Manifest = manifest,
                AnalysisResult = result
            };
        }
        finally
        {
            ClearWorkPlan();
        }
    }

    public OmniImportResult DryRun(string exportRoot)
    {
        return DryRun(exportRoot, null);
    }

    public OmniImportResult DryRun(string exportRoot, IProgress<OmniImportProgress>? progress)
    {
        return AnalyzeExport(exportRoot, null, progress).AnalysisResult;
    }

    public OmniApplyResult ApplySafeImport(IDatabase database, OmniImportSession session)
    {
        return ApplySafeImport(database, session, null, replaceScopedContent: false);
    }

    public OmniApplyResult ApplySafeImport(
        IDatabase database,
        OmniImportSession session,
        IProgress<OmniImportProgress>? progress,
        bool replaceScopedContent = false)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        var normalizedRoot = NormalizeRoot(session.ExportRoot);
        using var progressReporter = new ThrottledProgress(progress);
        ActivateWorkPlan(OmniImportWorkPlan.CreateApplyPlan());
        try
        {
            ReportStageProgress(progressReporter, OmniImportStageId.ValidateExport, normalizedRoot);
            var manifest = BuildExportManifest(normalizedRoot, progressReporter);
            if (!session.Manifest.Matches(manifest))
            {
                ReportStageProgress(progressReporter, OmniImportStageId.PrepareApply, "Cached analysis is stale, refreshing");
                session.Manifest = manifest;
                session.AnalysisResult = DryRunInternal(normalizedRoot, manifest, progressReporter);
                session.ApplyResult = null;
                session.ClearRenderedArtifacts();
            }

            var applyResult = ApplySafeImport(
                database,
                normalizedRoot,
                session.AnalysisResult,
                session.Manifest,
                progressReporter,
                manifestIndexed: true,
                replaceScopedContent: replaceScopedContent);
            session.ApplyResult = applyResult;
            session.ClearRenderedArtifacts();
            return applyResult;
        }
        finally
        {
            ClearWorkPlan();
        }
    }

    private OmniImportResult DryRunInternal(
        string exportRoot,
        OmniExportManifest manifest,
        IProgress<OmniImportProgress>? progress)
    {
        var result = new OmniImportResult();
        result.ExportRoot = exportRoot;
        OmniModeMapper.LoadCatalog(result.ExportRoot);
        result.Report.ModeCatalogEntriesLoaded = OmniModeMapper.ModeCatalogCount;
        ReportStageProgress(progress, OmniImportStageId.PrepareAnalysis);
        ScanGcmTags(result.ExportRoot, result);
        LoadArchetypes(exportRoot, manifest, result, progress);
        BuildScopedReferenceClosure(exportRoot, manifest, result, progress);
        ScanPowers(exportRoot, manifest, result, progress);
        ScanEntities(exportRoot, manifest, result, progress);
        ExpandReferencedEntityScope(result.ExportRoot, manifest, result);
        ScanPetImportManifest(result.ExportRoot, manifest, result);
        BackfillRetainedEntityClassSummaries(exportRoot, result);
        LoadClassTables(exportRoot, manifest, result, progress);
        ScanEnhancementData(exportRoot, manifest, result, progress);
        ReportStageProgress(progress, OmniImportStageId.AnalysisComplete, $"{result.Report.PowersInScope:n0} scoped powers", markComplete: true);
        return result;
    }

    private OmniExportManifest BuildExportManifest(string exportRoot, IProgress<OmniImportProgress>? progress)
    {
        ReportStageProgress(progress, OmniImportStageId.IndexFiles, "Building export manifest");

        var archetypeRoot = Path.Combine(exportRoot, "archetypes");
        var powersRoot = Path.Combine(exportRoot, "powers");
        var tableRoot = Path.Combine(exportRoot, "tables");
        var entitiesRoot = Path.Combine(exportRoot, "entities");
        var tagsRoot = Path.Combine(exportRoot, "tags");
        var enhancementsRoot = Path.Combine(exportRoot, "enhancements");
        var enhancementSetsRoot = Path.Combine(exportRoot, "enhancement_sets");
        var recipesRoot = Path.Combine(exportRoot, "recipes");
        var legacyRecipesRoot = Path.Combine(exportRoot, "base_recipes");
        var salvageRoot = Path.Combine(exportRoot, "salvage");

        List<string> archetypeFiles = [];
        List<string> classTableFiles = [];
        List<string> powerFiles = [];
        List<string> powersetIndexFiles = [];
        List<string> entityFiles = [];
        List<string> tagFiles = [];
        List<string> enhancementFiles = [];
        List<string> enhancementSetFiles = [];
        List<string> recipeFiles = [];
        List<string> legacyRecipeFiles = [];
        List<string> salvageFiles = [];
        List<string> policyFiles = [];

        Action[] manifestLoaders =
        [
            () => archetypeFiles = EnumerateSortedFiles(archetypeRoot, "*.json", SearchOption.TopDirectoryOnly).ToList(),
            () => classTableFiles = EnumerateSortedFiles(tableRoot, "class_*.json", SearchOption.TopDirectoryOnly).ToList(),
            () => powerFiles = Directory.Exists(powersRoot)
                ? Directory.EnumerateFiles(powersRoot, "*.json", SearchOption.AllDirectories)
                    .Where(file => !Path.GetFileName(file).Equals("index.json", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : [],
            () => powersetIndexFiles = Directory.Exists(powersRoot)
                ? Directory.EnumerateFiles(powersRoot, "index.json", SearchOption.AllDirectories)
                    .Where(file => !IsCategoryRootIndex(Path.GetRelativePath(powersRoot, file)))
                    .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                    .ToList()
                : [],
            () => entityFiles = EnumerateSortedFiles(entitiesRoot, "*.json", SearchOption.TopDirectoryOnly).ToList(),
            () => tagFiles = EnumerateSortedFiles(tagsRoot, "*.json", SearchOption.TopDirectoryOnly).ToList(),
            () => enhancementFiles = Directory.Exists(enhancementsRoot)
                ? Directory.EnumerateDirectories(enhancementsRoot)
                    .SelectMany(dir => EnumerateSortedFiles(dir, "*.json", SearchOption.TopDirectoryOnly))
                    .ToList()
                : [],
            () => enhancementSetFiles = EnumerateSortedFiles(enhancementSetsRoot, "*.json", SearchOption.TopDirectoryOnly).ToList(),
            () => recipeFiles = EnumerateSortedFiles(recipesRoot, "*.json", SearchOption.AllDirectories).ToList(),
            () => legacyRecipeFiles = EnumerateSortedFiles(legacyRecipesRoot, "*.json", SearchOption.AllDirectories).ToList(),
            () => salvageFiles = EnumerateSortedFiles(salvageRoot, "*.json", SearchOption.TopDirectoryOnly).ToList(),
            () => policyFiles = new[]
                {
                    "enhancement_diversification.json",
                    "enhancement_effectiveness.json",
                    "enhancement_exemplar_scaling.json",
                    "enhancement_set_groups.json",
                    "set_bonuses.json",
                    "set_conversions.json"
                }
                .Select(name => Path.Combine(exportRoot, name))
                .Where(File.Exists)
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToList()
        ];

        Parallel.ForEach(
            manifestLoaders,
            new ParallelOptions { MaxDegreeOfParallelism = GetAdaptiveParallelDegree(4) },
            load => load());

        var relevantFiles = archetypeFiles
            .Concat(classTableFiles)
            .Concat(powerFiles)
            .Concat(powersetIndexFiles)
            .Concat(entityFiles)
            .Concat(tagFiles)
            .Concat(enhancementFiles)
            .Concat(enhancementSetFiles)
            .Concat(recipeFiles)
            .Concat(legacyRecipeFiles)
            .Concat(salvageFiles)
            .Concat(policyFiles)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var latestWriteUtcTicks = 0L;
        var totalBytes = 0L;
        var signatureBuilder = new StringBuilder(relevantFiles.Count * 48);
        foreach (var file in relevantFiles)
        {
            var info = new FileInfo(file);
            if (!info.Exists)
            {
                continue;
            }

            latestWriteUtcTicks = Math.Max(latestWriteUtcTicks, info.LastWriteTimeUtc.Ticks);
            totalBytes += info.Length;
            signatureBuilder.Append(Path.GetRelativePath(exportRoot, file).Replace('\\', '/'));
            signatureBuilder.Append('|');
            signatureBuilder.Append(info.Length);
            signatureBuilder.Append('|');
            signatureBuilder.Append(info.LastWriteTimeUtc.Ticks);
            signatureBuilder.AppendLine();
        }

        var signatureBytes = SHA256.HashData(Encoding.UTF8.GetBytes(signatureBuilder.ToString()));
        var fingerprint = new OmniExportFingerprint(
            exportRoot,
            relevantFiles.Count,
            latestWriteUtcTicks,
            totalBytes,
            Convert.ToHexString(signatureBytes));

        ReportStageProgress(
            progress,
            OmniImportStageId.IndexFiles,
            $"{relevantFiles.Count:n0} files indexed",
            relevantFiles.Count,
            relevantFiles.Count,
            markComplete: true);

        return new OmniExportManifest
        {
            ExportRoot = exportRoot,
            Fingerprint = fingerprint,
            ArchetypeFiles = archetypeFiles,
            ClassTableFiles = classTableFiles,
            PowerFiles = powerFiles,
            PowersetIndexFiles = powersetIndexFiles,
            EntityFiles = entityFiles,
            TagFiles = tagFiles,
            EnhancementFiles = enhancementFiles,
            EnhancementSetFiles = enhancementSetFiles,
            RecipeFiles = recipeFiles,
            LegacyRecipeFiles = legacyRecipeFiles,
            SalvageFiles = salvageFiles,
            PolicyFiles = policyFiles,
            RelevantFiles = relevantFiles
        };
    }

    private static IEnumerable<string> EnumerateSortedFiles(string root, string pattern, SearchOption option)
    {
        return !Directory.Exists(root)
            ? []
            : Directory.EnumerateFiles(root, pattern, option)
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase);
    }

    private void BuildScopedReferenceClosure(
        string exportRoot,
        OmniExportManifest manifest,
        OmniImportResult result,
        IProgress<OmniImportProgress>? progress)
    {
        var powersRoot = Path.Combine(exportRoot, "powers");
        if (!Directory.Exists(powersRoot))
        {
            return;
        }

        ReportStageProgress(progress, OmniImportStageId.ExpandScopedReferences);

        var powerFiles = manifest.PowerFiles;
        var powerFilesByFullName = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var temporaryPowerFiles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var cachedTemporaryPowerRefsByFile = new Dictionary<string, IReadOnlyList<string>>(StringComparer.OrdinalIgnoreCase);
        var pendingReferencedPowers = new Queue<string>();
        var pendingTemporaryPowers = new Queue<string>();
        var pendingEntities = new Queue<string>();
        var initialPetPowersets = result.Scope.RetainedPowersets
            .Where(powerset => result.Scope.GetPowersetType(powerset) == Enums.ePowerSetType.Pet &&
                               IsPetRoot(GroupNamePart(powerset)))
            .Select(CanonicalizeOmniFullName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var pendingPetPowersets = new Queue<string>(initialPetPowersets);
        var processedTemporaryPowers = new HashSet<string>(
            result.Scope.RetainedPowerFullNames.Where(IsTemporaryPowerFullName),
            StringComparer.OrdinalIgnoreCase);
        var processedReferencedPowers = new HashSet<string>(
            result.Scope.RetainedPowerFullNames.Where(fullName => !IsTemporaryPowerFullName(fullName)),
            StringComparer.OrdinalIgnoreCase);
        var processedEntities = new HashSet<string>(
            result.Scope.RetainedEntityIds.Select(NormalizeEntityKey),
            StringComparer.OrdinalIgnoreCase);
        var processedPetPowersets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var queuedTemporaryPowers = new HashSet<string>(processedTemporaryPowers, StringComparer.OrdinalIgnoreCase);
        var queuedReferencedPowers = new HashSet<string>(processedReferencedPowers, StringComparer.OrdinalIgnoreCase);
        var queuedEntities = new HashSet<string>(processedEntities, StringComparer.OrdinalIgnoreCase);
        var queuedPetPowersets = new HashSet<string>(initialPetPowersets, StringComparer.OrdinalIgnoreCase);

        var entityFiles = manifest.EntityFiles
            .ToDictionary(
                file => NormalizeEntityKey(Path.GetFileNameWithoutExtension(file)),
                file => file,
                StringComparer.OrdinalIgnoreCase);
        var powerFilesByDirectory = manifest.PowerFiles
            .GroupBy(file => Path.GetDirectoryName(file) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(file => file, StringComparer.OrdinalIgnoreCase).ToArray(),
                StringComparer.OrdinalIgnoreCase);
        var powersetIndexFiles = manifest.PowersetIndexFiles
            .Select(file => new
            {
                File = file,
                Powerset = ReadJson<OmniPowersetDefinition>(file)
            })
            .Where(entry => entry.Powerset != null && !string.IsNullOrWhiteSpace(entry.Powerset.FullName))
            .GroupBy(entry => CanonicalizeOmniFullName(entry.Powerset!.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().File, StringComparer.OrdinalIgnoreCase);

        for (var powerFileIndex = 0; powerFileIndex < powerFiles.Count; powerFileIndex++)
        {
            var file = powerFiles[powerFileIndex];
            ReportStageProgress(
                progress,
                OmniImportStageId.ExpandScopedReferences,
                "Scanning power references",
                powerFileIndex + 1,
                powerFiles.Count);

            var power = ReadJson<OmniPowerDefinition>(file);
            if (power == null)
            {
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(power.FullName);
            if (!powerFilesByFullName.ContainsKey(canonicalFullName))
            {
                powerFilesByFullName[canonicalFullName] = file;
            }

            if (IsTemporaryPowerFullName(canonicalFullName) && !temporaryPowerFiles.ContainsKey(canonicalFullName))
            {
                temporaryPowerFiles[canonicalFullName] = file;
            }

            if (ShouldSkipPowerDefinition(power))
            {
                continue;
            }

            var relative = Path.GetRelativePath(powersRoot, file);
            if (!result.Scope.IsPowerFileInScope(relative, power.Powerset, power.Archetypes, power.FullName))
            {
                continue;
            }

            EnqueueReferencedEntities(power, result, pendingEntities, queuedEntities);
            EnqueueReferencedTemporaryPowers(file, pendingTemporaryPowers, queuedTemporaryPowers, cachedTemporaryPowerRefsByFile);
            EnqueueReferencedRedirectPowers(power, pendingReferencedPowers, pendingTemporaryPowers, queuedReferencedPowers, queuedTemporaryPowers);
        }

        var processedClosureItems = 0;
        var observedClosureItems = Math.Max(1, pendingReferencedPowers.Count + pendingTemporaryPowers.Count + pendingEntities.Count + pendingPetPowersets.Count);
        ReportClosureProgress();

        while (pendingReferencedPowers.Count > 0 || pendingTemporaryPowers.Count > 0 || pendingEntities.Count > 0 || pendingPetPowersets.Count > 0)
        {
            while (pendingReferencedPowers.Count > 0)
            {
                processedClosureItems++;
                var canonicalPower = CanonicalizeOmniFullName(pendingReferencedPowers.Dequeue());
                if (string.IsNullOrWhiteSpace(canonicalPower) ||
                    IsTemporaryPowerFullName(canonicalPower) ||
                    !processedReferencedPowers.Add(canonicalPower))
                {
                    ReportClosureProgress();
                    continue;
                }

                result.Scope.AddRetainedPower(canonicalPower);
                if (!powerFilesByFullName.TryGetValue(canonicalPower, out var file))
                {
                    ReportClosureProgress();
                    continue;
                }

                var power = ReadJson<OmniPowerDefinition>(file);
                if (power == null || ShouldSkipPowerDefinition(power))
                {
                    ReportClosureProgress();
                    continue;
                }

                EnqueueReferencedEntities(power, result, pendingEntities, queuedEntities);
                EnqueueReferencedTemporaryPowers(file, pendingTemporaryPowers, queuedTemporaryPowers, cachedTemporaryPowerRefsByFile);
                EnqueueReferencedRedirectPowers(power, pendingReferencedPowers, pendingTemporaryPowers, queuedReferencedPowers, queuedTemporaryPowers);
                ReportClosureProgress();
            }

            while (pendingTemporaryPowers.Count > 0)
            {
                processedClosureItems++;
                var canonicalPower = CanonicalizeOmniFullName(pendingTemporaryPowers.Dequeue());
                if (!IsTemporaryPowerFullName(canonicalPower) || !processedTemporaryPowers.Add(canonicalPower))
                {
                    ReportClosureProgress();
                    continue;
                }

                result.Scope.AddRetainedPower(canonicalPower);
                if (!temporaryPowerFiles.TryGetValue(canonicalPower, out var file))
                {
                    ReportClosureProgress();
                    continue;
                }

                var power = ReadJson<OmniPowerDefinition>(file);
                if (power == null || ShouldSkipPowerDefinition(power))
                {
                    ReportClosureProgress();
                    continue;
                }

                EnqueueReferencedEntities(power, result, pendingEntities, queuedEntities);
                EnqueueReferencedTemporaryPowers(file, pendingTemporaryPowers, queuedTemporaryPowers, cachedTemporaryPowerRefsByFile);
                EnqueueReferencedRedirectPowers(power, pendingReferencedPowers, pendingTemporaryPowers, queuedReferencedPowers, queuedTemporaryPowers);
                ReportClosureProgress();
            }

            while (pendingEntities.Count > 0)
            {
                processedClosureItems++;
                var entityRef = pendingEntities.Dequeue();
                var normalizedEntityKey = NormalizeEntityKey(entityRef);
                if (!processedEntities.Add(normalizedEntityKey) ||
                    !entityFiles.TryGetValue(normalizedEntityKey, out var entityFile))
                {
                    ReportClosureProgress();
                    continue;
                }

                var entity = ReadJson<OmniEntityDefinition>(entityFile);
                if (entity == null)
                {
                    ReportClosureProgress();
                    continue;
                }

                var actor = OmniPetClassifier.Classify(entity);
                PrepareReferencedEntityActor(actor, result.Scope);
                var retainedClassName = ResolveRetainedEntityClassName(actor.ClassName);
                result.Scope.AddReferencedEntityId(actor.EntityName);
                result.Scope.AddReferencedEntityClass(retainedClassName);
                result.Scope.AddReferencedEntityPowersets(actor.Powersets);
                result.Actors[actor.EntityName] = actor;
                EnqueueActorReferencedPowerContent(
                    actor,
                    powerFilesByFullName,
                    result.Scope,
                    result,
                    pendingReferencedPowers,
                    pendingEntities,
                    pendingTemporaryPowers,
                    queuedReferencedPowers,
                    queuedEntities,
                    queuedTemporaryPowers,
                    cachedTemporaryPowerRefsByFile);

                foreach (var actorPowerset in actor.Powersets
                             .Where(powerset => result.Scope.GetPowersetType(powerset) == Enums.ePowerSetType.Pet &&
                                                IsPetRoot(GroupNamePart(powerset)))
                             .Select(CanonicalizeOmniFullName))
                {
                    if (queuedPetPowersets.Add(actorPowerset))
                    {
                        pendingPetPowersets.Enqueue(actorPowerset);
                    }
                }

                ReportClosureProgress();
            }

            while (pendingPetPowersets.Count > 0)
            {
                processedClosureItems++;
                var canonicalPowerset = pendingPetPowersets.Dequeue();
                if (!processedPetPowersets.Add(canonicalPowerset) ||
                    !powersetIndexFiles.TryGetValue(canonicalPowerset, out var indexFile))
                {
                    ReportClosureProgress();
                    continue;
                }

                var setDirectory = Path.GetDirectoryName(indexFile);
                if (string.IsNullOrWhiteSpace(setDirectory))
                {
                    ReportClosureProgress();
                    continue;
                }

                if (!powerFilesByDirectory.TryGetValue(setDirectory, out var setPowerFiles))
                {
                    ReportClosureProgress();
                    continue;
                }

                foreach (var file in setPowerFiles)
                {
                    var power = ReadJson<OmniPowerDefinition>(file);
                    if (power == null || ShouldSkipPowerDefinition(power))
                    {
                        continue;
                    }

                    EnqueueReferencedEntities(power, result, pendingEntities, queuedEntities);
                    EnqueueReferencedTemporaryPowers(file, pendingTemporaryPowers, queuedTemporaryPowers, cachedTemporaryPowerRefsByFile);
                    EnqueueReferencedRedirectPowers(power, pendingReferencedPowers, pendingTemporaryPowers, queuedReferencedPowers, queuedTemporaryPowers);
                }

                ReportClosureProgress();
            }
        }

        void ReportClosureProgress()
        {
            var pendingCount = pendingReferencedPowers.Count + pendingTemporaryPowers.Count + pendingEntities.Count + pendingPetPowersets.Count;
            observedClosureItems = Math.Max(observedClosureItems, processedClosureItems + pendingCount);
            ReportStageProgress(
                progress,
                OmniImportStageId.ExpandScopedReferences,
                $"Queued power/temp/entity/pet: {pendingReferencedPowers.Count:n0}/{pendingTemporaryPowers.Count:n0}/{pendingEntities.Count:n0}/{pendingPetPowersets.Count:n0}",
                processedClosureItems,
                observedClosureItems,
                markComplete: pendingCount == 0 && processedClosureItems >= observedClosureItems);
        }
    }

    private static void EnqueueReferencedEntities(
        OmniPowerDefinition power,
        OmniImportResult result,
        Queue<string> pendingEntities,
        ISet<string> queuedEntities)
    {
        foreach (var entityRef in GetEntCreateEntityRefs(power))
        {
            result.ReferencedEntities.Add(entityRef);
            if (queuedEntities.Add(NormalizeEntityKey(entityRef)))
            {
                pendingEntities.Enqueue(entityRef);
            }
        }
    }

    private static void EnqueueReferencedTemporaryPowers(
        string file,
        Queue<string> pendingTemporaryPowers,
        ISet<string> queuedTemporaryPowers,
        IDictionary<string, IReadOnlyList<string>> cachedTemporaryPowerRefsByFile)
    {
        if (!cachedTemporaryPowerRefsByFile.TryGetValue(file, out var referencedTemporaryPowers))
        {
            referencedTemporaryPowers = ReadReferencedTemporaryPowers(file).ToArray();
            cachedTemporaryPowerRefsByFile[file] = referencedTemporaryPowers;
        }

        foreach (var tempPower in referencedTemporaryPowers)
        {
            if (queuedTemporaryPowers.Add(CanonicalizeOmniFullName(tempPower)))
            {
                pendingTemporaryPowers.Enqueue(tempPower);
            }
        }
    }

    private static void EnqueueReferencedRedirectPowers(
        OmniPowerDefinition power,
        Queue<string> pendingReferencedPowers,
        Queue<string> pendingTemporaryPowers,
        ISet<string> queuedReferencedPowers,
        ISet<string> queuedTemporaryPowers)
    {
        foreach (var redirect in power.Redirects)
        {
            var targetFullName = CanonicalizeOmniFullName(redirect.Name);
            if (string.IsNullOrWhiteSpace(targetFullName))
            {
                continue;
            }

            if (IsTemporaryPowerFullName(targetFullName))
            {
                if (queuedTemporaryPowers.Add(targetFullName))
                {
                    pendingTemporaryPowers.Enqueue(targetFullName);
                }

                continue;
            }

            if (queuedReferencedPowers.Add(targetFullName))
            {
                pendingReferencedPowers.Enqueue(targetFullName);
            }
        }
    }

    private void EnqueueActorReferencedPowerContent(
        OmniBuildActor actor,
        IReadOnlyDictionary<string, string> powerFilesByFullName,
        OmniImportScope scope,
        OmniImportResult result,
        Queue<string> pendingReferencedPowers,
        Queue<string> pendingEntities,
        Queue<string> pendingTemporaryPowers,
        ISet<string> queuedReferencedPowers,
        ISet<string> queuedEntities,
        ISet<string> queuedTemporaryPowers,
        IDictionary<string, IReadOnlyList<string>> cachedTemporaryPowerRefsByFile)
    {
        foreach (var powerFullName in actor.Powers
                     .Where(value => !string.IsNullOrWhiteSpace(value))
                     .Select(CanonicalizeOmniFullName)
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .Where(powerFullName => IsAllowedEntityPowerFullName(powerFullName, scope)))
        {
            if (!powerFilesByFullName.TryGetValue(powerFullName, out var file))
            {
                continue;
            }

            var power = ReadJson<OmniPowerDefinition>(file);
            if (power == null || ShouldSkipPowerDefinition(power))
            {
                continue;
            }

            EnqueueReferencedEntities(power, result, pendingEntities, queuedEntities);
            EnqueueReferencedTemporaryPowers(file, pendingTemporaryPowers, queuedTemporaryPowers, cachedTemporaryPowerRefsByFile);
            EnqueueReferencedRedirectPowers(power, pendingReferencedPowers, pendingTemporaryPowers, queuedReferencedPowers, queuedTemporaryPowers);
        }
    }

    private static void PrepareReferencedEntityActor(OmniBuildActor actor, OmniImportScope scope)
    {
        actor.Powers = actor.Powers
            .Where(power => !string.IsNullOrWhiteSpace(power))
            .Select(CanonicalizeOmniFullName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        actor.SyntheticPowerAliasesBySource.Clear();
        actor.SyntheticPowersetFullName = string.Empty;
        actor.SyntheticPowersetDisplayName = string.Empty;

        actor.Powersets = actor.Powers
            .Where(powerFullName => IsAllowedEntityPowerFullName(powerFullName, scope))
            .Select(FullSetName)
            .Where(powerset => !string.IsNullOrWhiteSpace(powerset))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsAllowedEntityPowerFullName(string powerFullName, OmniImportScope scope)
    {
        var canonical = CanonicalizeOmniFullName(powerFullName);
        return scope.IsIncludedPowerRoot(canonical) ||
               scope.IsRetainedPower(canonical);
    }

    private static IEnumerable<string> ReadReferencedTemporaryPowers(string file)
    {
        string rawJson;
        try
        {
            rawJson = File.ReadAllText(file);
        }
        catch
        {
            yield break;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in TemporaryPowerReferenceRegex.Matches(rawJson))
        {
            var fullName = CanonicalizeOmniFullName(match.Value);
            if (IsTemporaryPowerFullName(fullName) && seen.Add(fullName))
            {
                yield return fullName;
            }
        }
    }

    public void ApplyClassAttributesToDatabase(IDatabase database, OmniImportResult result)
    {
        ApplyClassAttributesToDatabase(database, result, null);
    }

    public OmniApplyResult RefreshClassAttributes(IDatabase database, string exportRoot)
    {
        if (database == null)
        {
            throw new ArgumentNullException(nameof(database));
        }

        if (string.IsNullOrWhiteSpace(exportRoot))
        {
            throw new ArgumentException("Omni export root is required.", nameof(exportRoot));
        }

        var normalizedRoot = NormalizeRoot(exportRoot);
        if (!Directory.Exists(normalizedRoot))
        {
            throw new DirectoryNotFoundException(normalizedRoot);
        }

        var result = new OmniImportResult
        {
            ExportRoot = normalizedRoot
        };

        var manifest = BuildExportManifest(normalizedRoot, null);
        LoadArchetypes(normalizedRoot, manifest, result, null);
        LoadClassTables(normalizedRoot, manifest, result, null);

        var applyResult = new OmniApplyResult();
        TrackApplyClassTables(result, applyResult);
        ApplyClassAttributesToDatabase(database, result, applyResult);
        return applyResult;
    }

    public OmniApplyResult ApplySafeImport(IDatabase database, string exportRoot, OmniImportResult dryRunResult)
    {
        return ApplySafeImport(database, exportRoot, dryRunResult, null, null, replaceScopedContent: false);
    }

    public OmniApplyResult ApplySafeImport(
        IDatabase database,
        string exportRoot,
        OmniImportResult dryRunResult,
        IProgress<OmniImportProgress>? progress,
        bool replaceScopedContent = false)
    {
        return ApplySafeImport(database, exportRoot, dryRunResult, null, progress, replaceScopedContent: replaceScopedContent);
    }

    private OmniApplyResult ApplySafeImport(
        IDatabase database,
        string exportRoot,
        OmniImportResult dryRunResult,
        OmniExportManifest? manifest,
        IProgress<OmniImportProgress>? progress,
        bool manifestIndexed = false,
        bool replaceScopedContent = false)
    {
        if (database == null)
        {
            throw new ArgumentNullException(nameof(database));
        }

        if (dryRunResult == null)
        {
            throw new ArgumentNullException(nameof(dryRunResult));
        }

        if (string.IsNullOrWhiteSpace(exportRoot))
        {
            throw new ArgumentException("Omni export root is required.", nameof(exportRoot));
        }

        var normalizedRoot = NormalizeRoot(exportRoot);
        if (!Directory.Exists(normalizedRoot))
        {
            throw new DirectoryNotFoundException(normalizedRoot);
        }

        if (!string.Equals(normalizedRoot, dryRunResult.ExportRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Run a dry run for this Omni export folder before applying the import.");
        }

        var applyResult = new OmniApplyResult();
        OmniModeMapper.LoadCatalog(normalizedRoot);
        applyResult.ModeCatalogEntriesLoaded = OmniModeMapper.ModeCatalogCount;
        var progressReporter = progress;
        if (!manifestIndexed)
        {
            ReportStageProgress(progressReporter, OmniImportStageId.IndexFiles, "Validating cached export manifest");
        }
        ReportStageProgress(progressReporter, OmniImportStageId.PrepareApply, "Preparing safe import");
        var beforeIntegrity = CaptureImportIntegritySnapshot(database);
        IReadOnlyCollection<PowersetIconInfo> powersetIcons = replaceScopedContent
            ? Array.Empty<PowersetIconInfo>()
            : CapturePowersetIcons(database);
        ApplyGcmTags(database, normalizedRoot, applyResult);
        TrackApplyClassTables(dryRunResult, applyResult);
        ApplyClassAttributesToDatabase(database, dryRunResult, applyResult);
        ResetRuntimeImportMetadata(database);
        PruneExcludedArchetypeContent(database, dryRunResult.Scope, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.PrepareApply, $"{applyResult.ClassAttributesStored:n0} class tables stored");
        RemoveExcludedOmniContent(database, applyResult);
        RebuildSupportHeavyGroups(database, applyResult);
        var scopedPowersets = dryRunResult.CachedScopedPowersets.Count > 0
            ? dryRunResult.CachedScopedPowersets
            : LoadScopedPowersets(normalizedRoot, dryRunResult.Scope, manifest: manifest).ToList();
        var scopedPowers = dryRunResult.CachedScopedPowers.Count > 0
            ? dryRunResult.CachedScopedPowers
            : LoadScopedPowers(normalizedRoot, dryRunResult.Scope, progress: null, manifest: manifest).ToList();
        if (replaceScopedContent)
        {
            RemoveScopedContentForFreshImport(database, scopedPowersets, scopedPowers, applyResult);
        }

        var strictSetBonusFamiliesInScope = scopedPowersets
            .Where(powerset => !string.IsNullOrWhiteSpace(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .Where(IsStrictSetBonusPowerset)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        PurgeStrictSetBonusFamilies(database, strictSetBonusFamiliesInScope, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.PrepareApply,
            $"{applyResult.SupportPowersRemoved:n0} support-heavy powers removed", markComplete: true);
        EnsureScopedPowersets(database, normalizedRoot, dryRunResult.Scope, applyResult, progressReporter, manifest);
        RemoveStaleRedirectScopedContent(database, scopedPowersets, scopedPowers, applyResult);
        PurgeStrictSetBonusScopedPowers(database, scopedPowers, applyResult);
        var scopedPowerLookup = BuildScopedPowerLookup(scopedPowers);
        var classifier = new OmniPowerClassifier();
        var classifications = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => classifier.Classify(group.First(), scopedPowerLookup),
                StringComparer.OrdinalIgnoreCase);
        var mainImportScopedPowers = scopedPowers
            .Where(power => ShouldMainImportScopedPower(power, classifications))
            .ToList();
        var petManifestOwnedPowersets = scopedPowers
            .Where(power => IsPetManifestOwnedScopedPower(power, classifications))
            .Select(PowerPowersetFullName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        ReportStageProgress(progressReporter, OmniImportStageId.ApplyEntities, "Resolving pet manifests");
        var petManifest = dryRunResult.CachedPetManifest.Count > 0
            ? dryRunResult.CachedPetManifest
            : BuildPetImportManifest(normalizedRoot, dryRunResult.Scope, applyResult, petManifestOwnedPowersets, manifest);
        var scopedPowersetLookup = scopedPowersets
            .Where(powerset => !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        TrackBoostSetBonusScopeCoverage(
            applyResult,
            scopedPowersetLookup.Keys,
            DeriveScopedPowersetFullNames(scopedPowers.Where(power => !string.IsNullOrWhiteSpace(power.FullName))
                .Select(power => CanonicalizeOmniFullName(power.FullName))),
            scopedPowers.Where(power => !string.IsNullOrWhiteSpace(power.FullName))
                .Select(power => CanonicalizeOmniFullName(power.FullName)));
        EnsurePowersetsForScopedPowers(database, scopedPowers, dryRunResult.Scope, applyResult);
        var entityActors = dryRunResult.Actors.Count > 0
            ? dryRunResult.Actors
                .ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value,
                    StringComparer.OrdinalIgnoreCase)
            : LoadReferencedEntityActors(normalizedRoot, manifest, scopedPowers, dryRunResult.Scope, applyResult);
        UpsertReferencedEntities(database, entityActors, applyResult);
        ApplyEntityImportMetadata(database, normalizedRoot, manifest, entityActors, applyResult);
        TrackStaffMasteryScope(scopedPowers, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.ApplyEntities, $"{scopedPowers.Count:n0} scoped powers resolved", markComplete: true);

        var existingPowers = GetCanonicalizableDatabasePowers(database);
        var midsPowers = BuildCanonicalPowerLookup(existingPowers);

        var nextStaticIndex = (database.Power ?? [])
            .Where(p => p != null)
            .Select(p => p.StaticIndex)
            .DefaultIfEmpty(-2)
            .Max() + 1;

        for (var powerIndex = 0; powerIndex < mainImportScopedPowers.Count; powerIndex++)
        {
            var omniPower = mainImportScopedPowers[powerIndex];
            ReportStageProgress(
                progressReporter,
                OmniImportStageId.ApplyPowers,
                string.Empty,
                powerIndex + 1,
                mainImportScopedPowers.Count);
            var midsFullName = CanonicalizeOmniFullName(omniPower.FullName);
            var forceRecreateStrictSetBonus = IsStrictSetBonusPower(midsFullName);
            if (forceRecreateStrictSetBonus)
            {
                RemoveStrictSetBonusExistingPower(database, existingPowers, midsPowers, midsFullName, applyResult);
            }
            var staffTrace = IsStaffMasteryFullName(midsFullName);
            if (!forceRecreateStrictSetBonus &&
                !midsPowers.TryGetValue(midsFullName, out var midsPower) &&
                TryFindCompositePowerIdentityMatch(existingPowers, omniPower, midsFullName, scopedPowersetLookup, out midsPower))
            {
                midsPowers[midsFullName] = midsPower;
                applyResult.PowersMatched++;
                TrackBoostSetBonusPowerApply(midsFullName, "matched", applyResult);
                TrackEpicPowerApply(midsFullName, applyResult, "matched",
                    $"{midsFullName}: matched existing by composite identity");
                TrackPetPowerApply(midsFullName, applyResult, "matched",
                    $"{midsFullName}: matched existing by composite identity");
                if (staffTrace)
                {
                    applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                        $"{midsFullName}: matched existing by composite identity: {DescribePowerIdentity(midsPower)}");
                }

                applyResult.AddLimited(applyResult.UpdatedPowers,
                    $"{midsPower.FullName} display/internal-name match -> {FormatAliasForReport(omniPower.FullName, midsFullName)}");
            }
            else if (!midsPowers.TryGetValue(midsFullName, out midsPower))
            {
                midsPower = CreatePower(database, omniPower, nextStaticIndex++);
                ApplyCanonicalPowerName(midsPower, midsFullName);
                midsPowers[midsFullName] = midsPower;
                existingPowers.Add(midsPower);
                applyResult.PowersCreated++;
                TrackBoostSetBonusPowerApply(midsFullName, "created", applyResult);
                TrackEpicPowerApply(midsFullName, applyResult, "created",
                    $"{midsFullName}: created new power");
                TrackPetPowerApply(midsFullName, applyResult, "created",
                    $"{midsFullName}: created new power");
                if (staffTrace)
                {
                    applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                        $"{midsFullName}: created new power: {DescribePowerIdentity(midsPower)}");
                }

                applyResult.AddLimited(applyResult.CreatedPowers, FormatAliasForReport(omniPower.FullName, midsFullName));
            }
            else
            {
                applyResult.PowersMatched++;
                TrackBoostSetBonusPowerApply(midsFullName, "matched", applyResult);
                TrackEpicPowerApply(midsFullName, applyResult, "matched",
                    $"{midsFullName}: matched existing by canonical name");
                TrackPetPowerApply(midsFullName, applyResult, "matched",
                    $"{midsFullName}: matched existing by canonical name");
                if (staffTrace)
                {
                    applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                        $"{midsFullName}: matched existing by canonical name: {DescribePowerIdentity(midsPower)}");
                }
            }

            var priorClickBuff = midsPower.ClickBuff;
            var classification = classifications[midsFullName];

            if (midsPower.NeverAutoUpdate)
            {
                if (classification.NormalBuildPick &&
                    !string.Equals(midsPower.FullName, midsFullName, StringComparison.OrdinalIgnoreCase))
                {
                    var oldFullName = midsPower.FullName;
                    ApplyCanonicalPowerName(midsPower, midsFullName);
                    midsPower.IsModified = true;
                    applyResult.ScopedPowerSetIdentityRepairs++;
                    applyResult.AddLimited(applyResult.ScopedPowerSetIdentityRepairDetails,
                        $"{oldFullName} -> {midsFullName}: canonical identity restored despite NeverAutoUpdate");
                }

                if (classification.NormalBuildPick && midsPower.HiddenPower)
                {
                    ApplyPowerClassification(midsPower, classification);
                    midsPower.IsModified = true;
                    applyResult.PowersUpdated++;
                    applyResult.AddLimited(applyResult.UpdatedPowers,
                        $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: visibility/classification restored for normal build powerset pick despite NeverAutoUpdate");
                }

                applyResult.PowersSkippedNeverAutoUpdate++;
                TrackEpicPowerApply(midsFullName, applyResult, "skipped",
                    $"{midsFullName}: skipped by NeverAutoUpdate");
                TrackPetPowerApply(midsFullName, applyResult, "skipped",
                    $"{midsFullName}: skipped by NeverAutoUpdate");
                applyResult.AddLimited(applyResult.SkippedPowers, $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: NeverAutoUpdate");
                if (staffTrace)
                {
                    applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                        $"{midsFullName}: skipped by NeverAutoUpdate after classification {classification.Summary(midsFullName)}; current {DescribePowerIdentity(midsPower)}");
                }

                ApplyPlannerRuntimeMetadata(midsPower, omniPower);
                CapturePowerImportMetadata(database, midsPower, midsFullName, omniPower);
                continue;
            }

            var priorAttackTypes = midsPower.AttackTypes;
            ApplyPowerMetadata(midsPower, omniPower);
            TrackApplyPowerFieldCoverage(omniPower, applyResult);
            ApplyCanonicalPowerName(midsPower, midsFullName);
            ApplyPowerClassification(midsPower, classification);
            ApplyPseudoPetAbsorptionFlags(midsPower, omniPower, entityActors, applyResult);
            ApplyPlannerRuntimeMetadata(midsPower, omniPower);
            CapturePowerImportMetadata(database, midsPower, midsFullName, omniPower);
            if (staffTrace)
            {
                applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                    $"{midsFullName}: metadata/classification applied: {classification.Summary(midsFullName)}; current {DescribePowerIdentity(midsPower)}");
            }

            TrackApplyClassification(midsFullName, classification, priorClickBuff, applyResult);
            TrackApplyAttackVectors(midsFullName, priorAttackTypes, midsPower.AttackTypes, omniPower, applyResult);
            TrackApplyModes(midsFullName, omniPower, classification, applyResult);
            var effects = OmniMidsMapper.FlattenEffects(omniPower, applyResult).Cast<IEffect>().ToList();
            var nextUniqueId = effects.Count == 0 ? 1 : effects.Max(e => e.UniqueID) + 1;
            foreach (var redirect in omniPower.Redirects.Where(r => !string.IsNullOrWhiteSpace(r.Name)))
            {
                var redirectEffect = OmniMidsMapper.CreatePowerRedirectEffect(omniPower.FullName, redirect);
                redirectEffect.UniqueID = nextUniqueId++;
                redirectEffect.ActiveConditionals = redirectEffect.AdvancedConditions.ToLegacyActiveConditionals();
                if (redirectEffect.AdvancedConditions.Rows.Any(row =>
                        row.Kind == AdvancedConditionKind.SourceMode &&
                        row.Subject.Equals("FastSnipe", StringComparison.OrdinalIgnoreCase) &&
                        row.RawExpression.Contains("kEngaged", StringComparison.OrdinalIgnoreCase)))
                {
                    applyResult.SnipeEngagedAliases++;
                    applyResult.AddLimited(applyResult.SnipeEngagedAliasDetails,
                        $"{midsFullName}: {redirect.Requires} -> FastSnipe");
                }

                effects.Add(redirectEffect);
                applyResult.RedirectEffectsAdded++;
            }

            foreach (var effect in effects)
            {
                effect.PowerFullName = midsPower.FullName;
                effect.ActiveConditionals = effect.AdvancedConditions.ToLegacyActiveConditionals();
            }

            ApplyStrengthsDisallowedToEffects(effects, midsPower.IgnoreEnh, midsPower.TypedEnhancementRestrictions);
            midsPower.Effects = effects.ToArray();
            midsPower.HasGrantPowerEffect = effects.Any(effect => effect.EffectType == Enums.eEffectType.GrantPower);
            midsPower.HasPowerOverrideEffect = effects.Any(effect => effect.EffectType == Enums.eEffectType.PowerRedirect);
            applyResult.EffectsReplaced += effects.Count;

            if (midsPower.NeverAutoUpdateRequirements)
            {
                applyResult.RequirementsSkippedNeverAutoUpdateRequirements++;
                applyResult.AddLimited(applyResult.RequirementSkips, $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: NeverAutoUpdateRequirements");
            }
            else if (string.IsNullOrWhiteSpace(omniPower.Requires))
            {
                midsPower.AdvancedRequirements = new AdvancedConditionSet();
                midsPower.Requires = midsPower.AdvancedRequirements.ToLegacyRequirement();
                applyResult.RequirementsUpdated++;
            }
            else if (OmniExpressionConverter.TryConvertPowerRequirement(CanonicalizeOmniFullName(omniPower.Requires), out var requirements))
            {
                midsPower.AdvancedRequirements = requirements;
                midsPower.Requires = requirements.ToLegacyRequirement();
                applyResult.RequirementsUpdated++;
            }
            else
            {
                applyResult.RequirementsSkippedUnsupported++;
                applyResult.AddLimited(applyResult.RequirementSkips, $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: {omniPower.Requires}");
            }

            midsPower.IsModified = true;
            applyResult.PowersUpdated++;
            TrackBoostSetBonusPowerApply(midsFullName, "updated", applyResult);
            TrackEpicPowerApply(midsFullName, applyResult, "updated",
                $"{midsFullName}: metadata/effects/requirements updated");
            TrackPetPowerApply(midsFullName, applyResult, "updated",
                $"{midsFullName}: metadata/effects/requirements updated");
            applyResult.AddLimited(applyResult.UpdatedPowers, FormatAliasForReport(omniPower.FullName, midsFullName));
        }

        ReportStageProgress(progressReporter, OmniImportStageId.LinkPlannerMetadata, "Linking support powers");
        BuildSupportPowerLinks(scopedPowers, midsPowers, classifications, applyResult);
        RemodelPlannerStateFamilies(database, applyResult);
        RemodelTierOneArchetypeInherents(database, applyResult);
        EnsurePlannerModeBindings(database, applyResult);
        RepairAliasedPowersetIdentities(database, applyResult);
        RepairAliasedPowerIdentities(database, applyResult);
        RepairMalformedPowerNames(database, applyResult);
        RepairSetBonusSiblingPowerIdentities(database, scopedPowers, applyResult);
        existingPowers = GetCanonicalizableDatabasePowers(database);
        midsPowers = BuildCanonicalPowerLookup(existingPowers);
        UpsertPetSourceOfTruth(
            database,
            petManifest,
            scopedPowerLookup,
            midsPowers,
            classifications,
            classifier,
            applyResult,
            ref nextStaticIndex);
        RepairPetManifestPowerIdentities(database, petManifest, midsPowers, applyResult);
        BuildSupportPowerLinks(scopedPowers, midsPowers, classifications, applyResult);
        SortPowersByPowersetAndLevel(database, applyResult);
        RestorePowersetIcons(database, powersetIcons);
        RebuildScopedPowerEnhancementLegality(database, scopedPowers, applyResult);
        database.HasPersistedOmniRuntimeMetadata = true;

        ReportStageProgress(progressReporter, OmniImportStageId.ImportEnhancements, "Importing enhancement metadata");
        if (ReferenceEquals(DatabaseAPI.Database, database))
        {
            ApplyEnhancementImportStage(database, normalizedRoot, manifest, dryRunResult, applyResult);
            DatabaseAPI.NormalizeSetTypeTaxonomy(database);
            ReportStageProgress(progressReporter, OmniImportStageId.RebuildIds, "Resolving database IDs");
            DatabaseAPI.MatchIds();
            ReportStageProgress(progressReporter, OmniImportStageId.RebuildIds, markComplete: true);
        }
        else
        {
            ApplyEnhancementImportStage(database, normalizedRoot, manifest, dryRunResult, applyResult);
            DatabaseAPI.NormalizeSetTypeTaxonomy(database);
        }
        ReportStageProgress(progressReporter, OmniImportStageId.ImportEnhancements, markComplete: true);

        RepairImportedBoostAndSetBonusPowerEnhancementLegality(database, scopedPowers, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.LinkPlannerMetadata, "Repairing imported planner metadata");
        VerifyAndRepairScopedPowerGraph(
            database,
            scopedPowers,
            classifications,
            scopedPowersetLookup,
            entityActors,
            applyResult,
            ref nextStaticIndex);
        VerifyAndRepairRetainedTemporaryPowerGraph(
            database,
            scopedPowers,
            classifications,
            scopedPowersetLookup,
            entityActors,
            applyResult,
            ref nextStaticIndex);
        RebuildPowersetMembershipFromPowerGraph(database);
        RepairRequiredPseudoPetAbsorptionFlags(database, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.LinkPlannerMetadata, markComplete: true);
        ReportStageProgress(progressReporter, OmniImportStageId.RebuildIds, "Rebuilding IDs and power graph");
        TrackPoolImportIntegrity(database, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.RebuildIds, markComplete: true);
        ReportStageProgress(progressReporter, OmniImportStageId.FinalAudits, "Running final audits");
        TrackPetManifestLinkAudit(database, petManifest, applyResult);
        TrackPetPowerLinkAudit(database, scopedPowers, dryRunResult.Scope, applyResult);
        TrackRetainedTemporaryPowerIntegrity(database, scopedPowers, classifications, applyResult);
        TrackPseudoPetAbsorptionAudit(database, applyResult);
        TrackEpicImportLinkAudit(database, scopedPowers, scopedPowersets, applyResult);
        TrackSorceryEnflameApplyTrace(database, applyResult);
        TrackImportIntegrityAudit(database, scopedPowers, classifications, beforeIntegrity, applyResult);
        TrackBoostSetBonusImportAudit(database, scopedPowers, scopedPowersetLookup.Keys, applyResult);
        ReportStageProgress(progressReporter, OmniImportStageId.FinalAudits, markComplete: true);
        ReportStageProgress(progressReporter, OmniImportStageId.ApplyComplete, $"{applyResult.PowersUpdated:n0} powers updated", markComplete: true);
        return applyResult;
    }

    private Dictionary<string, OmniBuildActor> LoadReferencedEntityActors(
        string exportRoot,
        OmniExportManifest? manifest,
        IEnumerable<OmniPowerDefinition> scopedPowers,
        OmniImportScope scope,
        OmniApplyResult applyResult)
    {
        var refs = scopedPowers
            .SelectMany(GetEntCreateEntityRefs)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var actors = new Dictionary<string, OmniBuildActor>(StringComparer.OrdinalIgnoreCase);
        if (refs.Count == 0)
        {
            return actors;
        }

        var entitiesRoot = Path.Combine(exportRoot, "entities");
        if (!Directory.Exists(entitiesRoot))
        {
            applyResult.AddLimited(applyResult.PseudoPetAbsorptionAuditDetails,
                "Entity directory missing; pseudo-pet absorption cannot resolve EntCreate entities.");
            applyResult.PseudoPetAbsorptionAuditFailures++;
            return actors;
        }

        var entityFiles = Directory.EnumerateFiles(entitiesRoot, "*.json", SearchOption.TopDirectoryOnly)
            .ToDictionary(
                file => NormalizeEntityKey(Path.GetFileNameWithoutExtension(file)),
                file => file,
                StringComparer.OrdinalIgnoreCase);

        foreach (var entityRef in refs)
        {
            if (!entityFiles.TryGetValue(NormalizeEntityKey(entityRef), out var file))
            {
                applyResult.PseudoPetAbsorptionAuditFailures++;
                applyResult.AddLimited(applyResult.PseudoPetAbsorptionAuditDetails,
                    $"{entityRef}: referenced by EntCreate but no matching entity file was found.");
                continue;
            }

            var entity = ReadJson<OmniEntityDefinition>(file);
            if (entity == null)
            {
                applyResult.PseudoPetAbsorptionAuditFailures++;
                applyResult.AddLimited(applyResult.PseudoPetAbsorptionAuditDetails,
                    $"{entityRef}: matching entity file could not be read.");
                continue;
            }

            var actor = OmniPetClassifier.Classify(entity);
            PrepareReferencedEntityActor(actor, scope);
            var retainedClassName = ResolveRetainedEntityClassName(actor.ClassName);
            scope.AddReferencedEntityId(actor.EntityName);
            scope.AddReferencedEntityClass(retainedClassName);
            scope.AddReferencedEntityPowersets(actor.Powersets);

            actors[actor.EntityName] = actor;
        }

        return actors;
    }

    private static void UpsertReferencedEntities(
        IDatabase database,
        IReadOnlyDictionary<string, OmniBuildActor> entityActors,
        OmniApplyResult applyResult)
    {
        if (entityActors.Count == 0)
        {
            return;
        }

        var entities = database.Entities?.Where(entity => entity != null).ToList() ?? [];
        var existing = entities
            .Where(entity => !string.IsNullOrWhiteSpace(entity.UID))
            .GroupBy(entity => entity.UID, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var actor in entityActors.Values)
        {
            var existed = existing.TryGetValue(actor.EntityName, out var entity);
            if (!existed)
            {
                entity = SummonedEntity.AddEntity();
                entity.UID = actor.EntityName;
                entities.Add(entity);
                existing[actor.EntityName] = entity;
            }

            entity.DisplayName = string.IsNullOrWhiteSpace(actor.DisplayName) ? actor.EntityName : actor.DisplayName;
            var resolvedClassName = ResolveRetainedEntityClassName(actor.ClassName);
            entity.ClassName = resolvedClassName;
            entity.EntityType = actor.Kind == OmniBuildActorKind.PseudoPet
                ? Enums.eSummonEntity.PseudoPet
                : resolvedClassName.Contains("Henchman", StringComparison.OrdinalIgnoreCase)
                    ? Enums.eSummonEntity.Henchman
                    : Enums.eSummonEntity.Pet;
            entity.PowersetFullName = actor.Powersets
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(CanonicalizeOmniFullName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            entity.UpgradePowerFullName = entity.UpgradePowerFullName ?? [];

            if (actor.Kind == OmniBuildActorKind.PseudoPet)
            {
                if (existed)
                {
                    applyResult.PseudoPetEntitiesUpdated++;
                }
                else
                {
                    applyResult.PseudoPetEntitiesCreated++;
                }
            }
            else if (existed)
            {
                applyResult.RealPetEntitiesUpdated++;
            }
            else
            {
                applyResult.RealPetEntitiesCreated++;
            }

            applyResult.AddLimited(applyResult.PseudoPetEntityDetails,
                $"{actor.EntityName}: {actor.Kind}/{actor.ClassificationReason}, class={entity.ClassName}, powersets={string.Join(", ", entity.PowersetFullName)}");
        }

        database.Entities = entities.ToArray();
    }

    private static void ApplyPseudoPetAbsorptionFlags(
        IPower midsPower,
        OmniPowerDefinition omniPower,
        IReadOnlyDictionary<string, OmniBuildActor> entityActors,
        OmniApplyResult applyResult)
    {
        var pseudoRefs = GetEntCreateEntityRefs(omniPower)
            .Where(entityRef =>
                (entityActors.TryGetValue(entityRef, out var actor) && actor.Kind == OmniBuildActorKind.PseudoPet) ||
                ShouldForcePseudoPetAbsorption(omniPower.FullName, entityRef))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (pseudoRefs.Count == 0)
        {
            return;
        }

        var changed = !midsPower.AbsorbSummonEffects || !midsPower.AbsorbSummonAttributes;
        midsPower.AbsorbSummonEffects = true;
        midsPower.AbsorbSummonAttributes = true;
        if (changed)
        {
            applyResult.PseudoPetAbsorptionFlagsEnabled++;
        }

        applyResult.AddLimited(applyResult.PseudoPetAbsorptionFlagDetails,
            $"{midsPower.FullName}: pseudo EntCreate {string.Join(", ", pseudoRefs)} -> AbsorbSummonEffects=true, AbsorbSummonAttributes=true");
    }

    private static bool ShouldForcePseudoPetAbsorption(string powerFullName, string entityRef)
    {
        return IsRequiredPseudoPetAbsorptionCase(powerFullName, entityRef);
    }

    private static string RequiredPseudoPetAbsorptionCaseKey(string powerFullName, string entityRef)
    {
        return $"{NormalizeLookupKey(powerFullName)}|{NormalizeLookupKey(entityRef)}";
    }

    private static bool IsRequiredPseudoPetAbsorptionCase(string powerFullName, string? entityRef)
    {
        if (string.IsNullOrWhiteSpace(powerFullName) || string.IsNullOrWhiteSpace(entityRef))
        {
            return false;
        }

        return RequiredPseudoPetAbsorptionCaseKeys.Contains(
            RequiredPseudoPetAbsorptionCaseKey(powerFullName, entityRef));
    }

    private static void RepairRequiredPseudoPetAbsorptionFlags(IDatabase database, OmniApplyResult applyResult)
    {
        foreach (var power in database.Power ?? [])
        {
            if (power == null)
            {
                continue;
            }

            var pseudoRefs = power.Effects
                .Where(effect => effect.EffectType == Enums.eEffectType.EntCreate &&
                                 IsRequiredPseudoPetAbsorptionCase(power.FullName, effect.Summon))
                .Select(effect => effect.Summon)
                .Where(summon => !string.IsNullOrWhiteSpace(summon))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (pseudoRefs.Count == 0)
            {
                continue;
            }

            var changed = !power.AbsorbSummonEffects || !power.AbsorbSummonAttributes;
            power.AbsorbSummonEffects = true;
            power.AbsorbSummonAttributes = true;
            power.IsModified = true;
            if (changed)
            {
                applyResult.PseudoPetAbsorptionFlagsEnabled++;
                applyResult.AddLimited(applyResult.PseudoPetAbsorptionFlagDetails,
                    $"{power.FullName}: final pseudo-pet repair {string.Join(", ", pseudoRefs)} -> AbsorbSummonEffects=true, AbsorbSummonAttributes=true");
            }
        }
    }

    private static IEnumerable<string> GetEntCreateEntityRefs(OmniPowerDefinition power)
    {
        foreach (var effect in power.Effects)
        {
            foreach (var entityRef in GetEntCreateEntityRefs(effect))
            {
                yield return entityRef;
            }
        }
    }

    private static IEnumerable<string> GetEntCreateEntityRefs(OmniEffectDefinition effect)
    {
        foreach (var template in effect.Templates)
        {
            var paramType = template.Params?.Value<string>("type") ?? string.Empty;
            var entityDef = template.Params?.Value<string>("entity_def") ?? string.Empty;
            if (paramType.Equals("EntCreate", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(entityDef))
            {
                yield return entityDef;
            }
        }

        foreach (var child in effect.ChildEffects)
        {
            foreach (var entityRef in GetEntCreateEntityRefs(child))
            {
                yield return entityRef;
            }
        }
    }

    private void EnsureScopedPowersets(
        IDatabase database,
        string exportRoot,
        OmniImportScope scope,
        OmniApplyResult applyResult,
        IProgress<OmniImportProgress>? progress,
        OmniExportManifest? manifest = null)
    {
        var existing = (database.Powersets ?? [])
            .Where(p => p != null && !string.IsNullOrWhiteSpace(p.FullName))
            .GroupBy(p => CanonicalizeOmniFullName(p.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

        var scopedPowersets = LoadScopedPowersets(exportRoot, scope, manifest: manifest).ToList();
        for (var powersetIndex = 0; powersetIndex < scopedPowersets.Count; powersetIndex++)
        {
            var omniPowerset = scopedPowersets[powersetIndex];
            ReportStageProgress(
                progress,
                OmniImportStageId.ApplyPowersets,
                string.Empty,
                powersetIndex + 1,
                scopedPowersets.Count,
                markComplete: powersetIndex + 1 >= scopedPowersets.Count);
            if (string.IsNullOrWhiteSpace(omniPowerset.FullName))
            {
                continue;
            }

            var midsFullName = CanonicalizeOmniFullName(omniPowerset.FullName);
            TrackEpicPowersetScope(omniPowerset, midsFullName, applyResult);
            if (existing.TryGetValue(midsFullName, out var midsPowerset) ||
                TryFindPowersetIdentityMatch(database, omniPowerset, scope.GetPowersetType(omniPowerset.FullName), out midsPowerset))
            {
                existing[midsFullName] = midsPowerset;
                applyResult.PowersetsMatched++;
                TrackEpicPowersetApply(midsFullName, applyResult, "matched",
                    $"{FormatAliasForReport(omniPowerset.FullName, midsFullName)}: matched {DescribePowersetIdentity(midsPowerset)}");
                var scopedSetType = scope.GetPowersetType(omniPowerset.FullName);
                ApplyPowersetMetadata(midsPowerset, omniPowerset, scopedSetType, preserveExistingValues: true);
                ApplyCanonicalPowersetName(midsPowerset, midsFullName);
                if (midsPowerset.SetType == Enums.ePowerSetType.None)
                {
                    midsPowerset.SetType = InferPowersetType(database, midsFullName);
                }
                ApplyPowersetIcon(midsPowerset, omniPowerset, applyResult);

                midsPowerset.IsModified = true;
                applyResult.PowersetsUpdated++;
                TrackEpicPowersetApply(midsFullName, applyResult, "updated",
                    $"{midsFullName}: updated {DescribePowersetIdentity(midsPowerset)}");
                applyResult.AddLimited(applyResult.UpdatedPowersets, FormatAliasForReport(omniPowerset.FullName, midsFullName));
                continue;
            }

            var created = CreatePowerset(omniPowerset);
            ApplyCanonicalPowersetName(created, midsFullName);
            var createdScopedSetType = scope.GetPowersetType(omniPowerset.FullName);
            if (createdScopedSetType != Enums.ePowerSetType.None)
            {
                created.SetType = createdScopedSetType;
            }

            if (created.SetType == Enums.ePowerSetType.None)
            {
                created.SetType = InferPowersetType(database, midsFullName);
            }
            ApplyPowersetIcon(created, omniPowerset, applyResult);

            var powersets = database.Powersets ?? [];
            Array.Resize(ref powersets, powersets.Length + 1);
            powersets[^1] = created;
            database.Powersets = powersets;
            existing[midsFullName] = created;
            applyResult.PowersetsCreated++;
            TrackEpicPowersetApply(midsFullName, applyResult, "created",
                $"{FormatAliasForReport(omniPowerset.FullName, midsFullName)}: created {DescribePowersetIdentity(created)}");
            applyResult.AddLimited(applyResult.CreatedPowersets, FormatAliasForReport(omniPowerset.FullName, midsFullName));
        }
    }

    private static bool TryFindPowersetIdentityMatch(
        IDatabase database,
        OmniPowersetDefinition omniPowerset,
        Enums.ePowerSetType scopedSetType,
        out IPowerset matchedPowerset)
    {
        matchedPowerset = null!;
        var omniFullName = CanonicalizeOmniFullName(omniPowerset.FullName);
        var omniGroup = NormalizeName(GroupNamePart(omniFullName));
        var omniSetName = NormalizeName(SetNamePart(omniFullName));
        var omniDisplayName = NormalizeName(string.IsNullOrWhiteSpace(omniPowerset.DisplayName)
            ? LastNamePart(omniFullName)
            : omniPowerset.DisplayName);
        var omniSetType = scopedSetType != Enums.ePowerSetType.None
            ? scopedSetType
            : MapPowersetType(omniPowerset.PowerCategory, omniPowerset.FullName);
        var omniArchetypes = omniPowerset.Archetypes
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(omniGroup) || string.IsNullOrWhiteSpace(omniSetName) || string.IsNullOrWhiteSpace(omniDisplayName))
        {
            return false;
        }

        if (omniGroup.Equals("epic", StringComparison.OrdinalIgnoreCase))
        {
            return TryFindEpicPowersetIdentityMatch(
                database,
                omniPowerset,
                omniFullName,
                omniSetType,
                out matchedPowerset);
        }

        if (IsStrictSetBonusPowerset(omniFullName))
        {
            return false;
        }

        var candidates = (database.Powersets ?? [])
            .Where(powerset => powerset != null &&
                               PowersetTypeMatches(powerset.SetType, omniSetType) &&
                               NormalizeName(powerset.GroupName).Equals(omniGroup, StringComparison.OrdinalIgnoreCase) &&
                               PowersetNameMatches(powerset, omniSetName, omniDisplayName) &&
                               PowersetArchetypeMatches(powerset, omniArchetypes))
            .Take(2)
            .ToList();

        if (candidates.Count != 1)
        {
            return false;
        }

        matchedPowerset = candidates[0];
        return true;
    }

    private static bool PowersetTypeMatches(Enums.ePowerSetType existingType, Enums.ePowerSetType omniType)
    {
        return omniType == Enums.ePowerSetType.None || existingType == Enums.ePowerSetType.None || existingType == omniType;
    }

    private static bool PowersetNameMatches(IPowerset powerset, string omniSetName, string omniDisplayName)
    {
        return NormalizeName(powerset.SetName).Equals(omniSetName, StringComparison.OrdinalIgnoreCase) ||
               NormalizeName(SetNamePart(powerset.FullName)).Equals(omniSetName, StringComparison.OrdinalIgnoreCase) ||
               NormalizeName(powerset.DisplayName).Equals(omniDisplayName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool PowersetArchetypeMatches(IPowerset powerset, IReadOnlySet<string> omniArchetypes)
    {
        if (omniArchetypes.Count == 0 || string.IsNullOrWhiteSpace(powerset.ATClass))
        {
            return true;
        }

        return omniArchetypes.Contains(NormalizeName(powerset.ATClass));
    }

    private static HashSet<string> EpicClassKeys(
        string fullName,
        IEnumerable<string> archetypes,
        string atClass)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddEpicClassKey(keys, atClass);
        foreach (var archetype in archetypes)
        {
            AddEpicClassKey(keys, archetype);
        }

        foreach (var token in SetNamePart(CanonicalizeOmniFullName(fullName))
                     .Split('_', StringSplitOptions.RemoveEmptyEntries))
        {
            AddEpicClassKey(keys, token);
        }

        return keys;
    }

    private static void AddEpicClassKey(ISet<string> keys, string value)
    {
        switch (NormalizeName(value))
        {
            case "classblaster":
            case "blaster":
                keys.Add("Class_Blaster");
                break;
            case "classbrute":
            case "brute":
                keys.Add("Class_Brute");
                break;
            case "brutetanker":
            case "brutetank":
            case "tankbrute":
            case "tankerbrute":
                keys.Add("Class_Brute");
                keys.Add("Class_Tanker");
                break;
            case "classcontroller":
            case "controller":
                keys.Add("Class_Controller");
                break;
            case "classcorruptor":
            case "corruptor":
            case "corr":
                keys.Add("Class_Corruptor");
                break;
            case "classdefender":
            case "defender":
            case "def":
                keys.Add("Class_Defender");
                break;
            case "defendercorruptor":
            case "defendercorr":
            case "corruptordefender":
            case "corrdefender":
            case "defcorr":
            case "corrdef":
                keys.Add("Class_Corruptor");
                keys.Add("Class_Defender");
                break;
            case "classdominator":
            case "dominator":
            case "domingator":
                keys.Add("Class_Dominator");
                break;
            case "controllerdominator":
            case "controllerdom":
            case "dominatorcontroller":
            case "domcontroller":
            case "controldom":
            case "domcontrol":
                keys.Add("Class_Controller");
                keys.Add("Class_Dominator");
                break;
            case "classmastermind":
            case "mastermind":
                keys.Add("Class_Mastermind");
                break;
            case "classscrapper":
            case "scrapper":
                keys.Add("Class_Scrapper");
                break;
            case "scrapperstalker":
            case "scrapperstalk":
            case "stalkerscrapper":
            case "stalkerscrap":
            case "scrapstalk":
            case "stalkscrap":
                keys.Add("Class_Scrapper");
                keys.Add("Class_Stalker");
                break;
            case "classsentinel":
            case "sentinel":
                keys.Add("Class_Sentinel");
                break;
            case "classstalker":
            case "stalker":
                keys.Add("Class_Stalker");
                break;
            case "classtanker":
            case "tanker":
            case "tank":
                keys.Add("Class_Tanker");
                break;
            case "classarachnossoldier":
            case "arachnossoldier":
            case "classarachnoswidow":
            case "arachnoswidow":
            case "veat":
                keys.Add("Class_Arachnos_Soldier");
                keys.Add("Class_Arachnos_Widow");
                break;
        }
    }

    private static string EpicFamilyName(string setName, IReadOnlySet<string> classKeys)
    {
        var keptTokens = setName
            .Split('_', StringSplitOptions.RemoveEmptyEntries)
            .Where(token =>
            {
                var tokenKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                AddEpicClassKey(tokenKeys, token);
                return tokenKeys.Count == 0 || !tokenKeys.Overlaps(classKeys);
            });

        return NormalizeName(string.Join("_", keptTokens));
    }

    private static bool TryFindEpicPowersetIdentityMatch(
        IDatabase database,
        OmniPowersetDefinition omniPowerset,
        string omniFullName,
        Enums.ePowerSetType omniSetType,
        out IPowerset matchedPowerset)
    {
        matchedPowerset = null!;
        var omniClassKeys = EpicClassKeys(omniPowerset.FullName, omniPowerset.Archetypes, string.Empty);
        if (omniClassKeys.Count == 0)
        {
            return false;
        }

        var omniFamily = EpicFamilyName(SetNamePart(omniFullName), omniClassKeys);
        var omniDisplayName = NormalizeName(string.IsNullOrWhiteSpace(omniPowerset.DisplayName)
            ? SetNamePart(omniFullName)
            : omniPowerset.DisplayName);

        var candidates = (database.Powersets ?? [])
            .Where(powerset => powerset != null &&
                               PowersetTypeMatches(powerset.SetType, omniSetType) &&
                               NormalizeName(powerset.GroupName).Equals("epic", StringComparison.OrdinalIgnoreCase))
            .Where(powerset =>
            {
                var existingClassKeys = EpicClassKeys(powerset.FullName, [], powerset.ATClass);
                if (!existingClassKeys.Overlaps(omniClassKeys))
                {
                    return false;
                }

                var existingFamily = EpicFamilyName(SetNamePart(powerset.FullName), existingClassKeys);
                var existingDisplayName = NormalizeName(powerset.DisplayName);
                return existingFamily.Equals(omniFamily, StringComparison.OrdinalIgnoreCase) ||
                       existingDisplayName.Equals(omniDisplayName, StringComparison.OrdinalIgnoreCase);
            })
            .Take(2)
            .ToList();

        if (candidates.Count != 1)
        {
            return false;
        }

        matchedPowerset = candidates[0];
        return true;
    }

    private IEnumerable<OmniPowersetDefinition> LoadScopedPowersets(
        string exportRoot,
        OmniImportScope scope,
        OmniImportResult? dryRunResult = null,
        OmniExportManifest? manifest = null)
    {
        var powersRoot = Path.Combine(exportRoot, "powers");
        if (!Directory.Exists(powersRoot))
        {
            yield break;
        }

        var files = manifest?.PowersetIndexFiles?.Count > 0
            ? manifest.PowersetIndexFiles
            : Directory.EnumerateFiles(powersRoot, "index.json", SearchOption.AllDirectories)
                .Where(f => !Path.GetFullPath(f).Equals(
                    Path.GetFullPath(Path.Combine(powersRoot, "index.json")),
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

        foreach (var file in files)
        {
            var relative = Path.GetRelativePath(powersRoot, file);
            if (IsCategoryRootIndex(relative))
            {
                continue;
            }

            var powerset = ReadJson<OmniPowersetDefinition>(file);
            if (powerset == null)
            {
                continue;
            }

            TrackPetPowersetDiscovery(relative, dryRunResult);
            TrackEpicPowersetDiscovery(relative, powerset, dryRunResult);
            if (scope.IsPowersetFileInScope(relative, powerset.FullName, powerset.Archetypes))
            {
                TrackPetPowersetInScope(relative, dryRunResult);
                TrackEpicPowersetInScope(relative, powerset, dryRunResult);
                TrackPowersetIconDryRun(powerset, scope.GetPowersetType(powerset.FullName), dryRunResult);
                yield return powerset;
            }
        }
    }

    private static bool IsCategoryRootIndex(string relativePath)
    {
        var parts = relativePath
            .Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && parts[1].Equals("index.json", StringComparison.OrdinalIgnoreCase);
    }

    private static Powerset CreatePowerset(OmniPowersetDefinition source)
    {
        var powerset = new Powerset();
        ApplyPowersetMetadata(powerset, source, Enums.ePowerSetType.None, preserveExistingValues: false);
        powerset.IsNew = true;
        powerset.IsModified = true;
        return powerset;
    }

    private static void EnsurePowersetsForScopedPowers(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        OmniImportScope scope,
        OmniApplyResult applyResult)
    {
        if (scopedPowers.Count == 0)
        {
            return;
        }

        var powersets = database.Powersets ?? [];
        var existing = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var setFullName in scopedPowers
                     .Select(power => CanonicalizeOmniFullName(string.IsNullOrWhiteSpace(power.Powerset)
                         ? FullSetName(power.FullName)
                         : power.Powerset))
                     .Where(setName => !string.IsNullOrWhiteSpace(setName))
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (existing.ContainsKey(setFullName))
            {
                continue;
            }

            var powerset = new Powerset
            {
                FullName = setFullName,
                GroupName = GroupNamePart(setFullName),
                SetName = SetNamePart(setFullName),
                DisplayName = SetNamePart(setFullName).Replace("_", " ", StringComparison.Ordinal),
                SetType = scope.GetPowersetType(setFullName),
                IsNew = true,
                IsModified = true
            };

            if (powerset.SetType == Enums.ePowerSetType.None)
            {
                powerset.SetType = InferPowersetType(database, setFullName);
            }

            Array.Resize(ref powersets, powersets.Length + 1);
            powersets[^1] = powerset;
            existing[setFullName] = powerset;
            applyResult.MissingScopedPowersetsRepaired++;
            applyResult.PowersetsCreated++;
            applyResult.AddLimited(applyResult.MissingScopedPowersetRepairDetails, setFullName);
            applyResult.AddLimited(applyResult.CreatedPowersets, $"{setFullName}: repaired missing scoped powerset from imported power references");
        }

        database.Powersets = powersets;
    }

    private static ImportIntegritySnapshot CaptureImportIntegritySnapshot(IDatabase database)
    {
        var powersets = database.Powersets ?? [];
        var powersetNames = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var powersetInfos = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => CreatePowersetIntegrityInfo(group.First()), StringComparer.OrdinalIgnoreCase);

        var powerInfos = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Select(power => CreatePowerIntegrityInfo(power, powersetNames))
            .GroupBy(info => info.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        return new ImportIntegritySnapshot(powerInfos, powersetInfos);
    }

    private static List<PowersetIconInfo> CapturePowersetIcons(IDatabase database)
    {
        return (database.Powersets ?? [])
            .Where(powerset => powerset != null &&
                               !string.IsNullOrWhiteSpace(powerset.ImageName))
            .Select(powerset => new PowersetIconInfo(
                CanonicalizeOmniFullName(powerset.FullName),
                NormalizeName(powerset.GroupName),
                NormalizeName(powerset.SetName),
                NormalizeName(powerset.DisplayName),
                powerset.ImageName))
            .ToList();
    }

    private static void RestorePowersetIcons(IDatabase database, IReadOnlyCollection<PowersetIconInfo> iconCache)
    {
        if (iconCache.Count == 0)
        {
            return;
        }

        foreach (var powerset in database.Powersets ?? [])
        {
            if (powerset == null || !string.IsNullOrWhiteSpace(powerset.ImageName))
            {
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(powerset.FullName);
            var groupName = NormalizeName(powerset.GroupName);
            var setName = NormalizeName(powerset.SetName);
            var displayName = NormalizeName(powerset.DisplayName);
            var match = iconCache.FirstOrDefault(icon =>
                            string.Equals(icon.CanonicalFullName, canonicalFullName, StringComparison.OrdinalIgnoreCase)) ??
                        iconCache.FirstOrDefault(icon =>
                            string.Equals(icon.GroupName, groupName, StringComparison.OrdinalIgnoreCase) &&
                            (string.Equals(icon.SetName, setName, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(icon.DisplayName, displayName, StringComparison.OrdinalIgnoreCase)));

            if (match == null)
            {
                continue;
            }

            powerset.ImageName = match.ImageName;
            powerset.IsModified = true;
        }
    }

    private static void ApplyPowersetIcon(
        IPowerset powerset,
        OmniPowersetDefinition source,
        OmniApplyResult applyResult)
    {
        if (!ShouldAssignPowersetIcon(source, powerset.SetType))
        {
            return;
        }

        if (TryResolvePowersetIcon(source, out var imageName, out var candidates))
        {
            if (string.Equals(powerset.ImageName, imageName, StringComparison.OrdinalIgnoreCase))
            {
                applyResult.PoolPowersetIconsPreserved++;
                applyResult.AddLimited(applyResult.PoolPowersetIconAuditDetails,
                    $"{CanonicalizeOmniFullName(source.FullName)}: kept icon {powerset.ImageName}; Omni icon={source.Icon}");
                return;
            }

            var priorImageName = string.IsNullOrWhiteSpace(powerset.ImageName) ? "<blank>" : powerset.ImageName;
            powerset.ImageName = imageName;
            powerset.IsModified = true;
            applyResult.PoolPowersetIconsAssigned++;
            applyResult.AddLimited(applyResult.PoolIconAssignmentDetails,
                $"{CanonicalizeOmniFullName(source.FullName)}: set icon {imageName} from Omni icon={source.Icon} (was {priorImageName})");
            return;
        }

        applyResult.PoolPowersetIconsMissingAssets++;
        applyResult.AddLimited(applyResult.MissingPoolIconAssetDetails,
            $"{CanonicalizeOmniFullName(source.FullName)}: Omni icon={source.Icon}; candidates={string.Join(", ", candidates)}");
    }

    private static void TrackPowersetIconDryRun(
        OmniPowersetDefinition source,
        Enums.ePowerSetType scopedSetType,
        OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var powersetType = scopedSetType != Enums.ePowerSetType.None
            ? scopedSetType
            : MapPowersetType(source.PowerCategory, source.FullName);
        if (!ShouldAssignPowersetIcon(source, powersetType))
        {
            return;
        }

        dryRunResult.Report.PoolPowersetIconAuditCount++;
        if (TryResolvePowersetIcon(source, out var imageName, out var candidates))
        {
            dryRunResult.Report.PoolIconAssignmentsAvailableCount++;
            dryRunResult.Report.AddLimited(dryRunResult.Report.PoolIconAssignments,
                $"{CanonicalizeOmniFullName(source.FullName)}: Omni icon={source.Icon}; resolved asset={imageName}");
        }
        else
        {
            dryRunResult.Report.MissingPoolIconAssetCount++;
            dryRunResult.Report.AddLimited(dryRunResult.Report.MissingPoolIconAssets,
                $"{CanonicalizeOmniFullName(source.FullName)}: Omni icon={source.Icon}; candidates={string.Join(", ", candidates)}");
        }

        dryRunResult.Report.AddLimited(dryRunResult.Report.PoolPowersetIconAudit,
            $"{CanonicalizeOmniFullName(source.FullName)}: display={source.DisplayName}, internal={source.Name}, icon={source.Icon}");
    }

    private static void TrackPoolImportIntegrity(IDatabase database, OmniApplyResult applyResult)
    {
        var poolPowersets = (database.Powersets ?? [])
            .Where(powerset => powerset != null && powerset.SetType == Enums.ePowerSetType.Pool)
            .OrderBy(powerset => powerset.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var powerset in poolPowersets)
        {
            var powerNames = powerset.Powers?
                .Where(power => power != null)
                .Select(power => power.DisplayName)
                .ToArray() ?? [];
            var iconState = ResolveFinalIconState(powerset.ImageName);
            if (string.IsNullOrWhiteSpace(powerset.ImageName) || !iconState.Resolved)
            {
                applyResult.PoolIconIntegrityIssues++;
            }

            applyResult.AddLimited(applyResult.PoolIconIntegrityDetails,
                $"{powerset.FullName}: display={powerset.DisplayName}, nID={powerset.nID}, ImageName={(string.IsNullOrWhiteSpace(powerset.ImageName) ? "<blank>" : powerset.ImageName)}, asset={(iconState.Resolved ? iconState.FileName : "<missing>")}, powers={powerNames.Length} [{string.Join(", ", powerNames)}]");
        }

        var speed = poolPowersets.FirstOrDefault(powerset =>
            powerset.FullName.Equals("Pool.Speed", StringComparison.OrdinalIgnoreCase));
        var sorcery = poolPowersets.FirstOrDefault(powerset =>
            powerset.FullName.Equals("Pool.Sorcery", StringComparison.OrdinalIgnoreCase));

        if (speed != null)
        {
            foreach (var power in speed.Powers.Where(power => power != null &&
                                                              power.FullSetName.Equals("Pool.Sorcery", StringComparison.OrdinalIgnoreCase)))
            {
                applyResult.PoolPowerLinkIntegrityIssues++;
                applyResult.AddLimited(applyResult.PoolPowerLinkIntegrityDetails,
                    $"Pool.Speed contains Sorcery-linked power {power.FullName} with FullSetName={power.FullSetName}, PowerSetID={power.PowerSetID}, PowerSetIndex={power.PowerSetIndex}");
            }
        }
        else
        {
            applyResult.PoolPowerLinkIntegrityIssues++;
            applyResult.AddLimited(applyResult.PoolPowerLinkIntegrityDetails, "Pool.Speed powerset is missing after import.");
        }

        if (sorcery == null)
        {
            applyResult.PoolPowerLinkIntegrityIssues++;
            applyResult.AddLimited(applyResult.PoolPowerLinkIntegrityDetails, "Pool.Sorcery powerset is missing after import.");
            return;
        }

        var expectedSorceryPowers = new[]
        {
            "Pool.Sorcery.Spirit_Ward",
            "Pool.Sorcery.Arcane_Bolt",
            "Pool.Sorcery.Mystic_Flight",
            "Pool.Sorcery.Enflame",
            "Pool.Sorcery.Rune_of_Protection"
        };
        var sorceryPowerNames = sorcery.Powers
            .Where(power => power != null)
            .Select(power => power.FullName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var expectedPower in expectedSorceryPowers)
        {
            if (sorceryPowerNames.Contains(expectedPower))
            {
                continue;
            }

            applyResult.PoolPowerLinkIntegrityIssues++;
            applyResult.AddLimited(applyResult.PoolPowerLinkIntegrityDetails,
                $"Pool.Sorcery missing expected power {expectedPower}; actual=[{string.Join(", ", sorceryPowerNames)}]");
        }
    }

    private static (bool Resolved, string FileName) ResolveFinalIconState(string imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
        {
            return (false, string.Empty);
        }

        return AssetManager.TryResolveImageFileName([imageName], out var fileName)
            ? (true, fileName)
            : (false, string.Empty);
    }

    private static bool TryResolvePowersetIcon(
        OmniPowersetDefinition source,
        out string imageName,
        out IReadOnlyList<string> candidates)
    {
        candidates = BuildPowersetIconCandidates(source).ToArray();
        return AssetManager.TryResolveImageFileName(candidates, out imageName);
    }

    private static IEnumerable<string> BuildPowersetIconCandidates(OmniPowersetDefinition source)
    {
        var icon = source.Icon?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(icon))
        {
            yield return icon;
            yield return Path.ChangeExtension(icon, ".png") ?? icon;
            var iconBase = Path.GetFileNameWithoutExtension(icon);
            if (!string.IsNullOrWhiteSpace(iconBase))
            {
                yield return $"{iconBase}.png";
                var withoutSetSuffix = iconBase.EndsWith("_set", StringComparison.OrdinalIgnoreCase)
                    ? iconBase[..^"_set".Length]
                    : iconBase;
                yield return $"{withoutSetSuffix}.png";
                yield return $"{withoutSetSuffix.Replace("_", string.Empty)}.png";
            }
        }

        var displayName = source.DisplayName?.Trim() ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            yield return $"{displayName.Replace(" ", string.Empty).Replace("_", string.Empty)}.png";
        }

        var internalName = string.IsNullOrWhiteSpace(source.Name)
            ? LastNamePart(source.FullName)
            : source.Name;
        if (!string.IsNullOrWhiteSpace(internalName))
        {
            yield return $"{internalName.Replace("_", string.Empty)}.png";
        }

        if (icon.Equals("Speed_set.ico", StringComparison.OrdinalIgnoreCase))
        {
            yield return "SuperSpeed.png";
        }
    }

    private static bool ShouldAssignPowersetIcon(OmniPowersetDefinition source, Enums.ePowerSetType setType)
    {
        if (string.IsNullOrWhiteSpace(source.Icon))
        {
            return false;
        }

        return setType is Enums.ePowerSetType.Primary
            or Enums.ePowerSetType.Secondary
            or Enums.ePowerSetType.Pool
            or Enums.ePowerSetType.Ancillary
            or Enums.ePowerSetType.Incarnate
            or Enums.ePowerSetType.Redirect;
    }

    private static PowersetIntegrityInfo CreatePowersetIntegrityInfo(IPowerset powerset)
    {
        return new PowersetIntegrityInfo(
            CanonicalizeOmniFullName(powerset.FullName),
            powerset.FullName,
            powerset.GroupName,
            powerset.SetName,
            powerset.DisplayName,
            powerset.SetType,
            powerset.ATClass);
    }

    private static PowerIntegrityInfo CreatePowerIntegrityInfo(IPower power, IReadOnlySet<string> powersetNames)
    {
        var fullSetName = power.FullSetName;
        var missingFullSetName = string.IsNullOrWhiteSpace(fullSetName);
        var unresolvedFullSetName = !missingFullSetName &&
                                    !powersetNames.Contains(CanonicalizeOmniFullName(fullSetName));
        var orphan = power.PowerSetID < 0 || missingFullSetName || unresolvedFullSetName;
        return new PowerIntegrityInfo(
            power.FullName,
            fullSetName,
            power.GroupName,
            power.SetName,
            power.PowerName,
            power.DisplayName,
            power.PowerSetID,
            power.PowerSetIndex,
            power.HiddenPower,
            power.NeverAutoUpdate,
            orphan,
            missingFullSetName,
            unresolvedFullSetName);
    }

    private static void TrackImportIntegrityAudit(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications,
        ImportIntegritySnapshot before,
        OmniApplyResult applyResult)
    {
        var after = CaptureImportIntegritySnapshot(database);
        var powersetMemberships = BuildPowersetMembershipLookup(database);
        var beforeOrphans = before.Powers.Values
            .Where(power => power.IsOrphan)
            .Select(power => power.FullName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var afterOrphans = after.Powers.Values
            .Where(power => power.IsOrphan)
            .ToList();

        applyResult.OrphanPowersBeforeImport = beforeOrphans.Count;
        applyResult.OrphanPowersAfterImport = afterOrphans.Count;

        foreach (var orphan in afterOrphans.Where(power => !beforeOrphans.Contains(power.FullName)))
        {
            applyResult.NewOrphanPowersIntroduced++;
            applyResult.AddLimited(applyResult.NewOrphanPowerDetails, DescribePowerIntegrity(orphan));
        }

        var scopedNames = scopedPowers
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        foreach (var scopedName in scopedNames)
        {
            var disposition = classifications.TryGetValue(scopedName, out var classification)
                ? classification.ScopedDisposition
                : OmniScopedPowerDisposition.MainImportVisible;
            if (disposition == OmniScopedPowerDisposition.Excluded)
            {
                applyResult.ExcludedScopedOmniPowers++;
                applyResult.AddLimited(applyResult.ExcludedScopedOmniPowerDetails,
                    $"{scopedName}: intentionally excluded from main database import by scoped disposition.");
                continue;
            }

            if (disposition == OmniScopedPowerDisposition.PetManifestOwned)
            {
                applyResult.ManifestOwnedScopedOmniPowers++;
                applyResult.AddLimited(applyResult.ManifestOwnedScopedOmniPowerDetails,
                    $"{scopedName}: owned through pet/actor manifest import; final database.Power presence not required.");
                continue;
            }

            if (!after.Powers.TryGetValue(scopedName, out var powerInfo))
            {
                if (TryGetAcceptedScopedCanonicalReplacement(scopedName, out var replacementFullName) &&
                    after.Powers.TryGetValue(replacementFullName, out var replacementInfo) &&
                    !replacementInfo.IsOrphan &&
                    powersetMemberships.TryGetValue(CanonicalizeOmniFullName(replacementInfo.FullSetName), out var replacementMembers) &&
                    replacementMembers.Contains(replacementFullName))
                {
                    applyResult.AcceptedCanonicalScopedPowerReplacements++;
                    applyResult.AddLimited(applyResult.AcceptedCanonicalScopedPowerReplacementDetails,
                        $"{scopedName}: accepted canonical replacement {replacementFullName} retained in final database graph.");
                    continue;
                }

                applyResult.OrphanedScopedOmniPowers++;
                applyResult.AddLimited(applyResult.OrphanedScopedOmniPowerDetails, $"{scopedName}: imported scoped power not found in database.Power after import");
                continue;
            }

            if (powerInfo.IsOrphan)
            {
                applyResult.OrphanedScopedOmniPowers++;
                applyResult.AddLimited(applyResult.OrphanedScopedOmniPowerDetails, DescribePowerIntegrity(powerInfo));
                continue;
            }

            var owningSet = CanonicalizeOmniFullName(powerInfo.FullSetName);
            if (string.IsNullOrWhiteSpace(owningSet) ||
                !powersetMemberships.TryGetValue(owningSet, out var members) ||
                !members.Contains(scopedName))
            {
                applyResult.OrphanedScopedOmniPowers++;
                applyResult.AddLimited(applyResult.OrphanedScopedOmniPowerDetails,
                    $"{scopedName}: exists in database.Power but is not attached to owning powerset {powerInfo.FullSetName} in final graph.");
            }
        }

        foreach (var beforePowerset in before.Powersets)
        {
            if (!after.Powersets.TryGetValue(beforePowerset.Key, out var afterPowerset))
            {
                continue;
            }

            var beforeValue = beforePowerset.Value;
            if (PowersetIntegrityEquals(beforeValue, afterPowerset))
            {
                continue;
            }

            applyResult.PowersetIdentityChanges++;
            applyResult.AddLimited(applyResult.PowersetIdentityChangeDetails,
                $"{beforeValue.FullName}: {DescribePowersetIntegrity(beforeValue)} -> {DescribePowersetIntegrity(afterPowerset)}");
        }

        TrackDuplicateCompositePowerIdentities(database, applyResult);
        TrackStaffMasteryFinalState(database, applyResult);

        applyResult.AddLimited(applyResult.ImportIntegrityAuditDetails,
            $"Orphans before={applyResult.OrphanPowersBeforeImport}, after={applyResult.OrphanPowersAfterImport}, new={applyResult.NewOrphanPowersIntroduced}, scoped orphaned/missing={applyResult.OrphanedScopedOmniPowers}, accepted replacements={applyResult.AcceptedCanonicalScopedPowerReplacements}, scoped excluded={applyResult.ExcludedScopedOmniPowers}, scoped manifest-owned={applyResult.ManifestOwnedScopedOmniPowers}");
    }

    private static Dictionary<string, HashSet<string>> BuildPowersetMembershipLookup(IDatabase database)
    {
        return (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.First().Powers
                    .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
                    .Select(power => CanonicalizeOmniFullName(power.FullName))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, IPowerset> BuildCanonicalPowersetLookup(IDatabase database)
    {
        return (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    }

    private static bool TryGetAcceptedScopedCanonicalReplacement(string scopedFullName, out string replacementFullName)
    {
        var canonicalFullName = CanonicalizeOmniFullName(scopedFullName);
        if (!string.IsNullOrWhiteSpace(canonicalFullName) &&
            AcceptedScopedCanonicalPowerReplacements.TryGetValue(canonicalFullName, out var replacement))
        {
            replacementFullName = CanonicalizeOmniFullName(replacement);
            return true;
        }

        replacementFullName = string.Empty;
        return false;
    }

    private static void VerifyAndRepairScopedPowerGraph(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications,
        IReadOnlyDictionary<string, OmniPowersetDefinition> scopedPowersetLookup,
        IReadOnlyDictionary<string, OmniBuildActor> entityActors,
        OmniApplyResult applyResult,
        ref int nextStaticIndex)
    {
        var requiredScopedPowers = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName) && ShouldMainImportScopedPower(power, classifications))
            .ToList();
        if (requiredScopedPowers.Count == 0)
        {
            return;
        }

        var existingPowers = GetCanonicalizableDatabasePowers(database);
        var midsPowers = BuildCanonicalPowerLookup(existingPowers);
        var powersetsByName = BuildCanonicalPowersetLookup(database);
        var repaired = false;

        foreach (var omniPower in requiredScopedPowers)
        {
            var midsFullName = CanonicalizeOmniFullName(omniPower.FullName);
            var canonicalPowerset = PowerPowersetFullName(omniPower);
            if (string.IsNullOrWhiteSpace(midsFullName) || string.IsNullOrWhiteSpace(canonicalPowerset))
            {
                continue;
            }

            if (!powersetsByName.TryGetValue(canonicalPowerset, out var owningPowerset) &&
                scopedPowersetLookup.TryGetValue(canonicalPowerset, out var scopedPowerset))
            {
                var createdPowerset = CreatePowerset(scopedPowerset);
                if (createdPowerset.SetType == Enums.ePowerSetType.None)
                {
                    createdPowerset.SetType = MapPowersetType(scopedPowerset.PowerCategory, scopedPowerset.FullName);
                }
                var powersets = database.Powersets ?? [];
                Array.Resize(ref powersets, powersets.Length + 1);
                powersets[^1] = createdPowerset;
                database.Powersets = powersets;
                powersetsByName[canonicalPowerset] = createdPowerset;
                owningPowerset = createdPowerset;
                repaired = true;
                applyResult.MissingScopedPowersetsRepaired++;
                applyResult.PowersetsCreated++;
                applyResult.AddLimited(applyResult.MissingScopedPowersetRepairDetails,
                    $"{canonicalPowerset}: created during final scoped graph verification");
                applyResult.AddLimited(applyResult.CreatedPowersets,
                    $"{canonicalPowerset}: final scoped graph verification");
            }

            if (!midsPowers.TryGetValue(midsFullName, out var midsPower) &&
                TryFindCompositePowerIdentityMatch(existingPowers, omniPower, midsFullName, scopedPowersetLookup, out var compositeMatch))
            {
                var priorFullName = compositeMatch.FullName;
                ApplyCanonicalPowerName(compositeMatch, midsFullName);
                compositeMatch.IsModified = true;
                midsPower = compositeMatch;
                midsPowers[midsFullName] = compositeMatch;
                repaired = true;
                applyResult.ScopedPowerSetIdentityRepairs++;
                applyResult.AddLimited(applyResult.ScopedPowerSetIdentityRepairDetails,
                    $"{priorFullName} -> {midsFullName}: repaired during final scoped graph verification");
            }

            if (!midsPowers.TryGetValue(midsFullName, out midsPower))
            {
                midsPower = CreatePower(database, omniPower, nextStaticIndex++);
                ApplyCanonicalPowerName(midsPower, midsFullName);
                ApplyImportedPowerState(database, midsPower, omniPower,
                    classifications.TryGetValue(midsFullName, out var classification)
                        ? classification
                        : new OmniPowerClassification(),
                    entityActors,
                    applyResult);
                midsPowers[midsFullName] = midsPower;
                existingPowers.Add(midsPower);
                repaired = true;
                applyResult.PowersCreated++;
                applyResult.PowersUpdated++;
                applyResult.ScopedPowerSetIdentityRepairs++;
                applyResult.AddLimited(applyResult.CreatedPowers,
                    $"{midsFullName}: created during final scoped graph verification");
                applyResult.AddLimited(applyResult.UpdatedPowers,
                    $"{midsFullName}: populated during final scoped graph verification");
                applyResult.AddLimited(applyResult.ScopedPowerSetIdentityRepairDetails,
                    $"{midsFullName}: created missing required scoped power during final graph verification");
            }

            if (owningPowerset == null)
            {
                continue;
            }

            var inPowersetArray = owningPowerset.Powers.Any(power =>
                power != null &&
                string.Equals(CanonicalizeOmniFullName(power.FullName), midsFullName, StringComparison.OrdinalIgnoreCase));
            if (!inPowersetArray)
            {
                EnsurePowerAttachedToPowerset(database, owningPowerset, midsPower);
                repaired = true;
                applyResult.ScopedPowerSetIdentityRepairs++;
                applyResult.AddLimited(applyResult.ScopedPowerSetIdentityRepairDetails,
                    $"{midsFullName}: restored final owning powerset attachment for {owningPowerset.FullName}");
            }
        }

        if (repaired)
        {
            SortPowersByPowersetAndLevel(database, applyResult);
        }
    }

    private static void VerifyAndRepairRetainedTemporaryPowerGraph(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications,
        IReadOnlyDictionary<string, OmniPowersetDefinition> scopedPowersetLookup,
        IReadOnlyDictionary<string, OmniBuildActor> entityActors,
        OmniApplyResult applyResult,
        ref int nextStaticIndex)
    {
        var requiredTemporaryPowers = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName) &&
                            ShouldMainImportScopedPower(power, classifications) &&
                            IsRetainedTemporaryIntegrityPowerset(PowerPowersetFullName(power)))
            .ToList();
        if (requiredTemporaryPowers.Count == 0)
        {
            return;
        }

        var existingPowers = GetCanonicalizableDatabasePowers(database);
        var midsPowers = BuildCanonicalPowerLookup(existingPowers);
        var powersetsByName = BuildCanonicalPowersetLookup(database);
        var repaired = false;

        foreach (var omniPower in requiredTemporaryPowers)
        {
            var midsFullName = CanonicalizeOmniFullName(omniPower.FullName);
            var canonicalPowerset = PowerPowersetFullName(omniPower);
            if (string.IsNullOrWhiteSpace(midsFullName) || string.IsNullOrWhiteSpace(canonicalPowerset))
            {
                continue;
            }

            if (!powersetsByName.TryGetValue(canonicalPowerset, out var owningPowerset) &&
                scopedPowersetLookup.TryGetValue(canonicalPowerset, out var scopedPowerset))
            {
                var createdPowerset = CreatePowerset(scopedPowerset);
                var powersets = database.Powersets ?? [];
                Array.Resize(ref powersets, powersets.Length + 1);
                powersets[^1] = createdPowerset;
                database.Powersets = powersets;
                powersetsByName[canonicalPowerset] = createdPowerset;
                owningPowerset = createdPowerset;
                repaired = true;
                applyResult.PowersetsCreated++;
                applyResult.AddLimited(applyResult.CreatedPowersets,
                    $"{canonicalPowerset}: created during retained temporary power verification");
                applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                    $"{canonicalPowerset}: created missing retained temporary powerset during final verification");
            }

            if (!midsPowers.TryGetValue(midsFullName, out var midsPower))
            {
                midsPower = CreatePower(database, omniPower, nextStaticIndex++);
                ApplyCanonicalPowerName(midsPower, midsFullName);
                ApplyImportedPowerState(
                    database,
                    midsPower,
                    omniPower,
                    classifications.TryGetValue(midsFullName, out var classification)
                        ? classification
                        : new OmniPowerClassification(),
                    entityActors,
                    applyResult);
                midsPowers[midsFullName] = midsPower;
                existingPowers.Add(midsPower);
                repaired = true;
                applyResult.PowersCreated++;
                applyResult.PowersUpdated++;
                applyResult.AddLimited(applyResult.CreatedPowers,
                    $"{midsFullName}: created during retained temporary power verification");
                applyResult.AddLimited(applyResult.UpdatedPowers,
                    $"{midsFullName}: populated during retained temporary power verification");
                applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                    $"{midsFullName}: created missing retained temporary power during final verification");
            }
            else if (classifications.TryGetValue(midsFullName, out var classification))
            {
                ApplyPowerClassification(midsPower, classification);
                midsPower.IsModified = true;
            }

            if (owningPowerset == null)
            {
                continue;
            }

            if (!string.Equals(CanonicalizeOmniFullName(midsPower.FullSetName), canonicalPowerset, StringComparison.OrdinalIgnoreCase))
            {
                ApplyCanonicalPowerName(midsPower, midsFullName);
                midsPower.PowerSetID = owningPowerset.nID;
                midsPower.IsModified = true;
                repaired = true;
                applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                    $"{midsFullName}: restored owning powerset identity to {canonicalPowerset}");
            }

            var inPowersetArray = owningPowerset.Powers.Any(power =>
                power != null &&
                string.Equals(CanonicalizeOmniFullName(power.FullName), midsFullName, StringComparison.OrdinalIgnoreCase));
            if (!inPowersetArray)
            {
                EnsurePowerAttachedToPowerset(database, owningPowerset, midsPower);
                repaired = true;
                applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                    $"{midsFullName}: restored final owning powerset attachment for {owningPowerset.FullName}");
            }
        }

        if (repaired)
        {
            SortPowersByPowersetAndLevel(database, applyResult);
        }
    }

    private static bool IsRetainedTemporaryIntegrityPowerset(string? fullSetName)
    {
        var canonical = CanonicalizeOmniFullName(fullSetName ?? string.Empty);
        return RetainedTemporaryIntegrityPowersetNames.Contains(canonical, StringComparer.OrdinalIgnoreCase);
    }

    private static void TrackRetainedTemporaryPowerIntegrity(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications,
        OmniApplyResult applyResult)
    {
        var expectedByPowerset = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName) &&
                            ShouldMainImportScopedPower(power, classifications) &&
                            IsRetainedTemporaryIntegrityPowerset(PowerPowersetFullName(power)))
            .GroupBy(power => PowerPowersetFullName(power), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(power => CanonicalizeOmniFullName(power.FullName))
                    .Where(fullName => !string.IsNullOrWhiteSpace(fullName))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                StringComparer.OrdinalIgnoreCase);

        applyResult.RetainedTemporaryPowersetsTracked = expectedByPowerset.Count;
        applyResult.RetainedTemporaryPowersTracked = expectedByPowerset.Sum(entry => entry.Value.Count);
        applyResult.RetainedTemporaryHiddenPowers = 0;
        applyResult.RetainedTemporaryIntegrityFailures = 0;

        if (expectedByPowerset.Count == 0)
        {
            return;
        }

        var powersetsByName = BuildCanonicalPowersetLookup(database);
        var powersByName = BuildCanonicalPowerLookup(GetCanonicalizableDatabasePowers(database));

        foreach (var (powersetFullName, expectedPowers) in expectedByPowerset.OrderBy(entry => entry.Key, StringComparer.OrdinalIgnoreCase))
        {
            powersetsByName.TryGetValue(powersetFullName, out var powerset);
            var attachedPowers = powerset?.Powers
                .Where(power => power != null)
                .Select(power => CanonicalizeOmniFullName(power.FullName))
                .Where(fullName => !string.IsNullOrWhiteSpace(fullName))
                .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var hiddenCount = 0;
            var missingCount = 0;
            var detachedCount = 0;

            foreach (var expectedPowerFullName in expectedPowers.OrderBy(value => value, StringComparer.OrdinalIgnoreCase))
            {
                if (!powersByName.TryGetValue(expectedPowerFullName, out var databasePower))
                {
                    missingCount++;
                    applyResult.RetainedTemporaryIntegrityFailures++;
                    applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                        $"{expectedPowerFullName}: missing from database.Power");
                    continue;
                }

                if (databasePower.HiddenPower)
                {
                    hiddenCount++;
                }

                var canonicalSet = CanonicalizeOmniFullName(databasePower.FullSetName);
                var attached = attachedPowers.Contains(expectedPowerFullName) &&
                               string.Equals(canonicalSet, powersetFullName, StringComparison.OrdinalIgnoreCase) &&
                               databasePower.PowerSetID > -1 &&
                               databasePower.PowerSetIndex > -1;
                if (!attached)
                {
                    detachedCount++;
                    applyResult.RetainedTemporaryIntegrityFailures++;
                    applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                        $"{expectedPowerFullName}: retained but detached from {powersetFullName} (FullSetName={databasePower.FullSetName}, PowerSetID={databasePower.PowerSetID}, PowerSetIndex={databasePower.PowerSetIndex})");
                }
            }

            applyResult.RetainedTemporaryHiddenPowers += hiddenCount;
            applyResult.AddLimited(applyResult.RetainedTemporaryIntegrityDetails,
                $"{powersetFullName}: expected={expectedPowers.Count}, attached={expectedPowers.Count - missingCount - detachedCount}, hidden={hiddenCount}, missing={missingCount}, detached={detachedCount}");
        }
    }

    private static void EnsurePowerAttachedToPowerset(IDatabase database, IPowerset owningPowerset, IPower midsPower)
    {
        if (owningPowerset == null || midsPower == null)
        {
            return;
        }

        var canonicalPowerFullName = CanonicalizeOmniFullName(midsPower.FullName);
        if (string.IsNullOrWhiteSpace(canonicalPowerFullName))
        {
            return;
        }

        if (owningPowerset.Powers.Any(power =>
                power != null &&
                string.Equals(CanonicalizeOmniFullName(power.FullName), canonicalPowerFullName, StringComparison.OrdinalIgnoreCase)))
        {
            return;
        }

        var powerIndex = Array.FindIndex(database.Power ?? [], candidate => ReferenceEquals(candidate, midsPower));
        var nextIndex = owningPowerset.Powers.Length;

        var linkedPowers = owningPowerset.Powers;
        Array.Resize(ref linkedPowers, nextIndex + 1);
        linkedPowers[nextIndex] = midsPower;
        owningPowerset.Powers = linkedPowers;

        if (powerIndex >= 0)
        {
            var nPower = owningPowerset.Power;
            Array.Resize(ref nPower, nextIndex + 1);
            nPower[nextIndex] = powerIndex;
            owningPowerset.Power = nPower;
            midsPower.PowerIndex = powerIndex;
        }

        midsPower.PowerSetID = owningPowerset.nID;
        midsPower.PowerSetIndex = nextIndex;
        midsPower.IsModified = true;
    }

    private static void ApplyImportedPowerState(
        IDatabase database,
        IPower midsPower,
        OmniPowerDefinition omniPower,
        OmniPowerClassification classification,
        IReadOnlyDictionary<string, OmniBuildActor> entityActors,
        OmniApplyResult applyResult)
    {
        var midsFullName = CanonicalizeOmniFullName(omniPower.FullName);
        var priorAttackTypes = midsPower.AttackTypes;
        var priorClickBuff = midsPower.ClickBuff;

        ApplyPowerMetadata(midsPower, omniPower);
        ApplyCanonicalPowerName(midsPower, midsFullName);
        ApplyPowerClassification(midsPower, classification);
        ApplyPseudoPetAbsorptionFlags(midsPower, omniPower, entityActors, applyResult);
        ApplyPlannerRuntimeMetadata(midsPower, omniPower);
        CapturePowerImportMetadata(database, midsPower, midsFullName, omniPower);
        TrackApplyClassification(midsFullName, classification, priorClickBuff, applyResult);
        TrackApplyAttackVectors(midsFullName, priorAttackTypes, midsPower.AttackTypes, omniPower, applyResult);
        TrackApplyModes(midsFullName, omniPower, classification, applyResult);

        var effects = OmniMidsMapper.FlattenEffects(omniPower, applyResult).Cast<IEffect>().ToList();
        var nextUniqueId = effects.Count == 0 ? 1 : effects.Max(effect => effect.UniqueID) + 1;
        foreach (var redirect in omniPower.Redirects.Where(redirect => !string.IsNullOrWhiteSpace(redirect.Name)))
        {
            var redirectEffect = OmniMidsMapper.CreatePowerRedirectEffect(omniPower.FullName, redirect);
            redirectEffect.UniqueID = nextUniqueId++;
            redirectEffect.ActiveConditionals = redirectEffect.AdvancedConditions.ToLegacyActiveConditionals();
            if (redirectEffect.AdvancedConditions.Rows.Any(row =>
                    row.Kind == AdvancedConditionKind.SourceMode &&
                    row.Subject.Equals("FastSnipe", StringComparison.OrdinalIgnoreCase) &&
                    row.RawExpression.Contains("kEngaged", StringComparison.OrdinalIgnoreCase)))
            {
                applyResult.SnipeEngagedAliases++;
                applyResult.AddLimited(applyResult.SnipeEngagedAliasDetails,
                    $"{midsFullName}: {redirect.Requires} -> FastSnipe");
            }

            effects.Add(redirectEffect);
            applyResult.RedirectEffectsAdded++;
        }

        foreach (var effect in effects)
        {
            effect.PowerFullName = midsPower.FullName;
            effect.ActiveConditionals = effect.AdvancedConditions.ToLegacyActiveConditionals();
        }

        ApplyStrengthsDisallowedToEffects(effects, midsPower.IgnoreEnh, midsPower.TypedEnhancementRestrictions);
        midsPower.Effects = effects.ToArray();
        midsPower.HasGrantPowerEffect = effects.Any(effect => effect.EffectType == Enums.eEffectType.GrantPower);
        midsPower.HasPowerOverrideEffect = effects.Any(effect => effect.EffectType == Enums.eEffectType.PowerRedirect);
        applyResult.EffectsReplaced += effects.Count;

        if (midsPower.NeverAutoUpdateRequirements)
        {
            applyResult.RequirementsSkippedNeverAutoUpdateRequirements++;
            applyResult.AddLimited(applyResult.RequirementSkips,
                $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: NeverAutoUpdateRequirements");
        }
        else if (string.IsNullOrWhiteSpace(omniPower.Requires))
        {
            midsPower.AdvancedRequirements = new AdvancedConditionSet();
            midsPower.Requires = midsPower.AdvancedRequirements.ToLegacyRequirement();
            applyResult.RequirementsUpdated++;
        }
        else if (OmniExpressionConverter.TryConvertPowerRequirement(CanonicalizeOmniFullName(omniPower.Requires), out var requirements))
        {
            midsPower.AdvancedRequirements = requirements;
            midsPower.Requires = requirements.ToLegacyRequirement();
            applyResult.RequirementsUpdated++;
        }
        else
        {
            applyResult.RequirementsSkippedUnsupported++;
            applyResult.AddLimited(applyResult.RequirementSkips,
                $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: {omniPower.Requires}");
        }

        midsPower.IsModified = true;
    }

    private static void RebuildPowersetMembershipFromPowerGraph(IDatabase database)
    {
        foreach (var powerset in database.Powersets ?? [])
        {
            if (powerset == null)
            {
                continue;
            }

            powerset.Power = [];
            powerset.Powers = [];
        }

        var powersetsByName = BuildCanonicalPowersetLookup(database);
        var powers = database.Power ?? [];
        for (var index = 0; index < powers.Length; index++)
        {
            var power = powers[index];
            if (power == null)
            {
                continue;
            }

            power.PowerIndex = index;
            var canonicalPowerset = CanonicalizeOmniFullName(power.FullSetName);
            if (string.IsNullOrWhiteSpace(canonicalPowerset) ||
                !powersetsByName.TryGetValue(canonicalPowerset, out var powerset))
            {
                power.PowerSetID = -1;
                power.PowerSetIndex = -1;
                continue;
            }

            var length = powerset.Powers.Length;
            power.PowerSetID = powerset.nID;
            power.PowerSetIndex = length;

            var nPower = powerset.Power;
            Array.Resize(ref nPower, length + 1);
            nPower[length] = index;
            powerset.Power = nPower;

            var linkedPowers = powerset.Powers;
            Array.Resize(ref linkedPowers, length + 1);
            linkedPowers[length] = power;
            powerset.Powers = linkedPowers;
        }
    }

    private static void TrackBoostSetBonusScopeCoverage(
        OmniImportReport report,
        IEnumerable<string> explicitScopedPowersetFullNames,
        IEnumerable<string> derivedScopedPowersetFullNames,
        IEnumerable<string> scopedPowerFullNames)
    {
        var explicitPowersetNames = explicitScopedPowersetFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var derivedPowersetNames = derivedScopedPowersetFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var powersetNames = explicitPowersetNames
            .Concat(derivedPowersetNames)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var powerNames = scopedPowerFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        report.BoostPowersetsInScope = powersetNames.Count(IsBoostFullName);
        report.SetBonusPowersetsInScope = powersetNames.Count(IsSetBonusFullName);
        report.BoostExplicitPowersetsInScope = explicitPowersetNames.Count(IsBoostFullName);
        report.BoostDerivedPowersetsInScope = derivedPowersetNames.Count(IsBoostFullName);
        report.SetBonusExplicitPowersetsInScope = explicitPowersetNames.Count(IsSetBonusFullName);
        report.SetBonusDerivedPowersetsInScope = derivedPowersetNames.Count(IsSetBonusFullName);
        report.BoostPowersInScope = powerNames.Count(IsBoostFullName);
        report.SetBonusPowersInScope = powerNames.Count(IsSetBonusFullName);
        report.AddLimited(report.ScopedBoostSetBonusCoverage,
            $"Boosts powersets/powers in scope: {report.BoostPowersetsInScope}/{report.BoostPowersInScope} (explicit={report.BoostExplicitPowersetsInScope}, derived={report.BoostDerivedPowersetsInScope}); Set_Bonus powersets/powers in scope: {report.SetBonusPowersetsInScope}/{report.SetBonusPowersInScope} (explicit={report.SetBonusExplicitPowersetsInScope}, derived={report.SetBonusDerivedPowersetsInScope})");
    }

    private static void TrackBoostSetBonusScopeCoverage(
        OmniApplyResult applyResult,
        IEnumerable<string> explicitScopedPowersetFullNames,
        IEnumerable<string> derivedScopedPowersetFullNames,
        IEnumerable<string> scopedPowerFullNames)
    {
        var explicitPowersetNames = explicitScopedPowersetFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var derivedPowersetNames = derivedScopedPowersetFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var powersetNames = explicitPowersetNames
            .Concat(derivedPowersetNames)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var powerNames = scopedPowerFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        applyResult.BoostPowersetsInScope = powersetNames.Count(IsBoostFullName);
        applyResult.SetBonusPowersetsInScope = powersetNames.Count(IsSetBonusFullName);
        applyResult.BoostExplicitPowersetsInScope = explicitPowersetNames.Count(IsBoostFullName);
        applyResult.BoostDerivedPowersetsInScope = derivedPowersetNames.Count(IsBoostFullName);
        applyResult.SetBonusExplicitPowersetsInScope = explicitPowersetNames.Count(IsSetBonusFullName);
        applyResult.SetBonusDerivedPowersetsInScope = derivedPowersetNames.Count(IsSetBonusFullName);
        applyResult.BoostPowersInScope = powerNames.Count(IsBoostFullName);
        applyResult.SetBonusPowersInScope = powerNames.Count(IsSetBonusFullName);
    }

    private static void TrackBoostSetBonusPowerApply(string canonicalFullName, string action, OmniApplyResult applyResult)
    {
        if (IsBoostFullName(canonicalFullName))
        {
            switch (action)
            {
                case "matched":
                    applyResult.BoostPowersMatched++;
                    break;
                case "created":
                    applyResult.BoostPowersCreated++;
                    break;
                case "updated":
                    applyResult.BoostPowersUpdated++;
                    break;
            }

            return;
        }

        if (!IsSetBonusFullName(canonicalFullName))
        {
            return;
        }

        switch (action)
        {
            case "matched":
                applyResult.SetBonusPowersMatched++;
                break;
            case "created":
                applyResult.SetBonusPowersCreated++;
                break;
            case "updated":
                applyResult.SetBonusPowersUpdated++;
                break;
        }
    }

    private static void TrackBoostSetBonusImportAudit(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        IEnumerable<string> explicitScopedPowersetFullNames,
        OmniApplyResult applyResult)
    {
        var scopedPowerNames = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName))
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        var derivedScopedPowersetFullNames = DeriveScopedPowersetFullNames(scopedPowerNames);
        TrackBoostSetBonusScopeCoverage(applyResult, explicitScopedPowersetFullNames, derivedScopedPowersetFullNames, scopedPowerNames);

        var dbPowerNames = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var normalizedDbPowerNames = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => NormalizeLookupKey(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Select(power => CanonicalizeOmniFullName(power.FullName)).Distinct(StringComparer.OrdinalIgnoreCase).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var scopedPowerName in scopedPowerNames.Where(name => IsBoostFullName(name) || IsSetBonusFullName(name)))
        {
            if (dbPowerNames.Contains(scopedPowerName))
            {
                continue;
            }

            var isBoost = IsBoostFullName(scopedPowerName);
            var normalizedKey = NormalizeLookupKey(scopedPowerName);
            var classification = "import omission";
            if (normalizedDbPowerNames.TryGetValue(normalizedKey, out var fallbackMatches) && fallbackMatches.Length > 0)
            {
                classification = fallbackMatches.Length == 1
                    ? $"alias/name mismatch -> {fallbackMatches[0]}"
                    : $"ambiguous alias/name mismatch -> {string.Join(", ", fallbackMatches)}";
            }

            if (isBoost)
            {
                applyResult.MissingBoostPowersAfterImport++;
            }
            else
            {
                applyResult.MissingSetBonusPowersAfterImport++;
            }

            applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                $"{scopedPowerName}: missing after import ({classification})");
        }

        applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
            $"Boosts powersets/powers in scope={applyResult.BoostPowersetsInScope}/{applyResult.BoostPowersInScope} (explicit={applyResult.BoostExplicitPowersetsInScope}, derived={applyResult.BoostDerivedPowersetsInScope}), matched/created/updated={applyResult.BoostPowersMatched}/{applyResult.BoostPowersCreated}/{applyResult.BoostPowersUpdated}, missing after import={applyResult.MissingBoostPowersAfterImport}");
        applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
            $"Set_Bonus powersets/powers in scope={applyResult.SetBonusPowersetsInScope}/{applyResult.SetBonusPowersInScope} (explicit={applyResult.SetBonusExplicitPowersetsInScope}, derived={applyResult.SetBonusDerivedPowersetsInScope}), matched/created/updated={applyResult.SetBonusPowersMatched}/{applyResult.SetBonusPowersCreated}/{applyResult.SetBonusPowersUpdated}, missing after import={applyResult.MissingSetBonusPowersAfterImport}");

        TrackStrictSetBonusHierarchyAudit(database, scopedPowerNames, explicitScopedPowersetFullNames, applyResult);
    }

    private static IEnumerable<string> DeriveScopedPowersetFullNames(IEnumerable<string> scopedPowerFullNames)
    {
        return scopedPowerFullNames
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(FullSetName)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsBoostFullName(string fullName)
    {
        return GroupNamePart(CanonicalizeOmniFullName(fullName)).Equals("Boosts", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSetBonusFullName(string fullName)
    {
        return GroupNamePart(CanonicalizeOmniFullName(fullName)).Equals("Set_Bonus", StringComparison.OrdinalIgnoreCase);
    }

    private static readonly string[] StrictSetBonusPowersetNames =
    [
        "Set_Bonus.Global_Bonus",
        "Set_Bonus.Set_Bonus",
        "Set_Bonus.PVP_Set_Bonus"
    ];

    private static bool IsStrictSetBonusPowerset(string fullSetName)
    {
        var canonical = CanonicalizeOmniFullName(fullSetName);
        if (!GroupNamePart(canonical).Equals("Set_Bonus", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return StrictSetBonusPowersetNames.Contains(canonical, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsStrictSetBonusPower(string fullName)
    {
        return IsSetBonusFullName(fullName) && IsStrictSetBonusPowerset(FullSetName(CanonicalizeOmniFullName(fullName)));
    }

    private static void PurgeStrictSetBonusFamilies(
        IDatabase database,
        IReadOnlyCollection<string> strictFamiliesInScope,
        OmniApplyResult applyResult)
    {
        if (strictFamiliesInScope.Count == 0)
        {
            return;
        }

        var strictSetBonusFamilies = strictFamiliesInScope
            .Select(CanonicalizeOmniFullName)
            .Where(IsStrictSetBonusPowerset)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var powers = database.Power ?? [];
        var keptPowers = powers
            .Where(power => power == null ||
                            (!strictSetBonusFamilies.Contains(CanonicalizeOmniFullName(power.FullSetName)) &&
                             !IsStrictSetBonusPower(power.FullName)))
            .ToArray();
        applyResult.StrictSetBonusPowersPurged = powers.Length - keptPowers.Length;
        foreach (var power in powers.Where(power =>
                     power != null &&
                     (strictSetBonusFamilies.Contains(CanonicalizeOmniFullName(power.FullSetName)) ||
                      IsStrictSetBonusPower(power.FullName))))
        {
            applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                $"{power.FullName}: purged before strict Set_Bonus rebuild ({CanonicalizeOmniFullName(power.FullSetName)})");
        }

        database.Power = keptPowers;

        var powersets = database.Powersets ?? [];
        var keptPowersets = powersets
            .Where(powerset => powerset == null ||
                               !strictSetBonusFamilies.Contains(CanonicalizeOmniFullName(powerset.FullName)))
            .ToArray();
        applyResult.StrictSetBonusPowersetsPurged = powersets.Length - keptPowersets.Length;
        foreach (var powerset in powersets.Where(powerset =>
                     powerset != null &&
                     strictSetBonusFamilies.Contains(CanonicalizeOmniFullName(powerset.FullName))))
        {
            applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                $"{powerset.FullName}: purged before strict Set_Bonus rebuild.");
        }

        database.Powersets = keptPowersets;
    }

    private static void PurgeStrictSetBonusScopedPowers(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        OmniApplyResult applyResult)
    {
        var strictScopedPowerNames = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName))
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Where(IsStrictSetBonusPower)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (strictScopedPowerNames.Count == 0)
        {
            return;
        }

        var powers = database.Power ?? [];
        var removed = powers
            .Where(power => power != null &&
                            strictScopedPowerNames.Contains(CanonicalizeOmniFullName(power.FullName)))
            .ToArray();
        if (removed.Length == 0)
        {
            return;
        }

        database.Power = powers
            .Where(power => power == null ||
                            !strictScopedPowerNames.Contains(CanonicalizeOmniFullName(power.FullName)))
            .ToArray();

        applyResult.StrictSetBonusPowersPurged += removed.Length;
        foreach (var power in removed)
        {
            applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                $"{power.FullName}: purged by strict Set_Bonus exact-full-name cleanup");
        }
    }

    private static void RemoveStrictSetBonusExistingPower(
        IDatabase database,
        List<IPower?> existingPowers,
        Dictionary<string, IPower?> midsPowers,
        string canonicalFullName,
        OmniApplyResult applyResult)
    {
        var normalizedFullName = NormalizeLookupKey(canonicalFullName);
        var powers = database.Power ?? [];
        var removed = powers
            .Where(power => power != null &&
                            (string.Equals(CanonicalizeOmniFullName(power.FullName), canonicalFullName, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(NormalizeLookupKey(power.FullName), normalizedFullName, StringComparison.OrdinalIgnoreCase)))
            .ToArray();
        if (removed.Length == 0)
        {
            midsPowers.Remove(canonicalFullName);
            return;
        }

        database.Power = powers
            .Where(power => power == null || !removed.Any(candidate => ReferenceEquals(candidate, power)))
            .ToArray();

        foreach (var removedPower in removed)
        {
            existingPowers.RemoveAll(candidate => ReferenceEquals(candidate, removedPower));
            applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                $"{removedPower.FullName}: removed immediately before strict Set_Bonus authoritative recreate");
        }

        midsPowers.Remove(canonicalFullName);
    }

    private static void TrackStrictSetBonusHierarchyAudit(
        IDatabase database,
        IReadOnlyCollection<string> scopedPowerNames,
        IEnumerable<string> explicitScopedPowersetFullNames,
        OmniApplyResult applyResult)
    {
        var expectedPowersets = explicitScopedPowersetFullNames
            .Select(CanonicalizeOmniFullName)
            .Where(IsStrictSetBonusPowerset)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (expectedPowersets.Length == 0)
        {
            return;
        }

        var powersetsByName = (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var powersByName = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var strictDbPowerNames = powersByName.Keys
            .Where(IsStrictSetBonusPower)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var expectedStrictPowerNames = scopedPowerNames
            .Where(IsStrictSetBonusPower)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        applyResult.StrictSetBonusGlobalBonusAfterImport = strictDbPowerNames.Count(name =>
            string.Equals(FullSetName(name), "Set_Bonus.Global_Bonus", StringComparison.OrdinalIgnoreCase));
        applyResult.StrictSetBonusSetBonusAfterImport = strictDbPowerNames.Count(name =>
            string.Equals(FullSetName(name), "Set_Bonus.Set_Bonus", StringComparison.OrdinalIgnoreCase));
        applyResult.StrictSetBonusPvpSetBonusAfterImport = strictDbPowerNames.Count(name =>
            string.Equals(FullSetName(name), "Set_Bonus.PVP_Set_Bonus", StringComparison.OrdinalIgnoreCase));
        applyResult.StrictSetBonusDbOnlyExtrasAfterImport = strictDbPowerNames.Except(expectedStrictPowerNames, StringComparer.OrdinalIgnoreCase).Count();
        applyResult.StrictSetBonusMissingFullNamesAfterImport = expectedStrictPowerNames.Except(strictDbPowerNames, StringComparer.OrdinalIgnoreCase).Count();
        applyResult.StrictSetBonusCrossSiblingMismatchesAfterImport = 0;

        foreach (var expectedPowerset in expectedPowersets)
        {
            applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                powersetsByName.ContainsKey(expectedPowerset)
                    ? $"{expectedPowerset}: strict set-bonus powerset present after import."
                    : $"{expectedPowerset}: strict set-bonus powerset missing after import.");
        }

        foreach (var scopedPowerName in scopedPowerNames.Where(IsStrictSetBonusPower).OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
        {
            if (!powersByName.TryGetValue(scopedPowerName, out var power))
            {
                continue;
            }

            var expectedFullSet = FullSetName(scopedPowerName);
            var actualFullSet = CanonicalizeOmniFullName(power.GetPowerSet()?.FullName ?? power.FullSetName);
            if (!string.Equals(expectedFullSet, actualFullSet, StringComparison.OrdinalIgnoreCase))
            {
                applyResult.StrictSetBonusCrossSiblingMismatchesAfterImport++;
                applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                    $"{scopedPowerName}: strict set-bonus hierarchy mismatch; expected powerset {expectedFullSet}, actual {actualFullSet}.");
            }
        }

        applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
            $"Strict Set_Bonus rebuild purge powers/powersets={applyResult.StrictSetBonusPowersPurged}/{applyResult.StrictSetBonusPowersetsPurged}");
        applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
            $"Strict Set_Bonus rebuilt counts Global/Set/PvP={applyResult.StrictSetBonusGlobalBonusAfterImport}/{applyResult.StrictSetBonusSetBonusAfterImport}/{applyResult.StrictSetBonusPvpSetBonusAfterImport}");
        applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
            $"Strict Set_Bonus cross-sibling mismatches/extras/missing={applyResult.StrictSetBonusCrossSiblingMismatchesAfterImport}/{applyResult.StrictSetBonusDbOnlyExtrasAfterImport}/{applyResult.StrictSetBonusMissingFullNamesAfterImport}");
    }

    private static void TrackPetPowerLinkAudit(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        OmniImportScope scope,
        OmniApplyResult applyResult)
    {
        var powers = database.Power ?? [];
        var powersets = database.Powersets ?? [];
        var powersByName = powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var powersetsByName = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var omniPower in scopedPowers.Where(power => IsPetScopedPower(power, scope)))
        {
            var canonicalFullName = CanonicalizeOmniFullName(omniPower.FullName);
            var canonicalPowerset = PowerPowersetFullName(omniPower);
            var root = GroupNamePart(canonicalFullName);

            if (!powersByName.TryGetValue(canonicalFullName, out var midsPower) || midsPower == null)
            {
                applyResult.PetPowersMissingAfterImport++;
                applyResult.AddLimited(applyResult.PetPowersMissingAfterImportDetails,
                    $"{omniPower.FullName}: powerset={omniPower.Powerset}, canonical={canonicalFullName}");
                continue;
            }

            var powersetExists = powersetsByName.TryGetValue(canonicalPowerset, out var owningPowerset);
            if (!powersetExists || owningPowerset == null)
            {
                applyResult.PetPowersWithMissingPowerset++;
                applyResult.AddLimited(applyResult.PetPowersWithMissingPowersetDetails,
                    FormatPetPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, null, false));
                continue;
            }

            if (midsPower.PowerSetID < 0)
            {
                applyResult.PetPowersWithInvalidPowersetId++;
                applyResult.AddLimited(applyResult.PetPowersWithInvalidPowersetIdDetails,
                    FormatPetPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, owningPowerset, false));
                continue;
            }

            var inPowersetArray = owningPowerset.Powers.Any(power =>
                power != null &&
                string.Equals(CanonicalizeOmniFullName(power.FullName), canonicalFullName, StringComparison.OrdinalIgnoreCase));
            if (!inPowersetArray)
            {
                applyResult.PetPowersNotInPowersetArray++;
                applyResult.AddLimited(applyResult.PetPowersNotInPowersetArrayDetails,
                    FormatPetPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, owningPowerset, false));
                continue;
            }

            applyResult.PetPowersLinkedAfterMatchIds++;
            IncrementCount(applyResult.PetPowersLinkedByRoot, root);
        }

        applyResult.AddLimited(applyResult.PetImportDetails,
            $"Pet link audit: linked={applyResult.PetPowersLinkedAfterMatchIds}, missing={applyResult.PetPowersMissingAfterImport}, missing powerset={applyResult.PetPowersWithMissingPowerset}, invalid PowerSetID={applyResult.PetPowersWithInvalidPowersetId}, not in powerset array={applyResult.PetPowersNotInPowersetArray}");
    }

    private static void TrackPetManifestLinkAudit(
        IDatabase database,
        IReadOnlyCollection<PetImportManifest> petManifests,
        OmniApplyResult applyResult)
    {
        var powers = database.Power ?? [];
        var powersets = database.Powersets ?? [];
        var powersByName = powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var powersetsByName = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var powersetManifest in petManifests.SelectMany(manifest => manifest.Powersets))
        {
            var expected = powersetManifest.ExpectedPowerNames
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
            var expectedSet = expected.ToHashSet(StringComparer.OrdinalIgnoreCase);
            powersetsByName.TryGetValue(powersetManifest.CanonicalFullName, out var owningPowerset);
            var linked = owningPowerset?.Powers
                .Where(power => power != null && expectedSet.Contains(CanonicalizeOmniFullName(power.FullName)))
                .Select(power => CanonicalizeOmniFullName(power.FullName))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count() ?? 0;

            applyResult.AddLimited(applyResult.PetImportManifestDetails,
                $"{powersetManifest.CanonicalFullName}: expected={expected.Length}, linked={linked}, owning powerset={(owningPowerset?.FullName ?? "<missing>")}");

            if (expected.Length > 0 && linked == 0)
            {
                applyResult.PetSourcePowersetsWithZeroLinkedPowers++;
                applyResult.PetSourceIntegrityFailures++;
                applyResult.AddLimited(applyResult.PetPowersetsWithZeroLinkedPowersDetails,
                    $"{powersetManifest.CanonicalFullName}: expected {expected.Length} powers from {powersetManifest.SourcePath}, but none linked after MatchIds.");
            }

            if (owningPowerset == null)
            {
                if (expected.Length > 0)
                {
                    applyResult.PetSourceIntegrityFailures++;
                    applyResult.AddLimited(applyResult.PetLinkIntegrityFailureDetails,
                        $"{powersetManifest.CanonicalFullName}: owning pet powerset missing after import; source={powersetManifest.SourcePath}");
                }

                continue;
            }

            foreach (var expectedPowerName in expected)
            {
                if (!powersByName.TryGetValue(expectedPowerName, out var midsPower) || midsPower == null)
                {
                    applyResult.PetSourceIntegrityFailures++;
                    applyResult.AddLimited(applyResult.PetLinkIntegrityFailureDetails,
                        $"{expectedPowerName}: expected by {powersetManifest.CanonicalFullName} but not found in database.Power after import; source={powersetManifest.SourcePath}");
                    continue;
                }

                if (midsPower.PowerSetID < 0)
                {
                    applyResult.PetSourceIntegrityFailures++;
                    applyResult.AddLimited(applyResult.PetLinkIntegrityFailureDetails,
                        $"{expectedPowerName}: PowerSetID={midsPower.PowerSetID}; final FullSetName={midsPower.FullSetName}; owning powerset={owningPowerset.FullName}");
                    continue;
                }

                if (midsPower.PowerSetID != owningPowerset.nID)
                {
                    applyResult.PetSourceIntegrityFailures++;
                    applyResult.AddLimited(applyResult.PetLinkIntegrityFailureDetails,
                        $"{expectedPowerName}: linked to PowerSetID={midsPower.PowerSetID}, expected {owningPowerset.nID} for {owningPowerset.FullName}; final FullSetName={midsPower.FullSetName}");
                    continue;
                }

                var inPowersetArray = owningPowerset.Powers.Any(power =>
                    power != null &&
                    string.Equals(CanonicalizeOmniFullName(power.FullName), expectedPowerName, StringComparison.OrdinalIgnoreCase));
                if (!inPowersetArray)
                {
                    applyResult.PetSourceIntegrityFailures++;
                    applyResult.AddLimited(applyResult.PetLinkIntegrityFailureDetails,
                        $"{expectedPowerName}: PowerSetID points at {owningPowerset.FullName}, but the power is absent from powerset.Powers.");
                }
            }
        }
    }

    private static void TrackPseudoPetAbsorptionAudit(IDatabase database, OmniApplyResult applyResult)
    {
        var powers = database.Power?
            .Where(power => power != null && power.Effects.Any(effect => effect.EffectType == Enums.eEffectType.EntCreate))
            .ToList() ?? [];
        var wroteRealPetSample = false;

        foreach (var power in powers)
        {
            var isFocused = power.FullName.Equals("Pool.Sorcery.Enflame", StringComparison.OrdinalIgnoreCase);
            foreach (var effect in power.Effects.Where(effect => effect.EffectType == Enums.eEffectType.EntCreate))
            {
                var entity = effect.nSummon >= 0 && effect.nSummon < database.Entities.Length
                    ? database.Entities[effect.nSummon]
                    : null;
                var entityType = entity?.EntityType.ToString() ?? "unresolved";
                var powersets = entity?.GetNPowerset().ToArray() ?? [];
                var linkedPowers = powersets
                    .Where(index => index >= 0 && index < database.Powersets.Length)
                    .Sum(index => database.Powersets[index].Powers.Length);
                var shouldAudit = isFocused || entity is { IsPseudoPet: true } || (!wroteRealPetSample && entity is { IsRealPet: true });
                if (!shouldAudit)
                {
                    continue;
                }

                PlannerEffectResolution resolved;
                try
                {
                    resolved = PlannerEffectResolver.ResolvePower(new Power(power), new PlannerEffectResolutionContext
                    {
                        AbsorbPetEffects = power.AbsorbSummonEffects,
                        IncludeTrace = true
                    });
                }
                catch (Exception ex)
                {
                    applyResult.PseudoPetAbsorptionAuditFailures++;
                    applyResult.AddLimited(applyResult.PseudoPetAbsorptionAuditDetails,
                        $"{power.FullName}: pseudo-pet audit failed during resolution for entity {(entity?.UID ?? "<unresolved>")}: {ex.GetType().Name}: {ex.Message}");
                    continue;
                }

                var absorbedEffects = resolved.ResolvedPower.Effects.Count(absorbed => absorbed.Absorbed_Effect);
                var recurrences = resolved.ResolvedPower.Effects
                    .Where(absorbed => absorbed.PseudoPetRecurrence is { IsValid: true })
                    .Select(absorbed => absorbed.PseudoPetRecurrence!)
                    .GroupBy(recurrence => string.Join("|",
                        recurrence.EntityName,
                        recurrence.PetPowerName,
                        recurrence.SourceUsageTime,
                        recurrence.SourceActivatePeriod,
                        recurrence.EntCreateDuration,
                        recurrence.PetTickInterval,
                        recurrence.SpawnCount,
                        recurrence.TicksPerSpawn,
                        recurrence.TotalExpectedTicks))
                    .Select(group => group.First())
                    .ToList();
                var excludedNonPlannerPseudoPet = ShouldExcludePseudoPetFromAudit(power, entity, linkedPowers, absorbedEffects);
                var skipReason = GetPseudoPetAuditSkipReason(power, effect, entity, linkedPowers, absorbedEffects, recurrences.Count, excludedNonPlannerPseudoPet);
                var failure = GetPseudoPetAuditFailure(power, effect, entity, linkedPowers, absorbedEffects, recurrences.Count, excludedNonPlannerPseudoPet || !string.IsNullOrWhiteSpace(skipReason));
                if (excludedNonPlannerPseudoPet || !string.IsNullOrWhiteSpace(skipReason))
                {
                    applyResult.PseudoPetAbsorptionAuditSkipped++;
                    applyResult.AddLimited(applyResult.PseudoPetAbsorptionAuditSkippedDetails,
                        $"{power.FullName}: {(excludedNonPlannerPseudoPet ? "nonplanner pseudo entity" : skipReason)}");
                }
                else if (!string.IsNullOrWhiteSpace(failure))
                {
                    applyResult.PseudoPetAbsorptionAuditFailures++;
                    applyResult.AddLimited(applyResult.PseudoPetAbsorptionAuditDetails,
                        $"{power.FullName}: UsageTime={power.UsageTime}, ActivatePeriod={power.ActivatePeriod}, EntCreate={effect.Summon}, EntCreateDuration={effect.Duration}, nSummon={effect.nSummon}, entityType={entityType}, entityPowersets={string.Join(", ", entity?.PowersetFullName ?? [])}, linkedPowers={linkedPowers}, AbsorbSummonEffects={power.AbsorbSummonEffects}, AbsorbSummonAttributes={power.AbsorbSummonAttributes}, absorbedEffects={absorbedEffects}{(recurrences.Count > 0 ? $", recurrence={string.Join("; ", recurrences.Select(recurrence => $"{recurrence.PetPowerName}: {recurrence.TicksPerSpawn} ticks/{recurrence.EntCreateDuration:0.###}s spawn, {recurrence.SpawnCount} spawns/{recurrence.SourceUsageTime:0.###}s, totalTicks={recurrence.TotalExpectedTicks}"))}" : string.Empty)}, failure={failure}");
                }

                if (entity is { IsRealPet: true })
                {
                    wroteRealPetSample = true;
                }
            }
        }
    }

    private static bool ShouldExcludePseudoPetFromAudit(
        IPower power,
        SummonedEntity? entity,
        int linkedPowers,
        int absorbedEffects)
    {
        if (entity is not { IsPseudoPet: true })
        {
            return false;
        }

        var normalizedText = NormalizeLookupKey(string.Join(" ",
            power.FullName,
            entity.UID,
            entity.DisplayName,
            string.Join(" ", entity.ActorTags)));
        return normalizedText.Contains("prestige", StringComparison.OrdinalIgnoreCase) ||
               normalizedText.Contains("minipet", StringComparison.OrdinalIgnoreCase) ||
               normalizedText.Contains("vanity", StringComparison.OrdinalIgnoreCase) ||
               normalizedText.Contains("visual", StringComparison.OrdinalIgnoreCase) ||
               (absorbedEffects == 0 && linkedPowers == 0);
    }

    private static string GetPseudoPetAuditFailure(
        IPower power,
        IEffect entCreate,
        SummonedEntity? entity,
        int linkedPowers,
        int absorbedEffects,
        int recurrenceCount,
        bool excludedNonPlannerPseudoPet)
    {
        if (entity == null)
        {
            return "unresolved entity";
        }

        if (!entity.IsPseudoPet || excludedNonPlannerPseudoPet)
        {
            return string.Empty;
        }

        if (!power.AbsorbSummonEffects)
        {
            return "absorption flags off";
        }

        if (entity.GetNPowerset().Count == 0)
        {
            return "missing pet powerset";
        }

        if (linkedPowers == 0)
        {
            return "zero linked pet powers";
        }

        if (absorbedEffects == 0)
        {
            return "no absorbed effects";
        }

        if (recurrenceCount > 0)
        {
            return string.Empty;
        }

        if (entCreate.Duration <= 0)
        {
            return "recurrence missing EntCreate duration";
        }

        var sourceUsageWindow = power.UsageTime > 0
            ? power.UsageTime
            : power.ActivatePeriod > 0
                ? power.ActivatePeriod
                : entCreate.Duration;
        if (sourceUsageWindow <= 0)
        {
            return "recurrence missing source usage time";
        }

        var sourceCadence = power.ActivatePeriod > 0 ? power.ActivatePeriod : entCreate.Duration;
        if (sourceCadence <= 0)
        {
            return "recurrence missing source activate period";
        }

        return "recurrence not modeled";
    }

    private static string GetPseudoPetAuditSkipReason(
        IPower power,
        IEffect entCreate,
        SummonedEntity? entity,
        int linkedPowers,
        int absorbedEffects,
        int recurrenceCount,
        bool excludedNonPlannerPseudoPet)
    {
        if (excludedNonPlannerPseudoPet || entity is not { IsPseudoPet: true })
        {
            return string.Empty;
        }

        if (recurrenceCount > 0 || power.AbsorbSummonEffects)
        {
            return string.Empty;
        }

        var normalizedText = NormalizeLookupKey(string.Join(" ",
            power.FullName,
            entCreate.Summon,
            entity.UID,
            entity.DisplayName));
        if (normalizedText.Contains("generate_target", StringComparison.OrdinalIgnoreCase) ||
            normalizedText.Contains("res_target", StringComparison.OrdinalIgnoreCase) ||
            (normalizedText.Contains("ionjudgement", StringComparison.OrdinalIgnoreCase) &&
             normalizedText.Contains("jump", StringComparison.OrdinalIgnoreCase)))
        {
            return "execution/chain wrapper without planner-visible absorbed payload";
        }

        if (normalizedText.Contains("vanguardmdc", StringComparison.OrdinalIgnoreCase))
        {
            return "dummy-target scaffold without planner-visible absorbed payload";
        }

        if (IsLorePetDeliveryPseudoPetWrapper(power.FullName))
        {
            return "lore-pet delivery wrapper already represented by the pet payload";
        }

        return string.Empty;
    }

    private static bool IsLorePetDeliveryPseudoPetWrapper(string fullName)
    {
        var canonicalFullName = CanonicalizeOmniFullName(fullName);
        return canonicalFullName is
            "Incarnate.Lore_Pet_Clockwork_Boss.Anti-Matter_Ray" or
            "Incarnate.Lore_Pet_Carnival_Support_Attack.Spit_Fire" or
            "Incarnate.Lore_Pet_Vanguard_LT.Willie_Pete_Round";
    }

    private static void RepairPetManifestPowerIdentities(
        IDatabase database,
        IReadOnlyCollection<PetImportManifest> petManifests,
        IDictionary<string, IPower> midsPowers,
        OmniApplyResult applyResult)
    {
        var powersetsByName = (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var powersetManifest in petManifests.SelectMany(manifest => manifest.Powersets))
        {
            if (!powersetsByName.TryGetValue(powersetManifest.CanonicalFullName, out var owningPowerset) ||
                owningPowerset == null)
            {
                continue;
            }

            foreach (var expectedPowerName in powersetManifest.ExpectedPowerNames.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                if (midsPowers.ContainsKey(expectedPowerName))
                {
                    continue;
                }

                var expectedLeaf = LastNamePart(expectedPowerName);
                var candidate = owningPowerset.Powers.FirstOrDefault(power =>
                    power != null &&
                    IsPetManifestIdentityCandidate(power, expectedLeaf));
                if (candidate == null)
                {
                    continue;
                }

                var priorFullName = candidate.FullName;
                midsPowers.Remove(CanonicalizeOmniFullName(priorFullName));
                midsPowers.Remove(priorFullName);
                ApplyCanonicalPowerName(candidate, expectedPowerName);
                candidate.PowerName = expectedLeaf;
                candidate.IsModified = true;
                midsPowers[expectedPowerName] = candidate;
                applyResult.AliasedPowerIdentityRepairs++;
                applyResult.AddLimited(applyResult.AliasedPowerIdentityRepairDetails,
                    $"{priorFullName} -> {expectedPowerName}: repaired pet manifest identity drift inside {owningPowerset.FullName}");
            }
        }
    }

    private static bool IsPetManifestIdentityCandidate(IPower power, string expectedLeaf)
    {
        var normalizedLeaf = NormalizeLookupKey(expectedLeaf);
        return NormalizeLookupKey(power.PowerName) == normalizedLeaf ||
               NormalizeLookupKey(LastNamePart(power.FullName)) == normalizedLeaf ||
               NormalizeLookupKey(power.DisplayName) == normalizedLeaf;
    }

    private static string FormatPetPowerAuditLine(
        OmniPowerDefinition omniPower,
        string canonicalFullName,
        string canonicalPowerset,
        IPower midsPower,
        IPowerset? owningPowerset,
        bool inPowersetArray)
    {
        return $"{omniPower.FullName}: omni powerset={omniPower.Powerset}, canonical={canonicalFullName}, canonical powerset={canonicalPowerset}, final FullName={midsPower.FullName}, final FullSetName={midsPower.FullSetName}, PowerSetID={midsPower.PowerSetID}, PowerSetIndex={midsPower.PowerSetIndex}, owning powerset={(owningPowerset?.FullName ?? "<missing>")}, in powerset array={inPowersetArray}";
    }

    private static bool PowersetIntegrityEquals(PowersetIntegrityInfo left, PowersetIntegrityInfo right)
    {
        return string.Equals(left.FullName, right.FullName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(left.GroupName, right.GroupName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(left.SetName, right.SetName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(left.DisplayName, right.DisplayName, StringComparison.OrdinalIgnoreCase) &&
               left.SetType == right.SetType &&
               string.Equals(left.ATClass, right.ATClass, StringComparison.OrdinalIgnoreCase);
    }

    private static void TrackDuplicateCompositePowerIdentities(IDatabase database, OmniApplyResult applyResult)
    {
        var duplicates = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CreateCompositePowerIdentityKey(database, power), StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .ToList();

        foreach (var duplicate in duplicates)
        {
            applyResult.DuplicateCompositePowerIdentities++;
            applyResult.AddLimited(applyResult.DuplicateCompositePowerIdentityDetails,
                $"{duplicate.Key}: {string.Join(", ", duplicate.Select(power => power.FullName).Distinct(StringComparer.OrdinalIgnoreCase).Take(8))}");
        }
    }

    private static string CreateCompositePowerIdentityKey(IDatabase database, IPower power)
    {
        var powerset = GetPowerSetSafe(database, power);
        var group = NormalizeName(powerset?.GroupName ?? GroupNamePart(power.FullName));
        var setInternal = NormalizeName(powerset?.SetName ?? SetNamePart(power.FullName));
        var setDisplay = NormalizeName(powerset?.DisplayName ?? SetNamePart(power.FullName));
        var powerInternal = NormalizeName(power.PowerName);
        var powerDisplay = NormalizeName(power.DisplayName);
        return $"{group}|{setInternal}|{setDisplay}|{powerInternal}|{powerDisplay}";
    }

    private static IPowerset? GetPowerSetSafe(IDatabase database, IPower power)
    {
        var powersets = database.Powersets ?? [];
        return power.PowerSetID >= 0 && power.PowerSetID < powersets.Length
            ? powersets[power.PowerSetID]
            : null;
    }

    private static void TrackStaffMasteryScope(IReadOnlyCollection<OmniPowerDefinition> scopedPowers, OmniApplyResult applyResult)
    {
        foreach (var target in StaffMasteryFullNames)
        {
            var power = scopedPowers.FirstOrDefault(p =>
                string.Equals(CanonicalizeOmniFullName(p.FullName), target, StringComparison.OrdinalIgnoreCase));
            if (power == null)
            {
                applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                    $"{target}: not present in scoped Omni powers after LoadScopedPowers.");
                continue;
            }

            applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                $"{target}: scoped from Omni powerset={power.Powerset}, name={power.Name}, display={power.DisplayName}, type={power.Type}, auto_issue={power.AutoIssue}, show_in_manage={power.ShowInManage}, requires='{power.Requires}', level={power.AvailableLevel}");
        }
    }

    private static void TrackStaffMasteryFinalState(IDatabase database, OmniApplyResult applyResult)
    {
        foreach (var target in StaffMasteryFullNames)
        {
            var power = (database.Power ?? []).FirstOrDefault(p =>
                p != null && string.Equals(CanonicalizeOmniFullName(p.FullName), target, StringComparison.OrdinalIgnoreCase));
            if (power == null)
            {
                applyResult.AddLimited(applyResult.StaffMasteryTraceDetails, $"{target}: not found in database.Power after import.");
                continue;
            }

            var powerset = GetPowerSetSafe(database, power);
            var inPowersetArray = powerset?.Powers.Any(p => p != null &&
                string.Equals(p.FullName, power.FullName, StringComparison.OrdinalIgnoreCase)) == true;
            var dbEditorFilterReason = power.HiddenPower
                ? "filtered by DB editor because HiddenPower=true"
                : power.PowerSetID < 0
                    ? "shown under orphan powers because PowerSetID<0"
                    : !inPowersetArray
                        ? "not listed under powerset because MatchIds did not place it in powerset.Powers"
                        : "visible in powerset list";

            applyResult.AddLimited(applyResult.StaffMasteryTraceDetails,
                $"{target}: final {DescribePowerIdentity(power)}, powerset={(powerset?.FullName ?? "<null>")}, inPowersetArray={inPowersetArray}, editor={dbEditorFilterReason}");
        }
    }

    private static bool IsStaffMasteryFullName(string fullName)
    {
        return StaffMasteryFullNames.Contains(CanonicalizeOmniFullName(fullName));
    }

    private static string DescribePowerIdentity(IPower power)
    {
        return $"FullName={power.FullName}, FullSetName={power.FullSetName}, Group={power.GroupName}, Set={power.SetName}, PowerName={power.PowerName}, Display={power.DisplayName}, Hidden={power.HiddenPower}, NeverAutoUpdate={power.NeverAutoUpdate}, PowerSetID={power.PowerSetID}, PowerSetIndex={power.PowerSetIndex}";
    }

    private static string DescribePowersetIdentity(IPowerset powerset)
    {
        var classSummary = powerset.SetType == Enums.ePowerSetType.Ancillary &&
                           NormalizeName(powerset.GroupName).Equals("epic", StringComparison.OrdinalIgnoreCase)
            ? $", Classes={FormatEffectivePowersetClasses(powerset)}"
            : string.Empty;
        return $"FullName={powerset.FullName}, Group={powerset.GroupName}, Set={powerset.SetName}, Display={powerset.DisplayName}, Type={powerset.SetType}, ATClass={powerset.ATClass}{classSummary}";
    }

    private static string DescribePowerIntegrity(PowerIntegrityInfo power)
    {
        return $"{power.FullName}: FullSetName={power.FullSetName}, Group={power.GroupName}, Set={power.SetName}, PowerName={power.PowerName}, Display={power.DisplayName}, PowerSetID={power.PowerSetID}, Hidden={power.HiddenPower}, NeverAutoUpdate={power.NeverAutoUpdate}, missingFullSet={power.MissingFullSetName}, unresolvedFullSet={power.UnresolvedFullSetName}";
    }

    private static string DescribePowersetIntegrity(PowersetIntegrityInfo powerset)
    {
        return $"FullName={powerset.FullName}, Group={powerset.GroupName}, Set={powerset.SetName}, Display={powerset.DisplayName}, Type={powerset.SetType}, ATClass={powerset.ATClass}";
    }

    private static void ApplyPowersetMetadata(
        IPowerset powerset,
        OmniPowersetDefinition source,
        Enums.ePowerSetType scopedSetType,
        bool preserveExistingValues)
    {
        powerset.FullName = source.FullName;
        powerset.DisplayName = string.IsNullOrWhiteSpace(source.DisplayName) ? LastNamePart(source.FullName) : source.DisplayName;
        powerset.SetName = string.IsNullOrWhiteSpace(source.Name) ? LastNamePart(source.FullName) : source.Name;
        powerset.Description = source.DisplayHelp ?? string.Empty;
        powerset.SubName = source.DisplayShortHelp ?? string.Empty;
        var archetypes = source.Archetypes
            .Where(archetype => !string.IsNullOrWhiteSpace(archetype))
            .Select(NormalizeClassName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var mappedSetType = scopedSetType != Enums.ePowerSetType.None
            ? scopedSetType
            : MapPowersetType(source.PowerCategory, source.FullName);
        if (!preserveExistingValues || mappedSetType != Enums.ePowerSetType.None)
        {
            powerset.SetType = mappedSetType;
        }

        powerset.GroupName = GroupNamePart(source.FullName);

        if (!preserveExistingValues)
        {
            powerset.ATClass = string.Empty;
            powerset.UIDTrunkSet = string.Empty;
            powerset.UIDLinkSecondary = string.Empty;
        }

        if (archetypes.Length == 1)
        {
            powerset.ATClass = archetypes[0];
        }
        else if (archetypes.Length > 1)
        {
            powerset.ATClass = string.Empty;
        }

        powerset.UIDMutexSets ??= [];
        powerset.nIDMutexSets ??= [];
        powerset.Power ??= [];
        powerset.Powers ??= [];
    }

    private static IPower CreatePower(IDatabase database, OmniPowerDefinition source, int staticIndex)
    {
        var power = new Power
        {
            IsNew = true,
            StaticIndex = staticIndex
        };
        ApplyPowerMetadata(power, source);

        var powers = database.Power ?? [];
        Array.Resize(ref powers, powers.Length + 1);
        powers[^1] = power;
        database.Power = powers;
        return power;
    }

    private static void ApplyPowerMetadata(IPower power, OmniPowerDefinition source)
    {
        var (ignoreEnh, typedIgnoreEnh, ignoreEnhAxes, _) = ResolveDisallowedEnhancementPolicy(source, source.StrengthsDisallowed);
        var (ignoreBuff, typedIgnoreBuff, ignoreBuffAxes, _) = ResolveDisallowedEnhancementPolicy(source, source.GlobalStrengthsDisallowed);
        power.FullName = source.FullName;
        power.GroupName = GroupNamePart(source.FullName);
        power.SetName = SetNamePart(source.FullName);
        power.PowerName = string.IsNullOrWhiteSpace(source.Name) ? LastNamePart(source.FullName) : source.Name;
        power.IconName = string.IsNullOrWhiteSpace(source.Icon) ? string.Empty : Path.GetFileName(source.Icon);
        power.DisplayName = string.IsNullOrWhiteSpace(source.DisplayName) ? power.PowerName : source.DisplayName;
        power.DescLong = source.DisplayHelp ?? string.Empty;
        power.DescShort = source.DisplayShortHelp ?? string.Empty;
        power.Available = source.AvailableLevel;
        power.Level = source.AvailableLevel;
        power.PowerType = MapPowerType(source.Type);
        power.Accuracy = source.Accuracy;
        power.AccuracyMult = 1f;
        power.AttackTypes = MapAttackTypes(source.AttackTypes);
        power.EntitiesAffected = MapEntities(source.TargetsAffected);
        power.EntitiesAutoHit = MapEntities(source.TargetsAutoHit);
        power.Target = MapEntity(source.TargetType);
        power.TargetSecondary = MapEntity(source.TargetTypeSecondary);
        power.TargetLoS = string.IsNullOrWhiteSpace(source.TargetVisibility) ||
                          source.TargetVisibility.Contains("LineOfSight", StringComparison.OrdinalIgnoreCase);
        power.ModesRequired = OmniModeMapper.ToFlags(source.ModesRequired, out _);
        power.ModesDisallowed = OmniModeMapper.ToFlags(source.ModesDisallowed, out _);
        power.Range = source.Range;
        power.RangeSecondary = source.RangeSecondary;
        power.EndCost = source.EnduranceCost;
        power.InterruptTime = source.InterruptTime;
        power.RootTime = source.RootTime;
        power.CastTime = source.ActivationTime;
        power.RechargeTime = source.RechargeTime;
        power.BaseRechargeTime = source.RechargeTime;
        power.ActivatePeriod = source.ActivatePeriod;
        power.EffectArea = MapEffectArea(source.EffectArea);
        power.Radius = source.Radius;
        power.Arc = source.Arc;
        power.MaxTargets = source.MaxTargetsHit;
        power.MaxBoosts = string.IsNullOrWhiteSpace(source.MaxBoosts) ? "0" : source.MaxBoosts;
        power.NumAllowed = source.NumberAllowed;
        power.NumCharges = ResolveNumberOfCharges(source);
        power.UsageTime = ResolveUsageTime(source);
        power.LifeTime = source.PowerLifetime;
        power.LifeTimeInGame = source.PowerLifetimeInGame;
        power.DoNotSave = source.DoNotSave;
        power.BoostsAllowed = source.BoostsAllowed.ToArray();
        power.RechargeGroups = source.RechargeGroups
            .Where(group => !string.IsNullOrWhiteSpace(group))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        power.IgnoreEnh = ignoreEnh;
        power.Ignore_Buff = ignoreBuff;
        power.IgnoreEnhancementAxes = ignoreEnhAxes;
        power.IgnoreBuffEnhancementAxes = ignoreBuffAxes;
        power.TypedEnhancementRestrictions = TypedEnhancementLegality.Normalize(
            typedIgnoreEnh.Concat(typedIgnoreBuff));
        power.GroupMembership = source.ExclusionGroups
            .Where(group => !string.IsNullOrWhiteSpace(group))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        power.NGroupMembership = new int[power.GroupMembership.Length];
        power.CastFlags = ResolveCastFlags(source);
        if (TryMapAiReport(source.NotifyAiWhen, out var aiReport))
        {
            power.AIReport = aiReport;
        }

        power.CastThroughHold = source.CastThrough.Any(IsHoldCastThroughToken);
        power.IgnoreStrength = source.IgnoreStrength;
        power.IsModified = true;
    }

    private static void RebuildScopedPowerEnhancementLegality(
        IDatabase database,
        IEnumerable<OmniPowerDefinition> scopedPowers,
        OmniApplyResult applyResult)
    {
        var enhancementClasses = database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>();
        if (enhancementClasses.Length == 0 || database.Power == null || database.Power.Length == 0)
        {
            return;
        }

        var enhancementClassLookup = BuildEnhancementClassLookup(database);
        var powersByFullName = database.Power
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var scopedPowerDefinitions = scopedPowers
            .Where(source => !string.IsNullOrWhiteSpace(source.FullName))
            .GroupBy(source => CanonicalizeOmniFullName(source.FullName), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Where(ShouldRebuildScopedPowerEnhancementLegality)
            .ToList();

        var enhancementClassesById = enhancementClasses
            .Where(item => item.ID > 0)
            .GroupBy(item => item.ID)
            .ToDictionary(group => group.Key, group => group.First());
        var unresolvedLabelsInScope = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var scopedPower in scopedPowerDefinitions)
        {
            var canonicalFullName = CanonicalizeOmniFullName(scopedPower.FullName);
            if (!powersByFullName.TryGetValue(canonicalFullName, out var midsPower))
            {
                applyResult.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild++;
                applyResult.AddLimited(applyResult.ScopedPowerEnhancementLegalityDetails,
                    $"{canonicalFullName}: scoped power was not present in the database after import, so enhancement legality could not be rebuilt.");
                continue;
            }

            var prior = midsPower.Enhancements ?? Array.Empty<int>();
            var resolved = new List<int>();
            var unresolved = new List<string>();
            var seen = new HashSet<int>();
            foreach (var boostAllowed in scopedPower.BoostsAllowed.Where(value => !string.IsNullOrWhiteSpace(value)))
            {
                if (!TryResolvePowerBoostAllowedEnhancementClasses(
                        boostAllowed,
                        enhancementClassLookup,
                        enhancementClasses,
                        out var mappedClassIds) ||
                    mappedClassIds.Length == 0)
                {
                    unresolved.Add(boostAllowed);
                    var unresolvedKey = NormalizeLookupKey(boostAllowed);
                    if (!string.IsNullOrWhiteSpace(unresolvedKey) && !unresolvedLabelsInScope.ContainsKey(unresolvedKey))
                    {
                        unresolvedLabelsInScope[unresolvedKey] = $"{boostAllowed} (sample: {canonicalFullName})";
                    }

                    continue;
                }

                foreach (var classId in mappedClassIds)
                {
                    if (seen.Add(classId))
                    {
                        resolved.Add(classId);
                    }
                }
            }

            var filteredResolved = TypedEnhancementLegality.FilterAllowedEnhancementClassIds(
                database,
                resolved,
                midsPower.TypedEnhancementRestrictions);
            var preservedExistingLegality = false;
            if (scopedPower.BoostsAllowed.Count > 0 && resolved.Count == 0 && prior.Length > 0)
            {
                midsPower.Enhancements = TypedEnhancementLegality.FilterAllowedEnhancementClassIds(
                    database,
                    prior,
                    midsPower.TypedEnhancementRestrictions);
                preservedExistingLegality = true;
                applyResult.ScopedPowerEnhancementLegalityPreserved++;
            }
            else
            {
                midsPower.Enhancements = filteredResolved;
            }

            midsPower.IsModified = true;
            applyResult.ScopedPowerEnhancementLegalityRebuilt++;

            if (!prior.SequenceEqual(midsPower.Enhancements))
            {
                applyResult.ScopedPowerEnhancementLegalityChanged++;
            }

            if (scopedPower.BoostsAllowed.Count > 0 && midsPower.Enhancements.Length == 0)
            {
                applyResult.ScopedPowerEnhancementLegalityEmptyAfterRebuild++;
            }

            if (unresolved.Count == 0 && !(scopedPower.BoostsAllowed.Count > 0 && midsPower.Enhancements.Length == 0))
            {
                continue;
            }

            if (unresolved.Count > 0)
            {
                applyResult.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild++;
            }

            var sourceSummary = scopedPower.BoostsAllowed.Count == 0
                ? "<empty>"
                : string.Join(", ", scopedPower.BoostsAllowed.Distinct(StringComparer.OrdinalIgnoreCase));
            var priorSummary = FormatEnhancementLegalitySummary(prior, enhancementClassesById);
            var currentSummary = FormatEnhancementLegalitySummary(midsPower.Enhancements, enhancementClassesById);
            var unresolvedSummary = unresolved.Count == 0 ? "<none>" : string.Join(", ", unresolved.Distinct(StringComparer.OrdinalIgnoreCase));
            applyResult.AddLimited(applyResult.ScopedPowerEnhancementLegalityDetails,
                preservedExistingLegality
                    ? $"{canonicalFullName}: source boosts_allowed=[{sourceSummary}], preserved existing legality=[{currentSummary}] because translation was incomplete (prior [{priorSummary}]), unresolved=[{unresolvedSummary}]"
                    : $"{canonicalFullName}: source boosts_allowed=[{sourceSummary}], rebuilt legality -> [{currentSummary}] (was [{priorSummary}]), unresolved=[{unresolvedSummary}]");
        }

        applyResult.ScopedPowerEnhancementLegalityUnresolvedLabelCount = unresolvedLabelsInScope.Count;
        foreach (var unresolvedLabel in unresolvedLabelsInScope.Values.OrderBy(value => value, StringComparer.OrdinalIgnoreCase))
        {
            applyResult.AddLimited(applyResult.ScopedPowerEnhancementLegalityUnknownLabelDetails, unresolvedLabel);
        }
    }

    private static void RepairImportedBoostAndSetBonusPowerEnhancementLegality(
        IDatabase database,
        IEnumerable<OmniPowerDefinition> scopedPowers,
        OmniApplyResult applyResult)
    {
        if (database?.Power == null || database.Power.Length == 0)
        {
            return;
        }

        var enhancementClasses = database.EnhancementClasses ?? Array.Empty<Enums.sEnhClass>();
        var enhancementClassLookup = enhancementClasses.Length == 0
            ? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
            : BuildEnhancementClassLookup(database);
        var enhancementClassesById = enhancementClasses
            .Where(item => item.ID > 0)
            .GroupBy(item => item.ID)
            .ToDictionary(group => group.Key, group => group.First());
        var categoryOnlyBoostPowerFullNames = (database.EnhancementImportMetadata?.CategoryOnlyBoostPowerFullNames ?? [])
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(CanonicalizeOmniFullName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var powersByFullName = database.Power
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var linkedBoostLegality = BuildBoostPowerLinkedEnhancementLegalityLookup(database, enhancementClasses);
        var scopedDefinitions = scopedPowers
            .Where(source => !string.IsNullOrWhiteSpace(source.FullName))
            .GroupBy(source => CanonicalizeOmniFullName(source.FullName), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Where(source => IsBoostFullName(source.FullName) || IsSetBonusFullName(source.FullName))
            .ToList();

        foreach (var scopedPower in scopedDefinitions)
        {
            var canonicalFullName = CanonicalizeOmniFullName(scopedPower.FullName);
            if (!powersByFullName.TryGetValue(canonicalFullName, out var midsPower))
            {
                continue;
            }

            var prior = midsPower.Enhancements ?? Array.Empty<int>();
            var priorSummary = FormatEnhancementLegalitySummary(prior, enhancementClassesById);

            if (IsSetBonusFullName(canonicalFullName))
            {
                applyResult.SetBonusPowerLegalityRepairInspected++;
                if (prior.Length == 0)
                {
                    applyResult.SetBonusPowerLegalityRepairAlreadyEmpty++;
                    continue;
                }

                midsPower.Enhancements = Array.Empty<int>();
                midsPower.IsModified = true;
                applyResult.SetBonusPowerLegalityRepairCleared++;
                applyResult.BoostSetBonusPowerLegalityRepairChanged++;
                applyResult.AddLimited(applyResult.BoostSetBonusPowerLegalityRepairDetails,
                    $"{canonicalFullName}: cleared stale Set_Bonus legality -> [<empty>] (was [{priorSummary}]).");
                continue;
            }

            applyResult.BoostPowerLegalityRepairInspected++;
            if (TryResolveBoostPowerEnhancementLegality(
                    canonicalFullName,
                    midsPower,
                    scopedPower,
                    categoryOnlyBoostPowerFullNames,
                    linkedBoostLegality,
                    enhancementClassLookup,
                    enhancementClasses,
                    out var resolved,
                    out var repairSource,
                    out var unresolved))
            {
                var filteredResolved = TypedEnhancementLegality.FilterAllowedEnhancementClassIds(
                    database,
                    resolved,
                    midsPower.TypedEnhancementRestrictions);
                if (prior.SequenceEqual(filteredResolved))
                {
                    continue;
                }

                midsPower.Enhancements = filteredResolved;
                midsPower.IsModified = true;
                applyResult.BoostPowerLegalityRepairRebuilt++;
                applyResult.BoostSetBonusPowerLegalityRepairChanged++;
                var currentSummary = FormatEnhancementLegalitySummary(filteredResolved, enhancementClassesById);
                var unresolvedSummary = unresolved.Length == 0
                    ? "<none>"
                    : string.Join(", ", unresolved.Distinct(StringComparer.OrdinalIgnoreCase));
                applyResult.AddLimited(applyResult.BoostSetBonusPowerLegalityRepairDetails,
                    $"{canonicalFullName}: repaired boost legality from {repairSource} -> [{currentSummary}] (was [{priorSummary}]), unresolved=[{unresolvedSummary}]");
                continue;
            }

            applyResult.BoostPowerLegalityRepairUnresolved++;
            var unresolvedDetail = unresolved.Length == 0
                ? "<none>"
                : string.Join(", ", unresolved.Distinct(StringComparer.OrdinalIgnoreCase));
            if (prior.Length > 0)
            {
                applyResult.BoostPowerLegalityRepairPreserved++;
                applyResult.AddLimited(applyResult.BoostSetBonusPowerLegalityRepairDetails,
                    $"{canonicalFullName}: preserved existing boost legality [{priorSummary}] because no canonical repair source was available, unresolved=[{unresolvedDetail}]");
                continue;
            }

            applyResult.AddLimited(applyResult.BoostSetBonusPowerLegalityRepairDetails,
                $"{canonicalFullName}: no canonical boost legality source was available; leaving legality empty, unresolved=[{unresolvedDetail}]");
        }
    }

    private static bool ShouldRebuildScopedPowerEnhancementLegality(OmniPowerDefinition source)
    {
        if (source == null || string.IsNullOrWhiteSpace(source.FullName))
        {
            return false;
        }

        if (source.Type.Equals("Enhancement", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (IsBoostFullName(source.FullName) || IsSetBonusFullName(source.FullName))
        {
            return false;
        }

        return TryGetPositiveMaxBoosts(source, out _);
    }

    private static bool TryGetPositiveMaxBoosts(OmniPowerDefinition source, out int maxBoosts)
    {
        maxBoosts = 0;
        if (source == null || string.IsNullOrWhiteSpace(source.MaxBoosts))
        {
            return false;
        }

        if (!int.TryParse(source.MaxBoosts, out maxBoosts))
        {
            return false;
        }

        return maxBoosts > 0;
    }

    private static bool TryResolvePowerBoostAllowedEnhancementClasses(
        string boostAllowed,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        Enums.sEnhClass[] enhancementClasses,
        out int[] classIds)
    {
        var resolved = new List<int>();
        var seen = new HashSet<int>();

        foreach (var candidate in EnumeratePowerBoostAllowedLookupCandidates(boostAllowed))
        {
            if (!enhancementClassLookup.TryGetValue(NormalizeLookupKey(candidate), out var classIndex) ||
                classIndex < 0 ||
                classIndex >= enhancementClasses.Length)
            {
                continue;
            }

            var classId = enhancementClasses[classIndex].ID;
            if (seen.Add(classId))
            {
                resolved.Add(classId);
            }
        }

        classIds = resolved.ToArray();
        return classIds.Length > 0;
    }

    private static Dictionary<string, int[]> BuildBoostPowerLinkedEnhancementLegalityLookup(
        IDatabase database,
        Enums.sEnhClass[] enhancementClasses)
    {
        var result = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
        foreach (var enhancement in (database.Enhancements ?? Array.Empty<IEnhancement>())
                     .Where(enhancement => enhancement != null && enhancement.TypeID != Enums.eType.None))
        {
            var boostPower = enhancement.GetPower();
            if (boostPower == null || !IsBoostFullName(boostPower.FullName))
            {
                continue;
            }

            var translated = TranslateEnhancementClassRefsToPowerEnhancementIds(enhancement.ClassID, enhancementClasses);
            if (translated.Length == 0)
            {
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(boostPower.FullName);
            if (!result.TryGetValue(canonicalFullName, out var values))
            {
                values = new List<int>();
                result[canonicalFullName] = values;
            }

            foreach (var classId in translated)
            {
                if (!values.Contains(classId))
                {
                    values.Add(classId);
                }
            }
        }

        return result.ToDictionary(pair => pair.Key, pair => pair.Value.ToArray(), StringComparer.OrdinalIgnoreCase);
    }

    private static int[] TranslateEnhancementClassRefsToPowerEnhancementIds(
        IReadOnlyCollection<int>? classRefs,
        Enums.sEnhClass[] enhancementClasses)
    {
        if (classRefs == null || classRefs.Count == 0 || enhancementClasses.Length == 0)
        {
            return Array.Empty<int>();
        }

        var ids = new List<int>();
        var seen = new HashSet<int>();
        var enhancementClassIds = enhancementClasses
            .Where(item => item.ID > 0)
            .Select(item => item.ID)
            .ToHashSet();

        foreach (var classRef in classRefs)
        {
            if (classRef >= 0 && classRef < enhancementClasses.Length)
            {
                var classId = enhancementClasses[classRef].ID;
                if (classId > 0 && seen.Add(classId))
                {
                    ids.Add(classId);
                }

                continue;
            }

            if (enhancementClassIds.Contains(classRef) && seen.Add(classRef))
            {
                ids.Add(classRef);
            }
        }

        return ids.ToArray();
    }

    private static bool TryResolveBoostPowerEnhancementLegality(
        string canonicalFullName,
        IPower midsPower,
        OmniPowerDefinition scopedPower,
        IReadOnlySet<string> categoryOnlyBoostPowerFullNames,
        IReadOnlyDictionary<string, int[]> linkedBoostLegality,
        IReadOnlyDictionary<string, int> enhancementClassLookup,
        Enums.sEnhClass[] enhancementClasses,
        out int[] resolved,
        out string repairSource,
        out string[] unresolved)
    {
        if (categoryOnlyBoostPowerFullNames.Contains(canonicalFullName))
        {
            resolved = Array.Empty<int>();
            repairSource = "linked category-only enhancement";
            unresolved = Array.Empty<string>();
            return true;
        }

        if (linkedBoostLegality.TryGetValue(canonicalFullName, out var linked) && linked.Length > 0)
        {
            resolved = linked.ToArray();
            repairSource = "linked enhancement";
            unresolved = Array.Empty<string>();
            return true;
        }

        var boostAllowed = scopedPower.BoostsAllowed.Count > 0
            ? scopedPower.BoostsAllowed
            : (midsPower.BoostsAllowed ?? Array.Empty<string>()).ToList();
        var filtered = boostAllowed
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Where(value => !IsNonStatBoostPowerCompatibilityTag(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var mapped = new List<int>();
        var seen = new HashSet<int>();
        var unresolvedLabels = new List<string>();
        foreach (var boostAllowedLabel in filtered)
        {
            if (!TryResolvePowerBoostAllowedEnhancementClasses(
                    boostAllowedLabel,
                    enhancementClassLookup,
                    enhancementClasses,
                    out var mappedClassIds) ||
                mappedClassIds.Length == 0)
            {
                unresolvedLabels.Add(boostAllowedLabel);
                continue;
            }

            foreach (var classId in mappedClassIds)
            {
                if (seen.Add(classId))
                {
                    mapped.Add(classId);
                }
            }
        }

        unresolved = unresolvedLabels.ToArray();
        if (mapped.Count > 0)
        {
            resolved = mapped.ToArray();
            repairSource = "fallback concrete boost tags";
            return true;
        }

        resolved = Array.Empty<int>();
        repairSource = "unresolved";
        return false;
    }

    private static bool IsNonStatBoostPowerCompatibilityTag(string boostAllowed)
    {
        var normalized = NormalizeLookupKey(boostAllowed);
        return normalized == "enhancemultiple" || normalized.EndsWith("boost", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatEnhancementLegalitySummary(
        IReadOnlyCollection<int> classIds,
        IReadOnlyDictionary<int, Enums.sEnhClass> enhancementClassesById)
    {
        if (classIds == null || classIds.Count == 0)
        {
            return "<empty>";
        }

        return string.Join(", ", classIds.Select(classId =>
        {
            if (enhancementClassesById.TryGetValue(classId, out var enhClass))
            {
                return $"{classId}:{enhClass.ClassID}";
            }

            return classId.ToString();
        }));
    }

    private static IEnumerable<string> EnumeratePowerBoostAllowedLookupCandidates(string boostAllowed)
    {
        if (string.IsNullOrWhiteSpace(boostAllowed))
        {
            yield break;
        }

        yield return boostAllowed;

        switch (NormalizeLookupKey(boostAllowed))
        {
            case "enhanceaccuracy":
                yield return "Accuracy_Boost";
                break;
            case "reduceinterrupttime":
                yield return "Interrupt_Boost";
                break;
            case "enhanceconfuse":
                yield return "Confuse_Boost";
                break;
            case "enhancedamage":
                yield return "Damage_Boost";
                break;
            case "enhancedefense":
                yield return "Buff_Defense_Boost";
                break;
            case "enhancedefensedebuff":
                yield return "Debuff_Defense_Boost";
                break;
            case "enhanceendurancemodification":
                yield return "Recovery_Boost";
                break;
            case "reduceendurancecost":
                yield return "EnduranceDiscount_Boost";
                break;
            case "enhancefear":
                yield return "Fear_Boost";
                break;
            case "enhanceflyingspeed":
                yield return "SpeedFlying_Boost";
                break;
            case "enhanceheal":
                yield return "Heal_Boost";
                break;
            case "enhancehold":
                yield return "Hold_Boost";
                break;
            case "enhanceimmobilization":
                yield return "Immobilized_Boost";
                break;
            case "enhanceintangibility":
                yield return "Intangible_Boost";
                break;
            case "enhancejump":
                yield return "Jump_Boost";
                break;
            case "enhanceknockback":
                yield return "Knockback_Boost";
                break;
            case "enhancerange":
                yield return "Range_Boost";
                break;
            case "enhancerechargespeed":
                yield return "Recharge_Boost";
                break;
            case "enhancedamageresistance":
                yield return "Res_Damage_Boost";
                break;
            case "enhancerunningspeed":
                yield return "SpeedRunning_Boost";
                break;
            case "enhancesleep":
                yield return "Sleep_Boost";
                break;
            case "enhanceslowmovement":
                yield return "Slow_Boost";
                break;
            case "enhancedisorient":
                yield return "Stunned_Boost";
                break;
            case "enhancethreatduration":
                yield return "Taunt_Boost";
                break;
            case "enhancetohitbuffs":
                yield return "Buff_ToHit_Boost";
                break;
            case "enhancetohitdebuffs":
                yield return "Debuff_ToHit_Boost";
                break;
            case "incarnatealphacapable":
                yield return "Incarnate_Alpha_Boost";
                break;
            case "incarnatedestinycapable":
                yield return "Incarnate_Destiny_Boost";
                break;
            case "incarnatehybridcapable":
                yield return "Incarnate_Hybrid_Boost";
                break;
            case "incarnateinterfacecapable":
                yield return "Incarnate_Interface_Boost";
                break;
            case "incarnatejudgementcapable":
                yield return "Incarnate_Judgement_Boost";
                break;
            case "incarnatelorecapable":
                yield return "Incarnate_Lore_Boost";
                break;
        }
    }

    private static int ResolveUsageTime(OmniPowerDefinition source)
    {
        return HasJsonValue(source.MaxToggleOnTimeValue)
            ? source.MaxToggleOnTime
            : source.UsageTime;
    }

    private static Enums.eCastFlags ResolveCastFlags(OmniPowerDefinition source)
    {
        var castFlags = Enums.eCastFlags.None;
        if (source.CasterNearGround)
        {
            castFlags |= Enums.eCastFlags.NearGround;
        }

        if (source.TargetNearGround)
        {
            castFlags |= Enums.eCastFlags.TargetNearGround;
        }

        if (CanCastAfterDeath(source.CastWhenDead))
        {
            castFlags |= Enums.eCastFlags.CastableAfterDeath;
        }

        return castFlags;
    }

    private static bool CanCastAfterDeath(string castWhenDead)
    {
        var normalized = OmniModeMapper.Normalize(castWhenDead);
        return normalized.Equals("DeadOnly", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("DeadOrAlive", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("true", StringComparison.OrdinalIgnoreCase) ||
               normalized.Equals("ktrue", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsLossyCastWhenDeadMapping(string castWhenDead)
    {
        return OmniModeMapper.Normalize(castWhenDead).Equals("DeadOrAlive", StringComparison.OrdinalIgnoreCase);
    }

    private static int ResolveNumberOfCharges(OmniPowerDefinition source)
    {
        if (source.NumberOfCharges > 0 || !HasJsonValue(source.MaxNumberOfChargesValue))
        {
            return source.NumberOfCharges;
        }

        return source.MaxNumberOfCharges;
    }

    private static bool TryMapAiReport(string value, out Enums.eNotify notify)
    {
        if (!string.IsNullOrWhiteSpace(value) &&
            Enum.TryParse(value.Trim(), ignoreCase: true, out notify))
        {
            return true;
        }

        notify = Enums.eNotify.Always;
        return false;
    }

    private static bool IsHoldCastThroughToken(string value)
    {
        return value.Equals("Hold", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("Held", StringComparison.OrdinalIgnoreCase) ||
               value.Equals("kHeld", StringComparison.OrdinalIgnoreCase) ||
               value.Contains("Hold", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasJsonValue(JToken? token)
    {
        return token is { Type: not JTokenType.Null };
    }

    private static void ApplyCanonicalPowerName(IPower power, string fullName)
    {
        power.FullName = fullName;
        power.GroupName = GroupNamePart(fullName);
        power.SetName = SetNamePart(fullName);
        power.PowerName = LastNamePart(fullName);
    }

    private static void ApplyCanonicalPowersetName(IPowerset powerset, string fullName)
    {
        powerset.FullName = fullName;
        powerset.GroupName = GroupNamePart(fullName);
        powerset.SetName = SetNamePart(fullName);
    }

    private static void ApplyPowerClassification(IPower power, OmniPowerClassification classification)
    {
        power.PowerType = classification.PowerType;
        power.ClickBuff = classification.ClickBuff;
        power.AlwaysToggle = classification.AlwaysToggle;
        power.HiddenPower = classification.HiddenPower;
        power.IncludeFlag = classification.IncludeFlag;
        power.InherentType = classification.InherentType;
        if (power.IsNew)
        {
            power.ShowInSpecialPowerPicker = SpecialPowerCatalog.ShouldBackfillSpecialPowerPicker(power);
        }
    }

    private static void TrackApplyClassification(
        string fullName,
        OmniPowerClassification classification,
        bool priorClickBuff,
        OmniApplyResult applyResult)
    {
        if (classification.HiddenPower)
        {
            applyResult.HiddenSupportPowers++;
            applyResult.AddLimited(applyResult.HiddenSupportPowerDetails, classification.Summary(fullName));
        }
        else if (classification.Reasons.Any(r => r.Contains("visible gated", StringComparison.OrdinalIgnoreCase)))
        {
            applyResult.VisibleGatedPowers++;
            applyResult.AddLimited(applyResult.VisibleGatedPowerDetails, classification.Summary(fullName));
        }

        if (classification.RedirectKind == OmniRedirectKind.ExecutionVariant)
        {
            applyResult.RedirectExecutionVariants++;
            applyResult.AddLimited(applyResult.RedirectExecutionVariantDetails, classification.Summary(fullName));
        }

        if (priorClickBuff != classification.ClickBuff)
        {
            applyResult.ClickBuffClassificationChanges++;
            applyResult.AddLimited(applyResult.ClickBuffClassificationChangeDetails,
                $"{fullName}: ClickBuff {priorClickBuff} -> {classification.ClickBuff} ({string.Join("; ", classification.Reasons)})");
        }

        if (classification.ClassificationConfidence < 0.65f || classification.RedirectKind == OmniRedirectKind.Unknown)
        {
            applyResult.ManualClassificationReviews++;
            applyResult.AddLimited(applyResult.ManualClassificationReviewDetails, classification.Summary(fullName));
        }
    }

    private static void TrackApplyModes(
        string fullName,
        OmniPowerDefinition power,
        OmniPowerClassification classification,
        OmniApplyResult applyResult)
    {
        var requiredFlags = OmniModeMapper.ToFlags(power.ModesRequired, out var unknownRequired);
        var disallowedFlags = OmniModeMapper.ToFlags(power.ModesDisallowed, out var unknownDisallowed);
        if (requiredFlags != Enums.eModeFlags.None || disallowedFlags != Enums.eModeFlags.None)
        {
            applyResult.ModeFlagsAssigned++;
            applyResult.AddLimited(applyResult.ModeFlagDetails,
                $"{fullName}: required={requiredFlags}, disallowed={disallowedFlags}");
        }

        foreach (var mode in unknownRequired.Concat(unknownDisallowed).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            applyResult.UnknownModesPreserved++;
            applyResult.AddLimited(applyResult.UnknownModeDetails, $"{fullName}: {mode}");
        }

        if (classification.HiddenPower && power.ModesRequired.Count > 0)
        {
            applyResult.FormGatedPowersHidden++;
            applyResult.AddLimited(applyResult.FormGatedPowerDetails, classification.Summary(fullName));
        }

        if (classification.InherentType == Enums.eGridType.Class && classification.IncludeFlag && !classification.HiddenPower)
        {
            applyResult.ClassInherentsImported++;
            applyResult.AddLimited(applyResult.ClassInherentDetails, fullName);
        }

        if (IsSupportHeavyRebuildGroup(fullName))
        {
            applyResult.SupportHeavyPowersRecreated++;
            TrackPetPowerApply(fullName, applyResult, "recreated",
                $"{fullName}: support-heavy pet power recreated");
        }
    }

    internal sealed class PetImportManifest
    {
        public string Root { get; init; } = string.Empty;
        public bool RootPresent { get; init; }
        public List<PetPowersetManifest> Powersets { get; } = [];
    }

    internal sealed class PetPowersetManifest
    {
        public string Root { get; init; } = string.Empty;
        public string SourcePath { get; init; } = string.Empty;
        public OmniPowersetDefinition Powerset { get; init; } = new();
        public string CanonicalFullName { get; init; } = string.Empty;
        public List<string> ExpectedPowerNames { get; init; } = [];
        public Dictionary<string, OmniPowerDefinition> PowerFiles { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        public List<string> MissingPowerFiles { get; init; } = [];
    }

    private static List<string> GetExpectedPetPowerNames(
        OmniImportScope scope,
        string root,
        string canonicalSet,
        OmniPowersetDefinition powerset,
        IReadOnlyCollection<string> powerFileNames)
    {
        var fullManifestExpectation = powerset.PowerNames
            .Concat(powerset.Powers.Select(power => power.Name))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Select(CanonicalizeOmniFullName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (fullManifestExpectation.Count == 0)
        {
            fullManifestExpectation = powerFileNames
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        // Redirects and other non-pet roots can be pulled into pet scope because a summoned
        // entity references only a subset of their powers. In those cases, validate only the
        // retained subset instead of the entire source powerset.
        var isDirectPetRoot = IsPetRoot(root) || NormalizeName(root) == "incarnate";
        if (isDirectPetRoot)
        {
            return fullManifestExpectation;
        }

        var retainedSubset = scope.RetainedPowerFullNames
            .Where(name => string.Equals(FullSetName(name), canonicalSet, StringComparison.OrdinalIgnoreCase))
            .Select(CanonicalizeOmniFullName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
        return retainedSubset.Count > 0 ? retainedSubset : fullManifestExpectation;
    }

    private List<PetImportManifest> BuildPetImportManifest(
        string exportRoot,
        OmniImportScope scope,
        OmniApplyResult applyResult,
        IReadOnlySet<string>? retainedPetPowersets = null,
        OmniExportManifest? manifest = null)
    {
        var powersRoot = Path.Combine(exportRoot, "powers");
        var manifestsByRoot = new Dictionary<string, PetImportManifest>(StringComparer.OrdinalIgnoreCase);
        foreach (var root in PetImportRoots)
        {
            var rootPath = Path.Combine(powersRoot, root);
            var rootManifest = new PetImportManifest
            {
                Root = CanonicalizeOmniFullName(root),
                RootPresent = Directory.Exists(rootPath)
            };
            manifestsByRoot[root] = rootManifest;

            if (!rootManifest.RootPresent)
            {
                applyResult.PetManifestRootsMissing++;
                applyResult.AddLimited(applyResult.PetImportManifestDetails,
                    $"{root}: root not present in export.");
                continue;
            }

            applyResult.PetManifestRootsPresent++;
        }

        if (!Directory.Exists(powersRoot))
        {
            return manifestsByRoot.Values.OrderBy(manifest => manifest.Root, StringComparer.OrdinalIgnoreCase).ToList();
        }

        var indexFiles = manifest?.PowersetIndexFiles?.Count > 0
            ? manifest.PowersetIndexFiles.ToList()
            : Directory.EnumerateFiles(powersRoot, "index.json", SearchOption.AllDirectories)
                .Where(file => !IsCategoryRootIndex(Path.GetRelativePath(powersRoot, file)))
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .ToList();
        var powerFilesByDirectory = (manifest?.PowerFiles?.Count > 0 ? manifest.PowerFiles : Directory.EnumerateFiles(powersRoot, "*.json", SearchOption.AllDirectories))
            .Where(file => !Path.GetFileName(file).Equals("index.json", StringComparison.OrdinalIgnoreCase))
            .GroupBy(file => Path.GetDirectoryName(file) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(file => file, StringComparer.OrdinalIgnoreCase).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        foreach (var indexFile in indexFiles)
        {
            var powerset = ReadJson<OmniPowersetDefinition>(indexFile);
            if (powerset == null || string.IsNullOrWhiteSpace(powerset.FullName))
            {
                continue;
            }

            var canonicalSet = CanonicalizeOmniFullName(powerset.FullName);
            if (!IsRetainedPetPowerset(scope, powerset.FullName, canonicalSet, retainedPetPowersets))
            {
                continue;
            }

            var relativePath = Path.GetRelativePath(powersRoot, indexFile);
            var root = RootPart(relativePath);
            if (!manifestsByRoot.TryGetValue(root, out var powersetRootManifest))
            {
                powersetRootManifest = new PetImportManifest
                {
                    Root = CanonicalizeOmniFullName(root),
                    RootPresent = true
                };
                manifestsByRoot[root] = powersetRootManifest;
            }

            var setDirectory = Path.GetDirectoryName(indexFile) ?? Path.Combine(powersRoot, root);
            var setPowerFiles = powerFilesByDirectory.TryGetValue(setDirectory, out var discoveredFiles)
                ? discoveredFiles
                : Array.Empty<string>();
            var powerFiles = setPowerFiles
                .Select(ReadJson<OmniPowerDefinition>)
                .OfType<OmniPowerDefinition>()
                .Where(power => !string.IsNullOrWhiteSpace(power.FullName))
                .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

            var expected = GetExpectedPetPowerNames(scope, root, canonicalSet, powerset, powerFiles.Keys);

            var missing = expected
                .Where(powerName => !powerFiles.ContainsKey(powerName))
                .OrderBy(powerName => powerName, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var powersetManifest = new PetPowersetManifest
            {
                Root = powersetRootManifest.Root,
                SourcePath = relativePath,
                Powerset = powerset,
                CanonicalFullName = canonicalSet,
                ExpectedPowerNames = expected,
                PowerFiles = powerFiles,
                MissingPowerFiles = missing
            };

            powersetRootManifest.Powersets.Add(powersetManifest);
            applyResult.PetManifestPowersets++;
            applyResult.PetManifestExpectedPowers += expected.Count;
            applyResult.PetManifestPowerFiles += powerFiles.Count;
            applyResult.PetManifestMissingPowerFiles += missing.Count;
            applyResult.AddLimited(applyResult.PetImportManifestDetails,
                $"{powersetManifest.SourcePath}: expected={expected.Count}, files={powerFiles.Count}, missing files={missing.Count}");
        }

        return manifestsByRoot.Values.OrderBy(manifest => manifest.Root, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private void ScanPetImportManifest(string exportRoot, OmniExportManifest manifest, OmniImportResult result)
    {
        var report = result.Report;
        var scope = result.Scope;
        var powersRoot = Path.Combine(exportRoot, "powers");
        foreach (var root in PetImportRoots)
        {
            var rootPath = Path.Combine(powersRoot, root);
            if (!Directory.Exists(rootPath))
            {
                report.PetManifestRootsMissing++;
                report.AddLimited(report.PetImportManifestDetails, $"{root}: root not present in export.");
                continue;
            }

            report.PetManifestRootsPresent++;
        }

        if (!Directory.Exists(powersRoot))
        {
            return;
        }

        var scanResult = new OmniApplyResult();
        var manifests = BuildPetImportManifest(exportRoot, scope, scanResult, manifest: manifest);
        result.CachedPetManifest = manifests;
        foreach (var rootManifest in manifests)
        {
            foreach (var powersetManifest in rootManifest.Powersets)
            {
                report.PetManifestPowersets++;
                report.PetManifestExpectedPowers += powersetManifest.ExpectedPowerNames.Count;
                report.PetManifestPowerFiles += powersetManifest.PowerFiles.Count;
                report.PetManifestMissingPowerFiles += powersetManifest.MissingPowerFiles.Count;
                report.AddLimited(report.PetImportManifestDetails,
                    $"{powersetManifest.SourcePath}: expected={powersetManifest.ExpectedPowerNames.Count}, files={powersetManifest.PowerFiles.Count}, missing files={powersetManifest.MissingPowerFiles.Count}");
            }
        }
    }

    private static void UpsertPetSourceOfTruth(
        IDatabase database,
        IReadOnlyCollection<PetImportManifest> petManifests,
        IReadOnlyDictionary<string, OmniPowerDefinition> scopedPowerLookup,
        IDictionary<string, IPower> midsPowers,
        IDictionary<string, OmniPowerClassification> classifications,
        OmniPowerClassifier classifier,
        OmniApplyResult applyResult,
        ref int nextStaticIndex)
    {
        var powersetsByFullName = (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var existingPowersForCompositeMatching = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .ToList();
        var consumedCompositeMatches = new HashSet<IPower>(ReferenceEqualityComparer.Instance);

        foreach (var powersetManifest in petManifests.SelectMany(manifest => manifest.Powersets))
        {
            var setFullName = powersetManifest.CanonicalFullName;
            if (!powersetsByFullName.TryGetValue(setFullName, out var midsPowerset) &&
                !TryFindPowersetIdentityMatch(database, powersetManifest.Powerset, Enums.ePowerSetType.Pet, out midsPowerset))
            {
                midsPowerset = CreatePowerset(powersetManifest.Powerset);
                ApplyCanonicalPowersetName(midsPowerset, setFullName);
                midsPowerset.SetType = Enums.ePowerSetType.Pet;
                var powersets = database.Powersets ?? [];
                Array.Resize(ref powersets, powersets.Length + 1);
                powersets[^1] = midsPowerset;
                database.Powersets = powersets;
                powersetsByFullName[setFullName] = midsPowerset;
                applyResult.PowersetsCreated++;
                applyResult.PetSourcePowersetsCreated++;
                applyResult.AddLimited(applyResult.CreatedPowersets, FormatAliasForReport(powersetManifest.Powerset.FullName, setFullName));
                applyResult.AddLimited(applyResult.PetSourceOfTruthUpsertDetails,
                    $"{setFullName}: created pet powerset from {powersetManifest.SourcePath}");
            }
            else
            {
                powersetsByFullName[setFullName] = midsPowerset;
                applyResult.PowersetsMatched++;
                applyResult.PetSourcePowersetsMatched++;
            }

            ApplyPowersetMetadata(midsPowerset, powersetManifest.Powerset, Enums.ePowerSetType.Pet, preserveExistingValues: true);
            ApplyCanonicalPowersetName(midsPowerset, setFullName);
            midsPowerset.SetType = Enums.ePowerSetType.Pet;
            midsPowerset.IsModified = true;
            applyResult.PowersetsUpdated++;
            applyResult.PetSourcePowersetsUpdated++;

            foreach (var missingPower in powersetManifest.MissingPowerFiles)
            {
                applyResult.PetSourceIntegrityFailures++;
                applyResult.AddLimited(applyResult.PetLinkIntegrityFailureDetails,
                    $"{setFullName}: expected power listed in index but no JSON file was found: {missingPower}");
            }

            foreach (var expectedPowerName in powersetManifest.ExpectedPowerNames)
            {
                if (!powersetManifest.PowerFiles.TryGetValue(expectedPowerName, out var omniPower))
                {
                    continue;
                }

                var midsFullName = CanonicalizeOmniFullName(omniPower.FullName);
                var localPowersetLookup = new Dictionary<string, OmniPowersetDefinition>(StringComparer.OrdinalIgnoreCase)
                {
                    [setFullName] = powersetManifest.Powerset
                };
                if (!midsPowers.TryGetValue(midsFullName, out var midsPower))
                {
                    var compositeCandidates = existingPowersForCompositeMatching
                        .Where(power => !consumedCompositeMatches.Contains(power))
                        .ToList();
                    if (!TryFindCompositePowerIdentityMatch(
                            compositeCandidates,
                            omniPower,
                            midsFullName,
                            localPowersetLookup,
                            out midsPower))
                    {
                        midsPower = CreatePower(database, omniPower, nextStaticIndex++);
                        ApplyCanonicalPowerName(midsPower, midsFullName);
                        midsPowers[midsFullName] = midsPower;
                        applyResult.PowersCreated++;
                        applyResult.PetSourcePowersCreated++;
                        TrackPetPowerApply(midsFullName, applyResult, "created",
                            $"{midsFullName}: created from pet source-of-truth manifest");
                        applyResult.AddLimited(applyResult.CreatedPowers, FormatAliasForReport(omniPower.FullName, midsFullName));
                    }
                    else
                    {
                        midsPowers[midsFullName] = midsPower;
                        consumedCompositeMatches.Add(midsPower);
                        applyResult.PowersMatched++;
                        applyResult.PetSourcePowersMatched++;
                        TrackPetPowerApply(midsFullName, applyResult, "matched",
                            $"{midsFullName}: matched from pet source-of-truth manifest");
                    }
                }
                else
                {
                    midsPowers[midsFullName] = midsPower;
                    consumedCompositeMatches.Add(midsPower);
                    applyResult.PowersMatched++;
                    applyResult.PetSourcePowersMatched++;
                    TrackPetPowerApply(midsFullName, applyResult, "matched",
                        $"{midsFullName}: matched from pet source-of-truth manifest");
                }

                ApplyPetPowerFromSource(
                    midsPower,
                    omniPower,
                    midsFullName,
                    scopedPowerLookup,
                    classifications,
                    classifier,
                    applyResult);
            }
        }
    }

    private static void ApplyPetPowerFromSource(
        IPower midsPower,
        OmniPowerDefinition omniPower,
        string midsFullName,
        IReadOnlyDictionary<string, OmniPowerDefinition> scopedPowerLookup,
        IDictionary<string, OmniPowerClassification> classifications,
        OmniPowerClassifier classifier,
        OmniApplyResult applyResult)
    {
        var priorClickBuff = midsPower.ClickBuff;
        var priorAttackTypes = midsPower.AttackTypes;
        var classification = classifier.Classify(omniPower, scopedPowerLookup);
        ApplyPowerMetadata(midsPower, omniPower);
        TrackApplyPowerFieldCoverage(omniPower, applyResult);
        ApplyCanonicalPowerName(midsPower, midsFullName);
        ApplyPowerClassification(midsPower, classification);
        TrackApplyClassification(midsFullName, classification, priorClickBuff, applyResult);
        TrackApplyAttackVectors(midsFullName, priorAttackTypes, midsPower.AttackTypes, omniPower, applyResult);
        TrackApplyModes(midsFullName, omniPower, classification, applyResult);

        var effects = OmniMidsMapper.FlattenEffects(omniPower, applyResult).Cast<IEffect>().ToList();
        var nextUniqueId = effects.Count == 0 ? 1 : effects.Max(effect => effect.UniqueID) + 1;
        foreach (var redirect in omniPower.Redirects.Where(redirect => !string.IsNullOrWhiteSpace(redirect.Name)))
        {
            var redirectEffect = OmniMidsMapper.CreatePowerRedirectEffect(omniPower.FullName, redirect);
            redirectEffect.UniqueID = nextUniqueId++;
            redirectEffect.ActiveConditionals = redirectEffect.AdvancedConditions.ToLegacyActiveConditionals();
            effects.Add(redirectEffect);
            applyResult.RedirectEffectsAdded++;
        }

        foreach (var effect in effects)
        {
            effect.PowerFullName = midsPower.FullName;
            effect.ActiveConditionals = effect.AdvancedConditions.ToLegacyActiveConditionals();
        }

        ApplyStrengthsDisallowedToEffects(effects, midsPower.IgnoreEnh, midsPower.TypedEnhancementRestrictions);
        midsPower.Effects = effects.ToArray();
        midsPower.HasGrantPowerEffect = effects.Any(effect => effect.EffectType == Enums.eEffectType.GrantPower);
        midsPower.HasPowerOverrideEffect = effects.Any(effect => effect.EffectType == Enums.eEffectType.PowerRedirect);
        applyResult.EffectsReplaced += effects.Count;

        if (string.IsNullOrWhiteSpace(omniPower.Requires))
        {
            midsPower.AdvancedRequirements = new AdvancedConditionSet();
            midsPower.Requires = midsPower.AdvancedRequirements.ToLegacyRequirement();
            applyResult.RequirementsUpdated++;
        }
        else if (OmniExpressionConverter.TryConvertPowerRequirement(CanonicalizeOmniFullName(omniPower.Requires), out var requirements))
        {
            midsPower.AdvancedRequirements = requirements;
            midsPower.Requires = requirements.ToLegacyRequirement();
            applyResult.RequirementsUpdated++;
        }
        else
        {
            applyResult.RequirementsSkippedUnsupported++;
            applyResult.AddLimited(applyResult.RequirementSkips,
                $"{FormatAliasForReport(omniPower.FullName, midsFullName)}: {omniPower.Requires}");
        }

        midsPower.IsModified = true;
        classifications[midsFullName] = classification;
        applyResult.PowersUpdated++;
        applyResult.PetSourcePowersUpdated++;
        TrackPetPowerApply(midsFullName, applyResult, "updated",
            $"{midsFullName}: updated from pet source-of-truth manifest");
        applyResult.AddLimited(applyResult.UpdatedPowers, FormatAliasForReport(omniPower.FullName, midsFullName));
        applyResult.AddLimited(applyResult.PetSourceOfTruthUpsertDetails,
            $"{midsFullName}: updated; effects={midsPower.Effects.Length}, hidden={midsPower.HiddenPower}, type={midsPower.PowerType}");
    }

    private static void TrackPetPowerApply(
        string fullName,
        OmniApplyResult applyResult,
        string action,
        string detail)
    {
        if (!IsPetPowerFullName(fullName))
        {
            return;
        }

        var root = GroupNamePart(fullName);
        switch (action)
        {
            case "created":
                applyResult.PetPowersCreated++;
                IncrementCount(applyResult.PetPowersCreatedByRoot, root);
                break;
            case "matched":
                applyResult.PetPowersMatched++;
                IncrementCount(applyResult.PetPowersMatchedByRoot, root);
                break;
            case "updated":
                applyResult.PetPowersUpdated++;
                IncrementCount(applyResult.PetPowersUpdatedByRoot, root);
                break;
            case "skipped":
                applyResult.PetPowersSkipped++;
                IncrementCount(applyResult.PetPowersSkippedByRoot, root);
                break;
            case "recreated":
                applyResult.PetPowersRecreated++;
                IncrementCount(applyResult.PetPowersRecreatedByRoot, root);
                break;
        }

        applyResult.AddLimited(applyResult.PetImportDetails, detail);
    }

    private static void TrackEpicPowersetApply(
        string fullName,
        OmniApplyResult applyResult,
        string action,
        string detail)
    {
        if (!IsEpicPowersetFullName(fullName))
        {
            return;
        }

        switch (action)
        {
            case "created":
                applyResult.EpicPowersetsCreated++;
                break;
            case "matched":
                applyResult.EpicPowersetsMatched++;
                break;
            case "updated":
                applyResult.EpicPowersetsUpdated++;
                break;
        }

        applyResult.AddLimited(applyResult.EpicImportDetails, detail);
    }

    private static void TrackEpicPowerApply(
        string fullName,
        OmniApplyResult applyResult,
        string action,
        string detail)
    {
        if (!IsEpicPowerFullName(fullName))
        {
            return;
        }

        switch (action)
        {
            case "created":
                applyResult.EpicPowersCreated++;
                break;
            case "matched":
                applyResult.EpicPowersMatched++;
                break;
            case "updated":
                applyResult.EpicPowersUpdated++;
                break;
            case "skipped":
                applyResult.EpicPowersSkipped++;
                break;
        }

        applyResult.AddLimited(applyResult.EpicImportDetails, detail);
    }

    private static Dictionary<string, OmniPowerDefinition> BuildScopedPowerLookup(IEnumerable<OmniPowerDefinition> powers)
    {
        var lookup = new Dictionary<string, OmniPowerDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var power in powers.Where(p => !string.IsNullOrWhiteSpace(p.FullName)))
        {
            AddLookup(power.FullName, power);
            AddLookup(CanonicalizeOmniFullName(power.FullName), power);
        }

        return lookup;

        void AddLookup(string key, OmniPowerDefinition power)
        {
            if (!string.IsNullOrWhiteSpace(key) && !lookup.ContainsKey(key))
            {
                lookup[key] = power;
            }
        }
    }

    private static List<IPower> GetCanonicalizableDatabasePowers(IDatabase database)
    {
        return (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Cast<IPower>()
            .ToList();
    }

    private static Dictionary<string, IPower> BuildCanonicalPowerLookup(IEnumerable<IPower> powers)
    {
        return powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
    }

    private static void TrackEpicPowersetScope(
        OmniPowersetDefinition powerset,
        string canonicalFullName,
        OmniApplyResult applyResult)
    {
        if (!IsEpicPowersetFullName(canonicalFullName))
        {
            return;
        }

        var classKeys = EpicClassKeys(canonicalFullName, powerset.Archetypes, string.Empty);
        applyResult.AddLimited(applyResult.EpicPowersetIdentityDetails,
            $"{FormatAliasForReport(powerset.FullName, canonicalFullName)}: display={powerset.DisplayName}, internal={powerset.Name}, category={powerset.PowerCategory}, archetypes={string.Join(", ", powerset.Archetypes)}, classes={FormatClassKeys(classKeys)}");
        if (classKeys.Count > 0)
        {
            applyResult.AddLimited(applyResult.EpicPowersetPrefixSuffixCandidates,
                $"{canonicalFullName}: class={FormatClassKeys(classKeys)}, family={EpicFamilyName(SetNamePart(canonicalFullName), classKeys)}");
        }
    }

    private static void TrackEpicImportLinkAudit(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        IReadOnlyCollection<OmniPowersetDefinition> scopedPowersets,
        OmniApplyResult applyResult)
    {
        var powers = database.Power ?? [];
        var powersets = database.Powersets ?? [];
        var powersByName = powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var powersetsByName = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        foreach (var scopedPowerset in scopedPowersets.Where(powerset => IsEpicPowersetFullName(powerset.FullName)))
        {
            var canonicalFullName = CanonicalizeOmniFullName(scopedPowerset.FullName);
            if (!powersetsByName.TryGetValue(canonicalFullName, out var midsPowerset) || midsPowerset == null)
            {
                applyResult.AddLimited(applyResult.EpicPowersetIdentityDetails,
                    $"{scopedPowerset.FullName}: scoped Epic powerset not found after import; expected {canonicalFullName}");
                continue;
            }

            var linkedCount = midsPowerset.Powers?.Length ?? 0;
            applyResult.AddLimited(applyResult.EpicPowersetIdentityDetails,
                $"{FormatAliasForReport(scopedPowerset.FullName, canonicalFullName)}: final {DescribePowersetIdentity(midsPowerset)}, linkedPowers={linkedCount}");
            if (linkedCount == 0)
            {
                applyResult.EpicPowersetsWithZeroLinkedPowers++;
                applyResult.AddLimited(applyResult.EpicPowersetsWithZeroLinkedPowersDetails,
                    $"{canonicalFullName}: final {DescribePowersetIdentity(midsPowerset)}");
            }
        }

        foreach (var collisionGroup in powersets
                     .Where(powerset => powerset != null &&
                                        powerset.SetType == Enums.ePowerSetType.Ancillary &&
                                        NormalizeName(powerset.GroupName).Equals("epic", StringComparison.OrdinalIgnoreCase) &&
                                        !string.IsNullOrWhiteSpace(powerset.DisplayName))
                     .GroupBy(powerset => NormalizeName(powerset.DisplayName), StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            applyResult.AddLimited(applyResult.EpicPowersetDisplayNameCollisions,
                $"{collisionGroup.First().DisplayName}: {string.Join(", ", collisionGroup.Select(powerset => $"{powerset.FullName}[{FormatEffectivePowersetClasses(powerset)}]"))}");
        }

        foreach (var omniPower in scopedPowers.Where(power => IsEpicPowerFullName(power.FullName)))
        {
            var canonicalFullName = CanonicalizeOmniFullName(omniPower.FullName);
            var canonicalPowerset = CanonicalizeOmniFullName(string.IsNullOrWhiteSpace(omniPower.Powerset)
                ? FullSetName(canonicalFullName)
                : omniPower.Powerset);

            if (!powersByName.TryGetValue(canonicalFullName, out var midsPower) || midsPower == null)
            {
                applyResult.EpicPowersMissingAfterImport++;
                applyResult.AddLimited(applyResult.EpicPowersMissingAfterImportDetails,
                    $"{omniPower.FullName}: powerset={omniPower.Powerset}, canonical={canonicalFullName}");
                continue;
            }

            var powersetExists = powersetsByName.TryGetValue(canonicalPowerset, out var owningPowerset);
            if (!powersetExists || owningPowerset == null)
            {
                applyResult.EpicPowersWithMissingPowerset++;
                applyResult.AddLimited(applyResult.EpicPowersWithMissingPowersetDetails,
                    FormatEpicPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, null, false));
                continue;
            }

            if (midsPower.PowerSetID < 0)
            {
                applyResult.EpicPowersWithInvalidPowersetId++;
                applyResult.AddLimited(applyResult.EpicPowersWithInvalidPowersetIdDetails,
                    FormatEpicPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, owningPowerset, false));
                continue;
            }

            var inPowersetArray = owningPowerset.Powers.Any(power =>
                power != null &&
                string.Equals(CanonicalizeOmniFullName(power.FullName), canonicalFullName, StringComparison.OrdinalIgnoreCase));
            if (!inPowersetArray)
            {
                applyResult.EpicPowersNotInPowersetArray++;
                applyResult.AddLimited(applyResult.EpicPowersNotInPowersetArrayDetails,
                    FormatEpicPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, owningPowerset, false));
                continue;
            }

            if (midsPower.HiddenPower)
            {
                applyResult.EpicPowersHiddenInDbEditor++;
                applyResult.AddLimited(applyResult.EpicPowersHiddenInDbEditorDetails,
                    FormatEpicPowerAuditLine(omniPower, canonicalFullName, canonicalPowerset, midsPower, owningPowerset, true));
            }

            applyResult.EpicPowersLinkedAfterMatchIds++;
        }

        applyResult.AddLimited(applyResult.EpicImportDetails,
            $"Epic link audit: linked={applyResult.EpicPowersLinkedAfterMatchIds}, zero powersets={applyResult.EpicPowersetsWithZeroLinkedPowers}, missing={applyResult.EpicPowersMissingAfterImport}, missing powerset={applyResult.EpicPowersWithMissingPowerset}, invalid PowerSetID={applyResult.EpicPowersWithInvalidPowersetId}, not in powerset array={applyResult.EpicPowersNotInPowersetArray}, hidden={applyResult.EpicPowersHiddenInDbEditor}");
    }

    private static string FormatEffectivePowersetClasses(IPowerset powerset)
    {
        var classKeys = EpicClassKeys(powerset.FullName, powerset.GetArchetypes(), powerset.ATClass);
        return classKeys.Count == 0 ? powerset.ATClass : FormatClassKeys(classKeys);
    }

    private static string FormatEpicPowerAuditLine(
        OmniPowerDefinition omniPower,
        string canonicalFullName,
        string canonicalPowerset,
        IPower midsPower,
        IPowerset? owningPowerset,
        bool inPowersetArray)
    {
        return $"{omniPower.FullName}: powerset={omniPower.Powerset}, canonical={canonicalFullName}, canonicalPowerset={canonicalPowerset}, final={DescribePowerIdentity(midsPower)}, owner={(owningPowerset == null ? "<missing>" : DescribePowersetIdentity(owningPowerset))}, inPowersetArray={inPowersetArray}";
    }

    private static void RebuildSupportHeavyGroups(IDatabase database, OmniApplyResult applyResult)
    {
        var powers = database.Power ?? [];
        var keptPowers = powers
            .Where(power => power == null ||
                            !IsSupportHeavyRebuildGroup(power.FullName) ||
                            IsPlannerControlInherent(power))
            .ToArray();
        applyResult.SupportPowersRemoved = powers.Length - keptPowers.Length;
        foreach (var power in keptPowers.Where(IsPlannerControlInherent))
        {
            applyResult.PlannerInherentsPreserved++;
            applyResult.AddLimited(applyResult.PlannerInherentDetails, power.FullName);
        }

        database.Power = keptPowers;

        var powersets = database.Powersets ?? [];
        var keptPowersets = powersets
            .Where(powerset => powerset == null || !IsSupportHeavyRebuildGroup(powerset.FullName))
            .ToArray();
        applyResult.SupportPowersetsRemoved = powersets.Length - keptPowersets.Length;
        database.Powersets = keptPowersets;
    }

    private static void PruneExcludedArchetypeContent(IDatabase database, OmniImportScope scope, OmniApplyResult applyResult)
    {
        var classes = database.Classes ?? [];
        var retainedClasses = classes
            .Where(archetype => archetype != null)
            .Cast<Archetype>()
            .Where(archetype => scope.IsRetainedClass(archetype.ClassName))
            .ToArray();
        applyResult.ExcludedArchetypesRemoved = classes.Count(archetype => archetype != null) - retainedClasses.Length;
        foreach (var archetype in classes.Where(archetype =>
                     archetype != null &&
                     !scope.IsRetainedClass(archetype.ClassName)))
        {
            applyResult.AddLimited(
                applyResult.ExcludedClassContentRemovalDetails,
                $"{archetype.ClassName}: removed archetype because it is not playable and not on the retained Omni whitelist.");
        }

        database.Classes = retainedClasses;

        var removedPowersetNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var powersets = database.Powersets ?? [];
        var keptPowersets = new List<IPowerset?>(powersets.Length);
        foreach (var powerset in powersets)
        {
            if (powerset == null)
            {
                keptPowersets.Add(null);
                continue;
            }

            var className = NormalizeClassName(powerset.ATClass);
            var canonicalFullName = CanonicalizeOmniFullName(powerset.FullName);
            var isRetainedPowerset = scope.IsRetainedPowerset(canonicalFullName) ||
                                     scope.IsRetainedPowerset(powerset.FullName) ||
                                     scope.IsRetainedPowerPowerset(canonicalFullName) ||
                                     scope.IsRetainedPowerPowerset(powerset.FullName);
            var root = GroupNamePart(!string.IsNullOrWhiteSpace(canonicalFullName)
                ? canonicalFullName
                : powerset.FullName);
            var isPetPowerset = powerset.SetType == Enums.ePowerSetType.Pet ||
                                scope.GetPowersetType(canonicalFullName) == Enums.ePowerSetType.Pet;
            var keepByIncludedRoot = IsAlwaysIncludedScopeRoot(root) ||
                                     (scope.IsIncludedPowerRoot(canonicalFullName) &&
                                      IsPlayableArchetypeScopeRoot(root) &&
                                      scope.IsPlayableArchetype(className));
            if (isRetainedPowerset ||
                (!isPetPowerset && keepByIncludedRoot))
            {
                keptPowersets.Add(powerset);
                continue;
            }

            applyResult.ExcludedClassPowersetsRemoved++;
            if (!string.IsNullOrWhiteSpace(canonicalFullName))
            {
                removedPowersetNames.Add(canonicalFullName);
            }

            applyResult.AddLimited(
                applyResult.ExcludedClassContentRemovalDetails,
                $"{powerset.FullName}: removed powerset because it is neither in a retained playable scope root nor explicitly retained by reachable summon content (class {className}).");
        }

        database.Powersets = keptPowersets.ToArray();

        var powers = database.Power ?? [];
        var keptPowers = new List<IPower?>(powers.Length);
        foreach (var power in powers)
        {
            if (power == null)
            {
                keptPowers.Add(null);
                continue;
            }

            var canonicalFullSetName = CanonicalizeOmniFullName(power.FullSetName);
            if (string.IsNullOrWhiteSpace(canonicalFullSetName) || !removedPowersetNames.Contains(canonicalFullSetName))
            {
                keptPowers.Add(power);
                continue;
            }

            applyResult.ExcludedClassPowersRemoved++;
            applyResult.AddLimited(
                applyResult.ExcludedClassContentRemovalDetails,
                $"{power.FullName}: removed power because owning powerset {canonicalFullSetName} was excluded by archetype retention policy.");
        }

        database.Power = keptPowers.ToArray();

        var entities = database.Entities ?? [];
        var keptEntities = new List<SummonedEntity>(entities.Length);
        foreach (var entity in entities.Where(entity => entity != null))
        {
            var className = ResolveRetainedEntityClassName(entity.ClassName);
            if (scope.IsRetainedEntity(entity.UID))
            {
                keptEntities.Add(entity);
                continue;
            }

            applyResult.ExcludedClassEntitiesRemoved++;
            applyResult.AddLimited(
                applyResult.ExcludedClassContentRemovalDetails,
                $"{entity.UID}: removed entity because it is not referenced by retained playable summon content (class {className}).");
        }

        database.Entities = keptEntities.ToArray();
    }

    private static void RemoveExcludedOmniContent(IDatabase database, OmniApplyResult applyResult)
    {
        var powers = database.Power ?? [];
        var keptPowers = powers
            .Where(power => power == null ||
                            (!OmniImportScope.IsExcludedPowerset(CanonicalizeOmniFullName(power.FullSetName)) &&
                             !OmniImportScope.IsExcludedPower(CanonicalizeOmniFullName(power.FullName))))
            .ToArray();
        applyResult.ExcludedPowersRemoved = powers.Length - keptPowers.Length;
        foreach (var power in powers.Where(power =>
                     power != null &&
                     (OmniImportScope.IsExcludedPowerset(CanonicalizeOmniFullName(power.FullSetName)) ||
                      OmniImportScope.IsExcludedPower(CanonicalizeOmniFullName(power.FullName)))))
        {
            var reason = OmniImportScope.IsExcludedPowerset(CanonicalizeOmniFullName(power.FullSetName))
                ? $"{power.FullSetName} is excluded from Homecoming import."
                : "the individual power is excluded from Homecoming import.";
            applyResult.AddLimited(applyResult.ExcludedContentRemovalDetails,
                $"{power.FullName}: removed because {reason}");
        }

        database.Power = keptPowers;

        var powersets = database.Powersets ?? [];
        var keptPowersets = powersets
            .Where(powerset => powerset == null ||
                               !OmniImportScope.IsExcludedPowerset(CanonicalizeOmniFullName(powerset.FullName)))
            .ToArray();
        applyResult.ExcludedPowersetsRemoved = powersets.Length - keptPowersets.Length;
        foreach (var powerset in powersets.Where(powerset =>
                     powerset != null &&
                     OmniImportScope.IsExcludedPowerset(CanonicalizeOmniFullName(powerset.FullName))))
        {
            applyResult.AddLimited(applyResult.ExcludedContentRemovalDetails,
                $"{powerset.FullName}: removed because the powerset is excluded from Homecoming import.");
        }

        database.Powersets = keptPowersets;
    }

    private static void RemoveScopedContentForFreshImport(
        IDatabase database,
        IReadOnlyCollection<OmniPowersetDefinition> scopedPowersets,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        OmniApplyResult applyResult)
    {
        var scopedPowersetNames = scopedPowersets
            .Where(powerset => !string.IsNullOrWhiteSpace(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .Concat(scopedPowers
                .Where(power => !string.IsNullOrWhiteSpace(power.FullName))
                .Select(PowerPowersetFullName))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var scopedPowerNames = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName))
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (scopedPowersetNames.Count == 0 && scopedPowerNames.Count == 0)
        {
            return;
        }

        var removedPowers = 0;
        var keptPowers = new List<IPower?>(database.Power?.Length ?? 0);
        foreach (var power in database.Power ?? [])
        {
            if (power == null)
            {
                keptPowers.Add(null);
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(power.FullName);
            var canonicalFullSetName = CanonicalizeOmniFullName(power.FullSetName);
            var remove = (!string.IsNullOrWhiteSpace(canonicalFullName) && scopedPowerNames.Contains(canonicalFullName)) ||
                         (!string.IsNullOrWhiteSpace(canonicalFullSetName) && scopedPowersetNames.Contains(canonicalFullSetName));
            if (!remove)
            {
                keptPowers.Add(power);
                continue;
            }

            removedPowers++;
        }

        database.Power = keptPowers.ToArray();

        var removedPowersets = 0;
        var keptPowersets = new List<IPowerset?>(database.Powersets?.Length ?? 0);
        foreach (var powerset in database.Powersets ?? [])
        {
            if (powerset == null)
            {
                keptPowersets.Add(null);
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(powerset.FullName);
            if (string.IsNullOrWhiteSpace(canonicalFullName) || !scopedPowersetNames.Contains(canonicalFullName))
            {
                keptPowersets.Add(powerset);
                continue;
            }

            removedPowersets++;
        }

        database.Powersets = keptPowersets.ToArray();

        applyResult.AddLimited(
            applyResult.ImportIntegrityAuditDetails,
            $"Fresh scoped import: removed {removedPowers:n0} scoped powers and {removedPowersets:n0} scoped powersets before apply.");
    }

    private static void RemoveStaleRedirectScopedContent(
        IDatabase database,
        IReadOnlyCollection<OmniPowersetDefinition> scopedPowersets,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        OmniApplyResult applyResult)
    {
        var retainedRedirectPowersets = scopedPowersets
            .Where(powerset => IsRedirectScopedFullName(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .Where(fullName => !string.IsNullOrWhiteSpace(fullName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var retainedRedirectPowers = scopedPowers
            .Where(power => IsRedirectScopedFullName(power.FullName))
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Where(fullName => !string.IsNullOrWhiteSpace(fullName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (retainedRedirectPowersets.Count == 0 && retainedRedirectPowers.Count == 0)
        {
            return;
        }

        var powersets = database.Powersets ?? [];
        var keptPowersets = new List<IPowerset?>(powersets.Length);
        var removedRedirectPowersets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var powerset in powersets)
        {
            if (powerset == null)
            {
                keptPowersets.Add(null);
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(powerset.FullName);
            if (!IsRedirectScopedFullName(canonicalFullName) || retainedRedirectPowersets.Contains(canonicalFullName))
            {
                keptPowersets.Add(powerset);
                continue;
            }

            removedRedirectPowersets.Add(canonicalFullName);
            applyResult.AddLimited(
                applyResult.ExcludedContentRemovalDetails,
                $"{powerset.FullName}: removed stale redirect powerset not present in current Omni export.");
        }

        database.Powersets = keptPowersets.ToArray();

        var powers = database.Power ?? [];
        var keptPowers = new List<IPower?>(powers.Length);
        foreach (var power in powers)
        {
            if (power == null)
            {
                keptPowers.Add(null);
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(power.FullName);
            var canonicalFullSetName = CanonicalizeOmniFullName(power.FullSetName);
            var removeStaleRedirectPower =
                (IsRedirectScopedFullName(canonicalFullName) && !retainedRedirectPowers.Contains(canonicalFullName)) ||
                (!string.IsNullOrWhiteSpace(canonicalFullSetName) &&
                 removedRedirectPowersets.Contains(canonicalFullSetName));
            if (!removeStaleRedirectPower)
            {
                keptPowers.Add(power);
                continue;
            }

            applyResult.AddLimited(
                applyResult.ExcludedContentRemovalDetails,
                $"{power.FullName}: removed stale redirect power not present in current Omni export.");
        }

        database.Power = keptPowers.ToArray();
    }

    private static bool IsSupportHeavyRebuildGroup(string fullName)
    {
        return NormalizeName(GroupNamePart(fullName)) is
            "pets" or
            "temporarypowers" or
            "incarnatepets" or
            "kheldianpets" or
            "mastermindpets" or
            "villainpets";
    }

    private static bool IsRedirectScopedFullName(string? fullName)
    {
        return !string.IsNullOrWhiteSpace(fullName) &&
               NormalizeName(GroupNamePart(fullName)).Equals("redirects", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPlannerControlInherent(IPower? power)
    {
        if (power == null || !string.Equals(GroupNamePart(power.FullName), "Inherent", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return TryGetPlannerModeFromPowerName(power, out var mode) && IsGlobalPlannerControlMode(mode);
    }

    private static readonly HashSet<PlannerMode> GlobalPlannerControlModes = [];

    private const string FastSnipePlannerPowerFullName = "Inherent.Inherent.Fast_Snipe";
    private const string FastSnipePlannerClassName = "Class_Blaster";
    private const string FastSnipePlannerIconName = "fast_snipe.png";

    private static void EnsurePlannerModeBindings(IDatabase database, OmniApplyResult applyResult)
    {
        var powers = database.Power ?? [];
        var auditScope = powers
            .Where(power => power is { IsNew: true } || power?.IsModified == true)
            .ToArray();
        var discoveredModes = DiscoverPlannerModes(auditScope.Length > 0 ? auditScope : powers, applyResult);
        if (discoveredModes.Count == 0)
        {
            return;
        }

        var powersByName = powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .GroupBy(power => power.FullName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);

        var nextStaticIndex = powers
            .Where(power => power != null)
            .Select(power => power.StaticIndex)
            .DefaultIfEmpty(-2)
            .Max() + 1;

        foreach (var mode in discoveredModes.OrderBy(mode => PlannerModeMapper.ToCanonicalName(mode), StringComparer.OrdinalIgnoreCase))
        {
            if (!IsGlobalPlannerControlMode(mode))
            {
                continue;
            }

            var powerName = PlannerModeMapper.ToPowerName(mode);
            var fullName = $"Inherent.Inherent.{powerName}";
            var createdPower = false;
            if (!powersByName.TryGetValue(fullName, out var power))
            {
                power = CreateSyntheticPlannerPower(mode, nextStaticIndex++);
                Array.Resize(ref powers, powers.Length + 1);
                powers[^1] = power;
                powersByName[power.FullName] = power;
                applyResult.SyntheticPlannerInherentsCreated++;
                createdPower = true;
            }

            var inherentType = GetPlannerModeInherentType(mode, power);
            power.IncludeFlag = true;
            power.HiddenPower = false;
            power.InherentType = inherentType;
            power.IsModified = true;
            applyResult.AddLimited(applyResult.PlannerModeDetails,
                $"{power.FullName}: planner control {PlannerModeMapper.ToCanonicalName(mode)}, InherentType={inherentType}");
            if (createdPower)
            {
                applyResult.AddLimited(applyResult.SyntheticPlannerInherentDetails,
                    $"{power.FullName}: InherentType={inherentType}");
            }

            AddPlannerModePayload(power, mode, applyResult);
        }

        EnsureFastSnipePlannerBinding(
            database,
            ref powers,
            powersByName,
            ref nextStaticIndex,
            applyResult);

        database.Power = powers;
    }

    private static void EnsureFastSnipePlannerBinding(
        IDatabase database,
        ref IPower?[] powers,
        IDictionary<string, IPower?> powersByName,
        ref int nextStaticIndex,
        OmniApplyResult applyResult)
    {
        var sourcePowers = FindFastSnipeSourcePowers(powers);
        var createdPower = false;
        if (!powersByName.TryGetValue(FastSnipePlannerPowerFullName, out var power) || power == null)
        {
            if (sourcePowers.Count == 0)
            {
                return;
            }

            power = CreateSyntheticPlannerPower(PlannerMode.FastSnipe, nextStaticIndex++);
            Array.Resize(ref powers, powers.Length + 1);
            powers[^1] = power;
            powersByName[power.FullName] = power;
            applyResult.SyntheticPlannerInherentsCreated++;
            createdPower = true;
        }

        power.HiddenPower = true;
        power.IncludeFlag = sourcePowers.Count > 0;
        power.InherentType = Enums.eGridType.Power;
        power.PowerType = Enums.ePowerType.Auto_;
        power.AlwaysToggle = sourcePowers.Count > 0;
        power.Available = 1;
        power.Level = 1;
        power.IconName = FastSnipePlannerIconName;
        power.Requires = BuildFastSnipePlannerRequirement(database, sourcePowers);
        power.AdvancedRequirements = AdvancedConditionSet.FromLegacyRequirement(power.Requires);
        power.IsModified = true;

        AddPlannerModePayload(power, PlannerMode.FastSnipe, applyResult);

        if (sourcePowers.Count == 0)
        {
            applyResult.AddLimited(
                applyResult.PlannerModeDetails,
                $"{power.FullName}: disabled FastSnipe auto-grant because no qualifying Blaster sniper powers were found.");
            return;
        }

        applyResult.AddLimited(
            applyResult.PlannerModeDetails,
            $"{power.FullName}: auto-granted FastSnipe for {FastSnipePlannerClassName} when any qualifying sniper power is chosen ({sourcePowers.Count} source(s)).");

        if (createdPower)
        {
            applyResult.AddLimited(
                applyResult.SyntheticPlannerInherentDetails,
                $"{power.FullName}: hidden auto-granted planner state for Blaster sniper powers.");
        }
    }

    private static Requirement BuildFastSnipePlannerRequirement(
        IDatabase database,
        IReadOnlyList<IPower> sourcePowers)
    {
        var classes = database.Classes ?? [];
        var blasterClassIndex = classes.TryFindIndex(cls =>
            string.Equals(cls?.ClassName, FastSnipePlannerClassName, StringComparison.OrdinalIgnoreCase));

        return new Requirement
        {
            ClassName = blasterClassIndex >= 0 ? [FastSnipePlannerClassName] : [],
            NClassName = blasterClassIndex >= 0 ? [blasterClassIndex] : [],
            PowerID = sourcePowers.Select(power => new[] { power.FullName, string.Empty }).ToArray(),
            NPowerID = sourcePowers.Select(power => new[] { power.PowerIndex, -1 }).ToArray()
        };
    }

    private static List<IPower> FindFastSnipeSourcePowers(IEnumerable<IPower?> powers)
    {
        return powers
            .Where(IsFastSnipeSourcePower)
            .Cast<IPower>()
            .GroupBy(power => power.FullName, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(power => power.FullName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsFastSnipeSourcePower(IPower? power)
    {
        if (power == null ||
            power.HiddenPower ||
            power.InherentType != Enums.eGridType.None ||
            string.IsNullOrWhiteSpace(power.FullName) ||
            !OmniModeMapper.IsSnipePlannerContext(power.FullName))
        {
            return false;
        }

        return IsBlasterPlannerSourcePower(power) && ReferencesPlannerMode(power, PlannerMode.FastSnipe);
    }

    private static bool IsBlasterPlannerSourcePower(IPower power)
    {
        if (power.FullName.StartsWith("Blaster_", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return string.Equals(
            power.GetPowerSet()?.ATClass,
            FastSnipePlannerClassName,
            StringComparison.OrdinalIgnoreCase);
    }

    private static bool ReferencesPlannerMode(IPower power, PlannerMode mode)
    {
        if (HasCanonicalPlannerModeCoverage(power, mode))
        {
            return true;
        }

        foreach (var effect in power.Effects ?? [])
        {
            if (effect != null &&
                TryMapSpecialCaseToPlannerMode(effect.SpecialCase, out var bridgedMode) &&
                bridgedMode == mode)
            {
                return true;
            }
        }

        return false;
    }

    private static HashSet<PlannerMode> DiscoverPlannerModes(IEnumerable<IPower?> powers, OmniApplyResult applyResult)
    {
        var discovered = new HashSet<PlannerMode>();
        foreach (var power in powers.Where(power => power != null))
        {
            if (TryGetPlannerModeFromPowerName(power!, out var powerNameMode))
            {
                AddDiscovered(powerNameMode, $"{power.FullName}: planner control power");
            }

            foreach (var effect in power.Effects ?? [])
            {
                if (effect == null)
                {
                    continue;
                }

                if (effect.EffectType is Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode &&
                    PlannerModeMapper.TryGetPlannerMode(effect.ModeName, out var effectMode))
                {
                    AddDiscovered(effectMode, $"{power.FullName}: {effect.EffectType} {effect.ModeName}");
                }

                if (TryMapSpecialCaseToPlannerMode(effect.SpecialCase, out var bridgedMode))
                {
                    if (!HasCanonicalPlannerModeCoverage(power!, bridgedMode))
                    {
                        applyResult.SpecialCaseCompatibilityBridges++;
                        applyResult.AddLimited(applyResult.SpecialCaseCompatibilityBridgeDetails,
                            $"{power.FullName}: {effect.SpecialCase} -> {PlannerModeMapper.ToCanonicalName(bridgedMode)}");
                    }

                    AddDiscovered(bridgedMode, $"{power.FullName}: legacy SpecialCase {effect.SpecialCase}");
                }

                foreach (var row in effect.AdvancedConditions?.Rows ?? [])
                {
                    if (row.Kind == AdvancedConditionKind.SourceMode &&
                        row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated &&
                        OmniModeMapper.TryGetPlannerMode(row.Subject, out var conditionMode))
                    {
                        AddDiscovered(conditionMode, $"{power.FullName}: condition {row.Subject}");
                    }
                }
            }
        }

        return discovered;

        void AddDiscovered(PlannerMode mode, string detail)
        {
            if (mode == PlannerMode.None)
            {
                return;
            }

            if (discovered.Add(mode))
            {
                applyResult.PlannerModesDiscovered++;
            }

            applyResult.AddLimited(applyResult.PlannerModeDetails, detail);
        }
    }

    private static bool TryGetPlannerModeFromPowerName(IPower power, out PlannerMode mode)
    {
        return PlannerModeMapper.TryGetPlannerMode(power.PowerName, out mode) ||
               PlannerModeMapper.TryGetPlannerMode(LastNamePart(power.FullName), out mode);
    }

    private static bool HasCanonicalPlannerModeCoverage(IPower power, PlannerMode mode)
    {
        if (mode == PlannerMode.None)
        {
            return false;
        }

        if (TryGetPlannerModeFromPowerName(power, out var powerNameMode) && powerNameMode == mode)
        {
            return true;
        }

        foreach (var effect in power.Effects ?? [])
        {
            if (effect == null)
            {
                continue;
            }

            if (effect.EffectType is Enums.eEffectType.SetMode or Enums.eEffectType.UnsetMode &&
                PlannerModeMapper.TryGetPlannerMode(effect.ModeName, out var effectMode) &&
                effectMode == mode)
            {
                return true;
            }

            foreach (var row in effect.AdvancedConditions?.Rows ?? [])
            {
                if (row.Kind == AdvancedConditionKind.SourceMode &&
                    row.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated &&
                    OmniModeMapper.TryGetPlannerMode(row.Subject, out var conditionMode) &&
                    conditionMode == mode)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsGlobalPlannerControlMode(PlannerMode mode)
    {
        return GlobalPlannerControlModes.Contains(mode);
    }

    private static Enums.eGridType GetPlannerModeInherentType(PlannerMode mode, IPower? power)
    {
        if (power?.InherentType == Enums.eGridType.Class)
        {
            return Enums.eGridType.Class;
        }

        return IsClassPlannerControlMode(mode)
            ? Enums.eGridType.Class
            : Enums.eGridType.Power;
    }

    private static bool IsClassPlannerControlMode(PlannerMode mode)
    {
        return false;
    }

    private static Power CreateSyntheticPlannerPower(PlannerMode mode, int staticIndex)
    {
        var powerName = PlannerModeMapper.ToPowerName(mode);
        var displayName = PlannerModeMapper.ToDisplayName(mode);
        return new Power
        {
            FullName = $"Inherent.Inherent.{powerName}",
            GroupName = "Inherent",
            SetName = "Inherent",
            PowerName = powerName,
            IconName = mode == PlannerMode.FastSnipe ? FastSnipePlannerIconName : string.Empty,
            DisplayName = displayName,
            DescShort = $"Planner toggle for {displayName}.",
            DescLong = $"Enables Mids planner effects that depend on {displayName}.",
            PowerType = Enums.ePowerType.Auto_,
            IncludeFlag = true,
            HiddenPower = false,
            InherentType = GetPlannerModeInherentType(mode, null),
            Available = 1,
            Level = 1,
            StaticIndex = staticIndex,
            IsNew = true,
            IsModified = true,
            Effects = []
        };
    }

    private static void AddPlannerModePayload(IPower power, PlannerMode mode, OmniApplyResult applyResult)
    {
        var modeName = PlannerModeMapper.ToCanonicalName(mode);
        if (string.IsNullOrWhiteSpace(modeName))
        {
            return;
        }

        if ((power.Effects ?? []).Any(effect =>
                effect is { EffectType: Enums.eEffectType.SetMode } &&
                PlannerModeMapper.TryGetPlannerMode(effect.ModeName, out var existingMode) &&
                existingMode == mode))
        {
            return;
        }

        var effects = power.Effects ?? [];
        var nextUniqueId = effects
            .Where(effect => effect != null)
            .Select(effect => effect.UniqueID)
            .DefaultIfEmpty(0)
            .Max() + 1;
        var modeEffect = new Effect
        {
            PowerFullName = power.FullName,
            UniqueID = nextUniqueId,
            EffectClass = Enums.eEffectClass.Primary,
            EffectType = Enums.eEffectType.SetMode,
            ToWho = Enums.eToWho.Self,
            BaseProbability = 1f,
            nMagnitude = 1,
            nDuration = 0,
            ModeName = modeName,
            ModeId = OmniModeMapper.TryFromModeName(modeName, out var modeId, out var modeFlag) ? modeId : -1,
            ModeFlag = modeFlag,
            AdvancedConditions = new AdvancedConditionSet()
        };

        Array.Resize(ref effects, effects.Length + 1);
        effects[^1] = modeEffect;
        power.Effects = effects;
        power.IsModified = true;
        applyResult.PlannerModePayloadsAdded++;
        applyResult.AddLimited(applyResult.PlannerModePayloadDetails,
            $"{power.FullName}: SetMode {modeName}, InherentType={power.InherentType}");
    }

    private static bool TryMapSpecialCaseToPlannerMode(Enums.eSpecialCase specialCase, out PlannerMode mode)
    {
        mode = specialCase switch
        {
            Enums.eSpecialCase.Domination => PlannerMode.Domination,
            Enums.eSpecialCase.Scourge => PlannerMode.Scourge,
            Enums.eSpecialCase.CriticalHit => PlannerMode.CriticalHit,
            Enums.eSpecialCase.Assassination => PlannerMode.Assassination,
            Enums.eSpecialCase.Containment => PlannerMode.Containment,
            Enums.eSpecialCase.Defiance => PlannerMode.Defiance,
            Enums.eSpecialCase.ComboLevel1 => PlannerMode.ComboLevel1,
            Enums.eSpecialCase.ComboLevel2 => PlannerMode.ComboLevel2,
            Enums.eSpecialCase.ComboLevel3 => PlannerMode.ComboLevel3,
            Enums.eSpecialCase.FastMode => PlannerMode.FastMode,
            Enums.eSpecialCase.PerfectionOfBody1 => PlannerMode.PerfectionOfBody1,
            Enums.eSpecialCase.PerfectionOfBody2 => PlannerMode.PerfectionOfBody2,
            Enums.eSpecialCase.PerfectionOfBody3 => PlannerMode.PerfectionOfBody3,
            Enums.eSpecialCase.PerfectionOfMind1 => PlannerMode.PerfectionOfMind1,
            Enums.eSpecialCase.PerfectionOfMind2 => PlannerMode.PerfectionOfMind2,
            Enums.eSpecialCase.PerfectionOfMind3 => PlannerMode.PerfectionOfMind3,
            Enums.eSpecialCase.PerfectionOfSoul1 => PlannerMode.PerfectionOfSoul1,
            Enums.eSpecialCase.PerfectionOfSoul2 => PlannerMode.PerfectionOfSoul2,
            Enums.eSpecialCase.PerfectionOfSoul3 => PlannerMode.PerfectionOfSoul3,
            Enums.eSpecialCase.DefensiveAdaptation => PlannerMode.DefensiveAdaptation,
            Enums.eSpecialCase.EfficientAdaptation => PlannerMode.EfficientAdaptation,
            Enums.eSpecialCase.OffensiveAdaptation => PlannerMode.OffensiveAdaptation,
            Enums.eSpecialCase.PackMentality => PlannerMode.PackMentality,
            Enums.eSpecialCase.FastSnipe => PlannerMode.FastSnipe,
            _ => PlannerMode.None
        };

        return mode != PlannerMode.None;
    }

    private static void BuildSupportPowerLinks(
        IEnumerable<OmniPowerDefinition> scopedPowers,
        IReadOnlyDictionary<string, IPower> midsPowers,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications,
        OmniApplyResult applyResult)
    {
        var scopedPowerList = scopedPowers.ToList();
        var inherentGridControls = classifications
            .Where(pair => pair.Value is { HiddenPower: true, IncludeFlag: true, InherentType: not Enums.eGridType.None })
            .Select(pair => pair.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        RemoveInherentGridControlsFromSubPowerLinks(midsPowers.Values, inherentGridControls, applyResult);

        foreach (var omniPower in scopedPowerList)
        {
            var childFullName = CanonicalizeOmniFullName(omniPower.FullName);
            if (!classifications.TryGetValue(childFullName, out var classification) || !classification.HiddenPower)
            {
                continue;
            }

            if (classification is { IncludeFlag: true, InherentType: not Enums.eGridType.None })
            {
                continue;
            }

            var parentFullName = ResolveSupportParent(omniPower, scopedPowerList);
            if (string.IsNullOrWhiteSpace(parentFullName))
            {
                continue;
            }

            parentFullName = CanonicalizeOmniFullName(parentFullName);
            if (!midsPowers.TryGetValue(parentFullName, out var parent) ||
                !midsPowers.ContainsKey(childFullName))
            {
                applyResult.SupportPowerLinksUnresolved++;
                applyResult.AddLimited(applyResult.SupportPowerLinkSkips,
                    $"{childFullName}: parent {parentFullName} did not resolve.");
                continue;
            }

            var subPowers = (parent.UIDSubPower ?? [])
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            if (!subPowers.Contains(childFullName, StringComparer.OrdinalIgnoreCase))
            {
                subPowers.Add(childFullName);
                parent.UIDSubPower = subPowers.ToArray();
                parent.IsModified = true;
                applyResult.SupportPowerLinksCreated++;
                applyResult.AddLimited(applyResult.SupportPowerLinks, $"{parentFullName} -> {childFullName}");
            }
        }
    }

    private static void RemoveInherentGridControlsFromSubPowerLinks(
        IEnumerable<IPower> powers,
        IReadOnlySet<string> inherentGridControls,
        OmniApplyResult applyResult)
    {
        if (inherentGridControls.Count == 0)
        {
            return;
        }

        foreach (var parent in powers)
        {
            var subPowers = parent.UIDSubPower ?? [];
            if (subPowers.Length == 0)
            {
                continue;
            }

            var cleaned = subPowers
                .Where(subPower => !inherentGridControls.Contains(subPower))
                .ToArray();
            if (cleaned.Length == subPowers.Length)
            {
                continue;
            }

            parent.UIDSubPower = cleaned;
            parent.IsModified = true;
            applyResult.AddLimited(applyResult.SupportPowerLinkSkips,
                $"{parent.FullName}: removed {subPowers.Length - cleaned.Length} planner control subpower link(s); these display in the inherent grid.");
        }
    }

    private static string ResolveSupportParent(OmniPowerDefinition child, IReadOnlyCollection<OmniPowerDefinition> scopedPowers)
    {
        if (OmniPowerClassifier.TryGetSinglePowerRequirement(child, out var parentPowerName))
        {
            return NormalizeSupportParentRequirement(parentPowerName);
        }

        var modeParentName = child.ModesRequired
            .Select(ModeToParentFullName)
            .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));
        if (string.IsNullOrWhiteSpace(modeParentName))
        {
            return string.Empty;
        }

        if (modeParentName.Contains('.', StringComparison.Ordinal))
        {
            return modeParentName;
        }

        var childSet = $"{GroupNamePart(child.FullName)}.{SetNamePart(child.FullName)}";
        return scopedPowers.FirstOrDefault(power =>
                   string.Equals($"{GroupNamePart(power.FullName)}.{SetNamePart(power.FullName)}", childSet, StringComparison.OrdinalIgnoreCase) &&
                   LastNamePart(power.FullName).Contains(modeParentName, StringComparison.OrdinalIgnoreCase) &&
                   !string.Equals(power.FullName, child.FullName, StringComparison.OrdinalIgnoreCase))
               ?.FullName ?? string.Empty;
    }

    private static string NormalizeSupportParentRequirement(string requirement)
    {
        var normalized = requirement?.Trim() ?? string.Empty;
        while (normalized.StartsWith("!", StringComparison.Ordinal))
        {
            normalized = normalized[1..].TrimStart();
        }

        return normalized;
    }

    private static string ModeToParentFullName(string mode)
    {
        var normalized = NormalizeName(mode);
        if (normalized.Contains("brightnova", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("peacebringerblaster", StringComparison.OrdinalIgnoreCase))
        {
            return "Peacebringer_Offensive.Luminous_Blast.Bright_Nova";
        }

        if (normalized.Contains("whitedwarf", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("peacebringertanker", StringComparison.OrdinalIgnoreCase))
        {
            return "Peacebringer_Defensive.Luminous_Aura.White_Dwarf";
        }

        if (normalized.Contains("darknova", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("warshadeblaster", StringComparison.OrdinalIgnoreCase))
        {
            return "Warshade_Offensive.Umbral_Blast.Dark_Nova";
        }

        if (normalized.Contains("blackdwarf", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("warshadetanker", StringComparison.OrdinalIgnoreCase))
        {
            return "Warshade_Defensive.Umbral_Aura.Black_Dwarf";
        }

        return string.Empty;
    }

    private static string CanonicalizeOmniFullName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        var canonical = value;
        foreach (var alias in PowerNameAliases)
        {
            if (canonical.Equals(alias.Key, StringComparison.OrdinalIgnoreCase))
            {
                return alias.Value;
            }

            canonical = ReplaceAliasToken(canonical, alias.Key, alias.Value);
        }

        return canonical;
    }

    private static string ReplaceAliasToken(string value, string alias, string replacement)
    {
        var index = value.IndexOf(alias, StringComparison.OrdinalIgnoreCase);
        while (index >= 0)
        {
            var beforeOk = index == 0 || !IsPowerNameCharacter(value[index - 1]);
            var afterIndex = index + alias.Length;
            var afterOk = afterIndex >= value.Length || !IsPowerNameCharacter(value[afterIndex]);
            if (beforeOk && afterOk)
            {
                value = value[..index] + replacement + value[afterIndex..];
                index += replacement.Length;
            }
            else
            {
                index = afterIndex;
            }

            index = value.IndexOf(alias, index, StringComparison.OrdinalIgnoreCase);
        }

        return value;
    }

    private static bool IsPowerNameCharacter(char value)
    {
        return char.IsLetterOrDigit(value) || value == '_';
    }

    private static void RepairAliasedPowerIdentities(IDatabase database, OmniApplyResult applyResult)
    {
        var powersets = database.Powersets ?? [];
        var powersetNames = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (powersetNames.Count == 0)
        {
            return;
        }

        var powers = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Cast<IPower>()
            .ToList();
        var canonicalPowers = powers
            .Where(power => IsCanonicalPowerIdentity(power, powersetNames))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var remove = new List<IPower>();

        foreach (var power in powers)
        {
            var canonicalFullName = CanonicalizeOmniFullName(power.FullName);
            if (string.IsNullOrWhiteSpace(canonicalFullName))
            {
                continue;
            }

            var canonicalFullSetName = FullSetName(canonicalFullName);
            if (string.IsNullOrWhiteSpace(canonicalFullSetName) ||
                !powersetNames.Contains(canonicalFullSetName))
            {
                continue;
            }

            if (IsPowerIdentityCurrent(power, canonicalFullName))
            {
                canonicalPowers.TryAdd(canonicalFullName, power);
                continue;
            }

            if (canonicalPowers.TryGetValue(canonicalFullName, out var existing) &&
                !ReferenceEquals(existing, power))
            {
                if (CanRemoveAliasedDuplicate(power, existing, canonicalFullName))
                {
                    remove.Add(power);
                    applyResult.AliasedPowerDuplicateRemovals++;
                    applyResult.AddLimited(applyResult.AliasedPowerDuplicateRemovalDetails,
                        $"{power.FullName} removed; canonical duplicate is {existing.FullName}");
                }
                else
                {
                    applyResult.AliasedPowerIdentityCollisions++;
                    applyResult.AddLimited(applyResult.AliasedPowerIdentityCollisionDetails,
                        $"{power.FullName} wanted {canonicalFullName}, but {existing.FullName} already exists and did not look like a duplicate");
                }

                continue;
            }

            var oldFullName = power.FullName;
            ApplyCanonicalPowerName(power, canonicalFullName);
            power.IsModified = true;
            canonicalPowers[canonicalFullName] = power;
            applyResult.AliasedPowerIdentityRepairs++;
            applyResult.AddLimited(applyResult.AliasedPowerIdentityRepairDetails,
                $"{oldFullName} -> {canonicalFullName}");
        }

        if (remove.Count == 0)
        {
            return;
        }

        database.Power = (database.Power ?? [])
            .Where(power => power == null || !remove.Any(candidate => ReferenceEquals(candidate, power)))
            .ToArray();
    }

    private static void RepairAliasedPowersetIdentities(IDatabase database, OmniApplyResult applyResult)
    {
        var powersets = (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .Cast<IPowerset>()
            .ToList();
        var canonicalPowersets = powersets
            .Where(powerset => IsPowersetIdentityCurrent(powerset, CanonicalizeOmniFullName(powerset.FullName)))
            .GroupBy(powerset => CanonicalizeOmniFullName(powerset.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var remove = new List<IPowerset>();

        foreach (var powerset in powersets)
        {
            var canonicalFullName = CanonicalizeOmniFullName(powerset.FullName);
            if (string.IsNullOrWhiteSpace(canonicalFullName))
            {
                continue;
            }

            if (IsPowersetIdentityCurrent(powerset, canonicalFullName))
            {
                if (canonicalPowersets.TryGetValue(canonicalFullName, out var existing) &&
                    !ReferenceEquals(existing, powerset))
                {
                    if (CanRemoveAliasedPowersetDuplicate(powerset, existing, canonicalFullName))
                    {
                        remove.Add(powerset);
                        applyResult.AliasedPowersetDuplicateRemovals++;
                        applyResult.AddLimited(applyResult.AliasedPowersetDuplicateRemovalDetails,
                            $"{powerset.FullName} removed; canonical duplicate is {existing.FullName}");
                    }
                    else
                    {
                        applyResult.AliasedPowersetIdentityCollisions++;
                        applyResult.AddLimited(applyResult.AliasedPowersetIdentityCollisionDetails,
                            $"{powerset.FullName} duplicates {existing.FullName}, but did not look like a removable duplicate");
                    }
                }
                else
                {
                    canonicalPowersets[canonicalFullName] = powerset;
                }

                continue;
            }

            if (canonicalPowersets.TryGetValue(canonicalFullName, out var canonicalExisting) &&
                !ReferenceEquals(canonicalExisting, powerset))
            {
                if (CanRemoveAliasedPowersetDuplicate(powerset, canonicalExisting, canonicalFullName))
                {
                    if (ShouldPreferAliasedPowersetAsKeeper(powerset, canonicalExisting))
                    {
                        var aliasedFullName = powerset.FullName;
                        ApplyCanonicalPowersetName(powerset, canonicalFullName);
                        powerset.IsModified = true;
                        canonicalPowersets[canonicalFullName] = powerset;
                        remove.Add(canonicalExisting);
                        applyResult.AliasedPowersetIdentityRepairs++;
                        applyResult.AliasedPowersetDuplicateRemovals++;
                        applyResult.AddLimited(applyResult.AliasedPowersetIdentityRepairDetails,
                            $"{aliasedFullName} -> {canonicalFullName}; kept older/icon-bearing powerset and removed duplicate {canonicalExisting.FullName}");
                        applyResult.AddLimited(applyResult.AliasedPowersetDuplicateRemovalDetails,
                            $"{canonicalExisting.FullName} removed; canonical identity moved onto existing {aliasedFullName}");
                    }
                    else
                    {
                        remove.Add(powerset);
                        applyResult.AliasedPowersetDuplicateRemovals++;
                        applyResult.AddLimited(applyResult.AliasedPowersetDuplicateRemovalDetails,
                            $"{powerset.FullName} removed; canonical duplicate is {canonicalExisting.FullName}");
                    }
                }
                else
                {
                    applyResult.AliasedPowersetIdentityCollisions++;
                    applyResult.AddLimited(applyResult.AliasedPowersetIdentityCollisionDetails,
                        $"{powerset.FullName} wanted {canonicalFullName}, but {canonicalExisting.FullName} already exists and did not look like a duplicate");
                }

                continue;
            }

            var oldFullName = powerset.FullName;
            ApplyCanonicalPowersetName(powerset, canonicalFullName);
            powerset.IsModified = true;
            canonicalPowersets[canonicalFullName] = powerset;
            applyResult.AliasedPowersetIdentityRepairs++;
            applyResult.AddLimited(applyResult.AliasedPowersetIdentityRepairDetails,
                $"{oldFullName} -> {canonicalFullName}");
        }

        if (remove.Count == 0)
        {
            return;
        }

        database.Powersets = (database.Powersets ?? [])
            .Where(powerset => powerset == null || !remove.Any(candidate => ReferenceEquals(candidate, powerset)))
            .ToArray();
    }

    private static bool IsPowersetIdentityCurrent(IPowerset powerset, string canonicalFullName)
    {
        return string.Equals(powerset.FullName, canonicalFullName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(powerset.GroupName, GroupNamePart(canonicalFullName), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(powerset.SetName, SetNamePart(canonicalFullName), StringComparison.OrdinalIgnoreCase);
    }

    private static bool CanRemoveAliasedPowersetDuplicate(
        IPowerset powerset,
        IPowerset existing,
        string canonicalFullName)
    {
        if (!CanonicalizeOmniFullName(powerset.FullName).Equals(canonicalFullName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!PowersetTypeMatches(powerset.SetType, existing.SetType))
        {
            return false;
        }

        return NormalizeName(powerset.DisplayName).Equals(NormalizeName(existing.DisplayName), StringComparison.OrdinalIgnoreCase) ||
               NormalizeName(powerset.GroupName).Equals(NormalizeName(existing.GroupName), StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldPreferAliasedPowersetAsKeeper(IPowerset aliased, IPowerset canonical)
    {
        if (!string.IsNullOrWhiteSpace(aliased.ImageName) && string.IsNullOrWhiteSpace(canonical.ImageName))
        {
            return true;
        }

        if (aliased.nID >= 0 && canonical.nID >= 0 && aliased.nID < canonical.nID)
        {
            return true;
        }

        return false;
    }

    private static void SortPowersByPowersetAndLevel(IDatabase database, OmniApplyResult applyResult)
    {
        var before = BuildPowerOrderBySet(database.Power);
        Array.Sort(database.Power);
        var after = BuildPowerOrderBySet(database.Power);

        foreach (var setName in after.Keys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase))
        {
            if (!before.TryGetValue(setName, out var beforeOrder) ||
                beforeOrder.SequenceEqual(after[setName], StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            applyResult.PowersetsReorderedByPowerLevel++;
            applyResult.AddLimited(applyResult.PowersetPowerOrderDetails,
                $"{setName}: {FormatPowerOrder(beforeOrder)} -> {FormatPowerOrder(after[setName])}");
        }
    }

    private static Dictionary<string, List<string>> BuildPowerOrderBySet(IEnumerable<IPower?> powers)
    {
        return powers
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Cast<IPower>()
            .GroupBy(power => power.FullSetName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.Select(power => $"{power.PowerName}@{power.Level}").ToList(),
                StringComparer.OrdinalIgnoreCase);
    }

    private static string FormatPowerOrder(IReadOnlyCollection<string> values)
    {
        const int max = 10;
        var text = string.Join(", ", values.Take(max));
        return values.Count > max ? $"{text}, ... +{values.Count - max}" : text;
    }

    private static void RepairMalformedPowerNames(IDatabase database, OmniApplyResult applyResult)
    {
        var powersets = database.Powersets ?? [];
        var powersetNames = powersets
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (powersetNames.Count == 0)
        {
            return;
        }

        var powers = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Cast<IPower>()
            .ToList();
        var canonicalPowers = powers
            .Where(power => IsCanonicalPowerIdentity(power, powersetNames))
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var remove = new List<IPower>();

        foreach (var power in powers)
        {
            var repairedFullName = RepairMalformedFullNamePrefix(power.FullName);
            if (string.Equals(repairedFullName, power.FullName, StringComparison.Ordinal) ||
                string.IsNullOrWhiteSpace(repairedFullName))
            {
                continue;
            }

            var canonicalFullName = CanonicalizeOmniFullName(repairedFullName);
            var canonicalFullSetName = FullSetName(canonicalFullName);
            if (string.IsNullOrWhiteSpace(canonicalFullSetName) ||
                !powersetNames.Contains(canonicalFullSetName))
            {
                continue;
            }

            if (canonicalPowers.TryGetValue(canonicalFullName, out var existing) &&
                !ReferenceEquals(existing, power))
            {
                if (CanRemoveMalformedDuplicate(power, existing, canonicalFullName))
                {
                    remove.Add(power);
                    applyResult.MalformedPowerDuplicateRemovals++;
                    applyResult.AddLimited(applyResult.MalformedPowerDuplicateRemovalDetails,
                        $"{power.FullName} removed; canonical duplicate is {existing.FullName}");
                }
                else
                {
                    applyResult.MalformedPowerNameCollisions++;
                    applyResult.AddLimited(applyResult.MalformedPowerNameCollisionDetails,
                        $"{power.FullName} wanted {canonicalFullName}, but {existing.FullName} already exists and did not look like a duplicate");
                }

                continue;
            }

            var oldFullName = power.FullName;
            ApplyCanonicalPowerName(power, canonicalFullName);
            power.IsModified = true;
            canonicalPowers[canonicalFullName] = power;
            applyResult.MalformedPowerNameRepairs++;
            applyResult.AddLimited(applyResult.MalformedPowerNameRepairDetails,
                $"{oldFullName} -> {canonicalFullName}");
        }

        if (remove.Count == 0)
        {
            return;
        }

        database.Power = (database.Power ?? [])
            .Where(power => power == null || !remove.Any(candidate => ReferenceEquals(candidate, power)))
            .ToArray();
    }

    private static void RepairSetBonusSiblingPowerIdentities(
        IDatabase database,
        IReadOnlyCollection<OmniPowerDefinition> scopedPowers,
        OmniApplyResult applyResult)
    {
        var targetSetBonusPowers = scopedPowers
            .Where(power => !string.IsNullOrWhiteSpace(power.FullName) && IsStrictSetBonusPower(power.FullName))
            .Select(power => CanonicalizeOmniFullName(power.FullName))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (targetSetBonusPowers.Length == 0)
        {
            return;
        }

        var dbPowers = (database.Power ?? [])
            .Where(power => power != null && !string.IsNullOrWhiteSpace(power.FullName))
            .Cast<IPower>()
            .ToList();
        var exactLookup = dbPowers
            .GroupBy(power => CanonicalizeOmniFullName(power.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
        var powersetNames = (database.Powersets ?? [])
            .Where(powerset => powerset != null && !string.IsNullOrWhiteSpace(powerset.FullName))
            .Select(powerset => CanonicalizeOmniFullName(powerset.FullName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var targetFullName in targetSetBonusPowers)
        {
            if (exactLookup.ContainsKey(targetFullName))
            {
                continue;
            }

            var targetFullSet = FullSetName(targetFullName);
            if (string.IsNullOrWhiteSpace(targetFullSet) || !powersetNames.Contains(targetFullSet))
            {
                continue;
            }

            var targetLeaf = LastNamePart(targetFullName);
            var candidates = dbPowers
                .Where(power => IsStrictSetBonusPower(power.FullName) &&
                                !string.Equals(CanonicalizeOmniFullName(power.FullName), targetFullName, StringComparison.OrdinalIgnoreCase) &&
                                string.Equals(LastNamePart(CanonicalizeOmniFullName(power.FullName)), targetLeaf, StringComparison.OrdinalIgnoreCase))
                .Take(2)
                .ToList();

            if (candidates.Count != 1)
            {
                if (candidates.Count > 1)
                {
                    applyResult.AddLimited(applyResult.BoostSetBonusImportAuditDetails,
                        $"{targetFullName}: strict set-bonus sibling repair ambiguous; candidates={string.Join(", ", candidates.Select(power => power.FullName))}");
                }

                continue;
            }

            var candidate = candidates[0];
            var oldFullName = candidate.FullName;
            ApplyCanonicalPowerName(candidate, targetFullName);
            candidate.IsModified = true;
            exactLookup[targetFullName] = candidate;
            applyResult.ScopedPowerSetIdentityRepairs++;
            applyResult.AddLimited(applyResult.ScopedPowerSetIdentityRepairDetails,
                $"{oldFullName} -> {targetFullName}: repaired strict set-bonus sibling misattachment");
        }
    }

    private static string RepairMalformedFullNamePrefix(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName) || IsValidFullNameStart(fullName[0]))
        {
            return fullName;
        }

        var index = 0;
        while (index < fullName.Length && !IsValidFullNameStart(fullName[index]))
        {
            index++;
        }

        return index >= fullName.Length ? string.Empty : fullName[index..];
    }

    private static bool IsValidFullNameStart(char value)
    {
        return char.IsLetterOrDigit(value) || value == '_';
    }

    private static bool CanRemoveMalformedDuplicate(IPower malformedPower, IPower canonicalPower, string canonicalFullName)
    {
        if (!string.Equals(
                CanonicalizeOmniFullName(RepairMalformedFullNamePrefix(malformedPower.FullName)),
                canonicalFullName,
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var malformedNames = new[]
            {
                malformedPower.PowerName,
                malformedPower.DisplayName,
                LastNamePart(RepairMalformedFullNamePrefix(malformedPower.FullName))
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var canonicalNames = new[]
            {
                canonicalPower.PowerName,
                canonicalPower.DisplayName,
                LastNamePart(canonicalPower.FullName),
                LastNamePart(canonicalFullName)
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return malformedNames.Overlaps(canonicalNames);
    }

    private static bool IsCanonicalPowerIdentity(IPower power, IReadOnlySet<string> canonicalPowersetNames)
    {
        var canonicalFullName = CanonicalizeOmniFullName(power.FullName);
        return canonicalPowersetNames.Contains(FullSetName(canonicalFullName)) &&
               IsPowerIdentityCurrent(power, canonicalFullName);
    }

    private static bool IsPowerIdentityCurrent(IPower power, string canonicalFullName)
    {
        return string.Equals(power.FullName, canonicalFullName, StringComparison.OrdinalIgnoreCase) &&
               string.Equals(power.GroupName, GroupNamePart(canonicalFullName), StringComparison.OrdinalIgnoreCase) &&
               string.Equals(power.SetName, SetNamePart(canonicalFullName), StringComparison.OrdinalIgnoreCase);
    }

    private static bool CanRemoveAliasedDuplicate(IPower stalePower, IPower canonicalPower, string canonicalFullName)
    {
        if (!string.Equals(CanonicalizeOmniFullName(stalePower.FullName), canonicalFullName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var staleNames = new[]
            {
                stalePower.PowerName,
                stalePower.DisplayName,
                LastNamePart(stalePower.FullName)
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var canonicalNames = new[]
            {
                canonicalPower.PowerName,
                canonicalPower.DisplayName,
                LastNamePart(canonicalPower.FullName),
                LastNamePart(canonicalFullName)
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return staleNames.Overlaps(canonicalNames);
    }

    private static string FormatAliasForReport(string omniFullName, string midsFullName)
    {
        return string.Equals(omniFullName, midsFullName, StringComparison.OrdinalIgnoreCase)
            ? omniFullName
            : $"{omniFullName} -> {midsFullName}";
    }

    private static bool TryFindCompositePowerIdentityMatch(
        IReadOnlyCollection<IPower?> existingPowers,
        OmniPowerDefinition omniPower,
        string midsFullName,
        IReadOnlyDictionary<string, OmniPowersetDefinition> scopedPowersets,
        out IPower matchedPower)
    {
        matchedPower = null!;
        var setFullName = FullSetName(midsFullName);
        var strictScopedIdentity = IsStrictScopedIdentityFamily(midsFullName);
        var omniSetDefinition = scopedPowersets.TryGetValue(setFullName, out var powersetDefinition)
            ? powersetDefinition
            : null;
        var omniDisplayName = string.IsNullOrWhiteSpace(omniPower.DisplayName)
            ? LastNamePart(midsFullName)
            : omniPower.DisplayName;
        var omniPowerInternalName = string.IsNullOrWhiteSpace(omniPower.Name)
            ? LastNamePart(midsFullName)
            : omniPower.Name;
        var normalizedDisplay = NormalizeName(omniDisplayName);
        var normalizedInternal = NormalizeName(omniPowerInternalName);
        if (string.IsNullOrWhiteSpace(setFullName) || string.IsNullOrWhiteSpace(normalizedDisplay))
        {
            return false;
        }

        var candidates = existingPowers
            .Where(power => power != null &&
                            PowerSetIdentityMatches(power, setFullName, omniSetDefinition, strictScopedIdentity))
            .Cast<IPower>()
            .ToList();
        var internalCandidates = candidates
            .Where(power => PowerInternalIdentityMatches(power, normalizedInternal))
            .Take(2)
            .ToList();
        if (internalCandidates.Count == 1)
        {
            matchedPower = internalCandidates[0];
            return true;
        }

        if (internalCandidates.Count > 1 || strictScopedIdentity || !ShouldAllowDisplayNameCompositeFallback(omniPower))
        {
            return false;
        }

        var displayCandidates = candidates
            .Where(power => PowerDisplayIdentityMatches(power, normalizedDisplay))
            .Take(2)
            .ToList();
        if (displayCandidates.Count != 1)
        {
            return false;
        }

        matchedPower = displayCandidates[0];
        return true;
    }

    private static bool PowerSetIdentityMatches(
        IPower power,
        string omniFullSetName,
        OmniPowersetDefinition? omniPowerset,
        bool strictScopedIdentity = false)
    {
        var existingPowerset = power.GetPowerSet();
        var existingFullSetName = existingPowerset?.FullName ?? power.FullSetName;
        var existingCanonical = CanonicalizeOmniFullName(existingFullSetName);
        var omniCanonical = CanonicalizeOmniFullName(omniFullSetName);
        if (string.Equals(existingCanonical, omniCanonical, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (strictScopedIdentity)
        {
            return false;
        }

        if (IsStrictSetBonusPowerset(omniCanonical) || IsStrictSetBonusPowerset(existingCanonical))
        {
            return false;
        }

        if (IsEpicPowersetFullName(existingCanonical) || IsEpicPowersetFullName(omniCanonical))
        {
            return EpicPowerSetIdentityMatches(existingPowerset, existingCanonical, omniPowerset, omniCanonical);
        }

        var existingGroup = NormalizeName(string.IsNullOrWhiteSpace(existingPowerset?.GroupName)
            ? GroupNamePart(existingCanonical)
            : existingPowerset.GroupName);
        var omniGroup = NormalizeName(GroupNamePart(omniCanonical));
        if (!existingGroup.Equals(omniGroup, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var existingSetNames = new[]
            {
                existingPowerset?.SetName,
                SetNamePart(existingCanonical),
                existingPowerset?.DisplayName
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var omniSetNames = new[]
            {
                SetNamePart(omniCanonical),
                omniPowerset?.Name,
                omniPowerset?.DisplayName
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return existingSetNames.Overlaps(omniSetNames);
    }

    private static bool EpicPowerSetIdentityMatches(
        IPowerset? existingPowerset,
        string existingCanonicalFullSetName,
        OmniPowersetDefinition? omniPowerset,
        string omniCanonicalFullSetName)
    {
        if (!IsEpicPowersetFullName(existingCanonicalFullSetName) || !IsEpicPowersetFullName(omniCanonicalFullSetName))
        {
            return false;
        }

        IEnumerable<string> existingArchetypes = existingPowerset?.GetArchetypes() ?? Enumerable.Empty<string>();
        IEnumerable<string> omniArchetypes = omniPowerset?.Archetypes ?? Enumerable.Empty<string>();
        var existingClassKeys = EpicClassKeys(
            existingCanonicalFullSetName,
            existingArchetypes,
            existingPowerset?.ATClass ?? string.Empty);
        var omniClassKeys = EpicClassKeys(
            omniCanonicalFullSetName,
            omniArchetypes,
            string.Empty);
        if (existingClassKeys.Count == 0 || omniClassKeys.Count == 0 || !existingClassKeys.Overlaps(omniClassKeys))
        {
            return false;
        }

        var existingFamily = EpicFamilyName(SetNamePart(existingCanonicalFullSetName), existingClassKeys);
        var omniFamily = EpicFamilyName(SetNamePart(omniCanonicalFullSetName), omniClassKeys);
        if (existingFamily.Equals(omniFamily, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var existingDisplayName = NormalizeName(existingPowerset?.DisplayName);
        var omniDisplayName = NormalizeName(string.IsNullOrWhiteSpace(omniPowerset?.DisplayName)
            ? SetNamePart(omniCanonicalFullSetName)
            : omniPowerset.DisplayName);
        return !string.IsNullOrWhiteSpace(existingDisplayName) &&
               !string.IsNullOrWhiteSpace(omniDisplayName) &&
               existingDisplayName.Equals(omniDisplayName, StringComparison.OrdinalIgnoreCase);
    }

    private static bool PowerInternalIdentityMatches(IPower power, string normalizedInternal)
    {
        var existingPowerNames = new[]
            {
                power.PowerName,
                LastNamePart(power.FullName)
            }
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(NormalizeName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return existingPowerNames.Contains(normalizedInternal);
    }

    private static bool PowerDisplayIdentityMatches(IPower power, string normalizedDisplay)
    {
        return !string.IsNullOrWhiteSpace(power.DisplayName) &&
               NormalizeName(power.DisplayName).Equals(normalizedDisplay, StringComparison.OrdinalIgnoreCase);
    }

    private static bool ShouldAllowDisplayNameCompositeFallback(OmniPowerDefinition omniPower)
    {
        if (omniPower == null)
        {
            return false;
        }

        if (!omniPower.ShowInManage || !omniPower.ShowInInfo || omniPower.AutoIssue)
        {
            return false;
        }

        if (OmniPowerClassifier.TryGetSinglePowerRequirement(omniPower, out _))
        {
            return false;
        }

        return !IsCompositeMatchSensitiveExecutionGroup(GroupNamePart(omniPower.FullName));
    }

    private static bool IsCompositeMatchSensitiveExecutionGroup(string group)
    {
        return NormalizeName(group) is
            "redirects" or
            "pets" or
            "kheldianpets" or
            "mastermindpets" or
            "villainpets" or
            "incarnatepets";
    }

    private static bool IsStrictScopedIdentityFamily(string fullName)
    {
        var canonicalFullName = CanonicalizeOmniFullName(fullName);
        var fullSetName = FullSetName(canonicalFullName);
        return fullSetName is
            "Incarnate.Interface" or
            "Incarnate.Socket" or
            "Incarnate.Genesis_Silent" or
            "Incarnate.Hybrid_Silent" or
            "Incarnate.Interface_Silent";
    }

    private static string FullSetName(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 2 ? $"{parts[0]}.{parts[1]}" : string.Empty;
    }

    private OmniDataProviderId DetectOmniDataProviderId(string exportRoot)
    {
        foreach (var hint in ReadProviderHintsFromManifest(exportRoot))
        {
            var providerId = ServerRulesProfileResolver.DetectProviderIdFromHint(hint);
            if (providerId != OmniDataProviderId.Unknown)
            {
                return providerId;
            }
        }

        return ServerRulesProfileResolver.DetectProviderIdFromHint(
            Path.GetFileName(exportRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)));
    }

    private IEnumerable<string> ReadProviderHintsFromManifest(string exportRoot)
    {
        var manifestPath = Path.Combine(exportRoot, "assets", "manifest.json");
        var manifest = TryReadJson<JObject>(manifestPath);
        if (manifest == null)
        {
            yield break;
        }

        foreach (var sourceRoot in manifest["SourceRoots"]?.Values<string>() ?? [])
        {
            if (!string.IsNullOrWhiteSpace(sourceRoot))
            {
                yield return sourceRoot;
            }
        }

        var dialect = manifest.Value<string>("Dialect");
        if (!string.IsNullOrWhiteSpace(dialect))
        {
            yield return dialect;
        }
    }

    private void ApplyClassAttributesToDatabase(IDatabase database, OmniImportResult result, OmniApplyResult? applyResult)
    {
        var detectedProviderId = DetectOmniDataProviderId(result.ExportRoot);
        var resolvedProviderId = detectedProviderId != OmniDataProviderId.Unknown
            ? detectedProviderId
            : database.DataProviderId;
        var resolvedRulesetId = ServerRulesProfileResolver.ResolvePlannerRulesetId(detectedProviderId, database.PlannerRulesetId);

        database.ClassAttributes = result.ClassAttributes;
        database.OmniImportSource = OmniDatabaseImportSource.Omni;
        database.DataProviderId = resolvedProviderId;
        database.PlannerRulesetId = resolvedRulesetId;
        database.PlannerRulesetVersion = ServerRulesProfileResolver.ResolvePlannerRulesetVersion(resolvedRulesetId);
        database.HasCanonicalOmniPlannerMath = resolvedRulesetId != PlannerRulesetId.Legacy;
        if (applyResult != null)
        {
            applyResult.ClassAttributesStored = result.ClassAttributes.Count;
            applyResult.AddLimited(
                applyResult.ClassTableFileDetails,
                $"Planner provider/ruleset detected from export '{result.ExportRoot}': provider={database.DataProviderId}, ruleset={database.PlannerRulesetId}, version={database.PlannerRulesetVersion}.");
        }
        EnsureRetainedArchetypeShells(database, result, applyResult);

        if (database.Classes == null)
        {
            return;
        }

        foreach (var archetype in database.Classes.OfType<Archetype>())
        {
            var className = NormalizeClassName(archetype.ClassName);
            if (!database.ClassAttributes.TryGetValue(className, out var table))
            {
                continue;
            }

            ApplyLegacyArchetypeSummary(archetype, table);
            archetype.IsModified = true;
            if (applyResult != null)
            {
                applyResult.ArchetypeSummariesUpdated++;
            }
        }
    }

    private static void EnsureRetainedArchetypeShells(IDatabase database, OmniImportResult result, OmniApplyResult? applyResult)
    {
        var classes = database.Classes?.Where(archetype => archetype != null).ToList() ?? [];
        var existingClassNames = classes
            .Where(archetype => !string.IsNullOrWhiteSpace(archetype.ClassName))
            .Select(archetype => NormalizeClassName(archetype.ClassName))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var table in result.ClassAttributes.Values)
        {
            var className = NormalizeClassName(table.ClassName);
            if (string.IsNullOrWhiteSpace(className) ||
                existingClassNames.Contains(className) ||
                !result.Scope.IsRetainedClass(className))
            {
                continue;
            }

            var displayName = string.IsNullOrWhiteSpace(table.DisplayName)
                ? className.Replace("Class_", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("_", " ", StringComparison.Ordinal)
                : table.DisplayName;
            var archetype = new Archetype
            {
                ClassName = className,
                DisplayName = displayName,
                DescShort = displayName,
                DescLong = displayName,
                PrimaryGroup = table.PrimaryCategory,
                SecondaryGroup = table.SecondaryCategory,
                Playable = table.Playable,
                IsNew = true,
                IsModified = true
            };

            classes.Add(archetype);
            existingClassNames.Add(className);
            if (applyResult != null)
            {
                applyResult.AddLimited(
                    applyResult.ClassTableFileDetails,
                    $"{className}: created retained archetype shell from Omni summary metadata.");
            }
        }

        database.Classes = classes.ToArray();
    }

    private static Enums.ePowerSetType MapPowersetType(string category, string fullName)
    {
        var normalizedCategory = NormalizeName(category);
        var group = NormalizeName(GroupNamePart(fullName));
        return normalizedCategory switch
        {
            "primary" => Enums.ePowerSetType.Primary,
            "secondary" => Enums.ePowerSetType.Secondary,
            "epic" or "ancillary" => Enums.ePowerSetType.Ancillary,
            "pool" => Enums.ePowerSetType.Pool,
            "inherent" => Enums.ePowerSetType.Inherent,
            "temporarypowers" or "temporary" or "temp" => Enums.ePowerSetType.Temp,
            "prestige" => Enums.ePowerSetType.Temp,
            "setbonus" => Enums.ePowerSetType.SetBonus,
            "boosts" or "boost" => Enums.ePowerSetType.Boost,
            "incarnate" => Enums.ePowerSetType.Incarnate,
            "redirects" or "redirect" => Enums.ePowerSetType.Redirect,
            _ when group == "pool" => Enums.ePowerSetType.Pool,
            _ when group == "inherent" => Enums.ePowerSetType.Inherent,
            _ when group == "epic" => Enums.ePowerSetType.Ancillary,
            _ when group == "temporarypowers" => Enums.ePowerSetType.Temp,
            _ when group == "prestige" => Enums.ePowerSetType.Temp,
            _ when group == "setbonus" => Enums.ePowerSetType.SetBonus,
            _ when group == "boosts" => Enums.ePowerSetType.Boost,
            _ when group == "incarnate" => Enums.ePowerSetType.Incarnate,
            _ when group == "redirects" => Enums.ePowerSetType.Redirect,
            _ when group.Contains("pets", StringComparison.OrdinalIgnoreCase) => Enums.ePowerSetType.Pet,
            _ => Enums.ePowerSetType.None
        };
    }

    private static Enums.ePowerSetType InferPowersetType(IDatabase database, string fullName)
    {
        var groupName = GroupNamePart(fullName);
        if (string.IsNullOrWhiteSpace(groupName) || database.Classes == null)
        {
            return Enums.ePowerSetType.None;
        }

        foreach (var archetype in database.Classes.Where(a => a != null))
        {
            if (string.Equals(groupName, archetype.PrimaryGroup, StringComparison.OrdinalIgnoreCase))
            {
                return Enums.ePowerSetType.Primary;
            }

            if (string.Equals(groupName, archetype.SecondaryGroup, StringComparison.OrdinalIgnoreCase))
            {
                return Enums.ePowerSetType.Secondary;
            }

            if (string.Equals(groupName, archetype.EpicGroup, StringComparison.OrdinalIgnoreCase))
            {
                return Enums.ePowerSetType.Ancillary;
            }
        }

        return Enums.ePowerSetType.None;
    }

    private static Enums.ePowerType MapPowerType(string type)
    {
        return OmniPowerClassifier.MapPowerType(type);
    }

    private static Enums.eEffectArea MapEffectArea(string area)
    {
        return NormalizeName(area) switch
        {
            "singletarget" or "character" => Enums.eEffectArea.Character,
            "sphere" or "targetedaoe" or "pbaoe" => Enums.eEffectArea.Sphere,
            "cone" => Enums.eEffectArea.Cone,
            "location" or "locationaoe" => Enums.eEffectArea.Location,
            "volume" => Enums.eEffectArea.Volume,
            "map" => Enums.eEffectArea.Map,
            "room" => Enums.eEffectArea.Room,
            "touch" => Enums.eEffectArea.Touch,
            _ => Enums.eEffectArea.None
        };
    }

    private static Enums.eVector MapAttackTypes(IEnumerable<string> attackTypes)
    {
        var result = Enums.eVector.None;
        foreach (var attackType in attackTypes)
        {
            result |= NormalizeName(attackType).ToLowerInvariant() switch
            {
                "melee" => Enums.eVector.Melee_Attack,
                "ranged" => Enums.eVector.Ranged_Attack,
                "aoe" or "areaofeffect" => Enums.eVector.AOE_Attack,
                "smashing" or "smash" => Enums.eVector.Smashing_Attack,
                "lethal" => Enums.eVector.Lethal_Attack,
                "cold" => Enums.eVector.Cold_Attack,
                "fire" => Enums.eVector.Fire_Attack,
                "energy" => Enums.eVector.Energy_Attack,
                "negativeenergy" or "negative" or "negativeenergydamage" => Enums.eVector.Negative_Energy_Attack,
                "psionic" or "psi" => Enums.eVector.Psionic_Attack,
                "toxic" => Enums.eVector.Toxic_Attack,
                _ => Enums.eVector.None
            };
        }

        return result;
    }

    private static void TrackApplyAttackVectors(
        string fullName,
        Enums.eVector prior,
        Enums.eVector current,
        OmniPowerDefinition source,
        OmniApplyResult applyResult)
    {
        if (current == Enums.eVector.None || prior == current)
        {
            return;
        }

        applyResult.AttackVectorsMapped++;
        applyResult.AddLimited(applyResult.AttackVectorDetails,
            $"{fullName}: {prior} -> {current} from [{string.Join(", ", source.AttackTypes)}]");
    }

    private static Enums.eEntity MapEntities(IEnumerable<string> values)
    {
        var result = Enums.eEntity.None;
        foreach (var value in values)
        {
            result |= MapEntity(value);
        }

        return result;
    }

    private static Enums.eEntity MapEntity(string value)
    {
        var normalized = NormalizeName(value);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Enums.eEntity.None;
        }

        if (normalized.Contains("caster", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("self", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eEntity.Caster;
        }

        if (normalized.Contains("mypet", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("pet", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eEntity.MyPet;
        }

        if (normalized.Contains("player", StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Contains("dead", StringComparison.OrdinalIgnoreCase)
                ? Enums.eEntity.DeadPlayer
                : Enums.eEntity.Player;
        }

        if (normalized.Contains("teammate", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("ally", StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Contains("dead", StringComparison.OrdinalIgnoreCase)
                ? Enums.eEntity.DeadTeammate
                : Enums.eEntity.Teammate;
        }

        if (normalized.Contains("friend", StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Contains("dead", StringComparison.OrdinalIgnoreCase)
                ? Enums.eEntity.DeadFriend
                : Enums.eEntity.Friend;
        }

        if (normalized.Contains("villain", StringComparison.OrdinalIgnoreCase) ||
            normalized.Contains("foe", StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Contains("dead", StringComparison.OrdinalIgnoreCase)
                ? Enums.eEntity.DeadFoe
                : Enums.eEntity.Foe;
        }

        if (normalized.Contains("location", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eEntity.Location;
        }

        if (normalized.Contains("any", StringComparison.OrdinalIgnoreCase))
        {
            return Enums.eEntity.Any;
        }

        return Enums.eEntity.None;
    }

    private static string GroupNamePart(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    private static string SetNamePart(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 1 ? parts[1] : string.Empty;
    }

    private static string LastNamePart(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[^1] : string.Empty;
    }

    private static string NormalizeName(string value)
    {
        return (value ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("(", string.Empty, StringComparison.Ordinal)
            .Replace(")", string.Empty, StringComparison.Ordinal)
            .Trim()
            .ToLowerInvariant();
    }

    private void LoadArchetypes(
        string exportRoot,
        OmniExportManifest manifest,
        OmniImportResult result,
        IProgress<OmniImportProgress>? progress)
    {
        var archetypeRoot = Path.Combine(exportRoot, "archetypes");
        if (!Directory.Exists(archetypeRoot))
        {
            result.Report.AddLimited(result.Report.UnresolvedEntities, "Missing archetypes directory.");
            return;
        }

        var files = manifest.ArchetypeFiles;
        if (files.Count == 0)
        {
            return;
        }

        var loaded = new ConcurrentBag<(string File, OmniArchetypeDefinition Archetype)>();
        Parallel.ForEach(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = GetAdaptiveParallelDegree(8) },
            file =>
            {
                var archetype = ReadJson<OmniArchetypeDefinition>(file);
                if (archetype != null)
                {
                    loaded.Add((file, archetype));
                }
            });

        var ordered = loaded.OrderBy(entry => entry.File, StringComparer.OrdinalIgnoreCase).ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            var file = ordered[index].File;
            var archetype = ordered[index].Archetype;
            ReportStageProgress(
                progress,
                OmniImportStageId.ReadArchetypes,
                string.Empty,
                index + 1,
                ordered.Count,
                markComplete: index + 1 >= ordered.Count);

            result.Report.ArchetypesRead++;
            if (archetype.Playable)
            {
                result.Report.PlayableArchetypesRead++;
            }

            var className = NormalizeClassName(archetype.InternalName);
            if (!result.Scope.ShouldRetainArchetype(archetype))
            {
                result.Report.SkippedArchetypesRead++;
                result.Report.AddLimited(result.Report.SkippedArchetypes,
                    $"{className}: excluded non-playable archetype");
                continue;
            }

            result.Report.RetainedArchetypesRead++;
            result.Report.AddLimited(result.Report.RetainedArchetypes,
                $"{className}: {(archetype.Playable ? "playable" : "whitelisted non-playable")}");
            result.Scope.AddRetainedArchetype(archetype);

            var table = BuildClassAttributeTable(archetype);
            result.ClassAttributes[table.ClassName] = table;
            result.Report.ClassAttributeTablesImported++;

            if (!archetype.Playable)
            {
                continue;
            }

            foreach (var powerset in archetype.PrimaryPowersets
                         .Concat(archetype.SecondaryPowersets)
                         .Concat(archetype.EpicPowersets)
                         .Concat(archetype.Powersets))
            {
                if (OmniImportScope.IsExcludedPowerset(powerset))
                {
                    continue;
                }

                result.Report.AddLimited(result.Report.ImportedPowersets, powerset, 5000);
            }
        }
    }

    private void LoadClassTables(
        string exportRoot,
        OmniExportManifest manifest,
        OmniImportResult result,
        IProgress<OmniImportProgress>? progress)
    {
        var tableRoot = Path.Combine(exportRoot, "tables");
        if (!Directory.Exists(tableRoot))
        {
            result.Report.AddLimited(result.Report.UnresolvedEntities, "Missing tables directory.");
            return;
        }

        var files = manifest.ClassTableFiles;
        if (files.Count == 0)
        {
            return;
        }

        var loaded = new ConcurrentBag<(string File, OmniClassAttributeTable Table)>();
        Parallel.ForEach(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = GetAdaptiveParallelDegree(4) },
            file =>
            {
                var table = ReadClassTable(file);
                if (table != null && !string.IsNullOrWhiteSpace(table.ClassName))
                {
                    loaded.Add((file, table));
                }
            });

        var ordered = loaded.OrderBy(entry => entry.File, StringComparer.OrdinalIgnoreCase).ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            var file = ordered[index].File;
            var table = ordered[index].Table;
            ReportStageProgress(
                progress,
                OmniImportStageId.ReadClassTables,
                string.Empty,
                index + 1,
                ordered.Count,
                markComplete: index + 1 >= ordered.Count);

            result.Report.ClassTableFilesRead++;
            result.Report.AddLimited(result.Report.ClassTableFiles, Path.GetFileName(file), 500);
            if (!result.Scope.IsRetainedClass(table.ClassName))
            {
                result.Report.SkippedClassTableFiles++;
                result.Report.AddLimited(result.Report.SkippedClassTableFileDetails,
                    $"{Path.GetFileName(file)} -> {table.ClassName}: excluded class table");
                continue;
            }

            result.Report.RetainedClassTableFiles++;
            result.Report.AddLimited(result.Report.RetainedClassTableFileDetails,
                $"{Path.GetFileName(file)} -> {table.ClassName}");
            if (!result.ClassAttributes.TryGetValue(table.ClassName, out var existing))
            {
                existing = new OmniClassAttributeTable
                {
                    ClassName = table.ClassName
                };
                result.ClassAttributes[table.ClassName] = existing;
            }

            MergeClassTable(existing, table, result);
        }

        foreach (var classTable in result.ClassAttributes.Values.Where(c => c.HasAttributes && c.NamedTables.Count == 0))
        {
            result.Report.MissingClassTableReferences++;
            result.Report.AddLimited(result.Report.MissingClassTableReferenceDetails,
                $"{classTable.ClassName}: no matching tables/class_*.json named_tables were loaded.", 500);
        }
    }

    private static void TrackApplyClassTables(OmniImportResult dryRunResult, OmniApplyResult applyResult)
    {
        applyResult.ArchetypesRead = dryRunResult.Report.ArchetypesRead;
        applyResult.PlayableArchetypesRead = dryRunResult.Report.PlayableArchetypesRead;
        applyResult.RetainedArchetypesRead = dryRunResult.Report.RetainedArchetypesRead;
        applyResult.SkippedArchetypesRead = dryRunResult.Report.SkippedArchetypesRead;
        applyResult.ClassTableFilesRead = dryRunResult.Report.ClassTableFilesRead;
        applyResult.RetainedClassTableFiles = dryRunResult.Report.RetainedClassTableFiles;
        applyResult.SkippedClassTableFiles = dryRunResult.Report.SkippedClassTableFiles;
        applyResult.CanonicalNamedTablesStored = dryRunResult.ClassAttributes.Values.Sum(c => c.NamedTables.Count);
        foreach (var file in dryRunResult.Report.ClassTableFiles)
        {
            applyResult.AddLimited(applyResult.ClassTableFileDetails, file);
        }
    }

    private OmniClassAttributeTable? ReadClassTable(string file)
    {
        try
        {
            var obj = ReadJson<JObject>(file);
            if (obj == null)
            {
                return null;
            }

            var className = obj.Value<string>("name") ??
                            obj.Value<string>("internal_name") ??
                            Path.GetFileNameWithoutExtension(file);
            return new OmniClassAttributeTable
            {
                ClassName = NormalizeClassName(className),
                PrimaryCategory = obj.Value<string>("primary_category") ?? string.Empty,
                SecondaryCategory = obj.Value<string>("secondary_category") ?? string.Empty,
                NamedTables = ReadNamedTables(obj, "named_tables")
            };
        }
        catch
        {
            return null;
        }
    }

    private static void MergeClassTable(
        OmniClassAttributeTable target,
        OmniClassAttributeTable source,
        OmniImportResult result)
    {
        if (string.IsNullOrWhiteSpace(target.PrimaryCategory))
        {
            target.PrimaryCategory = source.PrimaryCategory;
        }

        if (string.IsNullOrWhiteSpace(target.SecondaryCategory))
        {
            target.SecondaryCategory = source.SecondaryCategory;
        }

        foreach (var namedTable in source.NamedTables)
        {
            if (target.NamedTables.TryGetValue(namedTable.Key, out var existingValues))
            {
                if (existingValues.Length == namedTable.Value.Length &&
                    existingValues.SequenceEqual(namedTable.Value))
                {
                    continue;
                }

                result.Report.DuplicateCanonicalNamedTables++;
                result.Report.AddLimited(result.Report.DuplicateNamedTables,
                    $"{target.ClassName}: conflicting values for {namedTable.Key}", 500);
            }

            target.NamedTables[namedTable.Key] = namedTable.Value;
            result.Report.CanonicalNamedTablesLoaded++;
        }
    }

    private static string NormalizeClassName(string value)
    {
        return OmniImportScope.NormalizeClassName(value);
    }

    private void ScanPowers(
        string exportRoot,
        OmniExportManifest manifest,
        OmniImportResult result,
        IProgress<OmniImportProgress>? progress)
    {
        var powersRoot = Path.Combine(exportRoot, "powers");
        if (!Directory.Exists(powersRoot))
        {
            result.Report.AddLimited(result.Report.UnresolvedEntities, "Missing powers directory.");
            return;
        }

        ReportStageProgress(progress, OmniImportStageId.LoadScopedPowersets);
        var scopedPowersets = LoadScopedPowersets(exportRoot, result.Scope, result, manifest).ToList();
        result.CachedScopedPowersets = scopedPowersets;
        result.ScopedPowersetFullNames.Clear();
        foreach (var scopedPowerset in scopedPowersets.Where(p => !string.IsNullOrWhiteSpace(p.FullName)))
        {
            result.ScopedPowersetFullNames.Add(CanonicalizeOmniFullName(scopedPowerset.FullName));
        }
        TrackEpicDryRunDisplayCollisions(scopedPowersets, result);
        ReportStageProgress(progress, OmniImportStageId.LoadScopedPowersets, $"{scopedPowersets.Count:n0} powersets loaded", markComplete: true);
        ReportStageProgress(progress, OmniImportStageId.LoadScopedPowers);
        var scopedPowers = LoadScopedPowers(exportRoot, result.Scope, result, progress, manifest).ToList();
        result.CachedScopedPowers = scopedPowers;
        result.ScopedPowerFullNames.Clear();
        foreach (var scopedPower in scopedPowers.Where(p => !string.IsNullOrWhiteSpace(p.FullName)))
        {
            result.ScopedPowerFullNames.Add(CanonicalizeOmniFullName(scopedPower.FullName));
        }
        TrackBoostSetBonusScopeCoverage(
            result.Report,
            result.ScopedPowersetFullNames,
            DeriveScopedPowersetFullNames(result.ScopedPowerFullNames),
            result.ScopedPowerFullNames);
        var scopedPowerLookup = BuildScopedPowerLookup(scopedPowers);
        var classifier = new OmniPowerClassifier();

        for (var index = 0; index < scopedPowers.Count; index++)
        {
            var power = scopedPowers[index];
            ReportStageProgress(
                progress,
                OmniImportStageId.ClassifyPowers,
                string.Empty,
                index + 1,
                scopedPowers.Count,
                markComplete: index + 1 >= scopedPowers.Count);
            result.Report.PowersInScope++;
            var classification = classifier.Classify(power, scopedPowerLookup);
            TrackDryRunClassification(CanonicalizeOmniFullName(power.FullName), classification, result);
            TrackSorceryEnflameDryRun(power, classification, result);
            ScanPowerMetadata(power, result);
        }
    }

    private IEnumerable<OmniPowerDefinition> LoadScopedPowers(
        string exportRoot,
        OmniImportScope scope,
        OmniImportResult? dryRunResult = null,
        IProgress<OmniImportProgress>? progress = null,
        OmniExportManifest? manifest = null)
    {
        var powersRoot = Path.Combine(exportRoot, "powers");
        if (!Directory.Exists(powersRoot))
        {
            yield break;
        }

        var files = manifest?.PowerFiles?.Count > 0
            ? manifest.PowerFiles.ToList()
            : Directory.EnumerateFiles(powersRoot, "*.json", SearchOption.AllDirectories)
                .Where(f => !Path.GetFileName(f).Equals("index.json", StringComparison.OrdinalIgnoreCase))
                .ToList();
        for (var index = 0; index < files.Count; index++)
        {
            var file = files[index];
            ReportStageProgress(
                progress,
                OmniImportStageId.LoadScopedPowers,
                string.Empty,
                index + 1,
                files.Count,
                markComplete: index + 1 >= files.Count);
            if (dryRunResult != null)
            {
                dryRunResult.Report.PowersConsidered++;
            }

            var power = ReadJson<OmniPowerDefinition>(file);
            if (power == null)
            {
                continue;
            }

            var relative = Path.GetRelativePath(powersRoot, file);
            TrackSupportHeavyDiscovery(relative, dryRunResult);
            TrackPetPowerDiscovery(relative, dryRunResult);
            TrackEpicPowerDiscovery(relative, dryRunResult);
            if (ShouldSkipPowerDefinition(power))
            {
                if (dryRunResult != null)
                {
                    dryRunResult.Report.PowersSkippedOutOfScope++;
                    dryRunResult.Report.AddLimited(dryRunResult.Report.SkippedPowerGroups,
                        $"{power.FullName}: non-build temporary/costume power");
                }

                continue;
            }

            if (!scope.IsPowerFileInScope(relative, power.Powerset, power.Archetypes, power.FullName))
            {
                if (dryRunResult != null)
                {
                    dryRunResult.Report.PowersSkippedOutOfScope++;
                    var group = relative.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).FirstOrDefault() ?? relative;
                    dryRunResult.Report.AddLimited(
                        dryRunResult.Report.SkippedPowerGroups,
                        $"{group}: contains skipped out-of-scope power files (not a full-root exclusion)");
                }

                continue;
            }

            TrackSupportHeavyInScope(relative, power, dryRunResult);
            TrackPetPowerInScope(relative, power, dryRunResult);
            TrackEpicPowerInScope(relative, power, dryRunResult);
            yield return power;
        }
    }

    private static bool ShouldSkipPowerDefinition(OmniPowerDefinition power)
    {
        if (IsCostumeTemporaryPower(power))
        {
            return true;
        }

        return false;
    }

    private static bool IsCostumeTemporaryPower(OmniPowerDefinition power)
    {
        if (!GroupNamePart(power.FullName).Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase) ||
            !SetNamePart(power.FullName).Equals("Temporary_Powers", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (ContainsCostumeToken(power.FullName) ||
            ContainsCostumeToken(power.Name) ||
            ContainsCostumeToken(power.DisplayName) ||
            ContainsCostumeToken(power.DisplayShortHelp))
        {
            return true;
        }

        return power.Effects.Any(HasSetCostumeEffect);
    }

    private static bool ContainsCostumeToken(string value)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Contains("costume", StringComparison.OrdinalIgnoreCase);
    }

    private static bool HasSetCostumeEffect(OmniEffectDefinition effect)
    {
        return effect.Templates.Any(template =>
                   template.Attribs.Any(attrib => attrib.Equals("SetCostume", StringComparison.OrdinalIgnoreCase) ||
                                                  attrib.Equals("Set_Costume", StringComparison.OrdinalIgnoreCase)) ||
                   template.Type.Equals("SetCostume", StringComparison.OrdinalIgnoreCase) ||
                   template.Type.Equals("Set_Costume", StringComparison.OrdinalIgnoreCase)) ||
               effect.ChildEffects.Any(HasSetCostumeEffect);
    }

    private static void TrackPetPowersetDiscovery(string relativePath, OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var root = RootPart(relativePath);
        if (!IsPetRoot(root))
        {
            return;
        }

        IncrementCount(dryRunResult.Report.PetPowersetFilesDiscovered, root);
        dryRunResult.Report.AddLimited(dryRunResult.Report.PetImportScopeDetails,
            $"{root}: discovered powerset {relativePath}");
    }

    private static void TrackPetPowersetInScope(string relativePath, OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var root = RootPart(relativePath);
        if (!IsPetRoot(root))
        {
            return;
        }

        IncrementCount(dryRunResult.Report.PetPowersetFilesInScope, root);
        dryRunResult.Report.AddLimited(dryRunResult.Report.PetImportScopeDetails,
            $"{root}: scoped powerset {relativePath}");
    }

    private static void TrackEpicPowersetDiscovery(
        string relativePath,
        OmniPowersetDefinition powerset,
        OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null || !IsEpicRoot(RootPart(relativePath)))
        {
            return;
        }

        dryRunResult.Report.EpicPowersetFilesDiscovered++;
        dryRunResult.Report.AddLimited(dryRunResult.Report.EpicImportScopeDetails,
            $"discovered powerset {powerset.FullName} from {relativePath}");
    }

    private static void TrackEpicPowersetInScope(
        string relativePath,
        OmniPowersetDefinition powerset,
        OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null || !IsEpicRoot(RootPart(relativePath)))
        {
            return;
        }

        dryRunResult.Report.EpicPowersetFilesInScope++;
        var canonicalFullName = CanonicalizeOmniFullName(powerset.FullName);
        var classKeys = EpicClassKeys(canonicalFullName, powerset.Archetypes, string.Empty);
        dryRunResult.Report.AddLimited(dryRunResult.Report.EpicImportScopeDetails,
            $"scoped powerset {FormatAliasForReport(powerset.FullName, canonicalFullName)} display={powerset.DisplayName}, internal={powerset.Name}, classes={FormatClassKeys(classKeys)}, category={powerset.PowerCategory}");
        dryRunResult.Report.AddLimited(dryRunResult.Report.EpicPowersetIdentityDetails,
            $"{FormatAliasForReport(powerset.FullName, canonicalFullName)}: display={powerset.DisplayName}, internal={powerset.Name}, category={powerset.PowerCategory}, archetypes={string.Join(", ", powerset.Archetypes)}, classes={FormatClassKeys(classKeys)}");
        if (classKeys.Count > 0)
        {
            var family = EpicFamilyName(SetNamePart(canonicalFullName), classKeys);
            dryRunResult.Report.AddLimited(dryRunResult.Report.EpicPowersetPrefixSuffixCandidates,
                $"{canonicalFullName}: class={FormatClassKeys(classKeys)}, family={family}");
        }
    }

    private static void TrackEpicDryRunDisplayCollisions(
        IReadOnlyCollection<OmniPowersetDefinition> scopedPowersets,
        OmniImportResult result)
    {
        foreach (var collisionGroup in scopedPowersets
                     .Where(powerset => IsEpicPowersetFullName(powerset.FullName) &&
                                        !string.IsNullOrWhiteSpace(powerset.DisplayName))
                     .GroupBy(powerset => NormalizeName(powerset.DisplayName), StringComparer.OrdinalIgnoreCase)
                     .Where(group => group.Count() > 1))
        {
            result.Report.AddLimited(result.Report.EpicPowersetDisplayNameCollisions,
                $"{collisionGroup.First().DisplayName}: {string.Join(", ", collisionGroup.Select(powerset => $"{powerset.FullName}[{FormatClassKeys(EpicClassKeys(powerset.FullName, powerset.Archetypes, string.Empty))}]"))}");
        }
    }

    private static void TrackSorceryEnflameDryRun(
        OmniPowerDefinition power,
        OmniPowerClassification classification,
        OmniImportResult result)
    {
        if (!power.FullName.Equals("Pool.Sorcery.Enflame", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        result.Report.SorceryEnflameTraceCount++;
        result.Report.AddLimited(result.Report.SorceryEnflamePickabilityTrace,
            $"{power.FullName}: level={power.AvailableLevel}, type={power.Type}, show_in_manage={power.ShowInManage}, hidden={classification.HiddenPower}, include={classification.IncludeFlag}, normalBuildPick={classification.NormalBuildPick}, reasons={string.Join("; ", classification.Reasons)}");

        if (OmniExpressionConverter.TryConvertPowerRequirement(CanonicalizeOmniFullName(power.Requires), out var requirements))
        {
            result.Report.AddLimited(result.Report.SorceryEnflamePickabilityTrace,
                $"{power.FullName}: requirements={FormatAdvancedRequirementRows(requirements)}");
        }
        else
        {
            result.Report.PoolRequirementEvaluationFailureCount++;
            result.Report.AddLimited(result.Report.PoolRequirementEvaluationFailures,
                $"{power.FullName}: could not convert requirement '{power.Requires}'");
        }
    }

    private static void TrackSorceryEnflameApplyTrace(IDatabase database, OmniApplyResult applyResult)
    {
        var power = (database.Power ?? [])
            .FirstOrDefault(p => p != null && p.FullName.Equals("Pool.Sorcery.Enflame", StringComparison.OrdinalIgnoreCase));
        applyResult.SorceryEnflameTraceCount++;
        if (power == null)
        {
            applyResult.PoolRequirementEvaluationFailures++;
            applyResult.AddLimited(applyResult.PoolRequirementEvaluationFailureDetails,
                "Pool.Sorcery.Enflame: missing from database after safe import.");
            return;
        }

        var owningPowerset = GetOwningPowerset(database, power);
        var inPowersetArray = owningPowerset?.Powers?.Any(p =>
            p != null &&
            (ReferenceEquals(p, power) ||
             p.PowerIndex == power.PowerIndex ||
             p.FullName.Equals(power.FullName, StringComparison.OrdinalIgnoreCase))) == true;
        var allowedClasses = (database.Classes ?? [])
            .Where(cls => cls != null && cls.Playable)
            .Select(cls => cls!)
            .Where(cls => power.AllowedForClass(cls.Idx))
            .Select(cls => cls.ClassName)
            .ToArray();

        applyResult.AddLimited(applyResult.SorceryEnflamePickabilityTraceDetails,
            $"{power.FullName}: FullSetName={power.FullSetName}, PowerSetID={power.PowerSetID}, PowerSetIndex={power.PowerSetIndex}, owning={(owningPowerset?.FullName ?? "<missing>")}, inPowersetArray={inPowersetArray}, level={power.Level}, hidden={power.HiddenPower}, include={power.IncludeFlag}, allowedPlayableClasses={allowedClasses.Length}");
        applyResult.AddLimited(applyResult.SorceryEnflamePickabilityTraceDetails,
            $"{power.FullName}: advanced requirements={FormatAdvancedRequirementRows(power.AdvancedRequirements)}, legacy groups={FormatLegacyRequirementGroups(power.Requires)}");

        if (power.HiddenPower || power.Level <= 0 || !inPowersetArray || owningPowerset == null || allowedClasses.Length == 0)
        {
            applyResult.PoolRequirementEvaluationFailures++;
            applyResult.AddLimited(applyResult.PoolRequirementEvaluationFailureDetails,
                $"{power.FullName}: picker-blocked; hidden={power.HiddenPower}, level={power.Level}, owning={(owningPowerset?.FullName ?? "<missing>")}, inPowersetArray={inPowersetArray}, allowedPlayableClasses={allowedClasses.Length}");
        }
    }

    private static string FormatAdvancedRequirementRows(AdvancedConditionSet? set)
    {
        if (set is not { Rows.Count: > 0 })
        {
            return "<none>";
        }

        return string.Join(" | ", set.Rows.Select(row =>
            $"{row.Link}:{row.Kind}:{row.Subject}{(string.IsNullOrWhiteSpace(row.Value) ? string.Empty : "+" + row.Value)}{(row.Negated ? "!" : string.Empty)}"));
    }

    private static string FormatLegacyRequirementGroups(Requirement? requirement)
    {
        if (requirement == null)
        {
            return "<none>";
        }

        var groups = requirement.PowerID
            .Where(group => group.Any(value => !string.IsNullOrWhiteSpace(value)))
            .Select(group => $"({string.Join(" + ", group.Where(value => !string.IsNullOrWhiteSpace(value)))})")
            .ToArray();
        return groups.Length == 0 ? "<none>" : string.Join(" OR ", groups);
    }

    private static IPowerset? GetOwningPowerset(IDatabase database, IPower power)
    {
        var powersets = database.Powersets ?? [];
        if (power.PowerSetID >= 0 && power.PowerSetID < powersets.Length)
        {
            return powersets[power.PowerSetID];
        }

        var fullSetName = CanonicalizeOmniFullName(power.FullSetName);
        return powersets.FirstOrDefault(powerset =>
            powerset != null &&
            CanonicalizeOmniFullName(powerset.FullName).Equals(fullSetName, StringComparison.OrdinalIgnoreCase));
    }

    private static void TrackPetPowerDiscovery(string relativePath, OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var root = RootPart(relativePath);
        if (!IsPetRoot(root))
        {
            return;
        }

        IncrementCount(dryRunResult.Report.PetPowerFilesDiscovered, root);
    }

    private static void TrackEpicPowerDiscovery(string relativePath, OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null || !IsEpicRoot(RootPart(relativePath)))
        {
            return;
        }

        dryRunResult.Report.EpicPowerFilesDiscovered++;
    }

    private static void TrackPetPowerInScope(
        string relativePath,
        OmniPowerDefinition power,
        OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var root = RootPart(relativePath);
        if (!IsPetRoot(root))
        {
            return;
        }

        IncrementCount(dryRunResult.Report.PetPowerFilesInScope, root);
        IncrementCount(dryRunResult.Report.PetPowersLoaded, root);
        dryRunResult.Report.AddLimited(dryRunResult.Report.PetImportScopeDetails,
            $"{root}: loaded power {power.FullName} from {relativePath}");
    }

    private static void TrackEpicPowerInScope(
        string relativePath,
        OmniPowerDefinition power,
        OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null || !IsEpicRoot(RootPart(relativePath)))
        {
            return;
        }

        dryRunResult.Report.EpicPowerFilesInScope++;
        dryRunResult.Report.EpicPowersLoaded++;
        dryRunResult.Report.AddLimited(dryRunResult.Report.EpicImportScopeDetails,
            $"loaded power {power.FullName} powerset={power.Powerset} from {relativePath}");
    }

    private static void TrackSupportHeavyDiscovery(string relativePath, OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var root = RootPart(relativePath);
        switch (root)
        {
            case "temporary_powers":
                dryRunResult.Report.TemporaryPowerFilesDiscovered++;
                break;
            case "inherent":
                dryRunResult.Report.InherentPowerFilesDiscovered++;
                break;
        }

        if (IsSupportHeavyRoot(root))
        {
            dryRunResult.Report.SupportHeavyFilesDiscovered++;
            dryRunResult.Report.AddLimited(dryRunResult.Report.SupportHeavyFiles, relativePath, 500);
        }
    }

    private static void TrackSupportHeavyInScope(string relativePath, OmniPowerDefinition power, OmniImportResult? dryRunResult)
    {
        if (dryRunResult == null)
        {
            return;
        }

        var root = RootPart(relativePath);
        switch (root)
        {
            case "temporary_powers":
                dryRunResult.Report.TemporaryPowerFilesInScope++;
                break;
            case "inherent":
                dryRunResult.Report.InherentPowerFilesInScope++;
                break;
        }

        if (IsSupportHeavyRoot(root))
        {
            dryRunResult.Report.SupportHeavyFilesInScope++;
        }

        TrackModes(power.FullName, power.ModesRequired, power.ModesDisallowed, dryRunResult);
    }

    private static void TrackModes(
        string fullName,
        IEnumerable<string> requiredModes,
        IEnumerable<string> disallowedModes,
        OmniImportResult dryRunResult)
    {
        var requiredFlags = OmniModeMapper.ToFlags(requiredModes, out var unknownRequired);
        var disallowedFlags = OmniModeMapper.ToFlags(disallowedModes, out var unknownDisallowed);
        var knownRequired = requiredModes
            .Where(IsKnownMode)
            .Select(OmniModeMapper.Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var knownDisallowed = disallowedModes
            .Where(IsKnownMode)
            .Select(OmniModeMapper.Normalize)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (knownRequired.Length > 0 || knownDisallowed.Length > 0)
        {
            dryRunResult.Report.PowerLevelModeGateMappedCount++;
            dryRunResult.Report.AddLimited(dryRunResult.Report.PowerLevelModeGates,
                $"{fullName}: required={FormatMappedModes(knownRequired, requiredFlags)}, disallowed={FormatMappedModes(knownDisallowed, disallowedFlags)}", 500);
        }

        foreach (var mode in unknownRequired.Concat(unknownDisallowed).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            dryRunResult.Report.UnknownModeCount++;
            dryRunResult.Report.AddLimited(dryRunResult.Report.UnknownModes, $"{fullName}: {mode}", 500);
        }
    }

    private static bool IsKnownMode(string mode)
    {
        if (string.IsNullOrWhiteSpace(mode))
        {
            return false;
        }

        var normalized = OmniModeMapper.Normalize(mode);
        return OmniModeMapper.TryToFlag(normalized, out _) ||
               OmniModeMapper.TryFromModeName(normalized, out _, out _) ||
               OmniModeMapper.TryGetPlannerMode(normalized, out _);
    }

    private static string FormatMappedModes(IReadOnlyCollection<string> modes, Enums.eModeFlags flags)
    {
        if (modes.Count == 0)
        {
            return "None";
        }

        return flags == Enums.eModeFlags.None
            ? string.Join(", ", modes)
            : $"{flags} ({string.Join(", ", modes)})";
    }

    private static string RootPart(string relativePath)
    {
        return relativePath
            .Replace('\\', '/')
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault() ?? string.Empty;
    }

    private static bool IsSupportHeavyRoot(string root)
    {
        return NormalizeName(root) is
            "inherent" or
            "pets" or
            "temporarypowers" or
            "incarnate" or
            "incarnatepets" or
            "kheldianpets" or
            "mastermindpets" or
            "villainpets";
    }

    private static bool IsPetRoot(string root)
    {
        return NormalizeName(root) is
            "incarnatepets" or
            "kheldianpets" or
            "mastermindpets" or
            "pets" or
            "villainpets";
    }

    private static bool IsPetPowerFullName(string fullName)
    {
        return IsPetRoot(GroupNamePart(fullName));
    }

    private static bool IsTemporaryPowerFullName(string fullName)
    {
        return string.Equals(GroupNamePart(fullName), "Temporary_Powers", StringComparison.OrdinalIgnoreCase);
    }

    private static string PowerPowersetFullName(OmniPowerDefinition power)
    {
        return CanonicalizeOmniFullName(string.IsNullOrWhiteSpace(power.Powerset)
            ? FullSetName(power.FullName)
            : power.Powerset);
    }

    private static bool IsPetScopedPower(OmniPowerDefinition power, OmniImportScope scope)
    {
        var fullSetName = PowerPowersetFullName(power);
        return scope.GetPowersetType(fullSetName) == Enums.ePowerSetType.Pet ||
               IsPetPowerFullName(CanonicalizeOmniFullName(power.FullName));
    }

    private static bool ShouldMainImportScopedPower(
        OmniPowerDefinition power,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications)
    {
        return GetScopedDisposition(power, classifications) is
            OmniScopedPowerDisposition.MainImportVisible or
            OmniScopedPowerDisposition.MainImportHidden;
    }

    private static bool IsPetManifestOwnedScopedPower(
        OmniPowerDefinition power,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications)
    {
        return GetScopedDisposition(power, classifications) == OmniScopedPowerDisposition.PetManifestOwned;
    }

    private static OmniScopedPowerDisposition GetScopedDisposition(
        OmniPowerDefinition power,
        IReadOnlyDictionary<string, OmniPowerClassification> classifications)
    {
        var fullName = CanonicalizeOmniFullName(power.FullName);
        return classifications.TryGetValue(fullName, out var classification)
            ? classification.ScopedDisposition
            : OmniScopedPowerDisposition.MainImportVisible;
    }

    private static bool IsRetainedPetPowerset(
        OmniImportScope scope,
        string rawFullName,
        string canonicalFullName,
        IReadOnlySet<string>? retainedPetPowersets = null)
    {
        var retainedByType = scope.GetPowersetType(rawFullName) == Enums.ePowerSetType.Pet ||
                             scope.GetPowersetType(canonicalFullName) == Enums.ePowerSetType.Pet;
        if (!retainedByType)
        {
            return false;
        }

        return retainedPetPowersets == null ||
               retainedPetPowersets.Count == 0 ||
               retainedPetPowersets.Contains(canonicalFullName);
    }

    private static bool IsEpicRoot(string root)
    {
        return NormalizeName(root) == "epic";
    }

    private static bool IsEpicPowersetFullName(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && IsEpicRoot(parts[0]);
    }

    private static bool IsEpicPowerFullName(string fullName)
    {
        var parts = (fullName ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length >= 3 && IsEpicRoot(parts[0]);
    }

    private static string FormatClassKeys(IReadOnlyCollection<string> classKeys)
    {
        return classKeys.Count == 0
            ? "<unknown>"
            : string.Join(", ", classKeys.OrderBy(key => key, StringComparer.OrdinalIgnoreCase));
    }

    private static bool IsAlwaysIncludedScopeRoot(string root)
    {
        return NormalizeName(root) is
            "boosts" or
            "epic" or
            "incarnate" or
            "inherent" or
            "pool" or
            "prestige" or
            "redirects" or
            "setbonus";
    }

    private static bool IsPlayableArchetypeScopeRoot(string root)
    {
        return !string.IsNullOrWhiteSpace(root) &&
               !IsAlwaysIncludedScopeRoot(root) &&
               !IsPetRoot(root);
    }

    private static void IncrementCount(IDictionary<string, int> counts, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        counts.TryGetValue(key, out var count);
        counts[key] = count + 1;
    }

    private static string NormalizeRoot(string exportRoot)
    {
        return Path.GetFullPath(exportRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private static int GetAdaptiveParallelDegree(int ceiling)
    {
        var floor = Environment.ProcessorCount > 1 ? 2 : 1;
        return Math.Min(Math.Min(ceiling, 4), Math.Max(floor, Environment.ProcessorCount - 1));
    }

    private void ActivateWorkPlan(OmniImportWorkPlan workPlan)
    {
        _activeWorkPlan = workPlan;
    }

    private void ClearWorkPlan()
    {
        _activeWorkPlan = null;
    }

    private void ReportStageProgress(
        IProgress<OmniImportProgress>? progress,
        OmniImportStageId stageId,
        string detail = "",
        int current = 0,
        int total = 0,
        bool markComplete = false)
    {
        if (progress == null)
        {
            return;
        }

        if (_activeWorkPlan == null)
        {
            progress.Report(new OmniImportProgress(
                markComplete ? 100 : 0,
                stageId.ToString(),
                detail,
                current,
                total,
                OmniImportWorkPlan.ToStageToken(stageId)));
            return;
        }

        if (total > 0 && !markComplete && !ShouldReportProgressPoint(current, total))
        {
            return;
        }

        progress.Report(_activeWorkPlan.Report(stageId, detail, current, total, markComplete));
    }

    private static void ScanGcmTags(string exportRoot, OmniImportResult result)
    {
        var gcm = ReadGcmTags(exportRoot);
        result.Report.GcmBlankTagCount = gcm.BlankCount;
        foreach (var duplicate in gcm.Duplicates)
        {
            result.Report.GcmDuplicateTagCount++;
            result.Report.AddLimited(result.Report.GcmDuplicateTags, duplicate, 500);
        }

        var existing = DatabaseAPI.Database?.EffectIds ?? [];
        var existingLookup = new HashSet<string>(existing.Where(e => !string.IsNullOrWhiteSpace(e)), StringComparer.OrdinalIgnoreCase);
        foreach (var tag in gcm.Tags)
        {
            result.GcmTags.Add(tag);
            result.Report.GcmTagsImportedCount++;
            result.Report.AddLimited(result.Report.GcmTags, tag, 500);
            if (existingLookup.Contains(tag))
            {
                result.Report.GcmTagsAlreadyKnownCount++;
                result.Report.AddLimited(result.Report.GcmTagsAlreadyKnown, tag, 500);
            }
            else
            {
                result.Report.GcmTagsWouldAddCount++;
                result.Report.AddLimited(result.Report.GcmTagsWouldAdd, tag, 500);
            }
        }
    }

    private static void ApplyGcmTags(IDatabase database, string exportRoot, OmniApplyResult applyResult)
    {
        var gcm = ReadGcmTags(exportRoot);
        applyResult.GcmBlankTags = gcm.BlankCount;
        applyResult.GcmDuplicateTags = gcm.Duplicates.Count;
        foreach (var duplicate in gcm.Duplicates)
        {
            applyResult.AddLimited(applyResult.GcmDuplicateTagDetails, duplicate);
        }

        database.EffectIds ??= [];
        var existing = new HashSet<string>(
            database.EffectIds.Where(tag => !string.IsNullOrWhiteSpace(tag)),
            StringComparer.OrdinalIgnoreCase);

        foreach (var tag in gcm.Tags)
        {
            applyResult.GcmTagsRead++;
            if (existing.Contains(tag))
            {
                applyResult.GcmTagsAlreadyKnown++;
                applyResult.AddLimited(applyResult.GcmTagsAlreadyKnownDetails, tag);
                continue;
            }

            database.EffectIds.Add(tag);
            existing.Add(tag);
            applyResult.GcmTagsAdded++;
            applyResult.AddLimited(applyResult.GcmTagsAddedDetails, tag);
        }
    }

    private static GcmTagLoadResult ReadGcmTags(string exportRoot)
    {
        var path = Path.Combine(exportRoot, "gcm.json");
        var result = new GcmTagLoadResult();
        if (!File.Exists(path))
        {
            return result;
        }

        var array = JArray.Parse(File.ReadAllText(path));
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var token in array)
        {
            var tag = token.Value<string>()?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(tag))
            {
                result.BlankCount++;
                continue;
            }

            if (!seen.Add(tag))
            {
                result.Duplicates.Add(tag);
                continue;
            }

            result.Tags.Add(tag);
        }

        return result;
    }

    private sealed class GcmTagLoadResult
    {
        public List<string> Tags { get; } = [];
        public List<string> Duplicates { get; } = [];
        public int BlankCount { get; set; }
    }

    private static int ScaleProgress(int current, int total, int startPercent, int endPercent)
    {
        if (total <= 0)
        {
            return endPercent;
        }

        var ratio = Math.Clamp(current / (float)total, 0f, 1f);
        return startPercent + (int)Math.Round((endPercent - startPercent) * ratio);
    }

    private static void ReportProgress(
        IProgress<OmniImportProgress>? progress,
        int percent,
        string stage,
        string detail = "",
        int current = 0,
        int total = 0,
        string stageId = "")
    {
        if (progress == null)
        {
            return;
        }

        if (total > 0 && !ShouldReportProgressPoint(current, total))
        {
            return;
        }

        progress.Report(new OmniImportProgress(percent, stage, detail, current, total, stageId));
    }

    private static bool ShouldReportProgressPoint(int current, int total)
    {
        if (current <= 1 || current >= total)
        {
            return true;
        }

        var stride = Math.Max(1, total / 200);
        return current % stride == 0;
    }

    private sealed class ThrottledProgress : IProgress<OmniImportProgress>, IDisposable
    {
        private const int MinimumUpdateMilliseconds = 200;
        private readonly IProgress<OmniImportProgress>? _inner;
        private readonly System.Diagnostics.Stopwatch _stopwatch = System.Diagnostics.Stopwatch.StartNew();
        private OmniImportProgress? _pending;
        private int _lastPercent = -1;
        private string _lastStage = string.Empty;
        private int _lastCurrent = -1;
        private int _lastTotal = -1;
        private long _lastReportMilliseconds;

        public ThrottledProgress(IProgress<OmniImportProgress>? inner)
        {
            _inner = inner;
        }

        public void Report(OmniImportProgress value)
        {
            if (_inner == null)
            {
                return;
            }

            var elapsed = _stopwatch.Elapsed;
            var estimatedRemaining = value.EstimatedRemaining;
            if (!estimatedRemaining.HasValue && value.Total > 0 && value.Current > 0 && value.Current < value.Total)
            {
                var remainingUnits = value.Total - value.Current;
                var millisecondsPerUnit = elapsed.TotalMilliseconds / value.Current;
                estimatedRemaining = TimeSpan.FromMilliseconds(Math.Max(0, millisecondsPerUnit * remainingUnits));
            }

            value = value with
            {
                StageId = string.IsNullOrWhiteSpace(value.StageId)
                    ? NormalizeStageId(value.Stage)
                    : value.StageId,
                Elapsed = value.Elapsed ?? elapsed,
                EstimatedRemaining = estimatedRemaining,
                GlobalPercent = value.GlobalPercent ?? (double)value.ClampedPercent / 100d
            };

            _pending = value;
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds - _lastReportMilliseconds;
            var force = value.ClampedPercent is 0 or 100;
            var stageChanged = !string.Equals(value.Stage, _lastStage, StringComparison.Ordinal);
            var percentChanged = value.ClampedPercent != _lastPercent;
            var countChanged = value.Current != _lastCurrent || value.Total != _lastTotal;
            var shouldFlush = force ||
                              stageChanged ||
                              percentChanged ||
                              (countChanged && elapsedMilliseconds >= MinimumUpdateMilliseconds);
            if (!shouldFlush)
            {
                return;
            }

            Flush();
        }

        public void Dispose()
        {
            Flush();
        }

        private void Flush()
        {
            if (_inner == null || _pending == null)
            {
                return;
            }

            _inner.Report(_pending);
            _lastPercent = _pending.ClampedPercent;
            _lastStage = _pending.Stage;
            _lastCurrent = _pending.Current;
            _lastTotal = _pending.Total;
            _lastReportMilliseconds = _stopwatch.ElapsedMilliseconds;
            _pending = null;
        }

        private static string NormalizeStageId(string stage)
        {
            if (string.IsNullOrWhiteSpace(stage))
            {
                return string.Empty;
            }

            var builder = new StringBuilder(stage.Length);
            foreach (var character in stage.Trim().ToLowerInvariant())
            {
                builder.Append(char.IsLetterOrDigit(character) ? character : '-');
            }

            return builder.ToString().Trim('-');
        }
    }

    private static void TrackDryRunClassification(
        string fullName,
        OmniPowerClassification classification,
        OmniImportResult result)
    {
        if (classification.HiddenPower)
        {
            result.Report.HiddenSupportPowerCount++;
            result.Report.AddLimited(result.Report.HiddenSupportPowers, classification.Summary(fullName), 500);
        }
        else if (classification.Reasons.Any(r => r.Contains("visible gated", StringComparison.OrdinalIgnoreCase)))
        {
            result.Report.VisibleGatedPowerCount++;
            result.Report.AddLimited(result.Report.VisibleGatedPowers, classification.Summary(fullName), 500);
        }

        if (classification.RedirectKind == OmniRedirectKind.ExecutionVariant)
        {
            result.Report.RedirectExecutionVariantCount++;
            result.Report.AddLimited(result.Report.RedirectExecutionVariants, classification.Summary(fullName), 500);
        }

        if (classification.ClickBuff)
        {
            result.Report.ClickBuffPowerCount++;
            result.Report.AddLimited(result.Report.ClickBuffClassifications, classification.Summary(fullName), 500);
        }

        if (classification.InherentType == Enums.eGridType.Class && classification.IncludeFlag && !classification.HiddenPower)
        {
            result.Report.ClassInherentPowerCount++;
            result.Report.AddLimited(result.Report.ClassInherentPowers, classification.Summary(fullName), 500);
        }

        if (classification.HiddenPower && classification.Reasons.Any(r => r.Contains("mode-gated", StringComparison.OrdinalIgnoreCase)))
        {
            result.Report.HiddenByPowerLevelModeGateCount++;
            result.Report.AddLimited(result.Report.HiddenByPowerLevelModeGates, classification.Summary(fullName), 500);
        }

        if (classification.ClassificationConfidence < 0.65f || classification.RedirectKind == OmniRedirectKind.Unknown)
        {
            result.Report.ManualClassificationReviewCount++;
            result.Report.AddLimited(result.Report.ManualClassificationReviews, classification.Summary(fullName), 500);
        }
    }

    private static void TrackPowerFieldCoverage(OmniPowerDefinition power, OmniImportReport report)
    {
        var owner = power.FullName;
        if (HasJsonValue(power.MaxToggleOnTimeValue))
        {
            AddMappedPowerField(report, $"{owner}: max_toggle_on_time -> UsageTime = {power.MaxToggleOnTime}");
        }
        else if (HasJsonValue(power.UsageTimeValue))
        {
            AddMappedPowerField(report, $"{owner}: usage_time -> UsageTime = {power.UsageTime}");
        }

        TrackChargeFieldCoverage(power, report);

        if (power.ExclusionGroups.Count > 0)
        {
            AddMappedPowerField(report, $"{owner}: exclusion_groups -> GroupMembership = {FormatStringList(power.ExclusionGroups)}");
        }

        if (HasJsonValue(power.RootTimeValue))
        {
            AddMappedPowerField(report, $"{owner}: root_time -> RootTime = {power.RootTime}");
        }

        if (power.RechargeGroups.Count > 0)
        {
            AddMappedPowerField(report, $"{owner}: recharge_groups -> RechargeGroups = {FormatStringList(power.RechargeGroups)}");
        }

        if (!string.IsNullOrWhiteSpace(power.NotifyAiWhen))
        {
            if (TryMapAiReport(power.NotifyAiWhen, out var mapped))
            {
                AddMappedPowerField(report, $"{owner}: notify_ai_when -> AIReport = {mapped}");
            }
            else
            {
                AddPowerFieldConflict(report, $"{owner}: notify_ai_when has no Mids eNotify mapping: {power.NotifyAiWhen}");
            }
        }

        TrackCastFlagCoverage(power, report);

        if (power.CastThrough.Count > 0)
        {
            if (power.CastThrough.Any(IsHoldCastThroughToken))
            {
                AddMappedPowerField(report, $"{owner}: cast_through -> CastThroughHold = true from {FormatStringList(power.CastThrough)}");
            }

            var nonHold = power.CastThrough.Where(value => !IsHoldCastThroughToken(value)).ToList();
            if (nonHold.Count > 0)
            {
                AddDeferredPowerField(report, $"{owner}: cast_through values not represented by Mids boolean CastThroughHold: {FormatStringList(nonHold)}");
            }
        }

        TrackEnhancementPolicyCoverage(power, report);
        TrackDeferredPowerFields(power, report);
        TrackExtensionPowerFields(power, report);
    }

    private static void TrackApplyPowerFieldCoverage(OmniPowerDefinition power, OmniApplyResult result)
    {
        var owner = power.FullName;
        if (HasJsonValue(power.MaxToggleOnTimeValue))
        {
            AddMappedPowerField(result, $"{owner}: max_toggle_on_time -> UsageTime = {power.MaxToggleOnTime}");
        }
        else if (HasJsonValue(power.UsageTimeValue))
        {
            AddMappedPowerField(result, $"{owner}: usage_time -> UsageTime = {power.UsageTime}");
        }

        TrackChargeFieldCoverage(power, result);

        if (power.ExclusionGroups.Count > 0)
        {
            AddMappedPowerField(result, $"{owner}: exclusion_groups -> GroupMembership = {FormatStringList(power.ExclusionGroups)}");
        }

        if (HasJsonValue(power.RootTimeValue))
        {
            AddMappedPowerField(result, $"{owner}: root_time -> RootTime = {power.RootTime}");
        }

        if (power.RechargeGroups.Count > 0)
        {
            AddMappedPowerField(result, $"{owner}: recharge_groups -> RechargeGroups = {FormatStringList(power.RechargeGroups)}");
        }

        if (!string.IsNullOrWhiteSpace(power.NotifyAiWhen))
        {
            if (TryMapAiReport(power.NotifyAiWhen, out var mapped))
            {
                AddMappedPowerField(result, $"{owner}: notify_ai_when -> AIReport = {mapped}");
            }
            else
            {
                AddPowerFieldConflict(result, $"{owner}: notify_ai_when has no Mids eNotify mapping: {power.NotifyAiWhen}");
            }
        }

        TrackCastFlagCoverage(power, result);

        if (power.CastThrough.Count > 0)
        {
            if (power.CastThrough.Any(IsHoldCastThroughToken))
            {
                AddMappedPowerField(result, $"{owner}: cast_through -> CastThroughHold = true from {FormatStringList(power.CastThrough)}");
            }

            var nonHold = power.CastThrough.Where(value => !IsHoldCastThroughToken(value)).ToList();
            if (nonHold.Count > 0)
            {
                AddDeferredPowerField(result, $"{owner}: cast_through values not represented by Mids boolean CastThroughHold: {FormatStringList(nonHold)}");
            }
        }

        TrackEnhancementPolicyCoverage(power, result);
        TrackDeferredPowerFields(power, result);
        TrackExtensionPowerFields(power, result);
    }

    private static void TrackCastFlagCoverage(OmniPowerDefinition power, OmniImportReport report)
    {
        if (power.CasterNearGround)
        {
            AddMappedPowerField(report, $"{power.FullName}: caster_near_ground -> CastFlags.NearGround");
        }

        if (power.TargetNearGround)
        {
            AddMappedPowerField(report, $"{power.FullName}: target_near_ground -> CastFlags.TargetNearGround");
        }

        if (!string.IsNullOrWhiteSpace(power.CastWhenDead))
        {
            if (CanCastAfterDeath(power.CastWhenDead))
            {
                AddMappedPowerField(report,
                    $"{power.FullName}: cast_when_dead '{power.CastWhenDead}' -> CastFlags.CastableAfterDeath");
                if (IsLossyCastWhenDeadMapping(power.CastWhenDead))
                {
                    AddMappedPowerFieldWithFallback(report,
                        $"{power.FullName}: cast_when_dead '{power.CastWhenDead}' collapsed to Mids CastableAfterDeath bit");
                }
            }
            else
            {
                AddMappedPowerField(report,
                    $"{power.FullName}: cast_when_dead '{power.CastWhenDead}' -> CastFlags.None");
            }
        }
    }

    private static void TrackCastFlagCoverage(OmniPowerDefinition power, OmniApplyResult result)
    {
        if (power.CasterNearGround)
        {
            AddMappedPowerField(result, $"{power.FullName}: caster_near_ground -> CastFlags.NearGround");
        }

        if (power.TargetNearGround)
        {
            AddMappedPowerField(result, $"{power.FullName}: target_near_ground -> CastFlags.TargetNearGround");
        }

        if (!string.IsNullOrWhiteSpace(power.CastWhenDead))
        {
            if (CanCastAfterDeath(power.CastWhenDead))
            {
                AddMappedPowerField(result,
                    $"{power.FullName}: cast_when_dead '{power.CastWhenDead}' -> CastFlags.CastableAfterDeath");
                if (IsLossyCastWhenDeadMapping(power.CastWhenDead))
                {
                    AddMappedPowerFieldWithFallback(result,
                        $"{power.FullName}: cast_when_dead '{power.CastWhenDead}' collapsed to Mids CastableAfterDeath bit");
                }
            }
            else
            {
                AddMappedPowerField(result,
                    $"{power.FullName}: cast_when_dead '{power.CastWhenDead}' -> CastFlags.None");
            }
        }
    }

    private static void ScanPowerMetadata(OmniPowerDefinition power, OmniImportResult result)
    {
        TrackPowerFieldCoverage(power, result.Report);

        if (!string.IsNullOrWhiteSpace(power.Requires))
        {
            result.Report.PowerRequirements++;
            TrackPowerRequirement(power.FullName, power.Requires, result);
        }

        if (!string.IsNullOrWhiteSpace(power.TargetRequires))
        {
            result.Report.TargetRequirements++;
            result.Report.AddLimited(result.Report.PowerTargetRequiresNotImported,
                $"{power.FullName} target_requires retained for planner routing: {power.TargetRequires}");
            if (LooksLikePlannerModeTargetRequires(power.TargetRequires))
            {
                result.Report.PowerTargetRequiresManualReviewCount++;
                result.Report.AddLimited(result.Report.PowerTargetRequiresManualReview,
                    $"{power.FullName} target_requires may contain source mode planner logic: {power.TargetRequires}");
            }
        }

        if (!string.IsNullOrWhiteSpace(power.ActivateRequires))
        {
            if (IsSustainedStatPower(power))
            {
                TrackExpression(power.FullName, power.ActivateRequires, result, AdvancedConditionEvaluationMode.ReportOnly);
                result.Report.AddLimited(result.Report.RawAdvancedExpressions,
                    $"{power.FullName} activate_requires retained for review: {power.ActivateRequires}");
            }
            else
            {
                result.Report.ActivateRequirementsIgnored++;
                result.Report.AddLimited(result.Report.IgnoredFields,
                    $"{power.FullName} activate_requires ignored: {power.ActivateRequires}");
            }
        }

        AddIgnoredField(power.FullName, "server_tray_requires", power.ServerTrayRequires, result);
        AddIgnoredField(power.FullName, "confirm_requires", power.ConfirmRequires, result);
        if (!string.IsNullOrWhiteSpace(power.HighlightExpression) &&
            power.HighlightExpression.Contains("kEngaged", StringComparison.OrdinalIgnoreCase) &&
            OmniModeMapper.IsSnipePlannerContext(power.FullName))
        {
            result.Report.SnipeEngagedAliasCount++;
            result.Report.AddLimited(result.Report.SnipeEngagedAliases,
                $"{power.FullName}: highlight_expression kEngaged -> FastSnipe", 500);
        }

        AddIgnoredField(power.FullName, "highlight_expression", power.HighlightExpression, result);

        if (!string.IsNullOrWhiteSpace(power.MaxTargetsExpression))
        {
            result.Report.DynamicMaxTargetExpressions++;
            result.Report.AddLimited(result.Report.DynamicExpressionsNotEvaluated,
                $"{power.FullName} max_targets_expression: {power.MaxTargetsExpression}");
        }

        if (!string.IsNullOrWhiteSpace(power.ChainEffectExpression))
        {
            result.Report.ChainExpressions++;
            result.Report.AddLimited(result.Report.DynamicExpressionsNotEvaluated,
                $"{power.FullName} chain_effect_expr: {power.ChainEffectExpression}");
        }

        if (!string.IsNullOrWhiteSpace(power.ChainTargetExpression))
        {
            result.Report.ChainExpressions++;
            result.Report.AddLimited(result.Report.DynamicExpressionsNotEvaluated,
                $"{power.FullName} chain_target_expr: {power.ChainTargetExpression}");
        }

        foreach (var redirect in power.Redirects)
        {
            result.Report.RedirectRules++;
            if (string.IsNullOrWhiteSpace(redirect.Name))
            {
                result.Report.AddLimited(result.Report.UnresolvedRedirects, $"{power.FullName} has redirect with no destination.");
            }

            if (!string.IsNullOrWhiteSpace(redirect.Requires))
            {
                TrackExpression(power.FullName, redirect.Requires, result, AdvancedConditionEvaluationMode.ReportOnly);
            }
        }

        foreach (var effect in power.Effects)
        {
            ScanEffect(power.FullName, effect, result);
        }
    }

    private static bool LooksLikePlannerModeTargetRequires(string expression)
    {
        return !string.IsNullOrWhiteSpace(expression) &&
               (expression.Contains("source.Mode?", StringComparison.OrdinalIgnoreCase) ||
                expression.Contains("source>mode", StringComparison.OrdinalIgnoreCase) ||
                expression.Contains("source.mode", StringComparison.OrdinalIgnoreCase));
    }

    private static void TrackPvTargetGating(
        string powerFullName,
        OmniEffectDefinition effect,
        OmniEffectTemplate template,
        OmniImportResult result)
    {
        var targetsPlayer = TargetsEntity(effect.RequiresExpression, "player") || TargetsEntity(template.JitRequires, "player");
        var targetsCritter = TargetsEntity(effect.RequiresExpression, "critter") || TargetsEntity(template.JitRequires, "critter");
        var table = template.Table ?? string.Empty;
        var pvMode = "Any";
        var inference = string.Empty;

        if (targetsPlayer && !targetsCritter)
        {
            pvMode = "PvP";
            inference = "target entity expression";
            result.Report.PvModeInferredFromTargetEntityCount++;
        }
        else if (targetsCritter && !targetsPlayer)
        {
            pvMode = "PvE";
            inference = "target entity expression";
            result.Report.PvModeInferredFromTargetEntityCount++;
        }
        else if (targetsPlayer && targetsCritter)
        {
            inference = "ambiguous target entity expression";
            result.Report.PvModeAmbiguousCount++;
        }
        else if (table.Contains("pvp", StringComparison.OrdinalIgnoreCase))
        {
            pvMode = "PvP";
            inference = "modifier table";
            result.Report.PvModeInferredFromTableCount++;
        }
        else if (table.Contains("pve", StringComparison.OrdinalIgnoreCase))
        {
            pvMode = "PvE";
            inference = "modifier table";
            result.Report.PvModeInferredFromTableCount++;
        }

        if ((targetsPlayer && pvMode != "PvP") || (targetsCritter && pvMode != "PvE"))
        {
            result.Report.PvTargetMappingMismatchCount++;
            result.Report.AddLimited(result.Report.PvTargetMappingMismatches,
                $"{powerFullName}: target_enttype player={targetsPlayer} critter={targetsCritter}, inferred PvMode={pvMode}, template.target='{template.Target}', requires='{effect.RequiresExpression}', jit='{template.JitRequires}'");
        }

        if (!IsFocusedPvTargetAuditPower(powerFullName))
        {
            return;
        }

        result.Report.PvTargetAuditCount++;
        var rows = OmniExpressionConverter.ToConditionSet(
            AdvancedConditionEvaluationMode.ReportOnly,
            powerFullName,
            effect.RequiresExpression,
            template.JitRequires);
        result.Report.AddLimited(result.Report.PvTargetGatingAudit,
            $"{powerFullName}: template.target='{template.Target}', requires='{effect.RequiresExpression}', jit='{template.JitRequires}', inferred PvMode={pvMode} ({inference}), conditions={FormatConditionRows(rows)}");
    }

    private static bool IsFocusedPvTargetAuditPower(string fullName)
    {
        var normalized = (fullName ?? string.Empty)
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace(".", string.Empty, StringComparison.Ordinal)
            .ToLowerInvariant();
        return normalized.Contains("enflame", StringComparison.OrdinalIgnoreCase) ||
               normalized.Contains("rainofarrows", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TargetsEntity(string expression, string entity)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            return false;
        }

        var escaped = Regex.Escape(entity);
        return Regex.IsMatch(
                   expression,
                   $@"target\s*>\s*enttype\s*(?:eq|==)\s*['""]?{escaped}['""]?",
                   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant) ||
               Regex.IsMatch(
                   expression,
                   $@"['""]?{escaped}['""]?\s+target\s*>\s*enttype\s*(?:eq|==)",
                   RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    private static string FormatConditionRows(AdvancedConditionSet conditions)
    {
        if (conditions.Rows.Count == 0)
        {
            return "none";
        }

        return string.Join("; ", conditions.Rows.Select(row =>
            $"{row.Link}:{row.Kind}:{row.Subject}{row.Operator}{row.Value}:mode={row.EvaluationMode}:unsupported={row.Unsupported}"));
    }

    private static void ScanEffect(string powerFullName, OmniEffectDefinition effect, OmniImportResult result)
    {
        result.Report.EffectsVisited++;
        TrackEffectGroupTags(powerFullName, effect.Tags, result);
        if (!string.IsNullOrWhiteSpace(effect.RequiresExpression))
        {
            result.Report.EffectRequiresExpressions++;
            TrackExpression(powerFullName, effect.RequiresExpression, result, AdvancedConditionEvaluationMode.ReportOnly);
        }

        foreach (var template in effect.Templates)
        {
            result.Report.EffectTemplatesVisited++;
            TrackPvTargetGating(powerFullName, effect, template, result);
            TrackTemplateFilterTags(powerFullName, template, result);
            if (!string.IsNullOrWhiteSpace(template.JitRequires))
            {
                result.Report.JitRequiresExpressions++;
                TrackExpression(powerFullName, template.JitRequires, result, AdvancedConditionEvaluationMode.ReportOnly);
            }

            if (template.Params != null)
            {
                result.Report.ParamsVisited++;
                var paramType = template.Params.Value<string>("type") ?? string.Empty;
                if (paramType.Equals("EntCreate", StringComparison.OrdinalIgnoreCase))
                {
                    result.Report.EntCreateParams++;
                    var entityDef = template.Params.Value<string>("entity_def") ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(entityDef))
                    {
                        result.Report.PseudoEntCreatesWithoutEntityDef++;
                        result.Report.PseudoPetActors++;
                        result.Report.AddLimited(result.Report.PseudoPetClassifications,
                            $"{powerFullName}: EntCreate has no entity_def; treating as flattened pseudo/delivery actor.");
                    }
                    else
                    {
                        result.ReferencedEntities.Add(entityDef);
                    }
                }
                else if (paramType.Equals("EffectFilter", StringComparison.OrdinalIgnoreCase))
                {
                    TrackUnsupportedEffectFilterParams(powerFullName, template.Params, result);
                }
            }

            foreach (var attrib in template.Attribs)
            {
                if (string.IsNullOrWhiteSpace(attrib))
                {
                    result.Report.AddLimited(result.Report.UnknownAttribMappings, $"{powerFullName} has blank attrib.");
                }
            }

            if (template.Attribs.Any(attrib =>
                    attrib.Equals("Set_Mode", StringComparison.OrdinalIgnoreCase) ||
                    attrib.Equals("Unset_Mode", StringComparison.OrdinalIgnoreCase)))
            {
                var rawModeName = template.ModeName ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(rawModeName))
                {
                    var normalized = OmniModeMapper.Normalize(rawModeName);
                    result.Report.EffectModePayloadMappedCount++;
                    result.Report.AddLimited(result.Report.EffectModePayloads, $"{powerFullName}: {rawModeName} -> {normalized}");
                    if (OmniModeMapper.TryGetPlannerMode(normalized, out var plannerMode))
                    {
                        result.Report.PlannerModeDiscoveredCount++;
                        result.Report.AddLimited(result.Report.PlannerModesDiscovered,
                            $"{powerFullName}: {normalized} -> {PlannerModeMapper.ToCanonicalName(plannerMode)}", 500);
                    }
                }
                else
                {
                    var modeId = (int)MathF.Round(template.Magnitude);
                    if (OmniModeMapper.TryFromModeId(modeId, out var mappedName, out var mappedFlag))
                    {
                        result.Report.EffectModePayloadMappedCount++;
                        result.Report.AddLimited(result.Report.EffectModePayloads,
                            $"{powerFullName}: mode id {modeId} -> {mappedName} ({mappedFlag})");
                        if (OmniModeMapper.TryGetPlannerMode(mappedName, out var plannerMode))
                        {
                            result.Report.PlannerModeDiscoveredCount++;
                            result.Report.AddLimited(result.Report.PlannerModesDiscovered,
                                $"{powerFullName}: mode id {modeId} -> {PlannerModeMapper.ToCanonicalName(plannerMode)}", 500);
                        }
                    }
                    else
                    {
                        result.Report.UnknownEffectModeCount++;
                        result.Report.AddLimited(result.Report.UnknownEffectModes,
                            $"{powerFullName}: {string.Join(",", template.Attribs)} mode id {modeId}");
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(template.Type))
            {
                result.Report.AddLimited(result.Report.UnknownEffectMappings, $"{powerFullName} template has no type.");
            }

            if (!string.IsNullOrWhiteSpace(template.Table) && !CanonicalModifierTableExists(result, template.Table))
            {
                result.Report.MissingModifierTableReferences++;
                result.Report.AddLimited(result.Report.MissingModifierTableReferenceDetails,
                    $"{powerFullName}: modifier table '{template.Table}' was not found in canonical class tables.", 500);
            }
        }

        foreach (var childEffect in effect.ChildEffects)
        {
            result.Report.ChildEffectsVisited++;
            if (!ChildEffectHasRepresentableSurface(childEffect))
            {
                result.Report.AddLimited(result.Report.ChildEffectsNotRepresented,
                    $"{powerFullName}: child effect has no templates, params, or nested child effects.");
            }

            ScanEffect(powerFullName, childEffect, result);
        }
    }

    private static bool ChildEffectHasRepresentableSurface(OmniEffectDefinition effect)
    {
        return effect.Templates.Any(template =>
                   !string.IsNullOrWhiteSpace(template.Type) ||
                   template.Attribs.Count > 0 ||
                   template.Params != null) ||
               effect.ChildEffects.Count > 0;
    }

    private static bool CanonicalModifierTableExists(OmniImportResult result, string tableName)
    {
        var canonicalName = DatabaseAPI.NormalizeModifierTableName(tableName);
        if (result.ClassAttributes.Values.Any(classTable => classTable.NamedTables.ContainsKey(canonicalName)))
        {
            return true;
        }

        return DatabaseAPI.ModifierTableExists(canonicalName);
    }

    private static void TrackEffectGroupTags(string powerFullName, IEnumerable<string> tags, OmniImportResult result)
    {
        foreach (var tag in DistinctTags(tags))
        {
            result.Report.EffectGroupTagCount++;
            result.Report.AddLimited(result.Report.EffectGroupTags, $"{powerFullName}: {tag}", 500);
            TrackTagMissingFromGcm(powerFullName, tag, result);
        }
    }

    private static void TrackTemplateFilterTags(string powerFullName, OmniEffectTemplate template, OmniImportResult result)
    {
        if (!IsChanceModTemplate(template))
        {
            return;
        }

        var powerLocal = IsPowerLocalChanceModTemplate(template);
        var tags = GetEffectFilterTags(template).ToArray();
        if (tags.Length == 0)
        {
            TrackChanceModReport(result, powerFullName, powerLocal, string.Empty);
        }

        foreach (var tag in tags)
        {
            result.Report.TemplateFilterTagCount++;
            result.Report.AddLimited(result.Report.TemplateFilterTags, $"{powerFullName}: {tag}", 500);
            TrackTagMissingFromGcm(powerFullName, tag, result);
            TrackChanceModReport(result, powerFullName, powerLocal, tag);
        }
    }

    private static void TrackUnsupportedEffectFilterParams(
        string powerFullName,
        JObject parameters,
        OmniImportResult result)
    {
        var unsupportedKeys = new[]
        {
            "power_names",
            "power_name",
            "powers",
            "powerset_names",
            "powersets",
            "category_names",
            "categories"
        };
        var present = unsupportedKeys
            .Where(key => parameters.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var token) &&
                          token is { Type: not JTokenType.Null } &&
                          HasMeaningfulEffectFilterToken(token))
            .ToArray();
        if (present.Length == 0)
        {
            return;
        }

        result.Report.UnsupportedEffectFilterCount++;
        result.Report.AddLimited(result.Report.UnsupportedEffectFilters,
            $"{powerFullName}: EffectFilter has report-only filters {string.Join(", ", present)}", 500);
    }

    private static bool IsChanceModTemplate(OmniEffectTemplate template)
    {
        return IsGlobalChanceModTemplate(template) || IsPowerLocalChanceModTemplate(template);
    }

    private static bool IsGlobalChanceModTemplate(OmniEffectTemplate template)
    {
        return template.Attribs.Any(attrib =>
                   attrib.Equals("Global_Chance_Mod", StringComparison.OrdinalIgnoreCase) ||
                   attrib.Equals("GlobalChanceMod", StringComparison.OrdinalIgnoreCase)) ||
               template.Type.Equals("Global_Chance_Mod", StringComparison.OrdinalIgnoreCase) ||
               template.Type.Equals("GlobalChanceMod", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPowerLocalChanceModTemplate(OmniEffectTemplate template)
    {
        return template.Attribs.Any(attrib =>
                   attrib.Equals("Power_Chance_Mod", StringComparison.OrdinalIgnoreCase) ||
                   attrib.Equals("PowerChanceMod", StringComparison.OrdinalIgnoreCase)) ||
               template.Type.Equals("Power_Chance_Mod", StringComparison.OrdinalIgnoreCase) ||
               template.Type.Equals("PowerChanceMod", StringComparison.OrdinalIgnoreCase);
    }

    private static void TrackChanceModReport(
        OmniImportResult result,
        string powerFullName,
        bool powerLocal,
        string reward)
    {
        if (powerLocal)
        {
            result.Report.PowerLocalChanceModsMappedCount++;
        }
        else
        {
            result.Report.GlobalChanceModsMappedCount++;
        }

        result.Report.AddLimited(
            result.Report.ChanceModMappings,
            $"{powerFullName}: mapped {(powerLocal ? "power-local" : "global")} chance mod reward '{(string.IsNullOrWhiteSpace(reward) ? "*" : reward)}'");
    }

    private static IEnumerable<string> GetEffectFilterTags(OmniEffectTemplate template)
    {
        if (template.Params != null &&
            string.Equals(template.Params.Value<string>("type"), "EffectFilter", StringComparison.OrdinalIgnoreCase))
        {
            var paramTags = ReadStringArray(template.Params["tags"]).ToArray();
            if (paramTags.Length > 0)
            {
                return DistinctTags(paramTags);
            }
        }

        return DistinctTags(template.Tags);
    }

    private static IEnumerable<string> ReadStringArray(JToken? token)
    {
        if (token == null)
        {
            yield break;
        }

        if (token.Type == JTokenType.Array)
        {
            foreach (var value in token.Values<string>())
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    yield return value.Trim();
                }
            }

            yield break;
        }

        var scalar = token.Type == JTokenType.String
            ? token.ToString()
            : string.Empty;
        if (!string.IsNullOrWhiteSpace(scalar))
        {
            yield return scalar.Trim();
        }
    }

    private static bool HasMeaningfulEffectFilterToken(JToken? token)
    {
        return ReadStringArray(token).Any();
    }

    private static IEnumerable<string> DistinctTags(IEnumerable<string>? tags)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tag in tags ?? [])
        {
            var trimmed = tag.Trim();
            if (!string.IsNullOrWhiteSpace(trimmed) && seen.Add(trimmed))
            {
                yield return trimmed;
            }
        }
    }

    private static void TrackTagMissingFromGcm(string powerFullName, string tag, OmniImportResult result)
    {
        if (result.GcmTags.Count == 0 || result.GcmTags.Contains(tag))
        {
            return;
        }

        result.Report.EffectTagMissingFromGcmCount++;
        result.Report.AddLimited(result.Report.EffectTagsMissingFromGcm, $"{powerFullName}: {tag}", 500);
    }

    private void ScanEntities(
        string exportRoot,
        OmniExportManifest manifest,
        OmniImportResult result,
        IProgress<OmniImportProgress>? progress)
    {
        result.Report.EntityReferences = result.ReferencedEntities.Count;
        if (result.ReferencedEntities.Count == 0)
        {
            ReportStageProgress(progress, OmniImportStageId.ResolveEntities, "No referenced entities", markComplete: true);
            return;
        }

        var entitiesRoot = Path.Combine(exportRoot, "entities");
        if (!Directory.Exists(entitiesRoot))
        {
            result.Report.AddLimited(result.Report.UnresolvedEntities, "Missing entities directory.");
            return;
        }

        var entityFiles = manifest.EntityFiles
            .ToDictionary(
                f => NormalizeEntityKey(Path.GetFileNameWithoutExtension(f)),
                f => f,
                StringComparer.OrdinalIgnoreCase);
        result.CachedEntityFileLookup = new Dictionary<string, string>(entityFiles, StringComparer.OrdinalIgnoreCase);

        var entityRefs = result.ReferencedEntities.OrderBy(e => e, StringComparer.OrdinalIgnoreCase).ToList();
        for (var index = 0; index < entityRefs.Count; index++)
        {
            var entityRef = entityRefs[index];
            ReportStageProgress(
                progress,
                OmniImportStageId.ResolveEntities,
                string.Empty,
                index + 1,
                entityRefs.Count,
                markComplete: index + 1 >= entityRefs.Count);
            if (!entityFiles.TryGetValue(NormalizeEntityKey(entityRef), out var file))
            {
                result.Report.EntityReferencesUnresolved++;
                result.Report.AddLimited(result.Report.UnresolvedEntities,
                    $"{entityRef}: referenced by scoped EntCreate but no matching entity file was found.");
                continue;
            }

            var entity = ReadJson<OmniEntityDefinition>(file);
            if (entity == null)
            {
                result.Report.EntityReferencesUnresolved++;
                result.Report.AddLimited(result.Report.UnresolvedEntities,
                    $"{entityRef}: matching entity file could not be read.");
                continue;
            }

            result.Report.EntitiesRead++;
            var actor = OmniPetClassifier.Classify(entity);
            PrepareReferencedEntityActor(actor, result.Scope);
            var retainedClassName = ResolveRetainedEntityClassName(actor.ClassName);
            result.Scope.AddReferencedEntityId(actor.EntityName);
            result.Scope.AddReferencedEntityClass(retainedClassName);
            result.Scope.AddReferencedEntityPowersets(actor.Powersets);

            result.Report.EntityReferencesResolved++;
            result.Actors[actor.EntityName] = actor;
            if (actor.Kind == OmniBuildActorKind.RealPet)
            {
                result.Report.RealPetActors++;
            }
            else
            {
                result.Report.PseudoPetActors++;
                result.Report.AddLimited(result.Report.PseudoPetClassifications,
                    $"{actor.EntityName}: {actor.ClassificationReason}");
            }
        }
    }

    private static string NormalizeEntityKey(string value)
    {
        return new string((value ?? string.Empty)
            .Where(char.IsLetterOrDigit)
            .Select(char.ToLowerInvariant)
            .ToArray());
    }

    private static string ResolveRetainedEntityClassName(string className)
    {
        return NormalizeClassName(string.IsNullOrWhiteSpace(className) ? DefaultRetainedEntityClass : className);
    }

    private void ExpandReferencedEntityScope(string exportRoot, OmniExportManifest manifest, OmniImportResult result)
    {
        var powersRoot = Path.Combine(exportRoot, "powers");
        var entitiesRoot = Path.Combine(exportRoot, "entities");
        if (!Directory.Exists(powersRoot) || !Directory.Exists(entitiesRoot))
        {
            return;
        }

        var entityFiles = (manifest.EntityFiles.Count > 0 ? manifest.EntityFiles : EnumerateSortedFiles(entitiesRoot, "*.json", SearchOption.TopDirectoryOnly))
            .ToDictionary(
                file => NormalizeEntityKey(Path.GetFileNameWithoutExtension(file)),
                file => file,
                StringComparer.OrdinalIgnoreCase);
        var powersetIndexFiles = (manifest.PowersetIndexFiles.Count > 0
                ? manifest.PowersetIndexFiles
                : Directory.EnumerateFiles(powersRoot, "index.json", SearchOption.AllDirectories)
                    .Where(file => !IsCategoryRootIndex(Path.GetRelativePath(powersRoot, file))))
            .Select(file => new
            {
                File = file,
                Powerset = ReadJson<OmniPowersetDefinition>(file)
            })
            .Where(entry => entry.Powerset != null && !string.IsNullOrWhiteSpace(entry.Powerset.FullName))
            .GroupBy(entry => CanonicalizeOmniFullName(entry.Powerset!.FullName), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.First().File, StringComparer.OrdinalIgnoreCase);
        var powerFilesByDirectory = manifest.PowerFiles
            .GroupBy(file => Path.GetDirectoryName(file) ?? string.Empty, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group.OrderBy(file => file, StringComparer.OrdinalIgnoreCase).ToArray(),
                StringComparer.OrdinalIgnoreCase);

        var pendingPowersets = new Queue<string>(result.Scope.RetainedPowersets
            .Where(powerset => result.Scope.GetPowersetType(powerset) == Enums.ePowerSetType.Pet)
            .Select(CanonicalizeOmniFullName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase));
        var processedPowersets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var processedEntities = new HashSet<string>(result.Scope.RetainedEntityIds
            .Select(NormalizeEntityKey), StringComparer.OrdinalIgnoreCase);

        while (pendingPowersets.Count > 0)
        {
            var canonicalPowerset = pendingPowersets.Dequeue();
            if (!processedPowersets.Add(canonicalPowerset) ||
                !powersetIndexFiles.TryGetValue(canonicalPowerset, out var indexFile))
            {
                continue;
            }

            var setDirectory = Path.GetDirectoryName(indexFile);
            if (string.IsNullOrWhiteSpace(setDirectory))
            {
                continue;
            }

            if (!powerFilesByDirectory.TryGetValue(setDirectory, out var setPowerFiles))
            {
                continue;
            }

            foreach (var file in setPowerFiles)
            {
                var power = ReadJson<OmniPowerDefinition>(file);
                if (power == null)
                {
                    continue;
                }

                foreach (var entityRef in GetEntCreateEntityRefs(power))
                {
                    var normalizedEntityKey = NormalizeEntityKey(entityRef);
                    if (!processedEntities.Add(normalizedEntityKey) ||
                        !entityFiles.TryGetValue(normalizedEntityKey, out var entityFile))
                    {
                        continue;
                    }

                    var entity = ReadJson<OmniEntityDefinition>(entityFile);
                    if (entity == null)
                    {
                        continue;
                    }

                    var actor = OmniPetClassifier.Classify(entity, power.FullName);
                    PrepareReferencedEntityActor(actor, result.Scope);
                    var retainedClassName = ResolveRetainedEntityClassName(actor.ClassName);
                    result.Scope.AddReferencedEntityId(actor.EntityName);
                    result.Scope.AddReferencedEntityClass(retainedClassName);
                    result.Scope.AddReferencedEntityPowersets(actor.Powersets);
                    result.Actors[actor.EntityName] = actor;

                    foreach (var actorPowerset in actor.Powersets
                                 .Where(powerset => result.Scope.GetPowersetType(powerset) == Enums.ePowerSetType.Pet)
                                 .Select(CanonicalizeOmniFullName))
                    {
                        if (!processedPowersets.Contains(actorPowerset))
                        {
                            pendingPowersets.Enqueue(actorPowerset);
                        }
                    }
                }
            }
        }
    }

    private void BackfillRetainedEntityClassSummaries(string exportRoot, OmniImportResult result)
    {
        var missingClassNames = result.Scope.RetainedClassNames
            .Select(NormalizeClassName)
            .Where(className => !string.IsNullOrWhiteSpace(className) && !result.ClassAttributes.ContainsKey(className))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (missingClassNames.Count == 0)
        {
            return;
        }

        var archetypeRoot = Path.Combine(exportRoot, "archetypes");
        if (!Directory.Exists(archetypeRoot))
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(archetypeRoot, "*.json", SearchOption.TopDirectoryOnly))
        {
            if (missingClassNames.Count == 0)
            {
                break;
            }

            var archetype = ReadJson<OmniArchetypeDefinition>(file);
            if (archetype == null)
            {
                continue;
            }

            var className = NormalizeClassName(archetype.InternalName);
            if (!missingClassNames.Remove(className))
            {
                continue;
            }

            var table = BuildClassAttributeTable(archetype);
            result.ClassAttributes[table.ClassName] = table;
            result.Report.ClassAttributeTablesImported++;
            result.Report.RetainedArchetypesRead++;
            if (result.Report.SkippedArchetypesRead > 0)
            {
                result.Report.SkippedArchetypesRead--;
            }

            result.Report.SkippedArchetypes.RemoveAll(entry =>
                entry.StartsWith($"{className}:", StringComparison.OrdinalIgnoreCase));
            result.Report.AddLimited(
                result.Report.RetainedArchetypes,
                $"{className}: retained because scoped summoned-entity content depends on this class.");
        }
    }

    private void ScanEnhancementData(
        string exportRoot,
        OmniExportManifest manifest,
        OmniImportResult result,
        IProgress<OmniImportProgress>? progress)
    {
        ReportStageProgress(progress, OmniImportStageId.NormalizeEnhancementInputs);
        var report = result.Report;
        var normalizedData = LoadNormalizedEnhancementImportData(exportRoot, manifest, progress);
        result.CachedNormalizedEnhancementData = normalizedData;
        var enhancementDirs = new[]
        {
            "enhancements",
            "enhancement_sets",
            "salvage"
        };

        foreach (var directoryName in enhancementDirs)
        {
            var directory = Path.Combine(exportRoot, directoryName);
            if (!Directory.Exists(directory))
            {
                report.AddLimited(report.IgnoredFields, $"Missing optional {directoryName} directory.");
            }
        }

        if (!Directory.Exists(Path.Combine(exportRoot, "recipes")) &&
            !Directory.Exists(Path.Combine(exportRoot, "base_recipes")))
        {
            report.AddLimited(report.IgnoredFields, "Missing optional recipes/ or base_recipes/ directory.");
        }

        var policyFiles = new[]
        {
            "enhancement_diversification.json",
            "enhancement_effectiveness.json",
            "enhancement_exemplar_scaling.json",
            "enhancement_set_groups.json",
            "set_bonuses.json",
            "set_conversions.json"
        };

        foreach (var fileName in policyFiles)
        {
            if (File.Exists(Path.Combine(exportRoot, fileName)))
            {
                report.EnhancementPolicyFilesPresent++;
            }
            else
            {
                report.EnhancementPolicyFilesMissing++;
                report.AddLimited(report.EnhancementMissingPolicyFiles, $"Missing optional {fileName}.");
                report.AddLimited(report.IgnoredFields, $"Missing optional {fileName}.");
            }
        }

        report.EnhancementRecipeSourceDirectory = normalizedData.RecipeSourceDirectoryName;
        report.EnhancementDefinitionsDiscovered = normalizedData.EnhancementSourceRecordsDiscovered;
        report.EnhancementSetsDiscovered = normalizedData.EnhancementSets.Count;
        report.EnhancementRecordsExcludedByPolicy = normalizedData.EnhancementRecordsExcludedByPolicy;
        report.ClassicEnhancementSourceVariantsDiscovered = normalizedData.ClassicEnhancementSourceVariantsDiscovered;
        report.ClassicEnhancementLogicalRecords = normalizedData.ClassicEnhancementLogicalRecords;
        report.ClassicEnhancementVariantsFolded = normalizedData.ClassicEnhancementVariantsFolded;
        report.RecipeLevelFilesDiscovered = normalizedData.Recipes.Count;
        report.FoldedEnhancementRecipeCount = normalizedData.Recipes.Count;
        report.RecipeLevelVariantsDiscovered = normalizedData.RecipeLevelVariantsDiscovered;
        report.RecipeRecordsExcludedByPolicy = normalizedData.RecipeRecordsExcludedByPolicy;
        report.SalvageDefinitionsDiscovered = normalizedData.Salvage.Count;
        report.EnhancementLevelVariantsDiscovered = normalizedData.Enhancements.Sum(definition => definition.LevelVariants.Count);
        report.StructuredBoostsAllowedParsedCount = normalizedData.StructuredBoostsAllowedParsedCount;
        report.EnhancementShapeCompatibilityFallbacks = normalizedData.ShapeCompatibilityFallbackCount;
        report.InventionCraftedVariantsDiscovered = normalizedData.InventionCraftedVariantsDiscovered;
        report.InventionAttunedVariantsDiscovered = normalizedData.InventionAttunedVariantsDiscovered;
        report.InventionSuperiorVariantsDiscovered = normalizedData.InventionSuperiorVariantsDiscovered;
        report.InventionSuperiorAttunedVariantsDiscovered = normalizedData.InventionSuperiorAttunedVariantsDiscovered;
        report.InventionStandaloneCraftedVariantsDiscovered = normalizedData.InventionStandaloneCraftedVariantsDiscovered;
        report.InventionStandaloneAttunedVariantsDiscovered = normalizedData.InventionStandaloneAttunedVariantsDiscovered;
        report.InventionStandaloneSuperiorVariantsDiscovered = normalizedData.InventionStandaloneSuperiorVariantsDiscovered;
        report.InventionStandaloneSuperiorAttunedVariantsDiscovered = normalizedData.InventionStandaloneSuperiorAttunedVariantsDiscovered;
        report.IoSetFamiliesCraftedOnly = normalizedData.IoSetFamiliesCraftedOnly;
        report.IoSetFamiliesAttunedOnly = normalizedData.IoSetFamiliesAttunedOnly;
        report.IoSetFamiliesSuperiorOnly = normalizedData.IoSetFamiliesSuperiorOnly;
        report.IoSetFamiliesSuperiorAttunedOnly = normalizedData.IoSetFamiliesSuperiorAttunedOnly;
        report.IoSetFamiliesCraftedAndAttuned = normalizedData.IoSetFamiliesCraftedAndAttuned;
        report.IoSetFamiliesCraftedAndSuperior = normalizedData.IoSetFamiliesCraftedAndSuperior;
        report.IoSetFamiliesCraftedAndSuperiorAttuned = normalizedData.IoSetFamiliesCraftedAndSuperiorAttuned;
        report.IoSetFamiliesAttunedAndSuperior = normalizedData.IoSetFamiliesAttunedAndSuperior;
        report.IoSetFamiliesAttunedAndSuperiorAttuned = normalizedData.IoSetFamiliesAttunedAndSuperiorAttuned;
        report.IoSetFamiliesSuperiorAndSuperiorAttuned = normalizedData.IoSetFamiliesSuperiorAndSuperiorAttuned;
        report.IoSetFamiliesCraftedAttunedSuperior = normalizedData.IoSetFamiliesCraftedAttunedSuperior;
        report.IoSetFamiliesCraftedAttunedSuperiorAttuned = normalizedData.IoSetFamiliesCraftedAttunedSuperiorAttuned;
        report.IoSetFamiliesCraftedSuperiorSuperiorAttuned = normalizedData.IoSetFamiliesCraftedSuperiorSuperiorAttuned;
        report.IoSetFamiliesAttunedSuperiorSuperiorAttuned = normalizedData.IoSetFamiliesAttunedSuperiorSuperiorAttuned;
        report.IoSetFamiliesAllFourVariants = normalizedData.IoSetFamiliesAllFourVariants;
        report.EnhancementMalformedRecordsSkipped =
            normalizedData.EnhancementMalformedRecordsSkipped +
            normalizedData.EnhancementSetMalformedRecordsSkipped +
            normalizedData.RecipeMalformedRecordsSkipped +
            normalizedData.SalvageMalformedRecordsSkipped;
        foreach (var detail in normalizedData.ShapeValidationDetails)
        {
            report.AddLimited(report.EnhancementSourceShapeValidation, detail);
        }
        foreach (var detail in normalizedData.ClassicEnhancementFoldDetails)
        {
            report.AddLimited(report.ClassicEnhancementFoldingDetails, detail);
        }
        foreach (var detail in normalizedData.InventionVariantAuditDetails)
        {
            report.AddLimited(report.InventionVariantAudit, detail);
        }

        report.AddLimited(report.EnhancementImportScanDetails,
            $"Classic variants discovered/logical/folded: {normalizedData.ClassicEnhancementSourceVariantsDiscovered}/{normalizedData.ClassicEnhancementLogicalRecords}/{normalizedData.ClassicEnhancementVariantsFolded}");
        report.AddLimited(report.EnhancementImportScanDetails,
            $"Invention variants discovered crafted/attuned/superior/superior-attuned: {normalizedData.InventionCraftedVariantsDiscovered}/{normalizedData.InventionAttunedVariantsDiscovered}/{normalizedData.InventionSuperiorVariantsDiscovered}/{normalizedData.InventionSuperiorAttunedVariantsDiscovered}");
        report.AddLimited(report.EnhancementImportScanDetails,
            $"IO set family coverage crafted-only/attuned-only/superior-attuned-only/crafted+attuned/crafted+superior-attuned: {normalizedData.IoSetFamiliesCraftedOnly}/{normalizedData.IoSetFamiliesAttunedOnly}/{normalizedData.IoSetFamiliesSuperiorAttunedOnly}/{normalizedData.IoSetFamiliesCraftedAndAttuned}/{normalizedData.IoSetFamiliesCraftedAndSuperiorAttuned}");
        ReportStageProgress(progress, OmniImportStageId.NormalizeEnhancementInputs, $"{normalizedData.Enhancements.Count:n0} enhancement records normalized", markComplete: true);

        foreach (var definition in normalizedData.Enhancements)
        {
            report.AddLimited(report.EnhancementImportScanDetails,
                $"{definition.Name}: type={definition.EnhancementType}, family={definition.EnhancementFamily}, levels={definition.LevelMin + 1}-{definition.LevelMax + 1}");

            if (!string.IsNullOrWhiteSpace(definition.PowerFullName))
            {
                if (result.ScopedPowerFullNames.Contains(definition.PowerFullName))
                {
                    report.EnhancementBoostPowerLinksResolvedDryRun++;
                }
                else
                {
                    report.EnhancementBoostPowerLinksMissingDryRun++;
                    report.AddLimited(report.EnhancementPowerLinkAudit,
                        $"{definition.Name}: boost power '{definition.PowerFullName}' is missing from the scanned Omni power scope.");
                }
            }
        }

        foreach (var definition in normalizedData.EnhancementSets)
        {
            report.AddLimited(report.EnhancementImportScanDetails,
                $"{definition.Name}: set group={definition.GroupName}, boost lists={definition.BoostLists.Count}");
        }

        foreach (var recipe in normalizedData.Recipes)
        {
            if (result.ScopedPowerFullNames.Contains(recipe.EnhancementReward))
            {
                report.RecipeRewardLinksResolvedDryRun++;
            }
            else
            {
                report.RecipeRewardLinksMissingDryRun++;
                report.AddLimited(report.EnhancementPowerLinkAudit,
                    $"{recipe.Name}: recipe reward power '{recipe.EnhancementReward}' is missing from the scanned Omni power scope.");
            }
        }

        foreach (var setBonusSet in normalizedData.SetBonusDefinitions.Values)
        {
            foreach (var bonus in setBonusSet.Bonuses)
            {
                foreach (var powerName in bonus.AutoPowers
                             .Concat(string.IsNullOrWhiteSpace(bonus.BonusPower) ? [] : [bonus.BonusPower])
                             .Where(value => !string.IsNullOrWhiteSpace(value))
                             .Select(CanonicalizeOmniFullName)
                             .Distinct(StringComparer.OrdinalIgnoreCase))
                {
                    if (result.ScopedPowerFullNames.Contains(powerName))
                    {
                        report.EnhancementSetBonusLinksResolvedDryRun++;
                    }
                    else
                    {
                        report.EnhancementSetBonusLinksMissingDryRun++;
                        report.AddLimited(report.EnhancementPowerLinkAudit,
                            $"{setBonusSet.Name}: set bonus power '{powerName}' is missing from the scanned Omni power scope.");
                    }
                }
            }
        }

        InspectEnhancementReconciliationDryRun(normalizedData, report);
    }

    private T? TryReadJson<T>(string path) where T : class
    {
        if (!File.Exists(path))
        {
            return null;
        }

        return ReadJson<T>(path);
    }

    private static string FormatLevelBand(OmniEnhancementDefinition definition)
    {
        if (definition.LevelVariants.Count > 0)
        {
            return $"{definition.LevelVariants.Min()}-{definition.LevelVariants.Max()} ({definition.LevelVariants.Count} variants)";
        }

        return $"{definition.MinLevel}-{definition.MaxLevel}";
    }

    private static OmniClassAttributeTable BuildClassAttributeTable(OmniArchetypeDefinition archetype)
    {
        var table = new OmniClassAttributeTable
        {
            ClassName = NormalizeClassName(archetype.InternalName),
            DisplayName = archetype.DisplayName,
            PrimaryCategory = archetype.PrimaryCategory,
            SecondaryCategory = archetype.SecondaryCategory,
            Playable = archetype.Playable
        };

        if (archetype.ExtensionData == null)
        {
            return table;
        }

        table.Base = ReadFloatMap(archetype.ExtensionData, "attrib_base");
        table.Min = ReadFloatMap(archetype.ExtensionData, "attrib_min");
        table.Max = ReadFloatArrayMap(archetype.ExtensionData, "attrib_max");
        table.MaxMax = ReadFloatArrayMap(archetype.ExtensionData, "attrib_max_max");
        table.StrengthMin = ReadFloatMap(archetype.ExtensionData, "attrib_strength_min");
        table.StrengthMax = ReadFloatArrayMap(archetype.ExtensionData, "attrib_strength_max");
        table.ResistanceMin = ReadFloatMap(archetype.ExtensionData, "attrib_resistance_min");
        table.ResistanceMax = ReadFloatArrayMap(archetype.ExtensionData, "attrib_resistance_max");
        table.DiminishingStrength = ReadFloatMap(archetype.ExtensionData, "attrib_diminishing_str");
        table.DiminishingCurrent = ReadFloatMap(archetype.ExtensionData, "attrib_diminishing_cur");
        table.NamedTables = new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);

        return table;
    }

    private static void ApplyLegacyArchetypeSummary(Archetype archetype, OmniClassAttributeTable table)
    {
        archetype.DisplayName = string.IsNullOrWhiteSpace(table.DisplayName) ? archetype.DisplayName : table.DisplayName;
        archetype.PrimaryGroup = string.IsNullOrWhiteSpace(table.PrimaryCategory) ? archetype.PrimaryGroup : table.PrimaryCategory;
        archetype.SecondaryGroup = string.IsNullOrWhiteSpace(table.SecondaryCategory) ? archetype.SecondaryGroup : table.SecondaryCategory;
        archetype.Playable = table.Playable;

        archetype.Hitpoints = (int)Math.Round(table.GetMax("hit_points", 50) ?? archetype.Hitpoints);
        archetype.HPCap = table.GetMaxMax("hit_points", 50) ?? archetype.HPCap;
        archetype.PerceptionCap = table.GetMaxMax("perception_radius", 50) ?? archetype.PerceptionCap;
        archetype.BaseRegen = table.GetBase("regeneration") ?? archetype.BaseRegen;
        archetype.BaseRecovery = table.GetBase("recovery") ?? archetype.BaseRecovery;
        archetype.BaseThreat = table.GetBase("threat_level") ?? archetype.BaseThreat;

        archetype.RegenCap = table.GetMax("regeneration", 50) ?? archetype.RegenCap;
        archetype.RecoveryCap = table.GetMax("recovery", 50) ?? archetype.RecoveryCap;
        archetype.RechargeCap = table.GetMax("recharge_time", 50) ?? archetype.RechargeCap;
        archetype.ResCap = table.GetMax("damage_resistance", 50) ?? archetype.ResCap;
        archetype.DamageCap = table.GetMax("damage", 50) ?? archetype.DamageCap;
    }

    private static Dictionary<string, float> ReadFloatMap(IDictionary<string, JToken> data, string key)
    {
        var obj = FirstObject(data, key);
        return obj?.Properties()
                   .Where(p => p.Value.Type is JTokenType.Float or JTokenType.Integer)
                   .ToDictionary(p => p.Name, p => p.Value.Value<float>(), StringComparer.OrdinalIgnoreCase)
               ?? new Dictionary<string, float>(StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, float[]> ReadFloatArrayMap(IDictionary<string, JToken> data, string key)
    {
        var obj = FirstObject(data, key);
        return obj?.Properties()
                   .Where(p => p.Value is JArray)
                   .ToDictionary(p => p.Name, p => p.Value.Values<float>().ToArray(), StringComparer.OrdinalIgnoreCase)
               ?? new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
    }

    private static Dictionary<string, float[]> ReadNamedTables(IDictionary<string, JToken> data, string key)
    {
        if (!data.TryGetValue(key, out var token) || token is not JObject obj)
        {
            return new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
        }

        return ReadNamedTables(obj, key);
    }

    private static Dictionary<string, float[]> ReadNamedTables(JObject data, string key)
    {
        if (!data.TryGetValue(key, StringComparison.OrdinalIgnoreCase, out var token) || token is not JObject obj)
        {
            return new Dictionary<string, float[]>(StringComparer.OrdinalIgnoreCase);
        }

        return obj.Properties()
            .Where(p => p.Value is JArray)
            .ToDictionary(p => p.Name, p => p.Value.Values<float>().ToArray(), StringComparer.OrdinalIgnoreCase);
    }

    private static JObject? FirstObject(IDictionary<string, JToken> data, string key)
    {
        if (!data.TryGetValue(key, out var token))
        {
            return null;
        }

        if (token is JArray { Count: > 0 } array && array[0] is JObject first)
        {
            return first;
        }

        return token as JObject;
    }

    private T? ReadJson<T>(string path) where T : class
    {
        try
        {
            using var stream = File.OpenRead(path);
            using var streamReader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(streamReader);
            var serializer = JsonSerializer.Create(_settings);
            return serializer.Deserialize<T>(jsonReader);
        }
        catch
        {
            return null;
        }
    }

    private static void TrackPowerRequirement(string owner, string expression, OmniImportResult result)
    {
        var conversion = OmniExpressionConverter.ConvertPowerRequirement(expression);
        TrackConditionSet(owner, expression, conversion.Requirements, result);

        foreach (var row in conversion.ReportOnlyFragments.Rows)
        {
            result.Report.ReportOnlyPowerRequirementFragmentCount++;
            result.Report.AddLimited(result.Report.ReportOnlyPowerRequirementFragments, $"{owner}: {row.RawExpression}", 500);
            result.Report.ReportOnlyExpressionCount++;
            result.Report.AddLimited(result.Report.ReportOnlyExpressions, $"{owner}: {row.RawExpression}", 500);
        }

        if (!conversion.Success)
        {
            result.Report.UnsupportedPowerRequirementCount++;
            result.Report.AddLimited(result.Report.UnsupportedPowerRequirements, $"{owner}: {expression}");
        }
    }

    private static void TrackExpression(
        string owner,
        string expression,
        OmniImportResult result,
        AdvancedConditionEvaluationMode fallbackMode)
    {
        var set = OmniExpressionConverter.ToConditionSet(fallbackMode, owner, expression);
        TrackConditionSet(owner, expression, set, result);
    }

    private static void TrackConditionSet(
        string owner,
        string expression,
        AdvancedConditionSet set,
        OmniImportResult result)
    {
        foreach (var row in set.Rows)
        {
            TrackConditionMode(owner, row, result);
            switch (row.EvaluationMode)
            {
                case AdvancedConditionEvaluationMode.BuildEvaluated:
                    result.Report.BuildEvaluatedExpressionCount++;
                    result.Report.AddLimited(result.Report.BuildEvaluatedExpressions, $"{owner}: {row.RawExpression}", 500);
                    if (row.Kind == AdvancedConditionKind.SourceMode &&
                        row.Subject.Equals("FastSnipe", StringComparison.OrdinalIgnoreCase) &&
                        row.RawExpression.Contains("kEngaged", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Report.SnipeEngagedAliasCount++;
                        result.Report.AddLimited(result.Report.SnipeEngagedAliases,
                            $"{owner}: {row.RawExpression} -> FastSnipe", 500);
                    }

                    if (row.Kind == AdvancedConditionKind.SourceMode &&
                        OmniModeMapper.TryGetPlannerMode(row.Subject, out var plannerMode))
                    {
                        result.Report.PlannerModeDiscoveredCount++;
                        result.Report.AddLimited(result.Report.PlannerModesDiscovered,
                            $"{owner}: {row.Subject} -> {PlannerModeMapper.ToCanonicalName(plannerMode)}", 500);
                    }

                    break;
                case AdvancedConditionEvaluationMode.RuntimeTargetOnly:
                    result.Report.RuntimeTargetExpressionCount++;
                    result.Report.AddLimited(result.Report.RuntimeTargetExpressions, $"{owner}: {row.RawExpression}", 500);
                    break;
                case AdvancedConditionEvaluationMode.ReportOnly:
                    result.Report.ReportOnlyExpressionCount++;
                    result.Report.AddLimited(result.Report.ReportOnlyExpressions, $"{owner}: {row.RawExpression}", 500);
                    break;
            }
        }

        if (set.Rows.Any(r => r.Kind == AdvancedConditionKind.AdvancedExpression || r.Unsupported))
        {
            result.Report.RawAdvancedExpressionCount++;
            result.Report.AddLimited(result.Report.RawAdvancedExpressions, $"{owner}: {expression}");
        }

        var unsupportedBuildRows = set.Rows
            .Where(r => r.Kind == AdvancedConditionKind.AdvancedExpression &&
                        r.Unsupported &&
                        r.EvaluationMode == AdvancedConditionEvaluationMode.BuildEvaluated)
            .ToList();
        foreach (var row in unsupportedBuildRows)
        {
            var rawExpression = string.IsNullOrWhiteSpace(row.RawExpression)
                ? expression
                : row.RawExpression;
            result.Report.UnsupportedBuildExpressionCount++;
            result.Report.AddLimited(result.Report.UnsupportedBuildExpressions, $"{owner}: {rawExpression}");

            foreach (var token in OmniExpressionConverter.FindUnknownTokens(rawExpression))
            {
                result.Report.UnknownExpressionTokenCount++;
                result.Report.AddLimited(result.Report.UnknownExpressionTokens, $"{owner}: {token}");
            }
        }
    }

    private static void TrackConditionMode(string owner, AdvancedConditionRow row, OmniImportResult result)
    {
        if (row.Kind is not (AdvancedConditionKind.SourceMode or AdvancedConditionKind.TargetMode) ||
            string.IsNullOrWhiteSpace(row.Subject))
        {
            return;
        }

        var normalized = OmniModeMapper.Normalize(row.Subject);
        if (!IsKnownMode(normalized))
        {
            return;
        }

        result.Report.ConditionModeMappedCount++;
        var subject = row.Kind == AdvancedConditionKind.TargetMode ? "target" : "source";
        var suffix = row.Negated ? " not" : string.Empty;
        result.Report.AddLimited(result.Report.ConditionModes,
            $"{owner}: {subject} mode{suffix} {row.Subject} -> {normalized}", 500);
    }

    private static void AddIgnoredField(string owner, string fieldName, string value, OmniImportResult result)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            result.Report.AddLimited(result.Report.IgnoredFields, $"{owner} {fieldName}: {value}");
            result.Report.ReportOnlyExpressionCount++;
            result.Report.AddLimited(result.Report.ReportOnlyExpressions, $"{owner} {fieldName}: {value}", 500);
        }
    }

    private static void TrackChargeFieldCoverage(OmniPowerDefinition power, OmniImportReport report)
    {
        var hasNumber = HasJsonValue(power.NumberOfChargesValue);
        var hasMax = HasJsonValue(power.MaxNumberOfChargesValue);
        if (hasNumber)
        {
            AddMappedPowerField(report, $"{power.FullName}: number_of_charges -> NumCharges = {power.NumberOfCharges}");
        }

        if (hasNumber && hasMax && power.NumberOfCharges > 0 && power.MaxNumberOfCharges > 0 &&
            power.NumberOfCharges != power.MaxNumberOfCharges)
        {
            AddKnownChargeCapacityExtension(report,
                $"{power.FullName}: number_of_charges={power.NumberOfCharges}, max_num_charges={power.MaxNumberOfCharges}; using number_of_charges as active charges and max_num_charges as extension cap");
        }
        else if ((!hasNumber || power.NumberOfCharges <= 0) && hasMax && power.MaxNumberOfCharges > 0)
        {
            AddMappedPowerFieldWithFallback(report,
                $"{power.FullName}: max_num_charges -> NumCharges fallback = {power.MaxNumberOfCharges}");
        }
    }

    private static void TrackChargeFieldCoverage(OmniPowerDefinition power, OmniApplyResult result)
    {
        var hasNumber = HasJsonValue(power.NumberOfChargesValue);
        var hasMax = HasJsonValue(power.MaxNumberOfChargesValue);
        if (hasNumber)
        {
            AddMappedPowerField(result, $"{power.FullName}: number_of_charges -> NumCharges = {power.NumberOfCharges}");
        }

        if (hasNumber && hasMax && power.NumberOfCharges > 0 && power.MaxNumberOfCharges > 0 &&
            power.NumberOfCharges != power.MaxNumberOfCharges)
        {
            AddKnownChargeCapacityExtension(result,
                $"{power.FullName}: number_of_charges={power.NumberOfCharges}, max_num_charges={power.MaxNumberOfCharges}; using number_of_charges as active charges and max_num_charges as extension cap");
        }
        else if ((!hasNumber || power.NumberOfCharges <= 0) && hasMax && power.MaxNumberOfCharges > 0)
        {
            AddMappedPowerFieldWithFallback(result,
                $"{power.FullName}: max_num_charges -> NumCharges fallback = {power.MaxNumberOfCharges}");
        }
    }

    private static void TrackDeferredPowerFields(OmniPowerDefinition power, OmniImportReport report)
    {
        foreach (var entry in ImportedPowerPolicyDiagnostics.DescribeSource(power))
        {
            AddPowerPolicyDiagnostic(report, power.FullName, entry);
        }
    }

    private static void TrackDeferredPowerFields(OmniPowerDefinition power, OmniApplyResult result)
    {
        foreach (var entry in ImportedPowerPolicyDiagnostics.DescribeSource(power))
        {
            AddPowerPolicyDiagnostic(result, power.FullName, entry);
        }
    }

    private static void TrackExtensionPowerFields(OmniPowerDefinition power, OmniImportReport report)
    {
        if (power.ExtensionData == null)
        {
            return;
        }

        foreach (var (field, value) in power.ExtensionData.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (IgnoredPowerJsonFields.Contains(field))
            {
                AddIgnoredPowerField(report, power.FullName, field, value);
            }
            else
            {
                AddUnknownPowerField(report, $"{power.FullName}: {field} = {FormatJson(value)}");
            }
        }
    }

    private static void TrackExtensionPowerFields(OmniPowerDefinition power, OmniApplyResult result)
    {
        if (power.ExtensionData == null)
        {
            return;
        }

        foreach (var (field, value) in power.ExtensionData.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase))
        {
            if (IgnoredPowerJsonFields.Contains(field))
            {
                AddIgnoredPowerField(result, power.FullName, field, value);
            }
            else
            {
                AddUnknownPowerField(result, $"{power.FullName}: {field} = {FormatJson(value)}");
            }
        }
    }

    private static void AddMappedPowerField(OmniImportReport report, string detail)
    {
        report.PowerFieldsMappedCount++;
        report.AddLimited(report.MappedPowerFields, detail, 500);
    }

    private static void AddMappedPowerField(OmniApplyResult result, string detail)
    {
        result.PowerFieldsMapped++;
        result.AddLimited(result.MappedPowerFieldDetails, detail, 500);
    }

    private static void AddMappedPowerFieldWithFallback(OmniImportReport report, string detail)
    {
        report.PowerFieldsMappedWithFallbackCount++;
        report.AddLimited(report.PowerFieldsMappedWithFallback, detail, 500);
    }

    private static void AddMappedPowerFieldWithFallback(OmniApplyResult result, string detail)
    {
        result.PowerFieldsMappedWithFallback++;
        result.AddLimited(result.PowerFieldFallbackDetails, detail, 500);
    }

    private static void AddPowerFieldConflict(OmniImportReport report, string detail)
    {
        report.PowerFieldConflictCount++;
        report.AddLimited(report.PowerFieldConflicts, detail, 500);
    }

    private static void AddKnownChargeCapacityExtension(OmniImportReport report, string detail)
    {
        report.KnownChargeCapacityExtensions++;
        report.AddLimited(report.KnownChargeCapacityDetailEntries, detail, 500);
    }

    private static void AddPowerFieldConflict(OmniApplyResult result, string detail)
    {
        result.PowerFieldConflicts++;
        result.AddLimited(result.PowerFieldConflictDetails, detail, 500);
    }

    private static void AddKnownChargeCapacityExtension(OmniApplyResult result, string detail)
    {
        result.KnownChargeCapacityExtensions++;
        result.AddLimited(result.KnownChargeCapacityDetailEntries, detail, 500);
    }

    private static void AddDeferredPowerField(OmniImportReport report, string detail)
    {
        report.DeferredPowerFieldCount++;
        report.AddLimited(report.DeferredPowerFields, detail, 500);
    }

    private static void AddDeferredPowerField(OmniApplyResult result, string detail)
    {
        result.DeferredPowerFields++;
        result.AddLimited(result.DeferredPowerFieldDetails, detail, 500);
    }

    private static void AddPowerPolicyDiagnostic(OmniImportReport report, string owner, ImportedPowerPolicyDiagnosticEntry entry)
    {
        var detail = ImportedPowerPolicyDiagnostics.FormatEntry(owner, entry);
        if (entry.Category == ImportedPowerPolicyDiagnosticCategory.AppliedNow)
        {
            AddMappedPowerField(report, detail);
            return;
        }

        AddDeferredPowerField(report, detail);
    }

    private static void AddPowerPolicyDiagnostic(OmniApplyResult result, string owner, ImportedPowerPolicyDiagnosticEntry entry)
    {
        var detail = ImportedPowerPolicyDiagnostics.FormatEntry(owner, entry);
        if (entry.Category == ImportedPowerPolicyDiagnosticCategory.AppliedNow)
        {
            AddMappedPowerField(result, detail);
            return;
        }

        AddDeferredPowerField(result, detail);
    }

    private static void AddIgnoredPowerField(
        OmniImportReport report,
        string owner,
        string fieldName,
        JToken? value,
        string? detailNote = null,
        string? ownerKind = null)
    {
        report.IgnoredPowerFieldCount++;
        IncrementCount(report.IgnoredPowerFieldOwnerKindCounts, GetIgnoredPowerFieldOwnerKind(owner, ownerKind));
        IncrementCount(report.IgnoredPowerFieldCategoryCounts, GetIgnoredPowerFieldCategory(fieldName));
        IncrementCount(report.IgnoredPowerFieldNameCounts, fieldName);

        if (!ShouldSampleIgnoredPowerField(fieldName, value, detailNote, report.IgnoredPowerFields))
        {
            return;
        }

        report.AddLimited(report.IgnoredPowerFields,
            BuildIgnoredPowerFieldSample(owner, fieldName, value, detailNote), 150);
    }

    private static void AddIgnoredPowerField(
        OmniApplyResult result,
        string owner,
        string fieldName,
        JToken? value,
        string? detailNote = null,
        string? ownerKind = null)
    {
        result.IgnoredPowerFields++;
        IncrementCount(result.IgnoredPowerFieldOwnerKindCounts, GetIgnoredPowerFieldOwnerKind(owner, ownerKind));
        IncrementCount(result.IgnoredPowerFieldCategoryCounts, GetIgnoredPowerFieldCategory(fieldName));
        IncrementCount(result.IgnoredPowerFieldNameCounts, fieldName);

        if (!ShouldSampleIgnoredPowerField(fieldName, value, detailNote, result.IgnoredPowerFieldDetails))
        {
            return;
        }

        result.AddLimited(result.IgnoredPowerFieldDetails,
            BuildIgnoredPowerFieldSample(owner, fieldName, value, detailNote), 150);
    }

    private static void AddUnknownPowerField(OmniImportReport report, string detail)
    {
        report.UnknownPowerFieldCount++;
        report.AddLimited(report.UnknownPowerFields, detail, 500);
    }

    private static void AddUnknownPowerField(OmniApplyResult result, string detail)
    {
        result.UnknownPowerFields++;
        result.AddLimited(result.UnknownPowerFieldDetails, detail, 500);
    }

    private static string FormatStringList(IEnumerable<string> values)
    {
        return string.Join(", ", values.Where(value => !string.IsNullOrWhiteSpace(value)).Distinct(StringComparer.OrdinalIgnoreCase));
    }

    private static string FormatJson(JToken? token)
    {
        if (!HasJsonValue(token))
        {
            return string.Empty;
        }

        return token!.ToString(Formatting.None);
    }

    private static string GetIgnoredPowerFieldCategory(string fieldName)
    {
        return NormalizeLookupKey(fieldName) switch
        {
            var normalized when normalized.StartsWith("highlight", StringComparison.OrdinalIgnoreCase) ||
                                normalized.StartsWith("tray", StringComparison.OrdinalIgnoreCase) ||
                                normalized.StartsWith("servertray", StringComparison.OrdinalIgnoreCase) ||
                                normalized is "rememberstance" or "ignorestance"
                => "UI / Tray / Highlight metadata",
            var normalized when normalized.StartsWith("message", StringComparison.OrdinalIgnoreCase) ||
                                normalized is "selfconfirm" or "timetoconfirm"
                => "Client / Server confirmation metadata",
            var normalized when normalized.StartsWith("display", StringComparison.OrdinalIgnoreCase) ||
                                normalized is "icon" or "shortname" or "localavailablelevel"
                => "Display / Icon metadata",
            var normalized when normalized.StartsWith("position", StringComparison.OrdinalIgnoreCase) ||
                                normalized.StartsWith("vecbox", StringComparison.OrdinalIgnoreCase) ||
                                normalized.StartsWith("chain", StringComparison.OrdinalIgnoreCase) ||
                                normalized is "facetarget" or "isenvironmenthit" or "shuffletargetlist" or
                                    "targetuntargetable" or "travelsuppressiontime" or "toggledetoggletime" or
                                    "toggledroppable" or "toggleignores" or "toggleontime" or
                                    "worksonuntouchable" or "worksthroughvisionphase" or "ignoretogglemaxdistance"
                => "Combat-shape / client-simulation metadata",
            _ => "Other nonplanner fields"
        };
    }

    private static string GetIgnoredPowerFieldOwnerKind(string owner, string? explicitOwnerKind = null)
    {
        if (!string.IsNullOrWhiteSpace(explicitOwnerKind))
        {
            return explicitOwnerKind;
        }

        var fullName = CanonicalizeOmniFullName(owner);
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return "Other";
        }

        var group = GroupNamePart(fullName);
        if (group.Equals("Boosts", StringComparison.OrdinalIgnoreCase))
        {
            return "Boost";
        }

        if (group.Equals("Set_Bonus", StringComparison.OrdinalIgnoreCase))
        {
            return "EnhancementPolicy";
        }

        return fullName.Count(character => character == '.') >= 2
            ? "Power"
            : "Other";
    }

    private static bool ShouldSampleIgnoredPowerField(
        string fieldName,
        JToken? value,
        string? detailNote,
        IReadOnlyCollection<string> existingSamples)
    {
        if (CountIgnoredPowerFieldSamples(existingSamples, fieldName) >= 3)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(detailNote))
        {
            return true;
        }

        return !IsDefaultIgnoredPowerFieldValue(value);
    }

    private static int CountIgnoredPowerFieldSamples(IEnumerable<string> samples, string fieldName)
    {
        var prefix = $"{fieldName}: ";
        return samples.Count(sample => sample.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsDefaultIgnoredPowerFieldValue(JToken? token)
    {
        if (!HasJsonValue(token))
        {
            return true;
        }

        return token!.Type switch
        {
            JTokenType.Boolean => !token.Value<bool>(),
            JTokenType.Integer => token.Value<long>() == 0,
            JTokenType.Float => Math.Abs(token.Value<double>()) < 0.000001d,
            JTokenType.String => string.IsNullOrWhiteSpace(token.Value<string>()),
            JTokenType.Array => !token.HasValues,
            JTokenType.Object => !token.HasValues,
            _ => false
        };
    }

    private static string BuildIgnoredPowerFieldSample(
        string owner,
        string fieldName,
        JToken? value,
        string? detailNote)
    {
        if (!string.IsNullOrWhiteSpace(detailNote))
        {
            return $"{fieldName}: {owner} -> {detailNote}";
        }

        return $"{fieldName}: {owner} ignored = {FormatJson(value)}";
    }

    private static void ApplyStrengthsDisallowedToEffects(
        IList<IEffect> effects,
        IReadOnlyCollection<Enums.eEnhance> disallowedEnhancements,
        IReadOnlyCollection<TypedEnhancementRestriction> typedRestrictions)
    {
        if (effects.Count == 0 || (disallowedEnhancements.Count == 0 && typedRestrictions.Count == 0))
        {
            return;
        }

        var blockedEnhancements = disallowedEnhancements.ToHashSet();
        foreach (var effect in effects.Where(effect => effect != null))
        {
            var mappedEnhance = MapEnhanceFromEffect(effect);
            var blockedByEnhance = mappedEnhance != Enums.eEnhance.None && blockedEnhancements.Contains(mappedEnhance);
            var blockedByTyped = TypedEnhancementLegality.EffectMatchesRestrictions(effect, typedRestrictions);
            if (blockedByEnhance || blockedByTyped)
            {
                effect.Buffable = false;
            }
        }
    }

    private static (Enums.eEnhance[] Mapped, TypedEnhancementRestriction[] Typed, EnhancementPolicyAxis[] Axes, string[] Unresolved) ResolveDisallowedEnhancementPolicy(
        OmniPowerDefinition power,
        IEnumerable<string> labels)
    {
        var mapped = new List<Enums.eEnhance>();
        var typed = new List<TypedEnhancementRestriction>();
        var axes = new List<EnhancementPolicyAxis>();
        var unresolved = new List<string>();
        var seenMapped = new HashSet<Enums.eEnhance>();
        var seenTyped = new HashSet<TypedEnhancementRestriction>();
        var seenAxes = new HashSet<EnhancementPolicyAxis>();
        var seenUnresolved = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        TypedEnhancementRestriction[]? inferredTypedRestrictions = null;

        TypedEnhancementRestriction[] GetInferredTypedRestrictions()
        {
            inferredTypedRestrictions ??= TypedEnhancementLegality.InferRestrictionsFromEffects(
                OmniMidsMapper.FlattenEffects(power).Cast<IEffect>());
            return inferredTypedRestrictions;
        }

        foreach (var label in labels.Where(value => !string.IsNullOrWhiteSpace(value)))
        {
            var resolvedAny = false;
            foreach (var candidate in EnumerateDisallowedEnhancementCandidates(label))
            {
                if (NormalizeLookupKey(candidate) == "toxic")
                {
                    var inferredForCandidate = GetInferredTypedRestrictions()
                        .Where(restriction => restriction.DamageType == Enums.eDamage.Toxic)
                        .ToArray();
                    if (inferredForCandidate.Length > 0)
                    {
                        resolvedAny = true;
                        foreach (var restriction in inferredForCandidate)
                        {
                            if (seenTyped.Add(restriction))
                            {
                                typed.Add(restriction);
                            }
                        }
                    }

                    continue;
                }

                if (EnhancementPolicyAxes.TryParse(candidate, out var axis))
                {
                    resolvedAny = true;
                    if (seenAxes.Add(axis))
                    {
                        axes.Add(axis);
                    }
                    continue;
                }

                if (!TryMapDisallowedEnhancementCandidate(candidate, out var mappedEnhancements))
                {
                    continue;
                }

                resolvedAny = true;
                foreach (var enhance in mappedEnhancements)
                {
                    if (enhance != Enums.eEnhance.None && seenMapped.Add(enhance))
                    {
                        mapped.Add(enhance);
                    }
                }
            }

            if (!resolvedAny && seenUnresolved.Add(label))
            {
                unresolved.Add(label);
            }
        }

        return (mapped.ToArray(), TypedEnhancementLegality.Normalize(typed), EnhancementPolicyAxes.Normalize(axes), unresolved.ToArray());
    }

    private static IEnumerable<string> EnumerateDisallowedEnhancementCandidates(string label)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var candidate in EnumeratePowerBoostAllowedLookupCandidates(label))
        {
            if (!string.IsNullOrWhiteSpace(candidate) && seen.Add(candidate))
            {
                yield return candidate;
            }
        }

        foreach (var candidate in label.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!string.IsNullOrWhiteSpace(candidate) && seen.Add(candidate))
            {
                yield return candidate;
            }
        }
    }

    private static bool TryMapDisallowedEnhancementCandidate(string candidate, out Enums.eEnhance[] mapped)
    {
        mapped = NormalizeLookupKey(candidate) switch
        {
            "accuracy" or "accuracyboost" => [Enums.eEnhance.Accuracy],
            "damage" or "damageboost" or "debuffdamageboost" or "buffdamageboost" => [Enums.eEnhance.Damage],
            "defense" or "buffdefenseboost" or "debuffdefenseboost" => [Enums.eEnhance.Defense],
            "endurancediscount" or "endurancediscountboost" => [Enums.eEnhance.EnduranceDiscount],
            "endurance" => [Enums.eEnhance.Endurance],
            "recovery" or "recoveryboost" => [Enums.eEnhance.Endurance, Enums.eEnhance.Recovery],
            "speedflying" or "speedflyingboost" => [Enums.eEnhance.SpeedFlying],
            "heal" or "hitpoints" or "regeneration" or "absorb" or "healboost"
                => [Enums.eEnhance.Heal, Enums.eEnhance.HitPoints, Enums.eEnhance.Regeneration, Enums.eEnhance.Absorb],
            "interrupt" or "interruptboost" => [Enums.eEnhance.Interrupt],
            "jumpheight" or "speedjumping" or "jumpboost" => [Enums.eEnhance.JumpHeight, Enums.eEnhance.SpeedJumping],
            "mez" or "confuseboost" or "fearboost" or "holdboost" or "immobilizedboost" or "intangibleboost" or
                "knockbackboost" or "sleepboost" or "stunnedboost" or "tauntboost"
                => [Enums.eEnhance.Mez],
            "range" or "rangeboost" => [Enums.eEnhance.Range],
            "rechargetime" or "xrechargetime" or "rechargeboost" => [Enums.eEnhance.RechargeTime],
            "resistance" or "resdamageboost" => [Enums.eEnhance.Resistance],
            "speedrunning" or "speedrunningboost" => [Enums.eEnhance.SpeedRunning],
            "tohit" or "bufftohitboost" or "debufftohitboost" => [Enums.eEnhance.ToHit],
            "slow" or "slowboost" => [Enums.eEnhance.Slow],
            _ => []
        };

        return mapped.Length > 0;
    }

    private static void TrackEnhancementPolicyCoverage(OmniPowerDefinition power, OmniImportReport report)
    {
        TrackEnhancementPolicyCoverage(
            power,
            power.StrengthsDisallowed,
            "strengths_disallowed",
            "IgnoreEnh plus matching effect Buffable=false",
            report);
        TrackEnhancementPolicyCoverage(
            power,
            power.GlobalStrengthsDisallowed,
            "global_strengths_disallowed",
            "Ignore_Buff",
            report);
    }

    private static void TrackEnhancementPolicyCoverage(OmniPowerDefinition power, OmniApplyResult result)
    {
        TrackEnhancementPolicyCoverage(
            power,
            power.StrengthsDisallowed,
            "strengths_disallowed",
            "IgnoreEnh plus matching effect Buffable=false",
            result);
        TrackEnhancementPolicyCoverage(
            power,
            power.GlobalStrengthsDisallowed,
            "global_strengths_disallowed",
            "Ignore_Buff",
            result);
    }

    private static void TrackEnhancementPolicyCoverage(
        OmniPowerDefinition power,
        IEnumerable<string> labels,
        string sourceField,
        string midsTarget,
        OmniImportReport report)
    {
        var (mapped, typed, axes, unresolved) = ResolveDisallowedEnhancementPolicy(power, labels);
        if (mapped.Length > 0 || typed.Length > 0 || axes.Length > 0)
        {
            AddMappedPowerField(report, $"{power.FullName}: {sourceField} -> {midsTarget} = {FormatEnhancementPolicyMapping(mapped, typed, axes)}");
        }

        if (unresolved.Length > 0)
        {
            AddPowerFieldConflict(report,
                $"{power.FullName}: {sourceField} has no Mids enhancement-policy mapping for {FormatStringList(unresolved)}");
        }
    }

    private static void TrackEnhancementPolicyCoverage(
        OmniPowerDefinition power,
        IEnumerable<string> labels,
        string sourceField,
        string midsTarget,
        OmniApplyResult result)
    {
        var (mapped, typed, axes, unresolved) = ResolveDisallowedEnhancementPolicy(power, labels);
        if (mapped.Length > 0 || typed.Length > 0 || axes.Length > 0)
        {
            AddMappedPowerField(result, $"{power.FullName}: {sourceField} -> {midsTarget} = {FormatEnhancementPolicyMapping(mapped, typed, axes)}");
        }

        if (unresolved.Length > 0)
        {
            AddPowerFieldConflict(result,
                $"{power.FullName}: {sourceField} has no Mids enhancement-policy mapping for {FormatStringList(unresolved)}");
        }
    }

    private static string FormatEnhanceList(IEnumerable<Enums.eEnhance> enhances)
    {
        return string.Join(", ", enhances.Select(enhance => Enum.GetName(typeof(Enums.eEnhance), enhance) ?? enhance.ToString()));
    }

    private static string FormatTypedEnhancementRestrictionList(IEnumerable<TypedEnhancementRestriction> restrictions)
    {
        return string.Join(", ",
            TypedEnhancementLegality.Normalize(restrictions)
                .Select(restriction => restriction.ToString()));
    }

    private static string FormatEnhancementPolicyMapping(
        IReadOnlyCollection<Enums.eEnhance> mapped,
        IReadOnlyCollection<TypedEnhancementRestriction> typed,
        IReadOnlyCollection<EnhancementPolicyAxis> axes)
    {
        var parts = new List<string>();
        if (mapped.Count > 0)
        {
            parts.Add(FormatEnhanceList(mapped));
        }

        if (typed.Count > 0)
        {
            parts.Add($"Typed[{FormatTypedEnhancementRestrictionList(typed)}]");
        }

        if (axes.Count > 0)
        {
            parts.Add($"Axes[{string.Join(", ", axes.Select(EnhancementPolicyAxes.GetDisplayName))}]");
        }

        return parts.Count == 0 ? "<empty>" : string.Join("; ", parts);
    }

    private static bool IsSustainedStatPower(OmniPowerDefinition power)
    {
        return power.Type.Equals("Auto", StringComparison.OrdinalIgnoreCase) ||
               power.Type.Equals("Toggle", StringComparison.OrdinalIgnoreCase) ||
               power.Type.Equals("ClickBuff", StringComparison.OrdinalIgnoreCase);
    }
}
