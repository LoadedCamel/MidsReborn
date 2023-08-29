using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Mids_Reborn.Core;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.ShareSystem;
using Mids_Reborn.Core.ShareSystem.RestModels;
using Mids_Reborn.Forms.Controls;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;

namespace Mids_Reborn.Forms.ImportExportItems
{
    internal static class ShareGenerator
    {
        internal static string GeneratedBuildDataChunk()
        {
            var buildData = CharacterBuildFile.GenerateShareData();
            //var chunks = buildData.Chunk(105);
            //var dataChunk = chunks.Aggregate(string.Empty, (current, chunk) => current + $"{new string(chunk)}\r\n");
            return buildData;
        }

        internal static string BuildDataFromChunk(string dataChunk)
        {
            var chunks = dataChunk.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var recompiledData = string.Concat(chunks);
            return recompiledData;
        }

        internal static Image SharedBuildImage(bool useAltGfx = false)
        {
            var gfxBytes = InfoGraphic.Generate(useAltGfx);
            using var stream = new MemoryStream(gfxBytes);
            var image = Image.FromStream(stream);
            return image;
        }

        internal static async void ShareMobileFriendlyBuild(Control control, bool inclIncarnate, bool inclAccolade, bool inclSetBonus, bool inclBreakdown)
        {
            var buildData = CharacterBuildFile.GenerateShareData();
            var imageData = InfoGraphic.GenerateImageData();

            var id = await ShareClient.RequestId();
            if (id == null)
            {
                var messageBox = new MessageBoxEx("Export Result", "Failed to generate the link.\r\nReason: Response id was null.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(control);
                return;
            }
            var submission = new SubmissionModel(id, buildData, imageData);
            var subResponse = await ShareClient.Submit(submission);
            if (subResponse == null) return;
            if (subResponse.BuildUrl == null || subResponse.ImageUrl == null || subResponse.Code == null)
            {
                var messageBox = new MessageBoxEx("Export Result", "Failed to generate the link.\r\nReason: Response data was null.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(control);
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
            var msgBox = new MessageBoxEx("Export Result", "The mobile friendly link has been placed in your clipboard.", MessageBoxEx.MessageBoxButtons.Okay);
            msgBox.ShowDialog(control);
        }
    }
}
