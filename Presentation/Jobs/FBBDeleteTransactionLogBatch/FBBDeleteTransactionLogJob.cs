using System;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;

namespace FBBDeleteTransactionLogBatch
{
    public class FBBDeleteTransactionLogJob
    {
        private Stopwatch _timer;
        private readonly ILogger logger;
        private readonly IQueryProcessor queryProcessor;
        private FBBDeleteTransactionLogQuery delTranLog;
        public FBBDeleteTransactionLogJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            this.logger = logger;
            this.queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                logger.Info("START : FBBDeleteTransactionLogJob");
                //============================================================
                delTranLog = new FBBDeleteTransactionLogQuery();
                //============================================================
                var result = queryProcessor.Execute(delTranLog);
                logger.Info("FBBDeleteTransactionLogJob: ReturnCode = " + result.ReturnCode);
                logger.Info("FBBDeleteTransactionLogJob: ReturnMessage = " + result.ReturnMessage);
                StopWatching("STOP : FBBDeleteTransactionLogJob ");
            }
            catch (Exception ex)
            {
                logger.Info("Exceiption FBBDeleteTransactionLogJob :" + string.Format(" is error on execute : {0}.",
                    ex.Message));
                logger.Info(ex.Message);
                StopWatching("STOP : FBBDeleteTransactionLogJob ");
            }
        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }
    }
}
