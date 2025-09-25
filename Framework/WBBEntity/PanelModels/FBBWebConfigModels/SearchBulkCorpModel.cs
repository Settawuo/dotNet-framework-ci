namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class SearchBulkCorpModel
    {
        public string BulkNumber { get; set; }
        public string CaNumber { get; set; }
        public string TaxID { get; set; }
    }

    public class SearchBulkCorpList
    {
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }

        public string p_bulk_number { get; set; }
        public string p_tax_id { get; set; }
        public string p_ca_no { get; set; }
        public string p_summary { get; set; }
        public string p_user { get; set; }
        public string p_date { get; set; }
    }

    public class SearchBulkCorpExportList
    {
        public string p_bulk_number { get; set; }
        public string p_tax_id { get; set; }
        public string p_ca_no { get; set; }
        public string p_summary { get; set; }
        public string p_user { get; set; }
        public string p_date { get; set; }
    }

    public class SearchBulkCorpOrderNumberModel
    {
        public string BulkNumber { get; set; }
        public string Status { get; set; }
    }

    public class SearchBulkCorpOrderNumberList
    {
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }

        public string bulk_no { get; set; }
        public string order_no { get; set; }
        public string install_address1 { get; set; }
        public string install_address2 { get; set; }
        public string install_address3 { get; set; }
        public string install_address4 { get; set; }
        public string install_address5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string request_install_date { get; set; }
        public string package_code { get; set; }
        public string package_bill_tha { get; set; }
        public string package_code_dis { get; set; }
        public string package_bill_tha_dis { get; set; }
        public string status { get; set; }
        public string sff_error_message { get; set; }
        public string workflow_error_message { get; set; }
        public string fibre_net_id { get; set; }
        public string ca_no { get; set; }
        public string ba_no { get; set; }
        public string sa_no { get; set; }
        public string installcapacity { get; set; }
    }

    public class SearchBulkCorpOrderNumberExportList
    {
        public string bulk_no { get; set; }
        public string order_no { get; set; }
        public string install_address1 { get; set; }
        public string install_address2 { get; set; }
        public string install_address3 { get; set; }
        public string install_address4 { get; set; }
        public string install_address5 { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string request_install_date { get; set; }
        public string package_code { get; set; }
        public string package_bill_tha { get; set; }
        public string package_code_dis { get; set; }
        public string package_bill_tha_dis { get; set; }
        public string status { get; set; }
        public string sff_error_message { get; set; }
        public string workflow_error_message { get; set; }
        public string fibre_net_id { get; set; }
        public string ca_no { get; set; }
        public string ba_no { get; set; }
        public string sa_no { get; set; }
        public string installcapacity { get; set; }
    }

    public class retInsertBulkCorpExisting
    {
        public string output_bulk_no { get; set; }
        public string output_account_category { get; set; }
        public string output_account_sub_category { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
        public string o_id_card_no { get; set; }
        public string o_id_card_type { get; set; }
        public string o_mobile_no { get; set; }
    }
}
