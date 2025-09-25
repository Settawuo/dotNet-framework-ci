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
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;



namespace FBBConfig.Controllers
{
    public class RequestFormReportController : FBBConfigController
    {
        //
        // GET: /Test/
        private readonly IQueryProcessor _queryProcessor;
        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run report date : {0}";

        public RequestFormReportController(ILogger logger, IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }
        public ActionResult RequestFormReport()
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            var headerQuery = new GetLovQuery
            {
                LovType = "FBBDORM_CONSTANT",
                LovName = "H_GRID_REQREPORT",
            };
            var headerData = _queryProcessor.Execute(headerQuery);
            ViewBag.configscreen = headerData;

            return View();
        }
        //private void SetViewBagLov(string screenType, string LovValue5)
        //{
        //    var filenameQuery = new GetLovQuery
        //    {
        //        LovType = "FBBDORM_CONSTANT",
        //        LovName = "H_GRID_REQREPORT",
        //    };
        //    var filenamerData = _queryProcessor.Execute(filenameQuery);
        //    ViewBag.configscreen = filenamerData;


        //    var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
        //    ViewBag.configscreen = filenamerData;
        //}

        public ActionResult SearchDataSourceRequest([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            RequestFormReturn result;
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            if (dataS != null && dataS != "")
            {
                var SearchPara = new JavaScriptSerializer().Deserialize<RequestFormReportModel>(dataS);
                result = GetDataSearchModel(SearchPara);

                return Json(new
                {
                    Data = result.P_RES_DATA,
                    Total = result.P_PAGE_COUNT
                });

                //return Json(
                //    result.ToDataSourceResult(request)
                //    , JsonRequestBehavior.AllowGet);

            }
            else
            {
                return null;
            }
        }
        private RequestFormReturn GetDataSearchModel(RequestFormReportModel SearchPara) //List<ReportRequestFormListDetail>
        {
            RequestFormReturn result = new RequestFormReturn();

            try
            {
                var query = new GetRequestFormReportQuery()
                {
                    P_DATE_FROM = SearchPara.P_DATE_FROM,
                    P_DATE_TO = SearchPara.P_DATE_TO,
                    P_REGION_CODE = SearchPara.P_REGION_CODE,
                    P_PROVINCE = SearchPara.P_PROVINCE,
                    P_PROCESS_STATUS = SearchPara.P_PROCESS_STATUS,
                    P_PAGE_INDEX = SearchPara.P_PAGE_INDEX,
                    P_PAGE_SIZE = SearchPara.P_PAGE_SIZE

                };

                //List<ReportRequestFormListDetail> resultList = new List<ReportRequestFormListDetail>();

                result = _queryProcessor.Execute(query);

                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetBaseException());
                return result;
            }
        }


        public ActionResult ExportExcelRequestForm(string dataS)
        {
            List<ReportRequestFormListDetail> listall;
            var searchModel = new JavaScriptSerializer().Deserialize<RequestFormReportModel>(dataS);
            //var result = GetDataSearchModel(searchoawcModel);
            listall = GetExportRequestForm(searchModel);

            rptCriteria = string.Format(rptCriteria, searchModel.P_DATE_FROM, searchModel.P_DATE_TO);
            rptName = string.Format(rptName, "Request Form Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));

            string filename = GetExcelName("RequestFormReport");

            var bytes = GenerateEntitytoExcel<ReportRequestFormListDetail>(listall, filename);

            return File(bytes, "application/excel", filename + ".xlsx");

        }

        public List<ReportRequestFormListDetail> GetExportRequestForm(RequestFormReportModel model)
        {

            RequestFormReturn result = new RequestFormReturn();
            List<ReportRequestFormListDetail> objList = new List<ReportRequestFormListDetail>();
            try
            {
                var query = new GetRequestFormReportQuery
                {
                    P_DATE_FROM = model.P_DATE_FROM,
                    P_DATE_TO = model.P_DATE_TO,
                    P_REGION_CODE = model.P_REGION_CODE,
                    P_PROVINCE = model.P_PROVINCE,
                    P_PROCESS_STATUS = model.P_PROCESS_STATUS,
                    P_PAGE_INDEX = 1,
                    P_PAGE_SIZE = 9999999


                };

                result = _queryProcessor.Execute(query);
                objList = result.P_RES_DATA;

                return objList;

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetBaseException());
                return new List<ReportRequestFormListDetail>();
            }

        }

        private string GetExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("ddMMyyyy");
            var filenameQuery = new GetLovQuery
            {
                LovType = "FBBDORM_CONSTANT",
                LovName = "REQREPORT_EXCEL_NAME",
            };
            var filenamerData = _queryProcessor.Execute(filenameQuery);
            var name = filenamerData[0].Text.ToString();

            result = string.Format(name, fileName, dateNow, timeNow);

