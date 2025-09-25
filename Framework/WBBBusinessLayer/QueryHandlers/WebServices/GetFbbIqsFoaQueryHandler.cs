using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
    public class GetFbbIqsFoaQueryHandler : IQueryHandler<GetFbbIqsFoaQuery, FbbIqsFoaModel>
    {
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FbbIqsFoaData> _repositoryDataPayGFoa;
        private readonly IEntityRepository<FBB_EMAIL_PROCESSING> _repositoryEmailProcessing;
        private readonly IEntityRepository<FBB_CFG_LOV> _repositoryLov;
        private readonly IEntityRepository<FBB_SENDMAIL_LOG> _repositorySendmailLog;

        public GetFbbIqsFoaQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog,
            IEntityRepository<FbbIqsFoaData> repositoryDataPayGFoa,
            IEntityRepository<FBB_CFG_LOV> repositoryLov,
            IEntityRepository<FBB_EMAIL_PROCESSING> repositoryEmailProcessing,
            IEntityRepository<FBB_SENDMAIL_LOG> repositorySendmailLog)
        {
            _uow = uow;
            _intfLog = intfLog;
            _repositoryDataPayGFoa = repositoryDataPayGFoa;
            _repositoryLov = repositoryLov;
            _repositoryEmailProcessing = repositoryEmailProcessing;
            _repositorySendmailLog = repositorySendmailLog;
        }

        public FbbIqsFoaModel Handle(GetFbbIqsFoaQuery query)
        {
            InterfaceLogCommand log = null;
            var result = new FbbIqsFoaModel();

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, "", "GetFbbIqsFoaQuery",
                    "GetFbbIqsFoaQueryHandler", null, "FBB", "");

                const string stringQueryDataPayGFoa = "select * from table(PKG_FBB_FOA_SMSSURVEY.GetDataPayGFoa)";
                result.Data = _repositoryDataPayGFoa.SqlQuery(stringQueryDataPayGFoa).ToList();

                if (result.Data.Any())
                {
                    result.RecordCount = result.Data.Count - 1;

                    var lovNas = (from lov in _repositoryLov.Get() where lov.LOV_TYPE == "BATCH_FCPO" select lov);

                    //Get Authen Nas
                    var domin = "";
                    var username = "";
                    var passNas = "";// Fixed Code scan : var pwd = "";
                    var lovNasAuthen = lovNas.FirstOrDefault(authen => authen.LOV_NAME == "NAS_AUTHEN");
                    if (lovNasAuthen != null)
                    {
                        var arrayAuthen = lovNasAuthen.DISPLAY_VAL.ToSafeString().Split('|').ToArray();
                        domin = arrayAuthen[0];
                        username = arrayAuthen[1];
                        passNas = arrayAuthen[2];

                        //Mock เครื่องตนเอง
                        //domin = "aware.local";
                        //username = "";
                        //pwd = "";
                    }

                    //Get & Generate File Name + File Path
                    string filePath = "";
                    var lovNasGenerateFile = lovNas.FirstOrDefault(authen => authen.LOV_NAME == "NAS_PATH");
                    if (lovNasGenerateFile != null)
                    {
                        filePath = lovNasGenerateFile.DISPLAY_VAL.ToSafeString();
                        result.FileName = String.Format(lovNasGenerateFile.LOV_VAL1.ToSafeString(),
                            DateTime.Now.ToString("yyyyMMdd"));
                        result.FullPathName = filePath.ToSafeString() + result.FileName.ToSafeString();

                        //Mock Error
                        //filePath = filePath.Replace("TempfileData", "Tempfile");
                    }

                    byte[] dataAsBytes =
                        result.Data.Select(x => x.RAWDATA)
                            .SelectMany(s => Encoding.UTF8.GetBytes(s + Environment.NewLine))
                            .ToArray();
                    var inputStream = new MemoryStream(dataAsBytes);

                    //WriteFile
                    result.WriteFileResult = FileHelper.CopyFileStream(username, passNas, domin, filePath, result.FileName, inputStream);

                    //Update Lov
                    if (result.WriteFileResult)
                    {
                        var lovNasLastDate = lovNas.FirstOrDefault(authen => authen.LOV_NAME == "GENERATE_LAST_DATE");
                        if (lovNasLastDate != null)
                        {
                            lovNasLastDate.DISPLAY_VAL = DateTime.Now.ToString("dd/MM/yyyy");
                            _repositoryLov.Update(lovNasLastDate);
                            _uow.Persist();
                        }
                    }
                }

                //Send Email กรณี Process Success
                result.SendEmailResult = EmailMangement(result, log);

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, result, log, "Success", "", "");
                return result;
            }
            catch (IOException io)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, io, log, "Error", io.GetErrorMessage(), "");
                result.ReturnCode = -1;
                result.ReturnDesc = io.GetErrorMessage();
            }
            catch (UnauthorizedAccessException un)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, un, log, "Error", un.GetErrorMessage(), "");
                result.ReturnCode = -1;
                result.ReturnDesc = un.GetErrorMessage();
            }
            catch (Exception ex)
            {
                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, ex, log, "Error", ex.GetErrorMessage(), "");
                result.ReturnCode = -1;
                result.ReturnDesc = ex.GetErrorMessage();
            }

            //Send Email กรณี Process Fail
            result.SendEmailResult = EmailMangement(result, log);
            return result;
        }

        #region Send Email

        private bool EmailMangement(FbbIqsFoaModel resultHandler, InterfaceLogCommand logHandler)
        {
            var resultSendEmail = false;
            var sendmailLog = new FBB_SENDMAIL_LOG
            {
                CUST_ROW_ID = logHandler.OutInterfaceLogId.ToSafeString(),
                PROCESS_NAME = "Sended",
                CREATE_USER = "FBBIQSFOA",
                CREATE_DATE = DateTime.Now,
                FILE_NAME = ""
            };

            try
            {
                const string stringQueryEmailInfo = "select * from table(PKG_FBB_FOA_SMSSURVEY.GetEmailInfo)";
                var emailInfoData = _repositoryEmailProcessing.SqlQuery(stringQueryEmailInfo).ToList();
                var emailInfo = emailInfoData.FirstOrDefault();
                if (emailInfo != null)
                {
                    string subject;
                    string content;
                    var pathFile = resultHandler.FullPathName.ToSafeString();
                    var mailFrom = emailInfo.SEND_FROM.ToSafeString();
                    var mailTo = emailInfo.SEND_TO.ToSafeString();
                    var mailCc = emailInfo.SEND_CC.ToSafeString();
                    var ipMailServer = emailInfo.IP_MAIL_SERVER.ToSafeString();
                    var fbbIqsFoaData = resultHandler.Data.FirstOrDefault(r => r.ROWNUMBER == 0) ?? new FbbIqsFoaData();

                    if (resultHandler.WriteFileResult)
                    {
                        subject = "BATCH FCPO RUN - SUCCESS";
                        content = "<strong>BATCH FCPO RUN - SUCCESS</strong>";
                        content += "</br>";
                        content += "</br>";
                        content += "<strong>Batch run : </strong>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        content += "</br>";
                        content += "<strong>Get between date : </strong>" + fbbIqsFoaData.GENERATE_DATE.ToSafeString();
                        content += "</br>";
                        content += "<strong>Record count : </strong>" + resultHandler.RecordCount.ToSafeString();
                        content += "</br>";
                        content += "<strong>File name : </strong>" + resultHandler.FileName.ToSafeString();
                        content += "</br>";
                    }
                    else
                    {
                        subject = "BATCH FCPO RUN - FAILED";
                        content = "<strong>BATCH FCPO RUN - FAILED</strong>";
                        content += "</br>";
                        content += "</br>";
                        content += "<strong>Batch run : </strong>" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                        content += "</br>";
                        content += "<strong>Error message : </strong>" + "<font color='red'>" + resultHandler.ReturnDesc + "</font>";
                        content += "</br>";
                        content += "<strong>Get between date : </strong>" + fbbIqsFoaData.GENERATE_DATE.ToSafeString();
                        content += "</br>";
                        content += "<strong>Record count : </strong>" + resultHandler.RecordCount.ToSafeString();
                        content += "</br>";
                        content += "<strong>File name : </strong>" + resultHandler.FileName.ToSafeString();
                        content += "</br>";
                    }

                    SendMail(subject, content, pathFile, mailFrom, mailTo, mailCc, ipMailServer);
                    resultSendEmail = true;

                    sendmailLog.RETURN_CODE = "0";
                    sendmailLog.RETURN_DESC = "";
                }
            }
            catch (Exception ex)
            {
                resultSendEmail = false;

                sendmailLog.RETURN_CODE = "-1";
                sendmailLog.RETURN_DESC = ex.GetErrorMessage();
            }

            //Create SendEmail Log 
            _repositorySendmailLog.Create(sendmailLog);
            _uow.Persist();

            return resultSendEmail;
        }

        private static void SendMail(string subject, string content, string pathFile, string mailFrom, string mailTo, string mailCc, string ipMailServer)
        {
            try
            {
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

        #endregion Send Email

    }
}
