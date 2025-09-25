using AIRNETEntity.Extensions;
using System;
using System.Linq;
using System.Net;
using WBBBusinessLayer.AisEmployeServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetWsLdapQueryIdQueryHandler : IQueryHandler<WsLdapQueryIdQuery, WsLdapQueryIdModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetWsLdapQueryIdQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _lov = lov;
        }


        public WsLdapQueryIdModel Handle(WsLdapQueryIdQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new WsLdapQueryIdModel();

            try
            {
                string AisEmployeServiceUrl = "";
                var AisEmployeServiceConfigData = _lov.Get(l => l.LOV_NAME == "URLAuthenLDAP_AisEmployeService" && l.ACTIVEFLAG == "Y" ).ToList();
                if(AisEmployeServiceConfigData != null && AisEmployeServiceConfigData.Count >0)
                {
                    AisEmployeServiceUrl = AisEmployeServiceConfigData.FirstOrDefault().LOV_VAL1.ToSafeString();
                }

                var queryInfo = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("WS_LDAP_QueryUID"));
                var projectCode = queryInfo.Where(x => x.DISPLAY_VAL == "ProjectCode").Select(x => x.LOV_VAL1).FirstOrDefault();
                var projectLevel = Convert.ToInt32(!string.IsNullOrEmpty(queryInfo.Where(x => x.DISPLAY_VAL == "ProjectLevel").Select(x => x.LOV_VAL1).FirstOrDefault())
                    ? queryInfo.Where(x => x.DISPLAY_VAL == "ProjectLevel").Select(x => x.LOV_VAL1).FirstOrDefault() : "0");

                //TODO: Write Log
                query.AisEmployeeUser = new WBBContract.Queries.WebServices.AisEmployeeUser
                {
                    Username = query.Username
                };
                query.ProjectQuery = new WBBContract.Queries.WebServices.ProjectQuery
                {
                    ProjectCode = projectCode,
                    ProjectLevel = projectLevel
                };
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionId, "WS_LDAP_QueryUID", "AisEmployeService", "", "FBB|" + query.FullUrl, "WBB");
                //End Log

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                ServicePointManager.ServerCertificateValidationCallback =
                    (s, certificate, chain, sslPolicyErrors) => true;


                using (var service = new AisEmployeServices.AisEmployeService())
                {
                    if (!string.IsNullOrEmpty(AisEmployeServiceUrl))
                        service.Url = AisEmployeServiceUrl;

                    var aisEmp = new AisEmployeServices.AisEmployeeUser
                    {
                        Username = query.Username
                    };
                    var projectQuery = new AisEmployeServices.ProjectQuery
                    {
                        ProjectCode = projectCode,
                        ProjectLevel = projectLevel
                    };

                    var response = service.WS_LDAP_QueryUID(aisEmp, projectQuery);
                    result.Code = response.Code.ToSafeString();
                    result.Message = response.Message;

                    if (result.Code == "0")
                    {
                        var serviceResponseWithAisEmployeeInfomation = (ServiceResponseWithAisEmployeeInfomation)response;
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, serviceResponseWithAisEmployeeInfomation, log, "Success", "", "WBB");

                        result.PinCode = serviceResponseWithAisEmployeeInfomation.AisEmployee.PinCode;
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log, "Failed", result.Message, "WBB");
                    }
                }


                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(string.Format("GetWsLdapQueryIdQueryHandler Error : {0}", ex.GetBaseException()));
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Error", ex.Message, "WBB");

                return result;
            }
        }

    }
}
