using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Threading;
using System.Globalization;
using System.Text.RegularExpressions;
using FBBPayGLastMileBydistanceBatch.CompositionRoot;
using System.Drawing;
using OfficeOpenXml.Style;
using WBBEntity.PanelModels;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using System.Diagnostics;

namespace FBBPayGLastMileBydistanceBatch
{
    class FBBPayGLastMileBydistanceBatchJob
    {
        private readonly ILogger _Logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendMailLastMileNotificationCommand> _sendMail;
        private readonly ICommandHandler<SendMailLastMileCommand> _SendMailLastMileExp;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private Stopwatch _timer;

        public FBBPayGLastMileBydistanceBatchJob(ILogger logger, IQueryProcessor queryProcessor
            , ICommandHandler<SendMailLastMileNotificationCommand> sendMail
            , ICommandHandler<SendMailLastMileCommand> SendMailLastMileExp
            , ICommandHandler<InterfaceLogCommand> intfLogCommand)
        {
            _Logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
            _SendMailLastMileExp = SendMailLastMileExp;
            _intfLogCommand = intfLogCommand;
        }
        public async Task ManagerAsync()
        {
            //--------------------Start: LastMileByditance ------------------------------

            try
            {
                _Logger.Info("LastmilebydistanceBatch start.");
                StartWatching();

                var result = GetData();
                if (result != null || result.Count > 0)
                {
                    var _dateAPPROVE = APPROVE_FM_TO();
                    // ------------------------Start : set data for SendEmail --------------------------------

                    string CS_APPROVE_DATE_FROM = _dateAPPROVE.dateFM;
                    string CS_APPROVE_DATE_TO = _dateAPPROVE.dateTO.ToString();
                    string STRING_CS_APPROVE_DATE = _dateAPPROVE.period;
                    string CS_DOC_DATE = _dateAPPROVE.dateDoc;
                    string CS_PAYMENT_DATE = _dateAPPROVE.datePay;

                    foreach (var datupd in result)
                    {
                        string[] SplitName = datupd.ACCOUNT_NAME.Split(' ');
                        if (string.IsNullOrEmpty(datupd.PERIOD))
                        {
                            datupd.PERIOD = STRING_CS_APPROVE_DATE;
                        }

                        datupd.ACCOUNT_NAME = SplitName[0].ToSafeString() + " XXX".ToSafeString();

                    }

                    //// ------------------------End : set data for SendEmail --------------------------------


                    ////update preriod and order status    
                    var updatePeriod = SendMailUpdateOrderStatus(result, _dateAPPROVE);

                    updatePeriod.Wait();

                    if (result.Count > 0)
                    {
                        // await Task.Run(() => );
                        var resultSendMail = CreateDataToDataTable(result
                                 , CS_APPROVE_DATE_FROM
                                 , CS_APPROVE_DATE_TO
                                 , STRING_CS_APPROVE_DATE
                                 , CS_DOC_DATE
                                 , CS_PAYMENT_DATE
                                 , "Batch Auto LMD");
                    }
                }
                StopWatching();
            }
            catch (Exception ex)
            {
                _Logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                StopWatching();
            }

        }

