using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WBBContract;
using WBBContract.Commands;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class ReportProblemCommandHandler : ICommandHandler<ReportProblemCommand>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<ReportProblemCommand> _reportProblempository;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;

        public ReportProblemCommandHandler(IEntityRepository<ReportProblemCommand> reportProblempository
                                            , ILogger logger
                                            , IEntityRepository<FBB_CFG_LOV> lovService
                                            , IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService)
        {
            _reportProblempository = reportProblempository;
            _logger = logger;
            _lovService = lovService;
            _emailProcService = emailProcService;
        }

        public void Handle(ReportProblemCommand command)
        {

            try
            {
                object[] paramOut;

                var retCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };

                var retReportProblemId = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                #region insert logging report problem

                _logger.Info("START_PROC_LOGGING_REPORT_PROBLEM");

                _reportProblempository.ExecuteStoredProc("WBB.PKG_FBB_REPORT_PROBLEM.PROC_LOGGING_REPORT_PROBLEM",
                    out paramOut,
                    new
                    {
                        p_in_REPORT_PROBLEM_ID = command.ReportProblemId,
                        p_in_CUST_INTERNET_NUM = command.CustInternetNum,
                        p_in_CUST_ID_CARD_TYPE = command.CustIdCardType,
                        p_in_CUST_ID_CARD_NUM = command.CustIdCardNum,
                        p_in_PROBLEM_TYPE = command.ProblemType,
                        p_in_PROBLEM_TYPE_DES = command.ProblemTypeDes,
                        p_in_PROBLEM_DETAILS = command.ProblemDetails,
                        p_in_CONTACT_INFO = command.ContactInfo,
                        p_in_CONTACT_EMAIL = command.ContactEmail,
                        p_in_CONTACT_NUM = command.ContactNumber,
                        p_in_RETURN_CODE = command.ReturnCode,
                        p_in_RETURN_DESC = command.ReturnDesc,
                        //// return 
                        p_return_code = retCode,
                        p_return_message = retMessage,
                        p_return_report_problem_id = retReportProblemId
                    });

                command.ReturnCode = retCode.Value != null ? retCode.Value.ToSafeString() : "-1";
                command.ReturnDesc = retMessage.Value != null ? retMessage.Value.ToSafeString() : "Error";
                command.ReportProblemId = retReportProblemId.Value != null
                    ? Convert.ToDecimal(retReportProblemId.Value.ToSafeString())
                    : 0;

                _logger.Info("END_PROC_LOGGING_REPORT_PROBLEM");
                _logger.Info("END_PROC_LOGGING_REPORT_PROBLEM_ReturnCode:"+command.ReturnCode.ToSafeString());

                #endregion

                if (command.ReturnCode == "-1")
                {
                    return;
                }
                command.ReturnDesc = null;
                #region send mail

                try
                {
                    var lov = _lovService.Get(l => l.LOV_TYPE.Equals("EMAIL_REPORT_PROBLEM"));

                    var subject = lov.Where(l => (!string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_NAME == "SUBJECT"))
                        .Select(l => l.LOV_VAL1).FirstOrDefault() ?? string.Empty;
                    var content = lov.Where(l => (!string.IsNullOrEmpty(l.LOV_NAME) && l.LOV_NAME == "CONTENT"))
                        .Select(l => l.LOV_VAL1).FirstOrDefault() ?? string.Empty;

                    var emailData =
                        _emailProcService.Get(e => e.PROCESS_NAME == "FBB_REPORT_PROBLEM").FirstOrDefault() ??
                        new FBB_EMAIL_PROCESSING();
                    var mailContent = string.Format(content, command.ProblemTypeDes, command.ContactInfo,
                        command.ContactEmail, command.ContactNumber, command.CustInternetNum, command.ProblemDetails);

                    // edit sent from email customer 
                    var fromAddress = new MailAddress(emailData.SEND_FROM.ToUpper() == "CUSTOMER" ? command.ContactEmail.Trim() : emailData.SEND_FROM.Trim());

                    var emailServerSplitValue = emailData.IP_MAIL_SERVER != null
                        ? emailData.IP_MAIL_SERVER.Split('|')
                        : null;
                    var fromPass = "V9!@M#V2zf@Q";// Fixed Code scan : var fromPassword = "V9!@M#V2zf@Q";
                    var host = "10.252.160.41";
                    var port = "25";
                    var domain = "corp.ais900dev.org";

                    var importance = "";

                    if (emailServerSplitValue != null && emailServerSplitValue.Length > 0)
                    {
                        host = emailServerSplitValue[0].Trim();
                        port = emailServerSplitValue[1].Trim();
                        domain = emailServerSplitValue[2].Trim();
                        fromPass = emailServerSplitValue[3].Trim();
                    }

                    var smtp = new SmtpClient
                    {
                        Host = host,
                        Port = Convert.ToInt32(port),
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = true,
                        Credentials = new NetworkCredential(fromAddress.User.Trim(), fromPass, domain),
                    };

                    var message = new MailMessage { From = fromAddress };

                    message.To.Add(emailData.SEND_TO.Trim());
                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.Body = mailContent;
                    message.Priority = GetMailPriority(importance);

                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    command.ReturnCode = "-1";
                    command.ReturnDesc = new string(ex.GetErrorMessage().Take(500).ToArray());
                    _logger.Info(ex.GetErrorMessage());
                }

                #endregion


                #region update logging report problem

                _logger.Info("START_PROC_LOGGING_REPORT_PROBLEM");

                _reportProblempository.ExecuteStoredProc("WBB.PKG_FBB_REPORT_PROBLEM.PROC_LOGGING_REPORT_PROBLEM",
                    out paramOut,
                    new
                    {
                        p_in_REPORT_PROBLEM_ID = command.ReportProblemId,
                        p_in_CUST_INTERNET_NUM = command.CustInternetNum,
                        p_in_CUST_ID_CARD_TYPE = command.CustIdCardType,
                        p_in_CUST_ID_CARD_NUM = command.CustIdCardNum,
                        p_in_PROBLEM_TYPE = command.ProblemType,
                        p_in_PROBLEM_TYPE_DES = command.ProblemTypeDes,
                        p_in_PROBLEM_DETAILS = command.ProblemDetails,
                        p_in_CONTACT_INFO = command.ContactInfo,
                        p_in_CONTACT_EMAIL = command.ContactEmail,
                        p_in_CONTACT_NUM = command.ContactNumber,
                        p_in_RETURN_CODE = command.ReturnCode,
                        p_in_RETURN_DESC = command.ReturnDesc,
                        //// return 
                        p_return_code = retCode,
                        p_return_message = retMessage,
                        p_return_report_problem_id = retReportProblemId
                    });

                if (command.ReturnCode != "0" || (retCode.Value != null && retCode.Value.ToSafeString() != "0"))
                {
                    command.ReturnCode = retCode.Value != null ? retCode.Value.ToSafeString() : "-1";
                    command.ReturnDesc = retMessage.Value != null ? retMessage.Value.ToSafeString() : "Error";
                    command.ReportProblemId = retReportProblemId.Value != null
                        ? Convert.ToDecimal(retReportProblemId.Value.ToSafeString())
                        : 0;
                }

                _logger.Info("END_PROC_LOGGING_REPORT_PROBLEM");

                #endregion

            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                command.ReturnCode = "-1";
                command.ReturnDesc = "Error call Report Problem Command handles : " + ex.GetErrorMessage();
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
