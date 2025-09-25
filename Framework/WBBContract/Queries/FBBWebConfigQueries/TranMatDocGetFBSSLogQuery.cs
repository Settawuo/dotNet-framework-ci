using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    //class TranMatDocGetFBSSLogQuery
    //{
    //}

    public class TranMatDocGetFBSSLogQuery : IQuery<List<GetTranMatDocGetFBSSLogQuery>>
    {
        public string inbound_filename { get; set; }

        public String inbound_comDayCallback { get; set; }
    }

    public class GetTranMatDocGetFBSSLogQuery
    {
        //Return 
        public string file_name { get; set; }
        public string flag_type { get; set; }
    }
    
}
