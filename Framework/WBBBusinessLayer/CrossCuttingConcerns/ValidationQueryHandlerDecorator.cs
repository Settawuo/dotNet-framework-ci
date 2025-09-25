namespace WBBBusinessLayer.CrossCuttingConcerns
{
    using SimpleInjector;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using System.Reflection;
    using WBBContract;
    using WBBEntity.Extensions;

    public class ValidationQueryHandlerDecorator<TQuery, TResult> : IQueryHandler<TQuery, TResult>
     where TQuery : IQuery<TResult>
    {
        private readonly IServiceProvider provider;
        private readonly IQueryHandler<TQuery, TResult> decorated;
        private readonly ILogger logger;
        protected Stopwatch timer;

        public ValidationQueryHandlerDecorator(Container container,
            IQueryHandler<TQuery, TResult> decorated,
            ILogger logger)
        {
            this.provider = container;
            this.decorated = decorated;
            this.logger = logger;
        }

        private object GetQueryPropValue(TQuery query, string propName)
        {
            Type t = query.GetType();

            PropertyInfo prop = t.GetProperty(propName);

            object list = prop.GetValue(query);

            return list;
        }

        private void StartWatch(TQuery query)
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public TResult Handle(TQuery query)
        {
            try
            {
                StartWatch(query);

                var validationContext = new ValidationContext(query, this.provider, null);

                Validator.ValidateObject(query, validationContext);
                this.logger.Info(typeof(TQuery).Name + " is valid.");

                var result = this.decorated.Handle(query);

                StopWatch(typeof(TQuery).Name);

                return result;
            }
            catch (ValidationException validEx)
            {
                this.logger.Info(typeof(TQuery).Name + " is not valid.");
                throw new System.ServiceModel.FaultException(validEx.Message, new System.ServiceModel.FaultCode("ValidationError"));
            }
            catch (System.Web.Services.Protocols.SoapException soapEx)
            {
                this.logger.Info(typeof(TQuery).Name + " is not valid.");
                throw new System.ServiceModel.FaultException(soapEx.Message, new System.ServiceModel.FaultCode("SoapError"));
            }
            catch (Exception ex)
            {
                this.logger.Error(typeof(TQuery).Name + string.Format(" is error on handle : {0}.",
                    ex.GetErrorMessage()));
                this.logger.Info(ex.RenderExceptionMessage());
                throw ex;
            }
        }
    }
}