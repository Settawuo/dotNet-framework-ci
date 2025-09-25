using SaleTrackingReport.CompositionRoot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using WBBBusinessLayer;
using WBBContract;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace SaleTrackingReport
{
    public class SaleTrackingReportJob
    {
        private Stopwatch _timer;
        private readonly ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;

        public SaleTrackingReportJob(ILogger logger, IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
        }

        public void Execute()
        {
            try
            {
                StartWatching();
                _logger.Info("SaleTrackingReportJob: Start");

                Console.WriteLine("Start");

                string dtFrom = "";
                string dtTo = "";
                string lcCode = "";

                #region Input Session
                //while (true)
                //{
                //    Console.Write("***************** Date From (dd/MM/yyyy) ***************** : ");
                //    dtFrom = Console.ReadLine();
                //    if (!string.IsNullOrEmpty(dtFrom))
                //    {
                //        string[] dtFromSplit = dtFrom.Split('/');
                //        if(dtFromSplit.Length == 3){
                //            if (int.Parse(dtFromSplit[1]) > 0 && int.Parse(dtFromSplit[1]) <= 12 && dtFromSplit[1].Length == 2)
                //            {
                //                if (int.Parse(dtFromSplit[0]) > 0 && int.Parse(dtFromSplit[0]) <= 31 && dtFromSplit[0].Length == 2)
                //                {
                //                    if (int.Parse(dtFromSplit[2]) > 1 && dtFromSplit[2].Length == 4)
                //                    {
                //                        break;
                //                    }
                //                    else
                //                    {
                //                        Console.BackgroundColor = ConsoleColor.Red;
                //                        Console.ForegroundColor = ConsoleColor.White;
                //                        Console.WriteLine("Wrong Year format! (yyyy)");
                //                        Console.ResetColor();
                //                    } 

                //                }
                //                else
                //                {
                //                    Console.BackgroundColor = ConsoleColor.Red;
                //                    Console.ForegroundColor = ConsoleColor.White;
                //                    Console.WriteLine("Wrong Day format! (dd)");
                //                    Console.ResetColor();
                //                } 

                //            }
                //            else
                //            {
                //                Console.BackgroundColor = ConsoleColor.Red;
                //                Console.ForegroundColor = ConsoleColor.White;
                //                Console.WriteLine("Wrong Month format! (MM)");
                //                Console.ResetColor();
                //            } 

                //        }
                //        else
                //        {
                //            Console.BackgroundColor = ConsoleColor.Red;
                //            Console.ForegroundColor = ConsoleColor.White;
                //            Console.WriteLine("Wrong Date format! (dd/MM/yyyy)");
                //            Console.ResetColor();
                //        }
                //    }
                //    else
                //    {
                //        Console.BackgroundColor = ConsoleColor.Red;
                //        Console.ForegroundColor = ConsoleColor.White;
                //        Console.WriteLine("Please enter any Date From!");
                //        Console.ResetColor();
                //    }
                //}
                //while (true)
                //{
                //    Console.Write("***************** Date To (dd/MM/yyyy) ***************** : ");
                //    dtTo = Console.ReadLine();
                //    if (!string.IsNullOrEmpty(dtTo))
                //    {
                //        string[] dtToSplit = dtTo.Split('/');
                //        if (dtToSplit.Length == 3)
                //        {
                //            if (int.Parse(dtToSplit[1]) > 0 && int.Parse(dtToSplit[1]) <= 12 && dtToSplit[1].Length == 2)
                //            {
                //                if (int.Parse(dtToSplit[0]) > 0 && int.Parse(dtToSplit[0]) <= 31 && dtToSplit[0].Length == 2)
                //                {
                //                    if (int.Parse(dtToSplit[2]) > 1 && dtToSplit[2].Length == 4)
                //                    {
                //                        break;
                //                    }
                //                    else
                //                    {
                //                        Console.BackgroundColor = ConsoleColor.Red;
                //                        Console.ForegroundColor = ConsoleColor.White;
                //                        Console.WriteLine("Wrong Year format! (yyyy)");
                //                        Console.ResetColor();
                //                    }

                //                }
                //                else
                //                {
                //                    Console.BackgroundColor = ConsoleColor.Red;
                //                    Console.ForegroundColor = ConsoleColor.White;
                //                    Console.WriteLine("Wrong Day format! (dd)");
                //                    Console.ResetColor();
                //                }

                //            }
                //            else
                //            {
                //                Console.BackgroundColor = ConsoleColor.Red;
                //                Console.ForegroundColor = ConsoleColor.White;
                //                Console.WriteLine("Wrong Month format! (MM)");
                //                Console.ResetColor();
                //            }

                //        }
                //        else
                //        {
                //            Console.BackgroundColor = ConsoleColor.Red;
                //            Console.ForegroundColor = ConsoleColor.White;
                //            Console.WriteLine("Wrong Date format! (dd/MM/yyyy)");
                //            Console.ResetColor();
                //        }
                //    }
                //    else
                //    {
                //        Console.BackgroundColor = ConsoleColor.Red;
                //        Console.ForegroundColor = ConsoleColor.White;
                //        Console.WriteLine("Please enter any Date To!)");
                //        Console.ResetColor();
                //    }
                //}
                //Console.Write("***************** Location Code ***************** : ");
                //lcCode = Console.ReadLine();

                #endregion

                var SaleTrackingExport = Bootstrapper.GetInstance<SaleTrackingExport>();

                List<LovValueModel> locationList = SaleTrackingExport.GetLovList("BATCH_SALE_TRACKING");

                Console.WriteLine("locationList: " + locationList.Count);

                if (string.IsNullOrEmpty(lcCode))
                {
                    foreach (LovValueModel location in locationList)
                    {
                        Console.WriteLine("Location: " + location.LovValue1.ToString());
                        SaleTrackingExport.ExportSaleTrackingData("{ 'LocCode': '" + location.LovValue1.ToString() + "', 'DateFrom': ' " + DateTime.Today.AddDays(-1).ToString("dd'/'MM'/'yyyy") + "', 'DateTo': '" + DateTime.Today.ToString("dd'/'MM'/'yyyy") + "' }", location.Name.ToString());
                        Console.WriteLine("Location: " + location.LovValue1.ToString() + " Finished");
                    }
                }
                else
                    SaleTrackingExport.ExportSaleTrackingData("{ 'LocCode': '" + lcCode + "', 'DateFrom': '" + dtFrom + "', 'DateTo': '" + dtTo + "' }", "");

                Console.WriteLine("Done");
                //Console.ReadLine();
                StopWatching("SaleTrackingReportJob ");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.ReadLine();
                _logger.Info("SaleTrackingReportJob :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("SaleTrackingReportJob ");
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
