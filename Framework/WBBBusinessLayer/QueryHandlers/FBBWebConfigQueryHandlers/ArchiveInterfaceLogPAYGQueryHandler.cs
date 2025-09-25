using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;
namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class ArchiveInterfaceLogPAYGQueryHandler : IQueryHandler<ArchiveInterfaceLogQuery, List<ArchiveInterfaceLogSendMailDetailList>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ArchiveInterfaceLogSendMailDetailList> _objService;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public ArchiveInterfaceLogPAYGQueryHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<ArchiveInterfaceLogSendMailDetailList> objService,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _logger = logger;
            _uow = uow;
            _objService = objService;
            _intfLog = intfLog;
            _emailProcService = emailProcService;
        }

        public List<ArchiveInterfaceLogSendMailDetailList> Handle(ArchiveInterfaceLogQuery query)
        {
            List<ArchiveInterfaceLogSendMailDetailList> executeResult = new List<ArchiveInterfaceLogSendMailDetailList>();

            try
            {
                _logger.Info("ArchiveInterfaceLogPAYGQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                var t_log_cur = new OracleParameter();
                t_log_cur.ParameterName = "t_log_cur";
                t_log_cur.OracleDbType = OracleDbType.RefCursor;
                t_log_cur.Direction = ParameterDirection.Output;

                var p_get_log_cur = new OracleParameter();
                p_get_log_cur.ParameterName = "p_get_log_cur";
                p_get_log_cur.OracleDbType = OracleDbType.RefCursor;
                p_get_log_cur.Direction = ParameterDirection.Output;


                executeResult = _objService.ExecuteReadStoredProc("WBB.PKG_FBB_INTERFACE_LOG_ARCHIVE.P_ARCHIVE_LOG",
                    new
                    {
                        p_date = query.p_date,
                        p_type = query.p_type,
                        //result = result,                     
                        ret_code = ret_code,
                        ret_msg = ret_msg,
                        t_log_cur = t_log_cur,
                        p_get_log_cur = p_get_log_cur

                    }).ToList();

                //TODO: Send Mail
                try
                {
                    SendMail(executeResult, "");
                }
                catch (Exception ex)
                {
                    SendMail(executeResult, "Cannot send the e-mail. Error: " + ex.Message);
                    throw new Exception("Cannot send the e-mail. Error: " + ex.Message);
                }

                if (ret_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    _logger.Info("End WBB.PKG_FBB_INTERFACE_LOG_ARCHIVE.P_ARCHIVE_LOG output msg: " + ret_msg);
                    return executeResult;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service WBB.PKG_FBB_INTERFACE_LOG_ARCHIVE.P_ARCHIVE_LOG output msg: " + ret_msg);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service WBB.PKG_FBB_INTERFACE_LOG_ARCHIVE.P_ARCHIVE_LOG" + ex.Message);
                SendMail(executeResult, "Error call service WBB.PKG_FBB_INTERFACE_LOG_ARCHIVE.P_ARCHIVE_LOG" + ex.Message);
                return null;
            }

        }

        private void SendMail(List<ArchiveInterfaceLogSendMailDetailList> _sendMailDetail, string msgCatch)
        {
            try
            {
                //TODO: GET Subject and detail 
                string mailFrom = "";
                string mailTo = "";
                string mailCC = "";
                string mailBCC = "";
                string ipMailServer = "";

                string subject = "";
                string content = "";

                if (_sendMailDetail.Count > 0)
                {
                    var sendMailDetaillist = _sendMailDetail.Select(x => new ArchiveInterfaceLogSendMailDetailList()
                    {
                        ret_code = x.ret_code,
                        ret_msg = x.ret_msg.ToSafeString(),
                        p_subject = x.p_subject.ToSafeString(),
                        p_body_h = x.p_body_h.ToSafeString(),
                        p_body_result = x.p_body_result.ToSafeString(),
                        p_body_summary = x.p_body_summary.ToSafeString(),
                        p_body_signature = x.p_body_signature.ToSafeString()
                    }).FirstOrDefault();

                    subject = sendMailDetaillist.p_subject.ToSafeString();

                    StringBuilder tempBody = new StringBuilder();
                    tempBody.Append("<p style='font-weight:bolder;'>" + sendMailDetaillist.p_body_h + "</p>");
                    tempBody.Append("<br />");
                    tempBody.Append("<p style='text-indent: 2.5em;'>" + sendMailDetaillist.p_body_result + "</p>");
                    tempBody.Append("<p style='text-indent: 2.5em;'>" + sendMailDetaillist.p_body_summary + "</p>");
                    tempBody.Append("<br />");
                    tempBody.Append("<p style='font-weight:bolder;'>" + sendMailDetaillist.p_body_signature + "</p>");

                    content = tempBody.ToSafeString();
                }
                else
                {
                    subject = string.Format("RESULT INTERFACE LOG IS FAILED");
                    content = "Please verify this error : " + msgCatch.ToSafeString();
                }

                //TODO: GET TABLE FBB_EMAIL_PROCESSING => PROCESS_NAME ='SEND_EMAIL_INTERFACE_LOG'
                var emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals("SEND_EMAIL_INTERFACE_LOG")).FirstOrDefault();
                if (emailData != null)
                {
                    mailFrom = emailData.SEND_FROM.ToSafeString();
                    mailTo = emailData.SEND_TO.ToSafeString();
                    mailCC = emailData.SEND_CC.ToSafeString();
                    mailBCC = emailData.SEND_BCC.ToSafeString();
                    ipMailServer = emailData.IP_MAIL_SERVER.ToSafeString();
                }

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
                message.To.Add(mailTo);
                if (!string.IsNullOrEmpty(mailCC))
                    message.CC.Add(mailCC);
                if (!string.IsNullOrEmpty(mailBCC))
                    message.Bcc.Add(mailBCC);
                message.IsBodyHtml = true;
                message.Subject = subject;
                message.Body = mailContent;
                message.Priority = GetMailPriority(string.Empty);

                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
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
}
