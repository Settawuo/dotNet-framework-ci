using System.Collections.Generic;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCheckATVTopupReplaceQuery : IQuery<CheckATVTopupReplaceModel>
    {
        public string TransactionId { get; set; }
        public string FullUrl { get; set; }
        public string P_FIBRENET_ID { get; set; }
        public string P_FLAG_LANG { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public List<Fbbor050PlayboxArrayModel> Fbbor050PlayboxList { get; set; }
    }

    public class Fbbor050PlayboxArrayModel
    {
        public string CPE_TYPE { get; set; }
        public string CPE_MODEL_NAME { get; set; }
        public string STATUS_DESC { get; set; }
        public string CPE_BRAND_NAME { get; set; }
        public string CPE_MODEL_ID { get; set; }
        public string CPE_GROUP_TYPE { get; set; }
        public string SN_PATTERN { get; set; }
        public string SERIAL_NO { get; set; }
        public string STATUS { get; set; }
    }
}
