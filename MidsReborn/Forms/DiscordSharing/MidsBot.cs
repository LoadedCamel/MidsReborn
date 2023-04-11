#nullable enable
using System;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.Json;
using static Mids_Reborn.Forms.DiscordSharing.Models;

namespace Mids_Reborn.Forms.DiscordSharing
{
    internal class MidsBot
    {
        private readonly RestClient _restClient;

        public MidsBot(RestClient restClient)
        {
            _restClient = restClient;
            //_restClient.UseSystemTextJson();
        }

        internal async Task<MbRegResponseModel> RegisterAsync(string id, string email, string username, string discriminator, string password)
        {
            var regParams = new MbRegisterModel
            {
                DiscordId = Convert.ToUInt64(id),
                Username = username,
                Discriminator = discriminator,
                Email = email,
                Password = password
            };
            var regRequest = new RestRequest("user/register");
            regRequest.AddJsonBody(regParams);
            
            return await _restClient.PostAsync<MbRegResponseModel>(regRequest);
        }

        internal async Task<MbAuthModel> RequestAccessToken(string id, string pass)
        {
            var atrParams = new MbAtRequestModel
            {
                DiscordId = Convert.ToUInt64(id),
                Password = pass
            };

            var atrRequest = new RestRequest("user/token");
            atrRequest.AddJsonBody(atrParams);
            return await _restClient.PostAsync<MbAuthModel>(atrRequest);
        }

        internal async Task<MbAuthModel> RefreshAccessToken(string id, string refreshToken)
        {
            var atrParams = new MbRftRequestModel
            {
                DiscordId = Convert.ToUInt64(id),
                RefreshToken = refreshToken
            };

            var atrRequest = new RestRequest("user/refresh-token");
            atrRequest.AddJsonBody(atrParams);
            return await _restClient.PostAsync<MbAuthModel>(atrRequest);
        }

        internal async Task<MbSubResponse> SubmitBuild(string accessToken, MbSubmissionModel model)
        {
            var subRequest = new RestRequest("build/submit-build");
            subRequest.AddJsonBody(model);
            //_restClient.Authenticator = new JwtAuthenticator(accessToken);
            return await _restClient.PostAsync<MbSubResponse>(subRequest);
        }

        internal async Task<MbServersResponseModel> GetUserChannels(string accessToken, string id)
        {
            var chanParams = new MbChanRequestModel
            {
                DiscordId = Convert.ToUInt64(id)
            };
            var chanRequest = new RestRequest("user/channels");
            chanRequest.AddJsonBody(chanParams);
            //_restClient.Authenticator = new JwtAuthenticator(accessToken);
            return await _restClient.PostAsync<MbServersResponseModel>(chanRequest);
        }

        internal async Task<MbRtResponseModel> GetResetToken(string id, string email)
        {
            var rtrParams = new MbRtRequestModel
            {
                DiscordId = Convert.ToUInt64(id),
                Email = email
            };
            var resetTokenRequest = new RestRequest("user/reset-token");
            resetTokenRequest.AddJsonBody(rtrParams);
            return await _restClient.PostAsync<MbRtResponseModel>(resetTokenRequest);
        }

        internal async Task<MbResetResponseModel> ResetPassword(string id, string password, string token)
        {
            var rpParams = new MbResetRequestModel
            {
                DiscordId = Convert.ToUInt64(id),
                NewPassword = password,
                ResetToken = token
            };

            var resetRequest = new RestRequest("user/reset-password");
            resetRequest.AddJsonBody(rpParams);
            return await _restClient.PostAsync<MbResetResponseModel>(resetRequest);
        }
    }
}
