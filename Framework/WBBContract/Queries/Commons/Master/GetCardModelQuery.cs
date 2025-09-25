using System;
using System.Collections.Generic;
using WBBEntity.Models;

namespace WBBContract.Queries.Commons.Master
{
    public class GetCardModelQuery : IQuery<List<FBB_CARDMODEL>>
    {


        public decimal CARDMODELID { get; set; }
        public decimal POSTSTARTINDEX { get; set; }
        public decimal MAXSLOT { get; set; }
        public string ACTIVEFLAG { get; set; }
        public string CREATED_BY { get; set; }

        public System.DateTime CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
        public string MODEL { get; set; }
        public string BRAND { get; set; }
        public int RESERVE { get; set; }
        public string DATAONLY_FLANG { get; set; }
        public string ResultQuery { get; set; }

    }
}
