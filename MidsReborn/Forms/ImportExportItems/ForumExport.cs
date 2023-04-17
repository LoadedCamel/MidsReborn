using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.BuildFile.RestModels;
using Mids_Reborn.Forms.Controls;
using RestSharp;

namespace Mids_Reborn.Forms.ImportExportItems
{
    internal class ForumExport
    {
        private string? Code { get; set; }
        private string? ShareUrl { get; set; }
        private string? ImageUrl { get; set; }

        private void LoadToClipboard(string? image, string? code)
        {
            var data = new DataObject();
            data.SetData(DataFormats.UnicodeText, $"[img]{image}[/img]\n[url=\"mrb://{code}\"]View This Build In MRB[/url]");
            Clipboard.SetDataObject(data, true);
        }

        public async void BuildExport()
        {
            try
            {
                var data = CharacterBuildFile.GenerateShareData();
                var infoGraphic = InfoGraphic.Generate();
                var response = await SendBuildToShare(data, infoGraphic);
                LoadToClipboard(response?.Image, response?.Code);
            }
            catch (Exception)
            {
                throw new Exception();
            }

            var msgBox = new MessageBoxEx($"Your build export has been loaded to your clipboard.\r\nYou may now paste it on the forum of your choice.", MessageBoxEx.MessageBoxButtons.Okay);
            msgBox.ShowDialog(Application.OpenForms["frmMain"]);
        }

        
        private async Task<ResponseModel?> SendBuildToShare(string buildData, string imageData)
        {
            var options = new RestClientOptions("https://mids.app")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var response = await client.GetJsonAsync<ResponseModel>("build/requestId");
            if (response == null || response.Status == "Failed")
            {
                var messageBox = new MessageBoxEx("Failed to obtain a share id.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            };
            var submission = new SubmissionModel(response.Id, buildData, imageData);
            var subResponse = await client.PostJsonAsync<SubmissionModel, ResponseModel>("build/submit", submission);
            if (subResponse == null)
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }

            if (subResponse.Status != "Failed") return subResponse;
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nReason: {subResponse.ErrorMessage}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }
        }
    }
}
