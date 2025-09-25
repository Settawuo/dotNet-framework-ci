using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBBPAYG_LoadFile_3BBReport.Model;
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
using System.IO;
using WBBContract.Commands.FBBWebConfigCommands;
using System.Globalization;
using Renci.SshNet;
using System.Xml.Linq;
using WBBEntity.Models;
using WBBContract.Queries.Commons.Masters;
using System.Net.Mail;
using System.Net;

namespace FBBPAYG_LoadFile_3BBReport
{


    public class FBBPayG_LoadFile_3BBReportBatch
    {
        private Stopwatch _timer;
        private string errorMsg = string.Empty;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        private readonly ICommandHandler<UpdateFbssFOAConfigTblCommand> _intfLogCommand;

        //private readonly ICommandHandler<InterfaceLogCommand> _intfLog;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLog;

        public FBBPayG_LoadFile_3BBReportBatch(ILogger logger, IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> sendSmsCommand, ICommandHandler<SendMailBatchNotificationCommand> sendMail,
            //ICommandHandler<InterfaceLogCommand> intfLog,
            ICommandHandler<UpdateFbssFOAConfigTblCommand> intfLogCommand,
            ICommandHandler<InterfaceLogPayGCommand> @interface)
        {
            this._logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = sendSmsCommand;
            //_intfLog = intfLog;
            _sendMail = sendMail;
            _intfLogCommand = intfLogCommand;
            _intfLog = @interface;
        }

        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        protected string _archiveLogPath = ConfigurationManager.AppSettings["archiveLog"].ToSafeString();
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
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

        public void ExecuteJob()
        {
            _logger.Info((object)("====================FBBPAYG_LoadFile_3BBReport Start===================="));
            var process = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "PROGRAM_PROCESS").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
            
            DateTime datenowNumber = DateTime.Now;
            string formatteddatenowNumber = datenowNumber.ToString("yyyyMMddHHmmss");
            Random random = new Random();
            long randomNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L;

            string runningNumber = formatteddatenowNumber + randomNumber;
            if (process != null)
            {
                this._logger.Info((object)($"PROGRAM PROCESS = " + process.DISPLAY_VAL));
                if (process.DISPLAY_VAL == "Y")
                {
                    InterfaceLogPayGCommand log = new InterfaceLogPayGCommand();

                    //Query Delete
                    var query_date_delete = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "DATE_DELETE")
                            .Where(record => record.ACTIVEFLAG == "Y")
                            .ToList();


                    //Query CREDENTIAL_NAS_TEMP
                    var nas_temp = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "NAS_TEMP")
                            .Where(record => record.ACTIVEFLAG == "Y")
                            .FirstOrDefault();
                    var usernamenas = nas_temp.VAL1;
                    var passwordnas = nas_temp.VAL2;
                    var domain_path_nas = nas_temp.VAL3;

                    //Decrypt NAS_TEMP
                    _userTemp = EncryptionUtility.Decrypt(usernamenas, _Key);
                    _pwdTemp = EncryptionUtility.Decrypt(passwordnas, _Key);



                    CredentialHelper crd = new CredentialHelper(_logger, _intfLog);

                    //NAS Temp DELETE ใช้ได้
                    var deletefile = crd.RemoveFileWithoutSourcePath(_userTemp, _pwdTemp, domain_path_nas, query_date_delete, runningNumber);

                    //DATE_START
                    var result_date_start = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "DATE_START")
                            .Where(record => record.ACTIVEFLAG == "Y")
                            .FirstOrDefault();
                    string date_start = result_date_start?.DISPLAY_VAL == "Y" ? result_date_start.VAL1.ToSafeString() : DateTime.Now.ToString("yyyyMMdd");

