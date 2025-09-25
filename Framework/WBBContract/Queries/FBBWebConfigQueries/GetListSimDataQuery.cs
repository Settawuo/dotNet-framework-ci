using System.Collections.Generic;
using WBBContract.Commands;

namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class GetListSimDataQuery : IQuery<List<LoadSimS4GetDataQueryModel>>
    {
        public string p_sheet_name { get; set; }
    }

    public class LoadSimS4GetDataQueryModel
    {
        public string DATA_BUFFER { get; set; }
    }
}
