using System.Windows.Forms;
using Mids_Reborn.UI.Forms;

namespace Mids_Reborn.Core.BuildFile
{
    public interface IBuildNotifier
    {
        void ShowError(string message);
        void ShowWarning(string message);
        DialogResult ShowWarningDialog(string message, string title, bool showIgnore = false);
    }

    public class BuildNotifier : IBuildNotifier
    {
        public void ShowError(string message)
        {
            MessageBoxEx.Show(message, "Error", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
        }

        public void ShowWarning(string message)
        {
            MessageBoxEx.Show(message, "Warning", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Warning);
        }

        public DialogResult ShowWarningDialog(string message, string title, bool showIgnore = false)
        {
            return MessageBoxEx.ShowDialog(message, title, MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Warning, showIgnore);
        }

    }
}
