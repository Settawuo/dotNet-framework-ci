using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Script.Serialization;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBHVR;
using WBBContract.Queries.FBBShareplex;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels;
using WBBEntity.PanelModels.ShareplexModels;

namespace LowUtilizeSaleReport
{
    public class LowUtilizeSaleReportExport
    {
        #region Properties

        private readonly ILogger _Logger;
        private readonly IQueryProcessor _queryProcessor;

        private string rptName = "Report Name : {0}";
        private string rptCriteria = "Date From : {0}  To : {1}";
        private string rptDate = "Run Report Date/Time : {0}";

        #endregion

        #region Constructor

        public LowUtilizeSaleReportExport(ILogger logger, IQueryProcessor queryProcessor)
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
        }

        #endregion

        #region Public Method

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

        public List<LowUtilizeSaleReportList> GetLowUtilizeSaleReport(LowUtilizeSaleReportModel lowUtilizeSaleReportModel)
        {
            _Logger.Info("GetLowUtilizeSaleReport Start");
            try
            {
                _Logger.Info("GetLowUtilizeSaleReport Try");
                List<LowUtilizeSaleReportList> result = new List<LowUtilizeSaleReportList>();

                // R24.07 Edit DB Shareplex to HVR
                var flagHVR = GetFlagHVR(); // get config flag shareplex or hvr

                if (flagHVR == "Y")
                {
                    _Logger.Info("Connect HVR FLAG");

                    var query = new GetLowUtilizeSaleReportHVRQuery()
                    {
                        p_location_code = lowUtilizeSaleReportModel.LocationCode.ToString()
                    };
                    result = _queryProcessor.Execute(query);
                }
                else
                {
                    _Logger.Info("Connect SharePlex FLAG");

                    var query = new GetLowUtilizeSaleReportQuery()
                    {
                        p_location_code = lowUtilizeSaleReportModel.LocationCode.ToString()
                    };
                    result = _queryProcessor.Execute(query);
                }

                return result;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error when call GetLowUtilizeSaleReport");
                _Logger.Info(ex.GetErrorMessage());
                return new List<LowUtilizeSaleReportList>(); ;
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

        #region Private Method



        #endregion

        #region Export Excel

        // [ExportExcel]
        public void ExportLowUtilizeSaleData(string dataS, string processName)
        {
            _Logger.Info("ExportLowUtilizeSaleData start");

            var lowUtilizeSaleReportModel = new JavaScriptSerializer().Deserialize<LowUtilizeSaleReportModel>(dataS);

            var dataout = GetLowUtilizeSaleReport(lowUtilizeSaleReportModel);

            if (dataout.Count > 0)
            {
                Console.WriteLine("dataout-location_code: " + dataout[0].location_code);

                var listall = ConvertLowUtilizeSaleReportModel(dataout);

                rptCriteria = string.Format(rptCriteria, lowUtilizeSaleReportModel.DateFrom, lowUtilizeSaleReportModel.DateTo);
                rptName = string.Format(rptName, "Low Utilize Sale Tracking Report");
                rptDate = string.Format(rptDate, DateTime.Now.ToDisplayText());

                string filename = GetLowUtilizeSaleReportExcelName("LowUtilizeSaleReport");

                var bytes = GenerateLowUtilizeSaleReportEntitytoExcel<LowUtilizeSaleReportExportList>(listall, filename, processName);

                // SendMail
                try
                {
                    _Logger.Info("SendMail start : ProcessName " + processName);
                    LovValueModel subjectEmail = GetLovList("EMAIL_BATCH_LOW_UTILIZE_SALE_RPT", "SUBJECT").FirstOrDefault();
                    LovValueModel contentEmail = GetLovList("EMAIL_BATCH_LOW_UTILIZE_SALE_RPT", "CONTENT").FirstOrDefault();

                    var emailData = GetEmailProcessing(processName, "B_LOW_U");

                    SendMail(subjectEmail.LovValue1, contentEmail.LovValue1, bytes, filename + ".xls", emailData.SEND_FROM, emailData.SEND_TO, emailData.SEND_CC, emailData.IP_MAIL_SERVER);
                    _Logger.Info("SendMail End");
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot send the e-mail. Error: " + ex.Message);
                }

            }
            else
            {
                _Logger.Info("Location Code Data not found.");
            }
        }

        // [ExportExcel] Convert LowUtilizeSaleReportList To LowUtilizeSaleReportExportList
        private List<LowUtilizeSaleReportExportList> ConvertLowUtilizeSaleReportModel(List<LowUtilizeSaleReportList> list)
        {
            var dataexportlist = list.Select(x => new LowUtilizeSaleReportExportList()
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
        private string GetLowUtilizeSaleReportExcelName(string fileName)
        {
            string result = string.Empty;

            DateTime currDateTime = DateTime.Now;

            string timeNow = currDateTime.ToString("HHmmss");
            string dateNow = currDateTime.ToString("yyyyMMdd");

            result = string.Format("{0}_{1}_{2}", fileName, dateNow, timeNow);

            return result;
        }

        // [ExportExcel] Generate Entity
        public byte[] GenerateLowUtilizeSaleReportEntitytoExcel<T>(List<T> data, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateLowUtilizeSaleReportEntitytoExcel start");
            System.ComponentModel.PropertyDescriptorCollection props = System.ComponentModel.TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            var lovDataScreen = GetLovList("SCREEN", "", LovValue5).Where(p => p.Name.StartsWith("C%")).OrderBy(o => o.OrderBy).ToList();

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

            var data_ = GenerateLowUtilizeSaleReportExcel(table, "WorkSheet", tempPath, fileName, LovValue5);
            return data_;
        }

        // [ExportExcel] Generate Excel
        private byte[] GenerateLowUtilizeSaleReportExcel(DataTable dataToExcel, string excelSheetName, string directoryPath, string fileName, string LovValue5)
        {
            _Logger.Info("GenerateLowUtilizeSaleReportExcel start");
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

        private string GetFlagHVR()
        {
            var query = new GetLovQuery()
            {
                LovType = "FBB_CONSTANT",
                LovName = "HVR_USE_FLAG"
            };
            var _FbbCfgLov = _queryProcessor.Execute(query);

            string flagHVR = (_FbbCfgLov != null && _FbbCfgLov.Any())
                 ? _FbbCfgLov.FirstOrDefault()?.LovValue1 ?? "N"
                 : "N";
            return flagHVR;
        }
    }
}
