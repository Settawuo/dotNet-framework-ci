using System;

namespace FBBMeshReport
{
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;

    public class FBBMeshReportJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;

        public FBBMeshReportJob(
            ILogger logger,
            IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            ExecuteFBBMeshReport();
        }

        public void ExecuteFBBMeshReport()
        {
            StartWatching();
            _logger.Info("FBBMeshReportJob: Start");
            Console.WriteLine("Start");

            try
            {
                QueryFBBMeshReport();

                _logger.Info("FBBMeshReport :" + string.Format(" is success."));
                Console.WriteLine("Done");

                StopWatching("FBBMeshReport");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                _logger.Info("FBBMeshReport :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());

                StopWatching("FBBMeshReport");
            }
        }

        private void QueryFBBMeshReport()
        {
            try
            {
                DateTime dateTimeNow = DateTime.Now;
                System.IFormatProvider format = new System.Globalization.CultureInfo("en-US");
                string dateTo = dateTimeNow.ToString("ddMMyyyy", format);

                var query = new GetFBBMeshReportQuery
                {
                    date_to = dateTo
                };

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
