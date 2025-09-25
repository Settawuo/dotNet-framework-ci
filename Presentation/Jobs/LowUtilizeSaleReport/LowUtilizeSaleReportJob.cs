using LowUtilizeSaleReport.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace LowUtilizeSaleReport
{
    public class LowUtilizeSaleReportJob
    {
        #region Properties

        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;

        #endregion

        #region Constructor

        public LowUtilizeSaleReportJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region Public Methods

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("LowUtilizeSaleReportJob: Start");

                Console.WriteLine("Start");

                string dtFrom = "";
                string dtTo = "";
                string lcCode = "";

                var LowUtilizeSaleExport = Bootstrapper.GetInstance<LowUtilizeSaleReportExport>();
                List<LovValueModel> locationList = LowUtilizeSaleExport.GetLovList("BATCH_LOW_UTILIZE_SALE_RPT");

                Console.WriteLine("Location List : " + locationList.Count);

                if (locationList.Count > 0)
                {
                    foreach (LovValueModel location in locationList)
                    {
                        Console.WriteLine("Location: " + location.LovValue1.ToString());
                        LowUtilizeSaleExport.ExportLowUtilizeSaleData("{ 'LocationCode': '" + location.LovValue1.ToString() + "', 'DateFrom': ' " + DateTime.Today.AddDays(-1).ToString("dd'/'MM'/'yyyy") + "', 'DateTo': '" + DateTime.Today.ToString("dd'/'MM'/'yyyy") + "' }", location.Name.ToString());
                        Console.WriteLine("Location: " + location.LovValue1.ToString() + " Finished");
                    }
                }
                else
                    LowUtilizeSaleExport.ExportLowUtilizeSaleData("{ 'LocationCode': '" + lcCode + "', 'DateFrom': '" + dtFrom + "', 'DateTo': '" + dtTo + "' }", "BATCH_LOW_UTILIZE_SALE_RPT");

                Console.WriteLine("Done");
                StopWatching("LowUtilizeSaleReportJob");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                _logger.Info("LowUtilizeSaleReportJob :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("LowUtilizeSaleReportJob ");
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
