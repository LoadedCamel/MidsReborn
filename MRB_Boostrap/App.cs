using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;
using MRB_Boostrap.Services;
using MRB_Boostrap.UI;
using MRB_Boostrap.Utilities;
using Serilog;

namespace MRB_Boostrap;

public sealed class App
{
    private readonly IPatchFlowManager _flowManager;
    private readonly List<UpdateEntry> _entries;
    private readonly IUIManager _ui;
    private readonly ILogger<App> _logger;

    public App(IPatchFlowManager flowManager, List<UpdateEntry> entries, IUIManager ui, ILogger<App> logger)
    {
        _flowManager = flowManager;
        _entries = entries;
        _ui = ui;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        foreach (var entry in _entries)
        {
            int result = await _flowManager.ExecutePatchFlowAsync(entry);
            if (result != 0)
            {
                _logger.LogWarning("Patch failed for {Name}. Skipping restart.", entry.Name);
                return;
            }

            _ui.UpdateStatus("Restarting Mids Reborn...");
            await Task.Delay(1000);

            ProcessUtils.StartMidsReborn();
        }
    }
}