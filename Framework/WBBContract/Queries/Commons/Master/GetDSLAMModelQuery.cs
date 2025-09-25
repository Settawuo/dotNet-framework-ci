using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.Commons.Master
{
    public class GetDSLAMModelQuery : IQuery<List<FBB_DSLAMMODEL>>
    {
        public string CvrId { get; set; }

        public decimal DSLAMMODELID { get; set; }

        public string MODEL { get; set; }

        public string BRAND { get; set; }

        public decimal MAXSLOT { get; set; }

        public decimal SLOTSTARTINDEX { get; set; }

        public string SH_BRAND { get; set; }

        public string ACTIVEFLAG { get; set; }


    }
}
