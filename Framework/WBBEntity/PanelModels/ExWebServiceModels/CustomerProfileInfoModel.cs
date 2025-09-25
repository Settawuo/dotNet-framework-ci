using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class CustomerProfileInfoModel
    {
        public CustomerProfileInfoModel()
        {
            if (list_customer_info == null)
                list_customer_info = new List<List_Customer_Info>();

            if (list_service_info == null)
                list_service_info = new List<List_Service_Info>();
        }

        public string Return_Code { get; set; }
        public string Return_Message { get; set; }
        public List<List_Customer_Info> list_customer_info { get; set; }
        public List<List_Service_Info> list_service_info { get; set; }
    }

    public class List_Customer_Info
    {
        public string internet_no { get; set; }
        public string order_no { get; set; }
        public string customer_name { get; set; }
        public string install_address { get; set; }
        public string billing_address { get; set; }
        public string mobile_contact_no { get; set; }
        public string first_appointment_date { get; set; }
        public string first_appointment_timeslot { get; set; }
    }

    public class List_Service_Info
    {
        public string service { get; set; }
        public string package_type { get; set; }
        public string package_name_th { get; set; }
        public string package_name_en { get; set; }
        public string sff_promotion_code { get; set; }
        public string package_code { get; set; }
        public string product_subtype { get; set; }
    }
}
