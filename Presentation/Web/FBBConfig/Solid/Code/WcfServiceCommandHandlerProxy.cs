using FBBConfig.CommandService;
using System;
using System.Linq;
using System.Net;
using WBBContract;

namespace FBBConfig.Solid.Code
{
    public sealed class WcfServiceCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    {
        public void Handle(TCommand command)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var service = new CommandServiceClient())
            {
                SetMaxBufferSize(service);
                object result = service.Execute(command);

                Update(source: result, destination: command);
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

        private static void SetMaxBufferSize(CommandServiceClient service)
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
}