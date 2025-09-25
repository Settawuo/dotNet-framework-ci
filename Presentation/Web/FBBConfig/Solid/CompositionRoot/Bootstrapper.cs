using FBBConfig.Solid.Code;
using FBBConfig.Solid.CrossCuttingConcerns;
using WBBBusinessLayer;
using WBBContract;

namespace FBBConfig.Solid.CompositionRoot
{
    using FBBConfig.Attributes;
    using Quartz;
    using Quartz.Impl;
    using Quartz.Spi;
    using SimpleInjector;
    using System;
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;
    using System.Web.Http;

    public static class Bootstrapper
    {
        private static Container _container;

        public static void Bootstrap()
        {
            _container = new Container();
            //var lifetimeScope = new LifetimeScopeLifestyle(true);
            //_container.EnableLifetimeScoping();
            _container.Options.PropertySelectionBehavior = new ImportPropertySelectionBehavior();

            RegisterMvcSpecificDependencies(_container);
            _container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            //_container.RegisterMvcAttributeFilterProvider();
            _container.RegisterMvcIntegratedFilterProvider();

            _container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorMvcDependencyResolver(_container);

            System.Web.Mvc.DependencyResolver.SetResolver(
                new SimpleInjector.Integration.Web.Mvc.SimpleInjectorDependencyResolver(_container));
        }

        private static void RegisterMvcSpecificDependencies(Container container)
        {
            container.Register<IPrincipal>(
                () => HttpContext.Current.User ?? Thread.CurrentPrincipal);
            container.RegisterSingleton<ILogger, DebugLogger>();
            container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();

            container.Register(
                typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.Register(
                typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

            //20180919
            container.RegisterSingleton<IQueryProcessorAsync, DynamicQueryProcessorAsync>();
            container.Register(typeof(IQueryHandlerAsync<,>), typeof(WcfServiceQueryHandlerProxyAsync<,>));
            container.RegisterDecorator(typeof(IQueryHandlerAsync<,>), typeof(FromWcfFaultTranslatorQueryHandlerDecoratorAsync<,>));

            container.RegisterDecorator(
                typeof(ICommandHandler<>),
                typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));

            // quartz .net ioc
            var schedulerFactory = new StdSchedulerFactory();
            //QuartzConfig(schedulerFactory);

            container.RegisterSingleton<ISchedulerFactory>(schedulerFactory);
            container.Register<IScheduler>(() => schedulerFactory.GetScheduler());
            container.RegisterSingleton<IJobFactory>(
                new SimpleInjectorJobFactory(container, Assembly.GetExecutingAssembly()));
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