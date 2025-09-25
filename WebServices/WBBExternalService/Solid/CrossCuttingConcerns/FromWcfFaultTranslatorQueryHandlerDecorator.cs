using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using System.Threading.Tasks;
using WBBContract;

namespace WBBExternalService.Solid.CrossCuttingConcerns
{
    public class FromWcfFaultTranslatorQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandler<TQuery, TResult> decoratee;

        public FromWcfFaultTranslatorQueryHandlerDecorator(IQueryHandler<TQuery, TResult> decoratee)
        {
            this.decoratee = decoratee;
        }

        public TResult Handle(TQuery query)
        {
            try
            {
                return this.decoratee.Handle(query);
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

    public class FromWcfFaultTranslatorQueryHandlerDecoratorAsync<TQuery, TResult> : IQueryHandlerAsync<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        private readonly IQueryHandlerAsync<TQuery, TResult> decoratee;

        public FromWcfFaultTranslatorQueryHandlerDecoratorAsync(IQueryHandlerAsync<TQuery, TResult> decoratee)
        {
            this.decoratee = decoratee;
        }

        public async Task<TResult> HandleAsync(TQuery query)
        {
            try
            {
                await this.decoratee.HandleAsync(query);
                return default(TResult);
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