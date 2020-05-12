using Microsoft.Analytics.Interfaces;
using Microsoft.Analytics.Interfaces.Streaming;
using Microsoft.Analytics.Types.Sql;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BitLyAPI
{
    public class BitLyAPI
    {
        //Token de Acesso BitLy: b7d64d5677fa5941f068d2ec80cedbfe3d033790
        private string _bitLyApiURL;
        private string _bitLyApiToken;

        public BitLyAPI()
        {
            _bitLyApiURL = ConfigurationManager.AppSettings["BitLyAPIUrl"];
            _bitLyApiToken = ConfigurationManager.AppSettings["BitLyAPIToken"];
        }

        public async Task<string> ShortenAsync(string long_url)
        {
            return await Task.Run(() => Shorten(long_url));
        }
        private string Shorten(string long_url)
        {
            if (checkAccessToken())
            {
                using (HttpClient client = new HttpClient())
                {
                    string temp = string.Format(_bitLyApiURL, _bitLyApiToken, WebUtility.UrlEncode(long_url));
                    var response = client.GetAsync(temp).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var message = response.Content.ReadAsStringAsync().Result;
                        dynamic obj = JsonConvert.DeserializeObject(message);
                        return obj.results[long_url].shortUrl;
                    }
                    else
                    {
                        return "Não foi possível encurtar a URL!";
                    }
                }
            }
            else
            {
                return "Não foi possível encurtar a URL!";
            }
        }

        private bool checkAccessToken()
        {
            if (string.IsNullOrEmpty(_bitLyApiToken))
                return false;
            string temp = string.Format(_bitLyApiURL, _bitLyApiToken, "google.com");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(temp).Result;
                return response.IsSuccessStatusCode;
            }
        }
    }
}