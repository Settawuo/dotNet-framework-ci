using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

namespace WBBExternalService
{


    /// <summary>
    /// SOAP XML worker method to view incoming and    /// outgoing SOAP messages to the WCF service.
    /// </summary>
    public class ConsoleOutputMessageInspector : IClientMessageInspector, IDispatchMessageInspector
    {
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            // Make a copy of the SOAP packet for viewing.


            //MessageBuffer buffer = request.CreateBufferedCopy(Int32.MaxValue);
            //Message msgCopy = buffer.CreateMessage();
            //request = buffer.CreateMessage();
            //// Get the SOAP XML content.
            //string strMessage = buffer.CreateMessage().ToString();
            //// Get the SOAP XML body content.
            //System.Xml.XmlDictionaryReader reader = msgCopy.GetReaderAtBodyContents();

            //do
            //{
            //    switch (reader.NodeType)
            //    {
            //        case XmlNodeType.Element:
            //            Console.Write("<{0}", reader.Name);
            //            while (reader.MoveToNextAttribute())
            //            {
            //                Console.Write(" {0}='{1}'", reader.Name, reader.Value);
            //                System.Diagnostics.Debug.WriteLine(" {0}='{1}'", reader.Name, reader.Value);
            //            }
            //            Console.Write(">");
            //            System.Diagnostics.Debug.WriteLine(">");

            //            break;
            //        case XmlNodeType.Text:
            //            Console.Write(reader.Value);
            //            System.Diagnostics.Debug.WriteLine(reader.Value);
            //            break;
            //        case XmlNodeType.EndElement:
            //            Console.Write("</{0}>", reader.Name);
            //            System.Diagnostics.Debug.WriteLine("</{0}>", reader.Name);
            //            break;
            //    }
            //} while (reader.Read());

            //string bodyData = reader.ReadOuterXml();
            //// Replace the body placeholder with the actual SOAP body.
            //strMessage = strMessage.Replace("... stream ...", bodyData);
            //// Display the SOAP XML.
            //System.Diagnostics.Debug.WriteLine("Received:\n" + strMessage);

            TransformMessage2(ref request);

            return null;
        }
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            // Make a copy of the SOAP packet for viewing.
            MessageBuffer buffer = reply.CreateBufferedCopy(Int32.MaxValue);
            reply = buffer.CreateMessage();
            // Display the SOAP XML.
            System.Diagnostics.Debug.WriteLine("Sending:\n" + buffer.CreateMessage().ToString());
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            throw new NotImplementedException();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            System.Diagnostics.Debug.WriteLine("Original: {0}", request);
            ChangeMessage(ref request);
            System.Diagnostics.Debug.WriteLine("Updated: {0}", request);
            return null;
        }

        private void ChangeMessage(ref Message message)
        {
            MemoryStream ms = new MemoryStream();
            Encoding encoding = Encoding.UTF8;
            XmlWriterSettings writerSettings = new XmlWriterSettings { Encoding = encoding };
            XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(ms));
            message.WriteBodyContents(writer);
            writer.Flush();
            string messageBodyString = encoding.GetString(ms.ToArray());

            // change the message body
            messageBodyString = messageBodyString.Replace("&", "555xx555");

            ms = new MemoryStream(encoding.GetBytes(messageBodyString));
            XmlReader bodyReader = XmlReader.Create(ms);
            Message originalMessage = message;
            message = Message.CreateMessage(originalMessage.Version, null, bodyReader);
            message.Headers.CopyHeadersFrom(originalMessage);
        }
        private Message TransformMessage2(ref Message oldMessage)
        {

            Message newMessage = null;


            MessageBuffer buffer = oldMessage.CreateBufferedCopy(Int32.MaxValue);
            Message msgCopy = buffer.CreateMessage();
            oldMessage = buffer.CreateMessage();
            // Get the SOAP XML content.
            string strMessage = buffer.CreateMessage().ToString();
            // Get the SOAP XML body content.
            System.Xml.XmlDictionaryReader reader = msgCopy.GetReaderAtBodyContents();
            string bodyData = reader.ReadOuterXml();
            // Replace the body placeholder with the actual SOAP body.
            strMessage = strMessage.Replace("... stream ...", bodyData);
            // Display the SOAP XML.
            System.Diagnostics.Debug.WriteLine("Received:\n" + strMessage);

            System.Xml.XmlDocument xdoc = new XmlDocument();

            xdoc.LoadXml(strMessage);
            reader.Close();

            //transform the xmldocument

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(xdoc.NameTable);

            nsmgr.AddNamespace("wbb", "http://tempuri.org/");



            XmlNode node = xdoc.SelectSingleNode("//wbb:BUILDING_NAME", nsmgr);

            if (node != null) node.InnerText = "[Modified in SimpleMessageInspector]" + node.InnerText;





            MemoryStream ms = new MemoryStream();

            XmlWriter xw = XmlWriter.Create(ms);

            xdoc.Save(xw);

            xw.Flush();

            xw.Close();



            ms.Position = 0;

            XmlReader xr = XmlReader.Create(ms);





            //create new message from modified XML document

            newMessage = Message.CreateMessage(oldMessage.Version, null, xr);

            newMessage.Headers.CopyHeadersFrom(oldMessage);

            newMessage.Properties.CopyProperties(oldMessage.Properties);



            return newMessage;

        }
    }
    /// <summary>
    /// SOAP XML Inspector endpoint.
    /// </summary>
    public class ConsoleOutputBehavior : Attribute, IEndpointBehavior, IServiceBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(

               new ConsoleOutputMessageInspector()

               );
        }
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            //ConsoleOutputMessageInspector inspector = new ConsoleOutputMessageInspector();
            //endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
        }
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
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
                        ConsoleOutputMessageInspector inspector = new ConsoleOutputMessageInspector();
                        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {

        }
    }
    public class ConsoleOutputBehaviorExtensionElement : BehaviorExtensionElement
    {
        protected override object CreateBehavior()
        {
            return new ConsoleOutputBehavior();
        }
        public override Type BehaviorType
        {
            get
            {
                return typeof(ConsoleOutputBehavior);
            }
        }
    }
    /// <summary>
    /// SOAP XML Inspector attribute.
    /// Add the attribute to your WCF class definition, as follows:
    /// [ConsoleHeaderOutputBehavior]
    /// public class MyWCFService : IMyWCFService { ... }
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConsoleHeaderOutputBehavior : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
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
                        ConsoleOutputMessageInspector inspector = new ConsoleOutputMessageInspector();
                        endpointDispatcher.DispatchRuntime.MessageInspectors.Add(inspector);
                    }
                }
            }
        }
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}