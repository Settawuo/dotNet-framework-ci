using System;
using System.Linq;
using System.Net;
using WBBBusinessLayer.SBNNewWebService;
using WBBBusinessLayer.SBNWebService;
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
    public class GetCancelOrderQueryHandler : IQueryHandler<GetCancelOrderQuery, SaveOrderResp>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;

        public GetCancelOrderQueryHandler(ILogger logger,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_CFG_LOV> lov,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _intfLog = intfLog;
            _uow = uow;
            _lov = lov;
        }

        public SaveOrderResp Handle(GetCancelOrderQuery query)
        {
            InterfaceLogCommand log = null;
            var response = new SaveOrderResp();

            try
            {
                //log = GetPackageListHelper.StartInterfaceAirWfLog(_uow, _intfLog, query,
                //     query.ID_Card_No, "CancelOrder", "GetCancelOrderQuery");
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.ID_Card_No, "CancelOrder", "GetCancelOrderQuery", null, "FBB", "");


                var temp = query.ListOrder;
                var CancelOderlist = temp.Select(o => new AirRegistCancelordRecord()
                {
                    OrderNo = o.ToSafeString()

                }).ToArray();

                var NewCancelOderlist = temp.Select(o => new airRegistCancelordRecord()
                {
                    orderNo = o.ToSafeString()

                }).ToArray();

                var sbnStatus = _lov.Get(l => l.LOV_TYPE == ("FBB_CONSTANT") && l.LOV_NAME == ("SBN_SERVICE")).FirstOrDefault();

                if (sbnStatus.LOV_VAL1 == "NEW")
                {
                    #region newSBNService
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;

                    using (var service = new SBNNewWebService.SBNWebServiceService())
                    {
                        service.Timeout = 600000;
                        var data = service.cancelOrderWeb(
                            ID_CARD_NO: query.ID_Card_No.ToSafeString(),
                            airRegistCancelordArray: NewCancelOderlist
                        );
                        response.RETURN_CODE = data.RETURN_CODE;
                        response.RETURN_MESSAGE = data.RETURN_MESSAGE;

                    }
                    #endregion
                }
                else
                {
                    #region oldSBNService
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback =
                        (s, certificate, chain, sslPolicyErrors) => true;

                    using (var service = new SBNWebService.SBNWebServiceService())
                    {
                        var data = service.cancelOrderWeb(
                            ID_CARD_NO: query.ID_Card_No.ToSafeString(),
                            airRegistCancelordArray: CancelOderlist
                        );
                        response.RETURN_CODE = data.RETURN_CODE;
                        response.RETURN_MESSAGE = data.RETURN_MESSAGE;

                    }
                    #endregion
                }



                //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, response, log, "Success", "");
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, response, log, "Success", "", "");
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());

                if (null != log)
                {
                    //GetPackageListHelper.EndInterfaceAirWfLog(_uow, _intfLog, ex, log,
                    //    "Failed", ex.GetErrorMessage());
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Failed", ex.GetErrorMessage(), "");
                }

                response.RETURN_CODE = -1;
                response.RETURN_MESSAGE = "Error Before Call Cancel Order Service " + ex.GetErrorMessage();

            }

            return response;
        }
    }
}
