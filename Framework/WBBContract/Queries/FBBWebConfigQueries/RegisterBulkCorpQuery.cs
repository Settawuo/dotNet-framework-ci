using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class RegisterBulkCorpQuery : IQuery<RegisterBulkCorpData>
    {
        public string p_user { get; set; }
        public string p_asc_code { get; set; }
        public string p_employee_id { get; set; }
        public string p_location_code { get; set; }

        public string p_id_card_no { get; set; }
        public string p_account_category { get; set; }
        public string p_account_sub_category { get; set; }
        public string p_id_card_type { get; set; }
        public string p_account_title { get; set; }
        public string p_account_name { get; set; }
        public string p_ca_house_no { get; set; }
        public string p_ca_moo { get; set; }
        public string p_ca_mooban { get; set; }
        public string p_ca_building_name { get; set; }
        public string p_ca_floor { get; set; }
        public string p_ca_room { get; set; }
        public string p_ca_soi { get; set; }
        public string p_ca_street { get; set; }
        public string p_ca_sub_district { get; set; }
        public string p_ca_district { get; set; }
        public string p_ca_province { get; set; }
        public string p_ca_postcode { get; set; }
        public string p_ca_phone { get; set; }
        public string p_ca_main_mobile { get; set; }

        public string p_ba_language { get; set; }
        public string p_ba_bill_name { get; set; }
        public string p_ba_bill_cycle { get; set; }
        public string p_ba_house_no { get; set; }
        public string p_ba_moo { get; set; }
        public string p_ba_mooban { get; set; }
        public string p_ba_building_name { get; set; }
        public string p_ba_floor { get; set; }
        public string p_ba_room { get; set; }
        public string p_ba_soi { get; set; }
        public string p_ba_street { get; set; }
        public string p_ba_sub_district { get; set; }
        public string p_ba_district { get; set; }
        public string p_ba_province { get; set; }
        public string p_ba_postcode { get; set; }
        public string p_ba_phone { get; set; }
        public string p_ba_main_mobile { get; set; }

        //return
        public string output_bulk_number { get; set; }
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }

    }

}
