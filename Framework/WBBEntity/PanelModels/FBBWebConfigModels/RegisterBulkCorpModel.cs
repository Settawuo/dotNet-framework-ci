using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class RegisterBulkCorpModel
    {

        public string p_user { get; set; }
        public string asc_code { get; set; }
        public string employee_id { get; set; }
        public string location_code { get; set; }

        public string id_card_no { get; set; }
        public string account_category { get; set; }
        public string account_sub_category { get; set; }
        public string id_card_type { get; set; }
        public string account_title { get; set; }
        public string account_name { get; set; }
        public string ca_house_no { get; set; }
        public string ca_moo { get; set; }
        public string ca_mooban { get; set; }
        public string ca_building_name { get; set; }
        public string ca_floor { get; set; }
        public string ca_room { get; set; }
        public string ca_soi { get; set; }
        public string ca_street { get; set; }
        public string ca_sub_district { get; set; }
        public string ca_district { get; set; }
        public string ca_province { get; set; }
        public string ca_postcode { get; set; }
        public string ca_phone { get; set; }
        public string ca_main_mobile { get; set; }

        public string ba_language { get; set; }
        public string ba_bill_name { get; set; }
        public string ba_bill_cycle { get; set; }
        public string ba_house_no { get; set; }
        public string ba_moo { get; set; }
        public string ba_mooban { get; set; }
        public string ba_building_name { get; set; }
        public string ba_floor { get; set; }
        public string ba_room { get; set; }
        public string ba_soi { get; set; }
        public string ba_street { get; set; }
        public string ba_sub_district { get; set; }
        public string ba_district { get; set; }
        public string ba_province { get; set; }
        public string ba_postcode { get; set; }
        public string ba_phone { get; set; }
        public string ba_main_mobile { get; set; }

        //return
        public string out_bulk_number { get; set; }
        public string out_return_code { get; set; }
        public string out_return_message { get; set; }
        public List<BulkAddress> BulkAddrList { get; set; }
        public List<UploadImageBulk> ListImageFileBulk { get; set; }
        //public UploadImageB ListImageFileB { get; set; }
        public string ClientIP { get; set; }
        public string Register_device { get; set; }
    }


    public class UploadImageBulk
    {
        public string FileNameBulk { get; set; }
    }

    public class RegisterBulkCorpData
    {
        public string output_bulk_number { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<BulkAddress> BulkAddList { get; set; }
    }

    public class BulkAddress
    {
        public string Account_Title { get; set; }
        public string Name { get; set; }
        public string VAT_Name { get; set; }
        public string id_card_type { get; set; }
        public string ID_Card_Number { get; set; }
        public string Main_Phone { get; set; }
        public string Main_Mobile { get; set; }
        public string Home_VAT_Address { get; set; }
        public string Billing_Account_Name { get; set; }
        public string Bill_Name { get; set; }
        public string Bill_Address { get; set; }
        public string Bill_Main_Phone { get; set; }
        public string Bill_Main_Mobile { get; set; }
        public string Bill_Cycle { get; set; }
        public string Bill_Language { get; set; }

    }

}


