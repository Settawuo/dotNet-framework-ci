using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using WBBContract;

namespace WBBExternalAPI.CompositionRoot
{
    public class LifetimeScopeQueryHandlerProxy<TQuery, TResult> : IQueryHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        private Container container;
        private Func<IQueryHandler<TQuery, TResult>> factory;

        public LifetimeScopeQueryHandlerProxy(Container container, Func<IQueryHandler<TQuery, TResult>> factory)
        {
            this.container = container;
            this.factory = factory;
        }

        [Obsolete]
        public TResult Handle(TQuery query)
        {
            var data = default(TResult);

            using (AsyncScopedLifestyle.BeginScope(container))
            {
                var handler = this.factory();
                data = handler.Handle(query);
            }

            return data;
        }
    }
}