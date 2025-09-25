using System.Collections.Generic;

namespace WBBEntity.PanelModels.WebServiceModels
{

    //public class TrackingModel
    //{

    //    public int RETURN_CODE { get; set; }
    //    public string RETURN_MESSAGE { get; set; }
    //    public List<CustomerModel> RESULT { get; set; }
    //    //public IEnumerable<CustomerModel> IEnumerableCustomerModel { get; set; }
    //}

    public class CustomerModel
    {
        public string ORDERID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string CURRENT_STATE { get; set; }
        public string REGISTERDATE { get; set; }
        public string PACKAGE { get; set; }
        public string HOUSE_NO { get; set; }
        public string COVERAGE_RESULT_FLAG { get; set; }
        public string Appointment_Date_1 { get; set; }
        public string Appointment_Date_2 { get; set; }
        public string Appointment_Date_3 { get; set; }
        public string COMPLATE_INSTALL_DATE { get; set; }
        public string CANCEL_INSTALL_REASON_TH { get; set; }
        public string CANCEL_INSTALL_REASON_EN { get; set; }

    }
}
