using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBContract.Queries.FBBWebConfigQueries;
using System.Configuration;

namespace FBBPAYGLoadSIMS4Batch
{
    public class FBBPAYGLoadSIMS4BatchMain
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<LoadSimS4DataInsertCommand> _insertCommand;
        private readonly ICommandHandler<GetListSimDataQuery> _loadSIM;
        private readonly ICommandHandler<LoadSimS4DataUpdateFlagCommand> _updateFlagCommand;
        private readonly ICommandHandler<InsertGenLogCommand> _addLogCommand;
        private readonly ICommandHandler<InsertLoadFileLogCommand> _addLoadFileLogCommand;
        private readonly ICommandHandler<TruncateSIMSlocTempCommand> _SIMSloc;
        private readonly ICommandHandler<TruncateSIMSlocTempHVRCommand> _SIMSlocHVR;

        protected string keyUser = ConfigurationManager.AppSettings["KeyUser"].ToSafeString();
        protected string keyPassword = ConfigurationManager.AppSettings["KeyPassword"].ToSafeString();

        public FBBPAYGLoadSIMS4BatchMain(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<LoadSimS4DataInsertCommand> insertCommand,
            ICommandHandler<GetListSimDataQuery> loadSIM,
            ICommandHandler<LoadSimS4DataUpdateFlagCommand> updateFlagCommand,
            ICommandHandler<InsertGenLogCommand> addLogCommand,
            ICommandHandler<InsertLoadFileLogCommand> addLoadFileLogCommand,
            ICommandHandler<TruncateSIMSlocTempCommand> SIMSloc,
            ICommandHandler<TruncateSIMSlocTempHVRCommand> SIMSlocHVR)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _insertCommand = insertCommand;
            _loadSIM = loadSIM;
            _updateFlagCommand = updateFlagCommand;
            _addLogCommand = addLogCommand;
            _addLoadFileLogCommand = addLoadFileLogCommand;
            _SIMSloc = SIMSloc;
            _SIMSlocHVR = SIMSlocHVR;
        }

        #region ProcessLoadSIM
        public void ExecuteJob()
        {
            #region Local test
            //var listSimData = GetFBBPayGSIMDataList("FBBPAYG_LOAD_SIM");

            //var cmd = new LoadSimS4DataUpdateFlagCommand()
            //{
            //    flag_fbss = "Y",
            //    serial = "2030493508447",
            //    status = "Available",
            //    ret_code = "1",
            //    ret_msg = ""
            //};
            //var updFlag = UpdateFlagFBBPayGSIM(cmd);

            //InsertGenLog("FBBPayGLoadSIM", "", "", "", "", "", "", "", "");

            //var sysDate = DateTime.Now.ToString("yyyyMMdd");
            //InsertLoadFileGenLog("FBBPayGLoadSIM", DateTime.Now, "Can not connect NAS", "N");

            //var cmd = new LoadSimS4DataInsertCommand() {
            //    psp_file = "'1210210002614','0889082614','FMC_SIM-3in1','C3010','Reserved','Shop ST9','3G','32k+CT','Service','','Pending Preparation','Y', '10/12/2017 14:34:05', 'sffadmin', '20/11/2017 14:34:41', '', ''",
            //    pt_name = "FBBPAYG_SIM",
            //    ret_code = "1",
            //    ret_msg = "",
            //};

            //InsertFBBPayGSIM(cmd);

            //CredentialHelper crd2 = new CredentialHelper(_logger);
            //var nasConnection2 = GetConnectionNasPAYG();
            //bool res = ProcessLoadSIM(crd2, nasConnection2);
            #endregion

            var sysDate = DateTime.Now.ToString("yyyyMMdd");
            var filename = "FBBPayGLoadSIM_" + sysDate;

            var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
            if (program_process.DISPLAY_VAL == "Y")
            {
                try
                {
                    Console.WriteLine($" HVR");
                    //Step 1 truncate temp table fbbpayg_sim_sloc_temp before insert data & Connect HVR table name wfm_r8.wfs_team_attr (C#)
                    var command = new TruncateSIMSlocTempHVRCommand { };
                    _SIMSlocHVR.Handle(command);

                    if (command.RET_CODE == "0")
                    {
                        _logger.Info("TruncateSIMSlocTempHVRCommand : " + command.RET_MSG);
                        _logger.Info("Data From HVR Total : " + command.Total);
                        Console.WriteLine($"TruncateSIMSlocTempHVRCommand : " + command.RET_MSG);
                        Console.WriteLine($"Data From HVR Total : " + command.Total);

                        CredentialHelper crd = new CredentialHelper(_logger);

                        //1. Check Config SFF NAS
                        Console.WriteLine($"Get SFF NAS");
                        var nasConnection = GetConnectionNasPAYG();
                        if (nasConnection == null)
                        {
                            // Can’t connect NAS
                            _logger.Info("FBBPayGLoadSIM : Can’t connect NAS");
                            // To do : insert fbss_load_file_log
                            InsertLoadFileGenLog(filename, DateTime.Now, "Can’t connect NAS", "N");
                        }
                        else
                        {
                            //Connection NAS
                            if (string.IsNullOrEmpty(nasConnection.username) || string.IsNullOrEmpty(nasConnection.password)
                                || string.IsNullOrEmpty(nasConnection.domain) || string.IsNullOrEmpty(nasConnection.fullpath))
                            {
                                // Not found connect setting
                                _logger.Info("FBBPayGLoadSIM : Not found connect setting");
                                // To do : insert fbss_load_file_log
                                InsertLoadFileGenLog(filename, DateTime.Now, "Not found connect setting", "N");
                            }
                            else
                            {

                                var connectNasFlag = crd.TestConectionNas(nasConnection.username, nasConnection.password, nasConnection.domain, nasConnection.fullpath);
                                if (connectNasFlag == false)
                                {
                                    // Can't conncet NAS
                                    _logger.Info("FBBPayGLoadSIM : Can't conncet NAS");
                                }
                                else
                                {
                                    bool readResult = ProcessLoadSIM(crd, nasConnection);
                                    if (readResult)
                                    {
                                        ProcessWriteSIMData(crd, nasConnection);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Info("TruncateSIMSlocTempHVRCommand : Failed " + command.RET_MSG);
                        Console.WriteLine($"TruncateSIMSlocTempHVRCommand : Failed" + command.RET_MSG);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("ProcessLoadSIM Exception : " + ex.GetErrorMessage());
                    _logger.Error("ProcessLoadSIM StackTrace : " + ex.StackTrace);
                    Console.WriteLine($"Exception:" + ex.GetErrorMessage());
                }
            }
            else
            {
                try
                {
                    Console.WriteLine($" Shareplex");
                    //Step 1 truncate temp table fbbpayg_sim_sloc_temp before insert data & Connect share plex table name wfm_r8.wfs_team_attr (C#)
                    var command = new TruncateSIMSlocTempCommand { };
                    _SIMSloc.Handle(command);
                    if (command.RET_CODE == "0")
                    {
                        _logger.Info("$TruncateSIMSlocTempCommand : " + command.RET_MSG);
                        _logger.Info("Data From Shareplex Total : " + command.Total);
                        Console.WriteLine($"TruncateSIMSlocTempCommand : " + command.RET_MSG);
                        Console.WriteLine($"Data From Shareplex Total : " + command.Total);

                        CredentialHelper crd = new CredentialHelper(_logger);

                        //1. Check Config SFF NAS
                        Console.WriteLine($"Get SFF NAS");
                        var nasConnection = GetConnectionNasPAYG();
                        if (nasConnection == null)
                        {
                            // Can’t connect NAS
                            _logger.Info("FBBPayGLoadSIM : Can’t connect NAS");
                            // To do : insert fbss_load_file_log
                            InsertLoadFileGenLog(filename, DateTime.Now, "Can’t connect NAS", "N");
                        } else
                        {
                            //Connection NAS
                            if (string.IsNullOrEmpty(nasConnection.username) || string.IsNullOrEmpty(nasConnection.password)
                               || string.IsNullOrEmpty(nasConnection.domain) || string.IsNullOrEmpty(nasConnection.fullpath))
                            {
                                // Not found connect setting
                                _logger.Info("FBBPayGLoadSIM : Not found connect setting");
                                // To do : insert fbss_load_file_log
                                InsertLoadFileGenLog(filename, DateTime.Now, "Not found connect setting", "N");
                            } else
                            {
                                var connectNasFlag = crd.TestConectionNas(nasConnection.username, nasConnection.password, nasConnection.domain, nasConnection.fullpath);
                                if (connectNasFlag == false)
                                {
                                    // Can't conncet NAS
                                    _logger.Info("FBBPayGLoadSIM : Can't conncet NAS");
                                }
                                else
                                {
                                    bool readResult = ProcessLoadSIM(crd, nasConnection);
                                    if (readResult)
                                    {
                                        ProcessWriteSIMData(crd, nasConnection);
                                    }
                                }
                            }
                        } 
                    }
                    else
                    {
                        _logger.Info("$TruncateSIMSlocTempCommand : Failed " + command.RET_MSG);
                        Console.WriteLine($"TruncateSIMSlocTempCommand : Failed" + command.RET_MSG);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("ProcessLoadSIM Exception : " + ex.GetErrorMessage());
                    Console.WriteLine($"Exception:" + ex.GetErrorMessage());
                }
            }

        }

        private bool ProcessLoadSIM(CredentialHelper crd, ConnectionConfig nas)
        {
            Console.WriteLine($"LoadSIM");

            bool result = false;    // Result of Method
            var sysDate = DateTime.Now.ToString("yyyyMMdd");
            var sysTime = DateTime.Now.ToString("HHmmss");
            var pathDir = string.Empty;     // Path of Input file
            var inputFileName = string.Empty;           // file name
            var inputFileNameCsvFormat = string.Empty;  // .csv
            var inputFileNameSyncFormat = string.Empty; // .sync
            
            var fullFileNameCsv = string.Empty;         // fbb_fmc_monitor_sim_20221115.csv
            var fullFileNameSync = string.Empty;        // fbb_fmc_monitor_sim_20221115.sync

            var syncData = 0;           // No of data records
            var readSyncSuccess = false;    // Flag to check read sync file success

            var csvText = string.Empty;

            FbssConfigTBL config = new FbssConfigTBL();

            // Assign path data
            pathDir = nas.fullpath;
            //pathDir = config.VAL1;
            if (!Directory.Exists(pathDir))
            {
                _logger.Info("FBBPayGLoadSIM : Directory path not found");
                //Directory.CreateDirectory(directoryPath)
            } else
            {
                _logger.Info("FBBPayGLoadSIM : Directory path found");
            }

            // Local Test
            //pathDir = "C:\\Temp\\LoadSim";

            //3. Get File name, File format config
            Console.WriteLine($"Get File name, File format config");
            config = Get_FBSS_CONFIG_TBL("FBBPAYG_LOAD_SIM", "IN_FILENAME", "Y").FirstOrDefault();
            if (config == null || string.IsNullOrEmpty(config.VAL1) || string.IsNullOrEmpty(config.VAL2) || string.IsNullOrEmpty(config.VAL3))
            {
                // Not found file setting
                _logger.Info("FBBPayGLoadSIM : Not found file setting");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "Not found file setting", "N");
                return false;
            }

            inputFileName = config.VAL1;
            inputFileNameCsvFormat = config.VAL2;
            inputFileNameSyncFormat = config.VAL3;

            //inputFileName = "fbb_fmc_monitor_sim";
            //inputFileNameCsvFormat = ".csv";
            //inputFileNameSyncFormat = ".sync";

            fullFileNameCsv = string.Format("{0}_{1}{2}", inputFileName, sysDate, inputFileNameCsvFormat);
            fullFileNameSync = string.Format("{0}_{1}{2}", inputFileName, sysDate, inputFileNameSyncFormat);
            var fullPathSync = Path.Combine(pathDir, fullFileNameSync); //pathDir + fullFileNameSync;
            //var fullPathSync = crd.PrepareFileCreateNotDelete(pathDir, fullFileNameSync);

            // 4. Check exist .sync file
            if (!File.Exists(fullPathSync))
            {
                // .sync file not found
                _logger.Info("FBBPayGLoadSIM : .sync file not found");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog(fullFileNameSync, DateTime.Now, ".sync file not found", "N");
                return false;
            } else
            {
                //_logger.Info("FBBPayGLoadSIM : .sync file found -> " + fullPathSync);
            }

            // 5. Check sync file
            Console.WriteLine($"Read .SYNC file");
            var syncDataTemp = CredentialHelper.ReadFileText(nas.username, nas.password, nas.domain, fullPathSync);
            //var syncDataTemp = File.ReadAllText(fullPathSync);
            syncDataTemp = !string.IsNullOrEmpty(syncDataTemp) ? syncDataTemp.Trim().Replace("\n","").Replace("\r","") : syncDataTemp;
            
            if (string.IsNullOrEmpty(syncDataTemp))
            {
                // Read .sync file unsuccess
                _logger.Info("FBBPayGLoadSIM : No data found");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog(fullFileNameSync, DateTime.Now, "No data found", "N");
                return false;
            }
            else
            {
                _logger.Info("FBBPayGLoadSIM : .sync content -> " + syncDataTemp);
                syncData = Convert.ToInt16(syncDataTemp);
                readSyncSuccess = true;
            }

            if (readSyncSuccess)
            {
                // 6. Check .csv file
                Console.WriteLine($"Read .CSV file");
                var fullFileNamePath = Path.Combine(pathDir, fullFileNameCsv);
                //var fullFileNamePath = crd.PrepareFileCreateNotDelete(pathDir, fullFileNameCsv);
                if (!File.Exists(fullFileNamePath))
                {
                    // Not found .csv file
                    _logger.Info("FBBPayGLoadSIM : Not found .csv file");
                    // To do : insert fbss_load_file_log
                    InsertLoadFileGenLog(fullFileNameCsv, DateTime.Now, "Not found .csv file", "N");
                    return false;
                } else
                {
                    //_logger.Info("FBBPayGLoadSIM : Found .csv file" + fullFileNamePath);
                }

                // 7. Read .CSV file
                csvText = CredentialHelper.ReadFileText(nas.username, nas.password, nas.domain ,fullFileNamePath);
                //csvText = File.ReadAllText(fullFileNamePath);
                csvText = !string.IsNullOrEmpty(csvText) ? csvText.Trim().Replace("\r","") : csvText;
                
                if (string.IsNullOrEmpty(csvText))
                {
                    // No data found on csv file
                    _logger.Info("FBBPayGLoadSIM : No data found on csv file");
                    // To do : insert fbss_load_file_log
                    InsertLoadFileGenLog(fullFileNameCsv, DateTime.Now, "No data found on csv file", "N");
                    return false;
                }

                string msgCSV = string.Empty;
                var csvLineResult = csvText.Split(new[] { '\n' });
                var prefixLastRow = string.Empty;
                var countRow = 0;
                var countSuccess = 0;
                var countFail = 0;
                //string[] csvData = null;

                for (int i = 0; i <= csvLineResult.Length - 1; i++)
                {
                    var text = csvLineResult[i];
                    //text = text.Replace("'", "");
                    prefixLastRow = text.Substring(0, 5);
                    countRow++;

                    if (countRow <= 2)
                    {
                        continue;
                    }
                    else if (countRow > 2 && prefixLastRow != "Total")
                    {
                        // 9. Insert/Update to Table FBBPAYG_SIM
                        //csvData = text.Split(new[] { ',' });

                        // อัปเดทลง store procedure f_insert_table
                        // Create Model LoadSimS4DataInsertCommand
                        LoadSimS4DataInsertCommand data = new LoadSimS4DataInsertCommand()
                        {
                            psp_file = text,
                            pt_name = "FBBPAYG_SIM"
                        };

                        //_logger.Info("FBBPayGLoadSIM : Insert FBBPAYG_SIM -> " + text);

                        var res = InsertFBBPayGSIM(data);

                        // เก็บผล บันทึก Log
                        if (res.ret_code == "0")
                        {
                            // Insert or Update is Successful
                            countSuccess++;
                            //_logger.Info("FBBPayGLoadSIM : Insert Success");
                        }
                        else
                        {
                            countFail++;
                            //_logger.Info("FBBPayGLoadSIM : Insert Failed");
                        }

                        //// Local test
                        //if (true)
                        //{
                        //    // Insert or Update is Successful
                        //    countSuccess++;
                        //}
                        //else
                        //{
                        //    countFail++;
                        //}

                        //// clear csvData
                        //csvData = null;
                    }
                    else if (countRow > 2 && prefixLastRow == "Total")
                    {
                        // 10. Read CSV file success
                        if (syncData == countSuccess)
                        {
                            msgCSV = string.Format("Success : Total {0} row/rows Success {1} row/rows", countSuccess, countSuccess);
                        }
                        else
                        {
                            msgCSV = string.Format("Fail : Total row {0} <<Total success {1}  - Total failed {2}>>", countSuccess, countSuccess, countFail);
                        }

                        // 11. Add Log Table fbss_load_file_log
                        _logger.Info("FBBPayGLoadSIM : " + msgCSV);
                        // To do :
                        InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, msgCSV);

                        result = true;
                        break;
                    }
                    else
                    {
                        // Read csv file unsuccess
                        _logger.Info("FBBPayGLoadSIM : Read csv file unsuccess");
                        // To do : insert fbss_load_file_log
                        InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "Read csv file unsuccess", "N");
                        return false;
                    }
                }
                
            } else
            {
                // Read.sync file unsuccess
                _logger.Info("FBBPayGLoadSIM : Read .sync file unsuccess");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "Read .sync file unsuccess", "N");
            }

            return result;
        }

        private bool ProcessWriteSIMData(CredentialHelper crd, ConnectionConfig nas)
        {
            Console.WriteLine($"WriteLoadSIM");

            var outPathDir = string.Empty;          // Path directory
            var outputFileName = string.Empty;      // File name
            var outputFileNameDatFormat = string.Empty;     // .dat
            var outputFileNameSyncFormat = string.Empty;    // .sync
            var outFileNameDat = string.Empty;              // fbb_fmc_monitor_sim_20250123_155634.dat
            var outFileNameSync = string.Empty;             // fbb_fmc_monitor_sim_20250123_155634.sync
            var sysDate = DateTime.Now.ToString("yyyyMMdd");
            var sysTime = DateTime.Now.ToString("HHmmss");
            //var fullPathDir = string.Empty;

            //var inputFileName = string.Empty;

            // 12. Generate .DAT File
            Console.WriteLine($"Get config directory");
            var config = Get_FBSS_CONFIG_TBL("FBBPAYG_GEN_FILE_SIM", "DIRECTORY", "Y").FirstOrDefault();
            if (config == null || string.IsNullOrEmpty(config.VAL1))
            {
                // Not found path directory
                _logger.Info("FBBPayGLoadSIM : Not found path directory");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "Not found path directory", "N");
                return false;
            }

            outPathDir = nas.fullpath + "//" + config.VAL1;             //Path.Combine(nas.fullpath, config.VAL1);       // Nas
            //outPathDir = config.VAL1;     // Local Directory
            if (!Directory.Exists(outPathDir))
            {
                _logger.Info("FBBPayGLoadSIM : Write directory path not found -> " + outPathDir);
                Directory.CreateDirectory(outPathDir);
            }
            else
            {
                _logger.Info("FBBPayGLoadSIM : Write directory path found -> " + outPathDir);
            }

            //if (outPathDir.Contains("/"))
            //{
            //    if (!outPathDir.EndsWith("/"))
            //    {
            //        outPathDir += "/";
            //    }
            //}
            //else
            //{
            //    outPathDir += "\\";
            //}
            //outPathDir = "C:\\Temp\\LoadSim\\Output";

            // 13. Get File name, File format
            Console.WriteLine($"Get File name, File format");
            config = Get_FBSS_CONFIG_TBL("FBBPAYG_GEN_FILE_SIM", "OUT_FILENAME", "Y").FirstOrDefault();
            if (config == null || string.IsNullOrEmpty(config.VAL1) || string.IsNullOrEmpty(config.VAL2) || string.IsNullOrEmpty(config.VAL3))
            {
                // Not found file name setting
                _logger.Info("FBBPayGLoadSIM : Not found file name setting");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "Not found file name setting", "N");
                return false;
            }

            outputFileName = config.VAL1;
            outputFileNameDatFormat = config.VAL2;
            outputFileNameSyncFormat = config.VAL3;

            //outputFileName = "1200_AIRNET_INV";
            //outputFileNameDatFormat = ".dat";
            //outputFileNameSyncFormat = ".sync";

            // 14. Set file output name
            outFileNameDat = string.Format("{0}_{1}_{2}{3}", outputFileName, sysDate, sysTime, outputFileNameDatFormat);
            outFileNameSync = string.Format("{0}_{1}_{2}{3}", outputFileName, sysDate, sysTime, outputFileNameSyncFormat);

            Console.WriteLine($"Query Sim Data");
            //// 15. Query Sim Data to generate .DAT file
            var listSimData = GetFBBPayGSIMDataList("FBBPAYG_GEN_FILE_SIM");

            // Local test
            //var simDataText = string.Empty;
            //simDataText = File.ReadAllText("C:\\LoadSim\\sim_data.dat");
            //var simDataArr = simDataText.Split('\n').ToList();
            //var listSimData = new List<LoadSimS4GetDataQueryModel>();
            //foreach (var text in simDataArr)
            //{
            //    LoadSimS4GetDataQueryModel simS4data = new LoadSimS4GetDataQueryModel();
            //    simS4data.sim_data = text;
            //    listSimData.Add(simS4data);
            //}

            if (listSimData == null || listSimData.Count() <= 0)
            {
                // No data found
                _logger.Info("FBBPayGLoadSIM : Not found data buffer");
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "No data found", "N");
                return false;
            }

            if (listSimData.Count() >= 1 && string.IsNullOrEmpty(listSimData[0].DATA_BUFFER))
            {
                // No data found
                _logger.Info("FBBPayGLoadSIM : Not found data buffer -> " + listSimData.Count());
                // To do : insert fbss_load_file_log
                InsertLoadFileGenLog("FBBPayGLoadSIM_" + sysDate, DateTime.Now, "No data found", "N");
                return false;
            }
            else
            {
                _logger.Info("FBBPayGLoadSIM : Found data buffer");
            }

            //var simData = loadSimData.sim_data;

            // 16. Generate .DAT File
            Console.WriteLine($"Generate .DAT File");
            // Write 1st row
            int countDatSuccess = 0;
            List<PAYGTransAirnetFileList> data = new List<PAYGTransAirnetFileList>();
            var rowData = new PAYGTransAirnetFileList();
            rowData.file_data = "01|" + outFileNameDat;
            rowData.file_name = outFileNameDat;

            data.Add(rowData);
            // crd.WriteFileLocal(outPathDir, outFileNameDat, data, true);  // Local 
            var firstLineResult = crd.WriteFile(nas.username, nas.password, nas.domain, outPathDir, data, true);  // Nas
            //if (firstLineResult)
            //{
            //    _logger.Info("FBBPayGLoadSIM : Write .dat first line success");
            //} else
            //{
            //    _logger.Info("FBBPayGLoadSIM : Write .dat first line failed");
            //}
            // Next row
            //var simData = simDataText.Split(new[] { '\n' });
            var bufferData = string.Empty;
            for (int i = 0; i <= listSimData.Count() - 1; i++)
            {
                data = new List<PAYGTransAirnetFileList>();
                rowData = new PAYGTransAirnetFileList();
                bufferData = listSimData[i].DATA_BUFFER.Replace("\r", "");
                var serialSim = bufferData.Split(new[] { '|' })[0];
                if (!string.IsNullOrEmpty(bufferData))
                {
                    data = new List<PAYGTransAirnetFileList>();
                    rowData = new PAYGTransAirnetFileList();
                    rowData.file_data = "02|" + bufferData;
                    rowData.file_name = outFileNameDat;

                    data.Add(rowData);
                    //crd.WriteFileLocal(outPathDir, outFileNameDat, data, false);      // Local
                    var res = crd.WriteFile(nas.username, nas.password, nas.domain, outPathDir, data, false);        // Nas
                    if (res)
                    {
                        countDatSuccess++;
                    }
                }

                // To do : Update Table FBBPAYG_SIM
                var cmd = new LoadSimS4DataUpdateFlagCommand()
                {
                    flag_fbss = "N",
                    serial = serialSim,
                    status = "Available"
                };
                var updFlag = UpdateFlagFBBPayGSIM(cmd);

                bufferData = string.Empty;
            }

            // Last row
            data = new List<PAYGTransAirnetFileList>();
            rowData = new PAYGTransAirnetFileList();
            rowData.file_data = "09|" + countDatSuccess;
            rowData.file_name = outFileNameDat;

            data.Add(rowData);
            //crd.WriteFileLocal(outPathDir, outFileNameDat, data, false);      // Local
            var lastLineResult = crd.WriteFile(nas.username, nas.password, nas.domain, outPathDir, data, false);        // Nas

            //if (lastLineResult)
            //{
            //    _logger.Info("FBBPayGLoadSIM : Write .dat last line success");
            //}
            //else
            //{
            //    _logger.Info("FBBPayGLoadSIM : Write .dat last line failed");
            //}

            //17. Generate .SYNC file
            Console.WriteLine($"Generate .SYNC File");
            data = new List<PAYGTransAirnetFileList>();
            rowData = new PAYGTransAirnetFileList();
            rowData.file_data = outFileNameSync + "|" + countDatSuccess;
            rowData.file_name = outFileNameSync;

            data.Add(rowData);
            //crd.WriteFileLocal(outPathDir, outFileNameSync, data);
            var syncResult = crd.WriteFile(nas.username, nas.password, nas.domain, outPathDir, data, true);        // Nas

            //if (syncResult)
            //{
            //    _logger.Info("FBBPayGLoadSIM : Write .sync file success");
            //}
            //else
            //{
            //    _logger.Info("FBBPayGLoadSIM : Write .sync file failed");
            //}

            //18. Insert log
            var msgSync = string.Format("Success : {0} row/rows << File name.dat {1}, File name.sync {2}>>", countDatSuccess, outFileNameDat, outFileNameSync);
            //InsertGenLog("FBBPayGLoadSIM", msgSync, "", "", "", "", "", "", "");
            _logger.Info("FBBPayGLoadSIM : " + msgSync);

            return true;
        }
        #endregion

        #region Method
        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL_LOV(string _CON_TYPE, string _CON_NAME)
        {
            try
            {
                var query = new GetFbssConfigTBLQuery()
                {
                    CON_TYPE = _CON_TYPE,
                    CON_NAME = _CON_NAME
                };
                var _FbssConfig = _queryProcessor.Execute(query);
                return _FbssConfig;
            }
            catch (Exception ex)
            {
                _logger.Info("Get_FBSS_CONFIG_TBL_LOV Exception : " + ex.GetErrorMessage());
                Console.WriteLine($"Exception:" + ex.GetErrorMessage());
                return null;
            }
        }

        public List<FbssConfigTBL> Get_FBSS_CONFIG_TBL(string _CON_TYPE, string _CON_NAME, string _ACTIVE_FLAG)
        {
            var query = new GetFbssConfigTBLQuery()
            {
                CON_TYPE = _CON_TYPE,
                CON_NAME = _CON_NAME,
                ACTIVEFLAG = _ACTIVE_FLAG
            };
            var _FbssConfig = _queryProcessor.Execute(query);

            return _FbssConfig;
        }

        private ConnectionConfig GetConnectionNasPAYG()
        {
            ConnectionConfig result = new ConnectionConfig();

            var nas = Get_FBSS_CONFIG_TBL("FBBPAYG_LOAD_SIM", "NAS_CONNECTION", "Y").FirstOrDefault();
            if (nas != null)
            {
                result.username = string.IsNullOrEmpty(nas.VAL1) ? "" : EncryptionUtility.Decrypt(nas.VAL1, keyUser);
                result.password = string.IsNullOrEmpty(nas.VAL2) ? "" : EncryptionUtility.Decrypt(nas.VAL2, keyPassword);
                result.domain = nas.VAL3;
                result.fullpath = nas.VAL4;

                //_logger.Info("Connection :  User -> " + result.username);
                //_logger.Info("Connection :  Password -> " + result.password);
                return result;
            } else
            {
                return null;
            }
        }

        // Insert into FBBPAYG_SIM
        public LoadSimS4DataInsertCommand InsertFBBPayGSIM(LoadSimS4DataInsertCommand command)
        {
            var query = new LoadSimS4DataInsertCommand()
            {
                psp_file = command.psp_file,
                pt_name = command.pt_name,
                ret_code = command.ret_code,
                ret_msg = command.ret_msg
            };
            _insertCommand.Handle(query);

            return query;
        }

        // Get Sim Data to Write in .DAT file
        public List<LoadSimS4GetDataQueryModel> GetFBBPayGSIMDataList(string p_sheet_name)
        {
            var query = new GetListSimDataQuery() {
                p_sheet_name = p_sheet_name
            };
            var lstBufferData = _queryProcessor.Execute(query);

            return lstBufferData;
        }

        // Update flag_fbss is Y in table FBBPAYG_SIM
        public LoadSimS4DataUpdateFlagCommand UpdateFlagFBBPayGSIM(LoadSimS4DataUpdateFlagCommand command)
        {
            var query = new LoadSimS4DataUpdateFlagCommand()
            {
                flag_fbss = command.flag_fbss,
                serial = command.serial,
                status = command.status,
                ret_code = command.ret_code,
                ret_msg = command.ret_msg
            };
            _updateFlagCommand.Handle(query);

            return query;
        }

        // Call Procedure to insert into FBB_INTERFACE_LOG_PAYG
        public InsertGenLogCommand InsertGenLog(string pfn_name, string pf_name, string pt_name, string pin_xml, string pout_ret, string pexc_det, string pout_xml, string prow_cnt, string pret_msg)
        {
            var query = new InsertGenLogCommand()
            {
                pfn_name = pfn_name,
                pf_name = pf_name,
                pt_name = pt_name,
                pin_xml = pin_xml,
                pout_ret = pout_ret,
                pexc_det = pexc_det,
                pout_xml = pout_xml,
                prow_cnt = prow_cnt,
                pret_msg = pret_msg,
            };
            _addLogCommand.Handle(query);
            return query;

        }

        public InsertLoadFileLogCommand InsertLoadFileGenLog(string filename, DateTime filedate, string message, string flag_type = "Y")
        {
            var command = new InsertLoadFileLogCommand()
            {
                filename = filename,
                filedate = filedate.Date,
                message = message,
                flag_type = flag_type,
                ret_code = "0",
                ret_msg = ""
            };
            _addLoadFileLogCommand.Handle(command);
            return command;
        }

        
        #endregion
    }
}
