using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDataConfigLookupTableQuery : IQuery<ConfigurationLookupView> //
    {
        public string LOOKUP_NAME { get; set; }
    }
}
