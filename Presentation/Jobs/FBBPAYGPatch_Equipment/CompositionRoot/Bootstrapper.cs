using FBBPAYGPatch_Equipment.Code;
using FBBPAYGPatch_Equipment.CrossCuttingConcerns;
using SimpleInjector;
using System;
using WBBBusinessLayer;
using WBBContract;

namespace FBBPAYGPatch_Equipment.CompositionRoot
{
    public class Bootstrapper : IDisposable
    {
        private bool disposed = false;
        private static Bootstrapper Instance { get; set; }
        private static Container container { get; set; }
        static Bootstrapper()
        {
            Instance = new Bootstrapper();

            container = new Container();

            container.RegisterSingleton<ILogger, DebugLogger>();
            container.RegisterSingleton<IQueryProcessor, DynamicQueryProcessor>();

            container.Register(typeof(ICommandHandler<>), typeof(WcfServiceCommandHandlerProxy<>));
            container.Register(typeof(IQueryHandler<,>), typeof(WcfServiceQueryHandlerProxy<,>));

            container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(FromWcfFaultTranslatorCommandHandlerDecorator<>));

            container.Verify();
        }

        public TService GetInstance<TService>() where TService : class
        {
            return container.GetInstance<TService>();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    container.Dispose();
                    // called via myClass.Dispose(). 
                    // OK to use any private object references
                }
                // Release unmanaged resources.
                // Set large fields to null.                
                disposed = true;
            }
        }

        public void Dispose() // Implement IDisposable
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Bootstrapper() // the finalizer
        {
            Dispose(false);
        }
    }

}
