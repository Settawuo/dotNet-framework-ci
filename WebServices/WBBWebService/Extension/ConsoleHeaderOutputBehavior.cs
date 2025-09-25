using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace WBBWebService.Extension
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsoleHeaderOutputBehavior : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            for (int i = 0; i < serviceHostBase.ChannelDispatchers.Count; i++)
            {
                ChannelDispatcher channelDispatcher = serviceHostBase.ChannelDispatchers[i] as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        ConsoleOutputHeadersMessageInspector inspector = new ConsoleOutputHeadersMessageInspector();
                        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }

    public class ConsoleOutputHeadersMessageInspector : IDispatchMessageInspector
    {
        private static string _orderRowId = null;

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            request = buffer.CreateMessage();


            Message originalMessage = buffer.CreateMessage();

            originalMessage.Headers.RemoveAll("Action", "http://schemas.microsoft.com/ws/2005/05/addressing/none");
            originalMessage.Headers.RemoveAll("To", "http://schemas.microsoft.com/ws/2005/05/addressing/none");

            // Get the SOAP XML content.
            string strMessage = originalMessage.ToString();

            // Get the SOAP XML body content.
            System.Xml.XmlDictionaryReader xrdr = originalMessage.GetReaderAtBodyContents();
            var bodyData = xrdr.ReadOuterXml();
            if (bodyData.IndexOf(WBBEntity.Extensions.Constants.SbnWebService.CUSTREGISTERCOMMAND.ToString(), System.StringComparison.OrdinalIgnoreCase) > 0)
            {
                // Replace the body placeholder with the actual SOAP body.
                if (strMessage != null)
                {
                    strMessage = strMessage.Replace("... stream ...", bodyData);

                    //Insert Received log
                    //SaveDataFromSoapMessage(WBBEntity.Extensions.Constants.SbnWebService.CUSTREGISTERCOMMAND.ToString(),strMessage,ActionType.Insert);

                    // Display the SOAP XML.
                    //Trace.WriteLine(string.Format("Received:\n{0}", strMessage));
                }
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            reply = buffer.CreateMessage();
            //Message originalMessage = buffer.CreateMessage();
            string strMessage = buffer.CreateMessage().ToString();

            if (strMessage != null && strMessage.IndexOf(WBBEntity.Extensions.Constants.SbnWebService.CUSTREGISTERCOMMAND.ToString(), System.StringComparison.OrdinalIgnoreCase) > 0)
            {
                //Update Sending log
                //SaveDataFromSoapMessage(WBBEntity.Extensions.Constants.SbnWebService.CUSTREGISTERCOMMAND.ToString(), strMessage, ActionType.Update);

                //Trace.WriteLine(string.Format("Sending:\n{0}", strMessage));

            }
        }

        //private static void SaveDataFromSoapMessage(string methodName, string soapMessage, ActionType action)
        //{
        //    var logger = new DebugLogger(null);
        //    logger.Info("SoapXml Raw = " + soapMessage);

        //    var outgoingOrderNew = Bootstrapper.GetInstance<ISaveOutgoingMessageCommandHandler>();
        //    var outgoingOrderNewCommand = new SaveOutgoingMessageCommand
        //    {
        //        MethodName = methodName,
        //        Action = action,
        //        SoapXml = soapMessage,
        //        OrderRowId = _orderRowId,
        //        MobileNo = ""
        //    };
        //    outgoingOrderNew.Handle(outgoingOrderNewCommand);
        //    _orderRowId = outgoingOrderNewCommand.OrderRowId;


        //}
    }
}