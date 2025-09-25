using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WBBBusinessLayer;
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

namespace FBBPAYG_LMD_GENFILE_SUBPAYMENT
{

    public class Fbbpayglmdgenfilesubpayment
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryProcessor _queryProcessor;
        public Fbbpayglmdgenfilesubpayment(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _sendSmsCommand = SendSmsCommand;
            _queryProcessor = queryProcessor;
        }
        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        protected string _userTarget;
        protected string _pwdTarget;
        protected string _dominTarget;
        protected string _pathTarget;

        public void log(string msg)
        {
            _logger.Info(msg);
        }
        private void StartWatching()
        {
            _timer = Stopwatch.StartNew();
        }

        private void StopWatching(string mode)
        {
            _timer.Stop();
            _logger.Info(string.Format("{0} take : {1}", mode, _timer.Elapsed));
        }

        private class OutputFileConfig
        {
            public string output_file_name { get; set; }
            public string output_file_name_dat_format { get; set; }
            public string output_file_name_sync_format { get; set; }
            public string output_file_name_log_format { get; set; }
            public string flag_filename { get; set; }
        }

        private class ArchiveSettings
        {
            public int date_archive { get; set; }
            public string flag_archive { get; set; }
            public int date_archive_div { get; set; }
            public string flag_archive_div { get; set; }
        }

