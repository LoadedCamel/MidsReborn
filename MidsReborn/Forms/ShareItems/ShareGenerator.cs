using Mids_Reborn.Core;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.ShareSystem;
using Mids_Reborn.Forms.Controls;
using Application = System.Windows.Forms.Application;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;

namespace Mids_Reborn.Forms.ShareItems
{
    internal class ShareGenerator
    {
        private readonly ShareClient _client;
        private string? _buildId;

        public ShareGenerator()
        {
            _client = new ShareClient();
            Initialize();
        }

        private async void Initialize()
        {
            _buildId = await _client.RequestBuildId();
        }

        public async void ShareBuild(bool forumFormat = false)
        {
            var buildData = CharacterBuildFile.GenerateShareData();
            var imageData = InfoGraphic.GenerateImageData();
            if (_buildId == null) return;
            var response = await _client.SubmitBuild(_buildId, buildData, imageData);
            if (response == null) return;
            // Do something with response here
        }

        public async void ShareMobileSafeBuild(bool inclIncarnate, bool inclAccolade, bool inclSetBonus, bool inclBreakdown)
        {
            var buildData = CharacterBuildFile.GenerateShareData();
            var imageData = InfoGraphic.GenerateImageData();
            if (_buildId == null) return;
            var subResponse = await _client.SubmitBuild(_buildId, buildData, imageData);
            if (subResponse is not { BuildUrl: not null, ImageUrl: not null }) return;
            var dataLink = new PageBuilder.DataLink
            {
                BuildLink = subResponse.BuildUrl,
                ImageLink = subResponse.ImageUrl
            };
            var builder = new PageBuilder();
            var pageData = builder.GeneratedPageData(dataLink, inclIncarnate, inclAccolade, inclSetBonus, inclBreakdown);
            if (subResponse.Code == null) return;
            var updResponse = await _client.UpdateBuild(subResponse.Code, pageData);
            var dataObject = new DataObject();
            dataObject.SetData(DataFormats.StringFormat, $"{updResponse?.PageUrl}");
            Clipboard.SetDataObject(dataObject, true);
            var msgBox = new MessageBoxEx("Your build export has been loaded to your clipboard.\r\nYou may now paste it on the forum of your choice.", MessageBoxEx.MessageBoxButtons.Okay);
            msgBox.ShowDialog(Application.OpenForms["frmMain"]);
        }
    }
}
