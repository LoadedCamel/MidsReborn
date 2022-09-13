#nullable enable
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mids_Reborn.Forms.DiscordSharing
{
    internal static class Structs
    {
        internal struct AuthorizedServer
        {
            [JsonProperty("serverName")]
            public string ServerName { get; set; }
            [JsonProperty("serverId")]
            public ulong ServerId { get; set; }
            [JsonProperty("channels")]
            public List<Channel> Channels { get; set; }
        }

        internal struct Channel
        {
            [JsonProperty("channelName")]
            public string ChannelName { get; set; }
            [JsonProperty("channelId")]
            public ulong ChannelId { get; set; }
        }

        internal struct Validation
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }
    }
}
