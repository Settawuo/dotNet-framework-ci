using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Commands.FBBWebConfigCommands
{
    public class DeleteConfigurationLookupCommand
    {
        public string LOOKUP_NAME { get; set; }
        public string USER { get; set; }
        public string return_code { get; set; }
        public string return_msg { get; set; }
    }
}
