using System;

namespace FBBTrackingCompetitor
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;

    public class FBBTrackingCompetitorJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;

        public FBBTrackingCompetitorJob(
            ILogger logger,
            IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            ExecuteFBBTrackingCompetitor();
        }

        public void ExecuteFBBTrackingCompetitor()
        {
            _logger.Info("FBBTrackingCompetitorJob.");
            StartWatching();
            try
            {
                QueryFBBTrackingCompetitor();

                _logger.Info("FBBTrackingCompetitor :" + string.Format(" is success."));

                StopWatching("FBBTrackingCompetitor");
            }
            catch (Exception ex)
            {
                _logger.Info("FBBTrackingCompetitor :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());

                StopWatching("FBBTrackingCompetitor");
            }
        }

        private void QueryFBBTrackingCompetitor()
        {
            try
            {
                var query = new GetFBBTrackingCompetitorQuery();

                var result = _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.RenderExceptionMessage());
            }
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string getLov)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", getLov, _timer.Elapsed));
        }

    }
}
