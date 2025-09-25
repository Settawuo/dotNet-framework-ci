using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.AirNetWirelessCoverage
{
    public class GetLastGiftVoucherLotByVoucherMasterIDQueryHandler : IQueryHandler<GetLastGiftVoucherLotByVoucherMasterIDQuery, long>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_VOUCHER_PIN> _VoucherTable;

        public GetLastGiftVoucherLotByVoucherMasterIDQueryHandler(IWBBUnitOfWork uow, IEntityRepository<FBB_VOUCHER_PIN> VoucherTable)
        {
            _uow = uow;
            _VoucherTable = VoucherTable;
        }

        public long Handle(GetLastGiftVoucherLotByVoucherMasterIDQuery query)
        {
            try
            {
                long lot = 0;
                var PINList = _VoucherTable.Get(t => t.VOUCHER_MASTER_ID == query.Voucher_Master_ID);
                if (PINList.Any())
                {
                    lot = PINList.Max(x => x.LOT);
                }
                return lot;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
