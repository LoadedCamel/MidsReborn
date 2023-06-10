using Mids_Reborn.Core.ShareSystem.RestModels;
using Mids_Reborn.Forms.Controls;
using RestSharp;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mids_Reborn.Core.ShareSystem
{
    internal class ShareClient
    {
        private readonly RestClientOptions _clientOptions = new() { MaxTimeout = -1 };
        private readonly RestClient _restClient;

        public ShareClient()
        {
            _restClient = new RestClient(_clientOptions);
        }

        public async Task<string?> RequestBuildId()
        {
            var response = await _restClient.GetJsonAsync<ResponseModel>("build/requestId");
            if (response != null && response.Status != "Failed") return response.Id;
            var messageBox = new MessageBoxEx("Failed to obtain a share id.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
            messageBox.ShowDialog(Application.OpenForms["frmMain"]);
            return null;
        }

        public async Task<ResponseModel?> SubmitBuild(string id, string buildData, string imageData)
        {
            var submission = new SubmissionModel(id, buildData, imageData);
            var response = await _restClient.PostJsonAsync<SubmissionModel, ResponseModel>("build/submit", submission);
            if (response == null)
            {
                var messageBox = new MessageBoxEx("Failed to submit build data to the server.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }

            if (response.Status != "Failed") return response;
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nReason: {response.ErrorMessage}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }
        }

        public async Task<ResponseModel?> UpdateBuild(string code, string pageData)
        {
            var update = new UpdateModel(code, pageData);
            var response = await _restClient.PostJsonAsync<UpdateModel, ResponseModel>("build/update-page", update);
            if (response == null)
            {
                var messageBox = new MessageBoxEx("Failed to update page data.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }

            if (response.Status != "Failed") return response;
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nReason: {response.ErrorMessage}", MessageBoxEx.MessageBoxButtons.Okay, MessageBoxEx.MessageBoxIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }
        }
    }
}
