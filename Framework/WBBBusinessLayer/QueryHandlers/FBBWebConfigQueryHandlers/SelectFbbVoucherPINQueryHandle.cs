using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class SelectFbbVoucherPINQueryHandle : IQueryHandler<GetDateVoucherPINQuery, string>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_VOUCHER_PIN> _FBB_VOUCHER_PIN;

        public SelectFbbVoucherPINQueryHandle(ILogger logger, IEntityRepository<FBB_VOUCHER_PIN> FBB_VOUCHER_PIN)
        {
            _logger = logger;
            _FBB_VOUCHER_PIN = FBB_VOUCHER_PIN;
        }

        public string Handle(GetDateVoucherPINQuery query)
        {

            var SUBCONTRACT = (from f in _FBB_VOUCHER_PIN.Get()
                               where f.VOUCHER_PIN == query.VoucherPIN
                               && ((DateTime.Now >= f.START_DATE || f.START_DATE == null) && (DateTime.Now <= f.EXPIRE_DATE || f.EXPIRE_DATE == null))
                               select f).FirstOrDefault();
            if (SUBCONTRACT != null)
            {
                return SUBCONTRACT.PIN_STATUS.ToString();
            }


            return "";


        }

    }
}
