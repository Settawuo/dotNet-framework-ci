using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetListShortNamePackageQuery : IQuery<List<ListShortNameModel>>
    {
        public string id_card_no { get; set; }

        public List<AIR_CHANGE_OLD_PACKAGE_ARRAY> airChangePromotionCode_List { get; set; }

        public string transaction_id { get; set; }

        public string FullUrl { get; set; }

    }

    public class AIR_CHANGE_OLD_PACKAGE_ARRAY
    {
        public string enddt { get; set; }
        public string productSeq { get; set; }
        public string sffPromotionCode { get; set; }
        public string startdt { get; set; }

    }

}
