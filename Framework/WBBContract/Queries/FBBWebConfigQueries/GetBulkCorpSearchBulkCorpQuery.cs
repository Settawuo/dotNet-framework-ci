using System.Collections.Generic;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetBulkCorpSearchBulkCorpQuery : IQuery<List<SearchBulkCorpList>>
    {
        public string P_BULK_NUMBER { get; set; }
        public string P_CA_NUMBER { get; set; }
        public string P_TAX_ID { get; set; }

        // return code
        public int output_return_code { get; set; }
        public string output_return_message { get; set; }
    }
}
