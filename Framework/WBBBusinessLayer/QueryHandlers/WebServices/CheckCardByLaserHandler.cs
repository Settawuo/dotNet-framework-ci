using Newtonsoft.Json;
using RestSharp;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using WBBBusinessLayer.Extension;
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
    public class CheckCardByLaserHandler : IQueryHandler<CheckCardByLaserQuery, CheckCardByLaserMappingModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _fbbCfgLov;

        public CheckCardByLaserHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> fbbCfgLov
            )
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _fbbCfgLov = fbbCfgLov;
        }

        public CheckCardByLaserMappingModel Handle(CheckCardByLaserQuery query)
        {
            var result = new CheckCardByLaserMappingModel();
            InterfaceLogCommand log = null;
            var cardByLaserModel = new CheckCardByLaserModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.xssbtransactionid,
                    "CheckCardByLaser", "DOPA-Service", query.Body.idCardNo, "FBB", "");

                var client = new RestClient(query.Url);

                var request = new RestRequest(query.Method, Method.POST)
                {
                    RequestFormat = DataFormat.Json,
                    JsonSerializer = new RestSharpJsonSerializer()
                };
                request.AddHeader("x-ssb-origin", query.xssborigin);
                request.AddHeader("x-ssb-service-origin", query.xssbserviceorigin);
                request.AddHeader("x-ssb-transaction-id", query.xssbtransactionid);
                request.AddHeader("x-ssb-order-channel", query.xssborderchannel);
                request.AddHeader("x-ssb-version", query.xssbversion);
                request.AddBody(query.Body);

                /* ถ้ามีการส่งค่าไปที่ Url ที่เป็น https จะต้องใช้ Code บรรทัดล่าง เพื่อให้ bypass ผ่านไปได้ */

                //ServicePointManager.Expect100Continue = true;
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //ServicePointManager.ServerCertificateValidationCallback =
                //    (s, certificate, chain, sslPolicyErrors) => true;

                var response = client.Execute(request);
                var content = response.Content;

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    cardByLaserModel = JsonConvert.DeserializeObject<CheckCardByLaserModel>(response.Content) ?? new CheckCardByLaserModel();

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log,
                                              cardByLaserModel.resultCode == "20000" ? "Success" : "Failed", "", "");

                    result.resultCode = cardByLaserModel.resultCode;

                    if (cardByLaserModel.result != null && cardByLaserModel.result.dataInfo != null)
                    {
                        //Maping message
                        var resultlov = from lov in _fbbCfgLov.Get()
                                        where lov.LOV_TYPE == "DOPA_MAPPING_DESC"
                                        select lov;
                        var textForCheck = Regex.Replace(
                                    cardByLaserModel.result.dataInfo.stDesc,
                                    @"\\u(?<Value>[a-zA-Z0-9]{4})",
                                    m =>
                                    {
                                        return ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString();
                                    });

                        var resultlovCond = resultlov.FirstOrDefault(lov => lov.LOV_VAL3 == cardByLaserModel.result.dataInfo.stCode && lov.LOV_VAL4.Replace(" ", "") == textForCheck.Replace(" ", ""));
                        if (resultlovCond != null)
                        {
                            if (query.Lang == "T")
                            {
                                result.messageMapping = resultlovCond.LOV_VAL1;
                            }
                            else if (query.Lang == "E")
                            {
                                result.messageMapping = resultlovCond.LOV_VAL2;
                            }
                        }
                    }
                    else
                    {
                        //Maping resultcode
                        var resultlov = from lov in _fbbCfgLov.Get()
                                        where lov.LOV_TYPE == "DOPA_MAPPING_CODE"
                                        select lov;
                        var resultlovCond = resultlov.FirstOrDefault(lov => lov.LOV_NAME == cardByLaserModel.resultCode);
                        if (resultlovCond != null)
                        {
                            if (query.Lang == "T")
                            {
                                result.messageMapping = resultlovCond.LOV_VAL1;
                            }
                            else if (query.Lang == "E")
                            {
                                result.messageMapping = resultlovCond.LOV_VAL2;
                            }
                        }
                    }
                }
                else
                {
                    // ---------- Log File ----------
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", response.ErrorMessage, "");
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetBaseException().ToString(), "");
                _logger.Info(ex.GetErrorMessage());
            }

            return result;
        }
    }
}
