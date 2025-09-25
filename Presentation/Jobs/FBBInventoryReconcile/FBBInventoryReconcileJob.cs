using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.FBBWebConfigCommands;
using WBBContract.Queries.Commons.Masters;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.Extensions;
using WBBEntity.PanelModels.FBBWebConfigModels;

using System.Configuration;
using System.Security.Principal;
using System.Runtime.InteropServices;
using System.IO;

namespace FBBInventoryReconcile
{
    public class FBBInventoryReconcileJob
    {

        private string errorMsg = string.Empty;
        public ILogger _logger;
        private readonly ICommandHandler<InventoryReconcileCommand> _reconcile;
        private readonly ICommandHandler<ReconcileCPECommand> _reconcileShareplex;
        private readonly ICommandHandler<ReconcileCPEHVRCommand> _reconcileHVR;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryProcessor _queryProcessor;
        private readonly ICommandHandler<InterfaceLogPayGCommand> _intfLog;

        public FBBInventoryReconcileJob(
            ILogger logger,
            ICommandHandler<InventoryReconcileCommand> reconcile,
            ICommandHandler<ReconcileCPECommand> reconcileShareplex,
            ICommandHandler<ReconcileCPEHVRCommand> reconcileHVR,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand,
            ICommandHandler<InterfaceLogPayGCommand> @interface)
        {
            _logger = logger;
            _reconcile = reconcile;
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
            try
            {
                this.ReconcileJob();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                SendSms();
            }


            StopWatching("close.");
        }

        private void ReconcileJob()
        {
            var ROLLBACK = Get_FBB_CFG_LOV("FBB_INVENTORYRECONCILE", "ROLLBACK");


            var process_hvr = Get_FBSS_CONFIG_TBL_LOV("FBBPayG_ConnectDB", "HVRDB").FirstOrDefault();
            _logger.Info("ROLLBACK : " + (ROLLBACK == null ? "N" : ROLLBACK));
            if (ROLLBACK == "Y")
            {
                _logger.Info("OLD");
                //Old
                _reconcile.Handle(new InventoryReconcileCommand());
            }
            else
            {
                try
                {
                    if (process_hvr.DISPLAY_VAL == "Y")
                    {
                        //HVR
                        _logger.Info("HVR");

                        var date_delete = Get_FBSS_CONFIG_TBL_LOV("FBB_INVENTORY_RECONCILE", "DATE_DELETE").Where(record => record.ACTIVEFLAG == "Y")
                                .ToList();
                        var nas_query = Get_FBSS_CONFIG_TBL_LOV("FBB_INVENTORY_RECONCILE", "CONFIG_FILE_PATH").Where(record => record.ACTIVEFLAG == "Y").FirstOrDefault();
                        var file_name = nas_query.DISPLAY_VAL;
                        var nas_path = nas_query.VAL1;
                        var usernamenas = nas_query.VAL2;
                        var passwordnas = nas_query.VAL3;
                        var domain_path_nas = nas_query.VAL4;
                        _logger.Info("EncryptionUtility");
                        _userTemp = EncryptionUtility.Decrypt(usernamenas, _Key);
                        _pwdTemp = EncryptionUtility.Decrypt(passwordnas, _Key);

                        CredentialHelper crd = new CredentialHelper(_logger, _intfLog);
                        var deletefile = crd.RemoveFile(_userTemp, _pwdTemp, domain_path_nas, nas_path, date_delete, nas_query.DISPLAY_VAL);


                        _logger.Info("HVR Command");
                        var command = new ReconcileCPEHVRCommand { };
                        _reconcileHVR.Handle(command);


                        var writefile = crd.WriteFile(_userTemp, _pwdTemp, domain_path_nas, nas_path, command.RET_CUR_FILE);

                    }
                    else
                    {
                        //new call shareplex R10.08
                        var command = new ReconcileCPECommand { };
                        _reconcileShareplex.Handle(command);
                    }
                }catch(Exception ex)
                {
                    _logger.Info(ex);
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

        public string Get_FBB_CFG_LOV(string LOV_TYPE, string LOV_NAME)
        {
            try
            {
                var query = new GetLovQuery()
                {
                    LovType = LOV_TYPE,
                    LovName = LOV_NAME
                };
                var _FbbCfgLov = _queryProcessor.Execute(query);

                string result = (from e in _FbbCfgLov select e.ActiveFlag).FirstOrDefault();

                return result;
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                return "";
            }
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
                            command.FullUrl = "FBBInventoryReconcile";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBInventoryReconcile Error";
                            _sendSmsCommand.Handle(command);
                            //Thread.Sleep(15000);
                        }

                    }

                }
            }


        }


    }
}
