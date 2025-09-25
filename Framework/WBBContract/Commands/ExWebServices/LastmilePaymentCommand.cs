namespace WBBContract.Commands.ExWebServices
{
    public class LastmilePaymentCommand
    {
        public string ORDER_NO { get; set; }
        public string ACCESS_NO { get; set; }
        public string TRANSACTION_ID { get; set; }
        public string ORDER_TYPE { get; set; }
        public string REUSED_FLAG { get; set; }
        public string REAL_DISTANCE { get; set; }
        public string MAP_DISTANCE { get; set; }
        public string DISP_DISTANCE { get; set; }
        public string ESRI_DISTANCE { get; set; }
        public string BUILDING_TYPE { get; set; }
        public string USER_ID { get; set; }
        public string ACTION_DATE { get; set; }
        //R19.03
        public string REQUEST_DISTANCE { get; set; }
        public string APPROVE_DISTANCE { get; set; }
        public string APPROVE_STAFF { get; set; }
        public string APPROVE_STATUS { get; set; }
        //End R19.03
        public string LAST_UPDATE_BY { get; set; }
        public string LAST_UPDATE_DATE { get; set; }

        public string RESULT_CODE { get; set; }
        public string RESULT_DESCRIPTION { get; set; }

        // Product Owner
        public string PRODUCT_OWNER { get; set; }

    }
    public class LastmileResule
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESCRIPTION { get; set; }
    }
}
