namespace MRB_Boostrap.UI;

public interface IUIManager
{
    Task DelayForReadabilityAsync(int milliseconds = 1000, CancellationToken cancellationToken = default);
    void UpdateStatus(string message);
    void UpdateVersion(string version);
    void UpdateProgress(float percent);
    void ShowProgressBar(bool visible);
    void TriggerFailure();
    void TriggerCancel();
    void TriggerRollback();
    void StartCleanup();
    void Close();
}