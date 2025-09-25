using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using WBBContract;

namespace WBBWebService.CompositionRoot
{
    public class LifetimeScopeCommandHandlerProxy<T> : ICommandHandler<T>
    {
        // Since this type is part of the composition root,
        // we are allowed to inject the container into it.
        private Container container;

        private Func<ICommandHandler<T>> factory;

        public LifetimeScopeCommandHandlerProxy(Container container, Func<ICommandHandler<T>> factory)
        {
            this.factory = factory;
            this.container = container;
        }

        [Obsolete]
        public void Handle(T command)
        {
            using (AsyncScopedLifestyle.BeginScope(container))
            {
                var handler = this.factory();

                handler.Handle(command);
            }
        }
    }
}