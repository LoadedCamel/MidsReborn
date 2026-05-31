using System;
using System.IO;
using System.Windows.Forms;
using Mids_Reborn.Core.Compatibility;
using Mids_Reborn.UI.Forms.Controls;

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
        void ShowCompatibilitySummary(CompatibilityLoadSummary summary);
        void ShowCompatibilityFailure(CompatibilityFailureReport failureReport);
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

        public void ShowCompatibilitySummary(CompatibilityLoadSummary summary)
        {
            if (summary == null || !summary.HasUserVisibleChanges)
            {
                return;
            }

            var message = summary.BuildDisplayMessage();
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            MessageBoxEx.Show(
                message,
                "Build Conversion Summary",
                MessageBoxEx.MessageBoxExButtons.Ok,
                MessageBoxEx.MessageBoxExIcon.Information);
        }

        public void ShowCompatibilityFailure(CompatibilityFailureReport failureReport)
        {
            if (failureReport == null)
            {
                return;
            }

            var message = failureReport.BuildDisplayMessage();
            var prompt = string.IsNullOrWhiteSpace(message)
                ? "The build could not be converted."
                : message + Environment.NewLine + Environment.NewLine +
                  "Do you want to save diagnostic details for the MRB team?";

            var result = MessageBoxEx.ShowDialog(
                prompt,
                failureReport.Title,
                MessageBoxEx.MessageBoxExButtons.YesNo,
                MessageBoxEx.MessageBoxExIcon.Error);

            if (result != DialogResult.Yes)
            {
                return;
            }

            using var saveDialog = new SaveFileDialog
            {
                Filter = @"JSON report (*.json)|*.json|Text report (*.txt)|*.txt|All files (*.*)|*.*",
                FileName = BuildCompatibilityFailureFileName(failureReport)
            };

            if (saveDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var content = Path.GetExtension(saveDialog.FileName)
                .Equals(".txt", StringComparison.OrdinalIgnoreCase)
                ? failureReport.BuildDisplayMessage()
                : failureReport.BuildDiagnosticJson();
            File.WriteAllText(saveDialog.FileName, content);

            MessageBoxEx.Show(
                $"Compatibility details saved to:{Environment.NewLine}{saveDialog.FileName}",
                "Saved",
                MessageBoxEx.MessageBoxExButtons.Ok,
                MessageBoxEx.MessageBoxExIcon.Information);
        }

        private static string BuildCompatibilityFailureFileName(CompatibilityFailureReport failureReport)
        {
            var safeName = string.IsNullOrWhiteSpace(failureReport.SourceName)
                ? "compatibility-failure"
                : Path.GetFileNameWithoutExtension(failureReport.SourceName);

            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                safeName = safeName.Replace(invalidCharacter, '_');
            }

            return $"{safeName}-compatibility-{DateTime.Now:yyyyMMdd-HHmmss}.json";
        }
    }
}
