using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetBulkCorpSearchBulkCorpOrderNumberQuery : IQuery<List<SearchBulkCorpOrderNumberList>>
    {
        public string P_BULK_NUMBER { get; set; }
        public string P_STATUS { get; set; }

        // return code
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
