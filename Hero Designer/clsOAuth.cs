using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.Remoting.Channels;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Windows.Forms;
using Base.Master_Classes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using RestSharp.Authenticators;
using HttpResponse = System.Web.HttpResponse;

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

        public static void GetAccessCode()
        {
            try
            {
                bool isCompleted = false;
                using HttpListener listener = new HttpListener();
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
                        Stream output = response.OutputStream;
                        output.Write(page, 0, page.Length);
                        isCompleted = true;
                    }
                    else if (request.Url.Query.Contains("?code="))
                    {
                        page = File.ReadAllBytes($"{FileIO.AddSlash(Application.StartupPath)}Web\\authorized.html");
                        response.ContentLength64 = page.Length;
                        Stream output = response.OutputStream;
                        output.Write(page, 0, page.Length);
                        var accessCode = request.Url.Query.Replace("?code=", "");
                        RequestToken(accessCode);
                        isCompleted = true;
                    }
                }
                listener.Stop();
            }
            catch (HttpListenerException e)
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
            Dictionary<string, object> authDict = new Dictionary<string, object>();

            PropertyInfo[] properties = typeof(DiscordObject).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                authDict.Add(property.Name, property.GetValue(jDiscordObject, null));
            }

            MidsContext.Config.DAuth = authDict;
            MidsContext.Config.DiscordAuthorized = true;
        }

        public static void RefreshToken(string refreshToken)
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
            Dictionary<string, object> authDict = new Dictionary<string, object>();

            PropertyInfo[] properties = typeof(DiscordObject).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                authDict.Add(property.Name, property.GetValue(jDiscordObject, null));
            }

            MidsContext.Config.DAuth = authDict;
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
            Dictionary<string, object> userDict = new Dictionary<string, object>();
            PropertyInfo[] properties = typeof(DiscordUser).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                userDict.Add(property.Name, property.GetValue(jUserObject, null));
            }

            MidsContext.Config.DUser = userDict;
        }

        public static void RequestServers(string tokenString)
        {
            var client = new RestClient(API_ENDPOINT);
            var request = new RestRequest("users/@me/guilds", Method.GET);
            client.Authenticator = new OAuth2AuthorizationRequestHeaderAuthenticator(tokenString, "Bearer");
            var response = client.Execute(request);
            var jUserObject = Deserialize<DiscordServers>(response.Content);
            Dictionary<string, DiscordServers> serversDict = jUserObject.ToDictionary(m => m.name);
            List<string> dServersList = new List<string>();
            foreach (KeyValuePair<string, DiscordServers> entry in serversDict)
            {
                dServersList.Add(entry.Value.name);
            }

            MidsContext.Config.DServers = dServersList;
            //MidsContext.Config.DServers = serversDict.ToDictionary(x => x.Key, x => (object)x.Value);
        }

        private static string GetCryptoKey()
        {
            byte[] data1 = Convert.FromBase64String("aHR0cDovL2hvb2tzLm1pZHNyZWJvcm4uY29tOjMwMDA=");
            byte[] data2 = Convert.FromBase64String("P21SR3V4elElZkJfVTMjdVh3RWV0WjZlRjMqPUNZOGM0cERnWXp0LWVOVz1ROUs2enM1OFA9ZTl2ZWpZJSplUHQ2PXM4eA==");
            Uri uri = new Uri(Encoding.UTF8.GetString(data1));
            string param = Encoding.UTF8.GetString(data2);
            
            var client = new RestClient(uri);
            var request = new RestRequest("crypto", Method.POST);
            request.AddQueryParameter("token", param);
            var response = client.Execute(request);
            return response.Content;
        }

        public static List<T> Deserialize<T>(string SerializedJSONString)
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
        [JsonProperty(PropertyName = "id")]
        public string id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "icon")]
        public string icon { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public bool owner { get; set; }

        [JsonProperty(PropertyName = "permissions")]
        public int permissions { get; set; }

        [JsonProperty(PropertyName = "features")]
        public List<string> features { get; set; }

        [JsonProperty(PropertyName = "permissions_new")]
        public string permissions_new { get; set; }
    }
}
