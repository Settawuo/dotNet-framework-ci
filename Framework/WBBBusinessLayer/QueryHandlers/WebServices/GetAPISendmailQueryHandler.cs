using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class GetAPISendmailQueryHandler : IQueryHandler<GetAPISendmailQuery, GetAPISendmailModel>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<object> _objService;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService;

        public GetAPISendmailQueryHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<object> objService,
            IEntityRepository<FBB_SENDMAIL_LOG> emailLogService)
        {
            _logger = logger;
            _uow = uow;
            _intfLog = intfLog;
            _objService = objService;
            _emailLogService = emailLogService;
        }

        public GetAPISendmailModel Handle(GetAPISendmailQuery query)
        {
            //R22.07 APISendmail
            InterfaceLogCommand log = null;
            GetAPISendmailModel executeResults = new GetAPISendmailModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.TransactionID, "APISendmail", "GetAPISendmailQueryHandler", "", "FBB", "WEB");

                var FIRSTNAME = new OracleParameter();
                FIRSTNAME.ParameterName = "FIRSTNAME";
                FIRSTNAME.OracleDbType = OracleDbType.Varchar2;
                FIRSTNAME.Direction = ParameterDirection.Input;
                FIRSTNAME.Value = query.firstname.ToSafeString();

                var LASTNAME = new OracleParameter();
                LASTNAME.ParameterName = "LASTNAME";
                LASTNAME.OracleDbType = OracleDbType.Varchar2;
                LASTNAME.Direction = ParameterDirection.Input;
                LASTNAME.Value = query.lastname.ToSafeString();

                var TELEPHONE = new OracleParameter();
                TELEPHONE.ParameterName = "TELEPHONE";
                TELEPHONE.OracleDbType = OracleDbType.Varchar2;
                TELEPHONE.Direction = ParameterDirection.Input;
                TELEPHONE.Value = query.telephone.ToSafeString();

                var EMAIL = new OracleParameter();
                EMAIL.ParameterName = "EMAIL";
                EMAIL.OracleDbType = OracleDbType.Varchar2;
                EMAIL.Direction = ParameterDirection.Input;
                EMAIL.Value = query.email.ToSafeString();

                var CORPORATE_NAME = new OracleParameter();
                CORPORATE_NAME.ParameterName = "CORPORATE_NAME";
                CORPORATE_NAME.OracleDbType = OracleDbType.Varchar2;
                CORPORATE_NAME.Direction = ParameterDirection.Input;
                CORPORATE_NAME.Value = query.corporate_name.ToSafeString();

                var MESSAGE = new OracleParameter();
                MESSAGE.ParameterName = "MESSAGE";
                MESSAGE.OracleDbType = OracleDbType.Varchar2;
                MESSAGE.Direction = ParameterDirection.Input;
                MESSAGE.Value = query.message.ToSafeString();

                var RETURN_CODE = new OracleParameter();
                RETURN_CODE.ParameterName = "RETURN_CODE";
                RETURN_CODE.OracleDbType = OracleDbType.Decimal;
                RETURN_CODE.Direction = ParameterDirection.Output;

                var RETURN_MESSAGE = new OracleParameter();
                RETURN_MESSAGE.ParameterName = "RETURN_MESSAGE";
                RETURN_MESSAGE.OracleDbType = OracleDbType.Varchar2;
                RETURN_MESSAGE.Size = 2000;
                RETURN_MESSAGE.Direction = ParameterDirection.Output;

                var SEND_EMAIL = new OracleParameter();
                SEND_EMAIL.ParameterName = "SEND_EMAIL";
                SEND_EMAIL.OracleDbType = OracleDbType.RefCursor;
                SEND_EMAIL.Direction = ParameterDirection.Output;

                var result = _objService.ExecuteStoredProcMultipleCursor("WBB.PKG_FBB_MICROSITE.PROC_SEND_EMAIL",
                    new object[]
                    {
                         FIRSTNAME,
                         LASTNAME,
                         TELEPHONE,
                         EMAIL,
                         CORPORATE_NAME,
                         MESSAGE,

                         //return code
                         RETURN_CODE,
                         RETURN_MESSAGE,
                         SEND_EMAIL
                    });
                if (result != null)
                {
                    executeResults.RESULT_CODE = result[0] != null ? result[0].ToSafeString() : "-1";
                    executeResults.RETURN_MESSAGE = result[1] != null ? result[1].ToSafeString() : "Error";

                    if (executeResults.RESULT_CODE != "-1")
                    {
                        DataTable data1 = (DataTable)result[2];
                        executeResults.SEND_EMAIL = data1.DataTableToList<APISendmail>();

                        string messageSendMail = SendMail(_uow, _emailLogService, executeResults.SEND_EMAIL.FirstOrDefault());
                        if (messageSendMail == "")
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Success", "", "");
                        else
                            InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", messageSendMail, "");
                    }
                    else
                    {
                        InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", executeResults.RETURN_MESSAGE, "");
                    }
                }
                else
                {
                    InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, executeResults, log, "Failed", "Error", "");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("GetAPISendmailQueryHandler : Error.");
                _logger.Info(ex.Message);
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, "", log, "Error", ex.StackTrace, "");
                throw;
            }
            return executeResults;
        }

        private static string SendMail(IWBBUnitOfWork _uow, IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService, APISendmail dataAPISendmail)
        {
            string messageSendMail = "";

            try
            {
                string subject = dataAPISendmail.SUBJECT_NAME;
                string content = SetContentSendmail(dataAPISendmail);
                string mailFrom = dataAPISendmail.SEND_FROM;
                string mailTo = dataAPISendmail.SEND_TO;
                string mailCc = dataAPISendmail.SEND_CC;
                string ipMailServer = dataAPISendmail.IP_MAIL_SERVER;

                var mailContent = content;
                var fromAddress = new MailAddress(mailFrom);
                var emailServerSplitValue = !string.IsNullOrEmpty(ipMailServer) ? ipMailServer.Split('|') : null;

                var host = "10.252.160.41";
                var port = "25";
                var domain = "corp.ais900dev.org";
                var fromPass = "V9!@M#V2zf@Q";// Fixed Code scan : var fromPassword = "V9!@M#V2zf@Q";

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
                if (!string.IsNullOrEmpty(mailCc))
                    message.CC.Add(mailCc);
                message.IsBodyHtml = true;
                message.Subject = subject;
                message.Body = mailContent;
                message.Priority = GetMailPriority(string.Empty);

                smtp.Send(message);
            }
            catch (Exception ex)
            {
                messageSendMail = ex.Message;
                throw ex;
            }
            finally
            {
                //insert sendmail log
                var newMailLog = new FBB_SENDMAIL_LOG();
                newMailLog.PROCESS_NAME = "APISendmail";
                newMailLog.CREATE_USER = "WBB";
                newMailLog.CREATE_DATE = DateTime.Now;

                if (messageSendMail != "")
                {
                    newMailLog.RETURN_CODE = "-1";
                    newMailLog.RETURN_DESC = messageSendMail;
                }
                else
                {
                    newMailLog.RETURN_CODE = "0";
                    newMailLog.RETURN_DESC = "Complete";
                }

                _emailLogService.Create(newMailLog);
                _uow.Persist();
            }

            return messageSendMail;
        }

        private static MailPriority GetMailPriority(string importance)
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

        private static string SetContentSendmail(APISendmail dataAPISendmail)
        {
            var contentNotofication = "";
            contentNotofication += dataAPISendmail.DETAIL_1.ToSafeString();
            contentNotofication += dataAPISendmail.DETAIL_2.ToSafeString();
            contentNotofication += dataAPISendmail.DETAIL_3.ToSafeString();
            contentNotofication += dataAPISendmail.DETAIL_4.ToSafeString();
            contentNotofication += dataAPISendmail.DETAIL_5.ToSafeString();
            contentNotofication += dataAPISendmail.CLOSE_EMAIL.ToSafeString();

            return contentNotofication;
        }
    }
}
