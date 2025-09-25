using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetScreenBulkCorpQuery : IQuery<BatchBulkCorpModel>
    {
        public string P_BULK_NUMBER { get; set; }
    }

    public class GetScreenBulkCorpSFFQuery : IQuery<BatchBulkCorpSFFModel>
    {
        public string P_BULK_NUMBER { get; set; }

    }
}
