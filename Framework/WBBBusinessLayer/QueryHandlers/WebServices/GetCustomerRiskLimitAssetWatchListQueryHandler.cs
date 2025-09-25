using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using RestSharp;
using System;
using System.Data;
using System.Net;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetCustomerRiskLimitAssetWatchListQueryHandler : IQueryHandler<GetCustomerRiskLimitAssetWatchListQuery, GetCustomerRiskLimitAssetWatchListQueryModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<object> _objService;
        public GetCustomerRiskLimitAssetWatchListQueryHandler(ILogger logger, IEntityRepository<FBB_INTERFACE_LOG> intfLog, IWBBUnitOfWork uow, IEntityRepository<object> objService)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _objService = objService;
        }

        public GetCustomerRiskLimitAssetWatchListQueryModel Handle(GetCustomerRiskLimitAssetWatchListQuery query)
        {
            string url = GetLimitassetUrl(query);
            var client = new RestClient(url);
            var request = RequestBuilder(query);
            var log = CreateLimitAssetLog(query);
            var response = client.Execute(request);
            try
            {
                var result = CheckStatus(response, log);
                if (result.resultCode != "20000")
                {
                    result.developerMessage = JsonConvert.SerializeObject(request) + Environment.NewLine + JsonConvert.SerializeObject(response);
                }
                return result;
            }
            catch (Exception ex)
            {
                var resultError = new GetCustomerRiskLimitAssetWatchListQueryModel
                {
                    resultDescription = JsonConvert.SerializeObject(response) + Environment.NewLine + ex.Message
                };
                return resultError;
            }
        }



        private OracleParameter OracleParam(string name, string value)
        => new OracleParameter { ParameterName = name, Value = value };

        private OracleParameter OracleResult(string name, int type)
         => new OracleParameter { ParameterName = name, OracleDbType = (OracleDbType)type, Direction = ParameterDirection.Output };

        private string GetLimitassetUrl(GetCustomerRiskLimitAssetWatchListQuery query)
        {
            var urlLimitMobile = GetUrlProc(
                OracleParam("p_urlType", "LIMIT_MOBILE"),
                OracleParam("p_idCardNo", query.IdCardNo),
                OracleParam("p_locationCode", query.LocationCode),
                OracleParam("p_userId", query.UserId),
                OracleParam("p_channel", query.Channel),
                OracleParam("p_cardType", query.CardType),
                OracleParam("p_assetType", query.AssetType),
                OracleParam("p_orderType", "newRegistrationFBB"),
                OracleParam("p_smartCardFlag", "N"),
                OracleResult("RETURN_CODE", 107),
                OracleResult("RETURN_MESSAGE", 126),
                OracleResult("RETURN_URL_CURROR", 121));
            return urlLimitMobile;
        }

        private RestRequest RequestBuilder(GetCustomerRiskLimitAssetWatchListQuery query)
        {
            var request = new RestRequest();
            request.Method = Method.GET;
            request.AddHeader("channel", query.Channel);
            request.AddHeader("username", query.Username);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

            return request;
        }

        private InterfaceLogCommand CreateLimitAssetLog(GetCustomerRiskLimitAssetWatchListQuery query)
       => InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.Transaction_Id, "GetCustomerRiskLimitAssetWatchList", "GetCustomerRiskLimitAssetWatchListQueryHandler", null, "FBB", "");

        private GetCustomerRiskLimitAssetWatchListQueryModel CheckStatus(IRestResponse response, InterfaceLogCommand log)
        {
            var StatusCode = "Failed";
            var ErrorMessage = "";
            var result = new GetCustomerRiskLimitAssetWatchListQueryModel();
            try
            {
                if (HttpStatusCode.OK.Equals(response.StatusCode))
                {
                    result = JsonConvert.DeserializeObject<GetCustomerRiskLimitAssetWatchListQueryModel>(response.Content);
                    StatusCode = "Success";
                }
                else
                {
                    ErrorMessage = response.ErrorMessage;
                    result.resultDescription = response.ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.GetBaseException().ToString();
                _logger.Info("GetOnlineQueryMobileInfoHandler Exception " + ex.GetErrorMessage());
            }
            finally
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, StatusCode, ErrorMessage, "");
            }
            return result;
        }

        private string GetUrlProc(OracleParameter urlType, OracleParameter idCardNo, OracleParameter locationCode, OracleParameter userId, OracleParameter channel, OracleParameter cardType, OracleParameter assetType, OracleParameter orderType, OracleParameter smartCardFlag, OracleParameter returnCode, OracleParameter returnMessage, OracleParameter returnUrlCurror)
        {
            returnMessage.Size = 2000;
            var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBBOR004_FRAUD.QUERY_DATA_URL",
                 new object[]
                 {
                     urlType,
                     idCardNo ,
                     locationCode ,
                     userId ,
                     channel ,
                     cardType ,
                     assetType ,
                     orderType ,
                     smartCardFlag ,
                      returnCode,
                      returnMessage,
                      returnUrlCurror
                 });

            DataTable data1 = (DataTable)result[2];
            var urlLimitMobile = data1.Rows[0]["URL_LIMIT_MOBILE"].ToString();
            return urlLimitMobile;
        }
    }
}
