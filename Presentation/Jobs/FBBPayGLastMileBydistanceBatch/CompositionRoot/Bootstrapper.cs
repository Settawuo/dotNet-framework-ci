using System.Reflection;
using SimpleInjector;

namespace FBBPayGLastMileBydistanceBatch.CompositionRoot
{
    using WBBContract;
    using FBBPayGLastMileBydistanceBatch.CrossCuttingConcerns;
    using FBBPayGLastMileBydistanceBatch.Code;
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

            container.Verify();
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return container.GetInstance<TService>();
        }
    }
}