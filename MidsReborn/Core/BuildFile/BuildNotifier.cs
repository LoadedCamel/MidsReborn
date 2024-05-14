using Mids_Reborn.Forms.Controls;
using System.Windows.Forms;

namespace Mids_Reborn.Core.BuildFile
{
    public interface IBuildNotifier
    {
        void ShowError(string message);
        DialogResult ShowErrorDialog(string message, string title);
        void ShowWarning(string message);
        DialogResult ShowWarningDialog(string message, string title, bool showIgnore = false);
        void ShowInfo(string message);
        DialogResult ShowInfoDialog(string message, string title);
    }

    public class BuildNotifier : IBuildNotifier
    {
        public void ShowError(string message)
        {
            MessageBoxEx.Show(message, "Error", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
        }

        public DialogResult ShowErrorDialog(string message, string title)
        {
            return MessageBoxEx.ShowDialog(message, title, MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
        }

        public void ShowWarning(string message)
        {
            MessageBoxEx.Show(message, "Warning", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Warning);
        }

        public DialogResult ShowWarningDialog(string message, string title, bool showIgnore = false)
        {
            return MessageBoxEx.ShowDialog(message, title, MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Warning, showIgnore);
        }

        public void ShowInfo(string message)
        {
            MessageBoxEx.Show(message, "Info", MessageBoxEx.MessageBoxExButtons.Ok);
        }

        public DialogResult ShowInfoDialog(string message, string title)
        {
            return MessageBoxEx.ShowDialog(message, title, MessageBoxEx.MessageBoxExButtons.Ok);
        }

    }
}
