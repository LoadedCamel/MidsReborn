using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Forms;
using Mids_Reborn.UIv2;

namespace Mids_Reborn
{
    public class MrbAppContext : ApplicationContext
    {
        public MrbAppContext(string[] args)
        {
            InitializeApplication(args).ConfigureAwait(false);
        }

        private async Task InitializeApplication(string[] args)
        {
            var splashScreen = new Loader();
            splashScreen.Show();

            // Wait for the loader to complete its operations.
            await splashScreen.LoadCompleted;

            // Once the splash screen has completed loading, proceed to show the main form.
            var mainForm = new frmMain(args);
            mainForm.Closed += (sender, e) => { ExitThread(); };

            mainForm.Show();
        }
    }

}