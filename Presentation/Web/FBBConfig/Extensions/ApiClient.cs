using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.UI.WebControls;

namespace FBBConfig.Extensions
{
    public class ApiClient
    {
        private string _url;
        private string _path;

        private readonly HttpClient HttpClient = new HttpClient();
        public ApiClient(string url)
        {
            this._url = url;
        }
        public ApiClient(string url, string _path)
        {
            this._url = url;
            this._path = _path;
        }

        public string Get(string requestUrl)
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                var response = HttpClient.GetAsync(requestUrl).Result;
                return response.RequestMessage.RequestUri.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T Post<T>(dynamic param, string clientSecert) 
        {
            try
            {
                string requestUrl = _url;
                if (!string.IsNullOrEmpty(_path))
                    requestUrl += $"/{_path}";

                using (var http = new HttpClient())
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    http.DefaultRequestHeaders.Add("Authorization", "Basic " + clientSecert);
                    var response =  http.PostAsync(requestUrl, new FormUrlEncodedContent(param)).Result;

                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var contentStr = response.Content.ReadAsStringAsync();
                            return response.Content.ReadAsAsync<T>().Result;
                        }
                        else {
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        }
                            
                    }
                    else
                    {
                        throw new Exception("Response is null");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T PostWithHeader<T>(string accessToken)
        {
            try
            {
                string requestUrl = _url;
                if (!string.IsNullOrEmpty(_path))
                    requestUrl += $"/{_path}";

                using (var http = new HttpClient()) 
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                    http.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                    var response = http.PostAsync(requestUrl, null).Result;

                    if (response != null)
                    {
                        if (response.IsSuccessStatusCode)
                            return response.Content.ReadAsAsync<T>().Result;
                        else
                            throw new Exception($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                    else
                    {
                        throw new Exception("Response is null");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
