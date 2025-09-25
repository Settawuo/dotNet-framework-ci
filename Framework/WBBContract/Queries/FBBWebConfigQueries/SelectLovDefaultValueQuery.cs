using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SelectLovDefaultValueQuery : IQuery<LovModel>
    {
        public string LOV_TYPE { get; set; }
    }
}
