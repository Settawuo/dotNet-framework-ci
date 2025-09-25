using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class QueryOrderQuery : IQuery<QueryOrderModel>
    {
        public List<FIBRENetID> FIBRENetID_List { get; set; }
        public string ORDER_TYPE { get; set; }
        public string FullUrl { get; set; }
    }

    public class FIBRENetID
    {
        public string FIBRENET_ID { get; set; }
        public string START_DATE { get; set; }
        public string END_DATE { get; set; }
    }

    public class ReleaseTimeSlotQuery : IQuery<ReleaseTimeSlotModel>
    {
        public string RESERVED_ID { get; set; }
        public string ORDER_ID { get; set; }
    }

    public class ResReleaseQuery : IQuery<ResReleaseModel>
    {
        public string RES_RESERVATION_ID { get; set; }
        public string ORDER_ID { get; set; }
    }
}
