using AIRNETEntity.StoredProc;
using System.Collections.Generic;

namespace WBBContract.Queries.WebServices
{
    public class GetTrackingQuery : IQuery<List<TrackingModel>>
    {
        public string P_Id_Card { get; set; }
        public string P_First_Name { get; set; }
        public string P_Last_Name { get; set; }
        public string P_Location_Code { get; set; }
        public string P_Asc_Code { get; set; }
        public string P_Date_From { get; set; }
        public string P_Date_To { get; set; }
        public string P_Cust_Name { get; set; }
        public string P_User { get; set; }
    }
}
