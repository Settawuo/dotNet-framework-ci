using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CheckCoverageMapServiceQueryHandler : IQueryHandler<CheckCoverageMapServiceQuery, CheckCoverageMapServiceDataModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_3BB> _intfLog3bb;
        private readonly IWBBUnitOfWork _uow;

        public CheckCoverageMapServiceQueryHandler(ILogger logger,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG_3BB> intfLog3bb,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _lov = lov;
            _intfLog3bb = intfLog3bb;
            _uow = uow;
        }

        public CheckCoverageMapServiceDataModel Handle(CheckCoverageMapServiceQuery query)
        {
            //R22.09 3BB : Add FbbCpGwInterface/Checkcoverage
            InterfaceLog3BBCommand log3bb = null;
            CheckCoverageMapServiceDataModel response = new CheckCoverageMapServiceDataModel();

            try
            {
                CheckCoverageMapServiceConfigModel config = new CheckCoverageMapServiceConfigModel();
                List<FBB_CFG_LOV> loveConfigList = null;
                loveConfigList = _lov.Get(l => l.ACTIVEFLAG.Equals("Y") && l.LOV_TYPE.Equals("FBB_CONFIG")).ToList();

                if (loveConfigList != null && loveConfigList.Count() > 0)
                {
                    config.Url = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "CheckCoverageMapServiceUrl") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "CheckCoverageMapServiceUrl").LOV_VAL1 : "";
                    config.UseSecurityProtocol = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "CheckCoverageMapServiceUrl") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "CheckCoverageMapServiceUrl").LOV_VAL2 : "";
                    config.ContentType = loveConfigList.FirstOrDefault(t => t.LOV_NAME == "CheckCoverageMapServiceUrl") != null ? loveConfigList.FirstOrDefault(t => t.LOV_NAME == "CheckCoverageMapServiceUrl").LOV_VAL3 : "";

                    if (config.Url != "")
                    {
                        string tmpSource = "AIS FIBRE";
                        if (query.FullUrl == "3BB")
                            tmpSource = "3BB";

                        var randomNo = new Random();
                        string genTransactionId = DateTime.Now.ToString("yyyyMMddHHmmss") + randomNo.Next(999).ToString();
                        CheckCoverageMapServiceConfigBody checkCoverageMapServiceConfigBody = new CheckCoverageMapServiceConfigBody()
                        {
                            latitude = query.latitude.ToSafeString(),
                            longitude = query.longitude.ToSafeString(),
                            transactionId = genTransactionId,
                            source = tmpSource
                        };

                        string BodyStr = JsonConvert.SerializeObject(checkCoverageMapServiceConfigBody);

                        config.BodyStr = BodyStr;

                        //if (query.FullUrl == "3BB")
                        log3bb = InterfaceLogServiceHelper.StartInterfaceLog3BB(_uow, _intfLog3bb, config, query.transactionId, "CheckCoverageMapService", "CheckCoverageMapServiceQueryHandler", "", "3BB", "");

                        var client = new RestClient(config.Url);
                        var request = new RestRequest();
                        request.Method = Method.POST;
                        request.AddHeader("Content-Type", config.ContentType);
                        request.AddParameter("application/json", BodyStr, ParameterType.RequestBody);

                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                        // execute the request
                        if (config.UseSecurityProtocol == "Y")
                        {
                            ServicePointManager.Expect100Continue = true;
                            ServicePointManager.ServerCertificateValidationCallback =
                                (s, certificate, chain, sslPolicyErrors) => true;
                        }

                        var responseData = client.Execute(request);

                        var content = responseData.Content; // raw content as string 

                        if (HttpStatusCode.OK.Equals(responseData.StatusCode))
                        {
                            var result = JsonConvert.DeserializeObject<CheckCoverageMapServiceConfigResult>(responseData.Content) ?? new CheckCoverageMapServiceConfigResult();
                            if (result != null)
                            {
                                response.returnCode = result.returnCode;
                                response.returnMessage = result.returnMessage;
                                response.coverage = result.coverage;
                                response.status = result.status;
                                response.subStatus = result.subStatus;
                                response.addressId = result.addressId;
                                response.inserviceDate = convertDate(result.inserviceDate.ToSafeString());
                                response.flowflag = result.flowflag;
                                response.sitecode = result.sitecode;
                                List<CheckCoverageMapServiceSplitterModel> splitterList = new List<CheckCoverageMapServiceSplitterModel>();
                                response.splitterList = new List<CheckCoverageMapServiceSplitterModel>();
                                if (result.splitterList != null && result.splitterList.Count() > 0)
                                {
                                    splitterList = result.splitterList.Select(s => new CheckCoverageMapServiceSplitterModel
                                    {
                                        Name = s.Name.ToSafeString(),
                                        Lon = s.Lon.ToSafeString(),
                                        Lat = s.Lat.ToSafeString(),
                                        Distance = s.Distance.ToSafeString(),
                                        DistanceType = s.DistanceType.ToSafeString(),
                                    }).ToList();
                                    response.splitterList = splitterList;
                                }

                                //if (query.FullUrl == "3BB")
                                InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, content, log3bb, "Success", "", "");
                            }
                            else
                            {
                                response.returnCode = "-1";
                                response.returnMessage = "result null";
                                response.splitterList = null;

                                //if (query.FullUrl == "3BB")
                                InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, content, log3bb, "Failed", "", "");
                            }
                        }
                        else
                        {
                            response.returnCode = "-1";
                            response.returnMessage = responseData.StatusCode.ToString();

                            //if (query.FullUrl == "3BB")
                            InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, content, log3bb, "Failed", "", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.returnCode = "-1";
                response.returnMessage = ex.Message;

                //if (query.FullUrl == "3BB")
                InterfaceLogServiceHelper.EndInterfaceLog3BB(_uow, _intfLog3bb, response, log3bb, "Failed", ex.Message, "");
            }

            return response;
        }

        private string convertDate(string strDate)
        {
            string responseDate = "";
            if (strDate.IndexOf("/") >= 0)
            {
                var strDateSplit = strDate.Split('/');
                responseDate = strDateSplit[2] + '-' + strDateSplit[1] + '-' + strDateSplit[0];
            }
            else if (strDate.IndexOf("-") >= 0)
            {
                responseDate = strDate;
            }

            return responseDate;
        }
    }
}
