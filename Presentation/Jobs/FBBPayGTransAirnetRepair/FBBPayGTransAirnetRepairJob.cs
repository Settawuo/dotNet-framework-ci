using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBBPayGTransAirnetRepair
{
    using FBBPayGTransAirnetRepair.Model;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics;
    using WBBBusinessLayer;
    using WBBContract;
    using WBBContract.Queries.FBBWebConfigQueries;
    using WBBEntity.Extensions;
    using WBBEntity.PanelModels.FBBWebConfigModels;

    public class FBBPayGTransAirnetRepairJob
    {
        private string errorMsg = string.Empty;
        string txtProcess = "process ->";

        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private Stopwatch _timer;

        public FBBPayGTransAirnetRepairJob(
            ILogger logger,
            IQueryProcessor queryProcessor)
        {
            _logger = logger;
            _queryProcessor = queryProcessor;

        }
        protected string _archivePath = ConfigurationManager.AppSettings["archive"].ToSafeString();
        protected string _pathTemp = ConfigurationManager.AppSettings["TEMPPATH"].ToSafeString();
        protected string _dominTemp = ConfigurationManager.AppSettings["TEMP_DOMAIN"].ToSafeString();
        protected string _userTemp = ConfigurationManager.AppSettings["TEMP_USER"].ToSafeString();
        protected string _pwdTemp = ConfigurationManager.AppSettings["TEMP_PWD"].ToSafeString();

        protected string _pathSAP = ConfigurationManager.AppSettings["SAP"].ToSafeString();
        protected string _dominSAP = ConfigurationManager.AppSettings["SAP_DOMAIN"].ToSafeString();
        protected string _userSAP = ConfigurationManager.AppSettings["SAP_USER"].ToSafeString();
        protected string _pwdSAP = ConfigurationManager.AppSettings["SAP_PWD"].ToSafeString();

        protected string _pathTarget = ConfigurationManager.AppSettings["TARGET"].ToSafeString();
        protected string _dominTarget = ConfigurationManager.AppSettings["TARGET_DOMAIN"].ToSafeString();
        protected string _userTarget = ConfigurationManager.AppSettings["TARGET_USER"].ToSafeString();
        protected string _pwdTarget = ConfigurationManager.AppSettings["TARGET_PWD"].ToSafeString();


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

            //StartWatching();
            Console.WriteLine("=====================================================================");
            Console.WriteLine("");
            Console.WriteLine("    Fibre Broadband - Pay as you go transfer airnet repair system    ");
            Console.WriteLine("");
            Console.WriteLine("=====================================================================");


            DateTime dateInput;
            string done = "N";
            do
            {
                //WriteOnBottomLine(" * Input syntax for type date is dd/MM/yyyy : ex. 01/01/1991");
                Console.WriteLine("");
                Console.WriteLine("Syntax for type date is dd/MM/yyyy : ex. 01/01/1991");
                Console.Write("Enter date : ");
                var userInput = Console.ReadLine();

                DateTime.TryParse(userInput, out dateInput);
                if (!dateInput.Equals(DateTime.MinValue))
                {
                    // Continue to look it up in the DB
                    //WriteOnBottomLine("");
                    Console.Write("Are you sure you want to continue? (y/n) : ");
                    string con = Console.ReadLine();


                    if (con.ToUpper().Equals("Y"))
                    {
                        DateTime currDateTime = dateInput;
                        string dateNow = currDateTime.ToString("yyyyMMdd");
                        string rootFileName = string.Format("{0}_{1}", "AIRNET_INV", dateNow);
                        string fileName = rootFileName;

                        CredentialHelper crd = new CredentialHelper(_logger);

                        //var allDirectoryPath = GetDirectory();

                        StringBuilder result = crd.GetFileSAP(
                                _userSAP,
                                _pwdSAP,
                                _dominSAP,
                                _pathSAP,
                                _archivePath,
                                fileName
                            );

                        bool fileData = true;
                        if (result.ToSafeString() == "")
                        {
                            //StopWatching("Fail!");
                            Console.WriteLine("{0} file is empty", txtProcess);

                            _logger.Info("== Fail == : file is empty");
                            fileData = false;
                        }

                        if (fileData)
                        {
                            Console.WriteLine("{0} {1}", txtProcess, result.ToString());
                            _logger.Info(string.Format("file data : {0}", result.ToString()));

                            bool copy = crd.CopyFile(
                                _userTemp,
                                _pwdTemp,
                                _dominTemp,
                                _archivePath,
                                _pathTemp
                            );


                            Console.WriteLine("{0} sap path {1}", txtProcess, string.Join(",", DesModel.copyList.ToArray()));
                            _logger.Info(string.Format("sap path : {0}", string.Join(",", DesModel.copyList.ToArray())));

                            var SAP_data = QueryBuild(result.ToString());
                            Console.WriteLine("{0} package code {1} - {2}", txtProcess, SAP_data.Return_Code, SAP_data.Return_Desc);
                            _logger.Info(string.Format("{0} package : {1} - {2}", txtProcess, SAP_data.Return_Code, SAP_data.Return_Desc));
                            if (SAP_data != null && SAP_data.Data.Any())
                            {
                                bool writeResult = crd.WriteFile(
                                    _userTarget,
                                    _pwdTarget,
                                    _dominTarget,
                                    _pathTarget,
                                    fileName,
                                    SAP_data.Data
                                );
                                _logger.Info(string.Format("destination is {0}", writeResult));

                                //StopWatching("Close!");
                                if (writeResult)
                                {
                                    Console.WriteLine("{0} success!", txtProcess);
                                    _logger.Info("== End ==");
                                }
                                else
                                {
                                    Console.WriteLine("{0} fail!", txtProcess);
                                    _logger.Info("== Fail == : can't write file.");
                                }

                                // Delete Temporary file
                                foreach (var arr in DesModel.copyList)
                                {
                                    bool deleteFile = crd.RemoveFile(
                                        _userTemp,
                                        _pwdTemp,
                                        _dominTemp,
                                        arr
                                    );
                                    //File.Delete(arr);
                                }
                                _logger.Info("remove temp file.");

                            }
                            else
                            {
                                Console.WriteLine("{0} data is null!", txtProcess);
                                _logger.Info("== Fail == : data is null");
                            }

                        }

                        //StopWatching("Fail!");

                    }
                    //else if (done.ToUpper().Equals("N"))
                    //{
                    //    Console.WriteLine("No");
                    //}

                }
                else Console.WriteLine("invalid format date!");


            } while (done.ToUpper().Equals("N"));





            //DateTime currDateTime = DateTime.Now;
            //string dateNow = currDateTime.ToString("yyyyMMdd");
            //string rootFileName = string.Format("{0}_{1}", "AIRNET_INV", dateNow);
            //string fileName = rootFileName;

            //CredentialHelper crd = new CredentialHelper(_logger);

            ////var allDirectoryPath = GetDirectory();

            //StringBuilder result = crd.GetFileSAP(
            //        _userSAP,
            //        _pwdSAP,
            //        _dominSAP,
            //        _pathSAP,
            //        _archivePath
            //    );

            //if (result.ToSafeString() == "")
            //{
            //    StopWatching("Fail!");
            //    _logger.Info("== Fail == : file is empty");
            //    return;
            //}

            //_logger.Info(string.Format("File Data : {0}", result.ToString()));

            //bool copy = crd.CopyFile(
            //    _userTemp,
            //    _pwdTemp,
            //    _dominTemp,
            //    _archivePath,
            //    _pathTemp
            //);

            //_logger.Info(string.Format("File copy : {0}", copy));
            //_logger.Info(string.Format("SAP Path : {0}", string.Join(",", DesModel.copyList.ToArray())));

            //var SAP_data = QueryBuild(result.ToString());
            //_logger.Info(string.Format("sql get data : {0} - {1}", SAP_data.Return_Code, SAP_data.Return_Desc));
            //if (SAP_data != null && SAP_data.Data.Any())
            //{
            //    // Delete Temporary file
            //    foreach (var arr in DesModel.copyList)
            //    {
            //        File.Delete(arr);
            //    }

            //    //string curTarget = GetCurrentPath(allDirectoryPath, "FBB_AIR") ?? _targetPath;

            //    bool writeResult = crd.WriteFile(
            //        _userTarget,
            //        _pwdTarget,
            //        _dominTarget,
            //        _pathTarget,
            //        fileName,
            //        SAP_data.Data
            //    );

            //    StopWatching("Close!");
            //    _logger.Info("== End ==");
            //    return;
            //}
            //StopWatching("Fail!");
            //_logger.Info("== Fail == : Data is null");

        }

        static void WriteOnBottomLine(string text)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            Console.CursorTop = Console.WindowTop + Console.WindowHeight - 1;
            Console.Write(new string(' ', Console.WindowWidth));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(text);
            // Restore previous position
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.SetCursorPosition(x, y);
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

    }
}
