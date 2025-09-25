using System;
using System.Linq;
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
    public class CheckPendingOrderQueryHandler : IQueryHandler<CheckPendingOrderQuery, CheckPendingOrderModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_CFG_LOV> _lov;
        private readonly IEntityRepository<FBB_REGISTER_PENDING_PAYMENT> _registerPendingPayment;

        public CheckPendingOrderQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lov,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_REGISTER_PENDING_PAYMENT> registerPendingPayment)
        {
            _logger = logger;
            _uow = uow;
            _lov = lov;
            _intfLog = intfLog;
            _registerPendingPayment = registerPendingPayment;
        }

        public CheckPendingOrderModel Handle(CheckPendingOrderQuery query)
        {
            InterfaceLogCommand log = null;
            CheckPendingOrderModel result = new CheckPendingOrderModel()
            {
                IsPendingOrder = false
            };

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.AisNonMobile, "CheckPendingOrderQuery", "CheckPendingOrderQueryHandler", "", "FBB|" + query.FullUrl, "");

                var registerPendingPaymentData = (from t in _registerPendingPayment.Get()
                                                  where t.AIS_NON_MOBILE == query.AisNonMobile
                                                     && t.REGISTER_TYPE == query.RegisterType
                                                     && t.PAYMENT_STATUS == null
                                                     && (t.WEB_PAYMENT_STATUS == null || t.WEB_PAYMENT_STATUS != "PAID")
                                                  select t).ToList();
                if (registerPendingPaymentData != null && registerPendingPaymentData.Count > 0)
                {
                    if (registerPendingPaymentData.FirstOrDefault().PAYMENT_METHOD.ToSafeString() != "R" && registerPendingPaymentData.FirstOrDefault().RETURN_ORDER.ToSafeString() != "")
                        result.IsPendingOrder = true;
                }

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "Success", "");

            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, new SFFServices.SffResponse(), log, "Failed", ex.Message, "");
            }

            return result;
        }
    }
}
