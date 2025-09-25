using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class UpdateResendOrderBulkCorpQuery : IQuery<Resendreturndata>
    {
        public string bulk_number { get; set; }
        public string p_user { get; set; }

        public string p_no { get; set; }
        public string p_order_number { get; set; }
        public string p_installaddress1 { get; set; }
        public string p_installaddress2 { get; set; }
        public string p_installaddress3 { get; set; }
        public string p_installaddress4 { get; set; }
        public string p_installaddress5 { get; set; }
        public string p_latitude { get; set; }
        public string p_longitude { get; set; }
        public string p_install_date { get; set; }
        public string p_file_name { get; set; }

        //output
        public string p_bulk_number_return { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }

    }
}
