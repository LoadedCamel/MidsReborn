using Newtonsoft.Json;
using RestSharp;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mids_Reborn.Core.ShareSystem.RestModels;
using RestSharp.Serializers.NewtonsoftJson;
using Mids_Reborn.Forms.Controls;

namespace Mids_Reborn.Core.ShareSystem
{
    internal static class ShareClient
    {
        private static RestClient Client
        {
            get
            {
                var options = new RestClientOptions("https://mids.app") { MaxTimeout = -1, };
                var serializerOpt = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
                return new RestClient(options, configureSerialization: s => s.UseNewtonsoftJson(serializerOpt));
            }
        }

        public static async Task<string?> RequestId()
        {
            var response = await Client.GetJsonAsync<ResponseModel>("build/requestId");
            if (response != null && response.Status != "Failed") return response.Id;
            var messageBox = new MessageBoxEx("Failed to obtain a share id.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
            messageBox.ShowDialog(Application.OpenForms["frmMain"]);
            return null;
        }

        public static async Task<ResponseModel?> Submit(SubmissionModel submission)
        {
            var subResponse = await Client.PostJsonAsync<SubmissionModel, ResponseModel>("build/submit", submission);
            if (subResponse == null)
            {
                var messageBox = new MessageBoxEx("Failed to submit build data to the server.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }

            if (subResponse.Status != "Failed") return subResponse;
            {
                var messageBox = new MessageBoxEx($"Failed to submit build data to the server.\r\nReason: {subResponse.ErrorMessage}", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }
        }

        public static async Task<ResponseModel?> UpdateBuildPage(UpdateModel update)
        {
            var updResponse = await Client.PostJsonAsync<UpdateModel, ResponseModel>("build/update-page", update);
            if (updResponse == null)
            {
                var messageBox = new MessageBoxEx("Failed to update page data.\r\nYou are either not connected to the internet or there may be an issue with the server.", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }

            if (updResponse.Status != "Failed") return updResponse;
            {
                var messageBox = new MessageBoxEx($"Failed to update page data.\r\nReason: {updResponse.ErrorMessage}", MessageBoxEx.MessageBoxExButtons.Ok, MessageBoxEx.MessageBoxExIcon.Error);
                messageBox.ShowDialog(Application.OpenForms["frmMain"]);
                return null;
            }

        }

        public static async Task<ImportModel?> GetBuild(string code)
        {
            var importResponse = await Client.GetJsonAsync<ImportModel>($"build/{code}");
            return importResponse;
        }
    }
}
