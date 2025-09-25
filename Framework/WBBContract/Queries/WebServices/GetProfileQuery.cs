using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetProfileQuery : IQuery<GetProfileQueryModel>
    {
        public string authorization { get; set; }
    }
}
