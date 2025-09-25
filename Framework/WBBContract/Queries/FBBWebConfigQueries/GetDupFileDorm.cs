using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetDupFileDorm : IQuery<Dupfile>
    {
        public string file_name { get; set; }
        public string user { get; set; }
        public decimal ret_Code { get; set; }
        public decimal results { get; set; }
    }
}

