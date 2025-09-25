using System;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;

namespace FBBResetSequenceBatch
{
    public class FBBResetSequenceJob
    {
        private Stopwatch _timer;
        private readonly ILogger logger;
        private readonly IQueryProcessor queryProcessor;
        private FBBResetSequenceQuery resSeq;

        public FBBResetSequenceJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            this.logger = logger;
            this.queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                logger.Info("START : FBBResetSequenceJob");
                //========================================================
                resSeq = new FBBResetSequenceQuery();
                //========================================================
                var result = queryProcessor.Execute(resSeq);
                logger.Info("FBBResetSequenceJob: ReturnCode = " + result.ReturnCode);
                logger.Info("FBBResetSequenceJob: ReturnMessage = " + result.ReturnMessage);
                StopWatching("STOP : FBBResetSequenceJob ");
            }
            catch (Exception ex)
            {
                logger.Info("Exceiption FBBResetSequenceJob :" + string.Format(" is error on execute : {0}.",
                     ex.Message));
                logger.Info(ex.Message);
                StopWatching("STOP : FBBResetSequenceJob ");
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
