using FBBPAYG_GenFile_DisneyPB.QueryServices;
using WBBContract;
namespace FBBPAYG_GenFile_DisneyPB.Code
{
   
    public sealed class WcfServiceQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        public TResult Handle(TQuery query)
        {
            using (QueryServiceClient queryServiceClient = new QueryServiceClient())
                return (TResult)queryServiceClient.Execute((object)query);
        }
    }
}
