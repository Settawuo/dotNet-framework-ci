using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace FBSSFixedOM010SendMailFileLogBatch
{
    public class FBSSFixedOM010SendMailFileLogBatchJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        public FBSSFixedOM010SendMailFileLogBatchJob(
             ILogger logger,
             IQueryProcessor queryProcessor,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail
           )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendMail = sendMail;
        }
        public void Execute()
        {
            _logger.Info("Start Execute FBSSFixedOM010SendMailFileLogBatchJob Log.");

            try
            {

                StartWatching();
                var query = new GetFixedOM010SendMailFileLogQuery() { };
                ReturnFBSSSendMailFileLogBatchModel result = _queryProcessor.Execute(query);
                QueryDataToSendMailOm010LoadFile(result);
                StopWatching("StopWatching Batch");
            }
            catch (Exception ex)
            {
                _logger.Info("Error FBSSFixedOM010SendMailFileLogBatch  Log :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching("StopWatching Batch Error.");
            }
            //  StopWatching("StopWatching FBSSFixedOM010SendMailFileLogBatch Log take");

        }
        public List<LovValueModel> GetLovList(string type, string name = "")
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name
                };

                var lov = _queryProcessor.Execute(query);
                return lov;
            }
            catch (Exception ex)
            {
                _logger.Info("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }
        #region SendEmail
        public void QueryDataToSendMailOm010LoadFile(ReturnFBSSSendMailFileLogBatchModel resultModel)
        {
            //TODO: GET Subject and detail 
            string strFrom = "";
            string strTo = "";
            string strCC = "";
            string strBCC = "";
            string IPMailServer = "";

            try
            {
                StartWatching();

                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string strSubject = GetLovList("FBB_BATCH_EMAIL", "MAIL_SUBJECT_OM010_NOTIFY")[0].LovValue1;
                strSubject += "_" + DateTime.Now.ToString("ddMMyyyyHHmmss");
                string strSuccess = @"Today FBSSFixedOM010SendMailFileLogBatchJob is Success. Please see detail as below.<br>";
                string strError = @"Today FBSSFixedOM010SendMailFileLogBatchJob is Error. Please see detail as below.<br>" +
                                  //@"Error message: " + _outErrorResult + "<br>" +
                                  "Please find attached for more error detail.<br>";

                string strIntro = "";
                if (resultModel.ret_code == "0")
                {
                    strIntro = strSuccess;
                }
                else
                {
                    strIntro = strError;
                }



                string Resultems = string.Empty;
                int LengData = 0;
                if (resultModel.ret_code == "0")
                {
                    if (resultModel.cur != null && resultModel.cur.Count > 0)
                    {
                        var resultdata = resultModel.cur.Select(s =>
                            string.Concat(s.file_name, "|", s.message_logfile, "|", s.modify_date)
                        ).ToList();


                        Resultems = string.Join(Environment.NewLine, resultdata);

                        //foreach (var result in resultModel.cur)
                        //{
                        //    Resultems += result.message_logfile + "|" + result.modify_date;
                        //    Resultems += Environment.NewLine;
                        //}
                        LengData = resultModel.cur.Count();
                    }
                    else
                    {
                        Resultems += "Data Not Found." + "|" + DateTime.Now;
                    }

                }
                else
                {
                    Resultems += resultModel.ret_msg + "|" + DateTime.Now;
                    Resultems += Environment.NewLine;
                }

                string strBody = "Dear All<br>" +
                   strIntro +
                   @"<ul><li>Data summary : " + LengData + "</li></ul>" +
                   @"Thank you<br>FBBS";

                string[] sendResult = SendmailOm010LoadFile("SEND_EMAIL_FBSS_FIXED_OM0101", "BATCH", strFrom, strTo, strCC,
                    strBCC, strSubject, strBody, IPMailServer, FromPass, Port, Domaim, Resultems);

                StopWatching("StopWatching FBSSFixedOM010 SendMail FileLog Batch Log take");
            }
            catch (Exception ex)
            {
                _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
                StopWatching("StopWatching FBSSFixedOM010 SendMail FileLog Batch Log  Error");
            }
        }
        public string[] SendmailOm010LoadFile(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
             , string subject, string body, string ip_mail_server, string frompass, string port, string domain, string _OutAttFile)
        {
            List<MemoryStreamAttachFiles> files = new List<MemoryStreamAttachFiles>();

            if (string.IsNullOrEmpty(_OutAttFile))
            {
                files = null;
            }
            else
            {
                MemoryStreamAttachFiles AttFile = new MemoryStreamAttachFiles();
                AttFile.file = new MemoryStream(Encoding.UTF8.GetBytes(_OutAttFile));
                AttFile.fileName = "FBssMessage_logfile_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";
                files.Add(AttFile);

            }
            _logger.Info("Sending an Email FBSS Fixed OM010 SendMailFile LogBatch Job : Start.");

            string[] result = new string[2];

            StartWatching();
            try
            {
                var command = new SendMailBatchNotificationCommand
                {
                    ProcessName = processname,
                    CreateUser = createuser,
                    SendTo = sendto,
                    SendFrom = sendfrom,
                    Subject = subject,
                    Body = body,
                    FromPassword = frompass,
                    Port = port,
                    Domaim = domain,
                    IPMailServer = ip_mail_server,
                    msAttachFiles = files
                };


                _sendMail.Handle(command);

                if (command.ReturnMessage == "Success.")
                {
                    result[0] = "0";
                    result[1] = "";
                }
                else
                {
                    result[0] = "-1";
                    result[1] = command.ReturnMessage;
                }

            }
            catch (Exception ex)
            {

                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage() + result[1]);
                StopWatching("Sending an Email Error");
            }

            return result;
        }


        #endregion
        #region Watching

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching(string Message)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", Message, _timer.Elapsed));

        }
        #endregion
    }
}
