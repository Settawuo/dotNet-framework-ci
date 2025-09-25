namespace WBBExternalAPI.Models.Request.EappController
{
    public class GetContentAddOnRequest
    {
        public string TRANSACTION_ID { set; get; }
        public string COLUMN_API { set; get; }
        public string LOV_TYPE { set; get; }
        public string LOV_NAME { set; get; }
        public string PACKAGE_CODE { set; get; }
    }
}