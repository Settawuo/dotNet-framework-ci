using System;
using System.Linq;
using WBBBusinessLayer.GssoSsoWebService;
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
    public class ConfirmOneTimePWQueryHandler : IQueryHandler<ConfirmOneTimePWQuery, GssoSsoResponseModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public ConfirmOneTimePWQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public GssoSsoResponseModel Handle(ConfirmOneTimePWQuery query)
        {
            string Service = "";
            InterfaceLogCommand log = null;
            GssoSsoResponseModel Response = new GssoSsoResponseModel();

            try
            {
                gssoSsoResponse data = new gssoSsoResponse();
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.transactionID, "ConfirmOneTimePWQuery", "GssoSsoWebService", query.msisdn, "FBB", "");
                var inputparam = (from z in _FBB_CFG_LOV.Get()
                                  where z.LOV_NAME == "SendOneTimePW" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "FBB_CONSTANT"
                                  select z).ToList();
                if (inputparam != null && inputparam.Count > 0)
                {
                    foreach (var item in inputparam)
                    {
                        if (item.DISPLAY_VAL == "service")
                            Service = item.LOV_VAL1;
                    }
                    var MobileNo = "66" + query.msisdn.Substring(1);

                    System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                    System.Net.ServicePointManager.Expect100Continue = true;
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                    //R23.4 Endpoint GssoSsoWebService.GssoSsoWebService
                    var endpointGssoSsoWeb = (from l in _FBB_CFG_LOV.Get() where l.LOV_NAME == "URL_GSSO_SSO" && l.ACTIVEFLAG == "Y" select l.LOV_VAL1).ToList().FirstOrDefault();

                    using (var service = new GssoSsoWebService.GssoSsoWebService())
                    {
                        if (!string.IsNullOrEmpty(endpointGssoSsoWeb)) service.Url = endpointGssoSsoWeb;
                        data = service.confirmOneTimePW(MobileNo, query.pwd, query.transactionID, Service);
                    }
                    if (data != null)
                    {
                        Response.code = data.code;
                        Response.description = data.description;
                        Response.isSuccess = data.isSuccess;
                        Response.operName = data.operName;
                        Response.orderRef = data.orderRef;
                        Response.pwd = data.pwd;
                        Response.transactionID = data.transactionID;

                    }

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", "CannotGetLov", "");
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetErrorMessage(), "");
                }

                return null;
            }

            return Response;
        }

    }

    public class SendOneTimePWQueryHandler : IQueryHandler<SendOneTimePWQuery, GssoSsoResponseModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _FBB_CFG_LOV;

        public SendOneTimePWQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> FBB_CFG_LOV)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _FBB_CFG_LOV = FBB_CFG_LOV;
        }

        public GssoSsoResponseModel Handle(SendOneTimePWQuery query)
        {
            string EmailAddr = "";
            string OtpChannel = "";
            string Service = "";
            int AddTimeoutMins = 15;
            bool WaitDR = false;
            bool WaitDRSpecified = false;
            int otpDigit = 0;

            InterfaceLogCommand log = null;
            GssoSsoResponseModel Response = new GssoSsoResponseModel();
            try
            {
                gssoSsoResponse data = new gssoSsoResponse();
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.msisdn, "SendOneTimePWQuery", "GssoSsoWebService", query.msisdn, "FBB", "");

                var inputparam = (from z in _FBB_CFG_LOV.Get()
                                  where z.LOV_NAME == "SendOneTimePW" && z.ACTIVEFLAG == "Y" && z.LOV_TYPE == "FBB_CONSTANT"
                                  select z).ToList();

                string serviceNameBy3BB = _FBB_CFG_LOV.Get().FirstOrDefault(cfg => cfg.LOV_NAME == "3BB_IPCAMERA" && cfg.LOV_TYPE == "FBB_CONFIG")?.LOV_VAL4;

                if (inputparam != null && inputparam.Count > 0)
                {
                    foreach (var item in inputparam)
                    {
                        if (item.DISPLAY_VAL == "emailAddr")
                            EmailAddr = item.LOV_VAL1;
                        if (item.DISPLAY_VAL == "otpChannel")
                            OtpChannel = item.LOV_VAL1;
                        if (item.DISPLAY_VAL == "service")
                            Service = query.use3BBService ? serviceNameBy3BB : item.LOV_VAL1;
                        if (item.DISPLAY_VAL == "addTimeoutMins")
                            int.TryParse(item.LOV_VAL1, out AddTimeoutMins);
                        if (item.DISPLAY_VAL == "waitDR")
                        {
                            if (item.LOV_VAL1 == "true")
                            {
                                WaitDR = true;
                                WaitDRSpecified = true;
                            }
                            else
                            {
                                WaitDR = false;
                                WaitDRSpecified = false;
                            }
                        }
                        if (item.DISPLAY_VAL == "otpDigit")
                            int.TryParse(item.LOV_VAL1, out otpDigit);
                    }
                }

                var MobileNo = "66" + query.msisdn.Substring(1);

                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                System.Net.ServicePointManager.Expect100Continue = true;
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;

                //R23.4 Endpoint GssoSsoWebService.GssoSsoWebService
                var endpointGssoSsoWeb = (from l in _FBB_CFG_LOV.Get() where l.LOV_NAME == "URL_GSSO_SSO" && l.ACTIVEFLAG == "Y" select l.LOV_VAL1).ToList().FirstOrDefault();

                using (var service = new GssoSsoWebService.GssoSsoWebService())
                {
                    if (!string.IsNullOrEmpty(endpointGssoSsoWeb)) service.Url = endpointGssoSsoWeb;
                    data = service.sendOneTimePW(MobileNo, EmailAddr, OtpChannel, Service, query.accountType, AddTimeoutMins, WaitDR, WaitDRSpecified, otpDigit);
                }

                if (data != null)
                {
                    Response.code = data.code;
                    Response.description = data.description;
                    Response.isSuccess = data.isSuccess;
                    Response.operName = data.operName;
                    Response.orderRef = data.orderRef;
                    Response.pwd = data.pwd;
                    Response.transactionID = data.transactionID;

                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, data, log, "Success", "", "");
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", "CannotGetLov", "");
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.GetErrorMessage(), "");
                }

                return null;
            }

            return Response;
        }

    }
}
