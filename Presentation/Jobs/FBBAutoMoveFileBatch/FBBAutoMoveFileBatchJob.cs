using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WBBContract.Queries.WebServices;

namespace FBBAutoMoveFileBatch
{
    using CompositionRoot;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.WebServiceModels;

    public class FBBAutoMoveFileBatchJob
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFilePathByOrderNoCommand> _updateFilePathByOrderNoCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        private string _outErrorResult = string.Empty;
        private int _numCompleted, _numCancelWf, _numCancelFbbs, _numOthers;

        //private int _countAll = 0;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public FBBAutoMoveFileBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateFilePathByOrderNoCommand> updateFilePathByOrderNoCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _updateFilePathByOrderNoCommand = updateFilePathByOrderNoCommand;
            _intfLogCommand = intfLogCommand;
            _sendMail = sendMail;
        }

        public bool AutoMoveFile()
        {
            _logger.Info("Start AutoMovefile");
            StartWatching();
            try
            {
                //var detail = GetFileName();
                //List<FilePathByOrderNo> completeCur = detail.RES_COMPLETE_CUR;
                //List<FilePathByOrderNo> cancelWfCur = detail.RES_CANCEL_WF_CUR;
                //List<FilePathByOrderNo> cancelFbssCur = detail.RES_CANCEL_FBSS_CUR;

                var detailCompleted = GetFileNameCompleted();
                var detailCancelWF = GetFileNameCancelWF();
                var detailCancelFBSS = GetFileNameCancelFBSS();
                var detailOthers = GetFileNameOthers();
                List<FilePathByOrderNo> completeCur = detailCompleted.RES_COMPLETE_CUR;
                List<FilePathByOrderNo> cancelWfCur = detailCancelWF.RES_CANCEL_WF_CUR;
                List<FilePathByOrderNo> cancelFbssCur = detailCancelFBSS.RES_CANCEL_FBSS_CUR;
                List<FilePathByOrderNo> othersCur = detailOthers.RES_OTHERS_CUR;


                // \\10.252.160.97\fbb
                var lovSourceNAS = GetLovList("FBB_CONSTANT", "Impersonate_App_New");
                var lovDestNAS = GetLovList("FBB_CONSTANT", "Impersonate_App_New");

                // Test local => D:\FBB
                //var lovSourceNAS = GetLovList("FBB_CONSTANT", "Impersonate_New");
                //var lovDestNAS = GetLovList("FBB_CONSTANT", "Impersonate_New");

                //-----
                //var lovSourceNASApp = GetLovList("FBB_CONSTANT", "Impersonate_App");
                //var lovDestNASApp = GetLovList("FBB_CONSTANT", "Impersonate_App_New");

                string username = lovSourceNAS[0].LovValue1;
                string password = lovSourceNAS[0].LovValue2;
                string ipAddress = lovSourceNAS[0].LovValue3;
                string sourceNAS = lovSourceNAS[0].LovValue4;
                string destNAS = lovDestNAS[0].LovValue4;

                var batchFolder = GetLovList("FBB_BATCH_FOLDER").Where(p => p.LovValue5 == "FBBOR006").ToList();
                AutoMoveFileBatchModel.DocumentFolder = batchFolder.Where(p => p.Name == "DOCUMENT").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.CompletedFolder = batchFolder.Where(p => p.Name == "COMPLETED").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.CancelWfFolder = batchFolder.Where(p => p.Name == "CANCEL_WORKFLOW").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.CancelFbbsFolder = batchFolder.Where(p => p.Name == "CANCEL_FBB").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.OthersFolder = "Others_";
                AutoMoveFileBatchModel.Username = batchFolder.Where(p => p.Name == "USER_NAME").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.SourceSystem = batchFolder.Where(p => p.Name == "SOURCE_SYSTEM").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.DocCode = batchFolder.Where(p => p.Name == "DOC_CODE").ToList()[0].LovValue1;

                using (var impersonator = new Impersonator(username, ipAddress, password, false))
                {
                    _logger.Info("Authen : " + ipAddress);
                    GenerateTextFile(destNAS);

                    Task taskComplete = Task.Factory.StartNew(() =>
                        CopyFile(completeCur, sourceNAS, destNAS, AutoMoveFileBatchModel.CompletedFolder, "COMPLETED"));
                    taskComplete.Wait();
                    _logger.Info("Task Complete Finished!");

                    Task taskCancelWf = Task.Factory.StartNew(() =>
                        CopyFile(cancelWfCur, sourceNAS, destNAS, AutoMoveFileBatchModel.CancelWfFolder, "CANCEL_WORKFLOW"));
                    taskCancelWf.Wait();
                    _logger.Info("Task Cancel Workflow Finished!");

                    Task taskCancelFbss = Task.Factory.StartNew(() =>
                        CopyFile(cancelFbssCur, sourceNAS, destNAS, AutoMoveFileBatchModel.CancelFbbsFolder, "CANCEL_FBSS"));
                    taskCancelFbss.Wait();
                    _logger.Info("Task Cancel FBSS Finished!");

                    Task taskOthers = Task.Factory.StartNew(() =>
                        CopyFile(othersCur, sourceNAS, destNAS, AutoMoveFileBatchModel.OthersFolder, "OTHERS"));
                    taskCancelFbss.Wait();
                    _logger.Info("Task Others Finished!");

                    //_numCompleted = CopyFile(completeCur, sourceNAS, destNAS, AutoMoveFileBatchModel.CompletedFolder);
                    //_numCancelWf = CopyFile(cancelWfCur, sourceNAS, destNAS, AutoMoveFileBatchModel.CancelWfFolder);
                    //_numCancelFbbs = CopyFile(cancelFbssCur, sourceNAS, destNAS, AutoMoveFileBatchModel.CancelFbbsFolder);

                    RemoveTextFile(destNAS);
                }

                if (_numCompleted + _numCancelWf + _numCancelFbbs + _numOthers == completeCur.Count + cancelWfCur.Count + cancelFbssCur.Count + othersCur.Count)
                {
                    InsetInterfacelog("Success", _numCompleted, _numCancelWf, _numCancelFbbs, _numOthers);
                    //InsetInterfacelog("Success", completeCur.Count, cancelWfCur.Count, cancelFbssCur.Count);
                    _logger.Info("Success AutoMovefile");
                    return true;
                }

                InsetInterfacelog("Failed", _numCompleted, _numCancelWf, _numCancelFbbs, _numOthers);
                //InsetInterfacelog("Failed", completeCur.Count, cancelWfCur.Count, cancelFbssCur.Count);
                return false;
            }
            catch (Exception ex)
            {
                _outErrorResult += "Error AutoMovefile : " + ex.GetErrorMessage() + Environment.NewLine;
                _logger.Info("Error AutoMovefile : " + ex.GetErrorMessage());
                InsetInterfacelog("Failed", _numCompleted, _numCancelWf, _numCancelFbbs, _numOthers);
                //InsetInterfacelog("Failed", 0 , 0 , 0);

                return false;
            }
        }

        private void RemoveTextFile(string destNAS)
        {
            var lineCount = File.ReadLines(AutoMoveFileBatchModel.TextFilePath).Count() - 1;
            using (StreamWriter sw = new StreamWriter(AutoMoveFileBatchModel.TextFilePath, true))
            {
                sw.WriteLine("09|" + lineCount);
            }

            if (Directory.GetFiles(destNAS + @"\sff_file_move\").Count(f => f.Contains("FBB_MOVE_FILENAME_")) > 365)
            {
                string oldestTextFile = Directory.GetFiles(destNAS + @"\sff_file_move\").OrderBy(f => f.Contains("FBB_MOVE_FILENAME_")).First();
                string oldestFileName = Path.GetFileNameWithoutExtension(oldestTextFile);
                string rootFolderPath = destNAS + @"\sff_file_move\";
                string filesToDelete = oldestFileName.Remove(oldestFileName.Length - 3) + "*.TXT";
                string[] fileList = Directory.GetFiles(rootFolderPath, filesToDelete);
                foreach (var file in fileList)
                {
                    File.Delete(file);
                }
            }
        }

        private void GenerateTextFile(string destNAS)
        {
            if (!Directory.Exists(destNAS + @"\sff_file_move\"))
            {
                Directory.CreateDirectory(destNAS + @"\sff_file_move\");
            }

            int existFile = Directory.GetFiles(destNAS + @"\sff_file_move\").Count(f => f.Contains("FBB_MOVE_FILENAME_" + DateTime.Now.ToString("yyyyMMdd"))) + 1;
            AutoMoveFileBatchModel.TextFilePath = destNAS + @"\sff_file_move\" + "FBB_MOVE_FILENAME_" + DateTime.Now.ToString("yyyyMMdd") + "_" + existFile.ToString("0#") + ".TXT";

            using (StreamWriter sw = File.CreateText(AutoMoveFileBatchModel.TextFilePath))
            {
                sw.WriteLine("01|" + Path.GetFileName(AutoMoveFileBatchModel.TextFilePath));
            }
        }

        public AutoMoveFileBatchModel GetFileName()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
        }

        // R18.4
        public AutoMoveFileBatchModel GetFileNameCompleted()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameCompletedQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
        }

        // R18.4
        public AutoMoveFileBatchModel GetFileNameCancelWF()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameCancelWFQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
        }

        // R18.4
        public AutoMoveFileBatchModel GetFileNameCancelFBSS()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameCancelFBSSQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
        }

        public AutoMoveFileBatchModel GetFileNameOthers()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameOthersQuery() { };

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

        private int CopyFile(List<FilePathByOrderNo> files, string sourceNAS, string destNAS, string folderName, string FlagFolder)
        {
            int numFile = 0;
            foreach (var file in files)
            {
                string sourceFileName = "", destFileName = "";
                try
                {
                    Regex digitsOnly = new Regex(@"[^\d]");
                    string oldfolderYearMonth = digitsOnly.Replace(file.FILE_PATH, "");
                    string folderYearMonth = DateTime.Now.AddDays(-1).ToString("yyyyMM");
                    sourceFileName = Path.Combine(sourceNAS, oldfolderYearMonth, file.FILE_NAME);
                    destFileName = Path.Combine(destNAS, folderYearMonth, AutoMoveFileBatchModel.DocumentFolder + folderYearMonth, folderName + folderYearMonth, file.FILE_NAME);


                    if (File.Exists(sourceFileName))
                    {
                        string destPath = Path.GetDirectoryName(destFileName);

                        if (!Directory.Exists(destPath))
                        {
                            Directory.CreateDirectory(destPath);
                        }

                        File.Copy(sourceFileName, destFileName, true);
                        _logger.Info("Copy file : " + file.ORDER_NO + " " + folderName + ": " + sourceFileName + " successful.");
                        string filePath = folderYearMonth + "/" + AutoMoveFileBatchModel.DocumentFolder + folderYearMonth + "/" + folderName + folderYearMonth;
                        if (DeleteSourceFile(file, destFileName, sourceFileName))
                        {
                            UpdatePath(file, filePath, destPath);
                        }

                        if (folderName == AutoMoveFileBatchModel.CompletedFolder && file.FILE_NAME.StartsWith("FBB_EAPP"))
                        {
                            using (StreamWriter sw = new StreamWriter(AutoMoveFileBatchModel.TextFilePath, true))
                            {
                                sw.WriteLine("02" + "|" +
                                             file.ORDER_NO + "|" +
                                             file.NON_MOBILE_NO + "|" +
                                             AutoMoveFileBatchModel.Username + "|" +
                                             AutoMoveFileBatchModel.SourceSystem + "|" +
                                             AutoMoveFileBatchModel.DocCode + "|" +
                                             destPath + @"\|" +
                                             Path.GetFileName(destFileName));
                            }
                        }

                        numFile++;
                    }
                    else
                    {
                        _outErrorResult += "File Not Found Order No." + file.ORDER_NO + ": " + sourceFileName + Environment.NewLine;
                        _logger.Info("File Not Found Order No." + file.ORDER_NO + ": " + sourceFileName);
                        throw new Exception("File Not Found Order No." + file.ORDER_NO + ": " + sourceFileName);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info("Can't copy Order No." + file.ORDER_NO + ": " + sourceFileName + " to " + Path.GetDirectoryName(destFileName) +
                                 "\n" + ex.GetErrorMessage());
                }
            }

            if (FlagFolder == "COMPLETED") _numCompleted = numFile;
            else if (FlagFolder == "CANCEL_WORKFLOW") _numCancelWf = numFile;
            else if (FlagFolder == "CANCEL_FBSS") _numCancelFbbs = numFile;

            //_countAll += numFile;

            return numFile;
        }

        private bool DeleteSourceFile(FilePathByOrderNo file, string destFileName, string sourceFileName)
        {
            bool isDelete = false;
            try
            {
                if (File.Exists(destFileName))
                {
                    File.Delete(sourceFileName);
                    isDelete = true;
                }
                else
                {
                    _logger.Info("File Not Found Order No." + file.ORDER_NO + ": " + sourceFileName);
                    throw new Exception("File Not Found Order No." + file.ORDER_NO + ": " + sourceFileName);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Can't delete Order No." + file.ORDER_NO + ": " + sourceFileName + "\n" + ex.GetErrorMessage());
                throw new Exception("Can't delete Order No." + file.ORDER_NO + ": " + sourceFileName + "\n" + ex.GetErrorMessage());
            }

            return isDelete;
        }

        private void UpdatePath(FilePathByOrderNo file, string filePath, string destPath)
        {
            try
            {
                if (!UpdateFilePathByOrderNo(file.ORDER_NO, filePath))
                {
                    throw new Exception("Can't update file :" + file.ORDER_NO + "\n" + destPath);
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Order No." + file.ORDER_NO + ": " + ex.GetErrorMessage());
            }
        }

        private bool UpdateFilePathByOrderNo(string orderNo, string filePath)
        {
            var command = new UpdateFilePathByOrderNoCommand()
            {
                P_Order_No = orderNo,
                P_File_Path = filePath,
            };

            _updateFilePathByOrderNoCommand.Handle(command);

            string result = command.RES_MESSAGE;
            if (result == "Success")
            {
                return true;
            }
            return false;
        }

        private void InsetInterfacelog(string outResult, int numCompleted, int numCancelWf, int numCancelFbbs, int numOthers)
        {
            var inXmlParam = new AutoMoveFileBatchLogInParam();
            inXmlParam.RuningDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            int numTotal = numCompleted + numCancelWf + numCancelFbbs + numOthers;
            var outXmlParam = new AutoMoveFileBatchLogOutParam();
            if (outResult == "Success")
            {
                outXmlParam.RemoveFileSummary = numTotal + " File";
                outXmlParam.FileCompleted = numCompleted + " File";
                outXmlParam.FileCancelWF = numCancelWf + " File";
                outXmlParam.FileCancelFBSS = numCancelFbbs + " File";
                outXmlParam.FileOthers = numOthers + " File";

            }

            var log = StartInterfaceLog(inXmlParam, "LogAutoMoveFile");
            EndInterfaceLog(outXmlParam, outResult, log);
        }

        private InterfaceLogCommand StartInterfaceLog<T>(T inXmlParam, string methodName)
        {
            var dbIntfCmd = new InterfaceLogCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = "",
                METHOD_NAME = methodName,
                SERVICE_NAME = "Auto_Move_File_Batch",
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
                int numTotal = _numCompleted + _numCancelWf + _numCancelFbbs;
                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string strSubject = GetLovList("FBB_BATCH_EMAIL", "MAIL_SUBJECT")[0].LovValue1;

                string strSuccess = @"Today FBBAutoMoveFileBatch is Success. Please see detail as below.<br>";
                string strError = @"Today FBBAutoMoveFileBatch is Error. Please see detail as below.<br>" +
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

                string strBody = "Dear All<br>" +
                    strIntro +
                    @"Remove file summary " + numTotal + " file<br>" +
                    @"<ul><li>File Completed " + _numCompleted + " file</li>" +
                    @"<li>File Cancel_WF " + _numCancelWf + " file</li>" +
                    @"<li>File Cancel_FBSS " + _numCancelFbbs + " file</li></ul>" +
                    @"Thank you<br>FBBS";

                string[] sendResult = Sendmail("SEND_EMAIL_MOVE_FILE_BATCH", "BATCH", strFrom, strTo, strCC,
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
                errorAttFile.fileName = "AutoMoveFile_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".txt";
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