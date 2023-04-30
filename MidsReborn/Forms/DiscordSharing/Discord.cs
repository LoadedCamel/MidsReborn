using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators.OAuth2;
using RestSharp.Serializers.NewtonsoftJson;
using static Mids_Reborn.Forms.DiscordSharing.Models;

namespace Mids_Reborn.Forms.DiscordSharing
{
    internal class Discord
    {
        private readonly string _cId = Encoding.Unicode.GetString(Convert.FromBase64String(@"NwAyADkAMAAxADgAMgAwADgAOAAyADQAMAA2ADYAMQA3ADEA"));
        private readonly string _cse = Encoding.Unicode.GetString(Convert.FromBase64String(@"dgBGAGYAdgAtAFkAbwBqAGYAVwBwAF8ASABWAE0ASQBhAGUAeQA5AGUAcABfAGwAcQA3AEMAZQBfAGEAbwAxAA=="));
        private readonly RestClient _restClient;

        public Discord(RestClient restClient)
        {
            _restClient = restClient;
            //_restClient.UseNewtonsoftJson();
        }

        internal async Task<OAuthModel?> RequestAccessToken(string accessCode, string redirectUrl)
        {
            var atrParams = new AtRequestModel
            {
                client_id = _cId,
                client_secret = _cse,
                grant_type = "authorization_code",
                code = accessCode,
                redirect_uri = redirectUrl
            };

            var atRequest = new RestRequest("oauth2/token");
            atRequest.AddObject(atrParams);
            return await _restClient.PostAsync<OAuthModel>(atRequest);
        }

        internal async Task<OAuthModel?> RefreshAccessToken(string refreshToken)
        {
            var rftParams = new RftRequestModel
            {
                client_id = _cId,
                client_secret = _cse,
                grant_type = "refresh_token",
                refresh_token = refreshToken
            };
            var rftRequest = new RestRequest("oauth2/token", Method.Post);
            rftRequest.AddObject(rftParams);
            return await _restClient.PostAsync<OAuthModel>(rftRequest);
        }

        internal async Task<bool> RevokeAccessToken(string? accessToken)
        {
            var rtParams = new RtRequestModel
            {
                client_id = _cId,
                client_secret = _cse,
                access_token = accessToken
            };
            var rtRequest = new RestRequest("oauth2/token/revoke");
            rtRequest.AddObject(rtParams);
            return await _restClient.PostAsync<bool>(rtRequest);
        }

        internal async Task<DiscordUserModel> GetUserInfo(string? accessToken)
        {
            DiscordUserModel result = default;
            var infoRequest = new RestRequest("users/@me");
            //_restClient.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(accessToken, "Bearer");
            try
            {
                result = await _restClient.GetAsync<DiscordUserModel>(infoRequest);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}\r\n\r\n{ex.StackTrace}");
            }

            return result;
        }

        internal static async Task<Image> GetUserAvatar(string id, string avatarHash)
        {
            var restClient = new RestClient(new RestClientOptions("https://cdn.discordapp.com"));
            var request = new RestRequest($"avatars/{id}/{avatarHash}.png");
            var responseStream = await restClient.DownloadStreamAsync(request, CancellationToken.None);
            return responseStream != null ? Image.FromStream(responseStream) : Image.FromFile("images\\defaultAvatar.png");
        }
    }
}
