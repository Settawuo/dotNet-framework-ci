using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WBBContract.Queries.WebServices;

namespace FBBDeleteFileOrderCancel
{
    using CompositionRoot;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.WebServiceModels;

    public class FBBDeleteFileOrderCancelJob
    {
        #region Property

        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateDeleteFileByOrderNoCancelCommand> _updateDeleteFileByOrderNoCancelCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        private string _outErrorResult = string.Empty;
        private int _numCancel = 0;

        #endregion

        #region Constructor

        public FBBDeleteFileOrderCancelJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateDeleteFileByOrderNoCancelCommand> updateDeleteFileByOrderNoCancelCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _updateDeleteFileByOrderNoCancelCommand = updateDeleteFileByOrderNoCancelCommand;
            _intfLogCommand = intfLogCommand;
            _sendMail = sendMail;
        }

        #endregion

        #region Public Method

        public bool DeleteFileOrderCancel()
        {
            _logger.Info("Start DeleteFileOrderCancel");
            StartWatching();
            try
            {
                var detailDeleteFileOrder = GetListDeleteFileOrderCancel();
                List<FilePathByOrderNo> deletefileorderCur = detailDeleteFileOrder.RES_CANCEL_WF_CUR;

                // \\10.252.160.97\fbb
                var lovSourceNAS = GetLovList("FBB_CONSTANT", "Impersonate_App");

                string username = lovSourceNAS[0].LovValue1;
                string fromPass = lovSourceNAS[0].LovValue2;// Fixed Code scan : string password = lovSourceNAS[0].LovValue2;

                string ipAddress = lovSourceNAS[0].LovValue3;
                string sourceNAS = lovSourceNAS[0].LovValue4;

                if (detailDeleteFileOrder.RES_CODE != "-1")
                {
                    using (var impersonator = new Impersonator(username, ipAddress, fromPass, false))
                    {
                        _logger.Info("Authen : " + ipAddress);

                        int numCancel = 0;
                        foreach (var file in deletefileorderCur)
                        {
                            if (DeleteSourceFile(file))
                            {
                                UpdatePath(file);
                                numCancel++;
                            }
                        }

                        _numCancel = numCancel;
                    }

                    if (_numCancel == deletefileorderCur.Count())
                    {
                        InsertInterfacelog("Success", _numCancel);
                        _logger.Info("Success DeleteFileOrderCancel");
                        return true;
                    }
                    else
                    {
                        _outErrorResult += "Failed DeleteFileOrderCancel : List file != summary file" + Environment.NewLine;
                        InsertInterfacelog("Failed", _numCancel);
                        _logger.Info("Failed DeleteFileOrderCancel : List file != summary file");
                        return false;
                    }
                }
                else
                {
                    _outErrorResult += "Error ListDeleteFileOrderCancel : return -1" + Environment.NewLine;
                    InsertInterfacelog("Failed", _numCancel);
                    _logger.Info("Error ListDeleteFileOrderCancel : return -1");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _outErrorResult += "Error DeleteFileOrderCancel : " + ex.GetErrorMessage() + Environment.NewLine;
                InsertInterfacelog("Failed", _numCancel);
                _logger.Info("Error DeleteFileOrderCancel : " + ex.GetErrorMessage());
                return false;
            }
        }

        public FBBDeleteFileOrderCancelModel GetListDeleteFileOrderCancel()
        {
            FBBDeleteFileOrderCancelModel result = new FBBDeleteFileOrderCancelModel();
            var query = new GetListDeleteFileOrderCancelQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
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

        #endregion

        #region Private Method

        private bool DeleteSourceFile(FilePathByOrderNo file)
        {
            bool isDelete = false;

            Regex digitsOnly = new Regex(@"[/]");
            string newFilePath = digitsOnly.Replace(file.FILE_PATH, "\\");
            string sourceFileName = newFilePath + file.FILE_NAME;

            try
            {

                if (File.Exists(sourceFileName))
                {
                    File.Delete(sourceFileName);
                    isDelete = true;
                    _logger.Info("Delect Order No." + file.ORDER_NO + " Success.");
                }
                else
                {
                    _logger.Info("File Not Found Order No." + file.ORDER_NO + " : " + sourceFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Can't delete Order No." + file.ORDER_NO + " : " + sourceFileName + "\n" + ex.GetErrorMessage());
                //throw new Exception("Can't delete Order No." + file.ORDER_NO + " : " + sourceFileName + "\n" + ex.GetErrorMessage());
            }

            return isDelete;
        }

        private void UpdatePath(FilePathByOrderNo file)
        {
            Regex digitsOnly = new Regex(@"[/]");
            string newFilePath = digitsOnly.Replace(file.FILE_PATH, "\\");
            string sourceFileName = newFilePath + file.FILE_NAME;

            try
            {
                if (!UpdateFilePath(file.ORDER_NO))
                {
                    _logger.Info("Can't update Order No." + file.ORDER_NO + " : " + sourceFileName);
                }
                else
                {
                    _logger.Info("Update Order No." + file.ORDER_NO + " Success.");
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Can't update Order No." + file.ORDER_NO + " : " + sourceFileName + "\n" + ex.GetErrorMessage());
                //throw new Exception("Can't update Order No." + file.ORDER_NO + " : " + sourceFileName + "\n" + ex.GetErrorMessage());
            }
        }

        private bool UpdateFilePath(string orderNo)
        {
            var command = new UpdateDeleteFileByOrderNoCancelCommand()
            {
                P_ORDER_NO = orderNo
            };

            _updateDeleteFileByOrderNoCancelCommand.Handle(command);

            string result = command.RES_MESSAGE;
            if (result == "Success")
            {
                return true;
            }
            return false;
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        private void InsertInterfacelog(string outResult, int numCancel)
        {
            var inXmlParam = new FBBDeleteFileOrderCancelLogInParam();
            inXmlParam.RuningDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            int numTotal = numCancel;
            var outXmlParam = new FBBDeleteFileOrderCancelLogOutParam();
            if (outResult == "Success")
            {
                outXmlParam.DeleteFileSummary = numTotal + " File";
            }

            var log = StartInterfaceLog(inXmlParam, "LogFBBDeleteFileOrderCancel");
            EndInterfaceLog(outXmlParam, outResult, log);
        }

        private InterfaceLogCommand StartInterfaceLog<T>(T inXmlParam, string methodName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = "",
                METHOD_NAME = methodName,
                SERVICE_NAME = "FBBDeleteFileOrderCancel",
                IN_ID_CARD_NO = "",
                IN_XML_PARAM = inXmlParam.DumpToXml(),
                INTERFACE_NODE = "FBB",
                CREATED_BY = "Batch",
                CREATED_DATE = DateTime.Now
            };

            _intfLogCommand.Handle(dbIntfCmd);

            return dbIntfCmd;
        }

        private void EndInterfaceLog<T>(T outXmlParam, string outResult, InterfaceLogCommand log)
        {
            var dbEndIntfCmd = new InterfaceLogCommand
            {
                ActionType = ActionType.Update,
                IN_TRANSACTION_ID = "",
                OutInterfaceLogId = log.OutInterfaceLogId,
                REQUEST_STATUS = _outErrorResult != "" ? "Error" : "Success",
                OUT_RESULT = outResult,
                OUT_ERROR_RESULT = outResult,
                OUT_XML_PARAM = outXmlParam.DumpToXml(),
                UPDATED_BY = "Batch",
                UPDATED_DATE = DateTime.Now
            };

            _intfLogCommand.Handle(dbEndIntfCmd);
        }

        #endregion

        #region Send E-mail


        public void QueryDataToSendMail(bool isSuccess)
        {
            //TODO: GET Subject and detail 
            string strFrom = "";
            string strTo = "";
            string strCC = "";
            string strBCC = "";
            string IPMailServer = "";

            string subject = "";
            string content = "";

            try
            {
                int numTotal = _numCancel;
                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string strSubject = GetLovList("FBB_BATCH_EMAIL", "MAIL_SUBJECT_DELETE")[0].LovValue1;

                string strSuccess = GetLovList("FBB_BATCH_EMAIL", "MAIL_DETAIL_2")[0].LovValue1.ToString() + "<br>";
                //@"Today FBBDeleteFileOrderCancel is Success. Please see detail as below.<br>";
                string strError = @"Today FBBDeleteFileOrderCancel is Error. Please see detail as below.<br>" +
                                  //@"Error message: " + _outErrorResult + "<br>" +
                                  "Please find attached for more error detail.<br>";

                string strIntro = "";
                if (isSuccess)
                {
                    strIntro = strSuccess;
                }
                else
                {
                    strIntro = strError;
                }

                string strBody = GetLovList("FBB_BATCH_EMAIL", "MAIL_DETAIL_1")[0].LovValue1.ToString() + "<br>" +
                    //"Dear All<br>" +
                    strIntro +
                    @"DELETE file summary " + numTotal + " file<br>" +
                    GetLovList("FBB_BATCH_EMAIL", "MAIL_DETAIL_4")[0].LovValue1.ToString() + "<br>" +
                    GetLovList("FBB_BATCH_EMAIL", "MAIL_DETAIL_5")[0].LovValue1.ToString();
                //@"Thank you<br>FBBS";

                string[] sendResult = Sendmail("SEND_EMAIL_DELETE_FILE_BATCH", "BATCH", strFrom, strTo, strCC,
                    strBCC, strSubject, strBody, IPMailServer, FromPass, Port, Domaim);
            }
            catch (Exception ex)
            {
                _logger.Info(" Error QueryDataToSendMail :" + ex.GetErrorMessage());
            }
        }

        public string[] Sendmail(string processname, string createuser, string sendfrom, string sendto, string sendcc, string sendbcc
            , string subject, string body, string ip_mail_server, string frompass, string port, string domain)
        {
            List<MemoryStreamAttachFiles> files = new List<MemoryStreamAttachFiles>();
            if (string.IsNullOrEmpty(_outErrorResult))
            {
                files = null;
            }
            else
            {
                MemoryStreamAttachFiles errorAttFile = new MemoryStreamAttachFiles();
                errorAttFile.file = new MemoryStream(Encoding.UTF8.GetBytes(_outErrorResult));
                errorAttFile.fileName = "DeleteFileOrderCancel_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";
                files.Add(errorAttFile);
            }


            _logger.Info("Sending an Email : Start.");

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

                _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
                StopWatching("Sending an Email");

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
                _outErrorResult += "Sending an Email" + string.Format(" is error on execute : {0}.",
                                      ex.GetErrorMessage()) + Environment.NewLine;
                _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
                   ex.GetErrorMessage()));
                _logger.Info(ex.GetErrorMessage());

                StopWatching("Sending an Email");
            }

            return result;
        }

        #endregion
    }
}
