using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;
using MRB_Boostrap.Services;
using MRB_Boostrap.UI;
using MRB_Boostrap.Utilities;

namespace MRB_Boostrap;

public sealed class RestoreApp
{
    private readonly IRestoreFlowManager _flow;
    private readonly BootstrapArguments _args;
    private readonly IUIManager _ui;
    private readonly ILogger<RestoreApp> _logger;

    public RestoreApp(IRestoreFlowManager flow, BootstrapArguments args, IUIManager ui, ILogger<RestoreApp> logger)
    {
        _flow = flow;
        _args = args;
        _ui = ui;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        string target = _args.RollbackTarget!;
        string type = target.Equals("application", StringComparison.OrdinalIgnoreCase) ? "Application" : "Database";

        _logger.LogInformation("Initiating restore for {Target} ({Type})", target, type);
        int result = await _flow.ExecuteRestoreAsync(target, type, CancellationToken.None);


        if (result == 0)
        {
            _logger.LogInformation("Restore succeeded. Restarting Mids Reborn...");
            _ui.UpdateStatus("Restarting Mids Reborn...");
            await Task.Delay(1000);
            ProcessUtils.StartMidsReborn();
        }
        else
        {
            _logger.LogWarning("Restore failed or was cancelled for {Target}", target);
        }
    }
}