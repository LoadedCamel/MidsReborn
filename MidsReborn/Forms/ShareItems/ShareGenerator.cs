using System.Security.RightsManagement;
using Mids_Reborn.Core;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.ShareSystem;
using Mids_Reborn.Core.ShareSystem.RestModels;
using Mids_Reborn.Forms.Controls;
using Application = System.Windows.Forms.Application;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;

namespace Mids_Reborn.Forms.ShareItems
{
    internal static class ShareGenerator
    {
        internal static async void ShareBuild(bool useAltGfx = false, bool bbCode = false)
        {
            var buildData = CharacterBuildFile.GenerateShareData();
            var imageData = InfoGraphic.GenerateImageData(useAltGfx);
            var id = await ShareClient.RequestId();
            if (id == null) return;
            var submission = new SubmissionModel(id, buildData, imageData);
            var subResponse = await ShareClient.Submit(submission);
            if (subResponse == null) return;
            var dataObject = new DataObject();
            switch (bbCode)
            {
                case false:
                    break;
                case true:
                    var data = new DataObject();
                    data.SetData(DataFormats.UnicodeText, $"[img]{subResponse.ImageUrl}[/img]\n[url=\"{subResponse.BuildUrl}\"]View This Build In MRB[/url]");
                    break;
            }
            var messageBox = new MessageBoxEx("Submission Complete, the links have been copied to your clipboard.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
            messageBox.ShowDialog(Application.OpenForms["frmMain"]);
        }

        internal static async void ShareMobileFriendlyBuild(bool inclIncarnate, bool inclAccolade, bool inclSetBonus, bool inclBreakdown)
        {
            var buildData = CharacterBuildFile.GenerateShareData();
            var imageData = InfoGraphic.GenerateImageData();

            var id = await ShareClient.RequestId();
            if (id == null) return;
            var submission = new SubmissionModel(id, buildData, imageData);
            var subResponse = await ShareClient.Submit(submission);
            if (subResponse == null) return;
            if (subResponse.BuildUrl == null || subResponse.ImageUrl == null || subResponse.Code == null)
            {
                var messageBox = new MessageBoxEx("Failed to complete.\r\nReason: Response data was null.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return;
            }
            var dataLink = new PageBuilder.DataLink(subResponse.BuildUrl, subResponse.ImageUrl);
            var builder = new PageBuilder();
            var pageData = builder.GeneratedPageData(dataLink, inclIncarnate, inclAccolade, inclSetBonus, inclBreakdown);
            var update = new UpdateModel(subResponse.Code, pageData);
            var updateResponse = await ShareClient.UpdateBuildPage(update);
            if (updateResponse == null) return;
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.StringFormat, $"{updateResponse.PageUrl}");
            Clipboard.SetDataObject(dataObject, true);
            var msgBox = new MessageBoxEx("Your mobile friendly link has been added to your clipboard.\r\nYou may now paste it on the forum of your choice.", MessageBoxEx.MessageBoxButtons.Okay);
            msgBox.ShowDialog(Application.OpenForms["frmMain"]);
        }
    }
}