            return result;
        }

        public byte[] GenerateEntitytoExcel<T>(List<T> data, string fileName)
        {

            System.ComponentModel.PropertyDescriptorCollection properties =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (System.ComponentModel.PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }

            string tempPath = System.IO.Path.GetTempPath();
            var data_ = GenerateExcel(table, "WorkSheet", tempPath, fileName);
            return data_;
        }

        private byte[] GenerateExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName)
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
            //ExcelRange range = null;
            ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            int iRow;
            int iHeaderRow;
            string strRow;
            string strColumn1 = string.Empty;
            int iCol = 27;

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
                strRow = iRow.ToString();

                rangeHeader = worksheet.SelectedRange[iRow, 1, iRow, iCol];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#FFFF00"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                rangeHeader.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                rangeHeader.Style.Font.Bold = true;
                rangeHeader.Style.ShrinkToFit = false;

                worksheet.View.FreezePanes(iHeaderRow, 1);
                strColumn1 = string.Format("A{0}", strRow);


                var headerQuery = new GetLovQuery
                {
                    LovType = "FBBDORM_CONSTANT",
                    LovName = "H_GRID_REQREPORT",
                };
                var headerData = _queryProcessor.Execute(headerQuery);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                dataToExcel.Columns["RowNumber"].ColumnName = headerData[0].Text.ToString();
                dataToExcel.Columns["TYPE_CUSTOMER_REQUEST"].ColumnName = headerData[1].Text.ToString();
                dataToExcel.Columns["CREATED_DT"].ColumnName = headerData[2].Text.ToString();
                dataToExcel.Columns["CUSTOMER_FIRST_NAME"].ColumnName = headerData[3].Text.ToString();
                dataToExcel.Columns["CUSTOMER_LAST_NAME"].ColumnName = headerData[4].Text.ToString();
                dataToExcel.Columns["CONTRACT_PHONE"].ColumnName = headerData[5].Text.ToString();
                dataToExcel.Columns["DORMITORY_NAME"].ColumnName = headerData[6].Text.ToString();
                dataToExcel.Columns["TYPE_DORMITORY"].ColumnName = headerData[7].Text.ToString();
                dataToExcel.Columns["HOME_NO"].ColumnName = headerData[8].Text.ToString();
                dataToExcel.Columns["MOO"].ColumnName = headerData[9].Text.ToString();
                dataToExcel.Columns["SOI"].ColumnName = headerData[10].Text.ToString();
                dataToExcel.Columns["STREET"].ColumnName = headerData[11].Text.ToString();
                dataToExcel.Columns["Tumbon"].ColumnName = headerData[12].Text.ToString();
                dataToExcel.Columns["AMPHUR"].ColumnName = headerData[13].Text.ToString();
                dataToExcel.Columns["province"].ColumnName = headerData[14].Text.ToString();
                dataToExcel.Columns["ZIPCODE"].ColumnName = headerData[15].Text.ToString();
                dataToExcel.Columns["Region_Code"].ColumnName = headerData[16].Text.ToString();
                dataToExcel.Columns["A_BUILDING"].ColumnName = headerData[17].Text.ToString();
                dataToExcel.Columns["A_UNIT"].ColumnName = headerData[18].Text.ToString();
                dataToExcel.Columns["A_LIVING"].ColumnName = headerData[19].Text.ToString();
                dataToExcel.Columns["PHONE_CABLE"].ColumnName = headerData[20].Text.ToString();
                dataToExcel.Columns["PROBLEM_INTERNET"].ColumnName = headerData[21].Text.ToString();
                dataToExcel.Columns["A_UNIT_USE_INTERNET"].ColumnName = headerData[22].Text.ToString();
                dataToExcel.Columns["OLD_SYSTEM"].ColumnName = headerData[23].Text.ToString();
                dataToExcel.Columns["OLD_VENDOR_SERVICE"].ColumnName = headerData[24].Text.ToString();
                dataToExcel.Columns["SEND_TO_SALE_DT"].ColumnName = headerData[25].Text.ToString();
                dataToExcel.Columns["PROCESS_STATUS"].ColumnName = headerData[26].Text.ToString();
                worksheet.Cells[strColumn1].LoadFromDataTable(dataToExcel, true, TableStyles.None);
                worksheet.Cells.AutoFitColumns();


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
        public JsonResult SelectRegion(string langFlag = "N")
        {
            var query = new SelectRegionQuery
            {
                Lang_Flag = langFlag
            };
            var data = _queryProcessor.Execute(query);

            data.Insert(0, new LovModel { DISPLAY_VAL = "เลือกทั้งหมด", LOV_NAME = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SelectProvinceForSearch(string type = "FBBDORM_CONSTANT", string name = "INPUT_PROVINCE", string regionCode = "")
        {
            var query = new GetLovQuery
            {
                LovType = type,
                LovName = name,

            };
            List<LovValueModel> data;
            if (regionCode != "")
            {
                data = _queryProcessor.Execute(query).Where(p => p.LovValue2 == regionCode.ToString()).ToList();
                data.Insert(0, new LovValueModel { Text = "เลือกทั้งหมด", LovValue1 = "" });

            }
            else
            {
                data = _queryProcessor.Execute(query);
                data.Insert(0, new LovValueModel { Text = "เลือกทั้งหมด", LovValue1 = "" });
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectStatus(string type = "FBBDORM_CONSTANT", string name = "INPUT_STATUS")
        {
            var query = new GetLovQuery
            {
                LovType = type,
                LovName = name
            };

            var data = _queryProcessor.Execute(query);


            data.Insert(0, new LovValueModel { Text = "เลือกทั้งหมด", LovValue1 = "" });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
