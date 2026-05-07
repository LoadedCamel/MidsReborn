using System.Diagnostics;

namespace Mids_Reborn.Core.Omni;

internal enum OmniImportStageId
{
    ValidateExport,
    IndexFiles,
    PrepareAnalysis,
    ReadArchetypes,
    ExpandScopedReferences,
    LoadScopedPowersets,
    LoadScopedPowers,
    ClassifyPowers,
    ResolveEntities,
    ReadClassTables,
    NormalizeEnhancementInputs,
    PrepareApply,
    ApplyPowersets,
    ApplyPowers,
    ApplyEntities,
    LinkPlannerMetadata,
    ImportEnhancements,
    RebuildIds,
    FinalAudits,
    PreviewReport,
    AnalysisComplete,
    ApplyComplete
}

internal sealed class OmniImportWorkPlan
{
    private sealed record StageDefinition(OmniImportStageId Id, string Label, double Weight);

    private readonly Dictionary<OmniImportStageId, StageDefinition> _definitions;
    private readonly Dictionary<OmniImportStageId, double> _completedWeights = [];
    private readonly Stopwatch _elapsed = Stopwatch.StartNew();
    private OmniImportStageId? _activeStageId;

    public OmniImportWorkPlan(IEnumerable<(OmniImportStageId Id, string Label, double Weight)> stages)
    {
        _definitions = stages
            .Select(stage => new StageDefinition(stage.Id, stage.Label, Math.Max(0.1d, stage.Weight)))
            .ToDictionary(stage => stage.Id);
        TotalWeight = _definitions.Values.Sum(stage => stage.Weight);
    }

    public double TotalWeight { get; }

    public OmniImportProgress Report(
        OmniImportStageId stageId,
        string detail = "",
        int current = 0,
        int total = 0,
        bool markComplete = false)
    {
        var definition = _definitions[stageId];

        if (_activeStageId.HasValue && _activeStageId.Value != stageId)
        {
            MarkComplete(_activeStageId.Value);
        }

        _activeStageId = stageId;

        var ratio = total > 0
            ? Math.Clamp(current / (double)total, 0d, 1d)
            : (markComplete ? 1d : 0d);

        if (markComplete)
        {
            ratio = 1d;
            MarkComplete(stageId);
        }

        var completedWeight = _completedWeights.Values.Sum();
        if (!markComplete)
        {
            completedWeight += definition.Weight * ratio;
        }

        var globalPercent = TotalWeight <= 0
            ? 1d
            : Math.Clamp(completedWeight / TotalWeight, 0d, 1d);

        var estimatedRemaining = EstimateRemaining(globalPercent);
        return new OmniImportProgress(
            (int)Math.Round(globalPercent * 100d),
            definition.Label,
            detail,
            current,
            total,
            ToStageToken(stageId),
            _elapsed.Elapsed,
            estimatedRemaining,
            globalPercent);
    }

    private void MarkComplete(OmniImportStageId stageId)
    {
        if (_definitions.TryGetValue(stageId, out var definition))
        {
            _completedWeights[stageId] = definition.Weight;
        }
    }

    private TimeSpan? EstimateRemaining(double globalPercent)
    {
        if (globalPercent <= 0.001d || globalPercent >= 0.999d)
        {
            return globalPercent >= 0.999d ? TimeSpan.Zero : null;
        }

        var elapsed = _elapsed.Elapsed;
        if (elapsed.TotalMilliseconds <= 0)
        {
            return null;
        }

        var remainingRatio = (1d - globalPercent) / globalPercent;
        return TimeSpan.FromMilliseconds(Math.Max(0d, elapsed.TotalMilliseconds * remainingRatio));
    }

    internal static OmniImportWorkPlan CreateAnalysisPlan()
    {
        return new OmniImportWorkPlan(
        [
            (OmniImportStageId.ValidateExport, "Validating export", 2d),
            (OmniImportStageId.IndexFiles, "Indexing files", 6d),
            (OmniImportStageId.PrepareAnalysis, "Preparing analysis", 2d),
            (OmniImportStageId.ReadArchetypes, "Reading archetypes", 8d),
            (OmniImportStageId.ExpandScopedReferences, "Expanding scoped references", 12d),
            (OmniImportStageId.LoadScopedPowersets, "Loading powersets", 8d),
            (OmniImportStageId.LoadScopedPowers, "Scanning power files", 18d),
            (OmniImportStageId.ClassifyPowers, "Classifying powers", 14d),
            (OmniImportStageId.ResolveEntities, "Resolving entities and pet manifests", 10d),
            (OmniImportStageId.ReadClassTables, "Reading class tables", 6d),
            (OmniImportStageId.NormalizeEnhancementInputs, "Normalizing enhancement inputs", 10d),
            (OmniImportStageId.PreviewReport, "Preview/report build", 2d),
            (OmniImportStageId.AnalysisComplete, "Analysis complete", 2d)
        ]);
    }

    internal static OmniImportWorkPlan CreateApplyPlan()
    {
        return new OmniImportWorkPlan(
        [
            (OmniImportStageId.ValidateExport, "Validating export", 2d),
            (OmniImportStageId.IndexFiles, "Indexing files", 4d),
            (OmniImportStageId.PrepareApply, "Preparing safe import", 8d),
            (OmniImportStageId.ApplyPowersets, "Applying powersets", 10d),
            (OmniImportStageId.ApplyPowers, "Applying powers", 34d),
            (OmniImportStageId.ApplyEntities, "Applying entities and pet manifests", 8d),
            (OmniImportStageId.LinkPlannerMetadata, "Linking planner metadata", 10d),
            (OmniImportStageId.ImportEnhancements, "Importing enhancements", 12d),
            (OmniImportStageId.RebuildIds, "Rebuilding IDs", 4d),
            (OmniImportStageId.FinalAudits, "Final audits", 6d),
            (OmniImportStageId.ApplyComplete, "Safe import complete", 2d)
        ]);
    }

    internal static string ToStageToken(OmniImportStageId stageId)
    {
        return stageId switch
        {
            OmniImportStageId.ValidateExport => "validate-export",
            OmniImportStageId.IndexFiles => "index-files",
            OmniImportStageId.PrepareAnalysis => "prepare-analysis",
            OmniImportStageId.ReadArchetypes => "read-archetypes",
            OmniImportStageId.ExpandScopedReferences => "expand-scoped-references",
            OmniImportStageId.LoadScopedPowersets => "load-scoped-powersets",
            OmniImportStageId.LoadScopedPowers => "load-scoped-powers",
            OmniImportStageId.ClassifyPowers => "classify-powers",
            OmniImportStageId.ResolveEntities => "resolve-entities",
            OmniImportStageId.ReadClassTables => "read-class-tables",
            OmniImportStageId.NormalizeEnhancementInputs => "normalize-enhancement-inputs",
            OmniImportStageId.PrepareApply => "prepare-apply",
            OmniImportStageId.ApplyPowersets => "apply-powersets",
            OmniImportStageId.ApplyPowers => "apply-powers",
            OmniImportStageId.ApplyEntities => "apply-entities",
            OmniImportStageId.LinkPlannerMetadata => "link-planner-metadata",
            OmniImportStageId.ImportEnhancements => "import-enhancements",
            OmniImportStageId.RebuildIds => "rebuild-ids",
            OmniImportStageId.FinalAudits => "final-audits",
            OmniImportStageId.PreviewReport => "preview-report",
            OmniImportStageId.AnalysisComplete => "analysis-complete",
            OmniImportStageId.ApplyComplete => "apply-complete",
            _ => stageId.ToString()
        };
    }
}
