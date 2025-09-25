using System.Collections.Generic;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetGiftVoucherPINsQueryHandler : IQueryHandler<GetGiftVoucherPINsQuery, List<FBB_VOUCHER_PIN>>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG_ADMIN> _interfaceLog;
        private readonly IEntityRepository<FBB_VOUCHER_PIN> _VoucherTable;

        public GetGiftVoucherPINsQueryHandler(ILogger logger, IWBBUnitOfWork uow, IEntityRepository<FBB_INTERFACE_LOG_ADMIN> interfaceLog, IEntityRepository<FBB_VOUCHER_PIN> VoucherTable)
        {
            _logger = logger;
            _uow = uow;
            _interfaceLog = interfaceLog;
            _VoucherTable = VoucherTable;
        }

        public List<FBB_VOUCHER_PIN> Handle(GetGiftVoucherPINsQuery query)
        {
            List<FBB_VOUCHER_PIN> data;

            if (query.Lot > 0)
            {
                if (query.VOUCHER_MASTER_ID > 0)
                {
                    if (string.IsNullOrEmpty(query.VOUCHER_PIN))
                    {
                        data = _VoucherTable.Get(t => t.VOUCHER_MASTER_ID == query.VOUCHER_MASTER_ID && t.LOT == query.Lot).ToList();
                    }
                    else
                    {
                        data = _VoucherTable.Get(t => t.VOUCHER_MASTER_ID == query.VOUCHER_MASTER_ID && t.LOT == query.Lot && t.VOUCHER_PIN == query.VOUCHER_PIN).ToList();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(query.VOUCHER_PIN))
                    {
                        data = _VoucherTable.Get(t => t.LOT == query.Lot).ToList();
                    }
                    else
                    {
                        data = _VoucherTable.Get(t => t.LOT == query.Lot && t.VOUCHER_PIN == query.VOUCHER_PIN).ToList();
                    }
                }

            }
            else
            {
                if (query.VOUCHER_MASTER_ID > 0)
                {
                    if (string.IsNullOrEmpty(query.VOUCHER_PIN))
                    {
                        data = _VoucherTable.Get(t => t.VOUCHER_MASTER_ID == query.VOUCHER_MASTER_ID).ToList();
                    }
                    else
                    {
                        data = _VoucherTable.Get(t => t.VOUCHER_MASTER_ID == query.VOUCHER_MASTER_ID && t.VOUCHER_PIN == query.VOUCHER_PIN).ToList();
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(query.VOUCHER_PIN))
                    {
                        data = _VoucherTable.Get().ToList();
                    }
                    else
                    {
                        data = _VoucherTable.Get(t => t.VOUCHER_PIN == query.VOUCHER_PIN).ToList();
                    }
                }
            }
            return data;
        }
    }
}