                    //DATE_TO
                    var result_date_to = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "DATE_TO")
                            .Where(record => record.ACTIVEFLAG == "Y")
                            .FirstOrDefault();
                    string date_to = result_date_to?.DISPLAY_VAL == "Y" ? result_date_to.VAL1.ToSafeString() : DateTime.Now.ToString("yyyyMMdd");

                    //Update DATE_START
                    var queryUpdateDate_Start = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBPayG_LoadFile_3BBReport",
                        con_name = "DATE_START",
                        display_val = "Y",
                        val1 = date_start,
                        val2 = date_to,
                        flag = "EQUIP",
                        updated_by = "FBBPayG_LoadFile_3BBReport",
                    };
                    _intfLogCommand.Handle(queryUpdateDate_Start);
                    

                    //CREDENTIAL_NAS_DATA_POWER
                    var nas_datapower = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "NAS_DATAPOWER").FirstOrDefault();
                    var usernamenas_datapower = nas_datapower.VAL1;
                    var public_key = nas_datapower.VAL2;
                    var private_key = nas_datapower.VAL3;
                    var domain_nas = nas_datapower.VAL4;


                    //REPORT3BB
                    var report_3bb = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_LoadFile_3BBReport", "REPORT3BB")
                            .Where(record => record.ACTIVEFLAG == "Y")
                            .ToList();

                    //Decrypt Datapower
                    var _usnDatapower = EncryptionUtility.Decrypt(usernamenas_datapower, _Key);


                    

                    
                        SftpService sftpconnect = new SftpService(_logger);
                        sftpconnect.Host = nas_datapower.DISPLAY_VAL;
                        sftpconnect.Port = nas_datapower.VAL4.ToSafeInteger();
                        string bodyEmail = "";
                        var downloadfileItem = "";
                        var uploadfileItem = "";
                        var resultBody = "";
                        string message = "";
                        string receiveFileName = "";
                        string sentFileName = "";
                        string currentDirectory = Directory.GetCurrentDirectory();
                        DateTime startDate_ = new DateTime();
                        DateTime endDate_ = DateTime.ParseExact(date_to, "yyyyMMdd", CultureInfo.InvariantCulture);
                        string formattedDate = "";
                        /////////////////////////

                        
                        
                        List<AuthenticationMethod> _methods = SetAuthenticationMethod(_usnDatapower, currentDirectory +"\\"+ private_key);
                        var con = new ConnectionInfo(sftpconnect.Host, sftpconnect.Port, username: _usnDatapower, authenticationMethods: _methods.ToArray());


                    try
                    {
                        this._logger.Info((object)($"DataPower Connecting..."));

                        using (var client = new SftpClient(con))
                        {
                            client.Connect();

                            if (client.IsConnected)
                            {
                                _logger.Info((object)("DataPower Connected"));

                                foreach (var row in report_3bb)// Loop เอา path จาก query REPORT3BB
                                {
                                    List<string> downloadfileList = new List<string>();
                                    List<string> uploadfileList = new List<string>();
                                    if (result_date_start.DISPLAY_VAL == "N")
                                    {
                                        startDate_ = DateTime.Now;
                                    }
                                    else
                                    {
                                        startDate_ = DateTime.ParseExact(result_date_start.VAL1, "yyyyMMdd", CultureInfo.InvariantCulture);

                                    }
                                    if (result_date_to.DISPLAY_VAL == "N")
                                    {
                                        endDate_ = DateTime.Now;
                                    }
                                    else
                                    {
                                        endDate_ = DateTime.ParseExact(result_date_to.VAL1, "yyyyMMdd", CultureInfo.InvariantCulture);

                                    }

                                    // Download
                                    var downloadfile = sftpconnect.DownloadFile(row.DISPLAY_VAL, row.VAL1, client, startDate_, endDate_, out message);
                                    downloadfileList.AddRange(downloadfile);
                                    
                                    var downloadfileItemForInterfaceLog = "";
                                    foreach (var downloadResultList in downloadfileList)
                                    {
                                        downloadfileItemForInterfaceLog += "\n" + downloadResultList;
                                        downloadfileItem += downloadResultList + "<br>";
                                    }

                                    log.OUT_RESULT = "SUCCESS";
                                    log.METHOD_NAME = "GetReport3BBPerformance";
                                    log.REQUEST_STATUS = "SUCCESS";
                                    log.INTERFACE_NODE = "3BB / NAS TEMP";
                                    log.IN_TRANSACTION_ID = runningNumber;
                                    log.IN_XML_PARAM = downloadfileItemForInterfaceLog;
                                    log = StartInterfaceLog("", log);

                                    //Upload
                                    var uploadfile = crd.UploadFileToServer(row.DISPLAY_VAL, _userTemp, _pwdTemp, domain_path_nas, startDate_, endDate_, row.VAL2);
                                    uploadfileList.AddRange(uploadfile);


                                    var uploadfileItemForInterfaceLog = "";
                                    foreach (var uploadResultList in uploadfileList)
                                    {
                                        uploadfileItemForInterfaceLog += "\n" + uploadResultList;
                                        uploadfileItem += uploadResultList + "<br>";
                                    }
                                    log.OUT_RESULT = "SUCCESS";
                                    log.REQUEST_STATUS = "SUCCESS";
                                    log.OUT_XML_PARAM = uploadfileItemForInterfaceLog;
                                    EndInterfaceLog("", log);


                                }
                            }
                            

                        }
                        
                       }
                    catch (Exception ex)
                    {
                        //start
                        //end
                        log.METHOD_NAME = "GetReport3BBPerformance";
                        log.INTERFACE_NODE = "3BB";
                        log.IN_XML_PARAM = "";
                        log = StartInterfaceLog("", log);
                        log.OUT_RESULT = "FAILED";
                        log.REQUEST_STATUS = "FAILED";
                        log.OUT_ERROR_RESULT = ex.ToSafeString();
                        log.OUT_XML_PARAM = ex.ToSafeString();
                        EndInterfaceLog("", log);
                        Console.WriteLine(ex);
                    }
                    resultBody = "Download File Result :<br>" + downloadfileItem + "<br>Upload File Result :<br>" + uploadfileItem + "<br>Delete File Result :<br>" + deletefile;


                    ////////////////////////////////
                    ///UPDATE DATE
                    var queryUpdateDate_Start_N = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBPayG_LoadFile_3BBReport",
                        con_name = "DATE_START",
                        display_val = "N",
                        val1 = date_start,
                        val2 = date_to,
                        flag = "EQUIP",
                        updated_by = "FBBPayG_LoadFile_3BBReport",
                    };
                    _intfLogCommand.Handle(queryUpdateDate_Start_N);

                    var queryUpdateDate_To_N = new UpdateFbssFOAConfigTblCommand()
                    {
                        con_type = "FBBPayG_LoadFile_3BBReport",
                        con_name = "DATE_TO ",
                        display_val = "N",
                        val1 = date_start,
                        val2 = date_to,
                        flag = "EQUIP",
                        updated_by = "FBBPayG_LoadFile_3BBReport",
                    };
                    _intfLogCommand.Handle(queryUpdateDate_To_N);


                    // SEND EMAIL
                    var command = new SendMailBatchNotificationCommand
                    {
                        ProcessName = "FBBPayG_LoadFile_3BBReport",
                        Subject = "นำส่งข้อมูลไฟล์ Report 3BB Performance",
                        Body = resultBody,
                    };
                    _sendMail.Handle(command);
                }



            }
            this._logger.Info((object)($"====================FBBPAYG_LoadFile_3BBReport Stopped===================="));
        }

            private List<AuthenticationMethod> SetAuthenticationMethod(string UserName, string KeyFile)
        {
            var privateKey = new PrivateKeyFile(KeyFile);
            List<AuthenticationMethod> _methods = new List<AuthenticationMethod>();
            var keyFiles = new[] { privateKey };
            _methods.Add(new PrivateKeyAuthenticationMethod(UserName, keyFiles));

            return _methods;
        }


        public string GetCurrentPath(List<DirectoryList> data, string path)
        {
            return data.Where(m => m.DIRECTORY_NAME == path).Select(m => m.DIRECTORY_PATH).SingleOrDefault();
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



        static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }


    }
}
