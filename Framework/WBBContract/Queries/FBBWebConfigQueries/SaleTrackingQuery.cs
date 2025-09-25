using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SaleTrackingQuery : IQuery<List<SaleTrackingList>>
    {
        public string P_Id_Card { get; set; }
        public string P_First_Name { get; set; }
        public string P_Last_Name { get; set; }
        public string P_Location_Code { get; set; }
        public string P_Asc_Code { get; set; }
        public string P_Date_From { get; set; }
        public string P_Date_To { get; set; }
        //public string P_Status { get; set; }
        public string P_Cust_Name { get; set; }
        public string P_User { get; set; }

        // return code
        public int ret_code { get; set; }
        public string ret_msg { get; set; }

    }
}
