namespace WBBEntity.PanelModels.WebServiceModels
{
    public class OrderMCModel
    {
        public string API_URL { get; set; }
        //Request Header Parameters
        public string TRANSACTION_ID { get; set; }
        public string App_Source { get; set; }
        public string App_Destination { get; set; }
        public string ContentType { get; set; }

        //Request Body Parameters
        public string ORDER_NO { get; set; }
        public string ORDER_CHANNEL { get; set; }
        public string IS_CONTACT_CUSTOMER { get; set; }
        public string IS_IN_COVERAGE { get; set; }
        public string USER_ACTION { get; set; }
        public string DATE_ACTION { get; set; }
        public string ORDER_PRE_REGISTER { get; set; }
        public string STATUS_ORDER { get; set; }
        public string REMARK_NOTE { get; set; }

        public string BODY_PARAMETER_STR { get; set; }
    }

    public class OrderMCBody
    {
        public string ORDER_NO { get; set; }
        public string ORDER_CHANNEL { get; set; }
        public string IS_CONTACT_CUSTOMER { get; set; }
        public string IS_IN_COVERAGE { get; set; }
        public string USER_ACTION { get; set; }
        public string DATE_ACTION { get; set; }
        public string ORDER_PRE_REGISTER { get; set; }
        public string STATUS_ORDER { get; set; }
        public string REMARK_NOTE { get; set; }
    }

    public class OrderMCResponse
    {
        public string RESULT_CODE { get; set; }
        public string RESULT_DESC { get; set; }
        public string TRANSACTION_ID { get; set; }
    }
}
