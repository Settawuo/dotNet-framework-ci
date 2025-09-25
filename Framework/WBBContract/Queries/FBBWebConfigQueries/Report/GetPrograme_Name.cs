using System;
using System.Collections.Generic;

namespace WBBContract.Queries.FBBWebConfigQueries.Report
{
    public class GetPrograme_Name : IQuery<List<ListProgram>>
    {
        public String Progrmer_Code { get; set; }

    }
}
