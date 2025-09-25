using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using WBBExternalService.CompositionRoot;
using WBBExternalService.Solid.CompositionRoot;

namespace WBBExternalService.Solid.Code
{
    public class ErrorHandler : IErrorHandler, IServiceBehavior
    {
        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints,
            BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            IErrorHandler errorHandler = new ErrorHandler();

            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;

                if (channelDispatcher != null)
                {
                    channelDispatcher.ErrorHandlers.Add(errorHandler);
                }
            }
        }

        public bool HandleError(Exception error)
        {
            //Trace.TraceError(error.ToString());
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            var err = new StringBuilder();
            err.AppendLine("Source: " + error.Source);
            err.AppendLine("Target: " + error.TargetSite.ToString());
            err.AppendLine("Message: " + error.Message);
            err.AppendLine("Stack Trace: " + error.StackTrace.ToString());

            logger.Info("Error : " + err.ToString());
            // Returning true indicates you performed your behavior.
            return true;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // Shield the unknown exception
            FaultException faultException = new FaultException(
                "Server error encountered. All details have been logged.");
            MessageFault messageFault = faultException.CreateMessageFault();

            fault = Message.CreateMessage(version, messageFault, faultException.Action);
        }
    }
}