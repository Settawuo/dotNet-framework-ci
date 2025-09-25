using System.Collections.Generic;

namespace WBBEntity.PanelModels.ExWebServiceModels
{
    public class GenerateFilenamesForWorkflowModel
    {
        public string out_return_code { get; set; }
        public List<out_result_data> out_result { get; set; }
    }

    public class out_result_data
    {
        public string file_name { get; set; }
    }
}
