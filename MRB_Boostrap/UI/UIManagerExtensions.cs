namespace MRB_Boostrap.UI;

public static class UIManagerExtensions
{
    public static async Task ShowStatusAsync(this IUIManager ui, string message, int delayMs, bool? showProgressBar = null, CancellationToken token = default)
    {
        ui.UpdateStatus(message);
        if (showProgressBar.HasValue)
            ui.ShowProgressBar(showProgressBar.Value);

        await ui.DelayForReadabilityAsync(delayMs, token);
    }

    public static void ResetProgress(this IUIManager ui)
    {
        ui.ShowProgressBar(false);
        ui.UpdateProgress(0f);
    }
}