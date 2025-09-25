using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class NotificationBatchCommandHandler : ICommandHandler<NotificationBatchCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_CFG_LOV> _lovService;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _emailProcService;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _emailLogService;
        private readonly IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> _coverageResult;


        public NotificationBatchCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IEntityRepository<FBB_CFG_LOV> lovService,
            IEntityRepository<FBB_EMAIL_PROCESSING> emailProcService,
            IEntityRepository<FBB_SENDMAIL_LOG> emailLogService,
            IEntityRepository<FBB_FBSS_COVERAGEAREA_RESULT> coverageResult)
        {
            _logger = logger;
            _uow = uow;
            _emailProcService = emailProcService;
            _emailLogService = emailLogService;
            _lovService = lovService;
            _coverageResult = coverageResult;
        }

        public void Handle(NotificationBatchCommand command)
        {
            try
            {
                var body = "";
                var MailTo = "";
                var MailCC = "";
                var subject = "";
                var sendMailLogMessage = "";
                string procname = "";
                string tableCovresult = "";

                if (command.result != "" || command.result != null)
                {
                    procname = "QUERY_BUILDING_VILLAGE";
                    var listCovresult = (from re in _coverageResult.Get()
                                         where re.CREATED_BY == "FBBCONFIG" && ((DateTime)re.CREATED_DATE).Day == DateTime.Today.Day
                                         && ((DateTime)re.CREATED_DATE).Month == DateTime.Today.Month && ((DateTime)re.CREATED_DATE).Year == DateTime.Today.Year
                                         select re).ToList();

                    #region draw table 
                    tableCovresult += "<table border='1' style='border:solid; width: 100%' >";
                    tableCovresult += "<tr>";
                    tableCovresult += "<td style='text-align:center;'>ADDRESS_TYPE</td>";
                    tableCovresult += "<td style='text-align:center;'>POSTALCODE</td>";
                    tableCovresult += "<td style='text-align:center;'>SUB_DISRTRICT_NAME</td>";
                    tableCovresult += "<td style='text-align:center;'>LANGUAGE</td>";
                    tableCovresult += "<td style='text-align:center;'>BUILDING_NAME</td>";
                    tableCovresult += "<td style='text-align:center;'>BUILDING_NO</td>";
                    tableCovresult += "<td style='text-align:center;'>PHONE_FLAG</td>";
                    tableCovresult += "<td style='text-align:center;'>FLOOR_NO</td>";
                    tableCovresult += "<td style='text-align:center;'>ADDRESS_NO</td>";
                    tableCovresult += "<td style='text-align:center;'>MOO</td>";
                    tableCovresult += "<td style='text-align:center;'>SOI</td>";
                    tableCovresult += "<td style='text-align:center;'>ROAD</td>";
                    tableCovresult += "<td style='text-align:center;'>LATITUDE</td>";
                    tableCovresult += "<td style='text-align:center;'>LONGITUDE</td>";
                    tableCovresult += "<td style='text-align:center;'>UNIT_NO</td>";
                    tableCovresult += "<td style='text-align:center;'>COVERAGE</td>";
                    tableCovresult += "<td style='text-align:center;'>ADDRESS_ID</td>";
                    tableCovresult += "<td style='text-align:center;'>ACCESS_MODE_LIST</td>";
                    tableCovresult += "<td style='text-align:center;'>PLANNING_SITE_LIST</td>";
                    tableCovresult += "<td style='text-align:center;'>IS_PARTNER</td>";
                    tableCovresult += "<td style='text-align:center;'>PARTNER_NAME</td>";
                    tableCovresult += "<td style='text-align:center;'>PREFIXNAME</td>";
                    tableCovresult += "<td style='text-align:center;'>FIRSTNAME</td>";
                    tableCovresult += "<td style='text-align:center;'>LASTNAME</td>";
                    tableCovresult += "<td style='text-align:center;'>CONTACTNUMBER</td>";
                    tableCovresult += "<td style='text-align:center;'>PRODUCTTYPE</td>";
                    tableCovresult += "<td style='text-align:center;'>ZIPCODE_ROWID</td>";
                    tableCovresult += "<td style='text-align:center;'>OWNER_PRODUCT</td>";
                    tableCovresult += "<td style='text-align:center;'>TRANSACTION_ID</td>";
                    tableCovresult += "</tr>";

                    if (listCovresult.Count == 0)
                    {
                        tableCovresult = "";
                    }
                    else
                    {
                        foreach (var item in listCovresult)
                        {
                            tableCovresult += "<tr>";
                            tableCovresult += "<td>" + item.ADDRRESS_TYPE + "</td>";
                            tableCovresult += "<td>" + item.POSTAL_CODE + "</td>";
                            tableCovresult += "<td>" + item.SUB_DISTRICT_NAME + "</td>";
                            tableCovresult += "<td>" + item.LANGUAGE + "</td>";
                            tableCovresult += "<td>" + item.BUILDING_NAME + "</td>";
                            tableCovresult += "<td>" + item.BUILDING_NO + "</td>";
                            tableCovresult += "<td>" + item.PHONE_FLAG + "</td>";
                            tableCovresult += "<td>" + item.FLOOR_NO + "</td>";
                            tableCovresult += "<td>" + item.ADDRESS_NO + "</td>";
                            tableCovresult += "<td>" + item.MOO + "</td>";
                            tableCovresult += "<td>" + item.SOI + "</td>";
                            tableCovresult += "<td>" + item.ROAD + "</td>";
                            tableCovresult += "<td>" + item.LATITUDE + "</td>";
                            tableCovresult += "<td>" + item.LONGITUDE + "</td>";
                            tableCovresult += "<td>" + item.UNIT_NO + "</td>";
                            tableCovresult += "<td>" + item.COVERAGE + "</td>";
                            tableCovresult += "<td>" + item.ADDRESS_ID + "</td>";
                            tableCovresult += "<td>" + item.ACCESS_MODE_LIST + "</td>";
                            tableCovresult += "<td>" + item.PLANNING_SITE_LIST + "</td>";
                            tableCovresult += "<td>" + item.IS_PARTNER + "</td>";
                            tableCovresult += "<td>" + item.PARTNER_NAME + "</td>";
                            tableCovresult += "<td>" + item.PREFIXNAME + "</td>";
                            tableCovresult += "<td>" + item.FIRSTNAME + "</td>";
                            tableCovresult += "<td>" + item.LASTNAME + "</td>";
                            tableCovresult += "<td>" + item.CONTACTNUMBER + "</td>";
                            tableCovresult += "<td>" + item.PRODUCTTYPE + "</td>";
                            tableCovresult += "<td>" + item.ZIPCODE_ROWID + "</td>";
                            tableCovresult += "<td>" + item.OWNER_PRODUCT + "</td>";
                            tableCovresult += "<td>" + item.TRANSACTION_ID + "</td>";
                            tableCovresult += "</tr>";

                        }
                        tableCovresult += "</table>";
                    }
                    #endregion
                }
                else
                {
                    procname = "LOAD_SUBCONTRACTOR_TIMESLOT";

                }

                /// get detail
                var emailData = _emailProcService.Get(e => e.PROCESS_NAME.Equals(procname)).FirstOrDefault();
                var emailServerSplitValue = emailData.IP_MAIL_SERVER.Split('|');
                MailTo = emailData.SEND_TO.ToSafeString();
                MailCC = emailData.SEND_CC.ToSafeString();

                string[] MailTo2 = MailTo.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                string[] MailCC2 = MailCC.Split(';').Where(s => !string.IsNullOrEmpty(s)).ToArray();

                var fromAddress = new MailAddress(emailData.SEND_FROM);
                var fromPassword = "V9!@M#V2zf@Q";
                var host = "10.252.160.41";
                var port = "25";
                var domain = "corp.ais900dev.org";

                if (emailServerSplitValue.Length > 0)
                {
                    host = emailServerSplitValue[0];
                    port = emailServerSplitValue[1];
                    domain = emailServerSplitValue[2];
                    fromPassword = emailServerSplitValue[3];
                }

                try
                {

                    /// get subject and body
                    var templov = _lovService.Get(l => l.LOV_NAME.Equals(procname) && l.DISPLAY_VAL.Equals("EMAIL_ALERT")).FirstOrDefault();
                    if (command.result != "" || command.result != null)
                    {
                        subject = string.Format(templov.LOV_VAL1, "", DateTime.Now.ToDateDisplayText());
                        body = string.Format(templov.LOV_VAL2, "", command.result, command.errormsg, command.numRecInsert, command.numRecUpdate, command.numRecDelete);

                        body += "<br/>" + tableCovresult;
                    }
                    else
                    {
                        subject = templov.LOV_VAL1;
                        body = templov.LOV_VAL2 + " " + command.Cause;
                    }


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

                    message.IsBodyHtml = true;
                    message.Subject = subject;
                    message.Body = body;
                    message.Priority = MailPriority.High;
                    smtp.Send(message);
                }
                catch (Exception ex)
                {
                    sendMailLogMessage = new string(ex.GetErrorMessage().Take(500).ToArray());
                    _logger.Info(ex.GetErrorMessage());
                }

                /// insert log
                var newMailLog = new FBB_SENDMAIL_LOG();
                if (sendMailLogMessage != "")
                {
                    newMailLog.PROCESS_NAME = "Sended";
                    newMailLog.CREATE_USER = "BATCH";
                    newMailLog.CREATE_DATE = DateTime.Now;
                    newMailLog.RETURN_CODE = "-1";
                    newMailLog.RETURN_DESC = sendMailLogMessage;
                }
                else
                {
                    newMailLog.PROCESS_NAME = "Sended";
                    newMailLog.CREATE_USER = "BATCH";
                    newMailLog.CREATE_DATE = DateTime.Now;
                    newMailLog.RETURN_CODE = "0";
                    newMailLog.RETURN_DESC = sendMailLogMessage;
                }

                _emailLogService.Create(newMailLog);
                _uow.Persist();

            }
            catch (Exception ex)
            {
                _logger.Info("Error Occured When Handle NotificationBatchCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
        }
    }
}
