using Microsoft.Extensions.Logging;

namespace MRB_Boostrap.UI;

public sealed class NativeWindowUIManager : IUIManager
{
    private readonly ILogger<NativeWindowUIManager> _logger;

    public NativeWindowUIManager(ILogger<NativeWindowUIManager> logger)
    {
        _logger = logger;
    }

    public async Task DelayForReadabilityAsync(int milliseconds = 1000, CancellationToken cancellationToken = default)
    {
        await Task.Delay(milliseconds, cancellationToken);
    }

    public void UpdateStatus(string message)
    {
        if (ModernWindow.Instance is { } mw)
        {
            mw.PostUpdateStatus(message);
        }
        else
        {
            _logger.LogInformation("UI [Status]: {Message}", message);
        }
    }

    public void UpdateVersion(string version)
    {
        if (ModernWindow.Instance is { } mw)
        {
            mw.PostUpdateVersion(version);
        }
        else
        {
            _logger.LogInformation("UI [Version]: {Version}", version);
        }
    }

    public void UpdateProgress(float percent)
    {
        if (ModernWindow.Instance is { } mw)
        {
            mw.PostUpdateProgress(percent);
        }
        else
        {
            _logger.LogInformation("UI [Progress]: {Percent:P0}", percent);
        }
    }

    public void ShowProgressBar(bool visible)
    {
        if (ModernWindow.Instance is { } mw)
        {
            mw.PostShowProgressBar(visible);
        }
        else
        {
            _logger.LogInformation("UI [ShowProgressBar]: {Visible}", visible);
        }
    }

    public void TriggerFailure()
    {
        ModernWindow.Instance?.PostTriggerFailure();
    }

    public void TriggerCancel()
    {
        ModernWindow.Instance?.PostTriggerCancel();
    }

    public void TriggerRollback()
    {
        ModernWindow.Instance?.PostTriggerRollback();
    }

    public void StartCleanup()
    {
        ModernWindow.Instance?.PostStartCleanup();
    }

    public void Close()
    {
        ModernWindow.Instance?.PostCloseWindow();
    }
}