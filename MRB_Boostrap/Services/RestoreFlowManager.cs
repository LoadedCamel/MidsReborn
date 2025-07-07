using Microsoft.Extensions.Logging;
using MRB_Boostrap.UI;
using MRB_Boostrap.Utilities;

namespace MRB_Boostrap.Services;

public sealed class RestoreFlowManager : IRestoreFlowManager
{
    private readonly IBackupManager _backupManager;
    private readonly IUIManager _ui;
    private readonly ILogger<RestoreFlowManager> _logger;
    private readonly LoggingPipeline _pipeline;

    public RestoreFlowManager(
        IBackupManager backupManager,
        IUIManager ui,
        ILogger<RestoreFlowManager> logger, LoggingPipeline pipeline)
    {
        _backupManager = backupManager;
        _ui = ui;
        _logger = logger;
        _pipeline = pipeline;
    }

    public async Task<int> ExecuteRestoreAsync(string name, string patchType, CancellationToken cancellationToken)
    {
        using (_logger.BeginScope("RestoreFlow {Name}", name))
        {
            _logger.LogInformation("Starting manual restore for {Name} ({PatchType})", name, patchType);

            await _ui.ShowStatusAsync($"Restoring {name}...", 500, false, cancellationToken);

            string baseDir = AppContext.BaseDirectory;
            string installPath = patchType.Equals("Application", StringComparison.OrdinalIgnoreCase)
                ? baseDir
                : Path.Combine(baseDir, "Data", name);

            string backupPath = Path.Combine(baseDir, "Backup", name);

            // Ensure Mids Reborn is not running prior to restore
            await _ui.ShowStatusAsync("Checking for running processes...", 500, false, cancellationToken);

            if (ProcessUtils.IsProcessRunning("MidsReborn"))
            {
                _logger.LogWarning("Mids Reborn is running. Attempting to shut it down.");
                await _ui.ShowStatusAsync("Closing Mids Reborn...", 750, false, cancellationToken);

                bool closed = await ProcessUtils.KillProcessAsync("MidsReborn", cancellationToken);
                if (!closed)
                {
                    await _ui.ShowStatusAsync("Please close Mids Reborn and try again.", 1500, false, cancellationToken);
                    return -1;
                }
            }

            _ui.TriggerRollback();
            await _ui.ShowStatusAsync("Rolling back...", 500, true, cancellationToken);
            _ui.UpdateProgress(0f);

            bool restored = await _pipeline.RunStepAsync("Restore backup", () =>
                _backupManager.RestoreBackupAsync(installPath, backupPath, patchType, name, cancellationToken)); 
            

            if (restored)
            {
                _logger.LogInformation("Restore complete for {Name}", name);
                await _ui.ShowStatusAsync("Restore complete.", 1500, false, cancellationToken);
            }
            else
            {
                _logger.LogError("Restore failed for {Name}", name);
                await _ui.ShowStatusAsync("Restore failed.", 1500, false, cancellationToken);
            }

            _ui.ShowProgressBar(false);
            await Task.Delay(1500, cancellationToken);

            return restored ? 0 : -1;
        }
    }
}