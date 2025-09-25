using WBBEntity.PanelModels;

namespace WBBContract.Queries.WebServices
{
    public class GetPathPdfQuery : IQuery<GetPathPdfModel>
    {
        public string OderNo { get; set; }
    }
}