        private List<LastMileByDistanceOrderReturn> GetData()
        {
            try
            {
                var CFGqueryReportQuery = new GetCFGqueryReportQuery
                {
                    query_id = 251,
                    report_id = 1
                };
                var _getCFGqueryReportQuery = _queryProcessor.Execute(CFGqueryReportQuery);
                if (_getCFGqueryReportQuery != null || _getCFGqueryReportQuery.Count > 0)
                {
                    var query = new GetLastMileByDistanceOrderListQuery
                    {
                        p_Command = _getCFGqueryReportQuery.FirstOrDefault().query_1.ToString()
                    };

                    return _queryProcessor.Execute(query);
                }
                else
                {
                    _Logger.Info($"FBBPayGLastMileBydistanceBatchJob Data not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _Logger.Info($"FBBPayGLastMileBydistanceBatchJob getData ERROR :{ex.GetErrorMessage()}");
                return null;
            }
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching()
        {
            _timer.Stop();
            _Logger.Info("PAYG start FBBPayGLastMileBydistanceBatchJob take : " + _timer.Elapsed);
        }
        private async Task<string> SendMailUpdateOrderStatus(List<LastMileByDistanceOrderReturn> data, APPROVEFMTO aPPROVEFMTO)
        {
            string result = "";
            try
            {

                List<FixOrderListTmp> FixdeOrderList = new List<FixOrderListTmp>();
                FixOrderListTmp OrderListModel = new FixOrderListTmp();
                string CS_APPROVE_DATE_FROM = aPPROVEFMTO.dateFM.ToString();
                string CS_APPROVE_DATE_TO = aPPROVEFMTO.dateTO.ToString();

                foreach (var resule in data)
                {
                    OrderListModel = new FixOrderListTmp();
                    OrderListModel.ACCESS_NUMBER = resule.ACCESS_NUMBER.ToSafeString();
                    FixdeOrderList.Add(OrderListModel);
                }

                var SendExpComand = new SendMailLastMileCommand
                {
                    p_USER = "Batch LMD",
                    p_period_from = CS_APPROVE_DATE_FROM,
                    p_period_to = CS_APPROVE_DATE_TO,
                    fixed_order = FixdeOrderList,
                };

                _SendMailLastMileExp.Handle(SendExpComand);
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

        private APPROVEFMTO APPROVE_FM_TO()
        {
            try
            {
                var _DayOfWeek = (int)DateTime.Now.DayOfWeek;

                CultureInfo customCulture = new CultureInfo("en-US", true);
                customCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
                System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
                DateTime newDate = System.Convert.ToDateTime(DateTime.Now.ToString("dd/MM/yyyy"));
                var dateFM = _DayOfWeek == 1 ? newDate.AddDays(-9) : newDate.AddDays(-(9 + (_DayOfWeek - 1)));
                var dateTO = _DayOfWeek == 1 ? newDate.AddDays(-3) : newDate.AddDays(-(3 + (_DayOfWeek - 1)));

                var dataDoc = DateTime.Now.AddDays(2);
                var datePay = dataDoc.AddDays(30);

                var refdatefmto = new APPROVEFMTO
                {
                    dateFM = dateFM.ToString("dd/MM/yyyy", customCulture),
                    dateTO = dateTO.ToString("dd/MM/yyyy", customCulture),
                    period = GetPeriodDate(dateFM, dateTO),
                    dateDoc = dataDoc.ToString("dd/MM/yyyy", customCulture),
                    datePay = datePay.ToString("dd/MM/yyyy", customCulture)

                };
                return refdatefmto;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }

        private string GetPeriodDate(DateTime dateFM, DateTime dateTO)
        {

            CultureInfo customCulture = new CultureInfo("th-TH", true);
            customCulture.DateTimeFormat.ShortDatePattern = "dd MMM yy";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = customCulture;
            DateTime date_FM = System.Convert.ToDateTime(dateFM.ToString("dd MMM yy"));
            DateTime date_TO = System.Convert.ToDateTime(dateTO.ToString("dd MMM yy"));

            var dateThai = $"{dateFM.ToString("dd MMM yy")} - {dateTO.ToString("dd MMM yy")}";
            return dateThai;

        }

        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _Logger.Info(ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }

        private object CreateDataToDataTable(List<LastMileByDistanceOrderReturn> dataQueryNotNull,
            string cS_APPROVE_DATE_FROM,
            string cS_APPROVE_DATE_TO, string sTRING_CS_APPROVE_DATE,
            string cS_DOC_DATE, string cS_PAYMENT_DATE, string cS_REMARK)
        {
            //=============Lov Config 
            var LovWorkStatus = GetLovList("WF_STATUS", "").Where(l => l.LovValue3 == "SEND_MAIL" && l.LovValue5 == "FIXED_LASTMILE").ToList();
            string LovCompleted = LovWorkStatus.Where(c => c.OrderBy == 1).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovNodata = LovWorkStatus.Where(c => c.OrderBy == 2).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovRecheck = LovWorkStatus.Where(c => c.OrderBy == 3).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            InterfaceLogCommand log = null;
            string fileName = "";

            Task<string> parthfilename;
            string SUBCONTRACT_NAME = "";
            string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            var groupedByPeriodList = dataQueryNotNull
                       .GroupBy(u => u.PERIOD)
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
            dtComplete.Columns.Add("Total Paid", System.Type.GetType("System.String"));
            dtComplete.Columns.Add("ค่าแรกเข้า(Entry Fee)รวม VAT", System.Type.GetType("System.String"));
            DataTable dtReCheck = table.Clone();
            dtReCheck.TableName = LovRecheck;
            DataTable dtNoData = table.Clone();
            dtNoData.TableName = LovNodata;
            #endregion
            int successcount = 0;
            int errorcount = 0;
            int indexNoComp = 1;
            int indexNoRecheck = 1;
            int indexNoNoData = 1;
            string SUBCONTRACT_EMAIL = "";
            string emaillogstatus = string.Empty;
            List<LogSentEmail> emaillog = new List<LogSentEmail>();

            //var subcontractors = _queryProcessor.Execute(new GetSubcontractorQuery());

            int subcount = 0;
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
                        for (int j = 0; j <= groupBySubName.Count - 1; j++) // แยก subcontect
                        {
                            List<SubAccNo> SubAccNolist = new List<SubAccNo>();
                            var groupByOrderStatus = groupBySubName[j];


                            if (groupByOrderStatus.Count > 0)
                            {
                                for (int k = 0; k <= groupByOrderStatus.Count - 1; k++)
                                {

                                    fileName = GetFileNameExcel(groupByOrderStatus[k].ORG_ID.ToSafeString()
                                        , cS_APPROVE_DATE_FROM, cS_APPROVE_DATE_TO
                                        , sTRING_CS_APPROVE_DATE);

                                    SUBCONTRACT_NAME = groupByOrderStatus[k].SBC_CPY.ToSafeString();

                                    if (!string.IsNullOrEmpty(groupByOrderStatus[k].subcontract_email))
                                    {
                                        SUBCONTRACT_EMAIL = groupByOrderStatus[k].subcontract_email.Trim();
                                    }

                                    if (!string.IsNullOrEmpty(groupByOrderStatus[k].ORDER_STATUS.ToSafeString()))
                                    {

                                        if (groupByOrderStatus[k].ORDER_STATUS.ToUpper() == LovCompleted)
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
                                            values[9] = string.Format("{0:n2}", groupByOrderStatus[k].TOTAL_PAID.ToSafeDecimal());
                                            values[10] = string.Format("{0:n2}", groupByOrderStatus[k].ENTRY_FEE.ToSafeDecimal());
                                            summary += Convert.ToInt32(groupByOrderStatus[k].TOTAL_PAID.ToSafeDecimal());
                                            dtComplete.Rows.Add(values);
                                            indexNoComp++;
                                            #endregion

                                        }

                                        #region commant Orderstatus 
                                        else if (groupByOrderStatus[k].ORDER_STATUS.ToUpper() == LovRecheck)
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
                                        else if (groupByOrderStatus[k].ORDER_STATUS.ToUpper() == LovNodata)
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
                                            //  values[8] = groupByOrderStatus[k].NOTE.ToSafeString();
                                            dtNoData.Rows.Add(values);
                                            indexNoNoData++;
                                            #endregion

                                        }
                                        #endregion

                                    }

                                    var accNOModels = new SubAccNo
                                    {
                                        Accno = groupByOrderStatus[k].ACCESS_NUMBER.ToSafeString()
                                    };
                                    SubAccNolist.Add(accNOModels);
                                }

                                // insert row values                           
                                decimal sumLastMile = 0;
                                foreach (DataRow dr in dtComplete.Rows)
                                {
                                    if (!string.IsNullOrEmpty(dr[9].ToSafeString()))
                                    {
                                        sumLastMile += Convert.ToDecimal(dr[9].ToSafeString());
                                    }
                                }

                                dtComplete.Rows.Add(new Object[]{
                                    "","","","","","","","","ยอดรวม",string.Format("{0:n2}", sumLastMile),""

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
                                parthfilename = GenerateExcel(ds, fileName);

                                parthfilename.Wait();

                                strArr[0] = parthfilename.Result;
                                strArr[4] = fileName;

                                //send email to sub contract
                                var resultSendMail = sendEmailByAttachFile(fileName
                                        , SUBCONTRACT_NAME
                                        , sTRING_CS_APPROVE_DATE
                                        , strArr
                                        , cS_DOC_DATE
                                        , cS_PAYMENT_DATE
                                        , cS_REMARK
                                        , SUBCONTRACT_EMAIL);

                                resultSendMail.Wait();
                                String chkstatus = resultSendMail.Result.ToSafeString();

                                if (String.IsNullOrEmpty(chkstatus) == true)
                                {
                                    emaillogstatus = "Success";
                                    successcount += 1;
                                }
                                else
                                {
                                    emaillogstatus = chkstatus;
                                    errorcount += 1;

                                }
                                if (resultSendMail.IsCompleted)
                                {
                                    Task.FromResult(resultSendMail.Result);

                                    subcount += 1;
                                    var logEmailModels = new LogSentEmail
                                    {
                                        AccNo = SubAccNolist,
                                        emailsub = SUBCONTRACT_EMAIL,
                                        subname = SUBCONTRACT_NAME,
                                        usename = "",
                                        senddate = DateNow,
                                        SubCount = subcount.ToSafeString(),
                                        status = emaillogstatus
                                    };
                                    emaillog.Add(logEmailModels);
                                }
                                else
                                {
                                    resultSendMail.ContinueWith(
                                    t => t.Result,
                                    CancellationToken.None,
                                    TaskContinuationOptions.ExecuteSynchronously,
                                    TaskScheduler.Default);
                                    subcount += 1;
                                    var logEmailModels = new LogSentEmail
                                    {
                                        AccNo = SubAccNolist,
                                        emailsub = SUBCONTRACT_EMAIL,
                                        subname = SUBCONTRACT_NAME,
                                        usename = "".ToSafeString(),
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
                    //log = StartInterface(emaillog, "SendEmailtoSub", "", null, "SendEmailtoSub", "Start SendEmailtoSub");
                    //EndInterface("", log, null, ResultLog, "Send EmailComplete", "", CurrentUser.UserName.ToSafeString());

                    // here for log
                }
            }


            return "";
        }
        private async Task<string> sendEmailByAttachFile(string fileName, string SUBCONTRACT_NAME
                    , string STRING_CS_APPROVE_DATE, string[] LIST_FILE, string CS_DOC_DATE, string CS_PAYMENT_DATE, string CS_REMARK
                    , string sub_contact_email)
        {
            try
            {
                string result = "";
                string body = "";
                List<MemoryStreamAttachFiles> files = new List<MemoryStreamAttachFiles>();
                if (ScmOrderListModelBySendMail.msExcel == null)
                {
                    files = null;
                }
                else
                {
                    MemoryStreamAttachFiles attachFile = new MemoryStreamAttachFiles();
                    attachFile.file = ScmOrderListModelBySendMail.msExcel;
                    attachFile.fileName = fileName + ".xlsx";
                    files.Add(attachFile);
                }

                body = EmailTemplate(SUBCONTRACT_NAME, STRING_CS_APPROVE_DATE, LIST_FILE, CS_DOC_DATE, CS_PAYMENT_DATE, CS_REMARK);


                var command = new SendMailLastMileNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_FIXED_LASTMILE",
                    Subject = "ใบ certificate รอบการเบิก..." + STRING_CS_APPROVE_DATE + " " + SUBCONTRACT_NAME,
                    Body = body.ToString(),
                    msAttachFiles = files,
                    SendTo = sub_contact_email.ToSafeString()
                };
                _sendMail.Handle(command);

                _Logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                if (command.ReturnMessage == "Success.")
                {
                    result = "Success";
                }
                else
                {
                    result = command.ReturnMessage;
                }

                return result;
            }
            catch (Exception Ex)
            {

                return Ex.Message.ToSafeString();

            }


        }

        public string EmailTemplate(string SUBCONTRACT_NAME, string STRING_CS_APPROVE_DATE, string[] str, string CS_DOC_DATE, string CS_PAYMENT_DATE, string CS_REMARK)
        {

            try
            {
                //var impersonateVar = LovData.Where(l => l.Type == "MAIL_CONFIG" && l.LovValue5 == "FIXED_LASTMILE").ToList();
                var impersonateVar = GetLovList("MAIL_CONFIG", "").Where(l => l.Type == "MAIL_CONFIG" && l.LovValue5 == "FIXED_LASTMILE").ToList();
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


        private async Task<string> GenerateExcel(DataSet dataToExcel, string fileName)
        {
            //=============Lov Config 

            var LovWorkStatus = GetLovList("WF_STATUS", "").Where(l => l.LovValue3 == "SEND_MAIL" && l.LovValue5 == "FIXED_LASTMILE").ToList();
            string LovCompleted = LovWorkStatus.Where(c => c.OrderBy == 1).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovNodata = LovWorkStatus.Where(c => c.OrderBy == 2).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();
            string LovRecheck = LovWorkStatus.Where(c => c.OrderBy == 3).Select(x => x.LovValue1).FirstOrDefault().ToSafeString().ToUpper();


            string parthfilename = (fileName.Trim() + ".xlsx");
            var physicalPath = "";

            if (parthfilename != null && Path.GetExtension(parthfilename).ToLower() == ".xlsx")
            {
                var impersonateVar = GetLovList("CONFIG_PATH", "").SingleOrDefault(l => l.Type == "CONFIG_PATH" && l.Text == "ATTACH" && l.Name == "MAIL");
                if (impersonateVar != null)
                {
                    string user = impersonateVar.LovValue1;
                    string pass = impersonateVar.LovValue2;
                    string ip = impersonateVar.LovValue3;
                    string directoryPath = "";
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
                        //   {

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

                                        TxtHearder = "ไม่พบข้อมูลในระบบ  แจ้งให้ทางตัวแทนใช้โปรแกรม Last Mile Route  ";
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
                            ScmOrderListModelBySendMail.FileData = package.GetAsByteArray();
                        }

                        ScmOrderListModelBySendMail.msExcel = new MemoryStream(ScmOrderListModelBySendMail.FileData);

                        if (SaveFileNY.ToUpper() == "Y")
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                ms.CopyTo(new MemoryStream(ScmOrderListModelBySendMail.FileData));
                                ms.Seek(0, SeekOrigin.Begin);

                                if (!string.IsNullOrEmpty(user))
                                {
                                    using (var impersonator = new Impersonator(user, ip, pass, false))
                                    {
                                        using (FileStream fs = new FileStream(physicalPath, FileMode.OpenOrCreate))
                                        {
                                            new MemoryStream(ScmOrderListModelBySendMail.FileData).CopyTo(fs);
                                            fs.Flush();

                                        }
                                    }

                                }
                                else
                                {

                                    using (FileStream fs = new FileStream(physicalPath, FileMode.OpenOrCreate))
                                    {
                                        new MemoryStream(ScmOrderListModelBySendMail.FileData).CopyTo(fs);
                                        fs.Flush();

                                    }
                                }
                            }

                        }
                    }
                    catch (Exception exs)
                    {
                        _Logger.Info("Error GenExcel LastMile Sendmail" + exs.GetBaseException());
                        return "Error GenExcel LastMile Sendmail" + exs.Message.ToString();
                    }
                }
            }
            else
            {

            }
            return physicalPath;

        }


        private string GetFileNameExcel(string ORG_ID, string CS_APPROVE_DATE_FROM, string CS_APPROVE_DATE_TO, string STRING_CS_APPROVE_DATE)
        {
            try
            {
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();

                //38645_20201107_20201113_20201116103326_kiranee


                CS_APPROVE_DATE_FROM = DateTime.ParseExact(CS_APPROVE_DATE_FROM, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
                CS_APPROVE_DATE_TO = DateTime.ParseExact(CS_APPROVE_DATE_TO, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

                string fileName = ORG_ID + "_" + CS_APPROVE_DATE_FROM + "_" + CS_APPROVE_DATE_TO + "_" + DateNow;
                //string fileName = "Excel" + DateNow;
                return (fileName + "_LMD Auto").Trim();
            }
            catch (Exception ex)
            {
                _Logger.Info("Error Last mile Send mail GetFileNameExcel" + ex.GetBaseException());
                return ex.GetBaseException().ToString();
            }

        }

        public async Task<LastMileByDistanceOrderListReturn> getAllRecord(SearchLastMileByDistanceOrderListQuery query)
        {
            string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
            _Logger.Info("StartGetAllRecord" + DateNow.ToSafeString());
            var result = _queryProcessor.Execute(query);
            foreach (var item in result.cur)
            {
                item.INVOICE_AMOUNT_BFVAT = "";
                item.INVOICE_AMOUNT_VAT = getVatRATE(item.TOTAL_PAID);
                item.TOTAL_INVOICE_AMOUNT_VAT = string.Format("{0:#,0.00}", (item.TOTAL_PAID + getVatRATE(item.TOTAL_PAID)));
                item.CS_APPROVE_DATE_TEXT = item.CS_APPROVE_DATE.ToDateDisplayText();
                item.INVOICE_DATE_TEXT = item.INVOICE_DATE.ToDateDisplayText();
                item.LAST_UPDATE_DATE_TEXT = item.LAST_UPDATE_DATE.ToDateDisplayText();
                item.ORDER_STATUS_DT_TEXT = item.ORDER_STATUS_DT.ToDateDisplayText();
                item.APPOINTMENNT_DT_TEXT = item.APPOINTMENNT_DT.ToDateDisplayText();
                item.EFFECTIVE_END_DT_TEXT = item.EFFECTIVE_END_DT.ToDateDisplayText();
                item.SFF_ACTIVE_DATE_TEXT = item.SFF_ACTIVE_DATE.ToDateDisplayText();
                item.SFF_SUBMITTED_DATE_TEXT = item.SFF_SUBMITTED_DATE.ToDateDisplayText();
                item.FOA_SUBMIT_DATE_TEXT = item.FOA_SUBMIT_DATE.ToDateDisplayText();
                item.PAID_DATE_TEXT = item.PAID_DATE.ToDateDisplayText();
            }
            return result;

        }

        public decimal? getVatRATE(decimal? totalvalue)
        {
            decimal vat_value = 7;
            //decimal? bfvat = 0;
            decimal? bfvat = 0;
            var callvat = SelectFbbCfgLov("VAT_RATE", "FIXED_LASTMILE").FirstOrDefault();

            if (callvat != null)
            {
                vat_value = Convert.ToInt16(callvat.DISPLAY_VAL.ToSafeString());
            }
            bfvat = ((vat_value * totalvalue) / 100);
            //   double dc = Math.Round(Convert.ToDouble(bfvat), 2);
            return bfvat;
        }
        private List<LovModel> SelectFbbCfgLov(string lov_type, string lov_val5)
        {
            var query = new SelectLovByTypeAndLovVal5Query
            {
                LOV_TYPE = lov_type,
                LOV_VAL5 = lov_val5
            };
            return _queryProcessor.Execute(query);
        }
    }

    public class APPROVEFMTO
    {
        public string dateFM { get; set; }
        public string dateTO { get; set; }
        public string period { get; set; }
        public string dateDoc { get; set; }
        public string datePay { get; set; }
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

    public class SubAccNo
    {
        public string Accno { get; set; }
    }
}
