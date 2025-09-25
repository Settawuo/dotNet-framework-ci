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
using System.Reflection;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace FBBConfig.Controllers
{
    public class SaleTrackingController : FBBConfigController
    {
        #region Properties

        private readonly IQueryProcessor _queryProcessor;

        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run Report Date/Time : {0}";

        #endregion

        #region Constructor

        public SaleTrackingController(ILogger logger, IQueryProcessor queryProcessor)
        {
            base._Logger = logger;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region ActionResult

        // GET: /SaleTracking/Index
        public ActionResult Index(string LocCode = "", string AscCode = "", string DateFrom = "", string DateTo = "", string Status = "", string CustName = "")
        {
            if (null == base.CurrentUser)
                return RedirectToAction("Logout", "Account");

            ViewBag.User = base.CurrentUser;

            // Set ViewBag Lov
            SetViewBagLov("SCREEN", "FBBREPORT_SALE_1");
            var rptModel = new SaleTrackingModel();

            return View(rptModel);
        }

        // Read Search Sale Tracking DataDefault
        public ActionResult ReadSearchSaleTrackingDataDefault([DataSourceRequest] DataSourceRequest request, string dataS = "")
        {
            _Logger.Info("ReadSearchSaleTrackingDataDefault start");
            if (dataS != null && dataS != "")
            {
                try
                {
                    _Logger.Info("ReadSearchSaleTrackingDataDefault try");
                    var searchSaleTrackingModel = new JavaScriptSerializer().Deserialize<SaleTrackingModel>(dataS);

                    var query = GetSaleTrackingSearchModel(searchSaleTrackingModel);

                    var dataout = _queryProcessor.Execute(query);

                    var result = GetOrders(dataout);

                    if (searchSaleTrackingModel.Status != "All")
                    {
                        result = result.Where(t => t.current_state == searchSaleTrackingModel.Status).ToList();
                    }

                    return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                    //var jsonResult = Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
                    //jsonResult.MaxJsonLength = int.MaxValue;
                    //return jsonResult;
                }
                catch (Exception ex)
                {
                    _Logger.Info("Error when call ReadSearchSaleTrackingDataDefault");
                    _Logger.Info(ex.GetErrorMessage());
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        // Read Search Sale Tracking Click Button 
        public ActionResult ReadSearchSaleTracking(string dataS = "")
        {
            _Logger.Info("ReadSearchSaleTracking start");
            var searchSaleTrackingModel = new JavaScriptSerializer().Deserialize<SaleTrackingModel>(dataS);

            var query = GetSaleTrackingSearchModel(searchSaleTrackingModel);

            var dataout = _queryProcessor.Execute(query);

            var result = GetOrders(dataout);

            if (searchSaleTrackingModel.Status != "All")
            {
                result = result.Where(t => t.current_state == searchSaleTrackingModel.Status).ToList();
            }

            string itemDataS = "0";

            if (result != null && result.Count != 0)
            {
                itemDataS = "1";
            }

            return Json(new { itemDataS = itemDataS, }, JsonRequestBehavior.AllowGet);
            //var jsonResult = Json(new { itemDataS = itemDataS, }, JsonRequestBehavior.AllowGet);
            //jsonResult.MaxJsonLength = int.MaxValue;
            //return jsonResult;

        }

        #endregion

        #region Private Methods

        // Set ViewBag Lov
        private void SetViewBagLov(string screenType, string LovValue5)
        {
            var LovDataScreen = base.LovData.Where(p => p.Type == screenType && p.LovValue5 == LovValue5).ToList();
            ViewBag.configscreen = LovDataScreen;
        }

        // Get Order
        // เมื่อมีการแก้ไข Logic กรุณาแก้ไขที่ WBBWeb.Controllers => TrackingController Method GetOrders ใน #region Logic GetOrders ด้วย
        private List<SaleTrackingList> GetOrders(List<AIRNETEntity.StoredProc.TrackingModel> list)
        {
            try
            {
                var data = list.OrderByDescending(l => l.register_date).ToList().Select(x => new SaleTrackingList()
                {
                    fibrenet_id = x.fibrenet_id.ToSafeString(),
                    order_type = x.order_type.ToSafeString(),
                    start_date = x.start_date.ToSafeString(),
                    end_date = x.end_date.ToSafeString(),
                    current_state = x.current_state.ToSafeString(),

                    transaction_state = x.transaction_state.ToSafeString(),
                    complete_install_date = x.complete_install_date,
                    complete_install_date_str = x.complete_install_date.ToDateDisplayText(),
                    cancel_install_reason_en = x.cancel_install_reason_en,
                    onservice_date = x.onservice_date,

                    location_code = x.location_code.ToSafeString(),
                    asc_code = x.asc_code.ToSafeString(),
                    employee_id = x.employee_id.ToSafeString(),
                    register_date = x.register_date,
                    register_date_str = x.register_date.ToDateDisplayText(),
                    customer_name = x.customer_name.ToSafeString(),
                    building_village = x.building_village.ToSafeString(),
                    sub_district = x.sub_district.ToSafeString(),
                    district = x.district.ToSafeString(),
                    province = x.province.ToSafeString(),
                    technology = x.technology.ToSafeString(),
                    playbox_flag = x.playbox_flag.ToSafeString(),
                    fixedline_flag = x.fixedline_flag.ToSafeString(),
                    status_date = x.status_date.ToSafeString().Split(' ')[0]
                });

                List<SaleTrackingList> dataout = data.ToList();

                string lastestStatus = "Wait_for_Appointment";
                int lastestDate = 0;
                int lastestTime = 0;
                string lastestDateTime = "";

                foreach (var tmp in dataout)
                {
                    if (!string.IsNullOrEmpty(tmp.fibrenet_id)
                        && !string.IsNullOrEmpty(tmp.start_date)
                        && !string.IsNullOrEmpty(tmp.end_date)
                        && !string.IsNullOrEmpty(tmp.order_type))
                    {
                        List<FIBRENetID> FIBRENetID_List = new List<FIBRENetID>();
                        FIBRENetID FibreNet = new FIBRENetID()
                        {
                            FIBRENET_ID = tmp.fibrenet_id,
                            START_DATE = tmp.start_date,
                            END_DATE = tmp.end_date
                        };
                        FIBRENetID_List.Add(FibreNet);
                        var result = FBSSQueryOrder(FIBRENetID_List, tmp.order_type);

                        tmp.current_state = "No_Order";

                        if (result.Order_Details_List != null && result.Order_Details_List.Any())
                        {
                            tmp.current_state = "Wait_for_Appointment";

                            string transaction_state = result.Order_Details_List.FirstOrDefault().TRANSACTION_STATE;

                            if (result.Order_Details_List[0].ACTIVITY_DETAILS != null)
                            {
                                var Appointment_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Appointment");
                                if (Appointment_State_Info.Any())
                                {
                                    tmp.current_state = "Appointment";

                                    if (transaction_state == "Cancelled")
                                    {
                                        tmp.current_state = "Cancel Appointment";
                                    }

                                    int count = 1;
                                    foreach (var info_tmp in Appointment_State_Info)
                                    {
                                        try
                                        {
                                            PropertyInfo propertyInfo = tmp.GetType().GetProperty("appointment_date_" + count + "_str");
                                            propertyInfo.SetValue(tmp, Convert.ChangeType(info_tmp.APPOINTMENT_DATE, propertyInfo.PropertyType), null);

                                            propertyInfo = tmp.GetType().GetProperty("appointment_timeslot_" + count + "_str");
                                            propertyInfo.SetValue(tmp, Convert.ChangeType(info_tmp.APPOINTMENT_TIMESLOT, propertyInfo.PropertyType), null);
                                            count++;
                                        }
                                        catch (Exception)
                                        {
                                            continue;
                                        }
                                    }
                                }

                                var Install_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Install");
                                if (Install_State_Info.Any())
                                {
                                    tmp.current_state = "Installation";
                                    var complete_state = Install_State_Info.Where(x => string.IsNullOrEmpty(x.FOA_REJECT_REASON));
                                    if (complete_state.Any())
                                    {
                                        DateTime CompleteInstallDate;
                                        tmp.complete_install_date = DateTime.TryParseExact(complete_state.FirstOrDefault().COMPLETED_DATE, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out CompleteInstallDate) ? CompleteInstallDate : tmp.complete_install_date;
                                        tmp.complete_install_date_str = complete_state.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault().COMPLETED_DATE;
                                    }
                                    else
                                    {
                                        var newest_state = Install_State_Info.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault();
                                        DateTime CompleteInstallDate;
                                        tmp.complete_install_date_str = DateTime.TryParseExact(newest_state.COMPLETED_DATE, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out CompleteInstallDate) ? CompleteInstallDate.ToString("dd/MM/yyyy") : tmp.complete_install_date_str.ToSafeString();

                                        if (transaction_state == "Cancelled")
                                        {
                                            tmp.current_state = "Cancel Installation";
                                            tmp.cancel_install_reason_en = newest_state.FOA_REJECT_REASON;
                                            tmp.complete_install_date_str = newest_state.COMPLETED_DATE;
                                        }
                                    }
                                }

                                var Complete_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "SFF");
                                if (Complete_State_Info.Any())
                                {
                                    tmp.current_state = "On Service";
                                    tmp.onservice_date = Complete_State_Info.FirstOrDefault().COMPLETED_DATE;
                                }
                            }
                            foreach (var activityDetail in result.Order_Details_List[0].ACTIVITY_DETAILS)
                            {
                                int thisDate = 0;
                                string thisDateS = activityDetail.CREATED_DATE.Split(' ')[0];
                                thisDate = int.Parse(thisDateS.Split('/')[2] + thisDateS.Split('/')[1] + thisDateS.Split('/')[0]);

                                int thisTime = 0;
                                string thisTimeS = activityDetail.CREATED_DATE.Split(' ')[1];
                                thisTime = int.Parse(thisTimeS.Split(':')[0] + thisTimeS.Split(':')[1] + thisTimeS.Split(':')[2]);

                                string thisDateTime = activityDetail.CREATED_DATE;

                                Console.WriteLine("lastestStatus: " + lastestStatus);
                                Console.WriteLine("lastestDate : " + lastestDate + " / thisDate: " + thisDate);
                                Console.WriteLine("lastestTime : " + lastestTime + " / thisTime: " + thisTime);

                                if (lastestStatus == "Wait_for_Appointment" || (lastestDate <= thisDate && lastestTime < thisTime))
                                {
                                    if (activityDetail.ACTIVITY == "Appointment")
                                    {
                                        lastestStatus = "Appointment";
                                        if (transaction_state == "Cancelled")
                                        {
                                            lastestStatus = "Cancel Appointment";
                                        }
                                    }
                                    else if (activityDetail.ACTIVITY == "Install")
                                    {
                                        lastestStatus = "Installation";
                                        if (transaction_state == "Cancelled")
                                        {
                                            lastestStatus = "Cancel Installation";
                                        }
                                    }
                                    else if (activityDetail.ACTIVITY == "SFF")
                                    {
                                        lastestStatus = "On Service";
                                    }
                                    else
                                    {
                                        lastestStatus = "Wait_for_Appointment";
                                    }

                                    lastestDate = thisDate;
                                    lastestTime = thisTime;
                                    lastestDateTime = thisDateTime;
                                }
                            }
                            tmp.current_state = lastestStatus;
                            tmp.status_date = lastestDateTime;

                            if (lastestStatus != "Installation")
                            {
                                tmp.complete_install_date_str = "";
                            }
                        }
                    }
                }

                return dataout;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
            }

            return new List<SaleTrackingList>();
        }

        #endregion

        #region Public Methods

        // Set Lov DDL Status
        public JsonResult SetDDLStatus()
        {
            string lov_val5 = "FBBREPORT_SALE_1";
            string lov_name = "L_TRACKING_STATUS";
            string lov_type = "SCREEN";

            var LovDataStatus = base.LovData.Where(p => p.LovValue5 == lov_val5 && p.Name == lov_name && p.Type == lov_type).OrderBy(p => p.OrderBy)
                                    .Select(p =>
                                    {
                                        return new { LOV_NAME = p.LovValue1, LOV_VAL1 = p.LovValue1 };
                                    }).ToList();

            return Json(LovDataStatus, JsonRequestBehavior.AllowGet);
        }

        // Get SaleTrackingSearchModel
        public GetTrackingQuery GetSaleTrackingSearchModel(SaleTrackingModel searchSaleTrackingModel)
        {
            _Logger.Info("GetSaleTrackingSearchModel Start");
            try
            {
                _Logger.Info("GetSaleTrackingSearchModel Try");
                var query = new GetTrackingQuery()
                {
                    P_Id_Card = "",
                    P_First_Name = "",
                    P_Last_Name = "",
                    P_Location_Code = searchSaleTrackingModel.LocCode.ToString(),
                    P_Asc_Code = searchSaleTrackingModel.AscCode.ToString(),
                    P_Date_From = searchSaleTrackingModel.DateFrom.ToSafeString(),
                    P_Date_To = searchSaleTrackingModel.DateTo.ToSafeString(),
                    P_Cust_Name = searchSaleTrackingModel.CustName.ToString(),
                    P_User = "FBB_ADMIN"
                };

                return query;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error when call GetSaleTrackingSearchModel");
                _Logger.Info(ex.GetErrorMessage());
                return new GetTrackingQuery();
            }

        }

        // FBSSQueryOrder
        public QueryOrderModel FBSSQueryOrder(List<FIBRENetID> FIBRENetID_List, string ORDER_TYPE)
        {
            var query = new QueryOrderQuery();
            query.ORDER_TYPE = ORDER_TYPE;
            query.FIBRENetID_List = FIBRENetID_List;

            var result = _queryProcessor.Execute(query);

            return result;
        }

        #endregion

        #region Export Excel

        // [ExportExcel]
        public ActionResult ExportSaleTrackingData(string dataS)
        {
            var searchSaleTrackingModel = new JavaScriptSerializer().Deserialize<SaleTrackingModel>(dataS);

            var query = GetSaleTrackingSearchModel(searchSaleTrackingModel);

            var dataout = _queryProcessor.Execute(query);

            var result = GetOrders(dataout);

            if (searchSaleTrackingModel.Status != "All")
            {
                result = result.Where(t => t.current_state == searchSaleTrackingModel.Status).ToList();
            }

            var listall = ConvertSaleTrackingModel(result);

            rptCriteria = string.Format(rptCriteria, searchSaleTrackingModel.DateFrom, searchSaleTrackingModel.DateTo);
            rptName = string.Format(rptName, "Sale Tracking Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSaleTrackingExcelName("SaleTrackingReport");


            var bytes = GenerateSaleTrackingEntitytoExcel<SaleTrackingExportList>(listall, filename, "FBBREPORT_SALE_1");

            return File(bytes, "application/excel", filename + ".xls");

        }

        // [ExportExcel] Get Export Sale Tracking Report
        public List<SaleTrackingList> GetExportSaleTrackingReport(SaleTrackingModel searchSaleTrackingModel)
        {
            _Logger.Info("GetExportSaleTrackingReport start");
            try
            {
                _Logger.Info("GetExportSaleTrackingReport try");

                var query = GetSaleTrackingSearchModel(searchSaleTrackingModel);

                var dataout = _queryProcessor.Execute(query);

                var result = GetOrders(dataout);

                if (searchSaleTrackingModel.Status != "All")
                {
                    result = result.Where(t => t.current_state == searchSaleTrackingModel.Status).ToList();
                }

                return result;

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                _Logger.Info("Error when call GetExportSaleTrackingReport");
                return new List<SaleTrackingList>();
            }

        }

        // [ExportExcel] Convert SaleTrackingList To SaleTrackingExportList
        private List<SaleTrackingExportList> ConvertSaleTrackingModel(List<SaleTrackingList> list)
        {
            var dataexportlist = list.Select(x => new SaleTrackingExportList()
            {
                location_code = x.location_code.ToSafeString(),
                asc_code = x.asc_code.ToSafeString(),
                employee_id = x.employee_id.ToSafeString(),
                register_date = x.register_date_str,
                customer_name = x.customer_name.ToSafeString(),
                building_village = x.building_village.ToSafeString(),
                sub_district = x.sub_district.ToSafeString(),
                district = x.district.ToSafeString(),
                province = x.province.ToSafeString(),
                fibrenet_id = x.fibrenet_id.ToSafeString(),
                technology = x.technology.ToSafeString(),
                playbox_flag = x.playbox_flag.ToSafeString(),
                fixedline_flag = x.fixedline_flag.ToSafeString(),
                current_state = x.current_state.ToSafeString(),
                status_date = x.status_date.ToSafeString()
            });

            return dataexportlist.ToList();
        }

        // [ExportExcel] Get Excel File Name
        private string GetSaleTrackingExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateSaleTrackingEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateSaleTrackingEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props =
            System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = base.LovData.Where(p => p.Type == "SCREEN" && p.LovValue5 == LovValue5 && p.Name.StartsWith("C_")).ToList();

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

            var data_ = GenerateSaleTrackingExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateSaleTrackingExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateSaleTrackingExcel start");
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

        #endregion
    }
}
