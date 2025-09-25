using System;
using System.Collections.Generic;
using System.Linq;


namespace FBSSOM010Notification
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Text;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    //using CompositionRoot;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    public class FBSSOM010NotificationBatchJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        public FBSSOM010NotificationBatchJob(
             ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail
           )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
        }
        public string getFlag()
        {
            string FlagEmail = "N";
            var Data = SelectFbbCfgLov("FBB_BATCH_EMAIL", "FBSS_OM010_NOTIFY").FirstOrDefault();
            if (Data != null)
            {
                FlagEmail = Data.LOV_VAL4.ToSafeString();
            }
            return FlagEmail;
        }

        public string EmailTemplate(string MSG)
        {

            try
            {
                var data = SelectFbbCfgLov("FBB_BATCH_EMAIL", "FBSS_OM010_NOTIFY").FirstOrDefault();


                string SubjectDTL = data.LOV_VAL1.ToSafeString();

                StringBuilder tempBody = new StringBuilder();
                CultureInfo ThaiCulture = new CultureInfo("th-TH");
                CultureInfo UsaCulture = new CultureInfo("en-US");



                tempBody.Append("<p style='font-weight:bold;'></span>&nbsp;" + SubjectDTL);
                tempBody.Append("<br/>");
                tempBody.Append("</span> File OM010 is Fail:" + MSG.ToSafeString());
                tempBody.Append("<br/>");


                tempBody.Append("<br/><br/><br/>");

                tempBody.Append("<br/>");
                tempBody.Append("<span>");

                tempBody.Append("<br/></span></span></span></span>Best & Regard");




                string body = "";
                body = tempBody.ToSafeString();
                return body;
            }
            catch (Exception ex)
            {
                // _Logger.Info("Error last mile send mail Menthod EmailTemplate: " + ex.GetErrorMessage());
                return ex.GetErrorMessage();
            }
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
        private string sendEmail(string MSG)
        {
            string SendFrom = "";
            string SendTo = "";
            string SendCC = "";
            string SendBCC = "";
            string IPMailServer = "";
            string FromPass = "";// Fixed Code scan : string FromPassword = "";
            string Port = "";
            string Domaim = "";
            string[] result = new string[2];
            var data = SelectFbbCfgLov("FBB_BATCH_EMAIL", "FBSS_OM010_NOTIFY").FirstOrDefault();
            List<MemoryStreamAttachFiles> files = new List<MemoryStreamAttachFiles>();


            string SUBJECT = data.LOV_VAL1.ToSafeString();
            string SENDTO = data.LOV_VAL2.ToSafeString();
            string SENDCC = data.LOV_VAL3.ToSafeString();

            string emailMsg = "";
            string body = "";
            try
            {

                body = EmailTemplate(MSG);


                var command = new SendMailBatchNotificationCommand
                {
                    ProcessName = "FBSS_OM010_NOTIFY_BATCH",
                    CreateUser = "",
                    SendTo = SENDTO,
                    SendFrom = SendFrom,
                    Subject = SUBJECT,
                    Body = body,
                    FromPassword = FromPass,
                    Port = Port,
                    Domaim = Domaim,
                    IPMailServer = IPMailServer,
                    msAttachFiles = files
                };


                _sendMail.Handle(command);

                if (command.ReturnMessage == "Success.")
                {
                    result[0] = "0";
                    result[1] = "";
                }
                else
                {
                    result[0] = "-1";
                    result[1] = command.ReturnMessage;
                }


            }
            catch (Exception e)
            {

                emailMsg = e.Message;
            }
            return emailMsg;

        }
        public void SendEmail(string errormsg)
        {
            if (getFlag() == "Y")
            {
                _logger.Info("Flag Y sendEmail");
                sendEmail(errormsg);
            }
            else
            {
                _logger.Info("Flag N NotSendEmail");
            }

        }
        public void ExecuteSendEmail()
        {
            StartWatching();
            _logger.Info("Start Execute.");
            try
            {
                _logger.Info("checkstatusbyCode.");
                //   var query = new GetOM010EMailNotifyQuery() { };
                ReturnFBSSOM010Notify Dataresult = _queryProcessor.Execute(new GetOM010EMailNotifyQuery());
                if (Dataresult != null)
                {

                    string valuecode = string.Empty; string valuemsg = string.Empty;
                    valuecode = Dataresult.ret_code.ToSafeString();
                    valuemsg = Dataresult.msg.ToSafeString();
                    _logger.Info("pkg_fbbpayg_load_om010.p_check_load_om." + valuecode);
                    if (valuecode == "2")
                    //if (valuecode == "0")
                    {
                        _logger.Info("sendEmail & FileOM010Status :" + valuemsg);
                        SendEmail(valuemsg);
                    }
                    else
                    {
                        _logger.Info("notsendemail");
                        _logger.Info("FileOM010Status :" + valuemsg);
                    }
                }


            }
            catch (Exception ex)
            {
                string errormsg = ex.ToSafeString();
                _logger.Info("Execute Error...");
                _logger.Info(errormsg);


            }
            finally
            {
                StopWatching();
            }

        }
        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("FBSSOM010Notification take : " + _timer.Elapsed);
        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string Message)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", Message, _timer.Elapsed));

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
                _logger.Info("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }
    }
}
