using AIRNETEntity.Extensions;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Web.Script.Serialization;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GenerateTokenHandler : IQueryHandler<GenerateTokenQuery, GenerateTokenModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly MemoryCache cache = MemoryCache.Default;
        public GenerateTokenHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lov = lov;
            _uow = uow;
            _intfLog = intfLog;
        }

        public GenerateTokenModel Handle(GenerateTokenQuery query)
        {

            GenerateTokenModel generate = new GenerateTokenModel();

            try
            {
                //object accessToken = query.accessToken;
                string URL_GenerateToken = (from z in _lov.Get()
                                            where z.LOV_NAME == "GenerateToken" && z.LOV_TYPE == "FBB_CONSTANT" && z.ACTIVEFLAG == "Y"
                                            select z.LOV_VAL1).FirstOrDefault().ToSafeString();
                string accessToken = (string)cache.Get(query.accessTokenName);
                using (HttpClient client = new HttpClient())
                {
                    if (query.flag.ToUpper() != "N")
                    {
                        if (query.contents != null && accessToken == null && query.genTokenParam != null)
                        {
                            for (int count = 0; count <= 3; count++)
                            {

                                var tokenResult = GenerateToken(client, query, generate, URL_GenerateToken);

                                if (tokenResult.token != null)
                                {
                                    accessToken = tokenResult.token.ToString();
                                    tokenExpiration(query.accessTokenName, accessToken, tokenResult.expirationDate);
                                    break;
                                }
                                else
                                {
                                    if (count == 3) throw new Exception("call 3 times can not GenerateToken");
                                    continue;
                                }
                            }
                        }
                        if (query.contents != null && accessToken != null)
                        {
                            var result = CallAPIHttpClientNew(client, query, generate, accessToken);
                            if (!result.IsSuccessStatusCode && string.IsNullOrEmpty(accessToken))
                            {
                                var tokenResult = GenerateToken(client, query, generate, URL_GenerateToken);

                                if (tokenResult.token != null)
                                {
                                    accessToken = tokenResult.token.ToString();
                                    tokenExpiration(query.accessTokenName, accessToken, tokenResult.expirationDate);
                                    CallAPIHttpClientNew(client, query, generate, accessToken);
                                }
                                else
                                {
                                    throw new Exception("API CALL FAILED FOR CALL 1 LOOP CallAPIHttpClientNew");
                                }
                            }
                        }
                    }

                    else
                    {
                        CallAPIHttpClient(client, query, generate);
                    }
                }
            }
            catch (Exception ex)
            {
                generate.ret_code = "-1";
                generate.ret_mes = ex.Message;
            }



            return generate;
        }
        private TokenModel GenerateToken(HttpClient client, GenerateTokenQuery query, GenerateTokenModel generate, string URL_GenerateToken)
        {
            TokenModel accessToken = new TokenModel();
            InterfaceLogCommand log2 = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, "", query.parameter.transactionId, "GenerateToken", "GenerateTokenQuery", "", "FBB|" + query.fullURL, "");
            string resultContent = "";
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string jsonStr = JsonConvert.SerializeObject(query.genTokenParam);

                var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;
                var resultData = client.PostAsync(URL_GenerateToken, content).Result;
                resultContent = resultData.Content.ReadAsStringAsync().Result;

                if (resultData != null && resultData.IsSuccessStatusCode)
                {
                    accessToken = serializer.Deserialize<TokenModel>(resultContent);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Success", "", "");
                }
                else
                {
                    accessToken = serializer.Deserialize<TokenModel>(resultContent);
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", "API CALL FAILED", "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log2, "Failed", ex.GetErrorMessage(), "");
                throw ex;
            }

            return accessToken;

        }

        private GenerateTokenModel CallAPIHttpClientNew(HttpClient client, GenerateTokenQuery query, GenerateTokenModel generate, string accessToken)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                string jsonStr = query.contents;

                var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                var resultData = client.PostAsync(query.url_privitege, content).Result;
                string resultContent = resultData.Content.ReadAsStringAsync().Result;
                if (resultData != null && resultData.IsSuccessStatusCode)
                {
                    generate.resultData = resultContent;
                    generate.ret_code = resultData.StatusCode.ToString();
                    generate.ret_mes = resultData.RequestMessage.ToString();
                    generate.IsSuccessStatusCode = resultData.IsSuccessStatusCode;
                }
                else
                {
                    generate.resultData = resultContent;
                    generate.ret_code = resultData.StatusCode.ToString();
                    generate.ret_mes = resultData.RequestMessage.ToString();
                    generate.IsSuccessStatusCode = resultData.IsSuccessStatusCode;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return generate;
        }


        private GenerateTokenModel CallAPIHttpClient(HttpClient client, GenerateTokenQuery query, GenerateTokenModel generate)
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string jsonStr = query.contents;

                var content = new StringContent(jsonStr, Encoding.UTF8, "application/json");
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;
                var resultData = client.PostAsync(query.url_privitege, content).Result;
                string resultContent = resultData.Content.ReadAsStringAsync().Result;
                if (resultData != null && resultData.IsSuccessStatusCode)
                {
                    generate.resultData = resultContent;
                    generate.ret_code = resultData.StatusCode.ToString();
                    generate.ret_mes = resultData.RequestMessage.ToString();
                    generate.IsSuccessStatusCode = resultData.IsSuccessStatusCode;
                }
                else
                {
                    generate.resultData = resultContent;
                    generate.ret_code = resultData.StatusCode.ToString();
                    generate.ret_mes = resultData.RequestMessage.ToString();
                    generate.IsSuccessStatusCode = resultData.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return generate;
        }

        private void tokenExpiration(string key, string accessToken, DateTime expirationDate)
        {
            // Add an item to the cache with a key and value

            DateTime dateTimeValue = DateTime.Now;
            //DateTime referenceDate = new DateTime(2023, 3, 31, 20, 0, 0);
            TimeSpan timeSpan = expirationDate - dateTimeValue;
            double minutes = timeSpan.TotalMinutes;

            cache.Add(key, accessToken, DateTimeOffset.Now.AddMinutes(minutes));
        }

        private class TokenModel
        {
            public string userName { get; set; }
            public string token { get; set; }
            public DateTime expirationDate { get; set; }
        }
    }

}
