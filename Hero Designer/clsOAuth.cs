using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Base.Master_Classes;
using Hero_Designer.Forms;
using Microsoft.Win32;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Hero_Designer
{
    public class clsOAuth
    {
        private const string CLIENT_ID = "729018208824066171";
        private const string CLIENT_SECRET = "vFfv-YojfWp_HVMIaey9ep_lq7Ce_ao1";
        private const string API_ENDPOINT = "https://discord.com/api";
        private const string REDIRECT_URI = "http://localhost:60403";
        private const string SCOPE = "identify guilds";
        private static frmMain frmMain;

        public static void InitiateListener()
        {
            GetAccessCode();
        }

        private static void GetAccessCode()
        {
            try
            {
                var isCompleted = false;
                using var listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:60403/");
                listener.Start();
                while (!isCompleted)
                {
                    byte[] page;
                    var result = listener.GetContext();
                    var request = result.Request;
                    var response = result.Response;
                    if (request.Url.Query.Contains("access_denied"))
                    {
                        page = File.ReadAllBytes($"{FileIO.AddSlash(Application.StartupPath)}Web\\unauthorized.html");
                        response.ContentLength64 = page.Length;
                        var output = response.OutputStream;
                        output.Write(page, 0, page.Length);
                        isCompleted = true;
                    }
                    else if (request.Url.Query.Contains("?code="))
                    {
                        page = File.ReadAllBytes($"{FileIO.AddSlash(Application.StartupPath)}Web\\authorized.html");
                        response.ContentLength64 = page.Length;
                        var output = response.OutputStream;
                        output.Write(page, 0, page.Length);
                        var accessCode = request.Url.Query.Replace("?code=", "");
                        RequestToken(accessCode);
                        isCompleted = true;
                    }
                }

                listener.Stop();
            }
            catch (HttpListenerException)
            {
                //Nothing to see here
            }
        }

        /*public static void StopListener(HttpListener listener)
        {
            listener.Stop();
        }*/

        private static void RequestToken(string accessCode)
        {
            var client = new RestClient(API_ENDPOINT);
            var request = new RestRequest("oauth2/token", Method.POST);
            request.AddParameter("client_id", CLIENT_ID);
            request.AddParameter("client_secret", CLIENT_SECRET);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", accessCode);
            request.AddParameter("redirect_uri", REDIRECT_URI);
            request.AddParameter("scope", SCOPE);
            var response = client.Execute(request);
            var jDiscordObject = JsonConvert.DeserializeObject<DiscordObject>(response.Content);
            jDiscordObject.DateTime = DateTime.Now;
            var authDict = new Dictionary<string, object>();

            var properties = typeof(DiscordObject).GetProperties();
            foreach (var property in properties) authDict.Add(property.Name, property.GetValue(jDiscordObject, null));

            MidsContext.Config.DiscordAuthorized = true;
            var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RebornTeam\Mids Reborn\Token");
            if (key != null)
            {
                foreach (var kvp in authDict)
                {
                    key.SetValue(kvp.Key, kvp.Value);
                }
                key.Close();
            }
            else
            {
                key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\RebornTeam\Mids Reborn\Token");
                foreach (var kvp in authDict)
                {
                    key?.SetValue(kvp.Key, kvp.Value);
                }
                key?.Close();
            }
        }

        public static bool RefreshToken(string refreshToken)
        {
            var client = new RestClient(API_ENDPOINT);
            var request = new RestRequest("oauth2/token", Method.POST);
            request.AddParameter("client_id", CLIENT_ID);
            request.AddParameter("client_secret", CLIENT_SECRET);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);
            request.AddParameter("redirect_uri", REDIRECT_URI);
            request.AddParameter("scope", SCOPE);
            var response = client.Execute(request);
            var jDiscordObject = JsonConvert.DeserializeObject<DiscordObject>(response.Content);
            jDiscordObject.DateTime = DateTime.Now;
            var authDict = new Dictionary<string, object>();

            var properties = typeof(DiscordObject).GetProperties();
            foreach (var property in properties) authDict.Add(property.Name, property.GetValue(jDiscordObject, null));

            try
            {
                var subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RebornTeam\Mids Reborn\Token", true);
                if (subKey != null)
                {
                    foreach (var kvp in authDict)
                    {
                        subKey.SetValue(kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    subKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\RebornTeam\Mids Reborn\Token", true);
                    foreach (var kvp in authDict)
                    {
                        subKey?.SetValue(kvp.Key, kvp.Value);
                    }
                }
                subKey?.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
            }

            //MidsContext.Config.DAuth = authDict;
            return true;
        }

        public static string GetCryptedValue(string type, string name)
        {
            switch (type)
            {
                case "Auth":
                    MidsContext.Config.DAuth.TryGetValue(name, out var authValue);
                    return authValue?.ToString();
                case "User":
                    MidsContext.Config.DUser.TryGetValue(name, out var userValue);
                    return userValue?.ToString();
                default:
                    return null;
            }
        }

        public static void RequestUser(string tokenString)
        {
            var client = new RestClient(API_ENDPOINT);
            var request = new RestRequest("users/@me", Method.GET);
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(tokenString, "Bearer");
            var response = client.Execute(request);
            var jUserObject = JsonConvert.DeserializeObject<DiscordUser>(response.Content);
            var userDict = new Dictionary<string, object>();
            var properties = typeof(DiscordUser).GetProperties();
            foreach (var property in properties) userDict.Add(property.Name, property.GetValue(jUserObject, null));

            //MidsContext.Config.DUser = userDict;
            try
            {
                var subKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\RebornTeam\Mids Reborn\User", true);
                if (subKey != null)
                {
                    foreach (var kvp in userDict)
                    {
                        subKey.SetValue(kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    subKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\RebornTeam\Mids Reborn\User", true);
                    foreach (var kvp in userDict)
                    {
                        subKey.SetValue(kvp.Key, kvp.Value);
                    }
                }

                subKey.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        public static void RequestServers(string tokenString)
        {
            var client = new RestClient(API_ENDPOINT);
            var request = new RestRequest("users/@me/guilds", Method.GET);
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(tokenString, "Bearer");
            var response = client.Execute(request);
            var jServerObject = Deserialize<DiscordServerObject>(response.Content);
            var serverCount = jServerObject.Count;
            MidsContext.Config.DServerCount = serverCount;
            var serversDict = new Dictionary<string, Dictionary<string, string>>();
            for (var i = 0; i < serverCount; i++)
            {
                var serverInfo = new DiscordServerInfo {ServerNumber = $"Server{i}", Name = jServerObject[i].name, Id = jServerObject[i].id};
                serversDict.Add(serverInfo.ServerNumber, new Dictionary<string, string>());
                serversDict[serverInfo.ServerNumber].Add("name", serverInfo.Name);
                serversDict[serverInfo.ServerNumber].Add("id", serverInfo.Id);
            }
            var serverWrite = jServerObject.ToDictionary(x => x.name);
            var dServersList = new List<string>();
            foreach (var entry in serverWrite) dServersList.Add(entry.Value.name);
            MidsContext.Config.DServers = dServersList;
            try
            {
                foreach (var oKvp in serversDict)
                {
                    var subKey = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\RebornTeam\Mids Reborn\Servers\{oKvp.Key}", true);
                    if (subKey != null)
                    {
                        foreach (var iKvp in oKvp.Value)
                        {
                            subKey.SetValue(iKvp.Key, iKvp.Value);
                        }
                    }
                    else
                    {
                        subKey = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\RebornTeam\Mids Reborn\Servers\{oKvp.Key}", true);
                        foreach (var iKvp in oKvp.Value)
                        {
                            subKey?.SetValue(iKvp.Key, iKvp.Value);
                        }
                    }
                    subKey?.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
            //var dServersList = new List<string>();
            //foreach (var entry in serversDict) dServersList.Add(entry.Value.name);
            //MidsContext.Config.DServers2 = serversDict.ToDictionary(x => , x => (object)x.Value);
        }

        public static void RequestServerChannels(string tokenString, int serverCount)
        {
            for (var index = 0; index < serverCount; index++)
            {
                try
                {
                    Console.WriteLine($"Server{index}");
                    var aOpenSubKey = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\RebornTeam\Mids Reborn\Servers\Server{index}", true);
                    if (aOpenSubKey != null)
                    {
                        var sID = aOpenSubKey.GetValue("id");
                        var client = new RestClient(API_ENDPOINT);
                        var request = new RestRequest($@"guilds/{sID}/channels", Method.GET);
                        client.Authenticator =
                            new OAuth2AuthorizationRequestHeaderAuthenticator(tokenString, "Bearer");
                        var response = client.Execute(request);
                        var jServerObject = Deserialize<DiscordServerChannels>(response.Content);
                        var jChannelDict = jServerObject
                            .Where(x => x.guild_id == Convert.ToInt32(sID) && x.type == 0)
                            .ToDictionary(y => y.name);
                        foreach (var channel in jChannelDict)
                        {
                            Console.WriteLine($"{channel.Key} - {channel.Value.name} - {channel.Value.type}");
                        }
                    }

                    aOpenSubKey?.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e.Message}\r\n\r\n{e.StackTrace}");
                }
            }
        }

        private static List<T> Deserialize<T>(string SerializedJSONString)
        {
            var stuff = JsonConvert.DeserializeObject<List<T>>(SerializedJSONString);
            return stuff;
        }
    }

    public class DiscordObject
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class DiscordUser
    {
        public string id { get; set; }
        public string username { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordServers
    {
        [JsonProperty(PropertyName = "id")] public string id { get; set; }

        [JsonProperty(PropertyName = "name")] public string name { get; set; }

        [JsonProperty(PropertyName = "icon")] public string icon { get; set; }

        [JsonProperty(PropertyName = "owner")] public bool owner { get; set; }

        [JsonProperty(PropertyName = "permissions")]
        public int permissions { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<string> features { get; set; }

        [JsonProperty(PropertyName = "permissions_new")]
        public string permissions_new { get; set; }
    }
}