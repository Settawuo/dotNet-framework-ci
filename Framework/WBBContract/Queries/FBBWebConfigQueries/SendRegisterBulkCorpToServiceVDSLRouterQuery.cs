using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SendRegisterBulkCorpToServiceVDSLRouterQuery : IQuery<returnServiceVDSLRouterData>
    {
        public string p_bulk_number { get; set; }

        //return
        public string output_bulk_no { get; set; }
        public string output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
