using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using WBBBusinessLayer;
using WBBBusinessLayer.AuthenDBServices;
using WBBContract;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBWebService.Code;
using WBBWebService.CrossCuttingConcerns;
using WBBWebService.Extension;

namespace WBBWebService.CompositionRoot
{
    public static class Bootstrapper
    {
        private static Container container;

        public static object GetInstance(Type serviceType)
        {
            return container.GetInstance(serviceType);
        }

        public static T GetInstance<T>() where T : class
        {
            return container.GetInstance<T>();
        }

        public static void Bootstrap()
        {
            // Did you know the container can diagnose your configuration? Go to: http://bit.ly/YE8OJj.
            container = new Container();
            //container.EnableLifetimeScoping();

            //var lifetimeScope = new LifetimeScopeLifestyle(true);

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            BusinessLayerBootstrapper.Bootstrap(container);

            container.RegisterDecorator(typeof(ICommandHandler<>), typeof(ToWcfFaultTranslatorCommandHandlerDecorator<>));

            //container.EnablePerWcfOperationLifestyle();

            container.RegisterWcfServices(Assembly.GetExecutingAssembly());

            RegisterWcfSpecificDependencies();

            container.Verify();
        }

        private static void RegisterWcfSpecificDependencies()
        {
            container.Register<ILogger, DebugLogger>();
            container.Register<IPrincipal>(() => Thread.CurrentPrincipal);

            var logger = new DebugLogger(container);

            try
            {
                var dbConnString = "";
                if (Configurations.GetContext != null)
                {
                    dbConnString = Configurations.GetContext.ToString();
                    //logger.Info("Load ConnectionStrings in Web config:");
                }

                var dbConnStringAirNet = "";
                if (Configurations.GetAirNetContext != null)
                {
                    dbConnStringAirNet = Configurations.GetAirNetContext.ToString();
                }

                // FBBShareplex
                var dbConnStringFBBShareplexContext = "";
                if (Configurations.GetFBBShareplexContext != null)
                {
                    dbConnStringFBBShareplexContext = Configurations.GetFBBShareplexContext.ToString();
                }

                // FBBHVR
                var dbConnStringFBBHVRContext = "";
                if (Configurations.GetFBBHVRContext != null)
                {
                    dbConnStringFBBHVRContext = Configurations.GetFBBHVRContext.ToString();
                }

                //logger.Info(dbConnString);

                container.Register<IWBBDbFactory>(() => new DbFactory(dbConnString, Configurations.DbSchema), Lifestyle.Scoped);
                container.Register<IAIRDbFactory>(() => new AirNetDbFactory(dbConnStringAirNet, Configurations.DBSchemaAIRNET), Lifestyle.Scoped);
                container.Register<IFBBShareplexDbFactory>(() => new FBBShareplexDbFactory(dbConnStringFBBShareplexContext, Configurations.DbProjectCodeFBBShareplex), Lifestyle.Scoped);
                container.Register<IFBBHVRDbFactory>(() => new FBBHVRDbFactory(dbConnStringFBBHVRContext, Configurations.DbProjectCodeFBBHVR), Lifestyle.Scoped);

                //container.Register<IFBSSDbFactory>(() => new FBSSDbFactory(dbConnStringFBSS, Configurations.DbSchemaFBSS), lifetimeScope);

                container.Register<IWBBUnitOfWork, UnitOfWork>();
                container.Register<IAirNetUnitOfWork, AirNetUnitOfWork>();

                container.Register(typeof(IEntityRepository<>), typeof(EntityRepository<>));
                container.Register(typeof(IAirNetEntityRepository<>), typeof(AirNetEntityRepository<>));
                container.Register(typeof(IFBBShareplexEntityRepository<>), typeof(FBBShareplexEntityRepository<>));
                container.Register(typeof(IFBBHVREntityRepository<>), typeof(FBBHVREntityRepository<>));

                //container.RegisterOpenGeneric(typeof(IFBSSEntityRepository<>), typeof(FBSSEntityRepository<>), lifetimeScope);

                container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(LifetimeScopeQueryHandlerProxy<,>));
                container.RegisterDecorator(typeof(ICommandHandler<>), typeof(LifetimeScopeCommandHandlerProxy<>));


            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                throw ex;
            }
            // todo : deploy
        }
    }
}