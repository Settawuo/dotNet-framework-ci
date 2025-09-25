using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using WBBBusinessLayer;
using WBBBusinessLayer.CommandHandlers;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace FBBPayGTransMatdoc
{
    using FBBPayGTransMatdoc;
    using FBBPayGTransMatdoc.Model;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using FBBPayGTransMatdoc.Model;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;
    using WBBBusinessLayer;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;
    using System;
    using System.Data;
    using WBBContract;
    using WBBContract.Commands;
    using System.Linq;
    using System.Security.Principal;

    public class FBBPayGTransMatdocJob
    {
        private string errorMsg = string.Empty;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<InsertTranMatDocLoadFileLogCommand> _addLoadFileLogCommand;

        public FBBPayGTransMatdocJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
             ICommandHandler<InsertTranMatDocLoadFileLogCommand> addLoadFileLogCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
            _addLoadFileLogCommand = addLoadFileLogCommand;
        }

        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        //protected string _archiveLogPath = ConfigurationManager.AppSettings["archiveLog"].ToSafeString();
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
        protected string _archiveDomain = ConfigurationManager.AppSettings["archive_domain"].ToSafeString();

        protected string _pathTemp = ConfigurationManager.AppSettings["TEMPPATH"].ToSafeString();
        protected string _dominTemp = ConfigurationManager.AppSettings["TEMP_DOMAIN"].ToSafeString();

        protected string _pathSAP = ConfigurationManager.AppSettings["SAP"].ToSafeString();
        protected string _dominSAP = ConfigurationManager.AppSettings["SAP_DOMAIN"].ToSafeString();

        protected string _pathTarget = ConfigurationManager.AppSettings["TARGET"].ToSafeString();
        protected string _dominTarget = ConfigurationManager.AppSettings["TARGET_DOMAIN"].ToSafeString();
        protected string _userTemp;
        protected string _pwdTemp;
        protected string _userSAP;
        protected string _pwdSAP;
        protected string _userTarget;
        protected string _pwdTarget;
        protected string _userArchive;
        protected string _pwdArchive;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }


        public void ExecuteJob()
        {
            StartWatching();
            try
            {

                _logger.Info("Start FBBPayGTransMatdoc");

                var ConnectionNas = GetConnectionNasPAYG();

                //Matdoc Config Username & Password
                _userTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Username, _Key);
                _pwdTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Password, _Key);
                _userSAP = EncryptionUtility.Decrypt(ConnectionNas.NasSap.Username, _Key);
                _pwdSAP = EncryptionUtility.Decrypt(ConnectionNas.NasSap.Password, _Key);
                _userTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Username, _Key);
                _pwdTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Password, _Key);
                _userArchive = EncryptionUtility.Decrypt(ConnectionNas.NasArch.Username, _Key);
                _pwdArchive = EncryptionUtility.Decrypt(ConnectionNas.NasArch.Password, _Key);

                CredentialHelper crd = new CredentialHelper(_logger, _queryProcessor, _addLoadFileLogCommand);

                //////LocalPath
                //string sourceFullPathName = "C:\\MATdoc\\S4";
                //string archivePath = "C:\\MATdoc\\AR";
                //string tempPath = "C:\\MATdoc\\TEMP";
                //string targetPath = "C:\\MATdoc\\TG";

                ////Config-Appsetting
                string sourceFullPathName = _pathSAP;
                string archivePath = _archivePath;
                string tempPath = _pathTemp;
                string targetPath = _pathTarget;

                //get-roll-back-flag
                //var rollbackflag = GetLovList("FBBPAYGTRANSAIRNET_BATCH", "SAP_IN_ROLLBACK_FLAG");

                StringBuilder resultBF = crd.GetFileSAPNEWS4hana(
                        _userTemp,
                        _pwdTemp,
                        _dominTemp,
                        sourceFullPathName,
                        archivePath,
                        tempPath
                );

                //trim-result
                string result = "";
                if (resultBF.ToSafeString() == "")
                {
                    _logger.Info("No data!");

                    // No Data then delete file in temp path

                    crd.DeleteFileTemp(
                        _userTemp,
                        _pwdTemp,
                        _dominTemp,
                        sourceFullPathName,
                        archivePath,
                        tempPath
                    );

                    //DeleteFileTemp(crd, tempPath);

                    StopWatching("Close!");

                    _logger.Info("FBBPayGTransMatdoc End");

                    return;
                }

                if (resultBF.ToSafeString() != "")
                {
                    result = resultBF.ToSafeString().Trim('|');
                }

                // Check result file and call procedure
                List<string> resultFileName = new List<string>();
                string[] tempFileName = result.ToSafeString().Split('|');

                // GetFileChunkList
                var groupedFileList = crd.GetFileChunkList(_userTemp,
                        _pwdTemp,
                        _dominTemp,
                        sourceFullPathName,
                        archivePath, tempPath, tempFileName);

                // Update ship to on Store Procedure
                resultFileName = UpdateShipToData(groupedFileList, tempPath, crd, _userTemp, _pwdTemp, _dominTemp);

                #region Copy file in Temp NAS to Archive NAS and Target NAS.

                // Test Archive Nas & Target Nas
                var isAcessArchiveNas = CredentialHelper.TestConectionNas(_userArchive, _pwdArchive, _archiveDomain, archivePath);
                var isAcessTargetNas = CredentialHelper.TestConectionNas(_userTarget, _pwdTarget, _dominTarget, targetPath);
                if (isAcessArchiveNas && isAcessTargetNas)
                {
                    // Access Archive Nas and Target Nas
                    // Copy File Result to Archive Path
                    foreach (var filename in resultFileName)
                    {
                        //// Copy to STG
                        // Add file result name into copy list
                        SAPModel.copyList = new List<string>();
                        SAPModel.copyList.Add(filename);
                        // copy file
                        bool copyFile = crd.CopyFile(_userArchive,
                                                     _pwdArchive,
                                                     _archiveDomain,
                                                     tempPath,
                                                     archivePath);

                        if (copyFile)
                        {
                            _logger.Info(string.Format("Archive: {0}", filename));
                        }
                        else if (!copyFile)
                        {
                            // copy file is failed
                            _logger.Info(string.Format("Exception Message : Copy file failed (Archive): {0}", filename));
                        }
                    }

                    // Copy File Result to Target Path
                    foreach (var filename in resultFileName)
                    {
                        //// Copy to STG
                        // Add file result name into copy list
                        SAPModel.copyList = new List<string>();
                        SAPModel.copyList.Add(filename);
                        // copy file
                        bool copyFile = crd.CopyFile(_userTarget,
                                                     _pwdTarget,
                                                     _dominTarget,
                                                     tempPath,
                                                     targetPath);
                        if (copyFile)
                        {
                            _logger.Info(string.Format("Target: {0}", filename));
                        }
                        else if (!copyFile)
                        {
                            // copy file is failed
                            _logger.Info(string.Format("Exception Message : Copy file failed (Target) : {0}", filename));
                        }
                    }
                }
                else
                {
                    // Can not access Nas
                    if (!isAcessArchiveNas && !isAcessTargetNas)
                    {
                        // Can not access Archive Nas and Target Nas
                        _logger.Info("Exception Message : Access Denied SH_WIREDBB (Archive & Target Nas)");
                        //crd.InsertLoadFileGenLog("FBBPayGTransMatdoc", "Exception Message : Access Denied SH_WIREDBB (Archive & Target Nas)", "N");
                        crd.InsertLogAccessNas(
                            _userTemp,
                            _pwdTemp,
                            _dominTemp,
                            tempPath,
                            "Exception Message : Access Denied SH_WIREDBB (Archive & Target Nas)",
                            "N"
                            );
                    }
                    else
                    {
                        if (!isAcessArchiveNas)
                        {
                            // Can not access Archive Nas
                            _logger.Info("Exception Message : Access Denied SH_WIREDBB (Archive)");
                            //crd.InsertLoadFileGenLog("FBBPayGTransMatdoc", "Exception Message : Access Denied SH_WIREDBB (Archive)", "N");
                            crd.InsertLogAccessNas(
                            _userTemp,
                            _pwdTemp,
                            _dominTemp,
                            tempPath,
                            "Exception Message : Access Denied SH_WIREDBB (Archive)",
                            "N"
                            );
                        }
                        if (!isAcessTargetNas)
                        {
                            // Can not access Target Nas
                            _logger.Info("Exception Message : Access Denied SH_WIREDBB (Target)");
                            //crd.InsertLoadFileGenLog("FBBPayGTransMatdoc", "Exception Message : Access Denied SH_WIREDBB (Target)", "N");
                            crd.InsertLogAccessNas(
                            _userTemp,
                            _pwdTemp,
                            _dominTemp,
                            tempPath,
                            "Exception Message : Access Denied SH_WIREDBB (Target)",
                            "N"
                            );
                        }
                    }

                }
                #endregion

                #region Test in Local
                //// Copy File Result to Archive Path
                //foreach (var filename in resultFileName)
                //{
                //    // Copy in Local
                //    string archiveFullPath = System.IO.Path.Combine(archivePath, filename);
                //    string tempFullPath = System.IO.Path.Combine(tempPath, filename);
                //    if (System.IO.File.Exists(archiveFullPath))
                //    {
                //        crd.RemoveFile(archiveFullPath);
                //    }
                //    bool copyFile = CredentialHelper.CopyFile(tempFullPath, archiveFullPath);

                //    if (!copyFile)
                //    {
                //        // copy file is failed
                //        _logger.Info(string.Format("Exception Message : Access Denied SH_WIREDBB (Archive) : {0}", filename));
                //    }
                //}

                //// Copy File Result to Target Path
                //foreach (var filename in resultFileName)
                //{
                //    // Copy in Local
                //    string targetFullPath = System.IO.Path.Combine(targetPath, filename);
                //    string tempFullPath = System.IO.Path.Combine(tempPath, filename);
                //    if (System.IO.File.Exists(targetFullPath))
                //    {
                //        crd.RemoveFile(targetFullPath);
                //    }
                //    bool copyFile = CredentialHelper.CopyFile(tempFullPath, targetFullPath);
                //    if (!copyFile)
                //    {
                //        // copy file is failed
                //        _logger.Info(string.Format("Exception Message : Access Denied SH_WIREDBB (Target) : {0}", filename));
                //    }
                //}
                #endregion

         


            }
            catch (Exception ex)
            {
                //SendSms();
                StopWatching("Fail!");
                _logger.Error(ex.Message);
                _logger.Error(ex.StackTrace);
            }

            try
            {
                CredentialHelper crd = new CredentialHelper(_logger, _queryProcessor, _addLoadFileLogCommand);
                string tempPath = _pathTemp;

                #region Test in Local
                //string tempPath = "C:\\MATdoc\\TEMP";
                #endregion

                var ConnectionNas = GetConnectionNasPAYG();

                //Matdoc Config Username & Password
                _userTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Username, _Key);
                _pwdTemp = EncryptionUtility.Decrypt(ConnectionNas.NasTemp.Password, _Key);
                _userSAP = EncryptionUtility.Decrypt(ConnectionNas.NasSap.Username, _Key);
                _pwdSAP = EncryptionUtility.Decrypt(ConnectionNas.NasSap.Password, _Key);
                _userTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Username, _Key);
                _pwdTarget = EncryptionUtility.Decrypt(ConnectionNas.NasTarget.Password, _Key);
                _userArchive = EncryptionUtility.Decrypt(ConnectionNas.NasArch.Username, _Key);
                _pwdArchive = EncryptionUtility.Decrypt(ConnectionNas.NasArch.Password, _Key);

                //CredentialHelper crd = new CredentialHelper(_logger, _queryProcessor, _addLoadFileLogCommand);

                //////LocalPath
                //string sourceFullPathName = "C:\\MATdoc\\S4";
                //string archivePath = "C:\\MATdoc\\AR";
                //string tempPath = "C:\\MATdoc\\TEMP";
                //string targetPath = "C:\\MATdoc\\TG";

                ////Config-Appsetting
                string sourceFullPathName = _pathSAP;
                string archivePath = _archivePath;
                //string tempPath = _pathTemp;
                string targetPath = _pathTarget;

                crd.DeleteFileTemp(
                        _userTemp,
                        _pwdTemp,
                        _dominTemp,
                        sourceFullPathName,
                        archivePath,
                        tempPath
                    );

                StopWatching("Close!");

                _logger.Info("FBBPayGTransMatdoc End");

            }
            catch (Exception ex)
            {
                _logger.Error("Exception : " + ex.Message);
                _logger.Error("StackTrace : " + ex.StackTrace);
            }

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

        private ConnectionNasPAYGMatdocListResult GetConnectionNasPAYG()
        {
            var query = new GetConnectionNasPAYGMatdocQuery();
            var ss = _queryProcessor.Execute(query);
            return ss;
        }

        private PAYGTransAirnetListResult QueryBuild(string data)
        {
            var query = new PAYGTransAirnetQuery();
            query.f_list = data.Equals("") ? "0" : data;
            errorMsg = query.Return_Desc;
            return _queryProcessor.Execute(query);
        }

        //public Dictionary<string, List<PAYGTransAirnetFileList>> GetFileChunkList(string tempPath, string[] lstFileName)
        //{
        //    // Modify model to procedure
        //    var groupedFileList = new Dictionary<string, List<PAYGTransAirnetFileList>>();

        //    var widCurrent = WindowsIdentity.GetCurrent();
        //    // ReSharper disable once NotAccessedVariable
        //    InterfaceLogCommand log = new InterfaceLogCommand();
        //    WindowsImpersonationContext wic = null;
        //    List<string> resp = new List<string>();

        //    bool success = false;
        //    var lstTempFile = Directory.GetFiles(tempPath);
        //    var adminToken = new IntPtr();

        //    if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
        //    {

        //        if (Directory.Exists(tempPath))
        //    {
        //        // Get File เฉพาะ .dat
        //        var files = Directory.GetFiles(tempPath, "*.dat");
        //        foreach (var file in files)
        //        {
        //            try
        //            {
        //                // Get filename, file extension and content data
        //                string fileName = Path.GetFileNameWithoutExtension(file);
        //                string fileExtension = Path.GetExtension(file);
        //                string fileData = File.ReadAllText(file);

        //                // Check file name only 1200, 1300, 1800
        //                if (fileName.ToUpper().StartsWith("1200_AIRNET_INV") ||
        //                    fileName.ToUpper().StartsWith("1300_AIRNET_INV") ||
        //                    fileName.ToUpper().StartsWith("1800_AIRNET_INV"))
        //                {
        //                    if (!string.IsNullOrEmpty(fileData))
        //                    {
        //                        // Split data into 200 row per set.
        //                        var fileChunks = SplitDataByLineCount(fileData, 200);

        //                        // Set first index = 1 for ordering.
        //                        int chunkIndex = 1;

        //                        // Declare new list file data
        //                        var fileChunksList = new List<PAYGTransAirnetFileList>();

        //                        // Loop for add chunk data to new list file
        //                        foreach (var chunk in fileChunks)
        //                        {
        //                            fileChunksList.Add(new PAYGTransAirnetFileList
        //                            {
        //                                // Set chunk index follow filename ex. AAA_1.dat
        //                                // For ordering by chunk index after called from procedure.
        //                                file_name = $"{fileName}_{chunkIndex}{fileExtension}",
        //                                file_data = chunk   // Content data
        //                            });
        //                            chunkIndex++;
        //                        }
        //                        groupedFileList[fileName] = fileChunksList;
        //                    }
        //                    else
        //                    {
        //                        _logger.Error($"File : {fileName} is Empty");
        //                        //File.Delete(file);
        //                    }
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.Error($"Error reading file {file}: {ex.Message}");
        //            }
        //        }
        //    }
        //        else
        //        {
        //            _logger.Warn("Temp directory does not exist.");
        //        }

        //    }

        //    return groupedFileList;
        //}

        //public List<string> SplitDataByLineCount(string data, int maxLines)
        //{
        //    var chunks = new List<string>();
        //    var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // Support both \r\n and \n delimiters
        //    int totalLines = lines.Length;

        //    for (int start = 0; start < totalLines; start += maxLines)
        //    {
        //        var chunk = string.Join("\n", lines.Skip(start).Take(maxLines));
        //        if (!chunk.EndsWith("\n"))
        //        {
        //            chunk = chunk + "\n";
        //        }
        //        chunks.Add(chunk);
        //    }

        //    return chunks;
        //}

        public List<string> UpdateShipToData(Dictionary<string, List<PAYGTransAirnetFileList>> groupedFileList, string pathTemp, CredentialHelper crd, string username, string password, string domain)
        {
            // Declare result file name
            var resultFiles = new List<string>();

            _logger.Info($"Call PKG_FBBPAYG_AIRNET_INV_REC.p_fetch_trans_airnet_data");

            var fileDataDictionary = new Dictionary<string, List<(int chunkNumber, string fileData)>>();
            var response = new PAYGTransAirnetListResult();


          //  resultFiles.Clear();

            foreach (var chk in groupedFileList)
            {
                try
                {


                    fileDataDictionary.Clear();



                    // Get list value for call procedure
                    List<PAYGTransAirnetFileList> chunks = chk.Value;
                    var query = new PAYGEnhanceMatDocQuery
                    {
                        f_enchance_list = chunks
                    };
                    response = _queryProcessor.Execute(query);
                    if (response.Data != null && response?.Data.Count > 0)
                    {
                        foreach (var file in response.Data)
                        {
                            // Get filename and file content
                            string fileName2 = file.file_name;
                            string fileData2 = file.file_data;

                            // Split filename by _
                            var fileNameParts = Path.GetFileNameWithoutExtension(fileName2).Split('_');

                            // Keep filename except last character
                            string baseFileName = string.Join("_", fileNameParts.Take(fileNameParts.Length - 1));

                            // Keep last charactor of filename for ordering
                            int chunkNumber = int.Parse(fileNameParts.Last());

                            // Check exist filename in collection
                            if (!fileDataDictionary.ContainsKey(baseFileName))
                            {
                                // if not exist, new create.
                                // ex. 0000_TEST_TST_00000000_000000.dat,(1, TEST_DATA)
                                fileDataDictionary[baseFileName] = new List<(int, string)>();
                            }

                            // Add data to collection
                            // ex. (1,"TEST_DATA"), (2,"TEST_DATA"),...
                            fileDataDictionary[baseFileName].Add((chunkNumber, fileData2));

                        }

                        foreach (var entry in fileDataDictionary)
                        {
                            // Filename .dat
                            string finalFileName = $"{entry.Key}.dat";

                            // Sort chunks by asc
                            var sortedChunks = entry.Value.OrderBy(chunk => chunk.chunkNumber).ToList();

                            StringBuilder combinedFileData = new StringBuilder();
                            foreach (var chunk in sortedChunks)
                            {
                                if (!string.IsNullOrEmpty(chunk.fileData) && !chunk.fileData.StartsWith(entry.Key))
                                {
                                    combinedFileData.Append(chunk.fileData);
                                }
                            }
                            // Count 02 Line
                            int lineCountStartingWith02 = 0;
                            var combinedLines = combinedFileData.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                            lineCountStartingWith02 = combinedLines.Count(line => line.StartsWith("02|"));

                            // Check lineCountStartingWith02 
                            if (lineCountStartingWith02 == 0) continue;

                            // Line 09
                            if (combinedLines[combinedLines.Count() - 1].StartsWith("09|"))
                            {
                                combinedLines[combinedLines.Count() - 1] = $"09|{lineCountStartingWith02}";
                                combinedFileData = new StringBuilder();
                                combinedFileData.Append(string.Join("\n", combinedLines));
                                //for (var i = 0; i < combinedLines.Count(); i++)
                                //{

                                //}
                            }
                            else
                            {
                                combinedFileData.AppendLine($"09|{lineCountStartingWith02}");
                            }

                            string outputFilePath2 = Path.Combine(pathTemp, finalFileName);

                            //// Write file in Local
                            //File.WriteAllText(outputFilePath2, combinedFileData.ToString());// เขียนไฟล์ลง Folder Temp

                            // Write file in Staging
                            List<PAYGTransAirnetFileList> lstData = new List<PAYGTransAirnetFileList>();
                            PAYGTransAirnetFileList rowData = new PAYGTransAirnetFileList();
                            rowData.file_data = combinedFileData.ToString();
                            rowData.file_name = finalFileName;

                            lstData.Add(rowData);
                            crd.WriteFile(username, password, domain, pathTemp, finalFileName, lstData);

                            _logger.Info($"Merge File .dat : {finalFileName} Successfully");
                            if (!resultFiles.Contains(finalFileName))
                            {
                                resultFiles.Add(finalFileName);
                            }

                            // Change file extension
                            string syncFileName = Path.ChangeExtension(finalFileName, ".sync");
                            string syncFilePath = Path.Combine(pathTemp, syncFileName);
                            string syncFileContent = $"{finalFileName}|{lineCountStartingWith02}";

                            //// Write file in Local
                            //File.WriteAllText(syncFilePath, syncFileContent); //  เขียน .sync ลง Folder Temp

                            // Write file in Staging
                            lstData = new List<PAYGTransAirnetFileList>();
                            rowData = new PAYGTransAirnetFileList();
                            rowData.file_data = syncFileContent;
                            rowData.file_name = syncFileName;

                            lstData.Add(rowData);
                            crd.WriteFile(username, password, domain, pathTemp, syncFileName, lstData);

                            _logger.Info($"File .sync : {syncFileName} Create Successfully");
                            if (!resultFiles.Contains(syncFileName))
                            {
                                resultFiles.Add(syncFileName);
                            }
                        }
                    }
                    else
                    {
                        string fileName = chk.Key;
                        _logger.Info($"No Data Return from PKG for File: {fileName}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error Group File Process {chk.Key}: {ex.Message}");
                }
            }
            _logger.Info("Merge File Process Success.");

            return resultFiles;
        }

        //private bool DeleteFileTemp(CredentialHelper crd, string tmpPath)
        //{
        //    var widCurrent = WindowsIdentity.GetCurrent();
        //    // ReSharper disable once NotAccessedVariable
        //    InterfaceLogCommand log = new InterfaceLogCommand();
        //    WindowsImpersonationContext wic = null;
        //    List<string> resp = new List<string>();

        //    bool success = false;
        //    var lstTempFile = Directory.GetFiles(tmpPath);

        //    if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
        //    {
        //        wic = new WindowsIdentity(adminToken).Impersonate();

        //        foreach (var filename in lstTempFile)
        //        {
        //            var fullpath = Path.Combine(tmpPath, filename);
        //            if (File.Exists(fullpath))
        //            {
        //                // Code in STG
        //                bool deleteFile = crd.RemoveFile(
        //                    _userTemp,
        //                    _pwdTemp,
        //                    _dominTemp,
        //                    fullpath
        //                );

        //                //// Code in Local
        //                //bool deleteFile = crd.RemoveFile(fullpath);

        //                if (deleteFile)
        //                {
        //                    success = true;
        //                    _logger.Info(string.Format("Delete File success (Temp) : {0}", filename));
        //                }
        //                else
        //                {
        //                    _logger.Info(string.Format("Delete File unsuccess (Temp) : {0}", filename));
        //                }
        //            }

        //        }
        //    }
        //    return success;
        //}
    }
}
