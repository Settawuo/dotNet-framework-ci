using System;
using System.Collections.Generic;
using System.Linq;

namespace PAYGLoadFileToTable
{
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using WBBBusinessLayer;
    using WBBBusinessLayer.CommandHandlers;
    using WBBContract;
    using WBBContract.Commands;
    using WBBContract.Queries.Commons.Masters;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
    using WBBData.Repository;
    using WBBEntity.Extensions;
    using WBBEntity.Models;
    using WBBEntity.PanelModels;
    using WBBEntity.PanelModels.FBBWebConfigModels;



    public class PAYGLoadFileToTableJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IEntityRepository<FBB_CFG_LOV> _cfgLov;
        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        public PAYGLoadFileToTableJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            this._logger = logger;
            _queryProcessor = queryProcessor;
            _sendSmsCommand = SendSmsCommand;
        }

        public void Execute()
        {
            _logger.Info("PAYG start load file to table.");
            StartWatching();
            try
            {


                string usernameNas = "";
                string PassNas = "";

                var lovsourceNASReadfile = GetLovList("Config_OM10", "Config_OM10_READFILE");
                usernameNas = EncryptionUtility.Decrypt(lovsourceNASReadfile[0].LovValue1, _Key);
                PassNas = EncryptionUtility.Decrypt(lovsourceNASReadfile[0].LovValue2, _Key);
                string DomainNas = lovsourceNASReadfile[0].LovValue3;
                string sourceNas = lovsourceNASReadfile[0].LovValue4;

                _logger.Info($"== Start Call PAYGLoadFileToTableEnhanceQueryHandler - get file ==");
                var query = new PAYGLoadFileToTableEnhanceQuery
                {
                    flag_check = "getfilename"
                };
                var response = _queryProcessor.Execute(query);
                _logger.Info($"== End Call PAYGLoadFileToTableEnhanceQueryHandler - get file ==");

                List<string> namePartsList = new List<string>();
                namePartsList = response.Data.Select(a => a.file_name).ToList();


                CredentialHelper crd = new CredentialHelper(_logger);

                string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");
                if (!Directory.Exists(pathNewDirectory))
                {
                    Directory.CreateDirectory(pathNewDirectory);
                }

                _logger.Info($"== Start Call Credential GetFileSAP - Old file And Current file ==");
                StringBuilder result = crd.GetFileSAP(
                    usernameNas,
                    PassNas,
                    DomainNas,
                    sourceNas,
                    namePartsList
                );
                _logger.Info($"== End Call Credential GetFileSAP - Old file And Current file ==");



                List<PAYGLoadFileToTableEnhanceFileList> fileList = new List<PAYGLoadFileToTableEnhanceFileList>();
                var groupedFileList = new Dictionary<string, List<PAYGLoadFileToTableEnhanceFileList>>();//เก็บเป็น Dictionary (ชื่อไฟล์, List ข้อมูลข้างใน) เช่นมี .dat 2 ไฟล์ AAA.dat กับ BBB.dat
                                                                                              // ตัวอย่างจะได้เป็น {[AAA.dat, Count = 3]},{[BBB.dat, Count = 3]}
                if (Directory.Exists(pathNewDirectory))
                {
                    var dateNowFilter = DateTime.Now.ToString("yyyyMMdd");
                    var dateNowName = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    // Get File เฉพาะ .dat
                    var files = Directory.GetFiles(pathNewDirectory, "*.dat").Concat(Directory.GetFiles(pathNewDirectory, "*.sync")).ToArray();
                    //if not found data = ''
                    _logger.Info($"== Start Get filesNotExit ==");
                    var filesNotExits = namePartsList.Select(a =>
                    {
                        var match = Regex.Match(a, @"_(\d{8}_\d{6})");
                        var extractedDate = match.Success ? match.Groups[1].Value : string.Empty;

                        return new
                        {
                            todayFile = string.IsNullOrEmpty(extractedDate),
                            name = a.Replace($".dat", "").Replace($".sync", ""),
                            fileExtension = Path.GetExtension(a),
                            date = extractedDate,
                            fullName = a
                        };
                    })
                    .Where(a => !files.Select(f => Path.GetFileName(f)).Any(b => 
                                   b.EndsWith(a.fileExtension) //check .dat / .sync
                                && b.StartsWith(
                                    a.todayFile
                                        ? $"{a.name}_{dateNowFilter}"
                                        : $"{a.name}") //check file name is today file
                    ))
                    .Select(a => a.todayFile ? $"{a.name}_{dateNowName}{a.fileExtension}" : a.fullName)
                    .ToList();

                    foreach (var fileNotExit in filesNotExits)
                    {

                        var fileChunksList = new List<PAYGLoadFileToTableEnhanceFileList>();
                        fileChunksList.Add(new PAYGLoadFileToTableEnhanceFileList
                        {
                            file_name = fileNotExit,
                            file_data = null,
                            file_index = "1"
                        });
                        groupedFileList[fileNotExit] = fileChunksList;
                        _logger.Info($" added file not exit : {fileNotExit}");
                    }
                    _logger.Info($"== End Get filesNotExit ==");

                    //if have file
                    foreach (var file in files)// Loop ทำทีละไฟล์
                    {
                        try
                        {

                            string fileName = Path.GetFileNameWithoutExtension(file);// Get ชื่อไฟล์
                            string fileExtension = Path.GetExtension(file);//Get นามสกุลไฟล์ 
                            string fileData = File.ReadAllText(file);//Get ข้อมูลในไฟล์ทั้งหมด


                            if (!string.IsNullOrEmpty(fileData))// เช็คว่าไฟล์ข้างใน มีข้อมูลมั้ย
                            {
                                //Chunk ก็คือก้อน
                                var fileChunks = SplitDataByLineCount(fileData, 100);// แบ่งบรรทัดข้อมูลเป็น Chunk( List )
                                                                                     // set เป็น 200 บรรทัดต่อ Chunk 13/01/2025 -Pongpiboon Epic
                                int chunkIndex = 1;

                                var fileChunksList = new List<PAYGLoadFileToTableEnhanceFileList>();

                                foreach (var chunk in fileChunks)//เอา Chunk ที่ถูกแบ่งทั้งหมดมา Loop เพื่อทำชื่อไฟล์กับข้อมูลข้างใน
                                {
                                    fileChunksList.Add(new PAYGLoadFileToTableEnhanceFileList
                                    {
                                        file_name = $"{fileName}{fileExtension}",
                                        file_data = chunk,
                                        file_index = chunkIndex.ToSafeString()
                                    });
                                    chunkIndex++;
                                }
                                groupedFileList[fileName + fileExtension] = fileChunksList;// Add ข้อมูลเป็นของไฟล์นั้นๆ ตามชื่อ เช่น {[AAA.dat, Count = 3]}
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

                string serializedFileList = Newtonsoft.Json.JsonConvert.SerializeObject(fileList);
                var skippedFiles = new List<string>();// List File ที่ถูก skip เพราะ pkg ไม่ได้ return มา

                _logger.Info($"== Start Call PAYGLoadFileToTableEnhanceQueryHandler - insert/update table ==");
                try
                {

                    var fileDataDictionary = new Dictionary<string, List<(int chunkNumber, string fileData)>>();// เอาไว้เก็บเลข Chunk แล้วก็ข้อมูลข้างในไฟล์
                    //var response2 = new PAYGLoadFileToTableEnhanceListResult();

                    foreach (var kvp in groupedFileList)// Loop ส่งเข้า PKG ทีละไฟล์
                    {
                        try
                        {
                            List<PAYGLoadFileToTableEnhanceFileList> chunks = kvp.Value; // Get list ข้อมูลของไฟล์นั้นๆ เพื่อที่จะส่งเข้า PKG

                            var query2 = new PAYGLoadFileToTableEnhanceQuery
                            {
                                flag_check = "",
                                loadfile_enhance_list = chunks
                            };
                            var response2 = _queryProcessor.Execute(query2);
                            _logger.Error($"File action insert/update : {kvp.Key} ");

                            if (response2 != null)
                            {
                                switch (response2.Return_Code)
                                {
                                    case 1:
                                        _logger.Error("PAYG load file to table : Fail.");
                                        break;
                                    case 2:
                                        _logger.Error("PAYG load file to table : Pass with Error.");
                                        break;
                                    case 3:
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
                            _logger.Error($"File : {kvp.Key} is Error");
                        }
                    }

                    _logger.Info($"== End Call PAYGLoadFileToTableEnhanceQueryHandler - insert/update table ==");
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error insertTable file: {ex.Message}");
                }
                _logger.Info($"== End Call PAYGLoadFileToTableEnhanceQueryHandler - insert/update table ==");

                _logger.Info($"== Start Delete file in folder temp ==");
                var deleteTemp = crd.deleteFileTemp();
                _logger.Info($"== End Delete file in folder temp ==");


                _logger.Info($"END");

                //var data = QueryBuild();

                //if (data != null && data.Any())
                //{
                //    switch (data.FirstOrDefault())
                //    {
                //        case "1":
                //            _logger.Error("PAYG load file to table : Fail.");
                //            break;
                //        case "2":
                //            _logger.Error("PAYG load file to table : Pass with Error.");
                //            break;
                //        case "3":
                //            _logger.Error("Case not have some file and load file failed");
                //            break;
                //        default:
                //            _logger.Info("PAYG load file to table : Success.");
                //            break;
                //    }
                //}
                //else
                //{
                //    _logger.Error("PAYG load file to table : The process in Packages have a problem, please check.");
                //    _logger.Error(string.Format("PAYG load file to table : {0}", errorMsg.ToSafeString()));
                //}
            }
            catch (Exception ex)
            {
                _logger.Error("PAYG load file to table :" + string.Format(" is error on execute : {0}.",
                                 ex.GetErrorMessage()));
                _logger.Error(ex.RenderExceptionMessage());
                SendSms();
            }
            finally
            {
                StopWatching();
            }
        }

        private List<string> QueryBuild()
        {
            var query = new PAYGLoadFilToTableQuery();
            errorMsg = query.ErrorMessage;
            return _queryProcessor.Execute(query);
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
                            command.FullUrl = "PAYGLoadFileToTable";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "PAYGLoadFileToTable Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }

        public FileInfo[] GetFileslist(string sourcenaspath, string daygetfile)
        {
            FileInfo[] listfile = null;
            DateTime dategetfile = DateTime.Now.AddDays(-int.Parse(daygetfile)).Date;
            bool Foundfile = false;
            DirectoryInfo di;

            try
            {
                di = new DirectoryInfo(sourcenaspath);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //var cfgConfigQuery = _cfgLov.Get(x => x.LOV_TYPE == "FBB_TABLE" && x.ACTIVEFLAG == "N").ToList();
            var cfgConfigQuery = GetLovList("FBB_TABLE", "", true)
                .Where(a => a.ActiveFlag == "N")
                .ToList();

            foreach (var cfg in cfgConfigQuery)
            {
                string[] parts = cfg.Name.Split('_');
                string namePart = string.Join("_", parts, 0, parts.Length - 2);

                if (namePart != "FBSS_FIXED_OM010_RPT")
                {
                    var listsync = di.GetFiles(".dat")
                                    .Concat(di.GetFiles(".sync"))
                                    .Where(r => r.FullName.Contains(namePart))
                                    .ToList();
                    if (listsync.Count != 0)
                    {
                        Foundfile = true;
                    }
                }
            }

            //for (int i = 0; i < 5; i++)
            //{
            //    try
            //    {
            //        var listsync = di.GetFiles("*.sync").Where(r => r.FullName.Contains("FBSS_REPORT_OM")).ToList();
            //        if (listsync.Count != 0)
            //        {
            //            Foundfile = true;
            //            FileInfo[] file = di.GetFiles().Where(r => r.FullName.Contains("FBSS_REPORT_OM") && listsync.Select(x => Path.GetFileNameWithoutExtension(x.FullName)).Contains(Path.GetFileNameWithoutExtension(r.FullName))).ToArray();
            //            listfile = (from t in file
            //                        select new
            //                        {
            //                            date = GetDate(t.Name.Split('_')),
            //                            file = t
            //                        }).Where(x => x.date >= dategetfile).Select(y => y.file).ToArray();
            //            break;
            //        }
            //    }
            //    catch (Exception ex)
            //    {

            //    }

            //}
            if (!Foundfile)
            {
                throw new Exception(string.Format("Not Found File in Path : {0}", sourcenaspath));
            }

            return listfile;
        }

        public List<LovValueModel> GetLovList(string type, string name = "", bool flag = false)
        {
            try
            {
                var query = new GetLovQuery
                {
                    LovType = type,
                    LovName = name,
                    IgonreFlag = flag
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


        public List<string> SplitDataByLineCount(string data, int maxLines)
        {
            var chunks = new List<string>();
            var lines = data.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // Support both \r\n and \n delimiters
            int totalLines = lines.Length;

            for (int start = 0; start < totalLines; start += maxLines)
            {
                var chunk = string.Join("\n", lines.Skip(start).Take(maxLines));
                if (start < totalLines)
                {
                    chunk = chunk + "\n";
                }
                chunks.Add(chunk);
            }

            return chunks;
        }
    }
}
