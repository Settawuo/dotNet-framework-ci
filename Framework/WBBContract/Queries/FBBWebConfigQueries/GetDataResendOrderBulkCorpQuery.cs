using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDataResendOrderBulkCorpQuery : IQuery<ResendOrderBulkCorpModel>
    {
        public string p_bulk_number { get; set; }
    }
}
