using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using WBBContract.Commands;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetListExpireSimDataQuery : IQuery<List<LoadExpireSimS4GetDataQueryModel>>
    {
        public string sheet_query { get; set; }
    }

    public class LoadExpireSimS4GetDataQueryModel
    {
        public string data_buffer { get; set; }
    }
}
