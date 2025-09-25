using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WBBContract.Queries.WebServices;

namespace FBBAutoMoveFileMaintainBatch
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

    public class FBBAutoMoveFileMaintainBatchJob
    {
        #region Property

        private Stopwatch _timer;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<UpdateFilePathByOrderNoMaintainCommand> _updateFilePathByOrderNoMaintainCommand;
        private readonly ICommandHandler<InterfaceLogCommand> _intfLogCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;

        private string _outErrorResult = string.Empty;
        private int _numCompleted = 0;
        private int _numCancelWf = 0;
        private int _numCancelFbbs = 0;
        //private int _countAll = 0;
        private string _folderYear = string.Empty;
        private string _folderMonth = string.Empty;

        #endregion

        #region Constructor

        public FBBAutoMoveFileMaintainBatchJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<UpdateFilePathByOrderNoMaintainCommand> updateFilePathByOrderNoMaintainCommand,
            ICommandHandler<InterfaceLogCommand> intfLogCommand,
            ICommandHandler<SendMailBatchNotificationCommand> sendMail
            )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _updateFilePathByOrderNoMaintainCommand = updateFilePathByOrderNoMaintainCommand;
            _intfLogCommand = intfLogCommand;
            _sendMail = sendMail;
        }

        #endregion

        #region Public Method

        public bool AutoMoveFile()
        {
            _logger.Info("Start AutoMovefile Maintain");
            StartWatching();

            try
            {
                var detailCompleted = GetFileNameCompleted();
                var detailCancelWF = GetFileNameCancelWF();
                var detailCancelFBSS = GetFileNameCancelFBSS();
                List<FilePathByOrderNo> completeCur = detailCompleted.RES_COMPLETE_CUR;
                List<FilePathByOrderNo> cancelWfCur = detailCancelWF.RES_CANCEL_WF_CUR;
                List<FilePathByOrderNo> cancelFbssCur = detailCancelFBSS.RES_CANCEL_FBSS_CUR;

                // \\10.252.160.97\fbb
                var lovSourceNAS = GetLovList("FBB_CONSTANT", "Impersonate_App_New");
                var lovDestNAS = GetLovList("FBB_CONSTANT", "Impersonate_App_New");

                // Test local => D:\FBB
                //var lovSourceNAS = GetLovList("FBB_CONSTANT", "Impersonate_New");
                //var lovDestNAS = GetLovList("FBB_CONSTANT", "Impersonate_New");

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
                AutoMoveFileBatchModel.Username = batchFolder.Where(p => p.Name == "USER_NAME").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.SourceSystem = batchFolder.Where(p => p.Name == "SOURCE_SYSTEM").ToList()[0].LovValue1;
                AutoMoveFileBatchModel.DocCode = batchFolder.Where(p => p.Name == "DOC_CODE").ToList()[0].LovValue1;

                // Get lov Folder Year Month
                _folderYear = batchFolder.Where(p => p.Name == "FBB_FOLDER_YEAR").ToList()[0].LovValue1;
                _folderMonth = batchFolder.Where(p => p.Name == "FBB_FOLDER_MONTH").ToList()[0].LovValue1;

                if (detailCompleted.RES_CODE != "-1")
                {
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

                        RemoveTextFile(destNAS);
                    }

                    if (_numCompleted + _numCancelWf + _numCancelFbbs == completeCur.Count + cancelWfCur.Count + cancelFbssCur.Count)
                    {
                        InsertInterfacelog("Success", _numCompleted, _numCancelWf, _numCancelFbbs);
                        //InsertInterfacelog("Success", completeCur.Count, cancelWfCur.Count, cancelFbssCur.Count);
                        _logger.Info("Success AutoMovefile Maintain");
                        return true;
                    }

                    InsertInterfacelog("Failed", _numCompleted, _numCancelWf, _numCancelFbbs);
                    //InsetInterfacelog("Failed", completeCur.Count, cancelWfCur.Count, cancelFbssCur.Count);
                    return false;
                }
                else
                {
                    InsertInterfacelog("Failed", _numCompleted, _numCancelWf, _numCancelFbbs);
                    //InsertInterfacelog("Failed", completeCur.Count, cancelWfCur.Count, cancelFbssCur.Count);
                    _outErrorResult += "Error AutoMovefile Maintain : Completed return -1";
                    _logger.Info("Error AutoMovefile Maintain : Completed return -1");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _outErrorResult += "Error AutoMovefile : " + ex.GetErrorMessage() + Environment.NewLine;
                _logger.Info("Error AutoMovefile Maintain : " + ex.GetErrorMessage());
                InsertInterfacelog("Failed", _numCompleted, _numCancelWf, _numCancelFbbs);
                //InsertInterfacelog("Failed", 0, 0, 0);

                return false;
            }
        }

        // Get File Name Completed
        public AutoMoveFileBatchModel GetFileNameCompleted()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameCompletedMaintainQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
        }

        // Get File Name Cancel WF
        public AutoMoveFileBatchModel GetFileNameCancelWF()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameCancelWFMaintainQuery() { };

            result = _queryProcessor.Execute(query);

            return result;
        }

        // Get File Name Cancel FBSS
        public AutoMoveFileBatchModel GetFileNameCancelFBSS()
        {
            AutoMoveFileBatchModel result = new AutoMoveFileBatchModel();
            var query = new GetFileNameCancelFBSSMaintainQuery() { };

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

        private int CopyFile(List<FilePathByOrderNo> files, string sourceNAS, string destNAS, string folderName, string FlagFolder)
        {
            int numFile = 0;
            foreach (var file in files)
            {
                Regex digitsOnly = new Regex(@"[^\d]");
                string oldfolderYearMonth = digitsOnly.Replace(file.FILE_PATH, "");
                string folderYearMonth = _folderMonth;
                string sourceFileName = Path.Combine(sourceNAS, oldfolderYearMonth, file.FILE_NAME);
                string destFileName = Path.Combine(destNAS, _folderYear, folderYearMonth, AutoMoveFileBatchModel.DocumentFolder + folderYearMonth, folderName + folderYearMonth, file.FILE_NAME);

                try
                {
                    if (File.Exists(sourceFileName))
                    {
                        string destPath = Path.GetDirectoryName(destFileName);

                        if (!Directory.Exists(destPath))
                        {
                            Directory.CreateDirectory(destPath);
                        }

                        File.Copy(sourceFileName, destFileName, true);

                        _logger.Info("Copy file : " + file.ORDER_NO + " " + folderName + ": " + sourceFileName + " successful.");

                        string filePath = _folderYear + "/" + folderYearMonth + "/" + AutoMoveFileBatchModel.DocumentFolder + folderYearMonth + "/" + folderName + folderYearMonth;

                        if (UpdatePath(file, filePath, destPath))
                        {
                            DeleteSourceFile(file, destFileName, sourceFileName);
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

        private void DeleteSourceFile(FilePathByOrderNo file, string destFileName, string sourceFileName)
        {
            try
            {
                if (File.Exists(destFileName))
                {
                    File.Delete(sourceFileName);
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
        }

        private bool UpdatePath(FilePathByOrderNo file, string filePath, string destPath)
        {
            bool isUpdate = false;
            try
            {
                if (!UpdateFilePathByOrderNo(file.ORDER_NO, filePath))
                {
                    _logger.Info("Can't update file :" + file.ORDER_NO + "\n" + destPath);
                    throw new Exception("Can't update file :" + file.ORDER_NO + "\n" + destPath);
                }
                else
                {
                    isUpdate = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Order No." + file.ORDER_NO + ": " + ex.GetErrorMessage());
                throw new Exception("Order No." + file.ORDER_NO + ": " + ex.GetErrorMessage());
            }
            return isUpdate;
        }

        private bool UpdateFilePathByOrderNo(string orderNo, string filePath)
        {
            var command = new UpdateFilePathByOrderNoMaintainCommand()
            {
                P_Order_No = orderNo,
                P_File_Path = filePath,
            };

            _updateFilePathByOrderNoMaintainCommand.Handle(command);

            string result = command.RES_MESSAGE;
            if (result == "Success")
            {
                return true;
            }
            return false;
        }

        private void GenerateTextFile(string destNAS)
        {
            if (!Directory.Exists(destNAS + @"\sff_file_move_2\"))
            {
                Directory.CreateDirectory(destNAS + @"\sff_file_move_2\");
            }

            int existFile = Directory.GetFiles(destNAS + @"\sff_file_move_2\").Count(f => f.Contains("FBB_MOVE_FILENAME_" + DateTime.Now.ToString("yyyyMMdd"))) + 1;
            AutoMoveFileBatchModel.TextFilePath = destNAS + @"\sff_file_move_2\" + "FBB_MOVE_FILENAME_" + DateTime.Now.ToString("yyyyMMdd") + "_" + existFile.ToString("0#") + ".TXT";

            using (StreamWriter sw = File.CreateText(AutoMoveFileBatchModel.TextFilePath))
            {
                sw.WriteLine("01|" + Path.GetFileName(AutoMoveFileBatchModel.TextFilePath));
            }
        }

        private void RemoveTextFile(string destNAS)
        {
            var lineCount = File.ReadLines(AutoMoveFileBatchModel.TextFilePath).Count() - 1;
            using (StreamWriter sw = new StreamWriter(AutoMoveFileBatchModel.TextFilePath, true))
            {
                sw.WriteLine("09|" + lineCount);
            }

            if (Directory.GetFiles(destNAS + @"\sff_file_move_2\").Count(f => f.Contains("FBB_MOVE_FILENAME_")) > 365)
            {
                string oldestTextFile = Directory.GetFiles(destNAS + @"\sff_file_move_2\").OrderBy(f => f.Contains("FBB_MOVE_FILENAME_")).First();
                string oldestFileName = Path.GetFileNameWithoutExtension(oldestTextFile);
                string rootFolderPath = destNAS + @"\sff_file_move_2\";
                string filesToDelete = oldestFileName.Remove(oldestFileName.Length - 3) + "*.TXT";
                string[] fileList = Directory.GetFiles(rootFolderPath, filesToDelete);
                foreach (var file in fileList)
                {
                    File.Delete(file);
                }
            }
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

        private void InsertInterfacelog(string outResult, int numCompleted, int numCancelWf, int numCancelFbbs)
        {
            var inXmlParam = new AutoMoveFileBatchLogInParam();
            inXmlParam.RuningDateTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            int numTotal = numCompleted + numCancelWf + numCancelFbbs;
            var outXmlParam = new AutoMoveFileBatchLogOutParam();
            if (outResult == "Success")
            {
                outXmlParam.RemoveFileSummary = numTotal + " File";
                outXmlParam.FileCompleted = numCompleted + " File";
                outXmlParam.FileCancelWF = numCancelWf + " File";
                outXmlParam.FileCancelFBSS = numCancelFbbs + " File";
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

            try
            {
                int numTotal = _numCompleted + _numCancelWf + _numCancelFbbs;
                string FromPass = "";// Fixed Code scan : string FromPassword = "";
                string Port = "";
                string Domaim = "";
                string strSubject = GetLovList("FBB_BATCH_EMAIL", "MAIL_SUBJECT")[0].LovValue1;

                string strSuccess = @"Today FBBAutoMoveFileMaintainBatch is Success. Please see detail as below.<br>";
                string strError = @"Today FBBAutoMoveFileMaintainBatch is Error. Please see detail as below.<br>" +
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
