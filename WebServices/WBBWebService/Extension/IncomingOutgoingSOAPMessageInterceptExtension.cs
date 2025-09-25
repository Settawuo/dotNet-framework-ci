using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web.Services.Protocols;
using WBBBusinessLayer.CommandHandlers;
using WBBContract.Commands;
using WBBData.Repository;
using WBBWebService.Code;
using WBBWebService.CompositionRoot;

namespace WBBWebService.Extension
{
    public class IncomingOutgoingSoapMessageInterceptExtension : SoapExtension
    {
        Stream _originalStream;
        Stream _workingStream;


        public override Stream ChainStream(Stream stream)
        {
            // ChainStream method is called twice in the lifecycle of the SOAP
            // message processing. BEFORE the actual web service operation is
            // invoked and AFTER it has completed. 
            //
            // In case of outgoing response, .Net framework initializes a stream
            // as an instance of type SoapExtensionStream which does NOT support
            // reading from but write operations only. Therefore, we will chain
            // a local stream instance which will be passed to for processing by
            // actual web service method and will be read from when web service
            // method finishes processing. 
            //
            // Therefore, we need to copy contents from original stream to
            // working stream instance before 
            // Once we have read outgoing SOAP message, we will write contents 
            // from working stream to the original (SoapExtensionStream) instance, 
            // for HTTP pipeline to return it to caller.


            // Store reference to incoming stream locally
            _originalStream = stream;

            // Create a new working stream to work with
            _workingStream = new MemoryStream();
            return _workingStream;
        }

        public override object GetInitializer(Type serviceType)
        {
            return null;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo,
                               SoapExtensionAttribute attribute)
        {
            return null;
        }

        public override void Initialize(object initializer)
        {
            // do nothing...
        }

        public override void ProcessMessage(SoapMessage message)
        {
            string transactionId;
            DebugLogger logger;
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeDeserialize:
                    // Incoming message
                    logger = new DebugLogger(null);

                    try
                    {
                        Copy(_originalStream, _workingStream);
                        var messagelogBefore = LogMessageFromStream(_workingStream);

                        if (((System.Web.Services.Protocols.SoapClientMessage)(message)).Client != null &&
                           (((System.Web.Services.Protocols.WebClientProtocol)(((System.Web.Services.Protocols.SoapClientMessage)(message)).Client)).Credentials != null))
                        {
                            transactionId =
                                ((System.Net.NetworkCredential)(((System.Web.Services.Protocols.WebClientProtocol)(((System.Web.Services.Protocols.SoapClientMessage)(message)).Client)).Credentials)).UserName;

                            logger.Info("_workingStream >>> transactionId = " + transactionId);
                            SaveDataFromSoapMessage(message.MethodInfo.Name, messagelogBefore, ActionType.Update, transactionId);

                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Info("_workingStream >>> transactionId = " + ex.Message);
                    }



                    break;

                case SoapMessageStage.AfterDeserialize:
                    break;

                case SoapMessageStage.BeforeSerialize:
                    break;

                case SoapMessageStage.AfterSerialize:
                    // Outgoing message
                    logger = new DebugLogger(null);

                    try
                    {
                        var messagelogAfter = ""; // LogMessageFromStream(this._workingStream);

                        Copy(this._workingStream, this._originalStream);



                        if (((System.Web.Services.Protocols.SoapClientMessage)(message)).Client != null &&
                            (((System.Web.Services.Protocols.WebClientProtocol)(((System.Web.Services.Protocols.SoapClientMessage)(message)).Client)).Credentials != null))
                        {
                            transactionId =
                                ((System.Net.NetworkCredential)(((System.Web.Services.Protocols.WebClientProtocol)(((System.Web.Services.Protocols.SoapClientMessage)(message)).Client)).Credentials)).UserName;

                            logger.Info("_workingStream >>> transactionId = " + transactionId);
                            SaveDataFromSoapMessage(message.MethodInfo.Name, messagelogAfter, ActionType.Insert,
                                transactionId);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Info("_workingStream >>> transactionId = " + ex.Message);
                    }

                    break;
            }
        }

        private static string LogMessageFromStream(Stream stream)
        {

            string soapMessage = string.Empty;

            // Just making sure again that we have got a stream which we 
            // can read from AND after reading reset its position 
            //------------------------------------------------------------
            if (stream.CanRead && stream.CanSeek)
            {
                stream.Position = 0;

                StreamReader rdr = new StreamReader(stream);
                soapMessage = rdr.ReadToEnd();


                // IMPORTANT!! - Set the position back to zero on the original 
                // stream so that HTTP pipeline can now process it
                //------------------------------------------------------------
                stream.Position = 0;
            }

            // You have raw SOAP message, log it the way you want. I am using 
            // Trace class as I have a configured a trace listener which will 
            // write it to database table
            // http://girishjjain.com/blog/post/Advanced-Tracing.aspx
            //------------------------------------------------------------

            //Trace.WriteLine(soapMessage);
            //var logger = new DebugLogger(null);
            //logger.Info("_workingStream >>> logid = " + _orderRowId + ", Raw = " + soapMessage);

            //  FilterMessageFromSoapMessage(soapMessage);

            return soapMessage;


        }

        private void Copy(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.Write(reader.ReadToEnd());
            writer.Flush();
        }

        private static void SaveDataFromSoapMessage(string methodName, string soapMessage, ActionType action, string transactionId)
        {

            var resultcomPare = string.Empty;
            try
            {
                //Verify methodName
                var result = (WBBEntity.Extensions.Constants.SbnWebService)Enum.Parse(typeof(WBBEntity.Extensions.Constants.SbnWebService), methodName.ToUpper());
                resultcomPare = methodName;
            }
            catch (ArgumentException argumentException)
            {

            }

            if (string.IsNullOrEmpty(resultcomPare)) return;

            var logger = new DebugLogger(null);
            logger.Info("SoapXml Raw = " + soapMessage);

            var outgoingOrderNew = Bootstrapper.GetInstance<SaveOutgoingMessageCommandHandler>();
            var outgoingOrderNewCommand = new SaveOutgoingMessageCommand
            {
                MethodName = resultcomPare,
                Action = action,
                SoapXml = soapMessage,
                TransactionId = transactionId
            };
            outgoingOrderNew.Handle(outgoingOrderNewCommand);

        }
    }
}
