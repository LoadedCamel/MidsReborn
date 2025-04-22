namespace MRBLogging;

public interface ILogger
{
    void Info(string message);
    void Warning(string message);
    void Error(string message);
    void Error(Exception ex, string? context = null);
    void Fatal(Exception ex, string? context = null);
}