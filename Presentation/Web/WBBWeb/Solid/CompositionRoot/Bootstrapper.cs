using AIRNETEntity.StoredProc;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Http;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBWeb.CompositionRoot;
using WBBWeb.Solid.Code;
using WBBWeb.Solid.CrossCuttingConcerns;
//using SimpleInjector.Extensions;

namespace WBBWeb.Solid.CompositionRoot
{
    public static class Bootstrapper
    {
        private static Container _container;

        public static void Bootstrap()
        {
            _container = new Container();
            //var lifetimeScope = new LifetimeScopeLifestyle(true);
            //_container.EnableLifetimeScoping();
            //_container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            RegisterMvcSpecificDependencies(_container);
            _container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            //_container.RegisterMvcAttributeFilterProvider();
            _container.RegisterMvcIntegratedFilterProvider();

            _container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorMvcDependencyResolver(_container);

            System.Web.Mvc.DependencyResolver.SetResolver(
                new SimpleInjector.Integration.Web.Mvc.SimpleInjectorDependencyResolver(_container));

            var test = typeof(IQueryHandler<GetTrackingQuery, List<TrackingModel>>);
            var result = GetInstance(test);
            Debug.WriteLine($"IQueryHandler Generic: {test.FullName}");
            Debug.WriteLine($"IQueryHandler Instant: {result.GetType().FullName}");
            //GetTrackingQuery : IQuery<List<TrackingModel>>
        }

        private static void RegisterMvcSpecificDependencies(Container container)
        {
            container.Register<IPrincipal>(() => HttpContext.Current.User ?? Thread.CurrentPrincipal);
            container.Register<ILogger, DebugLogger>();
            container.Register<IQueryProcessor, DynamicQueryProcessor>();


            container.Register(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.Register(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

            container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));
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