        public Boolean ExecuteJob()
        {
            StartWatching();
            try
            {
                //2.Query Connect Nas
                var NAS_LMD_SUBPAYMENT = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "OUT_PATHNAME").FirstOrDefault();
                //if (lovReturned != null)
                //{
                _userTarget = EncryptionUtility.Decrypt(NAS_LMD_SUBPAYMENT.VAL2, _Key);
                _pwdTarget = EncryptionUtility.Decrypt(NAS_LMD_SUBPAYMENT.VAL3, _Key);
                _dominTarget = NAS_LMD_SUBPAYMENT.VAL4;
                _pathTarget = NAS_LMD_SUBPAYMENT.VAL1;


                var outputFileConfig = GetOutputFileConfig();
                var archiveSettings = GetArchiveSettings();
                CredentialHelper crd = new CredentialHelper(_logger);
                
                #region Remove File from Archive Date Process
                var isDeleteFileSucess = crd.RemoveFile(
                                                _userTarget,
                                                _pwdTarget,
                                                _dominTarget,
                                                _pathTarget,
                                                archiveSettings.date_archive,
                                                archiveSettings.date_archive_div,
                                                outputFileConfig.output_file_name,
                                                outputFileConfig.output_file_name_dat_format,
                                                outputFileConfig.output_file_name_sync_format,
                                                outputFileConfig.output_file_name_log_format
                                                );

                #endregion Remove File from Archive Date Process

                #region Fetch Process
                _logger.Info($"== FETCH PROCESS START ==");
                var queryFetch = new LMDGenfileSubpaymentQuery
                {
                    flag_check = "FETCH",
                };
                var resultFetch = _queryProcessor.Execute(queryFetch);

                var datDataList = resultFetch.out_cursor
                    .Where(item => item.FILE_NAME.IndexOf(".dat", StringComparison.OrdinalIgnoreCase) >= 0)
                    .GroupBy(item => item.FILE_NAME.Substring(0, item.FILE_NAME.LastIndexOf('_')))
                    .ToDictionary(
                        group => group.Key,
                        group => string.Concat(
                            group
                            .OrderBy(item => int.Parse(item.FILE_NAME.Split('_').Last().Replace(".dat", "")))
                            .Select(item => item.FILE_DATA)
                        )
                    );

                var updatedOutCursor = new List<LMDGenfileListCursor>();
                foreach (var group in datDataList)
                {
                    var newItem = new LMDGenfileListCursor
                    {
                        FILE_NAME = group.Key + ".dat",
                        FILE_DATA = group.Value
                    };

                    updatedOutCursor.Add(newItem);
                }

                var logAndSyncFiles = resultFetch.out_cursor
                    .Where(item => item.FILE_NAME.EndsWith(".log", StringComparison.OrdinalIgnoreCase) ||
                                  item.FILE_NAME.EndsWith(".sync", StringComparison.OrdinalIgnoreCase));

                updatedOutCursor.AddRange(logAndSyncFiles);
                resultFetch.out_cursor = updatedOutCursor;


                if (resultFetch != null & resultFetch.ret_code != null)
                {
                    _logger.Info($"ret_code: {resultFetch.ret_code}");
                }
                if (resultFetch != null & resultFetch.ret_msg != null)
                {
                    _logger.Info($"ret_msg: {resultFetch.ret_msg}");
                }
                _logger.Info($"== FETCH PROCESS END ==");
                #endregion Fetch Process


                string pathNewDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Temp/");
                if (!Directory.Exists(pathNewDirectory))
                {
                    Directory.CreateDirectory(pathNewDirectory);
                }

                
                if (resultFetch.out_cursor != null)
                {
                    #region Write File Process
                    _logger.Info($"== WRITE FILE TO TEMP START ==");
                    _logger.Info($"Path file written to : {pathNewDirectory}");
                    foreach (var item in resultFetch.out_cursor)
                    {

                        if (!string.IsNullOrEmpty(item.FILE_NAME) && !string.IsNullOrEmpty(item.FILE_DATA))
                        {
                            string filePath = Path.Combine(pathNewDirectory, item.FILE_NAME); // Full file path
                            File.WriteAllText(filePath, item.FILE_DATA);

                            _logger.Info($"File written: {item.FILE_NAME}");
                        }
                        else
                        {
                            _logger.Info($"invalid FILE_NAME or FILE_DATA. inside out_cursor");
                        }
                    }
                    _logger.Info($"== WRITE FILE TO TEMP END ==");
                    #endregion Write File Process


                    #region Place File to Target NAS Process
                    _logger.Info($"== PLACE FILE TO TARGET NAS START ==");
                    var TargetNas = crd.placeFileTargetNas(
                                _userTarget,
                                _pwdTarget,
                                _dominTarget,
                                _pathTarget
                            );
                    _logger.Info($"== PLACE FILE TO TARGET NAS END ==");
                    #endregion Place File to Target NAS Process

                }
                else
                {
                    _logger.Info($"out_cursor is null");
                }


                #region Update Process
                var queryUpdate = new LMDGenfileSubpaymentQuery
                {
                    flag_check = "UPDATE",
                };
                var resultUpdate = _queryProcessor.Execute(queryUpdate);
                if (resultUpdate != null & resultUpdate.ret_code != null)
                {
                    _logger.Info($"ret_code: {resultUpdate.ret_code}");
                }
                if (resultUpdate != null & resultUpdate.ret_msg != null)
                {
                    _logger.Info($"ret_msg: {resultUpdate.ret_msg}");
                }
                #endregion Update Process

                #region Delete In Temp
                _logger.Info($"== DELETE FILE TEMP START ==");
                string[] files = Directory.GetFiles(pathNewDirectory);

                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                        _logger.Info($"File deleted [Temp] : {Path.GetFileName(file)}");
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Failed to delete file [Temp] : {Path.GetFileName(file)}. Error: {ex.Message}");
                    }
                }
                _logger.Info($"== DELETE FILE TEMP END ==");
                #endregion Delete In Temp



                //    //3.Query Connect Nas
                //    bool copy = crd.ConectionNasTarget(
                //                _userTarget,
                //                _pwdTarget,
                //                _dominTarget,
                //                _pathTarget
                //            );

                //    if (copy)
                //    {
                //        _logger.Info("Conection Nas Pass");

                //        // 4. delete old file in Nas
                //        DeleteOldFile(crd);
                //    }
                //    else
                //    {
                //        _logger.Error("Conection Nas Fail");
                //    }
                //    //2.Query Connect Nas
                //}
                //else
                //{
                //    _logger.Error("not have some Config LMD_SUBPAYMENT Fail");
                //}





            }
            catch (Exception ex)
            {
                _logger.Error("ExecuteJob : " + ex.Message);
                SendSms();
                StopWatching("close.");
                return false;
            }
            StopWatching("close.");
            return true;
        }

        private OutputFileConfig GetOutputFileConfig()
        {
            var outputFileConfig = new OutputFileConfig();
            try
            {
                 var out_filename_data = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "OUT_FILENAME").FirstOrDefault();
                 outputFileConfig.output_file_name = out_filename_data.VAL1;
                 outputFileConfig.output_file_name_dat_format = out_filename_data.VAL2;
                 outputFileConfig.output_file_name_sync_format = out_filename_data.VAL3;
                 outputFileConfig.output_file_name_log_format = out_filename_data.VAL4;
                 outputFileConfig.flag_filename = out_filename_data.DISPLAY_VAL;

                if (outputFileConfig.flag_filename != "Y")
                {
                    outputFileConfig.output_file_name = "FBBPAYG_LMD_SUBPAYMENT";
                    outputFileConfig.output_file_name_dat_format = ".dat";
                    outputFileConfig.output_file_name_sync_format = ".sync";
                    outputFileConfig.output_file_name_log_format = ".log";
                }
            }
            catch (Exception)
            {
                outputFileConfig.output_file_name = "FBBPAYG_LMD_SUBPAYMENT";
                outputFileConfig.output_file_name_dat_format = ".dat";
                outputFileConfig.output_file_name_sync_format = ".sync";
                outputFileConfig.output_file_name_log_format = ".log";
            }
            return outputFileConfig;
        }

        private ArchiveSettings GetArchiveSettings()
        {
            var archiveSettings = new ArchiveSettings();
            try
            {
                var date_archive_query = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "DATE_ARCHIVE").FirstOrDefault();
                archiveSettings.date_archive = int.Parse(date_archive_query.VAL1);
                archiveSettings.flag_archive = date_archive_query.DISPLAY_VAL;
                if (archiveSettings.flag_archive != "Y")
                {
                    archiveSettings.date_archive = 7;
                }
            }
            catch (Exception)
            {
                archiveSettings.date_archive = 7;
            }

            try
            {
                var date_archive_div_query = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "DATE_ARCHIVE_DIV").FirstOrDefault();
                archiveSettings.date_archive_div = int.Parse(date_archive_div_query.VAL1);
                archiveSettings.flag_archive_div = date_archive_div_query.DISPLAY_VAL;

                if (archiveSettings.flag_archive_div != "Y")
                {
                    archiveSettings.date_archive_div = 0;
                }
            }
            catch (Exception)
            {
                archiveSettings.date_archive_div = 0;
            }

            return archiveSettings;
        }


        private void DeleteOldFile()
        {
            //int v_date_archive = 0;
            //string v_flag_archive = string.Empty;
            //int v_date_archive_div = 0;
            //string v_flag_archive_div = string.Empty;
            //string v_file_name_arc = string.Empty;
            //DateTime dateNow = DateTime.Now;
            //string v_output_file_name = string.Empty;
            //string v_output_file_name_dat_format = string.Empty;
            //string v_output_file_name_sync_format = string.Empty;
            //string v_output_file_name_log_format = string.Empty;
            //string v_flag_filename = string.Empty;

            //6. QUERY FILE NAME ON TABLE FBSS_CONFIG_TBL
            //try
            //{
            //    var out_filename_data = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "OUT_FILENAME").FirstOrDefault();
            //    v_output_file_name = out_filename_data.VAL1;
            //    v_output_file_name_dat_format = out_filename_data.VAL2;
            //    v_output_file_name_sync_format = out_filename_data.VAL3;
            //    v_output_file_name_log_format = out_filename_data.VAL4;
            //    v_flag_filename = out_filename_data.DISPLAY_VAL;
            //}
            //catch (Exception)
            //{
            //    v_output_file_name = "FBBPAYG_LMD_SUBPAYMENT";
            //    v_output_file_name_dat_format = ".dat";
            //    v_output_file_name_sync_format = ".sync";
            //    v_output_file_name_log_format = ".log";
            //}
            ////7. CHECK FLAG FILENAME
            //if (v_flag_filename != "Y")
            //{
            //    v_output_file_name = "FBBPAYG_LMD_SUBPAYMENT";
            //    v_output_file_name_dat_format = ".dat";
            //    v_output_file_name_sync_format = ".sync";
            //    v_output_file_name_log_format = ".log";
            //}
            //8. DELETE OLD FILE
            //8.1 SELECT DATE_ARCHIVE ON TABLE FBSS_CONFIG_TBL
            //try
            //{
            //    var date_archive = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "DATE_ARCHIVE").FirstOrDefault();
            //    v_date_archive = int.Parse(date_archive.VAL1);
            //    v_flag_archive = date_archive.DISPLAY_VAL;
            //}
            //catch (Exception)
            //{
            //    v_date_archive = 7;
            //}
            ////8.2 CHECK FLAG ARCHIVE
            //if (v_flag_archive != "Y")
            //{
            //    v_date_archive = 7;
            //}
            ////8.3 SELECT DATE_ARCHIVE ON TABLE FBSS_CONFIG_TBL
            //try
            //{
            //    var date_archive_div = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_LMD_SUBPAYMENT", "DATE_ARCHIVE_DIV").FirstOrDefault();
            //    v_date_archive_div = int.Parse(date_archive_div.VAL1);
            //    v_flag_archive_div = date_archive_div.DISPLAY_VAL;
            //}
            //catch (Exception)
            //{
            //    v_date_archive_div = 0;
            //}
            ////8.4 CHECK FLAG ARCHIVE_DIV
            //if (v_flag_archive_div != "Y")
            //{
            //    v_date_archive_div = 0;
            //}
            ////8.5 LOOP DELETE BY RECORD
            //var isDeleteFileSucess = crd.RemoveFile(
            //                                    _userTarget,
            //                                    _pwdTarget,
            //                                    _dominTarget,
            //                                    _pathTarget,
            //                                    v_date_archive,
            //                                    v_date_archive_div,
            //                                    v_output_file_name,
            //                                    v_output_file_name_dat_format,
            //                                    v_output_file_name_sync_format,
            //                                    v_output_file_name_log_format
            //                                    );
        }

        private void SendSms()
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
                            command.FullUrl = "FBBPAYG_LMD_GENFILE_SUBPAYMENT";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBPAYG_LMD_GENFILE_SUBPAYMENT Error";
                            _sendSmsCommand.Handle(command);
                        }

                    }

                }
            }
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
                //SendSms("FBBReturnedFixedAsset Error");
                _logger.Error("Error GetLovList : " + ex.GetErrorMessage());
                return new List<LovValueModel>();
            }
        }
    }
}
