using FBBConfig.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;


namespace FBBConfig.Controllers
{
    public class SLAReportController : FBBConfigController
    {
        private readonly IQueryProcessor _queryProcessor;

        public SLAReportController(
            ILogger logger,
            IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        //
        // GET: /SLAReport/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        public JsonResult SelecTechnology(string type, string lovval5, string lovname)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == type && p.Name == lovname).ToList();
            LovDataScreen.Insert(0, new LovValueModel { LovValue1 = "All", LovValue2 = "All" });
            return Json(LovDataScreen, JsonRequestBehavior.AllowGet);
        }

        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        private void SetHeaderLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5 && p.Name.StartsWith("R_HEADER_")).OrderBy(p => p.OrderBy).ToList();
            ViewBag.headerScreen = LovDataScreen;
        }

        public ActionResult Configuration(string page)
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;
            SetViewBagLov("FBBPAYG_SCREEN", page);

            return null;
        }

        public ActionResult AppointmentDetail()
        {
            this.Configuration("REPORTSLADTL");

            return View();
        }

        public ActionResult AppointmentSummary()
        {
            this.Configuration("REPORTSLASUM");

            return View();
        }



        public ActionResult AppointmentDetailAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<AppointmentDetailQuery>(dataS);
                var result = this.GetAppointmentDetail(searchEventModel);

                return Json(result.ToDataSourceResult(request));
            }
            else
            {
                return null;
            }
        }


        public List<AppointmentDetailTableModel> GetAppointmentDetail(AppointmentDetailQuery model)
        {
            try
            {
                model.dateFrom = model.dateFrom.Replace(@"/", string.Empty);
                model.dateTo = model.dateTo.Replace(@"/", string.Empty);
                //var query = new AppointmentDetailQuery()
                //{
                //    Service = model.SERVICE,
                //    DateFrom = model.VENDOR,
                //    DateTo = model.ORDER_TYPE
                //};
                List<AppointmentDetailModel> q = _queryProcessor.Execute(model);
                //List<AppointmentDetailTableModel> result = new List<AppointmentDetailTableModel>();

                var result = (from data in q
                              select new AppointmentDetailTableModel
                              {
                                  customer_order_no = data.customer_order_no,
                                  access_number = data.access_number,
                                  cust_name = data.cust_name,
                                  WORK_ORD_NO = data.WORK_ORD_NO,
                                  JOB_TYPE = data.JOB_TYPE,
                                  SERVICE = data.SERVICE,
                                  ORDER_CREATE_DATE = data.ORDER_CREATE_DATE.ToDateDisplayText(),
                                  CREATED_PROSPECT_DATE = data.CREATED_PROSPECT_DATE.ToDateDisplayText(),
                                  ALREADY_APPOINTMENT_DATE = data.ALREADY_APPOINTMENT_DATE.ToDateDisplayText(),
                                  SLA = data.SLA
                              }).ToList();

                //foreach (AppointmentDetailModel data in q)
                //{
                //    var list = new AppointmentDetailTableModel()
                //    {
                //        customer_order_no = data.customer_order_no,
                //        access_number = data.access_number,
                //        cust_name = data.cust_name,
                //        WORK_ORD_NO = data.WORK_ORD_NO,
                //        JOB_TYPE = data.JOB_TYPE,
                //        SERVICE = data.SERVICE,
                //        ORDER_CREATE_DATE = data.ORDER_CREATE_DATE.ToDateDisplayText(),
                //        CREATED_PROSPECT_DATE = data.CREATED_PROSPECT_DATE.ToDateDisplayText(),
                //        ALREADY_APPOINTMENT_DATE = data.ALREADY_APPOINTMENT_DATE.ToDateDisplayText(),
                //        ALREADY_APPOINTMENT_DATE = data.ALREADY_APPOINTMENT_DATE.HasValue ? data.ALREADY_APPOINTMENT_DATE.Value.ToString("dd/MM/yyyy HH:mm") : "",
                //        SLA = data.SLA
                //    };
                //    result.Add(list);
                //}

                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<AppointmentDetailTableModel>();
            }
        }

        public ActionResult ExportAppointmentDetail(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<AppointmentDetailQuery>(dataS);
            List<AppointmentDetailTableModel> result = this.GetAppointmentDetail(searchModel);

            string filename = GetExcelName("SLAAppointmentDetail");
            var bytes = AppointmentDetailExcel<AppointmentDetailTableModel>(result, filename, "SLADetail");
            return File(bytes, "application/excel", filename + ".xlsx");
        }

        public byte[] AppointmentDetailExcel<T>(List<T> data, string filename, string LovValue5)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            //var lovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBB_RPTPORT006").ToList();
            SetHeaderLov("FBBPAYG_SCREEN", "REPORTSLADTL");
            var lovDataScreen = ViewBag.headerScreen;
            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1, Type.GetType("System.String"));
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Type.GetType("System.String"));
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
            string tempPath = Path.GetTempPath();
            var data_ = GenerateAppointmentDetailExcel(table, "WorkSheet", tempPath, filename, LovValue5);
            return data_;
        }

        private byte[] GenerateAppointmentDetailExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            string finalFileNameWithPath = PrepareFile(directoryPath, fileName);
            var newFile = new FileInfo(finalFileNameWithPath);
            //ExcelRange range = null;
            //ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                rangeHeader = worksheet.SelectedRange[1, 1, 1, dataToExcel.Columns.Count];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.View.FreezePanes(2, 1);
                worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, true, TableStyles.None);

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





        public ActionResult AppointmentSummaryAsync([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            if (dataS != null && dataS != "")
            {
                var searchEventModel = new JavaScriptSerializer().Deserialize<AppointmentSummaryQuery>(dataS);
                var result = this.GetAppointmentSummary(searchEventModel);

                return Json(result.ToDataSourceResult(request));
            }
            else
            {
                return null;
            }
        }


        public List<AppointmentSummaryTableModel> GetAppointmentSummary(AppointmentSummaryQuery model)
        {
            try
            {
                model.dateFrom = model.dateFrom.Replace(@"/", string.Empty);
                model.dateTo = model.dateTo.Replace(@"/", string.Empty);
                //return  _queryProcessor.Execute(model);
                List<AppointmentSummaryModel> q = _queryProcessor.Execute(model);
                List<AppointmentSummaryTableModel> result = new List<AppointmentSummaryTableModel>();

                foreach (AppointmentSummaryModel data in q)
                {
                    var list = new AppointmentSummaryTableModel()
                    {
                        ORDER_CREATE_DATE = data.ORDER_CREATE_DATE.HasValue ? data.ORDER_CREATE_DATE.Value.ToString("dd/MM/yyyy") : "",
                        SERVICE = data.SERVICE,
                        SixHR = data.SixHR,
                        TWHR = data.TWHR,
                        ETHR = data.ETHR,
                        TFHR = data.TFHR,
                        FEHR = data.FEHR,
                        OverHR = data.OverHR,
                        ONPROCESS = data.ONPROCESS,
                        TOTAL = data.TOTAL,
                    };
                    result.Add(list);
                }

                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<AppointmentSummaryTableModel>();
            }
        }

        public ActionResult ExportAppointmentSummary(string dataS)
        {
            var searchModel = new JavaScriptSerializer().Deserialize<AppointmentSummaryQuery>(dataS);
            List<AppointmentSummaryTableModel> result = this.GetAppointmentSummary(searchModel);

            string filename = GetExcelName("SLAAppointmentSummary");
            var bytes = AppointmentSummaryExcel<AppointmentSummaryTableModel>(result, filename, "SLASummary");
            return File(bytes, "application/excel", filename + ".xlsx");
        }

        public byte[] AppointmentSummaryExcel<T>(List<T> data, string filename, string LovValue5)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            //var lovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == "FBB_RPTPORT006").ToList();
            SetHeaderLov("FBBPAYG_SCREEN", "REPORTSLASUM");
            var lovDataScreen = ViewBag.headerScreen;
            if (lovDataScreen.Count != 0)
            {
                for (int i = 0; i < props.Count; i++)
                {
                    table.Columns.Add(lovDataScreen[i].LovValue1, Type.GetType("System.String"));
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    PropertyDescriptor prop = props[i];
                    table.Columns.Add(prop.Name, Type.GetType("System.String"));
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
            string tempPath = Path.GetTempPath();
            var data_ = GenerateAppointmentSummaryExcel(table, "WorkSheet", tempPath, filename, LovValue5);
            return data_;
        }

        private byte[] GenerateAppointmentSummaryExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            string finalFileNameWithPath = PrepareFile(directoryPath, fileName);
            var newFile = new FileInfo(finalFileNameWithPath);
            //ExcelRange range = null;
            //ExcelRange rangeReportDetail = null;
            ExcelRange rangeHeader = null;

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                rangeHeader = worksheet.SelectedRange[1, 1, 1, dataToExcel.Columns.Count];
                rangeHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                rangeHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.ColorTranslator.FromHtml("#c9c9c9"));
                rangeHeader.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.View.FreezePanes(2, 1);
                worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, true, TableStyles.None);

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

        private string PrepareFile(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
            // Check File Duplicate
            if (System.IO.File.Exists(finalFileNameWithPath)) System.IO.File.Delete(finalFileNameWithPath);

            return finalFileNameWithPath;
        }

    }
}
