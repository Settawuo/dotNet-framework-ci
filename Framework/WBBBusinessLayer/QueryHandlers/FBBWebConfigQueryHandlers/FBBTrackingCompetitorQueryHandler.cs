using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers
{
    public class FBBTrackingCompetitorQueryHandler : IQueryHandler<GetFBBTrackingCompetitorQuery, List<FBBTrackingCompetitorModel>>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<string> _objService;
        private readonly IAirNetEntityRepository<FBBTrackingCompetitorModel> _objAirService;
        //private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public FBBTrackingCompetitorQueryHandler(ILogger logger,
            IEntityRepository<string> objService,
            IAirNetEntityRepository<FBBTrackingCompetitorModel> objAirService)
        {
            _logger = logger;
            _objService = objService;
            _objAirService = objAirService;
        }

        public List<FBBTrackingCompetitorModel> Handle(GetFBBTrackingCompetitorQuery query)
        {
            List<FBBTrackingCompetitorModel> executeResult = new List<FBBTrackingCompetitorModel>();

            try
            {
                _logger.Info("FBBTrackingCompetitorQueryHandler Start");
                var ret_code = new OracleParameter();
                ret_code.ParameterName = "ret_code";
                ret_code.OracleDbType = OracleDbType.Decimal;
                ret_code.Direction = ParameterDirection.Output;

                var ret_msg = new OracleParameter();
                ret_msg.ParameterName = "ret_msg";
                ret_msg.OracleDbType = OracleDbType.Varchar2;
                ret_msg.Size = 2000;
                ret_msg.Direction = ParameterDirection.Output;

                executeResult = _objAirService.ExecuteReadStoredProc("AIR_ADMIN.PKG_FBBOR007.FBBOR007",
                    new
                    {
                        ret_code = ret_code,
                        ret_msg = ret_msg

                    }).ToList();


                //TODO: Send Mail
                //try
                //{
                //    SendMail(executeResult, "");
                //}
                //catch (Exception ex)
                //{
                //    SendMail(executeResult, "Cannot send the e-mail. Error: " + ex.Message);
                //    throw new Exception("Cannot send the e-mail. Error: " + ex.Message);
                //}

                if (ret_code.Value.ToSafeString() == "0") // return 0 pass value to screen 
                {
                    _logger.Info("End AIR_ADMIN.PKG_FBBOR007.FBBOR007 output msg: " + ret_msg);
                    return null;

                }
                else //return -1 error
                {
                    _logger.Info("Error return -1 call service AIR_ADMIN.PKG_FBBOR007.FBBOR007 output msg: " + ret_msg);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                _logger.Info("Error call service AIR_ADMIN.PKG_FBBOR007.FBBOR007" + ex.Message);
                //SendMail(executeResult, "Error call service AIR_ADMIN.PKG_FBBOR007.FBBOR007" + ex.Message);
                return null;
            }

        }

        //private void SendMail(List<FBBTrackingCompetitorSendMailDetailList> _sendMailDetail, string msgCatch)
        //{
        //    try
        //    {
        //        //TODO: GET Subject and detail 
        //        string mailFrom = "";
        //        string mailTo = "";
        //        string mailCC = "";
        //        string mailBCC = "";
        //        string ipMailServer = "";

        //        string subject = "";
        //        string content = "";

        //        if (_sendMailDetail.Count > 0)
        //        {
        //            var sendMailDetaillist = _sendMailDetail.Select(x => new FBBTrackingCompetitorSendMailDetailList()
        //            {
        //                ret_code = x.ret_code,
        //                ret_msg = x.ret_msg.ToSafeString(),
        //                p_subject = x.p_subject.ToSafeString(),
        //                p_body_h = x.p_body_h.ToSafeString(),
        //                p_body_result = x.p_body_result.ToSafeString(),
        //                p_body_summary = x.p_body_summary.ToSafeString(),
        //                p_body_signature = x.p_body_signature.ToSafeString()
        //            }).FirstOrDefault();

        //            subject = sendMailDetaillist.p_subject.ToSafeString();

        //            StringBuilder tempBody = new StringBuilder();
        //            tempBody.Append("<p style='font-weight:bolder;'>" + sendMailDetaillist.p_body_h + "</p>");
        //            tempBody.Append("<br />");
        //            tempBody.Append("<p style='text-indent: 2.5em;'>" + sendMailDetaillist.p_body_result + "</p>");
        //            tempBody.Append("<p style='text-indent: 2.5em;'>" + sendMailDetaillist.p_body_summary + "</p>");
        //            tempBody.Append("<br />");
        //            tempBody.Append("<p style='font-weight:bolder;'>" + sendMailDetaillist.p_body_signature + "</p>");

        //            content = tempBody.ToSafeString();
        //        }
        //        else
        //        {
        //            subject = string.Format("RESULT INTERFACE LOG IS FAILED");
        //            content = "Please verify this error : " + msgCatch.ToSafeString();
        //        }

        //        //TODO: GET TABLE FBB_EMAIL_PROCESSING => PROCESS_NAME ='SEND_EMAIL_DATAWAREHOUSE'
        //        var emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals("SEND_EMAIL_DATAWAREHOUSE")).FirstOrDefault();
        //        if (emailData != null)
        //        {
        //            mailFrom = emailData.SEND_FROM.ToSafeString();
        //            mailTo = emailData.SEND_TO.ToSafeString();
        //            mailCC = emailData.SEND_CC.ToSafeString();
        //            mailBCC = emailData.SEND_BCC.ToSafeString();
        //            ipMailServer = emailData.IP_MAIL_SERVER.ToSafeString();
        //        }

        //        //TODO: Send Mail
        //        var mailContent = content;
        //        var fromAddress = new MailAddress(mailFrom);
        //        var emailServerSplitValue = !string.IsNullOrEmpty(ipMailServer) ? ipMailServer.Split('|') : null;

        //        var fromPassword = "V9!@M#V2zf@Q";
        //        var host = "10.252.160.41";
        //        var port = "25";
        //        var domain = "corp.ais900dev.org";

        //        if (emailServerSplitValue != null && emailServerSplitValue.Length > 0)
        //        {
        //            host = emailServerSplitValue[0];
        //            port = emailServerSplitValue[1];
        //            domain = emailServerSplitValue[2];
        //            fromPassword = emailServerSplitValue[3];
        //        }

        //        var smtp = new SmtpClient
        //        {
        //            Host = host,
        //            Port = Convert.ToInt32(port),
        //            DeliveryMethod = SmtpDeliveryMethod.Network,
        //            UseDefaultCredentials = true,
        //            Credentials = new NetworkCredential(fromAddress.User, fromPassword, domain),
        //        };

        //        var message = new MailMessage { From = fromAddress };
        //        message.To.Add(mailTo);
        //        if (!string.IsNullOrEmpty(mailCC))
        //            message.CC.Add(mailCC);
        //        if (!string.IsNullOrEmpty(mailBCC))
        //            message.Bcc.Add(mailBCC);
        //        message.IsBodyHtml = true;
        //        message.Subject = subject;
        //        message.Body = mailContent;
        //        message.Priority = GetMailPriority(string.Empty);

        //        smtp.Send(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        //private MailPriority GetMailPriority(string importance)
        //{
        //    var priority = new MailPriority();

        //    if (importance == "Low")
        //    {
        //        priority = MailPriority.Low;
        //    }
        //    else if (importance == "Normal")
        //    {
        //        priority = MailPriority.Normal;
        //    }
        //    else if (importance == "High")
        //    {
        //        priority = MailPriority.High;
        //    }

        //    return priority;
        //}

    }
}
