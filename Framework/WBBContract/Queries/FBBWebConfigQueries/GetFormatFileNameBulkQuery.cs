using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetFormatFileNameBulkQuery : IQuery<List<FileFormatModelBulk>>
    {
        public string language { get; set; }
        public string ID_CardType { get; set; }
        public string ID_CardNo { get; set; }
        public List<UploadImageBulk> ListFilenameBulk { get; set; }
    }
}
