using FBBConfig.QueryService;
using System;
using System.Net;
using System.Threading.Tasks;
using WBBContract;

namespace FBBConfig.Solid.Code
{
    public sealed class WcfServiceQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        public TResult Handle(TQuery query)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var service = new QueryServiceClient())
            {
                SetMaxBufferSize(service);
                return (TResult)service.Execute(query);
            }
        }

        private static void SetMaxBufferSize(QueryServiceClient service)
        {
            if (service.Endpoint.Binding is System.ServiceModel.BasicHttpBinding)
            {
                System.ServiceModel.BasicHttpBinding binding = (System.ServiceModel.BasicHttpBinding)service.Endpoint.Binding;

                var maxVal = int.MaxValue;

                binding.OpenTimeout = new TimeSpan(00, 60, 00);
                binding.CloseTimeout = new TimeSpan(00, 60, 00);
                binding.ReceiveTimeout = new TimeSpan(00, 60, 00);
                binding.SendTimeout = new TimeSpan(00, 60, 00);

                //binding.TransferMode = System.ServiceModel.TransferMode.Streamed;
                binding.MaxBufferSize = maxVal;
                binding.MaxBufferPoolSize = maxVal;
                binding.MaxReceivedMessageSize = maxVal;
                binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
                {
                    MaxArrayLength = maxVal,
                    MaxStringContentLength = maxVal
                };
            }
        }
    }



    public sealed class WcfServiceQueryHandlerProxyAsync<TQuery, TResult> : IQueryHandlerAsync<TQuery, TResult>
       where TQuery : IQuery<TResult>
    {
        public async Task<TResult> HandleAsync(TQuery query)
        {
            using (var service = new QueryServiceClient())
            {
                await service.ExecuteAsync(query);
            }

            return default(TResult);
        }
    }

}