using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetListLookupParamModel
    {
        public string PARAMETER_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
    }

    public class ConfigurationLookupParamView
    {
        public List<DataConfigLookupParamTable> dataConfigLookupParam { get; set; }
    }
    public class DataConfigLookupParam
    {
        public string PARAMETER_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
    }
    public class DataConfigLookupParamTable
    {
        public string PARAMETER_NAME { get; set; }
        public string DISPLAY_VAL { get; set; }
    }
    public class ConfigLookupParamResponse
    {
        public ConfigLookupParamResponse()
        {
            if (result_lookup_param_cur == null)
            {
                result_lookup_param_cur = new List<DataConfigLookupParamTable>();
            }

        }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<DataConfigLookupParamTable> result_lookup_param_cur { get; set; }
    }
    
}
