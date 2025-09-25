using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetDetailCalliMTopupReplaceQuery : IQuery<DetailCalliMTopupReplaceModel>
    {
        public string TransactionId { get; set; }
        public string FullUrl { get; set; }
        public string P_FIBRENET_ID { get; set; }
        public string P_CONTRACT_NO { get; set; }
        public string P_CUSTOMER_NAME { get; set; }
        public string P_SERIAL_NO { get; set; }
        public string P_RESERVED_ID { get; set; }
        public string P_TIME_SLOT { get; set; }
        public string P_DATE_TIME_SLOT { get; set; }
        public string P_ACCESS_MODE { get; set; }
        public string P_ADDRESS_ID { get; set; }
        public string P_COUNT_PB { get; set; }
    }
}
