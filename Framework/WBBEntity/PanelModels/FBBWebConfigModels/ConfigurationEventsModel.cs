using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationEventsSearchModel
    {
        public string EventCode { get; set; }
    }

    public class ConfigurationEventsModel
    {
        public string EFFECTIVE_DATE { get; set; }
        public string EXPIRE_DATE { get; set; }
        public string TECHNOLOGY { get; set; }
        public string PROVICE { get; set; }
        public string AMPHUR { get; set; }
        public string TUMBON { get; set; }
        public string ZIPCODE { get; set; }

    }

    public class ConfigurationEventSearchData
    {
        public string service_option { get; set; }
        public string event_code { get; set; }
        public string effective_date { get; set; }
        public string expire_date { get; set; }
        public string technology { get; set; }
        public string provice { get; set; }
        public string amphur { get; set; }
        public string tumbon { get; set; }
        public string zipcode { get; set; }
        public string plug_and_play_flag { get; set; }
        public bool plug_and_play_flag_bool { get; set; }
    }

    public class ConfigurationEventData
    {
        public decimal group_sub_contract { get; set; }
        public string event_code { get; set; }
        public string effective_date { get; set; }
        public string expire_date { get; set; }
        public string technology { get; set; }
        public string provice { get; set; }
        public string amphur { get; set; }
        public string tumbon { get; set; }
        public string zipcode { get; set; }
        public string sub_location_id { get; set; }
        public string sub_contract_name { get; set; }
        public string sub_team_id { get; set; }
        public string sub_team_name { get; set; }
        public string event_start_date { get; set; }
        public string event_end_date { get; set; }
        public string install_staff_id { get; set; }
        public string install_staff_name { get; set; }
        public string sub_row_id { get; set; }
        public string plug_and_play_flag { get; set; }
        public bool plug_and_play_flag_bool { get; set; }
    }

    public class ConfigurationEventSubContactSearchData
    {
        public string StaffValidateFail { get; set; }
        public string ShownButtom { get; set; }
        public string user { get; set; }
        public string hastmp { get; set; }
        public string service_option { get; set; }
        public string event_code { get; set; }
        public string technology { get; set; }
        public string provice { get; set; }
        public string district { get; set; }
        public string sub_district { get; set; }
        public string post_code { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public string plug_and_play_flag { get; set; }
        public bool plug_and_play_flag_bool { get; set; }
        public List<ConfigurationEventSubContactData> ConfigurationEventSubContactDataList { get; set; }
    }

    public class ConfigurationEventSubContactData
    {
        public bool CheckValidateStaff { get; set; }
        public string StaffValidateFail { get; set; }
        public string ShownButtom { get; set; }
        public string event_code { get; set; }
        public string indexData { get; set; }
        public string service_option { get; set; }
        public string sub_contact_id { get; set; }
        public string sub_contact { get; set; }
        public string sub_team_id { get; set; }
        public string sub_team { get; set; }
        public string start_date_event { get; set; }
        public string end_date_event { get; set; }
        public string start_date_subcontact { get; set; }
        public string end_date_subcontact { get; set; }
        public string plug_and_play_flag { get; set; }
        public bool plug_and_play_flag_bool { get; set; }
        public List<SubNameData> SubNameList { get; set; }
        public bool is_delete { get; set; }
    }

    public class SubNameData
    {
        public bool sub_name_select { get; set; }
        public bool sub_name_select_old { get; set; }
        public string sub_name { get; set; }
        public string sub_name_id { get; set; }
    }

    public class CapabilityData
    {
        public string subcontract_Location_CodeField;
        public string subcontract_Company_NameField;
        public List<CapabilityTeamData> CapabilityTeamList;
    }

    public class CapabilityTeamData
    {
        public string subcontract_Team_IdField;
        public string subcontract_Team_NameField;
        public List<CapabilityStaffData> staffField;
    }

    public class CapabilityStaffData
    {
        public string staff_CodeField;
        public string staff_NameField;
    }

    public class SubcontractForDDL
    {
        public string Subcontract_Code;
        public string Subcontract_Name;
    }

    public class SubcontractTeamForDDL
    {
        public string Subcontract_Code;
        public string Subcontract_Name;
        public string Subcontract_Team_Id;
        public string Subcontract_Team_Name;
    }

    public class SubcontractStaffForDDL
    {
        public string Subcontract_Code;
        public string Subcontract_Name;
        public string Subcontract_Team_Id;
        public string staff_Code;
        public string staff_Name;
    }

    public class ValidateStaff
    {
        public string o_return_code { get; set; }
    }
}
