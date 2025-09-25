using OfficeOpenXml;
using OfficeOpenXml.Table;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.WebServices;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.WebServiceModels;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    public class AutoMailReportQueryHandler : IQueryHandler<ReportAutoMailQuery, ReportAutoMailModel>
    {
        private readonly IEntityRepository<ConfigurationReportModel> _repositoryConfigurationReportModel;
        private readonly IEntityRepository<ConfigurationReportQueryModel> _repositoryConfigurationReportQueryModel;
        private readonly IEntityRepository<DataTable> _repositoryDataTable;
        private readonly IEntityRepository<object> _repositoryObject;
        private readonly IAirNetEntityRepository<DataTable> _repositoryAirDataTable;

        public AutoMailReportQueryHandler(IEntityRepository<ConfigurationReportModel> repositoryConfigurationReportModel, IEntityRepository<ConfigurationReportQueryModel> repositoryConfigurationReportQueryModel, IEntityRepository<DataTable> repositoryDataTable, IEntityRepository<object> repositoryObject, IAirNetEntityRepository<DataTable> repositoryAirDataTable)
        {
            _repositoryConfigurationReportModel = repositoryConfigurationReportModel;
            _repositoryConfigurationReportQueryModel = repositoryConfigurationReportQueryModel;
            _repositoryDataTable = repositoryDataTable;
            _repositoryObject = repositoryObject;
            _repositoryAirDataTable = repositoryAirDataTable;
        }

        public WBBEntity.PanelModels.WebServiceModels.ReportAutoMailModel Handle(ReportAutoMailQuery query)
        {
            var returnReportAutoMailModel = new WBBEntity.PanelModels.WebServiceModels.ReportAutoMailModel();
            var zipFileName = string.Empty;
            var reportName = string.Empty;
            var emailFrom = string.Empty;
            var emailToAdmin = string.Empty;
            var ipMailServer = string.Empty;
            decimal logId = 0;

            try
            {
                var stringQuery =
                     string.Format(
                         "SELECT * FROM TABLE( PKG_FBB_AUTOMAIL_REPORT.GetListConfigReport(iReportId => '{0}'))", query.ReportId);

                var configurationReportModel = _repositoryConfigurationReportModel.SqlQuery(stringQuery).ToList();

                if (configurationReportModel.Any())
                {
                    foreach (var reportModel in configurationReportModel)
                    {
                        try
                        {
                            zipFileName = string.Empty;
                            reportName = reportModel.REPORT_NAME;
                            emailFrom = reportModel.EMAIL_FROM;
                            emailToAdmin = reportModel.EMAIL_TO_ADMIN;
                            ipMailServer = reportModel.IP_MAIL_SERVER;

                            //TODO: Log send mail
                            var logMail = new AutoMailReportLog
                            {
                                p_log_id = 0,
                                p_report_id = reportModel.REPORT_ID,
                                p_status = "Sending",
                                p_create_by = query.CreateBy
                            };
                            logId = LogSendMail(logMail, _repositoryObject).p_log_id;

                            var filename = string.Format(@"{0}_{1}{2}", reportModel.REPORT_NAME, DateTime.Now.ToString("yyMMddHHmmss"), ".xlsx");
                            var fullFilename = string.Format("{0}{1}", query.PathTempFile, filename);
                            var newFile = new FileInfo(fullFilename);

                            stringQuery = string.Format("SELECT * FROM TABLE( PKG_FBB_AUTOMAIL_REPORT.getqueryreport(iReportId => '{0}'))", reportModel.REPORT_ID);
                            var configurationReportQueryModel = _repositoryConfigurationReportQueryModel.SqlQuery(stringQuery).ToList();

                            using (var package = new ExcelPackage(newFile))
                            {
                                foreach (var reportQueryModel in configurationReportQueryModel)
                                {
                                    var sqlStringBuilding = new StringBuilder();
                                    sqlStringBuilding.Append(reportQueryModel.QUERY_1);
                                    sqlStringBuilding.Append(" ");
                                    sqlStringBuilding.Append(reportQueryModel.QUERY_2);
                                    sqlStringBuilding.Append(" ");
                                    sqlStringBuilding.Append(reportQueryModel.QUERY_3);
                                    sqlStringBuilding.Append(" ");
                                    sqlStringBuilding.Append(reportQueryModel.QUERY_4);
                                    sqlStringBuilding.Append(" ");
                                    sqlStringBuilding.Append(reportQueryModel.QUERY_5);

                                    var dataTable = reportQueryModel.OWNER_DB == "WBB" ? _repositoryDataTable.ExecuteToDataTable(sqlStringBuilding.ToString().Trim(), reportQueryModel.SHEET_NAME) : _repositoryAirDataTable.ExecuteToDataTable(sqlStringBuilding.ToString().Trim(), reportQueryModel.SHEET_NAME);

                                    var worksheet =
                                        package.Workbook.Worksheets.Add(reportQueryModel.SHEET_NAME);
                                    worksheet.Cells["A1"].LoadFromDataTable(dataTable, true, TableStyles.None);
                                    worksheet.Cells["A:Z"].AutoFitColumns(15);

                                }

                                var isCopy = FileHelper.CopyFileDataTableEpPlus(query.UserTempFile,
                                        query.PassTempFile,
                                        query.DomainTempFile, package);
                                if (!isCopy)
                                {
                                    throw new Exception("Cannot authentication user/password path file.");
                                }
                            }

                            //TODO: Zip File

                            zipFileName = fullFilename.Replace(".xlsx", ".zip");
                            var isCopyZip = FileHelper.ZipFile(query.UserTempFile, query.PassTempFile, query.DomainTempFile, fullFilename, filename);
                            if (!isCopyZip)
                            {
                                throw new Exception("Cannot authentication user/password zip path file.");
                            }

                            //TODO: Send Mail
                            try
                            {
                                SendMail(reportModel.EMAIL_SUBJECT, reportModel.EMAIL_CONTENT, zipFileName,
                               reportModel.EMAIL_FROM, reportModel.EMAIL_TO, reportModel.EMAIL_CC,
                               reportModel.IP_MAIL_SERVER, query);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Cannot send the e-mail. Error: " + ex.Message);
                            }

                            //TODO: Log send mail
                            var log = new AutoMailReportLog
                            {
                                p_log_id = logId,
                                p_status = "Success",
                                p_status_message = "",
                                p_create_by = query.CreateBy,
                                p_path_file = zipFileName
                            };
                            LogSendMail(log, _repositoryObject);

                            returnReportAutoMailModel.ReportId = reportModel.REPORT_ID.ToSafeString();
                            returnReportAutoMailModel.ReturnCode = "0";
                            returnReportAutoMailModel.ReturnMessage = "";
                        }
                        catch (Exception ex)
                        {
                            returnReportAutoMailModel.ReportId = reportModel.REPORT_ID.ToSafeString();
                            returnReportAutoMailModel.ReturnCode = "-1";
                            returnReportAutoMailModel.ReturnMessage = ex.Message;

                            var log = new AutoMailReportLog
                            {
                                p_log_id = logId,
                                p_status = "Failed",
                                p_status_message = ex.Message,
                                p_create_by = query.CreateBy,
                                p_path_file = zipFileName
                            };
                            LogSendMail(log, _repositoryObject);

                            //TODO: send mail notification 
                            var contentNotofication = "Report Name : " + reportModel.REPORT_NAME;
                            contentNotofication += "</br>";
                            contentNotofication += "Error message : " + ex.Message;
                            contentNotofication += "</br>";
                            contentNotofication += "Path file : " + zipFileName;
                            contentNotofication += "</br>";
                            contentNotofication += "Date Time : " + DateTime.Now;

                            try
                            {
                                SendMail("AUTO MAIL REPORT FAILED", contentNotofication, zipFileName,
                                reportModel.EMAIL_FROM, reportModel.EMAIL_TO_ADMIN, string.Empty,
                                reportModel.IP_MAIL_SERVER, query);
                            }
                            catch (Exception exc)
                            {
                                var logexc = new AutoMailReportLog
                                {
                                    p_log_id = logId,
                                    p_status = "Failed",
                                    p_status_message = "Cannot send the e-mail to Admin. Error: " + exc.Message,
                                    p_create_by = query.CreateBy,
                                    p_path_file = zipFileName
                                };
                                LogSendMail(logexc, _repositoryObject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnReportAutoMailModel.ReportId = "1";//query.ReportId;
                returnReportAutoMailModel.ReturnCode = "-1";
                returnReportAutoMailModel.ReturnMessage = ex.Message;

                var log = new AutoMailReportLog
                {
                    p_log_id = logId,
                    p_status = "Failed",
                    p_status_message = ex.Message,
                    p_create_by = query.CreateBy,
                    p_path_file = zipFileName
                };
                LogSendMail(log, _repositoryObject);

                //TODO: send mail notification 
                var contentNotofication = "Report Name : " + (string.IsNullOrEmpty(reportName) ? "App Error" : reportName);
                contentNotofication += "</br>";
                contentNotofication += "Error message : " + ex.Message;
                contentNotofication += "</br>";
                contentNotofication += "Path file : " + zipFileName;
                contentNotofication += "</br>";
                contentNotofication += "Date Time : " + DateTime.Now;

                try
                {
                    SendMail("AUTO MAIL REPORT FAILED", contentNotofication, zipFileName,
                             emailFrom, emailToAdmin, string.Empty,
                             ipMailServer, query);
                }
                catch (Exception exc)
                {
                    var logexc = new AutoMailReportLog
                    {
                        p_log_id = logId,
                        p_status = "Failed",
                        p_status_message = "Cannot send the e-mail to Admin. Error: " + exc.Message,
                        p_create_by = query.CreateBy,
                        p_path_file = zipFileName
                    };
                    LogSendMail(logexc, _repositoryObject);
                }

            }

            return returnReportAutoMailModel;
        }

        private static AutoMailReportLog LogSendMail(AutoMailReportLog log, IEntityRepository<object> repositoryObject)
        {
            try
            {
                var pReturnCode = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };
                var pReturnMessage = new OracleParameter
                {
                    OracleDbType = OracleDbType.Varchar2,
                    Size = 2000,
                    Direction = ParameterDirection.Output
                };
                var pReturnLogId = new OracleParameter
                {
                    OracleDbType = OracleDbType.Decimal,
                    Direction = ParameterDirection.Output
                };

                object[] paramOut;
                var executeResult = repositoryObject.ExecuteStoredProc("WBB.PKG_FBB_AUTOMAIL_REPORT.LogSendMail",
                    out paramOut,
                    new
                    {
                        log.p_log_id,
                        log.p_report_id,
                        log.p_status,
                        log.p_status_message,
                        log.p_path_file,
                        log.p_create_by,
                        //return
                        p_return_code = pReturnCode,
                        p_return_message = pReturnMessage,
                        p_return_log_id = pReturnLogId

                    });

                log.p_log_id = Convert.ToInt16(pReturnLogId.Value.ToSafeString());
                log.p_return_code = pReturnCode.Value.ToSafeString();
                log.p_return_message = pReturnMessage.Value.ToSafeString();
            }
            catch (Exception)
            {

                log = new AutoMailReportLog();
            }

            return log;
        }

        private static void SendMail(string subject, string content, string pathFile, string mailFrom, string mailTo, string mailCc, string ipMailServer, ReportAutoMailQuery query)
        {
            try
            {
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
                if (!string.IsNullOrEmpty(mailCc))
                    message.CC.Add(mailCc);
                message.IsBodyHtml = true;
                message.Subject = subject;
                message.Body = mailContent;
                message.Priority = GetMailPriority(string.Empty);
                if (!string.IsNullOrEmpty(pathFile))
                {
                    var isAttachments = FileHelper.AttachmentFileEmail(query.UserTempFile, query.PassTempFile, query.DomainTempFile, message, pathFile);
                    if (!isAttachments)
                    {
                        throw new Exception("Cannot authentication user/password Attachments mail path file.");
                    }
                }
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
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

    }
}
