using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Security.Principal;
using WBBBusinessLayer.QueryHandlers;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class NotificationCommandHandler : ICommandHandler<NotificationCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService;
        private readonly IEntityRepository<FBB_REGISTER> _regService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;

        public NotificationCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService,
            IEntityRepository<FBB_SENDMAIL_LOG> emailLogService,
            IEntityRepository<FBB_REGISTER> regService, IEntityRepository<FBB_INTERFACE_LOG> intfLog)
        {
            _logger = logger;
            _uow = uow;
            _lovService = lovService;
            _emailProcService = emailProcService;
            _emailLogService = emailLogService;
            _regService = regService;
            _intfLog = intfLog;
        }

        public void Handle(NotificationCommand command)
        {
            InterfaceLogCommand log = null;
            var sendMailLogMessage = "";
            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, command, command.RunningNo.ToSafeString(), "NotificationCommandHandler", "NotificationCommandHandler", "", "FBB|Sendemail", "");

                var love = _lovService.Get(l => l.LOV_TYPE.Equals("EMAIL"));
                var cust = _regService
                    .Get(c => (!string.IsNullOrEmpty(c.ROW_ID) && c.ROW_ID == command.CustomerId));
                if (!cust.Any())
                {
                    _logger.Info("Customer is null with rowid = " + command.CustomerId);
                    return;
                }

                var custIdCardNum = cust.Select(cus => cus.CUST_ID_CARD_NUM).FirstOrDefault() ?? string.Empty;
                var costIdCardNumLast = string.Empty;
                if (!string.IsNullOrEmpty(custIdCardNum) && custIdCardNum.Length >= 4)
                {
                    costIdCardNumLast = custIdCardNum.Substring(custIdCardNum.Length - 4, 4);
                }

                var custName = cust.Select(c => c.CUST_NAME).FirstOrDefault() ?? string.Empty;

                string subject;
                string content;
                if (command.CurrentCulture.IsThaiCulture())
                {
                    subject = love.Where(l => (!string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_NAME == "SUBJECT"))
                        .Select(l => l.LOV_VAL1).FirstOrDefault() ?? string.Empty;
                    content = love.Where(l => (!string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_NAME == "CONTENT"))
                        .Select(l => l.LOV_VAL1).FirstOrDefault() ?? string.Empty;
                }
                else
                {
                    subject = love.Where(l => l.LOV_NAME == "SUBJECT")
                        .Select(l => l.LOV_VAL2).FirstOrDefault() ?? string.Empty;
                    content = love.Where(l => l.LOV_NAME == "CONTENT")
                        .Select(l => l.LOV_VAL2).FirstOrDefault() ?? string.Empty;
                }

                var emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals("SUBMIT_SUMMARY")).FirstOrDefault();
                var mailContent = string.Format(content, custName, costIdCardNumLast, custIdCardNum);

                //string[] MailCC = null;

                if (command.EmailModel.MailTo != null)
                {

                    try
                    {
                        //var fromAddress = new MailAddress(emailData.SEND_FROM);
                        var fromAddress = new MailAddress(emailData.SEND_FROM);
                        var emailServerSplitValue = emailData.IP_MAIL_SERVER.Split('|');
                        var fromPassword = string.Empty;          //"V9!@M#V2zf@Q"
                        var host = string.Empty;                  //"10.252.160.41"
                        var port = string.Empty;                  //"25";
                        var domain = string.Empty;                //"corp.ais900dev.org"

                        var importance = "";
                        var body = mailContent;

                        //14.05.23 e-app first
                        if (emailServerSplitValue.Length > 0)
                        {
                            if (emailServerSplitValue.Length == 2)
                            {
                                host = emailServerSplitValue[0];
                                port = emailServerSplitValue[1];

                                var MailTo = command.EmailModel.MailTo;
                             
                                var smtp = new SmtpClient()
                                {
                                    Host = host,
                                    Port = Convert.ToInt32(port),
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = false,

                                };
                                var message = new MailMessage();
                                message.From = fromAddress;
                                if (!string.IsNullOrEmpty(command.EmailModel.FilePath))
                                {
                                    using (var impersonator = new Impersonator(command.ImpersonateUser, command.ImpersonateIP,
                                               command.ImpersonatePass, false))
                                    {
                                        message.Attachments.Add(new Attachment(command.EmailModel.FilePath));
                                    }
                                }

                                try
                                {
                                    _logger.Info("NotificationCommandHandler Attachments : " + command.EmailModel.FilePath2);

                                    if (!string.IsNullOrEmpty(command.EmailModel.FilePath2))
                                    {
                                        using (var impersonator = new Impersonator(command.ImpersonateUser, command.ImpersonateIP,
                                                command.ImpersonatePass, false))
                                        {
                                            message.Attachments.Add(new Attachment(command.EmailModel.FilePath2));
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());

                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");

                                    _logger.Info("NotificationCommandHandler 1," + ex.GetErrorMessage());
                                }
                                message.To.Add(MailTo);
                                message.IsBodyHtml = true;
                                message.Subject = string.Format("{0} {1}", subject, ""); //DateTime.Now.ToDateDisplayText());
                                message.Body = body;
                                message.Priority = GetMailPriority(importance);
                                

                                _logger.Info(
                                    string.Format(
                                        "NotificationCommandHandler ,RunningNo = {0}, email_from = {1},email_to = {2}",
                                        command.RunningNo, message.From, message.To.Count()));

                                smtp.Send(message);

                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, smtp.ObjectToString(), log, "Success", "", "");

                            }
                            else //14.05.23 e-app first
                            {
                                host = emailServerSplitValue[0];
                                port = emailServerSplitValue[1];
                                domain = emailServerSplitValue[2];
                                fromPassword = emailServerSplitValue[3];

                                var MailTo = command.EmailModel.MailTo;

                                var smtp = new SmtpClient()
                                {
                                    Host = host,
                                    Port = Convert.ToInt32(port),
                                    DeliveryMethod = SmtpDeliveryMethod.Network,
                                    UseDefaultCredentials = true,
                                    Credentials = new NetworkCredential(fromAddress.User, fromPassword, domain),
                                };
                                var message = new MailMessage();
                                message.From = fromAddress;
                                if (!string.IsNullOrEmpty(command.EmailModel.FilePath))
                                {
                                    using (var impersonator = new Impersonator(command.ImpersonateUser, command.ImpersonateIP,
                                               command.ImpersonatePass, false))
                                    {
                                        message.Attachments.Add(new Attachment(command.EmailModel.FilePath));
                                    }
                                }

                                try
                                {
                                    _logger.Info("NotificationCommandHandler Attachments : " + command.EmailModel.FilePath2);

                                    if (!string.IsNullOrEmpty(command.EmailModel.FilePath2))
                                    {
                                        using (var impersonator = new Impersonator(command.ImpersonateUser, command.ImpersonateIP,
                                                command.ImpersonatePass, false))
                                        {
                                            message.Attachments.Add(new Attachment(command.EmailModel.FilePath2));
                                        }

                                    }
                                }
                                catch (Exception ex)
                                {
                                    sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());

                                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");

                                    _logger.Info("NotificationCommandHandler 1," + ex.GetErrorMessage());
                                }
                                message.To.Add(MailTo);
                                message.IsBodyHtml = true;
                                message.Subject = string.Format("{0} {1}", subject, ""); //DateTime.Now.ToDateDisplayText());
                                message.Body = body;
                                message.Priority = GetMailPriority(importance);

                                _logger.Info(
                                    string.Format(
                                        "NotificationCommandHandler ,RunningNo = {0}, email_from = {1},email_to = {2}",
                                        command.RunningNo, message.From, message.To.Count()));

                                smtp.Send(message);
                                //14.05.23 e-app first
                                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, smtp.ObjectToString(), log, "Success", "", "");
                            }
                        }
                        else
                        {
                            throw new Exception("IP_MAIL_SERVER does not exist.");
                        }
                    }
                    catch (Exception ex)
                    {
                        sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");
                        _logger.Info("NotificationCommandHandler 2," + ex.GetErrorMessage());
                    }

                    //var mailLog = _emailLogService.Get(m => m.RUNNING_NO == command.RunningNo).FirstOrDefault();
                    //if (null == mailLog)
                    //{
                    //    _logger.Info("NotificationCommandHandler mailLog is null");

                    //    command.ReturnCode = -1;
                    //    command.ReturnDesc = (string.IsNullOrEmpty(sendMailLogMessage)
                    //        ? "Sendmail Log is null."
                    //        : sendMailLogMessage);
                    //}
                    //else
                    //{
                    //    mailLog.FILE_NAME = command.EmailModel.FilePath;
                    //    mailLog.PROCESS_NAME = "Sended";
                    //    mailLog.RETURN_CODE = "0";
                    //    mailLog.RETURN_DESC = sendMailLogMessage;

                    //    _emailLogService.Update(mailLog);
                    //    _uow.Persist();

                    //    command.ReturnCode = mailLog.RETURN_CODE.ToSafeDecimal();
                    //    command.ReturnDesc = mailLog.RETURN_DESC;
                    //}
                }
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Failed", ex.Message, "");
                _logger.Info("Error Occured When Handle NotificationCommand");
                _logger.Info("NotificationCommandHandler 3," + ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
            finally
            {

                try
                {
                    _logger.Info(
                        string.Format(
                            "NotificationCommandHandler finally,RunningNo = {0}", command.RunningNo));

                    var mailLog = _emailLogService.Get(m => m.RUNNING_NO == command.RunningNo).FirstOrDefault();
                    if (null == mailLog)
                    {
                        _logger.Info("NotificationCommandHandler mailLog is null");

                        command.ReturnCode = -1;
                        command.ReturnDesc = (string.IsNullOrEmpty(sendMailLogMessage)
                            ? "Sendmail Log is null."
                            : sendMailLogMessage);
                    }
                    else
                    {
                        mailLog.FILE_NAME = string.Format("Path 1 = {0}, Path 2 = {1}", command.EmailModel.FilePath, command.EmailModel.FilePath2);
                        mailLog.PROCESS_NAME = "Sended";
                        mailLog.RETURN_CODE = "0";
                        mailLog.RETURN_DESC = sendMailLogMessage;

                        _emailLogService.Update(mailLog);
                        _uow.Persist();

                        command.ReturnCode = mailLog.RETURN_CODE.ToSafeDecimal();
                        command.ReturnDesc = mailLog.RETURN_DESC;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info("NotificationCommandHandler 4," + ex.GetErrorMessage());
                }

            }
        }

        private MailPriority GetMailPriority(string importance)
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
    }

    public class Impersonator : IDisposable
    {
        private WindowsImpersonationContext impersonatedUser = null;
        private IntPtr userToken;

        public Impersonator(string username, string domainOrServerName, string password, bool useDomain = true)
        {
            userToken = new IntPtr(0);

            bool logonResult = false;

            if (useDomain)
            {
                logonResult = LogonUser(username, domainOrServerName, password, LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT, ref userToken);
            }
            else
            {
                logonResult = LogonUser(username, domainOrServerName, password, LOGON32_LOGON_NEW_CREDENTIALS, LOGON32_PROVIDER_DEFAULT, ref userToken);
            }

            if (!logonResult)
            {
                if (string.IsNullOrEmpty(domainOrServerName))
                {
                    throw new InvalidOperationException(string.Format("Failed to impersonate to {0}", username));
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Failed to impersonate to {0}@{1}", username, domainOrServerName));
                }
            }

            impersonatedUser = new WindowsIdentity(userToken).Impersonate();
        }

        #region IDisposable Members

        public void Dispose()
        {
            try
            {
                if (impersonatedUser != null)
                {
                    impersonatedUser.Undo();
                    CloseHandle(userToken);
                }
            }
            catch { }
        }

        #endregion IDisposable Members

        #region external dll functions

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        public const int LOGON32_LOGON_BATCH = 4;
        public const int LOGON32_LOGON_SERVICE = 5;
        public const int LOGON32_LOGON_UNLOCK = 7;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8;
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_PROVIDER_WINNT35 = 1;
        public const int LOGON32_PROVIDER_WINNT40 = 2;
        public const int LOGON32_PROVIDER_WINNT50 = 3;

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        public static extern bool LogonUser(String lpszUserName, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        #endregion external dll functions
    }

}
