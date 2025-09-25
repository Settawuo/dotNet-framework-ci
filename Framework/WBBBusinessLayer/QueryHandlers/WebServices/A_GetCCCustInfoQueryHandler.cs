using Newtonsoft.Json;
using RestSharp;
using System;
using System.Linq;
using System.Net;
using System.Reflection;
using WBBBusinessLayer.CommandHandlers;
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
    public class A_GetCCCustInfoQueryHandler : IQueryHandler<A_GetCCCustInfoQuery, A_GetCCCustInfoModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public A_GetCCCustInfoQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }

        private InterfaceLogCommand StartInterfaceLog<T>(T query, string transactionId, string serviceName, string methodName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = WBBContract.Commands.ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = serviceName,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "FbbA_GetCCCustInfoInterfaceLog",

            };

            var log = InterfaceLogHelper.Log(_intfLog, dbIntfCmd);
            _uow.Persist();

            dbIntfCmd.OutInterfaceLogId = log.INTERFACE_ID;
            return dbIntfCmd;
        }

        private void EndInterfaceLog<T>(T output, InterfaceLogCommand dbIntfCmd, string result, string reason)
        {
            dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (result == "Success") ? "Success" : "Error";
            dbIntfCmd.OUT_RESULT = result;
            dbIntfCmd.OUT_ERROR_RESULT = (result == "Success") ? (reason.Length > 100 ? reason.Substring(0, 100) : result) : result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();

            InterfaceLogHelper.Log(_intfLog, dbIntfCmd);
            _uow.Persist();
        }

        public A_GetCCCustInfoModel Handle(A_GetCCCustInfoQuery query)
        {
            InterfaceLogCommand log = null;
            A_GetCCCustInfoModel result = new A_GetCCCustInfoModel();
            result.NETWORK_TYPE = "";
            result.SUB_NETWORK_TYPE = "";
            result.MOBILE_SEGMENT = "";
            result.REGISTER_DATE = "";
            result.MOBILE_NO_STATUS = "";

            try
            {
                var ConfigUseAthenaIntra = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("Athena_Intra_URL")).FirstOrDefault();
                if (ConfigUseAthenaIntra != null && ConfigUseAthenaIntra.LOV_VAL1.ToSafeString() != "" && ConfigUseAthenaIntra.LOV_VAL2.ToSafeString() == "Y")
                {
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "Athena_Intra", "Athena_Intra", query.DEVICE_TYPE + "|" + query.BROWSER_TYPE, "FBB|" + query.FullUrl, "");
                    //string tmpUrl = "https://test-athena.intra.ais:44300/domain/atn/customers/v1/customerSubscription/subScriptionProfile/msisdn/";
                    string tmpUrl = ConfigUseAthenaIntra.LOV_VAL1.ToSafeString();
                    string tmpMOBILE_NO = query.MOBILE_NO;
                    tmpMOBILE_NO = "66" + tmpMOBILE_NO.Substring(1) + ".json";

                    var client = new RestClient(tmpUrl + tmpMOBILE_NO);
                    var request = new RestRequest();
                    request.Method = Method.GET;

                    // execute the request
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;

                    var responseData = client.Execute(request);
                    var content = responseData.Content; // raw content as string
                    if (HttpStatusCode.OK.Equals(responseData.StatusCode) || HttpStatusCode.NotFound.Equals(responseData.StatusCode))
                    {
                        Athena_IntraModel resultAthena_Intra = null;
                        resultAthena_Intra = JsonConvert.DeserializeObject<Athena_IntraModel>(responseData.Content) ?? new Athena_IntraModel();
                        if (resultAthena_Intra != null)
                        {
                            if (resultAthena_Intra.resultCode == "20000" || resultAthena_Intra.resultCode == "40401")
                            {
                                if (resultAthena_Intra.resultCode == "20000" && resultAthena_Intra.resultData != null && resultAthena_Intra.resultData.subScriptionProfile != null)
                                {
                                    string tmpNETWORK_TYPE = resultAthena_Intra.resultData.subScriptionProfile.networkType.ToSafeString();
                                    string tmpSUB_NETWORK_TYPE = resultAthena_Intra.resultData.subScriptionProfile.chargeType.ToSafeString();
                                    string tmpMOBILE_SEGMENT = resultAthena_Intra.resultData.subScriptionProfile.segment.ToSafeString();
                                    string tmpREGISTER_DATE = resultAthena_Intra.resultData.subScriptionProfile.registerDate.ToSafeString();
                                    string tmpMOBILE_NO_STATUS = resultAthena_Intra.resultData.subScriptionProfile.subscriptionState.ToSafeString();
                                    if (tmpSUB_NETWORK_TYPE == "Hybrid-Post")
                                    {
                                        tmpSUB_NETWORK_TYPE = "Post-paid";
                                    }
                                    if (tmpREGISTER_DATE.Length > 10)
                                    {
                                        tmpREGISTER_DATE = tmpREGISTER_DATE.Substring(0, 10);
                                        string[] tmpREGISTER_DATEArry = tmpREGISTER_DATE.Split('/');
                                        if (tmpREGISTER_DATEArry.Length == 3)
                                        {
                                            tmpREGISTER_DATE = tmpREGISTER_DATEArry[2] + tmpREGISTER_DATEArry[1] + tmpREGISTER_DATEArry[0];
                                        }
                                    }
                                    result.NETWORK_TYPE = tmpNETWORK_TYPE;
                                    result.SUB_NETWORK_TYPE = tmpSUB_NETWORK_TYPE;
                                    result.MOBILE_SEGMENT = tmpMOBILE_SEGMENT;
                                    result.REGISTER_DATE = tmpREGISTER_DATE;
                                    result.MOBILE_NO_STATUS = tmpMOBILE_NO_STATUS;
                                }

                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                                return result;
                            }
                        }
                    }
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, content, log, "Failed", "", "WBBWEB");
                    return result;
                }
                else
                {
                    log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TRANSACTION_ID, "A_GetCCCustInfo", "A_GetCCCustInfo", query.DEVICE_TYPE + "|" + query.BROWSER_TYPE, "FBB|" + query.FullUrl, "");
                    var QueryInfo = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("A_GetCCCustInfo"));
                    string user = QueryInfo.Where(x => x.DISPLAY_VAL == "user").Select(x => x.LOV_VAL1).FirstOrDefault();
                    string password = QueryInfo.Where(x => x.DISPLAY_VAL == "password").Select(x => x.LOV_VAL1).FirstOrDefault();


                    using (var service = new A_GetCCCustInfoServices.FBB_BindingQSService())
                    {
                        service.Credentials = new NetworkCredential(user, password);

                        A_GetCCCustInfoServices.A_GetCCCustInfo objReq = new A_GetCCCustInfoServices.A_GetCCCustInfo()
                        {
                            inbuf = new A_GetCCCustInfoServices.fml32_A_GetCCCustInfo_In()
                            {
                                MOBILE_NO = query.MOBILE_NO
                            }
                        };
                        A_GetCCCustInfoServices.A_GetCCCustInfoResponse objResp = service.A_GetCCCustInfo(objReq);

                        foreach (var tmp in objResp.outbuf.GetType().GetProperties())
                        {
                            PropertyInfo propInfo = result.GetType().GetProperty(tmp.Name);
                            if (propInfo != null)
                            {
                                var tmpVal = objResp.outbuf.GetType().GetProperty(tmp.Name).GetValue(objResp.outbuf, null);

                                propInfo.SetValue(result, Convert.ChangeType(tmpVal, propInfo.PropertyType), null);
                            }
                        }
                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Error", ex.Message, "");

                return result;
            }
        }
    }
}
