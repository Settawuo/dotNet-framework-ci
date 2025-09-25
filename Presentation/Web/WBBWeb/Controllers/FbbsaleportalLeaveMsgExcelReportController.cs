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
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace WBBWeb.Controllers
{
    public class FbbsaleportalLeaveMsgExcelReportController : WBBController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;

        private string rptName = "Report Name : {0}";
        private string rptDate = "Run Report Date/Time : {0}";

        #endregion

        // GET: /Leavemessage Excel Report/

        #region Constructor

        public FbbsaleportalLeaveMsgExcelReportController(IQueryProcessor queryProcessor
              , ILogger logger)
        {
            _queryProcessor = queryProcessor;
            base.Logger = logger;
        }

        #endregion

        #region ActionResult

        public ActionResult SearchIndex()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Login", "Fbbsaleportal");

            ViewBag.User = CurrentUser;
            SetViewBagLov("FBBOR021");
            return View();
        }

        // Read Search Leave Msg File Name DataDefault
        public ActionResult ReadSearchLeaveMsgFileNameDataDefault([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            Logger.Info("ReadSearchLeaveMsgFileNameDataDefault start");

            string user_name = base.CurrentUser.UserName.ToSafeString();

            if (dataS != null && dataS != "")
            {
                try
                {
                    Logger.Info("ReadSearchSaleTrackingDataDefault try");
                    var searchLeaveMsgFileNameModel = new JavaScriptSerializer().Deserialize<SearchLeaveMsgFileNameModel>(dataS);

                    var query = new GetLeaveMessageSearchFileNameQuery()
                    {
                        P_USERNAME = user_name.ToSafeString(),
                        P_FILE_NAME = searchLeaveMsgFileNameModel.FileName.ToSafeString(),
                        P_START_DATE = searchLeaveMsgFileNameModel.StartDate.ToSafeString(),
                        P_END_DATE = searchLeaveMsgFileNameModel.EndDate.ToSafeString()
                    };

                    var result = _queryProcessor.Execute(query);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Info("Error when call ReadSearchLeaveMsgFileNameDataDefault");
                    Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        // Read Search Leave Msg File Name Detail DataDefault
        public ActionResult ReadSearchLeaveMsgFileNameDetailDataDefault([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            Logger.Info("ReadSearchLeaveMsgFileNameDetailDataDefault start");

            string user_name = base.CurrentUser.UserName.ToSafeString();

            if (dataS != null && dataS != "")
            {
                try
                {
                    Logger.Info("ReadSearchBulkCorpOrderNumberDataDefault try");
                    var searchLeaveMsgFileNameDetailModel = new JavaScriptSerializer().Deserialize<SearchLeaveMsgFileNameDetailModel>(dataS);

                    var query = new GetLeaveMessageSearchFileNameDetailQuery()
                    {
                        P_FILE_NAME = searchLeaveMsgFileNameDetailModel.FileName.ToSafeString(),
                        P_USERNAME = user_name.ToSafeString(),
                        P_STATUS = searchLeaveMsgFileNameDetailModel.Status.ToSafeString()
                    };

                    var result = _queryProcessor.Execute(query);

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Info("Error when call ReadSearchLeaveMsgFileNameDetailDataDefault");
                    Logger.Info(ex.GetErrorMessage());
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
            string lov_val5 = "FBBOR021";
            string lov_name = "L_SEARCH_STATUS";

            var LovDataStatus = base.LovData.Where(p => p.LovValue5 == lov_val5 && p.Name == lov_name).OrderBy(p => p.OrderBy)
                                    .Select(p =>
                                    {
                                        return new { LOV_NAME = p.LovValue2, LOV_VAL1 = p.LovValue1 };
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

        #region ExportExcel (Leavemessage Filename)

        // [ExportExcel]
        public ActionResult ExportReadSearchLeaveMsgFileNameData(string dataS)
        {
            string user_name = base.CurrentUser.UserName.ToSafeString();
            var searchLeaveMsgFileNameModel = new JavaScriptSerializer().Deserialize<SearchLeaveMsgFileNameModel>(dataS);

            var query = new GetLeaveMessageSearchFileNameQuery()
            {
                P_USERNAME = user_name.ToSafeString(),
                P_FILE_NAME = searchLeaveMsgFileNameModel.FileName.ToSafeString(),
                P_START_DATE = searchLeaveMsgFileNameModel.StartDate.ToSafeString(),
                P_END_DATE = searchLeaveMsgFileNameModel.EndDate.ToSafeString()
            };

            var result = _queryProcessor.Execute(query);

            var listall = ConvertSearchLeaveMsgFileNameModel(result);

            rptName = string.Format(rptName, "Search Leavemessage Filename Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSearchLeaveMsgFileNameExcelName("SearchLeavemessageFilenameReport");

            var bytes = GenerateSearchLeaveMsgFileNameEntitytoExcel<SearchLeaveMsgFileNameExportList>(listall, filename, "FBBOR021");

            return File(bytes, "application/excel", filename + ".xls");

        }

        // [ExportExcel] Convert SearchLeaveMsgFileNameList To SearchLeaveMsgFileNameExportList
        private List<SearchLeaveMsgFileNameExportList> ConvertSearchLeaveMsgFileNameModel(List<SearchLeaveMsgFileNameList> list)
        {
            var dataexportlist = list.Select(x => new SearchLeaveMsgFileNameExportList()
            {
                p_file_name = x.p_file_name.ToSafeString(),
                p_user_name = x.p_user_name.ToSafeString(),
                p_create_date = x.p_create_date.ToSafeString(),
                p_summary_record = x.p_summary_record.ToSafeString(),
                p_completed_record = x.p_completed_record.ToSafeString(),
                p_fail_record = x.p_fail_record.ToSafeString()
            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Get Excel File Name
        private string GetSearchLeaveMsgFileNameExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateSearchLeaveMsgFileNameEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            Logger.Info("GenerateSearchLeaveMsgFileNameEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5 && p.Name.StartsWith("L_COLUMN")).OrderBy(p => p.OrderBy).ToList();

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

            var data_ = GenerateSearchLeaveMsgFileNameExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateSearchLeaveMsgFileNameExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            Logger.Info("GenerateSearchLeaveMsgFileNameExcel start");
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
            int iCol = 6;

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

        #region ExportExcel (Leavemessage Filename Detail)

        // [ExportExcel]
        public ActionResult ExportReadSearchLeaveMsgFileNameDetailData(string dataS)
        {
            string user_name = base.CurrentUser.UserName.ToSafeString();
            var searchLeaveMsgFileNameDetailModel = new JavaScriptSerializer().Deserialize<SearchLeaveMsgFileNameDetailModel>(dataS);

            var query = new GetLeaveMessageSearchFileNameDetailQuery()
            {
                P_FILE_NAME = searchLeaveMsgFileNameDetailModel.FileName.ToString(),
                P_USERNAME = user_name.ToSafeString(),
                P_STATUS = searchLeaveMsgFileNameDetailModel.Status.ToString()
            };

            var result = _queryProcessor.Execute(query);

            var listall = ConvertSearchLeaveMsgFileNameDetailModel(result);

            rptName = string.Format(rptName, "Search Leavemessage Filename Detail Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSearchLeaveMsgFileNameDetailExcelName("SearchLeavemessageFilenameDetailReport");

            var bytes = GenerateSearchLeaveMsgFileNameDetailEntitytoExcel<SearchLeaveMsgFileNameDetailExportList>(listall, filename, "FBBOR021");

            return File(bytes, "application/excel", filename + ".xls");

        }

        // [ExportExcel] Convert SearchLeaveMsgFileNameDetailList To SearchLeaveMsgFileNameDetailList
        private List<SearchLeaveMsgFileNameDetailExportList> ConvertSearchLeaveMsgFileNameDetailModel(List<SearchLeaveMsgFileNameDetailList> list)
        {
            var dataexportlist = list.Select(x => new SearchLeaveMsgFileNameDetailExportList()
            {
                service_speed = x.service_speed.ToSafeString(),
                cust_name = x.cust_name.ToSafeString(),
                cust_surname = x.cust_surname.ToSafeString(),
                contact_mobile_no = x.contact_mobile_no.ToSafeString(),
                is_ais_mobile = x.is_ais_mobile.ToSafeString(),
                contact_time = x.contact_time.ToSafeString(),
                contact_email = x.contact_email.ToSafeString(),
                address_type = x.address_type.ToSafeString(),
                building_name = x.building_name.ToSafeString(),
                village_name = x.village_name.ToSafeString(),
                house_no = x.house_no.ToSafeString(),
                soi = x.soi.ToSafeString(),
                road = x.road.ToSafeString(),
                tumbol = x.tumbol.ToSafeString(),
                amphur = x.amphur.ToSafeString(),
                province = x.province.ToSafeString(),
                postal_code = x.postal_code.ToSafeString(),
                campaign_project_name = x.campaign_project_name.ToSafeString(),
                status = x.status.ToSafeString(),
                reason_code = x.reason_code.ToSafeString()
            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Get Excel File Name
        private string GetSearchLeaveMsgFileNameDetailExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateSearchLeaveMsgFileNameDetailEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            Logger.Info("GenerateSearchLeaveMsgFileNameDetailEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.LovValue5 == LovValue5 && p.Name.StartsWith("COLUMN_EXCEL_")).OrderBy(p => p.OrderBy).ToList();

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

            var data_ = GenerateSearchLeaveMsgFileNameDetailExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateSearchLeaveMsgFileNameDetailExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            Logger.Info("GenerateSearchLeaveMsgFileNameDetailExcel start");
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
            int iCol = 20;

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