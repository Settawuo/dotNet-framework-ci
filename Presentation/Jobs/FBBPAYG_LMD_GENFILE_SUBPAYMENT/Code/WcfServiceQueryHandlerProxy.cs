namespace FBBPAYG_LMD_GENFILE_SUBPAYMENT.Code
{
    using FBBPAYG_LMD_GENFILE_SUBPAYMENT.QueryService;
    using WBBContract;

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
