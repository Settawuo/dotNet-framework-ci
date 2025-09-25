using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class D_LstPMETaxRcptQuery : IQuery<D_LstPMETaxRcptModel>
    {
        public string TransactionId { get; set; }
        public string[] PM_RECEIPT_ID { get; set; }
        public string FullUrl { get; set; }
        public string InternetNo { get; set; }
    }
}
