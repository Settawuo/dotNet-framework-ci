using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using WBBContract;

namespace WBBExternalService.Solid.CrossCuttingConcerns
{
    public class FromWcfFaultTranslatorCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decoratee;

        public FromWcfFaultTranslatorCommandHandlerDecorator(ICommandHandler<TCommand> decoratee)
        {
            this.decoratee = decoratee;
        }

        public void Handle(TCommand command)
        {
            try
            {
                this.decoratee.Handle(command);
            }
            catch (FaultException ex)
            {
                if (ex.Code.Name == "ValidationError")
                {
                    throw new ValidationException(ex.Message);
                }

                if (ex.Code.Name == "SoapError")
                {
                    var fault = ex.CreateMessageFault();
                    var doc = new System.Xml.XmlDocument();
                    var nav = doc.CreateNavigator();
                    if (nav != null)
                    {
                        using (var writer = nav.AppendChild())
                        {
                            fault.WriteTo(writer, EnvelopeVersion.Soap11);
                        }

                        var s = doc.InnerXml;
                        throw new Exception(s);
                    }
                }

                throw ex;
            }
        }
    }
}