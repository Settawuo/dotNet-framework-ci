using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFormatFileNameEAPPQuery : IQuery<FileFormatModel>
    {
        public string ID_CardNo { get; set; }
    }
}
