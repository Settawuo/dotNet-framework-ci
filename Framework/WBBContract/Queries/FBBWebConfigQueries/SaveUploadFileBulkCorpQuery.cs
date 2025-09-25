using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class SaveUploadFileBulkCorpQuery : IQuery<ReturnUploadData>
    {
        public string p_bulk_number { get; set; }
        public string p_file_name { get; set; }
    }
}
