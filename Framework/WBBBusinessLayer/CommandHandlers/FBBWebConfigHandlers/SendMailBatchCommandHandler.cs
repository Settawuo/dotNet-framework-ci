using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers.FBBWebConfigHandlers
{
    public class SendMailBatchCommandHandler : ICommandHandler<SendMailBatchNotificationCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public SendMailBatchCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_SENDMAIL_LOG> emailLogService,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _logger = logger;
            _uow = uow;
            _emailLogService = emailLogService;
            _emailProcService = emailProcService;
        }

        public async void Handle(SendMailBatchNotificationCommand command)
        {
            try
            {
                var body = "";
                var MailTo = "";
                var MailCC = "";
                var subject = "";
                var sendMailLogMessage = "";
                var MailBCC = "";

                var emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals(command.ProcessName)).FirstOrDefault();
                if (emailData != null)
                {
                    command.SendFrom = emailData.SEND_FROM.ToSafeString();
                    command.SendTo = emailData.SEND_TO.ToSafeString();
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

                }
                catch (Exception ex)
                {
                    sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());
                    _logger.Info(ex.GetErrorMessage());
                    command.ReturnMessage = sendMailLogMessage;

                }

                // insert log
                var newMailLog = new FBB_SENDMAIL_LOG();
                newMailLog.PROCESS_NAME = "Sended";
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
                _logger.Info("Error Occured When Handle SendMailNotificationCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
                command.ReturnMessage = string.Format("Error Occured When Handle {0}.", ex.GetErrorMessage());
            }
        }
    }


    public class SendMailBatchPatchDataCommandHandler : ICommandHandler<SendMailBatchPatchDataCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBBPAYG_PATCH_SN_SENDMAIL_LOG> _emailLogService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;
        private readonly IEntityRepository<object> _entityRepository;


        public SendMailBatchPatchDataCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBBPAYG_PATCH_SN_SENDMAIL_LOG> emailLogService,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService,
            IEntityRepository<object> entityRepository)
        {
            _logger = logger;
            _uow = uow;
            _emailLogService = emailLogService;
            _emailProcService = emailProcService;
            _entityRepository = entityRepository;
        }

        public void Handle(SendMailBatchPatchDataCommand command)
        {
            var body = "";
            var MailTo = "";
            var MailCC = "";
            var subject = "";
            var sendMailLogMessage = "";
            var MailBCC = "";

            try
            {
                // insert log
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Varchar2;
                ret_code.Size = 2000;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var p_EMAIL = new OracleParameter();
                p_EMAIL.ParameterName = "p_EMAIL";
                p_EMAIL.Size = 2000;
                p_EMAIL.OracleDbType = OracleDbType.Varchar2;
                p_EMAIL.Direction = ParameterDirection.Output;

                var p_FILENAME = new OracleParameter();
                p_FILENAME.ParameterName = "p_FILENAME";
                p_FILENAME.Size = 2000;
                p_FILENAME.OracleDbType = OracleDbType.Varchar2;
                p_FILENAME.Direction = ParameterDirection.Input;
                p_FILENAME.Value = command.FileName;

                var executeResult = _entityRepository.ExecuteStoredProcExecuteReader("WBB.PKG_FBB_PAYG_PATCH_SN.p_search_patch_sn_sendmail",
                      new object[]
                       {
                                   p_FILENAME,

								   //Return
								   p_EMAIL,
                                   ret_code,
                                   ret_msg
                       });

                command.SendTo = p_EMAIL.Value.ToString();


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

                var message = new MailMessage();
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

                    message.From = fromAddress;

                    for (int i = 0; i < MailTo2.Length; i++)
                        message.To.Add(MailTo2[i]);

                    if (MailCC2 != null && MailCC2.Length > 0)
                        for (int i = 0; i < MailCC2.Length; i++)
                            message.CC.Add(MailCC2[i]);

                    if (MailBCC2 != null && MailBCC2.Length > 0)
                        for (int i = 0; i < MailBCC2.Length; i++)
                            message.Bcc.Add(MailBCC2[i]);

                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.Body = body;
                    message.Priority = MailPriority.High;

                    //if (item.AttachFiles != null && item.AttachFiles.Length > 0)
                    //	foreach (var file in item.AttachFiles)
                    //		message.Attachments.Add(new Attachment(file));

                    //if (item.msAttachFiles != null)
                    //	foreach (var items in item.msAttachFiles)
                    //		message.Attachments.Add(new Attachment(items.file, items.fileName, MediaTypeNames.Application.Octet));

                    command.ReturnMessage = "Success.";
                    smtp.SendMailAsync(message);
                }
                catch (Exception ex)
                {
                    sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());
                    _logger.Info(ex.GetErrorMessage());
                    command.ReturnMessage = "Failed ";

                }

            }
            catch (Exception ex)
            {
                _logger.Info("Error Occured When Handle SendMailNotificationCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
                command.ReturnMessage = string.Format("Error Occured When Handle {0}.", ex.ToSafeString());
            }
            finally
            {
                if (command.SendTo.ToSafeString() != "")
                {
                    var newMailLoginsert = new FBBPAYG_PATCH_SN_SENDMAIL_LOG()
                    {
                        FILE_NAME = command.FileName.ToSafeString(),
                        MAIL_TO = command.SendTo.ToSafeString(),
                        MAIL_CONTENT = command.Body.ToSafeString(),
                        MAIL_STATUS = command.ReturnMessage.ToSafeString(),
                        ERROR_MESSAGE = sendMailLogMessage,
                        CREATE_BY = "PAYGPatch",
                        CREATE_DATE = DateTime.Now,
                        UPDATE_BY = "PAYGPatch",
                        UPDATE_DATE = DateTime.Now
                    };
                    _emailLogService.Create(newMailLoginsert);
                }
            }

            _uow.Persist();



        }
    }
}
