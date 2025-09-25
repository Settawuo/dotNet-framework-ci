using System.Collections.Generic;
using WBBEntity.Minions;

namespace WBBContract.Minions
{
    public class MinionGetMinionServiceQuery : IQuery<List<MinionGetMinionServiceQueryModel>>
    {
        public decimal? ServiceId { get; set; }
        public decimal? ServiceMainId { get; set; }
        public string Flag { get; set; }
    }
}