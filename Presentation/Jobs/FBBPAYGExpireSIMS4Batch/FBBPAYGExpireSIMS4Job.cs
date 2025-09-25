using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WBBBusinessLayer;
using System.Web;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using System.Configuration;
using WBBContract.Commands;
using System.Security.Permissions;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace FBBPAYGExpireSIMS4Batch
{
    class FBBPAYGExpireSIMS4Job
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<GenfileExpireSIMCommand> _genfile;

        protected string keyUser = ConfigurationManager.AppSettings["KeyUser"].ToSafeString();
        protected string keyPassword = ConfigurationManager.AppSettings["KeyPassword"].ToSafeString();

        const int LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int LOGON32_LOGON_INTERACTIVE = 9;

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        public FBBPAYGExpireSIMS4Job(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<GenfileExpireSIMCommand> genfile)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _genfile = genfile;
        }


        #region Gen Files
        public void MainGenFiles(string _result_directory,string result_filename)
        {

            try
            {
                var nasConnection = GetConnectionNasPAYG();
                if (nasConnection == null)
                {
                    // Can’t connect NAS
                    _logger.Info("FBBPAYGExpireSIMS4 : Can’t connect NAS");
                    // To do : insert fbss_load_file_log
                    //InsertLoadFileGenLog("FBBPayGLoadSIM", DateTime.Now, "Can’t connect NAS");
                }
                else
                {
                    //Connection NAS
                    if (string.IsNullOrEmpty(nasConnection.username) || string.IsNullOrEmpty(nasConnection.password)
                       || string.IsNullOrEmpty(nasConnection.domain) || string.IsNullOrEmpty(nasConnection.fullpath))
                    {
                        // Not found connect setting
                        _logger.Info("FBBPAYGExpireSIMS4 : Not found connect setting");
                        // To do : insert fbss_load_file_log
                        //InsertLoadFileGenLog("FBBPayGLoadSIM", DateTime.Now, "Not found connect setting");
                    }
                    else
                    {
                        var connectNasFlag = TestConectionNas(nasConnection.username, nasConnection.password, nasConnection.domain, nasConnection.fullpath);
                        if (connectNasFlag == false)
                        {
                            // Can't conncet NAS
                            _logger.Info("FBBPAYGExpireSIMS4 : Can't conncet NAS");
                        }
                        else
                        {
                            
                            string expireReport_S4_Filename = "";
                            string expireReport_DirectoryPath = "";

                            DateTime currDateTime = DateTime.Now;

                            string dateNow = currDateTime.ToString("yyyyMMdd");
                            string timenow = DateTime.Now.ToString("hhmmss");
                            

                            //Get-Report-Query
                            List<LoadExpireSimS4GetDataQueryModel> _Sheet_query = GetData();

                            if (_Sheet_query != null)
                            {
                                //mock
                                //result_directory = "C:\\ExpireSIMS4\\Target";
                                //result_directory = Path.Combine(nasConnection.fullpath, result_directory);
                                //string Nas_result_directory = Path.Combine(nasConnection.fullpath, _result_directory);
                                string Nas_result_directory = nasConnection.fullpath + _result_directory;

                                if (!Directory.Exists(Nas_result_directory))
                                {
                                    Directory.CreateDirectory(Nas_result_directory);
                                    _logger.Info("FBBPAYGExpireSIMS4 : Create path " + Nas_result_directory + "");

                                }
                                else
                                {
                                    //FILE-NAME
                                    string nameExpireSimReportGendat = result_filename + "_" + dateNow + "_" + timenow + ".dat";
                                    string nameExpireSimReportGensync = result_filename + "_" + dateNow + "_" + timenow + ".sync";

                                    int countrowData = 0;

                                    List<LoadExpireSimS4GetDataQueryModel> firstDataRow = new List<LoadExpireSimS4GetDataQueryModel>();
                                    LoadExpireSimS4GetDataQueryModel firstRowDataTemp = new LoadExpireSimS4GetDataQueryModel();
                                    firstRowDataTemp.data_buffer = "01|" + nameExpireSimReportGendat + "";

                                    //Gen-file-.dat
                                    firstDataRow.Add(firstRowDataTemp);
                                    WriteFile(nasConnection.username, nasConnection.password, nasConnection.domain, Nas_result_directory, firstDataRow, true , nameExpireSimReportGendat);
                                    //WriteFileLocal(result_directory, nameExpireSimReportGendat, firstDataRow);

                                    string finalFileNameWithPath = PrepareFileCreateNotDelete(Nas_result_directory, nameExpireSimReportGendat);
                                    //existfilepath1300 = finalFileNameWithPath;
                                    _logger.Info(string.Format("Create File done: {0}", finalFileNameWithPath));

                                    foreach (LoadExpireSimS4GetDataQueryModel datarow in _Sheet_query)
                                    {
                                        using (StreamWriter fileAppend = File.AppendText(finalFileNameWithPath))
                                        {
                                            fileAppend.WriteLine("02|" + datarow.data_buffer + "");
                                            fileAppend.Close();
                                            countrowData = countrowData + 1;
                                        }
                                    }

                                    //Write Count
                                    using (StreamWriter fileAppend = File.AppendText(finalFileNameWithPath))
                                    {
                                        fileAppend.WriteLine("09|" + countrowData + "");
                                        fileAppend.Close();
                                    }

                                    //Gen-Sync

                                    List<LoadExpireSimS4GetDataQueryModel> firstDataRowSync = new List<LoadExpireSimS4GetDataQueryModel>();
                                    LoadExpireSimS4GetDataQueryModel firstRowDataTempSync = new LoadExpireSimS4GetDataQueryModel();
                                    firstRowDataTempSync.data_buffer = "" + nameExpireSimReportGendat + " | " + countrowData + "";

                                    //Gen-file-.dat
                                    firstDataRowSync.Add(firstRowDataTempSync);
                                    //WriteFileLocal(result_directory, nameExpireSimReportGensync, firstDataRowSync);
                                    WriteFile(nasConnection.username, nasConnection.password, nasConnection.domain, Nas_result_directory, firstDataRowSync, true, nameExpireSimReportGensync);

                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info("FBBPAYGExpireSIMS4 : Exception conncet NAS section");
                Console.WriteLine($"FBBPAYGExpireSIMS4 Exception : " + ex.GetErrorMessage());
            }
            
        }

        private ConnectionConfig GetConnectionNasPAYG()
        {
            ConnectionConfig result = new ConnectionConfig();

            var nas = Get_FBSS_CONFIG_TBL("FBBPAYG_REPORT_EXPIRE_SIM", "NAS_CONNECTION", "Y").FirstOrDefault();
            if (nas != null)
            {
                result.username = string.IsNullOrEmpty(nas.VAL1) ? "" : EncryptionUtility.Decrypt(nas.VAL1, keyUser);
                result.password = string.IsNullOrEmpty(nas.VAL2) ? "" : EncryptionUtility.Decrypt(nas.VAL2, keyPassword);
                result.domain = nas.VAL3;
                result.fullpath = nas.VAL4;

                _logger.Info("Connection :  User -> " + result.username);
                _logger.Info("Connection :  Password -> " + result.password);
                return result;
            }
            else
            {
                return null;
            }
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool TestConectionNas(string username, string pwd, string domin, string sourceFullPathName)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    if (File.Exists(sourceFullPathName))
                    {
                        _logger.Info("Test Connection : Found path -> " + sourceFullPathName);
                    }
                    else
                    {
                        _logger.Info("Test Connection : Not found path -> " + sourceFullPathName);
                    }
                    isSuccess = true;
                }
            }
            catch (Exception)
            {
                isSuccess = false;
            }
            finally
            {
                if (wic != null)
                {
                    wic.Undo();
                }
            }

            return isSuccess;
        }

        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        public bool WriteFile(string username, string pwd, string domin, string pathName, List<LoadExpireSimS4GetDataQueryModel> data, bool writeNewFile = true,string filenameGen = "")
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            WindowsImpersonationContext wic = null;
            var isSuccess = false;

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();

                    //string[] filelist = Directory.GetFiles(PrepareFile(pathName, ""));
                    //foreach (string files in filelist)
                    //{
                    //    File.Delete(PrepareFile(pathName, files));
                    //}
                    
                    //isSuccess = GenerateFile(data, pathName, filenameGen);
                    if (writeNewFile)
                    {
                        isSuccess = GenerateFile(data, pathName, filenameGen);
                        _logger.Info("Generate File Destination: " + pathName );
                    }
                    else
                    {
                        //isSuccess = PrepareFileContinue(pathName, null, data);
                    }
                    //this.RemoveTargetFileOverWeek(_archiveLocalPath);
                }
            }
            catch (Exception e)
            {
                _logger.Info("Generate File Destination Error! : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        private string PrepareFileCreateNotDelete(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            if (File.Exists(finalFileNameWithPath))
            {
                return finalFileNameWithPath;
            }
            else
            {
                return finalFileNameWithPath = "";
            }

            //return finalFileNameWithPath;
        }

        public bool WriteFileLocal(string pathName, string fileName, List<LoadExpireSimS4GetDataQueryModel> data)
        {
            bool isSuccess;

            try
            {
                isSuccess = GenerateFile(data, pathName, fileName);
            }
            catch (Exception e)
            {
                _logger.Info("Write file => Exception : " + e.Message);
                isSuccess = false;
            }

            return isSuccess;
        }

        private bool GenerateFile(List<LoadExpireSimS4GetDataQueryModel> datas, string directoryPath, string fileName)
        {
            DateTime currDateTime = DateTime.Now;

            foreach (LoadExpireSimS4GetDataQueryModel data in datas)
            {
                // write file to target
                string finalFileNameWithPath = PrepareFile(directoryPath, fileName);
                _logger.Info(string.Format("Gen File At Target Path : {0}", finalFileNameWithPath));
                StreamWriter file = new StreamWriter(finalFileNameWithPath, true);
                file.WriteLine(data.data_buffer);
                file.Close();
                
            }

            //fileLog.Close();

            return true;
        }

        private string PrepareFile(string directoryPath, string fileName)
        {
            string finalFileNameWithPath = Path.Combine(directoryPath, fileName);
            // Check Directory
            if (!Directory.Exists(directoryPath))
            {
                _logger.Info("Directory path not found -> " + directoryPath);
                return "N";
                //Directory.CreateDirectory(directoryPath)
            };
            // Check File Duplicate
            if (File.Exists(finalFileNameWithPath)) File.Delete(finalFileNameWithPath);

            return finalFileNameWithPath;
        }

        #endregion

        #region Middle Functions
        public string CheckBatchDirectory()
        {
            string result_DR = "";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_EXPIRE_SIM", "DIRECTORY").FirstOrDefault();
                _logger.Info("PROGRAM_PROCESS: " + program_process.ACTIVEFLAG);
                if (program_process.VAL1 != null)
                    result_DR = program_process.VAL1;

                return result_DR;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckBatchDirectory : " + ex.Message);
                return result_DR;
            }

        }

        public string CheckProcessStart()
        {
            string result_FN = "N";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_EXPIRE_SIM", "PROCESS").FirstOrDefault();
                _logger.Info("PROGRAM_PROCESS: " + program_process.ACTIVEFLAG);
                if (program_process.ACTIVEFLAG == "Y")
                    result_FN = "Y";

                return result_FN;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception Check Process : " + ex.Message);
                return result_FN;
            }

        }

        public string CheckBatchFilename()
        {
            string result_FN = "";
            try
            {
                var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_REPORT_EXPIRE_SIM", "OUT_FILENAME").FirstOrDefault();
                _logger.Info("PROGRAM_PROCESS: " + program_process.ACTIVEFLAG);
                if (program_process.VAL1 != null)
                    result_FN = program_process.VAL1;

                return result_FN;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckBatchFilename : " + ex.Message);
                return result_FN;
            }

        }

        public string GetExpireReportData()
        {
            string result_FN = "";
            try
            {
                var program_process = Get_FBB_Expire_DATA_REPORT("FBBPAYG_REPORT_EXPIRE_SIM").FirstOrDefault();
                _logger.Info("PROGRAM GetExpireReportData: " + program_process.data_buffer);
                if (program_process.data_buffer != "")
                    result_FN = "";

                return result_FN;
            }
            catch (Exception ex)
            {
                _logger.Error("Exception CheckBatchFilename : " + ex.Message);
                return result_FN;
            }

        }

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
                _logger.Error("Exception Get_FBSS_CONFIG_TBL_LOV : " + ex.Message);
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

        public List<LoadExpireSimS4GetDataQueryModel> GetData()
        {
            //string DATA_BUFFER = "";
            try
            {
                var CFGqueryReportQuery = new GetCFGqueryReportQuery
                {
                    Sheet_Name = "FBBPAYG_REPORT_EXPIRE_SIM"
                };
                var _getCFGqueryReportQuery = _queryProcessor.Execute(CFGqueryReportQuery);
                if (_getCFGqueryReportQuery != null || _getCFGqueryReportQuery.Any())
                {
                    //use-query
                    string reportQuery = _getCFGqueryReportQuery[0].query_1;

                    var DATA_BUFFER_DEMO = Get_FBB_Expire_DATA_REPORT(reportQuery);

                    if(DATA_BUFFER_DEMO != null)
                    {

                    }

                    return DATA_BUFFER_DEMO;
                }
                else
                {
                    _logger.Info($"FBBSendMailAutoLMD Data not found.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.Info($"FBBSendMailAutoLMD getData ERROR :{ex.GetErrorMessage()}");
                return null;
            }
        }

        //public List<CFGqueryReportModel> Get_FBB_CFG_QUERY_REPORT(string _Sheet_Name)
        //{
        //    try
        //    {
        //        //var query = new GetCFGqueryReportQuery()
        //        //{
        //        //    Sheet_Name = _Sheet_Name
        //        //};
        //        //var CFGqueryReportModel = _queryProcessor.Execute(query);

        //        return CFGqueryReportModel;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Error("Exception Get_FBB_CFG_QUERY_REPORT : " + ex.Message);
        //        return null;
        //    }
        //}

        public List<LoadExpireSimS4GetDataQueryModel> Get_FBB_Expire_DATA_REPORT(string Sheet_query)
        {
            try
            {
                //string Sheet_query = "FBBPAYG_REPORT_EXPIRE_SIM";

                var query = new GetListExpireSimDataQuery()
                {
                    sheet_query = Sheet_query
                };
                var _CFGqueryExpireDataReportModel = _queryProcessor.Execute(query);

                return _CFGqueryExpireDataReportModel;

            }
            catch (Exception ex)
            {
                _logger.Error("Exception Get_FBB_Expire_DATA_REPORT : " + ex.Message);
                return null;
            }
        }
        #endregion
    }
}
