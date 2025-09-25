using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class ConfigurationLookupView
    {
        public List<DataConfigLookupTable> dataConfigLookup { get; set; }
    }
    public class DataConfigLookup
    {
        public string LOOKUP_NAME { get; set; }
        public string RULE_NAME { get; set; }        
    }
    public class DataConfigLookupTable
    {
        public string LOOKUP_NAME { get; set; }
        public string RULE_NAME { get; set; }
        public string PAGE_INDEX { get; set; }
        public string PAGE_SIZE { get; set; }
        public string ONTOP_FLAG { get; set; }
        public string ONTOP_LOOKUP { get; set; }
        public string EFFECTIVE_DATE_START { get; set; }
        public DateTime EFFECTIVE_DATE_START_DT { get; set; }
        public string FLAG_DELETE { get; set; }
    }
    public class ConfigLookupResponse
    {
        public ConfigLookupResponse()
        {
            if (result_lookup_search_cur == null)
            {
                result_lookup_search_cur = new List<DataConfigLookupTable>();
            }           

        }
        public string ret_code { get; set; }
        public string ret_msg { get; set; }
        public List<DataConfigLookupTable> result_lookup_search_cur { get; set; }
    }
    public class DeleteConfigLookup
    {
        public string LOOKUP_NAME { get; set; }
        public string USER { get; set; }
    }
    public class DataDropDownLookup
    {
        public string LovValue1 { get; set; }
    }
    public class SymptomNameDropDown
    {
        public string LovValue1 { get; set; }
    }
    public class DistrictDropDown
    {
        public string LovValue1 { get; set; }
    }
    public class SubDistrictDropDown
    {
        public string LovValue1 { get; set; }
    }
    public class LookupDataInsert
    {
        public string lookup_name { get; set; }
        public string lookup_ontopflag { get; set; }
        public string lookup_ontop { get; set; }
        public List<List<LookupHeaderList>> lookup_header_list { get; set; }
        public string user { get; set; }
    }
    public class LookupHeaderList
    {
        public string parameter_name { get; set; }
        public string lookup_flag { get; set; }
        public string parameter_value { get; set; }                       
    }
    public class LookupDataUpdate
    {
        public string lookup_name { get; set; }
        public string lookup_ontopflag { get; set; }
        public string lookup_ontop { get; set; }
        public List<List<LookupHeaderListUpdate>> lookup_header_list { get; set; }
        public string user { get; set; }
    }
    public class LookupHeaderListUpdate
    {
        //public string LOOKUP_ID { get; set; }
        //public string lookup_status { get; set; }
        public string parameter_name { get; set; }
        public string lookup_flag { get; set; }
        public string parameter_value { get; set; }
    }
}
