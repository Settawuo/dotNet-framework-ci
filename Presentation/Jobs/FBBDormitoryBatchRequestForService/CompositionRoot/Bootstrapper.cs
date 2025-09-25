using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace FBBDormitoryBatchRequestForService.CompositionRoot
{
    using One2NetContract;
    using FBBDormitoryBatchRequestForService.CrossCuttingConcerns;
    using FBBDormitoryBatchRequestForService.Code;
    using One2NetBusinessLayer;

    public static class Bootstrapper
    {
        private static Container container;

        public static void Bootstrap()
        {
            container = new Container();

            container.RegisterSingle<ILogger, DebugLogger>();
            container.RegisterSingle<IQueryProcessor, DynamicQueryProcessor>();

            container.RegisterOpenGeneric(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.RegisterOpenGeneric(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

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