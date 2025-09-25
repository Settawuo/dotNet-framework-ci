using System;
using System.Linq;
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
    public class GetMobileDataUsageQueryHandler : IQueryHandler<GetMobileDataUsageQuery, MobileDataUsageModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetMobileDataUsageQueryHandler(ILogger logger,
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
                CREATED_BY = "FbbCampaignDataInterfaceLog",

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

        public MobileDataUsageModel Handle(GetMobileDataUsageQuery query)
        {
            InterfaceLogCommand log = null;
            MobileDataUsageModel result = new MobileDataUsageModel();

            try
            {
                var QueryInfo = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("CampaignDataService"));
                string user = QueryInfo.Where(x => x.DISPLAY_VAL == "user").Select(x => x.LOV_VAL1).FirstOrDefault();
                string password = QueryInfo.Where(x => x.DISPLAY_VAL == "password").Select(x => x.LOV_VAL1).FirstOrDefault();
                string method = QueryInfo.Where(x => x.DISPLAY_VAL == "method").Select(x => x.LOV_VAL1).FirstOrDefault();

                //log = StartInterfaceLog(query, query.MOBILE_NO, "CamapignDataService", "GetMobileDataUsage");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.MOBILE_NO, "CamapignDataService", "GetMobileDataUsage", null, "FBB", "");

                using (var service = new CampaignDataServices.CampaignDataService())
                {
                    //service.Credentials = new NetworkCredential(user, password);

                    CampaignDataServices.OBJRequest objReq = new CampaignDataServices.OBJRequest()
                    {
                        method = method,
                        parameter = new string[] { query.MOBILE_NO, query.networkType },
                        user = user,
                        password = password


                    };
                    CampaignDataServices.CampResponse objResp = service.execute(objReq);
                    result.codeResponse = objResp.codeResponse;
                    result.description = objResp.description;

                    foreach (CampaignDataServices.Parameter tmp in objResp.param1)
                    {
                        PropertyInfo propInfo = result.GetType().GetProperty(tmp.name);
                        if (propInfo != null)
                        {
                            propInfo.SetValue(result, Convert.ChangeType(tmp.value, propInfo.PropertyType), null);
                        }
                    }


                }

                //EndInterfaceLog(result, log, "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                return result;
            }
            catch (System.Exception ex)
            {
                //EndInterfaceLog(result, log, "Error", ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Error", ex.Message, "");

                result.codeResponse = "-1";
                result.description = "Internal Error : " + ex.Message;
                return result;
            }
        }
    }
}
