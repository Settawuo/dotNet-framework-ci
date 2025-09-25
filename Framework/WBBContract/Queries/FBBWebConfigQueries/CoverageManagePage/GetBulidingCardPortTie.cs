using System.Collections.Generic;

namespace WBBContract.Queries.FBBWebConfigQueries.CoverageManagePage
{


    public class GetBulidingCardPortTie : IQuery<List<BuailTieQuery>>
    {
        public decimal CVRID { get; set; }
        public decimal DSALMID { get; set; }
        public string falgDataTie { get; set; }

    }
}
