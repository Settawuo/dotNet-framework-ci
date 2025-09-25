using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBContract.Queries.WebServices
{
    public class DeniedAccessToken : IQuery<string>
    {
        public string id_token { get; set; }
        public string redirect_uri { get; set; }
    }
}
