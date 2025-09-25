using QuerySubContractTimeSlot.CompositionRoot;
using QuerySubContractTimeSlot.Security;
using System;
using System.IO;
using System.Linq;
using WBBEntity.Extensions;

namespace QuerySubContractTimeSlot
{
    class Program
    {
        private static string _filePath = "";
        private static string _fileName = "";
        private static string _errorMessage = "";
        private static bool _needRollbackWhenError = false;

        static void Main(string[] args)
        {
            Bootstrapper.Bootstrap();
            var temp = Bootstrapper.GetInstance<QuerySubContractTimeSlotJob>();
            var logger = Bootstrapper.GetInstance<DebugLogger>();

            try
            {
                var ImpersonateVar = temp
                                        .GetLovList("FBB_CONSTANT", "LOAD_SUBCONTRACTOR_TIMESLOT").Where(t => t.Text == "INPUT_PATH")
                                        .Select(t => t).FirstOrDefault();
                var Numrow = temp
                                .GetLovList("FBB_CONSTANT", "LOAD_SUBCONTRACTOR_TIMESLOT").Where(t => t.Text == "NUM_OF_ROW")
                                .Select(t => t.LovValue1).FirstOrDefault();

                if (ImpersonateVar == null || ImpersonateVar.LovValue4.ToSafeString() == "")
                {
                    temp.InsertInterfaceLog("", "Path not found.");
                    temp.Sendmail("Path not found.");
                }
                else
                {
                    var imagepathimer = @ImpersonateVar.LovValue4;
                    var user = ImpersonateVar.LovValue1;
                    var pass = ImpersonateVar.LovValue2;
                    var ip = ImpersonateVar.LovValue3;
                    var zteExportPath = @ImpersonateVar.LovValue5;

                    //var imagepathimer = @"\\10.252.160.97\VirtualNAS\FileSub";
                    //string user = "administrator";
                    //string pass = "P@ssw0rd";
                    //string ip = "10.252.160.97";
                    //var zteExportPath = @"\ExportTimeslot_zte";

                    //var imagepathimer = @"\\10.252.167.22\FBSS_NDEV001B";
                    //string user = @"nas_fbbweb";
                    //string pass = "Fbb1013@Ais";
                    //string ip = "10.252.167.22";
                    //var zteExportPath = @"\ExportTimeslot_zte";

                    //var imagepathimer = @"\\10.235.152.11\fbss_nlog801a";
                    //string user = "nas_fbbweb";
                    //string pass = "Fbb1013@Ais";
                    //string ip = "10.235.152.11";
                    //var zteExportPath = @"\ExportTimeslot_zte";

                    logger.Info("Path " + imagepathimer);
                    logger.Info("ZTE Export Path " + zteExportPath);
                    logger.Info("User " + user);
                    logger.Info("Pass " + pass);
                    logger.Info("Ip " + ip);

                    using (var unc = new UNCAccessWithCredentials())
                    {
                        if (unc.NetUseWithCredentials(imagepathimer, user, ip, pass))
                        {
                            var name = "";

                            if (unc.NetUseGetUserInfo(ref name))
                            {
                                logger.Info("After Impersonated : " + name);
                            }
                            else
                            {
                                logger.Info("Error When Get Impersonated User : " + unc.LastError);
                            }

                            //using (var impersonator = new Impersonator(user, ip, pass, false))
                            //{

                            logger.Info("Accessing Path : " + imagepathimer);

                            _filePath = imagepathimer + zteExportPath;
                            logger.Info("Accessing ZTE Time Slot Path " + _filePath);

                            if (!Directory.Exists(_filePath))
                                temp.InsertInterfaceLog(_filePath, "Path not found.");
                            else
                                logger.Info("Path Exsits");

                            if (!CanRead(_filePath))
                                logger.Info("Read Not Allowed");
                            else
                                logger.Info("Read Are Allowed");

                            var Datenow = DateTime.Now.ToString("yyyyMMdd");
                            logger.Info("Performing Data Date : " + Datenow);

                            var fileList = (from file in Directory.EnumerateFiles(_filePath, "*.dat") where file.Contains(Datenow) select file);
                            logger.Info("File(s) " + string.Join(", ", fileList));

                            var Datenowpack = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            int file_subCount = 0;

                            var files = (from file in fileList
                                         from line in File.ReadLines(file)
                                         select new
                                         {
                                             File = file,
                                             Line = line
                                         }).ToList();

                            logger.Info("Number of Line(s) " + files.Count);

                            var countline = 1;
                            var inputbuffer = "";

                            foreach (var file in files)
                            {
                                _fileName = file.File;
                                var line = file.Line;

                                if (line.Substring(0, 2) == "01")
                                {
                                    if (file_subCount == 0)
                                    {
                                        logger.Info("Inserting " + _fileName + " At 01");
                                        temp.InsertTimeSlot(_filePath, _fileName, line, Datenowpack);
                                    }
                                }
                                else if (line.Substring(0, 2) == "09") /// last line
                                {
                                    logger.Info("Inserting " + _fileName + "At 09");
                                    inputbuffer = inputbuffer + line;
                                    temp.InsertTimeSlot(_filePath, _fileName, inputbuffer, Datenowpack);
                                    inputbuffer = "";
                                }
                                else
                                {
                                    _needRollbackWhenError = true;

                                    if (countline < Convert.ToInt32(Numrow))
                                    {
                                        inputbuffer = inputbuffer + line + ";";
                                        countline++;
                                    }
                                    else
                                    {
                                        logger.Info("Inserting" + _fileName + " End At " + countline);
                                        temp.InsertTimeSlot(_filePath, _fileName, inputbuffer, Datenowpack);
                                        inputbuffer = "";
                                        countline = 1;
                                    }
                                }
                            }

                            if (files.Count <= 0)  //// file not found
                            {
                                temp.InsertInterfaceLog(_filePath, "File not found.");
                                temp.Sendmail("File not found.");
                            }

                            if (unc.NetUseDelete())
                            {
                                logger.Info("Leaving The Imperosnation : Success");
                            }
                            else
                            {
                                logger.Info("Leaving The Imperosnation : Failed With Error => " + unc.LastError);
                                temp.Sendmail("Leaving The Imperosnation : Failed With Error => " + unc.LastError);
                            }
                        }
                        else
                        {
                            logger.Info("Imperosnate Error : " + unc.LastError);
                            temp.Sendmail("Imperosnate Error : " + unc.LastError);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _errorMessage = ex.RenderExceptionMessage();
                temp._logger.Info("Error At QuerySubConTractQueryHandler");
                temp._logger.Info(_errorMessage);

                if (_needRollbackWhenError)
                {
                    temp.RollbackTimeSlot(_filePath, _fileName, "Insert Timslot Error Then Rollback : " + _errorMessage);
                }

                temp.InsertInterfaceLog("", "Error When  QuerySubConTract Handler : " + _errorMessage);
                temp.Sendmail("Error When  QuerySubConTract Handler : " + _errorMessage);
            }
        }

        private static bool CanRead(string path)
        {
            var logger = Bootstrapper.GetInstance<DebugLogger>();
            bool haveAccess = false;

            var di = new DirectoryInfo(path);

            if (di.Exists)
            {
                logger.Info("Directory Info Exists");

                try
                {
                    var acl = di.GetAccessControl();
                    haveAccess = true;

                    logger.Info("Have Access Control");
                }
                catch (UnauthorizedAccessException uae)
                {
                    if (uae.Message.Contains("read-only"))
                    {
                        haveAccess = true;
                        logger.Info("Read Only");
                    }
                }
            }
            else
            {
                logger.Info("Directory Not Exists Or Cannot Access");
            }

            return haveAccess;
        }
    }
}
