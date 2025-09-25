using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;

namespace FBBQueryOrderPendingDeduction
{
    public class FBBQueryOrderPendingDeductionJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private Stopwatch _timer;

        public FBBQueryOrderPendingDeductionJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> sendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = sendSmsCommand;
        }

        public void QueryOrderPendingDeduction()
        {
            _logger.Info("Start QueryOrderPendingDeduction Method");

            string fullUrl = "BATCH";

            var query = new QueryOrderPendingDeductionQuery
            {
                FullUrl = fullUrl
            };

            var result = _queryProcessor.Execute(query);

            _logger.Info("End QueryOrderPendingDeduction Method Result : " + result.ToString());
        }

        public void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        public void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public void LogMsg(string Msg)
        {
            _logger.Info(Msg);
        }

    }
}
