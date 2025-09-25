using FBBPAYG_LoadFile_3BBReport.QueryServices;
using WBBContract;
namespace FBBPAYG_LoadFile_3BBReport.Code
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