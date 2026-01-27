namespace MRB_Boostrap.Models;

public sealed class BootstrapArguments
{
    public bool ShowHelp { get; init; }
    public string? ManifestPath { get; init; }
    public string? RollbackTarget { get; init; }
    public BootstrapMode Mode { get; init; }

    public static BootstrapArguments Parse(string[] args)
    {
        if (args.Length == 0)
        {
            args = ["--help"];
        }

        string? manifest = null;
        string? rollback = null;
        bool help = args.Any(a => a.Equals("--help", StringComparison.OrdinalIgnoreCase) || a.Equals("-h"));

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--manifest", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                manifest = args[i + 1];
                i++;
            }
            else if (args[i].Equals("--rollback", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                rollback = args[i + 1];
                i++;
            }
        }

        var mode = help ? BootstrapMode.UiTest : // Not actually running, but halts early
            rollback != null ? BootstrapMode.Rollback :
            manifest != null ? BootstrapMode.Patch :
            BootstrapMode.UiTest;

        return new BootstrapArguments
        {
            ManifestPath = manifest,
            RollbackTarget = rollback,
            ShowHelp = help,
            Mode = mode
        };
    }

    public bool IsRollback => Mode == BootstrapMode.Rollback;
    public bool IsPatch => Mode == BootstrapMode.Patch;
    public bool IsUiTest => Mode == BootstrapMode.UiTest;
}