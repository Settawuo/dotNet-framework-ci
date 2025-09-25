using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetoutPostalCode :IQuery<string>
    {
        public string outPostalCode {get; set;}
        public string outTumbol { get; set; }
        public string outAmphur { get; set; }
        public string outProvince { get; set; }
        public string FagDataQuery { get; set; }

    }
}
