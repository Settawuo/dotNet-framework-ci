using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SendMailFTTHNotificationCommandHandler : ICommandHandler<SendMailFTTHNotificationCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _interLog;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;
        private string transactionID;

        public SendMailFTTHNotificationCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_SENDMAIL_LOG> emailLogService,
            IEntityRepository<FBB_INTERFACE_LOG> interLog,
        IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _logger = logger;
            _uow = uow;
            _emailLogService = emailLogService;
            _interLog = interLog;
            _emailProcService = emailProcService;
            transactionID = DateTime.Now.ToString("yyyyMMddHHmmss") + "FTTH_0001";
        }
        public async void Handle(SendMailFTTHNotificationCommand command)
        {

            try
            {
                var body = "";
                var MailTo = "";
                var MailCC = "";
                var subject = "";
                var sendMailLogMessage = "";
                var MailBCC = "";
                string DateNow = DateTime.Now.ToString("yyyyMMddHHmmss").ToString();
                interfacelog("Start", command);                       

                var emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals(command.ProcessName)).FirstOrDefault();
                if (emailData != null)
                {                    
                    command.SendFrom = emailData.SEND_FROM.ToSafeString();
                    //command.SendTo = emailData.SEND_TO.ToSafeString();
                    command.SendCC = emailData.SEND_CC.ToSafeString();
                    command.SendBCC = emailData.SEND_BCC.ToSafeString();

                    if (string.IsNullOrEmpty(command.IPMailServer))
                    {
                        var emailServerSplitValue = emailData.IP_MAIL_SERVER.Split('|');
                        if (emailServerSplitValue.Length > 0)
                        {
                            command.IPMailServer = emailServerSplitValue[0];
                            command.Port = emailServerSplitValue[1];
                            command.Domaim = emailServerSplitValue[2];
                            command.FromPassword = emailServerSplitValue[3];
                        }
                    }
                }

                MailTo = command.SendTo;
                MailCC = command.SendCC;
                MailBCC = command.SendBCC;

                string[] MailTo2 = MailTo.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                string[] MailCC2 = MailCC.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                string[] MailBCC2 = MailBCC.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                var fromAddress = new MailAddress(command.SendFrom);

                var fromPassword = command.FromPassword;
                var host = command.IPMailServer;
                var port = command.Port;
                var domain = command.Domaim;

                try
                {
                    subject = command.Subject.ToSafeString();
                    body = command.Body.ToSafeString();

                    var smtp = new SmtpClient
                    {
                        Host = host,
                        Port = Convert.ToInt32(port),
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(fromAddress.User, fromPassword, domain),
                    };

                    var message = new MailMessage();
                    message.From = fromAddress;

                    for (int i = 0; i < MailTo2.Length; i++)
                    {
                        message.To.Add(MailTo2[i]);
                    }

                    if (MailCC2 != null && MailCC2.Length > 0)
                    {
                        for (int i = 0; i < MailCC2.Length; i++)
                        {
                            message.CC.Add(MailCC2[i]);
                        }
                    }

                    if (MailBCC2 != null && MailBCC2.Length > 0)
                    {
                        for (int i = 0; i < MailBCC2.Length; i++)
                        {
                            message.Bcc.Add(MailBCC2[i]);
                        }
                    }

                    //#region tempBody
                    //StringBuilder tempBody = new StringBuilder();
                    //tempBody.Append("<p style='font-weight:bolder;'>SEND_MAIL_ONSERVICE_SPECIAL_FTTH</p>");
                    //tempBody.Append("<br/>");
                    //tempBody.Append("<span> SEND_MAIL_ONSERVICE_SPECIAL_FTTH:" + DateNow.ToSafeString());
                    //tempBody.Append("</span>");
                    //tempBody.Append("<br/>");
                    //tempBody.Append("<br/>");
                    //tempBody.Append("<br/>");
                    //tempBody.Append("<br/>");
                    //tempBody.Append("<span>Thanks.");
                    //tempBody.Append("</span>");
                    //#endregion
                    //body = tempBody.ToSafeString();
                    body = command.Body;

                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.Body = body;
                    message.Priority = MailPriority.High;

                    if (command.AttachFiles != null && command.AttachFiles.Length > 0)
                    {
                        foreach (var file in command.AttachFiles)
                        {

                            message.Attachments.Add(new Attachment(file));
                        }
                    }

                    if (command.msAttachFiles != null)
                    {

                        foreach (var item in command.msAttachFiles)
                        {
                            message.Attachments.Add(new Attachment(item.file, item.fileName, MediaTypeNames.Application.Octet));
                        }
                    }
                    command.ReturnMessage = "Success.";
                    await smtp.SendMailAsync(message);

                    smtp.Dispose();
                    message.Dispose();

                    //interfacelog("End", command);

                }
                catch (Exception ex)
                {
                    sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());
                    _logger.Info(ex.GetErrorMessage());
                    command.ReturnMessage = sendMailLogMessage;

                    interfacelog("End", command, sendMailLogMessage);
                }

                // insert log
                var newMailLog = new FBB_SENDMAIL_LOG();
                newMailLog.PROCESS_NAME = "SEND_MAIL_ONSERVICE_SPECIAL_FTTH";
                newMailLog.CREATE_USER = command.CreateUser;
                newMailLog.CREATE_DATE = DateTime.Now;

                if (sendMailLogMessage.Length > 0)
                {
                    newMailLog.RETURN_CODE = "-1";
                    newMailLog.RETURN_DESC = sendMailLogMessage;
                }
                else
                {
                    newMailLog.RETURN_CODE = "0";
                    newMailLog.RETURN_DESC = "Complete";
                }

                if (command.AttachFiles != null && command.AttachFiles.Length > 0)
                    newMailLog.FILE_NAME = string.Join("|", command.AttachFiles);

                _emailLogService.Create(newMailLog);
                _uow.Persist();

            }
            catch (Exception ex)
            {
                _logger.Info("Error Occured When Handle SendMailFTTHNotificationCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
                command.ReturnMessage = string.Format("Error Occured When Handle {0}.", ex.GetErrorMessage());
                interfacelog("End", command, ex.GetErrorMessage());
            }
        }
        private void interfacelog(string action, SendMailFTTHNotificationCommand command, string reason = "")
        {
            var dbIntfCmd = new InterfaceLogCommand();
            dbIntfCmd.IN_TRANSACTION_ID = transactionID;

            if (action == "Start")
            {
                dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Insert;
                dbIntfCmd.IN_TRANSACTION_ID = transactionID;
                dbIntfCmd.METHOD_NAME = "SendMailFTTHNotification Handle";
                dbIntfCmd.SERVICE_NAME = command.GetType().Name;
                dbIntfCmd.IN_ID_CARD_NO = null;
                dbIntfCmd.IN_XML_PARAM = command.DumpToXml();
                dbIntfCmd.OUT_RESULT = "";
                dbIntfCmd.INTERFACE_NODE = "SendMailFTTHNotification";
                dbIntfCmd.CREATED_BY = "SendMailFTTHNotification";

            }
            else
            {
                dbIntfCmd.ActionType = WBBContract.Commands.ActionType.Update;
                dbIntfCmd.REQUEST_STATUS = (command.ReturnMessage == "Success") ? "Success" : "Error";
                dbIntfCmd.OUT_RESULT = command.ReturnMessage;
                dbIntfCmd.OUT_ERROR_RESULT = reason.Length > 100 ? reason.Substring(0, 100) : reason;
                //dbIntfCmd.OUT_XML_PARAM = output;
                dbIntfCmd.UPDATED_BY = "SendMailFTTHNotification";
            }
            InterfaceLogHelper.Log(_interLog, dbIntfCmd);
        }
    }
}
