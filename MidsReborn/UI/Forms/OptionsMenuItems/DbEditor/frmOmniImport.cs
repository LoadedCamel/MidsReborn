using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Omni;
using Mids_Reborn.Properties;
using Newtonsoft.Json;

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
    private readonly CheckBox _freshScopedImport;
    private readonly Label _status;
    private readonly ProgressBar _progress;
    private readonly Label _progressStage;
    private readonly Label _progressDetail;
    private readonly Label _progressFooter;
    private readonly ToolTip _progressToolTip;
    private OmniImportSession? _currentSession;
    private OmniImportResult? _lastDryRunResult;
    private OmniApplyResult? _lastApplyResult;
    private string _lastExportRoot = string.Empty;
    private string _lastReport = string.Empty;
    private string _lastReportJson = string.Empty;
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
            Text = @"Analyze Export is optional. Apply Safe Import will analyze automatically when needed and reuse cached analysis for the current export.",
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
        _exportRoot.TextChanged += (_, _) => HandleExportRootChanged();
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

        var optionsPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 30,
            Padding = new Padding(12, 0, 12, 0),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false
        };

        _freshScopedImport = new CheckBox
        {
            AutoSize = true,
            ForeColor = Color.White,
            Text = @"Fresh scoped import (replace in-scope powers and powersets before apply)",
            UseVisualStyleBackColor = true
        };
        optionsPanel.Controls.Add(_freshScopedImport);

        var actionPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 44,
            Padding = new Padding(12, 4, 12, 4),
            FlowDirection = FlowDirection.LeftToRight
        };

        _dryRun = new Button
        {
            Text = @"Analyze Export",
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
            Enabled = Directory.Exists(_exportRoot.Text.Trim()),
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
            Height = 112,
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

        _progressFooter = new Label
        {
            Text = string.Empty,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Color.Silver,
            AutoEllipsis = true,
            UseMnemonic = false
        };

        _progress = new ProgressBar
        {
            Minimum = 0,
            Maximum = 100,
            Value = 0,
            Style = ProgressBarStyle.Continuous
        };
        progressPanel.Controls.Add(_progressStage);
        progressPanel.Controls.Add(_progressDetail);
        progressPanel.Controls.Add(_progressFooter);
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
        Controls.Add(optionsPanel);
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
        var exportRoot = GetSelectedExportRoot();
        if (!Directory.Exists(exportRoot))
        {
            ShowInlineValidationFailure(@"Select a valid Omni export folder first.");
            return;
        }

        PersistExportRoot(exportRoot);

        var previousSession = _currentSession;
        SetBusy(true, @"Analyzing Omni export...");
        RenderPreview(BuildRunningPreview(
            "Analyze Export",
            exportRoot,
            _currentSession,
            DescribeAnalyzeRunMode(previousSession, exportRoot)));
        ResetRenderedArtifacts(keepSession: true);

        try
        {
            await using var progress = new UiProgressBridge(this, UpdateProgress);

            var importTimer = Stopwatch.StartNew();
            var session = await RunImportWorkAsync(() => new OmniImporter().AnalyzeExport(exportRoot, _currentSession, progress));
            importTimer.Stop();
            await progress.FlushAsync();
            var runMode = DescribeCompletedAnalyzeRunMode(previousSession, session);
            _currentSession = session;
            SyncLegacyStateFromSession(session, includeApplyResult: false);

            var previewBuildTimer = Stopwatch.StartNew();
            var preview = BuildDryRunPreview(exportRoot, session.AnalysisResult, UiRenderMetrics.Pending(
                importTimer.Elapsed,
                TimeSpan.Zero,
                progress.AppliedCount,
                0), runMode);
            previewBuildTimer.Stop();

            var renderTimer = Stopwatch.StartNew();
            RenderPreview(preview);
            renderTimer.Stop();

            var finalMetrics = new UiRenderMetrics(
                importTimer.Elapsed,
                TimeSpan.Zero,
                previewBuildTimer.Elapsed,
                renderTimer.Elapsed,
                preview.Length,
                0,
                progress.AppliedCount);
            var finalPreview = BuildDryRunPreview(exportRoot, session.AnalysisResult, finalMetrics, runMode);
            RenderPreview(finalPreview);
            CachePreview(finalPreview);
            _lastReportFilePrefix = "omni-import-dry-run";
            _status.Text = @"Analysis complete. No database changes were made.";
            SetTerminalProgress(_status.Text, @"Preview updated.", succeeded: true);
        }
        catch (Exception ex)
        {
            _status.Text = @"Analyze export failed.";
            RenderPreview(BuildFailurePreview("Analyze export failed.", ex, CanSaveReport()));
            _saveReport.Enabled = CanSaveReport();
            SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
        }
        finally
        {
            SetBusy(false, _status.Text);
        }
    }

    private async void ApplyImport_Click(object? sender, EventArgs e)
    {
        var exportRoot = GetSelectedExportRoot();
        var freshScopedImport = _freshScopedImport.Checked;
        if (!Directory.Exists(exportRoot))
        {
            ShowInlineValidationFailure(@"Select a valid Omni export folder first.");
            return;
        }

        PersistExportRoot(exportRoot);
        var previousSession = _currentSession;
        SetBusy(true, @"Analyzing Omni export...");
        RenderPreview(BuildRunningPreview(
            "Apply Safe Import",
            exportRoot,
            _currentSession,
            DescribeApplyRunMode(previousSession, exportRoot, hasCachedApplyResult: previousSession?.ApplyResult != null, freshScopedImport)));
        ResetRenderedArtifacts(keepSession: true);

        OmniImportSession? session = null;
        TimeSpan analysisElapsed = TimeSpan.Zero;

        try
        {
            var importer = new OmniImporter();
            await using (var progress = new UiProgressBridge(this, UpdateProgress))
            {
                var analysisTimer = Stopwatch.StartNew();
                session = await RunImportWorkAsync(() => importer.AnalyzeExport(exportRoot, _currentSession, progress));
                analysisTimer.Stop();
                analysisElapsed = analysisTimer.Elapsed;
                await progress.FlushAsync();
                var analyzeMode = DescribeCompletedApplyAnalyzeMode(previousSession, session, freshScopedImport);
                _currentSession = session;
                SyncLegacyStateFromSession(session, includeApplyResult: false);
                RenderPreview(BuildRunningPreview(
                    "Apply Safe Import",
                    exportRoot,
                    session,
                    analyzeMode,
                    session.AnalysisResult));
            }
        }
        catch (Exception ex)
        {
            _status.Text = @"Analyze export failed.";
            RenderPreview(BuildFailurePreview("Analyze export failed.", ex, CanSaveReport()));
            SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
            SetBusy(false, _status.Text);
            return;
        }

        var dryRunResult = session.AnalysisResult;
        SetBusy(true, @"Applying safe import...");
        RenderPreview(BuildRunningPreview(
            "Apply Safe Import",
            exportRoot,
            session,
            DescribeApplyExecutionMode(previousSession, session, freshScopedImport),
            dryRunResult));

        try
        {
            await using var progress = new UiProgressBridge(this, UpdateProgress);

            var applyTimer = Stopwatch.StartNew();
            var applyResult = await RunImportWorkAsync(() =>
                new OmniImporter().ApplySafeImport(DatabaseAPI.Database, session, progress, freshScopedImport));
            applyTimer.Stop();
            await progress.FlushAsync();
            _currentSession = session;
            SyncLegacyStateFromSession(session, includeApplyResult: true);

            // Safe import can change powerset, enhancement, enhancement-set, and picker-facing
            // image assignments. Refresh the full image cache so DB editors opened
            // immediately afterward see current ImageIdx values instead of stale blanks.
            AssetManager.ReloadImages();

            var previewBuildTimer = Stopwatch.StartNew();
            var preview = BuildApplyPreview(exportRoot, dryRunResult, applyResult, UiRenderMetrics.Pending(
                analysisElapsed + applyTimer.Elapsed,
                TimeSpan.Zero,
                progress.AppliedCount,
                0), DescribeApplyExecutionMode(previousSession, session, freshScopedImport));
            previewBuildTimer.Stop();

            var renderTimer = Stopwatch.StartNew();
            RenderPreview(preview);
            renderTimer.Stop();

            var finalMetrics = new UiRenderMetrics(
                analysisElapsed + applyTimer.Elapsed,
                TimeSpan.Zero,
                previewBuildTimer.Elapsed,
                renderTimer.Elapsed,
                preview.Length,
                0,
                progress.AppliedCount);
            var finalPreview = BuildApplyPreview(exportRoot, dryRunResult, applyResult, finalMetrics, DescribeApplyExecutionMode(previousSession, session, freshScopedImport));
            CachePreview(finalPreview);
            RenderPreview(finalPreview);
            _lastReportFilePrefix = "omni-import-safe-import";
            _status.Text = freshScopedImport
                ? @"Fresh scoped import applied. Save Report writes the dry-run plus apply report."
                : @"Safe import applied. Save Report writes the dry-run plus apply report.";
            SetTerminalProgress(_status.Text, @"Preview updated.", succeeded: true);
        }
        catch (Exception ex)
        {
            _status.Text = @"Safe import failed.";
            RenderPreview(BuildFailurePreview("Safe import failed.", ex, CanSaveReport()));
            _saveReport.Enabled = CanSaveReport();
            SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
        }
        finally
        {
            SetBusy(false, _status.Text);
        }
    }

    private async void SaveReport_Click(object? sender, EventArgs e)
    {
        if (!CanSaveReport())
        {
            return;
        }

        var hasJsonReport = CanBuildJsonReport();
        using var dialog = new SaveFileDialog
        {
            Title = @"Save Omni Import Report",
            Filter = hasJsonReport
                ? @"JSON report (*.json)|*.json|Markdown report (*.md)|*.md|Text report (*.txt)|*.txt|All files (*.*)|*.*"
                : @"Markdown report (*.md)|*.md|Text report (*.txt)|*.txt|All files (*.*)|*.*",
            FileName = $"{_lastReportFilePrefix}-{DateTime.Now:yyyyMMdd-HHmmss}.{(hasJsonReport ? "json" : "md")}"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var extension = Path.GetExtension(dialog.FileName);
            var saveJson = string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase) ||
                           (string.IsNullOrWhiteSpace(extension) && dialog.FilterIndex == 1);

            try
            {
                SetBusy(true, @"Building report...");
                _progress.Style = ProgressBarStyle.Marquee;
                _progress.MarqueeAnimationSpeed = 30;
                _progressStage.Text = saveJson ? @"Building JSON report..." : @"Building markdown report...";
                _progressDetail.Text = @"Serializing report content...";
                _progressFooter.Text = string.Empty;
                _progressToolTip.SetToolTip(_progressDetail, _progressDetail.Text);
                await Task.Yield();

                var buildTimer = Stopwatch.StartNew();
                var content = await Task.Run(() => GetOrBuildReport(saveJson));
                buildTimer.Stop();

                _progressStage.Text = @"Saving report...";
                _progressDetail.Text = $@"Writing {content.Length:n0} characters to disk...";
                _progressFooter.Text = string.Empty;
                _progressToolTip.SetToolTip(_progressDetail, _progressDetail.Text);
                await Task.Run(() => File.WriteAllText(dialog.FileName, content, Encoding.UTF8));
                _status.Text = @"Report saved.";
                SetTerminalProgress(
                    _status.Text,
                    $@"{(saveJson ? "JSON" : "Markdown")} report saved ({content.Length:n0} chars, build {buildTimer.Elapsed.TotalMilliseconds:n0} ms).",
                    succeeded: true);
            }
            catch (Exception ex)
            {
                _status.Text = @"Report save failed.";
                RenderPreview(BuildFailurePreview("Report save failed.", ex, CanSaveReport()));
                SetTerminalProgress(_status.Text, @"See preview for failure details.", succeeded: false);
            }
            finally
            {
                _progress.Style = ProgressBarStyle.Continuous;
                _progress.MarqueeAnimationSpeed = 0;
                SetBusy(false, _status.Text);
            }
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
            _lastReportJson = string.Empty;
            _lastPreviewReport = report;
            _lastReportFilePrefix = "planner-math-report";
            _currentSession = null;
            _lastDryRunResult = null;
            _lastApplyResult = null;
            _lastExportRoot = string.Empty;
            RenderPreview(report);
            _saveReport.Enabled = true;
            _status.Text = @"Planner math report generated.";
            SetTerminalProgress(_status.Text, @"Report ready.", succeeded: true);
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
        _applyImport.Enabled = !busy && Directory.Exists(GetSelectedExportRoot());
        _saveReport.Enabled = !busy && CanSaveReport();
        UseWaitCursor = busy;
        _status.Text = status;
        if (busy)
        {
            _progress.Value = 0;
            _progressStage.Text = status;
            _progressDetail.Text = @"Preparing import session...";
            _progressFooter.Text = string.Empty;
            _progressToolTip.SetToolTip(_progressDetail, _progressDetail.Text);
        }
    }

    private void ShowInlineValidationFailure(string message)
    {
        _status.Text = message;
        _progress.Value = 0;
        RenderPreview(BuildStatusPreview("Omni Import", message, GetSelectedExportRoot(), _currentSession));
        _saveReport.Enabled = CanSaveReport();
        SetTerminalProgress(_status.Text, @"Correct the export path and try again.", succeeded: false);
    }

    private void ClearImportState(bool runCleanup)
    {
        PersistExportRoot(_exportRoot.Text.Trim());

        _currentSession = null;
        _lastDryRunResult = null;
        _lastApplyResult = null;
        _lastExportRoot = string.Empty;
        _lastReport = string.Empty;
        _lastReportJson = string.Empty;
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
        _progress.Value = progress.ClampedPercent;
        _progressStage.Text = ProgressStageText(progress);
        _progressDetail.Text = ProgressDetailText(progress);
        _progressFooter.Text = ProgressFooterText(progress);
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
        top += 24;
        _progressFooter.SetBounds(left, top, width, 18);
        top += 24;
        _progress.SetBounds(left, top, width, 18);
    }

    private static string ProgressStageText(OmniImportProgress progress)
    {
        return $"{progress.ClampedPercent}% - {progress.Stage}";
    }

    private static string ProgressDetailText(OmniImportProgress progress)
    {
        return !string.IsNullOrWhiteSpace(progress.Detail)
            ? progress.Detail
            : "Working...";
    }

    private static string ProgressFooterText(OmniImportProgress progress)
    {
        return progress.Total > 0
            ? $"{progress.Current:n0} of {progress.Total:n0}"
            : string.Empty;
    }

    private static Task<T> RunImportWorkAsync<T>(Func<T> work)
    {
        return Task.Run(work);
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
        _progressFooter.Text = string.Empty;
        _progressToolTip.SetToolTip(_progressDetail, detail);
    }

    private void RenderPreview(string preview)
    {
        _lastPreviewReport = preview;
        _report.Text = preview;
    }

    private static string BuildRunningPreview(
        string operationName,
        string exportRoot,
        OmniImportSession? session = null,
        string runMode = "",
        OmniImportResult? preflight = null)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {operationName} Running");
        builder.AppendLine();
        builder.AppendLine($"- Export root: {exportRoot}");
        builder.AppendLine($"- Session state: {DescribeSessionState(session, exportRoot)}");
        if (!string.IsNullOrWhiteSpace(runMode))
        {
            builder.AppendLine($"- Run mode: {runMode}");
        }

        if (preflight != null)
        {
            builder.AppendLine();
            builder.AppendLine("## Preflight Summary");
            builder.AppendLine($"- Scoped powers in scope: {preflight.Report.PowersInScope:n0}");
            builder.AppendLine($"- Unsupported power requirements: {preflight.Report.UnsupportedPowerRequirementCount:n0}");
            builder.AppendLine($"- Unknown effect mappings: {preflight.Report.UnknownEffectMappings.Count:n0}");
            builder.AppendLine($"- Unknown attribute mappings: {preflight.Report.UnknownAttribMappings.Count:n0}");
            builder.AppendLine($"- Enhancement definitions / sets discovered: {preflight.Report.EnhancementDefinitionsDiscovered:n0} / {preflight.Report.EnhancementSetsDiscovered:n0}");
            builder.AppendLine($"- Missing enhancement power links: {(preflight.Report.EnhancementBoostPowerLinksMissingDryRun + preflight.Report.EnhancementSetBonusLinksMissingDryRun):n0}");
        }

        builder.AppendLine();
        builder.AppendLine("## Status");
        builder.AppendLine("- Progress and preview will refresh as the current run advances.");
        builder.AppendLine("- Save Report reuses the last successful cached analysis/apply report until this run succeeds.");
        return builder.ToString();
    }

    private static string BuildStatusPreview(string title, string status, string exportRoot, OmniImportSession? session = null)
    {
        var builder = new StringBuilder();
        builder.AppendLine($"# {title}");
        builder.AppendLine();
        builder.AppendLine($"- Status: {status}");
        builder.AppendLine($"- Export root: {exportRoot}");
        builder.AppendLine($"- Session state: {DescribeSessionState(session, exportRoot)}");
        builder.AppendLine("- Analyze Export and Apply Safe Import remain available after the path is corrected.");
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

    private static string BuildDryRunJsonReport(string exportRoot, OmniImportResult result)
    {
        return JsonConvert.SerializeObject(new
        {
            report_type = "omni-import-dry-run",
            export_root = exportRoot,
            class_attributes_loaded = result.ClassAttributes.Count,
            actors_classified = result.Actors.Count,
            summary = result.Report.Summary,
            report = result.Report
        }, Formatting.Indented);
    }

    private static string BuildSafeImportMarkdownReport(string exportRoot, OmniImportResult dryRunResult, OmniApplyResult applyResult)
    {
        return $"{BuildFullDryRunReport(exportRoot, dryRunResult)}{Environment.NewLine}{Environment.NewLine}{applyResult.ToMarkdown()}";
    }

    private static string BuildSafeImportJsonReport(string exportRoot, OmniImportResult dryRunResult, OmniApplyResult applyResult)
    {
        return JsonConvert.SerializeObject(new
        {
            report_type = "omni-import-safe-import",
            export_root = exportRoot,
            summary = OmniReportLegibility.BuildSafeImportSummary(dryRunResult.Report, applyResult),
            dry_run = new
            {
                report_type = "omni-import-dry-run",
                export_root = exportRoot,
                class_attributes_loaded = dryRunResult.ClassAttributes.Count,
                actors_classified = dryRunResult.Actors.Count,
                report = dryRunResult.Report
            },
            apply = applyResult
        }, Formatting.Indented);
    }

    private static string BuildDryRunPreview(string exportRoot, OmniImportResult result, UiRenderMetrics metrics, string runMode = "")
    {
        var builder = new StringBuilder();
        builder.AppendLine(result.Report.ToPreviewMarkdown(exportRoot, result.ClassAttributes.Count, result.Actors.Count));
        AppendPreviewSummary(builder, metrics, runMode);
        return builder.ToString();
    }

    private static string BuildApplyPreview(string exportRoot, OmniImportResult dryRunResult, OmniApplyResult applyResult, UiRenderMetrics metrics, string runMode = "")
    {
        var builder = new StringBuilder();
        builder.AppendLine(dryRunResult.Report.ToPreviewMarkdown(exportRoot, dryRunResult.ClassAttributes.Count, dryRunResult.Actors.Count));
        builder.AppendLine();
        builder.AppendLine(applyResult.ToPreviewMarkdown());
        AppendPreviewSummary(builder, metrics, runMode);
        return builder.ToString();
    }

    private static void AppendPreviewSummary(StringBuilder builder, UiRenderMetrics metrics, string runMode)
    {
        builder.AppendLine();
        builder.AppendLine("## Run Summary");
        if (!string.IsNullOrWhiteSpace(runMode))
        {
            builder.AppendLine($"- Run mode: {runMode}");
        }

        builder.AppendLine($"- Total run time: {metrics.ImportElapsed:mm\\:ss}");
        builder.AppendLine($"- Progress updates applied: {metrics.ProgressEventsApplied:n0}");
        builder.AppendLine(metrics.FullReportLength > 0 || metrics.FullReportBuildElapsed > TimeSpan.Zero
            ? $"- Full report: ready ({metrics.FullReportBuildElapsed.TotalMilliseconds:n0} ms build)"
            : "- Full report: ready on demand through Save Report...");
        builder.AppendLine("- Database changes are only persisted when you save from the DB editor.");
    }

    private bool CanSaveReport()
    {
        return (_currentSession != null && (_currentSession.AnalysisResult != null || _currentSession.ApplyResult != null)) ||
               !string.IsNullOrWhiteSpace(_lastReport) ||
               !string.IsNullOrWhiteSpace(_lastReportJson) ||
               _lastDryRunResult != null ||
               _lastApplyResult != null;
    }

    private bool CanBuildJsonReport()
    {
        return (_currentSession != null && (_currentSession.AnalysisResult != null || _currentSession.ApplyResult != null)) ||
               !string.IsNullOrWhiteSpace(_lastReportJson) ||
               _lastDryRunResult != null ||
               _lastApplyResult != null;
    }

    private string GetOrBuildReport(bool json)
    {
        if (_currentSession != null)
        {
            if (json && !string.IsNullOrWhiteSpace(_currentSession.CachedJsonReport))
            {
                return _currentSession.CachedJsonReport;
            }

            if (!json && !string.IsNullOrWhiteSpace(_currentSession.CachedMarkdownReport))
            {
                return _currentSession.CachedMarkdownReport;
            }

            if (_currentSession.AnalysisResult != null && !string.IsNullOrWhiteSpace(_currentSession.ExportRoot))
            {
                var report = json
                    ? _currentSession.ApplyResult == null
                        ? BuildDryRunJsonReport(_currentSession.ExportRoot, _currentSession.AnalysisResult)
                        : BuildSafeImportJsonReport(_currentSession.ExportRoot, _currentSession.AnalysisResult, _currentSession.ApplyResult)
                    : _currentSession.ApplyResult == null
                        ? BuildFullDryRunReport(_currentSession.ExportRoot, _currentSession.AnalysisResult)
                        : BuildSafeImportMarkdownReport(_currentSession.ExportRoot, _currentSession.AnalysisResult, _currentSession.ApplyResult);
                if (json)
                {
                    _currentSession.CachedJsonReport = report;
                    _lastReportJson = report;
                }
                else
                {
                    _currentSession.CachedMarkdownReport = report;
                    _lastReport = report;
                }

                return report;
            }
        }

        if (json)
        {
            if (!string.IsNullOrWhiteSpace(_lastReportJson))
            {
                return _lastReportJson;
            }

            if (_lastDryRunResult != null && !string.IsNullOrWhiteSpace(_lastExportRoot))
            {
                _lastReportJson = _lastApplyResult == null
                    ? BuildDryRunJsonReport(_lastExportRoot, _lastDryRunResult)
                    : BuildSafeImportJsonReport(_lastExportRoot, _lastDryRunResult, _lastApplyResult);
                return _lastReportJson;
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(_lastReport))
            {
                return _lastReport;
            }

            if (_lastDryRunResult != null && !string.IsNullOrWhiteSpace(_lastExportRoot))
            {
                _lastReport = _lastApplyResult == null
                    ? BuildFullDryRunReport(_lastExportRoot, _lastDryRunResult)
                    : BuildSafeImportMarkdownReport(_lastExportRoot, _lastDryRunResult, _lastApplyResult);
                return _lastReport;
            }
        }

        return _lastReport;
    }

    private void HandleExportRootChanged()
    {
        if (_currentSession != null &&
            !string.Equals(_currentSession.ExportRoot, GetSelectedExportRoot(), StringComparison.OrdinalIgnoreCase))
        {
            _currentSession = null;
            _lastDryRunResult = null;
            _lastApplyResult = null;
            _lastExportRoot = string.Empty;
            _lastReport = string.Empty;
            _lastReportJson = string.Empty;
        }

        _applyImport.Enabled = Directory.Exists(GetSelectedExportRoot());
        _saveReport.Enabled = CanSaveReport();
    }

    private string GetSelectedExportRoot()
    {
        return _exportRoot.Text.Trim();
    }

    private void ResetRenderedArtifacts(bool keepSession)
    {
        _lastReport = string.Empty;
        _lastReportJson = string.Empty;
        if (keepSession)
        {
            _currentSession?.ClearRenderedArtifacts();
        }
    }

    private void SyncLegacyStateFromSession(OmniImportSession session, bool includeApplyResult)
    {
        _lastDryRunResult = session.AnalysisResult;
        _lastApplyResult = includeApplyResult ? session.ApplyResult : null;
        _lastExportRoot = session.ExportRoot;
        _saveReport.Enabled = true;
        _applyImport.Enabled = Directory.Exists(session.ExportRoot);
    }

    private void CachePreview(string preview)
    {
        _lastPreviewReport = preview;
        if (_currentSession != null)
        {
            _currentSession.CachedPreviewReport = preview;
        }
    }

    private static string DescribeSessionState(OmniImportSession? session, string exportRoot)
    {
        if (session == null)
        {
            return "No cached analysis";
        }

        if (!string.Equals(session.ExportRoot, exportRoot, StringComparison.OrdinalIgnoreCase))
        {
            return $"Cached analysis for {session.ExportRoot}";
        }

        return session.ApplyResult == null
            ? "Analysis cached for current export"
            : "Apply result available for current export";
    }

    private static string DescribeAnalyzeRunMode(OmniImportSession? previousSession, string exportRoot)
    {
        if (previousSession == null)
        {
            return "Building fresh analysis for the selected export.";
        }

        return string.Equals(previousSession.ExportRoot, exportRoot, StringComparison.OrdinalIgnoreCase)
            ? "Checking cached analysis for the current export."
            : $"Cached analysis belongs to {previousSession.ExportRoot}; rebuilding for the selected export.";
    }

    private static string DescribeCompletedAnalyzeRunMode(OmniImportSession? previousSession, OmniImportSession currentSession)
    {
        if (previousSession == null)
        {
            return "Fresh analysis completed for the selected export.";
        }

        return ReferenceEquals(previousSession, currentSession)
            ? "Cached analysis was still current and has been reused."
            : string.Equals(previousSession.ExportRoot, currentSession.ExportRoot, StringComparison.OrdinalIgnoreCase)
                ? "Cached analysis was stale and has been refreshed."
                : "Analysis was rebuilt for the selected export.";
    }

    private static string DescribeApplyRunMode(OmniImportSession? previousSession, string exportRoot, bool hasCachedApplyResult, bool freshScopedImport)
    {
        var description = string.Empty;
        if (previousSession == null)
        {
            description = "No cached analysis found. Apply Safe Import will analyze first, then continue automatically.";
        }
        else if (!string.Equals(previousSession.ExportRoot, exportRoot, StringComparison.OrdinalIgnoreCase))
        {
            description = $"Cached analysis belongs to {previousSession.ExportRoot}; refreshing analysis for the selected export before apply.";
        }
        else
        {
            description = hasCachedApplyResult
                ? "Using cached analysis for the current export before applying updated changes."
                : "Checking cached analysis for the current export before applying.";
        }

        return AppendApplyMode(description, freshScopedImport);
    }

    private static string DescribeCompletedApplyAnalyzeMode(OmniImportSession? previousSession, OmniImportSession currentSession, bool freshScopedImport)
    {
        var description = string.Empty;
        if (previousSession == null)
        {
            description = "Analysis prepared inline for this apply run.";
        }
        else
        {
            description = ReferenceEquals(previousSession, currentSession)
                ? "Using cached analysis for the current export."
                : string.Equals(previousSession.ExportRoot, currentSession.ExportRoot, StringComparison.OrdinalIgnoreCase)
                    ? "Cached analysis was stale and has been refreshed."
                    : "Analysis was rebuilt for the selected export before apply.";
        }

        return AppendApplyMode(description, freshScopedImport);
    }

    private static string DescribeApplyExecutionMode(OmniImportSession? previousSession, OmniImportSession currentSession, bool freshScopedImport)
    {
        var description = string.Empty;
        if (previousSession == null)
        {
            description = "Applying safe import after building fresh analysis.";
        }
        else
        {
            description = ReferenceEquals(previousSession, currentSession)
                ? "Applying safe import using cached analysis."
                : string.Equals(previousSession.ExportRoot, currentSession.ExportRoot, StringComparison.OrdinalIgnoreCase)
                    ? "Applying safe import after refreshing stale cached analysis."
                    : "Applying safe import after rebuilding analysis for the selected export.";
        }

        return AppendApplyMode(description, freshScopedImport);
    }

    private static string AppendApplyMode(string description, bool freshScopedImport)
    {
        return freshScopedImport
            ? $"{description} Fresh scoped import is enabled, so current in-scope powers and powersets will be replaced before apply."
            : $"{description} Incremental mode is enabled, so existing in-scope content will be updated in place.";
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
