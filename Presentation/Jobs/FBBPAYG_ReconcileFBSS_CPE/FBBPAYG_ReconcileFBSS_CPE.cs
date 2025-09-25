using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYG_ReconcileFBSS_CPE
{
    public class FBBPAYG_ReconcileFBSS_CPE
    {

        private string errorMsg = string.Empty;
        public ILogger _logger;
        private readonly ICommandHandler<FBBPAYGReconcileFBSSCommand> _reconcileShareplex;
        private readonly ICommandHandler<FBBPAYGReconcileFBSSHVRCommand> _reconcileHVR;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLog;
        public FBBPAYG_ReconcileFBSS_CPE(
            ILogger logger,
            ICommandHandler<FBBPAYGReconcileFBSSCommand> reconcileShareplex,
            ICommandHandler<FBBPAYGReconcileFBSSHVRCommand> reconcileHVR,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<InterfaceLogPayGCommand> @interface)
        {
            _logger = logger;
            _reconcileShareplex = reconcileShareplex;
            _reconcileHVR = reconcileHVR;
            _sendSmsCommand = SendSmsCommand;
            _queryProcessor = queryProcessor;
            _intfLog = @interface;
        }
        protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
        protected string _userTemp;
        protected string _pwdTemp;

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
            StartWatching();

            var process_hvr = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
            



            try
            {

                if (process_hvr.DISPLAY_VAL == "Y")
                {
                    var connect_nas = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_RECONCILEFBSS_CPE", "CONFIG_FILE_PATH").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                    var date_delete = Get_FBSS_CONFIG_TBL_LOV("FBBPAYG_RECONCILEFBSS_CPE", "DATE_DELETE").Where(record => record.ACTIVEFLAG == "Y")
                                        .ToList();
                    var file_name = connect_nas.DISPLAY_VAL;
                    var nas_path = connect_nas.VAL1;
                    var usernamenas = connect_nas.VAL2;
                    var passwordnas = connect_nas.VAL3;
                    var domain_path_nas = connect_nas.VAL4;
                    _logger.Info("EncryptionUtility");
                    _userTemp = EncryptionUtility.Decrypt(usernamenas, _Key);
                    _pwdTemp = EncryptionUtility.Decrypt(passwordnas, _Key);

                    CredentialHelper crd = new CredentialHelper(_logger, _intfLog);
                    var deletefile = crd.RemoveFile(_userTemp, _pwdTemp, domain_path_nas, nas_path, date_delete, connect_nas.DISPLAY_VAL);

                    _logger.Info("HVR Command");
                    var command = new FBBPAYGReconcileFBSSHVRCommand{
                        p_report_name = "FBBPAYG_RECONCILEFBSS_CPE"
                    };
                    _reconcileHVR.Handle(command);

                    var writefile = crd.WriteFile(_userTemp, _pwdTemp, domain_path_nas, nas_path, command.RET_CUR_FILE);

                }
                else
                {
                    var command = new FBBPAYGReconcileFBSSCommand
                    {
                        p_report_name = "FBBPAYG_RECONCILEFBSS_CPE"
                    };
                    _reconcileShareplex.Handle(command);
                    _logger.Error("ret_code : " + command.RET_CODE);
                    _logger.Error("ret_msg : " + command.RET_MSG);
                }
                    

                
            }
            catch (Exception ex)
            {
                _logger.Error("ExecuteJob : " + ex.Message);
                //SendSms();
            }
            StopWatching("close.");
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

        //public List<LovValueModel> Get_FBB_CFG_LOV_SHAREPLEX(string LOV_TYPE, string LOV_NAME)
        //{
        //    try
        //    {
        //        var query = new GetLovShareplexQuery()
        //        {
        //            LovType = LOV_TYPE,
        //            LovName = LOV_NAME
        //        };
        //        var result = _queryProcessor.Execute(query);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info("Get_FBB_CFG_LOV_SHAREPLEX : "+ex.GetErrorMessage());
        //        return new List<LovValueModel>();
        //    }
        //}

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
                            command.FullUrl = "FBBPAYG_ReconcileFBSS_CPE";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBPAYG_ReconcileFBSS_CPE Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }
    }
}
