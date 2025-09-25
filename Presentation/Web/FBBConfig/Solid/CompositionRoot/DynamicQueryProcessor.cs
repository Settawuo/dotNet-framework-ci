using SimpleInjector;
using System.Diagnostics;
using System.Threading.Tasks;
using WBBContract;

namespace FBBConfig.Solid.CompositionRoot
{
    public sealed class DynamicQueryProcessor : IQueryProcessor
    {
        private readonly Container container;

        public DynamicQueryProcessor(Container container)
        {
            this.container = container;
        }

        [DebuggerStepThrough]
        public TResult Execute<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = this.container.GetInstance(handlerType);

            return handler.Handle((dynamic)query);
        }
    }

    public sealed class DynamicQueryProcessorAsync : IQueryProcessorAsync
    {
        private readonly Container container;

        public DynamicQueryProcessorAsync(Container container)
        {
            this.container = container;
        }

        [DebuggerStepThrough]
        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandlerAsync<,>).MakeGenericType(query.GetType(), typeof(TResult));

            dynamic handler = this.container.GetInstance(handlerType);

            await handler.HandleAsync((dynamic)query);

            return default(TResult);
        }
    }
}