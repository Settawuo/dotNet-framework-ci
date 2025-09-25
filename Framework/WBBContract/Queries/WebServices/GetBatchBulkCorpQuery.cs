using WBBEntity.PanelModels.WebServiceModels;
namespace WBBContract.Queries.WebServices
{
    public class GetBatchBulkCorpQuery : IQuery<BatchBulkCorpModel>
    {
        public string P_BULK_NUMBER { get; set; }
        public string P_ORDER_NUMBER { get; set; }
    }

    public class GetBatchBulkCorpPackageQuery : IQuery<GetBulkCorpPackage>
    {
        public string P_CUST_ROW_ID { get; set; }
    }

    public class GetBatchBulkCorpSFFQuery : IQuery<BatchBulkCorpSFFModel>
    {
        public string P_BULK_NUMBER { get; set; }
        public string P_ORDER_NUMBER { get; set; }

    }
    public class GetBatchBulkCorpSFFReturn : IQuery<BatchBulkCorpSFFReturnModel>
    {
        public string P_BULK_NUMBER { get; set; }

    }
    public class GetBatchBulkCorpUpdateSFFReturn : IQuery<BatchBulkCorpUpdateSFFReturnModel>
    {
        public string P_ORDER_NO { get; set; }
        public string P_BA_NO { get; set; }
        public string P_CA_NO { get; set; }
        public string P_SA_NO { get; set; }
        public string P_MOBILE_NO { get; set; }
        public string P_USER { get; set; }
        public string p_error_reason { get; set; }
        public string p_interface_result { get; set; }


    }
    public class GetBatchBulkCorpWfAndSffStatus : IQuery<GetWFAndSFFStatus>
    {

    }

    public class GetBatchMailStatus : IQuery<GetEmailStatusModel>
    {

    }

}
