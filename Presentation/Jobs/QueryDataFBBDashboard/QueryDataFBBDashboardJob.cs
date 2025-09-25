using System;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands.FBBShareplex;
using WBBEntity.Extensions;

namespace QueryDataFBBDashboard
{
    public class QueryDataFBBDashboardJob
    {
        #region Properties

        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateQueryDataFBBDashboardCommand> _updateQueryDataFBBDashboard;

        #endregion

        #region Constructor

        public QueryDataFBBDashboardJob(ILogger logger, IQueryProcessor queryProcessor,
            ICommandHandler<UpdateQueryDataFBBDashboardCommand> updateQueryDataFBBDashboard)
        {
            _logger = logger;
            _updateQueryDataFBBDashboard = updateQueryDataFBBDashboard;
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("QueryDataFBBDashboardJob: Start");

                var command = new UpdateQueryDataFBBDashboardCommand { func = "" };
                string[] funcName = new string[] { "QueryFBBDashboardRegRegion", "QueryFBBDashboardRegProv", "QueryFBBDashboardRegDis", "QueryFBBDashboardRegSubdis", "QueryFBBDashboardRegTeam",
                                                   "QueryFBBDashboardCovRegion", "QueryFBBDashboardCovProv", "QueryFBBDashboardCovDis", "QueryFBBDashboardCovSubdis", "QueryFBBDashboardCovTeam",
                                                   "QueryFBBDashboardRegPack", "QueryFBBDashboardCovCust", "QueryFBBDashboardRegCust" };

                foreach (var func in funcName)
                {
                    command.func = func;
                    _updateQueryDataFBBDashboard.Handle(command);
                    _logger.Info("QueryDataFBBDashboardJob: ****** Finished ***** : " + func);
                }
                //_updateQueryDataFBBDashboard.Handle(command);

                _logger.Info("Done");
                StopWatching("QueryDataFBBDashboardJob");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                _logger.Info("QueryDataFBBDashboardJob :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("QueryDataFBBDashboardJob");
            }
        }

        #endregion

        #region Private Methods

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }

        #endregion
    }
}
