using SimpleInjector;
using SimpleInjector.Integration.Wcf;
using System;
using WBBBusinessLayer;
using WBBContract;
using WBBExternalService.CompositionRoot;
using WBBExternalService.Solid.Code;
using WBBExternalService.Solid.CrossCuttingConcerns;

namespace WBBExternalService.Solid.CompositionRoot
{
    public static class Bootstrapper
    {
        private static Container _container;

        public static void Bootstrap()
        {
            _container = new Container();

            //_container.EnablePerWcfOperationLifestyle();
            _container.Options.DefaultScopedLifestyle = new WcfOperationLifestyle();
            //_container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            //_container.RegisterWcfServices(Assembly.GetExecutingAssembly());

            RegisterWcfSpecificDependencies(_container);

            _container.Verify();

            SimpleInjectorServiceHostFactory.SetContainer(_container);
        }

        private static void RegisterWcfSpecificDependencies(Container container)
        {
            container.RegisterSingleton<ILogger, DebugLogger>();
            container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();
            container.RegisterSingleton<IQueryProcessorAsync, DynamicQueryProcessorAsync>();

            container.Register(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>), Lifestyle.Scoped);
            container.Register(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>), Lifestyle.Scoped);
            container.Register(typeof(IQueryHandlerAsync<,>), typeof(WcfServiceQueryHandlerProxyAsync<,>), Lifestyle.Scoped);

            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>), Lifestyle.Scoped);
            container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(FromWcfFaultTranslatorQueryHandlerDecorator<,>), Lifestyle.Scoped);
            container.RegisterDecorator(typeof(IQueryHandlerAsync<,>), typeof(FromWcfFaultTranslatorQueryHandlerDecoratorAsync<,>), Lifestyle.Scoped);
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return _container.GetInstance<TService>();
        }

        public static object GetInstance(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }
    }
}