using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class AddConfigurationLookupCommand
    {
        public string lookup_name { get; set; }
        public string lookup_ontopflag { get; set; }
        public string lookup_ontop { get; set; }
        public List<LookupDataList> lookup_list { get; set; }
        public string create_by { get; set; }
        public string modify_by { get; set; }
        public string return_code { get; set; }
        public string return_msg { get; set; }        
    }
    public class LookupDataList
    {
        public string lookup_id { get; set; }
        public string lookup_status { get; set; }
        public List<lookupDataHeader> lookup_header_list { get; set; }
    }
    public class lookupDataHeader
    {
        public string parameter_name { get; set; }
        public string parameter_value { get; set; }
        public string lookup_flag { get; set; }
    }
}
