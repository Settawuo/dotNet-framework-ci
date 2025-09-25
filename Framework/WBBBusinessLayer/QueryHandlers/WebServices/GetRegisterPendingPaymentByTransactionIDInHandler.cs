using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetRegisterPendingPaymentByTransactionIDInHandler : IQueryHandler<GetRegisterPendingPaymentByTransactionIDInQuery, GetRegisterPendingPaymentByTransactionIDInModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_REGISTER_PENDING_PAYMENT> _FBB_REGISTER_PENDING_PAYMENT;

        public GetRegisterPendingPaymentByTransactionIDInHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_REGISTER_PENDING_PAYMENT> FBB_REGISTER_PENDING_PAYMENT,
            IEntityRepository<object> objService)
        {
            _logger = logger;
            _uow = uow;
            _FBB_REGISTER_PENDING_PAYMENT = FBB_REGISTER_PENDING_PAYMENT;
        }

        public GetRegisterPendingPaymentByTransactionIDInModel Handle(GetRegisterPendingPaymentByTransactionIDInQuery query)
        {
            GetRegisterPendingPaymentByTransactionIDInModel results = new GetRegisterPendingPaymentByTransactionIDInModel();
            try
            {
                List<FBB_REGISTER_PENDING_PAYMENT> dataSelect = (from r in _FBB_REGISTER_PENDING_PAYMENT.Get()
                                                                 where r.PAYMENT_TRANSACTION_ID_IN == query.payment_transaction_id_in
                                                                 select r).ToList();
                if (dataSelect != null && dataSelect.Count > 0)
                {
                    List<RegisterPendingPaymentData> registerPendingPaymentList = dataSelect.Select(o => new RegisterPendingPaymentData()
                    {
                        ais_non_mobile = o.AIS_NON_MOBILE.ToSafeString(),
                        contact_mobile_phone1 = o.CONTACT_MOBILE_PHONE1.ToSafeString(),
                        payment_method = o.PAYMENT_METHOD.ToSafeString(),
                        payment_status = o.PAYMENT_STATUS,
                        created = o.CREATED
                    }).ToList();
                    results.RegisterPendingPaymentList = registerPendingPaymentList;
                }
                else
                {
                    results.resultcode = "Nodata";
                }
            }
            catch (Exception ex)
            {
                results.resultcode = "Error";
            }
            return results;
        }
    }
}
