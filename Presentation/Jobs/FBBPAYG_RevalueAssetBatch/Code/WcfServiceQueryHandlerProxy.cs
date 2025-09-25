using FBBPAYG_RevalueAssetBatch.QueryServices;
using WBBContract;

namespace FBBPAYG_RevalueAssetBatch.Code
{
    public sealed class WcfServiceQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public TResult Handle(TQuery query)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //ServicePointManager.Expect100Continue = true;
            //ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            using (var service = new QueryServiceClient())
            {
                return (TResult)service.Execute(query);
            }
        }
    }
}
