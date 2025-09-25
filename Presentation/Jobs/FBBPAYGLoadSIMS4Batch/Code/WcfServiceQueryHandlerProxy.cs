using FBBPAYGLoadSIMS4Batch.QueryServices;
using WBBContract;

namespace FBBPAYGLoadSIMS4Batch.Code
{
    public sealed class WcfServiceQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public TResult Handle(TQuery query)
        {
            using (var service = new QueryServiceClient())
            {
                return (TResult)service.Execute(query);
            }
        }
    }
}
