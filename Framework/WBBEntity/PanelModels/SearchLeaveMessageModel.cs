namespace WBBEntity.PanelModels
{
    public class SearchLeaveMessageModel
    {

    }

    public class SearchLeaveMsgFileNameModel
    {
        public string Username { get; set; }
        public string FileName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class SearchLeaveMsgFileNameList
    {
        public string p_file_name { get; set; }
        public string p_user_name { get; set; }
        public string p_create_date { get; set; }
        public string p_summary_record { get; set; }
        public string p_completed_record { get; set; }
        public string p_fail_record { get; set; }
    }

    public class SearchLeaveMsgFileNameExportList
    {
        public string p_file_name { get; set; }
        public string p_user_name { get; set; }
        public string p_create_date { get; set; }
        public string p_summary_record { get; set; }
        public string p_completed_record { get; set; }
        public string p_fail_record { get; set; }
    }

    public class SearchLeaveMsgFileNameDetailModel
    {
        public string FileName { get; set; }
        public string Username { get; set; }
        public string Status { get; set; }
    }

    public class SearchLeaveMsgFileNameDetailList
    {
        public int return_code { get; set; }
        public string return_message { get; set; }

        public string service_speed { get; set; }
        public string cust_name { get; set; }
        public string cust_surname { get; set; }
        public string contact_mobile_no { get; set; }
        public string is_ais_mobile { get; set; }
        public string contact_time { get; set; }
        public string contact_email { get; set; }
        public string address_type { get; set; }
        public string building_name { get; set; }
        public string village_name { get; set; }
        public string house_no { get; set; }
        public string soi { get; set; }
        public string road { get; set; }
        public string tumbol { get; set; }
        public string amphur { get; set; }
        public string province { get; set; }
        public string postal_code { get; set; }
        public string campaign_project_name { get; set; }
        public string status { get; set; }
        public string reason_code { get; set; }
    }

    public class SearchLeaveMsgFileNameDetailExportList
    {
        public string service_speed { get; set; }
        public string cust_name { get; set; }
        public string cust_surname { get; set; }
        public string contact_mobile_no { get; set; }
        public string is_ais_mobile { get; set; }
        public string contact_time { get; set; }
        public string contact_email { get; set; }
        public string address_type { get; set; }
        public string building_name { get; set; }
        public string village_name { get; set; }
        public string house_no { get; set; }
        public string soi { get; set; }
        public string road { get; set; }
        public string tumbol { get; set; }
        public string amphur { get; set; }
        public string province { get; set; }
        public string postal_code { get; set; }
        public string campaign_project_name { get; set; }
        public string status { get; set; }
        public string reason_code { get; set; }
    }
}
