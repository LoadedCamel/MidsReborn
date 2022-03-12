#nullable enable
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mids_Reborn.Forms.DiscordSharing
{
    internal static class Models
    {
        internal class OAuthModel
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }
            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }
            [JsonProperty("scope")]
            public string Scope { get; set; }
            public string Expirey => $"{DateTime.UtcNow.AddSeconds(ExpiresIn)}";
        }

        internal struct DiscordUserModel
        {
            [JsonProperty("id")]
            public string DiscordId { get; set; }
            [JsonProperty("username")]
            public string Username { get; set; }
            [JsonProperty("discriminator")]
            public string Discriminator { get; set; }
            [JsonProperty("avatar")]
            public string Avatar { get; set; }
            [JsonProperty("bot")]
            public bool Bot { get; set; }
            [JsonProperty("mfa_enabled")]
            public bool MfaEnabled { get; set; }
            [JsonProperty("verified")]
            public bool Verified { get; set; } 
            [JsonProperty("email")]
            public string Email { get; set; }
        }

        internal struct DiscordExpireModel
        {
            [JsonProperty("expires")]
            public DateTime Expires { get; set; }
        }

        internal class AtRequestModel
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string grant_type { get; set; }
            public string code { get; set; }
            public string redirect_uri { get; set; }
        }

        internal class RftRequestModel
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string grant_type { get; set; }
            public string refresh_token { get; set; }
        }

        internal class RtRequestModel
        {
            public string client_id { get; set; }
            public string client_secret { get; set; }
            public string access_token { get; set; }
        }

        internal class MbRegisterModel
        {
            public ulong DiscordId { get; set; }
            public string Username { get; set; }
            public string Discriminator { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
        }

        internal class MbAtRequestModel
        {
            public ulong DiscordId { get; set; }
            public string Password { get; set; }
        }

        internal class MbRftRequestModel
        {
            public ulong DiscordId { get; set; }
            public string RefreshToken { get; set; }
        }

        internal class MbSubmissionModel
        {
            public ulong Id { get; set; }
            public ulong Guild { get; set; }
            public ulong Channel { get; set; }
            public string BuildName { get; set; }
            public string Description { get; set; }
            public int Level { get; set; }
            public string Archetype { get; set; }
            public string Primary { get; set; }
            public string Secondary { get; set; }
            public Dictionary<string, string> Stats { get; set; }
            public string ByteString { get; set; }
        }

        internal class MbRtRequestModel
        {
            public ulong DiscordId { get; set; }
            public string Email { get; set; }
        }

        internal class MbResetRequestModel
        {
            public ulong DiscordId { get; set; }
            public string ResetToken { get; set; }
            public string NewPassword { get; set; }
        }

        internal class MbChanRequestModel
        {
            public ulong DiscordId { get; set; }
        }

        internal struct MbRegResponseModel
        {
            [JsonProperty("message")]
            public string Message { get; set; }
            [JsonProperty("succeeded")]
            public bool Succeeded { get; set; }
        }

        internal struct MbAuthModel
        {
            [JsonProperty("message")]
            public string? Message { get; set; }
            [JsonProperty("accessToken")]
            public string AccessToken { get; set; }
            [JsonProperty("refreshToken")]
            public string RefreshToken { get; set; }
            [JsonProperty("refreshTokenExpiration")]
            public string RefreshTokenExpiration { get; set; }
        }

        internal struct MbRtResponseModel
        {
            [JsonProperty("message")]
            public string? Message { get; set; }
            [JsonProperty("resetToken")]
            public string ResetToken { get; set; }
            [JsonProperty("resetTokenExpiration")]
            public string ResetTokenExpiration { get; set; }
        }

        internal struct MbResetResponseModel
        {
            [JsonProperty("message")]
            public string? Message { get; set; }
            [JsonProperty("reset")]
            public bool Succeeded { get; set; }
        }

        internal struct MbServersResponseModel
        {
            [JsonProperty("message")]
            public string? Message { get; set; }
            [JsonProperty("authorizedServers")]
            public List<Structs.AuthorizedServer> AuthorizedServers { get; set; }
        }

        internal struct MbSubResponse
        {
            [JsonProperty("message")]
            public string? Message { get; set; }
            [JsonProperty("succeeded")]
            public bool Succeeded { get; set; }
        }
    }
}
