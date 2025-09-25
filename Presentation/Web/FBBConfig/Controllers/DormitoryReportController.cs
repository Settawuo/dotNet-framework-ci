using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class DormitoryReportController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run report date/time : {0}";

        public DormitoryReportController(ILogger logger,
              IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        //
        // GET: /DormitoryReport/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /DormitoryReport/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /DormitoryReport/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /DormitoryReport/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DormitoryReport/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /DormitoryReport/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /DormitoryReport/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /DormitoryReport/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        public byte[] GenerateEntitytoDormPAYGExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == "FBBDORM_PAYG_SCREEN" && p.LovValue5 == LovValue5).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1.ToSafeString(), System.Type.GetType("System.String"));
                }

                switch (LovValue5)
                {
                    //case "DORMPAYG01":
                    case "DORMPAYG08":
                    case "DORMPAYG09":
                        table.Columns[1].DataType = typeof(System.DateTime);
                        table.Columns[1].ColumnName = table.Columns[1].ColumnName.Replace("<br/>", "");
                        break;
                    case "DORMPAYG02":
                        table.Columns[7].DataType = typeof(System.DateTime);
                        table.Columns[6].ColumnName = table.Columns[6].ColumnName.Replace("<br/>", "");//.DataType = typeof(System.DateTime);
                        table.Columns[7].ColumnName = table.Columns[7].ColumnName.Replace("<br/>", "");
                        break;
                    default:
                        break;
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
            var data_ = GenerateDormPAYGExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        private byte[] GenerateDormPAYGExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

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
                    //case "DORMPAYG01":
                    case "DORMPAYG08":
                    case "DORMPAYG09":
                        range = worksheet.SelectedRange[1, 2, dataToExcel.Rows.Count + iHeaderRow, 2];
                        range.Style.Numberformat.Format = "dd/MM/yyyy";
                        break;
                    case "DORMPAYG02":
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

        #region Dorm_Report1
        /*******************************************************************************************************************
         * Begin Report Customer Installation Tracki (Dorm_Report1)                                                        *
         *******************************************************************************************************************/

        public ActionResult CustomerInstallTrack(string dateFrom = "", string dateTo = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBDORM_PAYG_SCREEN", "DORMPAYG01");

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

            var rptModel = new CusNotRegisModel();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime dttmp;

                rptModel.DateFrom = DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
                rptModel.DateTo = DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
            }

            return View(rptModel);
        }

        public ActionResult ReadSearchRpt01([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt1Model = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
                var result = GetDataRpt01SearchModel(searchrpt1Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
                //var result = GetDataSearchModel(searchoawcModel);
                //return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                return null;
            }
        }

        public List<CusInstallTrackList> GetDataRpt01SearchModel(CusNotRegisModel searchrptModel)
        {

            var query = new GetCusInstallTrackQuery()
            {

                dateFrom = searchrptModel.DateFrom,
                dateTo = searchrptModel.DateTo

            };
            return GetRpt01SearchReqCurStageQueryData(query);
        }

        public List<CusInstallTrackList> GetRpt01SearchReqCurStageQueryData(GetCusInstallTrackQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportCusInstallTrack(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            List<CusInstallTrackList> listall = GetexportCusInstallTrack(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom.Value.ToDateDisplayText(), searchModel.DateTo.Value.ToDateDisplayText());
            rptName = string.Format(rptName, "Dormitory Customer Installation Tracking Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetExcelName("CustomerInstallTrack");

            var bytes = GenerateEntitytoDormPAYGExcel<CusInstallTrackList>(listall, filename, "DORMPAYG01");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<CusInstallTrackList> GetexportCusInstallTrack(CusNotRegisModel searchModel)
        {

            try
            {
                var query = new GetCusInstallTrackQuery
                {
                    dateFrom = searchModel.DateFrom,
                    dateTo = searchModel.DateTo

                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<CusInstallTrackList>();
            }

        }

        public ActionResult ReadSearchCusInstallTrack(string dataS = "")
        {
            var searchoawcModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            var result = GetDataRpt01SearchModel(searchoawcModel);

            string item = "0";

            if (result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report Customer Installation Tracki (Dorm_Report1)                                                          *
         *******************************************************************************************************************/

        #endregion

        #region Dorm Report02
        /*******************************************************************************************************************
         * Begin Report Customer Not Register (Dorm_Report2)                                                               *
         *******************************************************************************************************************/

        public ActionResult CustomerNotRegister(string dateFrom = "", string dateTo = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            //SetViewBagLov() -- get lov
            SetViewBagLov("FBBDORM_PAYG_SCREEN", "DORMPAYG02");

            var rptModel = new CusNotRegisModel();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime dttmp;

                rptModel.DateFrom = DateTime.TryParseExact(dateFrom.ToSafeString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
                rptModel.DateTo = DateTime.TryParseExact(dateTo.ToSafeString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
            }

            return View(rptModel);
        }

        public ActionResult ReadSearchRpt02([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt2Model = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
                var result = GetDataRpt02SearchModel(searchrpt2Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                //var searchoawcModel = new JavaScriptSerializer().Deserialize<AWCModel>(dataS);
                //var result = GetDataSearchModel(searchoawcModel);
                //return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                return null;
            }
        }

        public List<CusNotRegisList> GetDataRpt02SearchModel(CusNotRegisModel searchrptModel)
        {

            var query = new GetCustNotRegisQuery()
            {

                dateFrom = searchrptModel.DateFrom,
                dateTo = searchrptModel.DateTo

            };
            return GetRpt02SearchReqCurStageQueryData(query);
        }

        public List<CusNotRegisList> GetRpt02SearchReqCurStageQueryData(GetCustNotRegisQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportCustNotRegisData(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            List<CusNotRegisList> listall = GetexportCustNotRegis(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom.Value.ToDateDisplayText(), searchModel.DateTo.Value.ToDateDisplayText());
            rptName = string.Format(rptName, "Dormitory Customer Not Register Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetExcelName("CustomerNotRegister");

            var bytes = GenerateEntitytoDormPAYGExcel<CusNotRegisList>(listall, filename, "DORMPAYG02");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<CusNotRegisList> GetexportCustNotRegis(CusNotRegisModel searchModel)
        {

            try
            {
                var query = new GetCustNotRegisQuery
                {
                    dateFrom = searchModel.DateFrom,
                    dateTo = searchModel.DateTo

                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<CusNotRegisList>();
            }

        }

        public ActionResult ReadSearchCustNotRegis(string dataS = "")
        {
            var searchoawcModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            var result = GetDataRpt02SearchModel(searchoawcModel);

            string item = "0";

            if (result.Count != 0)
                item = result[0].Item_no.ToSafeString();

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report Customer Not Register   (Dorm_Report2)                                                               *
         *******************************************************************************************************************/

        #endregion

        #region Dorm Report08
        /*******************************************************************************************************************
         * Begin Report Sum Install Performance (Dorm_Report8)                                                             *
         *******************************************************************************************************************/

        public ActionResult SumInstallPerformance(string dateFrom = "", string dateTo = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            //SetViewBagLov() -- get lov
            SetViewBagLov("FBBDORM_PAYG_SCREEN", "DORMPAYG08");

            var rptModel = new CusNotRegisModel();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime dttmp;

                rptModel.DateFrom = DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
                rptModel.DateTo = DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
            }

            return View(rptModel);
        }

        public ActionResult ReadSearchRpt08([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt8Model = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
                var result = GetDataRpt08SearchModel(searchrpt8Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public List<SumInstallPerformanceList> GetDataRpt08SearchModel(CusNotRegisModel searchrptModel)
        {

            var query = new GetSumInstallQuery()
            {

                dateFrom = searchrptModel.DateFrom,
                dateTo = searchrptModel.DateTo

            };
            return GetRpt08SearchReqCurStageQueryData(query);
        }

        public List<SumInstallPerformanceList> GetRpt08SearchReqCurStageQueryData(GetSumInstallQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportSumInstallRegisData(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            List<SumInstallPerformanceList> listall = GetexportSumInstall(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom.Value.ToDateDisplayText(), searchModel.DateTo.Value.ToDateDisplayText());
            rptName = string.Format(rptName, "Dormitory Overview Status Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetExcelName("SumInstallPerformance");

            var bytes = GenerateEntitytoDormPAYGExcel<SumInstallPerformanceList>(listall, filename, "DORMPAYG08");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<SumInstallPerformanceList> GetexportSumInstall(CusNotRegisModel searchModel)
        {

            try
            {
                var query = new GetSumInstallQuery
                {
                    dateFrom = searchModel.DateFrom,
                    dateTo = searchModel.DateTo

                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<SumInstallPerformanceList>();
            }

        }

        public ActionResult ReadSearchSumInstall(string dataS = "")
        {
            var searchoawcModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            var result = GetDataRpt08SearchModel(searchoawcModel);

            string item = "0";

            if (result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }

        /*******************************************************************************************************************
         * End Report Sum Install Performance (Dorm_Report8)                                                               *
         *******************************************************************************************************************/
        #endregion

        #region Dorm_Report09

        /*******************************************************************************************************************
         * Begin Report Overview Status (Dorm_Report9)                                                                     *
         *******************************************************************************************************************/
        public ActionResult OverviewStatusReport(string dateFrom = "", string dateTo = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            //SetViewBagLov() -- get lov
            SetViewBagLov("FBBDORM_PAYG_SCREEN", "DORMPAYG09");

            var rptModel = new OverviewStatusModel();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime dttmp;

                rptModel.OSDateFrom = DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
                rptModel.OSDateTo = DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
            }

            return View(rptModel);
        }

        public ActionResult ReadSearchRpt09([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchorpt9Model = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
                var result = GetDataRpt09SearchModel(searchorpt9Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public List<OverviewStatusList> GetDataRpt09SearchModel(CusNotRegisModel searchrptModel)
        {

            var query = new GetOverviewStatusQuery()
            {

                os_DateFrom = searchrptModel.DateFrom,
                os_DateTo = searchrptModel.DateTo

            };
            return GetRpt09SearchReqCurStageQueryData(query);
        }

        public List<OverviewStatusList> GetRpt09SearchReqCurStageQueryData(GetOverviewStatusQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportOverviewStatus(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            List<OverviewStatusList> listall = GetexportOverviewStatus(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom.Value.ToDateDisplayText(), searchModel.DateTo.Value.ToDateDisplayText());
            rptName = string.Format(rptName, "Summary Dormitory Installation Performance Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetExcelName("OverviewStatusReport");

            var bytes = GenerateEntitytoDormPAYGExcel<OverviewStatusList>(listall, filename, "DORMPAYG09");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<OverviewStatusList> GetexportOverviewStatus(CusNotRegisModel searchModel)
        {

            try
            {
                var query = new GetOverviewStatusQuery
                {
                    os_DateFrom = searchModel.DateFrom,
                    os_DateTo = searchModel.DateTo

                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<OverviewStatusList>();
            }

        }

        public ActionResult ReadSearchOverviewStatus(string dataS = "")
        {
            var searchoawcModel = new JavaScriptSerializer().Deserialize<CusNotRegisModel>(dataS);
            var result = GetDataRpt09SearchModel(searchoawcModel);

            string item = "0";

            if (result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }
        /*******************************************************************************************************************
        * End Report Overview Status (Dorm_Report9)                                                                        * 
        ********************************************************************************************************************/

        #endregion
    }
}
