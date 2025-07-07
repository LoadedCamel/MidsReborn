using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MRB_Boostrap.Models;
using MRB_Boostrap.Services;
using MRB_Boostrap.UI;
using MRB_Boostrap.Utilities;
using Serilog;

namespace MRB_Boostrap;

internal class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "Logs"));

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.File(Path.Combine(AppContext.BaseDirectory, "Logs", "bootstrapper.log"),
                    rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();
        }
        catch (Exception ex)
        {
            // Absolute fallback
            File.WriteAllText("serilog_failed.log", "Failed to initialize Serilog:\n" + ex);
            MessageBox.Show("Fatal error initializing logging: " + ex.Message, "Bootstrapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        try
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) =>
            {
                Log.Logger.Fatal("Unhandled exception: {Exception}", e.ExceptionObject);
                Log.CloseAndFlush();
                MessageBox.Show($"Fatal error:\n{e.ExceptionObject}", "Bootstrapper Crash", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Log.Logger.Information("Bootstrapper starting...");

            var parsedArgs = BootstrapArguments.Parse(args);

            if (parsedArgs.ShowHelp)
            {
                const string help = """
                                    Mids Reborn Bootstrapper Usage:

                                      --manifest <file>       Applies one or more patches described in the manifest JSON.
                                      --rollback <name>       Restores a backup for either 'application' or a database name like 'Homecoming'.
                                      --uitest                Launches the UI in test mode for layout/design verification.
                                      --help | -h             Displays this help message.

                                    Examples:
                                      MRBBootstrap.exe --manifest update_manifest.json
                                      MRBBootstrap.exe --rollback application
                                      MRBBootstrap.exe --rollback Homecoming
                                    """;

                try
                {
                    if (Environment.UserInteractive && Console.OpenStandardOutput() != Stream.Null)
                        Console.WriteLine(help);
                    else
                        MessageBox.Show(help, "MRB Bootstrapper Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch
                {
                    MessageBox.Show(help, "MRB Bootstrapper Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                return;
            }

            // Pre-Host logic for patch mode
            List<UpdateEntry> entries= [];

            if (parsedArgs.IsPatch)
            {
                if (string.IsNullOrWhiteSpace(parsedArgs.ManifestPath))
                {
                    Log.Logger.Error("No manifest path provided.");
                    MessageBox.Show("Usage:\n  MRBBootstrap.exe --manifest <file>", "Missing Arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Log.Logger.Information("Manifest path: {Path}", parsedArgs.ManifestPath);

                try
                {
                    entries = UpdateManifestLoader.Load(parsedArgs.ManifestPath);
                    Log.Logger.Information("Loaded {Count} patch entries.", entries.Count);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Manifest parsing failed.");
                    MessageBox.Show("Invalid patch manifest:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Build DI host
            var builder = Host.CreateApplicationBuilder(args);
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(Log.Logger);

            builder.Services.AddSingleton(parsedArgs);
            builder.Services.AddSingleton(entries);
            builder.Services.AddSingleton<LoggingPipeline>();
            builder.Services.AddSingleton<IPatchFlowManager, PatchFlowManager>();
            builder.Services.AddSingleton<IRestoreFlowManager, RestoreFlowManager>();
            builder.Services.AddSingleton<IFileDownloader, FileDownloader>();
            builder.Services.AddSingleton<IFileStager, FileStager>();
            builder.Services.AddSingleton<IHashValidator, HashValidator>();
            builder.Services.AddSingleton<IFileDecompressor, FileDecompressor>();
            builder.Services.AddSingleton<IFileCompressor, FileCompressor>();
            builder.Services.AddSingleton<IBackupManager, BackupManager>();
            builder.Services.AddSingleton<IUIManager, NativeWindowUIManager>();
            builder.Services.AddSingleton<App>();
            builder.Services.AddSingleton<RestoreApp>();

            using var host = builder.Build();
            var services = host.Services;

            ModernWindow window;
            switch (parsedArgs.Mode)
            {
                case BootstrapMode.Patch:
                {
                    
                    try
                    {
                        Log.Logger.Information("Creating ModernWindow for patch...");
                        window = new ModernWindow();
                        Log.Logger.Information("ModernWindow created.");
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Fatal(ex, "Failed to create the patch window.");
                        MessageBox.Show("Could not create the patch window:\n" + ex.Message, "Bootstrapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            Log.Logger.Information("Starting patch process...");
                                await services.GetRequiredService<App>().RunAsync();
                            Log.Logger.Information("Patch complete. Closing UI.");
                            window.PostCloseWindow();
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Fatal(ex, "Fatal error during patching.");
                        }
                        finally
                        {
                            try
                            {
                                if (File.Exists(parsedArgs.ManifestPath))
                                {
                                    File.Delete(parsedArgs.ManifestPath);
                                    Log.Logger.Information("Deleted manifest file: {Path}", parsedArgs.ManifestPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Warning(ex, "Failed to delete manifest file.");
                            }
                        }
                    });

                    Log.Logger.Information("Showing patch window...");
                    window.Show();
                    Application.Run();
                    
                    break;
                }

                case BootstrapMode.Rollback:
                {
                    try
                    {
                        Log.Logger.Information("Creating ModernWindow for rollback...");
                        window = new ModernWindow();
                        Log.Logger.Information("ModernWindow created.");
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Fatal(ex, "Failed to create the rollback window.");
                        MessageBox.Show("Could not create the patch window:\n" + ex.Message, "Bootstrapper", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            Log.Logger.Information("Starting rollback process...");
                            await services.GetRequiredService<RestoreApp>().RunAsync();
                            Log.Logger.Information("Rollback complete. Closing UI.");
                            window.PostCloseWindow();
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Fatal(ex, "Fatal error during rollback.");
                        }
                    });

                    Log.Logger.Information("Showing rollback window...");
                    window.Show();
                    Application.Run();

                    break;
                }

                case BootstrapMode.UiTest:
                {
                    Log.Logger.Information("Running in UI test mode.");
                    ModernWindowTestApp.Run();

                    break;
                }

                default:
                    Log.Logger.Error("Invalid bootstrap mode.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex, "Fatal crash in Program.Main");
            MessageBox.Show($"Fatal error:\n{ex.Message}", "Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.Logger.Information("Shutting down.");
            Log.CloseAndFlush();
        }
    }
}
