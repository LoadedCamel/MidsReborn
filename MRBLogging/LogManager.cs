using Serilog;

namespace MRBLogging;

public static class LogManager
{
    private static bool _isConfigured;

    public static void Configure(string logFilePath, string appName)
    {
        if (_isConfigured) return;

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", appName)
            .WriteTo.File(logFilePath,
                rollingInterval: RollingInterval.Day,
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{Component}] {Message}{NewLine}{Exception}")
            .CreateLogger();

        _isConfigured = true;
    }

    public static ILogger GetLogger(string component)
    {
        return new Logger(component);
    }

    public static void Shutdown()
    {
        Log.CloseAndFlush();
    }
}