using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WBBBusinessLayer;
using WBBBusinessLayer.CommandHandlers;
using WBBContract;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;

namespace FBBPAYG_Load_File_OM010Batch
{
    class FBBPAYG_Load_File_OM010BatchJob
    {
        public ILogger _logger;
        private readonly IQueryProcessor _queryProcessor;
        private string errorMsg = string.Empty;
        private Stopwatch _timer;
        private readonly ICommandHandler<FBBLastMileByDistanceJobCommand> _commandLastmile;
        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();

        public FBBPAYG_Load_File_OM010BatchJob(
                ILogger logger,
                IQueryProcessor queryProcessor,
                ICommandHandler<FBBLastMileByDistanceJobCommand> commandLastmile
               )
        {
            _logger = logger;
            _queryProcessor = queryProcessor;
            _commandLastmile = commandLastmile;
        }

        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching()
        {
            _timer.Stop();
            _logger.Info("FBBPAYG_Load_File_OM010Batch Process time : " + _timer.Elapsed);
        }

        public void Execute()
        {
            _logger.Info("FBBPAYG_Load_File_OM010Batch start.");
            StartWatching();
            try
            {
                var lovsourceNASReadfile = GetLovList("Config_OM10", "Config_OM10_READFILE");
                var lovDesMovefile = GetLovList("Config_OM10", "Config_OM10_MOVEFILE");
                var lovRollback = GetLovList("Decrypt_BatchOM010", "ROLLBACK");

                string usernamenas = "";
                string Passnas = "";//Fixed Code scan : string passwordnas = "";
                if (lovRollback != null && lovRollback.Count() > 0)
                {
                    usernamenas = lovsourceNASReadfile[0].LovValue1;
                    Passnas = lovsourceNASReadfile[0].LovValue2;
                }
                else
                {
                    usernamenas = EncryptionUtility.Decrypt(lovsourceNASReadfile[0].LovValue1, _Key);
                    Passnas = EncryptionUtility.Decrypt(lovsourceNASReadfile[0].LovValue2, _Key);
                }
                //cfg.Host = "10.252.160.101";
                //cfg.Port = 22;
                //cfg.UserName = "leardnk8";
                //cfg.KeyFile = "leardnk";
                string ipAddressnas = lovsourceNASReadfile[0].LovValue3;
                string sourceNas = lovsourceNASReadfile[0].LovValue4;
                string dategetfile = lovsourceNASReadfile[0].Text;
                string desmovesucess = lovDesMovefile[0].LovValue1;
                string desmoveerror = lovDesMovefile[0].LovValue2;

                using (var impersonator = new Impersonator(usernamenas, ipAddressnas, Passnas, false))
                {
                    _logger.Info("Star :GetFileNas");
                    _logger.Info("Authen : " + ipAddressnas);
                    _logger.Info("sourceNas : " + sourceNas);

                    var listfile = GetFileslist(sourceNas, dategetfile);
                    int countfile = listfile.Where(x => x.Extension == ".dat").ToList().Count;
                    _logger.Info("Total file : " + countfile);

                    foreach (var file in from t in listfile
                                         where t.Extension == ".dat"
                                         orderby t.Name
                                         select t)
                    {
                        try
                        {
                            _logger.Info("Star :ReadFile:" + file.Name);
                            var fileInfos = new List<FileInfo>();
                            var listdata = ReadDatatoFile(file);
                            _logger.Info("Readdata:" + file.Name + " Success");
                            _logger.Info("Total Data : " + listdata.Count.ToString());

                            var command = new FBBLastMileByDistanceJobCommand()
                            {
                                P_LIST_FIXED_OM0101 = listdata
                            };

                            _commandLastmile.Handle(command);
                            //var statusupdate = (command.ret_code == "0") ? true : false;
                            var ret_message = command.ret_msg;
                            _logger.Info("retcode :" + command.ret_code.ToString());
                            _logger.Info("Message : " + ret_message);

                            #region movefile
                            //fileInfos.Add(file);
                            //fileInfos.Add(listfile.First(r => r.Name == (file.Name.Replace(".dat", ".sync"))));

                            //if (statusupdate)
                            //{

                            //    var statusmovefile = Movefile(fileInfos, desmovesucess);
                            //    _logger.Info("MovefileSucces :" + statusmovefile.ToSafeString());
                            //}
                            //else
                            //{

                            //    var statusmovefile = Movefile(fileInfos, desmoveerror);
                            //    _logger.Info("MovefileErroe : " + statusmovefile.ToSafeString());
                            //}
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            _logger.Info(string.Format("Error Exception :{0} to File :{1}", ex.GetErrorMessage(), file.Name));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Info("FBBPAYG_Load_File_OM010Batch :" + string.Format(" is error : {0}.",
                ex.GetErrorMessage()));
                _logger.Info(ex.RenderExceptionMessage());
            }
            finally
            {
                StopWatching();
                _logger.Info("FBBPAYG_Load_File_OM010Batch end.");
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

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var listsync = di.GetFiles("*.sync").Where(r => r.FullName.Contains("FBSS_REPORT_OM")).ToList();
                    if (listsync.Count != 0)
                    {
                        Foundfile = true;
                        FileInfo[] file = di.GetFiles().Where(r => r.FullName.Contains("FBSS_REPORT_OM") && listsync.Select(x => Path.GetFileNameWithoutExtension(x.FullName)).Contains(Path.GetFileNameWithoutExtension(r.FullName))).ToArray();
                        listfile = (from t in file
                                    select new
                                    {
                                        date = GetDate(t.Name.Split('_')),
                                        file = t
                                    }).Where(x => x.date >= dategetfile).Select(y => y.file).ToArray();
                        break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            if (!Foundfile)
            {
                throw new Exception(string.Format("Not Found File in Path : {0}", sourcenaspath));
            }

            return listfile;
        }

        public bool Movefile(List<FileInfo> files, string desmove)
        {
            bool status = false;
            try
            {
                foreach (var file in files)
                {
                    var des = desmove + @"\\" + file.Name;
                    File.Move(file.FullName, des);
                }
                status = true;
            }
            catch (Exception ex)
            {
                _logger.Info(string.Format("Error MoveFile Exception :{0}", ex.GetErrorMessage()));
            }
            return status;
        }

        public DateTime? GetDate(string data)
        {
            DateTime? date;
            try
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                date = DateTime.ParseExact(data, "dd/MM/yyyy HHmmss", culture);
            }
            catch (Exception ex)
            {
                date = null;
            }
            return date;
        }

        public DateTime GetDate(string[] data)
        {
            DateTime date;
            try
            {
                System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo("en-US");
                date = DateTime.ParseExact(data[3], "yyyyMMdd", culture);
            }
            catch (Exception ex)
            {
                date = DateTime.Now;
            }
            return date;
        }

        public int? GetInt(string dataint)
        {
            int? data;

            try
            {
                data = int.Parse(dataint);
            }
            catch (Exception ex)
            {
                data = null;
            }
            return data;
        }

        public List<FBB_LastMileByDistanceJob> ReadDatatoFile(FileInfo file)
        {
            DirectoryInfo di = new DirectoryInfo(file.FullName);
            var listdata = new List<FBB_LastMileByDistanceJob>();
            string totalrecord = string.Empty;

            try
            {
                using (StreamReader srsync = new StreamReader(file.FullName.Replace(".dat", ".sync")))
                {
                    string[] recode = srsync.ReadToEnd().Split('|');
                    totalrecord = recode[1];
                    srsync.Close();
                    //totalrecord = recode[1].Remove(recode[1].Count() - 1);
                    //srsync.Close();
                }

                if (File.Exists(Path.Combine(file.FullName)))
                {
                    using (StreamReader sr = new StreamReader(file.FullName))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] linedata = line.Split('|');

                            var data = new FBB_LastMileByDistanceJob();

                            if (linedata[0] == "02")
                            {
                                data.FILE_NAME = file.Name;
                                data.ACC_NBR = linedata[1];
                                data.USER_NAME = linedata[2];
                                data.SBC_CPY = linedata[3];
                                data.PRODUCT_NAME = linedata[4];
                                data.ON_TOP1 = linedata[5];
                                data.ON_TOP2 = linedata[6];
                                data.VOIP_NUMBER = linedata[7];
                                data.SERVICE_PACK_NAME = linedata[8];
                                data.ORD_NO = linedata[9];
                                data.ORD_TYPE = linedata[10];
                                data.ORDER_SFF = linedata[11];
                                data.APPOINTMENT_DATE = GetDate(linedata[12]);
                                data.SFF_ACTIVE_DATE = GetDate(linedata[13]);
                                data.APPROVE_JOB_FBSS_DATE = GetDate(linedata[14]);
                                data.COMPLETED_DATE = GetDate(linedata[15]);
                                data.ORDER_STATUS = linedata[16];
                                data.REJECT_REASON = linedata[17];
                                data.MATERIAL_CODE_CPESN = linedata[18];
                                data.CPE_SN = linedata[19];
                                data.CPE_MODE = linedata[20];
                                data.MATERIAL_CODE_STBSN = linedata[21];
                                data.STB_SN = linedata[22];
                                data.MATERIAL_CODE_ATASN = linedata[23];
                                data.ATA_SN = linedata[24];
                                data.MATERIAL_CODE_WIFIROUTESN = linedata[25];
                                data.WIFI_ROUTER_SN = linedata[26];
                                data.STO_LOCATION = linedata[27];
                                data.VENDOR_CODE = linedata[28];
                                data.FOA_REJECT_REASON = linedata[29];
                                data.RE_APPOINTMENT_REASON = linedata[30];
                                data.PHASE_PO = linedata[31];
                                data.SFF_SUBMITTED_DATE = GetDate(linedata[32]);
                                data.EVENT_CODE = linedata[33];
                                data.REGION = linedata[34];
                                data.TOTAL_FEE = GetInt(linedata[35]);
                                data.FEE_CODE = linedata[36];
                                data.ADDR_ID = linedata[37];
                                data.ADDR_NAME_TH = linedata[38];
                                data.TRANSFER_DATE = GetDate(linedata[39]);
                                data.TOTAL_ROW = int.Parse(totalrecord);
                                data.USER_CODE = linedata[41];
                                listdata.Add(data);
                            }
                        }
                        sr.Close();
                    }
                }
                else
                {
                    _logger.Info("Not Exists file :" + file.FullName);
                }
            }
            catch (Exception ex)
            {
                string errormessage = (ex.InnerException != null) ? ex.InnerException.Message : ex.Message;
                _logger.Info("Can't to Readfile :" + file.FullName + " Error :" + errormessage);
            }
            return listdata;
        }
    }
}
