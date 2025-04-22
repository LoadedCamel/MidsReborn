using Serilog;

namespace MRBLogging;

public class Logger : ILogger
{
    private readonly Serilog.ILogger _logger;

    public Logger(string component)
    {
        _logger = Log.ForContext("Component", component);
    }

    public void Info(string message) => _logger.Information(message);
    public void Warning(string message) => _logger.Warning(message);
    public void Error(string message) => _logger.Error(message);
    
    public void Error(Exception ex, string? context = null)
    {
        _logger.Error(ex, string.IsNullOrWhiteSpace(context) ? "An error occurred" : context);
    }

    public void Fatal(Exception ex, string? context = null)
    {
        _logger.Fatal(ex, string.IsNullOrWhiteSpace(context) ? "Fatal error" : context);
    }
}