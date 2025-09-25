using FBBConfig.Models;
using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.Report;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class LogInterfaceController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private string _rptName = "Report Name : {0}";
        private string _rptPackageName = "Package Name : {0}";
        private string _rptMethodName = "Method Name : {0}";
        private string _rptTable = "Table : {0}";
        private string _rptFileName = "File Name : {0}";
        private string _rptCriteria = "Report Date From : {0}  To : {1}";
        private string _rptDate = "Run report date/time : {0}";
        private string _rptOrder = "Order by : {0} {1}";

        public LogInterfaceController(ILogger logger,
            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }
        private void SetViewBagLov(string screenType)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType).ToList();
            ViewBag.configscreen = LovDataScreen;
        }


        public ActionResult Index(string dateFrom = "", string dateTo = "")
        {
            try
            {
                if (null == CurrentUser)
                    return RedirectToAction("Logout", "Account");

                ViewBag.User = CurrentUser;
                SetViewBagLov("LOG_INTERFACE_SCREEN");

                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

                var LovIntTran = base.LovData.Where(p => p.Type == "LOG_INT_TRAN_LOV").ToList();
                var transactionList = LovIntTran.Select(l => new LogInterfaceModel
                {
                    IN_TRANSACTION_ID = l.Text
                }).ToList().OrderBy(m => m.IN_TRANSACTION_ID);

                var transaction = transactionList.Select(l => new SelectListItem { Value = l.IN_TRANSACTION_ID, Text = l.IN_TRANSACTION_ID });

                var LovIntMet = base.LovData.Where(p => p.Type == "LOG_INT_MET_LOV").ToList();
                var methodNameList = LovIntMet.Select(l => new LogInterfaceModel
                {
                    METHOD_NAME = l.Text
                }).ToList().OrderBy(m => m.METHOD_NAME);


                var methodName = methodNameList.Select(l => new SelectListItem { Value = l.METHOD_NAME, Text = l.METHOD_NAME });

                var LovIntIn = base.LovData.Where(p => p.Type == "LOG_INT_IN_LOV").ToList();
                var inIdCardNoList = LovIntIn.Select(l => new LogInterfaceModel
                {
                    IN_ID_CARD_NO = l.Text
                }).ToList().OrderBy(m => m.IN_ID_CARD_NO);

                var inIdCardNo = inIdCardNoList.Select(l => new SelectListItem { Value = l.IN_ID_CARD_NO, Text = l.IN_ID_CARD_NO });

                return View(new LogInterfaceView { PACKAGE_NAME = transaction, METHOD_NAME = methodName, TABLE = inIdCardNo });
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return View(new LogInterfaceView());
            }

        }

        public List<LogInterfaceModel> GetLogInterface(GetLogInterfaceQuery model)
        {
            try
            {
                List<LogInterfaceModel> listResult = _queryProcessor.Execute(model);
                return listResult;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LogInterfaceModel>();
            }
        }

        public List<LogInterfaceModel> GetExportLogInterface(GetExportLogInterfaceQuery model)
        {
            try
            {
                List<LogInterfaceModel> listResult = _queryProcessor.Execute(model);
                return listResult;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LogInterfaceModel>();
            }
        }

        public LogInterfaceReportResponse UpdateLogInterfaceQuery(UpdateInterfaceQuery model)
        {
            try
            {
                LogInterfaceReportResponse listResult = _queryProcessor.Execute(model);
                return listResult;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new LogInterfaceReportResponse();
            }
        }

        public ActionResult UpdateLogInterface(string dataS = "")
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<UpdateInterfaceQuery>(dataS);
                LogInterfaceReportResponse result = this.UpdateLogInterfaceQuery(searchEventModel);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public ActionResult GetLogInterfaceAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<GetLogInterfaceQuery>(dataS);
                var requestsort = request.Sorts.FirstOrDefault() ?? new SortDescriptor();
                searchEventModel.SortColumn = requestsort.Member;
                searchEventModel.SortBy = !string.IsNullOrEmpty(requestsort.Member) ? (requestsort.SortDirection == ListSortDirection.Ascending ? "asc" : "desc") : string.Empty;
                searchEventModel.PageNo = request.Page;
                searchEventModel.RecordsPerPage = request.PageSize;

                var reportdatas = this.GetLogInterface(searchEventModel);

                var resultGridListShow = (from temp in reportdatas
                                          select new LogInterfaceReportGridList
                                          {
                                              INTERFACE_ID = temp.INTERFACE_ID,
                                              IN_TRANSACTION_ID = temp.IN_TRANSACTION_ID,
                                              METHOD_NAME = temp.METHOD_NAME,
                                              IN_ID_CARD_NO = temp.IN_ID_CARD_NO,
                                              SERVICE_NAME = temp.SERVICE_NAME,
                                              INPUT = temp.INPUT,
                                              OUTPUT = temp.OUTPUT,
                                              INTERFACE_NODE = temp.INTERFACE_NODE,
                                              CREATED_BY = temp.CREATED_BY,
                                              CREATED_DATE = temp.CREATED_DATE.ToDisplayText()
                                          }).ToList();

                request.Sorts = null;

                var result = resultGridListShow.ToDataSourceResult(request);
                result.Data = resultGridListShow;
                var reportTotal = reportdatas.FirstOrDefault();

                result.Total = reportTotal != null ? Convert.ToInt32(reportTotal.ALL_RECORDS) : 0;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }
        public ActionResult ExportLogInterfaceReport(string dataS, string criteria)
        {
            var listall = new List<LogInterfaceModel>();
            var searchModel = new JavaScriptSerializer().Deserialize<GetExportLogInterfaceQuery>(dataS);

            if (searchModel.CREATE_DATE_FROM != null && searchModel.CREATE_DATE_TO != null)
                _rptCriteria = string.Format(_rptCriteria, searchModel.CREATE_DATE_FROM.Value.ToDateDisplayText(), searchModel.CREATE_DATE_TO.Value.ToDateDisplayText());
            _rptName = string.Format(_rptName, "Log Interface Report");
            _rptDate = string.Format(_rptDate, DateTime.Now.ToDisplayText());
            _rptPackageName = string.Format(_rptPackageName, !string.IsNullOrEmpty(searchModel.IN_TRANSACTION_ID) ? searchModel.IN_TRANSACTION_ID : "ALL");
            _rptMethodName = string.Format(_rptMethodName, !string.IsNullOrEmpty(searchModel.METHOD_NAME) ? searchModel.METHOD_NAME : "ALL");
            _rptTable = string.Format(_rptTable, !string.IsNullOrEmpty(searchModel.IN_ID_CARD_NO) ? searchModel.IN_ID_CARD_NO : "ALL");
            _rptFileName = string.Format(_rptFileName, !string.IsNullOrEmpty(searchModel.SERVICE_NAME) ? searchModel.IN_TRANSACTION_ID : "ALL");

            if (string.IsNullOrEmpty(searchModel.SortColumnName))
            {
                var lovData = LovData.Where(p => p.Type == "LOG_INTERFACE_SCREEN" && p.LovValue5 == "PROBLEM01" && p.OrderBy != null).OrderBy(p => p.OrderBy).ToList().FirstOrDefault();
                searchModel.SortColumnName = lovData != null ? lovData.LovValue1 : string.Empty;
            }

            _rptOrder = string.Format(_rptOrder, searchModel.SortColumnName,
                !string.IsNullOrEmpty(searchModel.SortBy) ? searchModel.SortBy == "desc" ? "descending" : "ascending"
                : "descending");

            listall = this.GetExportLogInterface(searchModel);
            var resultReport = (from temp in listall
                                select new LogInterfaceReportExportList
                                {
                                    IN_TRANSACTION_ID = temp.IN_TRANSACTION_ID,
                                    METHOD_NAME = temp.METHOD_NAME,
                                    IN_ID_CARD_NO = temp.IN_ID_CARD_NO,
                                    SERVICE_NAME = temp.SERVICE_NAME,
                                    INPUT = temp.INPUT,
                                    OUTPUT = temp.OUTPUT,
                                    INTERFACE_NODE = temp.INTERFACE_NODE,
                                    CREATED_BY = temp.CREATED_BY,
                                    CREATED_DATE = temp.CREATED_DATE.ToDisplayText()
                                }).ToList();

            string filename = GetExcelName("LogInterface");

            var bytes = GenerateEntityExcel<LogInterfaceReportExportList>(resultReport, filename, "LOGINTERFACE");

            return File(bytes, "application/excel", filename + ".xlsx");
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
        public byte[] GenerateEntityExcel<T>(List<T> data, string fileName, string lovValue5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();


            var lovDataScreen = LovData.Where(p => p.Type == "LOG_INTERFACE_SCREEN" && p.LovValue5 == lovValue5 && p.OrderBy != null).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1.ToSafeString().Replace("<br/>", " ").Replace("<br />", " "), Type.GetType("System.String"));
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    System.ComponentModel.PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Type.GetType("System.String"));
                }
            }

            var values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            string tempPath = Path.GetTempPath();
            var dataExcel = GenerateExcel(table, "WorkSheet", tempPath, fileName);
            return dataExcel;
        }

        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }

            //string currentDirectorypath = Environment.CurrentDirectory;

            fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            string finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);

            const int iCol = 9;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(_rptName);
                worksheet.Cells["A3:G3"].Merge = true;
                worksheet.Cells["A3,G3"].LoadFromText(_rptPackageName);
                worksheet.Cells["A4:G4"].Merge = true;
                worksheet.Cells["A4,G4"].LoadFromText(_rptMethodName);
                worksheet.Cells["A5:G5"].Merge = true;
                worksheet.Cells["A5,G5"].LoadFromText(_rptTable);
                worksheet.Cells["A6:G6"].Merge = true;
                worksheet.Cells["A6,G6"].LoadFromText(_rptFileName);
                worksheet.Cells["A7:G7"].Merge = true;
                worksheet.Cells["A7,G7"].LoadFromText(_rptCriteria);
                worksheet.Cells["A8:G8"].Merge = true;
                worksheet.Cells["A8,G8"].LoadFromText(_rptDate);
                worksheet.Cells["A9:G9"].Merge = true;
                worksheet.Cells["A9,G9"].LoadFromText(_rptOrder);

                ExcelRange rangeReportDetail = worksheet.SelectedRange[2, 1, 9, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                const int iRow = 12;
                const int iHeaderRow = iRow + 1;
                string strRow = iRow.ToSafeString();

                ExcelRange rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                string strColumn1 = string.Format("A{0}", strRow);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);
                var columnFit = string.Format("A{0}:H{0}", strRow);
                worksheet.Cells[columnFit].AutoFitColumns(15);

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
    }
}
