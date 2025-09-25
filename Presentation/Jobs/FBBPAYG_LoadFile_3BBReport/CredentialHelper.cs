using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Security.Principal;
using FBBPAYG_LoadFile_3BBReport.Model;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using WBBBusinessLayer;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBContract;
using Renci.SshNet;
using WBBContract.Commands;

namespace FBBPAYG_LoadFile_3BBReport
{
    
    public class CredentialHelper
    {

        public ILogger _logger;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLog;
        public CredentialHelper(ILogger logger, ICommandHandler<InterfaceLogPayGCommand> intfLog)
        {
            this._logger = logger;
            _intfLog = intfLog;
        }

        protected int _dayCallback = ConfigurationManager.AppSettings["DayCallback"].ToSafeInteger();
        protected int _dayArchiveForOrigin = ConfigurationManager.AppSettings["DayArchiveForOrigin"].ToSafeInteger();
        protected int _dayArchiveForDestination = ConfigurationManager.AppSettings["DayArchiveForDestination"].ToSafeInteger();
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
        protected string _archiveLogPath = ConfigurationManager.AppSettings["archiveLog"].ToSafeString();
        protected string _archiveLocalPath = ConfigurationManager.AppSettings["archiveLocal"].ToSafeString();
        public string Host { get; set; }
        public int Port { get; set; }


        private InterfaceLogPayGCommand StartInterfaceLog<T>(T inXmlParam, InterfaceLogPayGCommand log)
        {
            var dbIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = ActionType.Insert,
                IN_TRANSACTION_ID = log.IN_TRANSACTION_ID,
                METHOD_NAME = log.METHOD_NAME,
                SERVICE_NAME = "Batch FBBPayG_LoadFile_3BBReport",
                IN_ID_CARD_NO = "",
                IN_XML_PARAM = log.IN_XML_PARAM.DumpToXml(),
                INTERFACE_NODE = log.INTERFACE_NODE,
                CREATED_BY = "FBBPAYG_BATCH",
                CREATED_DATE = DateTime.Now
            };
            _intfLog.Handle(dbIntfCmd);

            return dbIntfCmd;
        }
        private void EndInterfaceLog<T>(T outXmlParam, InterfaceLogPayGCommand log)
        {
            var dbEndIntfCmd = new InterfaceLogPayGCommand
            {
                ActionType = ActionType.Update,
                IN_TRANSACTION_ID = "",
                OutInterfaceLogId = log.OutInterfaceLogId,
                REQUEST_STATUS = log.REQUEST_STATUS,
                OUT_RESULT = log.OUT_RESULT,
                OUT_ERROR_RESULT = log.OUT_ERROR_RESULT,
                OUT_XML_PARAM = log.OUT_XML_PARAM.DumpToXml(),
                UPDATED_BY = "FBBPAYG_Batch",
                INTERFACE_NODE = log.INTERFACE_NODE,
                UPDATED_DATE = DateTime.Now
            };
            _intfLog.Handle(dbEndIntfCmd);
        }

        const int LOGON32_PROVIDER_DEFAULT = 0;
        // This parameter causes LogonUser to create a primary token.
        const int LOGON32_LOGON_INTERACTIVE = 9;

