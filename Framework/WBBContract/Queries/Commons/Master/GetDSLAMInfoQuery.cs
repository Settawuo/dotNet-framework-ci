using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.Commons.Master
{
    public class GetDSLAMInfoQuery : IQuery<List<FBB_DSLAM_INFO>>
    {
        //public string NodeName{ get; set; }

        public decimal DSLAMID { get; set; }

        public string CREATED_BY { get; set; }

        public System.DateTime CREATED_DATE { get; set; }

        public string UPDATE_BY { get; set; }

        public System.DateTime? UPDATE_DATE { get; set; }

        public decimal DSLAMNUMBER { get; set; }

        public decimal DSLAMMODELID { get; set; }

        public string ACTIVEFLAG { get; set; }

        public string NODEID { get; set; }

        public string REGION_CODE { get; set; }

        public string LOT_NUMBER { get; set; }

        public string NodeName { get; set; }




    }
}
