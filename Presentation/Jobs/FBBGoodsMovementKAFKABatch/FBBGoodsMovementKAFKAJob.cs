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
//using System.Security.Principal;
using System.Runtime.InteropServices;
using System.IO;
using WBBContract.Queries.WebServices;

namespace FBBGoodsMovementKAFKA
{
    public class FBBGoodsMovementKAFKAJob
    {

        private string errorMsg = string.Empty;
        public ILogger _logger;
        private Stopwatch _timer;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryProcessor _queryProcessor;

        public FBBGoodsMovementKAFKAJob(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _sendSmsCommand = SendSmsCommand;
            _queryProcessor = queryProcessor;
        }
        //protected string _Key = ConfigurationManager.AppSettings["KEY"].ToSafeString();
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
                this.GoodsMovementKAFKAJob();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                //SendSms();
            }


            StopWatching("close.");
        }

        private void GoodsMovementKAFKAJob()
        {
            _logger.Info("GoodsMovementKAFKAJob");

            var program_process = Get_FBSS_CONFIG_TBL_LOV("FBBGoodsMovementKAFKA", "PROGRAM_PROCESS").FirstOrDefault();
            string msg = "";
            _logger.Info("GoodsMovementKAFKAJob PROGRAM_PROCESS : " + program_process.DISPLAY_VAL);
            if (program_process.DISPLAY_VAL == "Y")
            {
                var query = new GoodsMovementKAFKAQuery()
                {
                    action = "CONSUMER"
                };
                var queryResponse = _queryProcessor.Execute(query);

                if (queryResponse.ret_code == null || queryResponse.ret_code == string.Empty)
                {
                    queryResponse.ret_msg = "No Data To Consume";
                }
                msg = queryResponse.ret_code == "Success" ? "Consume Success " + queryResponse.return_transactions : "Error " + queryResponse.ret_msg;
                _logger.Info("Consumer Status : " + msg);
                _logger.Info("Pending Asset : " + queryResponse.ret_code_pending_asst +" || Msg : "+ queryResponse.ret_msg_pending_asst);

                if (queryResponse.item_json != null && queryResponse.item_json.Count > 0)
                {
                    _logger.Info("Start Produce Pending Asset");
                    var queryProduce = new GoodsMovementKAFKAQuery()
                    {
                        action = "PRODUCER",
                        item_json = queryResponse.item_json

                    };
                    var queryResponseProduce = _queryProcessor.Execute(queryProduce);
                    _logger.Info("End Produce Pending Asset");
                }
                else
                {
                    _logger.Info("No Data To Produce Pending Asset");
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
