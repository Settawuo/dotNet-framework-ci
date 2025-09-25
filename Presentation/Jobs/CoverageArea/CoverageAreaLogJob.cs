using System;
using System.Collections.Generic;
using System.Linq;

namespace CoverageAreaLog
{
    using System.Configuration;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Commands.FBSS;
    using WBBContract.Queries.Commons.Masters;
    //using WBBEntity.PanelModels.WebServiceModels;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;

    public class CoverageAreaLogJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<CoverageAreaLogCommand> _CoverageAreaLog;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private Stopwatch _timer;

        public CoverageAreaLogJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<CoverageAreaLogCommand> CoverageAreaLog,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _CoverageAreaLog = CoverageAreaLog;
            _sendMail = sendMail;
        }

        public void Execute()
        {
            try
            {
                _logger.Info("Coverage Area Log.");
                StartWatching();
                var command = new CoverageAreaLogCommand
                {
                };

                _CoverageAreaLog.Handle(command);
                //command.Return_Message = "Test Send mail";


                _logger.Info(string.Format("Coverage Area Log : {0}", command.Return_Message));

                StopWatching("Coverage Area Log");

                //Send mail when not success. 
                //Change to check from Return_Code 0 for success.
                if (command.Return_Code.ToSafeDecimal() != 0)
                {
                    if (SendEmailError)
                        SendEmail(command.Return_Message);
                    else
                        _logger.Info("Coverage Area Log : Error without Sending an Email.");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Coverage Area Log :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("Coverage Area Log");

                SendEmail(ex.GetErrorMessage());
            }

        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string process)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", process, _timer.Elapsed));
        }

        public void SendEmail(string errorString)
        {
            _logger.Info("Sending an Email.");
            StartWatching();

            string FromPass = "";// Fixed Code scan : string FromPassword = "";
            string Port = "";
            string Domaim = "";
            string IPMailServer = "";
            var vMailServer = GetLovList("FBBDORM_CONSTANT", "").Where(p => p.Name.Contains("VAR") && p.LovValue5 == "FBBDORM010").ToList();
            if (vMailServer != null && vMailServer.Count > 0)
            {
                foreach (var key in vMailServer)
                {
                    switch (key.Name)
                    {
                        case "VAR_FROM_PASSWORD":
                            FromPass = key.LovValue1;
                            break;
                        case "VAR_HOST":
                            IPMailServer = key.LovValue1;
                            break;
                        case "VAR_PORT":
                            Port = key.LovValue1;
                            break;
                        case "VAR_DOMAIN":
                            Domaim = key.LovValue1;
                            break;
                    }
                }
            }
            else
            {
                FromPass = "V9!@M#V2zf@Q";
                IPMailServer = "10.252.160.41";
                Port = "25";
                Domaim = "corp.ais900dev.org";
            }

            string strFrom = "";
            string strTo = "";
            string strCC = "";
            string strBCC = "";

            string strSubject = "";
            string strBody = "";
            var MailDetail = GetLovList("FBB_CONSTANT", "SEND_EMAIL_ERROR_CASE").Where(p => p.Text == "EMAIL_ALERT").FirstOrDefault();
            if (MailDetail == null || MailDetail.LovValue1.ToSafeString() == "" || MailDetail.LovValue2.ToSafeString() == "")
            {
                strSubject = string.Format("{0} is Failed", ModeSendEmail);
                strBody = "Please verify this error : " + errorString;
            }
            else
            {
                strSubject = string.Format(MailDetail.LovValue1, ModeSendEmail);
                strBody = MailDetail.LovValue2 + " " + errorString;
            }

            try
            {
                var command = new SendMailBatchNotificationCommand
                {
                    ProcessName = "SEND_EMAIL_ERROR_CASE",
                    CreateUser = "Batch",
                    SendTo = strTo,
                    SendCC = strCC,
                    SendBCC = strBCC,
                    SendFrom = strFrom,
                    Subject = strSubject,
                    Body = strBody,
                    FromPassword = FromPass,
                    Port = Port,
                    Domaim = Domaim,
                    IPMailServer = IPMailServer
                };

                _sendMail.Handle(command);

                _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                StopWatching("Send an Email");

            }
            catch (Exception ex)
            {
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Send an Email");
            }
            //return result;
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
                _logger.Info(string.Format("Error GetLovList [Type({0}), Name({1})]: {2}", type, name, ex.GetErrorMessage()));
                return new List<LovValueModel>();
            }
        }

        public static string ModeSendEmail
        {
            get { return string.Format("[{0}-Batch] ConverageArea", ConfigurationManager.AppSettings["ModeSendEmail"].ToSafeString()); }
        }

        public static bool SendEmailError
        {
            get
            {
                var result = false;
                bool.TryParse(ConfigurationManager.AppSettings["SendEmailError"], out result);
                return result;
            }
        }
    }
}
