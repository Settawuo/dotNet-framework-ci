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
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class PaymentController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;
        private string _rptName = "Report Name : {0}";
        private string _rptInternetNo = "Internet No : {0}";
        private string _rptCriteria = "Payment Date From : {0}  To : {1}";
        private string _rptDate = "Run report date/time : {0}";
        private string _rptOrder = "Order by : {0} {1}";

        public PaymentController(ILogger logger, IQueryProcessor queryProcessor)
        {
            _queryProcessor = queryProcessor;
            _Logger = logger;
        }

        private void SetViewBagLov(string screenType, string lovValue5)
        {
            var lovDataScreen = LovData.Where(p => p.Type == screenType && p.LovValue5 == lovValue5).ToList();
            ViewBag.configscreen = lovDataScreen;
        }

        public ActionResult Report(string dateFrom = "", string dateTo = "", string sortBy = "", string sortColumn = "")
        {
            if (null == CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = CurrentUser;
            SetViewBagLov("FBBREPORT_PAYMENT_SCREEN", "PAYMENT01");

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");

            var rptModel = new GetReportPaymentQuery();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime dttmp;

                rptModel.DateFrom = DateTime.TryParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
                rptModel.DateTo = DateTime.TryParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dttmp) ? dttmp.Date : (DateTime?)null;
            }

            return View(rptModel);
        }

        //public ActionResult ReportSearch(string dataS = "")
        //{
        //    var searchModel = new JavaScriptSerializer().Deserialize<GetReportPaymentQuery>(dataS);
        //    var result = GetDataReportPayment(searchModel);

        //    string item = "0";

        //    if (result.Count != 0)
        //        item = "1";

        //    return Json(new {item}, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult ReportRead([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (!string.IsNullOrEmpty(dataS))
            {
                var searchrptModel = new JavaScriptSerializer().Deserialize<GetReportPaymentQuery>(dataS);
                var requestsort = request.Sorts.FirstOrDefault() ?? new SortDescriptor();
                searchrptModel.SortColumn = requestsort.Member;
                searchrptModel.SortBy = !string.IsNullOrEmpty(requestsort.Member) ? (requestsort.SortDirection == ListSortDirection.Ascending ? "asc" : "desc") : string.Empty;
                searchrptModel.PageNo = request.Page;
                searchrptModel.RecordsPerPage = request.PageSize;
                var reportdatas = GetDataReportPayment(searchrptModel);
                request.Sorts = null;

                var result = reportdatas.ToDataSourceResult(request);
                result.Data = reportdatas;
                var reportTotal = reportdatas.FirstOrDefault();

                result.Total = reportTotal != null ? Convert.ToInt32(reportTotal.ALL_RECORDS) : 0;

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public List<ReportPaymentModel> GetDataReportPayment(GetReportPaymentQuery searchrptModel)
        {
            List<ReportPaymentModel> listResult = _queryProcessor.Execute(searchrptModel);
            return listResult;
        }

        public ActionResult Export(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<GetExportPaymentQuery>(dataS);
            List<ExportPaymentModel> listall = _queryProcessor.Execute(searchModel);

            if (searchModel.DateFrom != null && searchModel.DateTo != null)
                _rptCriteria = string.Format(_rptCriteria, searchModel.DateFrom.Value.ToDateDisplayText(), searchModel.DateTo.Value.ToDateDisplayText());
            _rptInternetNo = string.Format(_rptInternetNo, searchModel.InternetNo);
            _rptName = string.Format(_rptName, "Payment Report");
            _rptDate = string.Format(_rptDate, DateTime.Now.ToDisplayText());

            if (string.IsNullOrEmpty(searchModel.SortColumnName))
            {
                var lovData = LovData.Where(p => p.Type == "FBBREPORT_PAYMENT_SCREEN" && p.LovValue5 == "PAYMENT01" && p.OrderBy != null).OrderBy(p => p.OrderBy).ToList().FirstOrDefault();
                searchModel.SortColumnName = lovData != null ? lovData.LovValue2 : string.Empty;
            }
            _rptOrder = string.Format(_rptOrder, searchModel.SortColumnName,
                !string.IsNullOrEmpty(searchModel.SortBy) ? searchModel.SortBy == "desc" ? "descending" : "ascending"
                : "descending");

            string filename = GetExcelName("Report");

            var bytes = GenerateEntityExcel(listall, filename, "PAYMENT01");

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        private string GetExcelName(string fileName)
        {
            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            string result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        public byte[] GenerateEntityExcel<T>(List<T> data, string fileName, string lovValue5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();


            var lovDataScreen = LovData.Where(p => p.Type == "FBBREPORT_PAYMENT_SCREEN" && p.LovValue5 == lovValue5 && p.OrderBy != null).OrderBy(p => p.OrderBy).ToList();

            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue2.ToSafeString().Replace("<br/>", " ").Replace("<br />", " "), Type.GetType("System.String"));
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

            const int iCol = 8;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(_rptName);
                worksheet.Cells["A3:G3"].Merge = true;
                worksheet.Cells["A3,G3"].LoadFromText(_rptInternetNo);
                worksheet.Cells["A4:G4"].Merge = true;
                worksheet.Cells["A4,G4"].LoadFromText(_rptCriteria);
                worksheet.Cells["A5:G5"].Merge = true;
                worksheet.Cells["A5,G5"].LoadFromText(_rptOrder);
                worksheet.Cells["A6:G6"].Merge = true;
                worksheet.Cells["A6,G6"].LoadFromText(_rptDate);

                ExcelRange rangeReportDetail = worksheet.SelectedRange[2, 1, 6, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                const int iRow = 9;
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
                var columnFit = string.Format("C{0}:G{0}", strRow);
                worksheet.Cells["A9"].AutoFitColumns(20);
                worksheet.Cells["B9"].AutoFitColumns(25);
                worksheet.Cells[columnFit].AutoFitColumns(15);
                worksheet.Cells["H9"].AutoFitColumns(30);

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
