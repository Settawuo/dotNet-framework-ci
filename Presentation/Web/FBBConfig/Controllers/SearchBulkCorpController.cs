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
    public class SearchBulkCorpController : FBBConfigController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;

        private string rptName = "Report Name : {0}";
        private string rptDate = "Run Report Date/Time : {0}";

        #endregion

        #region Constructor

        public SearchBulkCorpController(ILogger logger, IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region ActionResult

        // View Index
        public ActionResult SearchIndex(string BulkNumber = "", string Status = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBBULK001");
            return View();
        }

        // Read Search Bulk Corp DataDefault
        public ActionResult ReadSearchBulkCorpDataDefault([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            _Logger.Info("ReadSearchBulkCorpDataDefault start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    _Logger.Info("ReadSearchSaleTrackingDataDefault try");
                    var searchBulkCorpModel = new JavaScriptSerializer().Deserialize<SearchBulkCorpModel>(dataS);

                    var query = new GetBulkCorpSearchBulkCorpQuery()
                    {
                        P_BULK_NUMBER = searchBulkCorpModel.BulkNumber.ToString(),
                        P_CA_NUMBER = searchBulkCorpModel.CaNumber.ToString(),
                        P_TAX_ID = searchBulkCorpModel.TaxID.ToString()
                    };

                    var result = _queryProcessor.Execute(query);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _Logger.Info("Error when call ReadSearchBulkCorpDataDefault");
                    _Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        // Read Search Bulk Corp Order Number DataDefault
        public ActionResult ReadSearchBulkCorpOrderNumberDataDefault([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            _Logger.Info("ReadSearchBulkCorpOrderNumberDataDefault start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    _Logger.Info("ReadSearchBulkCorpOrderNumberDataDefault try");
                    var SearchBulkCorpOrderNumberModel = new JavaScriptSerializer().Deserialize<SearchBulkCorpOrderNumberModel>(dataS);

                    var query = new GetBulkCorpSearchBulkCorpOrderNumberQuery()
                    {
                        P_BULK_NUMBER = SearchBulkCorpOrderNumberModel.BulkNumber.ToString(),
                        P_STATUS = SearchBulkCorpOrderNumberModel.Status.ToString()
                    };

                    var result = _queryProcessor.Execute(query);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    _Logger.Info("Error when call ReadSearchBulkCorpOrderNumberDataDefault");
                    _Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region JsonResult

        // Set Lov DDL Status
        public JsonResult SetDDLStatus()
        {
            string lov_val5 = "FBBBULK001";
            string lov_type = "SCREEN_SEARCH_STATUS";

            var LovDataStatus = base.LovData.Where(p => p.LovValue5 == lov_val5 && p.Type == lov_type).OrderBy(p => p.OrderBy)
                                    .Select(p =>
                                    {
                                        return new { LOV_NAME = p.Name, LOV_VAL1 = p.LovValue1 };
                                    }).ToList();

            return Json(LovDataStatus, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Private Method

        // Set Lov ViewBag
        private void SetViewBagLov(string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        #endregion

        #region ExportExcel (Bulk Number)

        // [ExportExcel]
        public ActionResult ExportReadSearchBulkCorpData(string dataS)
        {
            var searchBulkCorpModel = new JavaScriptSerializer().Deserialize<SearchBulkCorpModel>(dataS);

            var query = new GetBulkCorpSearchBulkCorpQuery()
            {
                P_BULK_NUMBER = searchBulkCorpModel.BulkNumber.ToString(),
                P_CA_NUMBER = searchBulkCorpModel.CaNumber.ToString(),
                P_TAX_ID = searchBulkCorpModel.TaxID.ToString()
            };

            var result = _queryProcessor.Execute(query);

            var listall = ConvertSearchBulkCorpModel(result);

            rptName = string.Format(rptName, "Search Account Bulk Corporate Report (Bulk Corporate Number)");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSearchBulkCorpExcelName("SearchAccountBulkCorporateReport");

            var bytes = GenerateSearchBulkCorpEntitytoExcel<SearchBulkCorpExportList>(listall, filename, "FBBBULK001");

            return File(bytes, "application/excel", filename + ".xls");

        }

        // [ExportExcel] Convert SearchBulkCorpList To SearchBulkCorpExportList
        private List<SearchBulkCorpExportList> ConvertSearchBulkCorpModel(List<SearchBulkCorpList> list)
        {
            var dataexportlist = list.Select(x => new SearchBulkCorpExportList()
            {
                p_bulk_number = x.p_bulk_number.ToSafeString(),
                p_tax_id = x.p_tax_id.ToSafeString(),
                p_ca_no = x.p_ca_no.ToSafeString(),
                p_summary = x.p_summary.ToSafeString(),
                p_user = x.p_user.ToSafeString(),
                p_date = x.p_date.ToSafeString()
            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Get Excel File Name
        private string GetSearchBulkCorpExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateSearchBulkCorpEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateSearchBulkCorpEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5 && p.Name.StartsWith("FIELD_SEARCH_")).OrderBy(p => p.OrderBy).ToList();

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

            var data_ = GenerateSearchBulkCorpExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateSearchBulkCorpExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateSearchBulkCorpExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 12;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 5;
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

        #endregion

        #region ExportExcel (Order Number)

        // [ExportExcel]
        public ActionResult ExportReadSearchBulkCorpOrderNumberData(string dataS)
        {
            var searchBulkCorpOrderNumberModel = new JavaScriptSerializer().Deserialize<SearchBulkCorpOrderNumberModel>(dataS);

            var query = new GetBulkCorpSearchBulkCorpOrderNumberQuery()
            {
                P_BULK_NUMBER = searchBulkCorpOrderNumberModel.BulkNumber.ToString(),
                P_STATUS = searchBulkCorpOrderNumberModel.Status.ToString()
            };

            var result = _queryProcessor.Execute(query);

            var listall = ConvertSearchBulkCorpOrderNumberModel(result);

            rptName = string.Format(rptName, "Search Account Bulk Corporate Report (Order Corporate Number)");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSearchBulkCorpOrderNumberExcelName("SearchAccountBulkCorporateReport");

            var bytes = GenerateSearchBulkCorpOrderNumberEntitytoExcel<SearchBulkCorpOrderNumberExportList>(listall, filename, "FBBBULK001");

            return File(bytes, "application/excel", filename + ".xls");

        }

        // [ExportExcel] Convert SearchBulkCorpOrderNumberList To SearchBulkCorpOrderNumberExportList
        private List<SearchBulkCorpOrderNumberExportList> ConvertSearchBulkCorpOrderNumberModel(List<SearchBulkCorpOrderNumberList> list)
        {
            var dataexportlist = list.Select(x => new SearchBulkCorpOrderNumberExportList()
            {
                bulk_no = x.bulk_no.ToSafeString(),
                order_no = x.order_no.ToSafeString(),
                install_address1 = x.install_address1.ToSafeString(),
                install_address2 = x.install_address2.ToSafeString(),
                install_address3 = x.install_address3.ToSafeString(),
                install_address4 = x.install_address4.ToSafeString(),
                install_address5 = x.install_address5.ToSafeString(),
                latitude = x.latitude.ToSafeString(),
                longitude = x.longitude.ToSafeString(),
                request_install_date = x.request_install_date.ToSafeString(),
                package_code = x.package_code.ToSafeString(),
                package_bill_tha = x.package_bill_tha.ToSafeString(),
                package_code_dis = x.package_code_dis.ToSafeString(),
                package_bill_tha_dis = x.package_bill_tha_dis.ToSafeString(),
                status = x.status.ToSafeString(),
                sff_error_message = x.sff_error_message.ToSafeString(),
                workflow_error_message = x.workflow_error_message.ToSafeString(),
                fibre_net_id = x.fibre_net_id.ToSafeString(),
                ca_no = x.ca_no.ToSafeString(),
                ba_no = x.ba_no.ToSafeString(),
                sa_no = x.sa_no.ToSafeString(),
                installcapacity = x.installcapacity.ToSafeString()
            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Get Excel File Name
        private string GetSearchBulkCorpOrderNumberExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateSearchBulkCorpOrderNumberEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateSearchBulkCorpOrderNumberEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5 && p.Name.StartsWith("COLUMN_SEARCH_")).OrderBy(p => p.OrderBy).ToList();

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

            var data_ = GenerateSearchBulkCorpOrderNumberExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateSearchBulkCorpOrderNumberExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateSearchBulkCorpOrderNumberExcel start");
            if (System.IO.File.Exists(directoryPath + "\\" + fileName + ".xls"))
            { System.IO.File.Delete(directoryPath + "\\" + fileName + ".xls"); }

            //string currentDirectorypath = Environment.CurrentDirectory;
            string finalFileNameWithPath = string.Empty;

            finalFileNameWithPath = string.Format("{0}\\{1}.xls", directoryPath, fileName);

            if (System.IO.File.Exists(finalFileNameWithPath))
            { System.IO.File.Delete(finalFileNameWithPath); }

            //Delete existing file with same file name.

            var newFile = new FileInfo(finalFileNameWithPath);
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 12;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                worksheet.Cells["A2:G2"].Merge = true;
                worksheet.Cells["A2,G2"].LoadFromText(rptName);
                worksheet.Cells["A3:I3"].Merge = true;
                worksheet.Cells["A3,I3"].LoadFromText(rptDate);
                rangeReportDetail = worksheet.SelectedRange[2, 1, 4, 4];
                rangeReportDetail.Style.Fill.PatternType = ExcelFillStyle.None;
                rangeReportDetail.Style.Font.Bold = true;
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E"));

                iRow = 5;
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

        #endregion
    }
}
