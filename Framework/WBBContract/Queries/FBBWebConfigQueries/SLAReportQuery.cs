using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class AppointmentDetailQuery : IQuery<List<AppointmentDetailModel>>
    {
        public string service { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }

    public class AppointmentSummaryQuery : IQuery<List<AppointmentSummaryModel>>
    {
        public string service { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }

}
