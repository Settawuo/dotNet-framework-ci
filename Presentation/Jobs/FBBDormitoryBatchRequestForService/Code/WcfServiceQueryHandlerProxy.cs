namespace FBBDormitoryBatchRequestForService.Code
{
    using FBBDormitoryBatchRequestForService.QueryServices;
    using One2NetContract;

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