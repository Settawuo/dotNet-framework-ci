using System;
using System.Net;
using System.ServiceModel;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBWeb.CompositionRoot;
using WBBWeb.QueryService;
using WBBWeb.Solid.CompositionRoot;

namespace WBBWeb.Solid.Code
{
    public sealed class WcfServiceQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private ILogger _Logger = Bootstrapper.GetInstance<DebugLogger>();

        public TResult Handle(TQuery query)
        {
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true;
            using (var service = new QueryServiceClient())
            {
                SetMaxBufferSize(service);
                //log data
                if (query.GetType().Name == "GetSaveOrderRespQuery")
                {
                    try
                    {
                        GetSaveOrderRespQuery getSaveOrderRespQuery = query as GetSaveOrderRespQuery;
                        _Logger.Info(string.Format("MOBILE = {0} : Data sent Execute = {1}"
                            , getSaveOrderRespQuery.QuickWinPanelModel.CustomerRegisterPanelModel.L_MOBILE
                            , getSaveOrderRespQuery.QuickWinPanelModel.DumpToXml()));
                    }
                    catch (Exception ex1)
                    {
                        _Logger.Info(string.Format("Exception WcfServiceQueryHandlerProxy Execute {0}", ex1.Message + ex1.StackTrace));
                    }
                }
                return (TResult)service.Execute(query);
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

                binding.Security.Mode = service.Endpoint.Address.Uri.Scheme == "https" ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportCredentialOnly;
            }
        }
    }
}