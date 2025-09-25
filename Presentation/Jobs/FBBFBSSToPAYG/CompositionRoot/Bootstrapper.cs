using System.Reflection;
using SimpleInjector;

namespace FBBFBSSToPAYG.CompositionRoot
{
    using WBBContract;
    using FBBFBSSToPAYG.CrossCuttingConcerns;
    using FBBFBSSToPAYG.Code;
    using WBBBusinessLayer;

    public static class Bootstrapper
    {
        private static Container container;

        public static void Bootstrap()
        {
            container = new Container();

            container.RegisterSingleton<ILogger, DebugLogger>();
            container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();

            container.Register(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));
            container.Register(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.RegisterDecorator(typeof(ICommandHandler<>),
              typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));

            container.Options.ResolveUnregisteredConcreteTypes = true;

            container.Verify();
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return container.GetInstance<TService>();
        }
    }
}