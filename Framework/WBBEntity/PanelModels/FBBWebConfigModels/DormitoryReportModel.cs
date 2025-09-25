using System;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class DormitoryReportModel
    {
    }


    public class CusInsatllTrackModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }


    public class CusNotRegisModel
    {
        public Nullable<DateTime> DateFrom { get; set; }
        public Nullable<DateTime> DateTo { get; set; }
    }

    public class CusNotRegisList
    {
        public decimal Item_no { get; set; }
        public string Dormitort_name { get; set; }
        public string Room_No { get; set; }
        public string Internet_number { get; set; }
        public string Account_name { get; set; }
        public string Contact_Mobile { get; set; }
        public string User_Created { get; set; }
        public DateTime? Installation_Complete_Date { get; set; }
    }

    public class OverviewStatusModel
    {
        public Nullable<DateTime> OSDateFrom { get; set; }
        public Nullable<DateTime> OSDateTo { get; set; }
    }

    public class OverviewStatusList
    {
        public decimal Item_no { get; set; }
        public DateTime? sale_key_dt { get; set; }
        public decimal all_cust { get; set; }
        public decimal Assign_job { get; set; }
        public decimal not_appoint { get; set; }
        public decimal install_success { get; set; }
        public decimal cc_install { get; set; }
    }

    public class SumInstallPerformanceList
    {
        public decimal Item_no { get; set; }
        public DateTime? install_dt { get; set; }
        public string dorm_name { get; set; }
        public string Sub_contact { get; set; }
        public decimal Order_cnt { get; set; }
        public decimal install_complete { get; set; }
        public decimal install_not_complete { get; set; }
        public decimal install_tot { get; set; }
    }

    public class CusInstallTrackList
    {
        public decimal Item_no { get; set; }
        public string request_date { get; set; }
        public string dormitory_name { get; set; }
        public string room { get; set; }
        public string cus_name { get; set; }
        public string contact_mobile_phone { get; set; }
        public string status { get; set; }
        public string created_user { get; set; }
        public string Installation_Date { get; set; }
        public string Install_status { get; set; }
        public string Sub_Contract_Name { get; set; }
        public string Promotion_Description { get; set; }
        public string storage_location { get; set; }
        public string sn { get; set; }
    }

}
