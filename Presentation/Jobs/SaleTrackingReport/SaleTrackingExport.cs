using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.WebServices;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBEntity.PanelModels.WebServiceModels;

namespace SaleTrackingReport
{
    public class SaleTrackingExport
    {
        #region Properties

        private readonly ILogger _Logger;
        private readonly IQueryProcessor _queryProcessor;

        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run Report Date/Time : {0}";

        #endregion

        #region Constructor

        public SaleTrackingExport(ILogger logger, IQueryProcessor queryProcessor)
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region Private Methods

        // Get Order
        // เมื่อมีการแก้ไข Logic กรุณาแก้ไขที่ WBBWeb.Controllers => TrackingController Method GetOrders ใน #region Logic GetOrders ด้วย
        private List<SaleTrackingReportList> GetOrders(List<AIRNETEntity.StoredProc.TrackingReportModel> list)
        {
            try
            {
                var data = list.Select(x => new SaleTrackingReportList()
                {
                    location_code = x.location_code.ToSafeString(),
                    asc_code = x.asc_code.ToSafeString(),
                    registered_date = x.registered_date.ToSafeString(),
                    appointment_date = x.appointment_date.ToSafeString(),
                    time_slot = x.time_slot.ToSafeString(),
                    customer_name = x.customer_name.ToSafeString(),
                    mobile_no = x.mobile_no.ToSafeString(),
                    telephone_no = x.telephone_no.ToSafeString(),
                    work_no = x.work_no.ToSafeString(),
                    fax_no = x.fax_no.ToSafeString(),
                    email = x.email.ToSafeString(),
                    address_id = x.address_id.ToSafeString(),
                    building_name_th = x.building_name_th.ToSafeString(),
                    building_name_en = x.building_name_en.ToSafeString(),
                    room_no = x.room_no.ToSafeString(),
                    floor_no = x.floor_no.ToSafeString(),
                    home_no = x.home_no.ToSafeString(),
                    moo = x.moo.ToSafeString(),
                    room = x.room.ToSafeString(),
                    soi = x.soi.ToSafeString(),
                    street = x.street.ToSafeString(),
                    sub_district = x.sub_district.ToSafeString(),
                    district = x.district.ToSafeString(),
                    province = x.province.ToSafeString(),
                    internet_no = x.internet_no.ToSafeString(),
                    install_date = x.install_date.ToSafeString(),
                    main_package = x.main_package.ToSafeString(),
                    speed = x.speed.ToSafeString(),
                    promotion_code_main = x.promotion_code_main.ToSafeString(),
                    price_fee_main = x.price_fee_main.ToSafeString(),
                    price_discount = x.price_discount.ToSafeString(),
                    promotion_code_ontop = x.promotion_code_ontop.ToSafeString(),
                    ontop_package = x.ontop_package.ToSafeString(),
                    price_fee_ontop = x.price_fee_ontop.ToSafeString(),
                    playbox_flag = x.playbox_flag.ToSafeString(),
                    fixedline_flag = x.fixedline_flag.ToSafeString(),
                    status = x.status.ToSafeString(),
                    status_date = x.status_date.ToSafeString(),
                    cs_note = x.cs_note.ToSafeString(),
                    cancel_reason = x.cancel_reason.ToSafeString(),
                    air_order_no = x.air_order_no.ToSafeString(),
                    order_type = x.order_type.ToSafeString(),
                    remark = x.remark.ToSafeString(),
                    fibrenet_id = x.fibrenet_id.ToSafeString(),
                    order_type_zte = x.order_type_zte.ToSafeString(),
                    start_date = x.start_date.ToSafeString(),
                    end_date = x.end_date.ToSafeString(),
                    event_flag = x.event_flag.ToSafeString()
                });

                List<SaleTrackingReportList> dataout = data.ToList();

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
                        var result = FBSSQueryOrder(FIBRENetID_List, tmp.order_type_zte);

                        tmp.status = "No_Order";

                        if (result.Order_Details_List != null && result.Order_Details_List.Any())
                        {

                            string lastestStatus = "Wait_for_Appointment";
                            int lastestDate = 0;
                            int lastestTime = 0;
                            string lastestDateTime = "";

                            tmp.status = "Wait_for_Appointment";

                            string transaction_state = result.Order_Details_List.FirstOrDefault().TRANSACTION_STATE;

                            if (result.Order_Details_List[0].ACTIVITY_DETAILS != null)
                            {
                                var Appointment_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Appointment");
                                if (Appointment_State_Info.Any())
                                {
                                    tmp.status = "Appointment";

                                    if (transaction_state == "Cancelled")
                                    {
                                        tmp.status = "Cancel Appointment";
                                    }
                                    //int count = 1;
                                    foreach (var info_tmp in Appointment_State_Info)
                                    {
                                        try
                                        {
                                            Console.WriteLine("info_tmp.APPOINTMENT_DATE: " + info_tmp.APPOINTMENT_DATE);
                                            if (string.IsNullOrEmpty(info_tmp.APPOINTMENT_DATE) && string.IsNullOrEmpty(info_tmp.APPOINTMENT_TIMESLOT))
                                            {
                                                tmp.appointment_date = tmp.appointment_date.ToSafeString();
                                                tmp.time_slot = tmp.time_slot.ToSafeString();
                                            }
                                            else
                                            {
                                                tmp.appointment_date = info_tmp.APPOINTMENT_DATE;
                                                tmp.time_slot = info_tmp.APPOINTMENT_TIMESLOT;
                                            }

                                            //PropertyInfo propertyInfo = tmp.GetType().GetProperty("appointment_date_" + count + "_str");
                                            //propertyInfo.SetValue(tmp, Convert.ChangeType(info_tmp.APPOINTMENT_DATE, propertyInfo.PropertyType), null);

                                            //Console.WriteLine("info_tmp.APPOINTMENT_DATE: " + info_tmp.APPOINTMENT_DATE);
                                            //tmp.appointment_date = info_tmp.APPOINTMENT_DATE;

                                            //propertyInfo = tmp.GetType().GetProperty("appointment_timeslot_" + count + "_str");
                                            //propertyInfo.SetValue(tmp, Convert.ChangeType(info_tmp.APPOINTMENT_TIMESLOT, propertyInfo.PropertyType), null);
                                            //count++;
                                        }
                                        catch (Exception)
                                        {
                                            Console.WriteLine("Exception");
                                            continue;
                                        }
                                    }
                                }

                                var Install_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "Install");
                                if (Install_State_Info.Any())
                                {
                                    tmp.status = "Installation";
                                    var complete_state = Install_State_Info.Where(x => string.IsNullOrEmpty(x.FOA_REJECT_REASON));
                                    if (complete_state.Any())
                                    {
                                        DateTime CompleteInstallDate;
                                        tmp.install_date = DateTime.TryParseExact(complete_state.FirstOrDefault().COMPLETED_DATE, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out CompleteInstallDate) ? CompleteInstallDate.ToString("dd/MM/yyyy") : tmp.install_date.ToSafeString();

                                        //tmp.complete_install_date_str = complete_state.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault().COMPLETED_DATE;
                                    }
                                    else
                                    {
                                        var newest_state = Install_State_Info.OrderByDescending(x => x.COMPLETED_DATE).FirstOrDefault();
                                        DateTime CompleteInstallDate;
                                        tmp.install_date = DateTime.TryParseExact(newest_state.COMPLETED_DATE, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out CompleteInstallDate) ? CompleteInstallDate.ToString("dd/MM/yyyy") : tmp.install_date.ToSafeString();

                                        if (transaction_state == "Cancelled")
                                        {
                                            tmp.status = "Cancel Installation";
                                            tmp.cancel_reason = newest_state.FOA_REJECT_REASON;
                                            tmp.install_date = newest_state.COMPLETED_DATE;
                                        }
                                        //tmp.complete_install_date_str = newest_state.COMPLETED_DATE;
                                    }
                                }

                                var Complete_State_Info = result.Order_Details_List[0].ACTIVITY_DETAILS.Where(x => x.ACTIVITY == "SFF");
                                if (Complete_State_Info.Any())
                                {
                                    tmp.status = "On Service";
                                    var newest_state = Complete_State_Info.FirstOrDefault().COMPLETED_DATE;
                                    DateTime CompleteInstallDate;
                                    tmp.install_date = DateTime.TryParseExact(newest_state, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"), DateTimeStyles.None, out CompleteInstallDate) ? CompleteInstallDate.ToString("dd/MM/yyyy") : tmp.install_date.ToSafeString();

                                    //tmp.onservice_date = Complete_State_Info.FirstOrDefault().COMPLETED_DATE;
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

                                if (lastestStatus == "Wait_for_Appointment" || (lastestDate < thisDate || (lastestDate == thisDate && lastestTime < thisTime)))
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
                            tmp.status = lastestStatus;
                            tmp.status_date = lastestDateTime;

                            if (lastestStatus != "Installation")
                            {
                                tmp.install_date = "";
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

            return new List<SaleTrackingReportList>();
        }

        public List<LovValueModel> GetLovList(string type, string name = "", string value5 = "")
        {
            List<LovValueModel> lovDatas = new List<LovValueModel>();
            try
            {
                if (value5 == "")
                {

                    var query = new GetLovQuery
                    {
                        LovType = type,
                        LovName = name
                    };
                    lovDatas = _queryProcessor.Execute(query);

                }
                else
                {
                    var query = new SelectLovByTypeAndLovVal5Query
                    {
                        LOV_TYPE = type,
                        LOV_VAL5 = value5
                    };
                    var lov = _queryProcessor.Execute(query);
                    if (lov != null && lov.Count > 0)
                    {
                        foreach (var item in lov)
                        {
                            LovValueModel lovData = new LovValueModel
                            {
                                LovValue1 = item.LOV_VAL1
                            };
                            lovDatas.Add(lovData);
                        }
                    }
                }

                return lovDatas;
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetLovList Error: " + ex.Message);
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        public FBB_EMAIL_PROCESSING GetEmailProcessing(string processName, string createBy = "")
        {
            try
            {
                var query = new GetEmailProcessingQuery
                {
                    CreateBy = createBy,
                    ProcessName = processName
                };

                var emailData = _queryProcessor.Execute(query);
                return emailData;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new FBB_EMAIL_PROCESSING();
            }
        }

        #endregion

        #region Public Methods

        // Get SaleTrackingSearchModel
        public GetTrackingReportQuery GetSaleTrackingSearchModel(SaleTrackingModel searchSaleTrackingModel)
        {
            _Logger.Info("GetSaleTrackingSearchModel Start");
            try
            {
                _Logger.Info("GetSaleTrackingSearchModel Try");
                var query = new GetTrackingReportQuery()
                {
                    P_Id_Card = "",
                    P_First_Name = "",
                    P_Last_Name = "",
                    P_Location_Code = searchSaleTrackingModel.LocCode.ToString(),
                    P_Asc_Code = "",
                    P_Date_From = searchSaleTrackingModel.DateFrom.ToSafeString(),
                    P_Date_To = searchSaleTrackingModel.DateTo.ToSafeString(),
                    P_Cust_Name = "",
                    P_User = "FBB_BATCH"
                };
                //FBB_ADMIN
                return query;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error when call GetSaleTrackingSearchModel");
                _Logger.Info(ex.GetErrorMessage());
                return new GetTrackingReportQuery();
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
        public void ExportSaleTrackingData(string dataS, string processName)
        {
            var searchSaleTrackingModel = new JavaScriptSerializer().Deserialize<SaleTrackingModel>(dataS);

            var query = GetSaleTrackingSearchModel(searchSaleTrackingModel);

            var dataout = _queryProcessor.Execute(query);

            Console.WriteLine();
            Console.WriteLine("dataout-registered_date: " + dataout[0].registered_date);

            var result = GetOrders(dataout);

            Console.WriteLine("result-registered_date: " + result[0].registered_date);

            //if (searchSaleTrackingModel.Status != "All")
            //{
            //    result = result.Where(t => t.status == searchSaleTrackingModel.Status).ToList();
            //}

            var listall = ConvertSaleTrackingModel(result);

            Console.WriteLine("listall-registered_date: " + listall[0].registered_date);
            Console.WriteLine();

            rptCriteria = string.Format(rptCriteria, searchSaleTrackingModel.DateFrom, searchSaleTrackingModel.DateTo);
            rptName = string.Format(rptName, "Sale Tracking Report");
            rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

            string filename = GetSaleTrackingExcelName("SaleTrackingReport");


            var bytes = GenerateSaleTrackingEntitytoExcel<SaleTrackingReportExportList>(listall, filename, "BatchSaleTracking");

            //var zipFileName = filename.Replace(".xlsx", ".zip");
            //using (var compressedFileStream = new MemoryStream())
            ////Create an archive and store the stream in memory.
            //using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
            //{
            //    //Create a zip entry for each attachment
            //    var zipEntry = zipArchive.CreateEntry(filename);

            //    //Get the stream of the attachment
            //    using (var originalFileStream = new MemoryStream(bytes))
            //    using (var zipEntryStream = zipEntry.Open())
            //    {
            //        //Copy the attachment stream to the zip entry stream
            //        originalFileStream.CopyTo(zipEntryStream);
            //    }

            //    //return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = zipFileName + ".zip" };
            //}

            //SendMail
            try
            {

                LovValueModel subjectEmail = GetLovList("EMAIL_BATCH_SALE_TRACKING", "SUBJECT").FirstOrDefault();
                LovValueModel contentEmail = GetLovList("EMAIL_BATCH_SALE_TRACKING", "CONTENT").FirstOrDefault();

                var emailData = GetEmailProcessing(processName, "B_SALETRAC");

                SendMail(subjectEmail.LovValue1, contentEmail.LovValue1, bytes, filename + ".xls", emailData.SEND_FROM, emailData.SEND_TO, emailData.SEND_CC, emailData.IP_MAIL_SERVER);
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot send the e-mail. Error: " + ex.Message);

            }

            //WriteFile
            //using (FileStream stream = new FileStream(System.Environment.CurrentDirectory + "/File/" + filename + ".xls", FileMode.Create, FileAccess.Write, FileShare.Read))
            //{
            //    stream.Write(bytes, 0, bytes.Length);
            //}

            //return File(bytes, "application/excel", filename + ".xls");

        }

        // [ExportExcel] Get Export Sale Tracking Report
        public List<SaleTrackingReportList> GetExportSaleTrackingReport(SaleTrackingModel searchSaleTrackingModel)
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
                    result = result.Where(t => t.status == searchSaleTrackingModel.Status).ToList();
                }

                return result;

            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                _Logger.Info("Error when call GetExportSaleTrackingReport");
                return new List<SaleTrackingReportList>();
            }

        }

        // [ExportExcel] Convert SaleTrackingList To SaleTrackingExportList
        private List<SaleTrackingReportExportList> ConvertSaleTrackingModel(List<SaleTrackingReportList> list)
        {
            var dataexportlist = list.Select(x => new SaleTrackingReportExportList()
            {
                location_code = x.location_code.ToSafeString(),
                asc_code = x.asc_code.ToSafeString(),
                registered_date = x.registered_date.ToSafeString(),
                appointment_date = x.appointment_date.ToSafeString(),
                time_slot = x.time_slot.ToSafeString(),
                customer_name = x.customer_name.ToSafeString(),
                mobile_no = x.mobile_no.ToSafeString(),
                telephone_no = x.telephone_no.ToSafeString(),
                work_no = x.work_no.ToSafeString(),
                fax_no = x.fax_no.ToSafeString(),
                email = x.email.ToSafeString(),
                address_id = x.address_id.ToSafeString(),
                building_name_th = x.building_name_th.ToSafeString(),
                building_name_en = x.building_name_en.ToSafeString(),
                room_no = x.room_no.ToSafeString(),
                floor_no = x.floor_no.ToSafeString(),
                home_no = x.home_no.ToSafeString(),
                moo = x.moo.ToSafeString(),
                room = x.room.ToSafeString(),
                soi = x.soi.ToSafeString(),
                street = x.street.ToSafeString(),
                sub_district = x.sub_district.ToSafeString(),
                district = x.district.ToSafeString(),
                province = x.province.ToSafeString(),
                internet_no = x.internet_no.ToSafeString(),
                install_date = x.install_date.ToSafeString(),
                main_package = x.main_package.ToSafeString(),
                speed = x.speed.ToSafeString(),
                promotion_code_main = x.promotion_code_main.ToSafeString(),
                price_fee_main = x.price_fee_main.ToSafeString(),
                price_discount = x.price_discount.ToSafeString(),
                promotion_code_ontop = x.promotion_code_ontop.ToSafeString(),
                ontop_package = x.ontop_package.ToSafeString(),
                price_fee_ontop = x.price_fee_ontop.ToSafeString(),
                playbox_flag = x.playbox_flag.ToSafeString(),
                fixedline_flag = x.fixedline_flag.ToSafeString(),
                status = x.status.ToSafeString(),
                status_date = x.status_date.ToSafeString(),
                cs_note = x.cs_note.ToSafeString(),
                cancel_reason = x.cancel_reason.ToSafeString(),
                air_order_no = x.air_order_no.ToSafeString(),
                order_type = x.order_type.ToSafeString(),
                remark = x.remark.ToSafeString(),
                event_flag = x.event_flag.ToSafeString()
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

            var lovDataScreen = GetLovList("SCREEN", "", LovValue5);

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
                    if (props[i].Name == "registered_date")
                        values[i] = values[i].ToString().Substring(0, 10);
                    if ((props[i].Name == "appointment_date" || props[i].Name == "install_date") && values[i].ToString() == "")
                        values[i] = "";
                    else if ((props[i].Name == "appointment_date" || props[i].Name == "install_date") && values[i].ToString() != "")
                        values[i] = values[i].ToString().Substring(0, 10);
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
                rangeReportDetail.Style.Font.Color.SetColor(System.Drawing.ColorTranslator.FromHtml("#17324E")); //#17324E

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

        #region SendMail

        private static void SendMail(string subject, string content, byte[] File, string fileName, string mailFrom, string mailTo, string mailCc, string ipMailServer)
        {
            try
            {
                //TODO: Send Mail
                var mailContent = content;
                var fromAddress = new MailAddress(mailFrom);
                var emailServerSplitValue = !string.IsNullOrEmpty(ipMailServer) ? ipMailServer.Split('|') : null;

                var fromPass = "V9!@M#V2zf@Q";// Fixed Code scan : var fromPassword = "V9!@M#V2zf@Q";
                var host = "10.252.160.41";
                var port = "25";
                var domain = "corp.ais900dev.org";

                if (emailServerSplitValue != null && emailServerSplitValue.Length > 0)
                {
                    host = emailServerSplitValue[0];
                    port = emailServerSplitValue[1];
                    domain = emailServerSplitValue[2];
                    fromPass = emailServerSplitValue[3];
                }

                var smtp = new SmtpClient
                {
                    Host = host,
                    Port = Convert.ToInt32(port),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = true,
                    Credentials = new NetworkCredential(fromAddress.User, fromPass, domain),
                };

                var message = new MailMessage { From = fromAddress };
                var mailToArray = mailTo.Split(';');
                var mailCCArray = mailCc.Split(';');

                foreach (string mail in mailToArray)
                    message.To.Add(mail);
                if (!string.IsNullOrEmpty(mailCc))
                {
                    foreach (string mailCC in mailCCArray)
                        message.CC.Add(mailCC);
                }
                message.IsBodyHtml = true;
                message.Subject = subject;
                message.Body = mailContent;
                message.Priority = GetMailPriority(string.Empty);

                message.Attachments.Add(new Attachment(new MemoryStream(File), fileName));
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static MailPriority GetMailPriority(string importance)
        {
            var priority = new MailPriority();

            if (importance == "Low")
            {
                priority = MailPriority.Low;
            }
            else if (importance == "Normal")
            {
                priority = MailPriority.Normal;
            }
            else if (importance == "High")
            {
                priority = MailPriority.High;
            }

            return priority;
        }

        #endregion

    }
}
