namespace MRB_Boostrap;

public static class ModernWindowTestApp
{
    public static void Run()
    {
        ModernWindow window = new();

        // Start simulation in background
        Task.Run(async () =>
        {
            string[] phases =
            [
                "Initializing...",
                "Checking for updates...",
                "Downloading patch...",
                "Validating...",
                "Installing..."
            ];

            for (int i = 0; i < phases.Length; i++)
            {
                window.PostUpdateStatus(phases[i]);
                window.PostShowProgressBar(true);
                window.PostUpdateProgress(i / (float)(phases.Length - 1));
                await Task.Delay(1000);
            }

            // Simulate rollback
            window.PostTriggerRollback();
            window.PostUpdateProgress(0.25f);
            await Task.Delay(800);
            window.PostUpdateProgress(0.65f);
            await Task.Delay(800);
            window.PostUpdateProgress(1.0f);
            await Task.Delay(800);

            window.PostStartCleanup();
            await Task.Delay(1000);

            window.PostUpdateStatus("Test complete. Closing...");
            window.PostShowProgressBar(false);
            await Task.Delay(1500);

            window.PostCloseWindow();
        });

        // ❗ This runs the message loop and blocks until the window closes
        window.Show(); // this wraps message loop inside itself
        Application.Run(); // to ensure form stays alive if Show() exits
    }
}
