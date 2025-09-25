using Excel.Log;
using FBBConfig.Extensions;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBConfig.Controllers
{


    public partial class
        ReportInstallationCostbyOrderController
    {

        #region ReportInstallationBySendMail
        public async Task<string> SendMailUpdateOrderStatus(List<ReportInstallationCostbyOrderListModel_Binding> data
             , List<reportInstallScmOrderListModelBySendMail> OrderListModelBySendMail
           )
        {
            string result = "";
            try

            {
                List<ReportInstallation_FixOrderListTmp> FixdeOrderList = new List<ReportInstallation_FixOrderListTmp>();
                ReportInstallation_FixOrderListTmp OrderListModel = new ReportInstallation_FixOrderListTmp();
                string CS_APPROVE_DATE_FROM = OrderListModelBySendMail.FirstOrDefault().CS_APPROVE_DATE_FROM;
                string CS_APPROVE_DATE_TO = OrderListModelBySendMail.FirstOrDefault().CS_APPROVE_DATE_TO;

                foreach (var resule in data)
                {
                    OrderListModel = new ReportInstallation_FixOrderListTmp();
                    OrderListModel.ACCESS_NUMBER = resule.ACCESS_NUMBER.ToSafeString();
                    FixdeOrderList.Add(OrderListModel);
                }
                var SendExpComand = new SendMailReportInstallationCommand
                {
                    p_USER = CurrentUser.UserName,
                    p_period_from = CS_APPROVE_DATE_FROM,
                    p_period_to = CS_APPROVE_DATE_TO,
                    fixed_order = FixdeOrderList,

                };
                _SendReportInstallationExp.Handle(SendExpComand);
                if (SendExpComand.ret_code == 1)
                {
                    result = "Update not success";
                }
                else
                {
                    result = SendExpComand.ret_code.ToString();
                }

            }
            catch (Exception ex)
            {
                _Logger.Info("Sending an Email- Cannot Update Period " + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                result = "Update not success";
            }
            return result;

        }
        public string ConvertDatePeriod(string FromDtm, string ToDtm)
        {
            try
            {
                CultureInfo ThaiCulture = new CultureInfo("th-TH");
                string PERIOD_DATE_FROM = "";
                string PERIOD_DATE_TO = "";

                if (!string.IsNullOrEmpty(FromDtm))
                {
                    var DtmForm = DateTime.ParseExact(FromDtm, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    PERIOD_DATE_FROM = DtmForm.ToString("dd MMM yy", ThaiCulture);
                }

                if (!string.IsNullOrEmpty(ToDtm))
                {
                    var DtmTo = DateTime.ParseExact(ToDtm, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    PERIOD_DATE_TO = DtmTo.ToString("dd MMM yy", ThaiCulture);
                }

                return PERIOD_DATE_FROM + " - " + PERIOD_DATE_TO;
            }
            catch (Exception ex)
            {
                _Logger.Info("Erro ConvertDatePERIOD : ");
                return ex.Message.ToString();
            }

        }


        public async Task SendEmail(List<ReportInstallation_FBB_access_list> AccNOList, string dataS)
        {
            try
            {
                string ACCNO = string.Empty;
                string checkMax = AccNOList.FirstOrDefault().ACCESS_NUMBER.ToSafeString();
                var query = new GetLovV2Query()
                {
                    LovType = "WF_STATUS_REPORT_INSTALLATION",
                    LovVal3 = "SEND_MAIL",
                    LovVal5 = "REPORT_INSTALLATION"

                };
                var LovWorkStatus = _queryProcessor.Execute(query).ToList();
                //var LovWorkStatus = LovData.Where(l => l.LovValue3 == "SEND_MAIL" && l.LovValue5 == "REPORT_INSTALLATION").ToList();
                string LovCompleted = LovWorkStatus.Where(c => c.OrderBy == 1).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
                string LovNodata = LovWorkStatus.Where(c => c.OrderBy == 2).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
                string LovRecheck = LovWorkStatus.Where(c => c.OrderBy == 3).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();

                ReportInstallationCostbyOrderModel searchModel = new ReportInstallationCostbyOrderModel();
                searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");

                //  "Complete", "Re Check", "No Data" ;

                string[] WorkStatus = new string[LovWorkStatus.Count];

                for (int i = 0; i < LovWorkStatus.Count; i++)
                {
                    WorkStatus[i] = LovWorkStatus[i].LovValue1;
                }


                if (searchModel.ORDER_STATUS == null)
                {
                    searchModel.ORDER_STATUS = WorkStatus.ToArray();
                }

                ReportInstallationCostbyOrderListReturn result = await getAllRecord();
                List<ReportInstallationCostbyOrderListModel_Binding> SearchResults = (List<ReportInstallationCostbyOrderListModel_Binding>)result.cur;
                if (checkMax != "MAX-ACCNO")
                {
                    SearchResults = SearchResults.Where(x => AccNOList.Select(m => m.ACCESS_NUMBER).Contains(x.ACCESS_NUMBER)).ToList();
                }
                //executeResults.result.cur.RemoveAll(
                //  item => string.IsNullOrWhiteSpace(item.INTERNET_NO)
                //        || string.IsNullOrWhiteSpace(item.MAIN_ASSET));
                var DataModel = new JavaScriptSerializer().Deserialize<List<reportInstallScmOrderListModelBySendMail>>(dataS);


                string CS_APPROVE_DATE_FROM = DataModel.FirstOrDefault().CS_APPROVE_DATE_FROM;
                string CS_APPROVE_DATE_TO = DataModel.FirstOrDefault().CS_APPROVE_DATE_TO;
                string STRING_CS_APPROVE_DATE = DataModel.FirstOrDefault().STRING_CS_APPROVE_DATE;
                string CS_DOC_DATE = DataModel.FirstOrDefault().CS_DOC_DATE;
                string CS_PAYMENT_DATE = DataModel.FirstOrDefault().CS_PAYMENT_DATE;
                string CS_REMARK = DataModel.FirstOrDefault().REMARK;

                for (int i = 0; i < SearchResults.Count; i++)
                {
                    if (!string.IsNullOrEmpty(SearchResults[i].ORDER_STATUS))
                    {
                        SearchResults[i].ORDER_STATUS = SearchResults[i].ORDER_STATUS.ToUpper();
                    }
                    else if (!string.IsNullOrEmpty(SearchResults[i].WORK_STATUS))
                    {
                        SearchResults[i].ORDER_STATUS = SearchResults[i].WORK_STATUS.ToUpper();
                    }
                }

                var DataQueryNotNull = SearchResults.Where(
                        // s => s.ORDER_STATUS != null
                        s => s.ORDER_STATUS == LovCompleted
                           || s.ORDER_STATUS == LovRecheck
                           || s.ORDER_STATUS == LovNodata
                       ).ToList();


                //var DataQuery = DataQueryNotNull.Where(
                //   s => s.ORDER_STATUS == "COMPLETE"
                //       || s.ORDER_STATUS == "RE CHECK"
                //       || s.ORDER_STATUS == "NO DATA"
                //    ).ToList();

                if (DataQueryNotNull != null)
                {
                    if (DataQueryNotNull.Count() <= 0)
                    {
                        //   return "NODATA";
                    }
                }

                foreach (var datupd in DataQueryNotNull)
                {
                    //string[] SplitName = datupd.ACCOUNT_NAME.Split(' ');
                    if (string.IsNullOrEmpty(datupd.PERIOD))
                    {
                        datupd.PERIOD = STRING_CS_APPROVE_DATE;
                    }

                    //datupd.ACCOUNT_NAME = SplitName[0].ToSafeString() + " XXX".ToSafeString();

                }

                //update preriod and order status    
                var updatePeriod = SendMailUpdateOrderStatus(DataQueryNotNull, DataModel);

                updatePeriod.Wait();

                if (DataQueryNotNull.Count > 0)
                {
                    // await Task.Run(() => );
                    var resultSendMail = CreateDataToDataTable(DataQueryNotNull
                             , CS_APPROVE_DATE_FROM
                             , CS_APPROVE_DATE_TO
                             , STRING_CS_APPROVE_DATE
                             , CS_DOC_DATE
                             , CS_PAYMENT_DATE
                             , CS_REMARK);
                }


            }
            catch (Exception ex)
            {
                _Logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                //  return "NotSuccess";

            }
            // return "Success";
            // return "Success";

        }


        public string EmailTemplate(string SUBCONTRACT_NAME, string STRING_CS_APPROVE_DATE, string[] str, string CS_DOC_DATE, string CS_PAYMENT_DATE, string CS_REMARK)
        {

            try
            {
                var query = new GetLovV2Query()
                {
                    LovType = "MAIL_CONFIG_REPORT_INSTALLATION",
                    LovVal5 = "REPORT_INSTALLATION"

                };
                var impersonateVar = _queryProcessor.Execute(query).ToList();
                //var impersonateVar = LovData.Where(l => l.Type == "MAIL_CONFIG_REPORT_INSTALLATION" && l.LovValue5 == "REPORT_INSTALLATION").ToList();
                string COMP_NAME = Regex.Replace(impersonateVar.SingleOrDefault(l => l.Name == "COMP_NAME").Text.ToSafeString(), @"\r\n?|\n", "<br />");
                string COMP_ADDR = Regex.Replace(impersonateVar.SingleOrDefault(l => l.Name == "COMP_ADDR").Text.ToSafeString(), @"\r\n?|\n", "<br />");

                string SCM_NAME = Regex.Replace(impersonateVar.SingleOrDefault(l => l.Name == "SCM_NAME").Text.ToSafeString(), @"\r\n?|\n", "<br />");
                string SCM_ADDR = Regex.Replace(impersonateVar.SingleOrDefault(l => l.Name == "SCM_ADDR").Text.ToSafeString(), @"\r\n?|\n", "<br />");
                string SCM_MAIL = Regex.Replace(impersonateVar.SingleOrDefault(l => l.Name == "SCM_MAIL").Text.ToSafeString(), @"\r\n?|\n", "<br />");

                string SCM_CONTACT = Regex.Replace(impersonateVar.SingleOrDefault(l => l.Name == "SCM_CONTACT").Text.ToSafeString(), @"\r\n?|\n", "<br />");

                string[] words = null;
                if (!string.IsNullOrEmpty(SCM_CONTACT))
                {
                    words = SCM_CONTACT.Split(';');
                }



                StringBuilder tempBody = new StringBuilder();
                CultureInfo ThaiCulture = new CultureInfo("th-TH");
                CultureInfo UsaCulture = new CultureInfo("en-US");

                if (!string.IsNullOrEmpty(CS_DOC_DATE))
                {
                    var DtNowCs = DateTime.ParseExact(CS_DOC_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    CS_DOC_DATE = DtNowCs.ToString("dddd dd MMMM ", ThaiCulture) + DtNowCs.ToString("yyyy ", UsaCulture);
                }

                if (!string.IsNullOrEmpty(CS_PAYMENT_DATE))
                {
                    var DtNowPayment = DateTime.ParseExact(CS_PAYMENT_DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    CS_PAYMENT_DATE = DtNowPayment.ToString("dddd dd MMMM ", ThaiCulture) + DtNowPayment.ToString("yyyy ", UsaCulture);
                }

                #region tempBody

                tempBody.Append("<p style='font-weight:bolder;'>เรียน..." + SUBCONTRACT_NAME + "</p>");
                //tempBody.Append("<br/>");
                if (!string.IsNullOrEmpty(CS_REMARK))
                {
                    //tempBody.Append("<span >Remark : " );
                    tempBody.Append("<br/>");
                    tempBody.Append("<span>** " + CS_REMARK);
                    tempBody.Append("<br/><br/>");
                }
                else
                {
                    tempBody.Append("<br/>");
                }

                tempBody.Append("<span style='padding-left: 50px;'>ทาง AIS Fibre ขอนำส่ง Certificate รอบการเบิก " + STRING_CS_APPROVE_DATE + " เพื่อใช้สำหรับตรวจสอบเอกสารประกอบกับใบวางบิลค่ะ ");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");

                tempBody.Append("รายละเอียดตามตารางด้านล่างค่ะ");
                tempBody.Append("<br/><br/><br/>");
                tempBody.Append("<span style='font-weight:bolder;background-color: #00ff00;'>ให้ออกใบกำกับภาษี พร้อมสำเนา 1 ฉบับ (สามารถถ่ายเอกสารได้)");
                tempBody.Append("</span>");
                tempBody.Append("<br/><br/>");
                tempBody.Append("<span>");
                tempBody.Append("ในนาม  " + COMP_NAME);
                tempBody.Append("<br/>" + COMP_ADDR);

                tempBody.Append("</span>");
                tempBody.Append("<br/><br/><br/>"); //<br/><br/>

                tempBody.Append("<u style='font-weight:bolder;' >**จัดส่งเอกสารทั้งหมดในนามที่อยู่**</u>");
                tempBody.Append("<br/>");
                tempBody.Append("<span style='background-color: #00ff00;'><u><b>ถึง ..</b> " + SCM_NAME + " </u></span>");
                tempBody.Append("<br/>");
                tempBody.Append("<span style='font-weight:bolder;background-color: #00ff00;'> ");
                tempBody.Append("<br/>" + SCM_ADDR);
                tempBody.Append("<br/>");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                if (str[1] != null)// complete
                {

                    tempBody.Append("<span style='font-weight:bolder; padding-left: 0px;'><u>รายละเอียดที่สามารถวางบิลได้</u>");
                    tempBody.Append("<br/>");
                    tempBody.Append("  " + "   Sheet Name : " + str[1]);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                    tempBody.Append("<br/>");
                }
                if (str[2] != null) //Re check
                {
                    tempBody.Append("<span style='font-weight:bolder; padding-left: 0px;'><u> อยู่ระหว่างการตรวจสอบข้อมูลเนื่องจากพบมีการบันทึกข้อมูลผิดปกติ </u>");
                    tempBody.Append("<br/>");
                    tempBody.Append("  " + "   Sheet Name : " + str[2]);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                    tempBody.Append("<br/>");
                }
                if (str[3] != null)//No data
                {

                    tempBody.Append("<span style='font-weight:bolder; padding-left: 0px;'><u>ไม่พบข้อมูลในระบบ  แจ้งให้ทางตัวแทนใช้โปรแกรม Last Mile Route </u>");
                    tempBody.Append("<br/>");
                    tempBody.Append(" " + "   Sheet Name : " + str[3]);
                    tempBody.Append("</span>");
                    tempBody.Append("<br/>");
                    tempBody.Append("<br/>");
                }

                tempBody.Append("<br/>");
                tempBody.Append("<span style='font-weight:bolder;'>");
                tempBody.Append("**หากส่งใบกำกับภาษี/ใบเสร็จรับเงิน (Invoice) และเอกสารการติดตั้ง ภายในวัน" + CS_DOC_DATE);
                tempBody.Append("<br/>");
                tempBody.Append("ทางตัวแทนฯจะได้รับเงินค่าติดตั้ง(โอนเงินเข้าบัญชีธนาคาร) ในวัน" + CS_PAYMENT_DATE);
                tempBody.Append("<br/>");
                tempBody.Append("(เงื่อนไข : ใบกำกับภาษีและเอกสารประกอบ ต้องถูกต้อง ครบถ้วน) กรณีไม่ทันกำหนดจะนำยอดไปรวมในรอบสัปดาห์ถัดไปค่ะ");
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<span style='font-weight:bolder;color:red'> หมายเหตุ: กรณีที่ท่านทำงาน ติดตั้ง แล้วไม่ได้รับ Report การวางบิล กรุณาติดต่อกลับที่  Email : " + SCM_MAIL);
                tempBody.Append("</span>");
                tempBody.Append("<br/>");
                tempBody.Append("<br/>");
                tempBody.Append("<span style=''>");
                if (words.Length > 0)
                {
                    foreach (string word in words)
                    {
                        tempBody.Append("<br/>");
                        tempBody.Append("" + word);
                    }
                }
                tempBody.Append("</span>");
                tempBody.Append("<br/>");

                #endregion
                string body = "";
                body = tempBody.ToSafeString();
                return body;
            }
            catch (Exception ex)
            {
                _Logger.Info("Error last mile send mail Menthod EmailTemplate: " + ex.GetErrorMessage());
                return ex.GetErrorMessage();
            }
        }

        private GenerateExcelModel GenerateExcel(DataSet dataToExcel, string fileName)
        {
            //=============Lov Config
            var query = new GetLovV2Query()
            {
                LovType = "WF_STATUS_REPORT_INSTALLATION",
                LovVal3 = "SEND_MAIL",
                LovVal5 = "REPORT_INSTALLATION"

            };
            var LovWorkStatus = _queryProcessor.Execute(query).ToList();
            //var LovWorkStatus = LovData.Where(l => l.LovValue3 == "SEND_MAIL" && l.LovValue5 == "REPORT_INSTALLATION").ToList();
            string LovCompleted = LovWorkStatus.Where(c => c.OrderBy == 1).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovNodata = LovWorkStatus.Where(c => c.OrderBy == 2).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovRecheck = LovWorkStatus.Where(c => c.OrderBy == 3).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();




            reportInstallScmOrderListModelBySendMail scmOrderListModelBySendMail = new reportInstallScmOrderListModelBySendMail();
            string parthfilename = (fileName.Trim() + ".xlsx");
            var physicalPath = "";



            if (parthfilename != null && Path.GetExtension(parthfilename).ToLower() == ".xlsx")
            {
                var query2 = new GetLovV2Query()
                {
                    LovType = "CONFIG_PATH",

                };
                var impersonateVar = _queryProcessor.Execute(query2).SingleOrDefault(l => l.Text == "ATTACH" && l.Name == "MAIL");
                //var impersonateVar = LovData.SingleOrDefault(l => l.Type == "CONFIG_PATH" && l.Text == "ATTACH" && l.Name == "MAIL");
                if (impersonateVar != null)
                {
                    string user = impersonateVar.LovValue1;
                    string pass = impersonateVar.LovValue2;
                    string ip = impersonateVar.LovValue3;
                    string directoryPath = "‪";
                    if (!string.IsNullOrEmpty(ip))
                    {
                        directoryPath = ip + "\\" + impersonateVar.LovValue4;
                    }
                    else
                    {
                        directoryPath = impersonateVar.LovValue4;
                    }



                    string SaveFileNY = impersonateVar.DefaultValue;
                    string TxtHearder = "";



                    physicalPath = Path.Combine(directoryPath, parthfilename);



                    try
                    {



                        //using (var impersonator = new Impersonator(user, ip, pass, false))
                        // {




                        FileInfo excel = new FileInfo(physicalPath);
                        DataTable dataTable = new DataTable();
                        Color colFromHex = ColorTranslator.FromHtml("#ffcc99");
                        Color colBorder = ColorTranslator.FromHtml("#e0ccff");



                        using (var package = new ExcelPackage(excel))
                        {



                            for (int i = 0; i <= dataToExcel.Tables.Count - 1; i++)
                            {
                                string sheetname = "";



                                var tableStatus = dataToExcel.Tables[i].TableName;
                                if (tableStatus == LovCompleted)
                                {
                                    if (dataToExcel.Tables[i].Rows.Count > 1) //เพราะมี Sum
                                    {



                                        TxtHearder = "รายละเอียดที่สามารถวางบิลได้";
                                        sheetname = LovCompleted;
                                    }



                                }
                                else if (tableStatus == LovRecheck)
                                {



                                    if (dataToExcel.Tables[i].Rows.Count > 0)
                                    {
                                        TxtHearder = "อยู่ระหว่างการตรวจสอบข้อมูลเนื่องจากพบมีการบันทึกข้อมูลผิดปกติ";
                                        sheetname = LovRecheck;



                                    }
                                }
                                else if (tableStatus == LovNodata)
                                {



                                    if (dataToExcel.Tables[i].Rows.Count > 0)
                                    {



                                        TxtHearder = "ไม่พบข้อมูลในระบบ  แจ้งให้ทางตัวแทนใช้โปรแกรม Report Installation Route  ";
                                        sheetname = LovNodata;



                                    }
                                }




                                if (!string.IsNullOrEmpty(sheetname))
                                {
                                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(sheetname);



                                    var rang = worksheet.Cells["A0:A0"];



                                    worksheet.Cells.Style.Font.Size = 10;
                                    worksheet.Cells.Style.Font.Name = "Arial Unicode MS";



                                    if (sheetname == LovCompleted)
                                    {
                                        worksheet.Cells["A1:N1"].Merge = true;
                                        worksheet.Cells["A1:N1"].LoadFromText(TxtHearder);
                                        rang = worksheet.Cells["A3:P3"];



                                    }
                                    else if (sheetname == LovRecheck)
                                    {
                                        worksheet.Cells["A1:I1"].Merge = true;
                                        worksheet.Cells["A1:I1"].LoadFromText(TxtHearder);
                                        rang = worksheet.Cells["A3:I3"];



                                    }
                                    else if (sheetname == LovNodata)
                                    {



                                        worksheet.Cells["A1:I1"].Merge = true;
                                        worksheet.Cells["A1:I1"].LoadFromText(TxtHearder);
                                        rang = worksheet.Cells["A3:I3"];



                                    }
                                    rang.Style.Font.Bold = true;
                                    worksheet.Cells["A1:N1"].Style.Font.Size = 12;
                                    worksheet.Cells["A1:N1"].Style.Font.UnderLine = true;
                                    worksheet.Cells["A1:N1"].Style.Font.Bold = true; //Header



                                    Color colHeaderHex = System.Drawing.ColorTranslator.FromHtml("#F7BE81");
                                    if (sheetname == LovCompleted)
                                    {
                                        worksheet.Cells["A3:Q3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells["A3:Q3"].Style.Fill.BackgroundColor.SetColor(colHeaderHex);
                                        worksheet.Cells["A3:Q3"].Style.Font.Bold = true; //Header data
                                    }
                                    else
                                    {
                                        worksheet.Cells["A3:I3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                        worksheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(colHeaderHex);
                                        worksheet.Cells["A3:I3"].Style.Font.Bold = true; //Header data
                                    }



                                    worksheet.Cells["A3"].LoadFromDataTable(dataToExcel.Tables[i], true, TableStyles.None)
                                    .AutoFitColumns();
                                }



                            }




                            //package.SaveAs(excel);
                            scmOrderListModelBySendMail.FileData = package.GetAsByteArray();


                        }
                        if (SaveFileNY.ToUpper() == "Y")
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ms.CopyTo(new MemoryStream(scmOrderListModelBySendMail.FileData));
                                ms.Seek(0, SeekOrigin.Begin);



                                if (!string.IsNullOrEmpty(user))
                                {
                                    using (var impersonator = new Impersonator(user, ip, pass, false))
                                    {
                                        using (FileStream fs = new FileStream(physicalPath, FileMode.OpenOrCreate))
                                        {
                                            new MemoryStream(scmOrderListModelBySendMail.FileData).CopyTo(fs);
                                            fs.Flush();



                                        }
                                    }



                                }
                                else
                                {



                                    using (FileStream fs = new FileStream(physicalPath, FileMode.OpenOrCreate))
                                    {
                                        new MemoryStream(scmOrderListModelBySendMail.FileData).CopyTo(fs);
                                        fs.Flush();



                                    }
                                }
                            }



                        }
                    }
                    catch (Exception exs)
                    {
                        _Logger.Info("Error GenExcel ReportInstallation Sendmail" + exs.GetBaseException());
                    }
                }
            }
            else
            {



            }
            var retobj = new GenerateExcelModel()
            {
                physicalPath = physicalPath,
                msExcel = new MemoryStream(scmOrderListModelBySendMail.FileData)
            };
            return retobj;
        }

        private async Task<string> CreateDataToDataTable(List<ReportInstallationCostbyOrderListModel_Binding> data
, string CS_APPROVE_DATE_FROM
, string CS_APPROVE_DATE_TO
, string STRING_CS_APPROVE_DATE
, string CS_DOC_DATE
, string CS_PAYMENT_DATE
, string CS_REMARK)
        {
            //=============Lov Config
            var query = new GetLovV2Query()
            {
                LovType = "WF_STATUS_REPORT_INSTALLATION",
                LovVal3 = "SEND_MAIL",
                LovVal5 = "REPORT_INSTALLATION"

            };
            var LovWorkStatus = _queryProcessor.Execute(query).ToList();
            //var LovWorkStatus = LovData.Where(l => l.LovValue3 == "SEND_MAIL" && l.LovValue5 == "REPORT_INSTALLATION").ToList();
            string LovCompleted = LovWorkStatus.Where(c => c.OrderBy == 1).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovNodata = LovWorkStatus.Where(c => c.OrderBy == 2).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovRecheck = LovWorkStatus.Where(c => c.OrderBy == 3).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            InterfaceLogCommand log = null;
            string fileName = "";



            GenerateExcelModel gnx;
            string SUBCONTRACT_NAME = "";
            string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            var groupedByPeriodList = data
            .GroupBy(u => u.PERIOD
            )
            .Select(grp => grp.ToList()).ToList();



            var ds = new DataSet();
            var sheetNameList = new List<string>();
            #region Head Datatable
            DataTable table = new DataTable();
            table.Columns.Add("NO.", System.Type.GetType("System.String"));
            table.Columns.Add("รอบการเบิก", System.Type.GetType("System.String"));
            table.Columns.Add("Complete Date", System.Type.GetType("System.String"));
            table.Columns.Add("Internet No.", System.Type.GetType("System.String"));
            table.Columns.Add("Account Name.", System.Type.GetType("System.String"));
            table.Columns.Add("วันนัดติดตั้ง", System.Type.GetType("System.String"));
            table.Columns.Add("Promotion", System.Type.GetType("System.String"));
            table.Columns.Add("Subcontract", System.Type.GetType("System.String"));



            object[] values = new object[table.Columns.Count];
            int summary = 0;



            DataTable dtComplete = table.Clone();
            dtComplete.TableName = LovCompleted;
            dtComplete.Columns.Add("ระยะสาย", System.Type.GetType("System.String"));
            //dtComplete.Columns.Add("ราคา Out door", System.Type.GetType("System.String"));
            //dtComplete.Columns.Add("ราคา In door", System.Type.GetType("System.String"));
            ////R19.03
            //dtComplete.Columns.Add("MAPPING COST", System.Type.GetType("System.String"));
            //dtComplete.Columns.Add("OVER LENGTH", System.Type.GetType("System.String"));
            //dtComplete.Columns.Add("OVER COST", System.Type.GetType("System.String"));
            //End R19.03
            dtComplete.Columns.Add("Total Paid", System.Type.GetType("System.String"));
            dtComplete.Columns.Add("ค่าแรกเข้า(Entry Fee)รวม VAT", System.Type.GetType("System.String"));
            // dtComplete.Columns.Add("Remark For Sub", System.Type.GetType("System.String"));



            DataTable dtReCheck = table.Clone();
            dtReCheck.TableName = LovRecheck;
            // dtReCheck.Columns.Add("Remark For Sub", System.Type.GetType("System.String"));



            DataTable dtNoData = table.Clone();
            dtNoData.TableName = LovNodata;
            // dtNoData.Columns.Add("Remark For Sub", System.Type.GetType("System.String"));
            #endregion
            int successcount = 0;
            int errorcount = 0;
            int indexNoComp = 1;
            int indexNoRecheck = 1;
            int indexNoNoData = 1;
            string SUBCONTRACT_EMAIL = "";
            string emaillogstatus = string.Empty;
            List<LogSentEmail> emaillog = new List<LogSentEmail>();



            int subcount = 0;



            // log = StartInterface(emaillog, "EmailExport", "", null, "EmailExport", "Start EmailExport");
            for (int i = 0; i <= groupedByPeriodList.Count - 1; i++)
            {



                if (groupedByPeriodList[i].Count > 0)
                {
                    //group by sub name
                    var groupBySubName = groupedByPeriodList[i]
                    .GroupBy(u => new { u.ORG_ID })
                    .Select(grp => grp.ToList()).ToList();



                    if (groupBySubName.Count > 0)
                    {
                        for (int j = 0; j <= groupBySubName.Count - 1; j++)
                        {
                            List<SubAccNo> SubAccNolist = new List<SubAccNo>();
                            var groupByOrderStatus = groupBySubName[j];



                            if (groupByOrderStatus.Count > 0)
                            {
                                for (int k = 0; k <= groupByOrderStatus.Count - 1; k++)
                                {



                                    fileName = GetFileNameExcel(groupByOrderStatus[k].ORG_ID.ToSafeString()
                                    , CS_APPROVE_DATE_FROM, CS_APPROVE_DATE_TO
                                    , STRING_CS_APPROVE_DATE);



                                    SUBCONTRACT_NAME = groupByOrderStatus[k].SBC_CPY.ToSafeString();
                                    if (!string.IsNullOrEmpty(groupByOrderStatus[k].SUB_MAIL))
                                    {
                                        SUBCONTRACT_EMAIL = groupByOrderStatus[k].SUB_MAIL.ToSafeString().Trim();
                                    }



                                    if (!string.IsNullOrEmpty(groupByOrderStatus[k].ORDER_STATUS.ToSafeString()))
                                    {



                                        if (groupByOrderStatus[k].ORDER_STATUS == LovCompleted)
                                        {
                                            #region workStatus COMPLETED
                                            values = new object[dtComplete.Columns.Count];



                                            values[0] = indexNoComp;
                                            values[1] = groupByOrderStatus[k].PERIOD.ToSafeString();
                                            values[2] = groupByOrderStatus[k].ORDER_STATUS_DT.ToDateDisplayText();
                                            values[3] = groupByOrderStatus[k].ACCESS_NUMBER.ToSafeString();
                                            values[4] = groupByOrderStatus[k].ACCOUNT_NAME.ToSafeString();
                                            values[5] = groupByOrderStatus[k].APPOINTMENNT_DT.ToDateDisplayText();
                                            values[6] = groupByOrderStatus[k].PROMOTION_NAME.ToSafeString();
                                            values[7] = groupByOrderStatus[k].SBC_CPY.ToSafeString();
                                            values[8] = string.Format("{0:n2}", groupByOrderStatus[k].DISTANCE_TOTAL.ToSafeDecimal());
                                            //values[9] = string.Format("{0:n2}", groupByOrderStatus[k].OUTDOOR_COST.ToSafeDecimal());
                                            //values[10] = string.Format("{0:n2}", groupByOrderStatus[k].INDOOR_COST.ToSafeDecimal());
                                            ////R19.03
                                            //values[11] = string.Format("{0:n2}", groupByOrderStatus[k].MAPPING_COST.ToSafeDecimal());
                                            //values[12] = string.Format("{0:n2}", groupByOrderStatus[k].OVER_LENGTH.ToSafeDecimal());
                                            //values[13] = string.Format("{0:n2}", groupByOrderStatus[k].OVER_COST.ToSafeDecimal());
                                            //End R19.03
                                            values[9] = string.Format("{0:n2}", groupByOrderStatus[k].TOTAL_PAID.ToSafeDecimal());
                                            values[10] = string.Format("{0:n2}", groupByOrderStatus[k].ENTRY_FEE.ToSafeDecimal());
                                            // values[16] = groupByOrderStatus[k].NOTE.ToSafeString();
                                            summary += Convert.ToInt32(groupByOrderStatus[k].TOTAL_PAID.ToSafeDecimal());
                                            dtComplete.Rows.Add(values);
                                            indexNoComp++;
                                            #endregion



                                        }
                                        else if (groupByOrderStatus[k].ORDER_STATUS == LovRecheck)
                                        {
                                            #region workStatus Recheck
                                            values = new object[dtReCheck.Columns.Count];
                                            values[0] = indexNoRecheck;
                                            values[1] = groupByOrderStatus[k].PERIOD.ToSafeString();
                                            values[2] = groupByOrderStatus[k].ORDER_STATUS_DT.ToDateDisplayText();
                                            values[3] = groupByOrderStatus[k].ACCESS_NUMBER.ToSafeString();
                                            values[4] = groupByOrderStatus[k].ACCOUNT_NAME.ToSafeString();
                                            values[5] = groupByOrderStatus[k].APPOINTMENNT_DT.ToDateDisplayText();
                                            values[6] = groupByOrderStatus[k].PROMOTION_NAME.ToSafeString();
                                            values[7] = groupByOrderStatus[k].SBC_CPY.ToSafeString();
                                            // values[8] = groupByOrderStatus[k].NOTE.ToSafeString();



                                            dtReCheck.Rows.Add(values);
                                            indexNoRecheck++;
                                            #endregion



                                        }
                                        else if (groupByOrderStatus[k].ORDER_STATUS == LovNodata)
                                        {
                                            #region workStatus Nodata
                                            values = new object[dtNoData.Columns.Count];
                                            values[0] = indexNoNoData;
                                            values[1] = groupByOrderStatus[k].PERIOD.ToSafeString();
                                            values[2] = groupByOrderStatus[k].ORDER_STATUS_DT.ToDateDisplayText();
                                            values[3] = groupByOrderStatus[k].ACCESS_NUMBER.ToSafeString();
                                            values[4] = groupByOrderStatus[k].ACCOUNT_NAME.ToSafeString();
                                            values[5] = groupByOrderStatus[k].APPOINTMENNT_DT.ToDateDisplayText();
                                            values[6] = groupByOrderStatus[k].PROMOTION_NAME.ToSafeString();
                                            values[7] = groupByOrderStatus[k].SBC_CPY.ToSafeString();
                                            // values[8] = groupByOrderStatus[k].NOTE.ToSafeString();
                                            dtNoData.Rows.Add(values);
                                            indexNoNoData++;
                                            #endregion



                                        }
                                    }



                                    var accNOModels = new SubAccNo
                                    {
                                        Accno = groupByOrderStatus[k].ACCESS_NUMBER.ToSafeString()
                                    };
                                    SubAccNolist.Add(accNOModels);




                                }



                                // insert row values
                                decimal sumReportInstallation = 0;
                                foreach (DataRow dr in dtComplete.Rows)
                                {
                                    if (!string.IsNullOrEmpty(dr[9].ToSafeString()))
                                    {
                                        sumReportInstallation += Convert.ToDecimal(dr[9].ToSafeString());
                                    }
                                }



                                dtComplete.Rows.Add(new Object[]{
"","","","","","","","","ยอดรวม",string.Format("{0:n2}", sumReportInstallation),""



});



                                ds.Tables.Clear();
                                ds.Tables.Add(dtComplete);
                                ds.Tables.Add(dtReCheck);
                                ds.Tables.Add(dtNoData);



                                //select sheet name
                                string[] strArr = new string[5];
                                if (dtComplete.Rows.Count > 1) //เพราะมี Sum
                                {
                                    strArr[1] = LovCompleted;
                                }



                                if (dtReCheck.Rows.Count > 0)
                                {
                                    strArr[2] = LovRecheck;
                                }



                                if (dtNoData.Rows.Count > 0)
                                {
                                    strArr[3] = LovNodata;
                                }




                                //export excel
                                gnx = GenerateExcel(ds, fileName);



                                strArr[0] = gnx.physicalPath.ToSafeString();
                                strArr[4] = fileName;




                                var fileStream = gnx.msExcel;

                                //send email to sub contract
                                var resultSendMail = sendEmailByAttachFile(fileName, fileStream
                                , SUBCONTRACT_NAME
                                , STRING_CS_APPROVE_DATE
                                , strArr
                                , CS_DOC_DATE
                                , CS_PAYMENT_DATE
                                , CS_REMARK
                                , SUBCONTRACT_EMAIL);



                                resultSendMail.Wait();
                                String chkstatus = resultSendMail.Result.ToSafeString();



                                emaillogstatus = chkstatus;



                                if (chkstatus == "Success")
                                    successcount += 1;
                                else
                                    errorcount += 1;


                                if (resultSendMail.IsCompleted)
                                {
                                    Task.FromResult(resultSendMail.Result);

                                    subcount += 1;
                                    var logEmailModels = new LogSentEmail
                                    {
                                        AccNo = SubAccNolist,
                                        emailsub = SUBCONTRACT_EMAIL,
                                        subname = SUBCONTRACT_NAME,
                                        usename = CurrentUser.UserName.ToSafeString(),
                                        senddate = DateNow,
                                        SubCount = subcount.ToSafeString(),
                                        status = emaillogstatus
                                    };
                                    emaillog.Add(logEmailModels);
                                }
                                else
                                {
                                    resultSendMail.ContinueWith(t => t.Result, CancellationToken.None,
                                    TaskContinuationOptions.ExecuteSynchronously,
                                    TaskScheduler.Default);
                                    subcount += 1;
                                    var logEmailModels = new LogSentEmail
                                    {
                                        AccNo = SubAccNolist,
                                        emailsub = SUBCONTRACT_EMAIL,
                                        subname = SUBCONTRACT_NAME,
                                        usename = CurrentUser.UserName.ToSafeString(),
                                        senddate = DateNow,
                                        SubCount = subcount.ToSafeString(),
                                        status = emaillogstatus



                                    };
                                    emaillog.Add(logEmailModels);
                                }
                                dtComplete.Clear();
                                dtReCheck.Clear();
                                dtNoData.Clear();



                                indexNoComp = 1;
                                indexNoRecheck = 1;
                                indexNoNoData = 1;
                                fileName = "";



                            }



                        }
                    }
                    string ResultLog = string.Empty;



                    if (successcount > 0 && errorcount == 0)
                    {
                        ResultLog = "Success";
                    }
                    if (successcount > 0 && errorcount > 0)
                    {
                        ResultLog = "Success With Error";
                    }
                    if (successcount == 0 && errorcount > 0)
                    {
                        ResultLog = "Error";
                    }
                    EndInterface("", log, null, ResultLog, "Send EmailComplete", "", CurrentUser.UserName.ToSafeString());
                    // here for log
                }
            }

            return "";
        }

        private async Task<string> sendEmailByAttachFile(string fileName, MemoryStream msExcel, string SUBCONTRACT_NAME, string STRING_CS_APPROVE_DATE, string[] LIST_FILE, string CS_DOC_DATE, string CS_PAYMENT_DATE, string CS_REMARK, string sub_contact_email)
        {
            try
            {
                string result = "";
                string body = "";
                List<MemoryStreamAttachFiles> files = new List<MemoryStreamAttachFiles>();
                if (msExcel == null)
                {
                    files = null;
                }
                else
                {
                    MemoryStreamAttachFiles attachFile = new MemoryStreamAttachFiles();
                    attachFile.file = msExcel;
                    attachFile.fileName = fileName + ".xlsx";
                    files.Add(attachFile);
                }
                body = EmailTemplate(SUBCONTRACT_NAME, STRING_CS_APPROVE_DATE, LIST_FILE, CS_DOC_DATE, CS_PAYMENT_DATE, CS_REMARK);

                var command = new SendMailReportInstallationNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_FIXED_REPORTINSTALLATION",
                    Subject = "ใบ certificate รอบการเบิก..." + STRING_CS_APPROVE_DATE + " " + SUBCONTRACT_NAME,
                    Body = body,
                    msAttachFiles = files,
                    SendTo = sub_contact_email.ToSafeString()
                };
                _sendMail.Handle(command);
                _Logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                result = command.ReturnMessage != "Success." ? result = command.ReturnMessage : "Success";
                return result;
            }
            catch (Exception Ex)
            {
                return Ex.Message.ToSafeString();
            }
        }

        private string GetFileNameExcel(string ORG_ID, string CS_APPROVE_DATE_FROM, string CS_APPROVE_DATE_TO, string STRING_CS_APPROVE_DATE)
        {
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();

                CS_APPROVE_DATE_FROM = DateTime.ParseExact(CS_APPROVE_DATE_FROM, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                CS_APPROVE_DATE_TO = DateTime.ParseExact(CS_APPROVE_DATE_TO, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

                string fileName = ORG_ID + "_" + CS_APPROVE_DATE_FROM + "_" + CS_APPROVE_DATE_TO + "_" + DateNow;
                //string fileName = "Excel" + DateNow;
                return (fileName + "_" + CurrentUser.UserName).Trim();
            }
            catch (Exception ex)
            {
                _Logger.Info("Error Report Installation Send mail GetFileNameExcel" + ex.GetBaseException());
                return ex.GetBaseException().ToString();
            }

        }
        #endregion

        #region ReportInstallation Re-Cal-Distance
        public JsonResult SelectFAPOWorkFlowStatus()
        {
            var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION");
            //   .Where(d => d.LOV_NAME.Contains("FAPO")).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "SELECT", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult FapoRecalByFile()
        {
            var data = SelectFbbCfgLov("WF_STATUS_REPORT_INSTALLATION", "REPORT_INSTALLATION")
             .Where(d => d.LOV_VAL1.Equals("Completed")).ToList();
            data.Insert(0, new LovModel { DISPLAY_VAL = "PLEASE SELECT", LOV_VAL1 = "ALL" });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRuleId()
        {
            var query = new GetRuleReportInstallationCostbyOrderQuery
            {

            };
            var data = _queryProcessor.Execute(query);
            data.Insert(0, new RuleReportInstallationCostbyOrderModel { rule_name = "SELECT", ruleid = "ALL", total_price = 0 });
            Session["TempTotalPrice"] = data;
            return Json(data, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTotalPrice(string new_ruid)
        {
            var data = Session["TempTotalPrice"] as List<RuleReportInstallationCostbyOrderModel>;
            return Json(data.Where(x => x.ruleid == new_ruid).Select(r => r.total_price), JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecalDistanceByOrder([DataSourceRequest] DataSourceRequest request
            , List<ReportInstallationRecal> ReportInstallationRecalList,
          string AccNO, string new_ruid, string remark)
        {
            string msg = "";
            if (ReportInstallationRecalList != null)
            {
                string ACCNO = string.Empty;
                foreach (var d in ReportInstallationRecalList)
                {
                    //  ACCNO = d.ACCESS_NUMBER;
                    if (d.ACCESS_NUMBER == "000000000" || d.ACCESS_NUMBER == "" || d.ACCESS_NUMBER == null)
                    {
                        return null;
                    }
                    if (d.DISTANCE == "" || d.DISTANCE == null)
                    {
                        d.DISTANCE = "0";
                    }
                }


            }

            try
            {

                var command = new ReportInstallationRecalByOrderCommand()
                {
                    p_recal_access_list = ReportInstallationRecalList,
                    //p_NEW_RULE_ID = new_ruid,
                    p_USER = CurrentUser.UserName,
                    //p_REMARK = remark

                };
                _ReportInstallationRecalByOrderCommand.Handle(command);

                if (command.ret_code == "0")
                {
                    msg = "Re-Cal Success.";

                }
                else
                {
                    msg = "Re-Cal Not Success.";
                }
                Session["TempSearchCriteria"] = null;

                #region Call API Subpayment
                ReportInstallationModelApiSubpayment modelSub = new ReportInstallationModelApiSubpayment();
                modelSub.Order_list = new List<ReportInstallationOrderList>();
                List<ReportInstallationOrderList> ListApi = new List<ReportInstallationOrderList>();
                foreach (var item in command.return_subpayment_cur)
                {
                    var model = new ReportInstallationOrderList()
                    {
                        Internet_no = item.access_number,
                        Order_no = item.order_no,
                        Distance_to_paid = item.distance_to_paid.ToSafeString(),
                        Total_Paid = item.total_paid.ToSafeString(),
                        Product = item.product,
                        Order_type = item.order_type,
                        Vendor_code = item.vendor_code,
                        LMD_status = item.lmd_status
                    };
                    ListApi.Add(model);
                }
                modelSub.Order_list.AddRange(ListApi);

                if (modelSub.Order_list.Count != 0)
                {
                    var result = Call_Subpayment_APIAsync(modelSub, CurrentUser.UserName);
                }
                Session["TempSearchCriteria"] = null;

                #endregion

                return Json(
                   new
                   {
                       Code = command.ret_code,
                       message = msg,
                   }, JsonRequestBehavior.AllowGet
                   );


            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(
                  new
                  {
                      Code = "1",
                      message = "Re-Cal Not Success.",
                  }, JsonRequestBehavior.AllowGet
                  );
            }
        }


        public JsonResult RecalDistanceByRuleId([DataSourceRequest] DataSourceRequest request
    , List<ReportInstallationRecal> ReportInstallationRecalList,
  string AccNO, string remark)
        {
            string msg = "";
            if (ReportInstallationRecalList != null)
            {
                string ACCNO = string.Empty;
                foreach (var d in ReportInstallationRecalList)
                {
                    //  ACCNO = d.ACCESS_NUMBER;
                    if (d.ACCESS_NUMBER == "000000000" || d.ACCESS_NUMBER == "" || d.ACCESS_NUMBER == null)
                    {
                        return null;
                    }
                    if (d.DISTANCE == "" || d.DISTANCE == null)
                    {
                        d.DISTANCE = "0";
                    }
                    if (!string.IsNullOrEmpty(d.RECAL_TOTAL_COST))
                    {
                        // Try parsing the cost as a decimal
                        if (decimal.TryParse(d.RECAL_TOTAL_COST, out decimal totalCost))
                        {
                            // Check if the total cost is a whole number and convert accordingly
                            d.RECAL_TOTAL_COST = totalCost % 1 == 0 ? ((int)totalCost).ToString() : totalCost.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(d.REMARK))
                    {
                        d.REMARK = "";
                    }
                }


            }

            try
            {

                var command = new ReportInstallationRecalByOrderCommand()
                {
                    p_recal_access_list = ReportInstallationRecalList,
                    //p_NEW_RULE_ID = new_ruid,
                    p_USER = CurrentUser.UserName,
                    //p_REMARK = remark

                };
                _ReportInstallationRecalByOrderCommand.Handle(command);

                if (command.ret_code == "0")
                {
                    msg = "Re-Cal Success.";

                }
                else
                {
                    msg = "Re-Cal Not Success.";
                }
                Session["TempSearchCriteria"] = null;

                #region Call API Subpayment
                ReportInstallationModelApiSubpayment modelSub = new ReportInstallationModelApiSubpayment();
                modelSub.Order_list = new List<ReportInstallationOrderList>();
                List<ReportInstallationOrderList> ListApi = new List<ReportInstallationOrderList>();
                foreach (var item in command.return_subpayment_cur)
                {
                    var model = new ReportInstallationOrderList()
                    {
                        Internet_no = item.access_number,
                        Order_no = item.order_no,
                        Distance_to_paid = item.distance_to_paid.ToSafeString(),
                        Total_Paid = item.total_paid.ToSafeString(),
                        Product = item.product,
                        Order_type = item.order_type,
                        Vendor_code = item.vendor_code,
                        LMD_status = item.lmd_status
                    };
                    ListApi.Add(model);
                }
                modelSub.Order_list.AddRange(ListApi);

                if (modelSub.Order_list.Count != 0)
                {
                    var result = Call_Subpayment_APIAsync(modelSub, CurrentUser.UserName);
                }
                Session["TempSearchCriteria"] = null;

                #endregion

                return Json(
                   new
                   {
                       Code = command.ret_code,
                       message = msg,
                   }, JsonRequestBehavior.AllowGet
                   );


            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(
                  new
                  {
                      Code = "1",
                      message = "Re-Cal Not Success.",
                  }, JsonRequestBehavior.AllowGet
                  );
            }
        }

        private async Task<JsonResult> Call_Subpayment_APIAsync(ReportInstallationModelApiSubpayment body, string username)
        {

            var result = new ReportInstallationResponse_API_Subpayment();
            var transactionID = DateTime.Now.ToString("yyyyMMddHHmmss") + "0001";
            InterfaceLogPayGCommand log = StartInterface<ReportInstallationModelApiSubpayment>(body, "Call API Subcontract Payment", transactionID, "");
            try
            {
                var api_subpayment_process = LovData.Where(l => l.Type == "FBBPAYG_LMD_SUBPAYMENT_REPORT_INSTALLATION" && l.Name == "UI_FLAG_LMD_SUBPAYMENT_REPORT_INSTALLATION").FirstOrDefault();

                if (api_subpayment_process != null)
                {
                    if (api_subpayment_process.ActiveFlag == "Y")
                    {
                        var json = new JavaScriptSerializer().Serialize(body);
                        var api_subpayment_url = LovData.Where(l => l.Type == "WS_SubcontractPayment_REPORT_INSTALLATION" && l.Name.Contains("UpdatedataRecal")).FirstOrDefault();
                        var client = new RestClient(api_subpayment_url.Text);
                        client.Timeout = -1;
                        var request = new RestRequest(Method.POST);
                        request.AddHeader("Update_by", username + "|LMD");
                        request.AddHeader("Update_Date", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
                        request.AddHeader("Content-Type", "application/json");
                        request.AddParameter("application/json", json, ParameterType.RequestBody);

                        ServicePointManager.Expect100Continue = true;
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                        ServicePointManager.ServerCertificateValidationCallback =
                            (s, certificate, chain, sslPolicyErrors) => true;
                        IRestResponse response = client.Execute(request);

                        if (HttpStatusCode.OK.Equals(response.StatusCode))
                        {
                            result = JsonConvert.DeserializeObject<ReportInstallationResponse_API_Subpayment>(response.Content) ?? new ReportInstallationResponse_API_Subpayment();
                            EndInterface<ReportInstallationResponse_API_Subpayment
>(result, log, transactionID, result.Result_code == "0000" ? "Success" : "Failed", "");
                        }
                        else
                        {
                            EndInterface<ReportInstallationResponse_API_Subpayment
>(result, log, transactionID, "Failed", "");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                EndInterface<ReportInstallationResponse_API_Subpayment
>(result, log, transactionID, "System Error", ex.GetErrorMessage());
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecalAllByOrder([DataSourceRequest] DataSourceRequest request,
          List<ReportInstallationRecal> ReportInstallationRecalList, string AccNO, string Status, string remark, string new_rule_id)
        {
            string msg = "";
            //   int CountStatusNotMatch = 0, CountStatustMatch = 0;
            int CountStatustMatch = 0;
            List<ReportInstallationRecal> ListReportInstallationRecal = new List<ReportInstallationRecal>();

            ReportInstallationCostbyOrderModel searchModel = (ReportInstallationCostbyOrderModel)TempData.Peek("TempSearchCriteria");
            var AllRecord = GetReportInstallationOrderListByPage(searchModel, 1, Decimal.MaxValue);
            var result = AllRecord.cur;
            var AccNOList = result.Where(a => GetOrderStatusByUserGroup().Contains(a.ORDER_STATUS) && ReportInstallationRecalList.Select(x => x.ACCESS_NUMBER).Contains(a.ACCESS_NUMBER)).ToList();

            //var AccNOList = result.Where(a => GetOrderStatusByUserGroup().Contains(a.ORDER_STATUS)).Select(a => new FBB_access_list { ACCESS_NUMBER = a.ACCESS_NUMBER }).ToList();

            try
            {
                foreach (var resultStatus in AccNOList)
                {
                    //if (resultStatus.ORDER_STATUS.ToLower().Replace(" ", "")
                    //    != Status.ToLower().Replace(" ", "") && Status.ToLower().Replace(" ", "") !="all"
                    //    && !string.IsNullOrEmpty(Status))
                    //{
                    //    CountStatusNotMatch += 1;
                    //}
                    //else
                    //{
                    string[] words = resultStatus.ORDER_NO.Split('_');
                    var model = new ReportInstallationRecal
                    {
                        ACCESS_NUMBER = resultStatus.ACCESS_NUMBER,
                        ORDER_NO = words[0],
                        NEW_RULE_ID = new_rule_id,
                        REMARK = remark
                    };
                    ListReportInstallationRecal.Add(model);
                    CountStatustMatch += 1;


                    //}
                }

                var command = new ReportInstallationRecalByOrderCommand()
                {
                    p_recal_access_list = ListReportInstallationRecal,
                    p_NEW_RULE_ID = new_rule_id,
                    p_USER = CurrentUser.UserName,
                    p_REMARK = remark

                };
                _ReportInstallationRecalByOrderCommand.Handle(command);
                if (command.ret_code == "0")
                {
                    msg = "Re-Cal Success. ";

                }
                else
                {
                    msg = "Re-Cal Not Success.";

                }
                return Json(
                   new
                   {
                       Code = command.ret_code,
                       message = msg,
                   }, JsonRequestBehavior.AllowGet
                   );


            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return Json(
                  new
                  {
                      Code = "1",
                      message = "Re-Cal Not Success.",
                  }, JsonRequestBehavior.AllowGet
                  );
            }
        }

        #region RecalByFile
        public JsonResult ReportInstallationCostbyOrderReCalByFile([DataSourceRequest] DataSourceRequest request, string status,
         string fileName)
        {


            if (string.IsNullOrEmpty(reportInstallFileModel.csv))
            {
                return Json("Please upload file", JsonRequestBehavior.AllowGet);
            }
            try
            {
                var lines = reportInstallFileModel.csv.Split(new[] { "\r\n", "\r", "\n", "\t", "|", "," }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length == 0)
                {
                    throw new Exception("Blank .csv file");
                }
                if (lines[0].Trim().Replace(" ", "") == "ACCESS_NUMBER" && lines[1].Trim().Replace(" ", "") == "ORDER_NO"
                    && lines[2].Trim().Replace(" ", "") == "EXISTING_RULE"
                    && lines[3].Trim().Replace(" ", "") == "NEW_RULE" && lines[4].Trim().Replace(" ", "") == "REMARK"
                    && (status.ToLower().Trim().Replace(" ", "") == "recheck" || status.ToLower().Trim().Replace(" ", "") == "completed"))
                {
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary = ExecuteRecalByFileReCheck(status, fileName);
                    return Json(
                       new
                       {
                           Code = dictionary["Code"],
                           message = dictionary["message"],
                       }, JsonRequestBehavior.AllowGet
                       );
                }
                else if (lines[0].Trim().Replace(" ", "") == "ACCESS_NUMBER" && lines[1].Trim().Replace(" ", "") == "VALIDATE_DISTANCE"
                    && lines[2].Trim().Replace(" ", "") == "REASON" && lines[3].Trim().Replace(" ", "") == "REMARK"
                    && status.ToLower().Trim().Replace(" ", "") == "dispute")
                {

                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    dictionary = ExecuteRecalByFileDispute(status, fileName);
                    return Json(
                       new
                       {
                           Code = dictionary["Code"],
                           message = dictionary["message"],
                       }, JsonRequestBehavior.AllowGet
                   );

                }
                else
                {
                    return Json(
                        new
                        {
                            Code = 1,
                            message = "Please Check File Formatch.",
                        }, JsonRequestBehavior.AllowGet
                    );
                }
            }
            catch (Exception e)
            {
                if (e.GetErrorMessage().Contains("Index was outside the bounds of the array."))
                {
                    return Json(
                              new
                              {
                                  Code = 1,
                                  message = "Please fill all necessary data.",
                              }, JsonRequestBehavior.AllowGet
                                );

                }

                return Json(
                       new
                       {
                           Code = 1,
                           message = e.GetErrorMessage().ToString(),
                       }, JsonRequestBehavior.AllowGet
                      );

            }
        }

        public Dictionary<string, string> ExecuteRecalByFileReCheck(string status, string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                //string ACCESS_NUMBER;
                //string NEW_RULE_ID;
                //string REMARK;
                //string EXISTING_RULE;
                //string ORDER_NO;

                string ACCNBR = string.Empty;
                string NEW_RULEID = string.Empty;
                string REMARK = string.Empty;
                string EX_RULE = string.Empty;
                string ORD_NO = string.Empty;

                var fileList = new List<ReportInstallationRecal>();
                var ReportInstallationRecalList = new List<ReportInstallationRecal>();
                var linesData = reportInstallFileModel.csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in linesData)
                {
                    var values = item.Split(',', '|');

                    //ACCESS_NUMBER = values[0];
                    //ORDER_NO = values[1];
                    //EXISTING_RULE = values[2];
                    //NEW_RULE_ID = values[3];
                    //REMARK = values[4];



                    if (values != null)
                    {
                        for (int i = 0; i <= values.Count() - 1; i++)
                        {
                            if (i == 0)
                            {
                                ACCNBR = values[i];
                            }
                            else if (i == 1)
                            {
                                ORD_NO = values[i];
                            }
                            else if (i == 2)
                            {
                                EX_RULE = values[i];
                            }
                            else if (i == 3)
                            {
                                NEW_RULEID = values[i];
                            }
                            else if (i == 4)
                            {
                                REMARK = values[i];
                            }


                        }
                    }

                    //new list 12.2021
                    var validateDis = ConvertValidateDistance("");
                    var model = new ReportInstallationRecal
                    {
                        ACCESS_NUMBER = ACCNBR.ToSafeString().Replace(" ", ""),
                        NEW_RULE_ID = NEW_RULEID.ToSafeString().Replace(" ", ""),
                        ORDER_NO = ORD_NO.ToSafeString().Replace(" ", ""),
                        DISTANCE = validateDis.ToSafeString(),
                        FLAG_RECAL = "RECAL",
                        REASON = "",
                        REMARK = REMARK
                    };
                    ReportInstallationRecalList.Add(model);
                }

                //new command model 12.2021
                ReportInstallationRecalList.RemoveAt(0);
                var command = new ReportInstallationRecalByOrderCommand()
                {
                    p_recal_access_list = ReportInstallationRecalList,
                    //p_NEW_RULE_ID = new_ruid,
                    p_USER = CurrentUser.UserName,
                    //p_REMARK = remark

                };
                _ReportInstallationRecalByOrderCommand.Handle(command);

                dictionary["Code"] = command.ret_code.ToSafeString();
                dictionary["message"] = command.ret_msg.ToSafeString();

                #region Call API Subpayment
                ReportInstallationModelApiSubpayment modelSub = new ReportInstallationModelApiSubpayment();
                modelSub.Order_list = new List<ReportInstallationOrderList>();
                List<ReportInstallationOrderList> ListApi = new List<ReportInstallationOrderList>();
                foreach (var item in command.return_subpayment_cur)
                {
                    var model = new ReportInstallationOrderList()
                    {
                        Internet_no = item.access_number,
                        Order_no = item.order_no,
                        Distance_to_paid = item.distance_to_paid.ToSafeString(),
                        Total_Paid = item.total_paid.ToSafeString(),
                        Product = item.product,
                        Order_type = item.order_type,
                        Vendor_code = item.vendor_code,
                        LMD_status = item.lmd_status
                    };
                    ListApi.Add(model);
                }
                modelSub.Order_list.AddRange(ListApi);

                if (modelSub.Order_list.Count != 0)
                {
                    var result = Call_Subpayment_APIAsync(modelSub, CurrentUser.UserName);
                }
                Session["TempSearchCriteria"] = null;

                #endregion
                // dictionary["message"] = command.messege_log_file.ToSafeString();
            }
            catch (Exception ex)
            {
                dictionary["Code"] = "1";
                dictionary["message"] = "ERROR.";
            }

            return dictionary;

        }

        public Dictionary<string, string> ExecuteRecalByFileDispute(string status, string fileName)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            try
            {
                //string ACCESS_NUMBER;
                //decimal VALIDATE_DIS;
                //string REASON;
                //string REMARK;
                //decimal num1;

                string ACCNBR = string.Empty;
                decimal VAL_DIS = 0;
                string REASON = string.Empty;
                string REMARK = string.Empty;
                decimal num1 = 0;



                var fileList = new List<ReportInstallation_FBB_update_file_list>();
                var linesData = reportInstallFileModel.csv.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in linesData)
                {
                    var values = item.Split(',', '|');

                    bool res = decimal.TryParse(values[1], out num1);
                    if (res == true)
                    {
                        // ACCESS_NUMBER = values[0];
                        //  VALIDATE_DIS = Convert.ToDecimal(values[1]);
                        // REASON = values[2];
                        // REMARK = values[3];
                        if (values != null)
                        {
                            for (int i = 0; i <= values.Count() - 1; i++)
                            {
                                if (i == 0)
                                {
                                    ACCNBR = values[i];
                                }
                                else if (i == 1)
                                {
                                    VAL_DIS = Convert.ToDecimal(values[i]);
                                }
                                else if (i == 2)
                                {
                                    REASON = values[i];
                                }
                                else if (i == 3)
                                {
                                    REMARK = values[i];
                                }

                            }
                        }
                        var model = new ReportInstallation_FBB_update_file_list
                        {
                            ACC_NBR = ACCNBR,
                            VALIDATE_DIS = VAL_DIS,
                            REASON = REASON,
                            REMARK = REMARK,
                        };
                        fileList.Add(model);
                    }

                }
                // fileList.RemoveAt(0);
                var command = new ReportInstallationCostbyOrderUpdateByFileCommand()
                {
                    p_INTERFACE = GetUserGroup(),
                    p_USER = CurrentUser.UserName,
                    p_STATUS = status,
                    p_filename = fileName,
                    p_file_list = fileList
                };

                _ReportInstallationCostbyOrderUpdateByFileCommand.Handle(command);
                if (command.ret_code != "0")
                {
                    dictionary["Code"] = command.ret_code.ToSafeString();
                    dictionary["message"] = "Update file Not success.";
                }
                else
                {
                    //"Update file success."
                    dictionary["Code"] = command.ret_code.ToSafeString();
                    dictionary["message"] = command.ret_msg.ToSafeString();
                }


            }
            catch (Exception ex)
            {
                dictionary["Code"] = "1";
                dictionary["message"] = ex.GetErrorMessage().ToSafeString();
            }

            return dictionary;

        }
        public ActionResult ReCalDistanceFapoReportInstallationfile_Save(IEnumerable<HttpPostedFileBase> RecalfapoReportInstallationfile)
        {
            if (RecalfapoReportInstallationfile != null)
            {
                try
                {
                    foreach (var file in RecalfapoReportInstallationfile)
                    {
                        if (Path.GetExtension(file.FileName).ToLower() == ".csv")
                        {
                            // Read bytes from http input stream
                            BinaryReader b = new BinaryReader(file.InputStream);
                            byte[] binData = b.ReadBytes(file.ContentLength);
                            reportInstallFileModel.csv = System.Text.Encoding.Default.GetString(binData);

                        }
                        else
                        {
                            var modelResponse = new { status = false, message = "Please upload .csv file extension", fileName = file.FileName };
                            return Json(modelResponse, "text/plain");
                        }
                    }

                }
                catch (Exception e)
                {
                    var modelResponse = new { status = false, message = e.GetErrorMessage(), fileName = "" };
                    return Json(modelResponse, "text/plain");
                }

            }

            var successResponse = new { status = true };
            return Json(successResponse, "text/plain");
        }

        public ActionResult ReCalDistanceFapoReportInstallationfile_Remove(string[] fapoReportInstallationfile)
        {
            if (fapoReportInstallationfile != null)
            {
                try
                {
                    reportInstallFileModel.csv = "";
                }
                catch (Exception e)
                {
                    return Content(e.GetErrorMessage());
                }
            }

            var modelResponse = new { status = false, message = "Please upload file." };
            return Json(modelResponse, "text/plain");
        }


        #endregion RecalByFile

        #endregion ReportInstallation Re-Cal-Distance

        public ActionResult ExportTemplate(string type)
        {

            if ((type.ToUpper().Replace(" ", "").Trim() == "RECHECK") || (type.ToUpper().Replace(" ", "").Trim() == "COMPLETED"))
            {

                return RedirectToAction("ExportTemplateRecal");
            }
            else if (type.ToUpper().Replace(" ", "").Trim() == "DISPUTE")
            {
                return RedirectToAction("ExportTemplateDispute");
            }
            return null;

        }

        public ActionResult ExportTemplateRecal()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplateRecalByFile.csv";
                byte[] bytes = dlf.ExportExcelTemplate(filename);
                return File(bytes, MediaTypeNames.Application.Octet, filename);
            }
            catch (Exception ex)
            {
                return null;
            }


        }
        public ActionResult ExportTemplateDispute()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplateDisputeByFile.csv";
                byte[] bytes = dlf.ExportExcelTemplate(filename);
                return File(bytes, MediaTypeNames.Application.Octet, filename);

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public ActionResult ExportTemplateConfirmPaid()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplateComfirmPaidByFile.csv";
                byte[] bytes = dlf.ExportExcelTemplate(filename);
                return File(bytes, MediaTypeNames.Application.Octet, filename);

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public ActionResult ExportTemplateValidateDistance()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplateValidateDistanceByFile.csv";
                byte[] bytes = dlf.ExportExcelTemplate(filename);
                return File(bytes, MediaTypeNames.Application.Octet, filename);

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public ActionResult ExportTemplatePaid()
        {
            try
            {
                Commone dlf = new Commone();
                string filename = "TemplatePaidByFile.csv";
                byte[] bytes = dlf.ExportExcelTemplate(filename);
                return File(bytes, MediaTypeNames.Application.Octet, filename);

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private InterfaceLogPayGCommand StartInterface<T>(T query, string methodName, string transactionId, string idCardNo)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = transactionId,
                METHOD_NAME = methodName,
                SERVICE_NAME = "Update Status Subcontract Payment",
                IN_ID_CARD_NO = idCardNo,
                IN_XML_PARAM = query.DumpToXml(),
                INTERFACE_NODE = "Subcontract Payment",
                CREATED_BY = "LMD",
            };
            _intfLogAPICommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }
        private void EndInterface<T>(T output, InterfaceLogPayGCommand dbIntfCmd, string transactionId, string result, string reason)
        {
            if (null == dbIntfCmd)
                return;

            switch (result)
            {
                case "Missing parameter":
                    break;
                case "Invalid Parameter":
                    break;
                case "System Error":
                    break;
                case "Success":
                    break;
                default:
                    result = "Failed";
                    break;
            }

            dbIntfCmd.ActionType = ActionType.Update;
            dbIntfCmd.REQUEST_STATUS = (!result.Contains("System")) ? "Success" : "Failed";
            dbIntfCmd.OUT_RESULT = (result == "Success") ? "Success" : "Failed";
            dbIntfCmd.OUT_ERROR_RESULT = result;
            dbIntfCmd.OUT_XML_PARAM = output.DumpToXml();
            dbIntfCmd.UPDATED_BY = "LMD";

            _intfLogAPICommand.Handle(dbIntfCmd);
        }

        private class warning
        {
            internal static readonly string Missing = "Missing parameter";
            internal static readonly string Invalid = "Invalid Parameter";
            internal static readonly string System = "System Error";
            internal static readonly string Failedcode = "0001";
            internal static readonly string Successcode = "0000";
        }

        public class SubAccNo
        {
            public string Accno { get; set; }
        }
        public class LogSentEmail
        {
            public List<SubAccNo> AccNo { get; set; }
            public string emailsub { get; set; }
            public string subname { get; set; }
            public string usename { get; set; }
            public string senddate { get; set; }
            public string SubCount { get; set; }
            public string status { get; set; }
        }

    }



}
