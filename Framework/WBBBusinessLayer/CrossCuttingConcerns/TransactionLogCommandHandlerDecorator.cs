//using System.Diagnostics;
//using System.Transactions;
//using System.Linq;
//using WBBContract;
//using WBBEntity.Extensions;
//using ObjectDumper;

//namespace WBBBusinessLayer.CrossCuttingConcerns
//{
//    public class TransactionLogCommandHandlerDecorator<TCommand> : ILogCommandHandler<TCommand>
//    {
//        private readonly ILogger logger;
//        private readonly ILogCommandHandler<TCommand> decorated;
//        protected Stopwatch timer;

//        public TransactionLogCommandHandlerDecorator(ILogger logger,
//            ILogCommandHandler<TCommand> decorated)
//        {
//            this.logger = logger;
//            this.decorated = decorated;
//        }

//        private void StartWatch(TCommand command)
//        {
//            timer = Stopwatch.StartNew();
//            string messageInfo = command.DumpToString(command.GetType().Name);
//            logger.Info(messageInfo);
//        }

//        private void StopWatch(string actionName)
//        {
//            timer.Stop();
//            logger.Info(string.Format("Handle '" + actionName + "' take total elapsed time: {0} seconds.", timer.Elapsed.TotalSeconds));
//        }

//        public void Handle(TCommand command)
//        {
//            var scope = new TransactionScope();
//            try
//            {
//                StartWatch(command);

//                using (scope)
//                {
//                    this.decorated.Handle(command);
//                    scope.Complete();
//                }

//                StopWatch(typeof(TCommand).Name);
//            }
//            catch (System.Exception ex)
//            {
//                scope.Dispose();
//                this.logger.Info("Handle " + typeof(TCommand).Name + " Error : " + (ex.InnerException == null ? ex.Message : ex.InnerException.Message));
//                this.logger.Info(ex.StackTrace);
//                throw ex;
//            }
//        }
//    }
//}