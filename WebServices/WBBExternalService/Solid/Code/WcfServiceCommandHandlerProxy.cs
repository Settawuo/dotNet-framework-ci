using Microsoft.Data.OData.Query.SemanticAst;
using System;
using System.Linq;
using System.Net;
using WBBBusinessLayer;
using WBBContract;
using WBBExternalService.CommandService;

namespace WBBExternalService.Solid.Code
{
    public sealed class WcfServiceCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ILogger _logger;
        public WcfServiceCommandHandlerProxy(ILogger logger)
        {
            _logger = logger;
        }
        public void Handle(TCommand command)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string commandName = command.GetType().Name;

            using (var service = new CommandServiceClient())
            {
                SetMaxBufferSize(service);
                try
                {
                    object result = service.Execute(command);

                    Update(source: result, destination: command);
                }
                catch (WebException wex)
                {
                    if (wex.Response is HttpWebResponse response)
                    {
                        _logger.Error("command Name : " + commandName);
                        _logger.Error("Call Webservice Error StatusCode : " + response.StatusCode);
                        _logger.Error("Error StatusDescription : " + response.StatusDescription);
                    }
                }
                catch (Exception ex) {
                    _logger.Error("command Name : " + commandName);
                    _logger.Error("Call Webservice Error : " + ex.Message);
                }
            }
        }

        private static void SetMaxBufferSize(CommandServiceClient service)
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

        private static void Update(object source, object destination)
        {
            var properties =
                from property in destination.GetType().GetProperties()
                where property.CanRead && property.CanWrite
                select property;

            foreach (var property in properties)
            {
                object value = property.GetValue(source, null);

                property.SetValue(destination, value, null);
            }
        }
    }
}