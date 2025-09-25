using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBBPAYGCheckLoadFileALLToTable
{
    using System.Diagnostics;
    using System.IO;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    public class FBBPAYGCheckLoadFileALLToTableJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        public FBBPAYGCheckLoadFileALLToTableJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
               ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
            _sendMail = sendMail;
        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }
        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("PAYG start load file to table take : " + _timer.Elapsed);
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
        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }
        public void SendSms(string jobname)
        {
            var getMobile = Get_FBSS_CONFIG_TBL_LOV("FBB_MOBILE_ERROR_BATCH", "MOBILE_SMS").FirstOrDefault();
            if (getMobile != null)
            {
                if (!string.IsNullOrEmpty(getMobile.VAL1) && getMobile.DISPLAY_VAL == "Y")
                {
                    var mobile = getMobile.VAL1.Split(',');

                    foreach (var item in mobile)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var command = new SendSmsCommand();
                            command.FullUrl = "FBBPAYGCheckLoadFileALLToTable";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBPAYGCheckLoadFileALLToTable Error: " + jobname;
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
        #region LastMileByDistance

        private List<string> QueryBuildLastMileByDistance()
        {
            try
            {
                var query = new LastMileByDistanceBatchQuery();
                errorMsg = query.ErrorMessage;
                return _queryProcessor.Execute(query);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                SendSms("LastMileByDistanceJob");
                return null;
            }

        }
        public void ExecuteLastMileByDistance()
        {
            _logger.Info("lastmilebydistance start.");
            StartWatching();
            try
            {
                var data = QueryBuildLastMileByDistance();

                if (data != null && data.Any())
                {

                    switch (data.FirstOrDefault())
                    {
                        case "1":
                            _logger.Info("lastmilebydistance : Fail.");
                            break;
                        default:
                            _logger.Info("lastmilebydistance  : Success.");
                            break;
                    }
                }
                else
                {
                    _logger.Info("lastmilebydistance : The process in Packages have a problem, please check.");
                    _logger.Info(string.Format("lastmilebydistance : {0}", errorMsg.ToSafeString()));
                }

                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("lastmilebydistance :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                SendSms("LastMileByDistanceJob");
            }
        }
        #endregion
        #region PAYGLoadFiletoTable
        private List<string> QueryBuildPAYGLoadFiletoTable()
        {
            var query = new PAYGLoadFilToTableQuery();
            errorMsg = query.ErrorMessage;
            return _queryProcessor.Execute(query);
        }
        public void ExecutePAYGLoadFiletoTable()
        {
            _logger.Info("PAYGLoadFileToTable START.");
            StartWatching();
            try
            {
                var data = QueryBuildPAYGLoadFiletoTable();

                if (data != null && data.Any())
                {
                    switch (data.FirstOrDefault())
                    {
                        case "1":
                            _logger.Error("PAYG load file to table : Fail.");
                            break;
                        case "2":
                            _logger.Error("PAYG load file to table : Pass with Error.");
                            break;
                        case "3":
                            _logger.Error("Case not have some file and load file failed");
                            break;
                        default:
                            _logger.Info("PAYG load file to table : Success.");
                            break;
                    }
                }
                else
                {
                    _logger.Error("PAYG load file to table : The process in Packages have a problem, please check.");
                    _logger.Error(string.Format("PAYG load file to table : {0}", errorMsg.ToSafeString()));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("PAYG load file to table :" + string.Format(" is error on execute : {0}.",
                                 ex.GetErrorMessage()));
                _logger.Error(ex.RenderExceptionMessage());
                SendSms("PAYGLoadFileToTableJob");
            }
            finally
            {
                StopWatching();
            }
        }

        #endregion
        #region PAYGCheckLoadFile
        public void ExecutePAYGCheckLoadFile()
        {
            _logger.Info("PAYGCHECKLOADFILE start.");
            StartWatching();
            try
            {
                var data = QueryBuildPAYGCheckLoadFile();

                if (data != null && data.Any())
                {
                    switch (data.FirstOrDefault())
                    {
                        case "1":
                            _logger.Info("PAYG Check load file : Fail.");
                            break;
                        case "2":
                            SendSmsPayGCheckLoadFile(2);
                            _logger.Info("PAYG Check load file  : Pass with Error.");
                            break;
                        case "3":
                            SendSmsPayGCheckLoadFile(3);
                            _logger.Info("PAYG Check load file  :Case not have some file and load file failed");
                            break;
                        default:
                            _logger.Info("PAYG Check load file  : Success.");
                            break;
                    }
                }
                else
                {
                    _logger.Info("PAYG Check load file : The process in Packages have a problem, please check.");
                    _logger.Info(string.Format("PAYGCheck load file : {0}", errorMsg.ToSafeString()));
                }

                StopWatching();
            }
            catch (Exception ex)
            {
                _logger.Info("Check load file :" + string.Format(" is error on execute : {0}.",
                    ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                StopWatching();
                SendSms("PAYGCHECKLOADFILEJob");
                throw ex;
            }
        }

        private List<string> QueryBuildPAYGCheckLoadFile()
        {
            var query = new PAYGCheckLoadFileQuery();
            errorMsg = query.ErrorMessage;
            return _queryProcessor.Execute(query);
        }
        public void SendSmsPayGCheckLoadFile(int ret_code)
        {
            string strMobile = GetLovList("FBB_SMS", "mobile_no")[0].Text;
            string strMessage = GetLovList("FBB_SMS", "message")[0].LovValue1;
            var msg = GetLovList("FBB_SMS", "SMS_MESSAGES").FirstOrDefault();
            string message = "";
            switch (ret_code)
            {
                case 1:
                    _logger.Info("PAYG Check load file : Case 1");
                    message = msg.LovValue1.ToSafeString();
                    break;
                case 2:
                    _logger.Info("PAYG Check load file : Case 2");
                    message = msg.LovValue2.ToSafeString();
                    break;
                case 3:
                    _logger.Info("PAYG Check load file : Case 3");
                    message = msg.LovValue3.ToSafeString();
                    break;
                default:
                    _logger.Info("PAYG Check load file : Case default");
                    message = msg.LovValue4.ToSafeString();
                    break;
            }
            //List<string> mess = new List<string>();
            if (!string.IsNullOrEmpty(strMobile))
            {
                var mobile = strMobile.Split(',');
                //if (!string.IsNullOrEmpty(strMessage))
                //{
                //    mess = Split(strMessage, 100).ToList();
                //}
                foreach (var item in mobile)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        //foreach (var m in mess)
                        //{
                        var command = new SendSmsCommand();
                        command.FullUrl = "FBBPayGCheckLoadFile";
                        command.Source_Addr = "FBBBATCH";
                        command.Destination_Addr = item;
                        command.Transaction_Id = item;
                        command.Message_Text = message;
                        _sendSmsCommand.Handle(command);
                        //Thread.Sleep(15000);
                        //}

                    }

                }

            }

        }
        #endregion
        #region SendEMail()
        private void ExcuteSendEmail()
        {
            try
            {

                StartWatching();
                var query = new GetFixedOM010SendMailFileLogQuery() { };
                ReturnFBSSSendMailFileLogBatchModel result = _queryProcessor.Execute(query);
                QueryDataToSendMailOm010LoadFile(result);
                // StopWatching("StopWatching Batch");
            }
            catch (Exception ex)
            {
                _logger.Info("Error FBSSFixedOM010SendMailFileLogBatch  Log :" + string.Format(" is error on execute : {0}.", ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
                //  StopWatching("StopWatching Batch Error.");
            }

        }

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
                string strSubject = GetLovList("FBB_BATCH_EMAIL", "MAIL_SUBJECT_OM010_SENDMAIL_FILE_LOG")[0].LovValue1;
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
                        foreach (var result in resultModel.cur)
                        {
                            Resultems += result.file_name.ToSafeString() + " : ";
                            Resultems += result.message_logfile + "|" + result.modify_date;
                            Resultems += Environment.NewLine;
                        }
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
        private void StopWatching(string Message)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} : {1}", Message, _timer.Elapsed));

        }
        #endregion 
        public void Execute()
        {
            _logger.Info("Start FBBPAYGCheckLoadFileALLToTable.");

            ExecuteLastMileByDistance();
            ExecutePAYGLoadFiletoTable();
            ExecutePAYGCheckLoadFile();
            ExcuteSendEmail();
        }

    }
}
