using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Caching;
using WBBBusinessLayer;
using WBBBusinessLayer.Extension;
using WBBContract;
using WBBExternalService.QueryService;

namespace WBBExternalService.Solid.Code
{
    public sealed class WcfServiceQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private readonly ILogger _logger;

        public WcfServiceQueryHandlerProxy(ILogger logger)
        {
            _logger = logger;
        }

        public TResult Handle(TQuery query)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string queryName = query.GetType().Name;

            using (var service = new QueryServiceClient())
            {
                SetMaxBufferSize(service);
                try
                {
                    return (TResult)service.Execute(query);
                }
                catch (WebException wex)
                {
                    if (wex.Response is HttpWebResponse response)
                    {
                        _logger.Error("Query Name : " + queryName);
                        _logger.Error("Call Webservice Error StatusCode : " + response.StatusCode);
                        _logger.Error("Error StatusDescription : " + response.StatusDescription);
                    }
                    
                    return default(TResult);
                }
                catch (Exception ex)
                {
                    _logger.Error("Query Name : " + queryName);
                    _logger.Error("Call Webservice Error : " + ex.Message);
                    return default(TResult);
                }
            }
        }

        private static void SetMaxBufferSize(QueryServiceClient service)
        {
            if (service.Endpoint.Binding is System.ServiceModel.BasicHttpBinding)
            {
                System.ServiceModel.BasicHttpBinding binding = (System.ServiceModel.BasicHttpBinding)service.Endpoint.Binding;

                var maxVal = int.MaxValue;

                binding.OpenTimeout = new TimeSpan(00, 20, 00);
                binding.CloseTimeout = new TimeSpan(00, 20, 00);
                binding.ReceiveTimeout = new TimeSpan(00, 20, 00);
                binding.SendTimeout = new TimeSpan(00, 20, 00);

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