        [DllImport("advapi32.DLL", SetLastError = true)]
        public static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType,
            int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);

        private List<AuthenticationMethod> SetAuthenticationMethod(string UserName, string KeyFile)
        {
            var privateKey = new PrivateKeyFile(KeyFile);
            List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
            var keyFiles = new[] { privateKey };
            _methods.Add(new PrivateKeyAuthenticationMethod(UserName, keyFiles));

            return _methods;
        }



        public List<string> UploadFileToServer(string displayPath, string username, string pwd, string domin, DateTime startDate_, DateTime endDate_, string destinationFilePath)
        {
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable
            InterfaceLogCommand log = new InterfaceLogCommand();
            WindowsImpersonationContext wic = null;
            List<string> resp = new List<string>();
            bool isRemoveSucess;

            string returnStatusfile = "";
            string downloadSuccess = "";
            string uploadFail = "";
            string uploadSuccess = "";
            

            try
            {
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    string localFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Download/" + displayPath);
                    if (Directory.Exists(localFolderPath))
                    {
                        foreach (string filePath in Directory.GetFiles(localFolderPath))
                        {
                            DateTime lastModified = File.GetLastWriteTime(filePath);
                            string fileName = Path.GetFileName(filePath);
                            //if (lastModified.Date >= startDate_.Date && lastModified.Date <= endDate_.Date)
                            //{
                                string destinationFileName = Path.Combine(destinationFilePath, fileName);

                                try
                                {
                                    this._logger.Info((object)("Upload Process Start"));
                                    File.Copy(filePath, destinationFileName, true);
                                    uploadSuccess = displayPath + " : " + " File Upload : " + Path.GetFileName(filePath) + " : SUCCESS";
                                    this._logger.Info((object)((uploadSuccess)));
                                    File.Delete(filePath);
                                    resp.Add(uploadSuccess);
                                }
                                catch (Exception)
                                {
                                    this._logger.Info((object)($"Upload failed for file: " + Path.GetFileName(filePath) + " . Error: {ex.Message}"));
                                    uploadFail = displayPath + " : " + Path.GetFileName(filePath) + ": FAILED";
                                    resp.Add(uploadFail);
                                    this._logger.Info((object)(uploadFail));
                                }
                            //}
                                
                        }
                        //Directory.Delete(localFolderPath, true);
                    }
                }
                else
                {
                    returnStatusfile = "UPLOAD PROCESS FAIL (CONNECT NAS TEMP)";
                    this._logger.Info((object)(returnStatusfile));
                    isRemoveSucess = false;
                }
                return resp;
            }
            catch (Exception ex)
            {
                this._logger.Info((object)("UPLOAD PROCESS " + ex.ToString()));
                resp.Add(ex.ToSafeString());

            }
            finally
            {
                if (wic != null)
                    wic.Undo();
            }
            return resp;
        }
        

        /// <summary>
        ///
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <param name="domin"></param>
        /// <param name="query_date_delete"></param>
        /// <returns></returns>
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public string RemoveFileWithoutSourcePath(string username, string pwd, string domin, List<FbssConfigTBL> query_date_delete, string runningNumber)
        {
            _logger.Info((object)("--Process Delete Start--"));
            // ReSharper disable once UnusedVariable
            var widCurrent = WindowsIdentity.GetCurrent();
            // ReSharper disable once NotAccessedVariable

            WindowsImpersonationContext wic = null;
            bool isRemoveSucess;
            Dictionary<string, int> arrays = new Dictionary<string, int>();//เพิ่ม Array ตามชื่อ  Display Val
            //List<string> returnmsg = new List<string>();
            string returnmsg = "";
            InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();
            log.METHOD_NAME = "DeleteReport3BBPerformance";
            log.INTERFACE_NODE = "NAS_TEMP";
            string inParam = "";
            foreach(var nasPathInParam in query_date_delete)
            {
                inParam += Environment.NewLine + "Path NAS " + nasPathInParam.VAL2 +"|" + DateTime.Now.AddDays(-int.Parse(nasPathInParam.VAL1)).ToString("dd-MM-yyyy");
            }
            log.IN_XML_PARAM = inParam;
            string in_xml_param = "";
            string outResult = "";
            log.IN_TRANSACTION_ID = runningNumber;
            log = StartInterfaceLog("", log);
            for (int i = 0; i < query_date_delete.Count; i++)//สร้าง array ตามชื่อ Display Val
            {
                arrays[query_date_delete[i].DISPLAY_VAL] = 0;
            }
            try
            {
                
                var adminToken = new IntPtr();
                if (LogonUser(username, domin, pwd, 9, 0, ref adminToken) != 0)
                {
                    wic = new WindowsIdentity(adminToken).Impersonate();
                    log.OUT_RESULT = "SUCCESS";
                    log.REQUEST_STATUS = "SUCCESS";
                    _logger.Info((object)("NasTemp Connected (Delete)"));
                    string delete_result_string = "";
                    foreach (var item in query_date_delete)
                    {
                        int val1 = int.Parse(item.VAL1);// จำนวนวัน ที่ต้องลบ val1
                        DateTime resultDate = DateTime.Now.AddDays(-val1);//จะได้วันที่ ที่ต้องเอาไปหาต่อว่าไฟล์ไหนมี date modified มาก่อนวันนี้
                        
                        _logger.Info((object)("Delete before " + resultDate.ToString("dd-MM-yyyy")));
                        int count_delete_file = 0;
                        try
                        {
                            var files_delete = (from file in Directory.EnumerateFiles(item.VAL2) select file).ToList();
                            foreach (var file in files_delete)// วนไฟล์ใน path
                            {
                                DateTime firstModified = System.IO.File.GetLastWriteTime(file);// get date modified ของ file
                                string fileName = Path.GetFileName(file);
                                _logger.Info((object)(item.DISPLAY_VAL +" : "+fileName));

                                if (firstModified < resultDate)// เช็คว่าไฟล์ไหนมาก่อน resultDate
                                {

                                    if (File.Exists(file))
                                    {
                                        
                                        count_delete_file++;
                                        File.Delete(file);
                                        _logger.Info((object)(item.DISPLAY_VAL +" : "+ "File Name: " + fileName + " Deleted"));
                                        log.OUT_RESULT = "SUCCESS";
                                        //deleteStatus = "SUCCESS";
                                    }
                                    else
                                    {
                                        _logger.Info((object)("File Name: " + fileName + " Not Exist"));
                                    }
                                }
                                arrays[item.DISPLAY_VAL] += count_delete_file;//นับเข้า array ที่มีชื่อตาม display_val
                                delete_result_string = "- " + arrays[item.DISPLAY_VAL] + " = " + count_delete_file + " file"+"\n"; // string เอาไว้ส่ง email ของ delete
                                
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Info((object)("Error " + ex));
                        }
                        if (!string.IsNullOrEmpty(outResult))
                        {
                            outResult += ", ";
                        }
                        outResult += item.DISPLAY_VAL + " = " + count_delete_file;
                        in_xml_param += item.VAL2 + " | " + resultDate.ToString("ddMMyyyy");
                        log.OUT_XML_PARAM = outResult.ToSafeString();
                        //_logger.Info((object)(item.DISPLAY_VAL + " = " + count_delete_file));
                        returnmsg = outResult;
                    }
                    _logger.Info((object)(outResult));//แสดงผลรวม

                }
                else
                {
                    _logger.Info((object)("Connect to Nas Temp Failed (Delete)"));
                    log.OUT_RESULT = "FAILED";
                    log.REQUEST_STATUS = "FAILED";
                    //deleteStatus = "FAILED";
                    isRemoveSucess = false;
                }
            }
            catch (Exception ex)
            {
                _logger.Info((object)(ex));
                Console.WriteLine("ex: " + ex);
                log.OUT_ERROR_RESULT = ex.ToSafeString();/////ห้ามเกิน 1000 limit character
                log.OUT_XML_PARAM = ex.ToSafeString();
            }
            finally
            {
                if (wic != null)
                    wic.Undo();
            }
            EndInterfaceLog("", log);
            return returnmsg;
        }


    }
}
