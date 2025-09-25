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
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetTokenFbbQueryHandler : IQueryHandler<GetTokenFbbQuery, GetTokenFbbModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IWBBUnitOfWork _uow;
        private readonly MemoryCache cache = MemoryCache.Default;
        public GetTokenFbbQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _lov = lov;
            _uow = uow;
        }
        public GetTokenFbbModel Handle(GetTokenFbbQuery query)
        {
            InterfaceLogCommand log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Channel, "GetTokenFbbQuery", "GetTokenFbbQueryHandler", "", "", "");
            _logger.Info($"GetTokenFbb Channel Start");
            _logger.Info($"GetTokenFbb Channel : {query.Channel}");
            _logger.Info($"GetTokenFbb ClientId : {query.ParamGetoken.client_id}");
            _logger.Info($"GetTokenFbb ClientSecret : {query.ParamGetoken.client_secret}");
            _logger.Info($"GetTokenFbb GrantType : {query.ParamGetoken.grant_type}");

            GetTokenFbbModel resultToken = new GetTokenFbbModel();
            string resultContent = string.Empty;
            TokenResultModel tokenResult = new TokenResultModel();
            TokenResultErrorModel tokenResultError = new TokenResultErrorModel();

            try
            {
                var listConfig = (from z in _lov.Get()
                                  where z.LOV_TYPE == "FBSS_Authen" && z.ACTIVEFLAG == "Y"
                                  select new LovModel
                                  {
                                      LOV_NAME = z.LOV_NAME,
                                      LOV_VAL1 = z.LOV_VAL1,
                                      LOV_VAL2 = z.LOV_VAL2,
                                      LOV_VAL3 = z.LOV_VAL3,
                                  }).ToList();
                query.Channel = query.Channel.ToUpper();
                string chanelToken = (string)cache.Get(query.Channel);

                if (string.IsNullOrEmpty(chanelToken))
                {
                    if (!string.IsNullOrEmpty(query.ParamGetoken.client_id) && !string.IsNullOrEmpty(query.ParamGetoken.client_secret))
                    {
                        var urlGetToken = listConfig != null && listConfig.Count > 0 ? !string.IsNullOrEmpty(listConfig.FirstOrDefault(i => i.LOV_NAME == "GenerateTokenFBSS").LOV_VAL1) ? listConfig.FirstOrDefault(i => i.LOV_NAME == "GenerateTokenFBSS").LOV_VAL1 : throw new Exception("Lov URLGettoken null or Empty") : throw new Exception("Not Lov listConfig");
                        var minutes = listConfig != null && listConfig.Count > 0 ? !string.IsNullOrEmpty(listConfig.FirstOrDefault(i => i.LOV_NAME == "FBBWEB").LOV_VAL3) ? listConfig.FirstOrDefault(i => i.LOV_NAME == "FBBWEB").LOV_VAL3 : throw new Exception("Lov expirationDate null or Empty") : throw new Exception("Not Lov listConfig");
                        _logger.Info($"GetTokenFbb Get url GetToken : {urlGetToken}");
                        using (HttpClient client = new HttpClient())
                        {
                            for (int count = 0; count < 3; count++)
                            {
                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                string bodyStr = JsonConvert.SerializeObject(query.ParamGetoken);

                                var content = new StringContent(bodyStr, Encoding.UTF8, "application/json");
                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                                ServicePointManager.ServerCertificateValidationCallback =
                                    (s, certificate, chain, sslPolicyErrors) => true;

                                var resultData = client.PostAsync(urlGetToken, content).Result;
                                resultContent = resultData.Content.ReadAsStringAsync().Result;
                                var statusCode = resultContent.IndexOf("-1");
                                if (resultData != null && resultData.IsSuccessStatusCode && statusCode == -1)
                                {
                                    tokenResult = serializer.Deserialize<TokenResultModel>(resultContent);
                                    tokenExpiration(query.Channel, tokenResult.access_token, int.Parse(minutes));
                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Success", "", "");
                                    _logger.Info($"GetTokenFbb Call API FBSS Success");
                                    resultToken.ret_code = "0";
                                    resultToken.ret_mes = "Success";
                                    break;
                                }
                                else
                                {
                                    if (count == 3)
                                    {
                                        tokenResultError = serializer.Deserialize<TokenResultErrorModel>(resultContent);
                                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultContent, log, "Failed", "API CALL 3 TIMES FAILED", "");
                                        resultToken.ret_code = "-1";
                                        resultToken.ret_mes = "API CALL 3 TIMES FAILED";
                                    }
                                    continue;
                                }

                            }
                        }
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", "client_id or client_secret is Null or Empty", "");
                        resultToken.ret_code = "-1";
                        resultToken.ret_mes = "client_id or client_secret is Null or Empty";
                    }
                }
                else
                {

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Success", "Token not expires", "");
                    resultToken.ret_code = "0";
                    resultToken.ret_mes = "Token not expires";
                }
            }
            catch (Exception ex)
            {
                resultToken.ret_code = "-1";
                resultToken.ret_mes = ex.Message;
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, resultToken, log, "Failed", ex.GetErrorMessage(), "");
                _logger.Info($"GetTokenFbb Exception: {ex.Message}");
            }

            return resultToken;
        }

        private void tokenExpiration(string key, string accessToken, int minutes)
        {
            //Unix Timestamp
            //long ConverUnix = long.Parse(unixTimestamp);
            //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(ConverUnix);
            //DateTime expirationDate = dateTimeOffset.DateTime;
            //DateTime dateTimeValue = DateTime.Now;
            //TimeSpan timeSpan = expirationDate - dateTimeValue;
            //double minutes = timeSpan.TotalMinutes;
            cache.Add(key, accessToken, DateTimeOffset.Now.AddMinutes(minutes));
        }
        private class TokenResultModel
        {
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string resultCode { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
        }
        private class TokenResultErrorModel
        {
            public string msg { get; set; }
            public string httpStatus { get; set; }
            public string resultCode { get; set; }
            public string resultDesc { get; set; }
            public string statusCode { get; set; }
        }
    }
}
