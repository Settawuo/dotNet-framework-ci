using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace FBBPayGTransAirnet
{
    using FBBPayGTransAirnet.Model;
    using System;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Text.RegularExpressions;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;


    public class FBBPayGTransAirnetJob
    {
        //public ILogger _logger;
        //private readonly IQueryProcessor _queryProcessor;
        //private readonly ICommandHandler<PAYGTransAirnetCommand> _transAirnet;       
        //private readonly ICommandHandler<SendMailBatchNotificationCommand> _sendMail;
        //private string errorMsg = string.Empty;
        //private Stopwatch _timer;

        private string errorMsg = string.Empty;
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        //public FBBPayGTransAirnetJob(ILogger logger,
        //    IQueryProcessor queryProcessor,
        //    ICommandHandler<PAYGTransAirnetCommand> transAirnet,
        //    ICommandHandler<SendMailBatchNotificationCommand> sendMail)
        //{
        //    _logger = logger;
        //    _transAirnet = transAirnet;
        //    _sendMail = sendMail;
        //    _queryProcessor = queryProcessor;
        //}


        public FBBPayGTransAirnetJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
        }

        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        protected string _KeyNas = ConfigurationManager.AppSettings["KEYNAS"].ToSafeString();
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
        protected string _user3BBSAP;
        protected string _pwd3BBSAP;
        protected string _userTarget;
        protected string _pwdTarget;
        protected string _usernasTarget;
        protected string _pwdnasTarget;
        protected string _usernasArchive;
        protected string _pwdnasArchive;

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        public List<string> SplitDataByLineCount(string data, int maxLines)
        {
            var chunks = new List<string>();
            var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // Support both \r\n and \n delimiters
            int totalLines = lines.Length;

            for (int start = 0; start < totalLines; start += maxLines)
            {
                var chunk = string.Join("\n", lines.Skip(start).Take(maxLines));
                chunks.Add(chunk);
            }

            return chunks;
        }


        public void ExecuteJob()
        {
            StartWatching();

            
            try
            {

                _logger.Info($"Start");
                var ConnectionNas = GetConnectionTransAirnetNasPAYG();
                CredentialHelper crd = new CredentialHelper(_logger);              
                
                var nasSapUser = ConnectionNas.NasSap.Username;
                var nasSapPwd = ConnectionNas.NasSap.Password;
                var nasSapDomain = ConnectionNas.NasSap.Domain;
                var nasSapPath = ConnectionNas.NasSap.Path;

                var nasArchiveUser = ConnectionNas.NasArchive.Username;
                var nasArchivePwd = ConnectionNas.NasArchive.Password;
                var nasArchiveDomain = ConnectionNas.NasArchive.Domain;
                var nasArchivePath = ConnectionNas.NasArchive.Path;

                var nasTargetUser = ConnectionNas.NasTarget.Username;
                var nasTargetPwd = ConnectionNas.NasTarget.Password;
                var nasTargetDomain = ConnectionNas.NasTarget.Domain;
                var nasTargetPath = ConnectionNas.NasTarget.Path;

                _userSAP = EncryptionUtility.Decrypt(nasSapUser, _Key);
                _pwdSAP = EncryptionUtility.Decrypt(nasSapPwd, _Key);
                
                _usernasArchive = EncryptionUtility.Decrypt(nasArchiveUser, _Key);
                _pwdnasArchive = EncryptionUtility.Decrypt(nasArchivePwd, _Key);
               
                _usernasTarget = EncryptionUtility.Decrypt(nasTargetUser, _Key);
                _pwdnasTarget = EncryptionUtility.Decrypt(nasTargetPwd, _Key);
                
                StringBuilder result = crd.ConnectNasSap(
                        _userSAP,
                        _pwdSAP,
                        nasSapDomain,
                        nasSapPath
                    );

                crd.RemoveFile(_usernasArchive, _pwdnasArchive, nasArchiveDomain, nasArchivePath);
              
                if (ConnectionNas.NasSap3BB.Path != null)
                {
                    var nas3BBSapUser = ConnectionNas.NasSap3BB.Username;
                    var nas3BBSapPwd = ConnectionNas.NasSap3BB.Password;
                    var nas3BBSapDomain = ConnectionNas.NasSap3BB.Domain;
                    var nas3BBSapPath = ConnectionNas.NasSap3BB.Path;
                    
                    _user3BBSAP = EncryptionUtility.Decrypt(nas3BBSapUser, _Key);
                    _pwd3BBSAP = EncryptionUtility.Decrypt(nas3BBSapPwd, _Key);

                    StringBuilder result3BB = crd.ConnectNasSap(
                        _user3BBSAP,
                        _pwd3BBSAP,
                        nas3BBSapDomain,
                        nas3BBSapPath
                    );
                }
                _logger.Info($"== Get File from NAS Done ==");

                _logger.Info($"== Check temp path ==");
                string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/"); //ของเดิม

                if (!Directory.Exists(pathNewDirectory))
                {
                    Directory.CreateDirectory(pathNewDirectory);
                }

                List<PAYGTransAirnetFileList> fileList = new List<PAYGTransAirnetFileList>();
                var groupedFileList = new Dictionary<string, List<PAYGTransAirnetFileList>>();//เก็บเป็น Dictionary (ชื่อไฟล์, List ข้อมูลข้างใน) เช่นมี .dat 2 ไฟล์ AAA.dat กับ BBB.dat
                                                                                              // ตัวอย่างจะได้เป็น {[AAA.dat, Count = 3]},{[BBB.dat, Count = 3]}
                if (Directory.Exists(pathNewDirectory))
                {
                    // Get File เฉพาะ .dat
                    var files = Directory.GetFiles(pathNewDirectory, "*.dat");

                    foreach (var file in files)// Loop ทำทีละไฟล์
                    {
                        try
                        {
                            
                             _logger.Info($"== Start Split Chunk ==");

                            string fileName = Path.GetFileNameWithoutExtension(file);// Get ชื่อไฟล์
                            string fileExtension = Path.GetExtension(file);//Get นามสกุลไฟล์ 
                            string fileData = File.ReadAllText(file);//Get ข้อมูลในไฟล์ทั้งหมด


                            if (!string.IsNullOrEmpty(fileData))// เช็คว่าไฟล์ข้างใน มีข้อมูลมั้ย
                            {
                                _logger.Info($"== Check file data ==");
                                //Chunk ก็คือก้อน
                                var fileChunks = SplitDataByLineCount(fileData, 200);// แบ่งบรรทัดข้อมูลเป็น Chunk( List )
                                                                                     // set เป็น 200 บรรทัดต่อ Chunk 13/01/2025 -Pongpiboon Epic
                                int chunkIndex = 1;

                                var fileChunksList = new List<PAYGTransAirnetFileList>();

                                foreach (var chunk in fileChunks)//เอา Chunk ที่ถูกแบ่งทั้งหมดมา Loop เพื่อทำชื่อไฟล์กับข้อมูลข้างใน
                                {
                                    fileChunksList.Add(new PAYGTransAirnetFileList
                                    {
                                        file_name = $"{fileName}_{chunkIndex}{fileExtension}",// ชื่อไฟล์จะถูก set ให้มีเลข Chunk ตามหลัง
                                                                                              // เช่น AAA_1.dat, AAA_2.dat, AAA_3.dat, ... เพื่อที่ตอนยิงเข้า PKG แล้วได้ Return กลับมาจะได้เรียงถูก
                                        file_data = chunk // assign ข้อมูลข้างในให้เป็นของไฟล์นั้นๆ
                                    });
                                    chunkIndex++;
                                }
                                _logger.Info($"== Check file data done ==");
                                groupedFileList[fileName] = fileChunksList;// Add ข้อมูลเป็นของไฟล์นั้นๆ ตามชื่อ เช่น {[AAA.dat, Count = 3]}
                            }
                            else
                            {
                                _logger.Error($"File : {fileName} is Empty");
                                File.Delete(file);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Error reading file {file}: {ex.Message}");
                        }
                    }
                }
                else
                {
                    _logger.Warn("Temp directory does not exist.");
                }

                var skippedFiles = new List<string>();// List File ที่ถูก skip เพราะ pkg ไม่ได้ return มา
                try
                {
                    _logger.Info($"== Start Call PAYGEnhanceTransAirnetQueryHandler ==");

                    var fileDataDictionary = new Dictionary<string, List<(int chunkNumber, string fileData)>>();// เอาไว้เก็บเลข Chunk แล้วก็ข้อมูลข้างในไฟล์
                    var response = new PAYGTransAirnetListResult();

                    foreach (var kvp in groupedFileList)// Loop ส่งเข้า PKG ทีละไฟล์
                    {
                        try
                        {
                            List<PAYGTransAirnetFileList> chunks = kvp.Value; // Get list ข้อมูลของไฟล์นั้นๆ เพื่อที่จะส่งเข้า PKG
                            var query = new PAYGEnhanceTransAirnetQuery
                            {
                                f_enchance_list = chunks
                            };
                            response = _queryProcessor.Execute(query);
                            if (response.Data != null && response?.Data.Count > 0)
                            {
                                foreach (var file in response.Data)//Loop Return ที่ได้จาก PKG
                                {
                                    string fileName2 = file.file_name;    // Get ชื่อไฟล์
                                    string fileData2 = file.file_data;    // Get ข้อมูลข้างใน
                                    if (fileName2.EndsWith(".sync", StringComparison.OrdinalIgnoreCase))
                                        continue;
                                    // ตัวอย่างชื่อไฟล์ "0000_TEST_TST_00000000_000000_1.dat"
                                    var fileNameParts = Path.GetFileNameWithoutExtension(fileName2).Split('_');// แบ่งส่วนของชื่อไฟล์ด้วย _
                                    string baseFileName = string.Join("_", fileNameParts.Take(fileNameParts.Length - 1));  // เก็บชื่อไฟล์ทั้งหมดยกเว้นส่วนสุดท้าย(หลังสุด)
                                    int chunkNumber = int.Parse(fileNameParts.Last());  //เก็บส่วนสุดท้ายของชื่อไฟล์ (หลังสุด เป็นตัวเลขที่เราเอาไว้เรียงลำดับ) 

                                    if (!fileDataDictionary.ContainsKey(baseFileName))// เช็คว่ามี Collection ที่ชื่อไฟล์นั้นๆอยู่หรือเปล่า ถ้าไม่มีจะสร้างใหม่
                                    {
                                        fileDataDictionary[baseFileName] = new List<(int, string)>();// ตัวอย่าง
                                                                                                     // 0000_TEST_TST_00000000_000000.dat,(1, TEST_DATA)
                                    }

                                    fileDataDictionary[baseFileName].Add((chunkNumber, fileData2));// Add เข้า Collection fileDataDictionary โดยที่ Value จะเป็นแบบนี้
                                                                                                   // ตัวอย่าง (1,"TEST_DATA"), (2,"TEST_DATA"),... จะเป็นเลขลำดับ Chunk และก็ข้อมูลข้างใน
                                }

                                foreach (var entry in fileDataDictionary) // Loop Collection fileDataDictionary
                                {
                                    string finalFileName = $"{entry.Key}.dat"; //Key จะเป็นชื่อไฟล์

                                    var sortedChunks = entry.Value.OrderBy(chunk => chunk.chunkNumber).ToList(); // เรียงจาก Chunk (น้อยไปมาก)

                                    StringBuilder combinedFileData = new StringBuilder();
                                    foreach (var chunk in sortedChunks)
                                    {
                                        combinedFileData.Append(chunk.fileData);
                                    }

                                    // แยกเป็นบรรทัด
                                    var combinedLines = combinedFileData.ToString().Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);

                                    // เอา 09| ออกถ้ามีอยู่แล้ว
                                    if (combinedLines.LastOrDefault()?.StartsWith("09|") == true)
                                    {
                                        combinedLines = combinedLines.Take(combinedLines.Length - 1).ToArray();
                                    }

                                    // นับจำนวนบรรทัดที่ขึ้นต้นด้วย 02|
                                    int lineCountStartingWith02 = combinedLines.Count(line => line.StartsWith("02|"));

                                    // รวมใหม่และลบช่องว่างด้านท้าย
                                    combinedFileData.Clear();
                                    combinedFileData.Append(string.Join(Environment.NewLine, combinedLines).TrimEnd());

                                    // เพิ่ม 09| บรรทัดใหม่
                                    combinedFileData.AppendLine(); // ขึ้นบรรทัดใหม่
                                    combinedFileData.AppendLine($"09|{lineCountStartingWith02}");

                                    string outputFilePath2 = Path.Combine(Directory.GetCurrentDirectory(), "Temp/", finalFileName);
                                    File.WriteAllText(outputFilePath2, combinedFileData.ToString());
                                    _logger.Info($"Merge File .dat : {finalFileName} Successfully");

                                    // สร้างไฟล์ .sync
                                    string syncFileName = Path.ChangeExtension(finalFileName, ".sync");
                                    string syncFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Temp/", syncFileName);
                                    string syncFileContent = $"{finalFileName}|{lineCountStartingWith02}";

                                    File.WriteAllText(syncFilePath, syncFileContent);
                                    _logger.Info($"File .sync : {syncFileName} Create Successfully");
                                }

                            }
                            else
                            {
                                string fileName = kvp.Key; // Get ชื่อไฟล์ที่ ไม่ได้ข้อมูลจาก PKG
                                _logger.Info($"No Data Return from PKG for File: {fileName}");
                                skippedFiles.Add(fileName); // เพิ่มชื่อไฟล์ที่ไม่ได้ข้อมูลลงไปใน List
                            }
                                


                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Error Group File Process {kvp.Key}: {ex.Message}");
                        }
                    }
                    _logger.Info("== Merge File Process Complete ==");

                    try
                    {
                        _logger.Info($"== Start place file to target nas path ==");
                        var nasTarget = crd.placeFileTargetNas(
                                _usernasTarget,
                                _pwdnasTarget,
                                nasTargetDomain,
                                nasTargetPath,
                                skippedFiles
                        );
                        _logger.Info($"== End place file to target nas path ==");

                        _logger.Info($"== Start place file to archive nas path ==");
                        var nasArchive = crd.placeFileTargetNas(
                                _usernasArchive,
                                _pwdnasArchive,
                                nasArchiveDomain,
                                nasArchivePath,
                                skippedFiles
                        );
                        _logger.Info($"== End place file to archive nas path ==");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error In Nas Connection part {ex.Message}");
                    }

                }
                catch (Exception ex)
                {
                    _logger.Error($"Error call PAYGEnhanceTransAirnetQuery: {ex.Message}");
                }

                _logger.Info($"== Start Delete file in folder temp ==");
                var deleteTemp = crd.deleteFileTemp();
                _logger.Info($"== End Delete file in folder temp ==");


                _logger.Info($"END");
            }
            catch (Exception ex)
            {
                SendSms();
                StopWatching("Fail!");
                _logger.Error(ex.Message);
            }
        }

        private StringBuilder SBlist(StringBuilder sb, string data, string split, bool status)
        {
            if (status)
            {
                sb.Append(split + data);
                return sb;
            }
            sb.Append(data);
            return sb;
        }

        private ConnectionNasPAYGListResult GetConnectionNasPAYG()
        {
            var query = new GetConnectionNasPAYGQuery();
            var ss = _queryProcessor.Execute(query);
            return ss;
        }

        private ConnectionNasPAYGTransAirnetListResult GetConnectionTransAirnetNasPAYG()
        {
            var query = new GetConnectionTransAirnetNasPAYGQuery();
            var ss = _queryProcessor.Execute(query);

            _logger.Info($"== Get connection Nas Done ==");
            return ss;
        }

        private List<DirectoryList> GetDirectory()
        {
            var query = new GetDirectoryQuery();
            return _queryProcessor.Execute(query);
        }

        private PAYGTransAirnetListResult QueryBuild(string data)
        {
            var query = new PAYGTransAirnetQuery();
            query.f_list = data.Equals("") ? "0" : data;
            errorMsg = query.Return_Desc;
            return _queryProcessor.Execute(query);
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
        public void SendSms()
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
                            command.FullUrl = "FBBPayGTransAirnet";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBPayGTransAirnet Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }

        //public void ExecuteJob()
        //{
        //    try
        //    {
        //        _logger.Info("Start PAYGTransAirnet.");
        //        StartWatching();
        //        var command = new PAYGTransAirnetCommand()
        //        {

        //        };

        //        _transAirnet.Handle(command);

        //        _logger.Info(string.Format("PAYGTransAirnet : {0}", command.Return_Message));

        //        StopWatching("PAYGTransAirnet");

        //        if (command.Return_Code.ToSafeDecimal() != 0)
        //        {
        //            if (SendEmailError)
        //                SendEmail(command.Return_Message);
        //            else
        //                _logger.Info("PAYGTransAirnet : Error without Sending an Email.");
        //        }

        //        _logger.Info("End PAYGTransAirnet.");
        //    }
        //    catch (Exception ex)
        //    {
        //        StopWatching("PAYGTransAirnet");
        //        _logger.Info("PAYGTransAirnet" + string.Format(" is error on execute : {0}.",
        //            ex.GetErrorMessage()));
        //        _logger.Info(ex.RenderExceptionMessage());
        //        StopWatching("PAYGTransAirnet");

        //        SendEmail(ex.GetErrorMessage());

        //        _logger.Info("End PAYGTransAirnet.");
        //    }
        //}

        //public static string ModeSendEmail
        //{
        //    get { return string.Format("[{0}-Batch] FBBPAYGTransAirnet", ConfigurationManager.AppSettings["ModeSendEmail"].ToSafeString()); }
        //}

        //public static bool SendEmailError
        //{
        //    get
        //    {
        //        var result = false;
        //        bool.TryParse(ConfigurationManager.AppSettings["SendEmailError"], out result);
        //        return result;
        //    }
        //}

        //public void SendEmail(string errorString)
        //{
        //    _logger.Info("Sending an Email.");
        //    StartWatching();

        //    string FromPassword = "";
        //    string Port = "";
        //    string Domaim = "";
        //    string IPMailServer = "";
        //    var vMailServer = GetLovList("FBBDORM_CONSTANT", "").Where(p => p.Name.Contains("VAR") && p.LovValue5 == "FBBDORM010").ToList();
        //    if (vMailServer != null && vMailServer.Count > 0)
        //    {
        //        foreach (var key in vMailServer)
        //        {
        //            switch (key.Name)
        //            {
        //                case "VAR_FROM_PASSWORD":
        //                    FromPassword = key.LovValue1;
        //                    break;
        //                case "VAR_HOST":
        //                    IPMailServer = key.LovValue1;
        //                    break;
        //                case "VAR_PORT":
        //                    Port = key.LovValue1;
        //                    break;
        //                case "VAR_DOMAIN":
        //                    Domaim = key.LovValue1;
        //                    break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        FromPassword = "V9!@M#V2zf@Q";
        //        IPMailServer = "10.252.160.41";
        //        Port = "25";
        //        Domaim = "corp.ais900dev.org";
        //    }

        //    string strFrom = "";
        //    string strTo = "";
        //    string strCC = "";
        //    string strBCC = "";

        //    string strSubject = "";
        //    string strBody = "";
        //    var MailDetail = GetLovList("FBB_CONSTANT", "SEND_EMAIL_ERROR_CASE").Where(p => p.Text == "EMAIL_ALERT").FirstOrDefault();
        //    if (MailDetail == null || MailDetail.LovValue1.ToSafeString() == "" || MailDetail.LovValue2.ToSafeString() == "")
        //    {
        //        strSubject = string.Format("{0} is Failed", ModeSendEmail);
        //        strBody = "Please verify this error : " + errorString;
        //    }
        //    else
        //    {
        //        strSubject = string.Format(MailDetail.LovValue1, ModeSendEmail);
        //        strBody = MailDetail.LovValue2 + " " + errorString;
        //    }

        //    try
        //    {
        //        var command = new SendMailBatchNotificationCommand
        //        {
        //            ProcessName = "SEND_EMAIL_ERROR_CASE",
        //            CreateUser = "Batch",
        //            SendTo = strTo,
        //            SendCC = strCC,
        //            SendBCC = strBCC,
        //            SendFrom = strFrom,
        //            Subject = strSubject,
        //            Body = strBody,
        //            FromPassword = FromPassword,
        //            Port = Port,
        //            Domaim = Domaim,
        //            IPMailServer = IPMailServer
        //        };

        //        _sendMail.Handle(command);

        //        _logger.Info(string.Format("Sending an Email : {0}.", command.ReturnMessage));
        //        StopWatching("Send an Email");

        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info("Sending an Email" + string.Format(" is error on execute : {0}.",
        //           ex.GetErrorMessage()));
        //        _logger.Info(ex.GetErrorMessage());

        //        StopWatching("Send an Email");
        //    }            
        //}

        //public List<LovValueModel> GetLovList(string type, string name = "")
        //{
        //    try
        //    {
        //        var query = new GetLovQuery
        //        {
        //            LovType = type,
        //            LovName = name
        //        };

        //        var lov = _queryProcessor.Execute(query);
        //        return lov;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info(string.Format("Error GetLovList [Type({0}), Name({1})]: {2}", type, name, ex.GetErrorMessage()));
        //        return new List<LovValueModel>();
        //    }
        //}

    }
}
