using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WBBBusinessLayer;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace FBBPAYG_LMD_SUBPAYMENT
{

    public class Fbbpayglmdsubpayment
    {
        private Stopwatch _timer;
        public ILogger _logger;
        private readonly ICommandHandler<SendSmsCommand> _sendSmsCommand;
        private readonly IQueryProcessor _queryProcessor;
        public Fbbpayglmdsubpayment(
            ILogger logger,
            IQueryProcessor queryProcessor,
            ICommandHandler<SendSmsCommand> SendSmsCommand)
        {
            _logger = logger;
            _sendSmsCommand = SendSmsCommand;
            _queryProcessor = queryProcessor;
        }
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
        public Boolean ExecuteJob()
        {
            StartWatching();
            try
            {
                var query = new LMDsubpaymentQuery();
                var result = _queryProcessor.Execute(query);

                _logger.Error("ret_code : " + result.ret_code);
                _logger.Error("ret_msg : " + result.ret_msg);
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
                            command.FullUrl = "FBBPAYG_LMD_SUBPAYMENT";
                            command.Source_Addr = "FBBBATCH";
                            command.Destination_Addr = item;
                            command.Transaction_Id = item;
                            command.Message_Text = "FBBPAYG_LMD_SUBPAYMENT Error";
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
    }
}
