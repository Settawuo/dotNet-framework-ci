using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class InsertBulkCorpExistingQuery : IQuery<retInsertBulkCorpExisting>
    {
        public string p_user { get; set; }
        public string p_asc_code { get; set; }
        public string p_employee_id { get; set; }
        public string p_location_code { get; set; }
        public string p_bulk_number { get; set; }

        //return
        public string output_bulk_no { get; set; }
        public string output_account_category { get; set; }
        public string output_account_sub_category { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }

    }
}
