using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            try
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
                foreach (var property in properties)
                    authDict.Add(property.Name, property.GetValue(jDiscordObject, null));

                MidsContext.Config.DiscordAuthorized = true;
                MidsContext.ConfigSp.Auth = authDict;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        public static bool RefreshToken(string refreshToken)
        {
            try
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
                foreach (var property in properties)
                    authDict.Add(property.Name, property.GetValue(jDiscordObject, null));

                MidsContext.ConfigSp.Auth = authDict;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }

            return false;
        }

        public static void RequestUser(string tokenString)
        {
            try
            {
                var client = new RestClient(API_ENDPOINT);
                var request = new RestRequest("users/@me", Method.GET);
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(tokenString, "Bearer");
                var response = client.Execute(request);
                var jUserObject = JsonConvert.DeserializeObject<DiscordUser>(response.Content);
                var userDict = new Dictionary<string, object>();
                var properties = typeof(DiscordUser).GetProperties();
                foreach (var property in properties) userDict.Add(property.Name, property.GetValue(jUserObject, null));

                MidsContext.ConfigSp.User = userDict;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        public static void RequestServers(string tokenString)
        {
            try
            {
                var client = new RestClient(API_ENDPOINT);
                var request = new RestRequest("users/@me/guilds", Method.GET);
                client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(tokenString, "Bearer");
                var response = client.Execute(request);
                var jServerObject = Deserialize<DiscordServerObject>(response.Content);
                var serverCount = jServerObject.Count;
                MidsContext.ConfigSp.ServerCount = serverCount;
                /*var serversDict = new Dictionary<string, Dictionary<string, string>>();
                for (var i = 0; i < serverCount; i++)
                {
                    var serverInfo = new DiscordServerInfo {ServerNumber = $"Server{i}", Name = jServerObject[i].name, Id = jServerObject[i].id};
                    serversDict.Add(serverInfo.ServerNumber, new Dictionary<string, string>());
                    serversDict[serverInfo.ServerNumber].Add("name", serverInfo.Name);
                    serversDict[serverInfo.ServerNumber].Add("id", serverInfo.Id);
                }*/
                var serverWrite = jServerObject.ToDictionary(x => x.name);
                var dServersList = new List<string>();
                foreach (var entry in serverWrite) dServersList.Add(entry.Value.id);
                //MidsContext.ConfigSp.Servers = serversDict;
                MidsContext.ConfigSp.ServerList = dServersList;
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}\r\n\r\n{e.StackTrace}");
            }
        }

        private static List<T> Deserialize<T>(string serializedJsonString)
        {
            var stuff = JsonConvert.DeserializeObject<List<T>>(serializedJsonString);
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