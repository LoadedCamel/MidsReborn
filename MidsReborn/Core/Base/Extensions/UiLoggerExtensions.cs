using System;
using System.Windows.Forms;
using MRBLogging;

namespace Mids_Reborn.Core.Base.Extensions
{
    public enum LogSeverity
    {
        Info,
        Warning,
        Error,
        Fatal
    }

    public static class UiLoggerExtensions
    {
        public static void ShowLog(this ILogger logger, string message, LogSeverity severity = LogSeverity.Info)
        {
            switch (severity)
            {
                case LogSeverity.Info:
                    logger.Info(message);
                    break;
                case LogSeverity.Warning:
                    logger.Warning(message);
                    break;
                case LogSeverity.Error:
                    logger.Error(message);
                    break;
                case LogSeverity.Fatal:
                    logger.Fatal(new Exception(message), message); // Wrap string in Exception for fatal
                    break;
            }

            MessageBox.Show(message, GetTitle(severity), MessageBoxButtons.OK, GetIcon(severity));
        }

        public static void ShowLog(this ILogger logger, Exception ex, LogSeverity severity = LogSeverity.Error, string? context = null)
        {
            var message = context ?? ex.Message;

            switch (severity)
            {
                case LogSeverity.Info:
                    logger.Info(message);
                    break;
                case LogSeverity.Warning:
                    logger.Warning(message);
                    break;
                case LogSeverity.Error:
                    logger.Error(ex, message);
                    break;
                case LogSeverity.Fatal:
                    logger.Fatal(ex, message);
                    break;
            }

            MessageBox.Show(message, GetTitle(severity), MessageBoxButtons.OK, GetIcon(severity));
        }

        private static string GetTitle(LogSeverity severity) => severity switch
        {
            LogSeverity.Info => "Information",
            LogSeverity.Warning => "Warning",
            LogSeverity.Error => "Error",
            LogSeverity.Fatal => "Fatal Error",
            _ => "Log"
        };

        private static MessageBoxIcon GetIcon(LogSeverity severity) => severity switch
        {
            LogSeverity.Info => MessageBoxIcon.Information,
            LogSeverity.Warning => MessageBoxIcon.Warning,
            LogSeverity.Error => MessageBoxIcon.Error,
            LogSeverity.Fatal => MessageBoxIcon.Error,
            _ => MessageBoxIcon.None
        };
    }
}
