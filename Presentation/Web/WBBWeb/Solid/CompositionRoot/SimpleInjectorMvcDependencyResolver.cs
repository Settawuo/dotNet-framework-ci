using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http.Dependencies;

namespace WBBWeb.CompositionRoot
{
    public sealed class SimpleInjectorMvcDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;

        [DebuggerStepThrough]
        public SimpleInjectorMvcDependencyResolver(Container container)
        {
            _container = container;
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }

        [DebuggerStepThrough]
        public object GetService(Type serviceType)
        {
            return ((IServiceProvider)_container).GetService(serviceType);
        }

        [DebuggerStepThrough]
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        public void Dispose()
        {
        }
    }
}