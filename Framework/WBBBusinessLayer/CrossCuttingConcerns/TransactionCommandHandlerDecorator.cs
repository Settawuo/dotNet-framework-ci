using System;
using System.Diagnostics;
using System.Transactions;
using WBBContract;
using WBBEntity.Extensions;

namespace WBBBusinessLayer.CrossCuttingConcerns
{
    public class TransactionCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ILogger logger;
        private readonly ICommandHandler<TCommand> decorated;
        protected Stopwatch timer;

        public TransactionCommandHandlerDecorator(ILogger logger,
            ICommandHandler<TCommand> decorated)
        {
            this.logger = logger;
            this.decorated = decorated;
        }

        private void StartWatch(TCommand command)
        {
            timer = Stopwatch.StartNew();
        }

        private void StopWatch(string actionName)
        {
            timer.Stop();
            logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
        }

        public void Handle(TCommand command)
        {
            var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(30));
            try
            {
                StartWatch(command);

                using (scope)
                {
                    this.decorated.Handle(command);
                    scope.Complete();
                }

                StopWatch(typeof(TCommand).Name);
            }
            catch (System.Exception ex)
            {
                StopWatch(typeof(TCommand).Name);
                scope.Dispose();
                this.logger.Info("Handle " + typeof(TCommand).Name + " Error : " + ex.GetErrorMessage());
                this.logger.Info(ex.RenderExceptionMessage());
                throw ex;
            }
        }
    }
}