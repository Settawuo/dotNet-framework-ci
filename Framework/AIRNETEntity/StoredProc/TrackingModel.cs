using System;

namespace AIRNETEntity.StoredProc
{
    public class TrackingModel
    {
        //public int RETURN_CODE { get; set; }
        //public string RETURN_MESSAGE { get; set; }

        public int ret_code { get; set; }
        public string ret_msg { get; set; }

        public string order_no { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string id_card_no { get; set; }
        public string tax_id { get; set; }
        public int? work_flow_id { get; set; }
        public int? flow_seq { get; set; }
        public string current_state { get; set; }
        public DateTime? register_date { get; set; }
        public string register_date_str { get; set; }
        public string package_name_tha { get; set; }
        public string house_no { get; set; }
        public string coverage_flag { get; set; }
        public DateTime? appointment_date_1 { get; set; }
        public DateTime? appointment_date_2 { get; set; }
        public DateTime? appointment_date_3 { get; set; }
        public string appointment_date_1_str { get; set; }
        public string appointment_date_2_str { get; set; }
        public string appointment_date_3_str { get; set; }

        public DateTime? complete_install_date { get; set; }
        public string complete_install_date_str { get; set; }
        public string cancel_install_reason_th { get; set; }
        public string cancel_install_reason_en { get; set; }

        // V 16.6

        public string technology { get; set; }
        public string install_address { get; set; }
        public string expect_install_date { get; set; }
        public string ontop_package { get; set; }
        public string onservice_date { get; set; }

        //16.8
        public string fibrenet_id { get; set; }
        public string order_type { get; set; }
        //public DateTime? start_date_dt { get; set; }
        public string start_date { get; set; }
        //public DateTime? end_date_dt { get; set; }
        public string end_date { get; set; }
        public string transaction_state { get; set; }
        public string appointment_timeslot_1_str { get; set; }
        public string appointment_timeslot_2_str { get; set; }
        public string appointment_timeslot_3_str { get; set; }


        //public List<CustomerModel> ret_data { get; set; }
        //public IEnumerable<CustomerModel> IEnumerableCustomerModel { get; set; }

        //16.12
        public string location_code { get; set; }
        public string asc_code { get; set; }
        public string employee_id { get; set; }
        public string customer_name { get; set; }
        public string building_village { get; set; }
        public string sub_district { get; set; }
        public string district { get; set; }
        public string province { get; set; }
        public string playbox_flag { get; set; }
        public string fixedline_flag { get; set; }
        public string status_date { get; set; }

        public decimal count_appointment { get; set; }
        public bool order_cancel { get; set; }

        public string additional_package { get; set; }
        public bool track_enable { get; set; }
        public GotoTrackModel track { get; set; }
        public string customerSatisfactionSurveyUrl { get; set; }
    }

    public class GotoTrackModel
    {
        public string wfmfurl { get; set; } = string.Empty;
        public string p_type { get; set; }
        public string p_language { get; set; }
        public string p_option { get; set; }
        public string p_channel { get; set; }
        public string p_refId { get; set; }
        public string p_urlback { get; set; }
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
