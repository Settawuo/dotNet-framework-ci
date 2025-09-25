using System.Collections.Generic;
namespace WBBContract.Queries.FBBWebConfigQueries
{
    public class LastMileByDistanceBatchQuery : IQuery<List<string>>
    {
        public string ErrorMessage { get; set; }
    }
}
