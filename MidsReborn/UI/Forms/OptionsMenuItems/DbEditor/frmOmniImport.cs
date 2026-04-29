using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Properties;

namespace Mids_Reborn.UI.Forms.OptionsMenuItems.DbEditor;

public sealed class frmOmniImport : Form
{
    private readonly TextBox _exportRoot;
    private readonly TextBox _report;
    private readonly Button _browse;
    private readonly Button _dryRun;
    private readonly Button _applyImport;
    private readonly Button _saveReport;
    private readonly Button _mathReport;
    private readonly Label _status;
    private readonly ProgressBar _progress;
    private readonly Label _progressStage;
    private readonly Label _progressDetail;
    private readonly ToolTip _progressToolTip;
    private OmniImportResult? _lastDryRunResult;
    private string _lastExportRoot = string.Empty;
    private string _lastReport = string.Empty;
    private string _lastPreviewReport = string.Empty;
    private string _lastReportFilePrefix = "omni-import-report";
    private bool _disposedState;

    public frmOmniImport()
    {
        Text = @"Database Import";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        BackColor = Color.FromArgb(0, 0, 32);
        ForeColor = Color.White;
        Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        ClientSize = new Size(920, 640);
        _progressToolTip = new ToolTip
        {
            ShowAlways = true
        };
        FormClosed += (_, _) => ClearImportState(runCleanup: true);

        var title = new Label
        {
            Text = @"Omni Import",
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var description = new Label
        {
            Text = @"Run a dry run first, then optionally apply the safe import to the loaded database.",
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleCenter
        };

        var rootPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 42,
            Padding = new Padding(12, 6, 12, 4),
            ColumnCount = 3
        };
        rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        rootPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110));

        rootPanel.Controls.Add(new Label
        {
            Text = @"Omni Export",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        }, 0, 0);

        _exportRoot = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = ResolveInitialExportRoot()
        };
        rootPanel.Controls.Add(_exportRoot, 1, 0);

        _browse = new Button
        {
            Text = @"Browse...",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(192, 192, 255),
            ForeColor = Color.Black
        };
        _browse.Click += Browse_Click;
        rootPanel.Controls.Add(_browse, 2, 0);

        var actionPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 44,
            Padding = new Padding(12, 4, 12, 4),
            FlowDirection = FlowDirection.LeftToRight
        };

        _dryRun = new Button
        {
            Text = @"Run Dry Run",
            Width = 150,
            Height = 32,
            BackColor = Color.MediumSeaGreen,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Popup,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point)
        };
        _dryRun.Click += DryRun_Click;
        actionPanel.Controls.Add(_dryRun);

        _applyImport = new Button
        {
            Text = @"Apply Safe Import",
            Width = 160,
            Height = 32,
            Enabled = false,
            BackColor = Color.LightSkyBlue,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Popup,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point)
        };
        _applyImport.Click += ApplyImport_Click;
        actionPanel.Controls.Add(_applyImport);

        _saveReport = new Button
        {
            Text = @"Save Report...",
            Width = 150,
            Height = 32,
            Enabled = false,
            BackColor = Color.PaleGoldenrod,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Popup,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point)
        };
        _saveReport.Click += SaveReport_Click;
        actionPanel.Controls.Add(_saveReport);

        _mathReport = new Button
        {
            Text = @"Generate Math Report...",
            Width = 190,
            Height = 32,
            BackColor = Color.Plum,
            ForeColor = Color.Black,
            FlatStyle = FlatStyle.Popup,
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point)
        };
        _mathReport.Click += MathReport_Click;
        actionPanel.Controls.Add(_mathReport);

        _status = new Label
        {
            Text = @"Ready.",
            AutoSize = false,
            Width = 360,
            Height = 32,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(12, 0, 0, 0)
        };
        actionPanel.Controls.Add(_status);

        var progressPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            Padding = new Padding(12, 4, 12, 8)
        };

        _progressStage = new Label
        {
            Text = @"Idle.",
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point),
            AutoEllipsis = true,
            UseMnemonic = false
        };

        _progressDetail = new Label
        {
            Text = @"Ready.",
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.Gainsboro,
            AutoEllipsis = true,
            UseMnemonic = false
        };
        _progressToolTip.SetToolTip(_progressDetail, _progressDetail.Text);

        _progress = new ProgressBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 0,
            Style = ProgressBarStyle.Continuous
        };
        progressPanel.Controls.Add(_progressStage);
        progressPanel.Controls.Add(_progressDetail);
        progressPanel.Controls.Add(_progress);
        progressPanel.Resize += (_, _) => LayoutProgressPanel(progressPanel);
        LayoutProgressPanel(progressPanel);

        _report = new TextBox
        {
            Dock = DockStyle.Fill,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            WordWrap = false,
            BackColor = Color.FromArgb(24, 24, 32),
            ForeColor = Color.White,
            Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point)
        };

        Controls.Add(_report);
        Controls.Add(progressPanel);
        Controls.Add(actionPanel);
        Controls.Add(rootPanel);
        Controls.Add(description);
        Controls.Add(title);
    }

    private void Browse_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = @"Select the Omni export root folder.",
            UseDescriptionForTitle = true,
            SelectedPath = Directory.Exists(_exportRoot.Text) ? _exportRoot.Text : string.Empty
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _exportRoot.Text = dialog.SelectedPath;
            PersistExportRoot(dialog.SelectedPath);
        }
    }

    private async void DryRun_Click(object? sender, EventArgs e)
    {
        var exportRoot = _exportRoot.Text.Trim();
        if (!Directory.Exists(exportRoot))
        {
            MessageBox.Show(this, @"Select a valid Omni export folder first.", @"Omni Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        PersistExportRoot(exportRoot);

        SetBusy(true, @"Scanning Omni export...");
        RenderPreview(BuildRunningPreview("Dry Run", exportRoot));
        _lastDryRunResult = null;
        _lastExportRoot = string.Empty;
        _applyImport.Enabled = false;

        try
        {
            await using var progress = new UiProgressBridge(this, UpdateProgress);

            var importTimer = Stopwatch.StartNew();
            var result = await RunImportWorkAsync(() => new OmniImporter().DryRun(exportRoot, progress));
            importTimer.Stop();
            await progress.FlushAsync();

            var fullReportTimer = Stopwatch.StartNew();
            var fullReport = BuildFullDryRunReport(exportRoot, result);
            fullReportTimer.Stop();

            var previewBuildTimer = Stopwatch.StartNew();
            var preview = BuildDryRunPreview(exportRoot, result, UiRenderMetrics.Pending(
                importTimer.Elapsed,
                fullReportTimer.Elapsed,
                progress.AppliedCount,
                fullReport.Length));
            previewBuildTimer.Stop();

            var renderTimer = Stopwatch.StartNew();
            RenderPreview(preview);
            renderTimer.Stop();

            var finalMetrics = new UiRenderMetrics(
                importTimer.Elapsed,
                fullReportTimer.Elapsed,
                previewBuildTimer.Elapsed,
                renderTimer.Elapsed,
                preview.Length,
                fullReport.Length,
                progress.AppliedCount);
            RenderPreview(BuildDryRunPreview(exportRoot, result, finalMetrics));

            _lastDryRunResult = result;
            _lastExportRoot = exportRoot;
            _lastReport = fullReport;
            _lastReportFilePrefix = "omni-import-dry-run";
            result.TrimForApply();
            _saveReport.Enabled = true;
            _applyImport.Enabled = true;
            _status.Text = @"Dry run complete. No database changes were made.";
            SetTerminalProgress(_status.Text, @"Preview updated.", succeeded: true);
        }
        catch (Exception ex)
        {
            _status.Text = @"Dry run failed.";
            RenderPreview(BuildFailurePreview("Dry run failed.", ex, !string.IsNullOrWhiteSpace(_lastReport)));
            _saveReport.Enabled = !string.IsNullOrWhiteSpace(_lastReport);
            _applyImport.Enabled = false;
            SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
        }
        finally
        {
            SetBusy(false, _status.Text);
        }
    }

    private async void ApplyImport_Click(object? sender, EventArgs e)
    {
        if (_lastDryRunResult == null || string.IsNullOrWhiteSpace(_lastExportRoot))
        {
            MessageBox.Show(this, @"Run a dry run before applying the import.", @"Omni Import", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var confirmation =
            "Apply the safe Omni import to the loaded database?\r\n\r\n" +
            "This replaces effects on matching existing powers, updates fully decompiled requirements, stores class attributes, and does not save the database file.\r\n\r\n" +
            $"Scoped powers: {_lastDryRunResult.Report.PowersInScope}\r\n" +
            $"Unsupported power requirements: {_lastDryRunResult.Report.UnsupportedPowerRequirementCount}\r\n" +
            $"Unknown effect mappings: {_lastDryRunResult.Report.UnknownEffectMappings.Count}\r\n" +
            $"Unknown attribute mappings: {_lastDryRunResult.Report.UnknownAttribMappings.Count}\r\n" +
            $"Enhancements discovered: {_lastDryRunResult.Report.EnhancementDefinitionsDiscovered}\r\n" +
            $"Enhancement sets discovered: {_lastDryRunResult.Report.EnhancementSetsDiscovered}\r\n" +
            $"Boosts powersets/powers in scope: {_lastDryRunResult.Report.BoostPowersetsInScope}/{_lastDryRunResult.Report.BoostPowersInScope}\r\n" +
            $"Set_Bonus powersets/powers in scope: {_lastDryRunResult.Report.SetBonusPowersetsInScope}/{_lastDryRunResult.Report.SetBonusPowersInScope}\r\n" +
            $"Folded enhancement recipes: {_lastDryRunResult.Report.FoldedEnhancementRecipeCount}\r\n" +
            $"Missing enhancement power links: {_lastDryRunResult.Report.EnhancementBoostPowerLinksMissingDryRun + _lastDryRunResult.Report.EnhancementSetBonusLinksMissingDryRun}";

        if (MessageBox.Show(this, confirmation, @"Apply Safe Omni Import", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        PersistExportRoot(_lastExportRoot);

        SetBusy(true, @"Applying safe import...");
        RenderPreview(BuildRunningPreview("Safe Import", _lastExportRoot));

        try
        {
            await using var progress = new UiProgressBridge(this, UpdateProgress);

            var importTimer = Stopwatch.StartNew();
            var applyResult = await RunImportWorkAsync(() =>
                new OmniImporter().ApplySafeImport(DatabaseAPI.Database, _lastExportRoot, _lastDryRunResult, progress));
            importTimer.Stop();
            await progress.FlushAsync();

            // Safe import can change powerset, enhancement, enhancement-set, and picker-facing
            // image assignments. Refresh the full image cache so DB editors opened
            // immediately afterward see current ImageIdx values instead of stale blanks.
            AssetManager.LoadImages();

            var fullReportTimer = Stopwatch.StartNew();
            var applyReport = applyResult.ToMarkdown();
            var fullReport = string.IsNullOrWhiteSpace(_lastReport)
                ? applyReport
                : $"{_lastReport}{Environment.NewLine}{Environment.NewLine}{applyReport}";
            fullReportTimer.Stop();

            var previewBuildTimer = Stopwatch.StartNew();
            var preview = BuildApplyPreview(_lastExportRoot, _lastDryRunResult, applyResult, UiRenderMetrics.Pending(
                importTimer.Elapsed,
                fullReportTimer.Elapsed,
                progress.AppliedCount,
                fullReport.Length));
            previewBuildTimer.Stop();

            var renderTimer = Stopwatch.StartNew();
            RenderPreview(preview);
            renderTimer.Stop();

            var finalMetrics = new UiRenderMetrics(
                importTimer.Elapsed,
                fullReportTimer.Elapsed,
                previewBuildTimer.Elapsed,
                renderTimer.Elapsed,
                preview.Length,
                fullReport.Length,
                progress.AppliedCount);
            RenderPreview(BuildApplyPreview(_lastExportRoot, _lastDryRunResult, applyResult, finalMetrics));

            _lastReport = fullReport;
            _lastReportFilePrefix = "omni-import-safe-import";
            _saveReport.Enabled = true;
            _status.Text = @"Safe import applied. Save Report writes the dry-run plus apply report.";
            SetTerminalProgress(_status.Text, @"Preview updated.", succeeded: true);

            MessageBox.Show(
                this,
                "Safe Omni import complete.\r\n\r\n" +
                $"Powersets created: {applyResult.PowersetsCreated}\r\n" +
                $"Powersets updated: {applyResult.PowersetsUpdated}\r\n" +
                $"Powers matched: {applyResult.PowersMatched}\r\n" +
                $"Powers created: {applyResult.PowersCreated}\r\n" +
                $"Powers updated: {applyResult.PowersUpdated}\r\n" +
                $"Effects replaced: {applyResult.EffectsReplaced}\r\n" +
                $"Requirements updated: {applyResult.RequirementsUpdated}\r\n" +
                $"Redirect effects added: {applyResult.RedirectEffectsAdded}\r\n" +
                $"Powers missing from Mids: {applyResult.PowersMissingFromMids}\r\n\r\n" +
                $"Salvage matched/created/updated: {applyResult.SalvageMatched}/{applyResult.SalvageCreated}/{applyResult.SalvageUpdated}\r\n" +
                $"Recipes matched/created/updated: {applyResult.RecipesMatched}/{applyResult.RecipesCreated}/{applyResult.RecipesUpdated}\r\n" +
                $"Enhancement sets matched/created/updated: {applyResult.EnhancementSetsMatched}/{applyResult.EnhancementSetsCreated}/{applyResult.EnhancementSetsUpdated}\r\n" +
                $"Enhancements matched/created/updated: {applyResult.EnhancementsMatched}/{applyResult.EnhancementsCreated}/{applyResult.EnhancementsUpdated}\r\n" +
                $"Classic enhancement variants discovered/logical/folded: {applyResult.ClassicEnhancementSourceVariantsDiscovered}/{applyResult.ClassicEnhancementLogicalRecords}/{applyResult.ClassicEnhancementVariantsFolded}\r\n" +
                $"Classic enhancement editor rows expected: {applyResult.ClassicEnhancementEditorRowsExpected}\r\n" +
                $"Enhancement class derivation effects/fallback/category-only/mismatch/unresolved: {applyResult.EnhancementClassIdsDerivedFromEffects}/{applyResult.EnhancementClassIdsFallbackUsed}/{applyResult.EnhancementClassIdsCategoryOnly}/{applyResult.EnhancementClassIdWrapperMismatches}/{applyResult.EnhancementClassIdsUnresolved}\r\n" +
                $"Known hidden/stateful effect mappings: {applyResult.KnownHiddenStatefulEffectMappings}\r\n" +
                $"Known unsupported effect mappings: {applyResult.KnownUnsupportedEffectMappings}\r\n" +
                $"Global/power-local chance mods mapped: {applyResult.GlobalChanceModsMapped}/{applyResult.PowerLocalChanceModsMapped}\r\n" +
                $"Vector defense/resistance template overrides: {applyResult.VectorDefenseTemplateOverrides}/{applyResult.VectorResistanceTemplateOverrides}\r\n" +
                $"Boosts powers in scope matched/created/updated: {applyResult.BoostPowersInScope} -> {applyResult.BoostPowersMatched}/{applyResult.BoostPowersCreated}/{applyResult.BoostPowersUpdated}\r\n" +
                $"Set_Bonus powers in scope matched/created/updated: {applyResult.SetBonusPowersInScope} -> {applyResult.SetBonusPowersMatched}/{applyResult.SetBonusPowersCreated}/{applyResult.SetBonusPowersUpdated}\r\n" +
                $"Scoped power legality rebuilt/changed/preserved/empty/unresolved/unknown-labels: {applyResult.ScopedPowerEnhancementLegalityRebuilt}/{applyResult.ScopedPowerEnhancementLegalityChanged}/{applyResult.ScopedPowerEnhancementLegalityPreserved}/{applyResult.ScopedPowerEnhancementLegalityEmptyAfterRebuild}/{applyResult.ScopedPowerEnhancementLegalityUnresolvedAfterRebuild}/{applyResult.ScopedPowerEnhancementLegalityUnresolvedLabelCount}\r\n" +
                $"Boost/Set_Bonus legality repair boosts inspected/rebuilt/preserved/unresolved: {applyResult.BoostPowerLegalityRepairInspected}/{applyResult.BoostPowerLegalityRepairRebuilt}/{applyResult.BoostPowerLegalityRepairPreserved}/{applyResult.BoostPowerLegalityRepairUnresolved}\r\n" +
                $"Boost/Set_Bonus legality repair set-bonus inspected/cleared/already-empty/changed: {applyResult.SetBonusPowerLegalityRepairInspected}/{applyResult.SetBonusPowerLegalityRepairCleared}/{applyResult.SetBonusPowerLegalityRepairAlreadyEmpty}/{applyResult.BoostSetBonusPowerLegalityRepairChanged}\r\n" +
                $"Boost power links resolved/missing: {applyResult.EnhancementBoostPowerLinksResolved}/{applyResult.EnhancementBoostPowerLinksMissing}\r\n" +
                $"Set bonus links resolved/missing: {applyResult.EnhancementSetBonusLinksResolved}/{applyResult.EnhancementSetBonusLinksMissing}\r\n" +
                $"Missing Boosts/Set_Bonus powers after import: {applyResult.MissingBoostPowersAfterImport}/{applyResult.MissingSetBonusPowersAfterImport}\r\n" +
                $"Naming mismatch repairs (boost/set-bonus/enh/set/recipe/salvage): {applyResult.BoostPowerAliasRepairs}/{applyResult.SetBonusPowerAliasRepairs}/{applyResult.EnhancementAliasRepairs}/{applyResult.EnhancementSetAliasRepairs}/{applyResult.RecipeAliasRepairs}/{applyResult.SalvageAliasRepairs}\r\n" +
                $"Enhancement icons preserved/assigned/missing: {applyResult.EnhancementIconsPreserved}/{applyResult.EnhancementIconsAssigned}/{applyResult.EnhancementIconsMissing}\r\n\r\n" +
                "The loaded database has been changed in memory. Use the DB editor save button to persist it.\r\n\r\n" +
                "Use Save Report... to write the combined dry-run and apply report.",
                @"Apply Safe Omni Import",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            _lastDryRunResult = null;
            _applyImport.Enabled = false;
        }
        catch (Exception ex)
        {
            _status.Text = @"Safe import failed.";
            RenderPreview(BuildFailurePreview("Safe import failed.", ex, !string.IsNullOrWhiteSpace(_lastReport)));
            _saveReport.Enabled = !string.IsNullOrWhiteSpace(_lastReport);
            SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
        }
        finally
        {
            SetBusy(false, _status.Text);
        }
    }

    private void SaveReport_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_lastReport))
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Title = @"Save Omni Import Report",
            Filter = @"Markdown report (*.md)|*.md|Text report (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = $"{_lastReportFilePrefix}-{DateTime.Now:yyyyMMdd-HHmmss}.md"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            File.WriteAllText(dialog.FileName, _lastReport, Encoding.UTF8);
        }
    }

    private async void MathReport_Click(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Title = @"Save Planner Math Diagnostic Report",
            Filter = @"Markdown report (*.md)|*.md|Text report (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = $"planner-math-report-{DateTime.Now:yyyyMMdd-HHmmss}.md"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        SetBusy(true, @"Generating planner math report...");

        try
        {
            var report = await Task.Run(() => PlannerMathDiagnosticRunner.GenerateReport(new PlannerMathDiagnosticOptions
            {
                OutputPath = dialog.FileName,
                IncludeCurrentBuildComparison = true,
                IncludeVerboseEffectRows = true
            }));

            _lastReport = report;
            _lastPreviewReport = report;
            _lastReportFilePrefix = "planner-math-report";
            RenderPreview(report);
            _saveReport.Enabled = true;
            _status.Text = @"Planner math report generated.";
            SetTerminalProgress(_status.Text, @"Report ready.", succeeded: true);
            MessageBox.Show(
                this,
                $"Planner math diagnostic report saved.\r\n\r\n{dialog.FileName}",
                @"Planner Math Report",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _status.Text = @"Planner math report failed.";
            RenderPreview(BuildFailurePreview("Planner math report failed.", ex, !string.IsNullOrWhiteSpace(_lastReport)));
            SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
        }
        finally
        {
            SetBusy(false, _status.Text);
        }
    }

    private void SetBusy(bool busy, string status)
    {
        _dryRun.Enabled = !busy;
        _browse.Enabled = !busy;
        _mathReport.Enabled = !busy;
        _applyImport.Enabled = !busy && _lastDryRunResult != null;
        UseWaitCursor = busy;
        _status.Text = status;
        if (busy)
        {
            _progress.Value = 0;
            _progressStage.Text = status;
            _progressDetail.Text = @"Starting...";
            _progressToolTip.SetToolTip(_progressDetail, _progressDetail.Text);
        }
    }

    private void ClearImportState(bool runCleanup)
    {
        PersistExportRoot(_exportRoot.Text.Trim());

        _lastDryRunResult = null;
        _lastExportRoot = string.Empty;
        _lastReport = string.Empty;
        _lastPreviewReport = string.Empty;
        _lastReportFilePrefix = "omni-import-report";

        if (!_report.IsDisposed)
        {
            _report.Clear();
            _report.ClearUndo();
        }

        if (!_applyImport.IsDisposed)
        {
            _applyImport.Enabled = false;
        }

        if (!_saveReport.IsDisposed)
        {
            _saveReport.Enabled = false;
        }

        if (runCleanup)
        {
            ReleaseImportMemory();
        }
    }

    private void UpdateProgress(OmniImportProgress progress)
    {
        _progress.Value = Math.Max(_progress.Value, progress.ClampedPercent);
        _progressStage.Text = ProgressStageText(progress);
        _progressDetail.Text = ProgressDetailText(progress);
        _progressToolTip.SetToolTip(_progressDetail, progress.DisplayText);
        _status.Text = progress.Stage;
    }

    private void LayoutProgressPanel(Control panel)
    {
        var bounds = panel.ClientRectangle;
        var left = panel.Padding.Left;
        var width = Math.Max(0, bounds.Width - panel.Padding.Horizontal);
        var top = panel.Padding.Top;
        _progressStage.SetBounds(left, top, width, 22);
        top += 24;
        _progressDetail.SetBounds(left, top, width, 24);
        top += 30;
        _progress.SetBounds(left, top, width, 18);
    }

    private static string ProgressStageText(OmniImportProgress progress)
    {
        var count = progress.Total > 0
            ? $" - {progress.Current:n0}/{progress.Total:n0}"
            : string.Empty;
        return $"{progress.ClampedPercent}% - {progress.Stage}{count}";
    }

    private static string ProgressDetailText(OmniImportProgress progress)
    {
        if (!string.IsNullOrWhiteSpace(progress.Detail))
        {
            return progress.Detail;
        }

        return progress.Total > 0
            ? $"{progress.Current:n0} of {progress.Total:n0}"
            : string.Empty;
    }

    private static Task<T> RunImportWorkAsync<T>(Func<T> work)
    {
        return Task.Factory.StartNew(() =>
        {
            var thread = Thread.CurrentThread;
            var originalPriority = thread.Priority;
            try
            {
                thread.Priority = ThreadPriority.AboveNormal;
                return work();
            }
            finally
            {
                thread.Priority = originalPriority;
            }
        }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private static void ReleaseImportMemory()
    {
        try
        {
            GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true, compacting: true);
            EmptyWorkingSet(Environment.ProcessId);
        }
        catch
        {
            // Memory pressure cleanup is best-effort; never fail an import because GC compaction was unavailable.
        }
    }

    private static void EmptyWorkingSet(int processId)
    {
        using var process = System.Diagnostics.Process.GetProcessById(processId);
        _ = EmptyWorkingSet(process.Handle);
    }

    [DllImport("psapi.dll")]
    private static extern bool EmptyWorkingSet(nint hProcess);

    protected override void Dispose(bool disposing)
    {
        if (!_disposedState)
        {
            if (disposing)
            {
                ClearImportState(runCleanup: false);
                _progressToolTip.Dispose();
            }

            _disposedState = true;
        }

        base.Dispose(disposing);
        if (disposing)
        {
            ReleaseImportMemory();
        }
    }

    private void SetTerminalProgress(string status, string detail, bool succeeded)
    {
        if (succeeded)
        {
            _progress.Value = 100;
        }

        _progressStage.Text = status;
        _progressDetail.Text = detail;
        _progressToolTip.SetToolTip(_progressDetail, detail);
    }

    private void RenderPreview(string preview)
    {
        _lastPreviewReport = preview;
        _report.Text = preview;
    }

    private static string BuildRunningPreview(string operationName, string exportRoot)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {operationName} Running");
        builder.AppendLine();
        builder.AppendLine($"- Export root: {exportRoot}");
        builder.AppendLine("- Preview will refresh when the current run completes.");
        builder.AppendLine("- Save Report keeps the last successful full report until this run succeeds.");
        return builder.ToString();
    }

    private static string BuildFailurePreview(string title, Exception ex, bool hasSavedReport)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {title}");
        builder.AppendLine();
        builder.AppendLine($"- {ex.GetType().Name}: {ex.Message}");
        if (hasSavedReport)
        {
            builder.AppendLine("- The last successful full report is still available through Save Report...");
        }

        if (!string.IsNullOrWhiteSpace(ex.StackTrace))
        {
            builder.AppendLine();
            builder.AppendLine("## Stack Trace");
            foreach (var line in ex.StackTrace.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries).Take(20))
            {
                builder.AppendLine($"- {line.Trim()}");
            }
        }

        return builder.ToString();
    }

    private static string BuildFullDryRunReport(string exportRoot, OmniImportResult result)
    {
        var builder = new StringBuilder();
        builder.AppendLine(result.Report.ToMarkdown());
        builder.AppendLine();
        builder.AppendLine("## Import Context");
        builder.AppendLine($"- Export root: {exportRoot}");
        builder.AppendLine($"- Class attributes loaded: {result.ClassAttributes.Count}");
        builder.AppendLine($"- Actors classified: {result.Actors.Count}");
        return builder.ToString();
    }

    private static string BuildDryRunPreview(string exportRoot, OmniImportResult result, UiRenderMetrics metrics)
    {
        var builder = new StringBuilder();
        builder.AppendLine(result.Report.ToPreviewMarkdown(exportRoot, result.ClassAttributes.Count, result.Actors.Count));
        AppendPreviewDiagnostics(builder, metrics);
        return builder.ToString();
    }

    private static string BuildApplyPreview(string exportRoot, OmniImportResult dryRunResult, OmniApplyResult applyResult, UiRenderMetrics metrics)
    {
        var builder = new StringBuilder();
        builder.AppendLine(dryRunResult.Report.ToPreviewMarkdown(exportRoot, dryRunResult.ClassAttributes.Count, dryRunResult.Actors.Count));
        builder.AppendLine();
        builder.AppendLine(applyResult.ToPreviewMarkdown());
        AppendPreviewDiagnostics(builder, metrics);
        return builder.ToString();
    }

    private static void AppendPreviewDiagnostics(StringBuilder builder, UiRenderMetrics metrics)
    {
        builder.AppendLine();
        builder.AppendLine("## UI Diagnostics");
        builder.AppendLine($"- Import elapsed: {metrics.ImportElapsed.TotalMilliseconds:n0} ms");
        builder.AppendLine($"- Full report build: {metrics.FullReportBuildElapsed.TotalMilliseconds:n0} ms");
        builder.AppendLine($"- Preview build: {metrics.PreviewBuildElapsed.TotalMilliseconds:n0} ms");
        builder.AppendLine($"- Preview render: {metrics.PreviewRenderElapsed.TotalMilliseconds:n0} ms");
        builder.AppendLine($"- Preview length: {metrics.PreviewLength:n0} chars");
        builder.AppendLine($"- Full report length: {metrics.FullReportLength:n0} chars");
        builder.AppendLine($"- Progress events applied: {metrics.ProgressEventsApplied:n0}");
    }

    private static string ResolveInitialExportRoot()
    {
        var savedPath = Settings.Default.LastOmniExportRoot?.Trim() ?? string.Empty;
        if (Directory.Exists(savedPath))
        {
            return savedPath;
        }

        return Directory.Exists(@"L:\Test-Omni\Homecoming-2026.0409")
            ? @"L:\Test-Omni\Homecoming-2026.0409"
            : string.Empty;
    }

    private static void PersistExportRoot(string exportRoot)
    {
        var normalizedPath = exportRoot?.Trim() ?? string.Empty;
        if (string.Equals(Settings.Default.LastOmniExportRoot ?? string.Empty, normalizedPath, StringComparison.Ordinal))
        {
            return;
        }

        Settings.Default.LastOmniExportRoot = normalizedPath;
        Settings.Default.Save();
    }

    private sealed record UiRenderMetrics(
        TimeSpan ImportElapsed,
        TimeSpan FullReportBuildElapsed,
        TimeSpan PreviewBuildElapsed,
        TimeSpan PreviewRenderElapsed,
        int PreviewLength,
        int FullReportLength,
        int ProgressEventsApplied)
    {
        public static UiRenderMetrics Pending(
            TimeSpan importElapsed,
            TimeSpan fullReportBuildElapsed,
            int progressEventsApplied,
            int fullReportLength)
        {
            return new UiRenderMetrics(
                importElapsed,
                fullReportBuildElapsed,
                TimeSpan.Zero,
                TimeSpan.Zero,
                0,
                fullReportLength,
                progressEventsApplied);
        }
    }

    private sealed class UiProgressBridge : IProgress<OmniImportProgress>, IAsyncDisposable
    {
        private readonly Control _owner;
        private readonly Action<OmniImportProgress> _update;
        private readonly object _sync = new();
        private readonly List<Task> _pending = [];
        private bool _disposed;

        public UiProgressBridge(Control owner, Action<OmniImportProgress> update)
        {
            _owner = owner;
            _update = update;
        }

        public int AppliedCount { get; private set; }

        public void Report(OmniImportProgress value)
        {
            Task? task = null;
            lock (_sync)
            {
                if (_disposed || _owner.IsDisposed)
                {
                    return;
                }

                if (!_owner.IsHandleCreated || !_owner.InvokeRequired)
                {
                    _update(value);
                    AppliedCount++;
                    return;
                }

                var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                _owner.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (!_owner.IsDisposed)
                        {
                            _update(value);
                            AppliedCount++;
                        }

                        completion.TrySetResult();
                    }
                    catch (Exception ex)
                    {
                        completion.TrySetException(ex);
                    }
                }));
                task = completion.Task;
                _pending.Add(task);
            }

            if (task != null)
            {
                _ = task.ContinueWith(_ =>
                {
                    lock (_sync)
                    {
                        _pending.Remove(task);
                    }
                }, TaskScheduler.Default);
            }
        }

        public async ValueTask DisposeAsync()
        {
            lock (_sync)
            {
                _disposed = true;
            }

            await FlushAsync();
        }

        public Task FlushAsync()
        {
            Task[] pending;
            lock (_sync)
            {
                pending = _pending.ToArray();
            }

            return pending.Length == 0 ? Task.CompletedTask : Task.WhenAll(pending);
        }
    }
}
