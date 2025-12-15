using System.Diagnostics;
using Serilog;

namespace MRB_Boostrap.Utilities;

public static class ProcessUtils
{
    public static bool IsProcessRunning(string processName)
    {
        return Process.GetProcessesByName(processName).Any();
    }

    public static async Task<bool> KillProcessAsync(string processName, CancellationToken cancellationToken = default)
    {
        var processes = Process.GetProcessesByName(processName);
        if (processes.Length == 0)
        {
            Log.Information("No running processes found matching: {Name}", processName);
            return true;
        }

        foreach (var proc in processes)
        {
            try
            {
                Log.Warning("Attempting to kill {Name} (PID: {Pid})", proc.ProcessName, proc.Id);
                if (!proc.CloseMainWindow())
                {
                    Log.Debug("No main window or CloseMainWindow failed — forcing Kill()");
                    proc.Kill();
                }

                await proc.WaitForExitAsync(cancellationToken);
                Log.Information("Successfully closed process {Name} (PID: {Pid})", proc.ProcessName, proc.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to terminate process {Name} (PID: {Pid})", proc.ProcessName, proc.Id);
                return false;
            }
        }

        await Task.Delay(1000, cancellationToken);
        return true;
    }

    public static void StartMidsReborn()
    {
        string exePath = Path.Combine(AppContext.BaseDirectory, "MidsReborn.exe");
        if (!File.Exists(exePath))
        {
            Log.Warning("MidsReborn.exe not found at {Path}. Cannot restart.", exePath);
            return;
        }

        try
        {
            Log.Information("Restarting Mids Reborn...");
            Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = AppContext.BaseDirectory,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to restart Mids Reborn.");
        }
    }
}