using System;
using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class GetFbssConfigTBLQuery : IQuery<List<FbssConfigTBL>>
    {
        public int CON_ID { get; set; }
        public string CON_TYPE { get; set; }
        public string CON_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
        public string VAL1 { get; set; }
        public string VAL2 { get; set; }
        public string VAL3 { get; set; }
        public string VAL4 { get; set; }
        public string VAL5 { get; set; }
        public string ACTIVEFLAG { get; set; }
        public int? ORDER_BY { get; set; }
        public string DEFAULT_VALUE { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public Nullable<System.DateTime> UPDATED_DATE { get; set; }
    }

}
