using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBEntity.Models;

namespace WBBContract.Queries.WebServices
{
    public class GetTokenFbbQuery : IQuery<GetTokenFbbModel>
    {
       public string Channel {  get; set; }
       public ParametersGetoken ParamGetoken { get; set; }
    }
     
    public class ParametersGetoken
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
    }
}
