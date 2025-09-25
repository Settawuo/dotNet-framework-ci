using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetOrderErrorLogQuery : IQuery<OrderErrorLogModel>
    {
        public string P_DATE_FROM { get; set; }
        public string P_DATE_TO { get; set; }

        public string P_ID_CARD_NO { get; set; }
        public string P_REQUEST_STATUS { get; set; }

        public int P_PAGE_INDEX { get; set; }
        public int P_PAGE_SIZE { get; set; }


    }

    public class GetStatusLogQuery : IQuery<List<StatusLogDropdown>>
    {
        public string status { get; set; }
    }

    public class GetPackagebyServiceQuery : IQuery<FB_Interfce_log_byServiceModel>
    {
        public string P_IN_TRANSACTION_ID { get; set; }
    }

}