namespace FBBDelectInterfaceLogARCBatch.Code
{
#pragma warning disable CS0234 // The type or namespace name 'QueryServices' does not exist in the namespace 'FBBDelectInterfaceLogARCBatch' (are you missing an assembly reference?)
    using FBBDelectInterfaceLogARCBatch.QueryServices;
#pragma warning restore CS0234 // The type or namespace name 'QueryServices' does not exist in the namespace 'FBBDelectInterfaceLogARCBatch' (are you missing an assembly reference?)
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
