using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetOwnerByBuildingQuery : IQuery<OwnerByBuildingModel>
    {
        public string LanguageFlag { get; set; }
        public string Building { get; set; }
    }
}
