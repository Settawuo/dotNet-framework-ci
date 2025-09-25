using System.Collections.Generic;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ResendOrderBulkCorpModel
    {
        public string p_bulk_number { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<ResendPmsg> p_message { get; set; }
        public List<ResendTech> p_resend_tech { get; set; }
        public List<ResendPackMain> p_package_main { get; set; }
        public List<ResendPackOntopDis> p_package_discount { get; set; }

    }

    public class ResendAccount
    {
        public string BULK_NUMBER { get; set; }
        public string TAX_ID { get; set; }
        public string TAX_ID_TYPE { get; set; }
        public string ACCOUNT_NAME { get; set; }
        public string ACCOUNT_CATE { get; set; }
        public string ACCOUNT_SUB { get; set; }
        public string CA_NUMBER { get; set; }
        public string SA_NUMBER { get; set; }
        public string BA_NUMBER { get; set; }
    }

    public class ResendPmsg
    {
        public string p_message { get; set; }

    }

    public class ResendPackMain
    {
        public string PACKAGE_MAIN_CODE { get; set; }
        public string PACKAGE_MAIN_NAME { get; set; }
        public string PACKAGE_MAIN_DESCRIP { get; set; }
        public string EDIT_BUTTON { get; set; }
    }

    public class ResendPackOntopDis
    {
        public string PACKAGE_DISCOUNT_CODE { get; set; }
        public string PACKAGE_DISCOUNT_NAME { get; set; }
        public string PACKAGE_DISCOUNT_DESCRIP { get; set; }
        public string EDIT_BUTTON { get; set; }
    }

    public class ResendCA
    {
        public string HOUSE_NUMBER { get; set; }
        public string MOO { get; set; }
        public string MOOBAN { get; set; }
        public string BUILDING_NAME { get; set; }
        public string FLOOR { get; set; }
        public string ROOM { get; set; }
        public string SOI { get; set; }
        public string STREET { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string PROVINCE { get; set; }
        public string POSTCODE { get; set; }
        public string PHONE { get; set; }
        public string MAIN_MOBILE { get; set; }
    }

    public class ResendBA
    {
        public string BILL_NAME { get; set; }
        public string BILL_CYCLE { get; set; }
        public string HOUSE_NUMBER { get; set; }
        public string MOO { get; set; }
        public string MOOBAN { get; set; }
        public string BUILDING_NAME { get; set; }
        public string FLOOR { get; set; }
        public string ROOM { get; set; }
        public string SOI { get; set; }
        public string STREET { get; set; }
        public string SUB_DISTRICT { get; set; }
        public string DISTRICT { get; set; }
        public string PROVINCE { get; set; }
        public string POSTCODE { get; set; }
        public string PHONE { get; set; }
        public string MAIN_MOBILE { get; set; }
    }

    public class ResendTech
    {
        public string TYPE_TECHNOLOGY { get; set; }
        public string ADDRESS_ID { get; set; }
        public string EVENT_CODE { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MAIN_PHONE { get; set; }
        public string MAIN_MOBILE { get; set; }
        public string EMAIL { get; set; }
        public string EDIT_BUTTON { get; set; }
    }

    public class ResendAccBulk
    {
        public string p_id_card_no { get; set; }
        public string p_account_category { get; set; }
        public string p_account_sub_category { get; set; }
        public string p_id_card_type { get; set; }
        public string p_account_title { get; set; }
        public string p_account_name { get; set; }

    }

    public class ResendCustInfo
    {
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
    }

    public class ResendBillInfo
    {
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
    }

    public class Resendreturndata
    {
        public string p_bulk_number_return { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }

    public class ResendGetData
    {
        public string p_bulk_number { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public List<ResendAccount> p_resend_account { get; set; }
        public List<ResendCA> p_resend_ca { get; set; }
        public List<ResendBA> p_resend_ba { get; set; }
        public List<ResendTech> p_resend_tech { get; set; }
    }

}
