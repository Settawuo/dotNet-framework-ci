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
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{
    public class CheckDuplicateMobileReportController : FBBConfigController
    {
        //
        // GET: /CheckDuplicateMobileReport/
        private readonly IQueryProcessor _queryProcessor;
        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run report date/time : {0}";

        public CheckDuplicateMobileReportController(IQueryProcessor queryProcessor)
        {

            _queryProcessor = queryProcessor;
        }
        public ActionResult Index(string dateFrom = "", string dateTo = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");
            ViewBag.User = base.CurrentUser;
            SetViewBagLov();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-us");
            var rptModel = new DuplicateMobileModel();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                DateTime dttmp;

                rptModel.DateFrom = dateFrom;
                rptModel.DateTo = dateTo;
            }
            return View(rptModel);
        }

        private void SetViewBagLov()
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBB_RPTPORT006").ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        public ActionResult ReadSearchDuplicateMobile(string dataS = "")
        {
            var searchoawcModel = new JavaScriptSerializer().Deserialize<DuplicateMobileModel>(dataS);
            var result = GetDataRptDupMobileSearchModel(searchoawcModel);

            string item = "0";

            if (result.Count != 0)
                item = "1";

            return Json(new { item = item, }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReadSearchRpt01([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchrpt1Model = new JavaScriptSerializer().Deserialize<DuplicateMobileModel>(dataS);
                var result = GetDataRptDupMobileSearchModel(searchrpt1Model);

                return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public List<DuplicateMobileList> GetDataRptDupMobileSearchModel(DuplicateMobileModel searchModel)
        {
            var query = new GetCheckDuplicateMobileQuery()
            {
                dateFrom = searchModel.DateFrom,
                dateTo = searchModel.DateTo
            };
            return GetDataRptDupMobileSearchData(query);
        }

        public List<DuplicateMobileList> GetDataRptDupMobileSearchData(GetCheckDuplicateMobileQuery query)
        {
            return _queryProcessor.Execute(query);
        }

        public ActionResult ExportCusInstallTrack(string dataS)
        {
            List<DuplicateMobileList> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<DuplicateMobileModel>(dataS);
            listall = GetexportCusInstallTrack(searchModel);
            rptCriteria = string.Format(rptCriteria, searchModel.DateFrom, searchModel.DateTo);
            rptName = string.Format(rptName, "Check Duplicate Mobile Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());
            string filename = GetExcelName("DuplicateMobile");
            var bytes = GenerateEntitytoDormPAYGExcel<DuplicateMobileList>(listall, filename, "DORMPAYG01");
            return File(bytes, "application/excel", filename + ".xlsx");
        }

        public byte[] GenerateEntitytoDormPAYGExcel<T>(List<T> data, string filename, string LovValue5)
        {
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            var lovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBB_RPTPORT006").ToList();
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
            var data_ = GenerateDormPAYGExcel(table, "WorkSheet", tempPath, filename, LovValue5);
            return data_;
        }
        private byte[] GenerateDormPAYGExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xlsx"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xlsx"); }
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
            string strMergeRow = string.Empty;
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

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        private List<DuplicateMobileList> GetexportCusInstallTrack(DuplicateMobileModel searchModel)
        {
            try
            {
                var query = new GetCheckDuplicateMobileQuery
                {
                    dateFrom = searchModel.DateFrom,
                    dateTo = searchModel.DateTo

                };

                return _queryProcessor.Execute(query);

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<DuplicateMobileList>();
            }
        }

    }
}
