using System.Collections.Generic;
using WBBEntity.PanelModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFormatFileNameQuery : IQuery<List<FileFormatModel>>
    {
        public string language { get; set; }
        public string ID_CardType { get; set; }
        public string ID_CardNo { get; set; }
        public List<UploadImage> ListFilename { get; set; }

    }
}
