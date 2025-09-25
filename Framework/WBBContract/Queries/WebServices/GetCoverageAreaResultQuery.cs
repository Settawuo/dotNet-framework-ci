using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetCoverageAreaResultQuery : IQuery<CoverageAreaResultModel>
    {
        public string TRANSACTION_ID { get; set; }
    }

    public class GetCoverageAreaResultByResultIDQuery : IQuery<CoverageAreaResultModel>
    {
        public string ResultID { get; set; }
    }
}
