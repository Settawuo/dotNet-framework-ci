using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.PanelModels.FBBWebConfigModels
{
    public class GetListPLookupNameModel
    {
        public decimal ROW_ID { get; set; }
        public string LOOKUP_ID { get; set; }
        public string LOOKUP_NAME { get; set; }
        public string PARAMETER_NAME { get; set; }
        public string PARAMETER_VALUE { get; set; }
    }
}
