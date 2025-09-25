using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.FBBWebConfigQueries.AirWirelessCoverage
{
    public class GetVoucherProjectDescriptionByGroupQuery : IQuery<List<FBB_VOUCHER_MASTER>>
    {
        public string voucher_project_group { get; set; }
    }
}
