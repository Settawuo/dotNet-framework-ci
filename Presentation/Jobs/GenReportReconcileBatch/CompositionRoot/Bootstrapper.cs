using SimpleInjector;

namespace GenReportReconcileBatch.CompositionRoot
{
    using GenReportReconcileBatch.Code;
    using GenReportReconcileBatch.CrossCuttingConcerns;
    using WBBBusinessLayer;
    using WBBContract;

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