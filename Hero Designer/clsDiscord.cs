using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Base.Master_Classes;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;

namespace Hero_Designer
{
    public class clsDiscord
    {
        public static string ShrinkTheDatalink(string strUrl)
        {
            var url = "http://tinyurl.com/api-create.php?url=" + strUrl;

            var objWebRequest = (HttpWebRequest)WebRequest.Create(url);
            objWebRequest.Method = "GET";
            using var objWebResponse = (HttpWebResponse)objWebRequest.GetResponse();
            var srReader = new StreamReader(objWebResponse.GetResponseStream() ?? throw new InvalidOperationException());

            var strHtml = srReader.ReadToEnd();

            srReader.Close();
            objWebResponse.Close();
            objWebRequest.Abort();

            return strHtml;
        }

        public static async Task Export()
        {

        }
    }
}
