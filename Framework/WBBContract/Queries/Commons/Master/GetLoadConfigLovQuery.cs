using WBBEntity.PanelModels;

namespace WBBContract.Queries.Commons.Masters
{
    public class GetLoadConfigLovQuery : IQuery<LoadConfigLovModel>
    {
        public string EVENT_NAME { get; set; }
    }
}
