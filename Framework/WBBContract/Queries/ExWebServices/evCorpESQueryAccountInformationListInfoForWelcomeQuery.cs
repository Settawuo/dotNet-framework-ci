using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class evCorpESQueryAccountInformationListInfoForWelcomeQuery : IQuery<evCorpESQueryAccountInformationListInfoForWelcomeModel>
    {
        public string inAccountNo { get; set; }
    }
}
