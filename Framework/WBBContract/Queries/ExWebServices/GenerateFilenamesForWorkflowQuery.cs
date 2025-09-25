using System.Collections.Generic;
using WBBEntity.PanelModels.ExWebServiceModels;

namespace WBBContract.Queries.ExWebServices
{
    public class GenerateFilenamesForWorkflowQuery : IQuery<GenerateFilenamesForWorkflowModel>
    {
        public string in_order_no { get; set; }
        public List<in_filenames_data> in_filenames { get; set; }
        public string in_user_name { get; set; }
    }

    public class in_filenames_data
    {
        public string FILE_NAME { get; set; }
    }
}
