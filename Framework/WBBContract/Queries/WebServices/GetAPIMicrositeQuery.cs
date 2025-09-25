using WBBEntity.PanelModels.WebServiceModels;

namespace WBBContract.Queries.WebServices
{
    public class GetAPIMicrositeQuery : IQuery<GetAPIMicrositeModel>
    {
        public string Transaction_Id { get; set; }
        public string App_Source { get; set; }
        public string App_Destination { get; set; }
        public string Content_Type { get; set; }
        public string BodyJson { get; set; }
    }
}
