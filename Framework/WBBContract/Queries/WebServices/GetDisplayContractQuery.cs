using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDisplayContractQuery : IQuery<GetDisplayContractModel>
    {
        public string P_FIBRENET_ID { get; set; }
        public string P_LANGUAGE { get; set; }
        public List<FbbDisplayData> FbbDisplayDatas { get; set; }
        public string FullUrl { get; set; }
    }

    public class FbbDisplayData
    {
        public string PENALTY { get; set; }
        public string TDMCONTRACTID { get; set; }
        public string CONTRACTNO { get; set; }
        public string DURATION { get; set; }
        public string CONTRACTNAME { get; set; }
    }
}
