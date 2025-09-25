using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class MarketingReportController : FBBConfigController
    {
        // GET: /MarketingReport/
        private readonly IQueryProcessor _queryProcessor;

        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run Report Date/Time : {0}";
        private List<string> rptCriterias = new List<string>();


        public MarketingReportController(ILogger logger,
              IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        public ActionResult Index()
        {
            return View();
        }


        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }


        #region Marketing Report - Tracking Competitor Report 02
        /*******************************************************************************************************************
         * Begin Tracking Competitor Report (FBBREPORT_MRK2)                                              *
         *******************************************************************************************************************/

        public ActionResult TrackingCompetitorReport(string DateFrom = "", string DateTo = "", string OrderStatus = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            SetViewBagLov("SCREEN", "FBB_RPTPORT007");
            var rptModel = new TrackingCompetitorRptModel();

            return View(rptModel);
        }

        public ActionResult ReadSearchTrackingCompetitorRpt02([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            _Logger.Info("ReadSearchTrackingCompetitorRpt02 start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    _Logger.Info("ReadSearchTrackingCompetitorRpt02 try");
                    var searchRpt02Model = new JavaScriptSerializer().Deserialize<TrackingCompetitorRptModel>(dataS);
                    var result = GetTrackingCompetitorRpt02SearchModel(searchRpt02Model);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _Logger.Info("Error when call ReadSearchTrackingCompetitorRpt02");
                    _Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public ActionResult ReadSearchTrackingCompetitorRpt(string dataS = "")
        {
            _Logger.Info("ReadSearchTrackingCompetitorRpt start");
            var searchTrackRpt02Model = new JavaScriptSerializer().Deserialize<TrackingCompetitorRptModel>(dataS);
            var result = GetTrackingCompetitorRpt02SearchModel(searchTrackRpt02Model);

            string itemDataS = "0";

            if (result != null && result.Count != 0)
            {
                itemDataS = "1";
            }

            return Json(new { itemDataS = itemDataS, }, JsonRequestBehavior.AllowGet);

        }

        public List<TrackingCompetitorRptList> GetTrackingCompetitorRpt02SearchModel(TrackingCompetitorRptModel searchRptModel)
        {
            _Logger.Info("GetTrackingCompetitorRpt02SearchModel Start");
            try
            {
                _Logger.Info("GetTrackingCompetitorRpt02SearchModel Try");
                var query = new TrackingCompetitorRptQuery()
                {
                    order_date_from = searchRptModel.DateFrom.ToSafeString(),
                    order_date_to = searchRptModel.DateTo.ToSafeString(),
                    order_status = searchRptModel.OrderStatus
                };

                return GetTrackingCompetitorRpt02QueryData(query);
            }
            catch (Exception ex)
            {
                _Logger.Info("Error when call GetTrackingCompetitorRpt02SearchModel");
                _Logger.Info(ex.GetErrorMessage());
                return new List<TrackingCompetitorRptList>();
            }

        }

        public List<TrackingCompetitorRptList> GetTrackingCompetitorRpt02QueryData(TrackingCompetitorRptQuery query)
        {
            _Logger.Info("GetTrackingCompetitorRpt02QueryData start");
            try
            {
                _Logger.Info("GetTrackingCompetitorRpt02QueryData try");
                var getResult = _queryProcessor.Execute(query);

                return getResult;
            }
            catch (Exception ex)
            {

                _Logger.Info("Error when call GetTrackingCompetitorRpt02QueryData");
                _Logger.Info(ex.GetErrorMessage());
                return null;
            }

        }

        //// Export to Excel Tracking Competitor Report////

        public ActionResult ExportTrackingCompetitorData(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<TrackingCompetitorRptModel>(dataS);

            List<TrackingCompetitorRptList> listall = GetTrackingCompetitorRpt02SearchModel(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom, searchModel.DateTo);
            rptName = string.Format(rptName, "Tracking Competitor Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetTrackingCompetitorExcelName("TrackingCompetitorReport");
            var bytes = GenerateTrackingCompetitorEntitytoExcel<TrackingCompetitorRptList>(listall, filename, "FBB_RPTPORT007");

            return File(bytes, "application/excel", filename + ".xls");

        }

        public List<TrackingCompetitorRptList> GetExportTrackingCompetitorReport(TrackingCompetitorRptModel searchModel)
        {
            _Logger.Info("GetExportTrackingCompetitorReport start");
            try
            {
                _Logger.Info("GetExportTrackingCompetitorReport try");
                var query = new TrackingCompetitorRptQuery
                {
                    order_date_from = searchModel.DateFrom.ToSafeString(),
                    order_date_to = searchModel.DateTo.ToSafeString(),
                    order_status = searchModel.OrderStatus
                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                _Logger.Info("Error when call GetExportTrackingCompetitorReport");
                return new List<TrackingCompetitorRptList>();
            }

        }

        // // Get Excel File Name// //
        private string GetTrackingCompetitorExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // // Generate Entity ///
        public byte[] GenerateTrackingCompetitorEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateTrackingCompetitorEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == LovValue5 && p.Name.Contains("G_")).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1.ToSafeString(), System.Type.GetType("System.String"));
                }

            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, System.Type.GetType("System.String"));
                }
            }

            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            string tempPath = System.IO.Path.GetTempPath();

            var data_ = GenerateTrackingCompetitorRptExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // // Generate Excel ///
        private byte[] GenerateTrackingCompetitorRptExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateTrackingCompetitorRptExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 8;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptCriteria);
                worksheet.Cells["A4:I4"].Merge = true;
                worksheet.Cells["A4,I4"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 7;
                iHeaderRow = iRow + 1;
                strRow = iRow.ToSafeString();

                switch (LovValue5)
                {

                    case "FBB_RPTPORT007":
                        range = worksheet.SelectedRange[1, 8, dataToExcel.Rows.Count + iHeaderRow, 8];
                        range.Style.Numberformat.Format = "dd/MM/yyyy";
                        break;
                    default:
                        iCol = 14;
                        break;
                }

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);

                //Step 4 : (Optional) Set the file properties like title, author and subject
                package.Workbook.Properties.Title = @"FBB Config";
                package.Workbook.Properties.Author = "FBB";
                package.Workbook.Properties.Subject = @"" + excelSheetName;

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                byte[] data = System.IO.File.ReadAllBytes(finalFileNameWithPath);
                return data;
            }

        }


        /*******************************************************************************************************************
         * End Tracking Competitor Report (FBBREPORT_MRK2)                                                * 
         *******************************************************************************************************************/
        #endregion

    }
}
