using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetBulkNumberforExistingQuery : IQuery<returnBulkNumber>
    {
        public string p_user { get; set; }

        //return
        public string output_bulk_no { get; set; }
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
