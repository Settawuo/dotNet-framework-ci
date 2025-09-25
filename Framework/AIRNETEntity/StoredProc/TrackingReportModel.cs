namespace AIRNETEntity.StoredProc
{
    public class TrackingReportModel
    {
        //public int RETURN_CODE { get; set; }
        //public string RETURN_MESSAGE { get; set; }

        public int ret_code { get; set; }
        public string ret_msg { get; set; }

        public string location_code { get; set; }
        public string asc_code { get; set; }
        public string registered_date { get; set; }
        public string appointment_date { get; set; }
        public string time_slot { get; set; }
        public string customer_name { get; set; }
        public string mobile_no { get; set; }
        public string telephone_no { get; set; }
        public string work_no { get; set; }
        public string fax_no { get; set; }
        public string email { get; set; }
        public string address_id { get; set; }
        public string building_name_th { get; set; }
        public string building_name_en { get; set; }
        public string room_no { get; set; }
        public string floor_no { get; set; }
        public string home_no { get; set; }
        public string moo { get; set; }
        public string room { get; set; }
        public string soi { get; set; }
        public string street { get; set; }
        public string sub_district { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string internet_no { get; set; }
        public string install_date { get; set; }
        public string main_package { get; set; }
        public string speed { get; set; }
        public string promotion_code_main { get; set; }
        public string price_fee_main { get; set; }
        public string price_discount { get; set; }
        public string promotion_code_ontop { get; set; }
        public string ontop_package { get; set; }
        public string price_fee_ontop { get; set; }
        public string playbox_flag { get; set; }
        public string fixedline_flag { get; set; }
        public string status { get; set; }
        public string status_date { get; set; }
        public string cs_note { get; set; }
        public string cancel_reason { get; set; }
        public string air_order_no { get; set; }
        public string order_type { get; set; }
        public string remark { get; set; }
        public string fibrenet_id { get; set; }
        public string order_type_zte { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string event_flag { get; set; }

        public decimal count_appointment { get; set; }
        public bool order_cancel { get; set; }
    }

    //public class CustomerModel
    //{
    //    //public string ORDERID { get; set; }
    //    //public string FIRSTNAME { get; set; }
    //    //public string LASTNAME { get; set; }
    //    //public string CURRENT_STATE { get; set; }
    //    //public string REGISTERDATE { get; set; }
    //    //public string PACKAGE { get; set; }
    //    //public string HOUSE_NO { get; set; }
    //    //public string COVERAGE_RESULT_FLAG { get; set; }
    //    //public string Appointment_Date_1 { get; set; }
    //    //public string Appointment_Date_2 { get; set; }
    //    //public string Appointment_Date_3 { get; set; }
    //    //public string COMPLATE_INSTALL_DATE { get; set; }
    //    //public string CANCEL_INSTALL_REASON_TH { get; set; }
    //    //public string CANCEL_INSTALL_REASON_EN { get; set; }


    //    public string order_no { get; set; }
    //    public string first_name { get; set; }
    //    public string last_name { get; set; }
    //    public string work_flow_id { get; set; }
    //    public string flow_seq { get; set; }
    //    public string current_state { get; set; }
    //    public string register_date { get; set; }
    //    public string package_name_tha { get; set; }
    //    public string house_no { get; set; }
    //    public string coverage_flag { get; set; }
    //    public string appointment_date_1 { get; set; }
    //    public string appointment_date_2 { get; set; }
    //    public string appointment_date_3 { get; set; }
    //    public string complete_install_date { get; set; }
    //    public string cancel_install_reason_th { get; set; }
    //    public string cancel_install_reason_en { get; set; }

    //}
}
