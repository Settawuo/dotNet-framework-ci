using SimpleInjector;
using System;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;

namespace WBBWeb.Solid.CompositionRoot
{
    public sealed class DynamicQueryProcessor : IQueryProcessor
    {
        private readonly ILogger _logger;
        private readonly Container _container;

        public DynamicQueryProcessor(ILogger logger,
            Container container)
        {
            _logger = logger;
            _container = container;
        }

        [DebuggerStepThrough]
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            try
            {
                var queryType = query.GetType();
                var resultType = typeof(TResult);

                Debug.WriteLine($"query: {queryType.FullName}");
                Debug.WriteLine($"reqult: {resultType.FullName}");

                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
                dynamic handler = _container.GetInstance(handlerType);
                return handler.Handle((dynamic)query);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error occurred {GetType().FullName}.Execute() : {ex.ToString()}");
                throw;
            }
        }

    }
}