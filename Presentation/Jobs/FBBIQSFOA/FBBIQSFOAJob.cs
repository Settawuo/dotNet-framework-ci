using System;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;

namespace FBBIQSFOA
{
    public class FBBIQSFOAJob
    {
        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;

        public FBBIQSFOAJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("FBBIQSFOAJob: Start");

                var getFbbIqsFoaQuery = new GetFbbIqsFoaQuery();
                var result = _queryProcessor.Execute(getFbbIqsFoaQuery);

                _logger.Info("FBBIQSFOAJob: ReturnCode = " + result.ReturnCode);
                _logger.Info("FBBIQSFOAJob: ReturnDesc = " + result.ReturnDesc);

                StopWatching("FBBIQSFOAJob ");
            }
            catch (Exception ex)
            {
                _logger.Info("FBBIQSFOAJob:" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("FBBIQSFOAJob ");
            }
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }

    }

}