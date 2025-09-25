using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetGiftVoucherPINsQuery : IQuery<List<FBB_VOUCHER_PIN>>
    {
        public string VOUCHER_PIN { get; set; }
        public long VOUCHER_MASTER_ID { get; set; }
        public long Lot { get; set; }
    }
}
