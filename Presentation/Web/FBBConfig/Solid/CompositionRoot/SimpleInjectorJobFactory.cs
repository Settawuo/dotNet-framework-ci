namespace FBBConfig.Solid.CompositionRoot
{
    using Quartz;
    using Quartz.Spi;
    using SimpleInjector;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class SimpleInjectorJobFactory : IJobFactory
    {
        private readonly Dictionary<Type, InstanceProducer> jobProducers;

        public SimpleInjectorJobFactory(Container container, params Assembly[] assemblies)
        {
            this.jobProducers = (
                from assembly in assemblies
                from type in assembly.GetTypes()
                where typeof(IJob).IsAssignableFrom(type)
                where !type.IsAbstract && !type.IsGenericTypeDefinition
                let ctor = container.Options.ConstructorResolutionBehavior
                    .GetConstructor(type)
                //.GetConstructor(typeof(IJob),type)
                let typeIsDecorator =
                    ctor.GetParameters().Any(p => p.ParameterType == typeof(IJob))
                where !typeIsDecorator
                let producer = Lifestyle.Transient.CreateProducer(typeof(IJob), type, container)
                select new { type, producer })
                .ToDictionary(t => t.type, t => t.producer);
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)this.jobProducers[bundle.JobDetail.JobType].GetInstance();
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable.Dispose();
        }
    }
}