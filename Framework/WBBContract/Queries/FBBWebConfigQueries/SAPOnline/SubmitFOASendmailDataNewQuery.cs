using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries.SAPOnline
{
    public class SubmitFOASendmailDataNewQuery : IQuery<List<SubmitFOAEquipment>>
    {
        public string orderNo { get; set; }
        public string internetNo { get; set; }
        public string status { get; set; }
        public string productName { get; set; }
        public string orderType { get; set; }
        public string companyCode { get; set; }
        public string serviceName { get; set; }
        public string subcontractorCode { get; set; }
        public string plant { get; set; }
        public string materialCode { get; set; }
        public string storLocation { get; set; }
        public string dateFrom { get; set; }
        public string dateTo { get; set; }
    }
}
