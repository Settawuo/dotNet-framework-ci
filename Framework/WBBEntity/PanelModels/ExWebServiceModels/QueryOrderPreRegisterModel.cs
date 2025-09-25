using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{

    public class PreRegisterModel
    {
        public string cust_name { get; set; }
        public string cust_surname { get; set; }
        public string order_date { get; set; }
        public string order_status { get; set; }
        public string order_result { get; set; }
    }

    public class outPreRegisterModel
    {
        public string CustName { get; set; }
        public string CustSurname { get; set; }
        public string OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string OrderResult { get; set; }
    }

    public class GetPreRegisterQueryData
    {
        public decimal ReturnCode { get; set; }
        public string ReturnMessage { get; set; }
        public List<PreRegisterModel> PreRegisterModel { get; set; }
    }

}
