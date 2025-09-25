using FBBAutoMailReport.Code;
using FBBAutoMailReport.CrossCuttingConcerns;
using SimpleInjector;
using WBBBusinessLayer;
using WBBContract;

namespace FBBAutoMailReport.CompositionRoot
{

    public static class Bootstrapper
    {
        private static Container _container;

        public static void Bootstrap()
        {
            _container = new Container();

            _container.RegisterSingleton<ILogger, DebugLogger>();
            _container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();

            _container.Register(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            _container.Register(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

            _container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));

            _container.Verify();
        }

        public static TService GetInstance<TService>() where TService : class
        {
            return _container.GetInstance<TService>();
        }
    }
}