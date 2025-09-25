using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCardInfoQuery : IQuery<List<FBB_CARD_INFO>>
    {
        public decimal DSLAMId { get; set; }
    }
}
