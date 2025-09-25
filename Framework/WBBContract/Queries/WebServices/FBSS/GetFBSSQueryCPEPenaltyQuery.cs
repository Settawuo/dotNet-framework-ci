using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices.FBSS
{
    public class GetFBSSQueryCPEPenaltyQuery : IQuery<List<FBSSQueryCPEPenaltyModel>>
    {
        public string TransactionId { get; set; }
        public string FullUrl { get; set; }
        public string OPTION { get; set; }
        public string FIBRENET_ID { get; set; }
        public string SERIAL_NO { get; set; }
        public string STATUS { get; set; }
        public string MAC_ADDRESS { get; set; }

    }

}
